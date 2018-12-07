Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class BillingReportByDeptSummaryReport
    Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(BillingReportByDeptDetailsReport))

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				Response.Redirect("/Account/Login")
			ElseIf Session("RoleName") = "User" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If (Not IsPostBack) Then

                Dim dSTran As DataSet = Session("BillingReportByDeptSummary")

                RPT_BillingReportByDeptSummary.Visible = True

                RPT_BillingReportByDeptSummary.Reset()
                RPT_BillingReportByDeptSummary.ProcessingMode = ProcessingMode.Local
                Dim rep As LocalReport = RPT_BillingReportByDeptSummary.LocalReport
                rep.Refresh()
                Dim BillingReportByDeptDetailsPath As String = ConfigurationManager.AppSettings("BillingReportByDeptSummaryPath")
                rep.ReportPath = Server.MapPath(BillingReportByDeptDetailsPath)

                Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
                Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())
                    Dim TransactionType As ReportParameter = New ReportParameter("TransactionType", Session("TransactionType").ToString())
                    RPT_BillingReportByDeptSummary.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate, TransactionType})

                    Dim rds As ReportDataSource = New ReportDataSource()
                rds.Name = "BillingReportByDeptDetails"
                rds.Value = dSTran.Tables(0)
                rep.DataSources.Add(rds)

                Dim rdsGrandProductTotal As ReportDataSource = New ReportDataSource()
                rdsGrandProductTotal.Name = "BillingDepartmentSummary"
                rdsGrandProductTotal.Value = dSTran.Tables(1)
                rep.DataSources.Add(rdsGrandProductTotal)

                Dim rdsGrandProductTotalEnd As ReportDataSource = New ReportDataSource()
                rdsGrandProductTotalEnd.Name = "BillingGrandProductTotalEnd"
                rdsGrandProductTotalEnd.Value = dSTran.Tables(2)
                rep.DataSources.Add(rdsGrandProductTotalEnd)

                Me.RPT_BillingReportByDeptSummary.LocalReport.DataSources.Add(rds)

            End If
        End If


		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/BillingReportByDeptSummary")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/BillingReportByDeptSummary")
    End Sub

End Class