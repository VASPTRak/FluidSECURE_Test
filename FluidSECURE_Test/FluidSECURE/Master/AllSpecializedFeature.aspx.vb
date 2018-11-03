Imports log4net
Imports log4net.Config
Public Class AllSpecializedFeature
    Inherits System.Web.UI.Page


    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllSpecializedFeature))

    Dim OBJMaster As MasterBAL = New MasterBAL()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            message.Visible = False
            ErrorMessage.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") <> "SuperAdmin" Then
                'Access denied
                Response.Redirect("/home")

            Else
                If (Not IsPostBack) Then
                    Dim OBJMaster = New MasterBAL()
                    Dim dtCustomerMenuLinkMasterDetails As DataTable = New DataTable()
                    dtCustomerMenuLinkMasterDetails = OBJMaster.GetCustomerMenuLinkMasterDetails()
                    If dtCustomerMenuLinkMasterDetails IsNot Nothing And dtCustomerMenuLinkMasterDetails.Rows.Count > 0 Then
                        gvCustomerMenuLink.DataSource = dtCustomerMenuLinkMasterDetails
                        gvCustomerMenuLink.DataBind()
                    Else
                        gvCustomerMenuLink.DataSource = Nothing
                        gvCustomerMenuLink.DataBind()
                    End If
                End If
            End If

        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
        Try

            Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
                                GridViewRow)

            Dim CustomerMenuLinkId As Integer = gvCustomerMenuLink.DataKeys(gvRow.RowIndex).Values("CustomerMenuLinkId").ToString()

            Response.Redirect("SpecializedFeature.aspx?CustomerMenuLinkId=" & CustomerMenuLinkId, False)

        Catch ex As Exception
            log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub RebindData(sColimnName As String, sSortOrder As String)
        Try

            Dim dt As DataTable = CType(Session("dtCustomerMenuLinks"), DataTable)
            dt.DefaultView.Sort = sColimnName + " " + sSortOrder
            gvCustomerMenuLink.DataSource = dt
            gvCustomerMenuLink.DataBind()
            ViewState("Column_Name") = sColimnName
            ViewState("Sort_Order") = sSortOrder

        Catch ex As Exception
            log.Error("Error occurred in RebindData Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub gvCustomerMenuLinkSorting(sender As Object, e As GridViewSortEventArgs) Handles gvCustomerMenuLink.Sorting
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
            log.Error("Error occurred in gvCustomerMenuLinkSorting Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try

    End Sub

    Protected Sub gvCustomerMenuLink_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvCustomerMenuLink.PageIndexChanging
        Try
            gvCustomerMenuLink.PageIndex = e.NewPageIndex

            Dim dtCust As DataTable = Session("dtCustomerMenuLinks")

            gvCustomerMenuLink.DataSource = dtCust
            gvCustomerMenuLink.DataBind()
        Catch ex As Exception
            log.Error("Error occurred in gvCustomerMenuLink_PageIndexChanging Exception is :" + ex.Message)
        End Try

    End Sub

    <System.Web.Services.WebMethod(True)>
    Public Shared Function DeleteRecord(ByVal CustomerMenuLinkId As String) As String
        Try
            Dim OBJMaster = New MasterBAL()
            Dim data As String = CreateData(CustomerMenuLinkId)
            Dim result As Integer = OBJMaster.DeleteCustomerMenuLink(CustomerMenuLinkId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
            If (result > 0) Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Deleted", "Firmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Deleted", "Firmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Specialized Feature record deletion failed.")
                End If
            End If

            Return result
        Catch ex As Exception
            log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
            Return 0
        End Try
    End Function

    Private Shared Function CreateData(CustomerMenuLinkId As Integer) As String
        Try
            Dim dtCustomerMenuLink As DataTable = New DataTable()

            Dim OBJMaster As MasterBAL = New MasterBAL()
            dtCustomerMenuLink = OBJMaster.GetCustomerMenuLinkMasterByCondition(" and CustomerMenuLinkId = " & CustomerMenuLinkId)
            Dim mapping As String = ""

            Dim dtCustomerMenuLinkMapping As DataTable = New DataTable()
            dtCustomerMenuLinkMapping = OBJMaster.GetCustomerMenuLinkMappingByCustomerMenuLinkId(Convert.ToInt32(CustomerMenuLinkId))

            If dtCustomerMenuLinkMapping IsNot Nothing And dtCustomerMenuLinkMapping.Rows.Count > 0 Then
                For i = 0 To dtCustomerMenuLinkMapping.Rows.Count - 1
                    mapping = mapping & " Name = " + dtCustomerMenuLink.Rows(0)("Name") + " CustomerName = " + dtCustomerMenuLinkMapping.Rows(i)("CustomerName")
                Next
            End If

            Dim data As String = "CustomerMenuLinkId = " & CustomerMenuLinkId & " ; " &
                                    "Name = " & dtCustomerMenuLink.Rows(0)("Name").ToString().Replace(",", " ") & " ; " &
                                    "Customer and Menu Link Mapping  = " & mapping.TrimEnd(";") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class