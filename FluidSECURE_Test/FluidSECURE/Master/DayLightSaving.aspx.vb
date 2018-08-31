Imports log4net
Imports log4net.Config

Public Class DayLightSaving
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(DayLightSaving))

	Dim OBJMaster As MasterBAL = New MasterBAL()
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then

				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Or Session("RoleName") = "CustomerAdmin" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then

					GetDayLightSavingDetails()

				End If
			End If


		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

		End Try
	End Sub

	Private Sub GetDayLightSavingDetails()
		Try

			Dim dtDayLightSaving As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtDayLightSaving = OBJMaster.GetDayLightSaving()

			If (dtDayLightSaving.Rows.Count > 0) Then
				CHK_DayLightSaving.Checked = dtDayLightSaving.Rows(0)("IsDayLightSaving")
			End If
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = "Day Light Saving = " & IIf(CHK_DayLightSaving.Checked = True, "Yes", "No")
			End If
		Catch ex As Exception

			log.Error("Error occurred in GetDayLightSavingDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Day light saving details, please try again later."

		End Try
	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function SaveDetails(ByVal DayLightSaving As String) As String
		Dim DayLightSavingClass As DayLightSaving = New DayLightSaving()

		Dim OBJMaster = New MasterBAL()
		Dim result As Integer = 0
		Try
			Dim DayLightSavingBit As Boolean = False

			If (DayLightSaving = "Y") Then
				DayLightSavingBit = True
			Else
				DayLightSavingBit = False
			End If

			result = OBJMaster.UpdateDayLightSaving(DayLightSavingBit, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Modified", "Day Light Saving", beforeData, "Day Light Saving = " & IIf(DayLightSaving = "Y", "Yes", "No") & " ; ", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Modified", "Day Light Saving", beforeData, "Day Light Saving = " & IIf(DayLightSaving = "Y", "Yes", "No") & " ; ", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Day Light Saving change failed.")
				End If
			End If
		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				CSCommonHelper.WriteLog("Modified", "Day Light Saving", beforeData, "Day Light Saving = " & IIf(DayLightSaving = "Y", "Yes", "No") & " ; ", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Day Light Saving change failed. Exception is : " & ex.Message)
			End If
			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
		End Try

		Return result

	End Function

End Class