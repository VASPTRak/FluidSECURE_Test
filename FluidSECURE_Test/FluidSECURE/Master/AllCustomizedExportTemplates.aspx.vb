Imports log4net.Config
Imports log4net

Public Class AllCustomizedExportTemplates
	Inherits System.Web.UI.Page

	Dim OBJMaster As MasterBAL = New MasterBAL()
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllCustomizedExportTemplates))

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession('Session Expired. Please Login Again.')", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")
			Else
				If (Not IsPostBack) Then
					BindColumns()
					BindCustomer()
					btnSearch_Click(Nothing, Nothing)
					DDL_ColumnName.Focus()
				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try

	End Sub

	Private Sub BindCustomer()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
			DDL_Customer.DataSource = dtColumns
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try

	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			OBJMaster = New MasterBAL()

			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions = " and CET.CompanyId=" & Session("CustomerId")
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and CET." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and CET." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")

			ElseIf ((DDL_Customer.SelectedValue <> "0") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and CET." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and CET." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			End If

			Dim dtCustomizedExportTemplates As DataTable = New DataTable()

			dtCustomizedExportTemplates = OBJMaster.GetCustomizedExportTemplatesByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString())

			Session("dtCustomizedExportTemplates") = dtCustomizedExportTemplates
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtCustomizedExportTemplates IsNot Nothing Then
                If dtCustomizedExportTemplates.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtCustomizedExportTemplates.Rows.Count)
                End If
            End If
            gvCustomizedExportTemplate.DataSource = dtCustomizedExportTemplates
			gvCustomizedExportTemplate.DataBind()

			ViewState("Column_Name") = "CustomizedExportTemplateId"
			ViewState("Sort_Order") = "DESC"

		Catch ex As Exception

			log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			DDL_ColumnName.Focus()
		End Try
	End Sub

	Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
		Try
			If (DDL_ColumnName.SelectedValue = "CompanyId") Then
				txt_value.Visible = False
				DDL_Customer.Visible = True
				DDL_Customer.Focus()
			Else
				txt_value.Visible = True
				DDL_Customer.Visible = False
				txt_value.Focus()
			End If
			txt_value.Text = ""
			DDL_Customer.SelectedValue = 0
		Catch ex As Exception
			log.Error("Error occurred in DDL_ColumnName_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
		Try

			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
								GridViewRow)

			Dim CustomizedExportTemplateId As Integer = gvCustomizedExportTemplate.DataKeys(gvRow.RowIndex).Values("CustomizedExportTemplateId").ToString()

			Response.Redirect("CustomizedExport.aspx?CustomizedExportTemplateId=" & CustomizedExportTemplateId, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetCustomizedExportColumnNameForSearch()

			DDL_ColumnName.DataSource = dtColumns
			DDL_ColumnName.DataValueField = "ColumnName"
			DDL_ColumnName.DataTextField = "ColumnEnglishName"
			DDL_ColumnName.DataBind()
			DDL_ColumnName.Items.Insert(0, New ListItem("Select Column", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindColumns Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting search column, please try again later."

		End Try
	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try
			Dim dt As DataTable = CType(Session("dtCustomizedExportTemplates"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvCustomizedExportTemplate.DataSource = dt
			gvCustomizedExportTemplate.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder

		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvCustomizedExportTemplate_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvCustomizedExportTemplate.Sorting
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
			log.Error("Error occurred in gvCustomizedExportTemplate_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub gvCustomizedExportTemplate_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvCustomizedExportTemplate.PageIndexChanging
		Try
			gvCustomizedExportTemplate.PageIndex = e.NewPageIndex

			Dim dtCustomizedExportTemplates As DataTable = Session("dtCustomizedExportTemplates")

			gvCustomizedExportTemplate.DataSource = dtCustomizedExportTemplates
			gvCustomizedExportTemplate.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvCustomizedExportTemplate_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal CustomizedExportTemplateId As String) As String
		Try

			Dim beforeData As String = ""
			Dim OBJMaster = New MasterBAL()
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(CustomizedExportTemplateId)
			End If

			Dim result As Integer = OBJMaster.DeleteCustomizedExport(CustomizedExportTemplateId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "CustomizedExportTemplate", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "CustomizedExportTemplate", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Customized Export Template deletion failed.")
				End If
			End If

			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try

	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/CustomizedExport")
	End Sub

	Private Shared Function CreateData(CustomizedExportTemplateId As Integer) As String
		Try

			Dim data As String = ""

			Dim dsData As DataSet = New DataSet()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dsData = OBJMaster.GetCustomizedExportTemplateById(CustomizedExportTemplateId)

			data = "CustomizedExportTemplateId = " & CustomizedExportTemplateId & " ; " &
					"Company = " & dsData.Tables(0).Rows(0)("Customername").ToString().Replace(",", " ") & " ; " &
					"Template Name = " & dsData.Tables(0).Rows(0)("CustomizedExportTemplateName").ToString().Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class