Imports System
Imports System.Web
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Owin
Imports log4net
Imports System.Net.Mail
Imports System.Net
Imports System.Net.Http

Partial Public Class ForgotPassword
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(ForgotPassword))

    Dim OBJMaster As MasterBAL
    Protected Property StatusMessage() As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
			Else
				Response.Redirect("/home")
			End If
			If (Not IsPostBack) Then
				Email.Text = Request.QueryString("Email").ToString()
			End If

		Catch ex As Exception
			ErrorMessage.Visible = True
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			FailureText.Text = IIf(FailureText.Text <> "", "", "Error occurred while loading details, please try again later.")
			Response.Redirect("/Account/Login")
        End Try

        'If (Not Request.QueryString("Email") = Nothing) Then
        'Email.Text = Request.QueryString("Email").ToString()
        'Else
        '    Response.Redirect("/Account/Login")
        'End If
        'Dim ItemMenu As Control = DirectCast(Master.FindControl("ItemMenu"), Control)
        'Dim ReportMenu As Control = DirectCast(Master.FindControl("ReportMenu"), Control)
        'Dim TransactionsMenu As Control = DirectCast(Master.FindControl("TransactionsMenu"), Control)
        'Dim Import As Control = DirectCast(Master.FindControl("Import"), Control)
        'Dim Export As Control = DirectCast(Master.FindControl("Export"), Control)
        'Dim Reconciliation As Control = DirectCast(Master.FindControl("Reconciliation"), Control)
        'Dim LogAction As Control = DirectCast(Master.FindControl("LogAction"), Control)
        'ItemMenu.Visible = False
        'ReportMenu.Visible = False
        'TransactionsMenu.Visible = False
        'Import.Visible = False
        'Export.Visible = False
        'Reconciliation.Visible = False
        'LogAction.Visible = False
    End Sub

    Protected Sub Forgot(sender As Object, e As EventArgs)
		Try

			If IsValid Then
				Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
				Dim user As ApplicationUser = manager.FindByEmail(Email.Text)
				If user Is Nothing Then
					FailureText.Text = "The user either does not exist."
					ErrorMessage.Visible = True
					Return
				End If
				Dim code = manager.GeneratePasswordResetToken(user.Id)
				Dim callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(code, Request)
				sendForgotPasswordEmail(Email.Text, "Please reset your password by clicking here <a href=""" & callbackUrl & """>Link</a>.")
				'manager.SendEmail(user.Id, "Reset Password", "Please reset your password by clicking here <a href=""" & callbackUrl & """>Link</a>.")
				loginForm.Visible = False
				DisplayEmail.Visible = True
			End If

		Catch ex As Exception
			log.Error("Error occurred in Forgot Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			FailureText.Text = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    Private Sub sendForgotPasswordEmail(Email As String, SubBody As String)
        Try
            Dim e As New EmailService()
            Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))
            Dim body As String = ""
            mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
            mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))
            Dim messageSend As New MailMessage()
            messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
            messageSend.[To].Add(New MailAddress(Email))
            messageSend.Subject = ConfigurationManager.AppSettings("ForgotPasswordSubject")
            body = "Hi " + Email + ", <br><br>"
            body += SubBody + "<br/>"
            body += " <br/>Thanks & Regards,<br/>FluidSecure"
            messageSend.Body = body
            messageSend.IsBodyHtml = True
            mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))
            Try
                mailClient.Send(messageSend)
            Catch ex As Exception
                log.Debug("Exception occurred in sending Forgot Password emails to EmailId : " & Email & ". ex is :" & ex.Message)
            End Try
        Catch ex As Exception
            log.Debug("Exception occurred in sending Forgot Password emails to EmailId : " & Email & ". ex is :" & ex.Message)
        End Try
    End Sub

End Class