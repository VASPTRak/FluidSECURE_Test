Imports log4net
Imports log4net.Config

Public Class AllOFFSiteTransactions
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllOFFSiteTransactions))

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
                        Session("TranDDL_WifiSSIdFLValue") = ""
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
                                DDL_WifiSSIdFL.Visible = False
                                DDL_Missed.Visible = False
                                TransDate.Visible = False
                            ElseIf (Session("TranDDL_ColumnName") = "HubId") Then

                                DDL_Customer.Visible = False
                                txt_value.Visible = False
                                DDL_WifiSSIdFL.Visible = False
                                DDL_Hub.Visible = True
                                DDL_Hub.SelectedValue = Session("TranDDL_HubValue")
                                DDL_Missed.Visible = False
                                TransDate.Visible = False
                            ElseIf (Session("TranDDL_ColumnName") = "WifiSSId") Then

                                DDL_Customer.Visible = False
                                txt_value.Visible = False
                                DDL_Hub.Visible = False
                                DDL_WifiSSIdFL.Visible = True
                                DDL_WifiSSIdFL.SelectedValue = Session("TranDDL_WifiSSIdFLValue")
                                DDL_Missed.Visible = False
                                TransDate.Visible = False
                            ElseIf (Session("TranDDL_ColumnName") = "TransactionStatus") Then

                                DDL_Customer.Visible = False
                                txt_value.Visible = False
                                DDL_Hub.Visible = False
                                DDL_WifiSSIdFL.Visible = False
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
                                DDL_WifiSSIdFL.Visible = False
                                TransDate.Focus()
                                TransDate.Visible = True
                                txtTransactionDateFrom.Text = Session("TranStartDateValue")
                                txtTransactionDateTo.Text = Session("TranEndDateValue")
                            Else
                                DDL_Customer.Visible = False
                                txt_value.Visible = True
                                DDL_Hub.Visible = False
                                DDL_WifiSSIdFL.Visible = False
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

                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

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

            strConditions = IIf(strConditions = "", " and Isnull(IsDeleted,0) = 0 and ISNULL(OFFSite,0) = 1 ", strConditions + " and Isnull(IsDeleted,0) = 0 and ISNULL(OFFSite,0) = 1 ")

            dtFluidSecureLink = OBJMaster.GetFluidSecureLinkForSearch(strConditions)
            DDL_WifiSSIdFL.DataSource = dtFluidSecureLink
            DDL_WifiSSIdFL.DataValueField = "WiFissid"
            DDL_WifiSSIdFL.DataTextField = "WiFissid"
            DDL_WifiSSIdFL.DataBind()

            DDL_WifiSSIdFL.Items.Insert(0, New ListItem("Select FluidSecure Link", "0"))

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

            strConditions = IIf(strConditions = "", " and ISNULL(T.OFFSite,0) = 1", strConditions + " and ISNULL(T.OFFSite,0) = 1")

            If (DDL_Missed.SelectedValue = "-1") Then
                'strConditions = IIf(strConditions = "", " and ISNULL(T.IsMissed,0) = 0", strConditions + " and ISNULL(T.IsMissed,0) = 0")
                'strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
            Else
                If (DDL_Missed.SelectedValue = "Completed") Then
                    strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
                ElseIf (DDL_Missed.SelectedValue = "IsMissed") Then
                    strConditions = (IIf(strConditions = "", " and (ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and (ISNULL(T.TransactionStatus,0) = 1 And ISNULL(T.IsMissed,0)= 1)  and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
                    'ElseIf (DDL_Missed.SelectedValue = "OnGoing") Then
                    '    'strConditions = (IIf(strConditions = "", " and ISNULL(T.IsMissed,0)= " + DDL_Missed.SelectedValue + "", strConditions + " and ISNULL(T.IsMissed,0) = " + DDL_Missed.SelectedValue + ""))
                    '    strConditions = (IIf(strConditions = "", " and ((ISNULL(T.TransactionStatus,0) = 1  or ISNULL(T.IsMissed,0)= 1) or ISNULL(T.TransactionStatus,0) = 0) and datediff(minute, T.[CreatedDate] ,getdate()) <= 15 ", strConditions + " and ((ISNULL(T.TransactionStatus,0) = 1  or ISNULL(T.IsMissed,0)= 1) or ISNULL(T.TransactionStatus,0) = 0) and datediff(minute, T.[CreatedDate] ,getdate()) <= 15"))
                    'ElseIf (DDL_Missed.SelectedValue = "NotStarted") Then
                    '    strConditions = (IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
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
                If DDL_WifiSSIdFL.SelectedValue.ToString() <> "0" Then
                    strConditions = IIf(strConditions = "", " and T." + DDL_ColumnName.SelectedValue + " = '" + DDL_WifiSSIdFL.SelectedValue + "'", strConditions + " and T." + DDL_ColumnName.SelectedValue + " = '" + DDL_WifiSSIdFL.SelectedValue + "'")
                End If
            End If

            Dim dtTransactions As DataTable = New DataTable()
            Session("TranConditions") = strConditions
            Dim dsT As DataSet = New DataSet()
            dsT = OBJMaster.GetTransactionsByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString(), False, 0, 0, False, "", "")
            dtTransactions = dsT.Tables(0)

            Session("dtTransactions") = dtTransactions

            gvTransactions.DataSource = dtTransactions
            gvTransactions.DataBind()
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtTransactions IsNot Nothing Then
                If dtTransactions.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtTransactions.Rows.Count)
                End If
            End If

            ViewState("Column_Name") = "TransactionId"
            ViewState("Sort_Order") = "DESC"

            Session("TranDDL_ColumnName") = DDL_ColumnName.SelectedValue
            Session("Trantxt_valueNameValue") = txt_value.Text
            Session("TranDDL_CustomerValue") = DDL_Customer.SelectedValue
            Session("TranDDL_HubValue") = DDL_Hub.SelectedValue
            Session("TranDDL_WifiSSIdFLValue") = DDL_WifiSSIdFL.SelectedValue
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

            Response.Redirect("OFFSiteTransaction?TransactionId=" & TransactionId, False)

        Catch ex As Exception

            log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub RebindData(sColimnName As String, sSortOrder As String)
        Try


            Dim dt As DataTable = CType(Session("dtTransactions"), DataTable)
            dt.DefaultView.Sort = sColimnName + " " + sSortOrder
            gvTransactions.DataSource = dt
            gvTransactions.DataBind()
            ViewState("Column_Name") = sColimnName
            ViewState("Sort_Order") = sSortOrder
        Catch ex As Exception

            log.Error("Error occurred in RebindData Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub gvTransactions_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvTransactions.Sorting
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

            log.Error("Error occurred in gvTransactions_Sorting Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub gvTransactions_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvTransactions.PageIndexChanging
        Try
            gvTransactions.PageIndex = e.NewPageIndex

            Dim dtTransactions As DataTable = Session("dtTransactions")

            gvTransactions.DataSource = dtTransactions
            gvTransactions.DataBind()


        Catch ex As Exception
            log.Error("Error occurred in gvTransactions_PageIndexChanging Exception is :" + ex.Message)
        End Try
    End Sub

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
                DDL_WifiSSIdFL.Visible = False
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
                DDL_WifiSSIdFL.Visible = False
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
                DDL_WifiSSIdFL.Visible = False
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
                DDL_WifiSSIdFL.Visible = False
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
                DDL_WifiSSIdFL.Visible = False
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

                'ElseIf (DDL_ColumnName.SelectedValue = "NotStarted") Then

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
                DDL_WifiSSIdFL.Visible = False
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
                DDL_WifiSSIdFL.Visible = True
                DDL_WifiSSIdFL.Focus()
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
                DDL_WifiSSIdFL.Visible = False
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
            Dim OBJMaster = New MasterBAL()

            Dim result As Integer = OBJMaster.DeleteTransaction(TransactionId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
            Return result
        Catch ex As Exception

            log.Error("Error occurred in DDL_ColumnName_SelectedIndexChanged Exception is :" + ex.Message)
            Return 0

        End Try
    End Function

    Protected Sub btn_New_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Master/OFFSiteTransaction")
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

    Private Sub BindTransactionStatus()
        Try

            DDL_Missed.Items.Insert(0, New ListItem("Select Transaction Status", "-1"))
            DDL_Missed.Items.Insert(1, New ListItem(ConfigurationManager.AppSettings("MissedText").ToString(), "IsMissed"))
            DDL_Missed.Items.Insert(2, New ListItem(ConfigurationManager.AppSettings("CompletedText").ToString(), "Completed"))

        Catch ex As Exception
            log.Error("Error occurred in BindTransactionStatus Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
End Class
