Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports System.Security.Cryptography
Imports System.IO
Imports log4net
Imports log4net.Config

Partial Public Class Login
	Inherits Page

	Dim OBJMaster As MasterBAL
	Dim ds As DataSet

	Private _keyV1 As Byte() = System.Text.Encoding.ASCII.GetBytes("(tr@!<(u!8*M+^e8")
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Login))

	Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			'RegisterHyperLink.NavigateUrl = "Register"
			' Enable this once you have account confirmation enabled for password reset functionality
			' ForgotPasswordHyperLink.NavigateUrl = "Forgot"
			'OpenAuthLogin.ReturnUrl = Request.QueryString("ReturnUrl")
			'Dim IPAddress As String = GetLocalIPAddress()
			'log.Debug("IPADRESS 1 : " & IPAddress)

			''DisplayIPAddresses()

			'IPAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString()

			'log.Debug("IPADRESS 2 : " & IPAddress)


			'log.Debug("IPADRESS 3 : " & IPAddress)
			Session.Clear()

			Session("IPAddress") = GetLocalIPAddress()

			log.Debug("IPADRESS 3 : " & Session("IPAddress"))

			Dim returnUrl = HttpUtility.UrlEncode(Request.QueryString("ReturnUrl"))
			'If Not [String].IsNullOrEmpty(returnUrl) Then
			'    RegisterHyperLink.NavigateUrl += "?ReturnUrl=" & returnUrl
			'End If
			If (Not IsPostBack) Then

				Dim AutheticationManager = HttpContext.Current.GetOwinContext().Authentication
				AutheticationManager.SignOut()
				If Not Request.Cookies("userInfoNew") Is Nothing Then
					Email.Text = DecryptString(Request.Cookies("userInfoNew")("usrEmail"))
					Password.Attributes("value") = DecryptString(Request.Cookies("userInfoNew")("usrPwd"))
					RememberMe.Checked = IIf(Request.Cookies("userInfoNew")("remember") = Nothing, False, Convert.ToBoolean(Request.Cookies("userInfoNew")("remember")))
					btnLogin.Focus()
				Else
					Email.Focus()
				End If

			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			FailureText.Text = IIf(FailureText.Text <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub
	'Public Shared Sub DisplayIPAddresses()
	'    Dim sb As StringBuilder = New StringBuilder()
	'    Dim networkInterfaces As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
	'    For Each network As NetworkInterface In networkInterfaces
	'        Dim properties As IPInterfaceProperties = network.GetIPProperties()
	'        For Each address As IPAddressInformation In properties.UnicastAddresses
	'            If address.Address.AddressFamily <> AddressFamily.InterNetwork Then Continue For
	'            If IPAddress.IsLoopback(address.Address) Then Continue For
	'            sb.AppendLine(address.Address.ToString() & " (" + network.Name & ")")
	'        Next
	'    Next

	'End Sub

	Private Function GetLocalIPAddress() As String
		Try

			Dim IPAddress As String = Request.ServerVariables("HTTP_X_FORWARDED_FOR")
			If IPAddress = "" Or IPAddress Is Nothing Then
				IPAddress = Request.ServerVariables("REMOTE_ADDR")
			End If

			Return IPAddress
		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			Return ""
		End Try
	End Function


	Protected Sub LogIn(sender As Object, e As EventArgs)
		Try
			If IsValid Then

				Dim MaxFailedAccessAttemptsBeforeLockout As Integer = Convert.ToInt32(ConfigurationManager.AppSettings("MaxFailedAccessAttemptsBeforeLockout").ToString())
				Dim DefaultAccountLockoutTimeSpan As Integer = ConfigurationManager.AppSettings("DefaultAccountLockoutTimeSpan").ToString()

				Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
				manager.MaxFailedAccessAttemptsBeforeLockout = MaxFailedAccessAttemptsBeforeLockout
				manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(DefaultAccountLockoutTimeSpan)

				Dim signinManager = Context.GetOwinContext().GetUserManager(Of ApplicationSignInManager)()
				Dim message As String = ""
				Dim User = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).FindByName(Email.Text)

				If Not IsNothing(User) Then

					Dim validCredentials = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).Find(Email.Text, Password.Text)
					If Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).IsLockedOut(User.Id) Then
						message = String.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", DefaultAccountLockoutTimeSpan)
						CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", String.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", DefaultAccountLockoutTimeSpan))
						FailureText.Text = message
						ErrorMessage.Visible = True
					ElseIf Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).GetLockoutEnabled(User.Id) And IsNothing(validCredentials) Then
						Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).AccessFailed(User.Id)
						If Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).IsLockedOut(User.Id) And Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).GetAccessFailedCount(User.Id) Then
							message = String.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", DefaultAccountLockoutTimeSpan)
							If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
								CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", String.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", DefaultAccountLockoutTimeSpan))
							End If
							FailureText.Text = message
							ErrorMessage.Visible = True
						Else
							Dim accessFailedCount As Integer = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).GetAccessFailedCount(User.Id)
							Dim attemptsLeft As Integer = MaxFailedAccessAttemptsBeforeLockout - accessFailedCount
							If accessFailedCount = 0 Then
								message = String.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", DefaultAccountLockoutTimeSpan)
							Else
								If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
									CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", "Invalid credentials.")
								End If
								message = String.Format("Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft)
								If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
									CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", String.Format("Invalid credentials. Person have {0} more attempt(s) before account gets locked out.", attemptsLeft))
								End If
							End If

							FailureText.Text = message
							ErrorMessage.Visible = True
						End If

					ElseIf IsNothing(validCredentials) Then
						message = "Invalid credentials. Please try again."
						FailureText.Text = message
						ErrorMessage.Visible = True
						If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
							CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", String.Format("Invalid credentials. Please try again."))
						End If
					Else
						Dim result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout:=True)
						Dim UserObj As ApplicationUser = manager.Find(Email.Text, Password.Text)
						'Dim PersonId As Integer = 0
						Select Case result
							Case SignInStatus.Success
								If UserObj.IsDeleted = False And UserObj.IsApproved = True Then
									Email.Attributes("value") = Email.Text
									Password.Attributes("value") = Password.Text
									OBJMaster = New MasterBAL()
                                    'Dim resultCheck As Integer = OBJMaster.CheckAlreadyAcceptedScheduledMaintenanceNotification(Email.Text)
                                    'If (resultCheck <= 0) Then
                                    '	ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenMaintainanceNotification();", True)
                                    'Else
                                    '	CheckAfterAccept()
                                    'End If

                                    CheckAfterAccept()

                                    Exit Select
								Else
									If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
										CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", "Person in System but NOT marked as ACTIVE.")
									End If
									FailureText.Text = "Person in System but NOT marked as ACTIVE. Please contact administrator."
									ErrorMessage.Visible = True
									Exit Select
								End If


								'Case SignInStatus.LockedOut
								'    Response.Redirect("/Account/Lockout")
								'    Exit Select
								'Case SignInStatus.RequiresVerification
								'    Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
								'                                    Request.QueryString("ReturnUrl"),
								'                                    RememberMe.Checked),
								'                      True)
								'    Exit Select
							Case Else
								If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
									CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", "Person in System but NOT marked as ACTIVE.")
								End If
								FailureText.Text = "Invalid login attempt"
								ErrorMessage.Visible = True
								Exit Select
						End Select
					End If
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", "Invalid Email Id.")
					End If
					FailureText.Text = "Invalid Email Id"
					ErrorMessage.Visible = True
				End If
			End If


		Catch ex As Exception
			log.Error("Error occurred in LogIn Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			FailureText.Text = "Error occurred while getting data, please try again later."

		End Try

	End Sub

	Private Function EncryptString(plainText As String) As String
		Using aesAlgo As Aes = Aes.Create()
			aesAlgo.Key = _keyV1
			aesAlgo.IV = _keyV1
			Dim encryptor As ICryptoTransform = aesAlgo.CreateEncryptor(aesAlgo.Key, aesAlgo.IV)
			Using stream As New MemoryStream()
				Using cryptoStream As New CryptoStream(stream, encryptor, CryptoStreamMode.Write)
					Using streamWriter As New StreamWriter(cryptoStream)
						streamWriter.Write(plainText)
					End Using
					Dim encryptedBytes = stream.ToArray()


					Return "1" + Convert.ToBase64String(encryptedBytes)
				End Using
			End Using
		End Using
	End Function

	Private Function DecryptString(encryptedText As String) As String
		encryptedText = encryptedText.Substring(1)

		Dim encryptedBytes = Convert.FromBase64String(encryptedText)

		Using aesAlgo As Aes = Aes.Create()
			aesAlgo.Key = _keyV1
			aesAlgo.IV = _keyV1
			Dim decryptor As ICryptoTransform = aesAlgo.CreateDecryptor(aesAlgo.Key, aesAlgo.IV)
			Using stream As New MemoryStream(encryptedBytes)
				Using cryptoStream As New CryptoStream(stream, decryptor, CryptoStreamMode.Read)
					Using streamReader As New StreamReader(cryptoStream)
						Dim plainText As String = streamReader.ReadToEnd()
						Return plainText
					End Using
				End Using
			End Using
		End Using
	End Function

	Protected Sub btnForgotPassword_Click(sender As Object, e As EventArgs)
		Response.Redirect(String.Format("~/Account/Forgot.aspx?Email={0}", Email.Text), False)
	End Sub

	Protected Sub btnAccepted_Click(sender As Object, e As EventArgs)
		Try
			OBJMaster = New MasterBAL()
			Dim result As Integer = OBJMaster.AddScheduledMaintenanceNotificationAccepted(Email.Text)
		Catch ex As Exception
			log.Error("Error occurred in btnAccepted_Click Exception is :" + ex.Message)
		End Try
		CheckAfterAccept()
	End Sub

	Private Sub CheckAfterAccept()
		Try

			Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
			Dim UserObj As ApplicationUser = manager.Find(Email.Text, Password.Text)
			Dim PersonId As Integer = 0
			If (RoleActions.GetRolesById(UserObj.RoleId).Name = "User") Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("logged in", "login", "", "", Email.Text, Session("IPAddress").ToString(), "fail", "Access denied.")
				End If
				FailureText.Text = "Access denied. Please contact administrator."
				ErrorMessage.Visible = True
				Exit Sub
			End If

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				CSCommonHelper.WriteLog("logged in", "login", "", "", UserObj.PersonName & "(" & UserObj.Email & ")", Session("IPAddress").ToString(), "success", "")
			End If

			OBJMaster = New MasterBAL()
			Dim dtPerson As DataTable = New DataTable()
			dtPerson = OBJMaster.GetPersonDetailByUniqueUserId(UserObj.Id)
            If Not dtPerson Is Nothing Then

                If dtPerson.Rows(0)("CustomerActiveInActive") = "Yes" Or dtPerson.Rows(0)("RoleName") = "SuperAdmin" Then
                    PersonId = dtPerson.Rows(0)("PersonId")

                    If (RememberMe.Checked = True) Then
                        Dim aCookie As New HttpCookie("userInfoNew")
                        aCookie.Values("usrEmail") = EncryptString(Email.Text)
                        aCookie.Values("usrPwd") = EncryptString(Password.Text)
                        aCookie.Values("remember") = RememberMe.Checked
                        aCookie.Expires = DateTime.Now.AddDays(30)
                        Response.Cookies.Add(aCookie)
                    Else
                        If Not Request.Cookies("userInfoNew") Is Nothing Then
                            Dim aCookie As New HttpCookie("userInfoNew")
                            aCookie.Values("usrEmail") = EncryptString(Email.Text)
                            aCookie.Values("usrPwd") = EncryptString(Password.Text)
                            aCookie.Expires = DateTime.Now.AddDays(-1)
                            Response.Cookies.Add(aCookie)
                        End If

                    End If

                    Session("PersonId") = PersonId
                    Session("UniqueId") = UserObj.Id
                    Session("RoleId") = UserObj.RoleId
                    Session("RoleName") = RoleActions.GetRolesById(UserObj.RoleId).Name
                    Session("PersonName") = UserObj.PersonName
                    Session("PersonEmail") = UserObj.Email
                    Session("Culture") = DrpLanguages.SelectedValue
                    Session("CustomerId") = 0
                    Session("IsTermConditionAgreed") = UserObj.IsTermConditionAgreed
                    Session("DateTimeTermConditionAccepted") = UserObj.DateTimeTermConditionAccepted

                    Context.GetOwinContext().GetUserManager(Of ApplicationUserManager).ResetAccessFailedCount(UserObj.Id)
                    If ConfigurationManager.AppSettings("AllowResetPassword").ToString().ToLower() = "yes" Then
                        Dim Days As Integer = Convert.ToInt32(ConfigurationManager.AppSettings("ResetPasswordDays").ToString()) 'New field for Adding Reset password date
                        Dim difference As TimeSpan = DateTime.Now.Subtract(UserObj.PasswordResetDate)
                        If difference.TotalDays > Days Then
                            Session("FromLoginPage") = "Y"
                            Response.Redirect(String.Format("~/Master/ResetPassword?PersonId={0}&UniqueUserId={1}", PersonId, UserObj.Id), False)
                        Else
                            Session("FromLoginPage") = "N"
                            Response.Redirect("/home")
                        End If
                    Else
                        Session("FromLoginPage") = "N"
                        Response.Redirect("/home")
                    End If
                Else
                    FailureText.Text = "Company  is inactive - Please contact FluidSecure Support  <a href='mailto:support@fluidsecure.com' target=_blank> support@fluidsecure.com</a>. "
                    ErrorMessage.Visible = True
                End If
                'Response.Redirect("/home")
            End If

        Catch ex As Exception
			log.Error("Error occurred in CheckAfterAccept Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			FailureText.Text = "Error occurred while getting data, please try again later."
		End Try
	End Sub
End Class
