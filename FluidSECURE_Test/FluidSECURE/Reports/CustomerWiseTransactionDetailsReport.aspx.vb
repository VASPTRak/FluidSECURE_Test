Imports log4net
Imports log4net.Config
Imports Microsoft.Reporting.WebForms

Public Class CustomerWiseTransactionDetailsReport
    Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CustomerWiseTransactionDetailsReport))

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

					Dim dSTran As DataSet = Session("CustomerWiseTransactionDetails")

					RPT_CustomerWiseTransactionDetails.Visible = True

					RPT_CustomerWiseTransactionDetails.Reset()
					RPT_CustomerWiseTransactionDetails.ProcessingMode = ProcessingMode.Local
					Dim rep As LocalReport = RPT_CustomerWiseTransactionDetails.LocalReport
					rep.Refresh()
					Dim BillingReportByDeptDetailsPath As String = ConfigurationManager.AppSettings("CustomerWiseTransactionDetailsPath")
					rep.ReportPath = Server.MapPath(BillingReportByDeptDetailsPath)

					Dim rds As ReportDataSource = New ReportDataSource()
					rds.Name = "CustomerWiseTransactionDetails"
					rds.Value = dSTran.Tables(0)
					rep.DataSources.Add(rds)

					Me.RPT_CustomerWiseTransactionDetails.LocalReport.DataSources.Add(rds)

				End If
			End If
		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			Response.Redirect("~/Reports/CustomerWiseTransactionDetails.aspx")
		End Try
	End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Reports/CustomerWiseTransactionDetails.aspx")
    End Sub

End Class