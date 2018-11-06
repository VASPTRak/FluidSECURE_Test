
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin

Imports System.IO
Imports System.Web.Script.Serialization
Imports log4net
Imports System.Net.Mail
Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json.Linq
Imports System.Resources

Public Class HandlerTrak
    Implements System.Web.IHttpHandler
    Public Shared ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Dim OBJServiceBAL As WebServiceBAL = New WebServiceBAL()
    Dim OBJMasterBAL As MasterBAL = New MasterBAL()
    Dim steps As String = "0"
    Shared IPAddress As String
    Dim resourceManager As ResourceManager

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        log4net.Config.XmlConfigurator.Configure()
        Try
            Dim userAgent = context.Request.UserAgent
            If (userAgent Is Nothing) Then
                userAgent = ""
            End If

            'Dim UrlReferrer = IIf(context.Request.UrlReferrer Is Nothing, "", context.Request.UrlReferrer.ToString())

            Dim Method = context.Request.HttpMethod
            If (Method Is Nothing) Then
                Method = ""
            End If

            IPAddress = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If IPAddress = "" Or IPAddress Is Nothing Then
                IPAddress = context.Request.ServerVariables("REMOTE_ADDR")
            End If
            If (IPAddress Is Nothing) Then
                IPAddress = ""
            End If
            'Dim provider As IServiceProvider = CType(context, IServiceProvider)

            'Dim worker As HttpWorkerRequest = CType(provider.GetService(GetType(HttpWorkerRequest)), HttpWorkerRequest)

            'Dim referer As String = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderReferer)

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Web Service Request", "Web Service Request", "", "userAgent = " & userAgent.ToString().Replace(",", " ") & " ; Referrer = " & "" &
                                        " ; Method = " & Method.ToString().Replace(",", " "), "", IPAddress, "sucsess", "")
            End If

            log.Debug("Start01")
            Dim headers = context.Request.Headers

            Dim encoding As Encoding = Encoding.UTF8
            Dim credentials As String = encoding.GetString(Convert.FromBase64String(headers.GetValues("Authorization").ToList()(0).ToString().Replace("Basic ", "").Trim()))
            log.Debug("credentials : " & credentials)
            Dim parts As String() = credentials.Split(":")
            Dim Imei As String = parts(0).Trim()
            Dim Email As String = parts(1).Trim()
            Dim Route As String = parts(2).Trim()

            Dim langCode As String = ""

            If (parts.Length > 3) Then
                Dim temp As String = parts(3).Trim()
                If (temp <> "en-US" And temp <> "es-ES") Then
                    Dim tempArray As String() = temp.Split(";")
                    If (tempArray.Length > 1) Then
                        langCode = tempArray(1)
                    Else
                        langCode = "en-US"
                    End If
                Else
                    langCode = parts(3).Trim()
                End If

            End If

            If (langCode Is Nothing Or langCode = "") Then
                langCode = "en-US"
            End If

            log.Info("langCode : " & langCode)

            System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(langCode)
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(langCode)

            resourceManager = New System.Resources.ResourceManager("Fuel_Secure.Resource", Reflection.Assembly.GetExecutingAssembly())

            Dim dsCompanyActive = New DataSet()
            dsCompanyActive = OBJServiceBAL.IsIMEIExists(Imei)


            If Not dsCompanyActive Is Nothing Then
                If dsCompanyActive.Tables.Count > 0 And dsCompanyActive.Tables(0).Rows.Count > 0 Then
                    If dsCompanyActive.Tables(0).Rows(0)("CustomerActiveInactive") = "No" Then
                        log.Error("Your Company is not  ACTIVE. Please contact your Administrator for more information.")
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg58"))
                        Return
                    End If
                End If
            End If

            CheckBusyStatusOfAllFluidSecureUnits()

            'log.Error(Route)
            If Route = "Register" Then

                Dim data1 = [String].Empty
                Using inputStream1 = New StreamReader(context.Request.InputStream)
                    data1 = inputStream1.ReadToEnd()
                    log.Debug("Register : " & data1)
                End Using
                'log.Debug("Register:" + data1)
                Dim RegParts As String() = Regex.Split(data1, "\#\:\#")

                Dim Name As String = RegParts(0).Trim()
                Dim Mobile As String = RegParts(1).Trim()
                Dim Emailid As String = RegParts(2).Trim()
                Dim IMEI_UDID As String = RegParts(3).Trim()
                Dim ReqFrom As String = RegParts(4).Trim()
                Dim CompanyName As String = RegParts(5).Trim()
                Dim RequestFromAPP As String = ""

                If (RegParts.Length > 6) Then
                    RequestFromAPP = RegParts(6).Trim()
                End If

                If (RequestFromAPP = "AP") Then
                    OBJMasterBAL = New MasterBAL()
                    log.Info("Name : " & Name & " : IMEI_UDID : " & IMEI_UDID)
                    Dim result As Integer = OBJMasterBAL.UpdateIMEI_UDIDFromHubName(Name, IMEI_UDID)
                    If (result = -1) Then
                        log.Error("Not registered, Hubname not found. Hubname : " & Name & ", IMEI_UDID :" & IMEI_UDID)
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg1"))
                    ElseIf (result = -2) Then
                        log.Error("Not registered, Other Phone numbers are registered to same HUB. Hubname : " & Name & ",IMEI_UDID :" & IMEI_UDID)
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg2"))
                    ElseIf (result = 1) Then
                        log.Info("Registration successful. Hubname : " & Name & ",IMEI_UDID :" & IMEI_UDID)
                        ErrorInAuthontication(context, "success", resourceManager.GetString("HandlerMsg3"))
                    Else
                        log.Error("Not registered, Not registered, Error occured while registering HUB.")
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg4"))
                    End If
                End If
                'company here

                'Check if imei already exists------------------------------
                Dim ds = New DataSet()
                ds = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                If Not ds Is Nothing Then
                    If ds.Tables.Count > 0 And ds.Tables(0).Rows.Count > 0 Then
                        'context.Response.Write(CreateResponse("409", "exists", "IMEI already exists", ""))
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg5"))
                    Else
                        'Check if Email alredy exists------------------------------
                        Dim dsEmail = New DataSet()
                        dsEmail = OBJServiceBAL.IsEmailExists(Emailid)

                        If Not dsEmail Is Nothing Then
                            If dsEmail.Tables.Count > 0 And dsEmail.Tables(0).Rows.Count > 0 Then
                                'update- concate imei by email in table-----------------------------------
                                Dim row As DataRow = dsEmail.Tables(0).Rows(0)
                                Dim em1 As String = row("Email")
                                Dim im1 As String = row("IMEI_UDID")
                                log.Debug("IMEI_UDID" & row("IMEI_UDID"))
                                If (RequestFromAPP = "AP") Then
                                    Dim IMEIList = im1.Split(",")
                                    log.Debug("IMEIList.Length " & IMEIList.Length)
                                    If IMEIList.Length > 0 And im1 <> "" Then
                                        log.Error("Not registered, Other Phone numbers are registered to same email address. IMEI : " & im1)
                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg6"))
                                    Else
                                        im1 = im1 + "," + IMEI_UDID
                                        im1 = im1.TrimStart(",")
                                        log.Info("1==>em1 : " & em1 & "im1 : " & im1 & "IMEI_UDID : " & IMEI_UDID)
                                        Dim result As Integer
                                        result = OBJServiceBAL.UpdateIMEIbyEmail(em1, im1, IMEI_UDID)
                                        If result = 0 Then
                                            'context.Response.Write(CreateResponse("200", "success", "Registration Successfull", ""))
                                            log.Info("Registration successful. IMEI : " & im1 & " , Email : " & em1)
                                            ErrorInAuthontication(context, "success", resourceManager.GetString("HandlerMsg3"))
                                        Else
                                            'context.Response.Write(CreateResponse("401", "fail", "Registration fail", ""))
                                            log.Info("Registration fail. IMEI : " & im1 & " , Email : " & em1)
                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg7"))
                                        End If
                                    End If

                                Else

                                    Dim IMEIList = im1.Split(",")
                                    If IMEIList.Length > 20 Then
                                        log.Error("Not registered, 21 phone numbers are registered to same email address. IMEI : " & im1)
                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg8"))
                                    Else
                                        im1 = im1 + "," + IMEI_UDID
                                        im1 = im1.TrimStart(",")
                                        Dim result As Integer
                                        log.Info("em1 : " & em1 & "im1 : " & im1 & "IMEI_UDID : " & IMEI_UDID)
                                        result = OBJServiceBAL.UpdateIMEIbyEmail(em1, im1, IMEI_UDID)
                                        If result = 0 Then

                                            OBJServiceBAL = New WebServiceBAL()
                                            Dim emailIds As String = OBJServiceBAL.GetCustomerAdminsByCustomerId(dsEmail.Tables(0).Rows(0)("CustomerId").ToString())

                                            SendRegistrationEmailToCustomerAdmins(emailIds, dsEmail.Tables(0).Rows(0)("PersonName").ToString(), context, True)

                                            'context.Response.Write(CreateResponse("200", "success", "Registration Successfull", ""))
                                            log.Info("Registration successful. IMEI : " & im1 & " , Email : " & em1)
                                            ErrorInAuthontication(context, "success", resourceManager.GetString("HandlerMsg3"))
                                        Else
                                            'context.Response.Write(CreateResponse("401", "fail", "Registration fail", ""))
                                            log.Info("Registration fail. IMEI : " & im1 & " , Email : " & em1)
                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg7"))
                                        End If
                                    End If

                                End If

                            Else
                                If Name = "" Then
                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg9"))
                                Else
                                    'insert values in table-----------------------------------
                                    'check company exists
                                    Dim dtCompany As DataTable
                                    dtCompany = OBJMasterBAL.GetCustomerDetailsByName(CompanyName)
                                    If Not dtCompany Is Nothing And dtCompany.Rows.Count > 0 Then
                                        Dim companyId As Integer
                                        companyId = Convert.ToInt32(dtCompany.Rows(0)("CustomerId").ToString())
                                        If (RequestFromAPP = "AP") Then
                                            funRegisterUser(context, Name, Mobile, Emailid, IMEI_UDID, ReqFrom, companyId, True)
                                        Else
                                            funRegisterUser(context, Name, Mobile, Emailid, IMEI_UDID, ReqFrom, companyId, False)
                                        End If

                                    Else
                                        log.Error("Invalid company name. Please re-enter the company name Or contact administrator. Company :  " & CompanyName)

                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg10"))
                                    End If

                                End If

                            End If
                        End If
                    End If
                End If

            ElseIf Route = "Other" Then
                steps = "1"
                'Check if imei alredy exists------------------------------
                Dim ds = New DataSet()
                ds = OBJServiceBAL.IsIMEIExists(Imei)
                If Not ds Is Nothing Then
                    If ds.Tables.Count > 0 And ds.Tables(0).Rows.Count > 0 Then
                        steps = "2"
                        Dim s As String = ""
                        Dim userObj = New UserData()
                        If Authenticate(Imei, Email, userObj) Then
                            Dim data2 = [String].Empty
                            Dim data3 As String = ""
                            Dim FromWhere As String = ""

                            context.Request.InputStream.Position = 0
                            Using inputStream2 = New StreamReader(context.Request.InputStream)
                                data3 = inputStream2.ReadToEnd()
                                log.Debug("Other :" & data3)
                            End Using

                            data2 = data3.Split(":")(0)
                            FromWhere = data3.Split(":")(1)
                            steps = "3"
                            If data2 = "Authenticate" Then
                                steps = "4"
                                'context.Response.Write(CreateResponse("200", "success", System.Net.HttpStatusCode.OK.ToString(), s))
                                'ErrorInAuthontication(context, "success", System.Net.HttpStatusCode.OK.ToString())
                                If (FromWhere = "A") Then

                                    CreateResponceForAndroidSSID(context, "success", resourceManager.GetString("HandlerMsg3"), userObj)

                                ElseIf (FromWhere = "I") Then
                                    steps = "5"
                                    'CreateResponceForAndroidSSID(context, "success", "Registration successful", userObj)

                                    GetSSIDForIphone(data3.Split(":")(2), Imei, context, userObj)

                                End If
                            End If

                            'If data2 = "" Then

                            'End If
                        Else
                            'context.Response.Write(CreateResponse("401", "notapproved", System.Net.HttpStatusCode.Unauthorized.ToString(), ""))
                            ErrorInAuthontication(context, "fail", "notapproved")
                        End If
                    Else
                        'context.Response.Write(CreateResponse("123", "newuser", "New Registration", ""))
                        ErrorInAuthontication(context, "fail", "New Registration")
                    End If
                Else
                    ErrorInAuthontication(context, "fail", "New Registration")
                End If
            ElseIf Route = "GetSSID" Then

                ''Get wifi ssid from Lat lng------------------
                'Dim data2 = [String].Empty
                'context.Request.InputStream.Position = 0
                'Using inputStream2 = New StreamReader(context.Request.InputStream)
                '    data2 = inputStream2.ReadToEnd()

                'End Using

                'Dim RegParts As String() = Regex.Split(data2, ",")

                'Dim Lat As Double = [Double].Parse(RegParts(0).Trim())
                'Dim Lng As Double = [Double].Parse(RegParts(1).Trim())

                'Dim ds = New DataSet()

                'Dim IDs As String = ""

                'Dim dsSSID = New DataSet()

                'ds = OBJMaster.IsIMEIExists(Imei)

                'If Not ds Is Nothing Then   'IMEI_UDID not exists
                '    If Not ds.Tables(0) Is Nothing Then
                '        If ds.Tables(0).Rows.Count <> 0 Then
                '            Dim dtPersonSiteMapping = New DataTable()
                '            OBJMasterBAL = New MasterBAL()
                '            Dim dt As DataTable = ds.Tables(0)
                '            Dim personId As Integer
                '            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                '            dtPersonSiteMapping = OBJMasterBAL.GetPersonSiteMapping(personId)
                '            If Not dtPersonSiteMapping Is Nothing Then  'Unauthorized fuel location- Person Site mapping does not exists
                '                If dtPersonSiteMapping.Rows.Count <> 0 Then
                '                    'ds = OBJMaster.GetSiteDetails()


                '                    For index As Integer = 0 To dtPersonSiteMapping.Rows.Count - 1
                '                        If Not dtPersonSiteMapping.Rows(index)("Latitude").ToString() Is Nothing And Not dtPersonSiteMapping.Rows(index)("Longitude").ToString() Is Nothing Then
                '                            Dim siteLat As Double = [Double].Parse(dtPersonSiteMapping.Rows(index)("Latitude").ToString())
                '                            Dim siteLng As Double = [Double].Parse(dtPersonSiteMapping.Rows(index)("Longitude").ToString())

                '                            Dim meterdistance As Double = distance1(Lat, Lng, siteLat, siteLng, "M")

                '                            If meterdistance <= 200 Then

                '                                Dim siteID As Integer = dtPersonSiteMapping.Rows(index)("SiteID")

                '                                IDs = siteID & "," & IDs

                '                            End If
                '                        End If

                '                    Next

                '                    If Not IDs = "" Then
                '                        dsSSID = OBJMaster.GetSSIDbySiteId(IDs)
                '                        'Dim resp As String =
                '                        SSIDdsToJson(context, dsSSID)
                '                        'context.Response.Write(resp)
                '                    Else
                '                        'context.Response.Write("empty")
                '                        ErrorInAuthontication(context, "fail", "Your Location is not near an authorized Fuel Site. Please contact your Administrator for more information.")
                '                    End If
                '                Else
                '                    ErrorInAuthontication(context, "fail", "Site not assigned. Please contact your Administrator for more information.")
                '                End If
                '            Else
                '                ErrorInAuthontication(context, "fail", "Site not assigned. Please contact your Administrator for more information.")
                '            End If
                '        Else
                '            ErrorInAuthontication(context, "fail", "IMEI not exists")
                '        End If
                '    Else
                '        ErrorInAuthontication(context, "fail", "IMEI not exists")
                '    End If



                '    'context.Response.Write(CreateResponse("", "", "", resp))

                'Else
                '    'context.Response.Write("empty")
                '    ErrorInAuthontication(context, "fail", "IMEI not exists")
                'End If

                context.Response.Write("Bad Request.")

            ElseIf Route = "AndroidSSID" Then
                'log.Error("AndroidSSID_001")
                'Get username pass from ssid------------------
                Dim data2 = [String].Empty
                context.Request.InputStream.Position = 0
                Using inputStream2 = New StreamReader(context.Request.InputStream)
                    data2 = inputStream2.ReadToEnd()
                    log.Debug("AndroidSSID : " & data2)
                End Using
                'log.Error("AndroidSSID_002")
                Dim RegParts As String() = Regex.Split(data2, "#:#")
                Dim strSSID As String = RegParts(0).Trim()
                'log.Error("AndroidSSID_01")
                Dim Lat As Double = [Double].Parse(RegParts(1).Trim()).ToString("0.00000")
                Dim Lng As Double = [Double].Parse(RegParts(2).Trim()).ToString("0.00000")
                'log.Error("AndroidSSID_02")
                Dim ds = New DataSet()

                Dim IDs As String = ""

                Dim dsSSID = New DataSet()

                ds = OBJServiceBAL.IsIMEIExists(Imei)
                'log.Error("AndroidSSID_1")
                If Not ds Is Nothing Then   'IMEI_UDID not exists
                    If Not ds.Tables(0) Is Nothing Then
                        If ds.Tables(0).Rows.Count <> 0 Then
                            Dim dtPersonSiteMapping = New DataTable()
                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = ds.Tables(0)
                            Dim personId As Integer
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            dtPersonSiteMapping = OBJMasterBAL.GetPersonSiteMapping(personId, 0)
                            If Not dtPersonSiteMapping Is Nothing Then  'Unauthorized fuel location- Person Site mapping does not exists
                                If dtPersonSiteMapping.Rows.Count <> 0 Then
                                    'ds = OBJMaster.GetSiteDetails()

                                    'log.Error("AndroidSSID_2")
                                    For index As Integer = 0 To dtPersonSiteMapping.Rows.Count - 1

                                        'If Not dtPersonSiteMapping.Rows(index)("Latitude").ToString() Is Nothing And Not dtPersonSiteMapping.Rows(index)("Longitude").ToString() Is Nothing Then
                                        '    Dim siteLat As Double = [Double].Parse(dtPersonSiteMapping.Rows(index)("Latitude").ToString())
                                        '    Dim siteLng As Double = [Double].Parse(dtPersonSiteMapping.Rows(index)("Longitude").ToString())

                                        '    Dim meterdistance As Double = distance1(Lat, Lng, siteLat, siteLng, "M")

                                        '    If meterdistance <= 200 Then

                                        '        Dim siteID As Integer = dtPersonSiteMapping.Rows(index)("SiteID")

                                        '        IDs = siteID & "," & IDs

                                        '    End If
                                        'End If

                                        Dim siteID As Integer = dtPersonSiteMapping.Rows(index)("SiteID")

                                        IDs = siteID & "," & IDs

                                    Next
                                    'log.Error("AndroidSSID_3")
                                    If Not IDs = "" Then
                                        dsSSID = OBJServiceBAL.GetAndroidSSID(strSSID, IDs)

                                        If (Not dsSSID Is Nothing) And dsSSID.Tables.Count > 0 And dsSSID.Tables(0).Rows.Count > 0 Then

                                            Dim rootOject = New RootObject()
                                            rootOject.ResponceMessage = "success"
                                            rootOject.ResponceText = "SSID Data"
                                            rootOject.SSIDDataObj = New List(Of SSIDData)()
                                            'log.Error("AndroidSSID_4")
                                            Dim objss = New SSIDData()

                                            objss.HoseId = dsSSID.Tables(0).Rows(0)("HoseId").ToString()
                                            objss.HoseNumber = "" '(dsSSID.Tables(0).Rows(0)("HoseNumber").ToString())
                                            objss.WifiSSId = (dsSSID.Tables(0).Rows(0)("WifiSSId").ToString().Trim())
                                            objss.ReplaceableHoseName = (dsSSID.Tables(0).Rows(0)("ReplaceableHoseName").ToString())

                                            If (objss.ReplaceableHoseName = "") Then
                                                objss.IsHoseNameReplaced = "Y"
                                                objss.ReplaceableHoseName = (dsSSID.Tables(0).Rows(0)("WifiSSId").ToString().Trim())
                                            Else
                                                objss.IsHoseNameReplaced = "N"
                                            End If
                                            'log.Error("AndroidSSID_5")
                                            objss.UserName = "" '(dsSSID.Tables(0).Rows(0)("UserName").ToString())
                                            'objss.Password = "" '(dsSSID.Tables(0).Rows(0)("Password").ToString())
                                            objss.Password = ConfigurationManager.AppSettings("FSPassword").ToString()
                                            objss.SiteId = (dsSSID.Tables(0).Rows(0)("SiteId").ToString())
                                            objss.SiteNumber = (dsSSID.Tables(0).Rows(0)("SiteNumber").ToString())
                                            objss.SiteName = "" '(dsSSID.Tables(0).Rows(0)("SiteName").ToString())
                                            objss.SiteAddress = (dsSSID.Tables(0).Rows(0)("SiteAddress").ToString())
                                            'log.Error("AndroidSSID_6")
                                            If (dsSSID.Tables(0).Rows(0)("Latitude") = "") Then
                                                objss.Latitude = ""
                                            Else
                                                objss.Latitude = Convert.ToDouble(dsSSID.Tables(0).Rows(0)("Latitude").ToString()).ToString("0.00000")
                                            End If

                                            If (dsSSID.Tables(0).Rows(0)("Longitude") = "") Then
                                                objss.Longitude = ""
                                            Else
                                                objss.Longitude = Convert.ToDouble(dsSSID.Tables(0).Rows(0)("Longitude").ToString()).ToString("0.00000")
                                            End If
                                            'log.Error("AndroidSSID_7")
                                            objss.MacAddress = dsSSID.Tables(0).Rows(0)("IPAddress").ToString().ToLower()

                                            If (dsSSID.Tables(0).Rows(0)("IsBusy") = True And Imei <> dsSSID.Tables(0).Rows(0)("BusyFromIMEI_UDID")) Then
                                                objss.IsBusy = "Y"
                                            Else
                                                objss.IsBusy = "N"
                                            End If

                                            If (dsSSID.Tables(0).Rows(0)("IsDefective") = True) Then
                                                objss.IsBusy = "Y"
                                            End If
                                            objss.IsDefective = dsSSID.Tables(0).Rows(0)("IsDefective").ToString()
                                            objss.ReconfigureLink = IIf(dsSSID.Tables(0).Rows(0)("ReconfigureLink").ToString() = "1", "True", "False")
                                            'objss.IsBusy = IIf(dsSSID.Tables(0).Rows(0)("IsBusy") = True, "Y", "N")
                                            'objss.PulserTimingAdjust = dsSSID.Tables(0).Rows(0)("PulserTimingAdjust")
                                            rootOject.SSIDDataObj.Add(objss)


                                            Dim seri As New JavaScriptSerializer()
                                            Dim json As String
                                            json = seri.Serialize(rootOject)
                                            context.Response.Write(json)

                                        Else
                                            log.Error("Unauthorized Wi-Fi selected. Please choose authorized FluidSecure Wi-Fi to proceed.")
                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg11"))
                                        End If
                                    Else
                                        'context.Response.Write("empty")
                                        log.Error("Your Location is not near an authorized Fluid Site. Please contact your Administrator for more information.")
                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg12"))
                                    End If
                                Else
                                    log.Error("Site not assigned. Please contact your Administrator for more information.")
                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg13"))
                                End If
                            Else
                                log.Error("Site not assigned. Please contact your Administrator for more information.")
                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg13"))
                            End If
                        Else
                            log.Error("IMEI not exists")
                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg14"))
                        End If
                    Else
                        log.Error("IMEI not exists")
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg14"))
                    End If
                Else
                    'context.Response.Write("empty")
                    log.Error("IMEI not exists")
                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg14"))
                End If

            ElseIf Route = "AuthorizationSequence" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    AuthorizationSequence(context)
                Else
                    log.Error("Not a valid Email And IMEI.  Email:  " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "TransactionComplete" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    TransactionComplete(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SetHoseNameReplacedFlag" Then

                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    Dim data = [String].Empty
                    Using inputStream = New StreamReader(context.Request.InputStream)
                        data = inputStream.ReadToEnd()
                        log.Debug("SetHoseNameReplacedFlag :" & data)
                        Dim javaScriptSerializer = New JavaScriptSerializer()
                        Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(HoseNameReplaced))

                        Dim IsHoseNameReplaced = DirectCast(serJsonDetails, HoseNameReplaced).IsHoseNameReplaced
                        Dim SiteId = DirectCast(serJsonDetails, HoseNameReplaced).SiteId
                        Dim HoseId = DirectCast(serJsonDetails, HoseNameReplaced).HoseId

                        Dim OBJWebServiceBAL = New WebServiceBAL()

                        Dim json As String
                        Dim result As Integer
                        Dim hoseNameReplacedResponceObj = New HoseNameReplacedResponce()

                        If (IsHoseNameReplaced = "Y") Then

                            result = OBJWebServiceBAL.UpdateIsHoseNameReplaced(SiteId, HoseId)
                            If (result = 1) Then
                                hoseNameReplacedResponceObj.ResponceMessage = "success"
                                hoseNameReplacedResponceObj.ResponceText = "Hose name replaced!"
                            Else
                                hoseNameReplacedResponceObj.ResponceMessage = "fail"
                                hoseNameReplacedResponceObj.ResponceText = "Hose name replace failed!"
                            End If
                        Else
                            hoseNameReplacedResponceObj.ResponceMessage = "fail"
                            hoseNameReplacedResponceObj.ResponceText = "IsHoseNameReplaced value must be Y!"
                        End If

                        json = javaScriptSerializer.Serialize(hoseNameReplacedResponceObj)
                        context.Response.Write(json)
                    End Using
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If

            ElseIf Route = "UpdateMACAddress" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    Dim data = [String].Empty
                    Using inputStream = New StreamReader(context.Request.InputStream)
                        data = inputStream.ReadToEnd()
                        log.Debug("UpdateMACAddress :" & data)
                        Dim javaScriptSerializer = New JavaScriptSerializer()
                        Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(SiteDetailsForUpdateMACAddress))

                        Dim SiteId = DirectCast(serJsonDetails, SiteDetailsForUpdateMACAddress).SiteId
                        Dim MACAddress = DirectCast(serJsonDetails, SiteDetailsForUpdateMACAddress).MACAddress
                        Dim RequestFrom As String = ""
                        Dim HubName As String = ""
                        If Not DirectCast(serJsonDetails, SiteDetailsForUpdateMACAddress).RequestFrom = Nothing Then
                            RequestFrom = DirectCast(serJsonDetails, SiteDetailsForUpdateMACAddress).RequestFrom
                        End If
                        If Not DirectCast(serJsonDetails, SiteDetailsForUpdateMACAddress).HubName = Nothing Then
                            HubName = DirectCast(serJsonDetails, SiteDetailsForUpdateMACAddress).HubName
                        End If

                        Dim OBJWebServiceBAL = New WebServiceBAL()

                        Dim json As String
                        Dim result As Integer
                        Dim siteDetailsResponseForUpdateMACAddressObj = New SiteDetailsResponseForUpdateMACAddress()

                        result = OBJWebServiceBAL.UpdateMACAddress(SiteId, MACAddress.ToLower(), RequestFrom, HubName)

                        If (result = 1) Then
                            siteDetailsResponseForUpdateMACAddressObj.ResponceMessage = "success"
                            siteDetailsResponseForUpdateMACAddressObj.ResponceText = "MAC Address replaced!"
                        Else
                            siteDetailsResponseForUpdateMACAddressObj.ResponceMessage = "fail"
                            siteDetailsResponseForUpdateMACAddressObj.ResponceText = "MAC Address replace failed!"
                        End If


                        json = javaScriptSerializer.Serialize(siteDetailsResponseForUpdateMACAddressObj)
                        context.Response.Write(json)

                    End Using
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If


            ElseIf Route = "ChangeBusyStatus" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    steps = "ChangeBusyStatus : 1"
                    Dim data = [String].Empty

                    Using inputStream = New StreamReader(context.Request.InputStream)
                        data = inputStream.ReadToEnd()
                        log.Debug("ChangeBusyStatus :" & data)
                        Dim javaScriptSerializer = New JavaScriptSerializer()
                        Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(ChangeBusyStatus))
                        steps = "ChangeBusyStatus : 2"
                        Dim SiteId = DirectCast(serJsonDetails, ChangeBusyStatus).SiteId
                        steps = "ChangeBusyStatus :3"
                        Dim OBJWebServiceBAL = New WebServiceBAL()

                        Dim json As String
                        Dim result As Integer
                        Dim changeBusyStatusResponse = New ChangeBusyStatusResponse()

                        result = OBJWebServiceBAL.ChangeBusyStatusOfFluidSecureUnit(SiteId, False, Imei)
                        steps = "ChangeBusyStatus : 4"
                        If (result = 1) Then
                            changeBusyStatusResponse.ResponceMessage = "success"
                            changeBusyStatusResponse.ResponceText = "Status changed!"
                        Else
                            changeBusyStatusResponse.ResponceMessage = "fail"
                            changeBusyStatusResponse.ResponceText = "Status changing failed!"
                        End If
                        steps = "ChangeBusyStatus : 5"

                        json = javaScriptSerializer.Serialize(changeBusyStatusResponse)

                        context.Response.Write(json)

                    End Using

                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If

            ElseIf Route = "CheckVehicleRequireOdometerEntryAndRequireHourEntry" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    CheckVehicleRequireOdometerEntryAndRequireHourEntry(context)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "logSummary" Then
                log.Debug("In logSummary : ")
                Using inputStream = New StreamReader(context.Request.InputStream)
                    Dim data = [String].Empty
                    data = inputStream.ReadToEnd()
                    log.Debug("In logSummary. Data: " & data)
                End Using
            ElseIf Route = "SavePreAuthTransactions" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    SavePreAuthTransactions(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "UpgradeCurrentVersionWithUgradableVersion" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    UpgradeCurrentVersionWithUgradableVersion(context)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "IsUpgradeCurrentVersionWithUgradableVersion" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    IsUpgradeCurrentVersionWithUgradableVersion(context)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "IsUpgradeCurrentFSVMVersionWithUgradableVersion" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    'IsUpgradeCurrentVersionWithUgradableVersionFSVM(context)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "GetLaunchedFSVMFirmwareDetails" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    'GetLaunchedFSVMFirmwareDetails(context)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "UpgradeTransactionStatus" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    UpgradeTransactionStatus(context)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "UpgradeIsBusyStatus" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    UpgradeIsBusyStatus(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "CheckValidPinOrFOBNUmber" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    CheckValidPersonPinOrFOBNUmber(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "CheckVehicleFobOnly" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    CheckVehicleFobOnly(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SaveTankMonitorReading" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    SaveTankMonitorReading(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SaveInventoryVeederTankMonitorReading" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    SaveInventoryVeederTankMonitorReading(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SaveDeliveryVeederTankMonitorReading" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    SaveDeliveryVeederTankMonitorReading(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "VINAuthorization" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    VINAuthorization(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "CheckAndValidateFSNPDetails" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    CheckAndValidateFSNPDetails(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SaveManualVehicleOdometer" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    SaveManualVehicleOdometer(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SaveDiagnosticLogs" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    SaveDiagnosticLogs(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SaveDiagnosticLogFile" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    Dim LogFrom As String = ""
                    If (parts.Length > 3) Then
                        Dim temp As String = parts(3).Trim()
                        Dim tempArray As String() = temp.Split(";")
                        LogFrom = tempArray(0)
                    End If
                    SaveDiagnosticLogFile(context, Imei, LogFrom)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "SaveMultipleTransactions" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    SaveMultipleTransactions(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "DefectiveBluetoothInfoEmail" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    DefectiveBluetoothInfoEmail(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "CheckPersonFobOnly" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    CheckPersonFobOnly(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            ElseIf Route = "GetVehicleByFSTagMacAddress" Then
                Dim checkValid As Boolean = CheckValidRequest(Email, Imei)
                If (checkValid = True) Then
                    context.Request.InputStream.Position = 0
                    GetVehicleByFSTagMacAddress(context, Imei)
                Else
                    log.Error("Not a valid Email and IMEI.  Email: " & Email & " , IMEI : " & Imei)
                    ErrorInAuthontication(context, "fail", "Not a valid Email and IMEI. Please contact  administrator.")
                End If
            End If
        Catch ex As Exception
            log.Error("Exception occurred while prcessing request. Exception is :" & ex.Message & " . in step : " & steps)
            If (Not ex.InnerException Is Nothing) Then
                log.Error("Inner Expcetion is :" & ex.InnerException.Message & " .")
            End If
            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))
            'ErrorInAuthontication(context, "fail", "Exception occurred while prcessing request. Exception is :" & ex.Message)
        End Try
    End Sub

    Public Function Authenticate(imei As String, email As String, ByRef objUserData As UserData) As Boolean
        Dim isAuthenticated As Boolean = False
        Dim ds = New DataSet()
        ds = OBJServiceBAL.GetUserIsApproved(imei)
        If Not ds Is Nothing Then
            If ds.Tables.Count > 0 And ds.Tables(0).Rows.Count > 0 Then
                Dim row As DataRow = ds.Tables(0).Rows(0)
                Dim dsEmail As String = row("Email")
                Dim PersonId As String = row("PersonId")
                Dim dsPersonName As String = row("PersonName")
                Dim FluidSecureSiteName As String = row("HubSiteName").ToString()
                log.Debug("PhoneNumber" & row("PhoneNumber"))
                Dim dsPhoneNumber As String = row("PhoneNumber").ToString()
                Dim dsIsApproved As String = row("IsApproved")
                Dim dsIMEI_UDID As String = row("IMEI_UDID")
                Dim IsVehicleHasFob As String = row("IsVehicleHasFob")
                Dim IsPersonHasFob As String = row("IsPersonHasFob")
                log.Debug("Authenticate _ 1")
                'Dim objUserData = New UserData()
                objUserData = New UserData()
                objUserData.Email = dsEmail
                objUserData.PhoneNumber = dsPhoneNumber
                objUserData.PersonId = PersonId
                objUserData.PersonName = dsPersonName
                objUserData.FluidSecureSiteName = FluidSecureSiteName
                objUserData.IsApproved = dsIsApproved
                objUserData.IMEI_UDID = dsIMEI_UDID
                log.Debug("Authenticate _ 2")
                Dim IsOdoMeterRequire As String = "0"
                Dim IsLoginRequire As String = "0"

                Dim IsDepartmentRequire As String = "0"
                Dim IsPersonnelPINRequire As String = "0"
                Dim IsOtherRequire As String = "0"
                Dim OtherLabel As String = ""

                Dim customerId = row("CustomerId")
                Dim dtCustomer = New DataTable()
                dtCustomer = OBJMasterBAL.GetCustomerId(customerId)
                If Not dtCustomer Is Nothing Then
                    IsOdoMeterRequire = "False" 'dtCustomer.Rows(0)("IsOdometerRequire").ToString()
                    IsLoginRequire = dtCustomer.Rows(0)("IsLoginRequire").ToString()
                    IsDepartmentRequire = dtCustomer.Rows(0)("IsDepartmentRequire").ToString()
                    IsPersonnelPINRequire = dtCustomer.Rows(0)("IsPersonnelPINRequire").ToString()
                    IsOtherRequire = dtCustomer.Rows(0)("IsOtherRequire").ToString()
                    OtherLabel = dtCustomer.Rows(0)("OtherLabel").ToString()
                End If

                objUserData.IsOdoMeterRequire = IsOdoMeterRequire
                objUserData.IsLoginRequire = IsLoginRequire
                objUserData.IsDepartmentRequire = IsDepartmentRequire
                objUserData.IsPersonnelPINRequire = IsPersonnelPINRequire
                log.Debug("Authenticate : IsPersonnelPINRequire = " & row("IsPersonnelPINRequire").ToString())
                objUserData.IsPersonnelPINRequireForHub = row("IsPersonnelPINRequire").ToString()
                objUserData.IsOtherRequire = IsOtherRequire
                objUserData.OtherLabel = OtherLabel
                objUserData.BluetoothCardReader = row("BluetoothCardReader").ToString() 'used in old app
                objUserData.BluetoothCardReaderMacAddress = row("BluetoothCardReaderMacAddress").ToString().ToUpper()
                objUserData.IsVehicleHasFob = IsVehicleHasFob
                objUserData.IsPersonHasFob = IsPersonHasFob
                objUserData.IsAccessForFOBApp = row("IsAccessForFOBApp").ToString()
                objUserData.LFBluetoothCardReader = row("LFBluetoothCardReader").ToString()
                log.Debug("VeederRootMacAddress 1 : " & row("VeederRootMacAddress").ToString())
                objUserData.LFBluetoothCardReaderMacAddress = row("LFBluetoothCardReaderMacAddress").ToString().ToUpper()
                objUserData.VeederRootMacAddress = row("VeederRootMacAddress").ToString()
                objUserData.CollectDiagnosticLogs = row("CollectDiagnosticLogs").ToString()
                objUserData.IsLogging = row("Islogging").ToString()
                log.Debug("IsGateHub : " & row("IsGateHub").ToString())
                objUserData.IsGateHub = row("IsGateHub").ToString()
                log.Debug("IsFluidSecureHub - " & row("IsFluidSecureHub").ToString())
                If row("IsFluidSecureHub").ToString() <> "False" Then
                    objUserData.IsVehicleNumberRequire = row("IsVehicleNumberRequire").ToString()
                Else
                    objUserData.IsVehicleNumberRequire = ds.Tables(1).Rows(0)(0).ToString()
                End If

                objUserData.WifiChannelToUse = row("WifiChannelToUse").ToString()

                'Dim seri As New JavaScriptSerializer()
                'Dim jStr As String = seri.Serialize(objUserData)
                'result = jStr
                isAuthenticated = True
            Else
                'result = "fail"
                isAuthenticated = False
            End If
        Else
            'result = "fail"
            isAuthenticated = False
        End If



        Return isAuthenticated
    End Function

    Public Function funRegisterUser(context As HttpContext, Name As String, Mobile As String, Email As String, IMEI_UDID As String, ReqFrom As String, companyId As Integer, IsAP As Boolean) As String



        Dim IsApproved As Boolean = False
        Dim user = New ApplicationUser()
        Dim manager = context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
        Dim signInManager = context.GetOwinContext().[Get](Of ApplicationSignInManager)()
        manager.PasswordValidator = New PasswordValidator() With {
                    .RequiredLength = 4,
                    .RequireNonLetterOrDigit = True,
                    .RequireDigit = True,
                    .RequireLowercase = True,
                    .RequireUppercase = True
                    }



        If (IsAP = True) Then

            user = New ApplicationUser() With {
    .UserName = Email,
    .Email = Email,
    .PersonName = "",
    .PhoneNumber = Mobile,
    .DepartmentId = 0,
    .FuelLimitPerTxn = 0,
    .FuelLimitPerDay = 0,
    .PreAuth = 0,
    .SoftUpdate = "N",
    .CreatedDate = DateTime.Now,
    .CreatedBy = Nothing,
    .IsDeleted = False,
    .IsApproved = True,
    .RequestFrom = ReqFrom,
    .SendTransactionEmail = False,
    .IMEI_UDID = IMEI_UDID,
    .RoleId = "936965f9-b2bc-486c-af9a-df1025bb2966",
    .ApprovedOn = DateTime.MinValue,
    .CustomerId = companyId,
    .IsFluidSecureHub = True,
    .PinNumber = Nothing,
    .IsUserForHub = False,
    .PasswordResetDate = DateTime.Now,
    .FOBNumber = "",
    .AdditionalEmailId = "",
    .IsPersonnelPINRequire = False,
    .BluetoothCardReader = "",
    .PrinterName = "",
    .PrinterMacAddress = "",
    .HubSiteName = "",
    .BluetoothCardReaderMacAddress = "",
    .LFBluetoothCardReader = "",
    .LFBluetoothCardReaderMacAddress = "",
       .VeederRootMacAddress = "",
       .CollectDiagnosticLogs = False,
       .IsVehicleHasFob = False,
       .IsPersonHasFob = False,
       .IsTermConditionAgreed = False,
       .DateTimeTermConditionAccepted = Nothing,
    .IsGateHub = False,
    .IsVehicleNumberRequire = False,
    .HubAddress = "",
    .IsLogging = 0
 }
            Dim result As IdentityResult = manager.Create(user, "FluidSecure*123")

            If result.Succeeded Then

                Try
                    ' Add in IMEI Person mapping
                    Dim OBJMaster = New MasterBAL()

                    OBJMaster = New MasterBAL()
                    Dim dtPerson As DataTable = New DataTable()
                    dtPerson = OBJMaster.GetPersonDetailByUniqueUserId(user.Id)
                    log.Error("Adding in IMEI Person mapping. PersonId - " & Convert.ToInt32(dtPerson.Rows(0)("PersonId").ToString()))
                    OBJMaster.IMEI_UDIDPersonMappingInsertUpdate(0, Convert.ToInt32(dtPerson.Rows(0)("PersonId").ToString()), IMEI_UDID, IsApproved, Nothing)
                Catch ex As Exception
                    log.Error("Error while Adding in IMEI Person mapping. Error - " & ex.Message)
                End Try



                OBJMasterBAL = New MasterBAL()
                Dim HubName As Integer = OBJMasterBAL.GetAndUpdateLastHubName()

                Dim FullHubName As String = "HUB" & HubName.ToString("00000000")

                user.PersonName = FullHubName

                manager.Update(user)

                OBJServiceBAL = New WebServiceBAL()
                Dim emailIds As String = OBJServiceBAL.GetCustomerAdminsByCustomerId(companyId)

                SendRegistrationEmailToCustomerAdmins(emailIds, Name, context, False)

                ErrorInAuthontication(context, "success", resourceManager.GetString("HandlerMsg3"))

            Else
                'context.Response.Write(CreateResponse("401", "fail", "Registration fail", ""))'if not AP then mark approved as false
                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg7"))
            End If

        Else

            user = New ApplicationUser() With {
    .UserName = Email,
    .Email = Email,
    .PersonName = Name,
    .PhoneNumber = Mobile,
    .DepartmentId = 0,
    .FuelLimitPerTxn = 0,
    .FuelLimitPerDay = 0,
    .PreAuth = 0,
    .SoftUpdate = "N",
    .CreatedDate = DateTime.Now,
    .CreatedBy = Nothing,
    .IsDeleted = False,
    .IsApproved = False,
    .RequestFrom = ReqFrom,
    .SendTransactionEmail = False,
    .IMEI_UDID = IMEI_UDID,
    .RoleId = "11df27ed-8d70-46a9-a925-7150326ffe75",
    .ApprovedOn = DateTime.MinValue,
    .CustomerId = companyId,
    .IsFluidSecureHub = False,
   .IsUserForHub = False,
   .PasswordResetDate = DateTime.Now,
   .FOBNumber = "",
   .AdditionalEmailId = "",
   .IsPersonnelPINRequire = False,
    .BluetoothCardReader = "",
    .PrinterName = "",
    .PrinterMacAddress = "",
    .HubSiteName = "",
    .BluetoothCardReaderMacAddress = "",
       .LFBluetoothCardReader = "",
       .LFBluetoothCardReaderMacAddress = "",
       .VeederRootMacAddress = "",
       .CollectDiagnosticLogs = False,
       .IsVehicleHasFob = False,
       .IsPersonHasFob = False,
       .IsTermConditionAgreed = False,
            .DateTimeTermConditionAccepted = Nothing,
    .IsGateHub = False,
    .IsVehicleNumberRequire = False,
    .HubAddress = "",
    .IsLogging = 0
 }

            Dim result As IdentityResult = manager.Create(user, "Fuel@123")

            If result.Succeeded Then

                Try
                    ' Add in IMEI Person mapping
                    Dim OBJMaster = New MasterBAL()

                    OBJMaster = New MasterBAL()
                    Dim dtPerson As DataTable = New DataTable()
                    dtPerson = OBJMaster.GetPersonDetailByUniqueUserId(user.Id)
                    log.Error("Adding in IMEI Person mapping. PersonId - " & Convert.ToInt32(dtPerson.Rows(0)("PersonId").ToString()))
                    OBJMaster.IMEI_UDIDPersonMappingInsertUpdate(0, Convert.ToInt32(dtPerson.Rows(0)("PersonId").ToString()), IMEI_UDID, IsApproved, Nothing)
                Catch ex As Exception
                    log.Error("Error while Adding in IMEI Person mapping. Error - " & ex.Message)
                End Try




                'context.Response.Write(CreateResponse("200", "success", "Registration successfull", ""))
                OBJServiceBAL = New WebServiceBAL()
                Dim emailIds As String = OBJServiceBAL.GetCustomerAdminsByCustomerId(companyId)

                SendRegistrationEmailToCustomerAdmins(emailIds, Name, context, False)

                ErrorInAuthontication(context, "success", resourceManager.GetString("HandlerMsg3"))

            Else
                'context.Response.Write(CreateResponse("401", "fail", "Registration fail", ""))
                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg7"))
            End If

        End If

        Return ""

    End Function

    Public Sub SSIDdsToJson(context As HttpContext, ds As DataSet, Data As UserData, personId As Integer, IMEI_UDID As String)
        steps = "21"
        Dim result As String = ""
        Dim rootOject = New RootObject()
        rootOject.ResponceMessage = "success"
        rootOject.ResponceText = "SSID Data"
        rootOject.objUserData = New UserData()

        rootOject.objUserData.Email = Data.Email
        rootOject.objUserData.PhoneNumber = Data.PhoneNumber
        rootOject.objUserData.PersonName = Data.PersonName 'rm.GetString("PersonName") 
        rootOject.objUserData.IsApproved = Data.IsApproved
        rootOject.objUserData.IMEI_UDID = Data.IMEI_UDID
        rootOject.objUserData.IsOdoMeterRequire = Data.IsOdoMeterRequire
        rootOject.objUserData.IsLoginRequire = Data.IsLoginRequire
        rootOject.objUserData.IsDepartmentRequire = Data.IsDepartmentRequire
        rootOject.objUserData.IsPersonnelPINRequire = Data.IsPersonnelPINRequire
        rootOject.objUserData.IsPersonnelPINRequireForHub = Data.IsPersonnelPINRequireForHub
        rootOject.objUserData.IsOtherRequire = Data.IsOtherRequire
        rootOject.objUserData.OtherLabel = Data.OtherLabel
        rootOject.objUserData.IsVehicleHasFob = Data.IsVehicleHasFob
        rootOject.objUserData.IsPersonHasFob = Data.IsPersonHasFob
        rootOject.objUserData.TimeOut = ConfigurationManager.AppSettings("WaitingTime").ToString()
        rootOject.objUserData.AndroidAppLatestVersion = ConfigurationManager.AppSettings("AndroidAppLatestVersion").ToString()
        rootOject.objUserData.AppUpgradeMsgDisplayAfterDays = ConfigurationManager.AppSettings("AppUpgradeMsgDisplayAfterDays").ToString()
        rootOject.objUserData.PersonId = Data.PersonId
        rootOject.objUserData.LFBluetoothCardReader = Data.LFBluetoothCardReader
        rootOject.objUserData.LFBluetoothCardReaderMacAddress = Data.LFBluetoothCardReaderMacAddress
        log.Debug("VeederRootMacAddress 2 : " & Data.VeederRootMacAddress.ToString())
        rootOject.objUserData.VeederRootMacAddress = Data.VeederRootMacAddress.ToString()
        rootOject.objUserData.IsAccessForFOBApp = Data.IsAccessForFOBApp
        rootOject.objUserData.CollectDiagnosticLogs = Data.CollectDiagnosticLogs
        rootOject.objUserData.IsLogging = Data.IsLogging
        rootOject.objUserData.WifiChannelToUse = Data.WifiChannelToUse
        rootOject.objUserData.IsGateHub = Data.IsGateHub
        rootOject.objUserData.IsVehicleNumberRequire = Data.IsVehicleNumberRequire

        rootOject.SSIDDataObj = New List(Of SSIDData)()

        If Not ds Is Nothing Then
            If ds.Tables.Count > 0 And ds.Tables(0).Rows.Count > 0 Then

                'Dim objSSIDList = New GetSSIDList()
                'objSSIDList.result = New List(Of SSIDData)()
                For index As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim row As DataRow = ds.Tables(0).Rows(index)

                    Dim objSSIDData = New SSIDData()
                    objSSIDData.ResponceMessage = "success"
                    objSSIDData.ResponceText = "SSID Data"
                    objSSIDData.HoseId = row("HoseId")
                    objSSIDData.HoseNumber = "" 'row("HoseNumber")
                    objSSIDData.WifiSSId = row("WifiSSId").ToString().Trim()
                    steps = "22"
                    objSSIDData.ReplaceableHoseName = IIf(IsDBNull(row("ReplaceableHoseName")), "", row("ReplaceableHoseName"))
                    If (objSSIDData.ReplaceableHoseName = "") Then
                        objSSIDData.IsHoseNameReplaced = "Y"
                        objSSIDData.ReplaceableHoseName = (row("WifiSSId").ToString().Trim())
                    Else
                        objSSIDData.IsHoseNameReplaced = "N"
                    End If
                    steps = "23"

                    steps = "24"
                    objSSIDData.UserName = "" 'row("UserName")
                    'objSSIDData.Password = "" 'row("Password")
                    objSSIDData.Password = ConfigurationManager.AppSettings("FSPassword").ToString()
                    objSSIDData.SiteId = row("SiteId")
                    objSSIDData.SiteNumber = row("SiteNumber")
                    objSSIDData.SiteName = "" 'row("SiteName")
                    steps = "24_1"
                    objSSIDData.SiteAddress = row("SiteAddress")
                    'log.Error(row("Latitude"))
                    steps = "24_3"
                    If (row("Latitude") = "") Then
                        objSSIDData.Latitude = ""
                    Else
                        objSSIDData.Latitude = row("Latitude").ToString("0.00000")
                    End If

                    steps = "24_2"
                    'log.Debug(row("Longitude"))
                    If (row("Longitude") = "") Then
                        objSSIDData.Longitude = ""
                    Else
                        steps = "24_21"
                        objSSIDData.Longitude = row("Longitude").ToString("0.00000")
                    End If

                    steps = "24_211"
                    'log.Debug("IPAddress" & row("IPAddress"))
                    objSSIDData.MacAddress = row("IPAddress").ToString().ToLower()
                    steps = "24_212"

                    If (row("IsBusy") = True And IMEI_UDID <> row("BusyFromIMEI_UDID")) Then
                        objSSIDData.IsBusy = "Y"
                    Else
                        objSSIDData.IsBusy = "N"
                    End If

                    If (row("IsDefective") = True) Then
                        objSSIDData.IsBusy = "Y"
                    End If
                    objSSIDData.IsDefective = row("IsDefective").ToString()

                    'objSSIDData.IsBusy = IIf(row("IsBusy") = True, "Y", "N")
                    Try

                        If (row("IsDefectiveEmailSent").ToString() = False And row("IsDefective") = True) Then

                            Dim emailIds As String = OBJServiceBAL.GetCustomerAdminsByCustomerId(row("CompanyId").ToString())
                            Dim emailArray As String() = emailIds.Split(",")
                            For Each email As String In emailArray
                                SendLinkDefectiveEmail(email, row("WifiSSId").ToString().Trim(), row("NumberOfZeroTransaction"), row("Company").ToString())
                            Next
                            Dim LinkDefectiveTrakEmail As String() = ConfigurationManager.AppSettings("LinkDefectiveTrakEmail").Split(";")

                            For Each email As String In LinkDefectiveTrakEmail
                                SendLinkDefectiveEmail(email, row("WifiSSId").ToString().Trim(), row("NumberOfZeroTransaction"), row("Company").ToString())
                            Next

                            OBJMasterBAL = New MasterBAL()
                            OBJMasterBAL.UpdateIsDefectiveEmailSent(row("SiteId"))

                        End If
                    Catch ex As Exception
                        log.Debug("Exception occurred in Sending defective email. ex Is : " & ex.Message)
                    End Try


                    'check is upgradable or not
                    Dim resultUpgradable As String = ""
                    OBJMasterBAL = New MasterBAL()
                    Dim dsUpgrade As DataSet
                    dsUpgrade = OBJMasterBAL.checkLaunchedAndExistedVersionAndUpdate(row("HoseId"), "", personId, 0)

                    If dsUpgrade IsNot Nothing Then
                        If dsUpgrade.Tables(0) IsNot Nothing Then
                            If dsUpgrade.Tables(0).Rows.Count > 0 Then
                                resultUpgradable = dsUpgrade.Tables(0).Rows(0)("resultUpgradable")
                            Else
                                resultUpgradable = "N"
                            End If
                        Else
                            resultUpgradable = "N"
                        End If
                    Else
                        resultUpgradable = "N"
                    End If

                    objSSIDData.IsUpgrade = resultUpgradable.ToUpper

                    'Add Launched link 
                    Dim dtFirmwares As DataTable = New DataTable()
                    dtFirmwares = OBJMasterBAL.GetLaunchedFirmwareDetails()
                    Dim FirmwareVersion As String = ""
                    Dim FilePath As String = ""

                    If (Not dtFirmwares Is Nothing) Then
                        If (dtFirmwares.Rows.Count > 0) Then
                            FirmwareVersion = dtFirmwares.Rows(0)("Version")
                            FilePath = "" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority & dtFirmwares.Rows(0)("FirmwareFilePath")
                        End If
                    End If
                    objSSIDData.FilePath = FilePath
                    objSSIDData.PulserTimingAdjust = row("PulserTimingAdjust")
                    objSSIDData.BluetoothCardReaderHF = Data.BluetoothCardReader 'row("BluetoothCardReader")
                    objSSIDData.BluetoothCardReaderMacAddress = Data.BluetoothCardReaderMacAddress.ToUpper()
                    objSSIDData.LFBluetoothCardReader = Data.LFBluetoothCardReader
                    objSSIDData.LFBluetoothCardReaderMacAddress = Data.LFBluetoothCardReaderMacAddress
                    objSSIDData.VeederRootMacAddress = Data.VeederRootMacAddress
                    objSSIDData.FSNPMacAddress = row("FSNPMacAddress").ToString()
                    objSSIDData.ReconfigureLink = IIf(row("ReconfigureLink").ToString(), "True", "False")
                    objSSIDData.IsTLDCall = IIf(row("PROBEMacAddress").ToString() = "", "False", "True")
                    rootOject.SSIDDataObj.Add(objSSIDData)
                Next

                steps = "24_213"
                Dim preAuthData As ResponsePreAuthTransactions = New ResponsePreAuthTransactions()
                preAuthData = PreAuthTransactionsDetails(personId)
                rootOject.PreAuthTransactionsObj = New ResponsePreAuthTransactions()
                rootOject.PreAuthTransactionsObj = preAuthData

                Dim seri As New JavaScriptSerializer()
                Dim json As String
                json = seri.Serialize(rootOject)
                log.Error("SSIDdsToJson Data : " & json)

                context.Response.Write(json)
            Else
                'result = "empty"
                ErrorInAuthontication(context, "fail", "No data found for site, Please contact administrator.")
            End If
        Else
            'result = "fail"
            ErrorInAuthontication(context, "fail", "No data found for site, Please contact administrator.")
        End If
    End Sub

    Public Function CreateResponse(ResponseCode As String, ResponseText As String, Message As String, data As String) As String
        Dim seri As New JavaScriptSerializer()
        Dim adi As New ArrayData()
        adi.ResponseCode = ResponseCode
        adi.ResponseText = ResponseText
        adi.Message = Message
        adi.data = data
        Return seri.Serialize(adi)
    End Function


    Private Sub DefectiveBluetoothInfoEmail(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Try
            Dim json As String
            Dim steps As String
            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                'data = context.Request("cmtxtnid_10_record")
                requestJson = data
                log.Debug("DefectiveBluetoothInfoEmail :" & data)
                Dim DefectiveBluetoothInfoEmailResponceObj = New DefectiveBluetoothInfoEmailResponce()
                Dim javaScriptSerializer As JavaScriptSerializer = New JavaScriptSerializer()

                Dim dsIMEI = New DataSet()
                dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                steps = "DefectiveBluetoothInfoEmail 2"
                Dim sendResponse As Integer = 0
                If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                    If Not dsIMEI.Tables(0) Is Nothing Then
                        If dsIMEI.Tables(0).Rows(0)("IsApproved").ToString() = "True" Then

                            If dsIMEI.Tables(0).Rows(0)("SendDefectiveBluetoothReaderEmail").ToString() = "False" Then

                                DefectiveBluetoothInfoEmailResponceObj.ResponceMessage = "success"
                                DefectiveBluetoothInfoEmailResponceObj.ResponceText = "Defective Bluetooth Info Email Skipped for this company!"

                                json = javaScriptSerializer.Serialize(DefectiveBluetoothInfoEmailResponceObj)
                                context.Response.Write(json)
                                Return
                            End If


                            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(DefectiveBluetoothInfoEmailMaser))
                            Dim HubName As String = DirectCast(serJsonDetails, DefectiveBluetoothInfoEmailMaser).HubName
                            Dim SiteName As String = DirectCast(serJsonDetails, DefectiveBluetoothInfoEmailMaser).SiteName
                            Dim LinkDefectiveTrakEmail As String() = ConfigurationManager.AppSettings("LinkDefectiveTrakEmail").Split(";")

                            Dim objWebServiceBAL As WebServiceBAL = New WebServiceBAL()
                            Dim dtDefectiveBluetoothInfo As DataTable = objWebServiceBAL.CheckDefectiveBluetoothInfoEmail(HubName)

                            If dtDefectiveBluetoothInfo IsNot Nothing Then
                                If dtDefectiveBluetoothInfo.Rows.Count > 0 Then
                                    log.Debug("IsInProgress: " & dtDefectiveBluetoothInfo.Rows(0)("IsInProgress").ToString())
                                    log.Debug("cntEntry: " & dtDefectiveBluetoothInfo.Rows(0)("cntEntry").ToString())
                                    If dtDefectiveBluetoothInfo.Rows(0)("IsInProgress").ToString() = "0" Then
                                        If dtDefectiveBluetoothInfo.Rows(0)("cntEntry").ToString() = "0" Then
                                            For index = 0 To LinkDefectiveTrakEmail.Length - 1
                                                log.Debug("LinkDefectiveTrakEmail:" & LinkDefectiveTrakEmail(index))
                                                If (LinkDefectiveTrakEmail(index) = "") Then
                                                    Continue For
                                                End If
                                                Dim body As String = String.Empty

                                                Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/DefectiveBluetoothInfoEmail.txt"))
                                                    body = sr.ReadToEnd()
                                                End Using

                                                '------------------
                                                body = body.Replace("CustomerEmail", LinkDefectiveTrakEmail(index))
                                                body = body.Replace("HubName", HubName)
                                                body = body.Replace("CompanyName", dsIMEI.Tables(0).Rows(0)("CustomerName").ToString())

                                                If SiteName IsNot Nothing Then
                                                    If SiteName <> "" Then
                                                        body = body.Replace("SiteName", SiteName)
                                                    Else
                                                        body = body.Replace("Site name : <b>SiteName</b> <br />", "")
                                                    End If
                                                End If

                                                Dim e As New EmailService()


                                                Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))


                                                mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
                                                mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

                                                Dim messageSend As New MailMessage()
                                                messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
                                                messageSend.[To].Add(New MailAddress(LinkDefectiveTrakEmail(index)))

                                                messageSend.Subject = "Attention required - " & dsIMEI.Tables(0).Rows(0)("CustomerName").ToString() & " -> " & HubName & "'s BT reader connection broken" & " "

                                                messageSend.Body = body

                                                messageSend.IsBodyHtml = True
                                                mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))

                                                'log.Debug("body:  " & body)

                                                Try
                                                    mailClient.Send(messageSend)
                                                    log.Debug("Mail sent to: " & LinkDefectiveTrakEmail(index))
                                                    sendResponse = 1
                                                Catch ex As Exception
                                                    log.Debug("Exception occurred in sending Link Defective emails to EmailId : " & LinkDefectiveTrakEmail(index) & ". ex is :" & ex.Message)
                                                    'ErrorInAuthontication(context, "fail", "Defective Bluetooth Info Email Send failed.")
                                                End Try

                                            Next


                                            If (sendResponse = 1) Then
                                                Try
                                                    objWebServiceBAL = New WebServiceBAL()
                                                    objWebServiceBAL.InsertUpdateDefectiveBluetoothInfoEmailRecord(HubName)
                                                    log.Debug("Update: " & HubName)
                                                Catch ex As Exception
                                                    log.Debug("Exception occurred in InsertUpdateDefectiveBluetoothInfoEmailRecord fot HubName : " & HubName & ". ex Is : " & ex.Message)
                                                End Try

                                                DefectiveBluetoothInfoEmailResponceObj.ResponceMessage = "success"
                                                DefectiveBluetoothInfoEmailResponceObj.ResponceText = "Defective Bluetooth Info Email Send successfully!"

                                                json = javaScriptSerializer.Serialize(DefectiveBluetoothInfoEmailResponceObj)
                                                context.Response.Write(json)
                                                Return
                                            Else
                                                log.Debug("ProcessRequest: DefectiveBluetoothInfoEmail- Email not sent to anyone.")
                                                ErrorInAuthontication(context, "fail", "Defective Bluetooth Info Email Send failed.")
                                            End If
                                        Else

                                            DefectiveBluetoothInfoEmailResponceObj.ResponceMessage = "success"
                                            DefectiveBluetoothInfoEmailResponceObj.ResponceText = "Already sent Defective Bluetooth Info Email so Skipped!"

                                            json = javaScriptSerializer.Serialize(DefectiveBluetoothInfoEmailResponceObj)
                                            context.Response.Write(json)
                                            Return
                                        End If
                                    Else
                                        DefectiveBluetoothInfoEmailResponceObj.ResponceMessage = "success"
                                        DefectiveBluetoothInfoEmailResponceObj.ResponceText = "Already sent Defective Bluetooth Info Email so Skipped!"

                                        json = javaScriptSerializer.Serialize(DefectiveBluetoothInfoEmailResponceObj)
                                        context.Response.Write(json)
                                        Return
                                    End If
                                Else
                                    DefectiveBluetoothInfoEmailResponceObj.ResponceMessage = "success"
                                    DefectiveBluetoothInfoEmailResponceObj.ResponceText = "Already sent Defective Bluetooth Info Email so Skipped!"

                                    json = javaScriptSerializer.Serialize(DefectiveBluetoothInfoEmailResponceObj)
                                    context.Response.Write(json)
                                    Return
                                End If
                            Else
                                DefectiveBluetoothInfoEmailResponceObj.ResponceMessage = "success"
                                DefectiveBluetoothInfoEmailResponceObj.ResponceText = "Already sent Defective Bluetooth Info Email so Skipped!"

                                json = javaScriptSerializer.Serialize(DefectiveBluetoothInfoEmailResponceObj)
                                context.Response.Write(json)
                                Return
                            End If
                        Else
                            log.Debug("ProcessRequest: DefectiveBluetoothInfoEmail- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: DefectiveBluetoothInfoEmail- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If
                Else
                    log.Debug("ProcessRequest: DefectiveBluetoothInfoEmail- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
                steps = "3"
            End Using

        Catch ex As Exception

            log.Error("Exception occurred while DefectiveBluetoothInfoEmail. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", "Defective Bluetooth Info Email Send failed.")

        End Try

    End Sub


    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Function distance1(lat1 As Double, lon1 As Double, lat2 As Double, lon2 As Double, unit As Char) As Double
        Dim theta As Double = lon1 - lon2
        Dim dist As Double = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta))
        dist = Math.Acos(dist)
        dist = rad2deg(dist)
        dist = dist * 60 * 1.1515
        If unit = "M"c Then
            dist = dist * 1.609344
            dist = dist * 1000
        ElseIf unit = "N"c Then
            dist = dist * 0.8684
        End If
        Return (dist)
    End Function

    Public Function deg2rad(deg As Double) As Double
        Return (deg * Math.PI / 180.0)
    End Function

    Public Function rad2deg(rad As Double) As Double
        Return (rad / Math.PI * 180.0)
    End Function

#Region "AuthorizationSequence"
    Private Sub AuthorizationSequence(context As HttpContext)
        steps = "AuthorizationSequence 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Info("AuthorizationSequence data : " & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(AuthorizationSequenceModel))
            Dim IMEI_UDID = DirectCast(serJsonDetails, AuthorizationSequenceModel).IMEIUDID
            Dim VehicleNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).VehicleNumber
            Dim OdoMeter = DirectCast(serJsonDetails, AuthorizationSequenceModel).OdoMeter
            Dim WifiSSId = DirectCast(serJsonDetails, AuthorizationSequenceModel).WifiSSId
            Dim SiteId = DirectCast(serJsonDetails, AuthorizationSequenceModel).SiteId
            Dim DepartmentNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).DepartmentNumber
            Dim PersonnelPIN = DirectCast(serJsonDetails, AuthorizationSequenceModel).PersonnelPIN
            Dim Other = DirectCast(serJsonDetails, AuthorizationSequenceModel).Other
            Dim RequestFrom = DirectCast(serJsonDetails, AuthorizationSequenceModel).RequestFrom
            Dim RequestFromAPP = DirectCast(serJsonDetails, AuthorizationSequenceModel).RequestFromAPP
            Dim FOBNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).FOBNumber
            Dim IsVehicleNumberRequire = DirectCast(serJsonDetails, AuthorizationSequenceModel).IsVehicleNumberRequire

            If (Not PersonnelPIN Is Nothing) Then
                PersonnelPIN = PersonnelPIN.Trim()
            Else
                PersonnelPIN = ""
            End If

            If (Not VehicleNumber Is Nothing) Then
                VehicleNumber = VehicleNumber.Trim()
            Else
                VehicleNumber = ""
            End If

            log.Debug("IsVehicleNumberRequire- " & IsVehicleNumberRequire)
            If (IsVehicleNumberRequire Is Nothing) Then
                IsVehicleNumberRequire = "True"
            End If

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "AuthorizationSequence 2"
            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then
                        Dim dtMain As DataTable = New DataTable()
                        'If (RequestFromAPP = "AP") Then 'commented for person B should be able to enter his pin on person A’s phone
                        Dim dsPerson = New DataSet()
                        'log.Debug("in AP")
                        If (PersonnelPIN.Trim() = "") Then
                            dtMain = dsIMEI.Tables(0)
                        Else
                            'dsPerson = OBJServiceBAL.GetPersonnelByPinNumber(PersonnelPIN, Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString()))
                            'dtMain = dsPerson.Tables(0)

                            dsPerson = OBJServiceBAL.GetPersonnelByPinNumber(PersonnelPIN.Trim(), Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString()))
                            If Not dsPerson Is Nothing Then
                                If (dsPerson.Tables(0).Rows.Count = 0) Then

                                    log.Debug("ProcessRequest: AuthorizationSequence- Invalid Pin. Please contact administrator. IMEI_UDID=" & IMEI_UDID & ". Pin" & PersonnelPIN.Trim())
                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg34"), "Pin")
                                    Return
                                Else
                                    dtMain = dsPerson.Tables(0)
                                End If
                            Else
                                log.Debug("ProcessRequest: AuthorizationSequence- Invalid Pin. Please contact administrator. IMEI_UDID=" & IMEI_UDID & ". Pin" & PersonnelPIN.Trim())
                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg34"), "Pin")
                                Return
                            End If
                        End If
                        'Else

                        '	dtMain = dsIMEI.Tables(0)

                        'End If

                        Dim printerMacAddress As String = ""
                        Dim PrinterName As String = ""
                        Dim BluetoothCardReader As String = ""
                        Dim BluetoothCardReaderMacAddress As String = ""
                        Try
                            printerMacAddress = dsIMEI.Tables(0).Rows(0)("PrinterMacAddress").ToString().ToLower()
                            PrinterName = dsIMEI.Tables(0).Rows(0)("PrinterName")
                            BluetoothCardReader = dsIMEI.Tables(0).Rows(0)("BluetoothCardReader")
                            BluetoothCardReaderMacAddress = dsIMEI.Tables(0).Rows(0)("BluetoothCardReaderMacAddress").ToString().ToUpper()
                            log.Info("BluetoothCardReader : " & BluetoothCardReader)
                        Catch ex As Exception
                            printerMacAddress = ""
                            PrinterName = ""
                            BluetoothCardReader = ""
                            BluetoothCardReaderMacAddress = ""
                        End Try

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "AuthorizationSequence 3"
                            Dim dtPersonSiteMapping = New DataTable()
                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            log.Debug("personId:" & personId)
                            Dim customerId As Integer
                            customerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            Dim RoleId As String
                            RoleId = dt.Rows(0)("RoleId").ToString()
                            dtPersonSiteMapping = OBJMasterBAL.GetPersonSiteMapping(personId, SiteId)
                            steps = "AuthorizationSequence 5"
                            If Not dtPersonSiteMapping Is Nothing Then  'Unauthorized fuel location- Person Site mapping does not exists
                                If dtPersonSiteMapping.Rows.Count <> 0 Then 'Person Site mapping does not contain data
                                    Dim dtVehicle = New DataTable()
                                    If (VehicleNumber.Trim() = "" And IsVehicleNumberRequire = "False") Then
                                        dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And LTRIM(RTRIM(V.VehicleNumber)) ='default'", personId, RoleId)
                                    ElseIf (VehicleNumber.Trim() <> "") Then
                                        dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And LTRIM(RTRIM(V.VehicleNumber)) ='" & IIf(VehicleNumber.Trim().ToLower().Contains("guest"), "guest", VehicleNumber.Trim()) & "'", personId, RoleId)
                                    ElseIf (FOBNumber <> "") Then
                                        dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & FOBNumber.ToString().Replace(" ", "") & "'", personId, RoleId)
                                    End If

                                    If Not dtVehicle Is Nothing Then 'Authorized vehicle?-Vehicle number not exists
                                        If dtVehicle.Rows.Count <> 0 Then ''Authorized vehicle?-Vehicle number not exists
                                            Dim dtVehicelSiteMapping = New DataTable()
                                            Dim vehicleId As Integer
                                            vehicleId = Integer.Parse(dtVehicle.Rows(0)("VehicleId").ToString())
                                            steps = "AuthorizationSequence 6 " & vehicleId

                                            If (dtVehicle.Rows(0)("IsActive").ToString() = "False") Then
                                                log.Debug("ProcessRequest: AuthorizationSequence - Vehicle is InActive. IMEI_UDID=" & IMEI_UDID)
                                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg16"), "Vehicle")
                                                Return
                                            End If

                                            dtVehicelSiteMapping = OBJMasterBAL.GetVehicleSiteMapping(vehicleId, SiteId)
                                            If Not dtVehicelSiteMapping Is Nothing Or IsVehicleNumberRequire = "False" Then     'Vehicle Site Mapping not exist
                                                If dtVehicelSiteMapping.Rows.Count <> 0 Or IsVehicleNumberRequire = "False" Then 'Vehicle Site Mapping not exist
                                                    'Authorized fuel date?
                                                    Dim dtSiteDays = New DataTable()
                                                    dtSiteDays = OBJMasterBAL.GetSiteDays(Integer.Parse(SiteId))
                                                    If Not dtSiteDays Is Nothing Then    'site Days not assigned to site
                                                        If dtSiteDays.Rows.Count <> 0 Then
                                                            Dim dateValue As New DateTime()
                                                            dateValue = DateTime.Now
                                                            steps = "AuthorizationSequence 7"
                                                            Dim currentDay = CInt(dateValue.DayOfWeek) + 1
                                                            Dim dayArray = dtSiteDays.AsEnumerable().[Select](Function(r) r.Field(Of Integer)("Day")).ToArray()
                                                            Dim isCurrentDayInDayArray = dayArray.Contains(currentDay)
                                                            If isCurrentDayInDayArray = True Then 'Current Day not in site days
                                                                Dim dtPersonalTiming = New DataTable()
                                                                steps = "AuthorizationSequence 8"
                                                                dtPersonalTiming = OBJMasterBAL.GetPersonnelTimings(personId)
                                                                If Not dtPersonalTiming Is Nothing Then 'Person timing does not exists
                                                                    If dtPersonalTiming.Rows.Count <> 0 Then 'Person timing does not exists
                                                                        Dim dtSiteTiming = New DataTable()
                                                                        dtSiteTiming = OBJMasterBAL.GetSiteTimings(SiteId)
                                                                        steps = "AuthorizationSequence 9"
                                                                        If Not dtSiteTiming Is Nothing Then 'Site timing does not exists
                                                                            If dtSiteTiming.Rows.Count <> 0 Then 'Site timing does not exists
                                                                                Dim dsCurrentTimeInPerson = New DataSet()
                                                                                dsCurrentTimeInPerson = OBJMasterBAL.CheckCurrentTimeInTimesTable(SiteId, personId)
                                                                                If Not dsCurrentTimeInPerson Is Nothing Then ' check Current Time In Times Table
                                                                                    If dsCurrentTimeInPerson.Tables.Count = 2 Then
                                                                                        If dsCurrentTimeInPerson.Tables(0).Rows.Count <> 0 And dsCurrentTimeInPerson.Tables(1).Rows.Count <> 0 Then 'Current Time not in SiteTiming and PersonnelTimings from-to timing
                                                                                            'PersonalVehicle mapping
                                                                                            Dim dtPersonVehicleMapping = New DataTable()
                                                                                            dtPersonVehicleMapping = OBJMasterBAL.GetPersonVehicleMapping(personId)
                                                                                            If Not dtPersonVehicleMapping Is Nothing Or IsVehicleNumberRequire = "False" Then 'Person vehicle mapping does not exists
                                                                                                If dtPersonVehicleMapping.Rows.Count <> 0 Or IsVehicleNumberRequire = "False" Then 'Person vehicle mapping does not exists
                                                                                                    Dim vehicleArray = dtPersonVehicleMapping.AsEnumerable().[Select](Function(r) r.Field(Of Integer)("VehicleId")).ToArray()
                                                                                                    steps = "AuthorizationSequence 10"
                                                                                                    Dim isVehicleIdInVehicleArray = vehicleArray.Contains(vehicleId)
                                                                                                    If isVehicleIdInVehicleArray Or IsVehicleNumberRequire = "False" Then ' veihcle id send by user not assigned to person -PersonVehicle mapping not match
                                                                                                        'IsOdometerRequire on screen mobile application
                                                                                                        Dim dtCustomer = New DataTable()
                                                                                                        dtCustomer = OBJMasterBAL.GetCustomerId(customerId)
                                                                                                        'log.Error("customerId : " & customerId)
                                                                                                        steps = "AuthorizationSequence 11"

                                                                                                        If Not dtCustomer Is Nothing Then
                                                                                                            If (RequestFrom = "A" Or RequestFrom = "I") Then

                                                                                                                'check valid department
                                                                                                                If Boolean.Parse(dtCustomer.Rows(0)("IsDepartmentRequire")) = True Then
                                                                                                                    Dim dtDept As DataTable = OBJMasterBAL.GetDeptDetails(" and D.CustomerId=" & customerId & " and D.Number = '" & DepartmentNumber & "'")
                                                                                                                    If Not dtDept Is Nothing Then
                                                                                                                        If (dtDept.Rows.Count <= 0) Then
                                                                                                                            'Department number is not valid
                                                                                                                            log.Error("ProcessRequest: AuthorizationSequence- Department number is not valid, please try again for IMEI_UDID=" & IMEI_UDID & ", Department number : " & DepartmentNumber)
                                                                                                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg17"), "Dept")
                                                                                                                            Return

                                                                                                                        End If
                                                                                                                    Else
                                                                                                                        'Department number is not valid
                                                                                                                        log.Error("ProcessRequest: AuthorizationSequence- Department number is not valid, please try again for IMEI_UDID=" & IMEI_UDID & ", Department number : " & DepartmentNumber)
                                                                                                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg17"), "Dept")
                                                                                                                        Return

                                                                                                                    End If
                                                                                                                End If

                                                                                                                If (RequestFromAPP <> "AP") Then

                                                                                                                    'check valid person PIN
                                                                                                                    If Boolean.Parse(dtCustomer.Rows(0)("IsPersonnelPINRequire")) = True Then
                                                                                                                        Dim dtPerson As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and LTRIM(RTRIM(ANU.PinNumber))='" & PersonnelPIN.Trim() & "' and ANU.PersonId=" & personId, personId, RoleId, True) 'GetPersonalDetails(" where ANU.PinNumber=" & PersonnelPIN & "")
                                                                                                                        If Not dtPerson Is Nothing Then
                                                                                                                            If (dtPerson.Rows.Count <= 0) Then
                                                                                                                                'Person PIN is invalid
                                                                                                                                log.Error("ProcessRequest: AuthorizationSequence- Person PIN is invalid, please try again for IMEI_UDID=" & IMEI_UDID & ", Person PIN : " & PersonnelPIN.Trim())
                                                                                                                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg18"), "Pin")
                                                                                                                                Return

                                                                                                                            End If
                                                                                                                        Else
                                                                                                                            'Person PIN is invalid
                                                                                                                            log.Error("ProcessRequest: AuthorizationSequence- Person PIN is invalid, please try again for IMEI_UDID=" & IMEI_UDID & ", Person PIN : " & PersonnelPIN.Trim())
                                                                                                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg18"), "Pin")
                                                                                                                            Return

                                                                                                                        End If
                                                                                                                    End If
                                                                                                                End If


                                                                                                                'If (RequestFrom = "I") Then
                                                                                                                CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId, printerMacAddress, PrinterName, BluetoothCardReader, BluetoothCardReaderMacAddress)
                                                                                                                'Else
                                                                                                                '    If dtVehicle.Rows(0)("RequireOdometerEntry") = "Y" Then 'If Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire")) = True Then
                                                                                                                '        steps = "AuthorizationSequence 12"
                                                                                                                '        If (OdoMeter > Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer")))) Then

                                                                                                                '            If dtVehicle.Rows(0)("CheckOdometerReasonable").ToString() = "Y" Then 'check odo meter reasonable
                                                                                                                '                steps = "AuthorizationSequence 13"
                                                                                                                '                'Odometer reasonable?
                                                                                                                '                'Current Odometer +odolimit < OdoMeter
                                                                                                                '                steps = "AuthorizationSequence 14"

                                                                                                                '                Dim odoMeterValue As Integer
                                                                                                                '                odoMeterValue = (Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))) + Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("Odolimit")), 0, dtVehicle.Rows(0)("Odolimit"))))
                                                                                                                '                steps = "AuthorizationSequence 16"
                                                                                                                '                If odoMeterValue <= OdoMeter Then
                                                                                                                '                    steps = "AuthorizationSequence 17"
                                                                                                                '                    ''proper fuel type
                                                                                                                '                    CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId) ', Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire"))
                                                                                                                '                Else
                                                                                                                '                    'Bad odometer, try again
                                                                                                                '                    log.Error("ProcessRequest: AuthorizationSequence- Bad odometer, please try again for IMEI_UDID=" & IMEI_UDID & ", Odometer : " & OdoMeter)
                                                                                                                '                    ErrorInAuthontication(context, "fail", "Bad odometer, please try again", "Odo")
                                                                                                                '                End If

                                                                                                                '            Else
                                                                                                                '                'proper fuel type
                                                                                                                '                CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId) ', Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire"))
                                                                                                                '            End If

                                                                                                                '        Else
                                                                                                                '            'Bad odometer, try again
                                                                                                                '            log.Error("ProcessRequest: AuthorizationSequence- Bad odometer. Entered odo meter reading is less than current odometer, please try again for IMEI_UDID=" & IMEI_UDID & ", Odometer : " & OdoMeter & ", Current Odometer : " & (Integer.Parse(dtVehicle.Rows(0)("CurrentOdometer"))))
                                                                                                                '            ErrorInAuthontication(context, "fail", "Bad odometer, please try again", "Odo")
                                                                                                                '        End If

                                                                                                                '    Else
                                                                                                                '        'proper fuel type
                                                                                                                '        CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId) ', Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire"))
                                                                                                                '    End If
                                                                                                                'End If
                                                                                                            Else
                                                                                                                'CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId)

                                                                                                                'need to delete when IPhone App is ready 
                                                                                                                If Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire")) = True Then
                                                                                                                    steps = "AuthorizationSequence 12"
                                                                                                                    If dtVehicle.Rows(0)("CheckOdometerReasonable").ToString() = "Y" Then 'check odo meter reasonable
                                                                                                                        steps = "AuthorizationSequence 13"
                                                                                                                        'Odometer reasonable?
                                                                                                                        'Current Odometer +odolimit < OdoMeter
                                                                                                                        steps = "AuthorizationSequence 14"

                                                                                                                        Dim odoMeterValue As Integer
                                                                                                                        odoMeterValue = (Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))) + Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("Odolimit")), 0, dtVehicle.Rows(0)("Odolimit"))))
                                                                                                                        steps = "AuthorizationSequence 16"
                                                                                                                        If odoMeterValue <= OdoMeter Then
                                                                                                                            steps = "AuthorizationSequence 17"
                                                                                                                            ''proper fuel type
                                                                                                                            CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId, printerMacAddress, PrinterName, BluetoothCardReader, BluetoothCardReaderMacAddress) ', Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire"))
                                                                                                                        Else
                                                                                                                            'Bad odometer, try again
                                                                                                                            log.Error("ProcessRequest: AuthorizationSequence- Bad odometer, please try again for IMEI_UDID=" & IMEI_UDID & ", Odometer : " & OdoMeter)
                                                                                                                            ErrorInAuthontication(context, "fail", "Bad odometer, please try again", "Odo")
                                                                                                                        End If

                                                                                                                    Else
                                                                                                                        'proper fuel type
                                                                                                                        CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId, printerMacAddress, PrinterName, BluetoothCardReader, BluetoothCardReaderMacAddress) ', Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire"))
                                                                                                                    End If
                                                                                                                Else
                                                                                                                    'proper fuel type
                                                                                                                    CheckFuelType(context, data, vehicleId, dtVehicle, personId, dt, RoleId, printerMacAddress, PrinterName, BluetoothCardReader, BluetoothCardReaderMacAddress) ', Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire"))
                                                                                                                End If
                                                                                                            End If


                                                                                                        Else
                                                                                                            'show no data found for customer
                                                                                                            log.Error("ProcessRequest: AuthorizationSequence- No data found for company,please try again for IMEI_UDID=" & IMEI_UDID & ", customer Id-" & customerId)
                                                                                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg19"))
                                                                                                        End If
                                                                                                    Else
                                                                                                        log.Debug("ProcessRequest: AuthorizationSequence- 1 The user is not authorized for this vehicle for IMEI_UDID=" & IMEI_UDID)
                                                                                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg20"), "Pin")
                                                                                                        'ErrorInAuthontication(context, "fail", "The user is not authorized for this vehicle, please contact administrator.", "Pin")
                                                                                                    End If
                                                                                                Else
                                                                                                    log.Debug("ProcessRequest: AuthorizationSequence- The user is not authorized for this vehicle for IMEI_UDID=" & IMEI_UDID)
                                                                                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg20"), "Pin")
                                                                                                    'ErrorInAuthontication(context, "fail", "The user is not authorized for this vehicle, please contact administrator.", "Pin")
                                                                                                End If
                                                                                            Else
                                                                                                log.Debug("ProcessRequest: AuthorizationSequence- The user is not authorized for this vehicle for IMEI_UDID=" & IMEI_UDID)
                                                                                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg20"), "Pin")
                                                                                                'ErrorInAuthontication(context, "fail", "The user is not authorized for this vehicle, please contact administrator.", "Pin")
                                                                                            End If
                                                                                        Else
                                                                                            log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel time for IMEI_UDID=" & IMEI_UDID)
                                                                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                                            'ErrorInAuthontication(context, "fail", "Unauthorized fuel time, please contact administrator.")
                                                                                        End If
                                                                                    Else
                                                                                        log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel time for IMEI_UDID=" & IMEI_UDID)
                                                                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                                        'ErrorInAuthontication(context, "fail", "Unauthorized fuel time, please contact administrator.")
                                                                                    End If
                                                                                Else
                                                                                    log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel time for IMEI_UDID=" & IMEI_UDID)
                                                                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                                    'ErrorInAuthontication(context, "fail", "Unauthorized fuel time, please contact administrator.")
                                                                                End If
                                                                            Else
                                                                                log.Debug("ProcessRequest: AuthorizationSequence- Fuel timings are not assigned for this hose, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg22"), "Pin")
                                                                                'ErrorInAuthontication(context, "fail", "Fuel timings are not assigned for this hose, please contact administrator")
                                                                            End If
                                                                        Else
                                                                            log.Debug("ProcessRequest: AuthorizationSequence- Fuel timings are not assigned for this hose, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg22"), "Pin")
                                                                            'ErrorInAuthontication(context, "fail", "Fuel timings are not assigned for this hose, please contact administrator.")
                                                                        End If
                                                                    Else
                                                                        log.Debug("ProcessRequest: AuthorizationSequence- Fuel timings are not assigned, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                        'ErrorInAuthontication(context, "fail", "Fuel timings are not assigned, please contact administrator.")
                                                                    End If
                                                                Else
                                                                    log.Debug("ProcessRequest: AuthorizationSequence- Fuel timings are not assigned, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                    'ErrorInAuthontication(context, "fail", "Fuel timings are not assigned, please contact administrator.")
                                                                End If
                                                            Else
                                                                log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel day. IMEI_UDID=" & IMEI_UDID)
                                                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg23"), "Pin")
                                                                'ErrorInAuthontication(context, "fail", "Unauthorized fuel day, Please contact administrator.")
                                                            End If
                                                        Else
                                                            log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel day. IMEI_UDID=" & IMEI_UDID)
                                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg23"), "Pin")
                                                            'ErrorInAuthontication(context, "fail", "Unauthorized fuel day, Please contact administrator.")
                                                        End If
                                                    Else
                                                        log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel location for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg23"), "Pin")
                                                        'ErrorInAuthontication(context, "fail", "Unauthorized fuel day, Please contact administrator.")
                                                    End If
                                                Else
                                                    log.Debug("ProcessRequest: AuthorizationSequence- Valid Vehicle but not Authorized for this Hose. for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg24"), "Vehicle")
                                                    'ErrorInAuthontication(context, "fail", "Valid Vehicle but not Authorized for this Hose, Please contact administrator.", "Vehicle") 'Unauthorized fuel location.
                                                End If
                                            Else
                                                log.Debug("ProcessRequest: AuthorizationSequence- Valid Vehicle but not Authorized for this Hose for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg24"), "Vehicle")
                                                'ErrorInAuthontication(context, "fail", "Valid Vehicle but not Authorized for this Hose, Please contact administrator.", "Vehicle") 'Unauthorized fuel location.
                                            End If
                                        Else
                                            log.Debug("ProcessRequest: AuthorizationSequence- 2 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI_UDID)
                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg25"), "Vehicle")
                                            'ErrorInAuthontication(context, "fail", "Unauthorized vehicle. Please contact administrator.", "Vehicle")
                                        End If
                                    Else
                                        log.Debug("ProcessRequest: AuthorizationSequence-3 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI_UDID)
                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg25"), "Vehicle")
                                        'ErrorInAuthontication(context, "fail", "Unauthorized vehicle, Please contact administrator.", "Vehicle")
                                    End If
                                Else
                                    log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel location for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg26"), "Pin")
                                    'ErrorInAuthontication(context, "fail", "Unauthorized fuel location, Please contact administrator.")
                                End If
                            Else
                                log.Debug("ProcessRequest: AuthorizationSequence- Unauthorized fuel location for IMEI_UDID=" & IMEI_UDID)
                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg26"), "Pin")
                                'ErrorInAuthontication(context, "fail", "Unauthorized fuel location, Please contact administrator.")
                            End If
                        Else
                            log.Debug("ProcessRequest: AuthorizationSequence- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: AuthorizationSequence- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: AuthorizationSequence- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: AuthorizationSequence- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using
    End Sub

    Private Sub CheckFuelType(context As HttpContext, data As String, VehicleId As Integer, dtVehicle As DataTable, PersonId As Integer, dt As DataTable, RoleId As String, printerMacAddress As String, PrinterName As String, BluetoothCardReader As String, BluetoothCardReaderMacAddress As String) ', IsOdometerRequire As Boolean
        Dim javaScriptSerializer = New JavaScriptSerializer()
        Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(AuthorizationSequenceModel))
        Dim IMEI_UDID = DirectCast(serJsonDetails, AuthorizationSequenceModel).IMEIUDID
        Dim VehicleNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).VehicleNumber
        Dim OdoMeter = DirectCast(serJsonDetails, AuthorizationSequenceModel).OdoMeter
        Dim WifiSSId = DirectCast(serJsonDetails, AuthorizationSequenceModel).WifiSSId
        Dim SiteId = DirectCast(serJsonDetails, AuthorizationSequenceModel).SiteId

        Dim TransactionFrom = DirectCast(serJsonDetails, AuthorizationSequenceModel).RequestFrom
        Dim CurrentLat = DirectCast(serJsonDetails, AuthorizationSequenceModel).CurrentLat
        Dim CurrentLng = DirectCast(serJsonDetails, AuthorizationSequenceModel).CurrentLng
        Dim DepartmentNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).DepartmentNumber
        Dim PersonnelPIN = DirectCast(serJsonDetails, AuthorizationSequenceModel).PersonnelPIN
        Dim Other = DirectCast(serJsonDetails, AuthorizationSequenceModel).Other
        Dim Hours = DirectCast(serJsonDetails, AuthorizationSequenceModel).Hours

        If (Not PersonnelPIN Is Nothing) Then
            PersonnelPIN = PersonnelPIN.Trim()
        Else
            PersonnelPIN = ""
        End If

        If (Not VehicleNumber Is Nothing) Then
            VehicleNumber = VehicleNumber.Trim()
        Else
            VehicleNumber = ""
        End If


        Dim CurrentLocationAddress As String = ""

        If (DepartmentNumber = Nothing) Then
            DepartmentNumber = ""
        End If
        OBJMasterBAL = New MasterBAL()
        steps = "CheckFuelType 1"
        Dim dtHose = OBJMasterBAL.GetHoseByCondition(" And LTRIM(RTRIM(h.WifiSSId)) ='" & WifiSSId.ToString().Trim().Replace("'", "''") & "' and s.SiteID =" & SiteId.ToString() & "", PersonId, RoleId)
        steps = "CheckFuelType 2"
        If Not dtHose Is Nothing Then ''Hose does not exists
            If dtHose.Rows.Count <> 0 Then ''Hose does not exists
                'Dim dtFuelTypeVehicle = New DataTable()
                'dtFuelTypeVehicle = OBJMasterBAL.GetFuelTypeVehicleMapping(VehicleId)
                'If Not dtFuelTypeVehicle Is Nothing Then ' vehicle and fuel type mapping does not exists
                Dim fuelTypeOfHose As Integer
                fuelTypeOfHose = Integer.Parse(dtHose.Rows(0)("FuelTypeId").ToString())
                'steps = "CheckFuelType 3"
                'Dim fuelTypeArray = dtFuelTypeVehicle.AsEnumerable().[Select](Function(r) r.Field(Of Integer)("FuelTypeId")).ToArray()
                'Dim isFuelTypeInFuelTypeArray = fuelTypeArray.Contains(fuelTypeOfHose)
                'If isFuelTypeInFuelTypeArray Then 'Wrong fuel type try different hose
                steps = "CheckFuelType 4"
                'get vehicle fuel limit per day
                'check vehicle fuel limit per day
                Dim vehicleFuelLimitForDay As Integer
                vehicleFuelLimitForDay = Integer.Parse(dtVehicle.Rows(0)("FuelLimitPerDay").ToString())
                steps = "CheckFuelType 5"
                Dim personFuelLimitForDay As Integer
                personFuelLimitForDay = Integer.Parse(dt.Rows(0)("FuelLimitPerDay").ToString())
                Dim phoneNumber As String = dt.Rows(0)("PhoneNumber").ToString()
                Dim dsTransactionFuelLimitForDay = New DataSet()
                dsTransactionFuelLimitForDay = OBJMasterBAL.GetSumOfFuelQuantity(PersonId, VehicleId) 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                steps = "CheckFuelType 6"
                If Not dsTransactionFuelLimitForDay Is Nothing Then
                    If dsTransactionFuelLimitForDay.Tables.Count = 2 Then
                        If Not dsTransactionFuelLimitForDay.Tables(0) Is Nothing And Not dsTransactionFuelLimitForDay.Tables(1) Is Nothing Then
                            If dsTransactionFuelLimitForDay.Tables(0).Rows.Count <> 0 And dsTransactionFuelLimitForDay.Tables(1).Rows.Count <> 0 Then
                                If Decimal.Parse(dsTransactionFuelLimitForDay.Tables(0).Rows(0)("FuelQuantityOfPerson").ToString()) < personFuelLimitForDay Or personFuelLimitForDay = 0 Then
                                    If Decimal.Parse(dsTransactionFuelLimitForDay.Tables(1).Rows(0)("FuelQuantityOfVehicle").ToString()) < vehicleFuelLimitForDay Or vehicleFuelLimitForDay = 0 Then
                                        steps = "CheckFuelType 7"
                                        'calculate min fuel limit per day
                                        '0 means unlimited
                                        Dim minLimitForPerson As Integer
                                        Dim minLimitForVehicle As Integer
                                        If personFuelLimitForDay <> 0 Then
                                            minLimitForPerson = personFuelLimitForDay - Decimal.Parse(dsTransactionFuelLimitForDay.Tables(0).Rows(0)("FuelQuantityOfPerson").ToString())
                                        Else
                                            minLimitForPerson = 0
                                        End If
                                        steps = "CheckFuelType 7_1"

                                        If vehicleFuelLimitForDay <> 0 Then
                                            minLimitForVehicle = vehicleFuelLimitForDay - Decimal.Parse(dsTransactionFuelLimitForDay.Tables(1).Rows(0)("FuelQuantityOfVehicle").ToString())
                                        Else
                                            minLimitForVehicle = 0
                                        End If
                                        steps = "CheckFuelType 7_2"
                                        Dim minLimit As Integer
                                        If minLimitForPerson = 0 Then
                                            minLimit = minLimitForVehicle
                                        ElseIf minLimitForVehicle = 0 Then
                                            minLimit = minLimitForPerson
                                        ElseIf minLimitForPerson <= minLimitForVehicle Then
                                            minLimit = minLimitForPerson
                                        Else
                                            minLimit = minLimitForVehicle
                                        End If
                                        steps = "CheckFuelType 7_3"
                                        'calculate min fuel limit per transaction
                                        Dim personFuellimitPerTran As Integer
                                        personFuellimitPerTran = 0
                                        If Not dt.Rows(0)("FuelLimitPerTxn".ToString()) Is Nothing Then
                                            personFuellimitPerTran = Integer.Parse(dt.Rows(0)("FuelLimitPerTxn".ToString()))
                                        End If
                                        steps = "CheckFuelType 7_4"
                                        Dim vehicleFuellimitPerTran As Integer
                                        vehicleFuellimitPerTran = 0
                                        If Not dtVehicle.Rows(0)("FuelLimitPerTxn").ToString() Is Nothing Then
                                            vehicleFuellimitPerTran = Integer.Parse(dtVehicle.Rows(0)("FuelLimitPerTxn").ToString())
                                        End If
                                        steps = "CheckFuelType 7_5"
                                        If minLimit = 0 Then
                                            If personFuellimitPerTran = 0 Then
                                                minLimit = vehicleFuellimitPerTran
                                            ElseIf vehicleFuellimitPerTran = 0 Then
                                                minLimit = personFuellimitPerTran
                                            ElseIf personFuellimitPerTran <= vehicleFuellimitPerTran Then
                                                minLimit = personFuellimitPerTran
                                            Else
                                                minLimit = vehicleFuellimitPerTran
                                            End If
                                        Else
                                            If minLimit >= personFuellimitPerTran And personFuellimitPerTran <> 0 Then
                                                minLimit = personFuellimitPerTran
                                            End If

                                            If minLimit >= vehicleFuellimitPerTran And vehicleFuellimitPerTran <> 0 Then
                                                minLimit = vehicleFuellimitPerTran
                                            End If
                                        End If

                                        steps = "CheckFuelType 7_6"


                                        'If personFuellimitPerTran < minLimit Then
                                        '    minLimit = personFuellimitPerTran
                                        'End If
                                        'If vehicleFuellimitPerTran < minLimit Then
                                        '    minLimit = vehicleFuellimitPerTran
                                        'End If


                                        Dim pulseRatio As Decimal
                                        Dim transactionDate As DateTime = DateTime.UtcNow.ToString()
                                        pulseRatio = Decimal.Parse(dtHose.Rows(0)("PulserRatio").ToString())
                                        Dim rootOject = New RootObject()
                                        rootOject.ResponceMessage = "success"
                                        rootOject.ResponceText = "All authentication complete"
                                        rootOject.ValidationFailFor = ""
                                        rootOject.ResponceData = New ResponceData()
                                        rootOject.ResponceData.MinLimit = minLimit
                                        steps = "CheckFuelType 7_6_1"
                                        rootOject.ResponceData.PulseRatio = pulseRatio
                                        rootOject.ResponceData.VehicleId = VehicleId
                                        rootOject.ResponceData.PersonId = PersonId
                                        rootOject.ResponceData.FuelTypeId = fuelTypeOfHose
                                        steps = "CheckFuelType 7_6_2"
                                        rootOject.ResponceData.PhoneNumber = phoneNumber
                                        steps = "CheckFuelType 7_6_3"
                                        rootOject.ResponceData.ServerDate = transactionDate
                                        rootOject.ResponceData.PumpOnTime = dtHose.Rows(0)("PumpOnTime")
                                        rootOject.ResponceData.PumpOffTime = dtHose.Rows(0)("PumpOffTime")
                                        rootOject.ResponceData.IsTLDCall = IIf(dtHose.Rows(0)("PROBEMacAddress").ToString() = "", "False", "True")
                                        rootOject.ResponceData.PulserStopTime = ConfigurationManager.AppSettings("PulserStopTime").ToString()


                                        steps = "CheckFuelType 7_7"

                                        Dim json As String

                                        ''IsOdometerRequire on screen mobile application
                                        'If IsOdometerRequire = True Then
                                        '    'update current odometer
                                        '    OBJMasterBAL.UpdateVehicleCurrentOdometer(VehicleId, OdoMeter)
                                        'End If

                                        'make IsBusy is true
                                        'Try

                                        '    Dim OBJWebServiceBAL = New WebServiceBAL()

                                        '    OBJWebServiceBAL.ChangeBusyStatusOfFluidSecureUnit(SiteId, True)


                                        'Catch ex As Exception
                                        '    log.Error("Error occurred in ChangeBusyStatusOfFluidSecureUnit. Exception is :" & ex.Message)
                                        'End Try

                                        CurrentLocationAddress = GetLocationAddress(CurrentLat, CurrentLng)

                                        Dim TransactionId As Integer = 0
                                        OBJMasterBAL = New MasterBAL()

                                        Dim HubId As Integer = 0

                                        If Not DirectCast(serJsonDetails, AuthorizationSequenceModel).HubId = Nothing Then
                                            HubId = DirectCast(serJsonDetails, AuthorizationSequenceModel).HubId
                                        End If
                                        Dim dsTransactionValuesData As DataSet
                                        Dim DepartmentName As String = ""
                                        Dim FuelTypeName As String = ""
                                        Dim Email As String = ""
                                        Dim PersonName As String = ""
                                        Dim CompanyName As String = ""
                                        Dim VehicleSum As Decimal = 0
                                        Dim DeptSum As Decimal = 0
                                        Dim VehPercentage As Decimal = 0
                                        Dim DeptPercentage As Decimal = 0
                                        Dim SurchargeType As String = "0"
                                        Dim ProductPrice As Decimal = 0

                                        dsTransactionValuesData = OBJMasterBAL.GetTransactionColumnsValueForSave(DepartmentNumber, fuelTypeOfHose, PersonId, VehicleId)

                                        If dsTransactionValuesData IsNot Nothing Then
                                            If dsTransactionValuesData.Tables.Count > 0 Then
                                                If dsTransactionValuesData.Tables(0) IsNot Nothing And dsTransactionValuesData.Tables(0).Rows.Count > 0 Then
                                                    DepartmentName = dsTransactionValuesData.Tables(0).Rows(0)("DeptName").ToString()
                                                    DepartmentNumber = dsTransactionValuesData.Tables(0).Rows(0)("DeptNumber").ToString()
                                                    VehicleSum = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("VehicleSum").ToString())
                                                    DeptSum = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("DeptSum").ToString())
                                                    VehPercentage = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("VehPercentage").ToString())
                                                    DeptPercentage = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("DeptPercentage").ToString())
                                                    SurchargeType = dsTransactionValuesData.Tables(0).Rows(0)("SurchargeType").ToString()
                                                End If
                                                If dsTransactionValuesData.Tables(1) IsNot Nothing And dsTransactionValuesData.Tables(1).Rows.Count > 0 Then
                                                    FuelTypeName = dsTransactionValuesData.Tables(1).Rows(0)("FuelTypeName").ToString()
                                                    ProductPrice = Decimal.Parse(dsTransactionValuesData.Tables(1).Rows(0)("ProductPrice").ToString())
                                                End If
                                                If dsTransactionValuesData.Tables(2) IsNot Nothing And dsTransactionValuesData.Tables(2).Rows.Count > 0 Then
                                                    Email = dsTransactionValuesData.Tables(2).Rows(0)("Email").ToString()
                                                    PersonName = dsTransactionValuesData.Tables(2).Rows(0)("PersonName").ToString()
                                                End If
                                                If dsTransactionValuesData.Tables(3) IsNot Nothing And dsTransactionValuesData.Tables(3).Rows.Count > 0 Then
                                                    CompanyName = dsTransactionValuesData.Tables(3).Rows(0)("CompanyName").ToString()
                                                End If
                                            End If
                                        End If
                                        steps = "CheckFuelType 7_8"
                                        TransactionId = OBJMasterBAL.InsertUpdateTransaction(VehicleId, SiteId, PersonId, OdoMeter, 0, fuelTypeOfHose, phoneNumber, WifiSSId.ToString().Trim(), transactionDate,
                                                                0, 0, TransactionFrom, 0, Convert.ToDouble(CurrentLat).ToString("0.00000"), Convert.ToDouble(CurrentLng).ToString("0.00000"), CurrentLocationAddress,
                                                              IIf(VehicleNumber.Trim().ToLower().Contains("guest"), VehicleNumber.Trim(), dtVehicle.Rows(0)("VehicleNumber").ToString().Trim()), DepartmentNumber, PersonnelPIN.Trim(), Other, IIf(Hours = "", -1, Hours), True, False, 0, HubId, 0,
                                                                 dtVehicle.Rows(0)("VehicleName").ToString(), DepartmentName, FuelTypeName, Email, PersonName, CompanyName, 0, Convert.ToInt32(dsTransactionValuesData.Tables(3).Rows(0)("CustomerId")), 0, 0, 0) '

                                        If (TransactionId = 0) Then
                                            log.Debug("ProcessRequest: AuthorizationSequence- Error occcured in saving transactions.")

                                            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))
                                            Return
                                        End If

                                        Dim dtSingleTransacton As DataTable = New DataTable()
                                        dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId, False)

                                        If (Not dtSingleTransacton Is Nothing) Then
                                            If (dtSingleTransacton.Rows.Count > 0) Then
                                                transactionDate = dtSingleTransacton.Rows(0)("TransactionDateTime")
                                            End If
                                        End If



                                        Dim CreateDataFor As String = ""
                                        CreateDataFor = VehicleNumber.Trim() & ";" & dtVehicle.Rows(0)("VehicleName").ToString() & ";" & DepartmentName & ";" & DepartmentNumber & ";" & VehicleNumber.Trim() & ";" & WifiSSId.ToString().Trim() & ";" & "0" & ";" &
                                                        Other & ";" & CompanyName & ";" & "" & ";" & Convert.ToDateTime(transactionDate).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(transactionDate).ToString("hh:mm:tt") & ";" &
                                                        PersonName & ";" & PersonnelPIN.Trim() & ";" & OdoMeter & ";" & "" & ";" & Hours & ";" & FuelTypeName & ";" & "Completed" & ";" & "0"

                                        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                            Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                                            CSCommonHelper.WriteLog("Added", "Transactions", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "success", "")
                                        End If

                                        log.Error("TransactionId" & TransactionId)

                                        rootOject.ResponceData.ServerDate = transactionDate
                                        rootOject.ResponceData.TransactionId = TransactionId

                                        OBJMasterBAL = New MasterBAL()
                                        Dim dtFirmwares As DataTable = New DataTable()
                                        dtFirmwares = OBJMasterBAL.GetLaunchedFirmwareDetails()
                                        Dim FirmwareVersion As String = ""
                                        Dim FilePath As String = ""

                                        If (Not dtFirmwares Is Nothing) Then
                                            If (dtFirmwares.Rows.Count > 0) Then
                                                FirmwareVersion = dtFirmwares.Rows(0)("Version")
                                                FilePath = "" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority & dtFirmwares.Rows(0)("FirmwareFilePath")
                                            End If
                                        End If
                                        steps = "CheckFuelType 7_9"
                                        rootOject.ResponceData.FirmwareVersion = FirmwareVersion
                                        rootOject.ResponceData.FilePath = FilePath
                                        rootOject.ResponceData.FOBNumber = dtVehicle.Rows(0)("FOBNumber").ToString().Replace(" ", "")
                                        rootOject.ResponceData.Company = CompanyName
                                        rootOject.ResponceData.Location = dtHose.Rows(0)("SiteAddress")
                                        rootOject.ResponceData.PersonName = PersonName
                                        rootOject.ResponceData.BluetoothCardReader = BluetoothCardReader 'dtHose.Rows(0)("BluetoothCardReader")
                                        rootOject.ResponceData.BluetoothCardReaderMacAddress = BluetoothCardReaderMacAddress.ToUpper() 'dtHose.Rows(0)("BluetoothCardReader")
                                        rootOject.ResponceData.PrinterName = PrinterName 'dt.Rows(0)("PrinterName").ToString() 'dtHose.Rows(0)("PrinterName")
                                        rootOject.ResponceData.PrinterMacAddress = printerMacAddress.ToLower()
                                        rootOject.ResponceData.VehicleSum = VehicleSum
                                        rootOject.ResponceData.DeptSum = DeptSum
                                        rootOject.ResponceData.VehPercentage = VehPercentage
                                        rootOject.ResponceData.DeptPercentage = DeptPercentage
                                        rootOject.ResponceData.SurchargeType = SurchargeType
                                        rootOject.ResponceData.ProductPrice = ProductPrice

                                        json = javaScriptSerializer.Serialize(rootOject)
                                        log.Info("AUTH RETURN JSON : " & json)
                                        context.Response.Write(json)
                                        steps = "CheckFuelType 7_10"
                                    Else
                                        log.Debug("ProcessRequest: AuthorizationSequence- Vehicle fuel limit for the day exceeded.")
                                        'context.Response.Write("Transaction Not Found") 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg30"))
                                    End If
                                Else
                                    log.Debug("ProcessRequest: AuthorizationSequence- Personal fuel limit for the day exceeded.")
                                    'context.Response.Write("Transaction Not Found") 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg31"))
                                End If
                            Else
                                log.Debug("ProcessRequest: AuthorizationSequence- Transaction Not Found")
                                'context.Response.Write("Transaction Not Found") 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg51"))
                            End If
                        End If
                    Else
                        log.Debug("ProcessRequest: AuthorizationSequence- Transaction Not Found")
                        'context.Response.Write("Transaction Not Found") 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg51"))
                    End If
                Else
                    log.Debug("ProcessRequest: AuthorizationSequence- Transaction Not Found")
                    'context.Response.Write("Transaction Not Found") 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg51"))
                End If
                'Else
                '    log.Debug("ProcessRequest: AuthorizationSequence- Wrong fuel type try different hose")
                '    'context.Response.Write("Wrong fuel type try different hose") 'Wrong fuel type try different hose
                '    ErrorInAuthontication(context, "fail", "Wrong fuel type, try a different hose")
                'End If
                'Else
                '    log.Debug("ProcessRequest: AuthorizationSequence- vehicle and fuel type mapping does not exists")
                '    'context.Response.Write("Unauthorized Vehicle") 'vehicle and fuel type mapping does not exists
                '    ErrorInAuthontication(context, "fail", "Fuel types are not assigned for this vehicle.")
                'End If
            Else
                log.Debug("ProcessRequest: AuthorizationSequence- Hose does not exists")
                'context.Response.Write("Unauthorized Vehicle") 'Hose does not exists
                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg32"))
            End If
        Else
            log.Debug("ProcessRequest: AuthorizationSequence- IMEI_UDID does not exist")
            'context.Response.Write("IMEI_UDID not exists") 'Hose does not exists
            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg32"))
        End If

    End Sub

    Private Sub ErrorInAuthontication(context As HttpContext, ResponceMessage As String, errorString As String, Optional ValidationFailFor As String = "")
        Dim javaScriptSerializer = New JavaScriptSerializer()
        Dim rootOject = New RootObject()
        rootOject.ResponceMessage = ResponceMessage
        rootOject.ResponceText = errorString
        rootOject.ValidationFailFor = ValidationFailFor
        rootOject.ResponceData = New ResponceData()
        rootOject.ResponceData.MinLimit = 0
        rootOject.ResponceData.PulseRatio = 0
        rootOject.ResponceData.VehicleId = 0
        rootOject.ResponceData.PersonId = 0
        rootOject.ResponceData.FuelTypeId = 0
        rootOject.ResponceData.PhoneNumber = ""
        rootOject.ResponceData.ServerDate = DateTime.UtcNow.ToString()
        rootOject.ResponceData.PulserStopTime = ""
        rootOject.ResponceData.PumpOnTime = ""
        rootOject.ResponceData.PumpOffTime = ""
        rootOject.ResponceData.TransactionId = 0
        rootOject.ResponceData.FirmwareVersion = ""
        rootOject.ResponceData.FilePath = ""
        rootOject.ResponceData.FOBNumber = ""
        rootOject.ResponceData.Company = ""
        rootOject.ResponceData.Location = ""
        rootOject.ResponceData.PersonName = ""
        rootOject.ResponceData.FluidSecureSiteName = ""

        rootOject.ResponceData.BluetoothCardReader = ""
        rootOject.ResponceData.BluetoothCardReaderMacAddress = ""
        rootOject.ResponceData.PrinterName = ""
        rootOject.ResponceData.LFBluetoothCardReader = ""
        rootOject.ResponceData.LFBluetoothCardReaderMacAddress = ""
        rootOject.ResponceData.VeederRootMacAddress = ""
        rootOject.ResponceData.CollectDiagnosticLogs = "False"
        rootOject.ResponceData.IsGateHub = "False"
        rootOject.ResponceData.IsVehicleNumberRequire = "False"

        Dim json As String
        json = javaScriptSerializer.Serialize(rootOject)
        context.Response.Write(json)

    End Sub


    Private Sub CreateResponceForAndroidSSID(context As HttpContext, ResponceMessage As String, errorString As String, data As UserData)
        Dim javaScriptSerializer = New JavaScriptSerializer()
        Dim rootOject = New RootObject()
        rootOject.ResponceMessage = ResponceMessage
        rootOject.ResponceText = errorString
        rootOject.objUserData = New UserData()
        rootOject.objUserData.Email = data.Email
        rootOject.objUserData.PhoneNumber = data.PhoneNumber
        rootOject.objUserData.PersonName = data.PersonName
        rootOject.objUserData.FluidSecureSiteName = data.FluidSecureSiteName
        rootOject.objUserData.IsApproved = data.IsApproved
        rootOject.objUserData.IMEI_UDID = data.IMEI_UDID
        rootOject.objUserData.IsOdoMeterRequire = data.IsOdoMeterRequire
        rootOject.objUserData.IsLoginRequire = data.IsLoginRequire
        rootOject.objUserData.IsDepartmentRequire = data.IsDepartmentRequire
        rootOject.objUserData.IsPersonnelPINRequire = data.IsPersonnelPINRequire
        rootOject.objUserData.IsPersonnelPINRequireForHub = data.IsPersonnelPINRequireForHub
        rootOject.objUserData.IsOtherRequire = data.IsOtherRequire
        rootOject.objUserData.OtherLabel = data.OtherLabel
        rootOject.objUserData.IsVehicleHasFob = data.IsVehicleHasFob
        rootOject.objUserData.IsPersonHasFob = data.IsPersonHasFob
        rootOject.objUserData.TimeOut = ConfigurationManager.AppSettings("WaitingTime").ToString()
        rootOject.objUserData.AndroidAppLatestVersion = ConfigurationManager.AppSettings("AndroidAppLatestVersion").ToString()
        rootOject.objUserData.AppUpgradeMsgDisplayAfterDays = ConfigurationManager.AppSettings("AppUpgradeMsgDisplayAfterDays").ToString()
        rootOject.objUserData.PersonId = data.PersonId
        rootOject.objUserData.BluetoothCardReaderMacAddress = data.BluetoothCardReaderMacAddress.ToUpper()
        rootOject.objUserData.BluetoothCardReader = data.BluetoothCardReader
        rootOject.objUserData.LFBluetoothCardReader = data.LFBluetoothCardReader
        rootOject.objUserData.LFBluetoothCardReaderMacAddress = data.LFBluetoothCardReaderMacAddress
        rootOject.objUserData.VeederRootMacAddress = data.VeederRootMacAddress
        rootOject.objUserData.IsAccessForFOBApp = data.IsAccessForFOBApp
        rootOject.objUserData.CollectDiagnosticLogs = data.CollectDiagnosticLogs
        rootOject.objUserData.IsLogging = data.IsLogging
        rootOject.objUserData.WifiChannelToUse = data.WifiChannelToUse
        rootOject.objUserData.IsGateHub = data.IsGateHub
        rootOject.objUserData.IsVehicleNumberRequire = data.IsVehicleNumberRequire

        Dim json As String
        json = javaScriptSerializer.Serialize(rootOject)
        context.Response.Write(json)
    End Sub

#End Region

#Region "TransactionComplete"
    Private Sub TransactionComplete(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data
                log.Info("data : " & data)
                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim transactionCompleteResponceObj = New TransactionCompleteResponce()
                Dim json As String

                If (data = "") Then
                    log.Info("Transaction json not found. Transaction json : " & data)
                    transactionCompleteResponceObj.ResponceMessage = "success"
                    transactionCompleteResponceObj.ResponceText = "Transaction json not found."

                    json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                    context.Response.Write(json)
                    Return
                End If


                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(TransactionComplete))
                Dim FuelQuantity = DirectCast(serJsonDetails, TransactionComplete).FuelQuantity
                Dim Pulses = DirectCast(serJsonDetails, TransactionComplete).Pulses
                Dim TransactionId = DirectCast(serJsonDetails, TransactionComplete).TransactionId
                Dim IsFuelingStop = DirectCast(serJsonDetails, TransactionComplete).IsFuelingStop
                Dim IsLastTransaction = DirectCast(serJsonDetails, TransactionComplete).IsLastTransaction



                log.Debug("TransactionId : " & TransactionId)
                If (FuelQuantity <= 0) Then

                    transactionCompleteResponceObj.ResponceMessage = "success"
                    transactionCompleteResponceObj.ResponceText = "Transaction with fuel quantity 0 is not allowed."

                    json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                    context.Response.Write(json)
                    Return
                End If

                Dim sendTransEmail As Boolean = False

                If (TransactionId Is Nothing) Then
                    log.Debug("In null transaction" & data)
                    Dim returnJson1 As String = SaveFailedTransactions(data, IMEI_UDID)
                    If (returnJson1 = "") Then
                        ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))
                    Else
                        context.Response.Write(returnJson1)
                    End If
                Else
                    If (TransactionId <> "0" Or TransactionId = "") Then

                        Dim OBJWebServiceBAL = New WebServiceBAL()
                        Dim OBJMasterBAL = New MasterBAL()

                        Dim dtSingleTransacton As DataTable = New DataTable()
                        dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId, False)

                        If (dtSingleTransacton.Rows.Count = 0) Then
                            log.Error("TransactionId not found. TransactionId : " & TransactionId)

                            ErrorInAuthontication(context, "fail", "TransactionId not found.")

                            Return
                        End If
                        'make IsBusy is false
                        Try

                            OBJWebServiceBAL.ChangeBusyStatusOfFluidSecureUnit(dtSingleTransacton.Rows(0)("SiteId"), False, IMEI_UDID)

                        Catch ex As Exception
                            log.Error("Error occurred in ChangeBusyStatusOfFluidSecureUnit. Exception is :" & ex.Message)
                        End Try

                        Try

                            Dim dsIMEI = New DataSet()
                            OBJServiceBAL = New WebServiceBAL()
                            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                            Dim UserCustomerId As Integer = Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString())
                            Dim UserCustomerName As String = dsIMEI.Tables(0).Rows(0)("CustomerName").ToString()
                            Dim TransactionCustomerId As Integer = dtSingleTransacton.Rows(0)("CustomerId").ToString()
                            Dim TransactionCustomerName As String = dtSingleTransacton.Rows(0)("Company").ToString()

                            If (UserCustomerId <> TransactionCustomerId Or UserCustomerName <> TransactionCustomerName) Then

                                transactionCompleteResponceObj.ResponceMessage = "success"
                                transactionCompleteResponceObj.ResponceText = "Transaction from another company so ignore it!"
                                log.Error("Transaction from another company so ignore it. TransactionId : " & TransactionId & " Hub company Name : " & UserCustomerName & " Transaction company Name :  " & TransactionCustomerName)
                                json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                                context.Response.Write(json)

                                Return
                            End If
                        Catch ex As Exception
                            log.Error("Error occurred in checking Is Transaction from another company. Exception is :" & ex.Message)
                        End Try

                        'Try

                        '	If (dtSingleTransacton.Rows(0)("FuelQuantity") > 0) Then

                        '		transactionCompleteResponceObj.ResponceMessage = "success"
                        '		transactionCompleteResponceObj.ResponceText = "Stored transaction Fuel Quantity is greater than 0 in cloud, so skipped the transaction!"
                        '		log.Error("Stored transaction Fuel Quantity is greater than 0 in cloud, so skipped the transaction. TransactionId : " & TransactionId & " Original FuelQuantity : " & dtSingleTransacton.Rows(0)("FuelQuantity") & " New Fuel Quantity :  " & FuelQuantity)
                        '		json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                        '		context.Response.Write(json)

                        '		Return
                        '	End If
                        'Catch ex As Exception
                        '	log.Error("Error occurred in checking Transaction Fuel Quantity is greater than 0. Exception is :" & ex.Message)
                        'End Try


                        'Dim TransactionId As Integer
                        If (dtSingleTransacton.Rows(0)("FuelQuantity") > FuelQuantity) Then

                            FuelQuantity = dtSingleTransacton.Rows(0)("FuelQuantity")
                            Pulses = dtSingleTransacton.Rows(0)("Pulses")

                            transactionCompleteResponceObj.ResponceMessage = "success"
                            transactionCompleteResponceObj.ResponceText = "Stored transaction Fuel Quantity is greater than new fuel Quantity in cloud, so skipped the transaction!"
                            log.Error("Stored transaction Fuel Quantity is greater than new fuel Quantity in cloud, so skipped the transaction. TransactionId : " & TransactionId & " Original FuelQuantity : " & dtSingleTransacton.Rows(0)("FuelQuantity") & " New Fuel Quantity :  " & FuelQuantity)
                            json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                            context.Response.Write(json)

                            Return

                        End If
                        Dim EmailSubject As String = ""
                        If (IsLastTransaction Is Nothing Or IsLastTransaction = "" Or IsLastTransaction = "0") Then
                            If (IsFuelingStop Is Nothing Or IsFuelingStop = "") Then 'backward compatibility for not sending email to use if quantity is less than  one gallon of previous email quantity .
                                If (FuelQuantity > dtSingleTransacton.Rows(0)("FuelQuantity")) Then
                                    If (dtSingleTransacton.Rows(0)("MailSendFuelQuantity") = 0) Then
                                        sendTransEmail = True
                                        log.Info("EmailSubject 1" & EmailSubject)
                                    Else
                                        If (FuelQuantity > dtSingleTransacton.Rows(0)("MailSendFuelQuantity") + 1) Then
                                            sendTransEmail = True
                                            EmailSubject = "Updated Transaction Receipt"
                                            log.Info("EmailSubject 12" & EmailSubject)
                                        End If
                                    End If
                                End If
                            Else
                                If (IsFuelingStop = "1" And dtSingleTransacton.Rows(0)("IsMailSent") = True) Then
                                    If (FuelQuantity > (dtSingleTransacton.Rows(0)("MailSendFuelQuantity") + 1)) Then
                                        sendTransEmail = True
                                        EmailSubject = "Updated Transaction Receipt"
                                    End If
                                ElseIf (IsFuelingStop = "1" And dtSingleTransacton.Rows(0)("IsMailSent") = False) Then
                                    sendTransEmail = True
                                ElseIf (IsFuelingStop = "0" And dtSingleTransacton.Rows(0)("IsMailSent") = False) Then
                                    sendTransEmail = True
                                End If

                            End If
                        Else
                            If (dtSingleTransacton.Rows(0)("IsMailSent") = True) Then
                                EmailSubject = "Updated Transaction Receipt"
                            End If
                            If (FuelQuantity > (dtSingleTransacton.Rows(0)("MailSendFuelQuantity") + 1)) Then
                                sendTransEmail = True
                            End If
                        End If

                        If (Pulses = Nothing) Then
                            Pulses = 0
                        End If
                        Dim CreateDataFor As String = ""
                        CreateDataFor = dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("VehicleName").ToString() & ";" & dtSingleTransacton.Rows(0)("DeptName").ToString() & ";" &
                                    dtSingleTransacton.Rows(0)("DepartmentNumber").ToString() & ";" & dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim() & ";" &
                                    dtSingleTransacton.Rows(0)("FuelQuantity").ToString() & ";" & dtSingleTransacton.Rows(0)("Other").ToString() & ";" & dtSingleTransacton.Rows(0)("Company").ToString() & ";" & "" & ";" &
                                    Convert.ToDateTime(dtSingleTransacton.Rows(0)("Date").ToString()).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(dtSingleTransacton.Rows(0)("Time").ToString()).ToString("hh: mm:tt") &
                                    ";" & dtSingleTransacton.Rows(0)("PersonName").ToString() & ";" & dtSingleTransacton.Rows(0)("PersonPin").ToString().Trim() & ";" &
                                    dtSingleTransacton.Rows(0)("CurrentOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("PreviousOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("Hours").ToString() & ";" &
                                    dtSingleTransacton.Rows(0)("FuelType").ToString() & ";" & dtSingleTransacton.Rows(0)("TranStatus").ToString() & ";" & dtSingleTransacton.Rows(0)("Pulses").ToString()

                        Dim Beforedata = CreateData(TransactionId, CreateDataFor)

                        TransactionId = OBJMasterBAL.InsertUpdateTransaction(dtSingleTransacton.Rows(0)("VehicleId"), 0, 0, dtSingleTransacton.Rows(0)("CurrentOdometer"), FuelQuantity, 0, "", "", DateTime.Now, TransactionId, dtSingleTransacton.Rows(0)("PersonId"),
                                                                            "", dtSingleTransacton.Rows(0)("PreviousOdometer"), "", "", "", "", "", "", "", 0, False, True, 0, 0, Pulses, "", "", "", "", "", "", 0, 0, 0, 0, 0) '

                        If TransactionId <> 0 Then 'success
                            Try

                                'IsOdometerRequire on screen mobile application
                                'OBJMasterBAL.UpdateVehicleCurrentOdometer(VehicleId, CurrentOdometer)

                                'Dim dtSingleTransacton As DataTable = New DataTable()
                                'dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId)
                                If (sendTransEmail = True) Then
                                    If (IIf(IsDBNull(dtSingleTransacton.Rows(0)("SendTransactionEmail")), False, dtSingleTransacton.Rows(0)("SendTransactionEmail")) = True) Then
                                        SendTransactionEmail(dtSingleTransacton.Rows(0)("Email"), dtSingleTransacton.Rows(0)("Date"), dtSingleTransacton.Rows(0)("Time"),
                                                        dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim(), dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim(), dtSingleTransacton.Rows(0)("FuelType"),
                                                        dtSingleTransacton.Rows(0)("TransactionNumber"), Math.Round(FuelQuantity, 2), dtSingleTransacton.Rows(0)("AdditionalEmailId"), dtSingleTransacton.Rows(0)("IsUserForHub"),
                                                        dtSingleTransacton.Rows(0)("IsOtherRequire"), dtSingleTransacton.Rows(0)("OtherLabel"), dtSingleTransacton.Rows(0)("Other"), dtSingleTransacton.Rows(0)("PersonName"),
                                                        dtSingleTransacton.Rows(0)("VehicleName"), EmailSubject, TransactionId)

                                        OBJMasterBAL = New MasterBAL()

                                        OBJMasterBAL.SetIsTransactionMailSent(TransactionId, FuelQuantity)

                                    End If
                                End If
                                'CreateDataFor = "Vehicle Number; Vehicle Name; Department; Department Number; Guest Vehicle Number; FluidSecure Link; Fuel Quantity; Other; Company; Cost; Transaction Date;
                                'Transaction Time; Person; Person PIN; Current Odometer; Previous Odometer; Hours; Fuel Type; Transaction Status;"

                                CreateDataFor = dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("VehicleName").ToString() & ";" & dtSingleTransacton.Rows(0)("DeptName").ToString() & ";" &
                                    dtSingleTransacton.Rows(0)("DepartmentNumber").ToString() & ";" & dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim() & ";" &
                                    FuelQuantity & ";" & dtSingleTransacton.Rows(0)("Other").ToString() & ";" & dtSingleTransacton.Rows(0)("Company").ToString() & ";" & "" & ";" &
                                    Convert.ToDateTime(dtSingleTransacton.Rows(0)("Date").ToString()).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(dtSingleTransacton.Rows(0)("Time").ToString()).ToString("hh: mm:tt") &
                                    ";" & dtSingleTransacton.Rows(0)("PersonName").ToString() & ";" & dtSingleTransacton.Rows(0)("PersonPin").ToString().Trim() & ";" &
                                    dtSingleTransacton.Rows(0)("CurrentOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("PreviousOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("Hours").ToString() & ";" &
                                    dtSingleTransacton.Rows(0)("FuelType").ToString() & ";" & "Completed" & ";" & Pulses

                                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                    Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                                    CSCommonHelper.WriteLog("Modified", "Transactions", Beforedata, writtenData, dtSingleTransacton.Rows(0)("PersonName").ToString() & "(" & dtSingleTransacton.Rows(0)("Email").ToString() & ")", IPAddress, "success", "")
                                End If


                            Catch ex As Exception
                                log.Debug("Exception occurred in sending transaction. Transaction # " & TransactionId & ". ex is :" & ex.Message)
                            End Try
                            'If IsOdometerRequire = True Then
                            '    'update current odometer

                            'End If
                            transactionCompleteResponceObj.ResponceMessage = "success"
                            transactionCompleteResponceObj.ResponceText = "Transaction completed successfully!"
                        Else 'fail
                            transactionCompleteResponceObj.ResponceMessage = "fail"
                            transactionCompleteResponceObj.ResponceText = "Error occurred during transaction!"

                            Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

                            'Dim logtransaction As String = String.Format("Failed transactions details: VehicleId : {0},SiteId:{1},PersonId:{2}, CurrentOdometer:{3}, FuelQuantity:{4}, FuelTypeId:{5}, PhoneNumber:{6}, WifiSSId:{7}," &
                            '                                             "TransactionDate:{8},TransactionFrom:{9},CurrentLat:{10},CurrentLng:{11},VehicleNumber:{12},DepartmentNumber:{13},PersonnelPIN:{14},Hours:{15}",
                            '                                             VehicleId, SiteId, PersonId, CurrentOdometer, FuelQuantity, FuelTypeId, PhoneNumber, WifiSSId, TransactionDate, TransactionFrom, CurrentLat, CurrentLng, VehicleNumber,
                            '                                             DepartmentNumber, PersonnelPIN, Hours)

                            Dim logtransaction As String = String.Format("Failed transactions details: FuelQuantity:{0},TransactionId:{1}", FuelQuantity, TransactionId)

                            TransactionFailedLog.Error(logtransaction)

                            CreateDataFor = dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("VehicleName").ToString() & ";" & dtSingleTransacton.Rows(0)("DeptName").ToString() & ";" &
                                   dtSingleTransacton.Rows(0)("DepartmentNumber").ToString() & ";" & dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim() & ";" &
                                   FuelQuantity & ";" & dtSingleTransacton.Rows(0)("Other").ToString() & ";" & dtSingleTransacton.Rows(0)("Company").ToString() & ";" & "" & ";" &
                                   Convert.ToDateTime(dtSingleTransacton.Rows(0)("Date").ToString()).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(dtSingleTransacton.Rows(0)("Time").ToString()).ToString("hh: mm:tt") &
                                   ";" & dtSingleTransacton.Rows(0)("PersonName").ToString() & ";" & dtSingleTransacton.Rows(0)("PersonPin").ToString().Trim() & ";" &
                                   dtSingleTransacton.Rows(0)("CurrentOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("PreviousOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("Hours").ToString() & ";" &
                                   dtSingleTransacton.Rows(0)("FuelType").ToString() & ";" & "Completed" & ";" & Pulses

                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                                CSCommonHelper.WriteLog("Modified", "Transactions", Beforedata, writtenData, dtSingleTransacton.Rows(0)("PersonName").ToString() & "(" & dtSingleTransacton.Rows(0)("Email").ToString() & ")", IPAddress, "fail", "Update Transactions Saved Failed.")
                            End If

                        End If

                    Else
                        transactionCompleteResponceObj.ResponceMessage = "success"
                        transactionCompleteResponceObj.ResponceText = "Error occurred during transaction!"
                        Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

                        'Dim logtransaction As String = String.Format("Failed transactions details: VehicleId : {0},SiteId:{1},PersonId:{2}, CurrentOdometer:{3}, FuelQuantity:{4}, FuelTypeId:{5}, PhoneNumber:{6}, WifiSSId:{7}," &
                        '                                             "TransactionDate:{8},TransactionFrom:{9},CurrentLat:{10},CurrentLng:{11},VehicleNumber:{12},DepartmentNumber:{13},PersonnelPIN:{14},Hours:{15}",
                        '                                             VehicleId, SiteId, PersonId, CurrentOdometer, FuelQuantity, FuelTypeId, PhoneNumber, WifiSSId, TransactionDate, TransactionFrom, CurrentLat, CurrentLng, VehicleNumber,
                        '                                             DepartmentNumber, PersonnelPIN, Hours)

                        Dim logtransaction As String = String.Format("Failed transactions details: FuelQuantity:{0},TransactionId:{1}", FuelQuantity, TransactionId)

                        TransactionFailedLog.Error(logtransaction)

                    End If
                End If

                json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                context.Response.Write(json)

                'End If

            End Using

        Catch ex As Exception

            Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            TransactionFailedLog.Error("Exception occurred while prcessing request. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try

    End Sub

    Private Sub SaveMultipleTransactions(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Try
            Dim json As String
            Dim steps As String
            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                'data = context.Request("cmtxtnid_10_record")
                requestJson = data
                log.Debug("SaveMultipleTransactions :" & data)
                Dim transactionCompleteResponceObj = New TransactionCompleteResponce()
                Dim javaScriptSerializer As JavaScriptSerializer = New JavaScriptSerializer()

                If (data = "") Then
                    log.Info("Multiple Transactions json not found. Transaction json : " & data)
                    transactionCompleteResponceObj.ResponceMessage = "success"
                    transactionCompleteResponceObj.ResponceText = "Multiple Transactions json not found."

                    json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                    context.Response.Write(json)
                    Return
                End If


                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(TransactionCompleteSaveMultipleTransactions))
                Dim cmtxtnid_10_record As List(Of TransactionComplete) = DirectCast(serJsonDetails, TransactionCompleteSaveMultipleTransactions).cmtxtnid_10_record
                steps = "1"
                For Each record As TransactionComplete In cmtxtnid_10_record
                    Try
                        updateTenTransactions(record.FuelQuantity, record.Pulses, record.TransactionId, IMEI_UDID)
                    Catch ex As Exception
                        log.Error("Error in SaveMultipleTransactions In For Each. Error - " & ex.Message)
                    End Try
                Next
                steps = "2"


                transactionCompleteResponceObj.ResponceMessage = "success"
                transactionCompleteResponceObj.ResponceText = "Transaction completed successfully!"

                json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                context.Response.Write(json)
                steps = "3"
            End Using

        Catch ex As Exception

            log.Error("Exception occurred while SaveMultipleTransactions. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "success", "Transactions saved successfully.")

        End Try

    End Sub

    Public Sub updateTenTransactions(FuelQuantity As Decimal, Pulses As Integer, TransactionId As String, IMEI_UDID As String)
        Try

            log.Debug("TransactionId : " & TransactionId)
            log.Debug("Fuel Quantity : " & FuelQuantity)

            log.Error("UpdateTenTransactions Step 1 ")

            If (FuelQuantity <= 0) Then

                'transactionCompleteResponceObj.ResponceMessage = "success"
                'transactionCompleteResponceObj.ResponceText = "Transaction with fuel quantity 0 is not allowed."

                'json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                'context.Response.Write(json)
                log.Error("UpdateTenTransactions - less than 0 QTY Step 2 ")
                Return
            End If

            If (TransactionId <> "0" Or TransactionId = "") Then

                log.Error("UpdateTenTransactions Step 3 ")

                Dim OBJWebServiceBAL = New WebServiceBAL()
                Dim OBJMasterBAL = New MasterBAL()

                Dim dtSingleTransacton As DataTable = New DataTable()
                dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId, False)

                If (dtSingleTransacton.Rows.Count = 0) Then
                    log.Error("TransactionId not found. TransactionId : " & TransactionId)

                    Return
                End If
                log.Error("UpdateTenTransactions Step 4 ")
                Try

                    Dim dsIMEI = New DataSet()
                    OBJServiceBAL = New WebServiceBAL()
                    dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                    Dim UserCustomerId As Integer = Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString())
                    Dim UserCustomerName As String = dsIMEI.Tables(0).Rows(0)("CustomerName").ToString()
                    Dim TransactionCustomerId As Integer = dtSingleTransacton.Rows(0)("CustomerId").ToString()
                    Dim TransactionCustomerName As String = dtSingleTransacton.Rows(0)("Company").ToString()

                    log.Error("UpdateTenTransactions Step 5 ")

                    If (UserCustomerId <> TransactionCustomerId Or UserCustomerName <> TransactionCustomerName) Then

                        log.Error("Transaction from another company so ignore it. TransactionId : " & TransactionId & " Hub company Name : " & UserCustomerName & " Transaction company Name :  " & TransactionCustomerName)

                        Return
                    End If
                Catch ex As Exception
                    log.Error("Error occurred in checking Is Transaction from another company. Exception is :" & ex.Message)
                End Try
                log.Error("UpdateTenTransactions Step 6 ")
                If (dtSingleTransacton.Rows(0)("FuelQuantity") > FuelQuantity) Then

                    FuelQuantity = dtSingleTransacton.Rows(0)("FuelQuantity")
                    Pulses = dtSingleTransacton.Rows(0)("Pulses")

                    log.Error("Stored transaction Fuel Quantity is greater than new fuel Quantity in cloud, so skipped the transaction. TransactionId : " & TransactionId & " Original FuelQuantity : " & dtSingleTransacton.Rows(0)("FuelQuantity") & " New Fuel Quantity :  " & FuelQuantity)

                    Return

                End If
                log.Error("UpdateTenTransactions Step 7 ")
                Dim CreateDataFor As String = ""
                CreateDataFor = dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("VehicleName").ToString() & ";" & dtSingleTransacton.Rows(0)("DeptName").ToString() & ";" &
                                dtSingleTransacton.Rows(0)("DepartmentNumber").ToString() & ";" & dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim() & ";" &
                                dtSingleTransacton.Rows(0)("FuelQuantity").ToString() & ";" & dtSingleTransacton.Rows(0)("Other").ToString() & ";" & dtSingleTransacton.Rows(0)("Company").ToString() & ";" & "" & ";" &
                                Convert.ToDateTime(dtSingleTransacton.Rows(0)("Date").ToString()).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(dtSingleTransacton.Rows(0)("Time").ToString()).ToString("hh: mm:tt") &
                                ";" & dtSingleTransacton.Rows(0)("PersonName").ToString() & ";" & dtSingleTransacton.Rows(0)("PersonPin").ToString().Trim() & ";" &
                                dtSingleTransacton.Rows(0)("CurrentOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("PreviousOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("Hours").ToString() & ";" &
                                dtSingleTransacton.Rows(0)("FuelType").ToString() & ";" & dtSingleTransacton.Rows(0)("TranStatus").ToString() & ";" & dtSingleTransacton.Rows(0)("Pulses").ToString()

                Dim Beforedata = CreateData(TransactionId, CreateDataFor)
                log.Error("UpdateTenTransactions Step 8 ")
                TransactionId = OBJMasterBAL.InsertUpdateTransaction(dtSingleTransacton.Rows(0)("VehicleId"), 0, 0, dtSingleTransacton.Rows(0)("CurrentOdometer"), FuelQuantity, 0, "", "", DateTime.Now, TransactionId, dtSingleTransacton.Rows(0)("PersonId"),
                                                                        "", dtSingleTransacton.Rows(0)("PreviousOdometer"), "", "", "", "", "", "", "", 0, False, True, 0, 0, Pulses, "", "", "", "", "", "", 0, 0, 0, 0, 0) '
                log.Error("UpdateTenTransactions Step 9 ")
                If TransactionId <> 0 Then 'success

                    CreateDataFor = dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("VehicleName").ToString() & ";" & dtSingleTransacton.Rows(0)("DeptName").ToString() & ";" &
                            dtSingleTransacton.Rows(0)("DepartmentNumber").ToString() & ";" & dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim() & ";" &
                            FuelQuantity & ";" & dtSingleTransacton.Rows(0)("Other").ToString() & ";" & dtSingleTransacton.Rows(0)("Company").ToString() & ";" & "" & ";" &
                            Convert.ToDateTime(dtSingleTransacton.Rows(0)("Date").ToString()).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(dtSingleTransacton.Rows(0)("Time").ToString()).ToString("hh: mm:tt") &
                            ";" & dtSingleTransacton.Rows(0)("PersonName").ToString() & ";" & dtSingleTransacton.Rows(0)("PersonPin").ToString().Trim() & ";" &
                            dtSingleTransacton.Rows(0)("CurrentOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("PreviousOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("Hours").ToString() & ";" &
                            dtSingleTransacton.Rows(0)("FuelType").ToString() & ";" & "Completed" & ";" & Pulses

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                        CSCommonHelper.WriteLog("Modified", "Transactions", Beforedata, writtenData, dtSingleTransacton.Rows(0)("PersonName").ToString() & "(" & dtSingleTransacton.Rows(0)("Email").ToString() & ")", IPAddress, "success", "")
                    End If

                Else 'fail

                    Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

                    Dim logtransaction As String = String.Format("Failed transactions details: FuelQuantity:{0},TransactionId:{1}", FuelQuantity, TransactionId)

                    TransactionFailedLog.Error(logtransaction)

                    CreateDataFor = dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("VehicleName").ToString() & ";" & dtSingleTransacton.Rows(0)("DeptName").ToString() & ";" &
                               dtSingleTransacton.Rows(0)("DepartmentNumber").ToString() & ";" & dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim() & ";" & dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim() & ";" &
                               FuelQuantity & ";" & dtSingleTransacton.Rows(0)("Other").ToString() & ";" & dtSingleTransacton.Rows(0)("Company").ToString() & ";" & "" & ";" &
                               Convert.ToDateTime(dtSingleTransacton.Rows(0)("Date").ToString()).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(dtSingleTransacton.Rows(0)("Time").ToString()).ToString("hh: mm:tt") &
                               ";" & dtSingleTransacton.Rows(0)("PersonName").ToString() & ";" & dtSingleTransacton.Rows(0)("PersonPin").ToString().Trim() & ";" &
                               dtSingleTransacton.Rows(0)("CurrentOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("PreviousOdometer").ToString() & ";" & dtSingleTransacton.Rows(0)("Hours").ToString() & ";" &
                               dtSingleTransacton.Rows(0)("FuelType").ToString() & ";" & "Completed" & ";" & Pulses

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                        CSCommonHelper.WriteLog("Modified", "Transactions", Beforedata, writtenData, dtSingleTransacton.Rows(0)("PersonName").ToString() & "(" & dtSingleTransacton.Rows(0)("Email").ToString() & ")", IPAddress, "fail", "Update Transactions Saved Failed.")
                    End If

                End If
                log.Error("UpdateTenTransactions Step 10 Complete.")
            Else

                Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

                Dim logtransaction As String = String.Format("Failed transactions details: FuelQuantity:{0},TransactionId:{1}", FuelQuantity, TransactionId)

                TransactionFailedLog.Error(logtransaction)

            End If
        Catch ex As Exception
            log.Error("Error occurred in updateTenTransactions. Exception is :" & ex.Message)
        End Try

    End Sub
#End Region

    Private Sub GetSSIDForIphone(data2 As String, Imei As String, context As HttpContext, userObj As UserData)
        'Get wifi ssid from Lat lng------------------
        steps = "6"
        Dim RegParts As String() = Regex.Split(data2, ",")

        Dim Lat As Double = [Double].Parse(RegParts(0).Trim()).ToString("0.00000")
        steps = "7"
        Dim Lng As Double = [Double].Parse(RegParts(1).Trim()).ToString("0.00000")
        steps = "8"
        Dim ds = New DataSet()

        Dim IDs As String = ""

        Dim dsSSID = New DataSet()

        ds = OBJServiceBAL.IsIMEIExists(Imei)
        steps = "9"
        If Not ds Is Nothing Then   'IMEI_UDID not exists
            If Not ds.Tables(0) Is Nothing Then
                If ds.Tables(0).Rows.Count <> 0 Then
                    Dim dtPersonSiteMapping = New DataTable()
                    steps = "10"
                    OBJMasterBAL = New MasterBAL()
                    Dim dt As DataTable = ds.Tables(0)
                    Dim personId As Integer
                    personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                    steps = "11"
                    dtPersonSiteMapping = OBJMasterBAL.GetPersonSiteMapping(personId, 0)
                    If Not dtPersonSiteMapping Is Nothing Then  'Unauthorized fuel location- Person Site mapping does not exists
                        If dtPersonSiteMapping.Rows.Count <> 0 Then
                            'ds = OBJMaster.GetSiteDetails()

                            For index As Integer = 0 To dtPersonSiteMapping.Rows.Count - 1
                                steps = "12"
                                If Not dtPersonSiteMapping.Rows(index)("Latitude").ToString() Is Nothing And Not dtPersonSiteMapping.Rows(index)("Longitude").ToString() Is Nothing Then

                                    If (Lat = 0 And Lng = 0) Then

                                        Dim siteID As Integer = dtPersonSiteMapping.Rows(index)("SiteID")

                                        IDs = siteID & "," & IDs
                                    ElseIf dtPersonSiteMapping.Rows(index)("DisableGeoLocation") = True Then
                                        Dim siteID As Integer = dtPersonSiteMapping.Rows(index)("SiteID")
                                        IDs = siteID & "," & IDs
                                    Else
                                        steps = "13"
                                        Dim siteLat As Double = [Double].Parse(dtPersonSiteMapping.Rows(index)("Latitude").ToString()).ToString("0.00000")
                                        steps = "14"
                                        Dim siteLng As Double = [Double].Parse(dtPersonSiteMapping.Rows(index)("Longitude").ToString()).ToString("0.00000")
                                        steps = "15"
                                        'Dim meterdistance As Double = distance1(Lat, Lng, siteLat, siteLng, "M")
                                        Dim meterdistance As Double = distance1(Lat.ToString().Substring(0, IIf(Lat.ToString().Split(".")(0).Length + 4 > Lat.ToString().Length, Lat.ToString().Length, Lat.ToString().Split(".")(0).Length + 4)),
                                        Lng.ToString().Substring(0, IIf(Lng.ToString().Split(".")(0).Length + 4 > Lng.ToString().Length, Lng.ToString().Length, Lng.ToString().Split(".")(0).Length + 4)),
                                            siteLat.ToString().Substring(0, IIf(siteLat.ToString().Split(".")(0).Length + 4 > siteLat.ToString().Length, siteLat.ToString().Length, siteLat.ToString().Split(".")(0).Length + 4)),
                                            siteLng.ToString().Substring(0, IIf(siteLng.ToString().Split(".")(0).Length + 4 > siteLng.ToString().Length, siteLng.ToString().Length, siteLng.ToString().Split(".")(0).Length + 4)),
                                            "M")
                                        steps = "16"
                                        If meterdistance <= 200 Then

                                            Dim siteID As Integer = dtPersonSiteMapping.Rows(index)("SiteID")

                                            IDs = siteID & "," & IDs

                                        End If
                                        steps = "17"
                                    End If
                                End If
                            Next

                            If Not IDs = "" Then
                                steps = "18"
                                dsSSID = OBJServiceBAL.GetSSIDbySiteId(IDs)
                                steps = "20"
                                'Dim resp As String =
                                SSIDdsToJson(context, dsSSID, userObj, personId, Imei)
                                'context.Response.Write(resp)
                            Else
                                steps = "19"
                                'context.Response.Write("empty")
                                ErrorInAuthonticationOnCheckApprove(context, "fail", resourceManager.GetString("HandlerMsg12"), userObj)
                            End If
                        Else
                            ErrorInAuthonticationOnCheckApprove(context, "fail", resourceManager.GetString("HandlerMsg13"), userObj)
                        End If
                    Else
                        ErrorInAuthonticationOnCheckApprove(context, "fail", resourceManager.GetString("HandlerMsg13"), userObj)
                    End If
                Else
                    ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg33"))
                End If
            Else
                ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg33"))
            End If



            'context.Response.Write(CreateResponse("", "", "", resp))

        Else
            'context.Response.Write("empty")
            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg33"))
        End If

    End Sub

    Private Sub ErrorInAuthonticationOnCheckApprove(context As HttpContext, ResponceMessage As String, errorString As String, userObj As UserData)

        Dim result As String = ""
        Dim rootOject = New RootObject()
        rootOject.ResponceMessage = "success"
        rootOject.ResponceText = resourceManager.GetString("HandlerMsg3")
        rootOject.objUserData = New UserData()

        rootOject.objUserData.Email = userObj.Email
        rootOject.objUserData.PhoneNumber = userObj.PhoneNumber
        rootOject.objUserData.PersonName = userObj.PersonName
        rootOject.objUserData.FluidSecureSiteName = userObj.FluidSecureSiteName
        rootOject.objUserData.IsApproved = userObj.IsApproved
        rootOject.objUserData.IMEI_UDID = userObj.IMEI_UDID
        rootOject.objUserData.IsOdoMeterRequire = userObj.IsOdoMeterRequire
        rootOject.objUserData.IsLoginRequire = userObj.IsLoginRequire
        rootOject.objUserData.IsDepartmentRequire = userObj.IsDepartmentRequire
        rootOject.objUserData.IsPersonnelPINRequire = userObj.IsPersonnelPINRequire
        rootOject.objUserData.IsPersonnelPINRequireForHub = userObj.IsPersonnelPINRequireForHub
        rootOject.objUserData.IsOtherRequire = userObj.IsOtherRequire
        rootOject.objUserData.IsVehicleHasFob = userObj.IsVehicleHasFob
        rootOject.objUserData.IsPersonHasFob = userObj.IsPersonHasFob
        rootOject.objUserData.OtherLabel = userObj.OtherLabel
        rootOject.objUserData.TimeOut = ConfigurationManager.AppSettings("WaitingTime").ToString()
        rootOject.objUserData.AndroidAppLatestVersion = ConfigurationManager.AppSettings("AndroidAppLatestVersion").ToString()
        rootOject.objUserData.AppUpgradeMsgDisplayAfterDays = ConfigurationManager.AppSettings("AppUpgradeMsgDisplayAfterDays").ToString()
        rootOject.objUserData.PersonId = userObj.PersonId
        rootOject.objUserData.LFBluetoothCardReader = userObj.LFBluetoothCardReader
        rootOject.objUserData.LFBluetoothCardReaderMacAddress = userObj.LFBluetoothCardReaderMacAddress
        rootOject.objUserData.VeederRootMacAddress = userObj.VeederRootMacAddress
        rootOject.objUserData.CollectDiagnosticLogs = userObj.CollectDiagnosticLogs
        rootOject.objUserData.IsLogging = userObj.IsLogging
        rootOject.objUserData.WifiChannelToUse = userObj.WifiChannelToUse
        rootOject.objUserData.IsGateHub = userObj.IsGateHub
        rootOject.objUserData.IsVehicleNumberRequire = userObj.IsVehicleNumberRequire

        rootOject.SSIDDataObj = New List(Of SSIDData)()

        Dim objSSIDData = New SSIDData()

        objSSIDData.ResponceMessage = "fail"
        objSSIDData.ResponceText = errorString

        rootOject.SSIDDataObj.Add(objSSIDData)

        Dim seri As New JavaScriptSerializer()
        Dim json As String
        json = seri.Serialize(rootOject)
        context.Response.Write(json)




        'Dim javaScriptSerializer = New JavaScriptSerializer()
        'Dim rootOject = New RootObject()
        'rootOject.ResponceMessage = ResponceMessage
        'rootOject.ResponceText = errorString
        'rootOject.ResponceData = New ResponceData()
        'rootOject.ResponceData.MinLimit = 0
        'rootOject.ResponceData.PulseRatio = 0
        'rootOject.ResponceData.VehicleId = 0
        'rootOject.ResponceData.PersonId = 0
        'rootOject.ResponceData.FuelTypeId = 0
        'rootOject.ResponceData.PhoneNumber = ""
        'rootOject.ResponceData.ServerDate = DateTime.Now.ToString()
        'Dim json As String
        'json = javaScriptSerializer.Serialize(rootOject)
        'context.Response.Write(json)
    End Sub

    Private Sub SendTransactionEmail(emailTo As String, DateValue As String, TimeValue As String, VehicleValue As String,
                                     HoseNameValue As String, ProductValue As String, TransactionNumber As String, QuantityValue As String,
                                     AdditionalEmailId As String, IsUserForHub As Boolean, IsOtherRequire As Boolean, OtherLabel As String, Other As String,
                                     PersonName As String, VehicleNameText As String, EmailSubject As String, TransactionId As Integer)
        Try

            Dim dtSingleTransacton As DataTable = New DataTable()
            dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId, False)
            Dim TransactionCost As Decimal = 0.0

            If dtSingleTransacton IsNot Nothing Then
                If dtSingleTransacton.Rows.Count > 0 Then
                    TransactionCost = Convert.ToDouble(dtSingleTransacton.Rows(0)("TransactionCost").ToString())
                End If
            End If

            If IsUserForHub = False Then

                Dim body As String = String.Empty

                Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/TransactionEmail.txt"))
                    body = sr.ReadToEnd()
                End Using

                '------------------
                body = body.Replace("CustomerEmail", emailTo)
                body = body.Replace("DateValue", DateValue)
                body = body.Replace("TimeValue", TimeValue)
                body = body.Replace("VehicleValue", VehicleValue)
                body = body.Replace("VehicleNameText", VehicleNameText)
                body = body.Replace("HoseNameValue", HoseNameValue)
                body = body.Replace("ProductValue", ProductValue)
                body = body.Replace("TransactionNumber", TransactionNumber)
                body = body.Replace("QuantityValue", QuantityValue)
                body = body.Replace("PersonName", PersonName)
                If IsOtherRequire Then
                    body = body.Replace("OtherLabel", "<th>" & OtherLabel & "</th>")
                    body = body.Replace("OtherValue", "<td>" & Other & "</td>")
                Else
                    body = body.Replace("OtherLabel", "")
                    body = body.Replace("OtherValue", "")
                End If

                log.Debug("TransactionCost - Email - " & TransactionCost)

                If Convert.ToDecimal(TransactionCost) <> 0.0 Then
                    body = body.Replace("TransactionCostLabel", "<th> Transaction Cost </th>")
                    body = body.Replace("TransactionCostValue", "<td>" & TransactionCost & "</td>")
                Else
                    body = body.Replace("TransactionCostLabel", "")
                    body = body.Replace("TransactionCostValue", "")
                End If


                Dim e As New EmailService()


                Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))


                mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
                mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

                Dim messageSend As New MailMessage()
                messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
                messageSend.[To].Add(New MailAddress(emailTo))


                messageSend.Subject = IIf(EmailSubject <> "", EmailSubject, ConfigurationManager.AppSettings("Subject"))
                messageSend.Body = body

                messageSend.IsBodyHtml = True
                mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))

                'log.Debug("body:  " & body)

                Try
                    mailClient.Send(messageSend)
                Catch ex As Exception
                    log.Debug("Exception occurred in sending transaction emails to EmailId : " & emailTo & ". ex is :" & ex.Message)
                End Try
            End If

            Try

                If AdditionalEmailId <> "" Then

                    Dim strSplitEmail() As String = AdditionalEmailId.ToLower().TrimEnd(";").Split(";")
                    strSplitEmail = strSplitEmail.Distinct().ToArray() ' removed duplicates from array

                    For i = 0 To strSplitEmail.Length - 1
                        Dim tempEmail As String = strSplitEmail(i).ToString()

                        'if Person email and Additional email is same then do not send email receipt again.
                        If (tempEmail.ToLower() = emailTo.ToLower()) Then
                            log.Debug(String.Format("Email already sent to email : {0}. Duplicate Email {1}", emailTo, tempEmail))
                            Continue For
                        End If

                        Dim bodyForAdditionalEmail As String = String.Empty
                        Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/TransactionEmail.txt"))
                            bodyForAdditionalEmail = sr.ReadToEnd()
                        End Using

                        'log.Debug("bodyForAdditionalEmail 1 = " & bodyForAdditionalEmail)


                        Dim mailClientAdd As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))

                        mailClientAdd.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
                        mailClientAdd.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

                        Dim messageSendAdd As New MailMessage()
                        messageSendAdd.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
                        messageSendAdd.[To].Add(New MailAddress(strSplitEmail(i).ToString()))

                        mailClientAdd.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("CustomerEmail", strSplitEmail(i).ToString())
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("DateValue", DateValue)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("TimeValue", TimeValue)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("VehicleValue", VehicleValue)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("VehicleNameText", VehicleNameText)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("HoseNameValue", HoseNameValue)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("ProductValue", ProductValue)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("TransactionNumber", TransactionNumber)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("QuantityValue", QuantityValue)
                        bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("PersonName", PersonName)
                        If IsOtherRequire Then
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("OtherLabel", "<th>" & OtherLabel & "</th>")
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("OtherValue", "<td>" & Other & "</td>")
                        Else
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("OtherLabel", "")
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("OtherValue", "")
                        End If

                        If Convert.ToDecimal(TransactionCost) <> 0.0 Then
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("TransactionCostLabel", "<th> Transaction Cost </th>")
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("TransactionCostValue", "<td>" & TransactionCost & "</td>")
                        Else
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("TransactionCostLabel", "")
                            bodyForAdditionalEmail = bodyForAdditionalEmail.Replace("TransactionCostValue", "")
                        End If

                        messageSendAdd.Subject = IIf(EmailSubject <> "", EmailSubject, ConfigurationManager.AppSettings("Subject"))
                        'log.Debug("bodyForAdditionalEmail = " & bodyForAdditionalEmail)

                        messageSendAdd.Body = bodyForAdditionalEmail
                        messageSendAdd.IsBodyHtml = True
                        mailClientAdd.Send(messageSendAdd)
                    Next
                End If
            Catch ex As Exception
                log.Debug("Exception occurred in sending transaction emails to EmailId : " & AdditionalEmailId & ". ex is :" & ex.Message)
            End Try

        Catch ex As Exception
            log.Debug("Exception occurred in sending transaction emails to EmailId : " & emailTo & ". ex is :" & ex.Message)
        End Try
    End Sub

    Private Sub CheckBusyStatusOfAllFluidSecureUnits()

        Try

            Dim OBJWebServiceBAL = New WebServiceBAL()
            'log.Error("WaitingTime: " + ConfigurationManager.AppSettings("WaitingTime").ToString())
            Dim WaitingTime As String = (Convert.ToDecimal(ConfigurationManager.AppSettings("WaitingTime").ToString()) * 60).ToString()
            'log.Error("WaitingTime: " + WaitingTime)

            OBJWebServiceBAL.CheckBusyStatusOfAllFluidSecureUnits(WaitingTime)

        Catch ex As Exception
            log.Error("Error occurred in CheckBusyStatusOfAllFluidSecureUnits. Exception is :" & ex.Message)
        End Try


    End Sub

    Private Sub CheckVehicleRequireOdometerEntryAndRequireHourEntry(context As HttpContext)
        steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("CheckVehicleRequireOdometerEntryAndRequireHourEntry :" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(AuthorizationSequenceModel))
            Dim IMEI_UDID = DirectCast(serJsonDetails, AuthorizationSequenceModel).IMEIUDID
            Dim VehicleNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).VehicleNumber
            Dim WifiSSId = DirectCast(serJsonDetails, AuthorizationSequenceModel).WifiSSId
            Dim SiteId = DirectCast(serJsonDetails, AuthorizationSequenceModel).SiteId
            Dim PersonnelPIN = DirectCast(serJsonDetails, AuthorizationSequenceModel).PersonnelPIN
            Dim RequestFromAPP = DirectCast(serJsonDetails, AuthorizationSequenceModel).RequestFromAPP
            Dim FOBNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).FOBNumber
            Dim IsVehicleNumberRequire = DirectCast(serJsonDetails, AuthorizationSequenceModel).IsVehicleNumberRequire

            If (Not PersonnelPIN Is Nothing) Then
                PersonnelPIN = PersonnelPIN.Trim()
            Else
                PersonnelPIN = ""
            End If

            If (Not VehicleNumber Is Nothing) Then
                VehicleNumber = VehicleNumber.Trim()
            Else
                VehicleNumber = ""
            End If
            log.Debug("IsVehicleNumberRequire- " & IsVehicleNumberRequire)
            If (IsVehicleNumberRequire Is Nothing) Then
                IsVehicleNumberRequire = "True"
            End If

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 2"
            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        'If (RequestFromAPP = "AP") Then  'commented for person B should be able to enter his pin on person A’s phone
                        'log.Debug("in AP")
                        Dim dsPerson = New DataSet()
                        If (PersonnelPIN.Trim() = "") Then
                            dtMain = dsIMEI.Tables(0)
                        Else
                            dsPerson = OBJServiceBAL.GetPersonnelByPinNumber(PersonnelPIN.Trim(), Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString()))
                            If Not dsPerson Is Nothing Then
                                If (dsPerson.Tables(0).Rows.Count = 0) Then

                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Invalid Pin. Please contact administrator. IMEI_UDID=" & IMEI_UDID & ". Pin" & PersonnelPIN.Trim())
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg34"), "Pin")
                                    Return
                                Else
                                    dtMain = dsPerson.Tables(0)
                                End If
                            Else
                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Invalid Pin. Please contact administrator. IMEI_UDID=" & IMEI_UDID & ". Pin" & PersonnelPIN.Trim())
                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg34"), "Pin")
                                Return
                            End If
                        End If
                        'Else
                        '	dtMain = dsIMEI.Tables(0)
                        'End If

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 3"
                            Dim dtPersonSiteMapping = New DataTable()
                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            log.Debug("personId:" & personId)
                            Dim customerId As Integer
                            customerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            Dim RoleId As String
                            RoleId = dt.Rows(0)("RoleId").ToString()
                            dtPersonSiteMapping = OBJMasterBAL.GetPersonSiteMapping(personId, SiteId)
                            steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 5"
                            If Not dtPersonSiteMapping Is Nothing Then  'Unauthorized fuel location- Person Site mapping does not exists
                                log.Debug("CheckVehicleRequireOdometerEntryAndRequireHourEntry: dtPersonSiteMapping")
                                If dtPersonSiteMapping.Rows.Count <> 0 Then 'Person Site mapping does not contain data
                                    Dim dtVehicle = New DataTable()

                                    If (VehicleNumber.Trim() = "" And IsVehicleNumberRequire = "False") Then
                                        dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And LTRIM(RTRIM(V.VehicleNumber)) ='default'", personId, RoleId)
                                    ElseIf (VehicleNumber.Trim() <> "") Then
                                        dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And LTRIM(RTRIM(V.VehicleNumber)) ='" & IIf(VehicleNumber.Trim().ToLower().Contains("guest"), "guest", VehicleNumber.Trim()) & "'", personId, RoleId)
                                    Else
                                        dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & FOBNumber.ToString().Replace(" ", "") & "'", personId, RoleId)

                                        If dtVehicle Is Nothing Then 'Fob number not exists

                                            Dim dtPersonForFobNumber As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','') ='" & FOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                            If (dtPersonForFobNumber.Rows.Count > 0) Then
                                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry - You have presented the wrong Fob/Card, please use another Fob/Card or Please contact administrator. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", ""))
                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg35"), "invalidfob")
                                                Return
                                            End If

                                            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry - FOBNumber not assigned. IMEI_UDID=" & IMEI_UDID & " FOBNumber=" & FOBNumber.ToString().Replace(" ", ""))
                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg36"), "invalidfob", "Yes")
                                            Return
                                        ElseIf dtVehicle.Rows.Count = 0 Then ''Fob number not exists
                                            Dim dtPersonForFobNumber As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','') ='" & FOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                            If (dtPersonForFobNumber.Rows.Count > 0) Then
                                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry - You have presented the wrong Fob/Card, please use another Fob/Card or Please contact administrator. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", ""))
                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg35"), "invalidfob")
                                                Return
                                            End If

                                            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry - Fob/Card Number not assigned. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.ToString().Replace(" ", ""))
                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg36"), "invalidfob", "Yes")
                                            Return
                                        End If

                                    End If


                                    If Not dtVehicle Is Nothing Then 'Authorized vehicle?-Vehicle number not exists
                                        log.Debug("CheckVehicleRequireOdometerEntryAndRequireHourEntry: In dtVehicle")
                                        If dtVehicle.Rows.Count <> 0 Then ''Authorized vehicle?-Vehicle number not exists
                                            Dim dtVehicelSiteMapping = New DataTable()
                                            Dim vehicleId As Integer
                                            vehicleId = Integer.Parse(dtVehicle.Rows(0)("VehicleId").ToString())
                                            steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 6 " & vehicleId

                                            If (FOBNumber <> "" And PersonnelPIN.Trim() = "") Then
                                                If (dtVehicle.Rows(0)("FOBNumber").ToString().Replace(" ", "") <> "" And dtVehicle.Rows(0)("FOBNumber").ToString().ToLower().Replace(" ", "") <> FOBNumber.ToLower().Replace(" ", "")) Then
                                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry - Vehicle number is already registered with another Fob/Card Number. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", "") & "Vehicle number=" & VehicleNumber.Trim())
                                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg37"), "invalidfob")
                                                    Return
                                                End If

                                                'One Fob/Card will not be acceptable for vehicle and personnel screens.
                                                Dim dtVehicleTemp As DataTable = New DataTable()
                                                dtVehicleTemp = (OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & FOBNumber.Replace(" ", "") & "'", personId, RoleId))

                                                If (dtVehicleTemp.Rows.Count = 0) Then
                                                    Dim dtPersonForFobNumber As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','') ='" & FOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                                    If (dtPersonForFobNumber.Rows.Count > 0) Then
                                                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry - Person is already registered with same Fob/Card Number. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", ""))
                                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg38"), "invalidfob")
                                                        Return
                                                    End If
                                                End If
                                                'One Fob/Card will not be acceptable for vehicle and personnel screens.

                                            End If


                                            If (dtVehicle.Rows(0)("IsActive").ToString() = "False") Then
                                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Vehicle is InActive. IMEI_UDID=" & IMEI_UDID)
                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg16"), "Vehicle")
                                                Return
                                            End If

                                            dtVehicelSiteMapping = OBJMasterBAL.GetVehicleSiteMapping(vehicleId, SiteId)
                                            If Not dtVehicelSiteMapping Is Nothing Or IsVehicleNumberRequire = "False" Then     'Vehicle Site Mapping not exist
                                                If dtVehicelSiteMapping.Rows.Count <> 0 Or IsVehicleNumberRequire = "False" Then 'Vehicle Site Mapping not exist
                                                    'Authorized fuel date?
                                                    Dim dtSiteDays = New DataTable()
                                                    dtSiteDays = OBJMasterBAL.GetSiteDays(Integer.Parse(SiteId))
                                                    If Not dtSiteDays Is Nothing Then    'site Days not assigned to site
                                                        If dtSiteDays.Rows.Count <> 0 Then
                                                            Dim dateValue As New DateTime()
                                                            dateValue = DateTime.Now
                                                            steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 7"
                                                            Dim currentDay = CInt(dateValue.DayOfWeek) + 1
                                                            Dim dayArray = dtSiteDays.AsEnumerable().[Select](Function(r) r.Field(Of Integer)("Day")).ToArray()
                                                            Dim isCurrentDayInDayArray = dayArray.Contains(currentDay)
                                                            If isCurrentDayInDayArray = True Then 'Current Day not in site days
                                                                Dim dtPersonalTiming = New DataTable()
                                                                steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 8"
                                                                dtPersonalTiming = OBJMasterBAL.GetPersonnelTimings(personId)
                                                                If Not dtPersonalTiming Is Nothing Then 'Person timing does not exists
                                                                    If dtPersonalTiming.Rows.Count <> 0 Then 'Person timing does not exists
                                                                        Dim dtSiteTiming = New DataTable()
                                                                        dtSiteTiming = OBJMasterBAL.GetSiteTimings(SiteId)
                                                                        steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 9"
                                                                        If Not dtSiteTiming Is Nothing Then 'Site timing does not exists
                                                                            If dtSiteTiming.Rows.Count <> 0 Then 'Site timing does not exists
                                                                                Dim dsCurrentTimeInPerson = New DataSet()
                                                                                dsCurrentTimeInPerson = OBJMasterBAL.CheckCurrentTimeInTimesTable(SiteId, personId)
                                                                                If Not dsCurrentTimeInPerson Is Nothing Then ' check Current Time In Times Table
                                                                                    If dsCurrentTimeInPerson.Tables.Count = 2 Then
                                                                                        If dsCurrentTimeInPerson.Tables(0).Rows.Count <> 0 And dsCurrentTimeInPerson.Tables(1).Rows.Count <> 0 Then 'Current Time not in SiteTiming and PersonnelTimings from-to timing
                                                                                            'PersonalVehicle mapping
                                                                                            Dim dtPersonVehicleMapping = New DataTable()
                                                                                            dtPersonVehicleMapping = OBJMasterBAL.GetPersonVehicleMapping(personId)
                                                                                            If Not dtPersonVehicleMapping Is Nothing Or IsVehicleNumberRequire = "False" Then 'Person vehicle mapping does not exists
                                                                                                If dtPersonVehicleMapping.Rows.Count <> 0 Or IsVehicleNumberRequire = "False" Then 'Person vehicle mapping does not exists
                                                                                                    Dim vehicleArray = dtPersonVehicleMapping.AsEnumerable().[Select](Function(r) r.Field(Of Integer)("VehicleId")).ToArray()
                                                                                                    steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 10"
                                                                                                    Dim isVehicleIdInVehicleArray = vehicleArray.Contains(vehicleId)
                                                                                                    If isVehicleIdInVehicleArray Or IsVehicleNumberRequire = "False" Then ' veihcle id send by user not assigned to person -PersonVehicle mapping not match
                                                                                                        'IsOdometerRequire on screen mobile application
                                                                                                        Dim dtCustomer = New DataTable()
                                                                                                        dtCustomer = OBJMasterBAL.GetCustomerId(customerId)
                                                                                                        'log.Error("customerId : " & customerId)
                                                                                                        steps = "CheckVehicleRequireOdometerEntryAndRequireHourEntry 11"

                                                                                                        CreateResponseForRequireOdoVehicle(context, data, vehicleId, dtVehicle, personId, dt, RoleId) ', Boolean.Parse(dtCustomer.Rows(0)("IsOdometerRequire"))

                                                                                                    Else
                                                                                                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- 1 The user is not authorized for this vehicle for IMEI_UDID=" & IMEI_UDID)
                                                                                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg20"), "Pin")
                                                                                                        'ErrorInRequireOdoVehicle(context, "fail", "The user Is Not authorized For this vehicle, please contact administrator.", "Pin")
                                                                                                    End If
                                                                                                Else
                                                                                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- The user is not authorized for this vehicle for IMEI_UDID=" & IMEI_UDID)
                                                                                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg20"), "Pin")
                                                                                                    'ErrorInRequireOdoVehicle(context, "fail", "The user is not authorized for this vehicle, please contact administrator.", "Pin")
                                                                                                End If
                                                                                            Else
                                                                                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- The user is not authorized for this vehicle for IMEI_UDID=" & IMEI_UDID)
                                                                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg20"), "Pin")
                                                                                                'ErrorInRequireOdoVehicle(context, "fail", "The user is not authorized for this vehicle, please contact administrator.", "Pin")
                                                                                            End If




                                                                                        Else
                                                                                            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel time for IMEI_UDID=" & IMEI_UDID)
                                                                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                                            'ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel time, please contact administrator.")
                                                                                        End If
                                                                                    Else
                                                                                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel time for IMEI_UDID=" & IMEI_UDID)
                                                                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                                        'ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel time, please contact administrator.")
                                                                                    End If
                                                                                Else
                                                                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel time for IMEI_UDID=" & IMEI_UDID)
                                                                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                                    'ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel time, please contact administrator.")
                                                                                End If
                                                                            Else
                                                                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Fuel timings are not assigned for this hose, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg22"), "Pin")
                                                                                ''ErrorInRequireOdoVehicle(context, "fail", "Fuel timings are not assigned for this hose, please contact administrator")
                                                                            End If
                                                                        Else
                                                                            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Fuel timings are not assigned for this hose, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg22"), "Pin")
                                                                            ''ErrorInRequireOdoVehicle(context, "fail", "Fuel timings are not assigned for this hose, please contact administrator.")
                                                                        End If
                                                                    Else
                                                                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Fuel timings are not assigned, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                        ''ErrorInRequireOdoVehicle(context, "fail", "Fuel timings are not assigned, please contact administrator.")
                                                                    End If
                                                                Else
                                                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Fuel timings are not assigned, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg21"), "Pin")
                                                                    ''ErrorInRequireOdoVehicle(context, "fail", "Fuel timings are Not assigned, please contact administrator.")
                                                                End If
                                                            Else
                                                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel day. IMEI_UDID=" & IMEI_UDID)
                                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg23"), "Pin")
                                                                'ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel day, Please contact administrator.")
                                                            End If
                                                        Else
                                                            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel day. IMEI_UDID=" & IMEI_UDID)
                                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg23"), "Pin")
                                                            'ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel day, Please contact administrator.")
                                                        End If
                                                    Else
                                                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel location for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg23"), "Pin")
                                                        'ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel day, Please contact administrator.")
                                                    End If
                                                Else
                                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Valid Vehicle but not Authorized for this Hose. for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg24"), "Vehicle")
                                                    'ErrorInRequireOdoVehicle(context, "fail", "Valid Vehicle but Not Authorized For this Hose, Please contact administrator.", "Vehicle") 'Unauthorized fuel location.
                                                End If
                                            Else
                                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Valid Vehicle but not Authorized for this Hose for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg24"), "Vehicle")
                                                'ErrorInRequireOdoVehicle(context, "fail", "Valid Vehicle but not Authorized for this Hose, Please contact administrator.", "Vehicle") 'Unauthorized fuel location.
                                            End If
                                        Else
                                            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- 2 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI_UDID)
                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg25"), "Vehicle")
                                            ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized vehicle. Please contact administrator.", "Vehicle")
                                        End If
                                    Else
                                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- 3 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI_UDID)
                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg25"), "Vehicle")
                                        ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized vehicle, Please contact administrator.", "Vehicle")
                                    End If
                                Else
                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel location for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg26"))
                                    ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel location, Please contact administrator.")
                                End If
                            Else
                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Unauthorized fuel location for IMEI_UDID=" & IMEI_UDID)
                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg26"))
                                ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized fuel location, Please contact administrator.")
                            End If
                        Else
                            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using

    End Sub

    Private Sub CreateResponseForRequireOdoVehicle(context As HttpContext, data As String, VehicleId As Integer, dtVehicle As DataTable, PersonId As Integer, dt As DataTable, RoleId As String) ', IsOdometerRequire As Boolean
        Dim javaScriptSerializer = New JavaScriptSerializer()
        Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(AuthorizationSequenceModel))
        Dim IMEI_UDID = DirectCast(serJsonDetails, AuthorizationSequenceModel).IMEIUDID
        Dim VehicleNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).VehicleNumber
        Dim OdoMeter = DirectCast(serJsonDetails, AuthorizationSequenceModel).OdoMeter
        Dim WifiSSId = DirectCast(serJsonDetails, AuthorizationSequenceModel).WifiSSId
        Dim SiteId = DirectCast(serJsonDetails, AuthorizationSequenceModel).SiteId
        Dim FOBNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).FOBNumber
        Dim PersonnelPIN = DirectCast(serJsonDetails, AuthorizationSequenceModel).PersonnelPIN

        If (Not PersonnelPIN Is Nothing) Then
            PersonnelPIN = PersonnelPIN.Trim()
        Else
            PersonnelPIN = ""
        End If

        If (Not VehicleNumber Is Nothing) Then
            VehicleNumber = VehicleNumber.Trim()
        Else
            VehicleNumber = ""
        End If

        OBJMasterBAL = New MasterBAL()
        steps = "CreateResponseForRequireOdoVehicle 1"
        Dim dtHose = OBJMasterBAL.GetHoseByCondition(" And LTRIM(RTRIM(h.WifiSSId)) ='" & WifiSSId.ToString().Trim().Replace("'", "''") & "' and s.SiteID =" & SiteId.ToString() & "", PersonId, RoleId)
        steps = "CreateResponseForRequireOdoVehicle 2"
        If Not dtHose Is Nothing Then ''Hose does not exists
            If dtHose.Rows.Count <> 0 Then ''Hose does not exists

                Dim fuelTypeOfHose As Integer
                fuelTypeOfHose = Integer.Parse(dtHose.Rows(0)("FuelTypeId").ToString())

                steps = "CreateResponseForRequireOdoVehicle 4"

                Dim vehicleFuelLimitForDay As Integer
                vehicleFuelLimitForDay = Integer.Parse(dtVehicle.Rows(0)("FuelLimitPerDay").ToString())
                steps = "CreateResponseForRequireOdoVehicle 5"
                Dim personFuelLimitForDay As Integer
                personFuelLimitForDay = Integer.Parse(dt.Rows(0)("FuelLimitPerDay".ToString()))
                Dim phoneNumber = dt.Rows(0)("PhoneNumber".ToString())
                Dim dsTransactionFuelLimitForDay = New DataSet()
                dsTransactionFuelLimitForDay = OBJMasterBAL.GetSumOfFuelQuantity(PersonId, VehicleId) 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                steps = "CreateResponseForRequireOdoVehicle 6"
                If Not dsTransactionFuelLimitForDay Is Nothing Then
                    If dsTransactionFuelLimitForDay.Tables.Count = 2 Then
                        If Not dsTransactionFuelLimitForDay.Tables(0) Is Nothing And Not dsTransactionFuelLimitForDay.Tables(1) Is Nothing Then
                            If dsTransactionFuelLimitForDay.Tables(0).Rows.Count <> 0 And dsTransactionFuelLimitForDay.Tables(1).Rows.Count <> 0 Then
                                If Decimal.Parse(dsTransactionFuelLimitForDay.Tables(0).Rows(0)("FuelQuantityOfPerson").ToString()) < personFuelLimitForDay Or personFuelLimitForDay = 0 Then
                                    If Decimal.Parse(dsTransactionFuelLimitForDay.Tables(1).Rows(0)("FuelQuantityOfVehicle").ToString()) < vehicleFuelLimitForDay Or vehicleFuelLimitForDay = 0 Then
                                        steps = "CreateResponseForRequireOdoVehicle 7"
                                        'calculate min fuel limit per day
                                        '0 means unlimited
                                        Dim minLimitForPerson As Integer
                                        Dim minLimitForVehicle As Integer
                                        If personFuelLimitForDay <> 0 Then
                                            minLimitForPerson = personFuelLimitForDay - Decimal.Parse(dsTransactionFuelLimitForDay.Tables(0).Rows(0)("FuelQuantityOfPerson").ToString())
                                        Else
                                            minLimitForPerson = 0
                                        End If


                                        If vehicleFuelLimitForDay <> 0 Then
                                            minLimitForVehicle = vehicleFuelLimitForDay - Decimal.Parse(dsTransactionFuelLimitForDay.Tables(1).Rows(0)("FuelQuantityOfVehicle").ToString())
                                        Else
                                            minLimitForVehicle = 0
                                        End If

                                        Dim minLimit As Integer
                                        If minLimitForPerson = 0 Then
                                            minLimit = minLimitForVehicle
                                        ElseIf minLimitForVehicle = 0 Then
                                            minLimit = minLimitForPerson
                                        ElseIf minLimitForPerson <= minLimitForVehicle Then
                                            minLimit = minLimitForPerson
                                        Else
                                            minLimit = minLimitForVehicle
                                        End If

                                        'calculate min fuel limit per transaction
                                        Dim personFuellimitPerTran As Integer
                                        personFuellimitPerTran = 0
                                        If Not dt.Rows(0)("FuelLimitPerTxn".ToString()) Is Nothing Then
                                            personFuellimitPerTran = Integer.Parse(dt.Rows(0)("FuelLimitPerTxn".ToString()))
                                        End If

                                        Dim vehicleFuellimitPerTran As Integer
                                        vehicleFuellimitPerTran = 0
                                        If Not dtVehicle.Rows(0)("FuelLimitPerTxn").ToString() Is Nothing Then
                                            vehicleFuellimitPerTran = Integer.Parse(dtVehicle.Rows(0)("FuelLimitPerTxn").ToString())
                                        End If

                                        If minLimit = 0 Then
                                            If personFuellimitPerTran = 0 Then
                                                minLimit = vehicleFuellimitPerTran
                                            ElseIf vehicleFuellimitPerTran = 0 Then
                                                minLimit = personFuellimitPerTran
                                            ElseIf personFuellimitPerTran <= vehicleFuellimitPerTran Then
                                                minLimit = personFuellimitPerTran
                                            Else
                                                minLimit = vehicleFuellimitPerTran
                                            End If
                                        Else
                                            If minLimit >= personFuellimitPerTran And personFuellimitPerTran <> 0 Then
                                                minLimit = personFuellimitPerTran
                                            End If

                                            If minLimit >= vehicleFuellimitPerTran And vehicleFuellimitPerTran <> 0 Then
                                                minLimit = vehicleFuellimitPerTran
                                            End If
                                        End If

                                        Dim pulseRatio As Decimal
                                        pulseRatio = Decimal.Parse(dtHose.Rows(0)("PulserRatio").ToString())
                                        Dim checkRequireOdoResponse = New CheckRequireOdoResponse()
                                        checkRequireOdoResponse.ResponceMessage = "success"
                                        checkRequireOdoResponse.ResponceText = "success"
                                        checkRequireOdoResponse.ValidationFailFor = ""

                                        checkRequireOdoResponse.IsOdoMeterRequire = IIf(dtVehicle.Rows(0)("RequireOdometerEntry") = "Y", "True", "False")
                                        checkRequireOdoResponse.IsHoursRequire = dtVehicle.Rows(0)("Hours")
                                        checkRequireOdoResponse.PreviousOdo = IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))
                                        checkRequireOdoResponse.PreviousHours = IIf(IsDBNull(dtVehicle.Rows(0)("CurrentHours")), 0, dtVehicle.Rows(0)("CurrentHours"))

                                        If IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")) Then
                                            log.Debug("In CurrentOdometer = 0 or Null")
                                            checkRequireOdoResponse.CheckOdometerReasonable = "False"
                                            checkRequireOdoResponse.OdometerReasonabilityConditions = 2
                                            checkRequireOdoResponse.OdoLimit = 0
                                            checkRequireOdoResponse.HoursLimit = 0
                                        ElseIf dtVehicle.Rows(0)("CurrentOdometer") = "0" Then
                                            log.Debug("In CurrentOdometer = 0 or Null")
                                            checkRequireOdoResponse.CheckOdometerReasonable = "False"
                                            checkRequireOdoResponse.OdometerReasonabilityConditions = 2
                                            checkRequireOdoResponse.OdoLimit = 0
                                            checkRequireOdoResponse.HoursLimit = 0
                                        Else
                                            log.Debug("In NOT CurrentOdometer = 0 or Null")
                                            checkRequireOdoResponse.CheckOdometerReasonable = IIf(dtVehicle.Rows(0)("CheckOdometerReasonable") = "Y", "True", "False")

                                            If (dtVehicle.Rows(0)("CheckOdometerReasonable") = "Y") Then

                                            checkRequireOdoResponse.OdometerReasonabilityConditions = IIf(Convert.ToString(dtVehicle.Rows(0)("OdometerReasonabilityConditions")) = "", 1, dtVehicle.Rows(0)("OdometerReasonabilityConditions"))

                                            checkRequireOdoResponse.OdoLimit = (Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))) +
                                                                                Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("Odolimit")), 0, dtVehicle.Rows(0)("Odolimit"))))

                                            checkRequireOdoResponse.HoursLimit = (Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("CurrentHours")), 0, dtVehicle.Rows(0)("CurrentHours"))) +
                                                                                Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("HoursLimit")), 0, dtVehicle.Rows(0)("HoursLimit"))))

                                        Else
                                                checkRequireOdoResponse.OdometerReasonabilityConditions = 2
                                                checkRequireOdoResponse.OdoLimit = 0
                                                checkRequireOdoResponse.HoursLimit = 0
                                            End If

                                        End If

                                        checkRequireOdoResponse.FOBNumber = dtVehicle.Rows(0)("FOBNumber").ToString().Replace(" ", "")
                                        checkRequireOdoResponse.VehicleNumber = dtVehicle.Rows(0)("VehicleNumber").ToString().Trim()
                                        'check current version with launch version


                                        If (FOBNumber <> "" And PersonnelPIN.Trim() = "") Then
                                            OBJMasterBAL.AssignedFOBNumberToVehicle(VehicleId, FOBNumber.Replace(" ", ""))

                                            Try
                                                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                    Dim writtenData As String = "VehicleId = " & VehicleId & " ; " & "FOBNumber= " & FOBNumber.Replace(" ", "") & ";"
                                                    OBJServiceBAL = New WebServiceBAL()
                                                    Dim dsIMEI As DataSet = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                                                    CSCommonHelper.WriteLog("Added", "AssignedFOBNumberToVehicle", "", writtenData, dsIMEI.Tables(0).Rows(0)("PersonName") & "(" & dsIMEI.Tables(0).Rows(0)("Email") & ")", IPAddress, "success", "")
                                                End If
                                            Catch ex As Exception

                                            End Try

                                        End If


                                        Dim json As String
                                        json = javaScriptSerializer.Serialize(checkRequireOdoResponse)

                                        context.Response.Write(json)
                                    Else
                                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Vehicle fuel limit for the day exceeded.")
                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg30"))
                                    End If
                                Else
                                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Personal fuel limit for the day exceeded.")
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg31"))
                                End If
                            Else
                                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Transaction Not Found")
                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg51"))
                            End If
                        End If
                    Else
                        log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Transaction Not Found")
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg51"))
                    End If
                Else
                    log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Transaction Not Found")
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg51"))
                End If
            Else
                log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- Hose does not exists")
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg32"))
            End If
        Else
            log.Debug("ProcessRequest: CheckVehicleRequireOdometerEntryAndRequireHourEntry- IMEI_UDID does not exist")
            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg32"))
        End If

    End Sub

    Private Sub ErrorInRequireOdoVehicle(context As HttpContext, ResponceMessage As String, errorString As String, Optional ValidationFailFor As String = "", Optional IsNewFob As String = "No")
        Dim javaScriptSerializer = New JavaScriptSerializer()
        Dim checkRequireOdoResponse = New CheckRequireOdoResponse()
        checkRequireOdoResponse.ResponceMessage = ResponceMessage
        checkRequireOdoResponse.ResponceText = errorString
        checkRequireOdoResponse.ValidationFailFor = ValidationFailFor
        checkRequireOdoResponse.IsOdoMeterRequire = 0
        checkRequireOdoResponse.IsHoursRequire = 0
        checkRequireOdoResponse.CheckOdometerReasonable = ""
        checkRequireOdoResponse.OdometerReasonabilityConditions = 2
        checkRequireOdoResponse.PreviousOdo = 0
        checkRequireOdoResponse.OdoLimit = 0
        checkRequireOdoResponse.FOBNumber = ""
        checkRequireOdoResponse.VehicleNumber = ""
        checkRequireOdoResponse.IsNewFob = IsNewFob
        checkRequireOdoResponse.PreviousHours = 0
        checkRequireOdoResponse.HoursLimit = 0

        Dim json As String
        json = javaScriptSerializer.Serialize(checkRequireOdoResponse)
        context.Response.Write(json)

    End Sub

    Private Sub SendRegistrationEmailToCustomerAdmins(emailTo As String, userName As String, context As HttpContext, IsRegister As Boolean)
        Try

            Dim body As String = String.Empty
            If (IsRegister = True) Then
                Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/SuccessfulRe-RegistrationEmailToAdmin.txt"))
                    body = sr.ReadToEnd()
                End Using
            Else
                Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/SuccessfulRegistrationEmailToAdmin.txt"))
                    body = sr.ReadToEnd()
                End Using

            End If
            '------------------
            body = body.Replace("userName", userName)
            body = body.Replace("FluidSecureCloudLink", String.Format("<a href='{0}' target='_blank'>FluidSecure Cloud</a>", context.Request.Url.ToString().Replace(context.Request.Url.AbsolutePath, "/Account/login")))

            Dim e As New EmailService()


            Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))

            mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
            mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

            Dim messageSend As New MailMessage()
            messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
            log.Info("Email send to: " + emailTo)
            'messageSend.To.Add(emailTo)

            messageSend.Subject = "New User Registered" 'User Registration
            messageSend.Body = body

            messageSend.IsBodyHtml = True
            mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))

            Dim emailArray As String() = emailTo.Split(",")

            For index = 0 To emailArray.Length - 1
                messageSend.To.Add(emailArray(index))
                mailClient.Send(messageSend)
                log.Info("Email send to: " + emailArray(index))
                messageSend.To.Remove(New MailAddress(emailArray(index)))
            Next


        Catch ex As Exception
            log.Debug("Exception occurred in SendRegistrationEmailToCustomerAdmins to EmailId : " & emailTo & ". ex is :" & ex.Message)
        End Try
    End Sub

    Public Function GetLocationAddress(CurrentLat As String, CurrentLng As String) As String

        Dim CurrentLocationAddress As String = ""

        Try

            Dim client As HttpClient = New HttpClient()
            Dim CallUrl As String = String.Format("http://maps.google.com/maps/api/geocode/json?latlng={0},{1}", CurrentLat, CurrentLng)
            Dim msg As New HttpRequestMessage(System.Net.Http.HttpMethod.[Get], CallUrl)
            msg.Headers.TryAddWithoutValidation("accept", "application/json")
            msg.Headers.TryAddWithoutValidation("Content-Type", "application/json")


            Dim result = client.SendAsync(msg).Result
            result.EnsureSuccessStatusCode()

            Dim ResponseObject = result.Content.ReadAsStringAsync()

            Dim parseJsonObject As JObject = JObject.Parse(ResponseObject.Result)

            If (parseJsonObject("status").ToString() = "OVER_QUERY_LIMIT") Then
                CurrentLocationAddress = ""
            Else
                CurrentLocationAddress = parseJsonObject("results")(0)("formatted_address").ToString()
            End If

        Catch ex As Exception
            'log.Debug("Exception occurred in GetLocationAddress to CurrentLat : " & CurrentLat & " , CurrentLng : " & CurrentLng & " . ex is :" & ex.Message)
        End Try

        Return CurrentLocationAddress
    End Function

    Private Function SaveFailedTransactions(data As String, IMEI_UDID As String) As String
        Dim requestJson As String = ""
        Try

            'Dim data = [String].Empty
            'Using inputStream = New StreamReader(context.Request.InputStream)
            'data = inputStream.ReadToEnd()
            Dim CreateDataFor As String = ""
            requestJson = data
            log.Info("data : " & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(TransactionComplete))
            Dim VehicleId = DirectCast(serJsonDetails, TransactionComplete).VehicleId
            Dim SiteId = DirectCast(serJsonDetails, TransactionComplete).SiteId
            Dim PersonId = DirectCast(serJsonDetails, TransactionComplete).PersonId
            Dim CurrentOdometer = DirectCast(serJsonDetails, TransactionComplete).CurrentOdometer
            Dim FuelQuantity = DirectCast(serJsonDetails, TransactionComplete).FuelQuantity
            Dim FuelTypeId = DirectCast(serJsonDetails, TransactionComplete).FuelTypeId
            Dim PhoneNumber = DirectCast(serJsonDetails, TransactionComplete).PhoneNumber
            Dim WifiSSId = DirectCast(serJsonDetails, TransactionComplete).WifiSSId
            Dim TransactionDate = DirectCast(serJsonDetails, TransactionComplete).TransactionDate
            Dim TransactionFrom = DirectCast(serJsonDetails, TransactionComplete).TransactionFrom
            Dim CurrentLat = DirectCast(serJsonDetails, TransactionComplete).CurrentLat
            Dim CurrentLng = DirectCast(serJsonDetails, TransactionComplete).CurrentLng
            Dim VehicleNumber = DirectCast(serJsonDetails, TransactionComplete).VehicleNumber

            Dim DepartmentNumber = DirectCast(serJsonDetails, TransactionComplete).DepartmentNumber
            Dim PersonnelPIN = DirectCast(serJsonDetails, TransactionComplete).PersonnelPIN
            Dim Other = DirectCast(serJsonDetails, TransactionComplete).Other
            Dim Hours = DirectCast(serJsonDetails, TransactionComplete).Hours

            If (Not PersonnelPIN Is Nothing) Then
                PersonnelPIN = PersonnelPIN.Trim()
            Else
                PersonnelPIN = ""
            End If

            If (Not VehicleNumber Is Nothing) Then
                VehicleNumber = VehicleNumber.Trim()
            Else
                VehicleNumber = ""
            End If

            Dim transactionCompleteResponceObj = New TransactionCompleteResponce()
            Dim json As String

            'If (FuelQuantity <= 0) Then

            '    transactionCompleteResponceObj.ResponceMessage = "fail"
            '    transactionCompleteResponceObj.ResponceText = "Transaction with fuel quantity 0 is not allowed."

            '    json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
            '    context.Response.Write(json)
            'Else

            Dim CurrentLocationAddress As String = ""

            Try
                Dim client As HttpClient = New HttpClient()
                Dim CallUrl As String = String.Format("http://maps.google.com/maps/api/geocode/json?latlng={0},{1}", CurrentLat, CurrentLng)
                Dim msg As New HttpRequestMessage(System.Net.Http.HttpMethod.[Get], CallUrl)
                msg.Headers.TryAddWithoutValidation("accept", "application/json")
                msg.Headers.TryAddWithoutValidation("Content-Type", "application/json")


                Dim result = client.SendAsync(msg).Result
                result.EnsureSuccessStatusCode()

                Dim ResponseObject = result.Content.ReadAsStringAsync()

                Dim parseJsonObject As JObject = JObject.Parse(ResponseObject.Result)

                CurrentLocationAddress = parseJsonObject("results")(0)("formatted_address").ToString()

            Catch ex As Exception

            End Try

            If (TransactionFrom = Nothing Or TransactionFrom = "") Then

                TransactionFrom = "M"

            End If

            If (VehicleNumber = Nothing) Then

                VehicleNumber = ""

            End If

            If (DepartmentNumber = Nothing) Then

                DepartmentNumber = ""

            End If

            'make IsBusy is false
            Try

                Dim OBJWebServiceBAL = New WebServiceBAL()

                OBJWebServiceBAL.ChangeBusyStatusOfFluidSecureUnit(SiteId, False, IMEI_UDID)

            Catch ex As Exception
                log.Error("Error occurred in Failed ChangeBusyStatusOfFluidSecureUnit. Exception is :" & ex.Message)
            End Try

            Dim OBJMasterBAL = New MasterBAL()
            Dim TransactionId As Integer

            Dim dsTransactionValuesData As DataSet
            Dim DepartmentName As String = ""
            Dim FuelTypeName As String = ""
            Dim Email As String = ""
            Dim PersonName As String = ""
            Dim CompanyName As String = ""
            Dim VehicleName As String = ""

            dsTransactionValuesData = OBJMasterBAL.GetTransactionColumnsValueForSave(DepartmentNumber, FuelTypeId, PersonId, VehicleId)

            If dsTransactionValuesData IsNot Nothing Then
                If dsTransactionValuesData.Tables.Count > 0 Then
                    If dsTransactionValuesData.Tables(0) IsNot Nothing And dsTransactionValuesData.Tables(0).Rows.Count > 0 Then
                        DepartmentName = dsTransactionValuesData.Tables(0).Rows(0)("DeptName").ToString()
                        DepartmentNumber = dsTransactionValuesData.Tables(0).Rows(0)("DeptNumber").ToString()
                    End If
                    If dsTransactionValuesData.Tables(1) IsNot Nothing And dsTransactionValuesData.Tables(1).Rows.Count > 0 Then
                        FuelTypeName = dsTransactionValuesData.Tables(1).Rows(0)("FuelTypeName").ToString()
                    End If
                    If dsTransactionValuesData.Tables(2) IsNot Nothing And dsTransactionValuesData.Tables(2).Rows.Count > 0 Then
                        Email = dsTransactionValuesData.Tables(2).Rows(0)("Email").ToString()
                        PersonName = dsTransactionValuesData.Tables(2).Rows(0)("PersonName").ToString()
                    End If
                    If dsTransactionValuesData.Tables(3) IsNot Nothing And dsTransactionValuesData.Tables(3).Rows.Count > 0 Then
                        CompanyName = dsTransactionValuesData.Tables(3).Rows(0)("CompanyName").ToString()
                    End If
                    If dsTransactionValuesData.Tables(4) IsNot Nothing And dsTransactionValuesData.Tables(4).Rows.Count > 0 Then
                        VehicleName = dsTransactionValuesData.Tables(4).Rows(0)("VehicleName").ToString()
                        VehicleNumber = IIf(VehicleNumber.Trim().ToLower().Contains("guest"), VehicleNumber.Trim(), dsTransactionValuesData.Tables(4).Rows(0)("VehicleNumber").ToString().Trim())
                    End If
                End If
            End If


            TransactionId = OBJMasterBAL.InsertUpdateTransaction(VehicleId, SiteId, PersonId, CurrentOdometer, FuelQuantity, FuelTypeId, PhoneNumber, WifiSSId.ToString().Trim(), TransactionDate,
                                                              0, 0, TransactionFrom, 0, Convert.ToDouble(CurrentLat).ToString("0.00000"), Convert.ToDouble(CurrentLng).ToString("0.00000"), CurrentLocationAddress,
                                                              VehicleNumber.Trim(), DepartmentNumber, PersonnelPIN.Trim(), Other, IIf(Hours = "", -1, Hours), False, False, 2, 0, 0,
                                                                 VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, 0, Convert.ToInt32(dsTransactionValuesData.Tables(3).Rows(0)("CustomerId")), 0, 0, 0) '

            If TransactionId <> 0 Then 'success
                Try

                    'IsOdometerRequire on screen mobile application
                    OBJMasterBAL.UpdateVehicleCurrentOdometer(VehicleId, CurrentOdometer)

                    Dim dtSingleTransacton As DataTable = New DataTable()
                    dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId, False)

                    Try
                        TransactionDate = dtSingleTransacton.Rows(0)("TransactionDateTime")
                    Catch ex As Exception

                    End Try
                    CreateDataFor = VehicleNumber.Trim() & ";" & VehicleName & ";" & DepartmentName & ";" & DepartmentNumber & ";" & VehicleNumber.Trim() & ";" & WifiSSId.ToString().Trim() & ";" & FuelQuantity & ";" &
                            Other & ";" & CompanyName & ";" & "" & ";" & Convert.ToDateTime(TransactionDate).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(TransactionDate).ToString("hh:mm:tt") & ";" &
                            PersonName & ";" & PersonnelPIN.Trim() & ";" & CurrentOdometer & ";" & dtSingleTransacton.Rows(0)("PreviousOdometer").ToString() & ";" & Hours & ";" & FuelTypeName & ";" & "Completed" & ";" & "0"


                    If (IIf(IsDBNull(dtSingleTransacton.Rows(0)("SendTransactionEmail")), False, dtSingleTransacton.Rows(0)("SendTransactionEmail")) = True) Then
                        SendTransactionEmail(dtSingleTransacton.Rows(0)("Email"), dtSingleTransacton.Rows(0)("Date"), dtSingleTransacton.Rows(0)("Time"),
                                        dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim(), dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim(), dtSingleTransacton.Rows(0)("FuelType"),
                                        dtSingleTransacton.Rows(0)("TransactionNumber"), dtSingleTransacton.Rows(0)("FuelQuantity"), dtSingleTransacton.Rows(0)("AdditionalEmailId"), dtSingleTransacton.Rows(0)("IsUserForHub"),
                                        dtSingleTransacton.Rows(0)("IsOtherRequire"), dtSingleTransacton.Rows(0)("OtherLabel"), dtSingleTransacton.Rows(0)("Other"),
                                        dtSingleTransacton.Rows(0)("PersonName"), dtSingleTransacton.Rows(0)("VehicleName"), "", TransactionId)

                        OBJMasterBAL = New MasterBAL()
                        OBJMasterBAL.SetIsTransactionMailSent(TransactionId, FuelQuantity)

                    End If

                Catch ex As Exception
                    log.Debug("Exception occurred in Failed sending transaction. Transaction # " & TransactionId & ". ex is :" & ex.Message)
                End Try

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                    CSCommonHelper.WriteLog("Added", "Transactions", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "success", "")
                End If
                transactionCompleteResponceObj.ResponceMessage = "success"
                transactionCompleteResponceObj.ResponceText = "Transaction completed successfully!"
            Else 'fail
                transactionCompleteResponceObj.ResponceMessage = "fail"
                transactionCompleteResponceObj.ResponceText = "Error occurred during transaction!"

                Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

                Dim logtransaction As String = String.Format("Again Storing Failed transactions details: VehicleId : {0},SiteId:{1},PersonId:{2}, CurrentOdometer:{3}, FuelQuantity:{4}, FuelTypeId:{5}, PhoneNumber:{6}, WifiSSId:{7}," &
                                                             "TransactionDate:{8},TransactionFrom:{9},CurrentLat:{10},CurrentLng:{11},VehicleNumber:{12},DepartmentNumber:{13},PersonnelPIN:{14},Hours:{15}",
                                                             VehicleId, SiteId, PersonId, CurrentOdometer, FuelQuantity, FuelTypeId, PhoneNumber, WifiSSId.ToString().Trim(), TransactionDate, TransactionFrom, CurrentLat, CurrentLng, VehicleNumber.Trim(),
                                                             DepartmentNumber, PersonnelPIN.Trim(), Hours)
                TransactionFailedLog.Error(logtransaction)
                CreateDataFor = VehicleNumber.Trim() & ";" & VehicleName & ";" & DepartmentName & ";" & DepartmentNumber & ";" & VehicleNumber.Trim() & ";" & WifiSSId.ToString().Trim() & ";" & FuelQuantity & ";" &
                            Other & ";" & CompanyName & ";" & "" & ";" & Convert.ToDateTime(TransactionDate).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(TransactionDate).ToString("hh:mm:tt") & ";" &
                            PersonName & ";" & PersonnelPIN.Trim() & ";" & CurrentOdometer & ";" & "" & ";" & Hours & ";" & FuelTypeName & ";" & "Completed" & ";" & "0"

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                    CSCommonHelper.WriteLog("Added", "Transactions", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "fail", "Transaction Save Failed.")
                End If

            End If
            json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
            Return json

            'End If

            'End Using

        Catch ex As Exception

            Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            TransactionFailedLog.Error("Exception occurred while Failed prcessing request. Exception is :" & ex.Message & " . Details : " & requestJson)

            'ErrorInAuthontication(context, "fail", "Some error has occurred, please try again after some time.")
            Return ""
        End Try

    End Function

#Region "Pre-AuthTransactions"

    Public Function PreAuthTransactionsDetails(personId As Integer) As ResponsePreAuthTransactions
        steps = "24_214"
        Dim rootOject = New ResponsePreAuthTransactions()
        rootOject.ResponceMessage = "success"
        rootOject.ResponceText = "PreAuthTransactions Data"

        rootOject.SitesObj = New List(Of SSIDData)()
        rootOject.TransactionObj = New List(Of Transactions)()

        OBJMasterBAL = New MasterBAL()

        Dim dtSites As DataTable = New DataTable()
        steps = "24_215"
        dtSites = OBJMasterBAL.GetPersonSiteMapping(personId, 0)
        steps = "24_216"
        If Not dtSites Is Nothing Then
            If dtSites.Rows.Count > 0 Then

                For index As Integer = 0 To dtSites.Rows.Count - 1
                    steps = "24_217"
                    Dim row As DataRow = dtSites.Rows(index)

                    Dim objSSIDData = New SSIDData()
                    objSSIDData.ResponceMessage = "success"
                    objSSIDData.ResponceText = "SSID Data"
                    objSSIDData.WifiSSId = row("WifiSSId").ToString().Trim()
                    objSSIDData.SiteId = row("SiteId")
                    'log.Info("PulserRatio :" & row("PulserRatio"))
                    'log.Info("PulserRatio -1 : " & row("PulserRatio").ToString())
                    Dim pulseRatio = Decimal.Parse(row("PulserRatio").ToString())
                    'log.Info("PulserRatio -2 : " & pulseRatio)
                    objSSIDData.PulserRatio = pulseRatio
                    objSSIDData.DecimalPulserRatio = pulseRatio
                    'log.Info(objSSIDData.PulserRatio)
                    objSSIDData.FuelTypeId = row("FuelTypeId")
                    objSSIDData.Password = ConfigurationManager.AppSettings("FSPassword").ToString()
                    objSSIDData.PumpOnTime = row("PumpOnTime")
                    objSSIDData.PumpOffTime = row("PumpOffTime")
                    objSSIDData.PulserStopTime = ConfigurationManager.AppSettings("PulserStopTime").ToString()
                    objSSIDData.ReconfigureLink = IIf(row("ReconfigureLink"), "True", "False")
                    rootOject.SitesObj.Add(objSSIDData)

                Next

            Else
                'ErrorInAuthontication(context, "fail", "sites not found for person, Please contact administrator.")
                Dim objSSIDData = New SSIDData()
                objSSIDData.ResponceMessage = "fail"
                objSSIDData.ResponceText = "sites not found for person, Please contact administrator."
                rootOject.SitesObj.Add(objSSIDData)

            End If
        Else
            Dim objSSIDData = New SSIDData()
            objSSIDData.ResponceMessage = "fail"
            objSSIDData.ResponceText = "sites not found for person, Please contact administrator."
            rootOject.SitesObj.Add(objSSIDData)

        End If
        steps = "24_218"
        OBJMasterBAL = New MasterBAL()

        Dim dtPreAuthTransactions As DataTable = New DataTable()

        dtPreAuthTransactions = OBJMasterBAL.GetPreAuthTransactionsByPersonId(personId, True)
        steps = "24_219"
        If Not dtPreAuthTransactions Is Nothing Then
            If dtPreAuthTransactions.Rows.Count > 0 Then

                For index As Integer = 0 To dtPreAuthTransactions.Rows.Count - 1
                    Dim row As DataRow = dtPreAuthTransactions.Rows(index)

                    Dim objTransData = New Transactions()
                    objTransData.ResponceMessage = "success"
                    objTransData.ResponceText = "Transaction data"
                    objTransData.TransactionId = row("TransactionId")
                    rootOject.TransactionObj.Add(objTransData)
                Next

            Else
                Dim objTransData = New Transactions()
                objTransData.ResponceMessage = "fail"
                objTransData.ResponceText = "Pre-Auth Transactions not found for person, Please contact administrator."
                rootOject.TransactionObj.Add(objTransData)
            End If
        Else
            Dim objTransData = New Transactions()
            objTransData.ResponceMessage = "fail"
            objTransData.ResponceText = "Pre-Auth Transactions not found for person, Please contact administrator."
            rootOject.TransactionObj.Add(objTransData)
        End If
        steps = "24_220"
        Return rootOject

    End Function

    Private Sub SavePreAuthTransactions(context As HttpContext, Imei As String)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("PreAuthTransactions : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(TransactionComplete))
                Dim SiteId = DirectCast(serJsonDetails, TransactionComplete).SiteId
                'Dim PersonId = DirectCast(serJsonDetails, TransactionComplete).PersonId
                Dim CurrentOdometer = DirectCast(serJsonDetails, TransactionComplete).CurrentOdometer
                Dim FuelQuantity = DirectCast(serJsonDetails, TransactionComplete).FuelQuantity
                Dim FuelTypeId = DirectCast(serJsonDetails, TransactionComplete).FuelTypeId
                Dim WifiSSId = DirectCast(serJsonDetails, TransactionComplete).WifiSSId
                Dim TransactionDate = DirectCast(serJsonDetails, TransactionComplete).TransactionDate
                Dim TransactionFrom = DirectCast(serJsonDetails, TransactionComplete).TransactionFrom
                Dim CurrentLat = DirectCast(serJsonDetails, TransactionComplete).CurrentLat
                Dim CurrentLng = DirectCast(serJsonDetails, TransactionComplete).CurrentLng
                Dim VehicleNumber = DirectCast(serJsonDetails, TransactionComplete).VehicleNumber
                Dim TransactionId = DirectCast(serJsonDetails, TransactionComplete).TransactionId
                Dim Pulses = DirectCast(serJsonDetails, TransactionComplete).Pulses

                If (Not VehicleNumber Is Nothing) Then
                    VehicleNumber = VehicleNumber.Trim()
                Else
                    VehicleNumber = ""
                End If

                Dim transactionCompleteResponceObj = New TransactionCompleteResponce()
                Dim json As String

                log.Debug("Pre-Auth TransactionId : " & TransactionId)

                OBJMasterBAL = New MasterBAL()
                If (TransactionId <> 0) Then
                    Dim OBJMasterBAL = New MasterBAL()

                    Dim dtSingleTransacton As DataTable = New DataTable()
                    dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId, False)

                    If (dtSingleTransacton.Rows.Count = 0) Then
                        log.Error("TransactionId not found. TransactionId : " & TransactionId)

                        ErrorInAuthontication(context, "fail", "TransactionId not found.")

                        Return
                    End If

                    Dim dsIMEI = New DataSet()
                    OBJServiceBAL = New WebServiceBAL()
                    dsIMEI = OBJServiceBAL.IsIMEIExists(Imei)
                    Dim UserCustomerId As Integer = Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString())
                    Dim UserCustomerName As String = dsIMEI.Tables(0).Rows(0)("CustomerName").ToString()
                    Dim TransactionCustomerId As Integer = dtSingleTransacton.Rows(0)("CustomerId").ToString()
                    Dim TransactionCustomerName As String = dtSingleTransacton.Rows(0)("Company").ToString()

                    If (UserCustomerId <> TransactionCustomerId Or UserCustomerName <> TransactionCustomerName) Then

                        transactionCompleteResponceObj.ResponceMessage = "success"
                        transactionCompleteResponceObj.ResponceText = "Transaction from another company so ignore it!"
                        log.Error("Transaction from another company so ignore it. TransactionId : " & TransactionId & " Hub company Name : " & UserCustomerName & " Transaction company Name :  " & TransactionCustomerName)
                        json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                        context.Response.Write(json)

                        Return
                    End If

                    TransactionId = OBJMasterBAL.UpdatePreAuthTransactions(SiteId, CurrentOdometer, FuelQuantity, FuelTypeId, WifiSSId.ToString().Trim(), TransactionDate, TransactionId, TransactionFrom, CurrentLat, CurrentLng, VehicleNumber.Trim(), Pulses)

                    If (TransactionId <> 0) Then


                        transactionCompleteResponceObj.ResponceMessage = "success"
                        transactionCompleteResponceObj.ResponceText = "Transaction completed successfully!"


                        dtSingleTransacton = OBJMasterBAL.GetTransactionById(TransactionId, False)

                        If (dtSingleTransacton.Rows.Count = 0) Then
                            log.Error("Transaction not found. TransactionId : " & TransactionId)

                            ErrorInAuthontication(context, "fail", "Transaction not found.")

                            Return
                        End If

                        Try

                            If (IIf(IsDBNull(dtSingleTransacton.Rows(0)("SendTransactionEmail")), False, dtSingleTransacton.Rows(0)("SendTransactionEmail")) = True) Then
                                SendTransactionEmail(dtSingleTransacton.Rows(0)("Email"), dtSingleTransacton.Rows(0)("Date"), dtSingleTransacton.Rows(0)("Time"),
                                                dtSingleTransacton.Rows(0)("VehicleNumber").ToString().Trim(), dtSingleTransacton.Rows(0)("WifiSSId").ToString().Trim(), dtSingleTransacton.Rows(0)("FuelType"),
                                                dtSingleTransacton.Rows(0)("TransactionNumber"), dtSingleTransacton.Rows(0)("FuelQuantity"), dtSingleTransacton.Rows(0)("AdditionalEmailId"), dtSingleTransacton.Rows(0)("IsUserForHub"),
                                                dtSingleTransacton.Rows(0)("IsOtherRequire"), dtSingleTransacton.Rows(0)("OtherLabel"), dtSingleTransacton.Rows(0)("Other"),
                                                dtSingleTransacton.Rows(0)("PersonName"), dtSingleTransacton.Rows(0)("VehicleName"), "", TransactionId)

                                OBJMasterBAL = New MasterBAL()

                                OBJMasterBAL.SetIsTransactionMailSent(TransactionId, FuelQuantity)


                            End If

                        Catch ex As Exception
                            log.Debug("Exception occurred in sending pre Auth transaction email. Transaction # " & TransactionId & ". ex is :" & ex.Message)
                        End Try

                    Else
                        transactionCompleteResponceObj.ResponceMessage = "fail"
                        transactionCompleteResponceObj.ResponceText = "Error occurred during transaction!"

                        Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

                        Dim logtransaction As String = String.Format("Failed Pre-Auth transactions details: SiteId : {0}, CurrentOdometer : {1}, FuelQuantity : {2}, FuelTypeId : {3}, WifiSSId : {4}, " &
                                                                     "TransactionDate : {5}, TransactionId : {6}, TransactionFrom : {7}, CurrentLat : {8}, CurrentLng : {9}, VehicleNumber : {10}",
                                                                     SiteId, CurrentOdometer, FuelQuantity, FuelTypeId, WifiSSId.ToString().Trim(), TransactionDate, TransactionId, TransactionFrom, CurrentLat, CurrentLng, VehicleNumber.Trim())


                        TransactionFailedLog.Error(logtransaction)
                    End If



                Else
                    transactionCompleteResponceObj.ResponceMessage = "fail"
                    transactionCompleteResponceObj.ResponceText = "TransactionId is not valid!"

                    Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

                    Dim logtransaction As String = String.Format("Failed Pre-Auth transactions details: SiteId : {0}, CurrentOdometer : {1}, FuelQuantity : {2}, FuelTypeId : {3}, WifiSSId : {4}, " &
                                                           "TransactionDate : {5}, TransactionId : {6}, TransactionFrom : {7}, CurrentLat : {8}, CurrentLng : {9}, VehicleNumber : {10}",
                                                           SiteId, CurrentOdometer, FuelQuantity, FuelTypeId, WifiSSId.ToString().Trim(), TransactionDate, TransactionId, TransactionFrom, CurrentLat, CurrentLng, VehicleNumber.Trim())


                    TransactionFailedLog.Error(logtransaction)

                End If

                json = javaScriptSerializer.Serialize(transactionCompleteResponceObj)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            TransactionFailedLog.Error("Exception occurred while prcessing request. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try

    End Sub

#End Region

    Private Function UpgradeCurrentVersionWithUgradableVersion(context As HttpContext)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("UpgradeCurrentVersionWithUgradableVersion : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(UpgradeCurrentVersionWithUgradableVersionMaster))
                Dim HoseId = DirectCast(serJsonDetails, UpgradeCurrentVersionWithUgradableVersionMaster).HoseId

                Dim Version = DirectCast(serJsonDetails, UpgradeCurrentVersionWithUgradableVersionMaster).Version

                Dim UpgradeCurrentVersionWithUgradableVersionResponseObj = New UpgradeCurrentVersionWithUgradableVersionResponse()
                Dim json As String

                log.Debug("Update HoseId : " & HoseId)
                log.Debug("With Version : " & Version)

                OBJMasterBAL = New MasterBAL()
                If (HoseId <> "") Then
                    If (Version <> "") Then
                        Dim dsUpgrade As DataSet
                        dsUpgrade = OBJMasterBAL.checkLaunchedAndExistedVersionAndUpdate(HoseId, Version, 0, 1)
                        Dim resultUpgradable As String = ""
                        If dsUpgrade IsNot Nothing Then
                            If dsUpgrade.Tables(0) IsNot Nothing Then
                                If dsUpgrade.Tables(0).Rows.Count > 0 Then
                                    resultUpgradable = dsUpgrade.Tables(0).Rows(0)("resultUpgradable")
                                Else
                                    resultUpgradable = "fail"
                                End If
                            Else
                                resultUpgradable = "fail"
                            End If
                        Else
                            resultUpgradable = "fail"
                        End If

                        If resultUpgradable IsNot "fail" Then
                            UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceMessage = "success"
                            UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceText = "Update completed successfully!"
                        Else
                            UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceMessage = "fail"
                            UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceText = "Error occurred during Update. !"
                        End If
                    Else
                        UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceMessage = "fail"
                        UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceText = "Error occurred during Update:Version blank !"
                    End If
                Else
                    UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceMessage = "fail"
                    UpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceText = "Error occurred during Update:HoseId blank !"
                End If

                json = javaScriptSerializer.Serialize(UpgradeCurrentVersionWithUgradableVersionResponseObj)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            log.Error("Exception occurred while prcessing request in UpgradeCurrentVersionWithUgradableVersion. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try
        Return ""
    End Function

    Private Function IsUpgradeCurrentVersionWithUgradableVersion(context As HttpContext)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("IsUpgradeCurrentVersionWithUgradableVersion : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(IsUpgradeCurrentVersionWithUgradableVersionMaster))
                Dim HoseId = DirectCast(serJsonDetails, IsUpgradeCurrentVersionWithUgradableVersionMaster).HoseId

                Dim PersonId = DirectCast(serJsonDetails, IsUpgradeCurrentVersionWithUgradableVersionMaster).PersonId

                Dim ISUpgradeCurrentVersionWithUgradableVersionResponseObj = New IsUpgradeCurrentVersionWithUgradableVersionResponse()
                Dim json As String

                log.Debug("HoseId : " & HoseId)
                log.Debug("With PersonId : " & PersonId)

                OBJMasterBAL = New MasterBAL()
                If (HoseId <> "") Then
                    If (PersonId <> "") Then
                        'check is upgradable or not
                        Dim resultUpgradable As String = ""
                        OBJMasterBAL = New MasterBAL()
                        Dim dsUpgrade As DataSet
                        dsUpgrade = OBJMasterBAL.checkLaunchedAndExistedVersionAndUpdate(HoseId, "", PersonId, 0)

                        If dsUpgrade IsNot Nothing Then
                            If dsUpgrade.Tables(0) IsNot Nothing Then
                                If dsUpgrade.Tables(0).Rows.Count > 0 Then
                                    resultUpgradable = dsUpgrade.Tables(0).Rows(0)("resultUpgradable")
                                Else
                                    resultUpgradable = "N"
                                End If
                            Else
                                resultUpgradable = "N"
                            End If
                        Else
                            resultUpgradable = "N"
                        End If
                        ISUpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceMessage = "success"
                        ISUpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceText = resultUpgradable
                    Else
                        ISUpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceMessage = "fail"
                        ISUpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceText = "Error occurred during Update:PersonId blank !"
                    End If
                Else
                    ISUpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceMessage = "fail"
                    ISUpgradeCurrentVersionWithUgradableVersionResponseObj.ResponceText = "Error occurred during Update:HoseId blank !"
                End If

                json = javaScriptSerializer.Serialize(ISUpgradeCurrentVersionWithUgradableVersionResponseObj)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            log.Error("Exception occurred while prcessing request in IsUpgradeCurrentVersionWithUgradableVersion. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try
        Return ""
    End Function

    Private Function UpgradeTransactionStatus(context As HttpContext)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("UpgradeTransactionStatus : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(UpgradeTransactionStatusMaster))
                Dim TransactionId = DirectCast(serJsonDetails, UpgradeTransactionStatusMaster).TransactionId

                Dim Status = DirectCast(serJsonDetails, UpgradeTransactionStatusMaster).Status

                Dim UpgradeTransactionStatusResponseObj = New UpgradeTransactionStatusResponse()
                Dim json As String

                log.Debug("TransactionId : " & TransactionId)
                log.Debug("With Status : " & Status)

                OBJMasterBAL = New MasterBAL()
                If (TransactionId <> 0) Then
                    If (Status <> 0) Then
                        Dim dsUpgrade As DataSet
                        dsUpgrade = OBJMasterBAL.SpUpgradeTransactionStatus(TransactionId, Status, 0)
                        Dim resultUpgradable As String = ""
                        If dsUpgrade IsNot Nothing Then
                            If dsUpgrade.Tables(0) IsNot Nothing Then
                                If dsUpgrade.Tables(0).Rows.Count > 0 Then
                                    resultUpgradable = dsUpgrade.Tables(0).Rows(0)("resultUpgradable")
                                Else
                                    resultUpgradable = "fail"
                                End If
                            Else
                                resultUpgradable = "fail"
                            End If
                        Else
                            resultUpgradable = "fail"
                        End If

                        If resultUpgradable IsNot "fail" Then
                            UpgradeTransactionStatusResponseObj.ResponceMessage = "success"
                            UpgradeTransactionStatusResponseObj.ResponceText = "Update completed successfully!"
                        Else
                            UpgradeTransactionStatusResponseObj.ResponceMessage = "fail"
                            UpgradeTransactionStatusResponseObj.ResponceText = "Error occurred during Update. !"
                        End If
                    Else
                        UpgradeTransactionStatusResponseObj.ResponceMessage = "fail"
                        UpgradeTransactionStatusResponseObj.ResponceText = "Error occurred during Update:Status 0 !"
                    End If
                Else
                    UpgradeTransactionStatusResponseObj.ResponceMessage = "fail"
                    UpgradeTransactionStatusResponseObj.ResponceText = "Error occurred during Update:TransactionId 0 !"
                End If

                json = javaScriptSerializer.Serialize(UpgradeTransactionStatusResponseObj)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            log.Error("Exception occurred while prcessing request in UpgradeTransactionStatus. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try
        Return ""
    End Function

    Private Function UpgradeIsBusyStatus(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("UpgradeIsBusyStatus : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(UpgradeIsBusyStatusMaster))
                Dim SiteId = DirectCast(serJsonDetails, UpgradeIsBusyStatusMaster).SiteId

                Dim UpgradeUpgradeIsBusyStatusResponseResponseObj = New UpgradeIsBusyStatusResponse()
                Dim json As String

                log.Debug("SiteId : " & SiteId)

                OBJMasterBAL = New MasterBAL()
                If (SiteId <> 0) Then
                    Try
                        log.Debug("UpgradeIsBusyStatus 1")
                        Dim OBJWebServiceBAL = New WebServiceBAL()
                        Dim dtSite As DataTable = OBJMasterBAL.GetHoseIdBySiteId(SiteId)
                        log.Debug("UpgradeIsBusyStatus 2")

                        If (dtSite.Rows.Count > 0) Then
                            log.Debug(dtSite.Rows(0)("IsBusy"))
                            If (Boolean.Parse(dtSite.Rows(0)("IsBusy")) = True And IMEI_UDID <> dtSite.Rows(0)("BusyFromIMEI_UDID")) Then
                                log.Debug("UpgradeIsBusyStatus 3")
                                UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceMessage = "success"
                                UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceText = "Y"
                            Else
                                log.Debug("UpgradeIsBusyStatus 4")
                                OBJWebServiceBAL.ChangeBusyStatusOfFluidSecureUnit(SiteId, True, IMEI_UDID)
                                UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceMessage = "success"
                                UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceText = "Busy"
                            End If
                        Else
                            log.Debug("UpgradeIsBusyStatus 5")
                            UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceMessage = "fail"
                            UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceText = "Not Busy"
                            log.Error("Error occurred in ChangeBusyStatusOfFluidSecureUnit. Site details not found")
                        End If
                    Catch ex As Exception
                        log.Debug("UpgradeIsBusyStatus 6")
                        UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceMessage = "fail"
                        UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceText = "Not Busy"
                        log.Error("Error occurred in ChangeBusyStatusOfFluidSecureUnit. Exception is :" & ex.Message)
                    End Try
                Else
                    log.Debug("UpgradeIsBusyStatus 7")
                    UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceMessage = "fail"
                    UpgradeUpgradeIsBusyStatusResponseResponseObj.ResponceText = "Not Busy"
                End If

                log.Debug("UpgradeIsBusyStatus 8")

                json = javaScriptSerializer.Serialize(UpgradeUpgradeIsBusyStatusResponseResponseObj)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            log.Error("Exception occurred while prcessing request in UpgradeIsBusyStatus. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try
        Return ""
    End Function

    Private Function CheckValidRequest(email As String, IMEI_UDID As String) As Boolean
        If (IMEI_UDID <> "" And email <> "") Then
            Dim dsEmail = New DataSet()
            dsEmail = OBJServiceBAL.IsEmailExists(email)
            If Not dsEmail Is Nothing Then
                If dsEmail.Tables.Count > 0 And dsEmail.Tables(0).Rows.Count > 0 Then
                    If (dsEmail.Tables(0).Rows(0)("IMEI_UDID").ToString().Contains(IMEI_UDID)) Then
                        Return True
                    Else
                        Return True
                    End If
                Else
                    Return True
                End If
            Else
                Return True
            End If
        Else
            Return True
        End If
    End Function

    Private Sub CheckValidPersonPinOrFOBNUmber(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("CheckValidPinOrFOBNUmber : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(CheckValidPersonPinOrFOBNUmber))
                Dim PersonFOBNumber = DirectCast(serJsonDetails, CheckValidPersonPinOrFOBNUmber).PersonFOBNumber
                Dim PersonPIN = DirectCast(serJsonDetails, CheckValidPersonPinOrFOBNUmber).PersonPIN
                Dim IsNewFob As String = "No"

                If (Not PersonPIN Is Nothing) Then
                    PersonPIN = PersonPIN.Trim()
                Else
                    PersonPIN = ""
                End If


                Dim CheckValidPersonPinOrFOBNUmberResponse = New CheckValidPersonPinOrFOBNUmberResponse()

                OBJMasterBAL = New MasterBAL()
                Dim dsIMEI = New DataSet()
                dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                    If Not dsIMEI.Tables(0) Is Nothing Then
                        If dsIMEI.Tables(0).Rows.Count <> 0 Then
                            Dim personId = Integer.Parse(dsIMEI.Tables(0).Rows(0)("PersonId").ToString())
                            Dim customerId As Integer
                            customerId = Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString())
                            Dim RoleId As String
                            RoleId = dsIMEI.Tables(0).Rows(0)("RoleId").ToString()
                            log.Info("CheckValidPersonPinOrFOBNUmber : 1")
                            If (PersonPIN.Trim() = "" And PersonFOBNumber <> "") Then
                                Dim dtPersonForFobNumber As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','')='" & PersonFOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                If (dtPersonForFobNumber.Rows.Count > 0) Then
                                    log.Info("CheckValidPersonPinOrFOBNUmber : 2")
                                    CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = "success"
                                    CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "success"
                                    CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                    CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = dtPersonForFobNumber.Rows(0)("PinNumber").ToString().Trim()
                                Else
                                    Dim dtVehicle As DataTable = New DataTable()
                                    dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & PersonFOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                    If (dtVehicle.Rows.Count > 0) Then
                                        log.Info("CheckValidPersonPinOrFOBNUmber :2_1")
                                        log.Debug("ProcessRequest: CheckValidPersonPinOrFOBNUmber - Fob/Card number is already registered with vehicle.You have presented the wrong Fob/Card, please use another FOB . IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & PersonFOBNumber.Replace(" ", "") & " Pin Number=" & PersonPIN.Trim())
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg35")
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                                    Else

                                        log.Info("CheckValidPersonPinOrFOBNUmber : 3")
                                        log.Debug("ProcessRequest: CheckValidPersonPinOrFOBNUmber - Fob/Card Number not assigned. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & PersonFOBNumber.Replace(" ", "") & " PersonPIN=" & PersonPIN.Trim())
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg40")
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                                        IsNewFob = "Yes"
                                    End If
                                End If
                            ElseIf (PersonPIN.Trim() <> "" And PersonFOBNumber <> "") Then
                                log.Info("CheckValidPersonPinOrFOBNUmber : 4")
                                Dim dtPersonForPIN As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And LTRIM(RTRIM(ANU.PinNumber)) ='" & PersonPIN.Trim() & "'", personId, RoleId)
                                If (dtPersonForPIN.Rows.Count = 0) Then
                                    log.Info("CheckValidPersonPinOrFOBNUmber : 5")
                                    'OBJMasterBAL.AssignedFOBNumberToPerson(PersonPIN, PersonFOBNumber)
                                    CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg41")
                                    CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                                    CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                    CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                                Else
                                    Dim dtPersonForFobNumberTemp As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','') ='" & PersonFOBNumber.Replace(" ", "") & "'", personId, RoleId)

                                    If (dtPersonForPIN.Rows(0)("FOBNumber").ToString() <> "" And dtPersonForPIN.Rows(0)("FOBNumber").ToString().ToLower().Replace(" ", "") <> PersonFOBNumber.ToLower().Replace(" ", "")) Then
                                        log.Info("CheckValidPersonPinOrFOBNUmber : 6")
                                        log.Debug("ProcessRequest: CheckValidPersonPinOrFOBNUmber - Pin Number is already registered with another Fob/Card Number. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & PersonFOBNumber.Replace(" ", "") & " Pin Number=" & PersonPIN.Trim())
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg42")
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                                    ElseIf (dtPersonForFobNumberTemp.Rows.Count = 0) Then
                                        'One FOB will not be acceptable for vehicle and personnel screens.
                                        Dim dtVehicle As DataTable = New DataTable()
                                        dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & PersonFOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                        If (dtVehicle.Rows.Count > 0) Then
                                            log.Info("CheckValidPersonPinOrFOBNUmber : 6_1")
                                            log.Debug("ProcessRequest: CheckValidPersonPinOrFOBNUmber - Fob/Card number is already registered with vehicle. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & PersonFOBNumber.Replace(" ", "") & " Pin Number=" & PersonPIN.Trim())
                                            CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg43")
                                            CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                                            CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                            CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                                        Else
                                            log.Info("CheckValidPersonPinOrFOBNUmber : 6_2")
                                            OBJMasterBAL.AssignedFOBNumberToPerson(PersonPIN.Trim(), PersonFOBNumber.Replace(" ", ""), customerId)
                                            Try
                                                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                    Dim writtenData As String = "PersonnelPIN = " & PersonPIN.Trim() & " ; " & "FOBNumber= " & PersonFOBNumber.Replace(" ", "") & ";" & "CustomerId=" & customerId
                                                    OBJServiceBAL = New WebServiceBAL()
                                                    CSCommonHelper.WriteLog("Added", "AssignedFOBNumberToPerson", "", writtenData, dsIMEI.Tables(0).Rows(0)("PersonName") & "(" & dsIMEI.Tables(0).Rows(0)("Email") & ")", IPAddress, "success", "")
                                                End If
                                            Catch ex As Exception
                                                log.Error("Error occured in AssignedFOBNumberToPerson. Exception is : " & ex.Message)
                                            End Try
                                            CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = "success"
                                            CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "success"
                                            CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                            CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                                        End If
                                        'One FOB will not be acceptable for vehicle and personnel screens.
                                    Else
                                        log.Info("CheckValidPersonPinOrFOBNUmber : 7")
                                        OBJMasterBAL.AssignedFOBNumberToPerson(PersonPIN.Trim(), PersonFOBNumber.Replace(" ", ""), customerId)
                                        Try
                                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                Dim writtenData As String = "PersonnelPIN = " & PersonPIN.Trim() & " ; " & "FOBNumber= " & PersonFOBNumber.Replace(" ", "") & ";" & "CustomerId=" & customerId
                                                OBJServiceBAL = New WebServiceBAL()
                                                CSCommonHelper.WriteLog("Added", "AssignedFOBNumberToPerson", "", writtenData, dsIMEI.Tables(0).Rows(0)("PersonName") & "(" & dsIMEI.Tables(0).Rows(0)("Email") & ")", IPAddress, "success", "")
                                            End If
                                        Catch ex As Exception
                                            log.Error("Error occured in AssignedFOBNumberToPerson. Exception is : " & ex.Message)
                                        End Try
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = "success"
                                        CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "success"
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                                        CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                                    End If
                                End If
                            End If
                        Else
                            log.Info("CheckValidPersonPinOrFOBNUmber : 8")
                            log.Debug("ProcessRequest: CheckValidPersonPinOrFOBNUmber - IMEI_UDID not found. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & PersonFOBNumber.Replace(" ", "") & " PersonPIN=" & PersonPIN.Trim())
                            CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg44")
                            CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                            CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                            CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                        End If
                    Else
                        log.Debug("ProcessRequest: CheckValidPersonPinOrFOBNUmber - IMEI_UDID not found. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & PersonFOBNumber.Replace(" ", "") & " PersonPIN=" & PersonPIN.Trim())
                        CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg44")
                        CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                        CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                        CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                    End If
                Else
                    log.Debug("ProcessRequest: CheckValidPersonPinOrFOBNUmber - IMEI_UDID not found. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & PersonFOBNumber.Replace(" ", "") & " PersonPIN=" & PersonPIN.Trim())
                    CheckValidPersonPinOrFOBNUmberResponse.ResponceMessage = resourceManager.GetString("HandlerMsg44")
                    CheckValidPersonPinOrFOBNUmberResponse.ResponceText = "fail"
                    CheckValidPersonPinOrFOBNUmberResponse.PersonFOBNumber = PersonFOBNumber.Replace(" ", "")
                    CheckValidPersonPinOrFOBNUmberResponse.PersonPIN = PersonPIN.Trim()
                End If

                CheckValidPersonPinOrFOBNUmberResponse.IsNewFob = IsNewFob
                Dim json As String

                json = javaScriptSerializer.Serialize(CheckValidPersonPinOrFOBNUmberResponse)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            log.Error("Exception occurred while prcessing request in CheckValidPersonPinOrFOBNUmber. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try

    End Sub

    Private Sub CheckVehicleFobOnly(context As HttpContext, IMEI As String)
        steps = "CheckVehicleFobOnly 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("CheckVehicleFobOnly :" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(AuthorizationSequenceModel))
            Dim IMEI_UDID = DirectCast(serJsonDetails, AuthorizationSequenceModel).IMEIUDID
            Dim VehicleNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).VehicleNumber
            Dim FOBNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).FOBNumber


            If (Not VehicleNumber Is Nothing) Then
                VehicleNumber = VehicleNumber.Trim()
            Else
                VehicleNumber = ""
            End If

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "CheckVehicleFobOnly 2"
            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        dtMain = dsIMEI.Tables(0)

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "CheckVehicleFobOnly 3"

                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            log.Debug("personId:" & personId)
                            Dim customerId As Integer
                            customerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            Dim RoleId As String
                            RoleId = dt.Rows(0)("RoleId").ToString()

                            steps = "CheckVehicleFobOnly 5"

                            Dim dtVehicle = New DataTable()
                            If (VehicleNumber.Trim() <> "") Then
                                dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And LTRIM(RTRIM(V.VehicleNumber)) ='" & IIf(VehicleNumber.Trim().ToLower().Contains("guest"), "guest", VehicleNumber.Trim()) & "'", personId, RoleId)
                            Else
                                dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & FOBNumber.ToString().Replace(" ", "") & "'", personId, RoleId)

                                If dtVehicle Is Nothing Then 'Fob number not exists
                                    log.Debug("ProcessRequest: CheckVehicleFobOnly - Fob/Card Number not assigned. IMEI_UDID=" & IMEI_UDID & " FOBNumber=" & FOBNumber.ToString().Replace(" ", ""))
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg36"), "invalidfob", "Yes")
                                    Return
                                ElseIf dtVehicle.Rows.Count = 0 Then ''Fob number not exists
                                    log.Debug("ProcessRequest: CheckVehicleFobOnly - Fob/Card Number not assigned. IMEI_UDID=" & IMEI_UDID & " FOBNumber=" & FOBNumber.ToString().Replace(" ", ""))
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg36"), "invalidfob", "Yes")
                                    Return
                                End If
                            End If

                            If Not dtVehicle Is Nothing Then 'Authorized vehicle?-Vehicle number not exists
                                log.Debug("CheckVehicleFobOnly: In dtVehicle")
                                If dtVehicle.Rows.Count <> 0 Then ''Authorized vehicle?-Vehicle number not exists

                                    Dim vehicleId As Integer
                                    vehicleId = Integer.Parse(dtVehicle.Rows(0)("VehicleId").ToString())
                                    steps = "CheckVehicleFobOnly 6 " & vehicleId

                                    If (FOBNumber <> "") Then
                                        If (dtVehicle.Rows(0)("FOBNumber").ToString().Replace(" ", "") <> "" And dtVehicle.Rows(0)("FOBNumber").ToString().ToLower().Replace(" ", "") <> FOBNumber.ToLower().Replace(" ", "")) Then
                                            log.Debug("ProcessRequest: CheckVehicleFobOnly - Vehicle number is already registered with another Fob/Card Number. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", "") & "Vehicle number=" & VehicleNumber.Trim())
                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg37"), "invalidfob")
                                            Return
                                        End If

                                        'One FOB will not be acceptable for vehicle and personnel screens.
                                        Dim dtVehicleTemp As DataTable = New DataTable()
                                        dtVehicleTemp = (OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & FOBNumber.Replace(" ", "") & "'", personId, RoleId))

                                        If (dtVehicleTemp.Rows.Count = 0) Then
                                            Dim dtPersonForFobNumber As DataTable = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','') ='" & FOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                            If (dtPersonForFobNumber.Rows.Count > 0) Then
                                                log.Debug("ProcessRequest: CheckVehicleFobOnly - Person is already registered with same Fob/Card Number. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", ""))
                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg38"), "invalidfob")
                                                Return
                                            End If
                                        End If
                                        'One Fob/Card will not be acceptable for vehicle and personnel screens.

                                    End If


                                    If (dtVehicle.Rows(0)("IsActive").ToString() = "False") Then
                                        log.Debug("ProcessRequest: CheckVehicleFobOnly- Vehicle is InActive. IMEI_UDID=" & IMEI_UDID)
                                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg16"), "Vehicle")
                                        Return
                                    End If

                                    Dim checkRequireOdoResponse = New CheckRequireOdoResponse()
                                    checkRequireOdoResponse.ResponceMessage = "success"
                                    checkRequireOdoResponse.ResponceText = resourceManager.GetString("HandlerMsg57")
                                    checkRequireOdoResponse.ValidationFailFor = ""
                                    checkRequireOdoResponse.FOBNumber = dtVehicle.Rows(0)("FOBNumber").ToString().Replace(" ", "")
                                    checkRequireOdoResponse.VehicleNumber = dtVehicle.Rows(0)("VehicleNumber").ToString().Trim()
                                    'check current version with launch version


                                    If (FOBNumber <> "") Then
                                        OBJMasterBAL.AssignedFOBNumberToVehicle(vehicleId, FOBNumber.Replace(" ", ""))
                                        Try
                                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                Dim writtenData As String = "VehicleId = " & vehicleId & " ; " & "FOBNumber= " & FOBNumber.Replace(" ", "") & ";"
                                                OBJServiceBAL = New WebServiceBAL()
                                                CSCommonHelper.WriteLog("Added", "AssignedFOBNumberToVehicle", "", writtenData, dsIMEI.Tables(0).Rows(0)("PersonName") & "(" & dsIMEI.Tables(0).Rows(0)("Email") & ")", IPAddress, "success", "")
                                            End If
                                        Catch ex As Exception
                                            log.Error("Error occured in AssignedFOBNumberToVehicle. Exception is : " & ex.Message)
                                        End Try
                                    End If

                                    Dim json As String
                                    json = javaScriptSerializer.Serialize(checkRequireOdoResponse)

                                    context.Response.Write(json)

                                Else
                                    log.Debug("ProcessRequest: CheckVehicleFobOnly- 2 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI_UDID)
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg45"), "Vehicle")
                                    ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized vehicle. Please contact administrator.", "Vehicle")
                                End If
                            Else
                                log.Debug("ProcessRequest: CheckVehicleFobOnly- 3 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI_UDID)
                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg25"), "Vehicle")
                                ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized vehicle, Please contact administrator.", "Vehicle")
                            End If

                        Else
                            log.Debug("ProcessRequest: CheckVehicleFobOnly- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: CheckVehicleFobOnly- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: CheckVehicleFobOnly- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: CheckVehicleFobOnly- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using
    End Sub

    Private Sub SaveTankMonitorReading(context As HttpContext, IMEI As String)
        steps = "SaveTankMonitorReading 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("SaveTankMonitorReading :" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(TankMonitorReading))
            Dim IMEI_UDID = DirectCast(serJsonDetails, TankMonitorReading).IMEI_UDID
            Dim ProbeReading = DirectCast(serJsonDetails, TankMonitorReading).ProbeReading
            Dim ReadingDateTime = DirectCast(serJsonDetails, TankMonitorReading).ReadingDateTime
            Dim TLD = DirectCast(serJsonDetails, TankMonitorReading).TLD
            Dim FromSiteId = DirectCast(serJsonDetails, TankMonitorReading).FromSiteId
            Dim LSB = DirectCast(serJsonDetails, TankMonitorReading).LSB
            Dim MSB = DirectCast(serJsonDetails, TankMonitorReading).MSB
            Dim TLDTemperature = DirectCast(serJsonDetails, TankMonitorReading).TLDTemperature
            Dim Response_code = DirectCast(serJsonDetails, TankMonitorReading).Response_code

            If (Not (LSB Is Nothing) And Not (MSB Is Nothing) And Not (LSB = "") And Not (MSB = "")) Then
                ProbeReading = CalculateProbeReading(LSB, MSB, Response_code)
            End If

            log.Info("ProbeReading==>" & ProbeReading)

            If (ProbeReading Is Nothing Or ProbeReading = "") Then
                Dim tankMonitorReadingResponse = New TankMonitorReadingResponse()
                tankMonitorReadingResponse.ResponceMessage = "success"
                tankMonitorReadingResponse.ResponceText = "Probe Reading not found."
                log.Debug("ProcessRequest: SaveTankMonitorReading- Probe Reading not found. ProbeReading = " & ProbeReading & " IMEI_UDID=" & IMEI_UDID)
                Dim json As String
                json = javaScriptSerializer.Serialize(tankMonitorReadingResponse)

                context.Response.Write(json)
                Return
            End If

            If (ProbeReading <= 0) Then
                Dim tankMonitorReadingResponse = New TankMonitorReadingResponse()
                tankMonitorReadingResponse.ResponceMessage = "success"
                tankMonitorReadingResponse.ResponceText = "Probe Reading is 0, so ignored the level"
                log.Debug("ProcessRequest: SaveTankMonitorReading- Probe Reading is 0, so ignored the level. ProbeReading = " & ProbeReading & " IMEI_UDID=" & IMEI_UDID)
                Dim json As String
                json = javaScriptSerializer.Serialize(tankMonitorReadingResponse)

                context.Response.Write(json)
                Return
            End If

            Dim calculatedTempareture As Decimal = 0
            If (TLDTemperature Is Nothing) Then
                calculatedTempareture = "20"
            Else
                calculatedTempareture = (TLDTemperature * 0.48876) - 50
            End If

            If (TLD Is Nothing Or TLD = "") Then
                Dim tankMonitorReadingResponse = New TankMonitorReadingResponse()
                tankMonitorReadingResponse.ResponceMessage = "success"
                tankMonitorReadingResponse.ResponceText = "TLD not found."
                log.Debug("ProcessRequest: SaveTankMonitorReading- TLD not found. TLD = " & TLD & " IMEI_UDID=" & IMEI_UDID)
                Dim json As String
                json = javaScriptSerializer.Serialize(tankMonitorReadingResponse)

                context.Response.Write(json)
                Return
            End If

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "SaveTankMonitorReading 2"

            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        dtMain = dsIMEI.Tables(0)

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "SaveTankMonitorReading 3"

                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())

                            Dim PersonName As String
                            PersonName = dt.Rows(0)("PersonName").ToString()

                            Dim Email As String
                            Email = dt.Rows(0)("Email").ToString()

                            steps = "SaveTankMonitorReading 5"
                            OBJMasterBAL = New MasterBAL()
                            'OBJMasterBAL.InsertTankMonitorDetail(ReadingDateTime, ProbeReading, FromSiteId, personId, TLD)
                            Dim result = OBJMasterBAL.SaveUpdateTankInventory(0, "", "Level", DateTime.Now, 0.0, "s", 0, personId, DateTime.Now, "",
                                                                              ReadingDateTime, ProbeReading, FromSiteId, TLD, "TLD", 0, calculatedTempareture, Response_code)
                            steps = "SaveTankMonitorReading 6"

                            Try
                                Dim CreateDataFor = "" & ";" & "Level" & ";" & DateTime.Now.ToString() & ";" & 0.0 & ";" & "s" & ";" & 0 & ";" & personId & ";" &
                            DateTime.Now.ToString() & ";" & "" & ";" & ReadingDateTime & ";" & ProbeReading & ";" & FromSiteId & ";" & TLD & ";" & "TLD"

                                If result <> 0 Then
                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveTankMonitorReading(TLD)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "success", "")
                                    End If
                                Else
                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveTankMonitorReading(TLD)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "fail", "Tank inventory reading saving failed.")
                                    End If
                                End If
                            Catch ex As Exception
                                log.Debug("Error in SaveTankMonitorReading (CreateDataSaveTankMonitorReading). Error is " + ex.Message)
                            End Try




                            Dim tankMonitorReadingResponse = New TankMonitorReadingResponse()
                            tankMonitorReadingResponse.ResponceMessage = "success"
                            tankMonitorReadingResponse.ResponceText = "Tank monitor reading saved."

                            Dim json As String
                            json = javaScriptSerializer.Serialize(tankMonitorReadingResponse)

                            context.Response.Write(json)

                        Else
                            log.Debug("ProcessRequest: SaveTankMonitorReading- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: SaveTankMonitorReading- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: SaveTankMonitorReading- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: SaveTankMonitorReading- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using
    End Sub

    Private Function CalculateProbeReading(LSB As String, MSB As String, Response_code As String) As Decimal
        Dim lsb_hex As String = Hex(LSB)
        Dim msb_hex As String = Hex(MSB)
        Dim Combine_hex As String = msb_hex & lsb_hex
        Dim tempProbeReading = Convert.ToInt64(Combine_hex, 16) 'CInt("&H" & Combine_hex)
        Dim calculatedProbeReading As Decimal = 0
        If (Response_code = "159") Then
            calculatedProbeReading = tempProbeReading * 0.0393700787 'convert mm to inch
        Else
            calculatedProbeReading = tempProbeReading / 128
        End If

        Return Math.Round(calculatedProbeReading, 1)

    End Function

    Private Sub SaveInventoryVeederTankMonitorReading(context As HttpContext, IMEI As String)
        steps = "SaveInventoryVeederTankMonitorReading 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("SaveInventoryVeederTankMonitorReading :" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(VR_Inventory))
            Dim IMEI_UDID = DirectCast(serJsonDetails, VR_Inventory).IMEI_UDID
            Dim VeederRootMacAddress = DirectCast(serJsonDetails, VR_Inventory).VeederRootMacAddress
            Dim TankNumber = DirectCast(serJsonDetails, VR_Inventory).TankNumber
            Dim AppDateTime = DirectCast(serJsonDetails, VR_Inventory).AppDateTime
            Dim VRDateTime = DirectCast(serJsonDetails, VR_Inventory).VRDateTime
            Dim ProductCode = DirectCast(serJsonDetails, VR_Inventory).ProductCode
            Dim TankStatus = DirectCast(serJsonDetails, VR_Inventory).TankStatus
            Dim Volume = DirectCast(serJsonDetails, VR_Inventory).Volume
            Dim TCVolume = DirectCast(serJsonDetails, VR_Inventory).TCVolume
            Dim Ullage = DirectCast(serJsonDetails, VR_Inventory).Ullage
            Dim Height = DirectCast(serJsonDetails, VR_Inventory).Height
            Dim Water = DirectCast(serJsonDetails, VR_Inventory).Water
            Dim Temperature = DirectCast(serJsonDetails, VR_Inventory).Temperature
            Dim WaterVolume = DirectCast(serJsonDetails, VR_Inventory).WaterVolume

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "SaveInventoryVeederTankMonitorReading 2"

            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        dtMain = dsIMEI.Tables(0)

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "SaveInventoryVeederTankMonitorReading 3"

                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            Dim CustomerId As Integer
                            Dim PhoneNumber As String
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            CustomerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            PhoneNumber = dt.Rows(0)("PhoneNumber").ToString()

                            Dim PersonName As String
                            PersonName = dt.Rows(0)("PersonName").ToString()

                            Dim Email As String
                            Email = dt.Rows(0)("Email").ToString()

                            steps = "SaveInventoryVeederTankMonitorReading 5"
                            OBJMasterBAL = New MasterBAL()
                            steps = "SaveInventoryVeederTankMonitorReading 5_1"
                            Dim AppDateTime1 As DateTime = DateTime.Parse(AppDateTime)
                            steps = "SaveInventoryVeederTankMonitorReading 5_2"
                            Dim VRDateTime1 As DateTime = DateTime.Parse(VRDateTime)
                            steps = "SaveInventoryVeederTankMonitorReading 5_3"
                            log.Debug("VRDateTime1==>" & VRDateTime1)
                            log.Debug("AppDateTime1==>" & AppDateTime1)
                            Dim VRResponse = New CheckVRResponse()
                            Dim result As Integer = OBJMasterBAL.InsertInventoryVeederTankMonitorDetail(PhoneNumber, personId, VeederRootMacAddress, AppDateTime1, TankNumber, VRDateTime1, ProductCode, TankStatus, Volume, TCVolume, Ullage, Height, Water,
                                                                                    Temperature, WaterVolume, personId, CustomerId)

                            Try
                                Dim CreateDataFor = result & ";" & PhoneNumber & ";" & personId & ";" & VeederRootMacAddress & ";" & AppDateTime1 & ";" & TankNumber & ";" & VRDateTime1 & ";" & ProductCode & ";" & TankStatus & ";" & Volume & ";" & TCVolume & ";" &
                                Ullage & ";" & Height & ";" & Water & ";" & Temperature & ";" & WaterVolume & ";" & personId & ";" & CustomerId

                                If (result > 0) Then
                                    steps = "SaveInventoryVeederTankMonitorReading 6"

                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveInventoryVeederTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveInventoryVeederTankMonitorReading(InventoryVeeder)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "success", "")
                                    End If

                                    VRResponse.ResponceMessage = "success"
                                    VRResponse.ResponceText = "VR Tank monitor reading saved."
                                ElseIf (result = -1) Then
                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveInventoryVeederTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveInventoryVeederTankMonitorReading(InventoryVeeder)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "fail", "Tank not found whose tank number is " & TankNumber)
                                    End If

                                    VRResponse.ResponceMessage = "fail"
                                    VRResponse.ResponceText = "Tank Not found whose tank number is " & TankNumber
                                Else
                                    steps = "SaveInventoryVeederTankMonitorReading 7"

                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveInventoryVeederTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveInventoryVeederTankMonitorReading(InventoryVeeder)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "fail", "")
                                    End If

                                    VRResponse.ResponceMessage = "fail"
                                    VRResponse.ResponceText = "VR Tank monitor reading saving failed."

                                End If
                            Catch ex As Exception
                                log.Debug("Error in SaveInventoryVeederTankMonitorReading (CreateDataSaveInventoryVeederTankMonitorReading). Error Is " + ex.Message)
                            End Try
                            Dim json As String
                            json = javaScriptSerializer.Serialize(VRResponse)

                            context.Response.Write(json)

                        Else
                            log.Debug("ProcessRequest: SaveInventoryVeederTankMonitorReading- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: SaveInventoryVeederTankMonitorReading- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: SaveInventoryVeederTankMonitorReading- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: SaveInventoryVeederTankMonitorReading- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using
    End Sub

    Private Sub SaveDeliveryVeederTankMonitorReading(context As HttpContext, IMEI As String)
        steps = "SaveDeliveryVeederTankMonitorReading 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("SaveDeliveryVeederTankMonitorReading :" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(VR_Delivery))
            Dim IMEI_UDID = DirectCast(serJsonDetails, VR_Delivery).IMEI_UDID
            Dim VeederRootMacAddress = DirectCast(serJsonDetails, VR_Delivery).VeederRootMacAddress
            Dim TankNumber = DirectCast(serJsonDetails, VR_Delivery).TankNumber
            Dim AppDateTime = DirectCast(serJsonDetails, VR_Delivery).AppDateTime
            Dim VRDateTime = DirectCast(serJsonDetails, VR_Delivery).VRDateTime
            Dim ProductCode = DirectCast(serJsonDetails, VR_Delivery).ProductCode
            Dim StartDateTime = DirectCast(serJsonDetails, VR_Delivery).StartDateTime
            Dim EndDateTime = DirectCast(serJsonDetails, VR_Delivery).EndDateTime
            Dim StartVolume = DirectCast(serJsonDetails, VR_Delivery).StartVolume
            Dim StartTCVolume = DirectCast(serJsonDetails, VR_Delivery).StartTCVolume
            Dim StartWater = DirectCast(serJsonDetails, VR_Delivery).StartWater
            Dim StartTemp = DirectCast(serJsonDetails, VR_Delivery).StartTemp
            Dim EndVolume = DirectCast(serJsonDetails, VR_Delivery).EndVolume
            Dim EndTCVolume = DirectCast(serJsonDetails, VR_Delivery).EndTCVolume
            Dim EndWater = DirectCast(serJsonDetails, VR_Delivery).EndWater
            Dim EndTemp = DirectCast(serJsonDetails, VR_Delivery).EndTemp
            Dim StartHeight = DirectCast(serJsonDetails, VR_Delivery).StartHeight
            Dim EndHeight = DirectCast(serJsonDetails, VR_Delivery).EndHeight

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "SaveDeliveryVeederTankMonitorReading 2"

            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        dtMain = dsIMEI.Tables(0)

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "SaveDeliveryVeederTankMonitorReading 3"

                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            Dim CustomerId As Integer
                            Dim PhoneNumber As String
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            CustomerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            PhoneNumber = dt.Rows(0)("PhoneNumber").ToString()

                            Dim PersonName As String
                            PersonName = dt.Rows(0)("PersonName").ToString()

                            Dim Email As String
                            Email = dt.Rows(0)("Email").ToString()

                            steps = "SaveDeliveryVeederTankMonitorReading 5"
                            OBJMasterBAL = New MasterBAL()

                            Dim result As Integer = OBJMasterBAL.InsertDeliveryVeederTankMonitorDetail(PhoneNumber, personId, VeederRootMacAddress, AppDateTime, TankNumber, VRDateTime, ProductCode, StartDateTime, EndDateTime, StartVolume,
                                                                               StartTCVolume, StartWater, StartTemp, EndVolume, EndTCVolume, EndWater, EndTemp, StartHeight, EndHeight, personId, CustomerId)
                            Dim VRResponse = New CheckVRResponse()

                            Try
                                Dim CreateDataFor = result & ";" & PhoneNumber & ";" & personId & ";" & VeederRootMacAddress & ";" & AppDateTime & ";" & TankNumber & ";" & VRDateTime & ";" & ProductCode & ";" & StartDateTime & ";" & EndDateTime & ";" & StartVolume & ";" &
                                StartTCVolume & ";" & StartWater & ";" & StartTemp & ";" & EndVolume & ";" & EndTCVolume & ";" & EndWater & ";" & EndTemp & ";" & StartHeight & ";" & EndHeight & ";" & personId & ";" & CustomerId

                                If (result > 0) Then
                                    steps = "SaveDeliveryVeederTankMonitorReading 6"

                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveDeliveryVeederTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveDeliveryVeederTankMonitorReading(InventoryVeeder)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "success", "")
                                    End If

                                    VRResponse.ResponceMessage = "success"
                                    VRResponse.ResponceText = "VR Tank monitor reading saved."
                                ElseIf (result = -1) Then

                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveDeliveryVeederTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveDeliveryVeederTankMonitorReading(InventoryVeeder)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "fail", "Tank Not found whose tank number Is " & TankNumber)
                                    End If

                                    VRResponse.ResponceMessage = "fail"
                                    VRResponse.ResponceText = "Tank Not found whose tank number is " & TankNumber

                                Else
                                    steps = "SaveDeliveryVeederTankMonitorReading 7"

                                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                        Dim writtenData As String = CreateDataSaveDeliveryVeederTankMonitorReading(result, CreateDataFor)
                                        CSCommonHelper.WriteLog("Added", "SaveDeliveryVeederTankMonitorReading(InventoryVeeder)", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "fail", "")
                                    End If

                                    VRResponse.ResponceMessage = "fail"
                                    VRResponse.ResponceText = "VR Tank monitor reading saving failed."

                                End If
                            Catch ex As Exception
                                log.Debug("Error in SaveDeliveryVeederTankMonitorReading (CreateDataSaveDeliveryVeederTankMonitorReading). Error is " + ex.Message)
                            End Try

                            Dim json As String
                            json = javaScriptSerializer.Serialize(VRResponse)

                            context.Response.Write(json)

                        Else
                            log.Debug("ProcessRequest: SaveDeliveryVeederTankMonitorReading- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: SaveDeliveryVeederTankMonitorReading- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: SaveDeliveryVeederTankMonitorReading- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: SaveDeliveryVeederTankMonitorReading- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using
    End Sub

    Private Sub VINAuthorization(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Try

            Dim data = [String].Empty
            Dim VehicleId = 0
            Dim personId As Integer = 0
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                Dim check = context.Request("TransactionDate")


                log.Info("VINAuthorization : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(FAVehicleAuthorizationMaster))
                Dim VehicleRecurringMSG = DirectCast(serJsonDetails, FAVehicleAuthorizationMaster).VehicleRecurringMSG
                Dim TransactionDate = DirectCast(serJsonDetails, FAVehicleAuthorizationMaster).TransactionDate
                Dim TransactionFrom = DirectCast(serJsonDetails, FAVehicleAuthorizationMaster).TransactionFrom
                Dim CurrentLat = DirectCast(serJsonDetails, FAVehicleAuthorizationMaster).CurrentLat
                Dim CurrentLng = DirectCast(serJsonDetails, FAVehicleAuthorizationMaster).CurrentLng
                Dim FSTagMacAddress = DirectCast(serJsonDetails, FAVehicleAuthorizationMaster).FSTagMacAddress
                Dim CurrentFSVMFirmwareVersion = DirectCast(serJsonDetails, FAVehicleAuthorizationMaster).CurrentFSVMFirmwareVersion
                'Dim VehicleRecurringMSG = context.Request("VehicleRecurringMSG")
                'Dim TransactionDate = context.Request("TransactionDate")
                'Dim TransactionFrom = context.Request("TransactionFrom")
                'Dim CurrentLat = context.Request("CurrentLat")
                'Dim CurrentLng = context.Request("CurrentLng")

                If CurrentFSVMFirmwareVersion Is Nothing Then
                    CurrentFSVMFirmwareVersion = ""
                End If

                Dim FAVehicleAuthorizationMasterResponseObj = New FAVehicleAuthorizationMasterResponse()
                Dim json As String

                log.Debug("VehicleRecurringMSG : " & VehicleRecurringMSG)

                Dim customerId As Integer = 0
                Dim RoleId As String = ""
                Dim Phonenumber As String = ""
                Dim PinNumber As String = ""
                OBJMasterBAL = New MasterBAL()
                Dim dsIMEI = New DataSet()

                dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                    If Not dsIMEI.Tables(0) Is Nothing Then
                        If dsIMEI.Tables(0).Rows.Count <> 0 Then
                            personId = Integer.Parse(dsIMEI.Tables(0).Rows(0)("PersonId").ToString())
                            customerId = Integer.Parse(dsIMEI.Tables(0).Rows(0)("CustomerId").ToString())
                            RoleId = dsIMEI.Tables(0).Rows(0)("RoleId")
                            Phonenumber = dsIMEI.Tables(0).Rows(0)("PhoneNumber").ToString()
                            PinNumber = dsIMEI.Tables(0).Rows(0)("PinNumber").ToString().Trim()
                        End If
                    End If
                End If
                Dim VIN As String = ""
                Dim Odometer As Integer = 0
                Dim RPM As String = ""
                Dim SPD As String = 0
                Dim MIL As String = 0
                Dim PC As String = 0

                Dim PID1 As String = ""
                Dim PID2 As String = ""
                Dim PID3 As String = ""
                Dim PID4 As String = ""
                Dim PID5 As String = ""
                Dim PID6 As String = ""
                Dim PID7 As String = ""
                Dim PID8 As String = ""
                Dim PID9 As String = ""
                Dim PID10 As String = ""

                Dim PID11 As String = ""
                Dim PID12 As String = ""
                Dim PID13 As String = ""
                Dim PID14 As String = ""
                Dim PID15 As String = ""
                Dim PID16 As String = ""
                Dim PID17 As String = ""
                Dim PID18 As String = ""
                Dim PID19 As String = ""
                Dim PID20 As String = ""

                log.Debug("VehicleRecurringMSG 0")
                OBJMasterBAL = New MasterBAL()
                If (VehicleRecurringMSG <> "") Then
                    Try
                        log.Debug("VehicleRecurringMSG 1.1")
                        Dim OBJWebServiceBAL = New WebServiceBAL()

                        VehicleRecurringMSG = VehicleRecurringMSG.Replace("{", "").Replace("}", "")

                        Dim strSplitComma As String() = VehicleRecurringMSG.Split(",")
                        Try
                            For index = 0 To strSplitComma.Length - 1
                                Dim strSplitEqual As String() = strSplitComma(index).Split("=")
                                Select Case strSplitEqual(0).ToLower()
                                    Case "vin"
                                        VIN = strSplitEqual(1).Trim()
                                    Case "rpm"
                                        RPM = Convert.ToInt32(strSplitEqual(1), 16)
                                    Case "spd"
                                        SPD = Convert.ToInt32(strSplitEqual(1), 16)
                                    Case "odok"
                                        Odometer = Convert.ToInt32(strSplitEqual(1), 16)
                                    Case "mil"
                                        MIL = strSplitEqual(1)
                                    Case "pc"
                                        PC = strSplitEqual(1) ' check this in string where in hex or not
                                    Case Else
                                        Console.WriteLine("You typed something else")
                                End Select
                            Next
                        Catch ex As Exception

                        End Try


                        log.Debug("VehicleRecurringMSG 1.2")


                        log.Debug("VehicleRecurringMSG 2")
                        'For index = 6 To (Convert.ToInt32(PC) + 5)
                        '    Dim strSplitEqual As String() = strSplitComma(index).Split("=")
                        '    Select Case Convert.ToInt32(strSplitEqual(0), 16)
                        '        Case 1
                        '            PID1 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 2
                        '            PID2 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 3
                        '            PID3 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 4
                        '            PID4 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 5
                        '            PID5 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 6
                        '            PID6 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 7
                        '            PID7 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 8
                        '            PID8 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 9
                        '            PID9 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 10
                        '            PID10 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 11
                        '            PID11 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 12
                        '            PID12 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 13
                        '            PID13 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 14
                        '            PID14 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 15
                        '            PID15 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 16
                        '            PID16 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 17
                        '            PID17 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 18
                        '            PID18 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 19
                        '            PID19 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case 20
                        '            PID20 = Convert.ToString(Convert.ToInt32(strSplitEqual(1), 16))
                        '        Case Else
                        '            Console.WriteLine("You typed something else")
                        '    End Select
                        'Next
                        log.Debug("VehicleRecurringMSG 3_1")


                        Dim VehicleName = ""
                        Dim VehicleNumber = ""
                        Dim DiffRawOdoAndManualOdo As Integer = 0
                        Dim dsVehicleValuesData As DataTable
                        dsVehicleValuesData = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And LTRIM(RTRIM(V.VIN)) ='" & VIN.Trim() & "'", personId, RoleId)
                        If dsVehicleValuesData IsNot Nothing And dsVehicleValuesData.Rows.Count > 0 Then



                            VehicleId = dsVehicleValuesData.Rows(0)("VehicleId")
                            VehicleName = dsVehicleValuesData.Rows(0)("VehicleName")
                            VehicleNumber = dsVehicleValuesData.Rows(0)("VehicleNumber")
                            log.Debug("DiffRawOdoAndManualOdo - " & dsVehicleValuesData.Rows(0)("DiffRawOdoAndManualOdo"))
                            If dsVehicleValuesData.Rows(0)("DiffRawOdoAndManualOdo").ToString() <> "" Then
                                DiffRawOdoAndManualOdo = Convert.ToInt32(dsVehicleValuesData.Rows(0)("DiffRawOdoAndManualOdo").ToString())
                            End If

                            ' Check and update FSTagMacAddress to vehicle
                            OBJServiceBAL.UpdateFSTagMacAddressToVehicle(FSTagMacAddress, personId, VehicleId)

                            Dim dsTransactionValuesData As DataSet
                            Dim DepartmentName As String = ""
                            Dim FuelTypeName As String = ""
                            Dim Email As String = ""
                            Dim PersonName As String = ""
                            Dim CompanyName As String = ""
                            Dim VehicleSum As Decimal = 0
                            Dim DeptSum As Decimal = 0
                            Dim VehPercentage As Decimal = 0
                            Dim DeptPercentage As Decimal = 0
                            Dim SurchargeType As String = "0"
                            Dim ProductPrice As Decimal = 0
                            Dim DepartmentNumber As String = ""

                            dsTransactionValuesData = OBJMasterBAL.GetTransactionColumnsValueForSave("", 0, personId, VehicleId)

                            If dsTransactionValuesData IsNot Nothing Then
                                If dsTransactionValuesData.Tables.Count > 0 Then
                                    If dsTransactionValuesData.Tables(0) IsNot Nothing And dsTransactionValuesData.Tables(0).Rows.Count > 0 Then
                                        DepartmentName = dsTransactionValuesData.Tables(0).Rows(0)("DeptName").ToString()
                                        DepartmentNumber = dsTransactionValuesData.Tables(0).Rows(0)("DeptNumber").ToString()
                                        VehicleSum = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("VehicleSum").ToString())
                                        DeptSum = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("DeptSum").ToString())
                                        VehPercentage = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("VehPercentage").ToString())
                                        DeptPercentage = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("DeptPercentage").ToString())
                                        SurchargeType = dsTransactionValuesData.Tables(0).Rows(0)("SurchargeType").ToString()
                                    End If
                                    If dsTransactionValuesData.Tables(1) IsNot Nothing And dsTransactionValuesData.Tables(1).Rows.Count > 0 Then
                                        FuelTypeName = dsTransactionValuesData.Tables(1).Rows(0)("FuelTypeName").ToString()
                                        ProductPrice = Decimal.Parse(dsTransactionValuesData.Tables(1).Rows(0)("ProductPrice").ToString())
                                    End If
                                    If dsTransactionValuesData.Tables(2) IsNot Nothing And dsTransactionValuesData.Tables(2).Rows.Count > 0 Then
                                        Email = dsTransactionValuesData.Tables(2).Rows(0)("Email").ToString()
                                        PersonName = dsTransactionValuesData.Tables(2).Rows(0)("PersonName").ToString()
                                    End If
                                    If dsTransactionValuesData.Tables(3) IsNot Nothing And dsTransactionValuesData.Tables(3).Rows.Count > 0 Then
                                        CompanyName = dsTransactionValuesData.Tables(3).Rows(0)("CompanyName").ToString()
                                    End If
                                End If
                            End If
                            steps = "VehicleRecurringMSG 3 - 2"

                            'OBJMasterBAL = New MasterBAL()
                            'Dim dtTransaction As DataTable = New DataTable()
                            'dtTransaction = OBJMasterBAL.GetTransactionsByCondition(" and T.VehicleId=" & VehicleId & " and (((ISNULL(T.TransactionStatus,0) = 1  or ISNULL(T.IsMissed,0)= 1) or ISNULL(T.TransactionStatus,0) = 0) and datediff(minute, T.[CreatedDate] ,getdate()) <= 15)", personId, RoleId, False)
                            'If (Not dtTransaction Is Nothing) Then

                            '    If (dtTransaction.Rows.Count > 0) Then
                            '        FAVehicleAuthorizationMasterResponseObj.ResponceMessage = "fail"
                            '        FAVehicleAuthorizationMasterResponseObj.ResponceText = "Transaction already exist."
                            '        log.Error("Transaction already exist.VIN:" & VIN.Trim() & " TransactionId:" & dtTransaction.Rows(0)("TransactionId"))
                            '        json = javaScriptSerializer.Serialize(FAVehicleAuthorizationMasterResponseObj)
                            '        context.Response.Write(json)
                            '        Return
                            '    End If

                            'End If

                            Dim TransactionId As Integer = 0
                            Dim CurrentOdo As Integer = 0
                            OBJMasterBAL = New MasterBAL()
                            Dim insertFlag As Boolean = False

                            Try
                                Dim dtTransactionVehicleInfo As DataTable = New DataTable()
                                Dim dsT As DataSet = New DataSet()
                                dsT = OBJMasterBAL.GetTransactionsByCondition(" and T.CompanyId = " & customerId & " and (((ISNULL(T.TransactionStatus,0) = 1  or ISNULL(T.IsMissed,0)= 1) or ISNULL(T.TransactionStatus,0) = 0) and datediff(minute, T.[CreatedDate] ,getdate()) <= 15) and LTRIM(RTRIM(T.VehicleNumber)) = '" & VehicleNumber.Trim() & "'", personId, RoleId, 0, 0, 0, False, "", "")
                                dtTransactionVehicleInfo = dsT.Tables(0)

                                If dtTransactionVehicleInfo IsNot Nothing Then
                                    If dtTransactionVehicleInfo.Rows.Count = 0 Then
                                        insertFlag = True
                                        log.Debug("In Insert")
                                    Else
                                        TransactionId = Convert.ToInt32(dtTransactionVehicleInfo.Rows(0)("TransactionId").ToString())
                                        CurrentOdo = Convert.ToInt32(dtTransactionVehicleInfo.Rows(0)("CurrentOdometer").ToString())
                                        insertFlag = False
                                        log.Debug("In Update - TransactionId - " & TransactionId & " CurrentOdo - " & CurrentOdo)
                                    End If
                                End If
                            Catch ex As Exception
                                log.Debug("Error VIN + steps - " & steps & ". Error is " + ex.Message)
                            End Try

                            steps = "VehicleRecurringMSG 3 - 2 -0"

                            Dim resultUpgradable As String = ""
                            resultUpgradable = IsUpgradeCurrentVersionWithUgradableVersionFSVM(dsVehicleValuesData.Rows(0)("VehicleId"), personId, CurrentFSVMFirmwareVersion)

                            If resultUpgradable = "" Then
                                resultUpgradable = "N"
                            End If

                            log.Debug("resultUpgradable: " & resultUpgradable & " VehicleId: " & dsVehicleValuesData.Rows(0)("VehicleId") & " personId: " & personId)

                            If resultUpgradable.ToUpper() = "Y" Then
                                FAVehicleAuthorizationMasterResponseObj = GetLaunchedFSVMFirmwareDetails(FAVehicleAuthorizationMasterResponseObj)
                            End If

                            FAVehicleAuthorizationMasterResponseObj.IsFSVMUpgradable = resultUpgradable

                            ' 1 km = 0.621371 miles

                            Dim KilometerTOMiles = Convert.ToDecimal(ConfigurationManager.AppSettings("KilometerTOMiles").ToString())
                            If insertFlag Then
                                TransactionId = OBJMasterBAL.InsertUpdateTransaction(VehicleId, 0, personId, (Odometer * KilometerTOMiles) - DiffRawOdoAndManualOdo, 0, 0, Phonenumber, "", TransactionDate,
                                                                0, 0, TransactionFrom, 0, CurrentLat, CurrentLng, "",
                                                              VehicleNumber.Trim(), DepartmentNumber, PinNumber.Trim(), "", -1, True, False, 0, personId, 0,
                                                                VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, 0, customerId, 0, Odometer, 1)
                                log.Debug("In Insert Transaction ODOK - " & Odometer)
                            Else
                                steps = "VehicleRecurringMSG 3 - 2 -1"
                                log.Debug("In CurrentOdo- " & CurrentOdo)
                                If CurrentOdo < ((Odometer * KilometerTOMiles) - DiffRawOdoAndManualOdo) And ((Odometer * KilometerTOMiles) - DiffRawOdoAndManualOdo) <> 0 Then
                                    TransactionId = OBJMasterBAL.InsertUpdateTransaction(VehicleId, 0, personId, (Odometer * KilometerTOMiles) - DiffRawOdoAndManualOdo, 0, 0, Phonenumber, "", TransactionDate,
                                                                TransactionId, 0, TransactionFrom, 0, CurrentLat, CurrentLng, "",
                                                              VehicleNumber.Trim(), DepartmentNumber, PinNumber.Trim(), "", -1, True, False, 0, personId, 0,
                                                                VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, 0, customerId, 0, Odometer, 1)

                                    log.Debug("In Update Transaction Calculated Odometer - " & (Odometer * KilometerTOMiles) + DiffRawOdoAndManualOdo)
                                    log.Debug("In Update Transaction ODOK - " & Odometer)
                                    log.Debug("In CurrentOdo- " & CurrentOdo)
                                End If

                                FAVehicleAuthorizationMasterResponseObj.ResponceMessage = "fail"
                                FAVehicleAuthorizationMasterResponseObj.ResponceText = "Transaction already exist."
                                log.Error("Transaction already exist.VIN:" & VIN.Trim() & " TransactionId:" & TransactionId)
                                json = javaScriptSerializer.Serialize(FAVehicleAuthorizationMasterResponseObj)
                                context.Response.Write(json)
                                Return

                            End If


                            steps = "VehicleRecurringMSG 3 - 3"
                            Dim CreateDataFor As String = ""
                            CreateDataFor = VIN.Trim() & ";" & VehicleName & ";" & DepartmentName & ";" & DepartmentNumber & ";" & VIN.Trim() & ";" & "" & ";" & "0" & ";" &
                                        "" & ";" & CompanyName & ";" & "" & ";" & Convert.ToDateTime(TransactionDate).ToString("MM/dd/yyyy") & ";" & Convert.ToDateTime(TransactionDate).ToString("hh: mm:tt") & ";" &
                                        PersonName & ";" & PinNumber.Trim() & ";" & Odometer & ";" & "" & ";" & -1 & ";" & "" & ";" & "Started" & ";" & "0 Raw Odometer = " & Odometer

                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                Dim writtenData As String = CreateData(TransactionId, CreateDataFor)
                                CSCommonHelper.WriteLog("Added", "Transactions", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "success", "")
                            End If

                            log.Error("TransactionId: " & TransactionId)

                            steps = "VehicleRecurringMSG  4"
                            Dim VINAuthorizationID As Integer = 0
                            VINAuthorizationID = OBJMasterBAL.InsertUpdateFSVM(VINAuthorizationID, TransactionId, customerId, RPM, SPD, MIL, Odometer, PC, PID1, PID2, PID3, PID4, PID5, PID6, PID7, PID8,
                                                                           PID9, PID10, PID11, PID12, PID13, PID14, PID15, PID16, PID17, PID18, PID19, PID20)
                            log.Debug("In VINAuthorizationID ODOK - " & Odometer)

                            Try
                                CreateDataFor = TransactionId & ";" & customerId & ";" & RPM & ";" & SPD & ";" & MIL & ";" & PC & ";" &
                                       PID1 & ";" & PID2 & ";" & PID3 & ";" & PID4 & ";" & PID5 & ";" & PID6 & ";" & PID7 & ";" & PID8 & ";" & PID9 & ";" & PID10 & ";" &
                                       PID11 & ";" & PID12 & ";" & PID13 & ";" & PID14 & ";" & PID15 & ";" & PID16 & ";" & PID17 & ";" & PID18 & ";" & PID19 & ";" & PID20

                                If (ConfigurationManager.AppSettings("VINAuthorizationID").ToString().ToLower() = "yes") Then
                                    Dim writtenData As String = CreateDataForVINAuthorization(VINAuthorizationID, CreateDataFor, Odometer)
                                    CSCommonHelper.WriteLog("Added", "VINAuthorization", "", writtenData, PersonName & "(" & Email & ")", IPAddress, "success", "")
                                End If
                            Catch ex As Exception
                                log.Debug("Error in CreateDataForVINAuthorization (CreateDataForVINAuthorization) + steps - " & steps & ". Error is " + ex.Message)
                            End Try


                            steps = "VehicleRecurringMSG 5"



                            FAVehicleAuthorizationMasterResponseObj.VehicleId = VehicleId
                            FAVehicleAuthorizationMasterResponseObj.ResponceMessage = "success"
                            FAVehicleAuthorizationMasterResponseObj.ResponceText = Convert.ToString(TransactionId)

                            'Dim PID21 As String = ""
                            'Dim PID22 As String = ""
                            'Dim PID23 As String = ""
                            'Dim PID24 As String = ""
                            'Dim PID25 As String = ""
                            'Dim PID26 As String = ""
                            'Dim PID27 As String = ""
                            'Dim PID28 As String = ""
                            'Dim PID29 As String = ""
                            'Dim PID30 As String = ""
                        Else
                            log.Debug("VehicleRecurringMSG 5.1")
                            Try
                                Dim VINAuthorizationID As Integer = 0
                                VINAuthorizationID = OBJMasterBAL.InsertUpdateFSVM(VINAuthorizationID, 0, customerId, RPM, SPD, MIL, Odometer, PC, PID1, PID2, PID3, PID4, PID5, PID6, PID7, PID8,
                                                                               PID9, PID10, PID11, PID12, PID13, PID14, PID15, PID16, PID17, PID18, PID19, PID20)
                            Catch ex As Exception

                            End Try
                            FAVehicleAuthorizationMasterResponseObj.ResponceMessage = "fail"
                            FAVehicleAuthorizationMasterResponseObj.ResponceText = "VIN not found. Please contact administrator"
                        End If
                    Catch ex As Exception
                        log.Debug("VehicleRecurringMSG 6")
                        FAVehicleAuthorizationMasterResponseObj.ResponceMessage = "fail"
                        FAVehicleAuthorizationMasterResponseObj.ResponceText = resourceManager.GetString("HandlerMsg15")
                        log.Error("Error occurred in VINAuthorization. Exception is :" & ex.Message)
                    End Try
                Else
                    log.Debug("VehicleRecurringMSG 7")
                    FAVehicleAuthorizationMasterResponseObj.ResponceMessage = "fail"
                    FAVehicleAuthorizationMasterResponseObj.ResponceText = "Vehicle Recurring Message is blank."
                End If

                log.Debug("VehicleRecurringMSG 8")

                json = javaScriptSerializer.Serialize(FAVehicleAuthorizationMasterResponseObj)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            log.Error("Exception occurred while prcessing request in VINAuthorization. + steps - " & steps & " Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try
    End Sub

    Private Function CreateData(TransactionId As Integer, CreateDataFor As String) As String
        Try

            Dim createDataFrom() = CreateDataFor.Split(";")

            Dim data As String = "TransactionId = " & TransactionId & " ; " &
                                    "VIN = " & createDataFrom(0) & " ; " &
                                    "Vehicle Name = " & createDataFrom(1) & " ; " &
                                    "Department = " & createDataFrom(2) & " ; " &
                                    "Department Number = " & createDataFrom(3) & " ; " &
                                    "Guest Vehicle Number = " & createDataFrom(4) & " ; " &
                                    "FluidSecure Link = " & createDataFrom(5) & " ; " &
                                    "Fuel Quantity = " & createDataFrom(6) & " ; " &
                                    "Other = " & createDataFrom(7) & " ; " &
                                    "Company = " & createDataFrom(8) & " ; " &
                                    "Cost = " & createDataFrom(9) & " ; " &
                                    "Transaction Date = " & createDataFrom(10) & " ; " &
                                    "Transaction Time = " & createDataFrom(11) & " ; " &
                                    "Person = " & createDataFrom(12) & " ; " &
                                    "Person PIN = " & createDataFrom(13) & " ; " &
                                    "Current Odometer = " & createDataFrom(14) & " ; " &
                                    "Previous Odometer = " & createDataFrom(15) & " ; " &
                                    "Hours = " & createDataFrom(16) & " ; " &
                                    "Fuel Type = " & createDataFrom(17) & " ; " &
                                    "Transaction Status = " & createDataFrom(18) & " ; " &
                                    "Pulses = " & createDataFrom(19) & " ; "

            'CreateDataFor = "Vehicle Number; Vehicle Name; Department; Department Number; Guest Vehicle Number; FluidSecure Link; Fuel Quantity; Other; Company; Cost; Transaction Date; Transaction Time; Person; Person PIN; Current Odometer; Previous Odometer; Hours; Fuel Type; Transaction Status;"

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Sub CheckAndValidateFSNPDetails(context As HttpContext, IMEI As String)
        steps = "CheckAndValidateFSNPDetails 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("CheckAndValidateFSNPDetails Before:" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            data = data.Replace("\", "").TrimEnd("""").TrimStart("""")
            log.Debug("CheckAndValidateFSNPDetails After:" & data)
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(CheckAndValidateFSNPDetail))
            Dim IMEI_UDID = DirectCast(serJsonDetails, CheckAndValidateFSNPDetail).IMEI_UDID
            Dim FSNPMacAddress = DirectCast(serJsonDetails, CheckAndValidateFSNPDetail).FSNPMacAddress
            Dim FSTagMacAddress = DirectCast(serJsonDetails, CheckAndValidateFSNPDetail).FSTagMacAddress

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "CheckAndValidateFSNPDetails 2"
            Dim rootOject = New CheckAndValidateFSNPDetailResponse()
            Dim json As String = ""
            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        dtMain = dsIMEI.Tables(0)

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "CheckAndValidateFSNPDetails 3"

                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            Dim CustomerId As Integer
                            Dim RoleId As String
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            CustomerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            RoleId = dt.Rows(0)("RoleId").ToString()

                            steps = "CheckAndValidateFSNPDetails 5"
                            OBJMasterBAL = New MasterBAL()

                            Dim dtHose As DataTable = OBJMasterBAL.GetHoseByCondition(" And h.FSNPMacAddress ='" & FSNPMacAddress & "' and h.CustomerID =" & CustomerId.ToString() & "", personId, RoleId)

                            If (dtHose Is Nothing Or dtHose.Rows.Count = 0) Then
                                FSNPDetailResponse(javaScriptSerializer, context, "fail", "FS Link not found")
                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- FS Link not found. Please contact administrator. IMEI_UDID=" & IMEI_UDID & " , FSNPMacAddress=" & FSNPMacAddress)
                                Return
                            End If

                            'If (dtHose.Rows(0)("IsBusy") = True) Then
                            '	FSNPDetailResponse(javaScriptSerializer, context, "fail", "FS Link is busy")
                            '	log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- FS Link is busy. Please contact administrator. IMEI_UDID=" & IMEI_UDID & " , FSNPMacAddress=" & FSNPMacAddress)
                            '	Return
                            'End If

                            Dim SiteId As Integer = dtHose.Rows(0)("SiteId")
                            Dim dtVehicle As DataTable = New DataTable()
                            dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & CustomerId & "  And V.FSTagMacAddress ='" & FSTagMacAddress & "'", personId, RoleId)

                            If (dtVehicle Is Nothing Or dtVehicle.Rows.Count = 0) Then
                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg45"))
                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Vehicle not found. Please contact administrator. IMEI_UDID=" & IMEI_UDID & " , FSTagMacAddress=" & FSTagMacAddress)
                                Return
                            End If

                            Dim VehicleId As Integer = dtVehicle.Rows(0)("VehicleId")
                            If (dtVehicle.Rows(0)("IsActive").ToString() = "False") Then
                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg16"))
                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Inactive vehicle, please contact administrator. IMEI_UDID=" & IMEI_UDID & " , VehicleId=" & VehicleId)
                                Return
                            End If

                            OBJMasterBAL = New MasterBAL()
                            Dim dtTransaction As DataTable = New DataTable()
                            Dim dsT As DataSet = New DataSet()
                            dsT = OBJMasterBAL.GetTransactionsByCondition(" and T.VehicleId=" & VehicleId & " and (((ISNULL(T.TransactionStatus,0) = 1  or ISNULL(T.IsMissed,0)= 1) or ISNULL(T.TransactionStatus,0) = 0) and datediff(minute, T.[CreatedDate] ,getdate()) <= 15)", personId, RoleId, False, 0, 0, False, "", "")
                            dtTransaction = dsT.Tables(0)

                            If (dtTransaction Is Nothing Or dtTransaction.Rows.Count = 0) Then
                                FSNPDetailResponse(javaScriptSerializer, context, "fail", "Transaction not found")
                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Transaction not found. Please contact administrator. IMEI_UDID=" & IMEI_UDID & " , VehicleId=" & VehicleId)
                                Return
                            End If


                            steps = "CheckAndValidateFSNPDetails 5.1"
                            OBJMasterBAL = New MasterBAL()

                            log.Debug("DiffRawOdoAndManualOdo - " & dtVehicle.Rows(0)("DiffRawOdoAndManualOdo").ToString())

                            If dtVehicle.Rows(0)("DiffRawOdoAndManualOdo").ToString().Trim() = "" Then
                                'Dim dtTransactionVehicleInfo As DataTable
                                'dtTransactionVehicleInfo = OBJMasterBAL.GetTransactionsByCondition(" and T.TransactionStatus = 2 and T.VehicleId = " & VehicleId, personId, RoleId, 0)
                                'If dtTransactionVehicleInfo IsNot Nothing Then
                                'If dtTransactionVehicleInfo.Rows.Count < 1 Then
                                FSNPDetailResponse(javaScriptSerializer, context, "success", "fail + AskOdo", VehicleId, Convert.ToString(dtVehicle.Rows(0)("VehicleNumber").ToString()), dtVehicle, SiteId, personId)
                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Completed Transaction not found. Please contact administrator. IMEI_UDID=" & IMEI_UDID & " , VehicleId=" & VehicleId)
                                Return
                                'End If
                                'End If
                            End If


                            Dim dtVehicelSiteMapping As DataTable = New DataTable()
                            dtVehicelSiteMapping = OBJMasterBAL.GetVehicleSiteMapping(VehicleId, SiteId)
                            If Not dtVehicelSiteMapping Is Nothing Then     'Vehicle Site Mapping not exist
                                If dtVehicelSiteMapping.Rows.Count <> 0 Then 'Vehicle Site Mapping not exist
                                    'Authorized fuel date?
                                    Dim dtSiteDays = New DataTable()
                                    dtSiteDays = OBJMasterBAL.GetSiteDays(Integer.Parse(SiteId))
                                    If Not dtSiteDays Is Nothing Then    'site Days not assigned to site
                                        If dtSiteDays.Rows.Count <> 0 Then
                                            Dim dateValue As New DateTime()
                                            dateValue = DateTime.Now
                                            steps = "CheckAndValidateFSNPDetails 7"
                                            Dim currentDay = CInt(dateValue.DayOfWeek) + 1
                                            Dim dayArray = dtSiteDays.AsEnumerable().[Select](Function(r) r.Field(Of Integer)("Day")).ToArray()
                                            Dim isCurrentDayInDayArray = dayArray.Contains(currentDay)
                                            If isCurrentDayInDayArray = True Then 'Current Day not in site days
                                                Dim dtPersonalTiming = New DataTable()
                                                steps = "CheckAndValidateFSNPDetails 8"
                                                dtPersonalTiming = OBJMasterBAL.GetPersonnelTimings(personId)
                                                If Not dtPersonalTiming Is Nothing Then 'Person timing does not exists
                                                    If dtPersonalTiming.Rows.Count <> 0 Then 'Person timing does not exists
                                                        Dim dtSiteTiming = New DataTable()
                                                        dtSiteTiming = OBJMasterBAL.GetSiteTimings(SiteId)
                                                        steps = "CheckAndValidateFSNPDetails 9"
                                                        If Not dtSiteTiming Is Nothing Then 'Site timing does not exists
                                                            If dtSiteTiming.Rows.Count <> 0 Then 'Site timing does not exists
                                                                Dim dsCurrentTimeInPerson = New DataSet()
                                                                dsCurrentTimeInPerson = OBJMasterBAL.CheckCurrentTimeInTimesTable(SiteId, personId)
                                                                If Not dsCurrentTimeInPerson Is Nothing Then ' check Current Time In Times Table
                                                                    If dsCurrentTimeInPerson.Tables.Count = 2 Then
                                                                        If dsCurrentTimeInPerson.Tables(0).Rows.Count <> 0 And dsCurrentTimeInPerson.Tables(1).Rows.Count <> 0 Then 'Current Time not in SiteTiming and PersonnelTimings from-to timing
                                                                            'PersonalVehicle mapping
                                                                            Dim dtPersonVehicleMapping = New DataTable()
                                                                            dtPersonVehicleMapping = OBJMasterBAL.GetPersonVehicleMapping(personId)
                                                                            If Not dtPersonVehicleMapping Is Nothing Then 'Person vehicle mapping does not exists
                                                                                If dtPersonVehicleMapping.Rows.Count <> 0 Then 'Person vehicle mapping does not exists
                                                                                    Dim vehicleArray = dtPersonVehicleMapping.AsEnumerable().[Select](Function(r) r.Field(Of Integer)("VehicleId")).ToArray()
                                                                                    steps = "CheckAndValidateFSNPDetails 10"
                                                                                    Dim isVehicleIdInVehicleArray = vehicleArray.Contains(VehicleId)
                                                                                    If isVehicleIdInVehicleArray Then ' veihcle id send by user not assigned to person -PersonVehicle mapping not match
                                                                                        'IsOdometerRequire on screen mobile application
                                                                                        steps = "CheckAndValidateFSNPDetails 11"

                                                                                        Dim fuelTypeOfHose As Integer
                                                                                        fuelTypeOfHose = Integer.Parse(dtHose.Rows(0)("FuelTypeId").ToString())

                                                                                        steps = "CheckAndValidateFSNPDetails 11_1"
                                                                                        'get vehicle fuel limit per day
                                                                                        'check vehicle fuel limit per day
                                                                                        Dim vehicleFuelLimitForDay As Integer
                                                                                        vehicleFuelLimitForDay = Integer.Parse(dtVehicle.Rows(0)("FuelLimitPerDay").ToString())
                                                                                        steps = "CheckAndValidateFSNPDetails 11_2"
                                                                                        Dim personFuelLimitForDay As Integer
                                                                                        personFuelLimitForDay = Integer.Parse(dt.Rows(0)("FuelLimitPerDay").ToString())
                                                                                        Dim phoneNumber As String = dt.Rows(0)("PhoneNumber").ToString()
                                                                                        Dim dsTransactionFuelLimitForDay = New DataSet()
                                                                                        dsTransactionFuelLimitForDay = OBJMasterBAL.GetSumOfFuelQuantity(personId, VehicleId) 'Get sum of person fuel transaction and vehicle fuel transaction for current day
                                                                                        steps = "CheckAndValidateFSNPDetails 11_3"
                                                                                        If Not dsTransactionFuelLimitForDay Is Nothing Then
                                                                                            If dsTransactionFuelLimitForDay.Tables.Count = 2 Then
                                                                                                If Not dsTransactionFuelLimitForDay.Tables(0) Is Nothing And Not dsTransactionFuelLimitForDay.Tables(1) Is Nothing Then
                                                                                                    If dsTransactionFuelLimitForDay.Tables(0).Rows.Count <> 0 And dsTransactionFuelLimitForDay.Tables(1).Rows.Count <> 0 Then
                                                                                                        If Decimal.Parse(dsTransactionFuelLimitForDay.Tables(0).Rows(0)("FuelQuantityOfPerson").ToString()) < personFuelLimitForDay Or personFuelLimitForDay = 0 Then
                                                                                                            If Decimal.Parse(dsTransactionFuelLimitForDay.Tables(1).Rows(0)("FuelQuantityOfVehicle").ToString()) < vehicleFuelLimitForDay Or vehicleFuelLimitForDay = 0 Then
                                                                                                                steps = "CheckAndValidateFSNPDetails 11_4"
                                                                                                                'calculate min fuel limit per day
                                                                                                                '0 means unlimited
                                                                                                                Dim minLimitForPerson As Integer
                                                                                                                Dim minLimitForVehicle As Integer
                                                                                                                If personFuelLimitForDay <> 0 Then
                                                                                                                    minLimitForPerson = personFuelLimitForDay - Decimal.Parse(dsTransactionFuelLimitForDay.Tables(0).Rows(0)("FuelQuantityOfPerson").ToString())
                                                                                                                Else
                                                                                                                    minLimitForPerson = 0
                                                                                                                End If
                                                                                                                steps = "CheckAndValidateFSNPDetails 11_5"

                                                                                                                If vehicleFuelLimitForDay <> 0 Then
                                                                                                                    minLimitForVehicle = vehicleFuelLimitForDay - Decimal.Parse(dsTransactionFuelLimitForDay.Tables(1).Rows(0)("FuelQuantityOfVehicle").ToString())
                                                                                                                Else
                                                                                                                    minLimitForVehicle = 0
                                                                                                                End If
                                                                                                                steps = "CheckAndValidateFSNPDetails 11_6"
                                                                                                                Dim minLimit As Integer
                                                                                                                If minLimitForPerson = 0 Then
                                                                                                                    minLimit = minLimitForVehicle
                                                                                                                ElseIf minLimitForVehicle = 0 Then
                                                                                                                    minLimit = minLimitForPerson
                                                                                                                ElseIf minLimitForPerson <= minLimitForVehicle Then
                                                                                                                    minLimit = minLimitForPerson
                                                                                                                Else
                                                                                                                    minLimit = minLimitForVehicle
                                                                                                                End If
                                                                                                                steps = "CheckAndValidateFSNPDetails 11_7"
                                                                                                                'calculate min fuel limit per transaction
                                                                                                                Dim personFuellimitPerTran As Integer
                                                                                                                personFuellimitPerTran = 0
                                                                                                                If Not dt.Rows(0)("FuelLimitPerTxn".ToString()) Is Nothing Then
                                                                                                                    personFuellimitPerTran = Integer.Parse(dt.Rows(0)("FuelLimitPerTxn".ToString()))
                                                                                                                End If
                                                                                                                steps = "CheckAndValidateFSNPDetails 11_8"
                                                                                                                Dim vehicleFuellimitPerTran As Integer
                                                                                                                vehicleFuellimitPerTran = 0
                                                                                                                If Not dtVehicle.Rows(0)("FuelLimitPerTxn").ToString() Is Nothing Then
                                                                                                                    vehicleFuellimitPerTran = Integer.Parse(dtVehicle.Rows(0)("FuelLimitPerTxn").ToString())
                                                                                                                End If
                                                                                                                steps = "CheckAndValidateFSNPDetails 11_9"
                                                                                                                If minLimit = 0 Then
                                                                                                                    If personFuellimitPerTran = 0 Then
                                                                                                                        minLimit = vehicleFuellimitPerTran
                                                                                                                    ElseIf vehicleFuellimitPerTran = 0 Then
                                                                                                                        minLimit = personFuellimitPerTran
                                                                                                                    ElseIf personFuellimitPerTran <= vehicleFuellimitPerTran Then
                                                                                                                        minLimit = personFuellimitPerTran
                                                                                                                    Else
                                                                                                                        minLimit = vehicleFuellimitPerTran
                                                                                                                    End If
                                                                                                                Else
                                                                                                                    If minLimit >= personFuellimitPerTran And personFuellimitPerTran <> 0 Then
                                                                                                                        minLimit = personFuellimitPerTran
                                                                                                                    End If

                                                                                                                    If minLimit >= vehicleFuellimitPerTran And vehicleFuellimitPerTran <> 0 Then
                                                                                                                        minLimit = vehicleFuellimitPerTran
                                                                                                                    End If
                                                                                                                End If

                                                                                                                steps = "CheckAndValidateFSNPDetails 11_10"

                                                                                                                Dim dsTransactionValuesData As DataSet
                                                                                                                Dim DepartmentName As String = ""
                                                                                                                Dim FuelTypeName As String = ""
                                                                                                                Dim Email As String = ""
                                                                                                                Dim PersonName As String = ""
                                                                                                                Dim CompanyName As String = ""
                                                                                                                Dim VehicleSum As Decimal = 0
                                                                                                                Dim DeptSum As Decimal = 0
                                                                                                                Dim VehPercentage As Decimal = 0
                                                                                                                Dim DeptPercentage As Decimal = 0
                                                                                                                Dim SurchargeType As String = "0"
                                                                                                                Dim ProductPrice As Decimal = 0
                                                                                                                Dim DepartmentNumber As String = ""

                                                                                                                dsTransactionValuesData = OBJMasterBAL.GetTransactionColumnsValueForSave(DepartmentNumber, fuelTypeOfHose, personId, VehicleId)

                                                                                                                If dsTransactionValuesData IsNot Nothing Then
                                                                                                                    If dsTransactionValuesData.Tables.Count > 0 Then
                                                                                                                        If dsTransactionValuesData.Tables(0) IsNot Nothing And dsTransactionValuesData.Tables(0).Rows.Count > 0 Then
                                                                                                                            DepartmentName = dsTransactionValuesData.Tables(0).Rows(0)("DeptName")
                                                                                                                            DepartmentNumber = dsTransactionValuesData.Tables(0).Rows(0)("DeptNumber")
                                                                                                                            VehicleSum = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("VehicleSum"))
                                                                                                                            DeptSum = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("DeptSum"))
                                                                                                                            VehPercentage = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("VehPercentage"))
                                                                                                                            DeptPercentage = Decimal.Parse(dsTransactionValuesData.Tables(0).Rows(0)("DeptPercentage"))
                                                                                                                            SurchargeType = dsTransactionValuesData.Tables(0).Rows(0)("SurchargeType")
                                                                                                                        End If
                                                                                                                        If dsTransactionValuesData.Tables(1) IsNot Nothing And dsTransactionValuesData.Tables(1).Rows.Count > 0 Then
                                                                                                                            FuelTypeName = dsTransactionValuesData.Tables(1).Rows(0)("FuelTypeName")
                                                                                                                            ProductPrice = Decimal.Parse(dsTransactionValuesData.Tables(1).Rows(0)("ProductPrice"))
                                                                                                                        End If
                                                                                                                        If dsTransactionValuesData.Tables(2) IsNot Nothing And dsTransactionValuesData.Tables(2).Rows.Count > 0 Then
                                                                                                                            Email = dsTransactionValuesData.Tables(2).Rows(0)("Email")
                                                                                                                            PersonName = dsTransactionValuesData.Tables(2).Rows(0)("PersonName")
                                                                                                                        End If
                                                                                                                        If dsTransactionValuesData.Tables(3) IsNot Nothing And dsTransactionValuesData.Tables(3).Rows.Count > 0 Then
                                                                                                                            CompanyName = dsTransactionValuesData.Tables(3).Rows(0)("CompanyName")
                                                                                                                        End If
                                                                                                                    End If
                                                                                                                End If

                                                                                                                OBJMasterBAL = New MasterBAL()
                                                                                                                OBJMasterBAL.UpdateFSVMTransaction(SiteId, dtHose.Rows(0)("FuelTypeId"), dtHose.Rows(0)("WifiSSid").ToString().Trim(), personId, dtTransaction.Rows(0)("TransactionId"))

                                                                                                                Dim pulseRatio = Decimal.Parse(dtHose.Rows(0)("PulserRatio").ToString())
                                                                                                                rootOject.ResponceMessage = "success"
                                                                                                                rootOject.ResponceText = "success"
                                                                                                                rootOject.MinLimit = minLimit 'need to check
                                                                                                                rootOject.SiteId = SiteId
                                                                                                                rootOject.PulseRatio = pulseRatio
                                                                                                                rootOject.VehicleId = VehicleId
                                                                                                                rootOject.PersonId = personId
                                                                                                                rootOject.FuelTypeId = dtHose.Rows(0)("FuelTypeId")
                                                                                                                rootOject.PhoneNumber = dtTransaction.Rows(0)("PhoneNumber")
                                                                                                                rootOject.ServerDate = dtTransaction.Rows(0)("TransactionDateTime")
                                                                                                                rootOject.PumpOnTime = dtHose.Rows(0)("PumpOnTime")
                                                                                                                rootOject.PumpOffTime = dtHose.Rows(0)("PumpOffTime")
                                                                                                                rootOject.PulserStopTime = ConfigurationManager.AppSettings("PulserStopTime").ToString()
                                                                                                                rootOject.TransactionId = dtTransaction.Rows(0)("TransactionId")
                                                                                                                rootOject.FirmwareVersion = "" 'need to check
                                                                                                                rootOject.FilePath = "" 'need to check
                                                                                                                rootOject.FOBNumber = dtVehicle.Rows(0)("FOBNumber").ToString().Replace(" ", "")
                                                                                                                rootOject.Company = CompanyName 'need to check
                                                                                                                rootOject.Location = dtHose.Rows(0)("SiteAddress")
                                                                                                                rootOject.PersonName = "" 'need to check
                                                                                                                rootOject.PrinterName = dsIMEI.Tables(0).Rows(0)("PrinterName") ' need to check
                                                                                                                rootOject.PrinterMacAddress = dsIMEI.Tables(0).Rows(0)("PrinterMacAddress").ToString().ToLower() ' need to check
                                                                                                                rootOject.VehicleSum = VehicleSum ' need to check
                                                                                                                rootOject.DeptSum = DeptSum ' need to check
                                                                                                                rootOject.VehPercentage = VehPercentage ' need to check
                                                                                                                rootOject.DeptPercentage = DeptPercentage ' need to check
                                                                                                                rootOject.SurchargeType = SurchargeType ' need to check
                                                                                                                rootOject.ProductPrice = ProductPrice ' need to check
                                                                                                                rootOject.VehicleNumber = Convert.ToString(dtVehicle.Rows(0)("VehicleNumber").ToString())
                                                                                                                rootOject.RequireManualOdo = "n"

                                                                                                                rootOject.PreviousOdo = IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))

                                                                                                                If (dtVehicle.Rows(0)("CheckOdometerReasonable") = "Y") Then

                                                                                                                    rootOject.OdometerReasonabilityConditions = IIf(Convert.ToString(dtVehicle.Rows(0)("OdometerReasonabilityConditions")) = "", 1, dtVehicle.Rows(0)("OdometerReasonabilityConditions"))

                                                                                                                    rootOject.OdoLimit = (Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))) +
                                                                                                                        Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("Odolimit")), 0, dtVehicle.Rows(0)("Odolimit"))))
                                                                                                                Else
                                                                                                                    rootOject.OdometerReasonabilityConditions = 2
                                                                                                                    rootOject.OdoLimit = 0
                                                                                                                End If

                                                                                                                rootOject.CheckOdometerReasonable = IIf(dtVehicle.Rows(0)("CheckOdometerReasonable") = "Y", "True", "False")

                                                                                                                Try
                                                                                                                    Dim resultUpgradable As String = ""
                                                                                                                    resultUpgradable = IsUpgradeCurrentVersionWithUgradableVersionFSNP(SiteId, personId, "")

                                                                                                                    log.Debug("resultUpgradable before: " & resultUpgradable)

                                                                                                                    If resultUpgradable = "" Then
                                                                                                                        resultUpgradable = "N"
                                                                                                                    End If

                                                                                                                    log.Debug("resultUpgradable after: " & resultUpgradable)

                                                                                                                    If resultUpgradable.ToUpper() = "Y" Then
                                                                                                                        rootOject = GetLaunchedFSNPFirmwareDetails(rootOject)
                                                                                                                    End If

                                                                                                                    rootOject.IsFSNPUpgradable = resultUpgradable
                                                                                                                Catch ex As Exception
                                                                                                                    log.Error(String.Format("Error Occurred while CheckAndValidateFSNPDetails -  Success - FSNP Firmware. Error is {0}.", ex.Message))
                                                                                                                End Try

                                                                                                                json = javaScriptSerializer.Serialize(rootOject)
                                                                                                                log.Debug("FSNP Check Response: " & json)
                                                                                                                context.Response.Write(json)

                                                                                                                Return
                                                                                                            Else
                                                                                                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg30"))
                                                                                                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Vehicle fuel limit for the day exceeded. IMEI_UDID=" & IMEI_UDID & " , VehicleId=" & VehicleId)
                                                                                                            End If
                                                                                                        Else
                                                                                                            FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg31"))
                                                                                                            ErrorInAuthontication(context, "fail", "Personal fuel limit for the day exceeded. IMEI_UDID=" & IMEI_UDID & " , PersonId=" & personId)
                                                                                                        End If
                                                                                                    Else
                                                                                                        FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg51"))
                                                                                                        log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Transaction Not Found")
                                                                                                    End If
                                                                                                Else
                                                                                                    FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg51"))
                                                                                                    log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Transaction Not Found")
                                                                                                End If
                                                                                            Else
                                                                                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg51"))
                                                                                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Transaction Not Found")
                                                                                            End If
                                                                                        Else
                                                                                            FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg51"))
                                                                                            log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Transactions Not Found")
                                                                                        End If
                                                                                    Else
                                                                                        FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg20"))
                                                                                        log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted you the permission to fuel this vehicle. Please contact your administrator. for IMEI_UDID=" & IMEI_UDID)
                                                                                    End If
                                                                                Else
                                                                                    FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg20"))
                                                                                    log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted you the permission to fuel this vehicle. Please contact your administrator. for IMEI_UDID=" & IMEI_UDID)
                                                                                End If
                                                                            Else
                                                                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg20"))
                                                                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted you the permission to fuel this vehicle. Please contact your administrator. for IMEI_UDID=" & IMEI_UDID)
                                                                            End If
                                                                        Else
                                                                            FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg21"))
                                                                            log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted you the permission to fuel at this time of day. Please contact your administrator. for IMEI_UDID=" & IMEI_UDID)
                                                                        End If
                                                                    Else
                                                                        FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg21"))
                                                                        log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted you the permission to fuel at this time of day. Please contact your administrator. for IMEI_UDID=" & IMEI_UDID)
                                                                    End If
                                                                Else
                                                                    FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg21"))
                                                                    log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted you the permission to fuel at this time of day. Please contact your administrator. for IMEI_UDID=" & IMEI_UDID)
                                                                End If
                                                            Else
                                                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg22"))
                                                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted Hose the permission to fuel at this time of day. Please contact your administrator. IMEI_UDID=" & IMEI_UDID)
                                                            End If
                                                        Else
                                                            FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg22"))
                                                            log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted Hose the permission to fuel at this time of day. Please contact your administrator. IMEI_UDID=" & IMEI_UDID)
                                                        End If
                                                    Else
                                                        FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg21"))
                                                        log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has Not granted you the permission to fuel at this time of day. Please contact your administrator. IMEI_UDID=" & IMEI_UDID)
                                                    End If
                                                Else
                                                    FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg21"))
                                                    log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Fuel timings are not assigned, please contact administrator. IMEI_UDID=" & IMEI_UDID)
                                                End If
                                            Else
                                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg23"))
                                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Unauthorized fuel day. IMEI_UDID=" & IMEI_UDID)
                                            End If
                                        Else
                                            FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg23"))
                                            log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has not given you the permission to fuel on this day. Please contact your administrator. IMEI_UDID=" & IMEI_UDID)
                                        End If
                                    Else
                                        FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg23"))
                                        log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your administrator has not given you the permission to fuel on this day. Please contact your administrator. for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                    End If
                                Else
                                    FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg24"))
                                    log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Valid Vehicle but not Authorized for this Hose. for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                                End If
                            Else
                                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg24"))
                                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Valid Vehicle but not Authorized for this Hose for IMEI_UDID=" & IMEI_UDID & " step " & steps)
                            End If
                        Else
                            FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg28"))
                            log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                        End If
                    Else
                        FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg29"))
                        log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                    End If

                Else
                    FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg29"))
                    log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                End If
            Else
                FSNPDetailResponse(javaScriptSerializer, context, "fail", resourceManager.GetString("HandlerMsg29"))
                log.Debug("ProcessRequest: CheckAndValidateFSNPDetails- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
            End If

        End Using
    End Sub

    Private Sub FSNPDetailResponse(javaScriptSerializer As JavaScriptSerializer, context As HttpContext,
                                   ResponseMessage As String, ResponseText As String, Optional vehicleId As String = "", Optional VehicleNumber As String = "", Optional dtVehicle As DataTable = Nothing, Optional SiteId As Integer = 0, Optional personId As Integer = 0)
        Dim rootOject As CheckAndValidateFSNPDetailResponse = New CheckAndValidateFSNPDetailResponse()
        Dim json As String = ""
        rootOject.ResponceMessage = ResponseMessage
        rootOject.ResponceText = ResponseText
        rootOject.VehicleId = vehicleId
        rootOject.VehicleNumber = VehicleNumber
        If ResponseText = "fail + AskOdo" Then
            rootOject.RequireManualOdo = "y"
            rootOject.PreviousOdo = IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))

            If (dtVehicle.Rows(0)("CheckOdometerReasonable") = "Y") Then

                rootOject.OdometerReasonabilityConditions = IIf(Convert.ToString(dtVehicle.Rows(0)("OdometerReasonabilityConditions")) = "", 1, dtVehicle.Rows(0)("OdometerReasonabilityConditions"))

                rootOject.OdoLimit = (Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("CurrentOdometer")), 0, dtVehicle.Rows(0)("CurrentOdometer"))) +
                    Integer.Parse(IIf(IsDBNull(dtVehicle.Rows(0)("Odolimit")), 0, dtVehicle.Rows(0)("Odolimit"))))
            Else
                rootOject.OdometerReasonabilityConditions = 2
                rootOject.OdoLimit = 0
            End If
            rootOject.CheckOdometerReasonable = IIf(dtVehicle.Rows(0)("CheckOdometerReasonable") = "Y", "True", "False")
        Else
            rootOject.RequireManualOdo = "n"
        End If
        If SiteId <> 0 Then
            Dim resultUpgradable As String = ""
            resultUpgradable = IsUpgradeCurrentVersionWithUgradableVersionFSNP(SiteId, personId, "")

            If resultUpgradable = "" Then
                resultUpgradable = "N"
            End If


            If resultUpgradable.ToUpper() = "Y" Then
                rootOject = GetLaunchedFSNPFirmwareDetails(rootOject)
            End If

            rootOject.IsFSNPUpgradable = resultUpgradable

        End If
        json = javaScriptSerializer.Serialize(rootOject)
        context.Response.Write(json)
    End Sub

    Private Sub SaveManualVehicleOdometer(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Dim SaveVehicleManualOdometerResponseObj = New SaveVehicleManualOdometerResponse()
        Dim json As String
        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("SaveManualVehicleOdometer : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(SaveVehicleManualOdometerMaster))

                Dim VehicleId = DirectCast(serJsonDetails, SaveVehicleManualOdometerMaster).VehicleId
                Dim Odometer = DirectCast(serJsonDetails, SaveVehicleManualOdometerMaster).Odometer

                OBJMasterBAL = New MasterBAL()

                OBJMasterBAL.UpdateVehicleManualOdometerAndDifference(VehicleId, Odometer)

                SaveVehicleManualOdometerResponseObj.ResponceMessage = "success"
                SaveVehicleManualOdometerResponseObj.ResponceText = "Manual Odometer updated successfully."
                json = javaScriptSerializer.Serialize(SaveVehicleManualOdometerResponseObj)
                context.Response.Write(json)
            End Using

        Catch ex As Exception

            log.Error("Exception occurred while prcessing request in SaveManualVehicleOdometer. Exception is :" & ex.Message & " . Details : " & requestJson)

            ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

        End Try
    End Sub


    Private Sub SaveDiagnosticLogs(context As HttpContext, IMEI_UDID As String)
        Dim requestJson As String = ""
        Dim CollectDiagnosticLogsDetailsResponseObj = New CollectDiagnosticLogsDetailsResponse()
        Dim json As String
        Dim javaScriptSerializer = New JavaScriptSerializer()

        Try

            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                'log.Info("SaveDiagnosticLogs : " & data)


                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(CollectDiagnosticLogsDetails))
                Dim Collectedlogs = DirectCast(serJsonDetails, CollectDiagnosticLogsDetails).Collectedlogs
                Dim LogFrom = DirectCast(serJsonDetails, CollectDiagnosticLogsDetails).LogFrom
                Dim FileName = DirectCast(serJsonDetails, CollectDiagnosticLogsDetails).FileName

                Dim dsIMEI = New DataSet()
                dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
                Dim PersonId = dsIMEI.Tables(0).Rows(0)("PersonId").ToString()


                Dim PersonDiagnosticLogs As ILog = LogManager.GetLogger("RollingLogFileAppenderForLogDiagnosticLogs")
                Dim path = PersonId & "/" & IMEI_UDID
                GlobalContext.Properties("path") = path

                'Dim tsForDL = DateTime.Now.ToString("yyyy MM dd")

                GlobalContext.Properties("fileNameForDL") = FileName
                'GlobalContext.Properties("fromlog") = "_" & LogFrom

                Config.XmlConfigurator.Configure()
                PersonDiagnosticLogs.Info(Collectedlogs & Environment.NewLine)

                CollectDiagnosticLogsDetailsResponseObj.ResponceMessage = "success"
                CollectDiagnosticLogsDetailsResponseObj.ResponceText = "Collected logs saved successfully."
                json = javaScriptSerializer.Serialize(CollectDiagnosticLogsDetailsResponseObj)
                context.Response.Write(json)

            End Using

        Catch ex As Exception

            log.Error("Exception occurred while prcessing request in SaveDiagnosticLogs. Exception is :" & ex.Message & " . Details : " & requestJson)
            CollectDiagnosticLogsDetailsResponseObj.ResponceMessage = "fail"
            CollectDiagnosticLogsDetailsResponseObj.ResponceText = resourceManager.GetString("HandlerMsg15")
            json = javaScriptSerializer.Serialize(CollectDiagnosticLogsDetailsResponseObj)
            context.Response.Write(json)


        End Try
    End Sub

    Private Sub SaveDiagnosticLogFile(context As HttpContext, IMEI_UDID As String, LogFrom As String)
        Dim data = [String].Empty
        Dim javaScriptSerializer = New JavaScriptSerializer()
        Dim CollectDiagnosticLogsDetailsResponseObj = New CollectDiagnosticLogsDetailsResponse()


        Dim json As String
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            'log.Info("SaveDiagnosticLogFile : " & data)
            'log.Info("LogFrom" & LogFrom)
            'Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(CollectDiagnosticLogsDetails))

            'log.Info("SaveDiagnosticLogFile : " & data)

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            Dim PersonId = dsIMEI.Tables(0).Rows(0)("PersonId").ToString()

            If context.Request.Files.Count > 0 Then

                Dim file As HttpPostedFile = Nothing

                For i As Integer = 0 To context.Request.Files.Count - 1
                    file = context.Request.Files(i)

                    If file.ContentLength > 0 Then
                        Try

                            Dim filename As String = file.FileName.Split(".")(0) & "_" & DateTime.Now.ToString("yyyy MM dd HHmmss") & "_" & LogFrom & "." & file.FileName.Split(".")(1)
                            'log.Info("filename==>" & filename)

                            Dim path = System.IO.Path.Combine(context.Server.MapPath("~/DiagnosticLogs/" & PersonId & "/" & IMEI_UDID & "/"), filename)
                            'log.Info("path==>" & path)
                            Dim folderPath = context.Server.MapPath("~/DiagnosticLogs/" & PersonId & "/" & IMEI_UDID & "/")
                            'log.Info("folderPath==>" & folderPath)

                            If Not System.IO.Directory.Exists(folderPath) Then
                                System.IO.Directory.CreateDirectory(folderPath)
                            End If

                            file.SaveAs(path)

                            CollectDiagnosticLogsDetailsResponseObj.ResponceMessage = "success"
                            CollectDiagnosticLogsDetailsResponseObj.ResponceText = "Collected logs saved successfully."
                            json = javaScriptSerializer.Serialize(CollectDiagnosticLogsDetailsResponseObj)
                            context.Response.Write(json)

                        Catch ex As Exception

                            log.Error("Exception occurred while prcessing request in SaveDiagnosticLogFile. Exception is :" & ex.Message)
                            CollectDiagnosticLogsDetailsResponseObj.ResponceMessage = "fail"
                            CollectDiagnosticLogsDetailsResponseObj.ResponceText = resourceManager.GetString("HandlerMsg15")
                            json = javaScriptSerializer.Serialize(CollectDiagnosticLogsDetailsResponseObj)
                            context.Response.Write(json)

                        End Try
                    Else

                    End If
                Next

            End If

        End Using
    End Sub

    Private Function IsUpgradeCurrentVersionWithUgradableVersionFSVM(VehicleId As String, PersonId As String, CurrentFSVMFirmwareVersion As String)
        Dim requestJson As String = ""
        Dim resultUpgradable As String = ""
        Try

            'Dim data = [String].Empty
            'Using inputStream = New StreamReader(context.Request.InputStream)
            'Data = inputStream.ReadToEnd()
            'requestJson = data

            'log.Info("IsUpgradeCurrentVersionWithUgradableVersionFSVM : " & data)

            'Dim javaScriptSerializer = New JavaScriptSerializer()

            'Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(IsUpgradeCurrentVersionWithUgradableVersionFSVMMaster))

            'Dim VehicleId = DirectCast(serJsonDetails, IsUpgradeCurrentVersionWithUgradableVersionFSVMMaster).VehicleId
            'Dim PersonId = DirectCast(serJsonDetails, IsUpgradeCurrentVersionWithUgradableVersionFSVMMaster).PersonId

            'Dim ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj = New IsUpgradeCurrentVersionWithUgradableVersionFSVMResponse()
            'Dim json As String

            log.Debug("VehicleId : " & VehicleId)
            log.Debug("With PersonId : " & PersonId)
            log.Debug("CurrentFSVMFirmwareVersion: " & CurrentFSVMFirmwareVersion)
            OBJMasterBAL = New MasterBAL()
            If (VehicleId <> "") Then
                If (PersonId <> "") Then
                    'check is upgradable or not
                    OBJMasterBAL = New MasterBAL()
                    Dim dsUpgrade As DataSet
                    dsUpgrade = OBJMasterBAL.FSVMcheckLaunchedAndExistedVersionAndUpdate(VehicleId, CurrentFSVMFirmwareVersion.Trim(), PersonId, 0)

                    If dsUpgrade IsNot Nothing Then
                        If dsUpgrade.Tables(0) IsNot Nothing Then
                            If dsUpgrade.Tables(0).Rows.Count > 0 Then
                                resultUpgradable = dsUpgrade.Tables(0).Rows(0)("resultUpgradable")
                            Else
                                resultUpgradable = "N"
                            End If
                        Else
                            resultUpgradable = "N"
                        End If
                    Else
                        resultUpgradable = "N"
                    End If
                    'ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj.ResponceMessage = "success"
                    'ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj.ResponceText = resultUpgradable
                Else
                    'ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj.ResponceMessage = "fail"
                    'ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj.ResponceText = "Error occurred during Update:PersonId blank !"
                    resultUpgradable = "N"
                End If
            Else
                'ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj.ResponceMessage = "fail"
                'ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj.ResponceText = "Error occurred during Update:VehicleId blank !"
                resultUpgradable = "N"
            End If

            'json = JavaScriptSerializer.Serialize(ISUpgradeCurrentVersionWithUgradableVersionFSVMResponseObj)
            'context.Response.Write(json)

            'End Using

        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            log.Error("Exception occurred while prcessing request in IsUpgradeCurrentVersionWithUgradableVersionFSVM. Exception is :" & ex.Message & " . Details : " & requestJson)

            'ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))
            resultUpgradable = "N"
        End Try
        Return resultUpgradable
    End Function

    Private Function GetLaunchedFSVMFirmwareDetails(FAVehicleAuthorizationMasterResponseObj As FAVehicleAuthorizationMasterResponse)
        Dim requestJson As String = ""
        Dim rootOject = New FSVMUpgradeFileObject()
        Try
            'Dim data = [String].Empty
            'Using inputStream = New StreamReader(context.Request.InputStream)
            'data = inputStream.ReadToEnd()
            'requestJson = Data

            'log.Info("GetLaunchedFSVMFirmwareDetails : " & data)

            'Dim javaScriptSerializer = New JavaScriptSerializer()
            'Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(IsUpgradeCurrentVersionWithUgradableVersionFSVMMaster))
            'Dim VehicleId = DirectCast(serJsonDetails, IsUpgradeCurrentVersionWithUgradableVersionFSVMMaster).VehicleId

            Dim dtFSVMFirmwares As DataTable = New DataTable()
            dtFSVMFirmwares = OBJMasterBAL.GetLaunchedFSVMFirmwareDetails()
            Dim FSVMFirmwareVersion As String = ""
            Dim FilePath As String = ""
            Dim PIC As String = ""
            Dim ESP32 As String = ""

            If (Not dtFSVMFirmwares Is Nothing) Then
                If (dtFSVMFirmwares.Rows.Count > 0) Then
                    FSVMFirmwareVersion = dtFSVMFirmwares.Rows(0)("Version")
                    FilePath = "" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority & dtFSVMFirmwares.Rows(0)("FSVMFirmwareFilePath")
                    If dtFSVMFirmwares.Rows(0)("FSVMFileType").ToString() = "1" Then
                        PIC = "Y"
                        ESP32 = "N"
                    Else
                        PIC = "N"
                        ESP32 = "Y"
                    End If
                End If
            End If

            'Dim json As String

            If FilePath <> "" Then
                'rootOject.ResponceData.FilePath = FilePath
                'rootOject.ResponceData.FSVMFirmwareVersion = FSVMFirmwareVersion
                'rootOject.ResponceMessage = "success"
                'rootOject.ResponceText = ""
                FAVehicleAuthorizationMasterResponseObj.FilePath = FilePath
                FAVehicleAuthorizationMasterResponseObj.FSVMFirmwareVersion = FSVMFirmwareVersion
                FAVehicleAuthorizationMasterResponseObj.PIC = PIC
                FAVehicleAuthorizationMasterResponseObj.ESP32 = ESP32
            Else
                'rootOject.ResponceData.FilePath = FilePath
                'rootOject.ResponceData.FSVMFirmwareVersion = FSVMFirmwareVersion
                'rootOject.ResponceMessage = "fail"
                'rootOject.ResponceText = "Error occurred during geting File."
                FAVehicleAuthorizationMasterResponseObj.FilePath = ""
                FAVehicleAuthorizationMasterResponseObj.FSVMFirmwareVersion = ""
                FAVehicleAuthorizationMasterResponseObj.PIC = ""
                FAVehicleAuthorizationMasterResponseObj.ESP32 = ""
            End If


            'json = javaScriptSerializer.Serialize(rootOject)
            'log.Error("SSIDdsToJson Data : " & json)

            'context.Response.Write(json)
            'End Using
        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")

            log.Error("Exception occurred while prcessing request in GetLaunchedFSVMFirmwareDetails. Exception is :" & ex.Message & " . Details : " & requestJson)

            'ErrorInAuthontication(context, "fail", resourceManager.GetString("HandlerMsg15"))

            FAVehicleAuthorizationMasterResponseObj.FilePath = ""
            FAVehicleAuthorizationMasterResponseObj.FSVMFirmwareVersion = ""

        End Try
        Return FAVehicleAuthorizationMasterResponseObj
    End Function

    Private Function IsUpgradeCurrentVersionWithUgradableVersionFSNP(SiteId As String, PersonId As String, CurrentFSVMFirmwareVersion As String)
        Dim requestJson As String = ""
        Dim resultUpgradable As String = ""
        Try

            log.Debug("SiteId : " & SiteId)
            log.Debug("With PersonId : " & PersonId)
            log.Debug("CurrentFSVMFirmwareVersion: " & CurrentFSVMFirmwareVersion)
            OBJMasterBAL = New MasterBAL()
            If (SiteId <> "") Then
                If (PersonId <> "") Then
                    'check is upgradable or not
                    OBJMasterBAL = New MasterBAL()
                    Dim dsUpgrade As DataSet
                    dsUpgrade = OBJMasterBAL.CheckLaunchedAndExistedFSNPVersionAndUpdate(SiteId, CurrentFSVMFirmwareVersion.Trim(), PersonId, 0)

                    If dsUpgrade IsNot Nothing Then
                        If dsUpgrade.Tables(0) IsNot Nothing Then
                            If dsUpgrade.Tables(0).Rows.Count > 0 Then
                                resultUpgradable = dsUpgrade.Tables(0).Rows(0)("resultUpgradable")
                            Else
                                resultUpgradable = "N"
                            End If
                        Else
                            resultUpgradable = "N"
                        End If
                    Else
                        resultUpgradable = "N"
                    End If
                Else
                    resultUpgradable = "N"
                End If
            Else
                resultUpgradable = "N"
            End If
        Catch ex As Exception

            log.Error("Exception occurred while prcessing request in IsUpgradeCurrentVersionWithUgradableVersionFSNP. Exception is :" & ex.Message & " . Details : " & requestJson)

            resultUpgradable = "N"
        End Try
        Return resultUpgradable
    End Function

    Private Function GetLaunchedFSNPFirmwareDetails(rootOject As CheckAndValidateFSNPDetailResponse)
        Dim requestJson As String = ""

        Try


            Dim dtFSNPFirmwares As DataTable = New DataTable()
            dtFSNPFirmwares = OBJMasterBAL.GetLaunchedFSNPFirmwareDetails()
            Dim FSNPFirmwareVersion As String = ""
            Dim FilePath As String = ""


            If (Not dtFSNPFirmwares Is Nothing) Then
                If (dtFSNPFirmwares.Rows.Count > 0) Then
                    FSNPFirmwareVersion = dtFSNPFirmwares.Rows(0)("Version")
                    FilePath = "" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority & dtFSNPFirmwares.Rows(0)("FSNPFirmwareFilePath")
                End If
            End If

            If FilePath <> "" Then

                rootOject.FilePath = FilePath
                rootOject.FirmwareVersion = FSNPFirmwareVersion
            Else
                rootOject.FilePath = ""
                rootOject.FirmwareVersion = ""
            End If

            log.Debug("FilePath: " & FilePath & " FSNPFirmwareVersion: " & FSNPFirmwareVersion)

        Catch ex As Exception
            log.Error("Exception occurred while prcessing request in GetLaunchedFSNPFirmwareDetails. Exception is :" & ex.Message & " . Details : " & requestJson)
            rootOject.FilePath = ""
            rootOject.FirmwareVersion = ""
        End Try
        Return rootOject
    End Function

#Region "Activity logs"

    Private Function CreateDataSaveTankMonitorReading(TankInventoryId As Integer, CreateDataFor As String) As String
        Try

            Dim createDataFrom() = CreateDataFor.Split(";")

            Dim data As String = "TankInventoryId = " & TankInventoryId & " ; " &
                                    "Tank Number = " & createDataFrom(0) & " ; " &
                                    "ENTRY_TYPE = " & createDataFrom(1) & " ; " &
                                    "InventoryDateTime = " & createDataFrom(2) & " ; " &
                                    "Quantity = " & createDataFrom(3) & " ; " &
                                    "DateType = " & createDataFrom(4) & " ; " &
                                    "CompanyId = " & createDataFrom(5) & " ; " &
                                    "UserId = " & createDataFrom(6) & " ; " &
                                    "EndDateForRD = " & createDataFrom(7) & " ; " &
                                    "FluidLink = " & createDataFrom(8) & " ; " &
                                    "ReadingDateTime = " & createDataFrom(9) & " ; " &
                                    "ProbeReading = " & createDataFrom(10) & " ; " &
                                    "FromSiteId = " & createDataFrom(11) & " ; " &
                                    "TLD = " & createDataFrom(12) & " ; " &
                                    "RecordType = " & createDataFrom(13) & " ; "
            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateDataSaveTankMonitorReading. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Function CreateDataSaveInventoryVeederTankMonitorReading(VR_InventoryId As Integer, CreateDataFor As String) As String
        Try

            Dim createDataFrom() = CreateDataFor.Split(";")

            Dim data As String = "VR_InventoryId = " & VR_InventoryId & " ; " &
                                    "Phone Number = " & createDataFrom(0) & " ; " &
                                    "HubId = " & createDataFrom(1) & " ; " &
                                    "VeederRootMacAddress = " & createDataFrom(2) & " ; " &
                                    "AppDateTime = " & createDataFrom(3) & " ; " &
                                    "Tank Number = " & createDataFrom(4) & " ; " &
                                    "VR DateTime = " & createDataFrom(5) & " ; " &
                                    "Product Code = " & createDataFrom(6) & " ; " &
                                    "Tank Status = " & createDataFrom(7) & " ; " &
                                    "Volume = " & createDataFrom(8) & " ; " &
                                    "TC Volume = " & createDataFrom(9) & " ; " &
                                    "Ullage = " & createDataFrom(10) & " ; " &
                                    "Height = " & createDataFrom(11) & " ; " &
                                    "Water = " & createDataFrom(12) & " ; " &
                                    "Temperature = " & createDataFrom(13) & " ; " &
                                    "Water Volume = " & createDataFrom(14) & " ; " &
                                    "UserId = " & createDataFrom(15) & " ; " &
                                    "CompanyId = " & createDataFrom(16) & " "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Function CreateDataSaveDeliveryVeederTankMonitorReading(VR_DeliveryId As Integer, CreateDataFor As String) As String
        Try

            Dim createDataFrom() = CreateDataFor.Split(";")

            Dim data As String = "VR_DeliveryId = " & VR_DeliveryId & " ; " &
                                    "Phone Number = " & createDataFrom(0) & " ; " &
                                    "HubId = " & createDataFrom(1) & " ; " &
                                    "VeederRootMacAddress = " & createDataFrom(2) & " ; " &
                                    "AppDateTime = " & createDataFrom(3) & " ; " &
                                    "Tank Number = " & createDataFrom(4) & " ; " &
                                    "VR DateTime = " & createDataFrom(5) & " ; " &
                                    "Product Code = " & createDataFrom(6) & " ; " &
                                    "Start Date Time = " & createDataFrom(7) & " ; " &
                                    "End Date Time = " & createDataFrom(8) & " ; " &
                                    "Start Volume = " & createDataFrom(9) & " ; " &
                                    "Start TC Volume = " & createDataFrom(10) & " ; " &
                                    "Start Water = " & createDataFrom(11) & " ; " &
                                    "Start Temp = " & createDataFrom(12) & " ; " &
                                    "End Volume = " & createDataFrom(13) & " ; " &
                                    "End TC Volume = " & createDataFrom(14) & " ; " &
                                    "End Water = " & createDataFrom(15) & " ; " &
                                    "End Temp = " & createDataFrom(16) & " ; " &
                                    "Start Height = " & createDataFrom(17) & " ; " &
                                    "End Height = " & createDataFrom(18) & " ; " &
                                    "UserId = " & createDataFrom(19) & " ; " &
                                    "CompanyId = " & createDataFrom(20) & " "
            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Function CreateDataForVINAuthorization(VINAuthorizationID As Integer, CreateDataFor As String, Odometer As String) As String
        Try

            Dim createDataFrom() = CreateDataFor.Split(";")

            Dim data As String = "VINAuthorizationID = " & VINAuthorizationID & " ; " &
                                    "TransactionId = " & createDataFrom(0) & " ; " &
                                    "customerId = " & createDataFrom(1) & " ; " &
                                    "RPM = " & createDataFrom(2) & " ; " &
                                    "SPD = " & createDataFrom(3) & " ; " &
                                    "MIL = " & createDataFrom(4) & " ; " &
                                    "PC = " & createDataFrom(5) & " ; " &
                                    "ODOK = " & Odometer & " & " &
                                    "PID1 = " & createDataFrom(6) & " ; " &
                                    "PID2 = " & createDataFrom(7) & " ; " &
                                    "PID3 = " & createDataFrom(8) & " ; " &
                                    "PID4 = " & createDataFrom(9) & " ; " &
                                    "PID5 = " & createDataFrom(10) & " ; " &
                                    "PID6 = " & createDataFrom(11) & " ; " &
                                    "PID7 = " & createDataFrom(12) & " ; " &
                                    "PID8 = " & createDataFrom(13) & " ; " &
                                    "PID9 = " & createDataFrom(14) & " ; " &
                                    "PID10 = " & createDataFrom(15) & " ; " &
                                    "PID11 = " & createDataFrom(16) & " ; " &
                                    "PID12 = " & createDataFrom(17) & " ; " &
                                    "PID13 = " & createDataFrom(18) & " ; " &
                                    "PID14 = " & createDataFrom(19) & " ; " &
                                    "PID15 = " & createDataFrom(20) & " ; " &
                                    "PID16 = " & createDataFrom(21) & " ; " &
                                    "PID17 = " & createDataFrom(22) & " ; " &
                                    "PID18 = " & createDataFrom(23) & " ; " &
                                    "PID19 = " & createDataFrom(19) & " ; " &
                                    "PID20 = " & createDataFrom(24) & " "


            Return data

        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function



#End Region

    Private Sub SendLinkDefectiveEmail(emailTo As String, LinkName As String, numberzero As Integer, CompanyName As String)
        Try

            Dim body As String = String.Empty

            Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/LinkDefectiveEmail.txt"))
                body = sr.ReadToEnd()
            End Using

            '------------------
            body = body.Replace("CustomerEmail", emailTo)
            body = body.Replace("LinkName", LinkName)
            body = body.Replace("numberzero", numberzero)
            body = body.Replace("CompanyName", CompanyName)


            Dim e As New EmailService()


            Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))


            mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
            mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

            Dim messageSend As New MailMessage()
            messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
            messageSend.[To].Add(New MailAddress(emailTo))


            messageSend.Subject = "Defective link " & CompanyName & "-" & LinkName

            messageSend.Body = body

            messageSend.IsBodyHtml = True
            mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))

            'log.Debug("body:  " & body)

            Try
                mailClient.Send(messageSend)
            Catch ex As Exception
                log.Debug("Exception occurred in sending Link Defective emails to EmailId : " & emailTo & ". ex is :" & ex.Message)
            End Try

        Catch ex As Exception
            log.Debug("Exception occurred in sending Link Defective emails to EmailId : " & emailTo & ". ex is :" & ex.Message)
        End Try

    End Sub

    Private Sub CheckPersonFobOnly(context As HttpContext, IMEI As String)
        steps = "CheckPersonFobOnly 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("CheckPersonFobOnly :" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(AuthorizationSequenceModel))
            Dim IMEI_UDID = DirectCast(serJsonDetails, AuthorizationSequenceModel).IMEIUDID
            Dim PersonnelPIN = DirectCast(serJsonDetails, AuthorizationSequenceModel).PersonnelPIN
            Dim FOBNumber = DirectCast(serJsonDetails, AuthorizationSequenceModel).FOBNumber

            If (Not PersonnelPIN Is Nothing) Then
                PersonnelPIN = PersonnelPIN.Trim()
            Else
                PersonnelPIN = ""
            End If

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI_UDID)
            steps = "CheckPersonFobOnly 2"
            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        dtMain = dsIMEI.Tables(0)

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "CheckPersonFobOnly 3"

                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            log.Debug("personId:" & personId)
                            Dim customerId As Integer
                            customerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            Dim RoleId As String
                            RoleId = dt.Rows(0)("RoleId").ToString()

                            steps = "CheckPersonFobOnly 5"

                            Dim dtPerson As DataTable = New DataTable()
                            If (PersonnelPIN.Trim() <> "") Then
                                dtPerson = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And LTRIM(RTRIM(ANU.PinNumber)) ='" & PersonnelPIN.Trim() & "'", personId, RoleId)
                            Else
                                dtPerson = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','')='" & FOBNumber.ToString().Replace(" ", "") & "'", personId, RoleId)

                                If dtPerson Is Nothing Then 'Fob number not exists
                                    log.Debug("ProcessRequest: CheckPersonFobOnly - FOBNumber not assigned. IMEI_UDID=" & IMEI_UDID & " FOBNumber=" & FOBNumber.ToString().Replace(" ", ""))
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg46"), "invalidfob", "Yes")
                                    Return
                                ElseIf dtPerson.Rows.Count = 0 Then ''Fob number not exists
                                    log.Debug("ProcessRequest: CheckPersonFobOnly - FOBNumber not assigned. IMEI_UDID=" & IMEI_UDID & " FOBNumber=" & FOBNumber.ToString().Replace(" ", ""))
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg46"), "invalidfob", "Yes")
                                    Return
                                End If
                            End If

                            If Not dtPerson Is Nothing Then
                                log.Debug("CheckPersonFobOnly: In dtPerson")
                                If dtPerson.Rows.Count <> 0 Then

                                    If (FOBNumber <> "") Then
                                        If (dtPerson.Rows(0)("FOBNumber").ToString().Replace(" ", "") <> "" And dtPerson.Rows(0)("FOBNumber").ToString().ToLower().Replace(" ", "") <> FOBNumber.ToLower().Replace(" ", "")) Then
                                            log.Debug("ProcessRequest: CheckPersonFobOnly - Pin is already registered with another Fob/Card Number. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", "") & " Personnel PIN=" & PersonnelPIN.Trim())
                                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg47"), "invalidfob")
                                            Return
                                        End If

                                        'One Fob/Card will not be acceptable for vehicle and personnel screens.
                                        Dim dtPersonTemp As DataTable = New DataTable()
                                        dtPersonTemp = OBJMasterBAL.GetPersonnelByNameAndNumberAndEmail(" and ANU.CustomerId=" & customerId & "  And REPLACE(ANU.FOBNumber,' ','')='" & FOBNumber.ToString().Replace(" ", "") & "'", personId, RoleId)
                                        If (dtPersonTemp.Rows.Count = 0) Then
                                            Dim dtVehicleForFobNumber As DataTable = OBJMasterBAL.GetVehicleByCondition(" and V.CustomerId=" & customerId & "  And REPLACE(V.FobNumber,' ','') ='" & FOBNumber.Replace(" ", "") & "'", personId, RoleId)
                                            If (dtVehicleForFobNumber.Rows.Count > 0) Then
                                                log.Debug("ProcessRequest: CheckPersonFobOnly - Fob/Card number is already registered with vehicle. IMEI_UDID=" & IMEI_UDID & " Fob/Card Number=" & FOBNumber.Replace(" ", ""))
                                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg43"), "invalidfob")
                                                Return
                                            End If
                                        End If
                                        'One Fob/Card will not be acceptable for vehicle and personnel screens.

                                    End If

                                    Dim checkRequireOdoResponse = New CheckRequireOdoResponse()
                                    checkRequireOdoResponse.ResponceMessage = "success"
                                    checkRequireOdoResponse.ResponceText = resourceManager.GetString("HandlerMsg49")
                                    checkRequireOdoResponse.ValidationFailFor = ""
                                    checkRequireOdoResponse.FOBNumber = dtPerson.Rows(0)("FOBNumber").ToString().Replace(" ", "")
                                    checkRequireOdoResponse.PersonPin = dtPerson.Rows(0)("PinNumber").ToString().Trim()
                                    'check current version with launch version


                                    If (FOBNumber <> "") Then
                                        OBJMasterBAL.AssignedFOBNumberToPerson(PersonnelPIN.Trim(), FOBNumber.Replace(" ", ""), customerId)
                                        Try
                                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                Dim writtenData As String = "PersonnelPIN = " & PersonnelPIN.Trim() & " ; " & "FOBNumber= " & FOBNumber.Replace(" ", "") & ";" & "CustomerId=" & customerId
                                                OBJServiceBAL = New WebServiceBAL()
                                                CSCommonHelper.WriteLog("Added", "AssignedFOBNumberToPerson", "", writtenData, dsIMEI.Tables(0).Rows(0)("PersonName") & "(" & dsIMEI.Tables(0).Rows(0)("Email") & ")", IPAddress, "success", "")
                                            End If
                                        Catch ex As Exception
                                            log.Error("Error occured in AssignedFOBNumberToPerson. Exception is : " & ex.Message)
                                        End Try
                                    End If

                                    Dim json As String
                                    json = javaScriptSerializer.Serialize(checkRequireOdoResponse)

                                    context.Response.Write(json)

                                Else
                                    log.Debug("ProcessRequest: CheckPersonFobOnly- 2 Invalid Pin. Please contact administrator. IMEI_UDID=" & IMEI_UDID & ". Pin" & PersonnelPIN.Trim())
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg18"), "PIN")
                                End If
                            Else
                                log.Debug("ProcessRequest: CheckPersonFobOnly- 3 Invalid Pin. Please contact administrator. IMEI_UDID=" & IMEI_UDID & ". Pin" & PersonnelPIN.Trim())
                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg18"), "PIN")
                            End If

                        Else
                            log.Debug("ProcessRequest: CheckPersonFobOnly- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI_UDID)
                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: CheckPersonFobOnly- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI_UDID)
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: CheckPersonFobOnly- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: CheckPersonFobOnly- IMEI_UDID does not exist for IMEI_UDID=" & IMEI_UDID)
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using
    End Sub

    Private Sub GetVehicleByFSTagMacAddress(context As HttpContext, IMEI As String)
        steps = "GetVehicleByFSTagMacAddress 1"
        Dim data = [String].Empty
        Using inputStream = New StreamReader(context.Request.InputStream)
            data = inputStream.ReadToEnd()
            log.Debug("GetVehicleByFSTagMacAddress :" & data)
            Dim javaScriptSerializer = New JavaScriptSerializer()
            Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(CheckAndValidateFSNPDetail))
            Dim FSTagMacAddress = DirectCast(serJsonDetails, CheckAndValidateFSNPDetail).FSTagMacAddress

            Dim dsIMEI = New DataSet()
            dsIMEI = OBJServiceBAL.IsIMEIExists(IMEI)
            steps = "GetVehicleByFSTagMacAddress 2"
            If Not dsIMEI Is Nothing Then   'IMEI_UDID not exists
                If Not dsIMEI.Tables(0) Is Nothing Then
                    If dsIMEI.Tables(0).Rows.Count <> 0 Then

                        Dim dtMain As DataTable = New DataTable()
                        dtMain = dsIMEI.Tables(0)

                        If dtMain.Rows(0)("IsApproved").ToString() = "True" Then
                            steps = "GetVehicleByFSTagMacAddress 3"

                            OBJMasterBAL = New MasterBAL()
                            Dim dt As DataTable = dtMain
                            Dim personId As Integer
                            personId = Integer.Parse(dt.Rows(0)("PersonId").ToString())
                            log.Debug("personId:" & personId)
                            Dim customerId As Integer
                            customerId = Integer.Parse(dt.Rows(0)("CustomerId").ToString())
                            Dim RoleId As String
                            RoleId = dt.Rows(0)("RoleId").ToString()

                            steps = "GetVehicleByFSTagMacAddress 5"

                            Dim dtVehicle = New DataTable()

                            dtVehicle = OBJMasterBAL.GetVehicleByCondition(" and rtrim(ltrim(V.FSTagMacAddress)) = '" & FSTagMacAddress.TrimStart().TrimEnd() & "' ", personId, RoleId)

                            If Not dtVehicle Is Nothing Then 'Authorized vehicle?-Vehicle number not exists
                                log.Debug("GetVehicleByFSTagMacAddress: In dtVehicle")
                                If dtVehicle.Rows.Count <> 0 Then ''Authorized vehicle?-Vehicle number not exists

                                    Dim VehicleNumber As String
                                    VehicleNumber = dtVehicle.Rows(0)("VehicleNumber").ToString().Trim()
                                    steps = "GetVehicleByFSTagMacAddress 6 " & VehicleNumber.Trim()

                                    Dim checkRequireOdoResponse = New CheckRequireOdoResponse()
                                    checkRequireOdoResponse.ResponceMessage = "success"
                                    checkRequireOdoResponse.ResponceText = "success"
                                    checkRequireOdoResponse.VehicleNumber = VehicleNumber.Trim()
                                    'check current version with launch version

                                    Dim json As String
                                    json = javaScriptSerializer.Serialize(checkRequireOdoResponse)

                                    context.Response.Write(json)

                                Else
                                    log.Debug("ProcessRequest: CheckVehicleFobOnly- 2 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI)
                                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg45"), "Vehicle")
                                    ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized vehicle. Please contact administrator.", "Vehicle")
                                End If
                            Else
                                log.Debug("ProcessRequest: CheckVehicleFobOnly- 3 Unauthorized fuel vehicle for IMEI_UDID=" & IMEI)
                                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg25"), "Vehicle")
                                ''ErrorInRequireOdoVehicle(context, "fail", "Unauthorized vehicle, Please contact administrator.", "Vehicle")
                            End If

                        Else
                            log.Debug("ProcessRequest: CheckVehicleFobOnly- Your registration request not approved. Please contact administrator. IMEI_UDID=" & IMEI)
                            ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg28"))
                        End If
                    Else
                        log.Debug("ProcessRequest: CheckVehicleFobOnly- Mobile is not registered in the system. Please contact administrator for IMEI_UDID=" & IMEI)
                        ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                    End If

                Else
                    log.Debug("ProcessRequest: CheckVehicleFobOnly- IMEI_UDID does not exist for IMEI_UDID=" & IMEI)
                    ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
                End If
            Else
                log.Debug("ProcessRequest: CheckVehicleFobOnly- IMEI_UDID does not exist for IMEI_UDID=" & IMEI)
                ErrorInRequireOdoVehicle(context, "fail", resourceManager.GetString("HandlerMsg29"))
            End If

        End Using
    End Sub

End Class



