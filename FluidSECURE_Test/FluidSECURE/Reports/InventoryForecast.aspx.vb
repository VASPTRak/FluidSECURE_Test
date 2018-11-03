Imports log4net
Imports log4net.Config
Public Class InventoryForecast
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(InventoryForecast))

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

                    txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
                    txtTransactionDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
                    txtTransactionTimeFrom.Text = "12:00 AM" 'DateTime.Now.ToString("hh:mm tt")
                    txtTransactionTimeTo.Text = "11:59 PM" ' DateTime.Now.ToString("hh:mm tt")

                    BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                    DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

                Else

                    txtTransactionDateFrom.Text = Request.Form(txtTransactionDateFrom.UniqueID)
                    txtTransactionDateTo.Text = Request.Form(txtTransactionDateTo.UniqueID)
                    txtTransactionTimeFrom.Text = Request.Form(txtTransactionTimeFrom.UniqueID)
                    txtTransactionTimeTo.Text = Request.Form(txtTransactionTimeTo.UniqueID)

                End If
                txtTransactionDateFrom.Focus()
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

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
        Try
            BindTanks(Convert.ToInt32(DDL_Customer.SelectedValue))
        Catch ex As Exception
            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

        Try

            OBJMaster = New MasterBAL()
            Dim dSTran As DataSet = New DataSet()
            Dim startDate As DateTime
            Try
                startDate = Convert.ToDateTime(Request.Form(txtTransactionDateFrom.UniqueID) + " " + Request.Form(txtTransactionTimeFrom.UniqueID)).ToString()
            Catch ex As Exception

                ErrorMessage.InnerText = "Wrong date format selected/enterd for 'From Date'."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                Return
            End Try
            Dim endDate As DateTime
            Try
                endDate = Convert.ToDateTime(Request.Form(txtTransactionDateTo.UniqueID) + " " + Request.Form(txtTransactionTimeTo.UniqueID)).ToString()
            Catch ex As Exception
                ErrorMessage.InnerText = "Wrong date format selected/enterd for 'To Date'."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                Return
            End Try

            If endDate < startDate Then
                ErrorMessage.InnerText = "'From Date' must less than 'To Date'."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                Return
            End If

            'get data from server
            dSTran = OBJMaster.GetInventoryForecastReport(startDate.ToString(), endDate.ToString(), ddl_TankNo.SelectedValue, DDL_Customer.SelectedValue, txtThreshold.Text)
            If (Not dSTran Is Nothing) Then

                If (dSTran.Tables.Count < 1) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData()

                        CSCommonHelper.WriteLog("Report Genereated", "Inventory Forecast Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                    Return
                ElseIf (dSTran.Tables(0).Columns.Count = 1 Or dSTran.Tables(0).Rows.Count < 1) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData()

                        CSCommonHelper.WriteLog("Report Genereated", "Inventory Forecast Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Average usgae for given date range and threshold is 0. Inventory Forecast unable to produce against selected criteria."
                    ErrorMessage.Visible = True
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData()

                    CSCommonHelper.WriteLog("Report Genereated", "Inventory Forecast Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                Return

            End If
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Inventory Forecast Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
            End If
            Session("InventoryForecastReport") = dSTran
            Session("FromDate") = startDate.ToString("dd-MMM-yyyy hh:mm tt")
            Session("ToDate") = endDate.ToString("dd-MMM-yyyy hh:mm tt")
            Session("InventoryForecastCompanyName") = DDL_Customer.SelectedItem.ToString()
            Response.Redirect("~/Reports/InventoryForecastReport")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Inventory Forecast Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
            End If
            ErrorMessage.InnerText = "Data not found against selected criteria."
            ErrorMessage.Visible = True
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
            Return
        Finally
            txtTransactionDateFrom.Focus()
        End Try

    End Sub

    Private Function CreateData() As String
        Try

            Dim data As String = " Date From = " & txtTransactionDateFrom.Text.Replace(",", "") & " ; " &
                                    " Time From = " & txtTransactionTimeFrom.Text.Replace(",", " ") & " ; " &
                                    " Date To = " & txtTransactionDateTo.Text.Replace(",", "") & " ; " &
                                    " Time To = " & txtTransactionTimeTo.Text.Replace(",", "") & " ; " &
                                    "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Threshold = " & txtThreshold.Text.Replace(",", " ") & " ; " &
                                    "Tank Number = " & ddl_TankNo.SelectedItem.Text.Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

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