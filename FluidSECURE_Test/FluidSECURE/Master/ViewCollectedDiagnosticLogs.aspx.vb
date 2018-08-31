Imports System.IO
Imports log4net
Imports log4net.Config

Public Class ViewCollectedDiagnosticLogs
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(ViewCollectedDiagnosticLogs))
	Dim OBJMaster As MasterBAL

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
				If (Not IsPostBack) Then
					If (Not Request.QueryString("PersonId") = Nothing And Not Request.QueryString("PersonId") = "" And Not Request.QueryString("UniqueUserId") = Nothing And Not Request.QueryString("UniqueUserId") = "") Then
						BindGrid(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))
					Else
						Response.Redirect("~/Master/AllPersonnel?Filter=Filter")
					End If
				End If
			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Protected Sub linkDownload_Click(sender As Object, e As EventArgs)
		Try

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
								GridViewRow)

			Dim Path As String = gvDiagnosticLogs.DataKeys(gvRow.RowIndex).Values("Path").ToString()
			Dim FileName As String = gvDiagnosticLogs.DataKeys(gvRow.RowIndex).Values("FileName").ToString()

			Dim fsNew As FileStream = New FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

			Dim FileSize As Long = 0


			FileSize = fsNew.Length

			Dim Buffer As Byte() = New Byte(CInt(FileSize)) {}
			fsNew.Read(Buffer, 0, (CInt(FileSize)))
			fsNew.Close()

			'		Response.Write("<b>File Contents: </b>");



			'Dim file As System.IO.FileInfo = New System.IO.FileInfo(Path)

			'IO.File.ReadAllLines(Path)

			'If file.Exists Then 'set appropriate headers
			Response.Clear()
			Response.AddHeader("Content-Disposition", "attachment; filename=" & FileName)
			'Response.AddHeader("Content-Length", File.Length.ToString())
			Response.ContentType = "application/octet-stream"
			Response.BinaryWrite(Buffer)
			Response.End()

			'Else
			'	ErrorMessage.Visible = True
			'	ErrorMessage.InnerText = "This file does not exist, please try again later."
			'End If

		Catch ex As Exception
			log.Error("Error occurred in linkDownload_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "File is busy, please try again later."
		End Try
	End Sub

	Private Sub BindGrid(PersonId As Integer, UniqueUserId As String)

		Try

			Dim dtLogs As DataTable = New DataTable()

			dtLogs.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
			dtLogs.Columns.Add("FileName", System.Type.[GetType]("System.String"))
			dtLogs.Columns.Add("PersonName", System.Type.[GetType]("System.String"))
			dtLogs.Columns.Add("Email", System.Type.[GetType]("System.String"))
			dtLogs.Columns.Add("Path", System.Type.[GetType]("System.String"))
			dtLogs.Columns.Add("IMEI", System.Type.[GetType]("System.String"))
			dtLogs.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))

			Dim dtPersonnel As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtPersonnel = OBJMaster.GetPersonnelByPersonIdAndId(PersonId, UniqueUserId)

			If (dtPersonnel.Rows.Count = 0) Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Person not found. Please try again later."
		Else
				Dim customerId As Integer = Convert.ToInt32(dtPersonnel.Rows(0)("CustomerId").ToString())
				If (Not Session("RoleName") = "SuperAdmin") Then

					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCustOld.Rows(0)("CustomerId").ToString() <> customerId.ToString()) Then

						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

						Return
					End If

				End If

				Dim filepath As String = Server.MapPath("\DiagnosticLogs\" & PersonId & "\")

				If (Directory.Exists(filepath)) Then
					Dim directories() As String = Directory.GetDirectories(filepath)

					For Each directoryTemp As String In directories

						Dim files() As String = Directory.GetFiles(directoryTemp)
						If (files.Length > 0) Then
							For Each file As String In files

								Dim dr As DataRow = dtLogs.NewRow()
								dr("PersonId") = PersonId
								dr("FileName") = Path.GetFileName(file)
								dr("PersonName") = dtPersonnel.Rows(0)("PersonName")
								dr("Email") = dtPersonnel.Rows(0)("Email")
								dr("Path") = directoryTemp & "\" & Path.GetFileName(file)
								dr("IMEI") = System.IO.Path.GetFileName(directoryTemp)
								dr("CreatedDate") = IO.File.GetCreationTime(file)

								dtLogs.Rows.Add(dr)
							Next
						End If
					Next

					Dim dv As DataView = dtLogs.DefaultView
					dv.Sort = "CreatedDate DESC"
					dtLogs = dv.ToTable()

					Session("dtLogs") = dtLogs
					gvDiagnosticLogs.DataSource = dtLogs
					gvDiagnosticLogs.DataBind()

					ViewState("Column_Name") = "FileName"
					ViewState("Sort_Order") = "DESC"

				Else
					gvDiagnosticLogs.Visible = False
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Files not found."
				End If

			End If

		Catch ex As Exception
			log.Error("Error occurred in BindGrid Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting details, please try again later."
		End Try
	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try
			Dim dt As DataTable = CType(Session("dtLogs"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvDiagnosticLogs.DataSource = dt
			gvDiagnosticLogs.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder

		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvDiagnosticLogs_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvDiagnosticLogs.Sorting
		Try
			If e.SortExpression = ViewState("Column_Name").ToString() Then
				If ViewState("Sort_Order").ToString() = "ASC" Then
					RebindData(e.SortExpression, "DESC")
				Else
					RebindData(e.SortExpression, "ASC")
				End If

			Else
				RebindData(e.SortExpression, "ASC")
			End If

		Catch ex As Exception
			log.Error("Error occurred in gvDiagnosticLogs_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvDiagnosticLogs_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvDiagnosticLogs.PageIndexChanging
		Try
			gvDiagnosticLogs.PageIndex = e.NewPageIndex
			Dim dtLogs As DataTable = Session("dtLogs")

			gvDiagnosticLogs.DataSource = dtLogs
			gvDiagnosticLogs.DataBind()
		Catch ex As Exception
			log.Error("Error occurred in gvDiagnosticLogs_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	Protected Sub btnDeleteALL_Click(sender As Object, e As EventArgs)
		Try

			Dim PersonId As String = Request.QueryString("PersonId")

			Dim folderpath As String = Server.MapPath("\DiagnosticLogs\" & PersonId & "\")

			DeleteDirectory(folderpath)

			message.Visible = True
			message.InnerText = "Records deleted successfully."

			BindGrid(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))

		Catch ex As Exception
			log.Error("Error occurred in btnDeleteALL_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while deleting records details, please try again later."
		End Try
	End Sub

	Protected Sub btnDeleteSelected_Click(sender As Object, e As EventArgs)
		Try

			For Each rows As GridViewRow In gvDiagnosticLogs.Rows

				Dim filePath = gvDiagnosticLogs.DataKeys(rows.RowIndex).Values("Path").ToString()

				If (TryCast(rows.FindControl("CHK_Selectlog"), CheckBox).Checked = True) Then

					File.Delete(filePath)

				End If

			Next
			message.Visible = True
			message.InnerText = "Records deleted successfully."
			BindGrid(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))
		Catch ex As Exception
			log.Error("Error occurred in btnDeleteSelected_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while deleting records details, please try again later."
		End Try
	End Sub

	Private Sub DeleteDirectory(path As String)
		Try

			If Directory.Exists(path) Then
				'Delete all files from the Directory
				For Each filepath As String In Directory.GetFiles(path)
					File.Delete(filepath)
				Next
				'Delete all child Directories
				For Each dir As String In Directory.GetDirectories(path)
					DeleteDirectory(dir)
				Next
				'Delete a Directory
				Directory.Delete(path)
			End If
		Catch ex As Exception
			log.Error("Error occurred in DeleteDirectory Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while deleting data, please try again later."
		End Try
	End Sub

End Class
