Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class DeptRPT
    Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(DeptRPT))

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

					Dim dtDept As DataTable = Session("DeptDetails")

					RPT_Dept.Visible = True

					RPT_Dept.Reset()
					RPT_Dept.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_Dept.LocalReport
					rep.Refresh()
					Dim PersonnelReportPath As String = ConfigurationManager.AppSettings("DeptReportPath")
					rep.ReportPath = Server.MapPath(PersonnelReportPath)

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "DeptDetails"
					rds.Value = dtDept
					rep.DataSources.Add(rds)

					Me.RPT_Dept.LocalReport.DataSources.Add(rds)

				End If
			End If
		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/DepartmentReport")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/DepartmentReport")
    End Sub

End Class