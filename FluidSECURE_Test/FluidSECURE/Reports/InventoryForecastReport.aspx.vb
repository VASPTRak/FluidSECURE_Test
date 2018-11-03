Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms
Public Class InventoryForecastReport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(InventoryForecastReport))
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

                    Dim dSTran As DataSet = Session("InventoryForecastReport")

                    RPT_InventoryForecastReport.Visible = True

                    RPT_InventoryForecastReport.Reset()
                    RPT_InventoryForecastReport.ProcessingMode = ProcessingMode.Local
                    Dim rep As LocalReport = RPT_InventoryForecastReport.LocalReport
                    rep.Refresh()
                    Dim TransReportByDateTimePath As String = ConfigurationManager.AppSettings("InventoryForecastPath")
                    rep.ReportPath = Server.MapPath(TransReportByDateTimePath)

                    Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("FromDate").ToString())
                    Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("ToDate").ToString())
                    Dim InactiveVehicleCompanyName As ReportParameter = New ReportParameter("InventoryForecastCompanyName", Session("InventoryForecastCompanyName").ToString())

                    RPT_InventoryForecastReport.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate, InactiveVehicleCompanyName})

                    Dim rds As ReportDataSource = New ReportDataSource()
                    rds.Name = "InventoryForecast"
                    rds.Value = dSTran.Tables(0)
                    rep.DataSources.Add(rds)

                    Me.RPT_InventoryForecastReport.LocalReport.DataSources.Add(rds)


                End If
            End If

        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            Response.Redirect("~/Reports/InventoryForecast")
        End Try
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/InventoryForecast")
    End Sub

End Class