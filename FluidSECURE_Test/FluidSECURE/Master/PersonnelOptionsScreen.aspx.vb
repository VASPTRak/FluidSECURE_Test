Imports log4net
Imports log4net.Config
Public Class PersonnelOptionsScreen
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(PersonnelOptionsScreen))
	Dim OBJMaster As MasterBAL = New MasterBAL()
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		XmlConfigurator.Configure()

		ErrorMessage.Visible = False
		message.Visible = False

		If CSCommonHelper.CheckSessionExpired() = False Then
			'unautorized access error log
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
		ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
			'Access denied
			Response.Redirect("/home")
		Else
			If Not IsPostBack Then

				lblHeader.Text = "Vehicle Options"
				BindCustomers(Session("PersonId").ToString(), Session("RoleId").ToString())
				DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
				BindTiming()

			End If
		End If
	End Sub

	Private Sub BindCustomers(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

			If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
				DDL_Customer.SelectedIndex = 1
				DDL_Customer.Enabled = False
				DDL_Customer.Visible = False
				divCompany.Visible = False
			End If
			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				If (Session("RoleName") = "GroupAdmin") Then
					DDL_Customer.SelectedValue = Session("CustomerId")
				Else
					DDL_Customer.SelectedIndex = 1
				End If
			End If

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting Companies , please try again later."

			' log.Error("Error occurred in BindCustomers Exception is :" + ex.Message)

		End Try
	End Sub
	Protected Sub btn_Assign_Click(sender As Object, e As EventArgs) Handles btn_assign.Click
		Try

			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenVehAssignModelBox();", True)
		Catch ex As Exception
			'log.Error("Error occurred in btn_DisableAllVehOdo_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnVehAssignOk_Click(sender As Object, e As EventArgs)
		Try
			OBJMaster.SavePersonnalVehicleMappingAgainstCustomer(DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId")))
			message.Visible = True
			message.InnerText = "Record Saved."
		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub


	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try


			BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))

		Catch ex As Exception

			Log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

		End Try
	End Sub

	Private Sub BindTiming()
		Try

			Dim dtTiming As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtTiming = OBJMaster.GetTiming()

			gv_FuelingTimes.DataSource = dtTiming
			gv_FuelingTimes.DataBind()
		Catch ex As Exception

			Log.Error("Error occurred in BindTiming Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

		End Try
	End Sub

	Private Sub BindSites(CustomerId As Integer)

		Try

			Dim dtSites As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtSites = OBJMaster.GetSiteByCondition("And c.CustomerId =" + CustomerId.ToString(), Session("PersonId").ToString(), Session("RoleId").ToString(), False)

			gv_Sites.DataSource = dtSites
			gv_Sites.DataBind()

			If CustomerId = 0 Then
				lblSiteMessage.Text = "Please select Company."
				lblSiteMessage.Visible = True
				gv_Sites.Visible = False
			ElseIf CustomerId <> 0 And dtSites.Rows.Count <> 0 Then

				lblSiteMessage.Visible = False
				gv_Sites.Visible = True
			ElseIf CustomerId <> 0 And dtSites.Rows.Count = 0 Then
				lblSiteMessage.Text = "Site not found for selected customer."
				lblSiteMessage.Visible = True
				gv_Sites.Visible = False
			End If

		Catch ex As Exception

			Log.Error("Error occurred in BindSites Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting sites, please try again later."

		End Try
	End Sub

	Protected Sub btnAssignTime_Click(sender As Object, e As EventArgs)
		Try
			Dim dtTimings As DataTable = New DataTable("dtPersonSiteTimings")
			dtTimings.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
			dtTimings.Columns.Add("TimeId", System.Type.[GetType]("System.String"))

			Dim dtPersonnel As DataTable = New DataTable()

			dtPersonnel = OBJMaster.GetPersonalDetails(" where C.CustomerId = " & DDL_Customer.SelectedValue)
			For i = 0 To dtPersonnel.Rows.Count - 1
				For Each item As GridViewRow In gv_FuelingTimes.Rows

					Dim CHK_FuelingTimes As CheckBox = TryCast(item.FindControl("CHK_FuelingTimes"), CheckBox)
					If (CHK_FuelingTimes.Checked = True) Then
						Dim dr As DataRow = dtTimings.NewRow()
						dr("PersonId") = dtPersonnel.Rows(i)("PersonId")
						dr("TimeId") = gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeId").ToString()
						dtTimings.Rows.Add(dr)
					End If
				Next
				OBJMaster.InsertPersonTimings(dtTimings, dtPersonnel.Rows(i)("PersonId"))
				dtTimings.Clear()
			Next
			message.Visible = True
			message.InnerText = "Record Saved."
		Catch ex As Exception
			log.Error("Error occurred in btnAssignTime_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub btnAssignHose_Click(sender As Object, e As EventArgs)
		Try
			Dim dtPersonSite As DataTable = New DataTable()
			dtPersonSite.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
			dtPersonSite.Columns.Add("SiteID", System.Type.[GetType]("System.Int32"))
			dtPersonSite.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
			dtPersonSite.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))

			Dim dtPersonnel As DataTable = New DataTable()

			dtPersonnel = OBJMaster.GetPersonalDetails(" where C.CustomerId = " & DDL_Customer.SelectedValue)
			For i = 0 To dtPersonnel.Rows.Count - 1
				For Each item As GridViewRow In gv_Sites.Rows

					Dim CHK_PersonSite As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
					If (CHK_PersonSite.Checked = True) Then
						Dim dr As DataRow = dtPersonSite.NewRow()
						dr("PersonId") = dtPersonnel.Rows(i)("PersonId")
						dr("SiteID") = gv_Sites.DataKeys(item.RowIndex).Values("SiteID").ToString()
						dr("CreatedDate") = DateTime.Now
						dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
						dtPersonSite.Rows.Add(dr)
					End If
				Next

				OBJMaster.InsertPersonSiteMapping(dtPersonSite, dtPersonnel.Rows(i)("PersonId"))
				dtPersonSite.Clear()
			Next
			message.Visible = True
			message.InnerText = "Record Saved."
		Catch ex As Exception
			log.Error("Error occurred in btnAssignHose_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub
End Class