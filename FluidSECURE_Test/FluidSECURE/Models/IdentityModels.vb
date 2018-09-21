Imports System
Imports System.Threading.Tasks
Imports System.Security.Claims
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin.Security

' You can add profile data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
Public Class ApplicationUser
    Inherits IdentityUser

    'Add new properties  


    Public Property PersonName() As String
        Get
            Return m_PersonName
        End Get
        Set(value As String)
            m_PersonName = value
        End Set
    End Property
    Private m_PersonName As String

    Public Property DepartmentId() As Integer
        Get
            Return m_DepartmentId
        End Get
        Set(value As Integer)
            m_DepartmentId = value
        End Set
    End Property
    Private m_DepartmentId As Integer


    Public Property FuelLimitPerTxn() As Integer?
        Get
            Return m_FuelLimitPerTxn
        End Get
        Set(value As Integer?)
            m_FuelLimitPerTxn = value
        End Set
    End Property
    Private m_FuelLimitPerTxn As Integer?

    Public Property FuelLimitPerDay() As Integer?
        Get
            Return m_FuelLimitPerDay
        End Get
        Set(value As Integer?)
            m_FuelLimitPerDay = value
        End Set
    End Property
    Private m_FuelLimitPerDay As Integer?

    Public Property PreAuth() As Integer?
        Get
            Return m_PreAuth
        End Get
        Set(value As Integer?)
            m_PreAuth = value
        End Set
    End Property
    Private m_PreAuth As Integer?


    Public Property SoftUpdate() As String
        Get
            Return m_SoftUpdateh
        End Get
        Set(value As String)
            m_SoftUpdateh = value
        End Set
    End Property
    Private m_SoftUpdateh As String

    Public Property CreatedDate() As DateTime
        Get
            Return m_CreatedDate
        End Get
        Set(value As DateTime)
            m_CreatedDate = value
        End Set
    End Property
    Private m_CreatedDate As Date

    Public Property CreatedBy() As Integer
        Get
            Return m_CreatedBy
        End Get
        Set(value As Integer)
            m_CreatedBy = value
        End Set
    End Property
    Private m_CreatedBy As Integer

    Public Property IsDeleted() As Boolean
        Get
            Return m_IsDeleted
        End Get
        Set(value As Boolean)
            m_IsDeleted = value
        End Set
    End Property
    Private m_IsDeleted As Boolean

    Public Property DeletedDate() As DateTime
        Get
            Return m_DeletedDate
        End Get
        Set(value As DateTime)
            m_DeletedDate = value
        End Set
    End Property
    Private m_DeletedDate As DateTime
    Public Property DeletedBy() As Integer
        Get
            Return m_DeletedBy
        End Get
        Set(value As Integer)
            m_DeletedBy = value
        End Set
    End Property
    Private m_DeletedBy As Integer

    Public Property LastModifiedDate() As DateTime
        Get
            Return m_LastModifiedDate
        End Get
        Set(value As DateTime)
            m_LastModifiedDate = value
        End Set
    End Property
    Private m_LastModifiedDate As DateTime

    Public Property LastModifiedBy() As Integer
        Get
            Return m_LastModifiedBy
        End Get
        Set(value As Integer)
            m_LastModifiedBy = value
        End Set
    End Property
    Private m_LastModifiedBy As Integer

    Public Property IsApproved() As Boolean
        Get
            Return m_IsApproved
        End Get
        Set(value As Boolean)
            m_IsApproved = value
        End Set
    End Property
    Private m_IsApproved As Boolean?


    Public Property RequestFrom() As String
        Get
            Return m_RequestFrom
        End Get
        Set(value As String)
            m_RequestFrom = value
        End Set
    End Property
    Private m_RequestFrom As String


    Public Property IMEI_UDID() As String
        Get
            Return m_IMEI_UDID
        End Get
        Set(value As String)
            m_IMEI_UDID = value
        End Set
    End Property
    Private m_IMEI_UDID As String

    Public Property RoleId() As String
        Get
            Return m_RoleId
        End Get
        Set(value As String)
            m_RoleId = value
        End Set
    End Property
    Private m_RoleId As String


    Public Property ApprovedOn() As DateTime
        Get
            Return m_ApprovedOn
        End Get
        Set(value As DateTime)
            m_ApprovedOn = value
        End Set
    End Property
    Private m_ApprovedOn As DateTime?


    Public Property ApprovedBy() As Integer
        Get
            Return m_ApprovedBy
        End Get
        Set(value As Integer)
            m_ApprovedBy = value
        End Set
    End Property
    Private m_ApprovedBy As Integer

    Public Property PinNumber() As String
        Get
            Return m_PinNumber
        End Get
        Set(value As String)
            m_PinNumber = value
        End Set
    End Property
    Private m_PinNumber As String

    Public Property CustomerId() As Integer
        Get
            Return m_CustomerId
        End Get
        Set(value As Integer)
            m_CustomerId = value
        End Set
    End Property
    Private m_CustomerId As Integer

    Public Property IsMainCustomerAdmin() As Boolean
        Get
            Return m_IsMainCustomerAdmin
        End Get
        Set(value As Boolean)
            m_IsMainCustomerAdmin = value
        End Set
    End Property
    Private m_IsMainCustomerAdmin As Boolean

    Public Property ExportCode() As String
        Get
            Return m_ExportCode
        End Get
        Set(value As String)
            m_ExportCode = value
        End Set
    End Property
    Private m_ExportCode As String

    Public Property SendTransactionEmail() As Boolean
        Get
            Return m_SendTransactionEmail
        End Get
        Set(value As Boolean)
            m_SendTransactionEmail = value
        End Set
    End Property

    Private m_SendTransactionEmail As Boolean

    Public Property IsFluidSecureHub() As Boolean
        Get
            Return m_IsFluidSecureHub
        End Get
        Set(value As Boolean)
            m_IsFluidSecureHub = value
        End Set
    End Property

    Private m_IsFluidSecureHub As Boolean

    Public Property IsUserForHub() As Boolean
        Get
            Return m_IsUserForHub
        End Get
        Set(value As Boolean)
            m_IsUserForHub = value
        End Set
    End Property

    Private m_IsUserForHub As Boolean

    'New field for Adding Reset password date
    Public Property PasswordResetDate() As DateTime
        Get
            Return m_PasswordResetDate
        End Get
        Set(value As DateTime)
            m_PasswordResetDate = value
        End Set
    End Property

    Private m_PasswordResetDate As DateTime?

    Public Property FOBNumber() As String
        Get
            Return m_FOBNumber
        End Get
        Set(value As String)
            m_FOBNumber = value
        End Set
    End Property

    Private m_FOBNumber As String

    Public Property AdditionalEmailId() As String
        Get
            Return m_AdditionalEmailId
        End Get
        Set(value As String)
            m_AdditionalEmailId = value
        End Set
    End Property

    Private m_AdditionalEmailId As String

    Public Property IsPersonnelPINRequire() As Boolean?
        Get
            Return m_IsPersonnelPINRequire
        End Get
        Set(value As Boolean?)
            m_IsPersonnelPINRequire = value
        End Set
    End Property

    Private m_IsPersonnelPINRequire As Boolean?

    Public Property BluetoothCardReader() As String
        Get
            Return m_BluetoothCardReader
        End Get
        Set(value As String)
            m_BluetoothCardReader = value
        End Set
    End Property

    Private m_BluetoothCardReader As String

    Public Property PrinterName() As String
        Get
            Return m_PrinterName
        End Get
        Set(value As String)
            m_PrinterName = value
        End Set
    End Property

    Private m_PrinterName As String

    Public Property PrinterMacAddress() As String
        Get
            Return m_PrinterMacAddress
        End Get
        Set(value As String)
            m_PrinterMacAddress = value
        End Set
    End Property

    Private m_PrinterMacAddress As String

    Public Property HubSiteName() As String
        Get
            Return m_HubSiteName
        End Get
        Set(value As String)
            m_HubSiteName = value
        End Set
    End Property

    Private m_HubSiteName As String

    Public Property BluetoothCardReaderMacAddress() As String
        Get
            Return m_BluetoothCardReaderMacAddress
        End Get
        Set(value As String)
            m_BluetoothCardReaderMacAddress = value
        End Set
    End Property

    Private m_BluetoothCardReaderMacAddress As String

    Public Property LFBluetoothCardReader() As String
        Get
            Return m_LFBluetoothCardReader
        End Get
        Set(value As String)
            m_LFBluetoothCardReader = value
        End Set
    End Property

    Private m_LFBluetoothCardReader As String

    Public Property LFBluetoothCardReaderMacAddress() As String
        Get
            Return m_LFBluetoothCardReaderMacAddress
        End Get
        Set(value As String)
            m_LFBluetoothCardReaderMacAddress = value
        End Set
    End Property

    Private m_LFBluetoothCardReaderMacAddress As String

    Public Property VeederRootMacAddress() As String
        Get
            Return m_VeederRootMacAddress
        End Get
        Set(value As String)
            m_VeederRootMacAddress = value
        End Set
    End Property

    Private m_VeederRootMacAddress As String

    Public Property CollectDiagnosticLogs() As Boolean?
        Get
            Return m_CollectDiagnosticLogs
        End Get
        Set(value As Boolean?)
            m_CollectDiagnosticLogs = value
        End Set
    End Property

    Private m_CollectDiagnosticLogs As Boolean?
    Public Property IsVehicleHasFob() As Boolean?
        Get
            Return m_IsVehicleHasFob
        End Get
        Set(value As Boolean?)
            m_IsVehicleHasFob = value
        End Set
    End Property

    Private m_IsVehicleHasFob As Boolean?

    Public Property IsPersonHasFob() As Boolean?
        Get
            Return m_IsPersonHasFob
        End Get
        Set(value As Boolean?)
            m_IsPersonHasFob = value
        End Set
    End Property

    Private m_IsPersonHasFob As Boolean?


    Public Property IsTermConditionAgreed() As Boolean?
        Get
            Return m_IsTermConditionAgreed
        End Get
        Set(value As Boolean?)
            m_IsTermConditionAgreed = value
        End Set
    End Property

    Private m_IsTermConditionAgreed As Boolean?

    Public Property DateTimeTermConditionAccepted() As DateTime?
        Get
            Return m_DateTimeTermConditionAccepted
        End Get
        Set(value As DateTime?)
            m_DateTimeTermConditionAccepted = value
        End Set
    End Property

    Private m_DateTimeTermConditionAccepted As DateTime?

    Public Property IsGateHub() As Boolean?
        Get
            Return m_IsGateHub
        End Get
        Set(value As Boolean?)
            m_IsGateHub = value
        End Set
    End Property

	Private m_IsGateHub As Boolean?

	Public Property IsVehicleNumberRequire() As Boolean?
		Get
			Return m_IsVehicleNumberRequire
		End Get
		Set(value As Boolean?)
			m_IsVehicleNumberRequire = value
		End Set
	End Property

    Private m_IsVehicleNumberRequire As Boolean?
    Public Property HubAddress() As String
        Get
            Return m_HubAddress
        End Get
        Set(value As String)
            m_HubAddress = value
        End Set
    End Property

    Private m_HubAddress As String
    'End Add new properties

    Public Function GenerateUserIdentity(manager As ApplicationUserManager) As ClaimsIdentity
        ' Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        Dim userIdentity = manager.CreateIdentity(Me, DefaultAuthenticationTypes.ApplicationCookie)
        ' Add custom user claims here
        Return userIdentity
    End Function

    Public Function GenerateUserIdentityAsync(manager As ApplicationUserManager) As Task(Of ClaimsIdentity)
        Return Task.FromResult(GenerateUserIdentity(manager))
    End Function
