Imports log4net
Imports log4net.Config

Public Class FluidSecureHubReport
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FluidSecureHubReport))

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

        Catch ex As Exception

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindAllPersonnels(CustomerId As Integer)
        Try
            Dim dtPersonnel As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(IsFluidSecureHub,0)=1 and cust.CustomerId = " & CustomerId, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

            DDL_Personnel.DataSource = dtPersonnel
            DDL_Personnel.DataValueField = "PersonId"
            DDL_Personnel.DataTextField = "HubSiteName"
            DDL_Personnel.DataBind()

            DDL_Personnel.Items.Insert(0, New ListItem("Select All Site", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting FluidSecure Hub, please try again later."

        End Try
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try

			BindAllPersonnels(Convert.ToInt32(DDL_Customer.SelectedValue))

		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

        Try


            OBJMaster = New MasterBAL()
            Dim dtPersonnel As DataTable = New DataTable()

            Dim strConditions As String = ""
            If (DDL_Customer.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " where ANU.CustomerId = " + DDL_Customer.SelectedValue, strConditions + " and ANU.CustomerId = " + DDL_Customer.SelectedValue)
            End If


            If (DDL_Personnel.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " where ANU.PersonId = " + DDL_Personnel.SelectedValue, strConditions + " and ANU.PersonId = " + DDL_Personnel.SelectedValue)
            End If

            strConditions += " and ISNULL(ANU.IsFluidSecureHub,0)=1  order by ANU.PersonName"
            'get data from server
            dtPersonnel = OBJMaster.GetPersonalDetails(strConditions)
            If (Not dtPersonnel Is Nothing) Then

                If (dtPersonnel.Rows.Count <= 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData = CreateData()
                        CSCommonHelper.WriteLog("Report Genereated", "FluidSecure  Hub Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData = CreateData()
                    CSCommonHelper.WriteLog("Report Genereated", "FluidSecure  Hub Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                Return

            End If
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData = CreateData()
                CSCommonHelper.WriteLog("Report Genereated", "FluidSecure  Hub Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If
            Session("PersonnelDetails") = dtPersonnel

            Response.Redirect("~/Reports/FluidSecureHubRPT")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData = CreateData()
                CSCommonHelper.WriteLog("Report Genereated", "FluidSecure Hub Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
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
                                    "FluidSecure Hub = " & DDL_Personnel.SelectedItem.Text.Replace(",", " ") & " ; "
            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function
End Class
