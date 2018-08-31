Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class CompanyHostingDateRpt
    Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CompanyHostingDateRpt))

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

					Dim dtCompany As DataTable = Session("CompanyHostingDateDetails")

					RPT_Company.Visible = True

					RPT_Company.Reset()
					RPT_Company.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_Company.LocalReport
					rep.Refresh()
					Dim CompanyHostingDatePath As String = ConfigurationManager.AppSettings("CompanyHostingDatePath")
					rep.ReportPath = Server.MapPath(CompanyHostingDatePath)

					Dim FromDate As ReportParameter = New ReportParameter("FromDate", Session("BeginningHostingDate").ToString())
					Dim ToDate As ReportParameter = New ReportParameter("ToDate", Session("EndingHostingDate").ToString())

					RPT_Company.LocalReport.SetParameters(New ReportParameter() {FromDate, ToDate})

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "CompanyHostingDateDetails"
					rds.Value = dtCompany
					rep.DataSources.Add(rds)

					Me.RPT_Company.LocalReport.DataSources.Add(rds)

				End If
			End If

		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/CompanyHostingDateReport")
		End Try
	End Sub

	Protected Sub btnBack_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Reports/CompanyHostingDateReport")
	End Sub

End Class