Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class ShipmentRPT
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(VehicleReportRPT))

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try
			XmlConfigurator.Configure()
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				Response.Redirect("/Account/Login")
			ElseIf Session("RoleName") <> "SuperAdmin" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If (Not IsPostBack) Then

					Dim dtShipment As DataTable = Session("ShipmentDetails")

					RPT_Shipment.Visible = True

					RPT_Shipment.Reset()
					RPT_Shipment.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_Shipment.LocalReport
					rep.Refresh()
					Dim VehicleReportPath As String = ConfigurationManager.AppSettings("ShipmentReportPath")
					rep.ReportPath = Server.MapPath(VehicleReportPath)

                    Dim ShipmentForLinkOrHub As ReportParameter = New ReportParameter("ShipmentForLinkOrHub", Session("ShipmentForLinkOrHub").ToString())
                    Dim Company As ReportParameter = New ReportParameter("Company", Session("Company").ToString())

                    RPT_Shipment.LocalReport.SetParameters(New ReportParameter() {ShipmentForLinkOrHub, Company})

                    Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "ShipmentsDetails"
					rds.Value = dtShipment
					rep.DataSources.Add(rds)

					Me.RPT_Shipment.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/ShipmentDetailReport")
		End Try
	End Sub

	Protected Sub btnBack_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Reports/ShipmentDetailReport")
	End Sub

End Class