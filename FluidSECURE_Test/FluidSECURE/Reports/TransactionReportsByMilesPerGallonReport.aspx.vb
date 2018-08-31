Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class TransactionReportsByMilesPerGallonReport
    Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TransactionReportsByMilesPerGallonReport))

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

                Dim dSTran As DataSet = Session("TransReportByMPGPerK")

                RPT_TransReportByMilesPerGallonReport.Visible = True

                RPT_TransReportByMilesPerGallonReport.Reset()
                RPT_TransReportByMilesPerGallonReport.ProcessingMode = ProcessingMode.Local
                Dim rep As LocalReport = RPT_TransReportByMilesPerGallonReport.LocalReport
                rep.Refresh()
                Dim TransReportByDateTimePath As String = ConfigurationManager.AppSettings("TransReportByMPGPerKPath")
                rep.ReportPath = Server.MapPath(TransReportByDateTimePath)

                Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
                Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())
                Dim MPGParam As ReportParameter = New ReportParameter("MPGParam", Session("MPGParam").ToString())

                RPT_TransReportByMilesPerGallonReport.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate, MPGParam})
                Dim rds As ReportDataSource = New ReportDataSource()
                rds.Name = "TransactionReportByMPGPerK"
                rds.Value = dSTran.Tables(0)
                rep.DataSources.Add(rds)

                Me.RPT_TransReportByMilesPerGallonReport.LocalReport.DataSources.Add(rds)

            End If
        End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/TransactionReportsByMilesPerGallon")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/TransactionReportsByMilesPerGallon")
    End Sub

End Class