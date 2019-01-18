Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms
Public Class VehiclePerformanceReport
    Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(VehiclePerformanceReport))

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

                Dim dSTran As DataSet = Session("VehicleByPerformanceDetails")

                RPT_VehiclePerformanceDetails.Visible = True

                RPT_VehiclePerformanceDetails.Reset()
                RPT_VehiclePerformanceDetails.ProcessingMode = ProcessingMode.Local
                Dim rep As LocalReport = RPT_VehiclePerformanceDetails.LocalReport
                rep.Refresh()
                Dim TransReportByDateTimePath As String = ConfigurationManager.AppSettings("VehicleByPerformancePath")
                rep.ReportPath = Server.MapPath(TransReportByDateTimePath)

                Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
                Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())
                Dim Fuel As ReportParameter = New ReportParameter("Fuel", Session("FuelType").ToString())
                    Dim TransactionStatusText As ReportParameter = New ReportParameter("TransactionStatusText", Session("TransactionStatusText").ToString())
                    RPT_VehiclePerformanceDetails.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate, Fuel, TransactionStatusText})

                    Dim rds As ReportDataSource = New ReportDataSource()
                rds.Name = "VehicleByPerformanceDetails"
                rds.Value = dSTran.Tables(0)
                rep.DataSources.Add(rds)

                Me.RPT_VehiclePerformanceDetails.LocalReport.DataSources.Add(rds)

            End If
        End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/VehiclePerformance")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/VehiclePerformance")
    End Sub

End Class