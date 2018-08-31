Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class VehicleReportRPT
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(VehicleReportRPT))

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

					Dim dtVehicle As DataTable = Session("VehicleDetails")

					RPT_Vehicle.Visible = True

					RPT_Vehicle.Reset()
					RPT_Vehicle.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_Vehicle.LocalReport
					rep.Refresh()
					Dim VehicleReportPath As String = ConfigurationManager.AppSettings("VehicleReportPath")
					rep.ReportPath = Server.MapPath(VehicleReportPath)

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "VehicleDetails"
					rds.Value = dtVehicle
					rep.DataSources.Add(rds)

					Me.RPT_Vehicle.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/VehicleReport")
		End Try
	End Sub

	Protected Sub btnBack_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Reports/VehicleReport")
	End Sub

End Class