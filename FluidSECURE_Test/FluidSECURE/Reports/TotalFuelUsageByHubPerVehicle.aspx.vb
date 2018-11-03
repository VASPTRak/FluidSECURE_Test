Imports log4net
Imports log4net.Config

Public Class TotalFuelUsageByHubPerVehicle
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TotalFuelUsageByHubPerVehicle))

    Dim OBJMaster As MasterBAL
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
            If Session("RoleName") <> "SuperAdmin" Then
                If Session("TotalFuelUsageByHubPerVehicle") <> "TotalFuelUsageByHubPerVehicle" Then
                    'Access denied 
                    Response.Redirect("/home")
                End If
            End If


            If Not IsPostBack Then
                txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
                txtTransactionDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
                txtTransactionTimeFrom.Text = "12:00 AM"
                txtTransactionTimeTo.Text = "11:59 PM"

                BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
            Else
                txtTransactionDateFrom.Text = Request.Form(txtTransactionDateFrom.UniqueID)
                txtTransactionDateTo.Text = Request.Form(txtTransactionDateTo.UniqueID)
                txtTransactionTimeFrom.Text = Request.Form(txtTransactionTimeFrom.UniqueID)
                txtTransactionTimeTo.Text = Request.Form(txtTransactionTimeTo.UniqueID)
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
            End If
            txtTransactionDateFrom.Focus()
        End If

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

    Private Sub BindAllHubs()
        Try
            Dim dtPersonnel As DataTable = New DataTable()
            OBJMaster = New MasterBAL()

            If (DDL_Customer.SelectedValue <> "0") Then
                dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(ANU.IsFluidSecureHub,0)=1  and ANU.CustomerId = " & DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

                DDL_HubName.DataSource = dtPersonnel
                DDL_HubName.DataValueField = "PersonId"
                DDL_HubName.DataTextField = "HubSiteName"
                DDL_HubName.DataBind()
            End If

            DDL_HubName.Items.Insert(0, New ListItem("Select Site", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindAllHubs Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting All Hubs, please try again later."

        End Try
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
        Try
            BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
            BindAllVehicles(Convert.ToInt32(DDL_Dept.SelectedValue))
            BindAllHubs()
        Catch ex As Exception

            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
        End Try
    End Sub

    Private Sub BindAllVehicles(deptId As Integer)
        Try
            Dim dtVehicle As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            If (deptId = 0) Then
                dtVehicle = OBJMaster.GetVehicleByCondition(" and  v.CustomerId  = " & DDL_Customer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())
            Else
                dtVehicle = OBJMaster.GetVehicleByCondition(" and V.DepartmentId  = " & deptId, Session("PersonId").ToString(), Session("RoleId").ToString())
            End If

            gv_Vehicles.DataSource = dtVehicle
            gv_Vehicles.DataBind()

            If gv_Vehicles.Rows.Count <> 0 Then

                lblVehicleMessage.Visible = False
                gv_Vehicles.Visible = True
            ElseIf gv_Vehicles.Rows.Count = 0 Then
                lblVehicleMessage.Text = "Vehicles not found for selected Company."
                lblVehicleMessage.Visible = True
                gv_Vehicles.Visible = False
            End If

            lst_Vehicle.DataSource = dtVehicle
            lst_Vehicle.DataValueField = "VehicleId"
            lst_Vehicle.DataTextField = "VehicleNumber"
            lst_Vehicle.DataBind()

        Catch ex As Exception

            log.Error("Error occurred in BindAllVehicles Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

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
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
                Return
            End Try
            Dim endDate As DateTime
            Try
                endDate = Convert.ToDateTime(Request.Form(txtTransactionDateTo.UniqueID) + " " + Request.Form(txtTransactionTimeTo.UniqueID)).ToString()
            Catch ex As Exception
                ErrorMessage.InnerText = "Wrong date format selected/enterd for 'To Date'."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
                Return
            End Try

            If endDate < startDate Then
                ErrorMessage.InnerText = "'From Date' must less than 'To Date'."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
                Return
            End If

            Dim strConditions As String = ""
            If (DDL_Customer.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and ANU.CustomerId = " + DDL_Customer.SelectedValue, strConditions + " and ANU.CustomerId = " + DDL_Customer.SelectedValue)
            End If

            If (DDL_Dept.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and d.DeptId = " + DDL_Dept.SelectedValue, strConditions + " and d.DeptId = " + DDL_Dept.SelectedValue)
            End If

            'If (DDL_Vehicle.SelectedValue <> "0") Then
            '    strConditions = IIf(strConditions = "", " and T.VehicleID = " + DDL_Vehicle.SelectedValue, strConditions + " and T.VehicleID = " + DDL_Vehicle.SelectedValue)
            'End If

            Dim SelectedVehicles As String = ""

            For Each item As ListItem In lst_Vehicle.Items
                If item.Selected Then
                    SelectedVehicles = IIf(SelectedVehicles = "", item.Value, SelectedVehicles + "," + item.Value)
                End If
            Next
            If (SelectedVehicles <> "") Then
                strConditions = IIf(strConditions = "", " and T.VehicleID in ( " + SelectedVehicles + ")", strConditions + " and T.VehicleID in ( " + SelectedVehicles + ")")
            End If


            If (DDL_HubName.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and ISNULL(T.HubId,0) = " + DDL_HubName.SelectedValue, strConditions + " and ISNULL(T.HubId,0) = " + DDL_HubName.SelectedValue)
            End If

            dSTran = OBJMaster.GetTotalFuelUsageByHubPerVehicleRptDetails(strConditions, startDate.ToString(), endDate.ToString())
            If (Not dSTran Is Nothing) Then

                If (dSTran.Tables(0).Rows.Count <= 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData = CreateData()
                        CSCommonHelper.WriteLog("Report Genereated", "Total Fuel Usage By Hub Per Vehicle", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData = CreateData()
                    CSCommonHelper.WriteLog("Report Genereated", "Total Fuel Usage By Hub Per Vehicle", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
                Return

            End If




            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData = CreateData()
                CSCommonHelper.WriteLog("Report Genereated", "Total Fuel Usage By Hub Per Vehicle", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If
            Session("TotalFuelUsageByHubPerVehicleDetails") = dSTran.Tables(0)
            Session("FromDate") = startDate.ToString("dd-MMM-yyyy hh:mm tt")
            Session("ToDate") = endDate.ToString("dd-MMM-yyyy hh:mm tt")
            Session("CustomerName") = DDL_Customer.SelectedItem.ToString()
            Response.Redirect("~/Reports/TotalFuelUsageByHubPerVehicleReport.aspx")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData = CreateData()
                CSCommonHelper.WriteLog("Report Genereated", "Total Fuel Usage By Hub Per Vehicle", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
            End If
            ErrorMessage.InnerText = "Data not found against selected criteria."
            ErrorMessage.Visible = True
            Return
        Finally
            txtTransactionDateFrom.Focus()
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
        End Try

    End Sub

    Protected Sub DDL_Dept_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            BindAllVehicles(Convert.ToInt32(DDL_Dept.SelectedValue))
        Catch ex As Exception

            log.Error("Error occurred in DDL_Dept_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lst_Vehicle]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
        End Try
    End Sub

    Private Function CreateData() As String
        Try
            Dim VehiclesBind As String = ""
            For Each item As ListItem In lst_Vehicle.Items
                If item.Selected Then
                    VehiclesBind = IIf(VehiclesBind = "", item.Text, VehiclesBind + " & " + item.Text)
                End If
            Next

            Dim data As String = "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Vehicle Number = " & VehiclesBind.Replace(",", " ") & " ; " &
                                    "Department = " & DDL_Dept.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Select Site = " & DDL_HubName.SelectedItem.Text.Replace(",", " ") & " ; "
            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub btnOk_Click(sender As Object, e As EventArgs)
        Try
            Dim isChecked As Boolean = False
            lst_Vehicle.ClearSelection()
            HDF_VehicleId.Value = ""
            For Each item As GridViewRow In gv_Vehicles.Rows

                Dim CHK_Vehicle As CheckBox = TryCast(item.FindControl("CHK_Vehicle"), CheckBox)
                If (CHK_Vehicle.Checked = True) Then
                    isChecked = True
                    HDF_VehicleId.Value = HDF_VehicleId.Value & "," & gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleId").ToString()
                    'HDF_VehicleNumber.Value = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber").ToString()
                    'lst_Vehicle.SelectedValue = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleId").ToString()
                End If
            Next
            Dim checkVehValue As String() = HDF_VehicleId.Value.Split(",")
            For Each item As ListItem In lst_Vehicle.Items
                If checkVehValue.Contains(item.Value) Then
                    item.Selected = True
                End If
            Next
        Catch ex As Exception
            log.Error("Error occurred in btnOk_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList;hideWait();", True)
        End Try
    End Sub

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
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();hideWait();", True)
        End Try
    End Sub

End Class