Imports log4net
Imports log4net.Config

Public Class AllTransactions
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllTransactions))

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

					BindTransactionStatus()

					txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
					txtTransactionDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")

					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
					TransDate.Visible = False

					BindColumns()
					BindCustomer()
					BindAllFuelType()
					'BindAllPersonnels()
					BindAllHubs()
					BindAllFluidSecureLink()

					If (Request.QueryString("Filter") = Nothing) Then
						Session("TranConditions") = ""
						Session("TranDDL_ColumnName") = ""
						Session("Trantxt_valueNameValue") = ""
						Session("TranDDL_CustomerValue") = ""
						Session("TranDDL_HubValue") = ""
						Session("TranDDL_WifissidValue") = ""
						Session("TranDDL_StatusValue") = ""
						Session("TranStartDateValue") = ""
						Session("TranEndDateValue") = ""
					End If

					If (Not Session("TranConditions") Is Nothing And Not Session("TranConditions") = "") Then
						DDL_ColumnName.SelectedValue = Session("TranDDL_ColumnName")
						If (Not Session("TranDDL_CustomerValue") Is Nothing And Not Session("TranDDL_CustomerValue") = "") Then
							If (Session("TranDDL_ColumnName") = "CompanyId") Then
								DDL_Customer.SelectedValue = Session("TranDDL_CustomerValue")
								DDL_Customer.Visible = True
								txt_value.Visible = False
								DDL_Hub.Visible = False
								DDL_WifiSSId.Visible = False
								DDL_Missed.Visible = False
								TransDate.Visible = False
							ElseIf (Session("TranDDL_ColumnName") = "HubId") Then

								DDL_Customer.Visible = False
								txt_value.Visible = False
								DDL_WifiSSId.Visible = False
								DDL_Hub.Visible = True
								DDL_Hub.SelectedValue = Session("TranDDL_HubValue")
								DDL_Missed.Visible = False
								TransDate.Visible = False
							ElseIf (Session("TranDDL_ColumnName") = "WifiSSId") Then

								DDL_Customer.Visible = False
								txt_value.Visible = False
								DDL_Hub.Visible = False
								DDL_WifiSSId.Visible = True
								DDL_WifiSSId.SelectedValue = Session("TranDDL_WifissidValue")
								DDL_Missed.Visible = False
								TransDate.Visible = False
							ElseIf (Session("TranDDL_ColumnName") = "TransactionStatus") Then

								DDL_Customer.Visible = False
								txt_value.Visible = False
								DDL_Hub.Visible = False
								DDL_WifiSSId.Visible = False
								DDL_Missed.Visible = True
								DDL_Missed.SelectedValue = Session("TranDDL_StatusValue")
								TransDate.Visible = False
							ElseIf (Session("TranDDL_ColumnName") = "TransactionDateTime") Then
								txt_value.Visible = False
								DDL_Customer.Visible = False
								DDL_Fuel.Visible = False
								OtherThanTransDate.Visible = False
								OtherThanTransDate1.Visible = False
								OtherThanTransDate2.Visible = False
								DDL_Missed.Visible = False
								DDL_Hub.Visible = False
								DDL_WifiSSId.Visible = False
								TransDate.Focus()
								TransDate.Visible = True
								txtTransactionDateFrom.Text = Session("TranStartDateValue")
								txtTransactionDateTo.Text = Session("TranEndDateValue")
							Else
								DDL_Customer.Visible = False
								txt_value.Visible = True
								DDL_Hub.Visible = False
								DDL_WifiSSId.Visible = False
								DDL_Missed.Visible = False
								TransDate.Visible = False
								If (Not Session("Trantxt_valueNameValue") Is Nothing And Not Session("Trantxt_valueNameValue") = "") Then
									txt_value.Text = Session("Trantxt_valueNameValue")
								Else
									txt_value.Text = ""
								End If
							End If
						End If
					End If

					btnSearch_Click(Nothing, Nothing)
				Else
					txtTransactionDateFrom.Text = Request.Form(txtTransactionDateFrom.UniqueID)
					txtTransactionDateTo.Text = Request.Form(txtTransactionDateTo.UniqueID)

					'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

				End If
				DDL_ColumnName.Focus()
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
			dtColumns = OBJMaster.GetTransactionColumnNameForSearch()

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
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try

	End Sub

	Private Sub BindAllFuelType()
		Try

			Dim CompanyId As Integer = 0

			OBJMaster = New MasterBAL()
			Dim dtFuel As DataTable = New DataTable()

			dtFuel = OBJMaster.GetFuelDetails(CompanyId)

			DDL_Fuel.DataSource = dtFuel
			DDL_Fuel.DataValueField = "FuelTypeId"
			DDL_Fuel.DataTextField = "FuelType"
			DDL_Fuel.DataBind()

			DDL_Fuel.Items.Insert(0, New ListItem("Select Fuel", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllFuelType Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting fuel types, please try again later."

		End Try

	End Sub

	'Private Sub BindAllPersonnels()
	'    Try
	'        Dim dtPersonnel As DataTable = New DataTable()
	'        Dim strConditions As String = ""
	'        If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
	'            strConditions = " and T.CompanyId= " & Session("CustomerId") + " "
	'        End If

	'        dtPersonnel = OBJMaster.GetTransactionsByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), False, 0, 0, False)
	'        Dim distinctDT As DataTable = dtPersonnel.DefaultView.ToTable(True, "PersonId", "PersonName")

	'        'DDL_users.DataSource = distinctDT

	'        'DDL_users.DataValueField = "PersonId"
	'        'DDL_users.DataTextField = "PersonName"
	'        'DDL_users.DataBind()

	'        'DDL_users.Items.Insert(0, New ListItem("Select Personnel", "0"))

	'    Catch ex As Exception

	'        log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
	'        ErrorMessage.Visible = True
	'        ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

	'    End Try
	'End Sub

	Private Sub BindAllHubs()
		Try
			Dim dtPersonnel As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			Dim strConditions As String = " and ISNULL(ANU.IsFluidSecureHub,0)=1 "

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += "   and ANU.CustomerId =" & Session("CustomerId") + " "
			End If

			dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
			If dtPersonnel.Rows.Count > 0 Then

				Dim query =
							From order In dtPersonnel.AsEnumerable()
							Order By order.Field(Of String)("HubSiteName")
							Select order

				Dim ViewForSort As DataView = query.AsDataView()

				DDL_Hub.DataSource = ViewForSort
				DDL_Hub.DataValueField = "PersonId"
				DDL_Hub.DataTextField = "HubSiteName"
				DDL_Hub.DataBind()
			End If

			DDL_Hub.Items.Insert(0, New ListItem("Select Site", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllHubs Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting All Hubs, please try again later."

		End Try
	End Sub

	Private Sub BindAllFluidSecureLink()
		Try
			Dim dtFluidSecureLink As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += "   and Companyid =" & Session("CustomerId") & " "
			End If

			strConditions = IIf(strConditions = "", " and Isnull(IsDeleted,0) = 0 and ISNULL(OFFSite,0) = 0 ", strConditions + " and Isnull(IsDeleted,0) = 0 and ISNULL(OFFSite,0) = 0 ")

			dtFluidSecureLink = OBJMaster.GetFluidSecureLinkForSearch(strConditions)
			DDL_WifiSSId.DataSource = dtFluidSecureLink
			DDL_WifiSSId.DataValueField = "WiFissid"
			DDL_WifiSSId.DataTextField = "WiFissid"
			DDL_WifiSSId.DataBind()

			DDL_WifiSSId.Items.Insert(0, New ListItem("Select FluidSecure Link", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllFluidSecureLink Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting All FluidSecureLink, please try again later."

		End Try
	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			OBJMaster = New MasterBAL()

			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions = " and T.CompanyId=" & Session("CustomerId")
			End If

			strConditions = IIf(strConditions = "", " and ISNULL(T.OFFSite,0) = 0", strConditions + " and ISNULL(T.OFFSite,0) = 0")

			If (DDL_Missed.SelectedValue = "-1") Then
				'strConditions = IIf(strConditions = "", " and ISNULL(T.IsMissed,0) = 0", strConditions + " and ISNULL(T.IsMissed,0) = 0")
				'strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
			Else
				If (DDL_Missed.SelectedValue = "Completed") Then
					strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
				ElseIf (DDL_Missed.SelectedValue = "OnGoing") Then
					'strConditions = (IIf(strConditions = "", " and ISNULL(T.IsMissed,0)= " + DDL_Missed.SelectedValue + "", strConditions + " and ISNULL(T.IsMissed,0) = " + DDL_Missed.SelectedValue + ""))
					strConditions = (IIf(strConditions = "", " and ((ISNULL(T.TransactionStatus,0) = 1  or ISNULL(T.IsMissed,0)= 1) or ISNULL(T.TransactionStatus,0) = 0) and datediff(minute, T.[CreatedDate] ,getdate()) <= 15 ", strConditions + " and ((ISNULL(T.TransactionStatus,0) = 1  or ISNULL(T.IsMissed,0)= 1) or ISNULL(T.TransactionStatus,0) = 0) and datediff(minute, T.[CreatedDate] ,getdate()) <= 15"))
				ElseIf (DDL_Missed.SelectedValue = "IsMissed") Then
					strConditions = (IIf(strConditions = "", " and (ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and (ISNULL(T.TransactionStatus,0) = 1 And ISNULL(T.IsMissed,0)= 1)  and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
				ElseIf (DDL_Missed.SelectedValue = "UserStopped") Then
					strConditions = (IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
				End If
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue = "VehicleName") Then
				strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and T." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
			ElseIf ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and T." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
			ElseIf ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			ElseIf ((DDL_Fuel.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_Fuel.SelectedValue + "", strConditions + " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_Fuel.SelectedValue + "")
				'ElseIf ((DDL_users.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
				'    strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_users.SelectedValue + "", strConditions + " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_users.SelectedValue + "")
			ElseIf (DDL_ColumnName.SelectedValue = "TransactionDateTime") Then
				If txtTransactionDateFrom.Text <> "" Then
					strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " between '" + txtTransactionDateFrom.Text + " 00:00:00.000 " + "' and '" + txtTransactionDateTo.Text + " 23:59:59.000" + "' ", strConditions + " and T." + DDL_ColumnName.SelectedValue + " between '" + txtTransactionDateFrom.Text + " 00:00:00.000 " + "' and '" + txtTransactionDateTo.Text + " 23:59:59.000" + "'")
					Session("TranStartDateValue") = txtTransactionDateFrom.Text
					Session("TranEndDateValue") = txtTransactionDateTo.Text
				Else
					strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " between '" + Request.Form(txtTransactionDateFrom.UniqueID) + " 00:00:00.000 " + "' and '" + Request.Form(txtTransactionDateTo.UniqueID) + " 23:59:59.000" + "' ", strConditions + " and T." + DDL_ColumnName.SelectedValue + " between '" + Request.Form(txtTransactionDateFrom.UniqueID) + " 00:00:00.000 " + "' and '" + Request.Form(txtTransactionDateTo.UniqueID) + " 23:59:59.000" + "'")
					Session("TranStartDateValue") = Request.Form(txtTransactionDateFrom.UniqueID)
					Session("TranEndDateValue") = Request.Form(txtTransactionDateTo.UniqueID)
				End If
			ElseIf DDL_ColumnName.SelectedValue = "HubId" Then
				strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_Hub.SelectedValue + "", strConditions + " and T." + DDL_ColumnName.SelectedValue + " = " + DDL_Hub.SelectedValue + "")
			ElseIf DDL_ColumnName.SelectedValue = "WifiSSId" Then
				If DDL_WifiSSId.SelectedValue.ToString() <> "0" Then
					strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " = '" + DDL_WifiSSId.SelectedValue + "'", strConditions + " and T." + DDL_ColumnName.SelectedValue + " = '" + DDL_WifiSSId.SelectedValue + "'")
				End If
			End If

			Dim dtTransactions As DataTable = New DataTable()
			Session("TranConditions") = strConditions
			dtTransactions = Me.GetTransactionsPageWise(1, strConditions, "", "", 1)
			Session("dtTransactions") = dtTransactions

			gvTransactions.DataSource = dtTransactions
			gvTransactions.DataBind()

			'ViewState("Column_Name") = "TransactionId"
			'ViewState("Sort_Order") = "DESC"

			Session("TranDDL_ColumnName") = DDL_ColumnName.SelectedValue
			Session("Trantxt_valueNameValue") = txt_value.Text
			Session("TranDDL_CustomerValue") = DDL_Customer.SelectedValue
			Session("TranDDL_HubValue") = DDL_Hub.SelectedValue
			Session("TranDDL_WifissidValue") = DDL_WifiSSId.SelectedValue
			Session("TranDDL_StatusValue") = DDL_Missed.SelectedValue


			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

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

			Dim TransactionId As Integer = gvTransactions.DataKeys(gvRow.RowIndex).Values("TransactionId").ToString()

			Response.Redirect("Transaction?TransactionId=" & TransactionId, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	'Private Sub RebindData(sColimnName As String, sSortOrder As String)
	'    Try
	'        Dim dt As DataTable = CType(Session("dtTransactions"), DataTable)
	'        dt.DefaultView.Sort = sColimnName + " " + sSortOrder
	'        gvTransactions.DataSource = dt
	'        gvTransactions.DataBind()
	'        ViewState("Column_Name") = sColimnName
	'        ViewState("Sort_Order") = sSortOrder
	'    Catch ex As Exception
	'        log.Error("Error occurred in RebindData Exception is :" + ex.Message)
	'        ErrorMessage.Visible = True
	'        ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
	'    End Try
	'End Sub



	'Protected Sub gvTransactions_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvTransactions.PageIndexChanging
	'    Try
	'        gvTransactions.PageIndex = e.NewPageIndex

	'        Dim dtTransactions As DataTable = Session("dtTransactions")

	'        gvTransactions.DataSource = dtTransactions
	'        gvTransactions.DataBind()


	'    Catch ex As Exception
	'        log.Error("Error occurred in gvTransactions_PageIndexChanging Exception is :" + ex.Message)
	'    End Try
	'End Sub

	Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
		Try
			If (DDL_ColumnName.SelectedValue = "CompanyId") Then

				txt_value.Visible = False
				DDL_Customer.Visible = True
				DDL_Fuel.Visible = False
				'DDL_users.Visible = False
				OtherThanTransDate.Visible = True
				OtherThanTransDate1.Visible = True
				OtherThanTransDate2.Visible = True
				TransDate.Visible = False
				DDL_Missed.Visible = False
				DDL_Hub.Visible = False
				DDL_WifiSSId.Visible = False
				DDL_Customer.Focus()

			ElseIf (DDL_ColumnName.SelectedValue = "FuelTypeID") Then

				txt_value.Visible = False
				DDL_Customer.Visible = False
				DDL_Fuel.Visible = True
				'DDL_users.Visible = False
				OtherThanTransDate.Visible = True
				OtherThanTransDate1.Visible = True
				OtherThanTransDate2.Visible = True
				TransDate.Visible = False
				DDL_Missed.Visible = False
				DDL_Hub.Visible = False
				DDL_WifiSSId.Visible = False
				DDL_Fuel.Focus()

			ElseIf (DDL_ColumnName.SelectedValue = "PersonName") Then

				txt_value.Visible = True
				DDL_Customer.Visible = False
				DDL_Fuel.Visible = False
				'DDL_users.Visible = False
				TransDate.Visible = False
				OtherThanTransDate.Visible = True
				OtherThanTransDate1.Visible = True
				OtherThanTransDate2.Visible = True
				DDL_Missed.Visible = False
				DDL_Hub.Visible = False
				DDL_WifiSSId.Visible = False
				txt_value.Focus()

			ElseIf (DDL_ColumnName.SelectedValue = "TransactionDateTime") Then

				TransDate.Visible = True
				txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
				txtTransactionDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

				txt_value.Visible = False
				DDL_Customer.Visible = False
				DDL_Fuel.Visible = False
				OtherThanTransDate.Visible = False
				OtherThanTransDate1.Visible = False
				OtherThanTransDate2.Visible = False
				'DDL_users.Visible = False
				DDL_Missed.Visible = False
				DDL_Hub.Visible = False
				DDL_WifiSSId.Visible = False
				TransDate.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "TransactionStatus") Then

				txt_value.Visible = False
				DDL_Customer.Visible = False
				DDL_Fuel.Visible = False
				'DDL_users.Visible = False
				OtherThanTransDate.Visible = True
				OtherThanTransDate1.Visible = True
				OtherThanTransDate2.Visible = True
				TransDate.Visible = False
				'DDL_users.Focus()
				DDL_Missed.Visible = True
				DDL_Hub.Visible = False
				DDL_WifiSSId.Visible = False
				DDL_Missed.Focus()
				'ElseIf (DDL_ColumnName.SelectedValue = "OnGoing") Then

				'    txt_value.Visible = False
				'    DDL_Customer.Visible = False
				'    DDL_Fuel.Visible = False
				'    'DDL_users.Visible = False
				'    OtherThanTransDate.Visible = True
				'    OtherThanTransDate1.Visible = True
				'    OtherThanTransDate2.Visible = True
				'    TransDate.Visible = False
				'    'DDL_users.Focus()
				'    DDL_Missed.Visible = True
				'    DDL_Missed.Focus()

				'ElseIf (DDL_ColumnName.SelectedValue = "UserStopped") Then

				'    txt_value.Visible = False
				'    DDL_Customer.Visible = False
				'    DDL_Fuel.Visible = False
				'    'DDL_users.Visible = False
				'    OtherThanTransDate.Visible = True
				'    OtherThanTransDate1.Visible = True
				'    OtherThanTransDate2.Visible = True
				'    TransDate.Visible = False
				'    'DDL_users.Focus()
				'    DDL_Missed.Visible = True
				'    DDL_Missed.Focus()


				'ElseIf (DDL_ColumnName.SelectedValue = "IsMissed") Then

				'    txt_value.Visible = False
				'    DDL_Customer.Visible = False
				'    DDL_Fuel.Visible = False
				'    'DDL_users.Visible = False
				'    OtherThanTransDate.Visible = True
				'    OtherThanTransDate1.Visible = True
				'    OtherThanTransDate2.Visible = True
				'    TransDate.Visible = False
				'    'DDL_users.Focus()
				'    DDL_Missed.Visible = True
				'    DDL_Missed.Focus()
			ElseIf DDL_ColumnName.SelectedValue = "HubId" Then
				txt_value.Visible = False
				DDL_Customer.Visible = False
				DDL_Fuel.Visible = False
				'DDL_users.Visible = False
				OtherThanTransDate.Visible = True
				OtherThanTransDate1.Visible = True
				OtherThanTransDate2.Visible = True
				TransDate.Visible = False
				DDL_Missed.Visible = False
				DDL_WifiSSId.Visible = False
				DDL_Hub.Visible = True
				DDL_Hub.Focus()
			ElseIf DDL_ColumnName.SelectedValue = "WifiSSId" Then
				txt_value.Visible = False
				DDL_Customer.Visible = False
				DDL_Fuel.Visible = False
				'DDL_users.Visible = False
				OtherThanTransDate.Visible = True
				OtherThanTransDate1.Visible = True
				OtherThanTransDate2.Visible = True
				TransDate.Visible = False
				DDL_Missed.Visible = False
				DDL_Hub.Visible = False
				DDL_WifiSSId.Visible = True
				DDL_WifiSSId.Focus()
			Else

				txt_value.Visible = True
				DDL_Customer.Visible = False
				DDL_Fuel.Visible = False
				'DDL_users.Visible = False
				TransDate.Visible = False
				OtherThanTransDate.Visible = True
				OtherThanTransDate1.Visible = True
				OtherThanTransDate2.Visible = True
				DDL_Missed.Visible = False
				DDL_Hub.Visible = False
				DDL_WifiSSId.Visible = False
				txt_value.Focus()

			End If
			txt_value.Text = ""
			DDL_Customer.SelectedValue = 0
			DDL_Fuel.SelectedValue = 0
			'DDL_users.SelectedValue = 0
			DDL_Missed.SelectedValue = -1
		Catch ex As Exception
			log.Error("Error occurred in DDL_ColumnName_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal TransactionId As String) As String
		Try
			Dim beforeData As String = ""
			Dim OBJMaster = New MasterBAL()
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(TransactionId)
			End If

			Dim result As Integer = OBJMaster.DeleteTransaction(TransactionId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))

			If (result <> 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Transactions", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Transactions", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Department deletion failed.")
				End If
			End If

			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/Transaction")
	End Sub

	Protected Sub gvTransactions_RowDataBound(sender As Object, e As GridViewRowEventArgs)
		Try


			If (e.Row.RowType = DataControlRowType.DataRow) Then
				Dim TransactionStatus As String = gvTransactions.DataKeys(e.Row.RowIndex).Values("TransactionStatusText").ToString()

				Dim linkEdit As LinkButton = DirectCast(e.Row.FindControl("linkEdit"), LinkButton)
				If linkEdit IsNot Nothing Then
					If TransactionStatus <> ConfigurationManager.AppSettings("CompletedText").ToString() And TransactionStatus <> ConfigurationManager.AppSettings("MissedText").ToString() Then
						linkEdit.Visible = False
					Else
						linkEdit.Visible = True
					End If
				End If

				Try
					Dim btnFSVM As Button = DirectCast(e.Row.FindControl("btnFSVM"), Button)
					Dim TransactionId As String = gvTransactions.DataKeys(e.Row.RowIndex).Values("TransactionId").ToString()
					Dim OBJMaster = New MasterBAL()
					Dim dsVMData As DataSet
					dsVMData = OBJMaster.GetVehicleRecurringMSGDetailsByTrnsactionId(TransactionId)
					If dsVMData.Tables(1) Is Nothing Then
						btnFSVM.Visible = False
					Else
						If dsVMData.Tables(1).Rows.Count > 0 Then
							btnFSVM.Visible = True
						Else
							btnFSVM.Visible = False
						End If
					End If
				Catch ex As Exception

				End Try

			End If


			'If (DDL_ColumnName.SelectedValue = "TransactionStatus" And DDL_Missed.SelectedValue = 1) Then
			'    gvTransactions.Columns(0).Visible = False
			'    'gvTransactions.Columns(1).Visible = False
			'Else
			'    gvTransactions.Columns(0).Visible = True
			'    gvTransactions.Columns(1).Visible = True
			'End If
		Catch ex As Exception
			log.Error("Error occurred in gvShipment_RowDataBound Exception is :" + ex.Message)
		End Try
	End Sub

	Private Shared Function CreateData(TransactionId As Integer) As String
		Try
			Dim dtTransaction As DataTable = New DataTable()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtTransaction = OBJMaster.GetTransactionById(TransactionId, False)


			Dim data As String = "TransactionId = " & TransactionId & " ; " &
									"Vehicle Number = " & dtTransaction.Rows(0)("VehicleNumber").ToString().Replace(",", " ") & " ; " &
									"Vehicle Name = " & dtTransaction.Rows(0)("VehicleName").ToString().Replace(",", " ") & " ; " &
									"Department = " & dtTransaction.Rows(0)("DeptName").ToString().Replace(",", " ") & " ; " &
									"Department Number = " & dtTransaction.Rows(0)("DepartmentNumber").ToString().Replace(",", " ") & " ; " &
									"FluidSecure Link = " & dtTransaction.Rows(0)("WifiSSId").ToString().Replace(",", " ") & " ; " &
									"Fuel Quantity = " & dtTransaction.Rows(0)("FuelQuantity").ToString().Replace(",", " ") & " ; " &
									"Other = " & dtTransaction.Rows(0)("Other").ToString().Replace(",", " ") & " ; " &
									"Company = " & dtTransaction.Rows(0)("Company").ToString().Replace(",", " ") & " ; " &
									"Cost = " & dtTransaction.Rows(0)("TransactionCost").ToString().Trim().Replace(",", " ") & " ; " &
									"Transaction Date = " & dtTransaction.Rows(0)("Date").ToString().Replace(",", " ") & " ; " &
									"Transaction Time = " & dtTransaction.Rows(0)("Time").ToString().Replace(",", " ") & " ; " &
									"Person = " & dtTransaction.Rows(0)("PersonName").ToString().Replace(",", " ") & " ; " &
									"Transaction Status = " & dtTransaction.Rows(0)("TranStatus").ToString().Replace(",", " ")
			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnFSVM_Click(sender As Object, e As EventArgs)
		Try
			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent, GridViewRow)
			Dim TransactionId As Integer = gvTransactions.DataKeys(gvRow.RowIndex).Values("TransactionId").ToString()
			Dim OBJMaster = New MasterBAL()
			Dim dsVMData As DataSet
			dsVMData = OBJMaster.GetVehicleRecurringMSGDetailsByTrnsactionId(TransactionId)

			If dsVMData.Tables(1) IsNot Nothing Then
				If dsVMData.Tables(1).Rows.Count > 0 Then
					lblMIL.Text = Convert.ToString(dsVMData.Tables(0).Rows(0)("MIL"))
					lblSPD.Text = Convert.ToString(dsVMData.Tables(0).Rows(0)("SPD"))
					lblRPM.Text = Convert.ToString(dsVMData.Tables(0).Rows(0)("RPM"))
					lblPC.Text = Convert.ToString(dsVMData.Tables(0).Rows(0)("PC"))
					lblODOK.Text = Convert.ToString(dsVMData.Tables(0).Rows(0)("ODOK"))
				Else
					lblMIL.Text = ""
					lblSPD.Text = ""
					lblRPM.Text = ""
					lblPC.Text = ""
					lblODOK.Text = ""
				End If
			Else
				lblMIL.Text = ""
				lblSPD.Text = ""
				lblRPM.Text = ""
				lblPC.Text = ""
				lblODOK.Text = ""
			End If

			If dsVMData.Tables(1) IsNot Nothing Then
				If dsVMData.Tables(1).Rows.Count > 0 Then
					grdVehicleRecurringMSG.DataSource = dsVMData.Tables(1)
					grdVehicleRecurringMSG.DataBind()
				Else
					grdVehicleRecurringMSG.DataSource = Nothing
					grdVehicleRecurringMSG.DataBind()
				End If
			Else
				grdVehicleRecurringMSG.DataSource = Nothing
				grdVehicleRecurringMSG.DataBind()
			End If

			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "openVehicleRecurringMSGModal();", True)
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while btnFSVM_Click. Error is {0}.", ex.Message))
		End Try
	End Sub

	Private Sub BindTransactionStatus()
		Try
			DDL_Missed.Items.Insert(0, New ListItem("Select Transaction Status", "-1"))
			DDL_Missed.Items.Insert(1, New ListItem(ConfigurationManager.AppSettings("OngoingText").ToString(), "OnGoing"))
			DDL_Missed.Items.Insert(2, New ListItem(ConfigurationManager.AppSettings("NotStartedText").ToString(), "UserStopped"))
			DDL_Missed.Items.Insert(3, New ListItem(ConfigurationManager.AppSettings("MissedText").ToString(), "IsMissed"))
			DDL_Missed.Items.Insert(4, New ListItem(ConfigurationManager.AppSettings("CompletedText").ToString(), "Completed"))
		Catch ex As Exception
			log.Error("Error occurred in BindTransactionStatus Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub
	Private Sub PopulatePager(ByVal recordCount As Integer, ByVal currentPage As Integer)
		Dim dblPageCount As Double = CType((CType(recordCount, Decimal) / Decimal.Parse(20)), Double)
		Dim pageCount As Integer = CType(Math.Ceiling(dblPageCount), Integer)
		Dim pages As New List(Of ListItem)

		If (pageCount > 0) Then

			Dim showMax As Integer = 10
			Dim startPage As Integer = currentPage - 10
			Dim endPage As Integer = currentPage + 9

			If (currentPage <> 1) Then
				pages.Add(New ListItem("First", "1", (currentPage > 1)))
			End If

			If (startPage <= 0) Then
				endPage = 10
				startPage = 1
			ElseIf (currentPage > startPage) Then
				startPage = currentPage
				endPage = startPage + 9
				pages.Add(New ListItem("...", startPage - 10, True))
			End If

			If (endPage > pageCount) Then
				endPage = pageCount
				startPage = endPage - showMax - 1
			End If


			For i = startPage To endPage
				If (Convert.ToInt16(i) > 0 And Convert.ToInt16(endPage) > 1) Then
					pages.Add(New ListItem(i.ToString, i.ToString, (i <> currentPage)))
				End If
			Next

			'If (endPage <= pageCount) Then
			'    pages.Add(New ListItem("...", endPage + 1, True))
			'End If
			If (currentPage <> pageCount) Then
				If (pageCount > 10) Then
					If (currentPage + 10 <= pageCount) Then
						pages.Add(New ListItem("...", endPage + 1, True))
					End If
				End If
				pages.Add(New ListItem("Last", pageCount.ToString, (currentPage < pageCount)))
			End If

		End If
		rptPager.DataSource = pages
		rptPager.DataBind()

	End Sub
	Protected Sub Page_Changed(ByVal sender As Object, ByVal e As EventArgs)
		Dim pageIndex As Integer = Integer.Parse(CType(sender, LinkButton).CommandArgument)

		Dim dtTransactions As DataTable = New DataTable()
		Dim strConditions As String = ""
		If (Not Session("TranConditions") Is Nothing) Then
			strConditions = Session("TranConditions")
		End If

		dtTransactions = Me.GetTransactionsPageWise(pageIndex, strConditions, gvTransactions.Attributes("CustomSortFields"),
												   gvTransactions.Attributes("CustomSortDirection"), 0)

		'Session("dtTransactions") = dtTransactions

		gvTransactions.DataSource = dtTransactions
		gvTransactions.DataBind()
	End Sub

	Private Function GetTransactionsPageWise(ByVal pageIndex As Integer, strConditions As String, CustomSortFields As String, CustomSortDirection As String,
											 firstLoad As Boolean) As DataTable
		'https://www.youtube.com/watch?v=roW6nUG7jx0

		OBJMaster = New MasterBAL()
		Dim dtTransactions As DataTable = New DataTable()
		Dim dsT As New DataSet()
		If (firstLoad = True Or CustomSortFields = "TransactionId") Then
			dsT = OBJMaster.GetTransactionsByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString(), False,
													   pageIndex, 20, 1, "", "")

		Else
			dsT = OBJMaster.GetTransactionsByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString(), False,
													   pageIndex, 20, 1, gvTransactions.Attributes("CustomSortFields"),
													   gvTransactions.Attributes("CustomSortDirection"))
		End If
		Dim recordCount As Integer = dsT.Tables(1).Rows(0)("TotalCount")
		Me.PopulatePager(recordCount, pageIndex)

		lblTotalNumberOfRecords.Text = "Total Records: 0"
		If dsT.Tables(1) IsNot Nothing Then
			If dsT.Tables(1).Rows.Count > 0 Then
				lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dsT.Tables(1).Rows(0)("TotalCount"))
			End If
		End If

		Return dsT.Tables(0)
	End Function

	Private Sub SortGridView(gv As GridView, e As GridViewSortEventArgs, ByRef sortdirection As SortDirection, ByRef sortfield As String)
		sortfield = e.SortExpression
		sortdirection = e.SortDirection
		If (gv.Attributes("CustomSortFields") IsNot Nothing And gv.Attributes("CustomSortDirection") IsNot Nothing) Then
			If (sortfield = gv.Attributes("CustomSortFields")) Then
				If (gv.Attributes("CustomSortDirection") = "ASC") Then
					sortdirection = SortDirection.Descending
				Else
					sortdirection = SortDirection.Ascending
				End If
			End If
		End If

		gv.Attributes("CustomSortFields") = sortfield
		gv.Attributes("CustomSortDirection") = IIf(sortdirection = SortDirection.Ascending, "ASC", "DESC")

	End Sub

	Protected Sub gvTransactions_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvTransactions.Sorting
		Try

			Dim sortDirection As SortDirection = SortDirection.Ascending
			Dim sortField As String = String.Empty

			SortGridView(gvTransactions, e, sortDirection, sortField)
			Dim strDirection = IIf(sortDirection = SortDirection.Ascending, "ASC", "DESC")

			Dim dtTransactions As DataTable = New DataTable()
			Dim strConditions As String = ""
			If (Not Session("TranConditions") Is Nothing) Then
				strConditions = Session("TranConditions")
			End If

			dtTransactions = Me.GetTransactionsPageWise(gvTransactions.PageIndex + 1, strConditions, e.SortExpression, e.SortDirection, 0)

			gvTransactions.DataSource = dtTransactions
			gvTransactions.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvTransactions_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

End Class