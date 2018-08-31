Imports log4net
Imports log4net.Config

Public Class ExportToWINCC
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(ExportToWINCC))

    Dim OBJMaster As MasterBAL
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

		ErrorMessage.Visible = False
        message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Support" Then 'Or Session("RoleName") = "Reports Only" 
                'Access denied 
                Response.Redirect("/home")
			Else
				If Not IsPostBack Then

					BindTransactionStatus()

					txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
					txtTransactionDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
					txtTransactionTimeFrom.Text = "12:00 AM"
					txtTransactionTimeTo.Text = "11:59 PM"

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

					txtTransactionDateFrom.Focus()
				Else

					txtTransactionDateFrom.Text = Request.Form(txtTransactionDateFrom.UniqueID)
					txtTransactionDateTo.Text = Request.Form(txtTransactionDateTo.UniqueID)
					txtTransactionTimeFrom.Text = Request.Form(txtTransactionTimeFrom.UniqueID)
					txtTransactionTimeTo.Text = Request.Form(txtTransactionTimeTo.UniqueID)

				End If

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

    Private Sub BindCustomer(PersonId As Integer, RoleId As String)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()

            dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

            DDL_Customer.DataSource = dtCust
            DDL_Customer.DataTextField = "CustomerName"
            DDL_Customer.DataValueField = "CustomerId"
            DDL_Customer.DataBind()
            DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

            If (Not Session("RoleName") = "SuperAdmin") Then
                DDL_Customer.SelectedIndex = 1
                DDL_Customer.Enabled = False
                DDL_Customer.Visible = False
                divCompany.Visible = False
            End If

            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                DDL_Customer.SelectedIndex = 1

            End If

        Catch ex As Exception

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Protected Sub btnExportTransactions_Click(sender As Object, e As EventArgs)
        Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			ErrorMessage.Visible = False
            message.Visible = False
            ErrorMessage.InnerText = ""
            message.InnerText = ""

            OBJMaster = New MasterBAL()
            Dim dSTran As DataSet = New DataSet()
            'start end date
            Dim startDate As DateTime
            startDate = Convert.ToDateTime(Request.Form(txtTransactionDateFrom.UniqueID) + " " + Request.Form(txtTransactionTimeFrom.UniqueID)).ToString()

            Dim endDate As DateTime
            endDate = Convert.ToDateTime(Request.Form(txtTransactionDateTo.UniqueID) + " " + Request.Form(txtTransactionTimeTo.UniqueID)).ToString()


            Dim strConditions As String = ""
            If (DDL_Customer.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and c.CustomerId = " + DDL_Customer.SelectedValue, strConditions + " and c.CustomerId = " + DDL_Customer.SelectedValue)
            End If

            'strConditions = IIf(strConditions = "", " and ISNULL(t.IsMissed,0) = " + DDL_Missed.SelectedValue, strConditions + " and ISNULL(t.IsMissed,0) = " + DDL_Missed.SelectedValue)
            If (DDL_TransactionStatus.SelectedValue = "2") Then
                strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
            ElseIf (DDL_TransactionStatus.SelectedValue = "0") Then
                strConditions = (IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
            ElseIf (DDL_TransactionStatus.SelectedValue = "1") Then
                strConditions = (IIf(strConditions = "", " and (ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and (ISNULL(T.TransactionStatus,0) = 1 And ISNULL(T.IsMissed,0)= 1)  and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
            End If

            strConditions += " order by t.TransactionDateTime"

            'get data from server
            dSTran = OBJMaster.ExportTransactions(startDate.ToString(), endDate.ToString(), strConditions, 2)
            If (Not dSTran Is Nothing) Then

                If (dSTran.Tables.Count < 1 Or dSTran.Tables(0).Rows.Count <= 0) Then

                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                    Return
                End If
            Else

                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                Return

            End If


            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition",
                    "attachment;filename=ExportToWINCC.txt")
            Response.Charset = ""
            Response.ContentType = "application/text"

            Dim sb As New StringBuilder()

            'For k As Integer = 0 To dSTran.Tables(0).Columns.Count - 1
            '    sb.Append(dSTran.Tables(0).Columns(k).ColumnName + ","c)
            'Next

            'append New line
            'sb.Append(vbCr & vbLf)

            For i As Integer = 0 To dSTran.Tables(0).Rows.Count - 1
                For k As Integer = 0 To dSTran.Tables(0).Columns.Count - 1
                    'add separator
                    If (dSTran.Tables(0).Columns(k).ColumnName = "TransactionStartDateTime") Then
                        sb.Append(Convert.ToDateTime(dSTran.Tables(0).Rows(i)(k)).ToString("MM/dd/yyyy hh:mm:ss tt").Replace(",", ";") + ","c)
                    Else
                        sb.Append(dSTran.Tables(0).Rows(i)(k).ToString().Replace(",", ";") + ","c)

                    End If


                Next
                'append new line
                sb.Append(vbCr & vbLf)
            Next
            Response.Output.Write(sb.ToString())
            'Response.Flush()
            Response.End()


        Catch ex As Exception

            log.Error("Error occurred in btnExportTransactions_Click Exception is :" + ex.Message)

        End Try
    End Sub

	Private Sub BindTransactionStatus()
		Try
			DDL_TransactionStatus.Items.Insert(0, New ListItem(ConfigurationManager.AppSettings("CompletedText").ToString(), "2"))
			DDL_TransactionStatus.Items.Insert(1, New ListItem(ConfigurationManager.AppSettings("NotStartedText").ToString(), "0"))
			DDL_TransactionStatus.Items.Insert(2, New ListItem(ConfigurationManager.AppSettings("MissedText").ToString(), "1"))

		Catch ex As Exception
			log.Error("Error occurred in BindTransactionStatus Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

End Class