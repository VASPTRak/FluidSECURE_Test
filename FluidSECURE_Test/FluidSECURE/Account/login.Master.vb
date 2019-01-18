Public Class login1
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim domain As String = HttpContext.Current.Request.Url.Authority
            If domain = "fluidsecure.cloudapp.net" Then
                Response.Redirect("~/SiteRedirect.html")
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class