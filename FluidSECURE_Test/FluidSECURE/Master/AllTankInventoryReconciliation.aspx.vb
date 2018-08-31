Imports log4net
Imports log4net.Config

Public Class AllTankInventoryReconciliation
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllTankInventoryReconciliation))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If (Not IsPostBack) Then
					If (Not Request.QueryString("Type") = Nothing And (Request.QueryString("Type") = "Level" Or Request.QueryString("Type") = "RD")) Then
						hdnEntryType.Value = Request.QueryString("Type")
						BindColumns()
						BindCustomer()
						BindTanks(Convert.ToInt32(DDL_Customer.SelectedValue))
						TransDate.Visible = False
						btnSearch_Click(Nothing, Nothing)
						DDL_ColumnName.Focus()
					Else
						Response.Redirect("/home")
					End If
				End If
			End If

		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Private Sub BindAllTankInvt()
		Try

			OBJMaster = New MasterBAL()
			Dim dtTankInvt As DataTable = New DataTable()

			dtTankInvt = OBJMaster.GetTankInventorys(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString(), hdnEntryType.Value)

			gvTankInvt.DataSource = dtTankInvt
			gvTankInvt.DataBind()

		Catch ex As Exception

			log.Error("Error occurred in BindAllTankInvt Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Tank Inventory, please try again later."

		End Try

	End Sub

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()

			dtColumns = OBJMaster.GetTankInventoryColumnNameForSearch("Level")

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

			If (Not Session("RoleName") = "SuperAdmin") Then
				DDL_Customer.SelectedIndex = 1
			End If

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				DDL_Customer.SelectedIndex = 1
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting customers, please try again later."

		End Try
	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			Dim strConditions As String = " and (ENTRY_TYPE = 'Level' or ENTRY_TYPE = 'RD') "

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and TankInventory.CompanyId=" & Session("CustomerId")
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and TankInventory." + DDL_ColumnName.SelectedValue + " = '" + txt_value.Text + "' ", strConditions + " and TankInventory." + DDL_ColumnName.SelectedValue + " = '" + txt_value.Text + "' ")
			ElseIf ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0" And DDL_ColumnName.SelectedValue = "CompanyId") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and " + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			ElseIf ((DDL_Datetype.SelectedValue <> "") And DDL_ColumnName.SelectedValue <> "0" And DDL_ColumnName.SelectedValue = "DateType") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " = '" + DDL_Datetype.SelectedValue + "'", strConditions + " and " + DDL_ColumnName.SelectedValue + " = '" + DDL_Datetype.SelectedValue + "'")
			ElseIf (DDL_ColumnName.SelectedValue = "InventoryDateTime") Then
				Dim endate = Convert.ToDateTime(Request.Form(txtDateTo.UniqueID)).AddDays(1).ToString("MM/dd/yyyy")
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " between '" + Request.Form(txtDateFrom.UniqueID) + "' and '" + endate + "' ", strConditions + " and " + DDL_ColumnName.SelectedValue + " between '" + Request.Form(txtDateFrom.UniqueID) + "' and '" + endate + "'")
			ElseIf (DDL_ColumnName.SelectedValue <> "0" And DDL_ColumnName.SelectedValue = "TankNumber" And ddl_TankNo.SelectedValue.ToString() <> "0") Then
				strConditions = IIf(strConditions = "", " and TankInventory." + DDL_ColumnName.SelectedValue + " = '" + ddl_TankNo.SelectedValue.ToString() + "' ", strConditions + " and TankInventory." + DDL_ColumnName.SelectedValue + " = '" + ddl_TankNo.SelectedValue.ToString() + "' ")
			End If

			OBJMaster = New MasterBAL()
			Dim dtTankInvt As DataTable = New DataTable()

			dtTankInvt = OBJMaster.GetTankInventorybyConditions(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtTankInvt") = dtTankInvt
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtTankInvt IsNot Nothing Then
                If dtTankInvt.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtTankInvt.Rows.Count)
                End If
            End If
            gvTankInvt.DataSource = dtTankInvt
			gvTankInvt.DataBind()

			ViewState("Column_Name") = "TankInventoryId "
			ViewState("Sort_Order") = "DESC"
			RebindData("TankInventoryId", "DESC")

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

			Dim TankinventoryId As Integer = Convert.ToInt32(gvTankInvt.DataKeys(gvRow.RowIndex).Values("TankInventoryId").ToString())

			Response.Redirect("TankInventoryReconciliation?TankinventoryId=" & TankinventoryId.ToString() + "&Type=" + hdnEntryType.Value, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try
			Dim dt As DataTable = CType(Session("dtTankInvt"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvTankInvt.DataSource = dt
			gvTankInvt.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder
		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTankInvt_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvTankInvt.Sorting
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
			log.Error("Error occurred in gvTankInvt_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTankInvt_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvTankInvt.PageIndexChanging
		Try
			gvTankInvt.PageIndex = e.NewPageIndex
			Dim dtTankInvt As DataTable = Session("dtTankInvt")

			gvTankInvt.DataSource = dtTankInvt
			gvTankInvt.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvTankInvt_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
		Try
			If (DDL_ColumnName.SelectedValue = "CompanyId") Then
				DDL_Customer.Visible = True
				txt_value.Visible = False
				DDL_Datetype.Visible = False
				TransDate.Visible = False
				ddl_TankNo.Visible = False
				OtherThanDate.Visible = True
				hiddenDiv.Visible = True
				DDL_Customer.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "InventoryDateTime") Then

				TransDate.Visible = True
				txtDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
				txtDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
				ddl_TankNo.Visible = False
				hiddenDiv.Visible = False
				OtherThanDate.Visible = False
				TransDate.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "TankNumber") Then
				DDL_Customer.Visible = False
				txt_value.Visible = False
				TransDate.Visible = False
				DDL_Datetype.Visible = False
				OtherThanDate.Visible = True
				hiddenDiv.Visible = True
				ddl_TankNo.Focus()
				ddl_TankNo.Visible = True
			ElseIf (DDL_ColumnName.SelectedValue = "DateType") Then
				DDL_Customer.Visible = False
				txt_value.Visible = False
				TransDate.Visible = False
				DDL_Datetype.Visible = True
				OtherThanDate.Visible = True
				ddl_TankNo.Visible = False
				hiddenDiv.Visible = True
				DDL_Datetype.Focus()
			Else
				DDL_Customer.Visible = False
				txt_value.Visible = True
				TransDate.Visible = False
				DDL_Datetype.Visible = False
				ddl_TankNo.Visible = False
				OtherThanDate.Visible = True
				hiddenDiv.Visible = True
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
	Public Shared Function DeleteRecord(ByVal TankInventoryId As String) As String
		Try
			Dim OBJMaster = New MasterBAL()
			Dim beforeData As String = ""



			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(Convert.ToInt32(TankInventoryId))
			End If
			Dim result As Integer = OBJMaster.DeleteTankInventory(TankInventoryId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "AllTankInventoryReconciliation", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "AllTankInventoryReconciliation", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "AllTankInventoryReconciliation deletion failed.")
				End If
			End If

			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)

		Response.Redirect("~/Master/TankInventoryReconciliation?TankinventoryId=" + "&Type=" + hdnEntryType.Value)

	End Sub

	Private Shared Function CreateData(TankInventoryId As Integer) As String
		Try
			Dim OBJMaster As MasterBAL = New MasterBAL()

			Dim dtTankInventory As DataTable = OBJMaster.GetTankInventorybyId(TankInventoryId)

			Dim data As String = ""
			If dtTankInventory IsNot Nothing And dtTankInventory.Rows.Count > 0 Then
				If dtTankInventory.Rows(0)("DateType").ToString() = "s" Then
					data = "TankInventoryId = " & TankInventoryId & " ; " &
						   "Tank Number = " & dtTankInventory.Rows(0)("TankNumber").ToString().Replace(",", " ") & " ; " &
						   "Company = " & dtTankInventory.Rows(0)("CompanyName").ToString().Replace(",", " ") & " ; " &
						   "Entry Type = Level " & " ; " &
						   "Date Type = Start" & " ; " &
						   "Start Date = " & dtTankInventory.Rows(0)("InventoryDate").ToString().Replace(",", " ") & " ; " &
						   "Start Time = " & dtTankInventory.Rows(0)("InventoryTime").ToString().ToString().Replace(",", " ") & " ; " &
						   "Starting Totalizer/Meter Number = " & dtTankInventory.Rows(0)("Quantity").ToString().Replace(",", " ") & " "
				ElseIf dtTankInventory.Rows(0)("DateType").ToString() = "e" Then
					data = "TankInventoryId = " & TankInventoryId & " ; " &
						   "Tank Number = " & dtTankInventory.Rows(0)("TankNumber").ToString().Replace(",", " ") & " ; " &
						   "Company = " & dtTankInventory.Rows(0)("CompanyName").ToString().Replace(",", " ") & " ; " &
						   "Entry Type = Level " & " ; " &
						   "Date Type = End" & " ; " &
						   "End Date = " & dtTankInventory.Rows(0)("InventoryDate").ToString().Replace(",", " ") & " ; " &
						   "End Time = " & dtTankInventory.Rows(0)("InventoryTime").ToString().Replace(",", " ") & " ; " &
						   "Ending Totalizer/Meter Number = " & dtTankInventory.Rows(0)("Quantity").ToString().Replace(",", " ") & " "
				Else
					data = "TankInventoryId = " & TankInventoryId & " ; " &
						   "Company = " & dtTankInventory.Rows(0)("CompanyName").ToString().Replace(",", " ") & " ; " &
						   "Tank Number = " & dtTankInventory.Rows(0)("TankNumber").ToString().Replace(",", " ") & " ; " &
						   "Entry Type = RECEIPT/DELIVERY " & " ; " &
						   "Date Type = Start and End" & " ; " &
						   "Start Date = " & dtTankInventory.Rows(0)("InventoryDate").ToString().Replace(",", " ") & " ; " &
						   "Start Time = " & dtTankInventory.Rows(0)("InventoryTime").ToString().Replace(",", " ") & " ; " &
						   "End Date = " & dtTankInventory.Rows(0)("EndDateForRD").ToString().Replace(",", " ") & " ; " &
						   "End Time = " & dtTankInventory.Rows(0)("EndTimeForRD").ToString().Replace(",", " ") & " ; " &
						   "Receipt/Delivery Tank Level Quantity = " & dtTankInventory.Rows(0)("Quantity").ToString().Replace(",", " ") & " "
				End If
			End If



			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnTANKRECONICIATIONREPORT_Click(sender As Object, e As EventArgs)
		Response.Redirect("/Reports/TankReconciliation.aspx")
	End Sub

	Private Sub BindTanks(CustomerId As Integer)
		OBJMaster = New MasterBAL()
		Dim dtTanks As DataTable = New DataTable()

		If (CustomerId = 0) Then
			dtTanks = OBJMaster.GetTankbyConditions(" ", Session("PersonId").ToString(), Session("RoleId").ToString())
		Else
			dtTanks = OBJMaster.GetTankbyConditions(" And T.CustomerId =" & CustomerId, Session("PersonId").ToString(), Session("RoleId").ToString())
		End If


		dtTanks = DirectCast(dtTanks.DefaultView, DataView).ToTable(True, "TankNumberNameForView", "TankNumber")

		ViewState("dtTanks") = dtTanks
		ddl_TankNo.DataSource = dtTanks
		ddl_TankNo.DataTextField = "TankNumberNameForView"
		ddl_TankNo.DataValueField = "TankNumber"
		ddl_TankNo.DataBind()
		ddl_TankNo.Items.Insert(0, New ListItem("Select Tank Number", "0"))

	End Sub

    'Protected Sub gvTankInvt_RowDataBound(sender As Object, e As GridViewRowEventArgs)
    '    Try
    '        If e.Row.RowType = DataControlRowType.DataRow Then
    '            'Get the value of column from the DataKeys using the RowIndex.
    '            Dim CostingMethod As Integer = Convert.ToInt32(gvTankInvt.DataKeys(e.Row.RowIndex).Values(1))
    '            Dim Entry_From As String = gvTankInvt.DataKeys(e.Row.RowIndex).Values(2)
    '            Dim grid_row As GridViewRow
    '            grid_row = e.Row
    '            Dim btnR As LinkButton = grid_row.FindControl("linkEdit")

    '            If btnR IsNot Nothing Then
    '                If CostingMethod = 1 And (Entry_From = "Receipt/Delivery" Or Entry_From = "RD") Then
    '                    btnR.Visible = False
    '                Else
    '                    btnR.Visible = True
    '                End If
    '            End If

    '        End If
    '    Catch ex As Exception
    '        log.Error("Error occurred in gvTankInvt_RowDataBound Exception is :" + ex.Message)
    '        ErrorMessage.Visible = True
    '        ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
    '    End Try
    'End Sub
End Class