End Class

'Public Class ApplicationRole
'    Inherits IdentityRole

'    Public Sub New()
'        MyBase.New()
'    End Sub

'    Public Sub New(ByVal name As String, ByVal DisplayName As String)
'        MyBase.New(name)
'        Me.DisplayName = DisplayName
'    End Sub

'    Public Overridable Property DisplayName As String
'End Class
'Public Class ApplicationUserRole
'    Inherits IdentityUserRole

'    Public Sub New()
'        MyBase.New()
'    End Sub

'    Public Property Role As ApplicationRole
'End Class


'{
'    Public ApplicationRole() :  base() { }

'    Public ApplicationRole(String name, string description)
'         base(name)
'    {
'        this.Description = description;
'    }

'    Public virtual String Description { Get; Set; }
'}

Public Class ApplicationDbContext
    Inherits IdentityDbContext(Of ApplicationUser)
    Public Sub New()
        MyBase.New("FluidSecureConnectionString", throwIfV1Schema:=False)
    End Sub

    Public Shared Function Create() As ApplicationDbContext
        Return New ApplicationDbContext()
    End Function
End Class

#Region "Helpers"
Public Class IdentityHelper
    'Used for XSRF when linking external logins
    Public Const XsrfKey As String = "xsrfKey"

    Public Shared Sub SignIn(manager As ApplicationUserManager, user As ApplicationUser, isPersistent As Boolean)
        Dim authenticationManager As IAuthenticationManager = HttpContext.Current.GetOwinContext().Authentication
        authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie)
        Dim identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie)
        authenticationManager.SignIn(New AuthenticationProperties() With {.IsPersistent = isPersistent}, identity)
    End Sub

    Public Const ProviderNameKey As String = "providerName"
    Public Shared Function GetProviderNameFromRequest(request As HttpRequest) As String
        Return request.QueryString(ProviderNameKey)
    End Function

    Public Const CodeKey As String = "code"
    Public Shared Function GetCodeFromRequest(request As HttpRequest) As String
        Return request.QueryString(CodeKey)
    End Function

    Public Const UserIdKey As String = "userId"
    Public Shared Function GetUserIdFromRequest(request As HttpRequest) As String
        Return HttpUtility.UrlDecode(request.QueryString(UserIdKey))
    End Function

    Public Shared Function GetResetPasswordRedirectUrl(code As String, request As HttpRequest) As String
        Dim absoluteUri = "/Account/ResetPassword?" + CodeKey + "=" + HttpUtility.UrlEncode(code)
        Return New Uri(request.Url, absoluteUri).AbsoluteUri.ToString()
    End Function

    Public Shared Function GetUserConfirmationRedirectUrl(code As String, userId As String, request As HttpRequest) As String
        Dim absoluteUri = "/Account/Confirm?" + CodeKey + "=" + HttpUtility.UrlEncode(code) + "&" + UserIdKey + "=" + HttpUtility.UrlEncode(userId)
        Return New Uri(request.Url, absoluteUri).AbsoluteUri.ToString()
    End Function

    Private Shared Function IsLocalUrl(url As String) As Boolean
        Return Not String.IsNullOrEmpty(url) AndAlso ((url(0) = "/"c AndAlso (url.Length = 1 OrElse (url(1) <> "/"c AndAlso url(1) <> "\"c))) OrElse (url.Length > 1 AndAlso url(0) = "~"c AndAlso url(1) = "/"c))
    End Function

    Public Shared Sub RedirectToReturnUrl(returnUrl As String, response As HttpResponse)
        If Not [String].IsNullOrEmpty(returnUrl) AndAlso IsLocalUrl(returnUrl) Then
            response.Redirect(returnUrl)
        Else
            response.Redirect("~/Account/login")
        End If
    End Sub
End Class
#End Region