Imports log4net.Config
Imports log4net
Imports Microsoft.Reporting.WebForms

Public Class TankReconciliationReport
    Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TankReconciliationReport))

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

					Dim dtTankInv As DataTable = Session("TankReconciliationDetails")

					RPT_TankReconciliationDetails.Visible = True

					RPT_TankReconciliationDetails.Reset()
					RPT_TankReconciliationDetails.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_TankReconciliationDetails.LocalReport
					rep.Refresh()
					Dim TankReconciliationReportDetailsPath As String = ConfigurationManager.AppSettings("TankReconciliationReportDetailsPath")
					rep.ReportPath = Server.MapPath(TankReconciliationReportDetailsPath)

					Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
					Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())

					RPT_TankReconciliationDetails.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate})

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "TankReconciliationReportDetails"
					rds.Value = dtTankInv
					rep.DataSources.Add(rds)

					Me.RPT_TankReconciliationDetails.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/TankReconciliation")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/TankReconciliation")
    End Sub

End Class