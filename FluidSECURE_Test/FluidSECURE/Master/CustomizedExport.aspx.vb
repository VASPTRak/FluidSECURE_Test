Imports System.Drawing
Imports log4net.Config
Imports System.Linq
Imports log4net

Public Class CustomizedExport
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CustomizedExport))

	Dim OBJMaster As MasterBAL
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			message1.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")
			Else
				If Not IsPostBack Then
					BindCustomers(Session("PersonId").ToString(), Session("RoleId").ToString())
					BindCustomizedExportFields()
					If (Not Request.QueryString("CustomizedExportTemplateId") = Nothing And Not Request.QueryString("CustomizedExportTemplateId") = "") Then
						hdfCustomizedExportTemplateId.Value = Request.QueryString("CustomizedExportTemplateId")
						BindCustomizedExportTemplateById(Request.QueryString("CustomizedExportTemplateId"))
						lblHeader.Text = "Edit Customized Export"
						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message1.Visible = True
							message.InnerText = "Record saved"
							message1.InnerText = "To set up additional options for your customized export, please see ""Automatic Export Settings"" in the Export menu"
						End If
					Else
						lblHeader.Text = "Create Customized Export"
						BindCustomizedExportTemplateById(0)
					End If
				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try

	End Sub

	Protected Sub btnMoveRight_Click(sender As Object, e As EventArgs)
		Try

			ErrorMessage.Visible = False
			ErrorMessage.InnerText = ""

			Dim dtExportColumns As DataTable = New DataTable()
			dtExportColumns = ViewState("dtExportColumns")
			dtExportColumns.Rows.Clear()
			For Each gr As GridViewRow In gvExportColumns.Rows
				Dim dr As DataRow = dtExportColumns.NewRow()
				dr("FieldName") = TryCast(gr.FindControl("lblFieldName"), Label).Text
				dr("FieldLength") = TryCast(gr.FindControl("txtFieldLength"), TextBox).Text
				dr("PaddingCharacter") = TryCast(gr.FindControl("DDL_PaddingCharacter"), DropDownList).SelectedValue
				dr("Justify") = TryCast(gr.FindControl("DDL_justification"), DropDownList).SelectedValue
				dr("FieldId") = gvExportColumns.DataKeys(gr.RowIndex).Values("FieldId").ToString()
				dr("Index") = gvExportColumns.DataKeys(gr.RowIndex).Values("Index").ToString()
				dtExportColumns.Rows.Add(dr)
			Next

			'dtExportColumns = ViewState("dtExportColumns")
			Dim tempListItems As ListBox = New ListBox()
			'tempListItems = Me.lstColumns
			Dim dtCustomizedExportFields As DataTable = New DataTable()
			dtCustomizedExportFields = ViewState("dtCustomizedExportFields")

			For Each item As ListItem In lstColumns.Items
				Dim rowCount As Integer = dtExportColumns.Rows.Count

				If (item.Selected = True) Then

					Dim dr As DataRow = dtExportColumns.NewRow()
					dr("FieldName") = item.Text
					dr("PaddingCharacter") = ""
					dr("Justify") = "None"
					Dim dtTemp As DataTable = New DataTable()
					Dim dv As DataView = dtCustomizedExportFields.DefaultView
					dv.RowFilter = "FieldId=" & item.Value
					dtTemp = dv.ToTable()
					dr("FieldLength") = dtTemp.Rows(0)("DefaultLength")
					dr("FieldId") = item.Value
					dr("Index") = rowCount + 1
					dtExportColumns.Rows.Add(dr)
					tempListItems.Items.Add(item)
				End If

			Next

			For Each item As ListItem In tempListItems.Items
				lstColumns.Items.Remove(item)
			Next
			If (lstColumns.Items.Count > 0) Then
				lstColumns.Items(0).Selected = True
			End If
			dtExportColumns.DefaultView.Sort = "Index ASC"
			dtExportColumns = dtExportColumns.DefaultView.ToTable()
			dtExportColumns.AcceptChanges()

			ViewState("dtExportColumns") = dtExportColumns

			gvExportColumns.DataSource = dtExportColumns
			gvExportColumns.DataBind()

		Catch ex As Exception
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while moving record right, please try again later."

			log.Error("Error occurred in  btnMoveRight_Click Exception is :" + ex.Message)
		End Try
	End Sub

	Protected Sub btnMoveLeft_Click(sender As Object, e As EventArgs)
		Try
			ErrorMessage.Visible = False
			ErrorMessage.InnerText = ""

			Dim dtExportColumns As DataTable = New DataTable()
			dtExportColumns = ViewState("dtExportColumns")
			dtExportColumns.Rows.Clear()
			For Each gr As GridViewRow In gvExportColumns.Rows
				Dim dr As DataRow = dtExportColumns.NewRow()
				dr("FieldName") = TryCast(gr.FindControl("lblFieldName"), Label).Text
				dr("FieldLength") = TryCast(gr.FindControl("txtFieldLength"), TextBox).Text
				dr("PaddingCharacter") = TryCast(gr.FindControl("DDL_PaddingCharacter"), DropDownList).SelectedValue
				dr("Justify") = TryCast(gr.FindControl("DDL_justification"), DropDownList).SelectedValue
				dr("FieldId") = gvExportColumns.DataKeys(gr.RowIndex).Values("FieldId").ToString()
				dr("Index") = gvExportColumns.DataKeys(gr.RowIndex).Values("Index").ToString()
				dtExportColumns.Rows.Add(dr)
			Next

			For Each row As GridViewRow In gvExportColumns.Rows

				Dim RDB_UpDwonRow As CheckBox = TryCast(row.FindControl("RDB_UpDwonRow"), CheckBox)
				If (RDB_UpDwonRow.Checked = True) Then

					Dim lblFieldName As Label = TryCast(row.FindControl("lblFieldName"), Label)

					Dim FieldName = lblFieldName.Text
					Dim FieldId = gvExportColumns.DataKeys(row.RowIndex).Values("FieldId").ToString()

					Dim lstitem = New ListItem()
					lstitem.Text = FieldName
					lstitem.Value = FieldId
					lstColumns.Items.Add(lstitem)


					Dim dv = dtExportColumns.DefaultView
					dv.RowFilter = "FieldId <> '" & FieldId & "'"
					dtExportColumns = dv.ToTable()

				End If
			Next
			dtExportColumns.DefaultView.Sort = "Index ASC"
			dtExportColumns = dtExportColumns.DefaultView.ToTable()
			dtExportColumns.AcceptChanges()

			ViewState("dtExportColumns") = dtExportColumns

			gvExportColumns.DataSource = dtExportColumns
			gvExportColumns.DataBind()

			SortListBox()

		Catch ex As Exception
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while moving record left, please try again later."

			log.Error("Error occurred in  btnMoveLeft_Click Exception is :" + ex.Message)
		End Try
	End Sub

	Protected Sub gvExportColumns_RowDataBound(sender As Object, e As GridViewRowEventArgs)
		Try
			If e.Row.RowType = DataControlRowType.DataRow Then
				Dim dtCustomizedExportFields As DataTable = New DataTable()
				dtCustomizedExportFields = ViewState("dtCustomizedExportFields")
				Dim FieldId As String = gvExportColumns.DataKeys(e.Row.RowIndex).Values("FieldId").ToString()

				For Each dr As DataRow In dtCustomizedExportFields.Rows
					If (dr("FieldId") = FieldId) Then
						e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml(dr("BgColour"))
						Exit For
					End If
				Next

			End If
		Catch ex As Exception
			log.Error("Error occurred in gvExportColumns_RowDataBound Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnMoveUp_Click(sender As Object, e As EventArgs)
		Try
			ErrorMessage.Visible = False
			ErrorMessage.InnerText = ""
			Dim rowIndex As Integer = 0
			For Each row As GridViewRow In gvExportColumns.Rows
				Dim RDB_UpDwonRow As RadioButton = TryCast(row.FindControl("RDB_UpDwonRow"), RadioButton)
				If (RDB_UpDwonRow.Checked = True) Then
					rowIndex = row.RowIndex
				End If
			Next

			If (rowIndex = 0) Then
				'ErrorMessage.Visible = True
				'ErrorMessage.InnerText = "You Cant move record Up"

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NoUpDown('You can not move record up.');", True)
				Return
			Else
				Dim dtExportColumns As DataTable = New DataTable()
				dtExportColumns = ViewState("dtExportColumns")

				Dim value As Integer = Convert.ToInt32(dtExportColumns.Rows(rowIndex)("Index").ToString())

				dtExportColumns.Rows(rowIndex)("Index") = Convert.ToInt32(value) - 1
				dtExportColumns.Rows(rowIndex - 1)("Index") = value
				dtExportColumns.DefaultView.Sort = "Index ASC"
				dtExportColumns = dtExportColumns.DefaultView.ToTable()
				dtExportColumns.AcceptChanges()

				ViewState("dtExportColumns") = dtExportColumns

				gvExportColumns.DataSource = dtExportColumns
				gvExportColumns.DataBind()
				For Each row As GridViewRow In gvExportColumns.Rows
					If (rowIndex - 1 = row.RowIndex) Then
						Dim RDB_UpDwonRow As RadioButton = TryCast(row.FindControl("RDB_UpDwonRow"), RadioButton)
						RDB_UpDwonRow.Checked = True
						RDB_UpDwonRow.Focus()
						row.BackColor = System.Drawing.ColorTranslator.FromHtml("#0762ac")
						'row.ForeColor = Color.White
					End If
				Next
			End If

		Catch ex As Exception
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while moving record up, please try again later."

			log.Error("Error occurred in  btnMoveUp_Click Exception is :" + ex.Message)
		End Try
	End Sub

	Protected Sub btnMoveDown_Click(sender As Object, e As EventArgs)
		Try
			ErrorMessage.Visible = False
			ErrorMessage.InnerText = ""
			Dim rowIndex As Integer = 0
			For Each row As GridViewRow In gvExportColumns.Rows
				Dim RDB_UpDwonRow As RadioButton = TryCast(row.FindControl("RDB_UpDwonRow"), RadioButton)
				If (RDB_UpDwonRow.Checked = True) Then
					rowIndex = row.RowIndex
				End If
			Next

			If (rowIndex = gvExportColumns.Rows.Count - 1) Then
				'ErrorMessage.Visible = True
				'ErrorMessage.InnerText = "You Cant move record down"

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NoUpDown('You can not move record down.');", True)

				Return
			Else
				Dim dtExportColumns As DataTable = New DataTable()
				dtExportColumns = ViewState("dtExportColumns")

				Dim value As Integer = Convert.ToInt32(dtExportColumns.Rows(rowIndex)("Index").ToString())

				dtExportColumns.Rows(rowIndex)("Index") = Convert.ToInt32(value) + 1
				dtExportColumns.Rows(rowIndex + 1)("Index") = value
				dtExportColumns.DefaultView.Sort = "Index ASC"
				dtExportColumns = dtExportColumns.DefaultView.ToTable()
				dtExportColumns.AcceptChanges()

				ViewState("dtExportColumns") = dtExportColumns

				gvExportColumns.DataSource = dtExportColumns
				gvExportColumns.DataBind()
				For Each row As GridViewRow In gvExportColumns.Rows
					If (rowIndex + 1 = row.RowIndex) Then
						Dim RDB_UpDwonRow As RadioButton = TryCast(row.FindControl("RDB_UpDwonRow"), RadioButton)
						RDB_UpDwonRow.Checked = True
						RDB_UpDwonRow.Focus()
						row.BackColor = System.Drawing.ColorTranslator.FromHtml("#0762ac")
						'row.ForeColor = Color.White

					End If
				Next
			End If
		Catch ex As Exception
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while movig record down , please try again later."

			log.Error("Error occurred in  btnMoveDown_Click Exception is :" + ex.Message)
		End Try
	End Sub

	Protected Sub btnExportTemplate_Click(sender As Object, e As EventArgs)
		Try
			ErrorMessage.Visible = False
			ErrorMessage.InnerText = ""

			Dim dtExportColumns As DataTable = New DataTable()
			dtExportColumns = ViewState("dtExportColumns")

			If (dtExportColumns.Rows.Count = 0) Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select at least one column in template"
				Return
			End If

			Response.Clear()
			Response.Buffer = True
			Response.AddHeader("content-disposition",
					"attachment;filename=ExportTemplate.csv")
			Response.Charset = ""
			Response.ContentType = "application/text"

			Dim sb As New StringBuilder()
			'For k As Integer = 0 To dtExportColumns.Columns.Count - 1
			'    sb.Append(dtExportColumns.Columns(k).ColumnName + ","c)
			'Next
			For k As Integer = 0 To dtExportColumns.Rows.Count - 1
				sb.Append(dtExportColumns.Rows(k)("FieldName") + ","c)
			Next

			'append new line
			sb.Append(vbCr & vbLf)

			Response.Output.Write(sb.ToString())
            'Response.Flush()
			Response.End()
		Catch ex As Exception
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while Exporting Customized Template , please try again later."

			log.Error("Error occurred in btnExportTemplate_Click  Exception is :" + ex.Message)
		End Try
	End Sub

	Private Sub BindCustomers(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
		 DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
                DDL_Customer.SelectedIndex = 1
                DDL_Customer.Enabled = False
                DDL_Customer.Visible = False
                divCompany.Visible = False
            End If

            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                If (Session("RoleName") = "GroupAdmin") Then
                    DDL_Customer.SelectedValue = Session("CustomerId")
                Else
                    DDL_Customer.SelectedIndex = 1
                End If
            End If

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting Companies , please try again later."

			log.Error("Error occurred in BindCustomers Exception is :" + ex.Message)

		End Try
	End Sub

	Private Sub BindCustomizedExportFields()
		Try

			OBJMaster = New MasterBAL()
			Dim dtCustomizedExportFields As DataTable = New DataTable()

			dtCustomizedExportFields = OBJMaster.GetCustomizedExportFields()

			ViewState("dtCustomizedExportFields") = dtCustomizedExportFields

			lstColumns.DataSource = dtCustomizedExportFields
			lstColumns.DataTextField = "FieldName"
			lstColumns.DataValueField = "FieldId"
			lstColumns.DataBind()

			SortListBox()

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting CustomizedExportFields , please try again later."

			log.Error("Error occurred in CustomizedExportFields Exception is :" + ex.Message)

		End Try
	End Sub

	Private Sub BindCustomizedExportTemplateById(CustomizedExportTemplateById As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dsCustomizedExportTemplates As DataSet = New DataSet()
			Dim dtCustomizedExportField As DataTable = New DataTable()

		dsCustomizedExportTemplates = OBJMaster.GetCustomizedExportTemplateById(CustomizedExportTemplateById)

			If (dsCustomizedExportTemplates.Tables(0).Rows.Count > 0) Then
                Dim isValid As Boolean = False
                If (Session("RoleName") = "GroupAdmin") Then
                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                    For Each drCusts As DataRow In dtCustOld.Rows
                        If (drCusts("CustomerId") = dsCustomizedExportTemplates.Tables(0).Rows(0)("CompanyId").ToString()) Then
                            isValid = True
                            Exit For
                        End If

                    Next
                End If


                If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                    Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCustOld.Rows(0)("CustomerId").ToString() <> dsCustomizedExportTemplates.Tables(0).Rows(0)("CompanyId").ToString()) Then

						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

						Return
					End If

				End If
			End If


			If (CustomizedExportTemplateById <> 0) Then
				DDL_Customer.SelectedValue = dsCustomizedExportTemplates.Tables(0).Rows(0)("CompanyId")
				txtTemplateName.Text = dsCustomizedExportTemplates.Tables(0).Rows(0)("CustomizedExportTemplateName")
			End If

			dtCustomizedExportField = dsCustomizedExportTemplates.Tables(1)
			ViewState("dtExportColumns") = dtCustomizedExportField

			dtCustomizedExportField.DefaultView.Sort = "Index ASC"
			dtCustomizedExportField = dtCustomizedExportField.DefaultView.ToTable()
			dtCustomizedExportField.AcceptChanges()
			Dim tempListItems As ListBox = New ListBox()

			For Each dr As DataRow In dtCustomizedExportField.Rows
				For Each item As ListItem In lstColumns.Items
					If (dr("FieldId") = item.Value) Then
						tempListItems.Items.Add(item)
					End If
				Next
			Next

			For Each item As ListItem In tempListItems.Items
				lstColumns.Items.Remove(item)
			Next

			gvExportColumns.DataSource = dtCustomizedExportField
			gvExportColumns.DataBind()

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(CustomizedExportTemplateById, True)
			End If

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting Customized Export Template , please try again later."

			log.Error("Error occurred in Customized Export Template Exception is :" + ex.Message)

		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs)
		Dim CustomizedExportTemplateId As Integer = 0
		Dim result As Integer = 0
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			If (Not hdfCustomizedExportTemplateId.Value = Nothing And Not hdfCustomizedExportTemplateId.Value = "") Then

				CustomizedExportTemplateId = hdfCustomizedExportTemplateId.Value

			End If

			Dim isDirty As Boolean = False

			Dim dtCustomizedExportFields As DataTable = New DataTable()
			dtCustomizedExportFields = ViewState("dtCustomizedExportFields")

			For Each grRow As GridViewRow In gvExportColumns.Rows
				Dim FieldLength As Integer = TryCast(grRow.FindControl("txtFieldLength"), TextBox).Text
				Dim FieldId As Integer = gvExportColumns.DataKeys(grRow.RowIndex).Values("FieldId").ToString()
				Dim dtTemp As DataTable = New DataTable()
				Dim dv As DataView = dtCustomizedExportFields.DefaultView
				dv.RowFilter = "FieldId=" & FieldId
				dtTemp = dv.ToTable()
				Dim LBL_FieldLenghtError As Label = TryCast(grRow.FindControl("LBL_FieldLenghtError"), Label)
				If (FieldLength < dtTemp.Rows(0)("MinimumLength") Or FieldLength > dtTemp.Rows(0)("MaximumLength")) Then
					LBL_FieldLenghtError.Text = "Please enter " & dtTemp.Rows(0)("FieldName") & " field length in between " & dtTemp.Rows(0)("MinimumLength") & " & " & dtTemp.Rows(0)("MaximumLength")
					LBL_FieldLenghtError.Visible = True
					isDirty = True
				Else
					LBL_FieldLenghtError.Text = ""
					LBL_FieldLenghtError.Visible = False
				End If
			Next

			If (isDirty = True) Then
				Return
			End If

			Dim CheckCustomizedExportTemplateNameExists As Boolean = False
			OBJMaster = New MasterBAL()

			CheckCustomizedExportTemplateNameExists = OBJMaster.CheckCustomizedExportTemplateNameExist(CustomizedExportTemplateId, txtTemplateName.Text, Convert.ToInt32(DDL_Customer.SelectedValue))

			If CheckCustomizedExportTemplateNameExists = True Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Customized Export Template Name Already Exists."

				Return

			End If

			OBJMaster = New MasterBAL()

			result = OBJMaster.SaveUpdateCustomizedExportTemplates(CustomizedExportTemplateId, DDL_Customer.SelectedValue, txtTemplateName.Text, Convert.ToInt32(Session("PersonId")))
			If result > 0 Then
				Dim dtCustomizedExportTemplateDetails As DataTable = New DataTable()
				'Add CustomizedExportTemplateDetails
				dtCustomizedExportTemplateDetails.Columns.Add("CustomizedExportTemplateId", System.Type.[GetType]("System.Int32"))
				dtCustomizedExportTemplateDetails.Columns.Add("FieldId", System.Type.[GetType]("System.Int32"))
				dtCustomizedExportTemplateDetails.Columns.Add("FieldLength", System.Type.[GetType]("System.Int32"))
				dtCustomizedExportTemplateDetails.Columns.Add("PaddingCharacter", System.Type.[GetType]("System.String"))
				dtCustomizedExportTemplateDetails.Columns.Add("Justify", System.Type.[GetType]("System.String"))

				For Each gr As GridViewRow In gvExportColumns.Rows

					Dim dr As DataRow = dtCustomizedExportTemplateDetails.NewRow()
					dr("CustomizedExportTemplateId") = result
					dr("FieldId") = gvExportColumns.DataKeys(gr.RowIndex).Values("FieldId").ToString()
					dr("FieldLength") = TryCast(gr.FindControl("txtFieldLength"), TextBox).Text
					dr("PaddingCharacter") = TryCast(gr.FindControl("DDL_PaddingCharacter"), DropDownList).SelectedValue
					dr("Justify") = TryCast(gr.FindControl("DDL_justification"), DropDownList).SelectedValue

					dtCustomizedExportTemplateDetails.Rows.Add(dr)

				Next

				OBJMaster = New MasterBAL()
				OBJMaster.InsertCustomizedExportTemplateDetails(dtCustomizedExportTemplateDetails, result)

				If (CustomizedExportTemplateId > 0) Then
					message.Visible = True
					message1.Visible = True
					message.InnerText = "Record saved"
					message1.InnerText = "To set up additional options for your customized export, please see ""Automatic Export Settings"" in the Export menu"
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(CustomizedExportTemplateId, False)
						CSCommonHelper.WriteLog("Modified", "CustomizedExportTemplate", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If

				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result, False)
						CSCommonHelper.WriteLog("Added", "CustomizedExportTemplate", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					'If (IsSaveAndAddNew = True) Then
					'    hdfVehicleId.Value = result
					'    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenPersonVehicleMappingBox(1);", True)

					'    'Response.Redirect(String.Format("~/Master/Vehicle"))
					'Else
					Response.Redirect(String.Format("~/Master/CustomizedExport?CustomizedExportTemplateId={0}&RecordIs=New", result))
					'End If
				End If

			Else
				If (CustomizedExportTemplateId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(CustomizedExportTemplateId, False)
						CSCommonHelper.WriteLog("Modified", "CustomizedExportTemplate", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Customized Export Template update failed")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Customized Export Template update failed, please try again"
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result, False)
						CSCommonHelper.WriteLog("Added", "CustomizedExportTemplate", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Customized Export Template Addition failed")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Customized Export Template Addition failed, please try again"
				End If
			End If


		Catch ex As Exception
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while saving record , please try again later."

			log.Error("Error occurred in  btnSave_Click. Exception is :" + ex.Message)
		Finally
			txtTemplateName.Focus()
		End Try
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/AllCustomizedExportTemplates.aspx")
	End Sub

	Protected Sub RDB_UpDwonRow_CheckedChanged(sender As Object, e As EventArgs)
		Try
			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
							   GridViewRow)

			Dim dtCustomizedExportFields As DataTable = New DataTable()
			dtCustomizedExportFields = ViewState("dtCustomizedExportFields")

			For Each gvRow1 As GridViewRow In gvExportColumns.Rows
				Dim FieldId As String = gvExportColumns.DataKeys(gvRow1.RowIndex).Values("FieldId").ToString()
				For Each dr As DataRow In dtCustomizedExportFields.Rows
					If (dr("FieldId") = FieldId) Then
						gvRow1.BackColor = System.Drawing.ColorTranslator.FromHtml(dr("BgColour"))
						gvRow1.ForeColor = Color.Black
						Exit For
					End If
				Next
			Next

			gvRow.BackColor = System.Drawing.ColorTranslator.FromHtml("#0762ac")
			TryCast(gvRow.FindControl("RDB_UpDwonRow"), RadioButton).Focus()
			'gvRow.ForeColor = Color.White
		Catch ex As Exception
			log.Error("Error occurred in RDB_UpDwonRow_CheckedChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Function CreateData(CustomizedExportTemplateId As Integer, IsBefore As Boolean) As String
		Try

			Dim data As String = ""
			Dim Exportcolumns As String = ""

			data = "CustomizedExportTemplateId = " & CustomizedExportTemplateId & " ; " &
					"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
					"Template Name = " & txtTemplateName.Text.Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Private Sub SortListBox()
		Try
			Dim tempdt As DataTable = New DataTable()
			tempdt.Columns.Add("FieldName", System.Type.[GetType]("System.String"))
			tempdt.Columns.Add("FieldId", System.Type.[GetType]("System.Int32"))

			For Each item As ListItem In lstColumns.Items
				Dim dr As DataRow = tempdt.NewRow()
				dr("FieldName") = item.Text
				dr("FieldId") = item.Value
				tempdt.Rows.Add(dr)
			Next
			Dim dv As DataView = tempdt.DefaultView
			dv.Sort = "FieldName"
			tempdt = dv.Table()

			lstColumns.DataSource = tempdt
			lstColumns.DataTextField = "FieldName"
			lstColumns.DataValueField = "FieldId"
			lstColumns.DataBind()
		Catch ex As Exception
			log.Error("Error occurred in SortListBox Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnClearSelection_Click(sender As Object, e As EventArgs)
		Try
			Dim dtCustomizedExportFields As DataTable = New DataTable()
			dtCustomizedExportFields = ViewState("dtCustomizedExportFields")

			For Each dr As DataRow In dtCustomizedExportFields.Rows
				For Each gr As GridViewRow In gvExportColumns.Rows
					Dim FieldId As String = gvExportColumns.DataKeys(gr.RowIndex).Values("FieldId").ToString()
					If (dr("FieldId") = FieldId) Then
						gr.BackColor = System.Drawing.ColorTranslator.FromHtml(dr("BgColour"))
						TryCast(gr.FindControl("RDB_UpDwonRow"), RadioButton).Checked = False
						Exit For
					End If
				Next
			Next

		Catch ex As Exception
			log.Error("Error occurred in btnClearSelection_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

End Class
