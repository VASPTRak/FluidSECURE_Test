Imports log4net
Imports log4net.Config
Public Class AllFSVMFirmwareUpgrades
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllFSVMFirmwareUpgrades))

    Dim OBJMaster As MasterBAL = New MasterBAL()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        XmlConfigurator.Configure()

        message.Visible = False
        ErrorMessage.Visible = False

        If Session("PersonId") Is Nothing And Session("UniqueId") Is Nothing And Session("RoleId") Is Nothing Then
            'unautorized access error log
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
        ElseIf Session("RoleName") <> "SuperAdmin" Then
            'Access denied
            Response.Redirect("/home")

        Else
            If (Not IsPostBack) Then
                BindColumns()
                If (Request.QueryString("Filter") = Nothing) Then
                    Session("FSVMFirmwareConditions") = ""
                    Session("FSVMFirmwareDDL_ColumnName") = ""
                    Session("FSVMFirmwaretxt_valueNameValue") = ""
                End If
                If (Not Session("FSVMFirmwareDDL_ColumnName") Is Nothing And Not Session("FSVMFirmwareDDL_ColumnName") = "") Then
                    DDL_Column.SelectedValue = Session("FSVMFirmwareDDL_ColumnName")
                    If (Not Session("FSVMFirmwareDDL_ColumnName") Is Nothing And Not Session("FSVMFirmwaretxt_valueNameValue") = "") Then
                        If (Session("FSVMFirmwaretxt_valueNameValue") <> "") Then
                            txt_value.Text = Session("FSVMFirmwaretxt_valueNameValue")
                        Else
                            txt_value.Text = ""
                        End If
                    End If
                End If
                If Session("RoleName") <> "SuperAdmin" Then
                    btn_New.Visible = False
                End If
                btnSearch_Click(Nothing, Nothing)

                DDL_Column.Focus()
            End If
        End If
    End Sub

    Private Sub BindColumns()
        Try

            OBJMaster = New MasterBAL()
            Dim dtColumns As DataTable = New DataTable()
            dtColumns = OBJMaster.GetFSVMFirmwareColumnNameForSearch()

            DDL_Column.DataSource = dtColumns
            DDL_Column.DataValueField = "ColumnName"
            DDL_Column.DataTextField = "ColumnEnglishName"
            DDL_Column.DataBind()
            DDL_Column.Items.Insert(0, New ListItem("Select Column", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindColumns Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting search column, please try again later."

        End Try

    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Try

            OBJMaster = New MasterBAL()

            Dim strConditions As String = ""
            If ((Not txt_value.Text = "") And DDL_Column.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text + "%'")
            End If

            OBJMaster = New MasterBAL()
            Dim dtFSVMFirmwares As DataTable = New DataTable()

            dtFSVMFirmwares = OBJMaster.GetFSVMFirmwaresByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            Session("FSVMFirmwareConditions") = strConditions
            Session("dtFSVMFirmwares") = dtFSVMFirmwares
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtFSVMFirmwares IsNot Nothing Then
                If dtFSVMFirmwares.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtFSVMFirmwares.Rows.Count)
                End If
            End If
            gvUploadedFSVMFirmware.DataSource = dtFSVMFirmwares
            gvUploadedFSVMFirmware.DataBind()

            Session("FSVMFirmwareDDL_ColumnName") = DDL_Column.SelectedValue
            Session("FSVMFirmwaretxt_valueNameValue") = txt_value.Text

            ViewState("Column_Name") = "FSVMFirmwareId"
            ViewState("Sort_Order") = "desc"

        Catch ex As Exception

            log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            DDL_Column.Focus()
        End Try
    End Sub

    Protected Sub linkEdit_Click(sender As Object, e As EventArgs)

        Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
                                GridViewRow)

        Dim FSVMFirmwareId As Integer = gvUploadedFSVMFirmware.DataKeys(gvRow.RowIndex).Values("FSVMFirmwareId").ToString()

        Response.Redirect("FSVMFirmwareUpgrades.aspx?FSVMFirmwareId=" & FSVMFirmwareId, False)

    End Sub

    Private Sub RebindData(sColimnName As String, sSortOrder As String)

        Dim dt As DataTable = CType(Session("dtFSVMFirmwares"), DataTable)
        dt.DefaultView.Sort = sColimnName + " " + sSortOrder
        gvUploadedFSVMFirmware.DataSource = dt
        gvUploadedFSVMFirmware.DataBind()
        ViewState("Column_Name") = sColimnName
        ViewState("Sort_Order") = sSortOrder

    End Sub

    Protected Sub gvUploadedFSVMFirmwareSorting(sender As Object, e As GridViewSortEventArgs) Handles gvUploadedFSVMFirmware.Sorting
        If e.SortExpression = ViewState("Column_Name").ToString() Then
            If ViewState("Sort_Order").ToString() = "ASC" Then
                RebindData(e.SortExpression, "DESC")
            Else
                RebindData(e.SortExpression, "ASC")
            End If

        Else
            RebindData(e.SortExpression, "ASC")
        End If
    End Sub

    Protected Sub gvUploadedFSVMFirmware_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvUploadedFSVMFirmware.PageIndexChanging
        Try
            gvUploadedFSVMFirmware.PageIndex = e.NewPageIndex

            Dim dtCust As DataTable = Session("dtFSVMFirmwares")

            gvUploadedFSVMFirmware.DataSource = dtCust
            gvUploadedFSVMFirmware.DataBind()
        Catch ex As Exception
            log.Error("Error occurred in gvUploadedFSVMFirmware_PageIndexChanging Exception is :" + ex.Message)
        End Try

    End Sub


    <System.Web.Services.WebMethod(True)>
    Public Shared Function DeleteRecord(ByVal FSVMFirmwareId As String) As String

        Dim OBJMaster = New MasterBAL()
        Dim data As String = CreateData(FSVMFirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")), HttpContext.Current.Session("RoleId"))
        Dim result As Integer = OBJMaster.DeleteFSVMFirmware(FSVMFirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
        If (result > 0) Then
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Deleted", "FSVMFirmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If
        Else
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Deleted", "FSVMFirmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "FSVMFirmware upgrade record deletion failed.")
            End If
        End If

        Return result

    End Function


    <System.Web.Services.WebMethod(True)>
    Public Shared Function LaunchFSVMFirmware(ByVal FSVMFirmwareId As String) As String

        Dim OBJMaster = New MasterBAL()
        Dim data As String = CreateData(FSVMFirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")), HttpContext.Current.Session("RoleId"))
        Dim result As Integer = OBJMaster.LaunchFSVMFirmware(FSVMFirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
        If (result > 0) Then
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Launched", "FSVMFirmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If
        Else
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Launched", "FSVMFirmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "FSVMFirmware Upgrade launched failed.")
            End If
        End If

        Return result

    End Function

    Protected Sub btn_New_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Master/FSVMFirmwareUpgrades")
    End Sub

    Protected Sub gvUploadedFSVMFirmware_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        'Checking the RowType of the Row  
        If (e.Row.RowType = DataControlRowType.DataRow) Then

            Dim IsLaunched As String = gvUploadedFSVMFirmware.DataKeys(e.Row.RowIndex).Values("IsLaunched")
            Dim IsUpgradable As String = gvUploadedFSVMFirmware.DataKeys(e.Row.RowIndex).Values("IsUpgradable")

            Dim lnkAutoUpgradeEnableDisable As LinkButton = TryCast(e.Row.FindControl("lnkAutoUpgradeEnableDisable"), LinkButton)
            If (IsLaunched = "Yes") Then
                e.Row.Cells(1).Visible = False
                e.Row.Cells(2).Visible = True
                'gvUploadedFSVMFirmware.Rows(e.Row.RowIndex).Cells(0).Visible = False
            Else
                e.Row.Cells(1).Visible = True
                e.Row.Cells(2).Visible = False
            End If

            If (IsUpgradable = "Yes") Then
                If IsLaunched = "No" Then
                    lnkAutoUpgradeEnableDisable.Enabled = False
                    e.Row.Cells(3).Enabled = False
                    e.Row.Cells(3).Style.Add("cursor", "not-allowed")
                Else
                    If lnkAutoUpgradeEnableDisable IsNot Nothing Then
                        lnkAutoUpgradeEnableDisable.Text = "ON"
                    End If
                End If
            Else
                If lnkAutoUpgradeEnableDisable IsNot Nothing Then
                    If IsLaunched = "No" Then
                        lnkAutoUpgradeEnableDisable.Enabled = False
                        e.Row.Cells(3).Enabled = False
                        e.Row.Cells(3).Style.Add("cursor", "not-allowed")
                    Else
                        lnkAutoUpgradeEnableDisable.Enabled = True
                    End If
                Else
                    lnkAutoUpgradeEnableDisable.Enabled = False
                    e.Row.Cells(3).Enabled = False
                    e.Row.Cells(3).Style.Add("cursor", "not-allowed")
                End If
            End If

        ElseIf (e.Row.RowType = DataControlRowType.Header) Then
            e.Row.Cells(2).Visible = False
        End If

    End Sub

    Protected Sub lnkAutoUpgradeEnableDisable_Click(sender As Object, e As EventArgs)
        Dim rowIndex As Integer = TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex
        Dim id As Integer = Convert.ToInt32(gvUploadedFSVMFirmware.DataKeys(rowIndex).Values(0))
        OBJMaster = New MasterBAL()
        OBJMaster.FSVMcheckLaunchedAndExistedVersionAndUpdate(id.ToString, "", 0, 3)
        btnSearch_Click(Nothing, Nothing)
    End Sub

    Private Shared Function CreateData(FSVMFirmwareId As Integer, PersonId As Integer, RoleId As String) As String
        Try
            Dim dtFSVMFirmware As DataTable = New DataTable()

            Dim OBJMaster As MasterBAL = New MasterBAL()
            dtFSVMFirmware = OBJMaster.GetFSVMFirmwaresByCondition(" and FSVMFirmwareId = " & FSVMFirmwareId, PersonId, RoleId)

            Dim dtTankInventory As DataSet = New DataSet()
            dtTankInventory = OBJMaster.GetFSVMFirmwareById(FSVMFirmwareId)
            Dim mapping As String = ""

            If dtTankInventory IsNot Nothing Then
                If dtTankInventory.Tables(1) IsNot Nothing Then
                    If dtTankInventory.Tables(1).Rows.Count > 0 Then
                        For index = 0 To dtTankInventory.Tables(1).Rows.Count - 1
                            mapping = mapping & " Site Link Name= " & dtTankInventory.Tables(1).Rows(0)("WifiSSId") & " Company Name = " & dtTankInventory.Tables(1).Rows(0)("CustomerName") & ";"
                        Next
                    End If
                End If
            End If

            Dim data As String = "FSVMFirmwareId = " & FSVMFirmwareId & " ; " &
                                    "FSVMFirmware File Name = " & dtFSVMFirmware.Rows(0)("FSVMFirmwareFileName").ToString().Replace(",", " ") & " ; " &
                                    "Version = " & dtFSVMFirmware.Rows(0)("Version").ToString() & " ; " &
                                    "FSVMFirmware and FuelSecure Link Mapping  = " & mapping.TrimEnd(";") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class