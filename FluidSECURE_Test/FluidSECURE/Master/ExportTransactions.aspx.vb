Imports System.IO
Imports log4net
Imports log4net.Config
Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Public Class ExportTransactions
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(ExportTransactions))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Support" Then 'Or Session("RoleName") = "Reports Only"
                'Access denied 
                Response.Redirect("/home")
			Else
				If Not IsPostBack Then

					BindTransactionStatus()

					txtFileName.Text = "TransactionDetails"
                txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
                txtTransactionDateTo.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
                txtTransactionTimeFrom.Text = "12:00 AM"
					txtTransactionTimeTo.Text = "11:59 PM"

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					DDL_ExportOption_SelectedIndexChanged(Nothing, Nothing)
                BindTransactionExportFields()
					txtTransactionDateFrom.Focus()
				Else

					txtTransactionDateFrom.Text = Request.Form(txtTransactionDateFrom.UniqueID)
					txtTransactionDateTo.Text = Request.Form(txtTransactionDateTo.UniqueID)
					txtTransactionTimeFrom.Text = Request.Form(txtTransactionTimeFrom.UniqueID)
					txtTransactionTimeTo.Text = Request.Form(txtTransactionTimeTo.UniqueID)

				End If

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

			End If


		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

		End Try
	End Sub

	Private Sub BindCustomer(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

			If (Not Session("RoleName") = "SuperAdmin") Then
				DDL_Customer.SelectedIndex = 1
				DDL_Customer.Enabled = False
				DDL_Customer.Visible = False
				divCompany.Visible = False
			End If

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				DDL_Customer.SelectedIndex = 1

			End If

			DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub btnExportTransactions_Click(sender As Object, e As EventArgs)
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			ErrorMessage.Visible = False
			message.Visible = False
			ErrorMessage.InnerText = ""
			message.InnerText = ""

            SaveTransactionExportFields()


			Dim dsTran As DataSet = New DataSet()
			If (DDL_CustomizedExportTemplate.SelectedValue = 0) Then
				dsTran = GetTransactions(1)
				If (Not dsTran Is Nothing) Then

					If (dsTran.Tables.Count < 1 Or dsTran.Tables(0).Rows.Count <= 0) Then

						ErrorMessage.InnerText = "Data not found against selected criteria."
						ErrorMessage.Visible = True
						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
						Return
					End If
				Else

					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
					Return

				End If
				Response.Clear()
				Response.Buffer = True
				If (DDL_ExportOption.SelectedValue = "1") Then
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".txt")
				ElseIf (DDL_ExportOption.SelectedValue = "2") Then
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".csv")
				ElseIf (DDL_ExportOption.SelectedValue = "3") Then

					WriteExcelWithNPOI("xls", dsTran.Tables(0), txtFileName.Text & ".xls", True, False)
					Return
				ElseIf (DDL_ExportOption.SelectedValue = "4") Then
					WriteExcelWithNPOI("xlsx", dsTran.Tables(0), txtFileName.Text & ".xlsx", True, False)
					Return
				Else
					Response.AddHeader("content-disposition",
					"attachment;filename=" & txtFileName.Text & ".csv")
				End If

				Response.Charset = ""
				Response.ContentType = "application/text"

				Dim separator As String = ","c

				If (DDL_ExportOption.SelectedValue = "2" Or DDL_ExportOption.SelectedValue = "1") Then
					If (DDL_Separator.SelectedValue = "none") Then
						separator = ""
					ElseIf (DDL_Separator.SelectedValue = "comma") Then
						separator = ","c
					Else
						separator = DDL_Separator.SelectedValue
					End If
				End If


				Dim sb As New StringBuilder()
				For k As Integer = 0 To dsTran.Tables(0).Columns.Count - 1
					sb.Append(dsTran.Tables(0).Columns(k).ColumnName + separator)
				Next
				'append new line
				sb.Append(vbCr & vbLf)
				For i As Integer = 0 To dsTran.Tables(0).Rows.Count - 1
					For k As Integer = 0 To dsTran.Tables(0).Columns.Count - 1
						'add separator
						If (dsTran.Tables(0).Columns(k).ColumnName = "Date") Then
							sb.Append(Convert.ToDateTime(dsTran.Tables(0).Rows(i)(k)).ToString(DDL_DateType.SelectedValue).Replace(",", ";") + separator)
						ElseIf (dsTran.Tables(0).Columns(k).ColumnName = "Time") Then
							sb.Append(Convert.ToDateTime(dsTran.Tables(0).Rows(i)(k)).ToString("hh:mm tt").Replace(",", ";") + separator)
						Else
							sb.Append(dsTran.Tables(0).Rows(i)(k).ToString().Replace(",", ";") + separator)

						End If


					Next
					sb = sb.Remove(sb.Length - 1, 1)
					'append new line
					sb.Append(vbCr & vbLf)
				Next
				Response.Output.Write(sb.ToString())
                'Response.Flush()
				Response.End()
			Else
				OBJMaster = New MasterBAL()
				dsTran = GetTransactions(3)

				If (Not dsTran Is Nothing) Then

					If (dsTran.Tables.Count < 1 Or dsTran.Tables(0).Rows.Count <= 0) Then

						ErrorMessage.InnerText = "Data not found against selected criteria."
						ErrorMessage.Visible = True
						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
						Return
					End If
				Else

					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
					Return

				End If

				Dim dsCustomizedExportTemplates As DataSet = New DataSet()
				Dim dtCustomizedExportField As DataTable = New DataTable()

				dsCustomizedExportTemplates = OBJMaster.GetCustomizedExportTemplateById(DDL_CustomizedExportTemplate.SelectedValue)
				dtCustomizedExportField = dsCustomizedExportTemplates.Tables(1)

				Dim dv As DataView = dsTran.Tables(0).DefaultView
				Dim fields As String = String.Join(",", (From row In dtCustomizedExportField.AsEnumerable Select row("FieldName")).ToArray)

				Dim columns() As String = fields.Split(",")

				'For Each dr As DataRow In dtCustomizedExportField.Rows
				'    fields.Add(dr("FieldName"))
				'Next

				Dim dtTemp As DataTable = dv.ToTable(False, columns)

				If (DDL_ExportOption.SelectedValue = "1") Then
					Response.Clear()
					Response.Buffer = True
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".txt")
					Response.Charset = ""
					Response.ContentType = "application/text"

					Dim sb As New StringBuilder()
					'For k As Integer = 0 To dtTemp.Columns.Count - 1
					'    sb.Append(dtTemp.Columns(k).ColumnName)
					'Next
					'append new line
					For i As Integer = 0 To dtTemp.Rows.Count - 1
						For k As Integer = 0 To dtTemp.Columns.Count - 1
							'add separator
							Dim dtTempCustomizedExportField As DataTable = New DataTable()
							Dim dvTemp As DataView = dtCustomizedExportField.DefaultView
							dvTemp.RowFilter = "FieldName='" & dtTemp.Columns(k).ColumnName & "'"
							dtTempCustomizedExportField = dvTemp.ToTable()

							Dim FieldLength As Integer = dtTempCustomizedExportField.Rows(0)("FieldLength").ToString()
							Dim FillCharacter As String = dtTempCustomizedExportField.Rows(0)("PaddingCharacter").ToString()
							Dim justify As String = dtTempCustomizedExportField.Rows(0)("Justify").ToString()

							If (dtTemp.Columns(k).ColumnName = "Date") Then

								Dim FieldColumnValue As String = Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString(DDL_DateType.SelectedValue)
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValueLength
								Dim finalValue As String = ""

								If (FillCharacter = "") Then
									FillCharacter = " "
								End If

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								sb.Append(finalValue)

							ElseIf (dtTemp.Columns(k).ColumnName = "Time") Then
								Dim FieldColumnValue As String = Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString("HHmm")
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValue
								Dim finalValue As String = ""

								If (FillCharacter = "") Then
									FillCharacter = " "
								End If

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								sb.Append(finalValue)

								'sb.Append(Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString("hhmm").PadRight(20, " "))
							Else
								'sb.Append(dtTemp.Rows(i)(k).ToString().Replace(",", ";"))

								Dim FieldColumnValue As String = dtTemp.Rows(i)(k).ToString().Replace(",", ";")
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValue
								Dim finalValue As String = ""

								If (FillCharacter = "") Then
									FillCharacter = " "
								End If

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								sb.Append(finalValue)

							End If
						Next
						'append new line
						sb.Append(vbCr & vbLf)
					Next
					Response.Output.Write(sb.ToString())
                    'Response.Flush()
                    Response.End()
				ElseIf (DDL_ExportOption.SelectedValue = "2") Then
					Response.Clear()
					Response.Buffer = True
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".csv")
					Response.Charset = ""
					Response.ContentType = "application/text"

					Dim sb As New StringBuilder()
					'For k As Integer = 0 To dtTemp.Columns.Count - 1
					'    sb.Append(dtTemp.Columns(k).ColumnName)
					'Next
					'append new line
					For i As Integer = 0 To dtTemp.Rows.Count - 1
						For k As Integer = 0 To dtTemp.Columns.Count - 1
							'add separator
							Dim dtTempCustomizedExportField As DataTable = New DataTable()
							Dim dvTemp As DataView = dtCustomizedExportField.DefaultView
							dvTemp.RowFilter = "FieldName='" & dtTemp.Columns(k).ColumnName & "'"
							dtTempCustomizedExportField = dvTemp.ToTable()

							Dim FieldLength As Integer = dtTempCustomizedExportField.Rows(0)("FieldLength").ToString()
							Dim FillCharacter As String = dtTempCustomizedExportField.Rows(0)("PaddingCharacter").ToString()
							Dim justify As String = dtTempCustomizedExportField.Rows(0)("Justify").ToString()

							If (dtTemp.Columns(k).ColumnName = "Date") Then

								Dim FieldColumnValue As String = Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString(DDL_DateType.SelectedValue)
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValueLength
								Dim finalValue As String = ""

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								If (DDL_Separator.SelectedValue = "none") Then
									sb.Append(finalValue)
								ElseIf (DDL_Separator.SelectedValue = "comma") Then
									sb.Append(finalValue + ","c)
								Else
									sb.Append(finalValue + DDL_Separator.SelectedValue)
								End If


							ElseIf (dtTemp.Columns(k).ColumnName = "Time") Then
								Dim FieldColumnValue As String = Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString("HHmm")
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValue
								Dim finalValue As String = ""

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								If (DDL_Separator.SelectedValue = "none") Then
									sb.Append(finalValue)
								ElseIf (DDL_Separator.SelectedValue = "comma") Then
									sb.Append(finalValue + ","c)
								Else
									sb.Append(finalValue + DDL_Separator.SelectedValue)
								End If

								'sb.Append(Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString("hhmm").PadRight(20, " "))
							Else
								'sb.Append(dtTemp.Rows(i)(k).ToString().Replace(",", ";"))

								Dim FieldColumnValue As String = dtTemp.Rows(i)(k).ToString().Replace(",", ";")
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValue
								Dim finalValue As String = ""

								If (FillCharacter = "") Then
									FillCharacter = " "
								End If

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								If (DDL_Separator.SelectedValue = "none") Then
									sb.Append(finalValue)
								ElseIf (DDL_Separator.SelectedValue = "comma") Then
									sb.Append(finalValue + ","c)
								Else
									sb.Append(finalValue + DDL_Separator.SelectedValue)
								End If

							End If
						Next
						sb = sb.Remove(sb.Length - 1, 1)
						'append new line
						sb.Append(vbCr & vbLf)
					Next
					Response.Output.Write(sb.ToString())
                    'Response.Flush()
					Response.End()
				Else
					Response.Clear()
					Response.Buffer = True
					'If (DDL_ExportOption.SelectedValue = "3") Then
					'	Response.AddHeader("content-disposition",
					'		"attachment;filename=" & txtFileName.Text & ".xls")
					'Else
					'	Response.AddHeader("content-disposition",
					'			"attachment;filename=" & txtFileName.Text & ".xlsx")
					'End If

					'Response.Charset = ""
					'Response.ContentType = "application/text"



					'Dim sb As New StringBuilder()
					'For k As Integer = 0 To dtTemp.Columns.Count - 1
					'    sb.Append(dtTemp.Columns(k).ColumnName)
					'Next
					'append new line

					Dim dtFinal As DataTable = New DataTable()
					'dtFinal = dtTemp.Clone()

					For l As Integer = 0 To dtTemp.Columns.Count - 1
						dtFinal.Columns.Add(dtTemp.Columns(l).ColumnName, System.Type.[GetType]("System.String"))
					Next

					For i As Integer = 0 To dtTemp.Rows.Count - 1
						Dim drNew As DataRow = dtFinal.NewRow()

						For k As Integer = 0 To dtTemp.Columns.Count - 1
							'add separator
							Dim dtTempCustomizedExportField As DataTable = New DataTable()
							Dim dvTemp As DataView = dtCustomizedExportField.DefaultView
							dvTemp.RowFilter = "FieldName='" & dtTemp.Columns(k).ColumnName & "'"
							dtTempCustomizedExportField = dvTemp.ToTable()

							Dim FieldLength As Integer = dtTempCustomizedExportField.Rows(0)("FieldLength").ToString()
							Dim FillCharacter As String = dtTempCustomizedExportField.Rows(0)("PaddingCharacter").ToString()
							Dim justify As String = dtTempCustomizedExportField.Rows(0)("Justify").ToString()

							If (dtTemp.Columns(k).ColumnName = "Date") Then

								Dim FieldColumnValue As String = Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString(DDL_DateType.SelectedValue)
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValueLength
								Dim finalValue As String = ""

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								'dtFinal.Rows(i)(k) = finalValue.ToString()
								drNew(k) = finalValue
								'sb.Append(finalValue + ","c)


							ElseIf (dtTemp.Columns(k).ColumnName = "Time") Then
								Dim FieldColumnValue As String = Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString("HHmm")
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValue
								Dim finalValue As String = ""

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								'sb.Append(finalValue + ","c)
								'dtFinal.Rows(i)(k) = finalValue
								drNew(k) = finalValue
								'sb.Append(Convert.ToDateTime(dtTemp.Rows(i)(k)).ToString("hhmm").PadRight(20, " "))
							Else
								'sb.Append(dtTemp.Rows(i)(k).ToString().Replace(",", ";"))

								Dim FieldColumnValue As String = dtTemp.Rows(i)(k).ToString().Replace(",", ";")
								Dim FieldColumnValueLength As Integer = FieldColumnValue.Length
								'Dim paddingLength As Integer = FieldLength - FieldColumnValue
								Dim finalValue As String = ""

								If (FillCharacter = "") Then
									FillCharacter = " "
								End If

								If (FieldLength > FieldColumnValueLength) Then
									If (justify = "Left") Then
										finalValue = FieldColumnValue.PadLeft(FieldLength, FillCharacter)
									ElseIf (justify = "Right") Then
										finalValue = FieldColumnValue.PadRight(FieldLength, FillCharacter)
									Else
										finalValue = FieldColumnValue
									End If
								ElseIf (FieldLength < FieldColumnValueLength) Then
									finalValue = FieldColumnValue.Substring(0, FieldLength)
								Else
									finalValue = FieldColumnValue
								End If

								'sb.Append(finalValue + ","c)
								'dtFinal.Rows(i)(k) = finalValue
								drNew(k) = finalValue

							End If
						Next

						dtFinal.Rows.Add(drNew)

						'sb = sb.Remove(sb.Length - 1, 1)
						''append new line
						'sb.Append(vbCr & vbLf)
					Next

					If (DDL_ExportOption.SelectedValue = "3") Then
						WriteExcelWithNPOI("xls", dtFinal, txtFileName.Text & ".xls", False, False)
					Else
						WriteExcelWithNPOI("xlsx", dtFinal, txtFileName.Text & ".xlsx", False, False)
					End If
					Return
					'Response.Output.Write(sb.ToString())
					'Response.Flush()
					'Response.End()
				End If
			End If
		Catch ex As Exception

			log.Error("Error occurred in btnExportTransactions_Click Exception is :" + ex.Message)

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while export transactions, please try again later."
		End Try
	End Sub

	Private Function GetTransactions(flag As Integer) As DataSet
		OBJMaster = New MasterBAL()
		Dim dSTran As DataSet = New DataSet()
		Try


			'start end date
			Dim startDate As DateTime
			startDate = Convert.ToDateTime(Request.Form(txtTransactionDateFrom.UniqueID) + " " + Request.Form(txtTransactionTimeFrom.UniqueID)).ToString()

			Dim endDate As DateTime
			endDate = Convert.ToDateTime(Request.Form(txtTransactionDateTo.UniqueID) + " " + Request.Form(txtTransactionTimeTo.UniqueID)).ToString()


			Dim strConditions As String = ""
			If (DDL_Customer.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and c.CustomerId = " + DDL_Customer.SelectedValue, strConditions + " and c.CustomerId = " + DDL_Customer.SelectedValue)
			End If

			'strConditions = IIf(strConditions = "", " and ISNULL(t.IsMissed,0) = " + DDL_Missed.SelectedValue, strConditions + " and ISNULL(t.IsMissed,0) = " + DDL_Missed.SelectedValue)
			If (DDL_TransactionStatus.SelectedValue = "2") Then
				strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
			ElseIf (DDL_TransactionStatus.SelectedValue = "0") Then
				strConditions = (IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
			ElseIf (DDL_TransactionStatus.SelectedValue = "1") Then
				strConditions = (IIf(strConditions = "", " and (ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and (ISNULL(T.TransactionStatus,0) = 1 And ISNULL(T.IsMissed,0)= 1)  and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
			End If

			strConditions += " order by t.TransactionDateTime"

			Dim DecimalType As Integer = 0
			If ddl_DecimalQTY.SelectedValue = 2 Then
				DecimalType = Convert.ToInt32(ddl_DecimalType.SelectedValue)
			End If

			'get data from server
			dSTran = OBJMaster.ExportTransactions(startDate.ToString(), endDate.ToString(), strConditions, flag, Convert.ToInt32(ddl_DecimalQTY.SelectedValue.ToString()), DecimalType)

		Catch ex As Exception
			log.Error("Error occurred in GetDefaultFormtTransactions Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while Getting transactions, please try again later."
		End Try

		Return dSTran

	End Function

	Protected Sub DDL_ExportOption_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try


			If (DDL_ExportOption.SelectedValue = "2") Then
				DDL_Separator.Visible = True
				seperatorDiv.Visible = True
			Else
				DDL_Separator.Visible = False
				seperatorDiv.Visible = False
			End If

		Catch ex As Exception
			log.Error("Error occurred in DDL_ExportOption_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			BindCustomizedExportTemplates(DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindCustomizedExportTemplates(CompanyId As Integer, PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()

			Dim dtCustomizedExportTemplateNames As DataTable = New DataTable()
			Dim CustomerId As Integer = 0

			Dim strConditions As String = ""

			strConditions = " and CET.CompanyId=" & CompanyId

			dtCustomizedExportTemplateNames = OBJMaster.GetCustomizedExportTemplatesByCondition(strConditions, PersonId, RoleId)

			DDL_CustomizedExportTemplate.DataSource = dtCustomizedExportTemplateNames
			DDL_CustomizedExportTemplate.DataTextField = "CustomizedExportTemplateName"
			DDL_CustomizedExportTemplate.DataValueField = "CustomizedExportTemplateId"
			DDL_CustomizedExportTemplate.DataBind()


			DDL_CustomizedExportTemplate.Items.Insert(0, New ListItem("Default Export Template", "0"))
			If (CompanyId <> 0) Then
				If (dtCustomizedExportTemplateNames.Rows.Count > 0) Then
					DDL_CustomizedExportTemplate.SelectedIndex = 1
				Else
					DDL_CustomizedExportTemplate.SelectedIndex = 0
					ErrorMessage.InnerText = "You have not created customized export template. Please create the same from ""Create Customized Export"" screen or use default template to export"
					ErrorMessage.Visible = True
				End If

			End If
		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub bttnExportTemplate_Click(sender As Object, e As EventArgs)
		Try
			ErrorMessage.Visible = False
			message.Visible = False
			ErrorMessage.InnerText = ""
			message.InnerText = ""


			Dim dsTran As DataSet = New DataSet()
			If (DDL_CustomizedExportTemplate.SelectedValue = 0) Then
				dsTran = GetTransactions(1)
				If (dsTran Is Nothing) Then
					ErrorMessage.InnerText = "Error occured while exporting template, please try again after sometime."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
					Return
				End If
				Response.Clear()
				Response.Buffer = True
				If (DDL_ExportOption.SelectedValue = "1") Then
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".txt")
				ElseIf (DDL_ExportOption.SelectedValue = "2") Then
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".csv")
				ElseIf (DDL_ExportOption.SelectedValue = "3") Then
					WriteExcelWithNPOI("xls", dsTran.Tables(0), txtFileName.Text & ".xls", True, True)
					Return
					'Response.AddHeader("content-disposition",
					'		"attachment;filename=" & txtFileName.Text & ".xls")
				ElseIf (DDL_ExportOption.SelectedValue = "4") Then
					WriteExcelWithNPOI("xlsx", dsTran.Tables(0), txtFileName.Text & ".xlsx", True, True)
					Return
					'Response.AddHeader("content-disposition",
					'		"attachment;filename=" & txtFileName.Text & ".xlsx")
				Else
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".csv")
				End If

				Response.Charset = ""
				Response.ContentType = "application/text"

				Dim separator As String = ","c

				If (DDL_ExportOption.SelectedValue = "2" Or DDL_ExportOption.SelectedValue = "1") Then
					If (DDL_Separator.SelectedValue = "none") Then
						separator = ""
					ElseIf (DDL_Separator.SelectedValue = "comma") Then
						separator = ","c
					Else
						separator = DDL_Separator.SelectedValue
					End If
				End If


				Dim sb As New StringBuilder()
				For k As Integer = 0 To dsTran.Tables(0).Columns.Count - 1
					sb.Append(dsTran.Tables(0).Columns(k).ColumnName + separator)
				Next
				'append new line
				sb.Append(vbCr & vbLf)

				Response.Output.Write(sb.ToString())
                'Response.Flush()
				Response.End()
			Else
				OBJMaster = New MasterBAL()
				dsTran = GetTransactions(3)

				If (dsTran Is Nothing) Then
					ErrorMessage.InnerText = "Error occured while exporting template, please try again after sometime."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
					Return
				End If

				Dim dsCustomizedExportTemplates As DataSet = New DataSet()
				Dim dtCustomizedExportField As DataTable = New DataTable()

				dsCustomizedExportTemplates = OBJMaster.GetCustomizedExportTemplateById(DDL_CustomizedExportTemplate.SelectedValue)
				dtCustomizedExportField = dsCustomizedExportTemplates.Tables(1)

				Dim dv As DataView = dsTran.Tables(0).DefaultView
				Dim fields As String = String.Join(",", (From row In dtCustomizedExportField.AsEnumerable Select row("FieldName")).ToArray)

				Dim columns() As String = fields.Split(",")

				'For Each dr As DataRow In dtCustomizedExportField.Rows
				'    fields.Add(dr("FieldName"))
				'Next

				Dim dtTemp As DataTable = dv.ToTable(False, columns)

				If (DDL_ExportOption.SelectedValue = "1") Then
					Response.Clear()
					Response.Buffer = True
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".txt")
					Response.Charset = ""
					Response.ContentType = "application/text"

					Dim sb As New StringBuilder()
					For k As Integer = 0 To dtTemp.Columns.Count - 1
						sb.Append(dtTemp.Columns(k).ColumnName)
					Next

					Response.Output.Write(sb.ToString())
                    'Response.Flush()
					Response.End()
				ElseIf (DDL_ExportOption.SelectedValue = "2") Then
					Response.Clear()
					Response.Buffer = True
					Response.AddHeader("content-disposition",
							"attachment;filename=" & txtFileName.Text & ".csv")
					Response.Charset = ""
					Response.ContentType = "application/text"
					Dim separator As String = ","c

					If (DDL_ExportOption.SelectedValue = "2" Or DDL_ExportOption.SelectedValue = "1") Then
						If (DDL_Separator.SelectedValue = "none") Then
							separator = ""
						ElseIf (DDL_Separator.SelectedValue = "comma") Then
							separator = ","c
						Else
							separator = DDL_Separator.SelectedValue
						End If
					End If
					Dim sb As New StringBuilder()
					For k As Integer = 0 To dtTemp.Columns.Count - 1
						sb.Append(dtTemp.Columns(k).ColumnName + separator)
					Next

					Response.Output.Write(sb.ToString())
                    'Response.Flush()
					Response.End()
				Else
					Response.Clear()
					Response.Buffer = True
					If (DDL_ExportOption.SelectedValue = "3") Then
						WriteExcelWithNPOI("xls", dtTemp, txtFileName.Text & ".xls", True, True)
						'Response.AddHeader("content-disposition",
						'	"attachment;filename=" & txtFileName.Text & ".xls")
					Else
						WriteExcelWithNPOI("xlsx", dtTemp, txtFileName.Text & ".xlsx", True, True)
						'Response.AddHeader("content-disposition",
						'		"attachment;filename=" & txtFileName.Text & ".xlsx")
					End If

					'Response.Charset = ""
					'Response.ContentType = "application/text"

					'Dim sb As New StringBuilder()
					'For k As Integer = 0 To dtTemp.Columns.Count - 1
					'	sb.Append(dtTemp.Columns(k).ColumnName + ","c)
					'Next

					'Response.Output.Write(sb.ToString())
					'Response.Flush()
					'Response.End()
				End If
			End If
		Catch ex As Exception

			log.Error("Error occurred in bttnExportTemplate_Click Exception is :" + ex.Message)

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while export template, please try again later."
		End Try
	End Sub

	'Private Sub GenerasteExcel(dtFinal As DataTable)
	'	''Create a dummy GridView

	'	'Dim GridView1 As New GridView()
	'	'GridView1.AllowPaging = False
	'	'GridView1.DataSource = dtFinal
	'	'GridView1.DataBind()


	'	'Response.Clear()
	'	'Response.Buffer = True
	'	'Response.AddHeader("content-disposition", "attachment;filename=DataTable.xls")
	'	'Response.Charset = ""
	'	'Response.ContentType = "application/vnd.ms-excel"
	'	'Dim sw As New StringWriter()
	'	'Dim hw As New HtmlTextWriter(sw)

	'	'For i As Integer = 0 To GridView1.Rows.Count - 1
	'	'	'Apply text style to each Row
	'	'	GridView1.Rows(i).Attributes.Add("class", "textmode")
	'	'Next

	'	'GridView1.RenderControl(hw)

	'	''style to format numbers to string

	'	'Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"

	'	'Response.Write(style)

	'	'Response.Output.Write(sw.ToString())

	'	'Response.Flush()

	'	'Response.End()

	'	'Using pck As ExcelPackage = New ExcelPackage()
	'	'	Dim ws As ExcelWorksheet = pck.Workbook.Worksheets.Add("Sheet1")
	'	'	ws.Cells("A1").LoadFromDataTable(dsTran.Tables(0), True)
	'	'	Dim s As Byte() = pck.GetAsByteArray()
    '	'	Response.AddHeader("Content-Type", "application/excel")

	'	'	'Response.AddHeader("Content-Disposition", "attachment;filename=DataTable.xlsx")

	'	'	Response.BinaryWrite(s)
	'	'	'Response.Output.Write(pck.GetAsByteArray())
	'	'	Response.Flush()
	'	'	Response.End()


	'	'End Using




	'	'Dim GridView1 As New GridView()
	'	'GridView1.AllowPaging = False
	'	'GridView1.DataSource = dsTran.Tables(0)
	'	'GridView1.DataBind()


	'	''Response.Clear()
	'	''Response.Buffer = True
	'	''Response.AddHeader("content-disposition", "attachment;filename=DataTable.xls")
	'	'Response.Charset = ""
	'	'Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
	'	'Dim sw As New StringWriter()
	'	'Dim hw As New HtmlTextWriter(sw)

	'	'For i As Integer = 0 To GridView1.Rows.Count - 1
	'	'	'Apply text style to each Row
	'	'	GridView1.Rows(i).Attributes.Add("class", "textmode")
	'	'Next

	'	'GridView1.RenderControl(hw)

	'	''style to format numbers to string

	'	''Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"

	'	''Response.Write(style)
	'	'Response.Output.Write(sw.ToString())
	'	'Response.Flush()
	'	'Response.End()

	'End Sub

	Public Sub WriteExcelWithNPOI(ByVal extension As String, ByVal dt As DataTable, filename As String, isDefaultTemplate As Boolean, isOnlyTemplate As Boolean)
		Try


			Dim workbook As IWorkbook

			If extension = "xlsx" Then
				workbook = New XSSFWorkbook()
			ElseIf extension = "xls" Then
				workbook = New HSSFWorkbook()
			Else
				Throw New Exception("This format is not supported")
			End If

			Dim sheet1 As ISheet = workbook.CreateSheet("Sheet 1")
			If (isOnlyTemplate = True) Then
				Dim row1 As IRow = sheet1.CreateRow(0)

				For j As Integer = 0 To dt.Columns.Count - 1
					Dim cell As ICell = row1.CreateCell(j)
					Dim columnName As String = dt.Columns(j).ToString()
					cell.SetCellValue(columnName)
				Next
			Else
				If (isDefaultTemplate = True) Then

					Dim row1 As IRow = sheet1.CreateRow(0)

					For j As Integer = 0 To dt.Columns.Count - 1
						Dim cell As ICell = row1.CreateCell(j)
						Dim columnName As String = dt.Columns(j).ToString()
						cell.SetCellValue(columnName)
					Next

					For i As Integer = 0 To dt.Rows.Count - 1
						Dim row As IRow = sheet1.CreateRow(i + 1)

						For j As Integer = 0 To dt.Columns.Count - 1
							Dim cell As ICell = row.CreateCell(j)
							Dim columnName As String = dt.Columns(j).ToString()
							cell.SetCellValue(dt.Rows(i)(columnName).ToString())
						Next
					Next

				Else

					For i As Integer = 0 To dt.Rows.Count - 1
						Dim row As IRow = sheet1.CreateRow(i)

						For j As Integer = 0 To dt.Columns.Count - 1
							Dim cell As ICell = row.CreateCell(j)
							Dim columnName As String = dt.Columns(j).ToString()
							cell.SetCellValue(dt.Rows(i)(columnName).ToString())
						Next
					Next

				End If

			End If

			Using exportData = New MemoryStream()
				Response.Clear()
				workbook.Write(exportData)

				If extension = "xlsx" Then
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
					Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", filename))
					Response.BinaryWrite(exportData.ToArray())
				ElseIf extension = "xls" Then
					Response.ContentType = "application/vnd.ms-excel"
					Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", filename))
					Response.BinaryWrite(exportData.GetBuffer())
				End If

				Response.[End]()
			End Using

		Catch ex As Exception
			log.Error("Error occurred in WriteExcelWithNPOI Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindTransactionStatus()
		Try
			DDL_TransactionStatus.Items.Insert(0, New ListItem("All", "3"))
			DDL_TransactionStatus.Items.Insert(1, New ListItem(ConfigurationManager.AppSettings("CompletedText").ToString(), "2"))
			DDL_TransactionStatus.Items.Insert(2, New ListItem(ConfigurationManager.AppSettings("NotStartedText").ToString(), "0"))
			DDL_TransactionStatus.Items.Insert(3, New ListItem(ConfigurationManager.AppSettings("MissedText").ToString(), "1"))

		Catch ex As Exception
			log.Error("Error occurred in BindTransactionStatus Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub ddl_DecimalQTY_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			If ddl_DecimalQTY.SelectedValue = 2 Then
				divDecimailType.Visible = True
			Else
				divDecimailType.Visible = False
			End If

		Catch ex As Exception
			log.Error("Error occurred in BindTransactionStatus Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub


    Protected Sub BindTransactionExportFields()
        Try
            OBJMaster = New MasterBAL()
            Dim dtExportFields As DataTable = New DataTable()

            dtExportFields = OBJMaster.GetALLPersonWiseExportTransactionFields(Convert.ToInt32(Session("PersonId")))

            If (dtExportFields IsNot Nothing) Then
                If (dtExportFields.Rows.Count > 0) Then
                    DDL_DateType.SelectedValue = dtExportFields.Rows(0)("DateFormat")
                    DDL_TransactionStatus.SelectedValue = dtExportFields.Rows(0)("TransactionStatus")
                    DDL_Customer.SelectedValue = dtExportFields.Rows(0)("CustomerId")
                    DDL_ExportOption.SelectedValue = dtExportFields.Rows(0)("FileType")
                    DDL_Separator.SelectedValue = dtExportFields.Rows(0)("Separator")
                    DDL_CustomizedExportTemplate.SelectedValue = dtExportFields.Rows(0)("CustomizedExportTemplate")
                    txtFileName.Text = dtExportFields.Rows(0)("Nametothefile")
                    ddl_DecimalQTY.SelectedValue = dtExportFields.Rows(0)("AddDecimal")
                    ddl_DecimalType.SelectedValue = dtExportFields.Rows(0)("DecimalType")
                End If
                DDL_ExportOption_SelectedIndexChanged(Nothing, Nothing)
                ddl_DecimalQTY_SelectedIndexChanged(Nothing, Nothing)
            End If
        Catch ex As Exception
            log.Error("Error occurred in BindTransactionExportFields Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while export, please try again later."
        End Try
    End Sub

    Protected Sub SaveTransactionExportFields()
        Try
            OBJMaster = New MasterBAL()
            Dim dtExportFields As DataTable = New DataTable()
            OBJMaster.SavePersonWiseExportTransactionFields(Convert.ToInt32(Session("PersonId")), DDL_DateType.SelectedValue.ToString(), DDL_TransactionStatus.SelectedValue.ToString(),
                                                            DDL_ExportOption.SelectedValue.ToString(), DDL_CustomizedExportTemplate.SelectedValue.ToString(), ddl_DecimalQTY.SelectedValue.ToString(),
                                                            DDL_Customer.SelectedValue.ToString(), DDL_Separator.SelectedValue.ToString(), txtFileName.Text, ddl_DecimalType.SelectedValue.ToString())
        Catch ex As Exception
            log.Error("Error occurred in SaveTransactionExportFields Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while export, please try again later."
        End Try
    End Sub
End Class