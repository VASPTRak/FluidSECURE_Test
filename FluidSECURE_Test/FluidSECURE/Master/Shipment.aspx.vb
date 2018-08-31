Imports log4net
Imports log4net.Config

Public Class Shipment
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Shipment))

	Dim OBJMaster As MasterBAL = New MasterBAL()
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "Support" Then 'And Session("RoleName") <> "CustomerAdmin"
				'Access denied
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then
					txtShipmentDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
					'txtShipmentTime.Text = DateTime.Now.ToString("hh:mm tt")

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

					If (Not Request.QueryString("ShipmentId") = Nothing And Not Request.QueryString("ShipmentId") = "") Then

						HDF_ShipmentId.Value = Request.QueryString("ShipmentId")

						BindShipmentDetails(Request.QueryString("ShipmentId"))
						btnFirst.Visible = True
						btnNext.Visible = True
						btnprevious.Visible = True
						btnLast.Visible = True
						lblHeader.Text = "Edit Shipment"
						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message.InnerText = "Record saved"
						End If
					Else
						btnFirst.Visible = False
						btnNext.Visible = False
						btnprevious.Visible = False
						btnLast.Visible = False
						lblof.Visible = False
						lblHeader.Text = "Add Shipment"
					End If
					txtFluidSecureUnitName.Focus()


				End If
			End If


			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Private Sub BindShipmentDetails(ShipmentId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtShipment As DataTable = New DataTable()
			dtShipment = OBJMaster.GetShipmentByShipmentID(ShipmentId)
			Dim cnt As Integer = 0
			If (dtShipment.Rows.Count > 0) Then

				txtFluidSecureUnitName.Text = dtShipment.Rows(0)("FluidSecureUnitName").ToString()
				DDL_Customer.SelectedValue = dtShipment.Rows(0)("CompanyId").ToString().TrimEnd().TrimStart()
				txtAddress.Text = dtShipment.Rows(0)("Address").ToString()
				txtShipmentDate.Text = Convert.ToDateTime(dtShipment.Rows(0)("ShipmentDate").ToString()).ToString("MM/dd/yyyy")
				'txtShipmentTime.Text = Convert.ToDateTime(dtShipment.Rows(0)("ShipmentDate").ToString()).ToString("hh:mm tt")

				OBJMaster = New MasterBAL()

				HDF_TotalShipments.Value = OBJMaster.GetShipmentIdByCondition(ShipmentId, False, False, False, False, True, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()))

				OBJMaster = New MasterBAL()
				Dim dtAllShipments As DataTable = New DataTable()
                'If Session("CompanyNameHeader") <> Nothing Then
                '	If Session("CompanyNameHeader") <> "" And Session("CompanyNameHeader") <> "Select All" Then
                '		dtAllShipments = OBJMaster.GetShipmentsByCondition(" and SD.Company = '" + Session("CompanyNameHeader").ToString() + "' ", Session("RoleId").ToString())
                '	Else
                '		dtAllShipments = OBJMaster.GetShipmentsByCondition("", Session("RoleId").ToString())
                '	End If
                'Else
                '	dtAllShipments = OBJMaster.GetShipmentsByCondition("", Session("RoleId").ToString())
                'End If

                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    dtAllShipments = OBJMaster.GetShipmentsByCondition(" and SD.CompanyId = " + Session("CustomerId").ToString() + " ", Session("RoleId").ToString())
                Else
                    dtAllShipments = OBJMaster.GetShipmentsByCondition("", Session("RoleId").ToString())
                End If

                dtAllShipments.PrimaryKey = New DataColumn() {dtAllShipments.Columns(0)}
				Dim dr As DataRow = dtAllShipments.Rows.Find(ShipmentId)
				If Not IsDBNull(dr) Then

					cnt = dtAllShipments.Rows.IndexOf(dr) + 1

				End If
				If (cnt >= HDF_TotalShipments.Value) Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				ElseIf (cnt <= 1) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt > 1 And cnt < HDF_TotalShipments.Value) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
				lblof.Text = cnt & " of " & HDF_TotalShipments.Value.ToString()

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					beforeData = CreateData(ShipmentId)
				End If

			Else
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Data Not found. Please try again after some time."
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindShipmentDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Shipment data, please try again later."
		Finally
			txtFluidSecureUnitName.Focus()
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		ValidateAndSaveShipment(False)
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/AllShipments")
	End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try
			Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value

			OBJMaster = New MasterBAL()
			Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, True, False, False, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()))
			HDF_ShipmentId.Value = ShipmentId
			BindShipmentDetails(ShipmentId)

		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try

			Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value

			OBJMaster = New MasterBAL()
			Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, False, False, False, True, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()))
			HDF_ShipmentId.Value = ShipmentId
			BindShipmentDetails(ShipmentId)

		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try

			Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value

			OBJMaster = New MasterBAL()
			Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, False, False, True, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()))
			HDF_ShipmentId.Value = ShipmentId
			BindShipmentDetails(ShipmentId)

		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try

			Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value

			OBJMaster = New MasterBAL()
			Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, False, True, False, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()))
			HDF_ShipmentId.Value = ShipmentId
			BindShipmentDetails(ShipmentId)

		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindCustomer(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()
			Dim CustomerId As Integer = 0
			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				CustomerId = Convert.ToInt32(Session("CustomerId"))
				'DDL_Customer.Visible = False
				'divCompany.Visible = False
			End If

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, CustomerId)

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()

			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Private Function CreateData(ShipmentId As Integer) As String
		Try

			Dim data As String = ""

			data = "ShipmentId = " & ShipmentId & " ; " &
					"FluidSecure Link Name = " & txtFluidSecureUnitName.Text.Replace(",", " ") & " ; " &
					"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
					"Address  = " & txtAddress.Text.Replace(",", " ") & " ; " &
					"Shipment Date  = " & txtShipmentDate.Text.Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
		ValidateAndSaveShipment(True)
	End Sub


	Private Sub ValidateAndSaveShipment(IsSaveAndAddNew As Boolean)

		Dim ShipmentId As Integer = 0

		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			If (Not HDF_ShipmentId.Value = Nothing And Not HDF_ShipmentId.Value = "") Then

				ShipmentId = HDF_ShipmentId.Value

			End If
			Dim CheckIdExists As Integer = 0
			OBJMaster = New MasterBAL()
			CheckIdExists = OBJMaster.CheckFluidSecureUnitExist(txtFluidSecureUnitName.Text, ShipmentId)

			If CheckIdExists = -1 Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "FluidSecure Link already exist."

				Return

			End If

			OBJMaster = New MasterBAL()

			Dim result As Integer = 0

			OBJMaster = New MasterBAL()

			Dim shipmentDatetime As DateTime = Request.Form(txtShipmentDate.UniqueID) '& " " & Request.Form(txtShipmentTime.UniqueID)

			result = OBJMaster.SaveUpdateShipment(ShipmentId, txtFluidSecureUnitName.Text, DDL_Customer.SelectedItem.Text.ToString().TrimStart().TrimEnd(), txtAddress.Text, Convert.ToInt32(Session("PersonId")), shipmentDatetime, DDL_Customer.SelectedValue)


			If result > 0 Then
				If (ShipmentId > 0) Then
					message.Visible = True
					message.InnerText = "Record saved"
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(ShipmentId)
						CSCommonHelper.WriteLog("Modified", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Shipment"))
					End If
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Shipment"))
					Else
						Response.Redirect(String.Format("~/Master/Shipment?ShipmentId={0}&RecordIs=New", result))
					End If
				End If

			Else
				If (ShipmentId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(ShipmentId)
						CSCommonHelper.WriteLog("Modified", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment update failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Shipment update failed, please try again"
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment Addition failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Shipment Addition failed, please try again"
				End If

			End If
			txtFluidSecureUnitName.Focus()

		Catch ex As Exception

			If (ShipmentId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(ShipmentId)
					CSCommonHelper.WriteLog("Modified", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment update failed. Exception is : " & ex.Message)
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(ShipmentId)
					CSCommonHelper.WriteLog("Added", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment Addition failed. Exception is : " & ex.Message)
				End If
			End If

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
			txtFluidSecureUnitName.Focus()

		Finally

			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

End Class
