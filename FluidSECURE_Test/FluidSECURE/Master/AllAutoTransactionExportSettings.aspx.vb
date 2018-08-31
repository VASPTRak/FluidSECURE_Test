Imports log4net
Imports log4net.Config

Public Class AllAutoTransactionExportSettings
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllAutoTransactionExportSettings))

	Dim OBJMaster As MasterBAL = New MasterBAL()

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession('Session Expired. Please Login Again.')", True)
			ElseIf Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "CustomerAdmin" Then
				'Access denied
				Response.Redirect("/home")
				Return
			Else
				If (Not IsPostBack) Then
					BindCustomer()
					BindColumns()
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
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))
		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting customer, please try again later."

		End Try
	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			OBJMaster = New MasterBAL()

			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and ATES.CompanyId=" & Session("CustomerId")
			End If

			Dim dtAutoTransactionExportSettings As DataTable = New DataTable()

			If ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then

				strConditions = IIf(strConditions = "", " and ATES." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and ATES." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")

			End If

			'Dim dtShipments As DataTable = New DataTable()

			'If Session("CompanyNameHeader") <> Nothing Then
			'    If Session("CompanyNameHeader") <> "" And Session("CompanyNameHeader") <> "Select All" Then
			'        dtShipments = OBJMaster.GetShipmentsByCondition(strConditions + " and SD.Company = '" + Session("CompanyNameHeader").ToString() + "' ", Session("RoleId").ToString())
			'    Else
			'        dtShipments = OBJMaster.GetShipmentsByCondition(strConditions, Session("RoleId").ToString())
			'    End If
			'Else
			'    dtShipments = OBJMaster.GetShipmentsByCondition(strConditions, Session("RoleId").ToString())
			'End If

			dtAutoTransactionExportSettings = OBJMaster.GetAutoTransactionExportSettingsByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtAutoTransactionExportSettings") = dtAutoTransactionExportSettings
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtAutoTransactionExportSettings IsNot Nothing Then
                If dtAutoTransactionExportSettings.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtAutoTransactionExportSettings.Rows.Count)
                End If
            End If
            gvAllAutoTransactionExportSettings.DataSource = dtAutoTransactionExportSettings
			gvAllAutoTransactionExportSettings.DataBind()

			ViewState("Column_Name") = "Comapny"
			ViewState("Sort_Order") = "ASC"

		Catch ex As Exception

			log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			DDL_ColumnName.Focus()
		End Try
	End Sub

	Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
		Try

			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
								GridViewRow)

			Dim AutoTransactionExportSettingId As Integer = gvAllAutoTransactionExportSettings.DataKeys(gvRow.RowIndex).Values("AutoTransactionExportSettingId").ToString()

			Response.Redirect("AutoTransactionExportSettings.aspx?AutoTransactionExportSettingId=" & AutoTransactionExportSettingId, False)

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
			dtColumns = OBJMaster.GetAutoTransactionExportSettingColumnNameForSearch()

			DDL_ColumnName.DataSource = dtColumns
			DDL_ColumnName.DataValueField = "ColumnName"
			DDL_ColumnName.DataTextField = "ColumnEnglishName"
			DDL_ColumnName.DataBind()
			'DDL_ColumnName.Items.Insert(0, New ListItem("Select Column", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindColumns Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting search column, please try again later."

		End Try
	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try

			Dim dt As DataTable = CType(Session("dtAutoTransactionExportSettings"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvAllAutoTransactionExportSettings.DataSource = dt
			gvAllAutoTransactionExportSettings.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder

		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvAllAutoTransactionExportSettings_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvAllAutoTransactionExportSettings.Sorting
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
			log.Error("Error occurred in gvAllAutoTransactionExportSettings_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvAllAutoTransactionExportSettings_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvAllAutoTransactionExportSettings.PageIndexChanging
		Try
			gvAllAutoTransactionExportSettings.PageIndex = e.NewPageIndex
			'btnSearch_Click(Nothing, Nothing)

			Dim dtAutoTransactionExportSettings As DataTable = Session("dtAutoTransactionExportSettings")

			gvAllAutoTransactionExportSettings.DataSource = dtAutoTransactionExportSettings
			gvAllAutoTransactionExportSettings.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvAllAutoTransactionExportSettings_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal AutoTransactionExportSettingId As String) As String
		Try
			Dim OBJMaster = New MasterBAL()
			Dim beforeData As String = ""

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(AutoTransactionExportSettingId)
			End If

			Dim result As Integer = OBJMaster.DeleteAutoTransactionExportSetting(AutoTransactionExportSettingId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Automatic Export Settings", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Automatic Export Settings", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "AutoTransactionExportSetting deletion failed.")
				End If
			End If
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/AutoTransactionExportSettings")
	End Sub

	Private Shared Function CreateData(AutoTransactionExportSettingId As Integer) As String
		Try

			Dim data As String = ""

			Dim dtAutoTransactionExportSetting As DataTable = New DataTable()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtAutoTransactionExportSetting = OBJMaster.GetAutoTransactionExportSettingsByAutoTransactionExportSettingId(AutoTransactionExportSettingId)
			Dim ExecutionTime As String = ""
			If (dtAutoTransactionExportSetting.Rows(0)("ExecutionTime").ToString() = "") Then
				ExecutionTime = ""
			Else
				ExecutionTime = Convert.ToDateTime(dtAutoTransactionExportSetting.Rows(0)("ExecutionTime").ToString()).ToString("hh:mm tt")
			End If

			data = "AutoTransactionExportSettingId = " & AutoTransactionExportSettingId & " ; " &
					"Company = " & dtAutoTransactionExportSetting.Rows(0)("CustomerName").ToString().Replace(",", " ") & " ; " &
					"Export Option = " & dtAutoTransactionExportSetting.Rows(0)("ExportOptionText").ToString().Replace(",", " ") & " ; " &
					"FTP Server Path  = " & dtAutoTransactionExportSetting.Rows(0)("FtpServerPath").ToString().Replace(",", " ") & " ; " &
					"FTP Username  = " & dtAutoTransactionExportSetting.Rows(0)("FtpUsername").ToString().Replace(",", " ") & " ; " &
					"FTP Password  = " & dtAutoTransactionExportSetting.Rows(0)("FtpPassword").ToString().Replace(",", " ") & " ; " &
					"Email  = " & dtAutoTransactionExportSetting.Rows(0)("EmailId").ToString().Replace(",", " ") & " ; " &
					"Active = " & dtAutoTransactionExportSetting.Rows(0)("Active").ToString().Replace(",", " ") & " ; " &
					"Execution Time = " & ExecutionTime & " ; " &
					"Time Zone = " & dtAutoTransactionExportSetting.Rows(0)("ActualTimeZone").ToString().Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function
End Class