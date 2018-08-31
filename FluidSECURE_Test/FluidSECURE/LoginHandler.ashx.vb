Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports System.Web.Script.Serialization
Imports log4net
Imports System.Resources

Public Class LoginHandler
	Implements System.Web.IHttpHandler
	Public Shared ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
	Dim OBJMaster As WebServiceBAL = New WebServiceBAL()
	Dim OBJMasterBAL As MasterBAL = New MasterBAL()
	Dim resourceManager As ResourceManager

	Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
		log4net.Config.XmlConfigurator.Configure()
		Try
			Dim headers = context.Request.Headers
			Dim encoding As Encoding = Encoding.UTF8
			Dim credentials As String = encoding.GetString(Convert.FromBase64String(headers.GetValues("Login").ToList()(0).ToString().Replace("Basic ", "").Trim()))
			Dim parts As String() = credentials.Split(":")
			Dim Imei As String = parts(0).Trim()
			Dim Email As String = parts(1).Trim()
			Dim Password As String = parts(2).Trim()
			Dim langCode As String = ""

			If (parts.Length > 3) Then
				langCode = parts(3).Trim()
			End If

			If (langCode Is Nothing Or langCode = "") Then
				langCode = "en-US"
			End If

			System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(langCode)
			System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(langCode)

			resourceManager = New System.Resources.ResourceManager("Fuel_Secure.Resource", Reflection.Assembly.GetExecutingAssembly())

			AuthonticateUser(context, Imei, Email, Password)
		Catch ex As Exception
			log.Debug("ProcessRequest:" + ex.Message)
			'GenerateResponse(context, "fail", System.Net.HttpStatusCode.BadRequest.ToString() + " You might be using old version. Kindly install latest version.")
			GenerateResponse(context, "fail", resourceManager.GetString("HandlerMsg15"))
		End Try
	End Sub

	Private Sub AuthonticateUser(context As HttpContext, Imei As String, Email As String, Password As String)
		Try
			If Imei = "" Or Imei = Nothing Then

				log.Error("AuthonticateUser: Invalid IMEI. IMEI :" + Imei)

				GenerateResponse(context, "fail", resourceManager.GetString("HandlerMsg52"))

				Return

			End If

			Dim manager = context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
			Dim signinManager = context.GetOwinContext().GetUserManager(Of ApplicationSignInManager)()

			'Dim result = signinManager.PasswordSignIn(Email, Password, False, shouldLockout:=False)
			Dim UserObj As ApplicationUser = manager.Find(Email, Password)
			If Not UserObj Is Nothing Then
				If UserObj.IsDeleted = False And UserObj.IsApproved = True Then
					Dim ImeiNumbers As String() = UserObj.IMEI_UDID.Split(",")

					If Not Array.Exists(ImeiNumbers, Function(element) element = Imei) Then

						GenerateResponse(context, "fail", resourceManager.GetString("HandlerMsg53"))

						Return

					End If

					GenerateResponse(context, "success", resourceManager.GetString("HandlerMsg54"))

				Else
					GenerateResponse(context, "fail", resourceManager.GetString("HandlerMsg55"))
				End If
			Else
				log.Debug("AuthonticateUser: User not found.")
				GenerateResponse(context, "fail", resourceManager.GetString("HandlerMsg56"))
			End If
		Catch ex As Exception
			log.Debug("AuthonticateUser:" + ex.Message)
			GenerateResponse(context, "fail", resourceManager.GetString("HandlerMsg15"))
		End Try
	End Sub


	Private Sub GenerateResponse(context As HttpContext, ResponceMessage As String, errorString As String)
		Dim javaScriptSerializer = New JavaScriptSerializer()
		Dim rootOject = New RootObject()
		rootOject.ResponceMessage = ResponceMessage
		rootOject.ResponceText = errorString
		Dim json As String
		json = javaScriptSerializer.Serialize(rootOject)
		context.Response.Write(json)
	End Sub

	ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
		Get
			Return False
		End Get
	End Property

End Class