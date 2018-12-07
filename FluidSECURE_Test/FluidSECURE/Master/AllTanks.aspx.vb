Imports log4net
Imports log4net.Config

Public Class AllTanks
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllTanks))

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

					BindColumns()
					BindCustomer()
					If (Request.QueryString("Filter") = Nothing) Then
						Session("TankConditions") = ""
						Session("TankDDL_ColumnName") = ""
						Session("Tanktxt_valueNameValue") = ""
						Session("TankDDL_CustomerValue") = ""
					End If

					If (Not Session("TankDDL_ColumnName") Is Nothing And Not Session("TankDDL_ColumnName") = "") Then
						DDL_ColumnName.SelectedValue = Session("TankDDL_ColumnName")
						If (Not Session("TankDDL_CustomerValue") Is Nothing And Not Session("TankDDL_CustomerValue") = "") Then
							If (Session("TankDDL_CustomerValue") <> 0) Then
								DDL_Customer.SelectedValue = Session("TankDDL_CustomerValue")
								DDL_Customer.Visible = True
								txt_value.Visible = False
							Else
								If (Not Session("Tanktxt_valueNameValue") Is Nothing And Not Session("Tanktxt_valueNameValue") = "") Then
									txt_value.Text = Session("Tanktxt_valueNameValue")
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

	'Private Sub BindAllTank()
	'    Try

	'        OBJMaster = New MasterBAL()
	'        Dim dtTank As DataTable = New DataTable()

	'        dtTank = OBJMaster.GetTanks(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())

	'        gvTank.DataSource = dtTank
	'        gvTank.DataBind()

	'    Catch ex As Exception

	'        log.Error("Error occurred in BindAllTank Exception is :" + ex.Message)
	'        ErrorMessage.Visible = True
	'        ErrorMessage.InnerText = "Error occurred while getting Tank, please try again later."

	'    End Try

	'End Sub

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetTankColumnNameForSearch()

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
			ErrorMessage.InnerText = "Error occurred while getting customers, please try again later."

		End Try
	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			Dim strConditions As String = ""

			Session("TankConditions") = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions = " and CustomerId=" & Session("CustomerId")
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and " + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
			ElseIf ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and " + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			End If

			Session("TankConditions") = strConditions
			Session("TankDDL_ColumnName") = DDL_ColumnName.SelectedValue
			Session("Tanktxt_valueNameValue") = txt_value.Text
			Session("TankDDL_CustomerValue") = DDL_Customer.SelectedValue

			If strConditions.Contains("CustomerId") Then strConditions = strConditions.Replace("CustomerId", "t.CustomerId")
			If strConditions.Contains("ExportCode") Then strConditions = strConditions.Replace("ExportCode", "t.ExportCode")
			If strConditions.Contains("TankChartId") Then strConditions = strConditions.Replace("CustomerId", "t.TankChartId")

			OBJMaster = New MasterBAL()
			Dim dtTank As DataTable = New DataTable()


			dtTank = OBJMaster.GetTankbyConditions(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtTank") = dtTank
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtTank IsNot Nothing Then
                If dtTank.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtTank.Rows.Count)
                End If
            End If
            gvTank.DataSource = dtTank
			gvTank.DataBind()

			ViewState("Column_Name") = "TankId"
			ViewState("Sort_Order") = "desc"
			RebindData("TankId", "desc")

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

			Dim TankId As Integer = gvTank.DataKeys(gvRow.RowIndex).Values("TankId").ToString()

			Response.Redirect("Tank?TankId=" & TankId, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try

			Dim dt As DataTable = CType(Session("dtTank"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvTank.DataSource = dt
			gvTank.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder

		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTank_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvTank.Sorting
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
			log.Error("Error occurred in gvTank_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTank_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvTank.PageIndexChanging
		Try
			gvTank.PageIndex = e.NewPageIndex
			Dim dtTank As DataTable = Session("dtTank")

			gvTank.DataSource = dtTank
			gvTank.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvTank_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
		Try

			If (DDL_ColumnName.SelectedValue = "CustomerId") Then
				DDL_Customer.Visible = True
				txt_value.Visible = False
				DDL_Customer.Focus()
			Else
				DDL_Customer.Visible = False
				txt_value.Visible = True
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

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal TankId As String) As String
		Try

			Dim beforeData As String = ""
			Dim OBJMaster = New MasterBAL()
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(TankId)
			End If


			Dim result As Integer = OBJMaster.DeleteTank(TankId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result <> 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Tank", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Tank", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Tank deletion failed.")
				End If
			End If


			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)

		Response.Redirect("~/Master/Tank")

	End Sub

	Private Shared Function CreateData(TankId As Integer) As String
		Try
			Dim dtTank As DataTable = New DataTable()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtTank = OBJMaster.GetTankbyId(TankId)


			Dim data As String = "TankId = " & dtTank.Rows(0)("TankId").ToString() & " ; " &
								  "Tank Number = " & dtTank.Rows(0)("TankNumber").ToString().Replace(",", " ") & " ; " &
								  "Tank Name = " & dtTank.Rows(0)("TankName").ToString().Replace(",", " ") & " ; " &
								  "ADDRESS = " & dtTank.Rows(0)("TankAddress").ToString().Replace(",", " ") & " ; " &
								  "REFILL_NOT = " & dtTank.Rows(0)("RefillNotice").ToString().Replace(",", " ") & " ; " &
								  "Company = " & dtTank.Rows(0)("CustomerName").ToString().Replace(",", " ") & " ; " &
								  "Export Code = " & dtTank.Rows(0)("ExportCode").ToString().Replace(",", " & ") & " ; " &
								  "PROBEMacAddress = " & dtTank.Rows(0)("PROBEMacAddress").ToString().Replace(",", " ") & " ; " &
								  "TANK_CHART = " & dtTank.Rows(0)("TankChartName").ToString().Replace(",", " ") & " ; "
			'"PRODUCT = " & dtTank.Rows(0)("FuelType").ToString().Replace(",", " ") & " ; " &

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function
End Class