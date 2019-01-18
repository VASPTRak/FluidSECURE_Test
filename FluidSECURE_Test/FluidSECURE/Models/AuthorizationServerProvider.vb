Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Infrastructure
Imports Microsoft.Owin.Security.OAuth
Imports System
Imports System.Collections.Concurrent
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Threading.Tasks
Imports System.Web


Public Class AuthorizationServerProvider
    Inherits OAuthAuthorizationServerProvider
    Public Overrides Async Function ValidateClientAuthentication(ByVal context As OAuthValidateClientAuthenticationContext) As Task
        context.Validated()
    End Function

    Public Overrides Function GrantRefreshToken(ByVal context As OAuthGrantRefreshTokenContext) As Task
        Dim newIdentity = New ClaimsIdentity(context.Ticket.Identity)
        newIdentity.AddClaim(New Claim("newClaim", "newValue"))
        Dim newTicket = New AuthenticationTicket(newIdentity, context.Ticket.Properties)
        context.Validated(newTicket)
        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Overrides Async Function GrantResourceOwnerCredentials(ByVal context As OAuthGrantResourceOwnerCredentialsContext) As Task
        Dim identity = New ClaimsIdentity(context.Options.AuthenticationType)

        Dim result As Integer = CSCommonHelper.CheckExternalLogin(context)
        If result = 1 Then
            'identity.AddClaim(New Claim(ClaimTypes.Role, acc.GetUserRole(context.UserName)))
            identity.AddClaim(New Claim("username", context.UserName))
            identity.AddClaim(New Claim(ClaimTypes.Name, context.UserName))
            context.Validated(identity)
        ElseIf result = -1 Then
            context.SetError("invalid_grant", "Access denied. Please contact administrator.")
            Return
        Else
            context.SetError("invalid_grant", "Provided username and password is incorrect")
            Return
        End If
    End Function
End Class

Public Class RefreshTokenProvider
    Implements IAuthenticationTokenProvider

    Private Shared _refreshTokens As ConcurrentDictionary(Of String, AuthenticationTicket) = New ConcurrentDictionary(Of String, AuthenticationTicket)()

    Public Async Function CreateAsync(ByVal context As AuthenticationTokenCreateContext) As Task

    End Function

    Public Sub Create(ByVal context As AuthenticationTokenCreateContext)
        Throw New NotImplementedException()
    End Sub

    Public Sub Receive(ByVal context As AuthenticationTokenReceiveContext)
        Throw New NotImplementedException()
    End Sub

    Public Async Function ReceiveAsync(ByVal context As AuthenticationTokenReceiveContext) As Task

    End Function

    Private Sub IAuthenticationTokenProvider_Create(context As AuthenticationTokenCreateContext) Implements IAuthenticationTokenProvider.Create
        Throw New NotImplementedException()
    End Sub

    Private Function IAuthenticationTokenProvider_CreateAsync(context As AuthenticationTokenCreateContext) As Task Implements IAuthenticationTokenProvider.CreateAsync
        Dim guids = Guid.NewGuid().ToString()
        Dim refreshTokenProperties = New AuthenticationProperties(context.Ticket.Properties.Dictionary) With {
            .IssuedUtc = context.Ticket.Properties.IssuedUtc,
            .ExpiresUtc = DateTime.UtcNow.AddMinutes(120)
        }
        Dim refreshTokenTicket = New AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties)
        _refreshTokens.TryAdd(guids, refreshTokenTicket)
        context.SetToken(guids)

        Return Task.FromResult(0)

    End Function

    Private Sub IAuthenticationTokenProvider_Receive(context As AuthenticationTokenReceiveContext) Implements IAuthenticationTokenProvider.Receive
        Throw New NotImplementedException()
    End Sub

    Private Function IAuthenticationTokenProvider_ReceiveAsync(context As AuthenticationTokenReceiveContext) As Task Implements IAuthenticationTokenProvider.ReceiveAsync
        Dim ticket As AuthenticationTicket
        Dim header As String = context.OwinContext.Request.Headers("Authorization")

        If _refreshTokens.TryRemove(context.Token, ticket) Then
            context.SetTicket(ticket)
        End If
        Return Task.FromResult(0)
    End Function
End Class
