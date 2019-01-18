Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports log4net
Imports log4net.Config

Public Class Company
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Company))

    Dim OBJMaster As MasterBAL
    Shared beforeData As String
    Shared afterCompanies As String

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
                    If Session("RoleName") = "SuperAdmin" Or Session("RoleName") = "GroupAdmin" Then
                        divMappCompanies.Visible = True
                    Else
                        divMappCompanies.Visible = False
                    End If

                    If (Not Request.QueryString("CustId") = Nothing And Not Request.QueryString("CustId") = "") Then
                        HDF_Custd.Value = Request.QueryString("CustId")
                        trPassword.Visible = False
                        trLabel.Visible = True
                        divDates.Visible = True
                        BindCustDetails(Request.QueryString("CustId"))
                        btnFirst.Visible = True
                        btnNext.Visible = True
                        btnprevious.Visible = True
                        btnLast.Visible = True
                        lblHeader.Text = "Edit Company Information"
                        hdfType.Value = "1"
                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"
                        End If
                        divCompanyNumber.Visible = True
                        hdrCompanyNumber.InnerText = "FS" & Request.QueryString("CustId").ToString()
                        If Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "GroupAdmin" Then
                            txtCustName.Enabled = False
                        End If


                        BindCompanies(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Request.QueryString("CustId").ToString())
                        BindCompaniesDataToCheckboxList(Request.QueryString("CustId").ToString())

                    Else
                        If Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "GroupAdmin" Then
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)
                            Return
                        End If
                        trLabel.Visible = False
                        btnFirst.Visible = False
                        btnNext.Visible = False
                        btnprevious.Visible = False
                        btnLast.Visible = False
                        lblof.Visible = False
                        lblHeader.Text = "Add Company Information"
                        hdfType.Value = "2"
                        divCompanyNumber.Visible = False
                        txtBeginningHostingDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                        txtEndingHostingDate.Text = DateTime.Now.AddYears(1).ToString("MM/dd/yyyy")
                        divDates.Visible = False
                        Session("CostingMethod") = "0"
                        divMappCompanies.Visible = False
                        BindCompanies(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)

                    End If


                    If Session("RoleName") = "SuperAdmin" Then
                        DivHideActive.Visible = False
                        DivShowActive.Visible = True
                    Else
                        DivHideActive.Visible = True
                        DivShowActive.Visible = False
                    End If

                    txtCustName.Focus()
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

                Else
                    'txtBeginningHostingDate.Text = Request.Form(txtBeginningHostingDate.UniqueID)
                    'txtEndingHostingDate.Text = Request.Form(txtEndingHostingDate.UniqueID)
                End If
                If Session("RoleName") <> "SuperAdmin" Then
                    divDates.Visible = False
                End If

            End If


        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Private Sub BindCustDetails(CustId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()
            dtCust = OBJMaster.GetCustomerId(CustId)
            Dim dscustomerAdmin = OBJMaster.GetCustomerAdmin(CustId)
            If dscustomerAdmin.Tables.Count > 0 Then
                If dscustomerAdmin.Tables(0).Rows.Count > 0 Then

                    Dim dtCustAdmin = dscustomerAdmin.Tables(0)
                    txtAdminUsername.Text = dtCustAdmin.Rows(0)("UserName").ToString()
                    HDF_UniqueUserId.Value = dtCustAdmin.Rows(0)("Id").ToString()
                    trPassword.Visible = False
                    trLabel.Visible = True
                Else
                    txtAdminUsername.Text = ""
                    HDF_UniqueUserId.Value = ""
                    trPassword.Visible = True
                    trLabel.Visible = False
                End If
            Else
                txtAdminUsername.Text = ""
                HDF_UniqueUserId.Value = ""
                trPassword.Visible = True
                trLabel.Visible = False
            End If


            Dim cnt As Integer = 0
            If (dtCust.Rows.Count > 0) Then
                Dim isValid As Boolean = False
                If (Session("RoleName") = "GroupAdmin") Then
                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                    For Each drCusts As DataRow In dtCustOld.Rows
                        If (drCusts("CustomerId") = dtCust.Rows(0)("CustomerId").ToString()) Then
                            isValid = True
                            Exit For
                        End If

                    Next
                End If

                If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                    If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtCust.Rows(0)("CustomerId").ToString()) Then

                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
                    End If

                End If

                txtCustName.Text = dtCust.Rows(0)("CustomerName").ToString()
                txtContactName.Text = dtCust.Rows(0)("ContactName").ToString()
                txtContactNumber.Text = dtCust.Rows(0)("ContactNumber").ToString()
                ' txtContactAddress.Text = dtCust.Rows(0)("ContactAddress").ToString()
                txtExportCode.Text = dtCust.Rows(0)("ExportCode").ToString()
                CHK_RequireLogin.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsLoginRequire").ToString())
                'CHK_RequireOdometer.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsOdometerRequire").ToString())
                chk_RequireDepartment.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsDepartmentRequire").ToString())
                chk_RequirePersonnelPIN.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsPersonnelPINRequire").ToString())
                chk_RequireOther.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsOtherRequire").ToString())
                chk_VehicleNumberRequire.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsVehicleNumberRequire").ToString())
                chkIsActive.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsCustomerActive").ToString())
                txtOtherLabel.Text = dtCust.Rows(0)("OtherLabel").ToString()
                txtStreetAddress.Text = dtCust.Rows(0)("StreetAddress").ToString()
                txtCity.Text = dtCust.Rows(0)("City").ToString()
                txtState.Text = dtCust.Rows(0)("State").ToString()
                txtZip.Text = dtCust.Rows(0)("Zip").ToString()
                txtCountry.Text = dtCust.Rows(0)("Country").ToString()


                DDL_Costing.SelectedValue = dtCust.Rows(0)("CostingMethod").ToString()
                hdfCostingMethodValue.Value = dtCust.Rows(0)("CostingMethod").ToString()
				Session("CostingMethod") = dtCust.Rows(0)("CostingMethod").ToString()
				rbl_FuelingType.SelectedValue = dtCust.Rows(0)("FuelingType").ToString()
				lblStartDate.Text = Convert.ToDateTime(dtCust.Rows(0)("StartDate").ToString()).ToString("MM/dd/yyyy")
				txtBeginningHostingDate.Text = Convert.ToDateTime(dtCust.Rows(0)("BeginningHostingDate").ToString()).ToString("MM/dd/yyyy")
                txtEndingHostingDate.Text = Convert.ToDateTime(dtCust.Rows(0)("EndingHostingDate").ToString()).ToString("MM/dd/yyyy")
                OBJMaster = New MasterBAL()

                Dim strConditions As String = ""

                If (Not Session("CompanyConditions") Is Nothing And Not Session("CompanyConditions") = "") Then
                    strConditions = Session("CompanyConditions")
                Else
                    If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                        strConditions = IIf(strConditions = "", " and CustomerId=" & Session("CustomerId"), strConditions & " and CustomerId=" & Session("CustomerId"))
                    End If
                End If

                HDF_TotalCust.Value = OBJMaster.GetCustIdByCondition(CustId, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)

                OBJMaster = New MasterBAL()
                Dim dtAllCust As DataTable = New DataTable()

                dtAllCust = OBJMaster.GetCustByConditions(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

                dtAllCust.PrimaryKey = New DataColumn() {dtAllCust.Columns(0)}
                Dim dr As DataRow = dtAllCust.Rows.Find(CustId)
                If Not IsDBNull(dr) Then

                    cnt = dtAllCust.Rows.IndexOf(dr) + 1

                End If
                If (HDF_TotalCust.Value = 1) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt >= HDF_TotalCust.Value) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                ElseIf (cnt <= 1) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt > 1 And cnt < HDF_TotalCust.Value) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                End If
                lblof.Text = cnt & " of " & HDF_TotalCust.Value.ToString()

            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Data not found. Please try again after some time."
            End If
            txtCustName.Focus()

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                beforeData = CreateData(CustId)
            End If


        Catch ex As Exception
            txtCustName.Focus()
            log.Error("Error occurred in BindCustDetails Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting company data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim CustId As Integer = 0
        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            If txtCustName.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Customer name and try again."
                txtCustName.Focus()
                Return
            End If

            If txtContactName.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Contact Name and try again."
                txtContactName.Focus()
                Return
            End If

            If txtContactNumber.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Contact Number and try again."
                txtContactNumber.Focus()
                Return
            End If

            If txtAdminUsername.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Contact Email and try again."
                txtAdminUsername.Focus()
                Return
            End If

            Dim expression As String = "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"

            Dim Match = Regex.Match(txtAdminUsername.Text, expression, RegexOptions.IgnoreCase)

            If Not Match.Success Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter valid email."
                txtAdminUsername.Focus()
                Return
            End If

            expression = "^[- +()]*[0-9][- +()0-9]*$"

            Match = Regex.Match(txtContactNumber.Text, expression, RegexOptions.IgnoreCase)

            If Not Match.Success Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter valid contact number."
                txtContactNumber.Focus()
                Return
            End If


            ' Check Tank without Delivery 
            Try
                If DDL_Costing.SelectedValue = "2" Then
                    If hdfCostingMethodValue.Value <> "2" Then

                        If hdfType.Value = "2" Then
                            lblErrorMessage.Text = "Please first create Company with any other Pricing method and Add delivery for evry Tank and then change Pricing method to Price Averaging."
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalMessage();LoadDateTimeControl();", True)
                            DDL_Costing.SelectedValue = Session("CostingMethod").ToString()
                            Return
                        Else
                            OBJMaster = New MasterBAL()
                            Dim dsTankNumber As DataSet = New DataSet()
                            dsTankNumber = OBJMaster.GetTanksWithoutDeliveryByCustomerId(Convert.ToInt32(HDF_Custd.Value))

                            If dsTankNumber IsNot Nothing Then
                                If dsTankNumber.Tables.Count > 0 Then
                                    If dsTankNumber.Tables(0).Rows.Count > 0 Then
                                        If dsTankNumber.Tables(0).Rows(0)(0).ToString() = "none" Then
                                            lblErrorMessage.Text = "Please add delivery for evry Tank and then change Pricing method to Price Averaging."
                                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalMessage();LoadDateTimeControl();", True)
                                            DDL_Costing.SelectedValue = Session("CostingMethod").ToString()
                                            Return
                                        ElseIf dsTankNumber.Tables(0).Rows(0)(0).ToString() <> "" Then
                                            lblErrorMessage.Text = "Please Add delivery for following Tank and then change Pricing method to Price Averaging. " & "<br>" & dsTankNumber.Tables(0).Rows(0)(0).ToString()
                                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalMessage();LoadDateTimeControl();", True)
                                            DDL_Costing.SelectedValue = Session("CostingMethod").ToString()
                                            Return
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Else
                    hdfCostingMethodValue.Value = DDL_Costing.SelectedValue.ToString()
                End If
            Catch ex As Exception
                log.Error(String.Format("Error Occurred while btnSave_Click(Check Tank without Delivery ). Error is {0}.", ex.Message))
                Return
            End Try

            'OBJMaster = New MasterBAL()
            'Dim dtSuperAdmin As DataTable = New DataTable()
            'dtSuperAdmin = OBJMaster.GetPersonnelByEmail(txtAdminUsername.Text)
            'If Not dtSuperAdmin Is Nothing And dtSuperAdmin.Rows.Count > 0 Then
            '	If (dtSuperAdmin.Rows(0)("Roles").ToString() = "SuperAdmin") Then
            '		ErrorMessage.Visible = True
            '		ErrorMessage.InnerText = "Contact Email is not customer admin, Please try again."
            '		ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
            '		Return
            '	End If
            'End If


            Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
            Dim roleManager = New RoleManager(Of IdentityRole)(New RoleStore(Of IdentityRole)(New ApplicationDbContext()))

            If (Not HDF_Custd.Value = Nothing And Not HDF_Custd.Value = "") Then

                CustId = HDF_Custd.Value

            End If

            Dim CheckCustExists As Boolean = False
            OBJMaster = New MasterBAL()
            CheckCustExists = OBJMaster.CustNameIsExists(txtCustName.Text, CustId)
            Dim result As Integer = 0

            If CheckCustExists = True Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Company name already exists."
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
                Return
            End If

            'check for user is already present in a system or not user

            'If (Session("RoleName") = "GroupAdmin") Then
            '    Dim dtCustOld As DataTable = New DataTable()

            '    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
            '    For Each drCusts As DataRow In dtCustOld.Rows
            '        If (drCusts("CustomerId") = dtVehicle.Rows(0)("CustomerId").ToString()) Then
            '            IsValid = True
            '            Exit For
            '        End If

            '    Next
            'End If



            Dim existingUser As DataTable = OBJMaster.GetPersonnelByEmail(txtAdminUsername.Text)
            If Not existingUser Is Nothing And existingUser.Rows.Count > 0 Then

                Dim isValid As Boolean = False

                If (existingUser.Rows(0)("Roles").ToString() = "GroupAdmin") Then
                    If (CustId = 0 And txtAdminUsername.Text = Session("PersonEmail")) Then
                        isValid = True
                    Else
                        Dim dtCustOld As DataTable = New DataTable()
                        dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(existingUser.Rows(0)("PersonId").ToString()), existingUser.Rows(0)("RoleId").ToString(), 0)
                        For Each drCusts As DataRow In dtCustOld.Rows
                            If (drCusts("CustomerId") = CustId) Then
                                isValid = True
                                Exit For
                            End If

                        Next
                    End If
                End If

                If (Not existingUser.Rows(0)("CustomerId").ToString() = CustId And existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin" And isValid = False) Then
                    ErrorMessage.Visible = True
					ErrorMessage.InnerText = “Cannot add Company: Contact email already exists”
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
                    Return
                End If

                'return if entered existing Contact Email is not customerAdmin
                If Not existingUser.Rows(0)("Roles").ToString() = "CustomerAdmin" And Not existingUser.Rows(0)("Roles").ToString() = "SuperAdmin" And Not existingUser.Rows(0)("Roles").ToString() = "GroupAdmin" Then
                    ErrorMessage.Visible = True
					ErrorMessage.InnerText = “Cannot add Company: Contact email is not customer admin.”
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
                    Return
                End If
            ElseIf (HDF_UniqueUserId.Value <> "") Then
                ErrorMessage.Visible = True
				ErrorMessage.InnerText = “Cannot add Company:Contact email is not found in system”
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
                Return
            End If

            'added contact email directly to customer table so commented code from here.
            'If Not CustId = 0 Then

            '    Dim oldCustomerAdminUser = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" And cust.CustomerId=CAST(" + CustId.ToString() + " as nvarchar) And IsMainCustomerAdmin = 1", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), False, True)
            '    If Not oldCustomerAdminUser Is Nothing And oldCustomerAdminUser.Rows.Count > 0 Then
            '        Dim customerAdminUserOld = New ApplicationUser()
            '        customerAdminUserOld = manager.FindById(oldCustomerAdminUser.Rows(0)("Id").ToString())

            '        'if entered email and old email is not same then set IsMainCustomerAdmin=0 to old MainCustomerAdmin 
            '        If Not customerAdminUserOld.Email = txtAdminUsername.Text Then
            '            customerAdminUserOld.IsMainCustomerAdmin = False
            '            Dim identityResultForOldUser As IdentityResult
            '            identityResultForOldUser = New IdentityResult()
            '            identityResultForOldUser = manager.Update(customerAdminUserOld)
            '            If identityResultForOldUser.Succeeded Then
            '                'success
            '            End If
            '        End If
            '    End If
            'End If

            Dim custRole As IdentityRole

            If Not existingUser Is Nothing And existingUser.Rows.Count > 0 Then
                custRole = roleManager.FindByName(existingUser.Rows(0)("Roles").ToString())
            Else
                custRole = roleManager.FindByName("CustomerAdmin")
            End If

            OBJMaster = New MasterBAL()
            Dim identityResult As IdentityResult
            Dim user = New ApplicationUser()
            Dim resultOfPassword As IdentityResult
            Dim errorStrForPasswordReset As String
            errorStrForPasswordReset = ""

            Dim otherLabel As String = ""

            If (txtOtherLabel.Text = "") Then
                otherLabel = "Other"
            Else
                otherLabel = txtOtherLabel.Text
            End If

            Dim BeginningHostingDate As DateTime
            Dim EndingHostingDate As DateTime

            If Request.Form(txtBeginningHostingDate.UniqueID) <> "" Then
                BeginningHostingDate = Request.Form(txtBeginningHostingDate.UniqueID)
            Else
                BeginningHostingDate = txtBeginningHostingDate.Text
            End If
            If Request.Form(txtEndingHostingDate.UniqueID) <> "" Then
                EndingHostingDate = Request.Form(txtEndingHostingDate.UniqueID)
            Else
                EndingHostingDate = txtEndingHostingDate.Text
            End If

            If EndingHostingDate < BeginningHostingDate Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Ending hosting date must be greater than beginning hosting date."
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
                Return
            End If

			'save company
			result = OBJMaster.SaveUpdateCustomer(CustId, txtCustName.Text, txtContactName.Text, txtContactNumber.Text, "", txtExportCode.Text, Convert.ToInt32(Session("PersonId")), CHK_RequireLogin.Checked, False,
												  chk_RequireDepartment.Checked, chk_RequirePersonnelPIN.Checked, chk_RequireOther.Checked, otherLabel, chkIsActive.Checked, chk_VehicleNumberRequire.Checked,
												  txtStreetAddress.Text, txtCity.Text, txtState.Text, txtZip.Text, txtCountry.Text, txtAdminUsername.Text, rbl_FuelingType.SelectedValue,
												  Convert.ToInt32(DDL_Costing.SelectedValue), BeginningHostingDate, EndingHostingDate)

			''Save Personal Vehicle Mapping
			'If chkAssignPerToVeh.Checked And result > 0 Then
			'    OBJMaster.SavePersonnalVehicleMappingAgainstCustomer(CustId, Convert.ToInt32(Session("PersonId")))
			'End If

			If result > 0 Then
                Session("CostingMethod") = DDL_Costing.SelectedValue.ToString()
                HDF_Custd.Value = result

                If Not existingUser Is Nothing And existingUser.Rows.Count > 0 Then
                    If Convert.ToInt32(existingUser.Rows(0)("CustomerId")) = result Then
                        HDF_UniqueUserId.Value = existingUser.Rows(0)("Id").ToString()
                        CustId = result
                    Else
                        HDF_UniqueUserId.Value = existingUser.Rows(0)("Id").ToString()
                    End If
                End If



                ''insert company Group admin person mapping
                'Dim dtCompanies As DataTable = New DataTable()
                'dtCompanies.Columns.Add("CompanyId", System.Type.[GetType]("System.Int32"))
                'dtCompanies.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
                'dtCompanies.Columns.Add("MappedOn", System.Type.[GetType]("System.DateTime"))
                'dtCompanies.Columns.Add("MappedBy", System.Type.[GetType]("System.Int32"))
                'dtCompanies.Columns.Add("ParentCompanyId", System.Type.[GetType]("System.Int32"))

                'Dim dtperson As DataTable = New DataTable()
                'dtperson = OBJMaster.GetPersonalDetails(" where ANU.CustomerId = " & HDF_Custd.Value & " and ANU.RoleId = '00cee925-1cad-4650-9dc3-d1fdf74d1f5c'")

                'If dtperson.Rows.Count > 0 Then
                '    For i = 0 To dtperson.Rows.Count - 1
                '        For Each item As GridViewRow In GV_Companies.Rows
                '            Dim CHK_MappCompanies As CheckBox = TryCast(item.FindControl("CHK_MappCompanies"), CheckBox)
                '            If (CHK_MappCompanies.Checked = True) Then
                '                Dim dr As DataRow = dtCompanies.NewRow()
                '                dr("CompanyId") = GV_Companies.DataKeys(item.RowIndex).Values("CustomerId").ToString()
                '                dr("PersonId") = dtperson.Rows(i)("PersonId")
                '                dr("MappedOn") = DateTime.Now
                '                dr("MappedBy") = Convert.ToInt32(Session("PersonId"))
                '                dr("ParentCompanyId") = HDF_Custd.Value
                '                dtCompanies.Rows.Add(dr)
                '                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                '                    Dim ValueToStore = "Company - " & GV_Companies.DataKeys(item.RowIndex).Values("CustomerName").ToString() & ", PersonId - " & dtperson.Rows(i)("PersonId")
                '                    afterCompanies = IIf(afterCompanies = "", ValueToStore, afterCompanies & ";" & ValueToStore)
                '                End If
                '            End If
                '        Next
                '    Next

                'End If
                Dim companies As String = ""
                Dim removeCompanies As String = ""
                For Each item As GridViewRow In GV_Companies.Rows
                    Dim CHK_MappCompanies As CheckBox = TryCast(item.FindControl("CHK_MappCompanies"), CheckBox)
                    If (CHK_MappCompanies.Checked = True) Then
                        companies = IIf(companies = "", GV_Companies.DataKeys(item.RowIndex).Values("CustomerId").ToString(), companies & "," & GV_Companies.DataKeys(item.RowIndex).Values("CustomerId").ToString())
                    Else
                        removeCompanies = IIf(removeCompanies = "", GV_Companies.DataKeys(item.RowIndex).Values("CustomerId").ToString(), removeCompanies & "," & GV_Companies.DataKeys(item.RowIndex).Values("CustomerId").ToString())

                    End If
                Next




                If CustId = 0 Then

                    Try

                        Dim dtPerson As DataTable = New DataTable()
                        dtPerson = OBJMaster.GetPersonnelByPersonIdAndId(Session("PersonId"), Session("UniqueId"))


                        OBJMaster.InsertParentChildCompanyMapping(dtPerson.Rows(0)("CustomerId"), HDF_Custd.Value, Convert.ToInt32(Session("PersonId")), "")

                    Catch ex As Exception
                        log.Info("Error occured in InsertParentChildCompanyMapping. exception is " & ex.Message)
                    End Try

                    'add user name and password to aspnetusers table with IsMainCustomerAdmin flag true

                    If HDF_UniqueUserId.Value = "" Then

                        user = New ApplicationUser() With {
                       .UserName = txtAdminUsername.Text,
                       .Email = txtAdminUsername.Text,
                       .PersonName = txtContactName.Text,
                       .PhoneNumber = txtContactNumber.Text,
                       .CreatedDate = DateTime.Now,
                       .CreatedBy = Convert.ToInt32(Session("PersonId")),
                       .IsDeleted = False,
                       .RoleId = custRole.Id,
                        .IsApproved = True,
                        .ApprovedBy = Convert.ToInt32(Session("PersonId")),
                        .ApprovedOn = DateTime.Now,
                        .CustomerId = Convert.ToInt32(result),
                       .IsMainCustomerAdmin = True, 'now not using this field to maintain Main Customer Admin, added contact email directly to customer table.
                       .SoftUpdate = "N",
                        .SendTransactionEmail = False,
                        .RequestFrom = "W",
                       .IMEI_UDID = "",
                        .IsFluidSecureHub = False,
                       .IsUserForHub = False,
                       .PasswordResetDate = DateTime.Now,
                       .FOBNumber = "",
                        .AdditionalEmailId = "",
                        .IsPersonnelPINRequire = False,
                        .BluetoothCardReader = "",
                        .PrinterName = "",
                        .PrinterMacAddress = "",
                        .HubSiteName = "",
                       .BluetoothCardReaderMacAddress = "",
       .LFBluetoothCardReader = "",
       .LFBluetoothCardReaderMacAddress = "",
           .VeederRootMacAddress = "",
       .CollectDiagnosticLogs = False,
      .IsVehicleHasFob = False,
      .IsPersonHasFob = False,
       .IsTermConditionAgreed = False,
       .DateTimeTermConditionAccepted = Nothing,
           .IsGateHub = False,
           .IsVehicleNumberRequire = False,
           .HubAddress = "",
           .IsLogging = 0,
           .IsSpecialImport = 0
                   }

                        identityResult = New IdentityResult()
                        identityResult = manager.Create(user, txtAdminPassword.Text)
                        If identityResult.Succeeded Then

                        End If

                    Else

                        user = manager.FindById(HDF_UniqueUserId.Value)

                        'user.UserName = txtAdminUsername.Text
                        'user.Email = txtAdminUsername.Text
                        'user.PersonName = txtContactName.Text
                        'user.PhoneNumber = txtContactNumber.Text
                        user.LastModifiedDate = DateTime.Now
                        user.LastModifiedBy = Convert.ToInt32(Session("PersonId"))
                        user.IsDeleted = False
                        user.ApprovedBy = Convert.ToInt32(Session("PersonId"))
                        'user.RoleId = custRole.Id
                        user.IsApproved = True
                        If Not existingUser Is Nothing And existingUser.Rows.Count > 0 Then
                            If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin" And existingUser.Rows(0)("Roles").ToString() <> "GroupAdmin") Then
                                user.CustomerId = Convert.ToInt32(result)
                            End If
                        End If
                        user.ApprovedOn = DateTime.Now
                        'user.SoftUpdate = "N"
                        'user.IsMainCustomerAdmin = True 'added contact email directly to customer table so commented code from here.
                        user.SendTransactionEmail = IIf(user.SendTransactionEmail = Nothing, False, user.SendTransactionEmail)
                        user.RequestFrom = IIf(user.RequestFrom = Nothing, "W", user.RequestFrom)

                        identityResult = New IdentityResult()

                        identityResult = manager.Update(user)
                        If identityResult.Succeeded Then
                            'update password
                            If Not txtAdminPassword.Text = "" Then
                                If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin" And existingUser.Rows(0)("Roles").ToString() <> "GroupAdmin") Then

                                    Dim code = manager.GeneratePasswordResetToken(HDF_UniqueUserId.Value)
                                    If Not code = Nothing Then
                                        resultOfPassword = manager.ResetPassword(HDF_UniqueUserId.Value, code, txtAdminPassword.Text)
                                        If resultOfPassword.Succeeded Then

                                        Else

                                            If (resultOfPassword.Errors.ToList().First().ToLower().Contains("password")) Then
                                                errorStrForPasswordReset = "Password MUST be minimum 6 characters Long And contain one (1) Of the following: Upper Case letter (A-Z), lower case letter (a-z), special character (!@#$%^&*), number (0-9)"
                                            Else
                                                errorStrForPasswordReset = resultOfPassword.Errors.ToList().First().ToString()
                                            End If

                                        End If
                                    End If

                                End If

                            End If
                        End If

                    End If

                Else

                    OBJMaster.InsertParentChildCompanyMapping(HDF_Custd.Value, companies, Convert.ToInt32(Session("PersonId")), removeCompanies)

                    If HDF_UniqueUserId.Value = "" Then
                        user = New ApplicationUser() With {
                  .UserName = txtAdminUsername.Text,
                  .Email = txtAdminUsername.Text,
                  .PersonName = txtContactName.Text,
                  .PhoneNumber = txtContactNumber.Text,
                  .CreatedDate = DateTime.Now,
                  .CreatedBy = Convert.ToInt32(Session("PersonId")),
                  .IsDeleted = False,
                  .RoleId = custRole.Id,
                   .IsApproved = True,
                   .ApprovedBy = Convert.ToInt32(Session("PersonId")),
                   .ApprovedOn = DateTime.Now,
                   .CustomerId = Convert.ToInt32(result),
                  .IsMainCustomerAdmin = True, 'now not using this field to maintain Main Customer Admin, added contact email directly to customer table.
                  .SoftUpdate = "N",
                  .SendTransactionEmail = False,
                  .RequestFrom = "W",
                  .IMEI_UDID = "",
                 .IsFluidSecureHub = False,
                .IsUserForHub = False,
                  .PasswordResetDate = DateTime.Now,
                  .FOBNumber = "",
                  .AdditionalEmailId = "",
                 .IsPersonnelPINRequire = False,
                .BluetoothCardReader = "",
                .PrinterName = "",
                .PrinterMacAddress = "",
                .HubSiteName = "",
                  .BluetoothCardReaderMacAddress = "",
       .LFBluetoothCardReader = "",
       .LFBluetoothCardReaderMacAddress = "",
           .VeederRootMacAddress = "",
       .CollectDiagnosticLogs = False,
       .IsVehicleHasFob = False,
       .IsPersonHasFob = False,
       .IsTermConditionAgreed = False,
       .DateTimeTermConditionAccepted = Nothing,
         .IsGateHub = False,
         .IsVehicleNumberRequire = False,
         .HubAddress = "",
           .IsLogging = 0,
                        .IsSpecialImport = 0
              }
                        identityResult = New IdentityResult()
                        identityResult = manager.Create(user, txtAdminPassword.Text)
                        If identityResult.Succeeded Then

                        End If
                    Else
                        user = manager.FindById(HDF_UniqueUserId.Value)

                        'user.UserName = txtAdminUsername.Text
                        'user.Email = txtAdminUsername.Text
                        'user.PersonName = txtContactName.Text
                        'user.PhoneNumber = txtContactNumber.Text
                        user.LastModifiedDate = DateTime.Now
                        user.LastModifiedBy = Convert.ToInt32(Session("PersonId"))
                        'user.IsDeleted = False
                        user.ApprovedBy = Convert.ToInt32(Session("PersonId"))
                        'user.RoleId = custRole.Id

                        user.IsApproved = True
                        user.ApprovedOn = DateTime.Now
                        'user.SoftUpdate = "N"
                        If Not existingUser Is Nothing And existingUser.Rows.Count > 0 Then
                            If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin" And existingUser.Rows(0)("Roles").ToString() <> "GroupAdmin") Then
                                user.CustomerId = Convert.ToInt32(result)
                            End If
                        End If
                        'user.IsMainCustomerAdmin = True 'added contact email directly To customer table so commented code from here
                        user.SendTransactionEmail = IIf(user.SendTransactionEmail = Nothing, False, user.SendTransactionEmail)
                        user.RequestFrom = IIf(user.RequestFrom = Nothing, "W", user.RequestFrom)

                        identityResult = New IdentityResult()

                        identityResult = manager.Update(user)
                        If identityResult.Succeeded Then
                            'update password
                            If Not txtAdminPassword.Text = "" Then
                                If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin" And existingUser.Rows(0)("Roles").ToString() <> "GroupAdmin") Then

                                    Dim code = manager.GeneratePasswordResetToken(HDF_UniqueUserId.Value)
                                    If Not code = Nothing Then
                                        resultOfPassword = manager.ResetPassword(HDF_UniqueUserId.Value, code, txtAdminPassword.Text)
                                        If resultOfPassword.Succeeded Then

                                        Else

                                            If (resultOfPassword.Errors.ToList().First().ToLower().Contains("password")) Then
                                                errorStrForPasswordReset = "Password MUST be minimum 6 characters long and contain one (1) of the following: Upper Case letter (A-Z), lower case letter (a-z), special character (!@#$%^&*), number (0-9)"
                                            Else
                                                errorStrForPasswordReset = resultOfPassword.Errors.ToList().First().ToString()
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If


                End If
            Else

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Company update failed, Please try again."
                Return
            End If

            If result > 0 And identityResult.Succeeded And errorStrForPasswordReset = "" Then
                If (CustId > 0) Then

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result)
                        CSCommonHelper.WriteLog("Modified", "Company", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                    message.Visible = True
                    message.InnerText = "Record Saved."
                    trPassword.Visible = False
                    trLabel.Visible = True
                Else

                    OBJMaster = New MasterBAL()
                    Dim dtPerson As DataTable = New DataTable()
                    dtPerson = OBJMaster.GetPersonDetailByUniqueUserId(user.Id)
                    Dim PersonId = dtPerson.Rows(0)("PersonId")

                    Dim dtAllVehicles As DataTable = New DataTable()
                    OBJMaster = New MasterBAL()
                    dtAllVehicles = OBJMaster.GetVehicleByCondition(" and v.VehicleNumber like '%guest%' and v.CustomerId=" + result.ToString(), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

                    Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

                    dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
                    dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
                    dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
                    dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))


                    For Each item As DataRow In dtAllVehicles.Rows

                        If (item("VehicleNumber").ToString().ToLower().Contains("guest") = True) Then
                            Dim dr As DataRow = dtVehicle.NewRow()
                            dr("PersonId") = PersonId
                            dr("VehicleId") = item("VehicleId").ToString()
                            dr("CreatedDate") = DateTime.Now
                            dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
                            dtVehicle.Rows.Add(dr)
                        End If
                    Next

                    OBJMaster.InsertPersonVehicleMapping(dtVehicle, PersonId)

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result)
                        CSCommonHelper.WriteLog("Added", "Company", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                    Response.Redirect(String.Format("~/Master/Company?CustId={0}&RecordIs=New", result))

                End If
            ElseIf result > 0 And identityResult.Succeeded = False Then
                'company added but user not saved
                Dim strIdentityError As String
                If identityResult.Errors.FirstOrDefault().ToString().Contains("is already taken") Then
                    strIdentityError = " Username " + txtAdminUsername.Text + " is already in use."
                ElseIf (identityResult.Errors.FirstOrDefault().ToString().ToLower().Contains("password")) Then
                    strIdentityError = "Password MUST be minimum 6 characters long and contain one (1) of the following: Upper Case letter (A-Z), lower case letter (a-z), special character (!@#$%^&*), number (0-9)"
                Else
                    strIdentityError = " " + identityResult.Errors.FirstOrDefault()
                End If

                If Not errorStrForPasswordReset = "" Then
                    errorStrForPasswordReset = errorStrForPasswordReset.Replace("'", " ")
                    strIdentityError += errorStrForPasswordReset
                End If

                If strIdentityError.Length > 0 Then
                    strIdentityError = strIdentityError.Replace("'", " ")
                End If
                If (CustId > 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = strIdentityError + " Please try again."
                Else
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = strIdentityError + " Please try again."
                End If
            Else
                If (CustId > 0) Then

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(CustId)
                        CSCommonHelper.WriteLog("Modified", "Company", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Company update failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Company update failed, Please try again."
                Else

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result)
                        CSCommonHelper.WriteLog("Added", "Company", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Company update failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Company addition failed, Please try again."
                End If

            End If
        Catch ex As Exception

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                If (CustId > 0) Then
                    Dim writtenData As String = CreateData(CustId)
                    CSCommonHelper.WriteLog("Added", "Company", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Company update failed. Exception is : " & ex.Message)
                Else
                    Dim writtenData As String = CreateData(CustId)
                    CSCommonHelper.WriteLog("Modified", "Company", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Company adding failed. Exception is : " & ex.Message)
                End If
            End If

            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
        Finally
            txtCustName.Focus()
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllCompanies?Filter=Filter")
    End Sub

    Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
        Try

            Dim strConditions As String = ""

            If (Not Session("CompanyConditions") Is Nothing And Not Session("CompanyConditions") = "") Then
                strConditions = Session("CompanyConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and CustomerId=" & Session("CustomerId"), strConditions & " and CustomerId=" & Session("CustomerId"))
                End If
            End If

            Dim CurrentCustId As Integer = HDF_Custd.Value

            OBJMaster = New MasterBAL()
            Dim CustId As Integer = OBJMaster.GetCustIdByCondition(CurrentCustId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            HDF_Custd.Value = CustId
            BindCustDetails(CustId)
            txtCustName.Focus()
        Catch ex As Exception
            log.Error("Error occurred in First_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try
    End Sub

    Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
        Try

            Dim strConditions As String = ""

            If (Not Session("CompanyConditions") Is Nothing And Not Session("CompanyConditions") = "") Then
                strConditions = Session("CompanyConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and CustomerId=" & Session("CustomerId"), strConditions & " and CustomerId=" & Session("CustomerId"))
                End If
            End If

            Dim CurrentCustId As Integer = HDF_Custd.Value

            OBJMaster = New MasterBAL()
            Dim CustId As Integer = OBJMaster.GetCustIdByCondition(CurrentCustId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            HDF_Custd.Value = CustId
            BindCustDetails(CustId)


        Catch ex As Exception
            log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Try

            Dim strConditions As String = ""

            If (Not Session("CompanyConditions") Is Nothing And Not Session("CompanyConditions") = "") Then
                strConditions = Session("CompanyConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and CustomerId=" & Session("CustomerId"), strConditions & " and CustomerId=" & Session("CustomerId"))
                End If
            End If

            Dim CurrentCustId As Integer = HDF_Custd.Value

            OBJMaster = New MasterBAL()
            Dim CustId As Integer = OBJMaster.GetCustIdByCondition(CurrentCustId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            HDF_Custd.Value = CustId
            BindCustDetails(CustId)


        Catch ex As Exception
            log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
        Try

            Dim strConditions As String = ""

            If (Not Session("CompanyConditions") Is Nothing And Not Session("CompanyConditions") = "") Then
                strConditions = Session("CompanyConditions")
            Else
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    strConditions = IIf(strConditions = "", " and CustomerId=" & Session("CustomerId"), strConditions & " and CustomerId=" & Session("CustomerId"))
                End If
            End If

            Dim CurrentCustId As Integer = HDF_Custd.Value

            OBJMaster = New MasterBAL()
            Dim CustId As Integer = OBJMaster.GetCustIdByCondition(CurrentCustId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
            HDF_Custd.Value = CustId
            BindCustDetails(CustId)


        Catch ex As Exception
            log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Function CreateData(CompanyId As Integer) As String
        Try

            Dim data As String = ""
            'If (isBefore = True) Then
            '    data = "CompanyId = " & CompanyId & " ; "
            'End If
            Dim BeginningHostingDate As String
            Dim EndingHostingDate As String

            If Request.Form(txtBeginningHostingDate.UniqueID) <> "" Then
                BeginningHostingDate = Request.Form(txtBeginningHostingDate.UniqueID)
            Else
                BeginningHostingDate = txtBeginningHostingDate.Text
            End If
            If Request.Form(txtEndingHostingDate.UniqueID) <> "" Then
                EndingHostingDate = Request.Form(txtEndingHostingDate.UniqueID)
            Else
                EndingHostingDate = txtEndingHostingDate.Text
            End If

            data = "CompanyId = " & CompanyId & " ; " &
                "Company Name = " & txtCustName.Text.Replace(",", " ") & " ; " &
                        "Contact Name = " & txtContactNumber.Text.Replace(",", " ") & " ; " &
                        "Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
                        "Require Login = " & IIf(CHK_RequireLogin.Checked = True, "Yes", "No") & " ; " &
                        "Require Department = " & IIf(chk_RequireDepartment.Checked = True, "Yes", "No") & " ; " &
                        "Require Personnel PIN = " & IIf(chk_RequirePersonnelPIN.Checked = True, "Yes", "No") & " ; " &
                        "Require Other = " & IIf(chk_RequireOther.Checked = True, "Yes", "No") & " ; " &
                        "Vehicle Number Require = " & IIf(chk_VehicleNumberRequire.Checked = True, "Yes", "No") & " ; " &
                        "Other label = " & txtOtherLabel.Text.Replace(",", " ") & " ; " &
                        "Contact Phone Number = " & txtContactNumber.Text & " ; " &
                        "Contact Email = " & txtAdminUsername.Text & " ; " &
                        "Costing Method = " & DDL_Costing.SelectedItem.ToString().Replace(",", " ") & " ; " &
                        "Companies bind to Group Admin = " & afterCompanies & " ; " &
                        "Start Date = " & lblStartDate.Text & " ; " &
                        "Beginning Hosting Date = " & BeginningHostingDate & " ; " &
                        "Ending Hosting Date = " & EndingHostingDate & " ; " &
                        "Street Address = " & txtStreetAddress.Text.Replace(",", " ") & " ; " &
                        "City = " & txtCity.Text.Replace(",", " ") & " ; " &
                        "State = " & txtState.Text.Replace(",", " ") & " ; " &
                        "Zip = " & txtZip.Text.Replace(",", " ") & " ; " &
                        "Country = " & txtCountry.Text.Replace(",", " ") & "  "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub DDL_Costing_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            If Session("CostingMethod") = "0" Then
                If (DDL_Costing.SelectedValue.ToString() = "1") Then
                    lblWarningMessage.Text = "To use your new pricing, assign each product a price in the Items -> <a href='/Master/AllFuels.aspx'>Products</a> "
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                ElseIf (DDL_Costing.SelectedValue.ToString() = "2") Then
                    lblWarningMessage.Text = "To use your new pricing, ensure that a delivery with a price has been assigned to each tank. Do this Under Reconciliation -> <a href='/Master/AllTankInventoryReconciliation.aspx?Type=Level'>Tank Inventory Reconciliation</a>, and Select 'Add new Tank Inventory Reconciliation'."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                ElseIf (DDL_Costing.SelectedValue.ToString() = "3") Then
                    lblWarningMessage.Text = "To use your new pricing, ensure that a delivery with a price has been assigned to each tank. Do this Under Reconciliation -> <a href='/Master/AllTankInventoryReconciliation.aspx?Type=Level'>Tank Inventory Reconciliation</a>, and Select 'Add new Tank Inventory Reconciliation'."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                End If
            ElseIf Session("CostingMethod") = "1" Then
                If (DDL_Costing.SelectedValue.ToString() = "2") Then
                    lblWarningMessage.Text = "To use your new pricing, ensure that a delivery with a price has been assigned to each tank. Do this Under Reconciliation -> <a href='/Master/AllTankInventoryReconciliation.aspx?Type=Level'>Tank Inventory Reconciliation</a>, and Select 'Add new Tank Inventory Reconciliation'. Past transactions will not be affected."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                ElseIf (DDL_Costing.SelectedValue.ToString() = "3") Then
                    lblWarningMessage.Text = "To use your new pricing, ensure that a delivery with a price has been assigned to each tank. Do this Under Reconciliation -> <a href='/Master/AllTankInventoryReconciliation.aspx?Type=Level'>Tank Inventory Reconciliation</a>, and Select 'Add new Tank Inventory Reconciliation'. Past transactions will not be affected."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                End If
            ElseIf Session("CostingMethod") = "2" Then
                If (DDL_Costing.SelectedValue.ToString() = "1") Then
                    lblWarningMessage.Text = "To use your new pricing, assign each product a price in the Items -> <a href='/Master/AllFuels.aspx'>Products</a>. <br/> Past transactions will not be affected."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                ElseIf (DDL_Costing.SelectedValue.ToString() = "3") Then
                    lblWarningMessage.Text = "If your Price Averaging has been working, you do not need any further setup to switch to FIFO pricing. Otherwise, to use your new pricing, ensure that a delivery with a price has been assigned to each tank. Do this Under Reconciliation -> <a href='/Master/AllTankInventoryReconciliation.aspx?Type=Level'>Tank Inventory Reconciliation</a>, and Select 'Add new Tank Inventory Reconciliation'. <br/> Past transactions will not be affected."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                End If
            ElseIf Session("CostingMethod") = "3" Then
                If (DDL_Costing.SelectedValue.ToString() = "1") Then
                    lblWarningMessage.Text = "To use your new pricing, assign each product a price in the Items -> <a href='/Master/AllFuels.aspx'>Products</a>. <br/> Past transactions will not be affected."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                ElseIf (DDL_Costing.SelectedValue.ToString() = "2") Then
                    lblWarningMessage.Text = "If your FIFO costing has been working, you do not need any further setup to switch to Price Averaging. Otherwise, to use your new pricing, ensure that a delivery with a price has been assigned to each tank. Do this Under Reconciliation -> <a href='/Master/AllTankInventoryReconciliation.aspx?Type=Level'>Tank Inventory Reconciliation</a>, and Select 'Add new Tank Inventory Reconciliation'. <br/> Past transactions will not be affected."
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
                End If
            End If

            If DDL_Costing.SelectedValue.ToString() <> Convert.ToString(Session("CostingMethod")) And DDL_Costing.SelectedValue.ToString() <> "0" Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
            Else
                Page.Validate()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
            End If
            txtAdminPassword.Attributes("value") = txtAdminPassword.Text
            txtConfirmPassword.Attributes("value") = txtConfirmPassword.Text
        Catch ex As Exception
            log.Error("Error occurred in DDL_Costing_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btn_EnableAllVehOdo_Click(sender As Object, e As EventArgs)
        Try
            hdfEnableDisable.Value = "1"
            'lblVehodo.Text = "Are you sure you want to Enable odometer for all vehicles in this Company ?"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenVehOdoModelBox();", True)
        Catch ex As Exception
            log.Error("Error occurred in btn_EnableAllVehOdo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btn_DisableAllVehOdo_Click(sender As Object, e As EventArgs)
        Try
            hdfEnableDisable.Value = "2"
            'lblVehodo.Text = "Are you sure you want to Disable odometer for all vehicles in this Company ?"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenVehOdoModelBox();", True)
        Catch ex As Exception
            log.Error("Error occurred in btn_DisableAllVehOdo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnVehOdoOk_Click(sender As Object, e As EventArgs)
        Try
            Dim flagEnableDisable As Boolean = True
            If hdfEnableDisable.Value = "2" Then
                flagEnableDisable = False
                hdfEnableDisable.Value = "0"
            ElseIf hdfEnableDisable.Value = "1" Then
                flagEnableDisable = True
                hdfEnableDisable.Value = "0"
            Else
                Return
            End If

            OBJMaster = New MasterBAL()
            Dim result As Integer = OBJMaster.SetEnableDisableVehOdoByCustID(flagEnableDisable, HDF_Custd.Value)

            If result = 1 Then
                message.Visible = True
                message.InnerText = "Record Saved."
            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Error occurred while updating data, please try again later."
            End If

        Catch ex As Exception
            log.Error("Error occurred in btnVehOdoOk_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnCancelMappCompanies_Click(sender As Object, e As EventArgs)
        Try

        Catch ex As Exception
            log.Error("Error occurred in btnCancelMappCompanies_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
        End Try
    End Sub

    Private Sub BindCompanies(PersonId As Integer, Roleid As String, CustomerID As Integer)
        Try
            Dim dtCompanies As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dtCompanies = OBJMaster.GetCustomerDetailsByPersonID(PersonId, Roleid, 0)


            Dim dtr() As DataRow = dtCompanies.Select("CustomerId = " & CustomerID)
            For Each dtrow As DataRow In dtr
                dtrow.Delete()
            Next
            dtCompanies.AcceptChanges()

            GV_Companies.DataSource = dtCompanies
            GV_Companies.DataBind()

        Catch ex As Exception

            log.Error("Error occurred in BindCompanies Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindCompaniesDataToCheckboxList(PersonId As Integer)
        Try
            'beforeFSlinks = ""
            OBJMaster = New MasterBAL()
            Dim dtChildCompanyMapping As DataTable = New DataTable()

            dtChildCompanyMapping = OBJMaster.GetParentChildCompanyMapping(PersonId)
            If dtChildCompanyMapping IsNot Nothing Then
                For Each dr As DataRow In dtChildCompanyMapping.Rows

                    For Each rows As GridViewRow In GV_Companies.Rows
                        If (dr("ChildCompanyId") = GV_Companies.DataKeys(rows.RowIndex).Values("CustomerId").ToString()) Then
                            TryCast(rows.FindControl("CHK_MappCompanies"), CheckBox).Checked = True

                            'Dim CustomerName = GV_Companies.DataKeys(rows.RowIndex).Values("CustomerName").ToString()
                            'If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                            '    beforeCompanies = IIf(beforeCompanies = "", CustomerName, beforeCompanies & ";" & CustomerName)
                            'End If
                        End If
                    Next
                Next
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindCompaniesDataToCheckboxList Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting BindCompaniesDataToCheckboxList, please try again later."

        End Try

    End Sub

End Class
