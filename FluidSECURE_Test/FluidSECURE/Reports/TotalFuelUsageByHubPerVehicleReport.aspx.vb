Imports log4net.Config
Imports log4net
Imports Microsoft.Reporting.WebForms
Public Class TotalFuelUsageByHubPerVehicleReport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TotalFuelUsageByHubPerVehicleReport))
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

                    Dim dtTotalFuelUse As DataTable = Session("TotalFuelUsageByHubPerVehicleDetails")

                    RPT_TotalFuelUsageByHubPerVehicleDetails.Visible = True

                    RPT_TotalFuelUsageByHubPerVehicleDetails.Reset()
                    RPT_TotalFuelUsageByHubPerVehicleDetails.ProcessingMode = ProcessingMode.Local
                    Dim rep As LocalReport = RPT_TotalFuelUsageByHubPerVehicleDetails.LocalReport
                    rep.Refresh()
                    Dim TankReconciliationReportDetailsPath As String = ConfigurationManager.AppSettings("TotalFuelUsageByHubPerVehiclePath")
                    rep.ReportPath = Server.MapPath(TankReconciliationReportDetailsPath)

                    Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
                    Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())
                    Dim CustomerName As ReportParameter = New ReportParameter("CustomerName", Session("CustomerName").ToString())

                    RPT_TotalFuelUsageByHubPerVehicleDetails.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate, CustomerName})

                    Dim rds As ReportDataSource = New ReportDataSource()
                    rds.Name = "TotalFuelUsageByHubPerVehicleDetails"
                    rds.Value = dtTotalFuelUse
                    rep.DataSources.Add(rds)

                    Me.RPT_TotalFuelUsageByHubPerVehicleDetails.LocalReport.DataSources.Add(rds)

                End If
            End If

        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            Response.Redirect("~/Reports/TotalFuelUsageByHubPerVehicle")
        End Try
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/TotalFuelUsageByHubPerVehicle")
    End Sub

End Class