Imports log4net
Imports log4net.Config

Public Class TankBalance
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TankBalance))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If (Not IsPostBack) Then
					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Private Sub BindCustomer(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support" And Not Session("RoleName") = "GroupAdmin") Then
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
			DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)
		Try
			OBJMaster = New MasterBAL()
			Dim dtTankInv As DataTable = New DataTable()

			Dim strConditions As String = ""
			Dim CustomerId As String = ""
			If (DDL_Customer.SelectedValue <> "0") Then
				CustomerId = DDL_Customer.SelectedValue.ToString()
			End If

			If (ddl_TankNo.SelectedIndex <> 0) Then
				strConditions = IIf(strConditions = "", " and TankNumber = '" + ddl_TankNo.SelectedValue & "'", strConditions + " and TankNumber = '" + ddl_TankNo.SelectedValue & "'")
			End If

			'get data from server
			dtTankInv = OBJMaster.GetTankBalanceDetails(strConditions, CustomerId)
			If (Not dtTankInv Is Nothing) Then

				If (dtTankInv.Rows.Count <= 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData = CreateData()
						CSCommonHelper.WriteLog("Report Genereated", "Tank Balance", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
					End If
					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
					Return
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData = CreateData()
					CSCommonHelper.WriteLog("Report Genereated", "Tank Balance", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
				End If
				ErrorMessage.InnerText = "Data not found against selected criteria."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return

			End If
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData = CreateData()
				CSCommonHelper.WriteLog("Report Genereated", "Tank Balance", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
			End If
            Session("TankBalanceDetails") = dtTankInv
            Session("ASOfDate") = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")
            Response.Redirect("~/Reports/TankBalanceReport")


		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData = CreateData()
				CSCommonHelper.WriteLog("Report Genereated", "Tank Balance", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
			End If
			ErrorMessage.InnerText = "Data not found against selected criteria."
			ErrorMessage.Visible = True
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
			Return
		Finally
			DDL_Customer.Focus()
		End Try
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
		Try
			Response.Redirect("/home")
		Catch ex As Exception

		End Try
	End Sub

	Private Function CreateData() As String
		Try

			Dim data As String = "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
								  "Tank Number = " & If(ddl_TankNo.SelectedItem.ToString() = "", "All Tanks", ddl_TankNo.SelectedValue.Replace(",", " ")) & " "
			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try
			BindTanks(DDL_Customer.SelectedValue)
		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Private Sub BindTanks(CustomerId As Integer)
		Try
			OBJMaster = New MasterBAL()
			Dim dtTanks As DataTable = New DataTable()
			dtTanks = OBJMaster.GetTankbyConditions(" And T.CustomerId =" & CustomerId, Session("PersonId").ToString(), Session("RoleId").ToString())

			ViewState("dtTanks") = dtTanks
			ddl_TankNo.DataSource = dtTanks
			ddl_TankNo.DataTextField = "TankNumberNameForView"
			ddl_TankNo.DataValueField = "TankNumber"
			ddl_TankNo.DataBind()
			ddl_TankNo.Items.Insert(0, New ListItem("Select Tank Number", "0"))
		Catch ex As Exception
			log.Error("Error occurred in BindTanks Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub
End Class
