Imports log4net
Imports log4net.Config

Public Class CustomerWiseTransactionDetails
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CustomerWiseTransactionDetails))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

		ErrorMessage.Visible = False
        message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then

				'unautorized access error log
				Response.Redirect("/Account/Login")
			ElseIf Session("RoleName") <> "SuperAdmin" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If Not IsPostBack Then

					'BindTransactionStatus()

					'txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
					'txtTransactionDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
					'txtTransactionTimeFrom.Text = "12:00 AM"
					'txtTransactionTimeTo.Text = "11:59 PM"

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
				Else

					'txtTransactionDateFrom.Text = Request.Form(txtTransactionDateFrom.UniqueID)
					'txtTransactionDateTo.Text = Request.Form(txtTransactionDateTo.UniqueID)
					'txtTransactionTimeFrom.Text = Request.Form(txtTransactionTimeFrom.UniqueID)
					'txtTransactionTimeTo.Text = Request.Form(txtTransactionTimeTo.UniqueID)

				End If
				'txtTransactionDateFrom.Focus()
				DDL_Customer.Focus()
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();", True)
			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

    'Private Sub BindTransactionStatus()
    '    DDL_TransactionStatus.Items.Insert(0, New ListItem(ConfigurationManager.AppSettings("CompletedText").ToString(), "2"))
    '    DDL_TransactionStatus.Items.Insert(1, New ListItem(ConfigurationManager.AppSettings("NotStartedText").ToString(), "0"))
    '    DDL_TransactionStatus.Items.Insert(2, New ListItem(ConfigurationManager.AppSettings("MissedText").ToString(), "1"))
    'End Sub

    Private Sub BindCustomer(PersonId As Integer, RoleId As String)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()

            dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

            DDL_Customer.DataSource = dtCust
            DDL_Customer.DataTextField = "CustomerName"
            DDL_Customer.DataValueField = "CustomerId"
            DDL_Customer.DataBind()


            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support") Then
                DDL_Customer.SelectedIndex = 1
                DDL_Customer.Enabled = False
                DDL_Customer.Visible = False
                divCompany.Visible = False
            End If


            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                DDL_Customer.SelectedIndex = 0
            Else
                DDL_Customer.Items.Insert(0, New ListItem("All Companies", "0"))
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

        Try
            OBJMaster = New MasterBAL()
            Dim dSTran As DataSet = New DataSet()
            'Dim startDate As DateTime
            'Try
            '    startDate = Convert.ToDateTime(Request.Form(txtTransactionDateFrom.UniqueID) + " " + Request.Form(txtTransactionTimeFrom.UniqueID)).ToString()
            'Catch ex As Exception

            '    ErrorMessage.InnerText = "Wrong date format selected/enterd for 'From Date'."
            '    ErrorMessage.Visible = True
            '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();", True)
            '    Return
            'End Try
            'Dim endDate As DateTime
            'Try
            '    endDate = Convert.ToDateTime(Request.Form(txtTransactionDateTo.UniqueID) + " " + Request.Form(txtTransactionTimeTo.UniqueID)).ToString()
            'Catch ex As Exception
            '    ErrorMessage.InnerText = "Wrong date format selected/enterd for 'To Date'."
            '    ErrorMessage.Visible = True
            '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();", True)
            '    Return
            'End Try

            'If endDate < startDate Then
            '    ErrorMessage.InnerText = "'From Date' must less than 'To Date'."
            '    ErrorMessage.Visible = True
            '    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();", True)
            '    Return
            'End If

            Dim strConditions As String = ""
            If (DDL_Customer.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and T.CompanyId = " + DDL_Customer.SelectedValue, strConditions + " and T.CompanyId = " + DDL_Customer.SelectedValue)
            End If

            Dim SelectedSiteIds As String = ""
			Dim flagForZeroSite = True
			For Each item As ListItem In lstSites.Items
				If item.Selected Then
					SelectedSiteIds = IIf(SelectedSiteIds = "", item.Value, SelectedSiteIds + "," + item.Value)
				Else
					flagForZeroSite = False
				End If
			Next
			If flagForZeroSite Then
				SelectedSiteIds = "0," + SelectedSiteIds
			End If
			If (SelectedSiteIds <> "") Then
                strConditions = IIf(strConditions = "", " and T.SiteID in ( " + SelectedSiteIds + ")", strConditions + " and T.SiteID in ( " + SelectedSiteIds + ")")
            End If

            'If (DDL_TransactionStatus.SelectedValue = "2") Then
            '    strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
            'ElseIf (DDL_TransactionStatus.SelectedValue = "0") Then
            '    strConditions = (IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
            'ElseIf (DDL_TransactionStatus.SelectedValue = "1") Then
            '    strConditions = (IIf(strConditions = "", " and (ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and (ISNULL(T.TransactionStatus,0) = 1 And ISNULL(T.IsMissed,0)= 1)  and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
            'End If


            'get data from server
            'dSTran = OBJMaster.GetCustomerWiseTransactionDetails(startDate.ToString(), endDate.ToString(), strConditions)
            dSTran = OBJMaster.GetCustomerWiseTransactionDetails(strConditions)
            If (Not dSTran Is Nothing) Then

                If (dSTran.Tables(0).Rows.Count <= 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData = CreateData()
                        CSCommonHelper.WriteLog("Report Genereated", "Customer Wise Transaction Details", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();", True)
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData = CreateData()
                    CSCommonHelper.WriteLog("Report Genereated", "Customer Wise Transaction Details", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();", True)
                Return

            End If
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData = CreateData()
                CSCommonHelper.WriteLog("Report Genereated", "Customer Wise Transaction Details", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If
            Session("CustomerWiseTransactionDetails") = dSTran

            'Session("FromDate") = startDate.ToString("dd-MMM-yyyy hh:mm tt")
            'Session("ToDate") = endDate.ToString("dd-MMM-yyyy hh:mm tt")

            Response.Redirect("~/Reports/CustomerWiseTransactionDetailsReport.aspx")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData = CreateData()
                CSCommonHelper.WriteLog("Report Genereated", "Customer Wise Transaction Details", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
            End If
            ErrorMessage.InnerText = "Data not found against selected criteria."
            ErrorMessage.Visible = True
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();", True)
            Return
        Finally
            'txtTransactionDateFrom.Focus()
            DDL_Customer.Focus()
        End Try

    End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try
			BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))

		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();loadMultiList();$('[id*=lstSites]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
		End Try
	End Sub

	Private Sub BindSites(CustomerId As Integer)
        Try


            Dim dtSites As DataTable = New DataTable()
            OBJMaster = New MasterBAL()

            dtSites = OBJMaster.GetSiteByCondition(" And c.CustomerId =" + CustomerId.ToString(), Session("PersonId").ToString(), Session("RoleId").ToString(), False)

            'DDL_Site.DataSource = dtSites
            'DDL_Site.DataValueField = "SiteId"
            'DDL_Site.DataTextField = "WifiSSid"
            'DDL_Site.DataBind()

            'DDL_Site.Items.Insert(0, New ListItem("Select All FluidSecure Link", "0"))

            lstSites.DataSource = dtSites
            lstSites.DataValueField = "SiteId"
            lstSites.DataTextField = "WifiSSid"
            lstSites.DataBind()

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting sites, please try again later."

            log.Error("Error occurred in BindSites Exception is :" + ex.Message)

        End Try
    End Sub

    Private Function CreateData() As String
        Try

            Dim FluidSecureLink As String = ""

            For Each item As ListItem In lstSites.Items
                If item.Selected Then
                    FluidSecureLink = IIf(FluidSecureLink = "", item.Text, FluidSecureLink + " & " + item.Text)
                End If
            Next

            Dim data As String = "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                 "FluidSecure Link    = " & FluidSecureLink.Replace(",", " ") & " ; "

            'Dim data As String = "Transaction Date From = " & txtTransactionDateFrom.Text.Replace(",", "") & " ; " &
            '                        "Transaction Time From = " & txtTransactionTimeFrom.Text.Replace(",", " ") & " ; " &
            '                        "Transaction Date To = " & txtTransactionDateTo.Text.Replace(",", "") & " ; " &
            '                        "Transaction Time To = " & txtTransactionTimeTo.Text.Replace(",", "") & " ; " &
            '                        "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
            '                        "Transaction Status = " & DDL_TransactionStatus.SelectedItem.Text.Replace(",", " ") & " ; "
            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class