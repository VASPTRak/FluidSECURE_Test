Public Class _Error
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If (Not IsPostBack) Then
			Session.Clear()
		End If
	End Sub

	Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Account/login.aspx")
	End Sub
End Class