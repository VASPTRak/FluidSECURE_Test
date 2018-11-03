Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms
Public Class CustomerDetailsByStateCountryReport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CustomerDetailsByStateCountryReport))
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            XmlConfigurator.Configure()
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                Response.Redirect("/Account/Login")
            ElseIf Session("RoleName") <> "SuperAdmin" Then
                'Access denied 
                Response.Redirect("/home")
            Else
                If (Not IsPostBack) Then

                    Dim dSTran As DataSet = Session("StateCountryWiseCustomerReport")

                    RPT_CustomerDetailsByStateCountry.Visible = True

                    RPT_CustomerDetailsByStateCountry.Reset()
                    RPT_CustomerDetailsByStateCountry.ProcessingMode = ProcessingMode.Local
                    Dim rep As LocalReport = RPT_CustomerDetailsByStateCountry.LocalReport
                    rep.Refresh()
                    Dim TransReportByDateTimePath As String = ConfigurationManager.AppSettings("StateCountryWiseCustomerReportPath")
                    rep.ReportPath = Server.MapPath(TransReportByDateTimePath)

                    Dim rds As ReportDataSource = New ReportDataSource()
                    rds.Name = "StateCountryWiseCustomer"
                    rds.Value = dSTran.Tables(0)
                    rep.DataSources.Add(rds)

                    Me.RPT_CustomerDetailsByStateCountry.LocalReport.DataSources.Add(rds)


                End If
            End If

        Catch ex As Exception
            Log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            Response.Redirect("~/Reports/CustomerDetailsByStateCountry")
        End Try
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/CustomerDetailsByStateCountry")
    End Sub

End Class