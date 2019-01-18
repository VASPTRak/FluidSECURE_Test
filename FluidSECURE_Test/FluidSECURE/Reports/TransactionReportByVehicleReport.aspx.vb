Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class TransactionReportByVehicleReport
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TransactionReportByVehicleReport))

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

					Dim dSTran As DataSet = Session("TransReportByVehicle")

					RPT_TransactionReportByVehicle.Visible = True

					RPT_TransactionReportByVehicle.Reset()
					RPT_TransactionReportByVehicle.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_TransactionReportByVehicle.LocalReport
					rep.Refresh()
					Dim TransReportByDateTimePath As String = ConfigurationManager.AppSettings("TransactionReportByVehiclePath")
					rep.ReportPath = Server.MapPath(TransReportByDateTimePath)

					Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
					Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())
					Dim TransactionType As ReportParameter = New ReportParameter("TransactionType", Session("TransactionType").ToString())
					Dim TransactionStatusText As ReportParameter = New ReportParameter("TransactionStatusText", Session("TransactionStatusText").ToString())
					Dim FuelingTypeCurrent As ReportParameter = New ReportParameter("FuelingTypeCurrent", Session("FuelingTypeCurrent").ToString())
					Dim FuelingTypePrevious As ReportParameter = New ReportParameter("FuelingTypePrevious", Session("FuelingTypePrevious").ToString())

					RPT_TransactionReportByVehicle.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate, TransactionType, TransactionStatusText, FuelingTypeCurrent, FuelingTypePrevious})

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "TransactionReportsByConditions"
					rds.Value = dSTran.Tables(0)
					rep.DataSources.Add(rds)

					Dim rds1 As ReportDataSource = New ReportDataSource()
					rds1.Name = "TransactionReportsByVehicleMilesDriven"
					rds1.Value = dSTran.Tables(1)
					rep.DataSources.Add(rds1)

					Dim rds2 As ReportDataSource = New ReportDataSource()
					rds2.Name = "TransactionReportsForTotalQuantity"
					rds2.Value = dSTran.Tables(2)
					rep.DataSources.Add(rds2)

					Me.RPT_TransactionReportByVehicle.LocalReport.DataSources.Add(rds)
					Me.RPT_TransactionReportByVehicle.LocalReport.DataSources.Add(rds1)
					Me.RPT_TransactionReportByVehicle.LocalReport.DataSources.Add(rds2)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/TransactionReportByVehicle")
		End Try
	End Sub

	Protected Sub btnBack_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Reports/TransactionReportByVehicle")
	End Sub

End Class