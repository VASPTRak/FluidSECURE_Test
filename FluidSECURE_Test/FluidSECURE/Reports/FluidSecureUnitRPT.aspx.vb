Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class FluidSecureUnitRPT
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FluidSecureUnitRPT))

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

                Dim dtFluidSecureUnit As DataTable = Session("FluidSecureUnitDetails")

                RPT_FluidSecureUnit.Visible = True

                RPT_FluidSecureUnit.Reset()
                RPT_FluidSecureUnit.ProcessingMode = ProcessingMode.Local
                Dim rep As LocalReport = RPT_FluidSecureUnit.LocalReport
                rep.Refresh()
                Dim PersonnelReportPath As String = ConfigurationManager.AppSettings("FluidSecureUnitReportPath")
                rep.ReportPath = Server.MapPath(PersonnelReportPath)

                Dim rds As ReportDataSource = New ReportDataSource()
                rds.Name = "FluidSecureUnitDetails"
                rds.Value = dtFluidSecureUnit
                rep.DataSources.Add(rds)


                Me.RPT_FluidSecureUnit.LocalReport.DataSources.Add(rds)

            End If
        End If

		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/FluidSecureUnitReport")

		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/FluidSecureUnitReport")
    End Sub


End Class