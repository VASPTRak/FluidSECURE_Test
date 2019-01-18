Imports System.Web.Http

Public Class WebApiConfig

    Public Shared Sub Register(ByVal config As HttpConfiguration)
        'Web API configuration And services

        ' Web API routes
        config.Routes.MapHttpRoute(name:="DefaultApi", routeTemplate:="api/{controller}/{action}/{id}", defaults:=New With {.id = RouteParameter.Optional})
    End Sub

End Class
