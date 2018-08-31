Imports log4net
Imports log4net.Config

Public Class TankChartDetails
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TankChartDetails))

	Dim OBJMaster As MasterBAL
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			If CSCommonHelper.CheckSessionExpired() = False Then

				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")
			Else
				If (Not Request.QueryString("TankChartId") = Nothing And Not Request.QueryString("TankChartId") = "") Then
					If (Not IsPostBack) Then
						lblHeader.Text = "Tank Chart Details"
						HDF_TankChartId.Value = Request.QueryString("TankChartId")
						BindTankChartDetails(Request.QueryString("TankChartId"))

					End If
				End If
			End If

		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

		End Try
	End Sub

	Private Sub BindTankChartDetails(TankChartId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtTankChartDetail As DataTable = New DataTable()
			dtTankChartDetail = OBJMaster.GetTankChartDetailsByTankChartId(TankChartId)

			Session("dtTankChartDetail") = dtTankChartDetail
			BindGrid()
		Catch ex As Exception
			log.Error("Error occurred in BindTankChartDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Tank Chart Details, please try again later."
		End Try
	End Sub

	Protected Sub gv_TankChartDetails_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)

		BindGrid()

	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
		Response.Redirect("/Master/TankChart.aspx?TankChartId=" & HDF_TankChartId.Value, False)
	End Sub

	Protected Sub OnRowEditing(sender As Object, e As GridViewEditEventArgs)
		Try

			gv_TankChartDetails.EditIndex = e.NewEditIndex
			BindGrid()

		Catch ex As Exception
			log.Error("Error occurred in OnRowEditing Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred OnRowEditing, please try again later."
		End Try
	End Sub

	Protected Sub OnRowUpdating(sender As Object, e As GridViewUpdateEventArgs)
		Dim TankChartDetailId As Integer = 0
		Dim GallonLevel As String = ""
		Dim LncLevel As String = ""
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			Dim row As GridViewRow = gv_TankChartDetails.Rows(e.RowIndex)
			TankChartDetailId = Convert.ToInt32(gv_TankChartDetails.DataKeys(e.RowIndex).Values("TankChartDetailId"))
			Dim TankChartId As Integer = Convert.ToInt32(gv_TankChartDetails.DataKeys(e.RowIndex).Values("TankChartId"))
			GallonLevel = DirectCast(row.Cells(0).FindControl("txtGallonLevel"), TextBox).Text
			LncLevel = DirectCast(row.Cells(0).FindControl("LBL_IncLevel"), Label).Text

            If GallonLevel IsNot Nothing Then

                If GallonLevel = "" Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter Gallon Level and try again."
                    ErrorMessage.Focus()
                    Return
                End If

                Dim resultDecimal As Decimal = 0

                If (GallonLevel <> "" And Not (Decimal.TryParse(GallonLevel, resultDecimal))) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter Gallon Level as decimal and try again."
                    ErrorMessage.Focus()
                    Return
                End If
            End If

            OBJMaster = New MasterBAL()
			OBJMaster.UpdateTankChartDetail(TankChartDetailId, GallonLevel, Session("PersonId"))

			gv_TankChartDetails.EditIndex = -1
			BindTankChartDetails(TankChartId)

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData(TankChartDetailId, LncLevel, GallonLevel)
				CSCommonHelper.WriteLog("Modified", "Tank Charts Details", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
			End If

		Catch ex As Exception
			log.Error("Error occurred in OnRowUpdating Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred OnRowUpdating, please try again later."

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData(TankChartDetailId, LncLevel, GallonLevel)
				CSCommonHelper.WriteLog("Modified", "Tank Charts Details", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank chart detail update failed. Exception is : " & ex.Message)
			End If

		End Try
	End Sub

	Protected Sub OnRowCancelingEdit(sender As Object, e As EventArgs)
		Try
			gv_TankChartDetails.EditIndex = -1
			BindGrid()
		Catch ex As Exception
			log.Error("Error occurred in OnRowCancelingEdit Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindGrid()
		Try

			Dim dtTankChartDetail As DataTable = Session("dtTankChartDetail")
			gv_TankChartDetails.DataSource = dtTankChartDetail
			gv_TankChartDetails.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in BindGrid Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred on binding grid, please try again later."
		End Try
	End Sub

	Protected Sub gv_TankChartDetails_RowDataBound(sender As Object, e As GridViewRowEventArgs)
		Try

			If (e.Row.RowState And DataControlRowState.Edit) = DataControlRowState.Edit Then

				Dim row As GridViewRow = e.Row
				Dim GallonLevel As TextBox = DirectCast(row.Cells(0).FindControl("txtGallonLevel"), TextBox)
				GallonLevel.Focus()

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then

					Dim TankChartDetailId As Integer = Convert.ToInt32(gv_TankChartDetails.DataKeys(row.RowIndex).Values("TankChartDetailId"))

					Dim LBL_IncLevel As Label = DirectCast(row.Cells(0).FindControl("LBL_IncLevel"), Label)

					beforeData = CreateData(TankChartDetailId, LBL_IncLevel.Text, GallonLevel.Text)

				End If

			End If

		Catch ex As Exception
			log.Error("Error occurred in gv_TankChartDetails_RowDataBound Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred on RowDataBound, please try again later."
		End Try
	End Sub

	Private Function CreateData(TankChartDetailId As Integer, IncrementLevel As String, GallonLevel As String) As String
		Try

			Dim data As String = ""
			data = "TankChartDetailId = " & TankChartDetailId & " ; " &
					"IncrementLevel = " & IncrementLevel.Replace(",", " ") & " ; " &
					"GallonLevel = " & GallonLevel.Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class