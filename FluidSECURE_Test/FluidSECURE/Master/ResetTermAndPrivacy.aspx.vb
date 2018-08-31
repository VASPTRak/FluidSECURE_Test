Imports log4net
Imports log4net.Config

Public Class ResetTermAndPrivacy
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Company))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") <> "SuperAdmin" Then
				'Access denied
				Response.Redirect("/home")
			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Protected Sub btnReset_Click(sender As Object, e As EventArgs)
		ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckConfirm();", True)
	End Sub

	Protected Sub btnConfirmReject_Click(sender As Object, e As EventArgs)
		Dim AcceptedData As String = ""
		Try

			OBJMaster = New MasterBAL()
			OBJMaster.UpdateTermAndPolicysAcceptance(Session("UniqueId").ToString(), 1)

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				AcceptedData = CreateData(Session("UniqueId").ToString())
				CSCommonHelper.WriteLog("Added", "Reset Tearm and Policy", "", AcceptedData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "Terms and Privacy Policys Reset Successfully.")
			End If

			ErrorMessage.Visible = False
			message.Visible = True
			message.InnerText = "Terms and Privacy Policys Reset Successfully."
		Catch ex As Exception
			log.Error("Error occurred in btnAccepted_Click Exception is :" + ex.Message)
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				AcceptedData = CreateData(Session("UniqueId").ToString())
				CSCommonHelper.WriteLog("Added", "Reset Tearm and Policy", "", AcceptedData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Terms and Privacy Policys Reset failed.")
			End If
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting company data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "ClosePopUp();", True)
		End Try
	End Sub

	Private Function CreateData(UniqueId As String) As String
		Try
			Dim data As String = ""

			data = "UniqueId = " & UniqueId & " ; " &
				   "Date of Acceptance = " & DateTime.Now.ToString().Replace(",", " ") & " ; " &
				   "Person Name = " & Session("PersonName").ToString().Replace(",", " ") & "  "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class