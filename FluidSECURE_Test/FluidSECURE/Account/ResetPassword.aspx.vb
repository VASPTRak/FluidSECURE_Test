
Imports Microsoft.AspNet.Identity

Imports Microsoft.AspNet.Identity.Owin

Imports log4net
Imports log4net.Config
Partial Public Class ResetPassword
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(ResetPassword))
    Protected Property StatusMessage() As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If CSCommonHelper.CheckSessionExpired() = False Then
		Else
			Response.Redirect("/home")
       End If
		XmlConfigurator.Configure()
	End Sub

    Protected Sub Reset_Click(sender As Object, e As EventArgs)
		Try

			Dim IPAddress As String = GetLocalIPAddress()
			Dim code As String = IdentityHelper.GetCodeFromRequest(Request)
			If code IsNot Nothing Then
				Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
				Dim user = manager.FindByName(Email.Text)
				If user Is Nothing Then
					ErrorMessage.Text = "No user found"
					Return
				End If
				Dim result = manager.ResetPassword(user.Id, code, Password.Text)
				If result.Succeeded Then
					'New field for Adding Reset password date
					user.PasswordResetDate = DateTime.Now
					Dim resultReset As IdentityResult = manager.Update(user)
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData()

						CSCommonHelper.WriteLog("Reset Password", "Reset Password", "", writtenData, "", IPAddress, "success", "")
					End If
					Response.Redirect("~/Account/ResetPasswordConfirmation")
					Return
				End If
				ErrorMessage.Text = result.Errors.FirstOrDefault()
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData()

					CSCommonHelper.WriteLog("Reset Password", "Reset Password", "", writtenData, "", IPAddress, "fail", "Reset Password failed. Exception is : " + result.Errors.FirstOrDefault())
				End If
				Return
			End If


			ErrorMessage.Text = "An error has occurred."

		Catch ex As Exception
			ErrorMessage.Text = "An error has occurred."
			log.Error(String.Format("Error Occurred in Reset_Click. Error is {0}.", ex.Message))
		End Try

	End Sub

    Private Function CreateData() As String
        Try
            Dim data As String = "Call From = Forgot Password - Login Page" & " ; " &
                                    "Email = " & Email.Text.Replace(",", " ") & " ; "
            Return data
        Catch ex As Exception
            Log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function


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

End Class