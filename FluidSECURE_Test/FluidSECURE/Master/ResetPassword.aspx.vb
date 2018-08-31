Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin.Security
Imports Owin
Imports log4net
Imports log4net.Config

Public Class ResetPassword1
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(ResetPassword1))

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				If Session("FromLoginPage") Is Nothing Then
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Else
					If Session("FromLoginPage") <> "Y" Then
						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
					End If
				End If
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("Support") = "Reports Only" Then
				If Session("FromLoginPage") <> "Y" Then
					'Access denied
					Response.Redirect("/home")
				Else
					If (Not IsPostBack) Then
						If (Not Request.QueryString("PersonId") = Nothing And Not Request.QueryString("PersonId") = "") Then
							HDF_PersonnelId.Value = Request.QueryString("PersonId")
							HDF_UniqueUserId.Value = Request.QueryString("UniqueUserId")

							lblHeader.Text = "Reset Password"
						Else
							'error
						End If
					End If
					txtPassword.Focus()
				End If
			Else
				If (Not IsPostBack) Then
					If (Not Request.QueryString("PersonId") = Nothing And Not Request.QueryString("PersonId") = "") Then
						HDF_PersonnelId.Value = Request.QueryString("PersonId")
						HDF_UniqueUserId.Value = Request.QueryString("UniqueUserId")

						lblHeader.Text = "Reset Password"
					Else
						'error
					End If
				End If
				txtPassword.Focus()
			End If

			If Session("FromLoginPage") = "Y" Then 'New field for Adding Reset password date
				Dim ItemMenu As Control = DirectCast(Master.FindControl("ItemMenu"), Control)
				Dim ReportMenu As Control = DirectCast(Master.FindControl("ReportMenu"), Control)
				Dim TransactionsMenu As Control = DirectCast(Master.FindControl("TransactionsMenu"), Control)
				Dim Import As Control = DirectCast(Master.FindControl("Import"), Control)
				Dim Export As Control = DirectCast(Master.FindControl("Export"), Control)
				Dim Reconciliation As Control = DirectCast(Master.FindControl("Reconciliation"), Control)
				Dim OtherMenu As Control = DirectCast(Master.FindControl("OtherMenu"), Control)
				ItemMenu.Visible = False
				ReportMenu.Visible = False
				TransactionsMenu.Visible = False
				Import.Visible = False
				Export.Visible = False
				Reconciliation.Visible = False
				OtherMenu.Visible = False

				btnCancel.Text = "Back"

				Session("PersonId") = Nothing
				Session("UniqueId") = Nothing
				Session("RoleId") = Nothing

				message.InnerText = "Your password has expired. Please reset."
				message.Visible = True
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		Try

			Dim PersonId As Integer = 0
			Dim UniqueUserId As String = 0

			If (Not HDF_PersonnelId.Value = Nothing And Not HDF_UniqueUserId.Value = "") Then

				PersonId = HDF_PersonnelId.Value
				UniqueUserId = HDF_UniqueUserId.Value
				Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
				Dim signinManager = Context.GetOwinContext().GetUserManager(Of ApplicationSignInManager)()
				Dim code = manager.GeneratePasswordResetToken(UniqueUserId)
				If Not code = Nothing Then
					Dim result = manager.ResetPassword(UniqueUserId, code, txtPassword.Text)
					If result.Succeeded Then
						message.Visible = True
						ErrorMessage.Visible = False
						message.InnerText = "Password reset successfully"

						Dim user = New ApplicationUser() 'New field for Adding Reset password date
						user = manager.FindById(UniqueUserId)
						user.PasswordResetDate = DateTime.Now
						Dim resultReset As IdentityResult = manager.Update(user)

						If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
							Dim writtenData As String = CreateData()

							CSCommonHelper.WriteLog("Reset Password", "Reset Password", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
						End If

						If resultReset.Succeeded Then
							message.Visible = True
							ErrorMessage.Visible = False
							message.InnerText = "Password reset successfully"
						End If

						If Session("FromLoginPage") = "Y" Then 'New field for Adding Reset password date
							Session("FromLoginPage") = "Reset"
							ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "GoToLoginPage();", True)
						End If

					Else
						'error
						If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
							Dim writtenData As String = CreateData()

							CSCommonHelper.WriteLog("Reset Password", "Reset Password", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Reset Password failed")
						End If

						ErrorMessage.Visible = True
						ErrorMessage.InnerText = result.Errors.ToList().First().ToString()
					End If
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData()

					CSCommonHelper.WriteLog("Reset Password", "Reset Password", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Reset Password failed")
				End If
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "An error has occurred, please try again."
			End If
		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData()

				CSCommonHelper.WriteLog("Reset Password", "Reset Password", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Reset Password failed. Exception is : " & ex.Message)
			End If

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while saving data"

			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
		Finally
			txtPassword.Focus()
		End Try

	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Try
			If Session("FromLoginPage") = "Y" Then 'New field for Adding Reset password date
				Context.GetOwinContext().Authentication.SignOut()
				Session.Clear()
				Response.Redirect("~/Account/Login")
			Else
				Response.Redirect("~/Master/AllPersonnel")
			End If

		Catch ex As Exception

			log.Error("Error occurred in btnCancel_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Private Function CreateData() As String
		Try
			Dim data As String = "Call From = " & IIf(Session("FromLoginPage").ToString() = "Y", "Password Expired - Login Page", "Personnel Page") & " ; " &
									"Person Id = " & HDF_PersonnelId.Value.Replace(",", " ") & " ; " &
									"UserId = " & HDF_UniqueUserId.Value.Replace(",", "") & " ; "
			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class