Imports log4net
Imports log4net.Config

Public Class AllTotalizerMeterReconciliation
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllTotalizerMeterReconciliation))

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
					If (Not Request.QueryString("Type") = Nothing And (Request.QueryString("Type") = "TM")) Then
						hdnEntryType.Value = Request.QueryString("Type")
						BindColumns()
						BindCustomer()
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

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()

			dtColumns = OBJMaster.GetTankInventoryColumnNameForSearch("TM")

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

			Dim strConditions As String = " and (ENTRY_TYPE = 'TM') "

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and TankInventory.CompanyId=" & Session("CustomerId")
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0" And DDL_ColumnName.SelectedValue = "FluidLink") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " = " + txt_value.Text + " ", strConditions + " and " + DDL_ColumnName.SelectedValue + " = " + txt_value.Text + " ")
			ElseIf ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0" And DDL_ColumnName.SelectedValue = "CompanyId") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and " + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			ElseIf ((DDL_Datetype.SelectedValue <> "") And DDL_ColumnName.SelectedValue <> "0" And DDL_ColumnName.SelectedValue = "DateType") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " = '" + DDL_Datetype.SelectedValue + "'", strConditions + " and " + DDL_ColumnName.SelectedValue + " = '" + DDL_Datetype.SelectedValue + "'")
			ElseIf (DDL_ColumnName.SelectedValue = "InventoryDateTime") Then
				strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " between '" + Request.Form(txtDateFrom.UniqueID) + "' and '" + Convert.ToDateTime(Request.Form(txtDateTo.UniqueID)).AddDays(1).ToString("MM/dd/yyyy") + "' ", strConditions + " and " + DDL_ColumnName.SelectedValue + " between '" + Request.Form(txtDateFrom.UniqueID) + "' and '" + Request.Form(txtDateTo.UniqueID) + "'")
			End If

			OBJMaster = New MasterBAL()
			Dim dtTot As DataTable = New DataTable()

			dtTot = OBJMaster.GetTankInventorybyConditions(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtTot") = dtTot
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtTot IsNot Nothing Then
                If dtTot.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtTot.Rows.Count)
                End If
            End If
            gvTot.DataSource = dtTot
			gvTot.DataBind()

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

			Dim TankinventoryId As Integer = Convert.ToInt32(gvTot.DataKeys(gvRow.RowIndex).Values("TankInventoryId").ToString())

			Response.Redirect("TotalizerMeterReconciliation?TankinventoryId=" & TankinventoryId.ToString() + "&Type=" + hdnEntryType.Value, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try


			Dim dt As DataTable = CType(Session("dtTot"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvTot.DataSource = dt
			gvTot.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder
		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTot_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvTot.Sorting
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
			log.Error("Error occurred in gvTot_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvTot_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvTot.PageIndexChanging
		Try
			gvTot.PageIndex = e.NewPageIndex
			Dim dtTot As DataTable = Session("dtTot")

			gvTot.DataSource = dtTot
			gvTot.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvTot_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
		Try
			If (DDL_ColumnName.SelectedValue = "CompanyId") Then
				DDL_Customer.Visible = True
				txt_value.Visible = False
				DDL_Datetype.Visible = False
				TransDate.Visible = False
				OtherThanDate.Visible = True
				hiddenDiv.Visible = True
				DDL_Customer.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "InventoryDateTime") Then

				TransDate.Visible = True
				txtDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
				txtDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
				hiddenDiv.Visible = False
				OtherThanDate.Visible = False
				TransDate.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "FluidLink") Then
				DDL_Customer.Visible = False
				txt_value.Visible = True
				TransDate.Visible = False
				DDL_Datetype.Visible = False
				OtherThanDate.Visible = True
				hiddenDiv.Visible = True
				txt_value.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "DateType") Then
				DDL_Customer.Visible = False
				txt_value.Visible = False
				TransDate.Visible = False
				DDL_Datetype.Visible = True
				OtherThanDate.Visible = True
				hiddenDiv.Visible = True
				DDL_Datetype.Focus()
			Else
				DDL_Customer.Visible = False
				txt_value.Visible = True
				TransDate.Visible = False
				DDL_Datetype.Visible = False
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
					CSCommonHelper.WriteLog("Deleted", "AllTotalizerMeterReconciliation", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "AllTotalizerMeterReconciliation", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "AllTotalizerMeterReconciliation deletion failed.")
				End If
			End If

			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/TotalizerMeterReconciliation?TankinventoryId=" + "&Type=" + hdnEntryType.Value)
	End Sub

	Protected Sub btnTotalizerMeterReconciliationReport_Click(sender As Object, e As EventArgs)
		Response.Redirect("/Reports/TotalizerMeterReconciliationR.aspx", False)
	End Sub

	Private Shared Function CreateData(TankInventoryId As Integer) As String
		Try
			Dim OBJMaster As MasterBAL = New MasterBAL()

			Dim dtTankInventory As DataTable = OBJMaster.GetTankInventorybyId(TankInventoryId)

			Dim data As String = ""
			If dtTankInventory IsNot Nothing And dtTankInventory.Rows.Count > 0 Then
				If dtTankInventory.Rows(0)("DateType").ToString() = "s" Then
					data = "TankInventoryId = " & TankInventoryId & " ; " &
						   "FluidSecure Link Number = " & dtTankInventory.Rows(0)("FluidLink").ToString().Replace(",", " ") & " ; " &
						   "Company = " & dtTankInventory.Rows(0)("CompanyName").ToString().Replace(",", " ") & " ; " &
						   "Entry Type = Totalizer/Meter " & " ; " &
						   "Date Type = Start" & " ; " &
						   "Start Date = " & dtTankInventory.Rows(0)("InventoryDate").ToString().Replace(",", " ") & " ; " &
						   "Start Time = " & dtTankInventory.Rows(0)("InventoryTime").ToString().ToString().Replace(",", " ") & " ; " &
						   "Starting Totalizer/Meter Number = " & dtTankInventory.Rows(0)("Quantity").ToString().Replace(",", " ") & " "
				Else
					data = "TankInventoryId = " & TankInventoryId & " ; " &
						   "FluidSecure Link Number = " & dtTankInventory.Rows(0)("FluidLink").ToString().Replace(",", " ") & " ; " &
						   "Company = " & dtTankInventory.Rows(0)("CompanyName").ToString().Replace(",", " ") & " ; " &
						   "Entry Type = Totalizer/Meter " & " ; " &
						   "Date Type = End" & " ; " &
						   "End Date = " & dtTankInventory.Rows(0)("InventoryDate").ToString().Replace(",", " ") & " ; " &
						   "End Time = " & dtTankInventory.Rows(0)("InventoryTime").ToString().Replace(",", " ") & " ; " &
						   "Ending Totalizer/Meter Number = " & dtTankInventory.Rows(0)("Quantity").ToString().Replace(",", " ") & " "
				End If
			End If



			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class