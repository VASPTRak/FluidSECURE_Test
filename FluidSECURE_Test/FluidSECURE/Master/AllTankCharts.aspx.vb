Imports log4net
Imports log4net.Config

Public Class AllTankCharts
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllTankCharts))

	Dim OBJMaster As MasterBAL = New MasterBAL()

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

					If (Request.QueryString("Filter") = Nothing) Then
						Session("TankChartConditions") = ""
						Session("TankChartDDL_ColumnName") = ""
						Session("TankCharttxt_valueNameValue") = ""
						Session("TankChartDDL_CustomerValue") = ""
					End If

					If (Not Session("TankChartDDL_ColumnName") Is Nothing And Not Session("TankChartDDL_ColumnName") = "") Then
						DDL_ColumnName.SelectedValue = Session("TankChartDDL_ColumnName")
						If (Not Session("TankChartDDL_CustomerValue") Is Nothing And Not Session("TankChartDDL_CustomerValue") = "") Then
							If (Session("TankChartDDL_CustomerValue") <> 0) Then
								DDL_Customer.SelectedValue = Session("TankChartDDL_CustomerValue")
								DDL_Customer.Visible = True
								txt_value.Visible = False
							Else
								If (Not Session("TankCharttxt_valueNameValue") Is Nothing And Not Session("TankCharttxt_valueNameValue") = "") Then
									txt_value.Text = Session("TankCharttxt_valueNameValue")
								Else
									txt_value.Text = ""
								End If
							End If
						End If
					End If


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
			Session("TankChartConditions") = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions = " and TC.CompanyId=" & Session("CustomerId")
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and TC." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and TC." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")

			ElseIf ((DDL_Customer.SelectedValue <> "0") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and TC." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and TC." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			End If

			Session("TankChartConditions") = strConditions
			Session("TankChartDDL_ColumnName") = DDL_ColumnName.SelectedValue
			Session("TankCharttxt_valueNameValue") = txt_value.Text
			Session("TankChartDDL_CustomerValue") = DDL_Customer.SelectedValue

			Dim dtTankChart As DataTable = New DataTable()

			dtTankChart = OBJMaster.GetTankChartsByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString())

			Session("dtTankChart") = dtTankChart
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtTankChart IsNot Nothing Then
                If dtTankChart.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtTankChart.Rows.Count)
                End If
            End If
            gvTankChart.DataSource = dtTankChart
			gvTankChart.DataBind()

			ViewState("Column_Name") = "TankChartName"
			ViewState("Sort_Order") = "ASC"

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

			Dim TankChartId As Integer = gvTankChart.DataKeys(gvRow.RowIndex).Values("TankChartId").ToString()

			Response.Redirect("/Master/TankChart.aspx?TankChartId=" & TankChartId, False)

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
			dtColumns = OBJMaster.GetTankChartColumnNameForSearch()

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
			Dim dt As DataTable = CType(Session("dtTankChart"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvTankChart.DataSource = dt
			gvTankChart.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder
		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTankChart_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvTankChart.Sorting
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
			log.Error("Error occurred in gvTankChart_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTankChart_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvTankChart.PageIndexChanging
		Try
			gvTankChart.PageIndex = e.NewPageIndex

			Dim dtTankChart As DataTable = Session("dtTankChart")

			gvTankChart.DataSource = dtTankChart
			gvTankChart.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvTankChart_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal TankChartId As String) As String
		Try
			Dim OBJMaster = New MasterBAL()
			Dim beforeData As String = ""

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(TankChartId)
			End If

			Dim result As Integer = OBJMaster.DeleteTankChart(TankChartId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Tank Charts", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Tank Charts", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Tank Chart deletion failed.")
				End If
			End If

			Return result

		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/TankChart")
	End Sub

	Private Shared Function CreateData(TankChartId As Integer) As String
		Try
			Dim data As String = ""

			Dim dtTankChart As DataTable = New DataTable()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtTankChart = OBJMaster.GetTankChartByTankChartId(TankChartId)

			data = "TankChartId = " & TankChartId & " ; " &
					"TankChartNumber = " & dtTankChart.Rows(0)("TankChartNumber").ToString().Replace(",", " ") & " ; " &
					"TankChartName = " & dtTankChart.Rows(0)("TankChartName").ToString().Replace(",", " ") & " ; " &
					"Description = " & dtTankChart.Rows(0)("Description").ToString().Replace(",", " ") & " ; " &
					"TankSize = " & dtTankChart.Rows(0)("TankSize").ToString().Replace(",", " ") & " ; " &
					"Company = " & dtTankChart.Rows(0)("CompanyName").ToString().Replace(",", " ") & " ; " &
					"Entries = " & dtTankChart.Rows(0)("Entries").ToString().Replace(",", " ") & " ; " &
					"FuelIncrement = " & dtTankChart.Rows(0)("FuelIncrement").ToString().Replace(",", " ") & " ; " &
					"FuelTypeId = " & dtTankChart.Rows(0)("FuelTypeId").ToString().Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function


End Class