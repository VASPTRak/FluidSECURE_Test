Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class FluidSecureHubRPT
    Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FluidSecureHubRPT))

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

					Dim dtPersonnel As DataTable = Session("PersonnelDetails")

					RPT_Personnel.Visible = True

					RPT_Personnel.Reset()
					RPT_Personnel.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_Personnel.LocalReport
					rep.Refresh()
					Dim PersonnelReportPath As String = ConfigurationManager.AppSettings("FluidSecureHubPath")
					rep.ReportPath = Server.MapPath(PersonnelReportPath)

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "PersonnelDetails"
					rds.Value = dtPersonnel
					rep.DataSources.Add(rds)

					Me.RPT_Personnel.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/FluidSecureHubReport")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/FluidSecureHubReport")
    End Sub

End Class