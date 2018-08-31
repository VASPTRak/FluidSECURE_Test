Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class BillingReportByVehicleSummaryReport
    Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(BillingReportByVehicleSummaryReport))

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

					Dim dSTran As DataSet = Session("BillingReportByVehicleSummary")

					RPT_BillingReportByVehicleSummary.Visible = True

					RPT_BillingReportByVehicleSummary.Reset()
					RPT_BillingReportByVehicleSummary.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_BillingReportByVehicleSummary.LocalReport
					rep.Refresh()
					Dim BillingReportByDeptDetailsPath As String = ConfigurationManager.AppSettings("BillingReportByVehicleSummaryPath")
					rep.ReportPath = Server.MapPath(BillingReportByDeptDetailsPath)

					Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
					Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())

					RPT_BillingReportByVehicleSummary.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate})

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "BillingReportByDeptDetails"
					rds.Value = dSTran.Tables(0)
					rep.DataSources.Add(rds)

					Dim rdsGrandProductTotal As ReportDataSource = New ReportDataSource()
					rdsGrandProductTotal.Name = "BillingVehicleSummary"
					rdsGrandProductTotal.Value = dSTran.Tables(1)
					rep.DataSources.Add(rdsGrandProductTotal)

                Dim rdsGrandProductTotalEnd As ReportDataSource = New ReportDataSource()
                rdsGrandProductTotalEnd.Name = "BillingGrandProductTotalEnd"
                rdsGrandProductTotalEnd.Value = dSTran.Tables(2)
                rep.DataSources.Add(rdsGrandProductTotalEnd)

                Me.RPT_BillingReportByVehicleSummary.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/BillingReportByVehicleSummary")
		End Try
	End Sub

	Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/BillingReportByVehicleSummary")
    End Sub

End Class