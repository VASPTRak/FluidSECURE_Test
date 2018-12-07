Imports log4net.Config
Imports log4net
Imports Microsoft.Reporting.WebForms
Imports System.Globalization

Public Class TankBalanceReport
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TankBalanceReport))

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

                    Dim dtTankInv As DataTable = Session("TankBalanceDetails")

					RPT_TankBalanceDetails.Visible = True

					RPT_TankBalanceDetails.Reset()
					RPT_TankBalanceDetails.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_TankBalanceDetails.LocalReport
					rep.Refresh()
					Dim TankReconciliationReportDetailsPath As String = ConfigurationManager.AppSettings("TankBalanceDetailsPath")
                    rep.ReportPath = Server.MapPath(TankReconciliationReportDetailsPath)
                    Dim ASOfDate As ReportParameter = New ReportParameter("ASOfDate", Session("ASOfDate").ToString())
                    RPT_TankBalanceDetails.LocalReport.SetParameters(New ReportParameter() {ASOfDate})

                    Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "TankBalanceDetails"
					rds.Value = dtTankInv
					rep.DataSources.Add(rds)

					Me.RPT_TankBalanceDetails.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/TankBalance")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/TankBalance")
    End Sub

End Class