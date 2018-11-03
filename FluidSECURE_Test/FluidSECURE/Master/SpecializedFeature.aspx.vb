Imports System.IO
Imports log4net.Config
Imports log4net
Public Class SpecializedFeature
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(SpecializedFeature))
    Dim OBJMaster As MasterBAL = New MasterBAL()
    Shared beforeData As String
    Shared beforeLinks As String
    Shared afterLinks As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then

                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") <> "SuperAdmin" Then
                'Access denied
                Response.Redirect("/home")

            Else

                If Not IsPostBack Then
                    If (Not Request.QueryString("CustomerMenuLinkId") = Nothing And Not Request.QueryString("CustomerMenuLinkId") = "") Then

                        hdfCustomerMenuLinkId.Value = Request.QueryString("CustomerMenuLinkId")
                        lblHeader.Text = "Edit Specialized Feature"
                        Dim dtSpecializedFeature As DataTable = New DataTable()
                        OBJMaster = New MasterBAL()
                        dtSpecializedFeature = OBJMaster.GetCustomerMenuLinkMasterByCondition(" and CustomerMenuLinkId = " + hdfCustomerMenuLinkId.Value)
                        If dtSpecializedFeature IsNot Nothing And dtSpecializedFeature.Rows.Count > 0 Then
                            lblSpecializedFeature.Text = dtSpecializedFeature.Rows(0)("Name").ToString()
                        End If
                        BindMenuLink(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
                        BindCheckBoxCompanyValues()
                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"
                        End If
                    Else
                        lblSpecializedFeature.Focus()
                        lblHeader.Text = "Add Specialized Feature"
                        BindMenuLink(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
                    End If

                End If
            End If


        Catch ex As Exception

            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)

        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If
            Dim CustomerMenuLinkId As Integer = 0
            If hdfCustomerMenuLinkId.Value <> "" And hdfCustomerMenuLinkId.Value <> "0" Then
                CustomerMenuLinkId = Convert.ToInt32(hdfCustomerMenuLinkId.Value)
            End If

            SaveCustomerMenuLinkMapping()
            message.Visible = True
            message.InnerText = "Record saved successfully"

            'Dim result As Integer = 0

            'OBJMaster = New MasterBAL()
            'result = OBJMaster.SaveUpdateSpecializedFeature(CustomerMenuLinkId, lblSpecializedFeature.Text.Trim,Convert.ToInt32(Session("PersonId")))

            'If result > 0 Then
            '    SaveCustomerMenuLinkMapping()
            '    message.Visible = True
            '    message.InnerText = "Record saved successfully"

            '    If CustomerMenuLinkId = 0 Then
            '        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
            '            Dim writtenData As String = CreateData(result, txtSpecializedFeature.Text, False)
            '            CSCommonHelper.WriteLog("Added", "Specialized Feature", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            '        End If
            '    Else
            '        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
            '            Dim writtenData As String = CreateData(result, txtSpecializedFeature.Text, False)
            '            CSCommonHelper.WriteLog("Modified", "Specialized Feature", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            '        End If
            '    End If


            '    'Response.Redirect(String.Format("~/Master/FSNPFirmwareUpgrades?FSNPFirmwareId={0}&RecordIs=New", result))
            'Else

            '    If CustomerMenuLinkId = 0 Then
            '        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
            '            Dim writtenData As String = CreateData(result, txtSpecializedFeature.Text, False)
            '            CSCommonHelper.WriteLog("Added", "Specialized Feature", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Adding Specialized Feature failed")
            '        End If
            '    Else
            '        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
            '            Dim writtenData As String = CreateData(result, txtSpecializedFeature.Text, False)
            '            CSCommonHelper.WriteLog("Modified", "Specialized Feature", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Modifing Specialized Feature failed")
            '        End If
            '    End If

            'ErrorMessage.Visible = True
            'ErrorMessage.InnerText = "Error occurred while saving record, please try again"

            'End If


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Added", "Specialized Feature", "", "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Adding Specialized Feature failed. Exception is : " & ex.Message)
            End If
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
        Finally

        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllSpecializedFeature.aspx")
    End Sub

    Private Sub BindMenuLink(PersonId As Integer, RoleId As String, flag As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtSpecializedFeature As DataTable = New DataTable()

            dtSpecializedFeature = OBJMaster.GetCustomerMenuLinkMasterByCondition(" and CustomerMenuLinkId = " + hdfCustomerMenuLinkId.Value)
            If dtSpecializedFeature IsNot Nothing And dtSpecializedFeature.Rows.Count > 0 Then
                gvSpecializedFeature.DataSource = dtSpecializedFeature
                gvSpecializedFeature.DataBind()
            End If
        Catch ex As Exception

            log.Error("Error occurred in BindMenuLink Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindCheckBoxCompanyValues()
        Try
            Dim dtCustomerMenuLinkMapping As DataTable = New DataTable()
            dtCustomerMenuLinkMapping = OBJMaster.GetCustomerMenuLinkMappingByCustomerMenuLinkId(Convert.ToInt32(hdfCustomerMenuLinkId.Value))
            beforeLinks = ""
            For Each SpecializedFeatureRows As GridViewRow In gvSpecializedFeature.Rows
                Dim checkCheckedFlag As Integer = 0
                ' Dim CheckArray As ArrayList = New ArrayList
                Dim CustomerMenuLinkId As String = gvSpecializedFeature.DataKeys(SpecializedFeatureRows.RowIndex).Values("CustomerMenuLinkId").ToString()
                Dim Name As String = gvSpecializedFeature.DataKeys(SpecializedFeatureRows.RowIndex).Values("Name").ToString()
                Dim gvCustomers As GridView = TryCast(SpecializedFeatureRows.FindControl("gvCustomers"), GridView)
                If gvCustomers IsNot Nothing Then
                    For Each CustomersRows As GridViewRow In gvCustomers.Rows
                        Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                        Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                        Dim ChkLinks As CheckBox = TryCast(CustomersRows.FindControl("ChkLinks"), CheckBox)
                        If ChkLinks IsNot Nothing Then
                            If dtCustomerMenuLinkMapping IsNot Nothing And dtCustomerMenuLinkMapping.Rows.Count > 0 Then
                                For i = 0 To dtCustomerMenuLinkMapping.Rows.Count - 1
                                    If CustomerId = dtCustomerMenuLinkMapping.Rows(i)("CustomerId") Then
                                        If ChkLinks IsNot Nothing Then
                                            ChkLinks.Checked = True
                                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                beforeLinks = IIf(beforeLinks = "", " Name = " + Name + " CustomerName = " + CustomerName, beforeLinks & ";" & " Name = " + Name + " CustomerName= " + CustomerName)
                                            End If
                                            'CheckArray.Add(SiteID)
                                        Else
                                            checkCheckedFlag = 1
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    Next
                    Dim DtView As DataView = New DataView(dtCustomerMenuLinkMapping)
                    DtView.RowFilter = "CustomerMenuLinkMappingId = " + CustomerMenuLinkId
                    If DtView.Count = gvCustomers.Rows.Count And gvCustomers.Rows.Count > 0 Then
                        Dim checkAll As CheckBox = DirectCast(gvCustomers.HeaderRow.FindControl("checkAll"), CheckBox)
                        If checkAll IsNot Nothing Then
                            checkAll.Checked = True
                        End If
                    End If
                End If
            Next

            beforeData = CreateData(Convert.ToInt32(hdfCustomerMenuLinkId.Value), lblSpecializedFeature.Text, True)

        Catch ex As Exception
            log.Error("Error occurred in BindCheckBoxCompanyValues Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

    Protected Sub OnRowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                'Dim CustomerMenuLinkId As Integer = gvSpecializedFeature.DataKeys(e.Row.RowIndex).Value("CustomerMenuLinkId").ToString()
                Dim gvCustomers As GridView = TryCast(e.Row.FindControl("gvCustomers"), GridView)

                Dim dtCustomers As DataTable = New DataTable()
                OBJMaster = New MasterBAL()

                dtCustomers = OBJMaster.GetCustomerDetailsByPersonID(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())

                If dtCustomers IsNot Nothing And dtCustomers.Rows.Count > 0 Then
                    gvCustomers.DataSource = dtCustomers
                    gvCustomers.DataBind()
                Else
                    gvCustomers.DataSource = Nothing
                    gvCustomers.DataBind()
                End If

            End If
        Catch ex As Exception
            log.Error("Error occurred in OnRowDataBound Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try

    End Sub

    Private Sub SaveCustomerMenuLinkMapping()
        Try
            Dim dtCustomerMenuLinkMapping As DataTable = New DataTable("dtCustomerMenuLinkMapping")

            dtCustomerMenuLinkMapping.Columns.Add("CustomerId", System.Type.[GetType]("System.Int32"))
            dtCustomerMenuLinkMapping.Columns.Add("CustomerMenuLinkId", System.Type.[GetType]("System.Int32"))

            afterLinks = ""
            Dim strSite As String = ""
            For Each SpecializedFeatureRows As GridViewRow In gvSpecializedFeature.Rows
                Dim CustomerMenuLinkId As String = gvSpecializedFeature.DataKeys(SpecializedFeatureRows.RowIndex).Values("CustomerMenuLinkId").ToString()
                Dim Name As String = gvSpecializedFeature.DataKeys(SpecializedFeatureRows.RowIndex).Values("Name").ToString()
                Dim gvCustomers As GridView = TryCast(SpecializedFeatureRows.FindControl("gvCustomers"), GridView)
                If gvCustomers IsNot Nothing Then
                    'Delete previous save mapping against companu anf FSNP firmware
                    OBJMaster.DeleteCustomerMenuLinkMapping(CustomerMenuLinkId, IIf(Session("CustomerId").ToString() = "", "0", Session("CustomerId").ToString()))
                    For Each CustomersRows As GridViewRow In gvCustomers.Rows
                        Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                        Dim ChkLinks As CheckBox = TryCast(CustomersRows.FindControl("ChkLinks"), CheckBox)
                        Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                        If ChkLinks IsNot Nothing Then
                            If ChkLinks.Checked Then
                                Dim dr As DataRow = dtCustomerMenuLinkMapping.NewRow()
                                dr("CustomerId") = CustomerId
                                dr("CustomerMenuLinkId") = CustomerMenuLinkId
                                dtCustomerMenuLinkMapping.Rows.Add(dr)
                                afterLinks = IIf(beforeLinks = "", " Name = " + Name + " CustomerName = " + CustomerName, beforeLinks & ";" & " Name = " + Name + " CustomerName= " + CustomerName)
                            End If
                        End If
                    Next
                End If
            Next

            If dtCustomerMenuLinkMapping IsNot Nothing And dtCustomerMenuLinkMapping.Rows.Count > 0 Then
                OBJMaster.InsertCustomerMenuLinkMapping(dtCustomerMenuLinkMapping)
            End If

        Catch ex As Exception
            log.Error("Error occurred in SaveCustomerMenuLinkMapping Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

    Private Shared Function CreateData(CustomerMenuLinkId As String, Name As String, IsBefore As Boolean) As String
        Try

            Dim data As String = ""

            Dim mapping As String = ""

            If IsBefore Then
                mapping = beforeLinks
            Else
                mapping = afterLinks
            End If

            data = "CustomerMenuLinkId = " & CustomerMenuLinkId & " ; " &
                                    "Specialized Feature Name = " & Name.Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class