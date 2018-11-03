Imports System.IO
Imports log4net
Imports log4net.Config
Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel
Public Class SpecializedExport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(SpecializedExport))

    Dim OBJMaster As MasterBAL
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Support" Then
                'Access denied 
                Response.Redirect("/home")
            Else
                If Session("RoleName") <> "SuperAdmin" Then
                    If Session("SpecializedExport") <> "SpecializedExport" Then
                        'Access denied 
                        Response.Redirect("/home")
                    End If
                End If

                If Not IsPostBack Then

                    txtFileName.Text = "TransactionDetails"
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
				'lblHeader.Text = "Specialized Export: " & DDL_Customer.SelectedItem.ToString()
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

            Dim dsTran As DataSet = New DataSet()

            dsTran = GetTransactions()
            If (Not dsTran Is Nothing) Then

                If (dsTran.Tables.Count < 1 Or dsTran.Tables(0).Rows.Count <= 0) Then

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
                        "attachment;filename=" & txtFileName.Text & ".txt")

            Response.Charset = ""
            Response.ContentType = "application/text"

            Dim separator As String = ","


            Dim sb As New StringBuilder()

            For i As Integer = 0 To dsTran.Tables(0).Rows.Count - 1
                Dim stringForConversion As String = dsTran.Tables(0).Rows(i)(0).ToString()
                Dim VehicleNumber As String = dsTran.Tables(0).Rows(i)(1).ToString()
                Dim HubExportCode As String = dsTran.Tables(0).Rows(i)(2).ToString()
                Dim Product As String = dsTran.Tables(0).Rows(i)(3).ToString()

                If VehicleNumber.Length > 10 Then
                    VehicleNumber = VehicleNumber.Substring(0, 10)
                End If
                If HubExportCode.Length > 7 Then
                    HubExportCode = VehicleNumber.Substring(0, 7)
                End If
                If Product.Length > 2 Then
                    Product = Product.Substring(0, 2)
                End If

                stringForConversion = stringForConversion.Replace("**VehicleNumber**", VehicleNumber.PadRight(10, " "))
                stringForConversion = stringForConversion.Replace("**HubExportCode**", HubExportCode.PadRight(7, " "))
                stringForConversion = stringForConversion.Replace("**Product**", Product.PadLeft(2, "0"))
                sb.Append(stringForConversion)
                'append new line
                sb.Append(vbCr & vbLf)
            Next

            Response.Output.Write(sb.ToString())

            Response.End()

        Catch ex As Exception

            log.Error("Error occurred in btnExportTransactions_Click Exception is :" + ex.Message)

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while export transactions, please try again later."
        End Try
    End Sub

    Private Function GetTransactions() As DataSet
        OBJMaster = New MasterBAL()
        Dim dSTran As DataSet = New DataSet()
        Try


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
            If chk_ExcludePreviouslyExportedTransactions.Checked Then
                strConditions = IIf(strConditions = "", " and isnull(T.IsPreviouslySpecializedExported,0) = 0 ", strConditions + " and isnull(T.IsPreviouslySpecializedExported,0) = 0 ")
            End If

            strConditions += " order by T.TransactionDateTime"

            Dim DecimalType As Integer = 0


            'get data from server
            dSTran = OBJMaster.SpecializedExportTransactions(startDate.ToString(), endDate.ToString(), strConditions)

        Catch ex As Exception
            log.Error("Error occurred in GetDefaultFormtTransactions Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while Getting transactions, please try again later."
        End Try

        Return dSTran

    End Function

End Class