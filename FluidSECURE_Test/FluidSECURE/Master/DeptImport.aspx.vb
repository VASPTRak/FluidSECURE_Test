Imports log4net
Imports log4net.Config
Imports System.IO

Public Class DeptImport
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(DeptImport))
	Dim OBJMaster As MasterBAL = New MasterBAL()

	Dim strLog As String = ""

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			LB_Error.Visible = False
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then
					GetCustomers(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

					ddlCustomer.Focus()

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

		End Try
	End Sub

	Private Sub GetCustomers(personId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCustomer As DataTable = New DataTable()
			dtCustomer = OBJMaster.GetCustomerDetailsByPersonID(personId, RoleId, Session("CustomerId").ToString())
			ddlCustomer.DataSource = dtCustomer
			ddlCustomer.DataTextField = "CustomerName"
			ddlCustomer.DataValueField = "CustomerId"
			ddlCustomer.DataBind()
			ddlCustomer.Items.Insert(0, New ListItem("Select Company", "0"))

			If (Not Session("RoleName") = "SuperAdmin") Then
				ddlCustomer.SelectedIndex = 1
				ddlCustomer.Enabled = False
				ddlCustomer.Visible = False
				divCompany.Visible = False
			End If

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				ddlCustomer.SelectedIndex = 1

			End If

		Catch ex As Exception

			log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			Dim fileExt As String

			fileExt = System.IO.Path.GetExtension(FU_Dept.FileName)

			If (fileExt <> ".csv" And fileExt <> ".txt") Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Only .csv or .txt files allowed to upload!."

				Return
			End If

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				CSCommonHelper.WriteLog("Import", "Department", "File Name = " & FU_Dept.FileName.Replace(",", " ") & " ; Company =  " & ddlCustomer.SelectedItem.Text.Replace(",", " "), "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
			End If

			System.Threading.Thread.Sleep("5000")

			Dim allContent As String = New StreamReader(FU_Dept.FileContent, Encoding.GetEncoding("iso-8859-1")).ReadToEnd()

			Dim returnCnts As String = ConvertStringIntoDatTableAndInsertData(allContent)



			message.Visible = True
			message.InnerText = returnCnts.Split(";")(0) & " departments imported successfully "

			If (strLog.Trim() <> "") Then
				LB_Error.Visible = True
				Session("Errorlogs") = strLog
				message.InnerText = message.InnerText + " , " + returnCnts.Split(";")(1) + " caused some errors."
			End If

		Catch ex As Exception
			message.InnerText = message.InnerText + " , Error occurred, Please try after some time."
			log.Error("Exception occurred on btnUpload_Click. Exception is : " & ex.Message)
		Finally
			ddlCustomer.Focus()
		End Try

	End Sub

	Protected Function ConvertStringIntoDatTableAndInsertData(data As String) As String
		'Dim insertDt As DataTable = New DataTable()
		Dim cnt As Integer = 0
		Dim ErrorCnt As Integer = 0
		Dim returnsCnt As String = ""
		Dim currentDateTime As String = ""
		Try
			currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

		Catch ex As Exception

			currentDateTime = DateTime.Now.ToString()

		End Try
		Try
			Dim Lines As String() = data.Split(New Char() {ControlChars.Lf})
			Dim Fields As String()
			Fields = Lines(0).Split(New Char() {","c})
			Dim Cols As Integer = Fields.GetLength(0)
			Dim dt As New DataTable()
			dt.TableName = "DepartmentDetails"
			dt.Columns.Add("DepartmentNumber", GetType(String))
			dt.Columns.Add("DepartmentName", GetType(String))
			dt.Columns.Add("AccountNumber", GetType(String))
			dt.Columns.Add("Address", GetType(String))
			dt.Columns.Add("Address2", GetType(String))
			dt.Columns.Add("ExportCode", GetType(String))
			dt.Columns.Add("RowIndex", GetType(Integer))

			Dim Row As DataRow
			For i As Integer = 3 To Lines.GetLength(0) - 1
				Fields = Lines(i).Split(New Char() {","c})

				If (Fields.Length = dt.Columns.Count - 1) Then
					Row = dt.NewRow()
					For f As Integer = 0 To Cols - 1
						Row(f) = Fields(f).ToString().Replace("'", "").Trim()
					Next
					Row(6) = i + 1
					dt.Rows.Add(Row)
				ElseIf (Fields.Length < 6 And Fields.Length > 1) Then

					Row = dt.NewRow()
					For f As Integer = 0 To Fields.Length - 1
						Row(f) = Fields(f).ToString().Replace("'", "").Trim()
					Next

					For f As Integer = Fields.Length To 5
						Row(f) = ""
					Next

					Row(6) = i + 1
					dt.Rows.Add(Row)
				ElseIf (Fields.Length > 2) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Invalid input format. Incorrect number of columns for the row number " & (i + 1) & ". Please correct the data and retry!"
					ErrorCnt = ErrorCnt + 1
				End If
			Next

			Dim rowIndex As Integer = 0
			Dim isDirty As Boolean = False
			Dim CheckIdExists As Integer = 0

			For Each dr As DataRow In dt.Rows

				isDirty = False

				rowIndex = dr("RowIndex")

				If (dr("DepartmentNumber") <> "") Then

					CheckIdExists = OBJMaster.DeptIDExists(dr("DepartmentNumber"), 0, Convert.ToInt32(ddlCustomer.SelectedValue), dr("DepartmentName"))

					If CheckIdExists = -1 Then
						strLog = strLog & Environment.NewLine & currentDateTime & "-- Department Number (" & dr("DepartmentNumber") & ") is already exist. Check Row  " & rowIndex & " & column 1 in uploaded file."
						isDirty = True
					ElseIf (dr("DepartmentNumber").ToString().Length > 10) Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Department Number (" & dr("DepartmentNumber") & ") is must be less than equal to 10 characters. Check Row  " & rowIndex & " & column 1 in uploaded file."
						isDirty = True
					End If
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Department Number field is required. Check Row  " & rowIndex & " & column 1 in uploaded file."
					isDirty = True
				End If

				If (dr("DepartmentName") <> "") Then
					CheckIdExists = OBJMaster.DeptIDExists(dr("DepartmentNumber"), 0, Convert.ToInt32(ddlCustomer.SelectedValue), dr("DepartmentName"))

					If CheckIdExists = -2 Then
						strLog = strLog & Environment.NewLine & currentDateTime & "-- Department Name (" & dr("DepartmentName") & ") is already exist. Check Row  " & rowIndex & " & column 2 in uploaded file."
						isDirty = True
					ElseIf (dr("DepartmentNumber").ToString().Length > 20) Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Department Name (" & dr("DepartmentName") & ") is must be less than equal to 20 characters. Check Row  " & rowIndex & " & column 2 in uploaded file."
						isDirty = True
					End If
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Department Name field is required. Check Row  " & rowIndex & " & column 2 in uploaded file."
					isDirty = True
				End If

				If (dr("AccountNumber") <> "") Then
					If (dr("AccountNumber").ToString().Length > 10) Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Account number (" & dr("AccountNumber") & ") is must be less than equal to 10 characters. Check Row  " & rowIndex & " & column 3 in uploaded file."
						isDirty = True
					End If
				End If

				If (dr("Address").ToString().Length > 25) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Address (" & dr("Address") & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 4 in uploaded file."
					isDirty = True
				End If


				If (dr("Address2").ToString().Length > 25) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Address 2 (" & dr("Address2") & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 5 in uploaded file."
					isDirty = True
				End If

				If (dr("ExportCode").ToString().Length > 25) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Export code (" & dr("ExportCode") & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 6 in uploaded file."
					isDirty = True
				End If


				If (isDirty = False) Then
					Dim result As Integer = InsertRecord(dr)
					If (result = 1) Then
						cnt = cnt + 1
					End If
				Else
					ErrorCnt = ErrorCnt + 1
				End If

			Next


			Return cnt & ";" & ErrorCnt

		Catch ex As Exception
			log.Error("Exception occured while importing file. Exception is : " & ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Exception occurred while importing file. Exception is : " & ex.Message
			Return "0;0"
		End Try
	End Function

	Private Function InsertRecord(dr As DataRow) As Integer ', dtSelectedFuels As DataTable
		Try

			Dim result As Integer = OBJMaster.SaveUpdateDept(0, dr("DepartmentName"), dr("DepartmentNumber"), dr("Address"), dr("Address2"), dr("AccountNumber"), ddlCustomer.SelectedValue, dr("ExportCode"), Convert.ToInt32(Session("PersonId")), 0, 0.0, 0.0, 0.0, 0.0)

			Return 1

		Catch ex As Exception

			log.Error("Exception occured while importing file. Exception is : " & ex.Message)

			Return 0
		End Try

	End Function

	Protected Sub LB_Error_Click(sender As Object, e As EventArgs)

		Dim ms As New MemoryStream()
		Dim tw As TextWriter = New StreamWriter(ms)
		tw.WriteLine(Session("Errorlogs"))
		tw.Flush()
		Dim bytes As Byte() = ms.ToArray()
		ms.Close()
		Response.Clear()
		Response.ContentType = "application/force-download"
		Response.AddHeader("content-disposition", "attachment;filename=Errorlogs.txt")
		Response.BinaryWrite(bytes)
		Response.[End]()

	End Sub

	Protected Sub lnkTemplate_Click(sender As Object, e As EventArgs)

		Dim path As String = Server.MapPath("\Content\Templates\DepartmentImportTemplate.csv")
		Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)
		If file.Exists Then 'set appropriate headers
			Response.Clear()
			Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
			Response.AddHeader("Content-Length", file.Length.ToString())
			Response.ContentType = "application/octet-stream"
			Response.WriteFile(file.FullName)
			Response.End()
		Else
			Response.Write("This file does not exist.")
		End If

	End Sub

End Class