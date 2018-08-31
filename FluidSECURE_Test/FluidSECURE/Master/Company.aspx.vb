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
                        If Session("RoleName") <> "SuperAdmin" Then
                            txtCustName.Enabled = False
                        End If
                    Else
                        If Session("RoleName") <> "SuperAdmin" Then
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
					'Session("CostingMethod") = ""
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

				If (Not Session("RoleName") = "SuperAdmin") Then

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
				txtContactAddress.Text = dtCust.Rows(0)("ContactAddress").ToString()
				txtExportCode.Text = dtCust.Rows(0)("ExportCode").ToString()
				CHK_RequireLogin.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsLoginRequire").ToString())
				'CHK_RequireOdometer.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsOdometerRequire").ToString())
				chk_RequireDepartment.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsDepartmentRequire").ToString())
				chk_RequirePersonnelPIN.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsPersonnelPINRequire").ToString())
                chk_RequireOther.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsOtherRequire").ToString())
                chkIsActive.Checked = Convert.ToBoolean(dtCust.Rows(0)("IsCustomerActive").ToString())
                txtOtherLabel.Text = dtCust.Rows(0)("OtherLabel").ToString()

				DDL_Costing.SelectedValue = dtCust.Rows(0)("CostingMethod").ToString()
				Session("CostingMethod") = dtCust.Rows(0)("CostingMethod").ToString()
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
				If (cnt >= HDF_TotalCust.Value) Then
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
				ErrorMessage.InnerText = "Data Not found. Please try again after some time."
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
				ErrorMessage.InnerText = "Company name Already Exists."
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
				Return
			End If

			'check for existing user
			'Dim existingUser = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" And Email='" + txtAdminUsername.Text + "'", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
			Dim existingUser As DataTable = OBJMaster.GetPersonnelByEmail(txtAdminUsername.Text)
			If Not existingUser Is Nothing And existingUser.Rows.Count > 0 Then

				If (Not existingUser.Rows(0)("CustomerId").ToString() = CustId) Then
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Contact Email is not from same company, Please try another Contact Email."
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
					Return
				End If

				'return if entered existing Contact Email is not customerAdmin
				If Not existingUser.Rows(0)("Roles").ToString() = "CustomerAdmin" And Not existingUser.Rows(0)("Roles").ToString() = "SuperAdmin" Then
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Contact Email is not customer admin, Please try another Contact Email."
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
					Return
				End If
			End If

			If Not CustId = 0 Then
				Dim oldUser = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" And cust.CustomerId=CAST(" + CustId.ToString() + " as nvarchar) And IsMainCustomerAdmin = 1", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), True)
				If Not oldUser Is Nothing And oldUser.Rows.Count > 0 Then
					Dim userOld = New ApplicationUser()
					userOld = manager.FindById(oldUser.Rows(0)("Id").ToString())
					If Not userOld.Email = txtAdminUsername.Text Then
						userOld.IsMainCustomerAdmin = False
						Dim identityResultForOldUser As IdentityResult
						identityResultForOldUser = New IdentityResult()
						identityResultForOldUser = manager.Update(userOld)
						If identityResultForOldUser.Succeeded Then
							'success
						End If
					End If
				End If
			End If
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
				ErrorMessage.InnerText = "Ending Hosting Date must be greater than Beginning Hosting Date."
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
				Return
			End If

            'save company
            result = OBJMaster.SaveUpdateCustomer(CustId, txtCustName.Text, txtContactName.Text, txtContactNumber.Text, txtContactAddress.Text, txtExportCode.Text, Convert.ToInt32(Session("PersonId")), CHK_RequireLogin.Checked, False,
                                                  chk_RequireDepartment.Checked, chk_RequirePersonnelPIN.Checked, chk_RequireOther.Checked, otherLabel, chkIsActive.Checked, Convert.ToInt32(DDL_Costing.SelectedValue),
                                                  BeginningHostingDate, EndingHostingDate)

            'Save Personal Vehicle Mapping
            If chkAssignPerToVeh.Checked And result > 0 Then
				OBJMaster.SavePersonnalVehicleMappingAgainstCustomer(CustId, Convert.ToInt32(Session("PersonId")))
			End If

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

				If CustId = 0 Then
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
                       .IsMainCustomerAdmin = True,
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
           .IsGateHub = False
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
							If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin") Then
								user.CustomerId = Convert.ToInt32(result)
							End If
						End If
						user.ApprovedOn = DateTime.Now
						'user.SoftUpdate = "N"
						user.IsMainCustomerAdmin = True
						user.SendTransactionEmail = IIf(user.SendTransactionEmail = Nothing, False, user.SendTransactionEmail)
						user.RequestFrom = IIf(user.RequestFrom = Nothing, "W", user.RequestFrom)

						identityResult = New IdentityResult()

						identityResult = manager.Update(user)
						If identityResult.Succeeded Then
							'update password
							If Not txtAdminPassword.Text = "" Then
								If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin") Then

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

				Else
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
                  .IsMainCustomerAdmin = True,
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
         .IsGateHub = False
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
							If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin") Then
								user.CustomerId = Convert.ToInt32(result)
							End If
						End If
						user.IsMainCustomerAdmin = True
						user.SendTransactionEmail = IIf(user.SendTransactionEmail = Nothing, False, user.SendTransactionEmail)
						user.RequestFrom = IIf(user.RequestFrom = Nothing, "W", user.RequestFrom)

						identityResult = New IdentityResult()

						identityResult = manager.Update(user)
						If identityResult.Succeeded Then
							'update password
							If Not txtAdminPassword.Text = "" Then
								If (existingUser.Rows(0)("Roles").ToString() <> "SuperAdmin") Then

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
					ErrorMessage.InnerText = "Company Addition failed, Please try again."
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
						"Contact Address = " & txtContactAddress.Text.Replace(",", " ") & " ; " &
						"Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
						"Require Login = " & IIf(CHK_RequireLogin.Checked = True, "Yes", "No") & " ; " &
						"Require Department = " & IIf(chk_RequireDepartment.Checked = True, "Yes", "No") & " ; " &
						"Require Personnel PIN = " & IIf(chk_RequirePersonnelPIN.Checked = True, "Yes", "No") & " ; " &
						"Require Other = " & IIf(chk_RequireOther.Checked = True, "Yes", "No") & " ; " &
						"Other label = " & txtOtherLabel.Text.Replace(",", " ") & " ; " &
						"Contact Phone Number = " & txtContactNumber.Text & " ; " &
						"Contact Email = " & txtAdminUsername.Text & " ; " &
						"Assign all personnel to all vehicles = " & IIf(chkAssignPerToVeh.Checked = True, "Yes", "No") & " ; " &
						"Costing Method = " & DDL_Costing.SelectedItem.ToString().Replace(",", " ") & " ; " &
						"Start Date = " & lblStartDate.Text & " ; " &
						"Beginning Hosting Date = " & BeginningHostingDate & " ; " &
						"Ending Hosting Date = " & EndingHostingDate & "  "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub DDL_Costing_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try
			If DDL_Costing.SelectedValue.ToString() <> Convert.ToString(Session("CostingMethod")) And DDL_Costing.SelectedValue.ToString() <> "0" Then
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModalWarningMessage();LoadDateTimeControl();", True)
			Else
				Page.Validate()
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
			End If
		Catch ex As Exception
			log.Error("Error occurred in DDL_Costing_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub
End Class
