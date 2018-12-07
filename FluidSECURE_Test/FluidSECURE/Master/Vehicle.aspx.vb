Imports log4net
Imports log4net.Config

Public Class Vehicle
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Vehicle))

    Dim OBJMaster As MasterBAL
    Shared beforeData As String
    Shared beforeProducts As String
    Shared beforeLinks As String
    Shared afterProducts As String
    Shared afterLinks As String

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
                If Not IsPostBack Then
                    Session("FSVMFirmanameValue") = ""
                    If Session("RoleName") = "CustomerAdmin" Then
                        txtFSTagMacAddress.Enabled = False
                    End If
                    txtVehicleNumber.Focus()
                    BindCustomers(Session("PersonId").ToString(), Session("RoleId").ToString())
                    BindSites(0)
                    DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                    BindDept()
                    BindFuelTypes(Convert.ToInt32(DDL_Customer.SelectedValue))
                    If (Not Request.QueryString("VehicleId") = Nothing And Not Request.QueryString("VehicleId") = "") Then
                        hdfVehicleId.Value = Request.QueryString("VehicleId")
                        BindVehicleDetails(Request.QueryString("VehicleId"))
                        btnFirst.Visible = True
                        btnNext.Visible = True
                        btnprevious.Visible = True
                        btnLast.Visible = True
                        lblHeader.Text = "Edit Vehicle Information"

                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"

                            Dim flag As Boolean = False
                            Dim vehicleId As Integer = 0
                            If (Not hdfVehicleId.Value = Nothing And Not hdfVehicleId.Value = "") Then
                                vehicleId = hdfVehicleId.Value
                            End If
                            Dim strWarning As String = "You have not assigned this vehicle Links. It will not be able to fuel."
                            For Each item As GridViewRow In gv_Sites.Rows
                                Dim CHK_FuelType As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
                                If (CHK_FuelType.Checked = True Or (txtVehicleNumber.Text.Trim().ToLower().Contains("guest") = True And vehicleId = 0)) Then
                                    flag = True
                                    Exit For
                                End If
                            Next
                            If Not flag Then
                                ErrorMessage.Visible = True
                                ErrorMessage.InnerText = strWarning
                            End If

                            Session("CustId") = DDL_Customer.SelectedValue
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenPersonVehicleMappingBox();", True)

                        End If
                        'CHK_CheckOdometerReasonable_CheckedChanged(Nothing, Nothing)
                    Else

                        btnFirst.Visible = False
                        btnNext.Visible = False
                        btnprevious.Visible = False
                        btnLast.Visible = False
                        lblof.Visible = False
                        lblHeader.Text = "Add Vehicle Information"
                        hideTotalMiles.Visible = False
                        HideTotalHours.Visible = False
                        hideShowOdometerReasonabilityeither.Visible = False
                        Dim result As DataSet
                        OBJMaster = New MasterBAL()
                        result = OBJMaster.FSVMcheckLaunchedAndExistedVersionAndUpdate("", "", 0, 2)

                        If result IsNot Nothing Then
                            If result.Tables(0) IsNot Nothing Then
                                If result.Tables(0).Rows.Count > 0 Then
                                    txt_FirmwareVer.Text = result.Tables(0).Rows(0)("launchedversion")
                                Else
                                    txt_FirmwareVer.Text = ""
                                End If
                            Else
                                txt_FirmwareVer.Text = ""
                            End If
                        Else
                            txt_FirmwareVer.Text = ""
                        End If

                    End If

                    'Allo Super admin to update current version
                    If Session("RoleName") = "SuperAdmin" Then
                        txt_FirmwareVer.Enabled = True
                    Else
                        txt_FirmwareVer.Enabled = False
                    End If

                End If
            End If

        Catch ex As Exception

            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

        End Try
    End Sub

    Private Sub BindCustomers(PersonId As Integer, RoleId As String)
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
                'DDL_Customer.Enabled = False
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

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting Companies , please try again later."

            log.Error("Error occurred in BindCustomers Exception is :" + ex.Message)

        End Try
    End Sub

    Private Sub BindDept()
        Try


            If (DDL_Dept.SelectedValue = 0) Then
                DDL_Dept.Items.Insert(0, New ListItem("Select Department", "0"))
            Else
                OBJMaster = New MasterBAL()
                Dim dtDept As DataTable = New DataTable()

                dtDept = OBJMaster.GetDepartments(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())

                DDL_Dept.DataSource = dtDept
                DDL_Dept.DataValueField = "DeptId"
                DDL_Dept.DataTextField = "Name"
                DDL_Dept.DataBind()
                DDL_Dept.Items.Insert(0, New ListItem("Select Department", "0"))
            End If

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting departments , please try again later."

            log.Error("Error occurred in BindDept Exception is :" + ex.Message)

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
            DDL_Dept.DataTextField = "NAME"
            DDL_Dept.DataValueField = "DeptId"

            DDL_Dept.DataBind()
            DDL_Dept.Items.Insert(0, New ListItem("Select Department", "0"))

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting department , please try again later."

            log.Error("Error occurred in BindDepartment Exception is :" + ex.Message)

        End Try

    End Sub

    Private Sub BindFuelTypesDataToCheckboxList(vehicleId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtFuelTypeVehicleMapping As DataTable = New DataTable()
            dtFuelTypeVehicleMapping = OBJMaster.GetFuelTypeVehicleMapping(vehicleId)

            If dtFuelTypeVehicleMapping IsNot Nothing Then
                For Each dr As DataRow In dtFuelTypeVehicleMapping.Rows

                    For Each rows As GridViewRow In gv_FuelTypes.Rows
                        If (dr("FuelTypeId") = gv_FuelTypes.DataKeys(rows.RowIndex).Values("FuelTypeId").ToString()) Then
                            TryCast(rows.FindControl("CHK_FuelType"), CheckBox).Checked = True
                        End If
                    Next

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        beforeProducts = IIf(beforeProducts = "", dr("FuelType"), beforeProducts & ";" & dr("FuelType"))
                    End If
                Next
            End If

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting FuelTypesDataToCheckboxList , please try again later."

            log.Error("Error occurred in BindFuelTypesDataToCheckboxList Exception is :" + ex.Message)

        End Try

    End Sub

    Private Sub BindVehicleSiteDataToCheckboxList(VehicleId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtVehiclesiteMapping As DataTable = New DataTable()

            dtVehiclesiteMapping = OBJMaster.GetVehicleSiteMapping(VehicleId, 0)
            If dtVehiclesiteMapping IsNot Nothing Then
                For Each dr As DataRow In dtVehiclesiteMapping.Rows

                    For Each rows As GridViewRow In gv_Sites.Rows
                        If (dr("SiteID") = gv_Sites.DataKeys(rows.RowIndex).Values("SiteID").ToString()) Then
                            TryCast(rows.FindControl("CHK_PersonSite"), CheckBox).Checked = True
                        End If
                    Next

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        beforeLinks = IIf(beforeLinks = "", dr("WifiSSId"), beforeLinks & ";" & dr("WifiSSId"))

                    End If
                Next
            End If

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting VehicleSiteDataToCheckboxList , please try again later."

            log.Error("Error occurred in BindVehicleSiteDataToCheckboxList Exception is :" + ex.Message)

        End Try

    End Sub

    Private Sub BindVehicleDetails(vehicleId As Integer)
        Try
            beforeLinks = ""
            beforeProducts = ""


            OBJMaster = New MasterBAL()
            Dim dtVehicle As DataTable = New DataTable()
            dtVehicle = OBJMaster.GetVehiclebyId(vehicleId)
            Dim cnt As Integer = 0
            If (dtVehicle.Rows.Count > 0) Then
                Dim isValid As Boolean = False
                If (Session("RoleName") = "GroupAdmin") Then
                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                    For Each drCusts As DataRow In dtCustOld.Rows
                        If (drCusts("CustomerId") = dtVehicle.Rows(0)("CustomerId").ToString()) Then
                            isValid = True
                            Exit For
                        End If

                    Next
                End If

                If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                    If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtVehicle.Rows(0)("CustomerId").ToString()) Then

                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
                    End If

                End If


                If (dtVehicle.Rows(0)("VehicleNumber").ToString() = "Default") Then
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                    Return
                End If

                BindFuelTypes(Convert.ToInt32(dtVehicle.Rows(0)("CustomerId").ToString()))
                BindSites(dtVehicle.Rows(0)("CustomerId").ToString())
                BindVehicleSiteDataToCheckboxList(vehicleId)
                BindFuelTypesDataToCheckboxList(vehicleId)

                txtComment.Text = dtVehicle.Rows(0)("Comment").ToString()
                txtCurrentOdometer.Text = dtVehicle.Rows(0)("CurrentOdometer").ToString()
                txtCurrentHrs.Text = dtVehicle.Rows(0)("CurrentHours").ToString()
                txtDescription.Text = dtVehicle.Rows(0)("Extension").ToString()
                txtFuelLimitPerDay.Text = dtVehicle.Rows(0)("FuelLimitPerDay").ToString()
                txtFuelLimitPerTxn.Text = dtVehicle.Rows(0)("FuelLimitPerTxn").ToString()
                txtLicensePlateNumber.Text = dtVehicle.Rows(0)("LicensePlateNumber").ToString()
                txtMake.Text = dtVehicle.Rows(0)("Make").ToString()
                txtModel.Text = dtVehicle.Rows(0)("Model").ToString()
                txtOdoLimit.Text = dtVehicle.Rows(0)("OdoLimit").ToString()
                txtHoursLimit.Text = dtVehicle.Rows(0)("HoursLimit").ToString()
                txtPrevOdometer.Text = dtVehicle.Rows(0)("PrevOdometer").ToString()
                txtPreviousHours.Text = dtVehicle.Rows(0)("PreviousHours").ToString()
                txtType.Text = dtVehicle.Rows(0)("Type").ToString()
                txtVehicleName.Text = dtVehicle.Rows(0)("VehicleName").ToString()
                txtVIN.Text = dtVehicle.Rows(0)("VIN").ToString()
                txtYear.Text = dtVehicle.Rows(0)("Year").ToString()

                txtVehicleNumber.Text = dtVehicle.Rows(0)("VehicleNumber").ToString()
                txtAccId.Text = dtVehicle.Rows(0)("Acc_Id").ToString()
                txtExportCode.Text = dtVehicle.Rows(0)("ExportCode").ToString()
                txtLastFueler.Text = dtVehicle.Rows(0)("PersonName").ToString()
                txtLastFuelDateTime.Text = dtVehicle.Rows(0)("LastFuelDateTIme").ToString()
                TXT_ExpectedMPGPerK.Text = dtVehicle.Rows(0)("ExpectedMPGPerK").ToString()

                CHK_RequireOdometerEntry.Checked = dtVehicle.Rows(0)("RequireOdometerEntry")
                CHK_CheckOdometerReasonable.Checked = dtVehicle.Rows(0)("CheckOdometerReasonable")
                CHK_Hours.Checked = dtVehicle.Rows(0)("Hours")
                RBL_UnitType.SelectedValue = dtVehicle.Rows(0)("MileageOrKilometers")
                txtLicenseState.Text = dtVehicle.Rows(0)("LicenseState")
                RBL_OdometerReasonabilityConditions.SelectedValue = dtVehicle.Rows(0)("OdometerReasonabilityConditions")
                txtFSTagMacAddress.Text = dtVehicle.Rows(0)("FSTagMacAddress")

                txt_FirmwareVer.Text = dtVehicle.Rows(0)("CurrentFSVMFirmwareVersion").ToString()
                Session("FSVMFirmanameValue") = dtVehicle.Rows(0)("CurrentFSVMFirmwareVersion").ToString()
                BindDepartment(Convert.ToInt32(dtVehicle.Rows(0)("CustomerId").ToString()))

                'CHK_CheckOdometerReasonable_CheckedChanged(Nothing, Nothing)

                If (CHK_CheckOdometerReasonable.Checked = True) Then
                    hideTotalMiles.Visible = True
                    HideTotalHours.Visible = True
                    hideShowOdometerReasonabilityeither.Visible = True
                Else
                    hideTotalMiles.Visible = False
                    HideTotalHours.Visible = False
                    hideShowOdometerReasonabilityeither.Visible = False
                End If


                Dim matchCustomer As ListItem
                matchCustomer = DDL_Customer.Items.FindByValue(dtVehicle.Rows(0)("CustomerId").ToString())
                If IsNothing(matchCustomer) Then
                    DDL_Customer.SelectedIndex = 0
                Else
                    DDL_Customer.SelectedValue = IIf(dtVehicle.Rows(0)("CustomerId").ToString() = "", 0, dtVehicle.Rows(0)("CustomerId").ToString())
                End If

                If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
                    DDL_Customer.Enabled = False
                End If


                Dim match As ListItem
                match = DDL_Dept.Items.FindByValue(dtVehicle.Rows(0)("DepartmentId").ToString())

                If IsNothing(match) Then
                    DDL_Dept.SelectedIndex = 0
                Else
                    DDL_Dept.SelectedValue = IIf(dtVehicle.Rows(0)("DepartmentId").ToString() = "", 0, dtVehicle.Rows(0)("DepartmentId").ToString())
                End If

                CHK_Active.Checked = dtVehicle.Rows(0)("IsActive").ToString()

                OBJMaster = New MasterBAL()

                Dim strConditions As String = ""
                If (Not Session("VehConditions") Is Nothing) Then
                    strConditions = Session("VehConditions")
                Else
                    If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                        strConditions = IIf(strConditions = "", " and V.CustomerId=" & Session("CustomerId"), strConditions & " and V.CustomerId=" & Session("CustomerId"))
                    End If
                End If



                HDF_TotalVehicle.Value = OBJMaster.GetVehicleIdByCondition(vehicleId, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)


                OBJMaster = New MasterBAL()
                Dim dtAllVehicles As DataTable = New DataTable()



                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and V.CustomerId=" & Session("CustomerId"), strConditions & " and V.CustomerId=" & Session("CustomerId"))
                End If

                dtAllVehicles = OBJMaster.GetVehicleByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                dtAllVehicles.PrimaryKey = New DataColumn() {dtAllVehicles.Columns(0)}
                Dim dr As DataRow = dtAllVehicles.Rows.Find(vehicleId)
                If Not IsDBNull(dr) Then

                    cnt = dtAllVehicles.Rows.IndexOf(dr) + 1

                End If
                If (HDF_TotalVehicle.Value = 1) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt >= HDF_TotalVehicle.Value) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                ElseIf (cnt <= 1) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt > 1 And cnt < HDF_TotalVehicle.Value) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                End If
                lblof.Text = cnt & " of " & HDF_TotalVehicle.Value.ToString()

                TXT_FoBNUM.Text = dtVehicle.Rows(0)("FOBNumber").ToString()

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    beforeData = CreateAfterData(vehicleId, True)
                End If

            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Data Not found. Please try again after some time."
            End If

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting VehicleDetails , please try again later."
            ErrorMessage.Focus()
            log.Error("Error occurred in BindVehicleDetails Exception is :" + ex.Message)
        Finally
            txtVehicleNumber.Focus()
        End Try

    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try

            Dim flag As Boolean = False
            Dim vehicleId As Integer = 0
            If (Not hdfVehicleId.Value = Nothing And Not hdfVehicleId.Value = "") Then
                vehicleId = hdfVehicleId.Value
            End If
            Dim strWarning As String = "You have not assigned this vehicle Links. It will not be able to fuel."
            For Each item As GridViewRow In gv_Sites.Rows
                Dim CHK_FuelType As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
                If (CHK_FuelType.Checked = True Or (txtVehicleNumber.Text.Trim().ToLower().Contains("guest") = True And vehicleId = 0)) Then
                    flag = True
                    Exit For
                End If
            Next
            If Not flag Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = strWarning
            End If

            ValidateAndSaveVehicle(False)

        Catch ex As Exception
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllVehicles.aspx?Filter=Filter")
    End Sub

    Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
        Try

            Dim strConditions As String = ""
            If (Not Session("VehConditions") Is Nothing) Then
                strConditions = Session("VehConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and Vehicles.CustomerId=" & Session("CustomerId"), strConditions & " and Vehicles.CustomerId=" & Session("CustomerId"))
                End If
            End If

            Dim CurrentVehicleId As Integer = hdfVehicleId.Value

            OBJMaster = New MasterBAL()
            Dim VehicleId As Integer = OBJMaster.GetVehicleIdByCondition(CurrentVehicleId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            hdfVehicleId.Value = VehicleId
            BindVehicleDetails(VehicleId)
        Catch ex As Exception
            log.Error("Error occurred in First_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
        Try

            Dim strConditions As String = ""
            If (Not Session("VehConditions") Is Nothing) Then
                strConditions = Session("VehConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and Vehicles.CustomerId=" & Session("CustomerId"), strConditions & " and Vehicles.CustomerId=" & Session("CustomerId"))
                End If
            End If
            Dim CurrentVehicleId As Integer = hdfVehicleId.Value

            OBJMaster = New MasterBAL()
            Dim VehicleId As Integer = OBJMaster.GetVehicleIdByCondition(CurrentVehicleId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            hdfVehicleId.Value = VehicleId
            BindVehicleDetails(VehicleId)
        Catch ex As Exception
            log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Try

            Dim strConditions As String = ""
            If (Not Session("VehConditions") Is Nothing) Then
                strConditions = Session("VehConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and Vehicles.CustomerId=" & Session("CustomerId"), strConditions & " and Vehicles.CustomerId=" & Session("CustomerId"))
                End If
            End If
            Dim CurrentVehicleId As Integer = hdfVehicleId.Value

            OBJMaster = New MasterBAL()
            Dim VehicleId As Integer = OBJMaster.GetVehicleIdByCondition(CurrentVehicleId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            hdfVehicleId.Value = VehicleId
            BindVehicleDetails(VehicleId)
        Catch ex As Exception
            log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
        Try

            Dim strConditions As String = ""
            If (Not Session("VehConditions") Is Nothing) Then
                strConditions = Session("VehConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and Vehicles.CustomerId=" & Session("CustomerId"), strConditions & " and Vehicles.CustomerId=" & Session("CustomerId"))
                End If
            End If
            Dim CurrentVehicleId As Integer = hdfVehicleId.Value

            OBJMaster = New MasterBAL()
            Dim VehicleId As Integer = OBJMaster.GetVehicleIdByCondition(CurrentVehicleId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            hdfVehicleId.Value = VehicleId
            BindVehicleDetails(VehicleId)
        Catch ex As Exception
            log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub BindSites(CustomerId As Integer)
        Try


            Dim dtSites As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtSites = OBJMaster.GetSiteByCondition(" And s.CustomerId =" & CustomerId, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), False)

            gv_Sites.DataSource = dtSites
            gv_Sites.DataBind()
            gv_Sites.Visible = True

            If CustomerId = 0 Then
                lblSiteMessage.Text = "Please select Company."
                lblSiteMessage.Visible = True
                gv_Sites.Visible = False
            ElseIf CustomerId <> 0 And dtSites.Rows.Count <> 0 Then
                lblSiteMessage.Text = ""
                lblSiteMessage.Visible = False

            ElseIf CustomerId <> 0 And dtSites.Rows.Count = 0 Then
                lblSiteMessage.Text = "FluidSecure Link not found for selected company."
                lblSiteMessage.Visible = True
                gv_Sites.Visible = False
            End If

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting sites, please try again later."

            log.Error("Error occurred in BindSites Exception is :" + ex.Message)

        End Try
    End Sub

    Private Sub BindFuelTypes(CustomerId As Integer)
        Try

            Dim dtFuelTpes As DataTable = New DataTable()
            OBJMaster = New MasterBAL()

            dtFuelTpes = OBJMaster.GetFuelDetails(CustomerId)


            gv_FuelTypes.DataSource = dtFuelTpes
            gv_FuelTypes.DataBind()
            gv_FuelTypes.Visible = True

            If dtFuelTpes.Rows.Count <> 0 Then
                lblFuelMessage.Visible = False
            ElseIf CustomerId = 0 Then
                lblFuelMessage.Text = "Please select company."
                lblFuelMessage.Visible = True
                gv_FuelTypes.Visible = False
            ElseIf dtFuelTpes.Rows.Count = 0 Then
                lblFuelMessage.Text = "Product not found for selected company."
                lblFuelMessage.Visible = True
                gv_FuelTypes.Visible = False
            End If
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting products, please try again later."

            log.Error("Error occurred in BindFuelTypes Exception is :" + ex.Message)

        End Try
    End Sub

    'Protected Sub DDL_Dept_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Dept.SelectedIndexChanged
    '    BindSites(Convert.ToInt32(DDL_Dept.SelectedValue))
    'End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
            BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))
            BindFuelTypes(Convert.ToInt32(DDL_Customer.SelectedValue))
        Catch ex As Exception
            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnCancelFuel_Click(sender As Object, e As EventArgs)
        Try
            If hdfVehicleId.Value.ToString() = Nothing Then
                BindFuelTypes(Convert.ToInt32(DDL_Customer.SelectedValue))
            Else
                BindFuelTypes(Convert.ToInt32(DDL_Customer.SelectedValue))
                BindFuelTypesDataToCheckboxList(Convert.ToInt32(hdfVehicleId.Value))
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnCancelFuel_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub btnCancelVehicle_Click(sender As Object, e As EventArgs)
        Try
            If hdfVehicleId.Value.ToString() = Nothing Then
                BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))
            Else
                BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))
                BindVehicleSiteDataToCheckboxList(Convert.ToInt32(hdfVehicleId.Value))
            End If
        Catch ex As Exception
            log.Error("Error occurred in btnCancelVehicle_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    <System.Web.Services.WebMethod(True)>
    Public Shared Function AddVehicleToAllPersons(VehicleId As String, CustomerId As String) As String
        Try

            Dim OBJMasterNew As MasterBAL = New MasterBAL()

            OBJMasterNew.MapPersonToNewGuestVehicle(Convert.ToInt32(HttpContext.Current.Session("PersonId")), VehicleId, Convert.ToInt32(HttpContext.Current.Session("CustId")))

            Return 1
        Catch ex As Exception
            log.Error("Error occurred in AddVehicleToAllPersons Exception is :" + ex.Message)
            Return 0
        End Try
    End Function

    Private Function CreateAfterData(VehicleId As Integer, IsBefore As Boolean) As String
        Try

            Dim products As String = ""
            Dim links As String = ""

            If (IsBefore = True) Then
                products = beforeProducts
                links = beforeLinks
            Else
                products = afterProducts
                links = afterLinks
            End If

            Dim data As String = "VehicleId = " & VehicleId & " ; " &
                                    "Vehicle Number = " & txtVehicleNumber.Text.Trim().Replace(",", " ") & " ; " &
                                    "Vehicle Name = " & txtVehicleName.Text.Replace(",", " ") & " ; " &
                                    "Description = " & txtDescription.Text.Replace(",", " ") & " ; " &
                                    "Department = " & DDL_Dept.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Account ID = " & txtAccId.Text.Replace(",", " ") & " ; " &
                                    "Make = " & txtMake.Text.Replace(",", " ") & " ; " &
                                    "Model = " & txtModel.Text.Replace(",", " ") & " ; " &
                                    "VIN = " & txtVIN.Text.Replace(",", " ") & " ; " &
                                    "Year = " & txtYear.Text & " ; " &
                                    "License Plate Number = " & txtLicensePlateNumber.Text.Replace(",", " ") & " ; " &
                                    "License State = " & txtLicenseState.Text.Replace(",", " ") & " ; " &
                                    "Type of Vehicle = " & txtType.Text.Replace(",", " ") & " ; " &
                                    "FOB Number = " & TXT_FoBNUM.Text.Replace(" ", "").Replace(",", " ") & " ; " &
                                    "Products = " & products & " ; " &
                                    "Authorized FluidSecure Links = " & links & " ; " &
                                    "Last Fluid Date/Time = " & txtLastFuelDateTime.Text & " ; " &
                                    "Current Odometer = " & txtCurrentOdometer.Text & " ; " &
                                    "Previous Odometer = " & txtPrevOdometer.Text & " ; " &
                                    "Current Hours = " & txtCurrentHrs.Text & " ; " &
                                    "Previous Hours = " & txtPreviousHours.Text & " ; " &
                                    "Last Fueler = " & txtLastFueler.Text.Replace(",", " ") & " ; " &
                                    "Require Odometer Entry = " & IIf(CHK_RequireOdometerEntry.Checked = True, "Yes", "No") & " ; " &
                                    "Hours = " & IIf(CHK_Hours.Checked = True, "Yes", "No") & " ; " &
                                    "Check Odometer Reasonability = " & IIf(CHK_CheckOdometerReasonable.Checked = True, "Yes", "No") & " ; " &
                                    "Mileage / Kilometers = " & IIf(RBL_UnitType.SelectedValue = "2", "Kilometers", "Mileage") & " ; " &
                                    "Odometer Reasonability either = " & IIf(RBL_OdometerReasonabilityConditions.SelectedValue = "2", "Don't allow fueling unless correct", "Allow fueling after 3 entry") & " ; " &
                                    "Total Miles allowed between Fueling = " & txtOdoLimit.Text & " ; " &
                                    "Total Hours allowed between Fueling = " & txtHoursLimit.Text & " ; " &
                                    "Fluid Limit Per Transaction = " & txtFuelLimitPerTxn.Text & " ; " &
                                    "Fuel Limit Per Day = " & txtFuelLimitPerDay.Text & " ; " &
                                    "Expected MPG or Liters/100KM = " & TXT_ExpectedMPGPerK.Text & " ; " &
                                    "Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
                                    "Comments = " & txtComment.Text.Replace(",", " ") & " ; " &
                                    "Inactive = " & CHK_Active.Checked & " ; " &
                                    "FSTag Mac Address = " & txtFSTagMacAddress.Text.Replace(",", " ") & " ; " &
                                    "Current Firmware Version = " & IIf(Session("RoleName") = "CustomerAdmin", Session("FirmanameValue").ToString().Replace(",", " "), txt_FirmwareVer.Text.Replace(",", " ")) & " ; " &
                                    "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & ""

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateAfterData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Sub MapVehicleToHub(VehicleId As Integer, UserId As Integer, CompanyId As Integer)
        Try

            For Each item As GridViewRow In gv_Sites.Rows

                Dim CHK_PersonSite As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
                If (CHK_PersonSite.Checked = True) Then
                    Dim dtPersonSiteMapping As DataTable = New DataTable()
                    dtPersonSiteMapping = OBJMaster.GetPersonSiteMappingBySiteId(gv_Sites.DataKeys(item.RowIndex).Values("SiteID").ToString(), CompanyId)
                    For Each dr As DataRow In dtPersonSiteMapping.Rows
                        OBJMaster.InsertSinglePersonnelVehicleMapping(dr("PersonId"), VehicleId, UserId)
                    Next
                End If

            Next
        Catch ex As Exception
            log.Error("Error occurred in MapVehicleToHub Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub CHK_CheckOdometerReasonable_CheckedChanged(sender As Object, e As EventArgs)
        Try

            If (CHK_CheckOdometerReasonable.Checked = True) Then
                hideTotalMiles.Visible = True
                HideTotalHours.Visible = True
                hideShowOdometerReasonabilityeither.Visible = True
            Else
                hideTotalMiles.Visible = False
                HideTotalHours.Visible = False
                hideShowOdometerReasonabilityeither.Visible = False
            End If
        Catch ex As Exception
            log.Error("Error occurred in CHK_CheckOdometerReasonable_CheckedChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
        ValidateAndSaveVehicle(True)
    End Sub

    Private Sub ValidateAndSaveVehicle(IsSaveAndAddNew As Boolean)
        Dim vehicleId As Integer = 0

        Try

            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            afterProducts = ""
            afterLinks = ""


            If (Not hdfVehicleId.Value = Nothing And Not hdfVehicleId.Value = "") Then

                vehicleId = hdfVehicleId.Value

            End If

            Dim result As Integer = 0

            Dim CheckVehicleNumberExist As Boolean = False
            OBJMaster = New MasterBAL()

            CheckVehicleNumberExist = OBJMaster.CheckVehicleNumberExist(txtVehicleNumber.Text.Trim(), vehicleId, Convert.ToInt32(DDL_Customer.SelectedValue))



            If CheckVehicleNumberExist = True Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Vehicle Number Already Exists."
                txtVehicleNumber.Focus()
                Return

            End If

            Dim CheckVINExist As Boolean = False
            If (txtVIN.Text.TrimStart().TrimEnd() <> "") Then
                CheckVINExist = OBJMaster.CheckVehicleVINExist(txtVIN.Text.TrimStart().TrimEnd(), vehicleId, Convert.ToInt32(DDL_Customer.SelectedValue))

                If CheckVINExist = True Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "VIN Already Exists."
                    txtVehicleNumber.Focus()
                    Return
                End If
            End If

            'If txt_FirmwareVer.Text = "" Then
            '    ErrorMessage.Visible = True
            '    ErrorMessage.InnerText = "Current Firmware Version not enter. Please enter current Firmware Version and try again."
            '    ErrorMessage.Focus()
            '    Return
            'End If

            Dim CheckVehicleFSTagMacAddressExist As Boolean = False
            OBJMaster = New MasterBAL()

            If txtFSTagMacAddress.Text <> "" Then
                CheckVehicleFSTagMacAddressExist = OBJMaster.CheckVehicleFSTagMacAddressExist(txtFSTagMacAddress.Text, vehicleId, Convert.ToInt32(DDL_Customer.SelectedValue))
            End If

            If CheckVehicleFSTagMacAddressExist = True Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Vehicle FSTag Mac Address Already Exists."
                txtVehicleNumber.Focus()
                Return

            End If

            If (CHK_CheckOdometerReasonable.Checked = True And txtOdoLimit.Text = "" And CHK_RequireOdometerEntry.Checked = True) Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total miles allowed between fueling."
                txtOdoLimit.Focus()
                Return

            End If

            If (CHK_CheckOdometerReasonable.Checked = True And txtOdoLimit.Text = "0" And CHK_RequireOdometerEntry.Checked = True) Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total miles allowed between fueling greater than 0."
                txtOdoLimit.Focus()
                Return

            End If

            Dim chkCount As Integer = 0

            For Each item As GridViewRow In gv_FuelTypes.Rows

                Dim CHK_FuelType As CheckBox = TryCast(item.FindControl("CHK_FuelType"), CheckBox)
                If (CHK_FuelType.Checked = True) Then
                    chkCount = chkCount + 1
                End If
            Next

            If (chkCount = 0) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select at least one product."
                txtVehicleNumber.Focus()
                Return
            End If

            TXT_FoBNUM.Text = TXT_FoBNUM.Text.Replace(" ", "")

            If TXT_FoBNUM.Text.Trim() <> "" Then
                Dim dtVehicleForFobNumber As DataTable = OBJMaster.GetVehicleByCondition(" and V.CustomerId=" & Convert.ToInt32(DDL_Customer.SelectedValue) & "  And REPLACE(V.FOBNumber,' ','') ='" & TXT_FoBNUM.Text & "' and V.VehicleId != " & vehicleId, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                Dim dtPersonForFobNumber As DataTable = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & Convert.ToInt32(DDL_Customer.SelectedValue) & "  And REPLACE(ANU.FOBNumber,' ','') ='" & TXT_FoBNUM.Text & "'", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

                If (dtVehicleForFobNumber.Rows.Count > 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Fob/Card  Number already exists. Please enter another Fob/Card number and try again."
                    TXT_FoBNUM.Focus()
                    Return
                ElseIf (dtPersonForFobNumber.Rows.Count > 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Fob/Card  Number already exists against person. Please enter another Fob/Card number and try again."
                    TXT_FoBNUM.Focus()
                    Return
                End If
            End If

            If txtVehicleNumber.Text.Trim() = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Vehicle number and try again."
                txtVehicleNumber.Focus()
                Return
            End If

            If DDL_Dept.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select Department and try again."
                DDL_Dept.Focus()
                Return
            End If

            If DDL_Customer.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select Company and try again."
                DDL_Customer.Focus()
                Return
            End If

            Dim resultInteger As Integer = 0

            If (txtCurrentOdometer.Text <> "" And Not (Integer.TryParse(txtCurrentOdometer.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Current Odometer as number and try again."
                txtCurrentOdometer.Focus()
                Return
            End If

            If (txtYear.Text <> "" And Not (Integer.TryParse(txtYear.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Year as number and try again."
                txtYear.Focus()
                Return
            End If

            If (txtFuelLimitPerTxn.Text <> "" And Not (Integer.TryParse(txtFuelLimitPerTxn.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Fluid Limit Per Transaction as number and try again."
                txtFuelLimitPerTxn.Focus()
                Return
            End If

            If (TXT_ExpectedMPGPerK.Text <> "" And Not (Integer.TryParse(TXT_ExpectedMPGPerK.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Expected MPG or Liters/100KM as number and try again."
                TXT_ExpectedMPGPerK.Focus()
                Return
            End If

            If (txtCurrentHrs.Text <> "" And Not (Integer.TryParse(txtCurrentHrs.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Current hours as number and try again."
                txtCurrentHrs.Focus()
                Return
            End If

            If (CHK_CheckOdometerReasonable.Checked = True And txtHoursLimit.Text = "" And CHK_Hours.Checked = True) Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total hours allowed between fueling."
                txtHoursLimit.Focus()
                Return

            End If

            If (CHK_CheckOdometerReasonable.Checked = True And txtHoursLimit.Text = "0" And CHK_Hours.Checked = True) Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total hours allowed between fueling greater than 0."
                txtHoursLimit.Focus()
                Return

            End If

            OBJMaster = New MasterBAL()

            txtVehicleNumber.Text = txtVehicleNumber.Text.Trim()

            Dim CurrentFSVMVersion As String = ""

            If vehicleId = 0 Then
                CurrentFSVMVersion = txt_FirmwareVer.Text
            Else
                If Session("RoleName") = "CustomerAdmin" Then
                    CurrentFSVMVersion = Session("FSVMFirmanameValue").ToString()
                Else
                    CurrentFSVMVersion = txt_FirmwareVer.Text
                End If
            End If

            result = OBJMaster.SaveUpdateVehicle(vehicleId, txtVehicleName.Text, txtMake.Text, txtModel.Text,
                                                 IIf(txtYear.Text = "", "-1", txtYear.Text), txtLicensePlateNumber.Text,
                                                 txtVIN.Text.TrimStart().TrimEnd(), txtType.Text, txtDescription.Text, txtComment.Text,
                                                 IIf(txtCurrentOdometer.Text = "", "-1", txtCurrentOdometer.Text), DDL_Dept.SelectedValue,
                                                 IIf(txtFuelLimitPerTxn.Text = "", 0, txtFuelLimitPerTxn.Text), IIf(txtFuelLimitPerDay.Text = "", 0, txtFuelLimitPerDay.Text), IIf(CHK_RequireOdometerEntry.Checked = True, "Y", "N"),
                                                 IIf(CHK_CheckOdometerReasonable.Checked = True, "Y", "N"),
                                                 IIf(txtOdoLimit.Text = "", "-1", txtOdoLimit.Text), txtVehicleNumber.Text.Trim(), txtAccId.Text,
                                                 txtExportCode.Text, Convert.ToInt32(Session("PersonId")), DDL_Customer.SelectedValue, IIf(TXT_ExpectedMPGPerK.Text = "", "-1", TXT_ExpectedMPGPerK.Text),
                                                 CHK_Hours.Checked, RBL_UnitType.SelectedValue, txtLicenseState.Text, RBL_OdometerReasonabilityConditions.SelectedValue,
                                                 TXT_FoBNUM.Text.Replace(" ", ""), CHK_Active.Checked, txtFSTagMacAddress.Text, IIf(txtCurrentHrs.Text = "", "-1", txtCurrentHrs.Text), IIf(txtHoursLimit.Text = "", "-1", txtHoursLimit.Text),
                                                 CurrentFSVMVersion, False)


            Dim dtFuelVehicle As DataTable = New DataTable("dtFuelTypeAndVehicle")

            dtFuelVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
            dtFuelVehicle.Columns.Add("FuelTypeId", System.Type.[GetType]("System.Int32"))


            For Each item As GridViewRow In gv_FuelTypes.Rows

                Dim CHK_FuelType As CheckBox = TryCast(item.FindControl("CHK_FuelType"), CheckBox)
                If (CHK_FuelType.Checked = True Or (txtVehicleNumber.Text.Trim().ToLower().Contains("guest") = True And vehicleId = 0)) Then
                    Dim dr As DataRow = dtFuelVehicle.NewRow()
                    dr("VehicleId") = result
                    dr("FuelTypeId") = gv_FuelTypes.DataKeys(item.RowIndex).Values("FuelTypeId").ToString()
                    dtFuelVehicle.Rows.Add(dr)

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        afterProducts = IIf(afterProducts = "", gv_FuelTypes.DataKeys(item.RowIndex).Values("FuelType").ToString(), afterProducts & ";" & gv_FuelTypes.DataKeys(item.RowIndex).Values("FuelType").ToString())
                    End If

                End If
            Next

            'insert site person mapping
            Dim dtVehicleSite As DataTable = New DataTable()
            dtVehicleSite.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
            dtVehicleSite.Columns.Add("SiteID", System.Type.[GetType]("System.Int32"))



            For Each item As GridViewRow In gv_Sites.Rows

                Dim CHK_FuelType As CheckBox = TryCast(item.FindControl("CHK_PersonSite"), CheckBox)
                If (CHK_FuelType.Checked = True Or (txtVehicleNumber.Text.Trim().ToLower().Contains("guest") = True And vehicleId = 0)) Then
                    Dim dr As DataRow = dtVehicleSite.NewRow()
                    dr("VehicleId") = result
                    dr("SiteID") = gv_Sites.DataKeys(item.RowIndex).Values("SiteID").ToString()
                    dtVehicleSite.Rows.Add(dr)

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        afterLinks = IIf(afterLinks = "", gv_Sites.DataKeys(item.RowIndex).Values("WifiSSId").ToString(), afterLinks & ";" & gv_Sites.DataKeys(item.RowIndex).Values("WifiSSId").ToString())
                    End If

                End If
            Next

            If result > 0 Then
                OBJMaster.InsertFuelTypeVehicleMapping(dtFuelVehicle, result)
                OBJMaster.InsertVehicleSiteMapping(dtVehicleSite, result)

                If (txtVehicleNumber.Text.Trim().ToLower().Contains("guest")) Then
                    OBJMaster.MapPersonToNewGuestVehicle(Convert.ToInt32(Session("PersonId")), result, DDL_Customer.SelectedValue)
                End If

                MapVehicleToHub(result, Convert.ToInt32(Session("PersonId")), DDL_Customer.SelectedValue)

            End If


            If result > 0 Then
                If (vehicleId > 0) Then
                    message.Visible = True
                    message.InnerText = "Record saved"

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateAfterData(vehicleId, False)
                        CSCommonHelper.WriteLog("Modified", "Vehicles", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    If (IsSaveAndAddNew = True) Then
                        Response.Redirect(String.Format("~/Master/Vehicle"))
                    End If
                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateAfterData(result, False)
                        CSCommonHelper.WriteLog("Added", "Vehicles", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    If (IsSaveAndAddNew = True) Then
                        hdfVehicleId.Value = result
                        Session("CustId") = DDL_Customer.SelectedValue
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenPersonVehicleMappingBox(1);", True)

                        'Response.Redirect(String.Format("~/Master/Vehicle"))
                    Else
                        Response.Redirect(String.Format("~/Master/Vehicle?VehicleId={0}&RecordIs=New", result))
                    End If
                End If


            Else
                If (vehicleId > 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateAfterData(vehicleId, False)
                        CSCommonHelper.WriteLog("Modified", "Vehicles", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Vehicle update failed")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Vehicle update failed, please try again"
                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateAfterData(result, False)
                        CSCommonHelper.WriteLog("Added", "Vehicles", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Vehicle Addition failed")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Vehicle Addition failed, please try again"
                End If

            End If
            txtVehicleNumber.Focus()
        Catch ex As Exception

            If (vehicleId > 0) Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateAfterData(vehicleId, False)
                    CSCommonHelper.WriteLog("Modified", "Vehicles", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Vehicle update failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Vehicle update failed, please try again"
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateAfterData(vehicleId, False)
                    CSCommonHelper.WriteLog("Added", "Vehicles", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Vehicle Addition failed. Exception is : " & ex.Message)
                End If
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Vehicle Addition failed, please try again"
            End If

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while saving record, please try again later."

            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
            txtVehicleNumber.Focus()
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Protected Sub btnClearFSTagMacAddress_Click(sender As Object, e As EventArgs)
        txtFSTagMacAddress.Text = ""
    End Sub
End Class
