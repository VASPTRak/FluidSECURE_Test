Imports log4net
Imports log4net.Config
Public Class InactiveVehicleByTransaction
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(InactiveVehicleByTransaction))

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

    Private Sub BindDepartment(CustomerId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtDept As DataTable = New DataTable()
            If CustomerId <> 0 Then
                dtDept = OBJMaster.GetDepartmentsByCustomerId(CustomerId)
                DDL_Dept.DataSource = dtDept
            Else
                DDL_Dept.DataSource = dtDept
            End If

            grd_Dept.DataSource = dtDept
            grd_Dept.DataBind()

            If grd_Dept.Rows.Count <> 0 Then
                lblDepartmentMessage.Visible = False
                grd_Dept.Visible = True
            ElseIf grd_Dept.Rows.Count = 0 Then
                lblDepartmentMessage.Text = "Department not found for selected Company."
                lblDepartmentMessage.Visible = True
                grd_Dept.Visible = False
            End If

            DDL_Dept.DataTextField = "NAME"
            DDL_Dept.DataValueField = "DeptId"

            DDL_Dept.DataBind()
            DDL_Dept.Items.Insert(0, New ListItem("Select All Department", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindDepartment Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting departments, please try again later."

        End Try
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
        Try
            BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
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

            Dim strConditions As String = ""
            If (DDL_Customer.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and T.CompanyId = " + DDL_Customer.SelectedValue, strConditions + " and T.CompanyId = " + DDL_Customer.SelectedValue)
            End If

            If (DDL_Dept.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and d.DeptId = " + DDL_Dept.SelectedValue, strConditions + " and d.DeptId = " + DDL_Dept.SelectedValue)
            End If


            'get data from server
            dSTran = OBJMaster.GetInactiveVehicleReport(startDate.ToString(), endDate.ToString(), IIf(DDL_Dept.SelectedValue = "0", 0, DDL_Dept.SelectedValue), DDL_Customer.SelectedValue)
            If (Not dSTran Is Nothing) Then

                If (dSTran.Tables.Count < 1) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData()

                        CSCommonHelper.WriteLog("Report Genereated", "Inactive Vehicle Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData()

                    CSCommonHelper.WriteLog("Report Genereated", "Inactive Vehicle Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                Return

            End If
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Inactive Vehicle Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
            End If
            Session("InactiveVehicleByTransaction") = dSTran
            Session("FromDate") = startDate.ToString("dd-MMM-yyyy hh:mm tt")
            Session("ToDate") = endDate.ToString("dd-MMM-yyyy hh:mm tt")
            Session("InactiveVehicleCompanyName") = DDL_Customer.SelectedItem.ToString()
            Response.Redirect("~/Reports/InactiveVehicleByTransactionReport")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Inactive Vehicle Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
            End If
            ErrorMessage.InnerText = "Data not found against selected criteria."
            ErrorMessage.Visible = True
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
            Return
        Finally
            txtTransactionDateFrom.Focus()
        End Try

    End Sub

    Protected Sub DDL_Dept_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            For Each rows As GridViewRow In grd_Dept.Rows
                If (DDL_Dept.SelectedValue = grd_Dept.DataKeys(rows.RowIndex).Values("DeptId").ToString()) Then
                    TryCast(rows.FindControl("RDB_Department"), RadioButton).Checked = True
                Else
                    TryCast(rows.FindControl("RDB_Department"), RadioButton).Checked = False
                End If
            Next
        Catch ex As Exception
            log.Error("Error occurred in DDL_Dept_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Private Function CreateData() As String
        Try

            Dim data As String = "Transaction Date From = " & txtTransactionDateFrom.Text.Replace(",", "") & " ; " &
                                    "Transaction Time From = " & txtTransactionTimeFrom.Text.Replace(",", " ") & " ; " &
                                    "Transaction Date To = " & txtTransactionDateTo.Text.Replace(",", "") & " ; " &
                                    "Transaction Time To = " & txtTransactionTimeTo.Text.Replace(",", "") & " ; " &
                                    "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Department = " & DDL_Dept.SelectedItem.Text.Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub btndeptOK_Click(sender As Object, e As EventArgs)
        Try

            Dim isChecked As Boolean = False
            For Each item As GridViewRow In grd_Dept.Rows

                Dim RDB_Department As RadioButton = TryCast(item.FindControl("RDB_Department"), RadioButton)
                If (RDB_Department.Checked = True) Then
                    isChecked = True
                    HDF_DeptId.Value = grd_Dept.DataKeys(item.RowIndex).Values("DeptId").ToString()
                    HDF_DeptNumber.Value = grd_Dept.DataKeys(item.RowIndex).Values("Number").ToString()
                    DDL_Dept.SelectedValue = grd_Dept.DataKeys(item.RowIndex).Values("DeptId").ToString()
                    Exit For
                End If
            Next

            If (isChecked = False) Then
                DDL_Dept.SelectedValue = 0
            End If
            DDL_Dept_SelectedIndexChanged(Nothing, Nothing)
        Catch ex As Exception
            log.Error("Error occurred in btndeptOK_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();hideWait();", True)
        End Try
    End Sub

End Class