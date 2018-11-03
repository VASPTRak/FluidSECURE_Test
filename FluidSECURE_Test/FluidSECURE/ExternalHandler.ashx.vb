Imports System.Web
Imports System.Web.Services
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports log4net
'Imports System.Web.Security
Imports log4net.Config
Imports System.IO
Imports System.Net.Mail
Imports System.Net

Public Class ExternalHandler
    Implements System.Web.IHttpHandler

    Dim RegistrationFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender3")
    Dim strLogs As String = ""
    Dim steps As String = ""

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Try
            XmlConfigurator.Configure()

            Dim action As String = context.Request.QueryString("action")
            If (action = "FromFluidSecure") Then

                RegistrationFailedLog.Info("In FluidSecure Service")

                'Dim request As HttpWebRequest = WebRequest.Create("http://103.8.126.241:89/ExternalHandler.ashx?Action=FromFluidSecure")

                'Dim response As HttpWebResponse = request.GetResponse()

                'context.Response.Write(response)

                steps = " Start ProcessRequest"
                GetAndStoreData(context)
                steps = " End ProcessRequest"

                context.Response.Write("success")

            End If
        Catch ex As Exception

            RegistrationFailedLog.Error("Exception Occurred on Process request. Exception is : " & ex.Message & " . Details : " & steps)

            'strLogs = strLogs & "<br>-- " & "Exception Occurred on Process request. Exception is : " & ex.Message & " . Details in steps : " & steps
            context.Response.Write("fail")
        Finally
            'send email to trakadmin
            If (strLogs <> "") Then
                SendRegistrationLogs(ConfigurationManager.AppSettings("RegistrationLogsEmail")) ' comment for live
            End If

        End Try

    End Sub

    Public Sub GetAndStoreData(context As HttpContext)
        steps = "Start GetAndStoreData"
        Dim externalBAL As ExternalBAL = New ExternalBAL()

        Dim lastUser_id As Integer = externalBAL.GetLastRegisteredUserEntryInSubscribers()

        Dim dtData As DataTable = New DataTable()
        dtData = externalBAL.GetSubscribersFromFluidSecure(lastUser_id)

        If (dtData Is Nothing) Then

            RegistrationFailedLog.Error("Subscribers not found.")
            'strLogs = strLogs & "<br>-- " & "Subscribers not found."
            Exit Sub

        End If

        If (dtData.Rows.Count = 0) Then

            RegistrationFailedLog.Error("Subscribers not found.")
            'strLogs = strLogs & "<br>-- " & "Subscribers not found."
            Exit Sub
        End If

        steps = "Got all subscribers data"

        Dim dtTemp As DataTable = New DataTable()
        Dim dtUniqueUserId As DataTable = New DataTable()
        Dim dtFinal As DataTable = New DataTable()

        dtFinal.Columns.Add("billing_first_name", System.Type.[GetType]("System.String"))
        dtFinal.Columns.Add("billing_last_name", System.Type.[GetType]("System.String"))
        dtFinal.Columns.Add("billing_phone", System.Type.[GetType]("System.String"))
        dtFinal.Columns.Add("billing_email", System.Type.[GetType]("System.String"))
        dtFinal.Columns.Add("billing_company", System.Type.[GetType]("System.String"))
        dtFinal.Columns.Add("billing_address_1", System.Type.[GetType]("System.String"))
        dtFinal.Columns.Add("billing_address_2", System.Type.[GetType]("System.String"))
        dtFinal.Columns.Add("user_id", System.Type.[GetType]("System.String"))

        dtUniqueUserId = dtData.DefaultView.ToTable(True, "user_id")


        For Each dr As DataRow In dtUniqueUserId.Rows

            steps = "add data to datatable for User_id:" & dr("user_id")

            dtTemp = New DataTable()
            Dim dv As DataView = dtData.DefaultView
            dv.RowFilter = "user_id=" & dr("user_id")
            dtTemp = dv.ToTable()
            Dim drNew As DataRow = dtFinal.NewRow()

            For Each drTemp As DataRow In dtTemp.Rows

                If (drTemp("meta_key") = "billing_first_name") Then
                    drNew("billing_first_name") = drTemp("meta_value")
                End If
                If (drTemp("meta_key") = "billing_last_name") Then
                    drNew("billing_last_name") = drTemp("meta_value")
                End If
                If (drTemp("meta_key") = "billing_phone") Then
                    drNew("billing_phone") = drTemp("meta_value")
                End If
                If (drTemp("meta_key") = "billing_email") Then
                    drNew("billing_email") = drTemp("meta_value")
                End If
                If (drTemp("meta_key") = "billing_company") Then
                    drNew("billing_company") = drTemp("meta_value")
                End If
                If (drTemp("meta_key") = "billing_address_1") Then
                    drNew("billing_address_1") = drTemp("meta_value")
                End If
                If (drTemp("meta_key") = "billing_address_2") Then
                    drNew("billing_address_2") = drTemp("meta_value")
                End If
                drNew("user_id") = drTemp("user_id")

            Next

            dtFinal.Rows.Add(drNew)

        Next

        RegisterUsers(dtFinal, context)

    End Sub

    Private Sub RegisterUsers(dtFinal As DataTable, context As HttpContext)
        steps = "Start Registering Users"

        Dim OBJMaster As MasterBAL = New MasterBAL()
        Dim cntCheck As Integer = 0
        For Each drFinal As DataRow In dtFinal.Rows

            steps = "Check details for user_id:" & drFinal("user_id").ToString()
            Dim ContactName As String = ""

            ContactName = drFinal("billing_first_name").ToString() & IIf(drFinal("billing_last_name").ToString() = "", "", " " & drFinal("billing_last_name").ToString())

            Dim companyAddress As String = ""

            companyAddress = drFinal("billing_address_1").ToString() & IIf(drFinal("billing_address_2").ToString() = "", "", " , " & drFinal("billing_address_2").ToString())

            cntCheck = 0
            If (drFinal("billing_company").ToString() = "") Then
                RegistrationFailedLog.Error(String.Format("billing_company  not found for the new User(Name:{0}, Adress:{1}, userid:{2})", ContactName, companyAddress, drFinal("user_id").ToString()))
                cntCheck = 1
                strLogs = strLogs & "<br>-- " & String.Format("billing_company not found for the new User(Name:{0}, Adress:{1}, userid:{2})", ContactName, companyAddress, drFinal("user_id").ToString())
            End If

            If (drFinal("billing_email").ToString() = "") Then
                RegistrationFailedLog.Error(String.Format("billing_email not found for the new User(Name:{0}, Adress:{1}, userid:{2})", ContactName, companyAddress, drFinal("user_id").ToString()))
                cntCheck = 1
                strLogs = strLogs & "<br>-- " & String.Format("billing_email not found for the new User(Name:{0}, Adress:{1}, userid:{2})", ContactName, companyAddress, drFinal("user_id").ToString())
            End If
            If (cntCheck = 1) Then
                Continue For
            End If

            Dim CheckCompanyExists As Boolean = False
            Dim dtCheckEmailExists As DataTable = New DataTable()

            OBJMaster = New MasterBAL()

            steps = "Check CompanyExists for user_id:" & drFinal("user_id").ToString() & " . Company Name : " & drFinal("billing_company").ToString()

            CheckCompanyExists = OBJMaster.CustNameIsExists(drFinal("billing_company").ToString(), 0)


            If CheckCompanyExists = True Then

                RegistrationFailedLog.Error(String.Format("billing_company is in already registered in FluidSecure cloud for the new User(CompanyName:{0},Name:{1}, Adress:{2}, userid:{3})", drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString()))
                strLogs = strLogs & "<br>-- " & String.Format("billing_company is in already registered in FluidSecure cloud for the new User(CompanyName:{0},Name:{1}, Adress:{2}, userid:{3})", drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString())
                Dim customerId As Integer = 0
                OBJMaster = New MasterBAL()
                Dim dtCompay As DataTable = New DataTable()
                dtCompay = OBJMaster.GetCustomerDetailsByName(drFinal("billing_company").ToString())
                If (dtCompay.Rows.Count > 0) Then
                    customerId = dtCompay.Rows(0)("CustomerId")
                End If


                OBJMaster = New MasterBAL()

                steps = "Check CheckEmailExists with company already exist  for user_id:" & drFinal("user_id").ToString() & " . Company Name : " & drFinal("billing_company").ToString() & " . Email : " & drFinal("billing_email").ToString()

                dtCheckEmailExists = OBJMaster.GetPersonnelByEmail(drFinal("billing_email").ToString())

                If dtCheckEmailExists.Rows.Count > 0 Then
                    RegistrationFailedLog.Error(String.Format("billing_email is in already registered in FluidSecure cloud for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString()))
                    strLogs = strLogs & "<br>-- " & String.Format("billing_email is in already registered in FluidSecure cloud for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString())
                    Continue For
                Else

                    ContactName = drFinal("billing_first_name").ToString() & IIf(drFinal("billing_last_name").ToString() = "", "", " " & drFinal("billing_last_name").ToString())

                    SaveUser(context, drFinal, ContactName, customerId, companyAddress)

                End If

                Continue For
            End If

            OBJMaster = New MasterBAL()
            dtCheckEmailExists = OBJMaster.GetPersonnelByEmail(drFinal("billing_email").ToString())

            steps = "Check CheckEmailExists with new company for user_id:" & drFinal("user_id").ToString() & " . Company Name : " & drFinal("billing_company").ToString() & " . Email : " & drFinal("billing_email").ToString()

            If dtCheckEmailExists.Rows.Count > 0 Then
                RegistrationFailedLog.Error(String.Format("billing_email is in already registered in FluidSecure cloud for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString()))
                strLogs = strLogs & "<br>-- " & String.Format("billing_email is in already registered in FluidSecure cloud for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString())
                Continue For
            End If

            Dim result As Integer = 0

            ContactName = drFinal("billing_first_name").ToString() & IIf(drFinal("billing_last_name").ToString() = "", "", " " & drFinal("billing_last_name").ToString())

            'Dim companyAddress As String = ""

            companyAddress = drFinal("billing_address_1").ToString() & IIf(drFinal("billing_address_2").ToString() = "", "", " , " & drFinal("billing_address_2").ToString())

            'save company
            result = OBJMaster.SaveUpdateCustomer(0, drFinal("billing_company").ToString(), ContactName, drFinal("billing_phone").ToString(), companyAddress, "", 0, False, False, False, False, False, "Other", 1, 0, "", "", "", "", "")



            If (result <> 0) Then
                steps = "Company created  for user_id:" & drFinal("user_id").ToString() & " . Company Name : " & drFinal("billing_company").ToString() & " . Email : " & drFinal("billing_email").ToString()
                RegistrationFailedLog.Error(String.Format("New Company successfully created for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString()))
                strLogs = strLogs & "<br>-- " & String.Format("New Company successfully created for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString())

                SaveUser(context, drFinal, ContactName, result, companyAddress)
            Else

                RegistrationFailedLog.Error(String.Format("Error Occurred while storing Comapny. Details are email:{0},company:{1},phone:{2},user_id:{3}", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(),
                                                               drFinal("billing_phone").ToString(), drFinal("user_id").ToString()))

                strLogs = strLogs & "<br>-- " & String.Format("Error Occurred while storing Comapny for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString())
            End If
        Next
    End Sub

    Private Sub SaveUser(context As HttpContext, drFinal As DataRow, ContactName As String, customerId As Integer, companyAddress As String)

        steps = "SaveUser  for user_id:" & drFinal("user_id").ToString() & " . Company Name : " & drFinal("billing_company").ToString() & " . Email : " & drFinal("billing_email").ToString()

        'add user name and password to aspnetusers table with IsMainCustomerAdmin flag true
        Dim identityResult As IdentityResult
        Dim user = New ApplicationUser()

        Dim manager = context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
        Dim roleManager = New RoleManager(Of IdentityRole)(New RoleStore(Of IdentityRole)(New ApplicationDbContext()))

        Dim custRole = roleManager.FindByName("CustomerAdmin")

        user = New ApplicationUser() With {
       .UserName = drFinal("billing_email").ToString(),
       .Email = drFinal("billing_email").ToString(),
       .PersonName = ContactName,
       .PhoneNumber = drFinal("billing_phone").ToString(),
       .CreatedDate = DateTime.Now,
       .CreatedBy = Nothing,
       .IsDeleted = False,
       .RoleId = custRole.Id,
       .IsApproved = True,
       .ApprovedBy = Nothing,
       .ApprovedOn = DateTime.Now,
       .CustomerId = customerId,
       .IsMainCustomerAdmin = True,
       .SoftUpdate = "N",
       .SendTransactionEmail = False,
       .RequestFrom = "F",
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
           .IsLogging = 0
   }
        identityResult = New IdentityResult()
        Dim pwd As String = PasswordService.GeneratePassword()
        identityResult = manager.Create(user, pwd)
        If identityResult.Succeeded Then
            Dim externalBAL As ExternalBAL = New ExternalBAL()

            externalBAL.UpdateLastRegisteredUserEntryInSubscribers(drFinal("user_id").ToString())

            steps = "Registration done  for user_id:" & drFinal("user_id").ToString() & " . Company Name : " & drFinal("billing_company").ToString() & " . Email : " & drFinal("billing_email").ToString()

            strLogs = strLogs & "<br>-- " & String.Format("Registration successful for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString())

            SendRegistrationEmail(user.Email, pwd) 'user.Email

        Else
            steps = "Registration failed  for user_id:" & drFinal("user_id").ToString() & " . Company Name : " & drFinal("billing_company").ToString() & " . Email : " & drFinal("billing_email").ToString()

            Dim strIdentityError As String
            If identityResult.Errors.FirstOrDefault().ToString().Contains("is already taken") Then
                strIdentityError = " Username " + drFinal("billing_email").ToString() + " is already in use."
            ElseIf (identityResult.Errors.FirstOrDefault().ToString().ToLower().Contains("password")) Then
                strIdentityError = "Password MUST be minimum 6 characters long and contain one (1) of the following: Upper Case letter (A-Z), lower case letter (a-z), special character (!@#$%^&*), number (0-9)"
            Else
                strIdentityError = " " + identityResult.Errors.FirstOrDefault()
            End If

            If strIdentityError.Length > 0 Then
                strIdentityError = strIdentityError.Replace("'", " ")
            End If

            RegistrationFailedLog.Error(String.Format("Error Occurred while storing user. Error is {0} . Details are email:{1},company:{2},phone:{3},user_id:{4}", strIdentityError, drFinal("billing_email").ToString(), drFinal("billing_company").ToString(),
                                                       drFinal("billing_phone").ToString(), drFinal("user_id").ToString()))

            strLogs = strLogs & "<br>-- " & String.Format("Error Occurred while storing for the new User(email:{0},CompanyName:{1},Name:{2}, Adress:{3}, userid:{4})", drFinal("billing_email").ToString(), drFinal("billing_company").ToString(), ContactName, companyAddress, drFinal("user_id").ToString())
        End If
    End Sub

    Private Sub SendRegistrationEmail(UserEmail As String, UserPassword As String)
        Try

            Dim body As String = String.Empty
            Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/RegistrationEmail.txt"))
                body = sr.ReadToEnd()
            End Using
            '------------------
            body = body.Replace("cloudLink", "http://" + HttpContext.Current.Request.Url.Authority)
            body = body.Replace("UserEmail", UserEmail)
            body = body.Replace("UserPassword", UserPassword)

            Dim e As New EmailService()


            Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))

            mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
            mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

            Dim messageSend As New MailMessage()
            messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
            messageSend.[To].Add(New MailAddress(UserEmail))

            messageSend.Subject = ConfigurationManager.AppSettings("RegistrationSubject")
            messageSend.Body = body

            messageSend.IsBodyHtml = True
            mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))


            mailClient.Send(messageSend)

        Catch ex As Exception

            RegistrationFailedLog.Error(String.Format("Error Occurred while sending registration email to user. Error is {0}. Details are email:{1}", ex.Message, UserEmail))

            strLogs = strLogs & "<br>-- " & String.Format("Error Occurred while sending registration email to user. Error is {0}. email:{1}", ex.Message, UserEmail)

        End Try
    End Sub

    Private Sub SendRegistrationLogs(UserEmail As String)
        Try

            Dim body As String = String.Empty
            Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/RegistrationLogs.txt"))
                body = sr.ReadToEnd()
            End Using
            '------------------
            body = body.Replace("userDetails", strLogs)

            Dim e As New EmailService()


            Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))

            mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
            mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

            Dim messageSend As New MailMessage()
            messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
            messageSend.[To].Add(New MailAddress(UserEmail))


            messageSend.Subject = ConfigurationManager.AppSettings("RegistrationLogSubject")
            messageSend.Body = body

            messageSend.IsBodyHtml = True
            mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))


            mailClient.Send(messageSend)

        Catch ex As Exception

            RegistrationFailedLog.Error(String.Format("Error Occurred while sending registration Logs to admin. Error is {0}. Details are email:{1}", ex.Message, UserEmail))

        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
