Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class FluidsecureLinkTotalizerDataEnteredReport
    Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FluidsecureLinkTotalizerDataEnteredReport))

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

					Dim dtTankInv As DataTable = Session("FluidsecureLinkTotalizerDataEnteredDetails")

					RPT_FluidsecureLinkTotalizerDataEnteredDetails.Visible = True

					RPT_FluidsecureLinkTotalizerDataEnteredDetails.Reset()
					RPT_FluidsecureLinkTotalizerDataEnteredDetails.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_FluidsecureLinkTotalizerDataEnteredDetails.LocalReport
					rep.Refresh()
					Dim TankReconciliationReportDetailsPath As String = ConfigurationManager.AppSettings("FluidsecureLinkTotalizerDataEnteredDetailsPath")
					rep.ReportPath = Server.MapPath(TankReconciliationReportDetailsPath)

					Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
					Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())

					RPT_FluidsecureLinkTotalizerDataEnteredDetails.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate})

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "TankInventoryDataEnteredDetails"
					rds.Value = dtTankInv
					rep.DataSources.Add(rds)

					Me.RPT_FluidsecureLinkTotalizerDataEnteredDetails.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/FluidsecureLinkTotalizerDataEntered")
		End Try
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/FluidsecureLinkTotalizerDataEntered")
    End Sub

End Class