Imports log4net
Imports log4net.Config

Public Class ShipmentDetailReport
	Inherits System.Web.UI.Page


	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(VehicleReport))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				Response.Redirect("/Account/Login")
			ElseIf Session("RoleName") <> "SuperAdmin" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If Not IsPostBack Then

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

				Else

				End If

				DDL_Customer.Focus()

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

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support") Then
				DDL_Customer.SelectedIndex = 1
				DDL_Customer.Enabled = False
				DDL_Customer.Visible = False
				divCompany.Visible = False
			End If


            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                DDL_Customer.SelectedIndex = 1
            Else
                DDL_Customer.Items.Insert(0, New ListItem("All Companies", "0"))
            End If

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub
	Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

		Try


			OBJMaster = New MasterBAL()
			Dim dtShipment As DataTable = New DataTable()

			Dim strConditions As String = ""
			If (DDL_Customer.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and SD.CompanyId = " + DDL_Customer.SelectedValue, strConditions + " and SD.CompanyId = " + DDL_Customer.SelectedValue)
			End If

			strConditions = IIf(strConditions = "", " and SD.ShipmentForLinkOrHub= " + DDL_ShipmentFor.SelectedValue, strConditions + " and SD.ShipmentForLinkOrHub= " + DDL_ShipmentFor.SelectedValue)

			If (DDL_returned.SelectedValue = "2") Then
				strConditions = IIf(strConditions = "", " and SD.IsReturned=1", strConditions + " and SD.IsReturned=1")
			End If

			strConditions += " "

            'get data from server
            dtShipment = OBJMaster.GetShipmentDetails(strConditions, DDL_ShipmentFor.SelectedValue)
            If (Not dtShipment Is Nothing) Then

				If (dtShipment.Rows.Count <= 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData()

						CSCommonHelper.WriteLog("Report Genereated", "Shipment Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
					End If
					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					Return
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData()

					CSCommonHelper.WriteLog("Report Genereated", "Shipment Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
				End If
				ErrorMessage.InnerText = "Data not found against selected criteria."
				ErrorMessage.Visible = True
				Return

			End If
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData()

				CSCommonHelper.WriteLog("Report Genereated", "Shipment Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
			End If

			Session("ShipmentDetails") = dtShipment
			Session("ShipmentForLinkOrHub") = DDL_ShipmentFor.SelectedValue
			Session("Company") = DDL_Customer.SelectedItem.Text

			Response.Redirect("~/Reports/ShipmentRPT")


		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData()

				CSCommonHelper.WriteLog("Report Genereated", "Shipment Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
			End If
			ErrorMessage.InnerText = "Data not found against selected criteria."
			ErrorMessage.Visible = True
			Return
		Finally
			DDL_Customer.Focus()
		End Try

	End Sub

	Private Function CreateData() As String
		Try

			Dim data As String = "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
								 "Shipment details for = " & DDL_ShipmentFor.SelectedItem.Text.Replace(",", " ") & " ; " &
								 "Returned =" & DDL_returned.SelectedItem.Text.Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class