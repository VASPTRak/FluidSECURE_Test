Imports Newtonsoft.Json
Imports System.Net
Imports System.Net.Http
Imports System.Web

Public Class AuthorizeAttribute
    Inherits System.Web.Http.AuthorizeAttribute
    Protected Overrides Sub HandleUnauthorizedRequest(ByVal actionContext As System.Web.Http.Controllers.HttpActionContext)
        If Not HttpContext.Current.User.Identity.IsAuthenticated Then
            'MyBase.HandleUnauthorizedRequest(actionContext)
            actionContext.Response = New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized) With {
              .Content = New StringContent(JsonConvert.SerializeObject(New With {.Message = "Authorization failed."
          })),
          .StatusCode = HttpStatusCode.Forbidden
            }
        Else
            actionContext.Response = New HttpResponseMessage(System.Net.HttpStatusCode.Forbidden) With {
                .Content = New StringContent(JsonConvert.SerializeObject(New With {.Message = "Authorization failed."
            })),
            .StatusCode = HttpStatusCode.Forbidden
        }
        End If
    End Sub

End Class
