Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports log4net
Imports log4net.Config

Public Class RegisterCompany
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(RegisterCompany))

    Dim OBJMaster As MasterBAL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False

            If Not IsPostBack Then

            End If
            txtCustName.Focus()
        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try

            Dim CustId As Integer = 0
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
                Return
            End If

            'check for existing user
            Dim existingUser = OBJMaster.GetPersonnelByEmail(txtAdminUsername.Text)

            If Not existingUser Is Nothing And existingUser.Rows.Count > 0 Then
                'return if entered email is already is system
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Contact email is already registered, Please contact administrator."
                Return
            End If

            Dim CheckValidShipment As Integer = False
            OBJMaster = New MasterBAL()

            CheckValidShipment = OBJMaster.CheckValidShipment(txtShiptmentFluidSecureUnitName.Text)


            If CheckValidShipment = 0 Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Error occured. please try after some time."
                Return
            ElseIf CheckValidShipment = -1 Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Shipment yet not shipped. Please contact administrator."
                Return
            ElseIf CheckValidShipment = -2 Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Shipment is already registered with another company. Please contact administrator."
                Return
            End If

            Dim custRole = roleManager.FindByName("CustomerAdmin")
            OBJMaster = New MasterBAL()
            Dim identityResult As IdentityResult
            Dim user = New ApplicationUser()

            'save company
            result = OBJMaster.SaveUpdateCustomer(CustId, txtCustName.Text, txtContactName.Text, txtContactNumber.Text, txtContactAddress.Text, "", 0, False, False, False, False, False, "Other", 1, 1)

            If result > 0 Then
                HDF_Custd.Value = result

                'add user name and password to aspnetusers table with IsMainCustomerAdmin flag true

                user = New ApplicationUser() With {
               .UserName = txtAdminUsername.Text,
               .Email = txtAdminUsername.Text,
               .PersonName = txtContactName.Text,
               .PhoneNumber = txtContactNumber.Text,
               .CreatedDate = DateTime.Now,
               .CreatedBy = Nothing,
               .IsDeleted = False,
               .RoleId = custRole.Id,
                .IsApproved = True,
                .ApprovedBy = Nothing,
                .ApprovedOn = DateTime.Now,
                .CustomerId = Convert.ToInt32(result),
               .IsMainCustomerAdmin = True,
               .SoftUpdate = "N",
                .SendTransactionEmail = False,
                .RequestFrom = "W",
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

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Company addition failed, Please try again."
                Return

            End If

            If result > 0 And identityResult.Succeeded Then

                OBJMaster.UpdateFieldsAfterAddingUserFromRegisterCompany(result, txtAdminUsername.Text, txtShiptmentFluidSecureUnitName.Text)
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "SuccessMsg()", True)

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

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Company Addition failed, Please try again."

            End If

            txtCustName.Focus()

        Catch ex As Exception

            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
            txtCustName.Focus()
        Finally

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFuction();", True)
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Account/login")
    End Sub

End Class