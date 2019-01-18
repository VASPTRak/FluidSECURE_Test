Public Class SiteMaster
    Inherits MasterPage
    Private Const AntiXsrfTokenKey As String = "__AntiXsrfToken"
    Private Const AntiXsrfUserNameKey As String = "__AntiXsrfUserName"
    Private _antiXsrfTokenValue As String

    Protected Sub Page_Init(sender As Object, e As EventArgs)
        ' The code below helps to protect against XSRF attacks
        Dim requestCookie = Request.Cookies(AntiXsrfTokenKey)
        Dim requestCookieGuidValue As Guid
        If requestCookie IsNot Nothing AndAlso Guid.TryParse(requestCookie.Value, requestCookieGuidValue) Then
            ' Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value
            Page.ViewStateUserKey = _antiXsrfTokenValue
        Else
            ' Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N")
            Page.ViewStateUserKey = _antiXsrfTokenValue

            Dim responseCookie = New HttpCookie(AntiXsrfTokenKey) With {
                 .HttpOnly = True,
                 .Value = _antiXsrfTokenValue
            }
            If FormsAuthentication.RequireSSL AndAlso Request.IsSecureConnection Then
                responseCookie.Secure = True
            End If
            Response.Cookies.[Set](responseCookie)
        End If

        AddHandler Page.PreLoad, AddressOf master_Page_PreLoad
    End Sub

    Protected Sub master_Page_PreLoad(sender As Object, e As EventArgs)
        'Response.Redirect("~/SiteRedirect.html")

        If Not IsPostBack Then
            ' Set Anti-XSRF token
            ViewState(AntiXsrfTokenKey) = Page.ViewStateUserKey
            ViewState(AntiXsrfUserNameKey) = If(Context.User.Identity.Name, [String].Empty)

            Dim userAgent = Context.Request.UserAgent
            If (userAgent Is Nothing) Then
                userAgent = ""
            End If

            Dim UrlReferrer = ""
            If Context.Request.UrlReferrer IsNot Nothing Then
                UrlReferrer = Context.Request.UrlReferrer.ToString()
            End If

            Dim Method = Context.Request.HttpMethod
            If (Method Is Nothing) Then
                Method = ""
            End If

            Dim IPAddress As String = Context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If IPAddress = "" Or IPAddress Is Nothing Then
                IPAddress = Context.Request.ServerVariables("REMOTE_ADDR")
            End If
            If (IPAddress Is Nothing) Then
                IPAddress = ""
            End If
            'Dim provider As IServiceProvider = CType(context, IServiceProvider)

            'Dim worker As HttpWorkerRequest = CType(provider.GetService(GetType(HttpWorkerRequest)), HttpWorkerRequest)

            'Dim referer As String = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderReferer)

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Page Request", "Page Request", "", "userAgent = " & userAgent.ToString().Replace(",", " ") & " ; Referrer = " & UrlReferrer &
                                        " ; Method = " & Method.ToString().Replace(",", " "), "", IPAddress, "sucsess", "")
            End If

        Else
            ' Validate the Anti-XSRF token
            If DirectCast(ViewState(AntiXsrfTokenKey), String) <> _antiXsrfTokenValue OrElse DirectCast(ViewState(AntiXsrfUserNameKey), String) <> (If(Context.User.Identity.Name, [String].Empty)) Then
                Throw New InvalidOperationException("Validation of Anti-XSRF token failed.")
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("login") Then
            'If Session("PersonId") Is Nothing Then
            '    ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "MSG", "OPENURL();", True)
            'End If
        Else
            Dim AutheticationManager = HttpContext.Current.GetOwinContext().Authentication
            AutheticationManager.SignOut()
        End If

        If Session("FromLoginPage") <> "Y" Then 'New field for Adding Reset password date

            OtherMenu.Visible = False
            CompanyHostingReport.Visible = False
            ShipmentReport.Visible = False
            SpecializedFeature.Visible = False
            Session("SpecializedExport") = ""
            Session("TotalFuelUsageByHubPerVehicle") = ""
            Session("SpecializedPersonnelImport") = ""
            Session("SpecializedVehicleImport") = ""
            Session("SpecializedVehicleInactiveImport") = ""
            Session("SpecializedTransactionImport") = ""
            CustomerWiseTransactionDetails.Visible = False
            ResetTermsPrivacyPolicys.Visible = False
            Export_WINCC.Visible = False
            SpecializedExport.Visible = False
            TotalFuelUsageByHubPerVehicle.Visible = False
            SpecializedPersonnelImport.Visible = False
            SpecializedVehicleImport.Visible = False
            SpecializedVehicleInactiveImport.Visible = False
            SpecializedTransactionImport.Visible = False

            If Session("RoleName") = "User" Then

                ItemMenu.Visible = False
                ReportMenu.Visible = False
                TransactionsMenu.Visible = False
                Export.Visible = False
                Import.Visible = False
                Reconciliation.Visible = False
                Export_WINCC.Visible = False
            ElseIf Session("RoleName") = "Reports Only" Then
                ItemMenu.Visible = False
                ReportMenu.Visible = True
                TransactionsMenu.Visible = False
                Import.Visible = False
                Reconciliation.Visible = False

                Export.Visible = True
                Export_WINCC.Visible = True
                StandardExport.Visible = True
                CustomizedExport.Visible = False
                TransactionExportSetting.Visible = False
            ElseIf Session("RoleName") = "Support" Then
                ItemMenu.Visible = False

                VehiclestMenu.Visible = False
                PersonnelMenu.Visible = False
                DepartmentsMenu.Visible = False
                CompaniesMenu.Visible = False
                ProductsMenu.Visible = False
                FluidSecureMenu.Visible = False
                UploadedFirmware.Visible = False
                UploadedTLDFirmware.Visible = False
                UploadedFSVMFirmware.Visible = False
                UploadedFSNPFirmware.Visible = False
                FluidSecureHubMenu.Visible = False
                DayLightSavingId.Visible = False
                Export_WINCC.Visible = False

                ReportMenu.Visible = True

                TransactionsMenu.Visible = False
                Export.Visible = False
                Import.Visible = False
                Reconciliation.Visible = False
            End If

            If (Session("RoleName") = "SuperAdmin") Then
                ShipmentMenu.Visible = True
                UploadedFirmware.Visible = True
                UploadedTLDFirmware.Visible = True
                UploadedFSVMFirmware.Visible = True
                UploadedFSNPFirmware.Visible = True
                OtherMenu.Visible = True
                CompanyHostingReport.Visible = True
                ShipmentReport.Visible = True
                SpecializedFeature.Visible = True
                CustomerWiseTransactionDetails.Visible = True
                ResetTermsPrivacyPolicys.Visible = True
                DayLightSavingId.Visible = True
                TransactionExportSetting.Visible = True
                Export_WINCC.Visible = True

                ' Specilized Export
                SpecializedExport.Visible = True
                SpecializedExport.InnerHtml = "<a href=/Master/SpecializedExport.aspx>Specialized Export: Hawaii Telecom" & "</a>"

                ' TotalFuelUsageByHubPerVehicle
                TotalFuelUsageByHubPerVehicle.Visible = True

                ' Specilized Personnel Import
                SpecializedPersonnelImport.Visible = True
                SpecializedPersonnelImport.InnerHtml = "<a href=/Master/SpecializedPersonnelImport.aspx>Specialized Personnel Import: Hawaii Telecom" & "</a>"

                ' Specilized Vehicle Import
                SpecializedVehicleImport.Visible = True
                SpecializedVehicleImport.InnerHtml = "<a href=/Master/SpecializedVehicleImport.aspx>Specialized Vehicle Import: Hawaii Telecom" & "</a>"

                ' Specialized Vehicle Inactive Import
                SpecializedVehicleInactiveImport.Visible = True
                SpecializedVehicleInactiveImport.InnerHtml = "<a href=/Master/SpecializedInActiveVehicleImport.aspx>Specialized Vehicle Inactive Import: Hawaii Telecom" & "</a>"

                ' Specialized Transaction Import
                SpecializedTransactionImport.Visible = True
                SpecializedTransactionImport.InnerHtml = "<a href=/Master/SpecializedTransactionImport.aspx>Specialized Transaction Import: Hawaii Telecom" & "</a>"

            Else
                If (Session("RoleName") = "Support") Then 'If (Session("RoleName") = "CustomerAdmin" Or Session("RoleName") = "Support") Then
                    ShipmentMenu.Visible = True
                    UploadedFirmware.Visible = False
                    UploadedTLDFirmware.Visible = False
                    UploadedFSVMFirmware.Visible = False
                    UploadedFSNPFirmware.Visible = False
                    OtherMenu.Visible = True
                Else
                    ShipmentMenu.Visible = False
                End If
                UploadedFirmware.Visible = False
                UploadedTLDFirmware.Visible = False
                UploadedFSVMFirmware.Visible = False
                UploadedFSNPFirmware.Visible = False
                DayLightSavingId.Visible = False

                If (Session("RoleName") = "CustomerAdmin") Or (Session("RoleName") = "GroupAdmin") Then
                    TransactionExportSetting.Visible = True
                    Export.Visible = True


                    ' Specilized Feature
                    If Session("PersonId") IsNot Nothing And Session("UniqueId") IsNot Nothing Then
                        Dim OBJMaster As MasterBAL = New MasterBAL()
                        Dim dtPersonnel As DataTable = New DataTable()
                        Dim cnt As Integer = 0

                        dtPersonnel = OBJMaster.GetPersonnelByPersonIdAndId(Convert.ToInt32(Session("PersonId")), Session("UniqueId").ToString())
                        If dtPersonnel IsNot Nothing And dtPersonnel.Rows.Count > 0 Then
                            OBJMaster = New MasterBAL()

                            Dim dtuspGetCustMenuMapingById As DataTable = New DataTable()
                            dtuspGetCustMenuMapingById = OBJMaster.GetCustMenuMapingById("", dtPersonnel.Rows(0)("CustomerId").ToString())
                            If dtuspGetCustMenuMapingById IsNot Nothing And dtuspGetCustMenuMapingById.Rows.Count > 0 Then

                                Dim dr As DataRow() = dtuspGetCustMenuMapingById.Select("CustomerMenuLinkId = 1")
                                If dr.Count > 0 Then
                                    SpecializedExport.Visible = True
                                    If Session("CompanyNameHeader").ToString() IsNot Nothing Then
                                        Session("SpecializedExport") = "SpecializedExport"
                                        If (Session("RoleName") = "CustomerAdmin") Then
                                            SpecializedExport.InnerHtml = "<a href=/Master/SpecializedExport.aspx>Specialized Export : " & Session("CompanyNameHeader").ToString() & "</a>"
                                        Else
                                            SpecializedExport.InnerHtml = "<a href=/Master/SpecializedExport.aspx>Specialized Export: Hawaii Telecom" & "</a>"
                                        End If
                                    Else
                                        SpecializedExport.Visible = False
                                    End If
                                Else
                                    SpecializedExport.Visible = False
                                End If

                                dr = dtuspGetCustMenuMapingById.Select("CustomerMenuLinkId = 2")
                                If dr.Count > 0 Then
                                    Session("TotalFuelUsageByHubPerVehicle") = "TotalFuelUsageByHubPerVehicle"
                                    TotalFuelUsageByHubPerVehicle.Visible = True
                                Else
                                    TotalFuelUsageByHubPerVehicle.Visible = False
                                End If

                                dr = dtuspGetCustMenuMapingById.Select("CustomerMenuLinkId = 3")
                                If dr.Count > 0 Then
                                    SpecializedPersonnelImport.Visible = True
                                    If Session("CompanyNameHeader").ToString() IsNot Nothing Then
                                        Session("SpecializedPersonnelImport") = "SpecializedPersonnelImport"
                                        If (Session("RoleName") = "CustomerAdmin") Then
                                            SpecializedPersonnelImport.InnerHtml = "<a href=/Master/SpecializedPersonnelImport.aspx>Specialized Personnel Import: " & Session("CompanyNameHeader").ToString() & "</a>"
                                        Else
                                            SpecializedPersonnelImport.InnerHtml = "<a href=/Master/SpecializedPersonnelImport.aspx>Specialized Personnel Import: Hawaii Telecom" & "</a>"
                                        End If
                                    Else
                                        SpecializedPersonnelImport.Visible = False
                                    End If
                                Else
                                    SpecializedPersonnelImport.Visible = False
                                End If
                                dr = dtuspGetCustMenuMapingById.Select("CustomerMenuLinkId = 4")
                                If dr.Count > 0 Then
                                    SpecializedVehicleImport.Visible = True
                                    If Session("CompanyNameHeader").ToString() IsNot Nothing Then
                                        Session("SpecializedVehicleImport") = "SpecializedVehicleImport"
                                        If (Session("RoleName") = "CustomerAdmin") Then
                                            SpecializedVehicleImport.InnerHtml = "<a href=/Master/SpecializedVehicleImport.aspx>Specialized Vehicle Import: " & Session("CompanyNameHeader").ToString() & "</a>"
                                        Else
                                            SpecializedVehicleImport.InnerHtml = "<a href=/Master/SpecializedVehicleImport.aspx>Specialized Vehicle Import: Hawaii Telecom" & "</a>"
                                        End If
                                    Else
                                        SpecializedVehicleImport.Visible = False
                                    End If
                                Else
                                    SpecializedVehicleImport.Visible = False
                                End If
                                dr = dtuspGetCustMenuMapingById.Select("CustomerMenuLinkId = 5")
                                If dr.Count > 0 Then
                                    SpecializedVehicleInactiveImport.Visible = True
                                    If Session("CompanyNameHeader").ToString() IsNot Nothing Then
                                        Session("SpecializedVehicleInactiveImport") = "SpecializedVehicleInactiveImport"
                                        If (Session("RoleName") = "CustomerAdmin") Then
                                            SpecializedVehicleInactiveImport.InnerHtml = "<a href=/Master/SpecializedInActiveVehicleImport.aspx>Specialized Vehicle Inactive Import: " & Session("CompanyNameHeader").ToString() & "</a>"
                                        Else
                                            SpecializedVehicleInactiveImport.InnerHtml = "<a href=/Master/SpecializedInActiveVehicleImport.aspx>Specialized Vehicle Inactive Import: Hawaii Telecom" & "</a>"
                                        End If
                                    Else
                                        SpecializedVehicleInactiveImport.Visible = False
                                    End If
                                Else
                                    SpecializedVehicleInactiveImport.Visible = False
                                End If
                                dr = dtuspGetCustMenuMapingById.Select("CustomerMenuLinkId = 6")
                                If dr.Count > 0 Then
                                    SpecializedTransactionImport.Visible = True
                                    If Session("CompanyNameHeader").ToString() IsNot Nothing Then
                                        Session("SpecializedTransactionImport") = "SpecializedTransactionImport"
                                        If (Session("RoleName") = "CustomerAdmin") Then
                                            SpecializedTransactionImport.InnerHtml = "<a href=/Master/SpecializedTransactionImport.aspx>Specialized Transaction Import: " & Session("CompanyNameHeader").ToString() & "</a>"
                                        Else
                                            SpecializedTransactionImport.InnerHtml = "<a href=/Master/SpecializedTransactionImport.aspx>Specialized Transaction Import: Hawaii Telecom" & "</a>"
                                        End If
                                    Else
                                        SpecializedTransactionImport.Visible = False
                                    End If
                                Else
                                    SpecializedTransactionImport.Visible = False
                                End If
                            Else
                                SpecializedExport.Visible = False
                                TotalFuelUsageByHubPerVehicle.Visible = False
                                SpecializedPersonnelImport.Visible = False
                                SpecializedVehicleImport.Visible = False
                                SpecializedVehicleInactiveImport.Visible = False
                                SpecializedTransactionImport.Visible = False
                            End If
                        Else
                            SpecializedExport.Visible = False
                            TotalFuelUsageByHubPerVehicle.Visible = False
                            SpecializedPersonnelImport.Visible = False
                            SpecializedVehicleImport.Visible = False
                            SpecializedVehicleInactiveImport.Visible = False
                            SpecializedTransactionImport.Visible = False
                        End If
                    Else
                        SpecializedExport.Visible = False
                        TotalFuelUsageByHubPerVehicle.Visible = False
                        SpecializedPersonnelImport.Visible = False
                        SpecializedVehicleImport.Visible = False
                        SpecializedVehicleInactiveImport.Visible = False
                        SpecializedTransactionImport.Visible = False
                    End If

                End If
            End If
        End If
        If Session("CompanyNameHeader") <> Nothing Then
            If (Session("CompanyNameHeader").ToString <> "" And Session("CompanyNameHeader").ToString <> "Select All") Then
                If hdfHide.Value <> "homepage" Then
                    lblMasterCompany.Text = "Company: "
                    lblMasterCompanyName.Text = " " + Session("CompanyNameHeader").ToString()
                Else
                    lblMasterCompany.Text = ""
                    lblMasterCompanyName.Text = ""
                End If
            Else
                lblMasterCompany.Text = ""
                lblMasterCompanyName.Text = ""
            End If
        Else
            lblMasterCompany.Text = ""
            lblMasterCompanyName.Text = ""
        End If

    End Sub
    Protected Sub Unnamed_LoggingOut(sender As Object, e As LoginCancelEventArgs)
        Context.GetOwinContext().Authentication.SignOut()
        Session.Abandon()
    End Sub

End Class
