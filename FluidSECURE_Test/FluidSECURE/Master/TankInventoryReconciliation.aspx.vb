Imports log4net
Imports log4net.Config

Public Class TankInventoryReconciliation
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TankInventoryReconciliation))
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
                    Session("CostingMethodForDelivery") = "0"
                    If (Not Request.QueryString("Type") = Nothing And (Request.QueryString("Type") = "Level" Or Request.QueryString("Type") = "RD")) Then
                        hdnEntryType.Value = Request.QueryString("Type")
                        BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                        DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                        If Request.QueryString("Tankinventoryid") IsNot Nothing And Request.QueryString("Tankinventoryid") <> "" Then
                            lblHeader.Text = "Edit Inventory Information"
                            hdnTankInventory.Value = Request.QueryString("Tankinventoryid")
                            bindTankInventoryData()
                            If (Request.QueryString("RecordIs") = "New") Then
                                message.Visible = True
                                message.Text = "Record saved"
                                ErrorMessage.Visible = False
                                ErrorMessage.Text = ""
                                ddl_TankNo.Focus()
                            End If
                        Else
                            lblHeader.Text = "Add Inventory Information"
                            txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                            txtStartTime.Text = DateTime.Now.ToString("hh:mm tt")
                            'txtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                            'txtEndTime.Text = DateTime.Now.ToString("hh:mm tt")
                            txtDELIStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                            txtDELIStartTime.Text = DateTime.Now.ToString("hh:mm tt")
                            'txtDELIEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                            'txtDELIEndTime.Text = DateTime.Now.ToString("hh:mm tt")
                            txtDELILevelQuan.Text = 0
                            'txtEndLevelQuan.Text = 0
                            txtStartLevelQuan.Text = 0
                            txtProductPrice.Text = "0"
                            If (Request.QueryString("RecordIs") = "New") Then
                                message.Visible = True
                                message.Text = "Record saved"
                                ErrorMessage.Visible = False
                                ErrorMessage.Text = ""
                                Try
                                    If (Request.QueryString("TankNumber") IsNot Nothing) Then
                                        ddl_TankNo.SelectedValue = Request.QueryString("TankNumber").ToString()
                                    End If
                                    If (Request.QueryString("CustomerID") IsNot Nothing) Then
                                        DDL_Customer.SelectedValue = Request.QueryString("CustomerID").ToString()
                                    End If
                                Catch ex As Exception
                                End Try
                                ddl_TankNo.Focus()
                            End If
                        End If

                        BindAllGrid()
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

            If (Not Session("RoleName") = "SuperAdmin") Then
                DDL_Customer.SelectedIndex = 1
                DDL_Customer.Enabled = False
                DDL_Customer.Visible = False
                divCompany.Visible = False
            End If

            Session("CostingMethodForDelivery") = dtCust.Rows(0)("CostingMethod").ToString()

            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                DDL_Customer.SelectedIndex = 1
                'If dtCust.Rows(0)("CostingMethod").ToString() = "1" Then
                '    DeliDiv.Visible = False
                'Else
                '    DeliDiv.Visible = True
                'End If
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Protected Sub btnStartSave_Click(sender As Object, e As EventArgs)
        Try
            If ddl_TankNo.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select TANK number"
                ddl_TankNo.Focus()
                Return
            ElseIf DDL_Customer.SelectedValue.ToString() = "0" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please Select Company"
                DDL_Customer.Focus()
                Return
            Else
                'OBJMaster = New MasterBAL
                'Dim dtTankCheck As DataTable = New DataTable()
                'dtTankCheck = OBJMaster.GetSiteByCondition(" and h.TankNumber = " + txtTankNo.Text + " and c.CustomerId = " + DDL_Customer.SelectedValue.ToString(), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                'If dtTankCheck IsNot Nothing Then
                '    If dtTankCheck.Rows.Count = 0 Then
                '        ErrorMessage.Visible = True
                '        ErrorMessage.Text = "Tank number not found for selected Company. Please check and enter again."
                '        ddl_TankNo.Focus()
                '        Return
                '    End If
                'End If
            End If

            OBJMaster = New MasterBAL
            Dim dtTank As DataTable = New DataTable()
            Dim TankInventoryId As Integer = 0
            If hdnTankInventory.Value <> "" Then
                TankInventoryId = Convert.ToInt32(hdnTankInventory.Value)
            End If

            If txtStartLevelQuan.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Tank Level Quantity and try again."
                txtStartLevelQuan.Focus()
                Return
            End If

            Dim InventoryDateTime As DateTime
            Try
                InventoryDateTime = Request.Form(txtStartDate.UniqueID) & " " & Request.Form(txtStartTime.UniqueID)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select valid Tank Inventory Date/time and try again"
                Return
            End Try

            Dim resultDecimal As Decimal = 0

            If (txtStartLevelQuan.Text <> "" And Not (Decimal.TryParse(txtStartLevelQuan.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Enter Tank Level Quantity as decimal and try again."
                txtStartLevelQuan.Focus()
                Return
            End If


            Dim condition = ""
            condition = " and TankInventory.TankNumber = '" + ddl_TankNo.SelectedValue.ToString() + "' and TankInventoryId <> " + TankInventoryId.ToString() + " and DateType = 's' and ENTRY_TYPE = 'Level' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " and InventoryDateTime = '" + InventoryDateTime + "' "
            dtTank = OBJMaster.GetTankInventorybyConditions(condition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            If dtTank IsNot Nothing Then
                If dtTank.Rows.Count > 0 Then
                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Start Level already save for Tank " + ddl_TankNo.SelectedValue.ToString() + " and Date and Time " + InventoryDateTime.ToString("MM/dd/yyyy hh:mm:tt")
                    ErrorMessage.Focus()
                    Return
                End If
            End If


            Dim result = OBJMaster.SaveUpdateTankInventory(TankInventoryId, ddl_TankNo.SelectedValue.ToString(), "Level", InventoryDateTime, Convert.ToDecimal(txtStartLevelQuan.Text), "s", Convert.ToInt32(DDL_Customer.SelectedValue), Convert.ToInt32(Session("PersonId").ToString()), DateTime.Now, "", DateTime.Now, 0.0, 0, "", "Manual")

            If result > 0 Then
                If (hdnTankInventory.Value <> "") Then
                    message.Visible = True
                    message.Text = "Start Level Record saved"

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 1)
                        CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 1)
                        CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                    txtStartTime.Text = DateTime.Now.ToString("hh:mm tt")
                    txtStartLevelQuan.Text = "0"
                    message.Visible = True
                    message.Text = "Start Level Record saved"
                End If
                ErrorMessage.Visible = False
                ErrorMessage.Text = ""
                ddl_TankNo.Focus()
                If TankInventoryId <> 0 Then
                    Response.Redirect("TankInventoryReconciliation?TankinventoryId=" & result.ToString() + "&Type=" + hdnEntryType.Value & "&RecordIs=New", False)
                Else
                    Response.Redirect("TankInventoryReconciliation?Type=" + hdnEntryType.Value & "&RecordIs=New&TankNumber=" & ddl_TankNo.SelectedValue.ToString() & "&CustomerID=" & DDL_Customer.SelectedValue.ToString(), False)
                End If

            Else
                If (hdnTankInventory.Value <> "") Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 1)
                        CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation update failed")
                    End If

                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Start Level Save failed, please try again"

                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 1)
                        CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation Addition failed")
                    End If

                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Start Level Save failed, please try again"

                End If
                ddl_TankNo.Focus()
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnStartSave_Click Exception is :" + ex.Message)
            If (hdnTankInventory.Value <> "") Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 1)
                    CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation update failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Tank Inventory Reconciliation Start Level update failed, please try again"
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 1)
                    CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation Addition failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Tank Inventory Reconciliation Start Level Addition failed, please try again"
            End If
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();ClosePopUpStart();", True)
        End Try
    End Sub

    'Protected Sub btnEndSave_Click(sender As Object, e As EventArgs)
    '    Try
    '        If ddl_TankNo.SelectedIndex = 0 Then
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Please select TANK number"
    '            ddl_TankNo.Focus()
    '            Return
    '        ElseIf DDL_Customer.SelectedValue.ToString() = "0" Then
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Please Select Company"
    '            DDL_Customer.Focus()
    '            Return
    '        Else
    '            'OBJMaster = New MasterBAL
    '            'Dim dtTankCheck As DataTable = New DataTable()
    '            'dtTankCheck = OBJMaster.GetSiteByCondition(" and h.TankNumber = " + txtTankNo.Text + " and c.CustomerId = " + DDL_Customer.SelectedValue.ToString(), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
    '            'If dtTankCheck IsNot Nothing Then
    '            '    If dtTankCheck.Rows.Count = 0 Then
    '            '        ErrorMessage.Visible = True
    '            '        ErrorMessage.Text = "Tank number not found for selected Company. Please check and enter again."
    '            '        txtTankNo.Focus()
    '            '        Return
    '            '    End If
    '            'End If
    '        End If

    '        OBJMaster = New MasterBAL
    '        Dim dtTank As DataTable = New DataTable()
    '        Dim TankInventoryId As Integer = 0
    '        If hdnTankInventory.Value <> "" Then
    '            TankInventoryId = Convert.ToInt32(hdnTankInventory.Value)
    '        End If
    '        Dim InventoryDateTime As DateTime = Request.Form(txtEndDate.UniqueID) & " " & Request.Form(txtEndTime.UniqueID)
    '        Dim condition = ""
    '        condition = " and TankInventory.TankNumber = '" + ddl_TankNo.SelectedValue.ToString() + "' and TankInventoryId <> " + TankInventoryId.ToString() + " and DateType = 'e' and ENTRY_TYPE = 'Level' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " and InventoryDateTime = '" + InventoryDateTime + "' "
    '        dtTank = OBJMaster.GetTankInventorybyConditions(condition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
    '        If dtTank IsNot Nothing Then
    '            If dtTank.Rows.Count > 0 Then
    '                ErrorMessage.Visible = True
    '                ErrorMessage.Text = "End Level already save for Tank " + ddl_TankNo.SelectedValue.ToString() + " and Date and Time " + InventoryDateTime.ToString("MM/dd/yyyy hh:mm:tt")
    '                ErrorMessage.Focus()
    '                Return
    '            End If
    '        End If


    '        Dim result = OBJMaster.SaveUpdateTankInventory(TankInventoryId, ddl_TankNo.SelectedValue.ToString(), "Level", InventoryDateTime, Convert.ToDecimal(txtEndLevelQuan.Text), "e", Convert.ToInt32(DDL_Customer.SelectedValue), Convert.ToInt32(Session("PersonId").ToString()), DateTime.Now, "", DateTime.Now, 0.0, 0, "", "Manual")

    '        If result > 0 Then
    '            If (hdnTankInventory.Value <> "") Then
    '                message.Visible = True
    '                message.Text = "End Level Record saved"

    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    Dim writtenData As String = CreateData(result, 2)
    '                    CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
    '                End If

    '            Else
    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    Dim writtenData As String = CreateData(result, 2)
    '                    CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
    '                End If
    '                message.Visible = True
    '                message.Text = "End Level Record saved"
    '                txtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
    '                txtEndTime.Text = DateTime.Now.ToString("hh:mm tt")
    '                txtEndLevelQuan.Text = "0"
    '            End If
    '            ErrorMessage.Visible = False
    '            ErrorMessage.Text = ""
    '            ddl_TankNo.Focus()
    '            Response.Redirect("TankInventoryReconciliation?TankinventoryId=" & result.ToString() + "&Type=" + hdnEntryType.Value & "&RecordIs=New", False)
    '        Else
    '            If (hdnTankInventory.Value <> "") Then
    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 2)
    '                    CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation update failed")
    '                End If

    '                ErrorMessage.Visible = True
    '                ErrorMessage.Text = "End Level Save failed, please try again"
    '            Else
    '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                    Dim writtenData As String = CreateData(result, 2)
    '                    CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation Addition failed")
    '                End If

    '                ErrorMessage.Visible = True
    '                ErrorMessage.Text = "End Level Save failed, please try again"
    '            End If
    '            ddl_TankNo.Focus()
    '        End If
    '    Catch ex As Exception
    '        log.Error("Error occurred in btnEndSave_Click Exception is :" + ex.Message)
    '        If (hdnTankInventory.Value <> "") Then
    '            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 2)
    '                CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation update failed. Exception is : " & ex.Message)
    '            End If
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Tank Inventory Reconciliation End Level update failed, please try again"
    '        Else
    '            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 2)
    '                CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation Addition failed. Exception is : " & ex.Message)
    '            End If
    '            ErrorMessage.Visible = True
    '            ErrorMessage.Text = "Tank Inventory Reconciliation End Level Addition failed, please try again"
    '        End If
    '    Finally
    '        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();ClosePopUpEnd();", True)
    '    End Try
    'End Sub

    Protected Sub btnDELISave_Click(sender As Object, e As EventArgs)
        Try
            If ddl_TankNo.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter TANK number"
                ddl_TankNo.Focus()
                Return
            ElseIf DDL_Customer.SelectedValue.ToString() = "0" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please Select Company"
                DDL_Customer.Focus()
                Return
            Else
                'OBJMaster = New MasterBAL
                'Dim dtTankCheck As DataTable = New DataTable()
                'dtTankCheck = OBJMaster.GetSiteByCondition(" and h.TankNumber = " + txtTankNo.Text + " and c.CustomerId = " + DDL_Customer.SelectedValue.ToString(), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                'If dtTankCheck IsNot Nothing Then
                '    If dtTankCheck.Rows.Count = 0 Then
                '        ErrorMessage.Visible = True
                '        ErrorMessage.Text = "Tank number not found for selected Company. Please check and enter again."
                '        txtTankNo.Focus()
                '        Return
                '    End If
                'End If
            End If

            If txtDELILevelQuan.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Receipt/Delivery Tank Level Quantity and try again."
                txtDELILevelQuan.Focus()
                Return
            End If

            Dim InventoryDateTime As DateTime
            Try
                InventoryDateTime = Request.Form(txtDELIStartDate.UniqueID) & " " & Request.Form(txtDELIStartTime.UniqueID)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select valid Receipt/Delivery Date Date/time and try again"
                Return
            End Try

            Dim resultDecimalQ As Decimal = 0

            If (txtDELILevelQuan.Text <> "" And Not (Decimal.TryParse(txtDELILevelQuan.Text, resultDecimalQ))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Enter Receipt/Delivery Tank Level Quantity as decimal and try again."
                txtDELILevelQuan.Focus()
                Return
            End If

            OBJMaster = New MasterBAL
            Dim dtTank As DataTable = New DataTable()
            Dim TankInventoryId As Integer = 0
            If hdnTankInventory.Value <> "" Then
                TankInventoryId = Convert.ToInt32(hdnTankInventory.Value)
            End If

            Dim condition = ""
            condition = " and TankInventory.TankNumber = '" + ddl_TankNo.SelectedValue.ToString() + "' and TankInventoryId <> " + TankInventoryId.ToString() + " and DateType = 'se' and ENTRY_TYPE = 'RD' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " and InventoryDateTime = '" + InventoryDateTime + "' "
            dtTank = OBJMaster.GetTankInventorybyConditions(condition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            If dtTank IsNot Nothing Then
                If dtTank.Rows.Count > 0 Then
                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "RECEIPT/DELIVERY already save for Tank " + ddl_TankNo.SelectedValue.ToString() + " and Start Date Time is " + InventoryDateTime.ToString("MM/dd/yyyy hh:mm:tt") + ". "
                    ErrorMessage.Focus()
                    Return
                End If
            End If

            If Session("CostingMethodForDelivery") <> 1 Then
                If txtProductPrice.Text = "" Then
                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Please enter Product Price and try again."
                    txtProductPrice.Focus()
                    Return
                End If

                Dim resultDecimal As Decimal = 0

                If (txtProductPrice.Text <> "" And Not (Decimal.TryParse(txtProductPrice.Text, resultDecimal))) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Please enter Enter Receipt/Delivery Product Price as decimal and try again."
                    txtProductPrice.Focus()
                    Return
                End If

                Session("CostingMethodForDelivery") = txtProductPrice.Text

                If Convert.ToDecimal(txtProductPrice.Text) = 0 Then
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckConfirmDeliWarning();LoadDateTimeControl();", True)
                Else
                    SaveDeliveryData(InventoryDateTime)
                End If
            Else
                If txtProductPrice.Text = "" Then
                    Session("CostingMethodForDelivery") = "0"
                    SaveDeliveryData(InventoryDateTime)
                Else
                    Dim resultDecimal As Decimal = 0

                    If (txtProductPrice.Text <> "" And Not (Decimal.TryParse(txtProductPrice.Text, resultDecimal))) Then
                        ErrorMessage.Visible = True
                        ErrorMessage.Text = "Please enter Enter Receipt/Delivery Product Price as decimal and try again."
                        txtProductPrice.Focus()
                        Return
                    End If
                    Session("CostingMethodForDelivery") = txtProductPrice.Text
                    SaveDeliveryData(InventoryDateTime)
                End If
            End If


            ' Dim InventoryEndDateTime As DateTime = Request.Form(txtDELIEndDate.UniqueID) & " " & Request.Form(txtDELIEndTime.UniqueID)

            'If InventoryEndDateTime < InventoryDateTime Then
            '    ErrorMessage.Visible = True
            '    ErrorMessage.Text = "Please select End date/time higer than Start date/time"
            '    txtDELIStartDate.Focus()
            '    Return
            'End If



        Catch ex As Exception
            log.Error("Error occurred in btnDELISave_Click Exception is :" + ex.Message)
            If (hdnTankInventory.Value <> "") Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 3)
                    CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation update failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Tank Inventory Reconciliation RECEIPT/DELIVERY update failed, please try again"
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 3)
                    CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation Addition failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Tank Inventory Reconciliation RECEIPT/DELIVERY Addition failed, please try again"
            End If
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();ClosePopUpEnd();", True)
        End Try
    End Sub

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

    Protected Sub btnCancelDELIStart_Click(sender As Object, e As EventArgs)
        Try

        Catch ex As Exception
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnCancelDELIEnd_Click(sender As Object, e As EventArgs)
        Try

        Catch ex As Exception
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnMainCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("/Master/AllTankInventoryReconciliation" + "?Type=" + hdnEntryType.Value)
    End Sub

    Protected Sub BindAllGrid()
        Try
            'Bind Start data
            OBJMaster = New MasterBAL()
            Dim dtStart As DataTable = New DataTable()
            dtStart = OBJMaster.GetTankInventorybyConditions(" and TankInventory.TankNumber = '" + ddl_TankNo.SelectedValue.ToString() + "' and DateType = 's' and ENTRY_TYPE = 'Level' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            If dtStart IsNot Nothing Then
                If dtStart.Rows.Count > 0 Then
                    gv_StartLevel.DataSource = dtStart
                    gv_StartLevel.DataBind()
                Else
                    gv_StartLevel.DataSource = Nothing
                    gv_StartLevel.DataBind()
                End If
            Else
                gv_StartLevel.DataSource = Nothing
                gv_StartLevel.DataBind()
            End If

            ''Bind End data
            'OBJMaster = New MasterBAL()
            'Dim dtEnd As DataTable = New DataTable()
            'dtEnd = OBJMaster.GetTankInventorybyConditions(" and TankInventory.TankNumber = '" + ddl_TankNo.SelectedValue.ToString() + "' and DateType = 'e' and ENTRY_TYPE = 'Level' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            'If dtEnd IsNot Nothing Then
            '    If dtEnd.Rows.Count > 0 Then
            '        gv_EndLevel.DataSource = dtEnd
            '        gv_EndLevel.DataBind()
            '    Else
            '        gv_EndLevel.DataSource = Nothing
            '        gv_EndLevel.DataBind()
            '    End If
            'Else
            '    gv_EndLevel.DataSource = Nothing
            '    gv_EndLevel.DataBind()
            'End If

            'Bind Deli Start data
            OBJMaster = New MasterBAL()
            Dim dtDeliStart As DataTable = New DataTable()
            dtDeliStart = OBJMaster.GetTankInventorybyConditions(" and TankInventory.TankNumber = '" + ddl_TankNo.SelectedValue.ToString() + "' and ENTRY_TYPE = 'Level' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            If dtDeliStart IsNot Nothing Then
                If dtDeliStart.Rows.Count > 0 Then
                    gv_DELIStartLevel.DataSource = dtDeliStart
                    gv_DELIStartLevel.DataBind()
                Else
                    gv_DELIStartLevel.DataSource = Nothing
                    gv_DELIStartLevel.DataBind()
                End If
            Else
                gv_DELIStartLevel.DataSource = Nothing
                gv_DELIStartLevel.DataBind()
            End If


            'Bind Deli End data
            OBJMaster = New MasterBAL()
            Dim dtDeliEnd As DataTable = New DataTable()
            dtDeliEnd = OBJMaster.GetTankInventorybyConditions(" and TankInventory.TankNumber = '" + ddl_TankNo.SelectedValue.ToString() + "' and ENTRY_TYPE = 'Level' and CompanyId = " + DDL_Customer.SelectedValue.ToString() + " ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            If dtDeliEnd IsNot Nothing Then
                If dtDeliEnd.Rows.Count > 0 Then
                    gv_DELIEndLevel.DataSource = dtDeliEnd
                    gv_DELIEndLevel.DataBind()
                Else
                    gv_DELIEndLevel.DataSource = Nothing
                    gv_DELIEndLevel.DataBind()
                End If
            Else
                gv_DELIEndLevel.DataSource = Nothing
                gv_DELIEndLevel.DataBind()
            End If

        Catch ex As Exception
            log.Error("Error occurred in BindAllGrid Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting BindAllGrid, please try again later."
        End Try
    End Sub

    Protected Sub BTN_StartType_Click(sender As Object, e As EventArgs)
        Try
            lblStartLevelTankNumber.Text = "Tank Number: " + ddl_TankNo.SelectedValue.ToString()
            BindAllGrid()

        Catch ex As Exception
            log.Error("Error occurred in BTN_StartType_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "$('#StartDatePicker').modal({show: true,backdrop: 'static',keyboard: false});", True)
        End Try
    End Sub

    'Protected Sub BTN_EndType_Click(sender As Object, e As EventArgs)
    '    lblEndLevelTankNumber.Text = "Tank Number: " + ddl_TankNo.SelectedValue.ToString()
    '    BindAllGrid()
    '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "$('#EndDatePicker').modal({show: true,backdrop: 'static',keyboard: false});", True)
    'End Sub

    Protected Sub BTN_DeliStartType_Click(sender As Object, e As EventArgs)
        Try

            lblDELIStartLevelTankNumber.Text = "Tank Number: " + ddl_TankNo.SelectedValue.ToString()
            BindAllGrid()

        Catch ex As Exception
            log.Error("Error occurred in BTN_DeliStartType_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "$('#DELIStartDatePicker').modal({show: true,backdrop: 'static',keyboard: false});", True)
        End Try
    End Sub

    Protected Sub BTN_DeliEndType_Click(sender As Object, e As EventArgs)
        Try

            lblDELIEndLevelTankNumber.Text = "Tank Number: " + ddl_TankNo.SelectedValue.ToString()
            BindAllGrid()
        Catch ex As Exception
            log.Error("Error occurred in BTN_DeliStartType_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "$('#DELIEndDatePicker').modal({show: true,backdrop: 'static',keyboard: false});", True)
        End Try
    End Sub

    Protected Sub bindTankInventoryData()
        Try
            OBJMaster = New MasterBAL
            Dim dtTank As DataTable = New DataTable()
            dtTank = OBJMaster.GetTankInventorybyId(Convert.ToInt32(hdnTankInventory.Value))
            If dtTank IsNot Nothing Then
                If dtTank.Rows.Count > 0 Then
                    If (Not Session("RoleName") = "SuperAdmin") Then

                        Dim dtCust As DataTable = New DataTable()

                        dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                        If (dtCust.Rows(0)("CustomerId").ToString() <> dtTank.Rows(0)("CompanyId").ToString()) Then

                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                            Return
                        End If

                    End If


                    ddl_TankNo.SelectedValue = dtTank.Rows(0)("TankNumber").ToString()
                    DDL_Customer.SelectedValue = dtTank.Rows(0)("CompanyId").ToString()
                    BindTanks(DDL_Customer.SelectedValue)
                    If dtTank.Rows(0)("DateType") = "s" Then
                        StartDiv.Visible = True
                        'EndDiv.Visible = False
                        DeliDiv.Visible = False

                        txtStartDate.Text = Convert.ToDateTime(dtTank.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy")
                        txtStartTime.Text = Convert.ToDateTime(dtTank.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt")
                        txtStartLevelQuan.Text = dtTank.Rows(0)("Quantity").ToString()

                        beforeData = "TankInventoryId = " & hdnTankInventory.Value & " ; " &
                                     "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                     "Tank Number = " & ddl_TankNo.SelectedValue.ToString().Replace(",", " ") & " ; " &
                                     "Entry Type = Level " & " ; " &
                                     "Date Type = Start" & " ; " &
                                     "Start Date = " & Convert.ToDateTime(dtTank.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy").Replace(",", " ") & " ; " &
                                     "Start Time = " & Convert.ToDateTime(dtTank.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt").Replace(",", " ") & " ; " &
                                     "Starting Tank Level Quantity = " & txtStartLevelQuan.Text.Replace(",", " ") & " "
                        'ElseIf dtTank.Rows(0)("DateType") = "e" Then
                        '    StartDiv.Visible = False
                        '    EndDiv.Visible = True
                        '    DeliDiv.Visible = False

                        '    txtEndDate.Text = Convert.ToDateTime(dtTank.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy")
                        '    txtEndTime.Text = Convert.ToDateTime(dtTank.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt")
                        '    txtEndLevelQuan.Text = dtTank.Rows(0)("Quantity").ToString()

                        '    beforeData = "TankInventoryId = " & hdnTankInventory.Value & " ; " &
                        '                 "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                        '                 "Tank Number = " & ddl_TankNo.SelectedValue.ToString().Replace(",", " ") & " ; " &
                        '                 "Entry Type = Level " & " ; " &
                        '                 "Date Type = End" & " ; " &
                        '                 "End Date = " & Convert.ToDateTime(dtTank.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy").Replace(",", " ") & " ; " &
                        '                 "End Time = " & Convert.ToDateTime(dtTank.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt").Replace(",", " ") & " ; " &
                        '                 "Enter Ending Tank Level Quantity = " & txtEndLevelQuan.Text.Replace(",", " ") & " "
                    ElseIf dtTank.Rows(0)("DateType") = "se" Then
                        StartDiv.Visible = False
                        'EndDiv.Visible = False
                        DeliDiv.Visible = True

                        txtDELIStartDate.Text = Convert.ToDateTime(dtTank.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy")
                        txtDELIStartTime.Text = Convert.ToDateTime(dtTank.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt")
                        'txtDELIEndDate.Text = Convert.ToDateTime(dtTank.Rows(0)("EndDateForRD").ToString()).ToString("MM/dd/yyyy")
                        'txtDELIEndTime.Text = Convert.ToDateTime(dtTank.Rows(0)("EndTimeForRD").ToString()).ToString("hh:mm tt")
                        txtDELILevelQuan.Text = dtTank.Rows(0)("Quantity").ToString()
                        txtProductPrice.Text = dtTank.Rows(0)("Price").ToString()

                        beforeData = "TankInventoryId = " & hdnTankInventory.Value & " ; " &
                                     "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                     "Tank Number = " & ddl_TankNo.SelectedValue.ToString().Replace(",", " ") & " ; " &
                                     "Entry Type = RECEIPT/DELIVERY " & " ; " &
                                     "Date Type = Start and End" & " ; " &
                                     "Start Date = " & Convert.ToDateTime(dtTank.Rows(0)("InventoryDate").ToString()).ToString("MM/dd/yyyy").Replace(",", " ") & " ; " &
                                     "Start Time = " & Convert.ToDateTime(dtTank.Rows(0)("InventoryTime").ToString()).ToString("hh:mm tt").Replace(",", " ") & " ; " &
                                     "End Date = " & Convert.ToDateTime(dtTank.Rows(0)("EndDateForRD").ToString()).ToString("MM/dd/yyyy").Replace(",", " ") & " ; " &
                                     "End Time = " & Convert.ToDateTime(dtTank.Rows(0)("EndTimeForRD").ToString()).ToString("hh:mm tt").Replace(",", " ") & " ; " &
                                     "Price = " & txtProductPrice.Text.Replace(",", " ") & " ; " &
                                     "Receipt/Delivery Tank Level Quantity = " & txtDELILevelQuan.Text.Replace(",", " ") & " "
                    End If
                End If
            End If
        Catch ex As Exception
            log.Error("Error occurred in bindTankInventoryData Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting bindTankInventoryData, please try again later."
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

    Protected Sub btnDELIStartOk_Click(sender As Object, e As EventArgs)
        Try
            For Each gv_DELILevelData As GridViewRow In gv_DELIStartLevel.Rows
                Dim Rdb_DELILevel As RadioButton = TryCast(gv_DELILevelData.FindControl("Rdb_DELILevel"), RadioButton)
                If Rdb_DELILevel.Checked Then
                    txtDELIStartDate.Text = gv_DELIStartLevel.Rows(gv_DELILevelData.RowIndex).Cells(1).Text
                    txtDELIStartTime.Text = gv_DELIStartLevel.Rows(gv_DELILevelData.RowIndex).Cells(2).Text
                    'txtDELILevelQuan.Text = gv_DELIStartLevel.Rows(gv_DELILevelData.RowIndex).Cells(3).Text
                    txtDELIStartDate.Focus()
                End If
            Next
        Catch ex As Exception
            log.Error("Error occurred in btnDELIOk_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting btnDELIStartOk_Click, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnDELIEndOk_Click(sender As Object, e As EventArgs)
        Try
            For Each gv_DELILevelData As GridViewRow In gv_DELIEndLevel.Rows
                Dim Rdb_DELILevel As RadioButton = TryCast(gv_DELILevelData.FindControl("Rdb_DELILevel"), RadioButton)
                If Rdb_DELILevel.Checked Then
                    'txtDELIEndDate.Text = gv_DELIEndLevel.Rows(gv_DELILevelData.RowIndex).Cells(1).Text
                    'txtDELIEndTime.Text = gv_DELIEndLevel.Rows(gv_DELILevelData.RowIndex).Cells(2).Text
                    'txtDELILevelQuan.Text = gv_DELIEndLevel.Rows(gv_DELILevelData.RowIndex).Cells(3).Text
                    'txtDELIEndDate.Focus()
                End If
            Next
        Catch ex As Exception
            log.Error("Error occurred in btnDELIOk_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting btnDELIEndOk_Click, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Private Function CreateData(TankInventoryId As Integer, DateType As Integer) As String
        Try

            Dim data As String = ""
            If DateType = 1 Then
                data = "TankInventoryId = " & TankInventoryId & " ; " &
                       "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                       "Tank Number = " & ddl_TankNo.SelectedValue.ToString().Replace(",", " ") & " ; " &
                       "Entry Type = Level " & " ; " &
                       "Date Type = Start" & " ; " &
                       "Start Date = " & Request.Form(txtStartDate.UniqueID).Replace(",", " ") & " ; " &
                       "Start Time = " & Request.Form(txtStartTime.UniqueID).ToString().Replace(",", " ") & " ; " &
                       "Starting Tank Level Quantity = " & txtStartLevelQuan.Text.Replace(",", " ") & " "
                'ElseIf DateType = 2 Then
                '    data = "TankInventoryId = " & TankInventoryId & " ; " &
                '           "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                '           "Tank Number = " & ddl_TankNo.SelectedValue.ToString().Replace(",", " ") & " ; " &
                '           "Entry Type = Level " & " ; " &
                '           "Date Type = End" & " ; " &
                '           "End Date = " & Request.Form(txtEndDate.UniqueID).Replace(",", " ") & " ; " &
                '           "End Time = " & Request.Form(txtEndTime.UniqueID).ToString().Replace(",", " ") & " ; " &
                '           "Enter Ending Tank Level Quantity = " & txtEndLevelQuan.Text.Replace(",", " ") & " "
            Else
                data = "TankInventoryId = " & TankInventoryId & " ; " &
                      "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                      "Tank Number = " & ddl_TankNo.SelectedValue.ToString().Replace(",", " ") & " ; " &
                      "Entry Type = RECEIPT/DELIVERY " & " ; " &
                      "Date Type = Start and End" & " ; " &
                      "Start Date = " & Request.Form(txtDELIStartDate.UniqueID).Replace(",", " ") & " ; " &
                      "Start Time = " & Request.Form(txtDELIStartTime.UniqueID).ToString().Replace(",", " ") & " ; " &
                      "Receipt/Delivery Tank Level Quantity = " & txtDELILevelQuan.Text.Replace(",", " ") & " ; " &
                      "Price = " & txtProductPrice.Text.Replace(",", " ") & " "

                '"End Date = " & Request.Form(txtDELIEndDate.UniqueID).Replace(",", " ") & " ; " &
                '"End Time = " & Request.Form(txtDELIEndTime.UniqueID).ToString().Replace(",", " ") & " ; " &
            End If



            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)

        Try
            BindTanks(DDL_Customer.SelectedValue)

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()

            'If DDL_Customer.SelectedIndex <> 0 Then
            dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), DDL_Customer.SelectedValue)
            Session("CostingMethodForDelivery") = dtCust.Rows(0)("CostingMethod").ToString()

            '    If dtCust.Rows(0)("CostingMethod").ToString() = "1" Then
            '        DeliDiv.Visible = False
            '    Else
            '        DeliDiv.Visible = True
            '    End If
            'ElseIf Session("RoleName") = "SuperAdmin" Then
            '    DeliDiv.Visible = True
            'ElseIf Session("RoleName") <> "SuperAdmin" Then
            '    dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
            '    If dtCust.Rows(0)("CostingMethod").ToString() = "1" Then
            '        DeliDiv.Visible = False
            '    Else
            '        DeliDiv.Visible = True
            '    End If
            'End If

        Catch ex As Exception
            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting data, please try again later."
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
            ErrorMessage.Text = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub SaveDeliveryData(InventoryDateTime As DateTime)
        Try
            OBJMaster = New MasterBAL
            Dim dtTank As DataTable = New DataTable()
            Dim TankInventoryId As Integer = 0
            If hdnTankInventory.Value <> "" Then
                TankInventoryId = Convert.ToInt32(hdnTankInventory.Value)
            End If

            Dim result = OBJMaster.SaveUpdateTankInventory(TankInventoryId, ddl_TankNo.SelectedValue.ToString(), "RD", InventoryDateTime, Convert.ToDecimal(txtDELILevelQuan.Text), "se", Convert.ToInt32(DDL_Customer.SelectedValue), Convert.ToInt32(Session("PersonId").ToString()), DateTime.Now, "", DateTime.Now, 0.0, 0, "", "Manual", Convert.ToDecimal(Session("CostingMethodForDelivery").ToString()))

            If result > 0 Then
                If (hdnTankInventory.Value <> "") Then
                    message.Visible = True
                    message.Text = "RECEIPT/DELIVERY Record saved"

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 3)
                        CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 3)
                        CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    message.Visible = True
                    message.Text = "RECEIPT/DELIVERY Record saved"
                    txtDELIStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                    txtDELIStartTime.Text = DateTime.Now.ToString("hh:mm tt")
                    'txtDELIEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                    'txtDELIEndTime.Text = DateTime.Now.ToString("hh:mm tt")
                    txtDELILevelQuan.Text = "0"
                End If
                ErrorMessage.Visible = False
                ErrorMessage.Text = ""
                ddl_TankNo.Focus()

                If TankInventoryId <> 0 Then
                    Response.Redirect("TankInventoryReconciliation?TankinventoryId=" & result.ToString() + "&Type=" + hdnEntryType.Value & "&RecordIs=New", False)
                Else
                    Response.Redirect("TankInventoryReconciliation?Type=" + hdnEntryType.Value & "&RecordIs=New&TankNumber=" & ddl_TankNo.SelectedValue.ToString() & "&CustomerID=" & DDL_Customer.SelectedValue.ToString(), False)
                End If


            Else
                If (hdnTankInventory.Value <> "") Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 3)
                        CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation update failed")
                    End If

                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "RECEIPT/DELIVERY Save failed, please try again"
                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, 3)
                        CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation Addition failed")
                    End If

                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "RECEIPT/DELIVERY Save failed, please try again"
                End If
                ddl_TankNo.Focus()
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnDELISave_Click Exception is :" + ex.Message)
            If (hdnTankInventory.Value <> "") Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 3)
                    CSCommonHelper.WriteLog("Modified", "TankInventoryReconciliation", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation update failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Tank Inventory Reconciliation RECEIPT/DELIVERY update failed, please try again"
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(Convert.ToInt32(hdnTankInventory.Value), 3)
                    CSCommonHelper.WriteLog("Added", "TankInventoryReconciliation", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "TankInventoryReconciliation Addition failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Tank Inventory Reconciliation RECEIPT/DELIVERY Addition failed, please try again"
            End If
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();ClosePopUpEnd();", True)
        End Try
    End Sub

    Protected Sub btnSaveDelivery_Click(sender As Object, e As EventArgs)
        Try
            Dim InventoryDateTime As DateTime
            Try
                InventoryDateTime = Request.Form(txtDELIStartDate.UniqueID) & " " & Request.Form(txtDELIStartTime.UniqueID)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select valid Receipt/Delivery Date Date/time and try again"
                Return
            End Try
            SaveDeliveryData(InventoryDateTime)
        Catch ex As Exception
            log.Error("Error occurred in btnSaveDelivery_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Tank Inventory Reconciliation RECEIPT/DELIVERY Addition failed, please try again"
        End Try
    End Sub
End Class
