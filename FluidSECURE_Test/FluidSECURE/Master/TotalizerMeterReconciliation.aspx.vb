Imports log4net
Imports log4net.Config

Public Class TotalizerMeterReconciliation
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TotalizerMeterReconciliation))
    Shared beforeData As String = ""
    Dim OBJMaster As MasterBAL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            'ErrorMessage.Visible = False
            message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
                'Access denied 
                Response.Redirect("/home")
            Else
                If Not IsPostBack Then
                    If (Not Request.QueryString("Type") = Nothing And (Request.QueryString("Type") = "TM")) Then
                        hdnEntryType.Value = Request.QueryString("Type")
                        BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                        bindFluidLink()
                        If Request.QueryString("Tankinventoryid") IsNot Nothing And Request.QueryString("Tankinventoryid") <> "" Then
                            lblHeader.Text = "Edit Totalizer/Meter Reconciliation Information"
                            hdnTankInventory.Value = Request.QueryString("Tankinventoryid")
                            bindTotalizerMeterReconciliationData()
                            If (Request.QueryString("RecordIs") = "New") Then
                                message.Visible = True
                                message.Text = "Record saved"
                                ErrorMessage.Visible = False
                                ErrorMessage.Text = ""
                                DDL_FluidLink.Focus()
                            End If
                        Else
                            lblHeader.Text = "Add Totalizer/Meter Reconciliation Information"
                            txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                            txtStartTime.Text = DateTime.Now.ToString("hh:mm tt")
                            'txtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                            'txtEndTime.Text = DateTime.Now.ToString("hh:mm tt")
                            DDL_FluidLink.SelectedIndex = 0
                            'txtEndLevelQuan.Text = 0
                            txtStartLevelQuan.Text = 0

                            If (Request.QueryString("RecordIs") = "New") Then
                                message.Visible = True
                                message.Text = "Record saved"
                                ErrorMessage.Visible = False
                                ErrorMessage.Text = ""
                                DDL_FluidLink.Focus()
                            End If
                        End If
                    Else
                        Response.Redirect("/home")
                    End If
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                End If
            End If


        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = IIf(ErrorMessage.Text <> "", "", "Error occurred while loading details, please try again later.")
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

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Protected Sub btnStartSave_Click(sender As Object, e As EventArgs)
        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            If DDL_FluidLink.SelectedValue.ToString() = "0" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select FluidSecure Link number"
                DDL_FluidLink.Focus()
                Return
            ElseIf DDL_Customer.SelectedValue.ToString() = "0" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please Select Company"
                DDL_Customer.Focus()
                Return
            End If

            Dim resultDecimalQ As Decimal = 0

            If (txtStartLevelQuan.Text = "") Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Enter Totalizer/Meter Number as decimal and try again."
                txtStartLevelQuan.Focus()
                Return
            End If

            If (txtStartLevelQuan.Text <> "" And Not (Decimal.TryParse(txtStartLevelQuan.Text, resultDecimalQ))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Enter Totalizer/Meter Number as decimal and try again."
                txtStartLevelQuan.Focus()
                Return
            End If

            OBJMaster = New MasterBAL
            Dim dtTot As DataTable = New DataTable()
            Dim TankInventoryId As Integer = 0
            If hdnTankInventory.Value <> "" Then
                TankInventoryId = Convert.ToInt32(hdnTankInventory.Value)
            End If

            Dim InventoryDateTime As DateTime
            Try
                InventoryDateTime = Request.Form(txtStartDate.UniqueID) & " " & Request.Form(txtStartTime.UniqueID)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select valid Date Date/time and try again"
                Return
            End Try

            Dim condition = ""
            condition = " and FluidLink = " + DDL_FluidLink.SelectedValue.ToString() + " and TankInventoryId <> " + TankInventoryId.ToString() + " and DateType = 's' and ENTRY_TYPE = 'TM' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " and InventoryDateTime = '" + InventoryDateTime + "' "
            dtTot = OBJMaster.GetTankInventorybyConditions(condition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            If dtTot IsNot Nothing Then
                If dtTot.Rows.Count > 0 Then
                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Start Level already save for FluidSecure Link Number  " + DDL_FluidLink.SelectedValue.ToString() + " and Date and Time " + InventoryDateTime.ToString("MM/dd/yyyy hh:mm:tt")
                    ErrorMessage.Focus()
                    Return
                End If
            End If


            Dim result = OBJMaster.SaveUpdateTankInventory(TankInventoryId, "", "TM", InventoryDateTime, Convert.ToDecimal(txtStartLevelQuan.Text), "s", Convert.ToInt32(DDL_Customer.SelectedValue), Convert.ToInt32(Session("PersonId").ToString()), DateTime.Now, DDL_FluidLink.SelectedValue.ToString(), DateTime.Now, 0.0, 0, "", "Manual")

            If result > 0 Then
                If (hdnTankInventory.Value <> "") Then
                    message.Visible = True
                    message.Text = "Start Level Record saved"

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 1)
                        CSCommonHelper.WriteLog("Modified", "TotalizerMeterReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 1)
                        CSCommonHelper.WriteLog("Added", "TotalizerMeterReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                    txtStartTime.Text = DateTime.Now.ToString("hh:mm tt")
                    txtStartLevelQuan.Text = "0"
                    message.Visible = True
                    message.Text = "Start Level Record saved"
                End If
                ErrorMessage.Visible = False
                ErrorMessage.Text = ""
                DDL_FluidLink.Focus()

                If TankInventoryId <> 0 Then
                    Response.Redirect("TotalizerMeterReconciliation?TankinventoryId=" & result.ToString() + "&Type=" + hdnEntryType.Value & "&RecordIs=New", False)
                Else
                    Response.Redirect("TotalizerMeterReconciliation?Type=" + hdnEntryType.Value & "&RecordIs=New", False)
                End If


            Else
                If (hdnTankInventory.Value <> "") Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 1)
                        CSCommonHelper.WriteLog("Modified", "TotalizerMeterReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation update failed")
                    End If

                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Start Level Save failed, please try again"

                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 1)
                        CSCommonHelper.WriteLog("Added", "TotalizerMeterReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation Addition failed")
                    End If

                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Start Level Save failed, please try again"

                End If
                DDL_FluidLink.Focus()
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnStartSave_Click Exception is :" + ex.Message)
            If (hdnTankInventory.Value <> "") Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 1)
                    CSCommonHelper.WriteLog("Modified", "TotalizerMeterReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation update failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Totalizer/Meter Reconciliation Start Level update failed, please try again"
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 1)
                    CSCommonHelper.WriteLog("Added", "TotalizerMeterReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation Addition failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Totalizer/Meter Reconciliation Start Level Addition failed, please try again"
            End If

        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();ClosePopUpStart();", True)
        End Try
    End Sub

    'Protected Sub btnEndSave_Click(sender As Object, e As EventArgs)
    '    Try
    '        If DDL_FluidLink.SelectedValue.ToString() = "0" Then
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Please select FluidSecure Link number"
    '            DDL_FluidLink.Focus()
    '            Return
    '        ElseIf DDL_Customer.SelectedValue.ToString() = "0" Then
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Please Select Company"
    '            DDL_Customer.Focus()
    '            Return
    '        End If

    '        OBJMaster = New MasterBAL
    '        Dim dtTot As DataTable = New DataTable()
    '        Dim TankInventoryId As Integer = 0
    '        If hdnTankInventory.Value <> "" Then
    '            TankInventoryId = Convert.ToInt32(hdnTankInventory.Value)
    '        End If
    '        Dim InventoryDateTime As DateTime = Request.Form(txtEndDate.UniqueID) & " " & Request.Form(txtEndTime.UniqueID)
    '        Dim condition = ""
    '        condition = " and FluidLink = " + DDL_FluidLink.SelectedValue.ToString() + " and TankInventoryId <> " + TankInventoryId.ToString() + " and DateType = 'e' and ENTRY_TYPE = 'TM' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " and InventoryDateTime = '" + InventoryDateTime + "' "
    '        dtTot = OBJMaster.GetTankInventorybyConditions(condition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
    '        If dtTot IsNot Nothing Then
    '            If dtTot.Rows.Count > 0 Then
    '                ErrorMessage.Visible = True
    '                ErrorMessage.Text = "End Level already save for FluidSecure Link Number  " + DDL_FluidLink.SelectedValue.ToString() + " and Date and Time " + InventoryDateTime.ToString("MM/dd/yyyy hh:mm:tt")
    '                ErrorMessage.Focus()
    '                Return
    '            End If
    '        End If


    '        Dim result = OBJMaster.SaveUpdateTankInventory(TankInventoryId, "", "TM", InventoryDateTime, Convert.ToInt32(txtEndLevelQuan.Text), "e", Convert.ToInt32(DDL_Customer.SelectedValue), Convert.ToInt32(Session("PersonId").ToString()), DateTime.Now, DDL_FluidLink.SelectedValue.ToString(), DateTime.Now, 0.0, 0, "", "Manual")

    '        If result > 0 Then
    '            If (hdnTankInventory.Value <> "") Then
    '                message.Visible = True
    '                message.Text = "End Level Record saved"

    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    Dim writtenData As String = CreateData(result, 2)
    '                    CSCommonHelper.WriteLog("Modified", "TotalizerMeterReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
    '                End If

    '            Else
    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    Dim writtenData As String = CreateData(result, 2)
    '                    CSCommonHelper.WriteLog("Added", "TotalizerMeterReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
    '                End If

    '                message.Visible = True
    '                message.Text = "End Level Record saved"
    '                txtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
    '                txtEndTime.Text = DateTime.Now.ToString("hh:mm tt")
    '                txtEndLevelQuan.Text = "0"
    '            End If
    '            ErrorMessage.Visible = False
    '            ErrorMessage.Text = ""
    '            DDL_FluidLink.Focus()
    '            Response.Redirect("TotalizerMeterReconciliation?TankinventoryId=" & result.ToString() + "&Type=" + hdnEntryType.Value & "&RecordIs=New", False)
    '        Else
    '            If (hdnTankInventory.Value <> "") Then
    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 2)
    '                    CSCommonHelper.WriteLog("Modified", "TotalizerMeterReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation update failed")
    '                End If

    '                ErrorMessage.Visible = True
    '                ErrorMessage.Text = "End Level Save failed, please try again"
    '            Else
    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                        Dim writtenData As String = CreateData(result, 2)
    '                        CSCommonHelper.WriteLog("Added", "TotalizerMeterReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation Addition failed")
    '                    End If

    '                    ErrorMessage.Visible = True
    '                    ErrorMessage.Text = "End Level Save failed, please try again"
    '                End If
    '                DDL_FluidLink.Focus()
    '            End If
    '        End If
    '    Catch ex As Exception
    '        log.Error("Error occurred in btnEndSave_Click Exception is :" + ex.Message)
    '        If (hdnTankInventory.Value <> "") Then
    '            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 2)
    '                CSCommonHelper.WriteLog("Modified", "TotalizerMeterReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation update failed. Exception is : " & ex.Message)
    '            End If
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Totalizer/Meter Reconciliation End Level update failed, please try again"
    '        Else
    '            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 2)
    '                CSCommonHelper.WriteLog("Added", "TotalizerMeterReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TotalizerMeterReconciliation Addition failed. Exception is : " & ex.Message)
    '            End If
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Totalizer/Meter Reconciliation End Level Addition failed, please try again"
    '        End If
    '    Finally
    '        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();ClosePopUpEnd();", True)
    '    End Try
    'End Sub

    Protected Sub btnCancelStart_Click(sender As Object, e As EventArgs)
        Try

        Catch ex As Exception
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnCancelEnd_Click(sender As Object, e As EventArgs)
        Try

        Catch ex As Exception
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnMainCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("/Master/AllTotalizerMeterReconciliation" + "?Type=" + hdnEntryType.Value, False)
    End Sub

    'Protected Sub BindAllGrid()
    '    Try
    '        'Bind Start data
    '        OBJMaster = New MasterBAL()
    '        Dim dtStart As DataTable = New DataTable()
    '        dtStart = OBJMaster.GetTankInventorybyConditions(" and FluidLink = " + DDL_FluidLink.SelectedValue.ToString() + " and DateType = 's' and ENTRY_TYPE = 'TM' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
    '        If dtStart IsNot Nothing Then
    '            If dtStart.Rows.Count > 0 Then
    '                gv_StartLevel.DataSource = dtStart
    '                gv_StartLevel.DataBind()
    '            Else
    '                gv_StartLevel.DataSource = Nothing
    '                gv_StartLevel.DataBind()
    '            End If
    '        Else
    '            gv_StartLevel.DataSource = Nothing
    '            gv_StartLevel.DataBind()
    '        End If

    '        'Bind End data
    '        OBJMaster = New MasterBAL()
    '        Dim dtEnd As DataTable = New DataTable()
    '        dtEnd = OBJMaster.GetTankInventorybyConditions(" and FluidLink = " + DDL_FluidLink.SelectedValue.ToString() + " and DateType = 'e' and ENTRY_TYPE = 'TM' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
    '        If dtEnd IsNot Nothing Then
    '            If dtEnd.Rows.Count > 0 Then
    '                gv_EndLevel.DataSource = dtEnd
    '                gv_EndLevel.DataBind()
    '            Else
    '                gv_EndLevel.DataSource = Nothing
    '                gv_EndLevel.DataBind()
    '            End If
    '        Else
    '            gv_EndLevel.DataSource = Nothing
    '            gv_EndLevel.DataBind()
    '        End If
    '    Catch ex As Exception
    '        log.Error("Error occurred in BindAllGrid Exception is :" + ex.Message)
    '        ErrorMessage.Visible = True
    '        ErrorMessage.Text = "Error occurred while getting BindAllGrid, please try again later."
    '    End Try
    'End Sub

    'Protected Sub BTN_StartType_Click(sender As Object, e As EventArgs)
    '    lblStartLevelFluidNumber.Text = "FluiedSecure Link Number: " + DDL_FluidLink.SelectedValue.ToString()
    '    BindAllGrid()
    '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "$('#StartDatePicker').modal({show: true,backdrop: 'static',keyboard: false});", True)
    'End Sub

    'Protected Sub BTN_EndType_Click(sender As Object, e As EventArgs)
    '    lblStartLevelFluidNumber.Text = "FluiedSecure Link Number: " + DDL_FluidLink.SelectedValue.ToString()
    '    BindAllGrid()
    '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "$('#EndDatePicker').modal({show: true,backdrop: 'static',keyboard: false});", True)
    'End Sub

    Protected Sub bindTotalizerMeterReconciliationData()
        Try
            OBJMaster = New MasterBAL
            Dim dtTot As DataTable = New DataTable()
            dtTot = OBJMaster.GetTankInventorybyId(Convert.ToInt32(hdnTankInventory.Value))
            If dtTot IsNot Nothing Then
                If dtTot.Rows.Count > 0 Then
                    Dim isValid As Boolean = False
                    If (Session("RoleName") = "GroupAdmin") Then
                        Dim dtCustOld As DataTable = New DataTable()

                        dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                        For Each drCusts As DataRow In dtCustOld.Rows
                            If (drCusts("CustomerId") = dtTot.Rows(0)("CompanyId").ToString()) Then
                                isValid = True
                                Exit For
                            End If

                        Next
                    End If

                    If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                        Dim dtCust As DataTable = New DataTable()

                        dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                        If (dtCust.Rows(0)("CustomerId").ToString() <> dtTot.Rows(0)("CompanyId").ToString()) Then

                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                            Return
                        End If

                    End If

                    DDL_Customer.SelectedValue = dtTot.Rows(0)("CompanyId").ToString()
                    bindFluidLink()
                    DDL_FluidLink.SelectedValue = dtTot.Rows(0)("FluidLink").ToString()

                    If dtTot.Rows(0)("DateType") = "s" Then
                        StartDiv.Visible = True
                        'EndDiv.Visible = False

                        txtStartDate.Text = Convert.ToDateTime(dtTot.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy")
                        txtStartTime.Text = Convert.ToDateTime(dtTot.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt")
                        txtStartLevelQuan.Text = dtTot.Rows(0)("Quantity").ToString()
                        beforeData = "TankInventoryId = " & hdnTankInventory.Value & " ; " &
                                     "FluidSecure Link Number = " & DDL_FluidLink.SelectedItem.Text.Replace(",", " ") & " ; " &
                                     "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                     "Entry Type = Totalizer/Meter " & " ; " &
                                     "Date Type = Start" & " ; " &
                                     "Start Date = " & Convert.ToDateTime(dtTot.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy").Replace(",", " ") & " ; " &
                                     "Start Time = " & Convert.ToDateTime(dtTot.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt").Replace(",", " ") & " ; " &
                                     "Starting Totalizer/Meter Number = " & txtStartLevelQuan.Text.Replace(",", " ") & " "
                        'ElseIf dtTot.Rows(0)("DateType") = "e" Then
                        '    StartDiv.Visible = False
                        '    'EndDiv.Visible = True

                        '    txtEndDate.Text = Convert.ToDateTime(dtTot.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy")
                        '    txtEndTime.Text = Convert.ToDateTime(dtTot.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt")
                        '    txtEndLevelQuan.Text = dtTot.Rows(0)("Quantity").ToString()
                        '    beforeData = "TankInventoryId = " & hdnTankInventory.Value & " ; " &
                        '                  "FluidSecure Link Number = " & DDL_FluidLink.SelectedItem.Text.Replace(",", " ") & " ; " &
                        '                  "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                        '                  "Entry Type = Totalizer/Meter " & " ; " &
                        '                  "Date Type = End" & " ; " &
                        '                  "End Date = " & Convert.ToDateTime(dtTot.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy").Replace(",", " ") & " ; " &
                        '                  "End Time = " & Convert.ToDateTime(dtTot.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt").Replace(",", " ") & " ; " &
                        '                  "Ending Totalizer/Meter Number = " & txtEndLevelQuan.Text.Replace(",", " ") & " "
                    End If
                End If
            End If
        Catch ex As Exception
            log.Error("Error occurred in bindTotalizerMeterReconciliationData Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting bindTotalizerMeterReconciliationData, please try again later."
        End Try
    End Sub

    Protected Sub btnStartOk_Click(sender As Object, e As EventArgs)
        Try
            For Each gv_StartLevelData As GridViewRow In gv_StartLevel.Rows
                Dim Rdb_StartLevel As RadioButton = TryCast(gv_StartLevelData.FindControl("Rdb_StartLevel"), RadioButton)
                If Rdb_StartLevel.Checked Then
                    txtStartDate.Text = gv_StartLevel.Rows(gv_StartLevelData.RowIndex).Cells(1).Text
                    txtStartTime.Text = gv_StartLevel.Rows(gv_StartLevelData.RowIndex).Cells(2).Text
                    txtStartLevelQuan.Text = gv_StartLevel.Rows(gv_StartLevelData.RowIndex).Cells(3).Text
                    txtStartDate.Focus()
                End If
            Next
        Catch ex As Exception
            log.Error("Error occurred in btnStartOk_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting btnStartOk_Click, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    'Protected Sub btnEndOk_Click(sender As Object, e As EventArgs)
    '    Try
    '        For Each gv_EndLevelData As GridViewRow In gv_EndLevel.Rows
    '            Dim Rdb_EndLevel As RadioButton = TryCast(gv_EndLevelData.FindControl("Rdb_EndLevel"), RadioButton)
    '            If Rdb_EndLevel.Checked Then
    '                txtEndDate.Text = gv_EndLevel.Rows(gv_EndLevelData.RowIndex).Cells(1).Text
    '                txtEndTime.Text = gv_EndLevel.Rows(gv_EndLevelData.RowIndex).Cells(2).Text
    '                txtEndLevelQuan.Text = gv_EndLevel.Rows(gv_EndLevelData.RowIndex).Cells(3).Text
    '                txtEndDate.Focus()
    '            End If
    '        Next
    '    Catch ex As Exception
    '        log.Error("Error occurred in btnEndOk_Click Exception is :" + ex.Message)
    '        ErrorMessage.Visible = True
    '        ErrorMessage.Text = "Error occurred while getting btnEndOk_Click, please try again later."
    '    Finally
    '        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
    '    End Try
    'End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            bindFluidLink()

        Catch ex As Exception
            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub bindFluidLink()
        Try

            OBJMaster = New MasterBAL()
            Dim dtFluid As DataTable = New DataTable()
            Dim strCondition = ""
            DDL_FluidLink.Items.Clear()
            If DDL_Customer.SelectedValue.ToString() = "0" Then
                strCondition = ""
            Else
                strCondition = "and c.CustomerId = " + DDL_Customer.SelectedValue.ToString()
                dtFluid = OBJMaster.GetSiteByCondition(strCondition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), False)
                If dtFluid.Rows.Count > 0 Then
                    Dim dtview As DataView = New DataView(dtFluid.DefaultView.ToTable(True, "SiteNumber", "WifiSSId"))
                    dtview.Sort = "SiteNumber"
                    DDL_FluidLink.DataSource = dtview
                    DDL_FluidLink.DataTextField = "WifiSSId"
                    DDL_FluidLink.DataValueField = "SiteNumber"
                    DDL_FluidLink.DataBind()
                End If
            End If

            DDL_FluidLink.Items.Insert(0, New ListItem("Select FluidSecure Link", "0"))
            DDL_FluidLink.SelectedIndex = 0

        Catch ex As Exception

            log.Error("Error occurred in bindFluidLink Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Function CreateData(TankInventoryId As Integer, DateType As Integer) As String
        Try

            Dim data As String = ""
            If DateType = 1 Then
                data = "TankInventoryId = " & TankInventoryId & " ; " &
                       "FluidSecure Link Number = " & DDL_FluidLink.SelectedItem.Text.Replace(",", " ") & " ; " &
                       "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                       "Entry Type = Totalizer/Meter " & " ; " &
                       "Date Type = Start" & " ; " &
                       "Start Date = " & Request.Form(txtStartDate.UniqueID).Replace(",", " ") & " ; " &
                       "Start Time = " & Request.Form(txtStartTime.UniqueID).ToString().Replace(",", " ") & " ; " &
                       "Starting Totalizer/Meter Number = " & txtStartLevelQuan.Text.Replace(",", " ") & " "
            Else
                'data = "TankInventoryId = " & TankInventoryId & " ; " &
                '       "FluidSecure Link Number = " & DDL_FluidLink.SelectedItem.Text.Replace(",", " ") & " ; " &
                '       "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                '       "Entry Type = Totalizer/Meter " & " ; " &
                '       "Date Type = End" & " ; " &
                '       "End Date = " & Request.Form(txtEndDate.UniqueID).Replace(",", " ") & " ; " &
                '       "End Time = " & Request.Form(txtEndTime.UniqueID).ToString().Replace(",", " ") & " ; " &
                '       "Ending Totalizer/Meter Number = " & txtEndLevelQuan.Text.Replace(",", " ") & " "
            End If


            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class
