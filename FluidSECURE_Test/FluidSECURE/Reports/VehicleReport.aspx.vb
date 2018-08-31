Imports log4net
Imports log4net.Config

Public Class VehicleReport
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
			ElseIf Session("RoleName") = "User" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If Not IsPostBack Then

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

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
            DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support") Then
                DDL_Customer.SelectedIndex = 1
                DDL_Customer.Enabled = False
                DDL_Customer.Visible = False
                divCompany.Visible = False
            End If


            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                DDL_Customer.SelectedIndex = 1

            End If

        Catch ex As Exception

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindAllVehicles(CustomerId As Integer)
        Try
            Dim dtVehicle As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtVehicle = OBJMaster.GetVehicleByCondition(" and  c.CustomerId  = " & CustomerId, Session("PersonId").ToString(), Session("RoleId").ToString())


            DDL_Vehicle.DataSource = dtVehicle
            DDL_Vehicle.DataValueField = "VehicleId"
            DDL_Vehicle.DataTextField = "VehicleNumber"
            DDL_Vehicle.DataBind()

            DDL_Vehicle.Items.Insert(0, New ListItem("Select All Vehicle", "0"))

            DDL_VehicleType.Items.Clear()
            If dtVehicle.Rows.Count > 0 Then
                Dim dtview As DataView = New DataView(dtVehicle.DefaultView.ToTable(True, "Type"))
                dtview.RowFilter = "Type <> ''"
                DDL_VehicleType.DataSource = dtview.ToTable()
                DDL_VehicleType.DataTextField = "Type"
                DDL_VehicleType.DataValueField = "Type"
                DDL_VehicleType.DataBind()
            End If

            DDL_VehicleType.Items.Insert(0, New ListItem("Select All Vehicle Type", "0"))
            DDL_VehicleType.SelectedIndex = 0

        Catch ex As Exception

            log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

        End Try
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try
			BindAllVehicles(Convert.ToInt32(DDL_Customer.SelectedValue))

		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

    Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

        Try


            OBJMaster = New MasterBAL()
            Dim dtVehicle As DataTable = New DataTable()

            Dim strConditions As String = ""
            If (DDL_Customer.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and V.CustomerId = " + DDL_Customer.SelectedValue, strConditions + " and V.CustomerId = " + DDL_Customer.SelectedValue)
            End If


            If (DDL_Vehicle.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and V.VehicleID = " + DDL_Vehicle.SelectedValue, strConditions + " and V.VehicleID = " + DDL_Vehicle.SelectedValue)
            End If

            If (DDL_VehicleType.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and V.Type = '" + DDL_VehicleType.SelectedValue + "' ", strConditions + " and V.Type = '" + DDL_VehicleType.SelectedValue + "' ")
            End If

            strConditions += " order by V.VehicleName"
            'get data from server
            dtVehicle = OBJMaster.GetVehicleDetails(strConditions)
            If (Not dtVehicle Is Nothing) Then

                If (dtVehicle.Rows.Count <= 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData()

                        CSCommonHelper.WriteLog("Report Genereated", "Vehicles Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData()

                    CSCommonHelper.WriteLog("Report Genereated", "Vehicles Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                Return

            End If
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Vehicles Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
            End If
            Session("VehicleDetails") = dtVehicle

            Response.Redirect("~/Reports/VehicleReportRPT")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Vehicles Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
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
                                 "Vehicle Number = " & DDL_Vehicle.SelectedItem.Text.Replace(",", " ") & " ; " &
                                 "Vehicle Type = " & DDL_VehicleType.SelectedItem.Text.Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class