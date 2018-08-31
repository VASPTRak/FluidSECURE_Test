Imports System
Imports System.Web
Imports Owin

Partial Public Class ResetPasswordConfirmation
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If CSCommonHelper.CheckSessionExpired() = False Then
		Else
			Response.Redirect("/home")
        End If

        'Dim ItemMenu As Control = DirectCast(Master.FindControl("ItemMenu"), Control)
        'Dim ReportMenu As Control = DirectCast(Master.FindControl("ReportMenu"), Control)
        'Dim TransactionsMenu As Control = DirectCast(Master.FindControl("TransactionsMenu"), Control)
        'Dim Import As Control = DirectCast(Master.FindControl("Import"), Control)
        'Dim Export As Control = DirectCast(Master.FindControl("Export"), Control)
        'Dim Reconciliation As Control = DirectCast(Master.FindControl("Reconciliation"), Control)
        'Dim LogAction As Control = DirectCast(Master.FindControl("LogAction"), Control)
        'ItemMenu.Visible = False
        'ReportMenu.Visible = False
        'TransactionsMenu.Visible = False
        'Import.Visible = False
        'Export.Visible = False
        'Reconciliation.Visible = False
        'LogAction.Visible = False
    End Sub
End Class