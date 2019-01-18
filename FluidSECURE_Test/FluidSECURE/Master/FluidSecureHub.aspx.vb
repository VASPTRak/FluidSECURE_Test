
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


Public Class FluidSecureHub
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FluidSecureHub))

    Dim OBJMaster As MasterBAL
    Shared beforeVehicles As String
    Shared afterVehicles As String
    Shared beforeFSlinks As String
    Shared afterFSlinks As String
    Shared beforeData As String


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
                If (Not IsPostBack) Then
                    BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                    'BindTiming()

                    txtIMEINumber.Text = ""
                    If (Not Request.QueryString("PersonId") = Nothing And Not Request.QueryString("PersonId") = "") Then
                        HDF_PersonnelId.Value = Request.QueryString("PersonId")
                        HDF_UniqueUserId.Value = Request.QueryString("UniqueUserId")
                        BindPersonnelDetails(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))

                        lblHeader.Text = "Edit FluidSecure Hub Information"

                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"
                        End If

                        btnFirst.Visible = True
                        btnNext.Visible = True
                        btnprevious.Visible = True
                        btnLast.Visible = True
                    Else
                        btnFirst.Visible = False
                        btnNext.Visible = False
                        btnprevious.Visible = False
                        btnLast.Visible = False
                        lblof.Visible = False

                        lblHeader.Text = "Add FluidSecure Hub Information"

                        BindDepartment(0)

                        DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                    End If


                    If Session("RoleName") <> "SuperAdmin" Then
                        UFLSLabel.Visible = False
                        UFLSCheckbox.Visible = False
                        UFLSHide.Visible = True
						WifiChannelToUse.Visible = False
						divViewHistory.visible = False
					Else
                        UFLSHide.Visible = False
						WifiChannelToUse.Visible = True
						divViewHistory.visible = True
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

    Private Sub BindVehicles(customerId As Integer)

        Try

            Dim dtVehicles As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtVehicles = OBJMaster.GetVehicleByCondition(" and v.CustomerId=" + customerId.ToString(), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

            gv_Vehicles.DataSource = dtVehicles
            gv_Vehicles.DataBind()


            If customerId = 0 Then
                lblVehicleMessage.Text = "Please select Company and then select vehicles."
                lblVehicleMessage.Visible = True
                gv_Vehicles.Visible = False
            ElseIf customerId <> 0 And dtVehicles.Rows.Count <> 0 Then

                lblVehicleMessage.Visible = False
                gv_Vehicles.Visible = True
            ElseIf customerId <> 0 And dtVehicles.Rows.Count = 0 Then
                lblVehicleMessage.Text = "Vehicles not found for selected Company."
                lblVehicleMessage.Visible = True
                gv_Vehicles.Visible = False
            End If


        Catch ex As Exception

            log.Error("Error occurred in BindVehicles Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting vehicles, please try again later."

        End Try


    End Sub

    Private Sub BindSites(CustomerId As Integer)

        Try
            Dim PersonId As Integer = 0
            If (Not HDF_PersonnelId.Value = Nothing And Not HDF_PersonnelId.Value = "") Then
                PersonId = HDF_PersonnelId.Value
            End If

            Dim dtSites As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtSites = OBJMaster.GetSiteByCondition(" And c.CustomerId =" & CustomerId.ToString() & "  and s.SiteID not in (select SiteId from PersonSiteMapping where PersonId <> " & PersonId & "  and PersonId in (select PersonId from AspNetUsers where CustomerId=" & CustomerId & "  and IsFluidSecureHub=1) and SiteId not in (select SiteId from PersonSiteMapping where PersonId = " & PersonId & ")) ",
                                                   Session("PersonId").ToString(), Session("RoleId").ToString(), False)
            'dtSites = OBJMaster.GetSiteByCondition(" And c.CustomerId =" & CustomerId.ToString() & " ", Session("PersonId").ToString(), Session("RoleId").ToString())
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

    Private Sub BindVehiclesDataToCheckboxList(PersonId As Integer)
        Try
            beforeVehicles = ""
            OBJMaster = New MasterBAL()
            Dim dtPersonVehicleMapping As DataTable = New DataTable()

            dtPersonVehicleMapping = OBJMaster.GetPersonVehicleMapping(PersonId)

            For Each dr As DataRow In dtPersonVehicleMapping.Rows

                For Each rows As GridViewRow In gv_Vehicles.Rows
                    If (dr("VehicleId") = gv_Vehicles.DataKeys(rows.RowIndex).Values("VehicleId").ToString()) Then
                        TryCast(rows.FindControl("CHK_Vehicle"), CheckBox).Checked = True
                    End If
                Next

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    beforeVehicles = IIf(beforeVehicles = "", dr("VehicleNumber"), beforeVehicles & ";" & dr("VehicleNumber"))

                End If
            Next

        Catch ex As Exception

            log.Error("Error occurred in BindVehiclesDataToCheckboxList Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting VehiclesDataToCheckboxList, please try again later."

        End Try

    End Sub

    Private Sub BindSitesDataToCheckboxList(PersonId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtPersonSiteMapping As DataTable = New DataTable()
            'Dim dtPrinterBCardInfo As DataTable = New DataTable()
            'Dim PrinterName As String = ""
            'Dim BCardReader As String = ""
            beforeFSlinks = ""
            dtPersonSiteMapping = OBJMaster.GetPersonSiteMapping(PersonId, 0)
            If dtPersonSiteMapping IsNot Nothing Then
                For Each dr As DataRow In dtPersonSiteMapping.Rows

                    For Each rows As GridViewRow In gv_Sites.Rows
                        If (dr("SiteID") = gv_Sites.DataKeys(rows.RowIndex).Values("SiteID").ToString()) Then
                            TryCast(rows.FindControl("CHK_PersonSite"), CheckBox).Checked = True
                            'dtPrinterBCardInfo = OBJMaster.UpdateAndGetPrinterNameAndBCardReader(Convert.ToInt32(gv_Sites.DataKeys(rows.RowIndex).Values("SiteID").ToString()), "", "", 1)
                            'If dtPrinterBCardInfo IsNot Nothing And dtPrinterBCardInfo.Rows.Count > 0 Then
                            'PrinterName = dtPrinterBCardInfo.Rows(0)("PrinterName")
                            'BCardReader = dtPrinterBCardInfo.Rows(0)("BluetoothCardReader")
                            'End If
                        End If
                    Next
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        beforeFSlinks = IIf(beforeFSlinks = "", dr("WifiSSID"), beforeFSlinks & ";" & dr("WifiSSID"))
                    End If

                Next
            End If
            'txtBCardReader.Text = BCardReader
            'txtPrinterName.Text = PrinterName
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
                Dim isValid As Boolean = False
                If (Session("RoleName") = "GroupAdmin") Then
                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                    For Each drCusts As DataRow In dtCustOld.Rows
                        If (drCusts("CustomerId") = dtPersonnel.Rows(0)("CustomerId").ToString()) Then
                            isValid = True
                            Exit For
                        End If

                    Next
                End If

                If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                    If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtPersonnel.Rows(0)("CustomerId").ToString()) Then

                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
                    End If

                End If

                If (dtPersonnel.Rows(0)("IsFluidSecureHub").ToString() = False) Then

                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                    Return

                End If

                BindSites(dtPersonnel.Rows(0)("CustomerId").ToString())
                BindSitesDataToCheckboxList(PersonId)

                BindDepartment(Convert.ToInt32(dtPersonnel.Rows(0)("CustomerId").ToString()))
                'BindPersonTimings(PersonId)

                txtPersonName.Text = dtPersonnel.Rows(0)("PersonName").ToString()
                txtEmail.Text = dtPersonnel.Rows(0)("Email").ToString()
                txtPhoneNumber.Text = dtPersonnel.Rows(0)("PhoneNumber").ToString()
                Try
                    DDL_Customer.SelectedValue = dtPersonnel.Rows(0)("CustomerId").ToString()

                Catch ex As Exception

                    DDL_Customer.SelectedValue = 0
                End Try

                If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
                    DDL_Customer.Enabled = False
                End If

                HDF_CompanyId.Value = DDL_Customer.SelectedValue

                BindVehicles(Convert.ToInt32(dtPersonnel.Rows(0)("CustomerId").ToString()))
                BindVehiclesDataToCheckboxList(PersonId)

                DDL_Department.SelectedValue = dtPersonnel.Rows(0)("DepartmentId").ToString()

                chkSoftUpdate.Checked = dtPersonnel.Rows(0)("SoftUpdate").ToString()

                txtExportCode.Text = dtPersonnel.Rows(0)("ExportCode").ToString()
                chkIsApproved.Checked = dtPersonnel.Rows(0)("IsApproved").ToString()
                CHK_IsPersonnelPINRequire.Checked = dtPersonnel.Rows(0)("IsPersonnelPINRequire").ToString()

                txtIMEINumber.Text = dtPersonnel.Rows(0)("IMEI_UDID").ToString()
                txtBCardReader.Text = dtPersonnel.Rows(0)("BluetoothCardReader").ToString()
                txtBluetoothCardReaderMacAddress.Text = dtPersonnel.Rows(0)("BluetoothCardReaderMacAddress").ToString()
                txtLFBCardReader.Text = dtPersonnel.Rows(0)("LFBluetoothCardReader").ToString()
                txtLFBluetoothCardReaderMacAddress.Text = dtPersonnel.Rows(0)("LFBluetoothCardReaderMacAddress").ToString()
                txtPrinterName.Text = dtPersonnel.Rows(0)("PrinterName").ToString()
                txtPrinterMACAddress.Text = dtPersonnel.Rows(0)("PrinterMacAddress").ToString()
                txtSiteName.Text = dtPersonnel.Rows(0)("SiteName").ToString()
                txtVeederRootMacAddress.Text = dtPersonnel.Rows(0)("VeederRootMacAddress").ToString()
                CHK_IsVehicleHasFob.Checked = dtPersonnel.Rows(0)("IsVehicleHasFob").ToString()
                CHK_IsPersonHasFob.Checked = dtPersonnel.Rows(0)("IsPersonHasFob").ToString()
                chk_GateHub.Checked = dtPersonnel.Rows(0)("IsGateHub").ToString()
                chk_IsVehicleNumberRequire.Checked = dtPersonnel.Rows(0)("IsVehicleNumberRequire").ToString()
                txtHUB_Address.Text = dtPersonnel.Rows(0)("HubAddress").ToString()
                 chk_IsLogging.Checked = dtPersonnel.Rows(0)("IsLogging").ToString()
                chk_HubForFA.Checked = dtPersonnel.Rows(0)("EnbDisHubForFA").ToString()
                txtContactName.Text = dtPersonnel.Rows(0)("ContactName").ToString()
                txtContactEmail.Text = dtPersonnel.Rows(0)("ContactEmail").ToString()
				DDL_WifiChannelToUse.SelectedValue = dtPersonnel.Rows(0)("WifiChannelToUse").ToString()
				HDF_HubIMEICurrentName.Value = dtPersonnel.Rows(0)("IMEI_UDID").ToString()
				HDF_HubIMEINameHistory.Value = dtPersonnel.Rows(0)("HubIMEIHistory").ToString()
				txtDeviceNumber.Text = dtPersonnel.Rows(0)("DevicePhone").ToString()
				HDF_HubCurrentDeviceNumber.Value = dtPersonnel.Rows(0)("DevicePhone").ToString()
				HDF_HubDeviceNumberHistory.Value = dtPersonnel.Rows(0)("DevicePhoneHistory").ToString()
				chkEnablePrinter.Checked = dtPersonnel.Rows(0)("EnablePrinter").ToString()

				BindHistory(PersonId)
				Dim strConditions As String = ""
                If (Not Session("FHubConditions") Is Nothing) Then
                    strConditions = Session("FHubConditions")
                Else
                    If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                        strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"))
                    End If
                End If

                Dim dt As DataTable = OBJMaster.GetPersonIdByCondition(PersonId, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), True, strConditions.Replace("ANU.", ""))
                HDF_TotalPersonnel.Value = dt.Rows(0)("TotalPersonId")

                OBJMaster = New MasterBAL()
                Dim dtAllPersonnel As DataTable = New DataTable()

                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = " and ISNULL(IsFluidSecureHub,0)=1 and ANU.CustomerId=" & Session("CustomerId")
                End If

                dtAllPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                dtAllPersonnel.PrimaryKey = New DataColumn() {dtAllPersonnel.Columns(0)}
                Dim dr As DataRow = dtAllPersonnel.Rows.Find(UniqueUserId)
                If Not IsDBNull(dr) Then

                    cnt = dtAllPersonnel.Rows.IndexOf(dr) + 1

                End If
                If (HDF_TotalPersonnel.Value = 1) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt >= HDF_TotalPersonnel.Value) Then
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

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    beforeData = CreateData(PersonId, True)
                End If

            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Data Not found. Please try again after some time."
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

            Else
                DDL_Department.DataSource = dtDept
            End If
            DDL_Department.DataTextField = "NAME"
            DDL_Department.DataValueField = "DeptId"

            DDL_Department.DataBind()
            DDL_Department.Items.Insert(0, New ListItem("Select Department", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindDepartment Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting departments, please try again later."

        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ValidateAndSaveFSHub(False)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllFluidSecureHub?Filter=Filter")
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
        Try

            BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
            BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
            BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))
            If (Not String.IsNullOrEmpty(HDF_PersonnelId.Value)) Then
                BindSitesDataToCheckboxList(HDF_PersonnelId.Value)
                BindVehiclesDataToCheckboxList(HDF_PersonnelId.Value)
            End If
            txtEmail.Attributes("value") = txtEmail.Text
            HDF_CompanyId.Value = (Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))

            If DDL_Customer.SelectedIndex <> 0 Then

                OBJMaster = New MasterBAL()
                Dim dtCustomer As DataTable = OBJMaster.GetCustomerId(DDL_Customer.SelectedValue.ToString())

                If dtCustomer IsNot Nothing And dtCustomer.Rows.Count > 0 Then
                    CHK_IsPersonnelPINRequire.Checked = dtCustomer.Rows(0)("IsPersonnelPINRequire")
                    chk_IsVehicleNumberRequire.Checked = dtCustomer.Rows(0)("IsVehicleNumberRequire")
                End If
            End If

        Catch ex As Exception

            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is : " + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub btnVehicleTypeCancle_Click(sender As Object, e As EventArgs)
        Try

            If HDF_PersonnelId.Value.ToString() = Nothing Then
                BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
            Else
                Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
                BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
                BindVehiclesDataToCheckboxList(personid)
            End If

        Catch ex As Exception

            log.Error("Error occurred in btnVehicleTypeCancle_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

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

    Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
        Try

            Dim CurrentPersonnelId As Integer = HDF_PersonnelId.Value
            Dim strConditions As String = ""
            If (Not Session("FHubConditions") Is Nothing) Then
                strConditions = Session("FHubConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"))
                End If
            End If
            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), True, strConditions.Replace("ANU.", ""))
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
            If (Not Session("FHubConditions") Is Nothing) Then
                strConditions = Session("FHubConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"))
                End If
            End If
            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), True, strConditions.Replace("ANU.", ""))
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
            If (Not Session("FHubConditions") Is Nothing) Then
                strConditions = Session("FHubConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"))
                End If
            End If
            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), True, strConditions.Replace("ANU.", ""))
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
            If (Not Session("FHubConditions") Is Nothing) Then
                strConditions = Session("FHubConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"), strConditions & " and ISNULL(IsFluidSecureHub,0)=1 and CustomerId=" & Session("CustomerId"))
                End If
            End If
            OBJMaster = New MasterBAL()
            Dim dtPerson As DataTable = OBJMaster.GetPersonIdByCondition(CurrentPersonnelId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), True, strConditions.Replace("ANU.", ""))
            HDF_PersonnelId.Value = dtPerson.Rows(0)("PersonId")
            HDF_UniqueUserId.Value = dtPerson.Rows(0)("Id")
            BindPersonnelDetails(HDF_PersonnelId.Value, HDF_UniqueUserId.Value)

        Catch ex As Exception

            log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Protected Sub btnCloseVehicle_Click(sender As Object, e As EventArgs)
        Try

            If HDF_PersonnelId.Value.ToString() = Nothing Then
                BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
            Else
                Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
                BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
                BindVehiclesDataToCheckboxList(personid)
            End If

        Catch ex As Exception

            log.Error("Error occurred in btnCloseVehicle_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

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

	Public Sub SaveOtherMapping(PersonId As Integer, UniqueUserId As String, HubIMEIHistory As String, DeviceNumberHistory As String)
		Try
			afterVehicles = ""
			afterFSlinks = ""

			HDF_PersonnelId.Value = PersonId
			HDF_UniqueUserId.Value = UniqueUserId

			OBJMaster = New MasterBAL()
			OBJMaster.InsertUpdateHubExtraInformation(PersonId, txtContactName.Text, txtContactEmail.Text, DDL_WifiChannelToUse.SelectedValue, Session("PersonId"), chk_HubForFA.Checked, HubIMEIHistory, txtDeviceNumber.Text, DeviceNumberHistory, chkEnablePrinter.Checked)


			Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

			dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
			dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
			dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
			dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))


			For Each item As GridViewRow In gv_Vehicles.Rows

				Dim CHK_Vehicle As CheckBox = TryCast(item.FindControl("CHK_Vehicle"), CheckBox)
				If (CHK_Vehicle.Checked = True Or gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber").ToString().ToLower().Contains("guest") = True) Then
					Dim dr As DataRow = dtVehicle.NewRow()
					dr("PersonId") = PersonId
					dr("VehicleId") = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleId").ToString()
					dr("CreatedDate") = DateTime.Now
					dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
					dtVehicle.Rows.Add(dr)

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						afterVehicles = IIf(afterVehicles = "", gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber"), afterVehicles & ";" & gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber"))
					End If

				End If
			Next


			OBJMaster.InsertPersonVehicleMapping(dtVehicle, PersonId)

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

					'OBJMaster.UpdateAndGetPrinterNameAndBCardReader(Convert.ToInt32(gv_Sites.DataKeys(item.RowIndex).Values("SiteID").ToString()), txtPrinterName.Text, txtBCardReader.Text, 0)
					'Else
					'OBJMaster.UpdateAndGetPrinterNameAndBCardReader(Convert.ToInt32(gv_Sites.DataKeys(item.RowIndex).Values("SiteID").ToString()), "", "", 0)
				End If
			Next

			OBJMaster.InsertPersonSiteMapping(dtPersonSite, PersonId)

		Catch ex As Exception
			log.Error(String.Format("Error Occurred while mapping. Error is {0}.", ex.Message))
		End Try
	End Sub

	Private Function CreateData(PersonId As Integer, IsBefore As Boolean) As String
        Try
            Dim vehicles As String = ""
            Dim links As String = ""

            If (IsBefore = True) Then
                vehicles = beforeVehicles
                links = beforeFSlinks
            Else
                vehicles = afterVehicles
                links = afterFSlinks
            End If

            Dim data As String = "PersonId = " & PersonId & " ; " &
                                    "FluidSecure Hub Name = " & txtPersonName.Text.Replace(",", " ") & " ; " &
                                    "Email (Username) = " & txtEmail.Text.Replace(",", " & ") & " ; " &
                                    "Phone Number = " & txtPhoneNumber.Text.Replace(",", " ") & " ; " &
                                    "IMEI Number = " & txtIMEINumber.Text.Replace(",", " & ") & " ; " &
                                    "Department Name = " & DDL_Department.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
                                    "Vehicles Allowed to Fuel = " & vehicles.Replace(",", " ") & " ; " &
                                    "Authorized Fueling links = " & links.Replace(",", " ") & " ; " &
                                    "Update FluidSecure Link Software on next Fueling? = " & IIf(chkSoftUpdate.Checked = True, "Yes", "No") & " ; " &
                                    "Active = " & IIf(chkIsApproved.Checked = True, "Yes", "No") & " ; " &
                                    "Personnel PIN Require = " & IIf(CHK_IsPersonnelPINRequire.Checked = True, "Yes", "No") & " ; " &
                                    "Vehicle Has a Fob/Card: = " & IIf(CHK_IsVehicleHasFob.Checked = True, "Yes", "No") & " ; " &
                                    "Person Has a Fob/Card: = " & IIf(CHK_IsPersonHasFob.Checked = True, "Yes", "No") & " ; " &
                                    "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "HF Bluetooth card reader = " & txtBCardReader.Text.Replace(",", " ") & " ; " &
                                    "Printer name = " & txtPrinterName.Text.Replace(",", " ") & " ; " &
                                    "Printer MAC Address = " & txtPrinterMACAddress.Text.Replace(",", " ") & " ; " &
                                    "Site Name = " & txtSiteName.Text.Replace(",", " ") & " ; " &
                                    "VeederRootMacAddress = " & txtVeederRootMacAddress.Text.Replace(",", " ") & " ; " &
                                    "HF Bluetooth Card Reader Mac Address = " & txtBluetoothCardReaderMacAddress.Text.Replace(",", " ") & " ; " &
                                    "LF Bluetooth Card Reader Mac Address = " & txtLFBCardReader.Text.Replace(",", " ") & " ; " &
                                    "LF Bluetooth Card Reader Mac Address = " & txtLFBluetoothCardReaderMacAddress.Text.Replace(",", " ") & " ; " &
                                    "Is Logging = " & IIf(chk_IsLogging.Checked = True, "Yes", "No") & " ; " &
                                    "Is Gate Hub = " & IIf(chk_GateHub.Checked = True, "Yes", "No") & " ; " &
                                    "Is Vehicle Number Required: = " & IIf(chk_IsVehicleNumberRequire.Checked = True, "Yes", "No") & " ; " &
                                    "Hub Address = " & txtHUB_Address.Text & " ; " &
                                    "Contact Name = " & txtContactName.Text & " ; " &
                                    "Contact Email = " & txtContactEmail.Text & " ; " &
                                    "Wifi Channel To Use = " & DDL_WifiChannelToUse.SelectedValue & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
        ValidateAndSaveFSHub(True)
    End Sub

    Private Sub ValidateAndSaveFSHub(IsSaveAndAddNew As Boolean)

        Dim PersonId As Integer = 0
        Dim steps As String = ""

        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            steps = "1"

            'If txtPhoneNumber.Text = "" Then
            '    ErrorMessage.Visible = True
            '    ErrorMessage.InnerText = "Please enter Phone Number and try again."
            '    txtPhoneNumber.Focus()
            '    Return
            'End If

            Dim expression = "^[- +()]*[0-9][- +()0-9]*$"

            Dim Match = Regex.Match(txtPhoneNumber.Text, expression, RegexOptions.IgnoreCase)
            If (txtPhoneNumber.Text <> "") Then
                If Not Match.Success Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter valid Phone number."
                    txtPhoneNumber.Focus()
                    Return
                End If
            End If

            If DDL_Customer.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select Company and try again."
                DDL_Customer.Focus()
                Return
            End If

            If DDL_Department.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select Department and try again."
                DDL_Department.Focus()
                Return
            End If

            Dim UniqueUserId As String = 0
            Dim AuthorizedEmailSend As Boolean = False

            If (Not HDF_PersonnelId.Value = Nothing And Not HDF_PersonnelId.Value = "") Then
                steps = "2"
                PersonId = HDF_PersonnelId.Value
                UniqueUserId = HDF_UniqueUserId.Value

            End If
            steps = "3"
            Dim CheckIdExists As Boolean = False
            OBJMaster = New MasterBAL()
            Dim user = New ApplicationUser()
            If (txtPhoneNumber.Text <> "") Then
                CheckIdExists = OBJMaster.PhoneNumberIsExists(txtPhoneNumber.Text, PersonId)
                steps = "4"
                If CheckIdExists = True Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Phone number already exists."
                    txtPhoneNumber.Focus()
                    Return

                End If
            End If

            steps = "6"
            Dim CheckIMEIExists As Boolean = False
            OBJMaster = New MasterBAL()
            steps = "7"

            If (txtIMEINumber.Text <> "") Then
                CheckIMEIExists = OBJMaster.CheckDuplicateIMEI_UDID(txtIMEINumber.Text, PersonId)

                If CheckIMEIExists = True Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "IMEI number already exist."
                    ErrorMessage.Focus()
                    Return

                End If
            End If

            If (chk_IsVehicleNumberRequire.Checked = False And CHK_IsPersonnelPINRequire.Checked = False) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select atleast one from Vehicle Number Require and Personnel PIN Require."
                txtSiteName.Focus()
                Return
            End If

            Dim IMEIPersonMappingId As Integer = 0

            If PersonId <> 0 Then
                OBJMaster = New MasterBAL()
                Dim dtIMEIPersonnelMapping As DataTable = New DataTable()

                dtIMEIPersonnelMapping = OBJMaster.GetIMEIPersonnelMappingByPersonId(PersonId)

                If (dtIMEIPersonnelMapping.Rows.Count > 0) Then
                    ' check in IMEI-PERSONNEL Mapping
                    OBJMaster = New MasterBAL()
                    IMEIPersonMappingId = Convert.ToInt32(dtIMEIPersonnelMapping.Rows(0)("IMEIPersonMappingId").ToString())
                    If (txtIMEINumber.Text <> "") Then
                        CheckIMEIExists = OBJMaster.CheckDuplicateIMEI_UDIDPersonMapping(txtIMEINumber.Text, Convert.ToInt32(HDF_PersonnelId.Value), IMEIPersonMappingId)

                        If CheckIMEIExists = True Then
                            ErrorMessage.Visible = True
                            ErrorMessage.InnerText = "IMEI number already exist."
                            ErrorMessage.Focus()
                            Return
                        End If
                    End If
                End If


            End If



            Dim IMEIList = txtIMEINumber.Text.Split(",")
            If IMEIList.Length > 1 Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Only one IMEI number allowed to assign for one hub."
                txtIMEINumber.Focus()
                Return

            End If

            Dim siteCnt As Integer = 0

            For Each item As GridViewRow In gv_Sites.Rows

                Dim CHK_PersonSite As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
                If (CHK_PersonSite.Checked = True) Then
                    siteCnt += 1
                End If

            Next

            If (chk_GateHub.Checked = True And siteCnt > 1) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Only one FluidSecure link is allowed be assigned per get hub."
                txtSiteName.Focus()
                Return
            End If

            If (siteCnt > 4) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Only four FluidSecure links are allowed be assigned per hub."
                ErrorMessage.Focus()
                Return
            End If

			If chkEnablePrinter.Checked Then
				If txtPrinterMACAddress.Text = "" Then
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Please enter MAC Address to enable Printer."
					txtPrinterMACAddress.Focus()
					Return
				End If
			End If

			If (txtPrinterName.Text = "" And txtPrinterMACAddress.Text <> "") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter printer name."
                txtPrinterName.Focus()
                Return
            End If

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

            If txtVeederRootMacAddress.Text <> "" Then
                Try
                    Dim strCondition = " and Id <> '" + HDF_UniqueUserId.Value + "' and VeederRootMacAddress = '" + txtVeederRootMacAddress.Text.ToUpper() + "'"
                    Dim dtData As DataTable = OBJMaster.GetIsVeederRootMacAddressAlredyUse(strCondition, Convert.ToInt32(DDL_Customer.SelectedValue))
                    If dtData IsNot Nothing Then
                        If dtData.Rows.Count > 0 Then
                            ErrorMessage.Visible = True
                            ErrorMessage.InnerText = "Veeder Root Mac Address already exists."
                            txtVeederRootMacAddress.Focus()
                            Return
                        End If
                    End If
                Catch ex As Exception

                End Try
            End If


			Dim HubIMEIHistory As String = ""
			If txtIMEINumber.Text = HDF_HubIMEICurrentName.Value Then
				HubIMEIHistory = HDF_HubIMEINameHistory.Value
			ElseIf HDF_HubIMEICurrentName.Value = "" Then
				HubIMEIHistory = ""
			Else
				HubIMEIHistory = HDF_HubIMEINameHistory.value + "," + HDF_HubIMEICurrentName.Value
			End If

			Dim HubDeviceNumberHistory As String = ""
			If txtDeviceNumber.Text = HDF_HubCurrentDeviceNumber.Value Then
				HubDeviceNumberHistory = HDF_HubDeviceNumberHistory.Value
			ElseIf HDF_HubCurrentDeviceNumber.Value = "" Then
				HubDeviceNumberHistory = ""
			Else
				HubDeviceNumberHistory = HDF_HubDeviceNumberHistory.Value + "," + HDF_HubCurrentDeviceNumber.Value
			End If

			If (PersonId <> 0) Then
                steps = "10"
                User = manager.FindById(UniqueUserId)

                If (User.CustomerId <> Convert.ToInt32(DDL_Customer.SelectedValue)) Then
                    User.IsMainCustomerAdmin = False
                End If

                steps = "11"
                'user.UserName = txtEmail.Text
                'user.Email = txtEmail.Text
                'user.PersonName = txtPersonName.Text
                User.DepartmentId = Convert.ToInt32(DDL_Department.SelectedValue)
                steps = "12"

                User.FuelLimitPerTxn = 0


                User.FuelLimitPerDay = 0

                steps = "14"
                User.PhoneNumber = txtPhoneNumber.Text


                User.PreAuth = 0

                steps = "15"
                User.SoftUpdate = IIf(chkSoftUpdate.Checked = True, "Y", "N")
                User.LastModifiedDate = DateTime.Now
                User.LastModifiedBy = Convert.ToInt32(Session("PersonId"))
                User.RoleId = "936965f9-b2bc-486c-af9a-df1025bb2966"


                User.PinNumber = Nothing

                steps = "16"
                User.CustomerId = Convert.ToInt32(DDL_Customer.SelectedValue)

                If (User.IsApproved = False And chkIsApproved.Checked = True) Then
                    AuthorizedEmailSend = True
                End If

                User.IsApproved = chkIsApproved.Checked
                If (chkIsApproved.Checked = True) Then
                    User.ApprovedOn = DateTime.Now
                    User.ApprovedBy = Convert.ToInt32(Session("PersonId"))
                End If
                steps = "17"
                User.ExportCode = txtExportCode.Text
                User.IMEI_UDID = txtIMEINumber.Text
                User.SendTransactionEmail = False
                User.RequestFrom = IIf(User.RequestFrom = Nothing, "W", User.RequestFrom)
                User.IsFluidSecureHub = True
                User.IsPersonnelPINRequire = CHK_IsPersonnelPINRequire.Checked
                User.BluetoothCardReader = txtBCardReader.Text
                User.BluetoothCardReaderMacAddress = txtBluetoothCardReaderMacAddress.Text.ToUpper()
                User.LFBluetoothCardReader = txtLFBCardReader.Text
                User.LFBluetoothCardReaderMacAddress = txtLFBluetoothCardReaderMacAddress.Text.ToUpper()
                User.PrinterName = txtPrinterName.Text
                User.PrinterMacAddress = txtPrinterMACAddress.Text.ToLower()
                User.HubSiteName = txtSiteName.Text
                User.VeederRootMacAddress = txtVeederRootMacAddress.Text.ToUpper()
                User.IsVehicleHasFob = CHK_IsVehicleHasFob.Checked
                User.IsPersonHasFob = CHK_IsPersonHasFob.Checked
                User.IsGateHub = chk_GateHub.Checked
                User.IsVehicleNumberRequire = chk_IsVehicleNumberRequire.Checked
                User.HubAddress = txtHUB_Address.Text
                User.IsLogging = chk_IsLogging.Checked
                steps = "18"
                Dim result As IdentityResult = manager.Update(User)

                If result.Succeeded Then
                    If (AuthorizedEmailSend = True And User.IsApproved = True) Then
                        SendAuthorizedEmail(User.Email)
                    End If
                    steps = "19"
                    HDF_PersonnelId.Value = PersonId
                    HDF_UniqueUserId.Value = UniqueUserId

					SaveOtherMapping(PersonId, user.Id, HubIMEIHistory, HubDeviceNumberHistory)

					steps = "31"

                End If

                If result.Succeeded Then
                    message.Visible = True
                    message.InnerText = "Record saved."

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(PersonId, False)
                        CSCommonHelper.WriteLog("Modified", "Fluid Secure Hub", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                    If (IsSaveAndAddNew = True) Then
                        Response.Redirect(String.Format("~/Master/FluidSecureHub"))
                    End If

                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(PersonId, False)
                        CSCommonHelper.WriteLog("Modified", "Fluid Secure Hub", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Fluid Secure Hub update failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Fluid Secure Hub update failed. " + result.Errors(0) + "'"
                End If
                steps = "32"

				' Update in IMEI Person mapping
				OBJMaster = New MasterBAL()

				OBJMaster.IMEI_UDIDPersonMappingInsertUpdate(IMEIPersonMappingId, Convert.ToInt32(HDF_PersonnelId.Value), txtIMEINumber.Text, chkIsApproved.Checked, Session("PersonId").ToString(), "Hub")

			Else
                Dim ApprOn As DateTime
                Dim ApprBy As Integer
                AuthorizedEmailSend = True

                If (chkIsApproved.Checked = True) Then

                    ApprOn = DateTime.Now
                    ApprBy = Convert.ToInt32(Session("PersonId"))

                End If

                OBJMaster = New MasterBAL()
                Dim HubName As Integer = OBJMaster.GetAndUpdateLastHubName()
                Dim FullHubName As String = "HUB" & HubName.ToString("00000000")

                steps = "33"
                user = New ApplicationUser() With {
               .UserName = FullHubName & "@gmail.com",
               .Email = FullHubName & "@gmail.com",
               .PersonName = FullHubName,
               .DepartmentId = DDL_Department.SelectedValue,
               .PhoneNumber = txtPhoneNumber.Text,
               .SoftUpdate = IIf(chkSoftUpdate.Checked = True, "Y", "N"),
               .CreatedDate = DateTime.Now,
               .CreatedBy = Convert.ToInt32(Session("PersonId")),
               .IsDeleted = False,
               .RoleId = "936965f9-b2bc-486c-af9a-df1025bb2966",
                .IsApproved = chkIsApproved.Checked,
               .ApprovedBy = ApprBy,
                .ApprovedOn = ApprOn,
                .ExportCode = txtExportCode.Text,
                .IMEI_UDID = txtIMEINumber.Text,
                .CustomerId = Convert.ToInt32(DDL_Customer.SelectedValue),
                .SendTransactionEmail = False,
                .RequestFrom = "W",
                .IsFluidSecureHub = True,
               .IsUserForHub = False,
                .PasswordResetDate = DateTime.Now,
                .FOBNumber = "",
                .AdditionalEmailId = "",
                .IsPersonnelPINRequire = CHK_IsPersonnelPINRequire.Checked,
                .BluetoothCardReader = txtBCardReader.Text,
                .PrinterName = txtPrinterName.Text,
                .PrinterMacAddress = txtPrinterMACAddress.Text.ToLower(),
                .HubSiteName = txtSiteName.Text,
               .BluetoothCardReaderMacAddress = txtBluetoothCardReaderMacAddress.Text.ToUpper(),
               .LFBluetoothCardReader = txtLFBCardReader.Text,
                .LFBluetoothCardReaderMacAddress = txtLFBluetoothCardReaderMacAddress.Text.ToUpper(),
                .VeederRootMacAddress = txtVeederRootMacAddress.Text.ToUpper(),
                .CollectDiagnosticLogs = False,
               .IsVehicleHasFob = CHK_IsVehicleHasFob.Checked,
               .IsPersonHasFob = CHK_IsPersonHasFob.Checked,
       .IsTermConditionAgreed = False,
       .DateTimeTermConditionAccepted = Nothing,
               .IsGateHub = chk_GateHub.Checked,
               .IsVehicleNumberRequire = chk_IsVehicleNumberRequire.Checked,
               .HubAddress = txtHUB_Address.Text,
               .IsLogging = chk_IsLogging.Checked,
           .IsSpecialImport = 0
            }

                steps = "34"

                User.FuelLimitPerTxn = 0
                steps = "35"

                User.FuelLimitPerDay = 0

                steps = "36"

                User.PreAuth = 0
                steps = "37"
                User.PinNumber = Nothing
                steps = "38"

                Dim result As IdentityResult = manager.Create(User, "FluidSecure*123")
                If result.Succeeded Then

                    OBJMaster = New MasterBAL()
                    Dim dtPerson As DataTable = New DataTable()
                    dtPerson = OBJMaster.GetPersonDetailByUniqueUserId(User.Id)
                    PersonId = dtPerson.Rows(0)("PersonId")


                    ' Update in IMEI Person mapping
                    OBJMaster = New MasterBAL()
                    OBJMaster.IMEI_UDIDPersonMappingInsertUpdate(IMEIPersonMappingId, PersonId, txtIMEINumber.Text, chkIsApproved.Checked, Session("PersonId").ToString(), "Hub")

                    txtPersonName.Text = FullHubName
                    txtEmail.Text = FullHubName & "@gmail.com"
                    manager.Update(User)

                    steps = "39"
                    If (AuthorizedEmailSend = True And User.IsApproved = True) Then
                        SendAuthorizedEmail(User.Email)
                    End If



					SaveOtherMapping(PersonId, user.Id, HubIMEIHistory, HubDeviceNumberHistory)

					steps = "49"

                End If
                steps = "50"

                If result.Succeeded Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(PersonId, False)

                        CSCommonHelper.WriteLog("Added", "Fluid Secure Hub", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    If (IsSaveAndAddNew = True) Then
                        Response.Redirect(String.Format("~/Master/FluidSecureHub"))
                    Else
                        Response.Redirect(String.Format("~/Master/FluidSecureHub?PersonId={0}&UniqueUserId={1}&RecordIs=New", PersonId, User.Id))
                    End If
                Else
                    ErrorMessage.Visible = True
                    If (result.Errors(0).ToLower().Contains("password")) Then
                        ErrorMessage.InnerText = "Password MUST be minimum 6 characters long and contain one (1) of the following: Upper Case letter (A-Z), lower case letter (a-z), special character (!@#$%^&*), number (0-9)"
                    Else
                        ErrorMessage.InnerText = "Registration failed." + result.Errors(0).Replace("'", "") + "'"
                    End If
                End If

            End If
            steps = "51"
			BindSitesDataToCheckboxList(PersonId)
			BindPersonnelDetails(PersonId, HDF_UniqueUserId.Value)
			txtPersonName.Focus()
        Catch ex As Exception

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData(PersonId, False)
                CSCommonHelper.WriteLog("Added", "Fluid Secure Hub", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Fluid Secure Hub update failed. Exception is : " & ex.Message)
            End If


            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Fluid Secure Hub update failed. " + ex.Message + "'"

            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message + " Step : " + steps)
            txtPersonName.Focus()
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try

    End Sub

	Protected Sub btn_ViewHistory_Click(sender As Object, e As EventArgs)
		Try
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenFSHistoryBox();", True)
		Catch ex As Exception
			log.Error("Error occurred in btn_DisableAllVehOdo_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub
	Private Sub BindHistory(PersonId)
		Try

			Dim dtFSHistory As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtFSHistory = OBJMaster.GetHubIMEIHistory(PersonId)
			Dim dv As DataView = dtFSHistory.DefaultView()
			dv.RowFilter = "Name <> ''"
			gv_HistoryView.DataSource = dv
			gv_HistoryView.DataBind()

			Dim dtDeviceNumberHistory As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtDeviceNumberHistory = OBJMaster.GetHubDeviceHistory(PersonId)
			Dim dvH As DataView = dtDeviceNumberHistory.DefaultView()
			dvH.RowFilter = "Name <> ''"
			gv_DeviceNumber.DataSource = dvH
			gv_DeviceNumber.DataBind()

		Catch ex As Exception

			log.Error("Error occurred in BindHistory Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Hub IMEI, please try again later."

		End Try
	End Sub
	Protected Sub btnCancelFSHistory_Click(sender As Object, e As EventArgs)
		Try
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CloseFSHistoryBox();", True)
		Catch ex As Exception
			log.Error("Error occurred in btn_DisableAllVehOdo_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub
	Protected Sub btnFSHistoryOk_Click(sender As Object, e As EventArgs)
		For i As Integer = 0 To gv_HistoryView.Rows.Count - 1

			Dim rb As RadioButton = DirectCast(gv_HistoryView.Rows(i).Cells(0).FindControl("rbHubIMEI"), RadioButton)
			If rb IsNot Nothing Then
				If rb.Checked = True Then

					Dim Name = gv_HistoryView.DataKeys(i).Value.ToString()
					txtIMEINumber.Text = Name
				End If


			End If
		Next
	End Sub

	Protected Sub btn_ViewDeviceHistory_Click(sender As Object, e As EventArgs)
		Try
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenDeviceNumberHistoryBox();", True)
		Catch ex As Exception
			log.Error("Error occurred in btn_ViewDeviceHistory_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnDeviceNumberHistoryOk_Click(sender As Object, e As EventArgs)
		For i As Integer = 0 To gv_DeviceNumber.Rows.Count - 1

			Dim rb As RadioButton = DirectCast(gv_DeviceNumber.Rows(i).Cells(0).FindControl("rbHubDeviceNumber"), RadioButton)
			If rb IsNot Nothing Then
				If rb.Checked = True Then

					Dim Name = gv_DeviceNumber.DataKeys(i).Value.ToString()
					txtDeviceNumber.Text = Name
				End If


			End If
		Next
	End Sub

	Protected Sub btnCancelDeviceNumber_Click(sender As Object, e As EventArgs)
		Try
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CloseDeviceNumberHistoryBox();", True)
		Catch ex As Exception
			log.Error("Error occurred in btnCancelDeviceNumber_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub
End Class
