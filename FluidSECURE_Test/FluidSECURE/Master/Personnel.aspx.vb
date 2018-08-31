
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Owin
Imports Microsoft.AspNet.Identity.EntityFramework
Imports log4net
Imports log4net.Config
Imports System.Web.Services
Imports System.IO
Imports System.Net.Mail
Imports System.Net

Public Class Personnel
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Personnel))

    Dim OBJMaster As MasterBAL
    Shared beforeData As String
    Shared beforeVehicles As String
    Shared afterVehicles As String
    Shared beforeFSlinks As String
    Shared afterFSlinks As String
    Shared beforefuelTimings As String
    Shared afterfuelTimings As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False


            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
                'Access denied
                Response.Redirect("/home")

            Else
                If (Session("RoleName") = "SuperAdmin") Then
                    divCollectDiagnosticLogs.Visible = True
                Else
                    divCollectDiagnosticLogs.Visible = False
                End If

                If (Not IsPostBack) Then
                    Session("ActiveInActive") = Nothing
                    BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                    BindTiming()
                    BindAccessLevles()

                    txtIMEINumber.Text = ""
                    If (Not Request.QueryString("PersonId") = Nothing And Not Request.QueryString("PersonId") = "") Then
                        VehicleHide.Visible = True
                        VehicleHide1.Visible = True
                        HDF_PersonnelId.Value = Request.QueryString("PersonId")
                        HDF_UniqueUserId.Value = Request.QueryString("UniqueUserId")
                        BindPersonnelDetails(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))

                        lblHeader.Text = "Edit Personnel Information"
                        PWDLabel.Visible = False
                        PWDTextbox.Visible = False
                        CPWDLabel.Visible = False
                        CPWDTextbox.Visible = False
                        PWDHide.Visible = True
                        CPWDHide.Visible = True
                        CPWDHide1.Visible = True
                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"

                            CheckValidMapping()

                        End If

                        btnFirst.Visible = True
                        btnNext.Visible = True
                        btnprevious.Visible = True
                        btnLast.Visible = True
                        ChangePWDLbl.Visible = True
                        ChangePWDCHK.Visible = True
                        divIMEI.Visible = True
                        divIMEIAdjustment.Visible = False
                    Else
                        VehicleHide.Visible = True
                        VehicleHide1.Visible = True
                        btnFirst.Visible = False
                        btnNext.Visible = False
                        btnprevious.Visible = False
                        btnLast.Visible = False
                        lblof.Visible = False
                        PWDHide.Visible = False
                        CPWDHide.Visible = False
                        CPWDHide1.Visible = False
                        lblHeader.Text = "Add Personnel Information"

                        BindDepartment(0)
                        lblPreAuthNotUsed.Text = "0"
                        lblPreAuthUsed.Text = "0"
                        ChangePWDLbl.Visible = False
                        ChangePWDCHK.Visible = False
                        divIMEI.Visible = False
                        divIMEIAdjustment.Visible = True
                        DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                    End If

                    If Session("RoleName") <> "SuperAdmin" Then
                        UFLSLabel.Visible = False
                        UFLSCheckbox.Visible = False
                    End If

                    txtPersonName.Focus()

                End If
            End If


        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    'Private Sub BindVehicles(customerId As Integer)

    '    Try

    '        Dim dtVehicles As DataTable = New DataTable()
    '        OBJMaster = New MasterBAL()
    '        dtVehicles = OBJMaster.GetVehicleByCondition(" and v.CustomerId=" + customerId.ToString(), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

    '        gv_Vehicles.DataSource = dtVehicles
    '        gv_Vehicles.DataBind()


    '        If customerId = 0 Then
    '            lblVehicleMessage.Text = "Please select Company and then select vehicles."
    '            lblVehicleMessage.Visible = True
    '            gv_Vehicles.Visible = False
    '        ElseIf customerId <> 0 And dtVehicles.Rows.Count <> 0 Then

    '            lblVehicleMessage.Visible = False
    '            gv_Vehicles.Visible = True
    '        ElseIf customerId <> 0 And dtVehicles.Rows.Count = 0 Then
    '            lblVehicleMessage.Text = "Vehicles not found for selected Company."
    '            lblVehicleMessage.Visible = True
    '            gv_Vehicles.Visible = False
    '        End If


    '    Catch ex As Exception

    '        log.Error("Error occurred in BindVehicles Exception is :" + ex.Message)
    '        ErrorMessage.Visible = True
    '        ErrorMessage.InnerText = "Error occurred while getting vehicles, please try again later."

    '    End Try


    'End Sub

    Private Sub BindSites(CustomerId As Integer)

        Try

            Dim dtSites As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtSites = OBJMaster.GetSiteByCondition("And c.CustomerId =" + CustomerId.ToString(), Session("PersonId").ToString(), Session("RoleId").ToString(), False)

            gv_Sites.DataSource = dtSites
            gv_Sites.DataBind()

            If CustomerId = 0 Then
                lblSiteMessage.Text = "Please select Company."
                lblSiteMessage.Visible = True
                gv_Sites.Visible = False
            ElseIf CustomerId <> 0 And dtSites.Rows.Count <> 0 Then

                lblSiteMessage.Visible = False
                gv_Sites.Visible = True
            ElseIf CustomerId <> 0 And dtSites.Rows.Count = 0 Then
                lblSiteMessage.Text = "Site not found for selected customer."
                lblSiteMessage.Visible = True
                gv_Sites.Visible = False
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindSites Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting sites, please try again later."

        End Try
    End Sub

    'Private Sub BindVehiclesDataToCheckboxList(PersonId As Integer)
    '    Try
    '        beforeVehicles = ""
    '        OBJMaster = New MasterBAL()
    '        Dim dtPersonVehicleMapping As DataTable = New DataTable()

    '        dtPersonVehicleMapping = OBJMaster.GetPersonVehicleMapping(PersonId)

    '        For Each dr As DataRow In dtPersonVehicleMapping.Rows

    '            For Each rows As GridViewRow In gv_Vehicles.Rows
    '                If (dr("VehicleId") = gv_Vehicles.DataKeys(rows.RowIndex).Values("VehicleId").ToString()) Then
    '                    TryCast(rows.FindControl("CHK_Vehicle"), CheckBox).Checked = True
    '                End If
    '            Next

    '            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '                beforeVehicles = IIf(beforeVehicles = "", dr("VehicleNumber"), beforeVehicles & ";" & dr("VehicleNumber"))

    '            End If
    '        Next

    '    Catch ex As Exception

    '        log.Error("Error occurred in BindVehiclesDataToCheckboxList Exception is :" + ex.Message)
    '        ErrorMessage.Visible = True
    '        ErrorMessage.InnerText = "Error occurred while getting VehiclesDataToCheckboxList, please try again later."

    '    End Try

    'End Sub

    Private Sub BindSitesDataToCheckboxList(PersonId As Integer)
        Try
            beforeFSlinks = ""
            OBJMaster = New MasterBAL()
            Dim dtPersonSiteMapping As DataTable = New DataTable()

            dtPersonSiteMapping = OBJMaster.GetPersonSiteMapping(PersonId, 0)
            If dtPersonSiteMapping IsNot Nothing Then
                For Each dr As DataRow In dtPersonSiteMapping.Rows

                    For Each rows As GridViewRow In gv_Sites.Rows
                        If (dr("SiteID") = gv_Sites.DataKeys(rows.RowIndex).Values("SiteID").ToString()) Then
                            TryCast(rows.FindControl("CHK_PersonSite"), CheckBox).Checked = True
                        End If
                    Next

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        beforeFSlinks = IIf(beforeFSlinks = "", dr("WifiSSID"), beforeFSlinks & ";" & dr("WifiSSID"))
                    End If


                Next
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindSitesDataToCheckboxList Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting SitesDataToCheckboxList, please try again later."

        End Try

    End Sub

    Private Sub BindPersonnelDetails(PersonId As Integer, UniqueUserId As String)
        Try

            OBJMaster = New MasterBAL()
            Dim dtPersonnel As DataTable = New DataTable()
            Dim cnt As Integer = 0

            dtPersonnel = OBJMaster.GetPersonnelByPersonIdAndId(PersonId, UniqueUserId)

            If (dtPersonnel.Rows.Count > 0) Then

                If (Not Session("RoleName") = "SuperAdmin") Then

                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                    If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtPersonnel.Rows(0)("CustomerId").ToString()) Then

                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
                    End If

                    If (dtPersonnel.Rows(0)("RoleName").ToString().ToLower() = "SuperAdmin".ToLower()) Then
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
                    End If

                End If

                If (dtPersonnel.Rows(0)("IsFluidSecureHub").ToString() = True) Then

                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                    Return

                End If


                BindSites(dtPersonnel.Rows(0)("CustomerId").ToString())
                BindSitesDataToCheckboxList(PersonId)

                BindDepartment(Convert.ToInt32(dtPersonnel.Rows(0)("CustomerId").ToString()))
                BindPersonTimings(PersonId)

                txtPersonName.Text = dtPersonnel.Rows(0)("PersonName").ToString()
                txtEmail.Text = dtPersonnel.Rows(0)("Email").ToString()
                txtPhoneNumber.Text = dtPersonnel.Rows(0)("PhoneNumber").ToString()
                Try
                    DDL_Customer.SelectedValue = dtPersonnel.Rows(0)("CustomerId").ToString()

                Catch ex As Exception

                    DDL_Customer.SelectedValue = 0
                End Try

                If (Not Session("RoleName") = "SuperAdmin") Then
                    DDL_Customer.Enabled = False
                End If


                'BindVehicles(Convert.ToInt32(dtPersonnel.Rows(0)("CustomerId").ToString()))
                'BindVehiclesDataToCheckboxList(PersonId)

                DDL_Department.SelectedValue = dtPersonnel.Rows(0)("DepartmentId").ToString()
                DDL_DepartmentNumber.SelectedValue = dtPersonnel.Rows(0)("DepartmentId").ToString()
                txtFuelLimitPertxtn.Text = dtPersonnel.Rows(0)("FuelLimitPerTxn").ToString()
                txtFuelLimitPerDay.Text = dtPersonnel.Rows(0)("FuelLimitPerDay").ToString()
                txtPreAuth.Text = dtPersonnel.Rows(0)("PreAuth").ToString()
                chkSoftUpdate.Checked = dtPersonnel.Rows(0)("SoftUpdate").ToString()
                Try
                    DDL_AccessLevels.SelectedValue = dtPersonnel.Rows(0)("RoleId").ToString()
                Catch ex As Exception

                End Try
                Dim roleName As String = RoleActions.GetRolesById(dtPersonnel.Rows(0)("RoleId").ToString()).Name
                If (roleName = "SuperAdmin") Then
                    DDL_AccessLevels.Visible = False
                    AccessLevels.Visible = False
                    AccessLevelsShow.Visible = True
                Else
                    DDL_AccessLevels.Visible = True
                    AccessLevels.Visible = True
                    AccessLevelsShow.Visible = False
                End If

                txtExportCode.Text = dtPersonnel.Rows(0)("ExportCode").ToString()
                chkIsApproved.Checked = dtPersonnel.Rows(0)("IsApproved").ToString()
                txtPinNumber.Text = dtPersonnel.Rows(0)("PinNumber").ToString()
                txtIMEINumber.Text = dtPersonnel.Rows(0)("IMEI_UDID").ToString()
                CHK_SendTransactionEmail.Checked = dtPersonnel.Rows(0)("SendTransactionEmail").ToString()
                If CHK_SendTransactionEmail.Checked Then
                    divAdditionalEmail.Visible = True
                Else
                    divAdditionalEmail.Visible = False
                End If
                HDF_PreviousPreAuthCount.Value = dtPersonnel.Rows(0)("PreAuth").ToString()
                CHK_HubUser.Checked = dtPersonnel.Rows(0)("IsUserForHub").ToString()
                txtAdditionalEmail.Text = dtPersonnel.Rows(0)("AdditionalEmailId").ToString()

                chk_CollectDiagnosticLogs.Checked = dtPersonnel.Rows(0)("CollectDiagnosticLogs").ToString()

                CHK_HubUser_CheckedChanged(Nothing, Nothing)

                TXT_FoBNUM.Text = dtPersonnel.Rows(0)("FOBNumber").ToString()

                lblTearmsAndPolicyAccepted.Text = dtPersonnel.Rows(0)("IsTermConditionAgreed").ToString()
                lblDateOfAcceptance.Text = dtPersonnel.Rows(0)("DateTimeTermConditionAccepted").ToString()

                Dim dtPreAuthTrans As DataTable = New DataTable()

                dtPreAuthTrans = OBJMaster.GetPreAuthTransactionsByPersonId(PersonId, False)

                Session("dtPreAuthTrans") = dtPreAuthTrans

                Dim dvNotUsedPreAuth As DataView = dtPreAuthTrans.DefaultView
                dvNotUsedPreAuth.RowFilter = "PreAuthStatus='N'"

                lblPreAuthNotUsed.Text = dvNotUsedPreAuth.ToTable().Rows.Count

                dvNotUsedPreAuth = dtPreAuthTrans.DefaultView
                dvNotUsedPreAuth.RowFilter = "PreAuthStatus='U'"

                lblPreAuthUsed.Text = dvNotUsedPreAuth.ToTable().Rows.Count

                Dim strConditions As String = ""
                If (Not Session("PerConditions") Is Nothing) Then
                    strConditions = Session("PerConditions")
                Else
                    If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                        strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=0 and CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=0 and CustomerId=" & Session("CustomerId"))
                    End If
                End If

                Dim dt As DataTable = OBJMaster.GetPersonIdByCondition(PersonId, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), False, strConditions.Replace("ANU.", ""))
                HDF_TotalPersonnel.Value = dt.Rows(0)("TotalPersonId")

                OBJMaster = New MasterBAL()
                Dim dtAllPersonnel As DataTable = New DataTable()

                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"))
                End If

                dtAllPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                dtAllPersonnel.PrimaryKey = New DataColumn() {dtAllPersonnel.Columns(0)}
                Dim dr As DataRow = dtAllPersonnel.Rows.Find(UniqueUserId)
                If Not IsDBNull(dr) Then

                    cnt = dtAllPersonnel.Rows.IndexOf(dr) + 1

                End If
                If (cnt >= HDF_TotalPersonnel.Value) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                ElseIf (cnt <= 1) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt > 1 And cnt < HDF_TotalPersonnel.Value) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                End If
                lblof.Text = cnt & " of " & HDF_TotalPersonnel.Value.ToString()



            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Data Not found. Please try again after some time."
            End If

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                beforeData = CreateBeforeData(dtPersonnel)
            End If


        Catch ex As Exception

            log.Error("Error occurred in BindPersonnelDetails Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting personnel data, please try again later."
        Finally
            txtPersonName.Focus()
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Private Sub BindPersonTimings(PersonId As Integer)
        Try
            beforefuelTimings = ""
            OBJMaster = New MasterBAL()
            Dim dtPersonTimings As DataTable = New DataTable()

            dtPersonTimings = OBJMaster.GetPersonnelTimings(PersonId)
            ViewState("dtTimings") = dtPersonTimings

            If dtPersonTimings IsNot Nothing Then
                For Each dr As DataRow In dtPersonTimings.Rows

                    For Each rows As GridViewRow In gv_FuelingTimes.Rows
                        If (dr("TimeId") = gv_FuelingTimes.DataKeys(rows.RowIndex).Values("TimeId").ToString()) Then
                            TryCast(rows.FindControl("CHK_FuelingTimes"), CheckBox).Checked = True
                        End If
                    Next
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        beforefuelTimings = IIf(beforefuelTimings = "", dr("TimeText"), beforefuelTimings & ";" & dr("TimeText"))
                    End If

                Next
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindPersonTimings Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting personnel timings, please try again later."

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
                DDL_Department.DataSource = dtDept
                DDL_DepartmentNumber.DataSource = dtDept
            Else
                DDL_Department.DataSource = dtDept
                DDL_DepartmentNumber.DataSource = dtDept
            End If

            DDL_Department.DataTextField = "NAME"
            DDL_Department.DataValueField = "DeptId"

            DDL_Department.DataBind()
            DDL_Department.Items.Insert(0, New ListItem("Select Department", "0"))


            DDL_DepartmentNumber.DataTextField = "NUMBER"
            DDL_DepartmentNumber.DataValueField = "DeptId"

            DDL_DepartmentNumber.DataBind()
            DDL_DepartmentNumber.Items.Insert(0, New ListItem("Select Department Number", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindDepartment Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting departments, please try again later."

        End Try
    End Sub

    Private Sub BindAccessLevles()
        Try

            OBJMaster = New MasterBAL()

            ''get role
            Dim PersonId As Integer
            PersonId = Convert.ToInt32(Session("PersonId").ToString())
            Dim UniqueId As String
            UniqueId = Session("UniqueId")
            Dim personObj = OBJMaster.GetPersonnelByPersonIdAndId(PersonId, UniqueId)

            OBJMaster = New MasterBAL()
            Dim dtAccessLevels As DataTable = New DataTable()
            Dim dtAccessLevelsCopy As DataTable = New DataTable()
            dtAccessLevels = OBJMaster.GetPersonAccessLevels()
            dtAccessLevelsCopy = dtAccessLevels.Clone
            dtAccessLevelsCopy.Clear()


            'Dim roleManager = New RoleManager(Of IdentityRole)(New RoleStore(Of IdentityRole)(New ApplicationDbContext()))

            'Dim roles = roleManager.Roles.ToList()

            If Not personObj Is Nothing Then
                If personObj.Rows.Count > 0 Then
                    For index = 0 To dtAccessLevels.Rows.Count - 1

                        If personObj.Rows(0)("Roles") <> "SuperAdmin" And personObj.Rows(0)("Roles") <> "Support" Then
                            Dim role = Nothing
                            If dtAccessLevels(index)("Name") <> "SuperAdmin" Or dtAccessLevels(index)("Name") <> "Support" Then
                                role = dtAccessLevels(index)("Name")
                            End If
                            If Not role Is Nothing Then
                                dtAccessLevelsCopy.Rows.Add(dtAccessLevels(index).ItemArray)
                            End If
                        Else
                            Dim role = Nothing
                            If dtAccessLevels(index)("Name") <> "SuperAdmin" Then
                                role = dtAccessLevels(index)("Name")
                            End If
                            If Not role Is Nothing Then
                                dtAccessLevelsCopy.Rows.Add(dtAccessLevels(index).ItemArray)
                            End If
                        End If
                        If personObj.Rows(0)("Roles") = "User" Then
                            Dim custRole = Nothing
                            If dtAccessLevels(index)("Name") <> "CustomerAdmin" Then
                                custRole = dtAccessLevels(index)("Name")
                            End If
                            If Not custRole Is Nothing Then
                                dtAccessLevelsCopy.Rows.Add(dtAccessLevels(index).ItemArray)
                            End If

                        End If

                    Next
                End If
            End If

            'Dim roles = RoleManager.Roles.ToList()
            'If Not personObj Is Nothing Then
            '    If personObj.Rows.Count > 0 Then
            '        If personObj.Rows(0)("Roles") <> "SuperAdmin" And personObj.Rows(0)("Roles") <> "Support" Then
            '            Dim role = RoleManager.FindByName("SuperAdmin")
            '            If Not role Is Nothing Then
            '                roles.Remove(role)
            '            End If
            '            role = RoleManager.FindByName("Support")
            '            If Not role Is Nothing Then
            '                roles.Remove(role)
            '            End If
            '        Else
            '            Dim role = RoleManager.FindByName("SuperAdmin")
            '            If Not role Is Nothing Then
            '                roles.Remove(role)
            '            End If
            '        End If
            '        If personObj.Rows(0)("Roles") = "User" Then
            '            Dim custRole = RoleManager.FindByName("CustomerAdmin")
            '            If Not custRole Is Nothing Then
            '                roles.Remove(custRole)
            '            End If
            '        End If
            '    End If
            'End If

            DDL_AccessLevels.DataSource = dtAccessLevelsCopy
            DDL_AccessLevels.DataTextField = "DisplayName"
            DDL_AccessLevels.DataValueField = "Id"
            DDL_AccessLevels.DataBind()
            DDL_AccessLevels.Items.Insert(0, New ListItem("Select Role", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindAccessLevles Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting access levels, please try again later."

        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'ValidateAndSavePerson(False)
        CheckValidMapping()

        ValidateAndSavePerson(False)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllPersonnel?Filter=Filter")
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
        Try

            BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
            'BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
            BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))
            txtEmail.Attributes("value") = txtEmail.Text
            txtUserPassword.Attributes("value") = txtUserPassword.Text
            txtConfirmPassword.Attributes("value") = txtConfirmPassword.Text
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
            hdfDirtyFlag.Value = 1
        Catch ex As Exception

            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Private Sub BindTiming()
        Try

            Dim dtTiming As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtTiming = OBJMaster.GetTiming()

            gv_FuelingTimes.DataSource = dtTiming
            gv_FuelingTimes.DataBind()
        Catch ex As Exception

            log.Error("Error occurred in BindTiming Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    'Protected Sub btnVehicleTypeCancle_Click(sender As Object, e As EventArgs)

    '    If HDF_PersonnelId.Value.ToString() = Nothing Then
    '        BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
    '    Else
    '        Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
    '        BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
    '        BindVehiclesDataToCheckboxList(personid)
    '    End If
    'End Sub

    Protected Sub btnPersonSitecancle_Click(sender As Object, e As EventArgs)
        Try

            If HDF_PersonnelId.Value.ToString() = Nothing Then
                BindSites(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
            Else
                Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
                BindSites(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
                BindSitesDataToCheckboxList(personid)
            End If
        Catch ex As Exception

            log.Error("Error occurred in btnPersonSitecancle_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Protected Sub btnPersonTimingBoxcancle_Click(sender As Object, e As EventArgs)
        Try

            If HDF_PersonnelId.Value.ToString() = Nothing Then

            Else
                Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
                BindPersonTimings(personid)
            End If

        Catch ex As Exception

            log.Error("Error occurred in btnPersonTimingBoxcancle_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
        Try

            Dim CurrentPersonnelId As Integer = HDF_PersonnelId.Value

            Dim strConditions As String = ""
            If (Not Session("PerConditions") Is Nothing) Then
                strConditions = Session("PerConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"))
                End If
            End If

            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), False, strConditions.Replace("ANU.", ""))
            HDF_PersonnelId.Value = dtPerson.Rows(0)("PersonId")
            HDF_UniqueUserId.Value = dtPerson.Rows(0)("Id")

            BindPersonnelDetails(HDF_PersonnelId.Value, HDF_UniqueUserId.Value)

        Catch ex As Exception

            log.Error("Error occurred in First_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
        Try

            Dim CurrentPersonnelId As Integer = HDF_PersonnelId.Value

            Dim strConditions As String = ""
            If (Not Session("PerConditions") Is Nothing) Then
                strConditions = Session("PerConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"))
                End If
            End If

            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), False, strConditions.Replace("ANU.", ""))
            HDF_PersonnelId.Value = dtPerson.Rows(0)("PersonId")
            HDF_UniqueUserId.Value = dtPerson.Rows(0)("Id")
            BindPersonnelDetails(HDF_PersonnelId.Value, HDF_UniqueUserId.Value)

        Catch ex As Exception

            log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Try

            Dim CurrentPersonnelId As Integer = HDF_PersonnelId.Value

            Dim strConditions As String = ""
            If (Not Session("PerConditions") Is Nothing) Then
                strConditions = Session("PerConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"))
                End If
            End If

            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), False, strConditions.Replace("ANU.", ""))
            HDF_PersonnelId.Value = dtPerson.Rows(0)("PersonId")
            HDF_UniqueUserId.Value = dtPerson.Rows(0)("Id")
            BindPersonnelDetails(HDF_PersonnelId.Value, HDF_UniqueUserId.Value)

        Catch ex As Exception

            log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
        Try

            Dim CurrentPersonnelId As Integer = HDF_PersonnelId.Value

            Dim strConditions As String = ""
            If (Not Session("PerConditions") Is Nothing) Then
                strConditions = Session("PerConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=0 and ANU.CustomerId=" & Session("CustomerId"))
                End If
            End If

            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), False, strConditions.Replace("ANU.", ""))
            HDF_PersonnelId.Value = dtPerson.Rows(0)("PersonId")
            HDF_UniqueUserId.Value = dtPerson.Rows(0)("Id")
            BindPersonnelDetails(HDF_PersonnelId.Value, HDF_UniqueUserId.Value)

        Catch ex As Exception

            log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    'Protected Sub btnCloseVehicle_Click(sender As Object, e As EventArgs)
    '    If HDF_PersonnelId.Value.ToString() = Nothing Then
    '        BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
    '    Else
    '        Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
    '        BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
    '        BindVehiclesDataToCheckboxList(personid)
    '    End If
    '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
    'End Sub

    Protected Sub btnCancelSite_Click(sender As Object, e As EventArgs)
        Try

            If HDF_PersonnelId.Value.ToString() = Nothing Then
                BindSites(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
            Else
                Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
                BindSites(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
                BindSitesDataToCheckboxList(personid)
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnCancelSite_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub btnCancelFuelingTimes_Click(sender As Object, e As EventArgs)
        Try

            If HDF_PersonnelId.Value.ToString() = Nothing Then
                BindTiming()
            Else
                Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
                BindTiming()
                BindPersonTimings(personid)
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnCancelFuelingTimes_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Private Sub SendAuthorizedEmail(UserEmail As String)
        Try

            Dim body As String = String.Empty
            Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/AuthorizedEmail.txt"))
                body = sr.ReadToEnd()
            End Using
            '------------------
            body = body.Replace("UserEmail", UserEmail)

            Dim e As New EmailService()


            Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))

            mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
            mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

            Dim messageSend As New MailMessage()
            messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
            messageSend.[To].Add(New MailAddress(UserEmail))


            messageSend.Subject = ConfigurationManager.AppSettings("AuthorizedEmailSubject")
            messageSend.Body = body

            messageSend.IsBodyHtml = True
            mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))


            mailClient.Send(messageSend)

        Catch ex As Exception

            log.Error(String.Format("Error Occurred while sending Authorized email to user. Error is {0}. Details are email:{1}", ex.Message, UserEmail))

        End Try
    End Sub

    Private Sub SavePreAuthTransactions(PersonId As Integer, PhoneNumber As String, UserId As Integer, PreAuthCount As Integer)
        Try

            OBJMaster = New MasterBAL()
            OBJMaster.InsertPreAuthTransaction(PersonId, PhoneNumber, UserId, PreAuthCount)
        Catch ex As Exception
            log.Error("Error occurred in SavePreAuthTransactions Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub CHK_HubUser_CheckedChanged(sender As Object, e As EventArgs)
        Try

            If (CHK_HubUser.Checked = True) Then
                'txtUserPassword.Enabled = False
                'CPWDHide.Visible = True
                'If lblHeader.Text <> "Edit Personnel Information" Then
                '    CPWDHide1.Visible = False
                'Else
                '    CPWDHide1.Visible = True
                'End If

                'CPWDLabel.Visible = False
                'CPWDTextbox.Visible = False
                txtEmail.Enabled = False
                txtPhoneNumber.Enabled = False
                txtIMEINumber.Enabled = False
                'txtUserPassword.Enabled = False
                'txtConfirmPassword.Enabled = False
                'CHK_SendTransactionEmail.Enabled = False
                txtPreAuth.Enabled = False
                btnIMEIPersonnelMapping.Enabled = False

            Else
                'txtUserPassword.Enabled = True
                'If lblHeader.Text <> "Edit Personnel Information" Then
                '    CPWDHide.Visible = False
                '    CPWDHide1.Visible = False
                '    CPWDLabel.Visible = True
                '    CPWDTextbox.Visible = True
                'End If
                txtEmail.Enabled = True
                txtPhoneNumber.Enabled = True
                txtIMEINumber.Enabled = True
                'txtUserPassword.Enabled = True
                'txtConfirmPassword.Enabled = True
                'CHK_SendTransactionEmail.Enabled = True
                txtPreAuth.Enabled = True
                btnIMEIPersonnelMapping.Enabled = True
            End If

            If (CHK_HubUser.Checked = True And CHK_ChangePWD.Checked = False) Then

                PWDLabel.Visible = False
                PWDTextbox.Visible = False
                CPWDLabel.Visible = False
                CPWDTextbox.Visible = False
                PWDHide.Visible = True
                CPWDHide.Visible = True
                CPWDHide1.Visible = True
                txtUserPassword.Enabled = False
                txtConfirmPassword.Enabled = False

            ElseIf (CHK_HubUser.Checked = True And CHK_ChangePWD.Checked = True) Then
                PWDLabel.Visible = True
                PWDTextbox.Visible = True
                CPWDLabel.Visible = True
                CPWDTextbox.Visible = True
                PWDHide.Visible = False
                CPWDHide.Visible = False
                CPWDHide1.Visible = False
                txtUserPassword.Enabled = True
                txtConfirmPassword.Enabled = True
            ElseIf (CHK_HubUser.Checked = True And CHK_ChangePWD.Checked = True) Then
                PWDLabel.Visible = True
                PWDTextbox.Visible = True
                CPWDLabel.Visible = True
                CPWDTextbox.Visible = True
                PWDHide.Visible = False
                CPWDHide.Visible = False
                CPWDHide1.Visible = False
                txtUserPassword.Enabled = True
                txtConfirmPassword.Enabled = True
            ElseIf (CHK_HubUser.Checked = False And CHK_ChangePWD.Checked = False) Then
                If HDF_PersonnelId.Value <> "" Then
                    PWDLabel.Visible = False
                    PWDTextbox.Visible = False
                    CPWDLabel.Visible = False
                    CPWDTextbox.Visible = False
                    PWDHide.Visible = True
                    CPWDHide.Visible = True
                    CPWDHide1.Visible = True
                    txtUserPassword.Enabled = False
                    txtConfirmPassword.Enabled = False
                Else
                    PWDLabel.Visible = True
                    PWDTextbox.Visible = True
                    CPWDLabel.Visible = True
                    CPWDTextbox.Visible = True
                    PWDHide.Visible = False
                    CPWDHide.Visible = False
                    CPWDHide1.Visible = False
                    txtUserPassword.Enabled = True
                    txtConfirmPassword.Enabled = True
                End If


            End If
        Catch ex As Exception
            log.Error("Error occurred in CHK_HubUser_CheckedChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Public Sub SaveOtherMapping(PersonId As Integer, UniqueUserId As String)

        Try
            afterVehicles = ""
            afterFSlinks = ""
            afterfuelTimings = ""

            HDF_PersonnelId.Value = PersonId
            HDF_UniqueUserId.Value = UniqueUserId

            'If (chk_vehicleMap.Checked = True) Then

            '    Dim dtVehicles As DataTable = New DataTable()
            '    OBJMaster = New MasterBAL()
            '    dtVehicles = OBJMaster.GetVehicleByCondition(" and v.CustomerId=" & DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

            '    Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

            '    dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
            '    dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
            '    dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
            '    dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))


            '    For Each drVehicles As DataRow In dtVehicles.Rows

            '        Dim dr As DataRow = dtVehicle.NewRow()
            '        dr("PersonId") = PersonId
            '        dr("VehicleId") = drVehicles("VehicleId").ToString()
            '        dr("CreatedDate") = DateTime.Now
            '        dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
            '        dtVehicle.Rows.Add(dr)

            '        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
            '            afterVehicles = IIf(afterVehicles = "", drVehicles("VehicleNumber"), afterVehicles & ";" & drVehicles("VehicleNumber"))
            '        End If
            '    Next


            '    OBJMaster.InsertPersonVehicleMapping(dtVehicle, PersonId)
            'End If
            'Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

            'dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
            'dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
            'dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
            'dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))


            'For Each item As GridViewRow In gv_Vehicles.Rows

            '    Dim CHK_Vehicle As CheckBox = TryCast(item.FindControl("CHK_Vehicle"), CheckBox)
            '    If (CHK_Vehicle.Checked = True Or gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber").ToString().ToLower().Contains("guest") = True) Then
            '        Dim dr As DataRow = dtVehicle.NewRow()
            '        dr("PersonId") = PersonId
            '        dr("VehicleId") = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleId").ToString()
            '        dr("CreatedDate") = DateTime.Now
            '        dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
            '        dtVehicle.Rows.Add(dr)

            '        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
            '            afterVehicles = IIf(afterVehicles = "", gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber"), afterVehicles & ";" & gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber"))
            '        End If

            '    End If

            'Next


            'OBJMaster.InsertPersonVehicleMapping(dtVehicle, PersonId)

            'insert site person mapping
            Dim dtPersonSite As DataTable = New DataTable()
            dtPersonSite.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
            dtPersonSite.Columns.Add("SiteID", System.Type.[GetType]("System.Int32"))
            dtPersonSite.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
            dtPersonSite.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))


            For Each item As GridViewRow In gv_Sites.Rows

                Dim CHK_PersonSite As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
                If (CHK_PersonSite.Checked = True) Then
                    Dim dr As DataRow = dtPersonSite.NewRow()
                    dr("PersonId") = PersonId
                    dr("SiteID") = gv_Sites.DataKeys(item.RowIndex).Values("SiteID").ToString()
                    dr("CreatedDate") = DateTime.Now
                    dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
                    dtPersonSite.Rows.Add(dr)
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        afterFSlinks = IIf(afterFSlinks = "", gv_Sites.DataKeys(item.RowIndex).Values("WifiSSId").ToString(), afterFSlinks & ";" & gv_Sites.DataKeys(item.RowIndex).Values("WifiSSId").ToString())
                    End If
                End If
            Next

            OBJMaster.InsertPersonSiteMapping(dtPersonSite, PersonId)

            'delete and add timing
            Dim dtTimings As DataTable = New DataTable("dtPersonSiteTimings")
            dtTimings.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
            dtTimings.Columns.Add("TimeId", System.Type.[GetType]("System.String"))


            For Each item As GridViewRow In gv_FuelingTimes.Rows

                Dim CHK_FuelingTimes As CheckBox = TryCast(item.FindControl("CHK_FuelingTimes"), CheckBox)
                If (CHK_FuelingTimes.Checked = True) Then
                    Dim dr As DataRow = dtTimings.NewRow()
                    dr("PersonId") = PersonId
                    dr("TimeId") = gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeId").ToString()
                    dtTimings.Rows.Add(dr)
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        afterfuelTimings = IIf(afterfuelTimings = "", gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeText").ToString(), afterfuelTimings & ";" & gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeText").ToString())
                    End If
                End If
            Next

            OBJMaster.InsertPersonTimings(dtTimings, PersonId)


        Catch ex As Exception
            log.Error(String.Format("Error Occurred while mapping. Error is {0}.", ex.Message))
        End Try

    End Sub

    Protected Sub DDL_DepartmentNumber_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try

            DDL_Department.SelectedValue = DDL_DepartmentNumber.SelectedValue
            txtEmail.Attributes("value") = txtEmail.Text
            txtUserPassword.Attributes("value") = txtUserPassword.Text
            txtConfirmPassword.Attributes("value") = txtConfirmPassword.Text
            hdfDirtyFlag.Value = 1
        Catch ex As Exception
            log.Error("Error occurred in DDL_DepartmentNumber_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub DDL_Department_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try

            DDL_DepartmentNumber.SelectedValue = DDL_Department.SelectedValue
            txtEmail.Attributes("value") = txtEmail.Text
            txtUserPassword.Attributes("value") = txtUserPassword.Text
            txtConfirmPassword.Attributes("value") = txtConfirmPassword.Text
            hdfDirtyFlag.Value = 1
        Catch ex As Exception
            log.Error("Error occurred in DDL_Department_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Private Function CreateAfterData(Email As String, RoleName As String) As String
        Try

            Dim data As String = "Is Hub user = " & CHK_HubUser.Checked & " ; " &
                                    "Person Name = " & txtPersonName.Text.Replace(",", " ") & " ; " &
                                    "PIN (additional security) = " & txtPinNumber.Text.Trim().Replace(",", " ") & " ; " &
                                    "FOB Number = " & TXT_FoBNUM.Text.Replace(" ", "").Replace(",", " ") & " ; " &
                                    "Email (Username) = " & Email & " ; " &
                                    "Phone Number = " & txtPhoneNumber.Text & " ; " &
                                    "IMEI Number = " & txtIMEINumber.Text.Replace(",", " & ") & " ; " &
                                    "Access Levels = " & IIf(RoleName = "SuperAdmin", "SuperAdmin", DDL_AccessLevels.SelectedItem.Text) & " ; " &
                                    "Department Name = " & DDL_Department.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Department Number = " & DDL_DepartmentNumber.SelectedItem.Text & " ; " &
                                    "Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
                                    "Pre-Authorization Transactions Count = " & txtPreAuth.Text & " ; " &
                                    "Fluid Limit Per Transaction = " & txtFuelLimitPertxtn.Text & " ; " &
                                    "Fluid Limit Per Day = " & txtFuelLimitPerDay.Text & " ; " &
                                    "Authorized Fueling Times = " & afterfuelTimings & " ; " &
                                    "Authorized Fueling links = " & afterFSlinks & " ; " &
                                    "Update FluidSecure Link Software on next Fueling? = " & chkSoftUpdate.Checked & " ; " &
                                    "Active = " & chkIsApproved.Checked & " ; " &
                                    "Send Transaction Email = " & CHK_SendTransactionEmail.Checked & " ; " &
                                    "Additional Email = " & txtAdditionalEmail.Text.Replace(",", " ") & " ; " &
                                    "Collect Diagnostic Logs = " & chk_CollectDiagnosticLogs.Checked & " ; " &
                                    "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & ""
            '"Vehicles Allowed to Fuel = " & afterVehicles & " ; " &

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateAfterData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Function CreateBeforeData(dtPersonnel As DataTable) As String
        Try

            Dim data As String = "PersonId = " & dtPersonnel.Rows(0)("PersonId").ToString() & " ; " &
                                    "Is Hub user = " & dtPersonnel.Rows(0)("IsUserForHub").ToString() & " ; " &
                                    "Person Name = " & dtPersonnel.Rows(0)("PersonName").ToString().Replace(",", " ") & " ; " &
                                    "PIN (additional security) = " & dtPersonnel.Rows(0)("PinNumber").ToString().Replace(",", " ") & " ; " &
                                    "FOB Number = " & dtPersonnel.Rows(0)("FOBNumber").ToString().Replace(",", " ") & " ; " &
                                    "Email (Username) = " & dtPersonnel.Rows(0)("Email").ToString() & " ; " &
                                    "Phone Number = " & dtPersonnel.Rows(0)("PhoneNumber").ToString() & " ; " &
                                    "IMEI Number = " & dtPersonnel.Rows(0)("IMEI_UDID").ToString().Replace(",", " & ") & " ; " &
                                    "Access Levels = " & dtPersonnel.Rows(0)("RoleName").ToString().Replace(",", " ") & " ; " &
                                    "Department Name = " & dtPersonnel.Rows(0)("DeptName").ToString().Replace(",", " ") & " ; " &
                                    "Department Number = " & dtPersonnel.Rows(0)("DeptNumber").ToString() & " ; " &
                                    "Export Code = " & dtPersonnel.Rows(0)("ExportCode").ToString().Replace(",", " ") & " ; " &
                                    "Pre-Authorization Transactions Count = " & dtPersonnel.Rows(0)("PreAuth").ToString() & " ; " &
                                    "Fluid Limit Per Transaction = " & dtPersonnel.Rows(0)("FuelLimitPerTxn").ToString() & " ; " &
                                    "Fluid Limit Per Day = " & dtPersonnel.Rows(0)("FuelLimitPerDay").ToString() & " ; " &
                                    "Authorized Fueling Times = " & beforefuelTimings & " ; " &
                                    "Authorized Fueling links = " & beforeFSlinks & " ; " &
                                    "Update FluidSecure Link Software on next Fueling? = " & dtPersonnel.Rows(0)("SoftUpdate").ToString() & " ; " &
                                    "Active = " & dtPersonnel.Rows(0)("IsApproved").ToString() & " ; " &
                                    "Send Transaction Email = " & dtPersonnel.Rows(0)("SendTransactionEmail").ToString() & " ; " &
                                    "Additional Email = " & dtPersonnel.Rows(0)("AdditionalEmailId").ToString().Replace(",", " ") & " ; " &
                                    "Collect Diagnostic Logs = " & dtPersonnel.Rows(0)("CollectDiagnosticLogs").ToString().Replace(",", " ") & " ; " &
                                    "Company = " & dtPersonnel.Rows(0)("CompanyName").ToString().Replace(",", " ") & ""
            '"Vehicles Allowed to Fuel = " & beforeVehicles & " ; " &

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateBeforeData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub CHK_SendTransactionEmail_CheckedChanged(sender As Object, e As EventArgs)
        Try
            If CHK_SendTransactionEmail.Checked Then
                divAdditionalEmail.Visible = True
            Else
                divAdditionalEmail.Visible = False
            End If
        Catch ex As Exception

        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub CHK_ChangePWD_CheckedChanged(sender As Object, e As EventArgs)
        Try

            If (CHK_HubUser.Checked = True And CHK_ChangePWD.Checked = False) Then

                PWDLabel.Visible = False
                PWDTextbox.Visible = False
                CPWDLabel.Visible = False
                CPWDTextbox.Visible = False
                PWDHide.Visible = True
                CPWDHide.Visible = True
                CPWDHide1.Visible = True
                txtUserPassword.Enabled = False
                txtConfirmPassword.Enabled = False

            ElseIf (CHK_HubUser.Checked = True And CHK_ChangePWD.Checked = True) Then
                PWDLabel.Visible = True
                PWDTextbox.Visible = True
                CPWDLabel.Visible = True
                CPWDTextbox.Visible = True
                PWDHide.Visible = False
                CPWDHide.Visible = False
                CPWDHide1.Visible = False
                txtUserPassword.Enabled = True
                txtConfirmPassword.Enabled = True
            ElseIf (CHK_HubUser.Checked = False And CHK_ChangePWD.Checked = True) Then
                PWDLabel.Visible = True
                PWDTextbox.Visible = True
                CPWDLabel.Visible = True
                CPWDTextbox.Visible = True
                PWDHide.Visible = False
                CPWDHide.Visible = False
                CPWDHide1.Visible = False
                txtUserPassword.Enabled = True
                txtConfirmPassword.Enabled = True
            ElseIf (CHK_HubUser.Checked = False And CHK_ChangePWD.Checked = False) Then
                PWDLabel.Visible = False
                PWDTextbox.Visible = False
                CPWDLabel.Visible = False
                CPWDTextbox.Visible = False
                PWDHide.Visible = True
                CPWDHide.Visible = True
                CPWDHide1.Visible = True
                txtUserPassword.Enabled = False
                txtConfirmPassword.Enabled = False
            End If

        Catch ex As Exception
            log.Error("Error occurred in CHK_ChangePWD_CheckedChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Private Sub ResetUserPassword(UniqueUserId As String, PWD As String)
        Try

            Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
            Dim signinManager = Context.GetOwinContext().GetUserManager(Of ApplicationSignInManager)()
            Dim code = manager.GeneratePasswordResetToken(UniqueUserId)
            If Not code = Nothing Then
                Dim result = manager.ResetPassword(UniqueUserId, code, PWD)
                If result.Succeeded Then
                    Dim user = New ApplicationUser() 'New field for Adding Reset password date
                    user = manager.FindById(UniqueUserId)
                    user.PasswordResetDate = DateTime.Now
                    Dim resultReset As IdentityResult = manager.Update(user)

                    If resultReset.Succeeded Then

                    End If

                Else

                End If
            End If

        Catch ex As Exception
            log.Error("Error occurred in ResetUserPassword Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
        ValidateAndSavePerson(True)
    End Sub

    Private Sub ValidateAndSavePerson(IsSaveAndAddNew As Boolean)

        Dim steps As String = ""

        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            txtPinNumber.Text = txtPinNumber.Text.Trim()

            steps = "1"
            Dim PersonId As Integer = 0
            Dim UniqueUserId As String = 0
            Dim AuthorizedEmailSend As Boolean = False

            If (Not HDF_PersonnelId.Value = Nothing And Not HDF_PersonnelId.Value = "") Then
                steps = "2"
                PersonId = HDF_PersonnelId.Value
                UniqueUserId = HDF_UniqueUserId.Value

            End If
            steps = "3"

            Dim resultVal As Integer = ValidateFields(PersonId)

            If (resultVal <> 1) Then
                Return
            End If

            Dim user = New ApplicationUser()

            steps = "8"
            Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
            Dim signInManager = Context.GetOwinContext().[Get](Of ApplicationSignInManager)()

            manager.PasswordValidator = New PasswordValidator() With {
                        .RequiredLength = 6,
                        .RequireNonLetterOrDigit = True,
                        .RequireDigit = True,
                        .RequireLowercase = True,
                        .RequireUppercase = True
                        }
            steps = "9"

            Dim additionalEmail As String = ""
            If CHK_SendTransactionEmail.Checked Then
                additionalEmail = txtAdditionalEmail.Text
                If txtAdditionalEmail.Text <> "" Then
                    Dim strSplitEmail() As String = txtAdditionalEmail.Text.TrimEnd(";").Split(";")
                    For i = 0 To strSplitEmail.Length - 1
                        Dim email As New Regex("\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
                        If Not email.IsMatch(strSplitEmail(i)) Then
                            ErrorMessage.Visible = True
                            ErrorMessage.InnerText = "Please eneter valid Emails Transaction Receipt and separate with "";"" if multiple and try again."
                            txtPersonName.Focus()
                            Session("Redirect") = ""
                            Return
                        End If
                    Next
                End If
            Else
                txtAdditionalEmail.Text = ""
                additionalEmail = ""
            End If




            If (PersonId <> 0) Then
                steps = "10"
                user = manager.FindById(UniqueUserId)

                If (user.CustomerId <> Convert.ToInt32(DDL_Customer.SelectedValue)) Then
                    user.IsMainCustomerAdmin = False
                End If
                If DDL_AccessLevels.SelectedItem.Text = "User" Then
                    user.IsMainCustomerAdmin = False
                End If

                If (CHK_HubUser.Checked = True) Then
                    steps = "11"
                    OBJMaster = New MasterBAL()
                    If (txtEmail.Text = "") Then

                        Dim HubPersonNumber As Integer = OBJMaster.GetAndUpdateLastHubPersonNumberEntry()

                        Dim email As String = "u" & HubPersonNumber & "@FluidSecureHub.com"

                        user.UserName = email
                        user.Email = email
                    Else
                        user.UserName = txtEmail.Text
                        user.Email = txtEmail.Text
                    End If
                Else
                    steps = "11"
                    user.UserName = txtEmail.Text
                    user.Email = txtEmail.Text

                End If

                user.PersonName = txtPersonName.Text
                user.DepartmentId = Convert.ToInt32(DDL_Department.SelectedValue)
                steps = "12"
                If (txtFuelLimitPertxtn.Text = "") Then
                    user.FuelLimitPerTxn = Nothing
                Else
                    user.FuelLimitPerTxn = Convert.ToInt32(txtFuelLimitPertxtn.Text)
                End If
                steps = "13"
                If (txtFuelLimitPerDay.Text = "") Then
                    user.FuelLimitPerDay = Nothing
                Else
                    user.FuelLimitPerDay = Convert.ToInt32(txtFuelLimitPerDay.Text)
                End If
                steps = "14"
                user.PhoneNumber = txtPhoneNumber.Text

                If (txtPreAuth.Text = "") Then
                    user.PreAuth = Nothing
                Else
                    user.PreAuth = Convert.ToInt32(txtPreAuth.Text)
                End If

                steps = "15"
                user.SoftUpdate = IIf(chkSoftUpdate.Checked = True, "Y", "N")
                user.LastModifiedDate = DateTime.Now
                user.LastModifiedBy = Convert.ToInt32(Session("PersonId"))

                Dim roleName As String = RoleActions.GetRolesById(user.RoleId).Name
                If (roleName <> "SuperAdmin") Then
                    user.RoleId = DDL_AccessLevels.SelectedValue
                End If

                If (txtPinNumber.Text.Trim() = "") Then
                    user.PinNumber = Nothing
                Else
                    user.PinNumber = txtPinNumber.Text.Trim()
                End If
                steps = "16"
                user.CustomerId = Convert.ToInt32(DDL_Customer.SelectedValue)

                If (user.IsApproved = False And chkIsApproved.Checked = True) Then
                    AuthorizedEmailSend = True
                End If

                user.IsApproved = chkIsApproved.Checked
                If (chkIsApproved.Checked = True) Then
                    user.ApprovedOn = DateTime.Now
                    user.ApprovedBy = Convert.ToInt32(Session("PersonId"))
                End If
                steps = "17"
                user.ExportCode = txtExportCode.Text
                user.IMEI_UDID = txtIMEINumber.Text
                user.SendTransactionEmail = CHK_SendTransactionEmail.Checked
                user.RequestFrom = IIf(user.RequestFrom = Nothing, "W", user.RequestFrom)
                user.IsFluidSecureHub = False
                user.IsUserForHub = CHK_HubUser.Checked
                user.FOBNumber = TXT_FoBNUM.Text.Replace(" ", "")
                user.AdditionalEmailId = additionalEmail
                user.CollectDiagnosticLogs = chk_CollectDiagnosticLogs.Checked
                steps = "18"
                Dim result As IdentityResult = manager.Update(user)

                If result.Succeeded Then

                    If Session("ActiveInActive") IsNot Nothing Then
                        ' Set IMEI Active - Inactive
                        checkActiveInActive()
                    End If


                    If (CHK_ChangePWD.Checked = True) Then
                        ResetUserPassword(user.Id, txtUserPassword.Text)
                    End If

                    If (AuthorizedEmailSend = True And user.IsApproved = True) Then
                        SendAuthorizedEmail(user.Email)
                    End If
                    steps = "19"

                    SaveOtherMapping(PersonId, UniqueUserId)

                    If (txtPreAuth.Text <> "") Then

                        SavePreAuthTransactions(PersonId, user.PhoneNumber, Session("PersonId"), txtPreAuth.Text)

                    End If

                    message.Visible = True
                    message.InnerText = "Record saved."

                    Dim dtPreAuthTrans As DataTable = New DataTable()

                    dtPreAuthTrans = OBJMaster.GetPreAuthTransactionsByPersonId(PersonId, False)

                    Session("dtPreAuthTrans") = dtPreAuthTrans

                    Dim dvNotUsedPreAuth As DataView = dtPreAuthTrans.DefaultView
                    dvNotUsedPreAuth.RowFilter = "PreAuthStatus='N'"

                    lblPreAuthNotUsed.Text = dvNotUsedPreAuth.ToTable().Rows.Count

                    dvNotUsedPreAuth = dtPreAuthTrans.DefaultView
                    dvNotUsedPreAuth.RowFilter = "PreAuthStatus='U'"

                    lblPreAuthUsed.Text = dvNotUsedPreAuth.ToTable().Rows.Count

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateAfterData(user.Email, roleName)
                        writtenData = "PersonId = " & PersonId & " ; " & writtenData

                        CSCommonHelper.WriteLog("Modified", "Personnel", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                    If (IsSaveAndAddNew = True) Then
                        Response.Redirect(String.Format("~/Master/Personnel"))
                    End If

                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateAfterData(user.Email, roleName)
                        writtenData = "PersonId = " & PersonId & " ; " & writtenData

                        CSCommonHelper.WriteLog("Modified", "Personnel", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Person update failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Update failed. " + result.Errors(0) + "'"
                End If
                steps = "32"
            Else
                If (CHK_HubUser.Checked = True) Then
                    txtUserPassword.Text = "FluidSecure*123"

                    OBJMaster = New MasterBAL()
                    Dim HubPersonNumber As Integer = OBJMaster.GetAndUpdateLastHubPersonNumberEntry()

                    Dim email As String = "u" & HubPersonNumber & "@FluidSecureHub.com"
                    Dim ApprOn As DateTime
                    Dim ApprBy As Integer

                    If (chkIsApproved.Checked = True) Then

                        ApprOn = DateTime.Now
                        ApprBy = Convert.ToInt32(Session("PersonId"))

                    End If
                    steps = "33"
                    user = New ApplicationUser() With {
                   .UserName = email,
                   .Email = email,
                   .PersonName = txtPersonName.Text,
                   .DepartmentId = DDL_Department.SelectedValue,
                   .PhoneNumber = "",
                   .SoftUpdate = "N",
                   .CreatedDate = DateTime.Now,
                   .CreatedBy = Convert.ToInt32(Session("PersonId")),
                   .IsDeleted = False,
                   .RoleId = DDL_AccessLevels.SelectedValue,
                    .IsApproved = chkIsApproved.Checked,
                   .ApprovedBy = ApprBy,
                    .ApprovedOn = ApprOn,
                    .ExportCode = txtExportCode.Text,
                    .IMEI_UDID = "",
                    .CustomerId = Convert.ToInt32(DDL_Customer.SelectedValue),
                    .SendTransactionEmail = False,
                    .RequestFrom = "W",
                    .IsFluidSecureHub = False,
                   .IsUserForHub = CHK_HubUser.Checked,
                   .PasswordResetDate = DateTime.Now,
                   .FOBNumber = TXT_FoBNUM.Text.Replace(" ", ""),
                   .AdditionalEmailId = additionalEmail,
                    .IsPersonnelPINRequire = False,
                   .BluetoothCardReader = "",
                    .PrinterName = "",
                    .PrinterMacAddress = "",
                    .HubSiteName = "",
                   .BluetoothCardReaderMacAddress = "",
       .LFBluetoothCardReader = "",
       .LFBluetoothCardReaderMacAddress = "",
       .VeederRootMacAddress = "",
       .CollectDiagnosticLogs = chk_CollectDiagnosticLogs.Checked,
        .IsVehicleHasFob = False,
        .IsPersonHasFob = False,
       .IsTermConditionAgreed = False,
       .DateTimeTermConditionAccepted = Nothing,
          .IsGateHub = False
               }
                    steps = "34"
                    If (txtFuelLimitPertxtn.Text = "") Then
                        user.FuelLimitPerTxn = Nothing
                    Else
                        user.FuelLimitPerTxn = Convert.ToInt32(txtFuelLimitPertxtn.Text)
                    End If
                    steps = "35"
                    If (txtFuelLimitPerDay.Text = "") Then
                        user.FuelLimitPerDay = Nothing
                    Else
                        user.FuelLimitPerDay = Convert.ToInt32(txtFuelLimitPerDay.Text)
                    End If


                    user.PreAuth = 0

                    steps = "37"
                    If (txtPinNumber.Text.Trim() = "") Then
                        user.PinNumber = Nothing
                    Else
                        user.PinNumber = txtPinNumber.Text.Trim()
                    End If
                    steps = "38"

                Else
                    Dim ApprOn As DateTime
                    Dim ApprBy As Integer
                    AuthorizedEmailSend = True

                    If (chkIsApproved.Checked = True) Then

                        ApprOn = DateTime.Now
                        ApprBy = Convert.ToInt32(Session("PersonId"))

                    End If
                    steps = "33"
                    user = New ApplicationUser() With {
                   .UserName = txtEmail.Text,
                   .Email = txtEmail.Text,
                   .PersonName = txtPersonName.Text,
                   .DepartmentId = DDL_Department.SelectedValue,
                   .PhoneNumber = txtPhoneNumber.Text,
                   .SoftUpdate = IIf(chkSoftUpdate.Checked = True, "Y", "N"),
                   .CreatedDate = DateTime.Now,
                   .CreatedBy = Convert.ToInt32(Session("PersonId")),
                   .IsDeleted = False,
                   .RoleId = DDL_AccessLevels.SelectedValue,
                    .IsApproved = chkIsApproved.Checked,
                   .ApprovedBy = ApprBy,
                    .ApprovedOn = ApprOn,
                    .ExportCode = txtExportCode.Text,
                    .IMEI_UDID = txtIMEINumber.Text,
                    .CustomerId = Convert.ToInt32(DDL_Customer.SelectedValue),
                    .SendTransactionEmail = CHK_SendTransactionEmail.Checked,
                    .RequestFrom = "W",
                    .IsFluidSecureHub = False,
                   .IsUserForHub = CHK_HubUser.Checked,
                   .PasswordResetDate = DateTime.Now,
                   .FOBNumber = TXT_FoBNUM.Text.Replace(" ", ""),
                    .AdditionalEmailId = additionalEmail,
                    .IsPersonnelPINRequire = False,
                   .BluetoothCardReader = "",
                    .PrinterName = "",
                    .PrinterMacAddress = "",
                    .HubSiteName = "",
                   .BluetoothCardReaderMacAddress = "",
       .LFBluetoothCardReader = "",
       .LFBluetoothCardReaderMacAddress = "",
       .VeederRootMacAddress = "",
       .CollectDiagnosticLogs = chk_CollectDiagnosticLogs.Checked,
        .IsVehicleHasFob = False,
        .IsPersonHasFob = False,
       .IsTermConditionAgreed = False,
       .DateTimeTermConditionAccepted = Nothing,
          .IsGateHub = False
               }
                    steps = "34"
                    If (txtFuelLimitPertxtn.Text = "") Then
                        user.FuelLimitPerTxn = Nothing
                    Else
                        user.FuelLimitPerTxn = Convert.ToInt32(txtFuelLimitPertxtn.Text)
                    End If
                    steps = "35"
                    If (txtFuelLimitPerDay.Text = "") Then
                        user.FuelLimitPerDay = Nothing
                    Else
                        user.FuelLimitPerDay = Convert.ToInt32(txtFuelLimitPerDay.Text)
                    End If

                    steps = "36"
                    If (txtPreAuth.Text = "") Then
                        user.PreAuth = Nothing
                    Else
                        user.PreAuth = Convert.ToInt32(txtPreAuth.Text)
                    End If
                    steps = "37"
                    If (txtPinNumber.Text.Trim() = "") Then
                        user.PinNumber = Nothing
                    Else
                        user.PinNumber = txtPinNumber.Text.Trim()
                    End If
                    steps = "38"
                End If


                Dim result As IdentityResult = manager.Create(user, txtUserPassword.Text)
                If result.Succeeded Then
                    steps = "39"
                    If (AuthorizedEmailSend = True And user.IsApproved = True) Then
                        SendAuthorizedEmail(user.Email)
                    End If

                    OBJMaster = New MasterBAL()
                    Dim dtPerson As DataTable = New DataTable()
                    dtPerson = OBJMaster.GetPersonDetailByUniqueUserId(user.Id)
                    PersonId = dtPerson.Rows(0)("PersonId")

                    SaveOtherMapping(PersonId, user.Id)

                    If (txtPreAuth.Text <> "" And txtPreAuth.Text <> "0") Then

                        SavePreAuthTransactions(PersonId, user.PhoneNumber, Session("PersonId"), txtPreAuth.Text)

                    End If

                    Dim roleName As String = RoleActions.GetRolesById(user.RoleId).Name

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateAfterData(user.Email, roleName)
                        writtenData = "PersonId = " & PersonId & " ; " & writtenData

                        CSCommonHelper.WriteLog("Added", "Personnel", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    If Session("Redirect") <> Nothing Then
                        If Session("Redirect").ToString() <> "Redirect" Then
                            If (IsSaveAndAddNew = True) Then
                                Response.Redirect(String.Format("~/Master/Personnel"))
                            Else
                                Response.Redirect(String.Format("~/Master/Personnel?PersonId={0}&UniqueUserId={1}&RecordIs=New", PersonId, user.Id))
                            End If
                        End If
                    Else
                        If (IsSaveAndAddNew = True) Then
                            Response.Redirect(String.Format("~/Master/Personnel"))
                        Else
                            Response.Redirect(String.Format("~/Master/Personnel?PersonId={0}&UniqueUserId={1}&RecordIs=New", PersonId, user.Id))
                        End If
                    End If


                Else
                    ErrorMessage.Visible = True
                    If (result.Errors(0).ToLower().Contains("password")) Then
                        ErrorMessage.InnerText = "Password MUST be minimum 6 characters long and contain one (1) of the following: Upper Case letter (A-Z), lower case letter (a-z), special character (!@#$%^&*), number (0-9)"
                    Else
                        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                            Dim writtenData As String = CreateAfterData(user.Email, "")
                            writtenData = "PersonId = " & PersonId & " ; " & writtenData

                            CSCommonHelper.WriteLog("Added", "Personnel", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Registration failed." + result.Errors(0).Replace("'", "") + "'")
                        End If

                        ErrorMessage.InnerText = "Registration failed." + result.Errors(0).Replace("'", "") + "'"
                    End If
                    Session("Redirect") = ""
                End If

            End If
            steps = "51"

            txtPersonName.Focus()
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Registration failed. " + ex.Message + "'"

            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message + " Step : " + steps)
            txtPersonName.Focus()
            Session("Redirect") = ""
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();CloseRedirectToVehicleMappingModel();", True)
        End Try

    End Sub

    Private Function ValidateFields(PersonId As Integer) As Integer
        Try
            Dim CheckIdExists As Boolean = False
            OBJMaster = New MasterBAL()


            If (txtPersonName.Text = "") Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter personnel name."
                txtPersonName.Focus()
                Return 0

            End If

            Dim expression As String = "^[^,]+$"

            Dim match = Regex.Match(txtPersonName.Text, expression, RegexOptions.IgnoreCase)

            If Not match.Success Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Comma not allowed in person name. Please remove comma and try again."
                txtPersonName.Focus()
                Return 0
            End If

            If (DDL_AccessLevels.SelectedValue = "0" Or DDL_AccessLevels.SelectedValue = "") Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select role."
                DDL_AccessLevels.Focus()
                Return 0

            End If

            If (txtPinNumber.Text.Trim() <> "") Then

                match = Regex.Match(txtPinNumber.Text.Trim(), expression, RegexOptions.IgnoreCase)

                If Not match.Success Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Comma not allowed in person PIN. Please remove comma and try again."
                    txtPinNumber.Focus()
                    Return 0
                End If

                Dim CheckPinNumberExists As Boolean = False
                OBJMaster = New MasterBAL()

                CheckPinNumberExists = OBJMaster.CheckPinNumberExist(txtPinNumber.Text.Trim(), PersonId, DDL_Customer.SelectedValue)


                If CheckPinNumberExists = True Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Personnel pin number already exists. Please enter another pin number and try again"
                    txtPinNumber.Focus()
                    Session("Redirect") = ""
                    Return 0

                End If

            End If

            If (CHK_HubUser.Checked = False) Then

                If (txtEmail.Text = "") Then

                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter email."
                    txtEmail.Focus()
                    Return 0

                End If

                expression = "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"

                match = Regex.Match(txtEmail.Text, expression, RegexOptions.IgnoreCase)

                If Not match.Success Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter valid email."
                    txtEmail.Focus()
                    Return 0
                End If
                If (Request.QueryString("PersonId") = Nothing Or Request.QueryString("PersonId") = "" Or CHK_ChangePWD.Checked = True) Then

                    If (txtUserPassword.Text = "") Then

                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Please enter password."
                        txtUserPassword.Focus()
                        Return 0

                    End If

                    If (txtConfirmPassword.Text = "") Then

                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Please enter confirm password."
                        txtConfirmPassword.Focus()
                        Return 0

                    End If

                    If (txtUserPassword.Text <> txtConfirmPassword.Text) Then

                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Password and confirm password do not match. Please re enter the confirm password."
                        txtConfirmPassword.Focus()
                        Return 0

                    End If

                End If

                If (txtPhoneNumber.Text = "") Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter phone number."
                    txtPhoneNumber.Focus()
                    Return 0
                End If

                expression = "^[- +()]*[0-9][- +()0-9]*$"

                match = Regex.Match(txtPhoneNumber.Text, expression, RegexOptions.IgnoreCase)

                If Not match.Success Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter valid contact number."
                    txtPhoneNumber.Focus()
                    Return 0
                End If

                If (txtPhoneNumber.Text <> "") Then
                    CheckIdExists = OBJMaster.PhoneNumberIsExists(txtPhoneNumber.Text, PersonId)

                    If CheckIdExists = True Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Personnel phone number already exists."
                        txtPhoneNumber.Focus()
                        Session("Redirect") = ""
                        Return 0

                    End If
                End If

            End If

            If (DDL_Department.SelectedValue = "0" Or DDL_Department.SelectedValue = "") Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select department."
                DDL_Department.Focus()
                Return 0

            End If

            If (txtExportCode.Text <> "") Then
                expression = "^[^,]+$"
                match = Regex.Match(txtExportCode.Text, expression, RegexOptions.IgnoreCase)

                If Not match.Success Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Comma not allowed in export code. Please remove comma and try again."
                    txtExportCode.Focus()
                    Return 0
                End If
            End If

            Dim resultPreAuth As Integer = 0

            If (Not (Integer.TryParse(txtPreAuth.Text, resultPreAuth)) And txtPreAuth.Text <> "") Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Pre-Authorization Transactions Count should be integer number."
                txtPreAuth.Focus()

                Return 0

            End If

            Dim resultFuelLimitPertxtn As Integer = 0

            If (Not (Integer.TryParse(txtFuelLimitPertxtn.Text, resultFuelLimitPertxtn)) And txtFuelLimitPertxtn.Text <> "") Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Fluid Limit Per Transaction should be integer number."
                txtFuelLimitPertxtn.Focus()

                Return 0

            End If

            Dim resultFuelLimitPerDay As Integer = 0

            If (Not (Integer.TryParse(txtFuelLimitPerDay.Text, resultFuelLimitPerDay)) And txtFuelLimitPerDay.Text <> "") Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Fluid Limit Per Day should be integer number."
                txtFuelLimitPerDay.Focus()

                Return 0

            End If

            If (txtFuelLimitPertxtn.Text <> "" And txtFuelLimitPerDay.Text <> "") Then

                If (Convert.ToInt32(txtFuelLimitPertxtn.Text) > Convert.ToInt32(txtFuelLimitPerDay.Text)) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Fuel Limit Per Transaction must be lesser than Fuel Limit Per Day."
                    txtFuelLimitPertxtn.Focus()
                    Session("Redirect") = ""
                    Return 0
                End If

            End If

            Dim CheckIMEIExists As Boolean = False
            OBJMaster = New MasterBAL()


            If (txtIMEINumber.Text <> "") Then
                CheckIMEIExists = OBJMaster.CheckDuplicateIMEI_UDID(txtIMEINumber.Text, PersonId)

                If CheckIMEIExists = True Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "IMEI number already exist."
                    ErrorMessage.Focus()
                    Session("Redirect") = ""
                    Return 0

                End If
            End If

            If (txtPreAuth.Text = "") Then
                txtPreAuth.Text = 0
            End If

            If (txtPreAuth.Text <> "" And PersonId <> 0) Then

                Dim dtPreAuthTrans As DataTable = New DataTable()
                dtPreAuthTrans = Session("dtPreAuthTrans")

                If (dtPreAuthTrans Is Nothing) Then
                    dtPreAuthTrans = New DataTable()
                End If
                If (dtPreAuthTrans.Rows.Count > 0) Then

                    Dim dvNotUsedPreAuth As DataView = dtPreAuthTrans.DefaultView

                    dvNotUsedPreAuth.RowFilter = "PreAuthStatus='U'"

                    Dim PreAuthUsed As Integer = dvNotUsedPreAuth.ToTable().Rows.Count

                    If (PreAuthUsed > txtPreAuth.Text) Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Pre-Authorization Transactions Count is always greater than equal to Pre-Authorization Used Transactions Count."
                        txtPreAuth.Focus()
                        Session("Redirect") = ""
                        Return 0
                    End If

                End If
            End If

            TXT_FoBNUM.Text = TXT_FoBNUM.Text.Replace(" ", "")

            If TXT_FoBNUM.Text.Trim() <> "" Then
                Dim dtPersonForFobNumber As DataTable = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & Convert.ToInt32(DDL_Customer.SelectedValue) & "  And REPLACE(ANU.FOBNumber,' ','') ='" & TXT_FoBNUM.Text & "' and ANU.PersonId != " & PersonId, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                Dim dtVehicleForFobNumber As DataTable = OBJMaster.GetVehicleByCondition(" and V.CustomerId=" & Convert.ToInt32(DDL_Customer.SelectedValue) & "  And REPLACE(V.FOBNumber,' ','') ='" & TXT_FoBNUM.Text & "' ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                If (dtPersonForFobNumber.Rows.Count > 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Fob/Card Number already exists. Please enter another Fob/Card number and try again."
                    TXT_FoBNUM.Focus()
                    Session("Redirect") = ""
                    Return 0
                ElseIf (dtVehicleForFobNumber.Rows.Count > 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Fob/Card Number already exists against vehicle. Please enter another Fob/Card number and try again."
                    TXT_FoBNUM.Focus()
                    Session("Redirect") = ""
                    Return 0
                End If
            End If

            If (DDL_AccessLevels.SelectedValue = "52d3b8e6-95e6-44ac-b7bf-0be3acb56ff5") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "You have selected invalid role."
                DDL_AccessLevels.Focus()
                Return 0
            End If

            If (chkIsApproved.Checked = True) Then
                If (DDL_AccessLevels.SelectedValue = "52d3b8e6-95e6-44ac-b7bf-0be3acb56ff5") Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "You can not active/inactive this user."
                    chkIsApproved.Checked = False
                    chkIsApproved.Focus()
                    Return 0
                End If
            End If

            Return 1
        Catch ex As Exception
            log.Error("Error occurred in ValidateFields Exception is :" + ex.Message)
            Return 0
        End Try

    End Function

    Protected Sub btnAddVehicles_Click(sender As Object, e As EventArgs)
        Try

            txtEmail.Attributes("value") = txtEmail.Text
            txtUserPassword.Attributes("value") = txtUserPassword.Text
            txtConfirmPassword.Attributes("value") = txtConfirmPassword.Text
            If (HDF_PersonnelId.Value <> "") Then
                If hdfDirtyFlag.Value = "1" Then
                    lblMessageRedirectToVehicleMapping.InnerText = "There are some changes made which will be lost. Do you want to save those before proceeding? "
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "RedirectToVehicleMappingModel();loadFunction();", True)
                Else
                    Dim PersonId As Integer = 0
                    Dim UniqueUserId As String = 0

                    If (Not HDF_PersonnelId.Value = Nothing And Not HDF_PersonnelId.Value = "") Then
                        PersonId = HDF_PersonnelId.Value
                        UniqueUserId = HDF_UniqueUserId.Value

                    End If
                    Response.Redirect(String.Format("~/Master/VehiclePersonMapping?PersonId={0}&UniqueUserId={1}", PersonId, UniqueUserId))
                End If
            Else
                lblMessageRedirectToVehicleMapping.InnerText = "There are some changes made which will be lost. Do you want to save those before proceeding? "
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "RedirectToVehicleMappingModel();loadFunction();", True)
            End If



        Catch ex As Exception
            log.Error("Error occurred in btnAddVehicles_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub CheckValidMapping()
        Try

            Dim IsValid As Boolean = False

            Dim strWarning As String = "You have not assigned this user LinksVehiclesTimings. They will not be able to fuel."

            For Each item As GridViewRow In gv_Sites.Rows
                Dim CHK_PersonSite As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
                If (CHK_PersonSite.Checked = True) Then
                    IsValid = True
                    Exit For
                End If
            Next
            Dim errorMsg As String = ""

            If IsValid = False Then
                errorMsg = " Links"
            End If

            IsValid = False

            For Each item As GridViewRow In gv_FuelingTimes.Rows

                Dim CHK_FuelingTimes As CheckBox = TryCast(item.FindControl("CHK_FuelingTimes"), CheckBox)
                If (CHK_FuelingTimes.Checked = True) Then
                    IsValid = True
                    Exit For
                End If
            Next
            If IsValid = False Then
                errorMsg = IIf(errorMsg = "", " Timings", errorMsg & " , Timings")
            End If
            IsValid = False

            Dim dtPersonVehicleMapping As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtPersonVehicleMapping = OBJMaster.GetPersonVehicleMapping(Request.QueryString("PersonId"))

            If (dtPersonVehicleMapping.Rows.Count > 0) Then
                IsValid = True
            End If

            If IsValid = False Then
                errorMsg = IIf(errorMsg = "", " Vehicles", errorMsg & " , Vehicles")
            End If


            If (errorMsg <> "") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = strWarning.Replace("LinksVehiclesTimings", errorMsg)
            End If

        Catch ex As Exception
            log.Error("Error occurred in CheckValidMapping Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnIMEIPersonnelMapping_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Master/IMEIPersonMapping.aspx?PersonId=" & HDF_PersonnelId.Value & "&UniqueUserId=" & HDF_UniqueUserId.Value, False)
    End Sub

    Protected Sub btnMyModalActiveInActiveNo_Click(sender As Object, e As EventArgs)
        Try
            Session("ActiveInActive") = 0
        Catch ex As Exception
            log.Error("Error occurred in btnMyModalActiveInActiveNo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "AcceptActiveInActive();", True)
        End Try
    End Sub

    Protected Sub btnMyModalActiveInActiveSuccess_Click(sender As Object, e As EventArgs)
        Try
            Session("ActiveInActive") = 1
        Catch ex As Exception
            log.Error("Error occurred in btnMyModalActiveInActiveSuccess_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "AcceptActiveInActive();", True)
        End Try
    End Sub

    Private Sub checkActiveInActive()
        Try
            Dim ActiveInActive As Boolean = Session("ActiveInActive")
            Session("ActiveInActive") = Nothing
            Dim PersonActiveClick As Boolean = hdfConfirmActiveInactive.Value

            OBJMaster = New MasterBAL()

            Try

                If Not CHK_HubUser.Checked Then
                    ' Set IMEI Active - Inactive
                    If ActiveInActive Then
                        OBJMaster = New MasterBAL()
                        OBJMaster.UpdateIMEIActiveFlagByPersonId(HDF_PersonnelId.Value, Session("PersonId"), PersonActiveClick)
                    End If
                End If

            Catch ex As Exception
                log.Error("Error occurred in checkActiveInActive + UpdateIMEIActiveFlagByPersonId Exception is :" + ex.Message)
            End Try

        Catch ex As Exception
            ErrorMessage.InnerText = "IMEI activation failed."
            ErrorMessage.Visible = True
            log.Error("Error occurred in checkActiveInActive Exception is :" + ex.Message)
        End Try
    End Sub

    Protected Sub btnRedirectToVehicleMappingNo_Click(sender As Object, e As EventArgs)
        Try

            If (HDF_PersonnelId.Value <> "") Then
                Dim PersonId As Integer = 0
                Dim UniqueUserId As String = 0

                If (Not HDF_PersonnelId.Value = Nothing And Not HDF_PersonnelId.Value = "") Then
                    PersonId = HDF_PersonnelId.Value
                    UniqueUserId = HDF_UniqueUserId.Value
                End If
                Response.Redirect(String.Format("~/Master/VehiclePersonMapping?PersonId={0}&UniqueUserId={1}", PersonId, UniqueUserId))
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CloseRedirectToVehicleMappingModel();", True)
                Return
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnRedirectToVehicleMappingNo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnRedirectToVehicleMappingYes_Click(sender As Object, e As EventArgs)
        Try

            If (HDF_PersonnelId.Value <> "") Then
                If (Page.IsValid) Then
                    Session("Redirect") = "Redirect"
                    ValidateAndSavePerson(False)
                    If Session("Redirect").ToString() = "Redirect" Then
                        Dim PersonId As Integer = 0
                        Dim UniqueUserId As String = 0

                        If (Not HDF_PersonnelId.Value = Nothing And Not HDF_PersonnelId.Value = "") Then
                            PersonId = HDF_PersonnelId.Value
                            UniqueUserId = HDF_UniqueUserId.Value
                        End If
                        Response.Redirect(String.Format("~/Master/VehiclePersonMapping?PersonId={0}&UniqueUserId={1}", PersonId, UniqueUserId))
                    Else
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CloseRedirectToVehicleMappingModel();", True)
                        Return
                    End If
                Else
                    Page.Validate("PersonelValidation")
                End If
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CloseRedirectToVehicleMappingModel();", True)
            Else
                If (Page.IsValid) Then
                    Session("Redirect") = "Redirect"
                    ValidateAndSavePerson(False)
                    If Session("Redirect").ToString() = "Redirect" Then
                        Dim PersonId As Integer = 0
                        Dim UniqueUserId As String = 0

                        If (Not HDF_PersonnelId.Value = Nothing And Not HDF_PersonnelId.Value = "") Then
                            PersonId = HDF_PersonnelId.Value
                            UniqueUserId = HDF_UniqueUserId.Value
                        End If
                        Response.Redirect(String.Format("~/Master/VehiclePersonMapping?PersonId={0}&UniqueUserId={1}", PersonId, UniqueUserId))
                    Else
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CloseRedirectToVehicleMappingModel();", True)
                        Return
                    End If
                Else
                    Page.Validate("PersonelValidation")
                End If
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CloseRedirectToVehicleMappingModel();", True)
                Return
            End If

        Catch ex As Exception
            log.Error("Error occurred in btnRedirectToVehicleMappingNo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub DDL_AccessLevels_SelectedIndexChanged(sender As Object, e As EventArgs)
        hdfDirtyFlag.Value = 1
    End Sub
End Class