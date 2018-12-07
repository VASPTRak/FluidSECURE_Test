Imports log4net
Imports log4net.Config

Public Class AllShipments
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllShipments))

    Dim OBJMaster As MasterBAL = New MasterBAL()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then

                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession('Session Expired. Please Login Again.')", True)
            ElseIf Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "Support" Then ' And Session("RoleName") <> "CustomerAdmin"
                'Access denied
                Response.Redirect("/home")
                Return
            Else
                If (Not IsPostBack) Then
                    BindColumns()
                    If (Request.QueryString("Filter") = Nothing) Then
                        Session("ShipmentConditions") = ""
                        Session("ShipmentDDL_ColumnName") = ""
                        Session("Shipmenttxt_valueNameValue") = ""
                    End If
                    If (Not Session("ShipmentDDL_ColumnName") Is Nothing And Not Session("ShipmentDDL_ColumnName") = "") Then
                        DDL_ColumnName.SelectedValue = Session("ShipmentDDL_ColumnName")
                        If (Not Session("ShipmentDDL_ColumnName") Is Nothing And Not Session("Shipmenttxt_valueNameValue") = "") Then
                            If (Session("Shipmenttxt_valueNameValue") <> "") Then
                                txt_value.Text = Session("Shipmenttxt_valueNameValue")
                            Else
                                txt_value.Text = ""
                            End If
                        End If
                    End If
                    btnSearch_Click(Nothing, Nothing)
                    DDL_ColumnName.Focus()
                    If Session("RoleName") = "CustomerAdmin" Then
                        btn_New.Visible = False
                        DDL_ColumnName.Visible = False
                        txt_value.Visible = False
                        btnSearch.Visible = False

                    Else
                        btn_New.Visible = True
                    End If
                End If
            End If
        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Try

            OBJMaster = New MasterBAL()

            Dim strConditions As String = ""

            If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and SD." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and SD." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")

                'ElseIf ((DDL_Customer.SelectedValue <> "0") And DDL_ColumnName.SelectedValue <> "0") Then
                '    strConditions = IIf(strConditions = "", " and F." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and F." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
            End If

            Dim dtShipments As DataTable = New DataTable()

            'If Session("CompanyNameHeader") <> Nothing Then
            '    If Session("CompanyNameHeader") <> "" And Session("CompanyNameHeader") <> "Select All" Then
            '        dtShipments = OBJMaster.GetShipmentsByCondition(strConditions + " and SD.CompanyId = " + Session("CompanyNameHeader").ToString() + " ", Session("RoleId").ToString())
            '    Else
            '        dtShipments = OBJMaster.GetShipmentsByCondition(strConditions, Session("RoleId").ToString())
            '    End If
            'Else
            '    dtShipments = OBJMaster.GetShipmentsByCondition(strConditions, Session("RoleId").ToString())
            'End If


            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                dtShipments = OBJMaster.GetShipmentsByCondition(strConditions + " and SD.CompanyId = " + Session("CustomerId").ToString() + " ", Session("RoleId").ToString())
                Session("ShipmentConditions") = strConditions + " and SD.CompanyId = " + Session("CustomerId").ToString() + " "
            Else
                dtShipments = OBJMaster.GetShipmentsByCondition(strConditions, Session("RoleId").ToString())
                Session("ShipmentConditions") = strConditions
            End If

            Session("dtShipments") = dtShipments
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtShipments IsNot Nothing Then
                If dtShipments.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtShipments.Rows.Count)
                End If
            End If
            gvShipment.DataSource = dtShipments
            gvShipment.DataBind()


            Session("ShipmentDDL_ColumnName") = DDL_ColumnName.SelectedValue
            Session("Shipmenttxt_valueNameValue") = txt_value.Text

            ViewState("Column_Name") = "FluidSecureUnitName"
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

            Dim ShipmentId As Integer = gvShipment.DataKeys(gvRow.RowIndex).Values("ShipmentId").ToString()

            Response.Redirect("Shipment.aspx?ShipmentId=" & ShipmentId, False)

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
            dtColumns = OBJMaster.GetShipmentColumnNameForSearch()

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
            Dim dt As DataTable = CType(Session("dtShipments"), DataTable)
            dt.DefaultView.Sort = sColimnName + " " + sSortOrder
            gvShipment.DataSource = dt
            gvShipment.DataBind()
            ViewState("Column_Name") = sColimnName
            ViewState("Sort_Order") = sSortOrder

        Catch ex As Exception

            log.Error("Error occurred in RebindData Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub gvShipment_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvShipment.Sorting
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

            log.Error("Error occurred in gvShipment_Sorting Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub gvShipment_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvShipment.PageIndexChanging
        Try
            gvShipment.PageIndex = e.NewPageIndex
            'btnSearch_Click(Nothing, Nothing)

            Dim dtShipments As DataTable = Session("dtShipments")

            gvShipment.DataSource = dtShipments
            gvShipment.DataBind()

        Catch ex As Exception
            log.Error("Error occurred in gvShipment_PageIndexChanging Exception is :" + ex.Message)
        End Try

    End Sub

    <System.Web.Services.WebMethod(True)>
    Public Shared Function DeleteRecord(ByVal ShipmentId As String) As String
        Try


            Dim OBJMaster = New MasterBAL()
            Dim beforeData As String = ""

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                beforeData = CreateData(ShipmentId)
            End If

            Dim result As Integer = OBJMaster.DeleteShipment(ShipmentId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
            If (result > 0) Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Deleted", "Shipments", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Deleted", "Shipments", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Shipment deletion failed.")
                End If
            End If
            Return result
        Catch ex As Exception
            log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
            Return 0
        End Try
    End Function

    Protected Sub btn_New_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Master/Shipment")
    End Sub

    Protected Sub gvShipment_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If Session("RoleName") = "CustomerAdmin" Then
                gvShipment.Columns(0).Visible = False
                gvShipment.Columns(1).Visible = False
                gvShipment.Columns(2).Visible = False
            End If
        Catch ex As Exception
            log.Error("Error occurred in gvShipment_RowDataBound Exception is :" + ex.Message)
        End Try
    End Sub

    Private Shared Function CreateData(ShipmentId As Integer) As String
        Try

            Dim data As String = ""

            Dim dtShipment As DataTable = New DataTable()

            Dim OBJMaster As MasterBAL = New MasterBAL()
            dtShipment = OBJMaster.GetShipmentByShipmentID(ShipmentId)

            data = "ShipmentId = " & ShipmentId & " ; " &
                    "FluidSecure Link Name = " & dtShipment.Rows(0)("FluidSecureUnitName").Replace(",", " ") & " ; " &
                    "Company = " & dtShipment.Rows(0)("Company").Replace(",", " ") & " ; " &
                    "Address  = " & dtShipment.Rows(0)("Address").Replace(",", " ") & " ; " &
                    "Shipment Date  = " & dtShipment.Rows(0)("ShipmentDate") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class