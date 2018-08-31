Imports log4net.Config
Imports log4net
Imports Microsoft.Reporting.WebForms

Public Class TotalizerMeterReconciliationReport
    Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TankInventoryDataEnteredReport))

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

					Dim dtTankInv As DataTable = Session("TotalizerMeterReconciliationDetails")

					RPT_TotalizerMeterReconciliationDetails.Visible = True

					RPT_TotalizerMeterReconciliationDetails.Reset()
					RPT_TotalizerMeterReconciliationDetails.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_TotalizerMeterReconciliationDetails.LocalReport
					rep.Refresh()
					Dim TankReconciliationReportDetailsPath As String = ConfigurationManager.AppSettings("TotalizerMeterReconciliationPath")
					rep.ReportPath = Server.MapPath(TankReconciliationReportDetailsPath)

					Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
					Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())

					RPT_TotalizerMeterReconciliationDetails.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate})

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "TankReconciliationReportDetails"
					rds.Value = dtTankInv
					rep.DataSources.Add(rds)

					Me.RPT_TotalizerMeterReconciliationDetails.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/TotalizerMeterReconciliationR")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/TotalizerMeterReconciliationR")
    End Sub

End Class