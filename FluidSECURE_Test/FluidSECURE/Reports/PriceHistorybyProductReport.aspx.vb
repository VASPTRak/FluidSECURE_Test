Imports log4net.Config
Imports log4net
Imports Microsoft.Reporting.WebForms

Public Class PriceHistorybyProductReport
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(PriceHistorybyProductReport))

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

					Dim dSTran As DataSet = Session("PriceHistorybyProduct")

					RPT_PriceHistorybyProductReport.Visible = True

					RPT_PriceHistorybyProductReport.Reset()
					RPT_PriceHistorybyProductReport.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_PriceHistorybyProductReport.LocalReport
					rep.Refresh()
					Dim TransReportByDateTimePath As String = ConfigurationManager.AppSettings("PriceHistorybyProductPath")
					rep.ReportPath = Server.MapPath(TransReportByDateTimePath)

					Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
					Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())

					RPT_PriceHistorybyProductReport.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate})

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "PriceHistorybyProduct"
					rds.Value = dSTran.Tables(0)
					rep.DataSources.Add(rds)

					Me.RPT_PriceHistorybyProductReport.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			Log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/PriceHistorybyProduct")
		End Try
	End Sub

	Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/PriceHistorybyProduct")
    End Sub

End Class