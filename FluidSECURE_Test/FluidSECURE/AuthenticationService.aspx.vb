Imports System.Web.Script.Serialization
Imports System.Net
Imports System.IO
Imports log4net
Imports log4net.Config

Public Class AuthonticationService
	Inherits System.Web.UI.Page

	Dim OBJMaster As MasterBAL = New MasterBAL()
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllTankCharts))

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            GetToken()
            ExportTransactions()
            'Dim a As MasterBAL = New MasterBAL()
            'Dim b As WebServiceBAL = New WebServiceBAL()
            'b.GetSSIDbySiteId("23")
            'XmlConfigurator.Configure()

            'Dim dtTankChart As DataTable = New DataTable()
            'dtTankChart = OBJMaster.GetTankChartsByCondition("", Session("PersonId").ToString(), Session("RoleId").ToString())
            'For Each dr As DataRow In dtTankChart.Rows
            '	SaveCoefficient(dr("TankChartId"))
            'Next

            'Dim lsb_hex As String = Hex(0)
            'Dim msb_hex As String = Hex(0)
            'Dim Combine_hex As String = msb_hex & lsb_hex
            'Dim tempProbeReading = Convert.ToInt64(Combine_hex, 16) 'CInt("&H" & Combine_hex)
            'Dim calculatedProbeReading As Decimal = 0
            'If ("" = "159") Then
            '    calculatedProbeReading = tempProbeReading * 0.0393700787 'convert mm to inch
            'Else
            '    calculatedProbeReading = tempProbeReading / 128
            'End If

            'Dim a As Decimal = Math.Round(calculatedProbeReading, 1)


            'a.AssignedFOBNumberToPerson("1", "65 35 F0 1C 90 00")
            'Dim a As ExternalBAL = New ExternalBAL()
            'Dim dt As DataTable = New DataTable()
            'dt = a.GetTransactionById()
            'GetSites()
            '    Dim ht As HandlerTrak = New HandlerTrak()
            '    ht.GetLocationAddress(18.5147055, 73.7806059)

            '    Dim WifiSSId As String = "Bolong's TEST Unit"

            '    'Dim OBJMasterBAL As MasterBAL = New MasterBAL()
            '    'Dim dtHose = OBJMasterBAL.GetHoseByCondition(" And h.WifiSSId ='" & WifiSSId.Replace("'", "''") & "' and s.SiteID =" & 7 & "", 25, "11df27ed-8d70-46a9-a925-7150326ffe75")
            '    Dim OBJMasterBAL = New MasterBAL()
            '    Dim dtFirmwares As DataTable = New DataTable()
            '    dtFirmwares = OBJMasterBAL.GetLaunchedFirmwareDetails()
            '    Dim FirmwareVersion As String = ""
            '    Dim FilePath As String = ""

            '    If (Not dtFirmwares Is Nothing) Then
            '        If (Not dtFirmwares.Rows.Count > 0) Then
            '            FirmwareVersion = dtFirmwares.Rows(0)("Version")
            '            FilePath = dtFirmwares.Rows(0)("FirmwareFilePath")
            '        End If
            '    End If
            testForAuthorizationSequence()
        End If

    End Sub


	Private Sub GetSites()
		OBJMaster = New MasterBAL()
		Dim dtSiteNames As DataTable = New DataTable()
		'dtSiteNames = OBJMaster.GetSiteList(75, "52d3b8e6-95e6-44ac-b7bf-0be3acb56ff5")
		'dtSiteNames = OBJMaster.GetSiteList(48, "936965f9-b2bc-486c-af9a-df1025bb2966")

		ddlSiteName.DataSource = dtSiteNames
		ddlSiteName.DataTextField = "SiteName"
		ddlSiteName.DataValueField = "SiteID"
		ddlSiteName.DataBind()
		ddlSiteName.Items.Insert(0, New ListItem("Select Site", "0"))


		ddlSiteId.DataSource = dtSiteNames
		ddlSiteId.DataTextField = "SiteName"
		ddlSiteId.DataValueField = "SiteID"
		ddlSiteId.DataBind()
		ddlSiteId.Items.Insert(0, New ListItem("Select Site", "0"))

	End Sub

	Protected Sub btnSendRequest_Click(sender As Object, e As EventArgs) Handles btnSendRequest.Click


		testForAuthorizationSequence()
	End Sub

	Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
		testForLogin()
	End Sub


	Public Function loginAuthTokenGeneratorForAuthorizationAndTransaction(flag As Integer) As String

		'Dim userPass As [String] = "C018B218-CF44-458F-976A-1059905C6F51" + ":" + "Bbb@trakeng.com"
		'Dim userPass As [String] = "358187076053951" + ":" + "ashwini_kad@vaspsolutions.com"
		'Dim userPass As [String] = "D3B6FACE-0D2D-48CA-9975-F77C4DE65E8E" + ":" + "Rico@gmail.com"
		'Dim userPass As [String] = "867371029053490" + ":" + "mj@vasp.com"
		Dim userPass As [String] = "77F19F06-A7BD-407C-A4F5-6A225F5BF57C:Amey@gmail.com"
		If flag = 0 Then
			Dim SecretKey As [String] = userPass + ":" + "Register"
			'Dim SecretKey As [String] = userPass + ":" + "GetSSID"
			'Dim SecretKey As [String] = userPass + ":" + "AndroidSSID"
			Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
			Return AuthBas64String
		ElseIf flag = 1 Then
			Dim SecretKey As [String] = userPass + ":" + "Other"
			Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
			Return AuthBas64String
		ElseIf flag = 3 Then
			Dim SecretKey As [String] = userPass + ":" + "AuthorizationSequence"
			Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
			Return AuthBas64String
		ElseIf flag = 4 Then
			Dim SecretKey As [String] = userPass + ":" + "SaveInventoryVeederTankMonitorReading"
			Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
			Return AuthBas64String
		ElseIf flag = 5 Then
			Dim SecretKey As [String] = userPass + ":" + "SaveDiagnosticLogs"
			Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
			Return AuthBas64String
		ElseIf flag = 6 Then
			Dim SecretKey As [String] = userPass + ":" + "SaveManualVehicleOdometer"
			Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
			Return AuthBas64String
		Else
			Dim SecretKey As [String] = userPass + ":" + "TransactionComplete"
			Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
			Return AuthBas64String
		End If

	End Function



	Public Sub testForLogin()
		lblResponseOfLogin.Text = ""
		Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:61118/LoginHandler.ashx"), HttpWebRequest)
		httpRequest.Method = "POST"
		httpRequest.ContentType = "application/x-www-form-urlencoded"

		Dim userPass As [String] = txtIMEINumber.Text + ":" + txtUserName.Text
		Dim SecretKey As [String] = userPass + ":" + txtPassword.Text
		Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)

		httpRequest.Headers.Add("Login", "Basic " + AuthBas64String)
		Dim encoding As New ASCIIEncoding()
		httpRequest.ContentLength = 0

		Dim httpWebResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
		Dim responseStream As New StreamReader(httpWebResponse.GetResponseStream())
		Dim strResponse As String = ""
		Do
			strResponse = responseStream.ReadLine()
		Loop While responseStream.EndOfStream = False
		lblResponce.Text = strResponse



	End Sub

	Public Sub testForAuthorizationSequence()
        lblResponce.Text = ""
        Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:61129/FluidSecureAPI.ashx"), HttpWebRequest)
        'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:61129/HandlerTrak.ashx"), HttpWebRequest)
        'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://104.237.195.193:1020/HandlerTrak.ashx"), HttpWebRequest)
        'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://103.8.126.241:88/HandlerTrak.ashx"), HttpWebRequest)
        'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://192.168.90.60:1020/HandlerTrak.ashx"), HttpWebRequest)
        httpRequest.Method = "POST"
		httpRequest.ContentType = "application/x-www-form-urlencoded"
		httpRequest.Headers.Add("Authorization", "Basic " + loginAuthTokenGeneratorForAuthorizationAndTransaction(6))
		Dim encoding As New ASCIIEncoding()

        'Dim datatopost As String = "WIFI VASP"
        ''Dim datatopost As String = "18.5143978581981 73.7801769889391"  '"18.514475,73.779348"
        'Dim datatopost As String = "yo yo#:#7458954#:#uih.hdj@vdh.com#:#454511#:#I"
        'Dim datatopost As String = "Authenticate:I:18.5150136,73.7823048"
        'Dim datatopost As String = "Fuel Secure Test#:#18.514486#:#73.7793633"
        'Dim datatopost As String = "Fuel Secure Test#:#18.514486#:#73.7793633"

        Dim authorizationSequenceModel As New TransactionsExportAPI()
        'authorizationSequenceModel.AppDateTime = "2018-05-15 18:51:55"
        'authorizationSequenceModel.Height = "1.4"
        authorizationSequenceModel.TransactionFromDate = "2018-01-01 00:00"
        authorizationSequenceModel.TransactionToDate = "2018-12-31 00:00"
        authorizationSequenceModel.CompanyName = "vaspsumedh"

        'authorizationSequenceModel.TCVolume = "1.2"
        'authorizationSequenceModel.TankNumber = "2"
        'authorizationSequenceModel.TankStatus = "FILL"
        'authorizationSequenceModel.Temperature = ""
        'authorizationSequenceModel.Ullage = "1.3"
        'authorizationSequenceModel.VRDateTime = "2018-05-15 18:51:55"
        'authorizationSequenceModel.VeederRootMacAddress = "123"
        'authorizationSequenceModel.Volume = 1.1
        'authorizationSequenceModel.Water = 1.5
        'authorizationSequenceModel.Temperature = 1.6
        'authorizationSequenceModel.WaterVolume = 1.7

        'Dim str As String
        'str = "Transtech 2nd FLR#:#18.514466#:#73.779387"

        Dim seri As New JavaScriptSerializer()
		'Dim jStr As String = seri.Serialize(Str)
		Dim jStr As String = seri.Serialize(authorizationSequenceModel)

		Dim bytedata As Byte() = encoding.GetBytes(jStr)

		httpRequest.ContentLength = bytedata.Length
		Dim requestStream As Stream = httpRequest.GetRequestStream()
		requestStream.Write(bytedata, 0, bytedata.Length)
		requestStream.Close()
		Dim httpWebResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
		Dim responseStream As New StreamReader(httpWebResponse.GetResponseStream())
		Dim strResponse As String = ""
		Do
			strResponse = responseStream.ReadLine()
		Loop While responseStream.EndOfStream = False
		lblResponce.Text = strResponse

	End Sub

	Public Sub testForTransactionComplete()
		lblTransactionResponce.Text = ""
		'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:61119/HandlerTrak.ashx"), HttpWebRequest)
		Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://103.8.126.241:89/HandlerTrak.ashx"), HttpWebRequest)
		'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://104.237.195.193:1020/HandlerTrak.ashx"), HttpWebRequest)
		httpRequest.Method = "POST"
		httpRequest.ContentType = "application/x-www-form-urlencoded"
		httpRequest.Headers.Add("Authorization", "Basic " + loginAuthTokenGeneratorForAuthorizationAndTransaction(4))
		Dim encoding As New ASCIIEncoding()

		Dim datatopost As String = "WIFI VASP"

		Dim transactionCompleteObj = New TransactionComplete()
		transactionCompleteObj.PersonId = Convert.ToInt32(1209)
		transactionCompleteObj.SiteId = Convert.ToInt32(2)
		transactionCompleteObj.VehicleId = Convert.ToInt32(35)
		transactionCompleteObj.CurrentOdometer = Convert.ToInt32(2500)
		transactionCompleteObj.FuelQuantity = Convert.ToInt32(4.18)
		transactionCompleteObj.FuelTypeId = Convert.ToInt32(115)
		transactionCompleteObj.PhoneNumber = "717-666-2222"
		transactionCompleteObj.TransactionDate = "4/27/2017 11:02:15 AM"
		transactionCompleteObj.TransactionFrom = "A"
		transactionCompleteObj.WifiSSId = "FUELSECURE1"

		Dim seri As New JavaScriptSerializer()
		'Dim jStr As String = seri.Serialize(authorizationSequenceModel)
		Dim jStr As String = seri.Serialize(transactionCompleteObj)
		Dim bytedata As Byte() = encoding.GetBytes(jStr)

		httpRequest.ContentLength = bytedata.Length
		Dim requestStream As Stream = httpRequest.GetRequestStream()
		requestStream.Write(bytedata, 0, bytedata.Length)
		requestStream.Close()
		Dim httpWebResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
		Dim responseStream As New StreamReader(httpWebResponse.GetResponseStream())
		Dim strResponse As String = ""
		Do
			strResponse = responseStream.ReadLine()
		Loop While responseStream.EndOfStream = False
		lblTransactionResponce.Text = strResponse

	End Sub

	Protected Sub btnClear_Click(sender As Object, e As EventArgs)
		lblResponce.Text = ""
		txtIMEIUDID.Text = ""
		txtOdoMeter.Text = ""
		txtVehicleNumber.Text = ""
		txtWifiSSId.Text = ""
		ddlSiteName.SelectedValue = "0"
	End Sub

	Protected Sub btnSendReqToTransaction_Click(sender As Object, e As EventArgs) Handles btnSendReqToTransaction.Click
		testForTransactionComplete()
	End Sub

	Protected Sub btnClearTransaction_Click(sender As Object, e As EventArgs)
		lblTransactionResponce.Text = ""
		ddlSiteId.SelectedValue = "0"
		txtPerson.Text = ""
		txtVehicleId.Text = ""
		txtCurrentOdoMeter.Text = ""
		txtFuelTypeId.Text = ""
		txtFuelQuantity.Text = ""
		txtPhoneNumber.Text = ""

	End Sub

	Protected Sub btnRegisteration_Click(sender As Object, e As EventArgs)
		Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:61120/HandlerTrak.ashx"), HttpWebRequest)
		httpRequest.Method = "POST"
		httpRequest.ContentType = "application/x-www-form-urlencoded"
		httpRequest.Headers.Add("Authorization", "Basic " + loginAuthTokenGeneratorForAuthorizationAndTransaction(0))
		Dim encoding As New ASCIIEncoding()

		'Dim datatopost As String = "WIFI VASP"

		'Dim registerObj = New Register()
		'registerObj.Name = "Rico"
		'registerObj.Mobile = "8529637411"
		'registerObj.Emailid = "Rico@gmail.com"
		'registerObj.IMEI_UDID = "D3B6FACE-0D2D-48CA-9975-F77C4DE65E8E"
		'registerObj.ReqFrom = "I"

		Dim seri As New JavaScriptSerializer()
		'Dim jStr As String = seri.Serialize(authorizationSequenceModel)
		Dim strdata As String
		'strdata = "amey#:#8529637411#:#useramey2@vasp.com#:#F5533A80-C411-480D-A80B-4C7C850E047C#:#I#:#SGS"
		strdata = " Rahul R. Shinde#:#745-623-5865#:#rsrahshi12@gmail.com#:#352116061162994#:#A#:#vasp"

		Dim jStr As String = seri.Serialize(strdata)
		Dim bytedata As Byte() = encoding.GetBytes(jStr)

		httpRequest.ContentLength = bytedata.Length
		Dim requestStream As Stream = httpRequest.GetRequestStream()
		requestStream.Write(bytedata, 0, bytedata.Length)
		requestStream.Close()
		Dim httpWebResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
		Dim responseStream As New StreamReader(httpWebResponse.GetResponseStream())
		Dim strResponse As String = ""
		Do
			strResponse = responseStream.ReadLine()
		Loop While responseStream.EndOfStream = False
		lblTransactionResponce.Text = strResponse

	End Sub

	Private Function SaveCoefficient(TankChartId As Integer) As Double()
		Dim coefficient(4) As Double
		Try

			'Implementation of Linest function
			OBJMaster = New MasterBAL()
			Dim dtTankChartDetail As DataTable = New DataTable()
			dtTankChartDetail = OBJMaster.GetTankChartDetailsByTankChartId(TankChartId)


			Dim datapointsx(dtTankChartDetail.Rows.Count - 1) As Double
			Dim datapointsy(dtTankChartDetail.Rows.Count - 1) As Double

			Dim i As Integer = 0
			For Each dr As DataRow In dtTankChartDetail.Rows
				datapointsx(i) = dr("IncrementLevel")
				datapointsy(i) = dr("GallonLevel")
				i = i + 1
			Next

			coefficient = MathNet.Numerics.Fit.Polynomial(datapointsx, datapointsy, 3, MathNet.Numerics.LinearRegression.DirectRegressionMethod.Svd)
			OBJMaster = New MasterBAL()
			OBJMaster.SaveCoefficients(TankChartId, coefficient(0), coefficient(1), coefficient(2), coefficient(3))
			'Dim xval As Double = "68"
			'Dim yval0 As Double = coefficient(0) + (coefficient(1) * xval) + (coefficient(2) * Math.Pow(xval, 2)) + coefficient(3) * Math.Pow(xval, 3)

		Catch ex As Exception
			log.Error("Error occurred in SaveCoefficient Exception is :" + ex.Message)
		End Try
		Return coefficient
	End Function


    Private Sub GetToken()

        Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:61129/token"), HttpWebRequest)
        httpRequest.Method = "POST"
        httpRequest.ContentType = "application/x-www-form-urlencoded"

        Dim encoding As New UTF8Encoding()
        Dim postData = "username=trakadmin@trak.com&password=Trak@123&grant_type=password"
        'Byte[] data = encoding.GetBytes(postData);

        Dim seri As New JavaScriptSerializer()
        Dim tokenOBJ = New tokenclass()
        tokenOBJ.username = "trakadmin@trak.com"
        tokenOBJ.password = "Trak@123"
        tokenOBJ.grant_type = "password"
        'New FormUrlEncodedContent(form)
        'Dim jStr As String = seri.Serialize("{""username"":""trakadmin@trak.com"",""password"":""Trak@123"",""grant_type"" : ""password""}")

        Dim bytedata As Byte() = encoding.GetBytes(postData)

        httpRequest.ContentLength = bytedata.Length
        Dim requestStream As Stream = httpRequest.GetRequestStream()
        requestStream.Write(bytedata, 0, bytedata.Length)
        requestStream.Close()
        Dim httpWebResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
        Dim responseStream As New StreamReader(httpWebResponse.GetResponseStream())
        Dim strResponse As String = ""
        Do
            strResponse = responseStream.ReadLine()
        Loop While responseStream.EndOfStream = False
        lblTransactionResponce.Text = strResponse

    End Sub

    Private Sub ExportTransactions()

        Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:61129/api/External/ExportTransactions"), HttpWebRequest)
        httpRequest.Method = "POST"
        httpRequest.ContentType = "application/x-www-form-urlencoded"
        httpRequest.Headers.Add("Authorization", "bearer Dhxb7ZNy6-zur8RSqWRp74OHMnGuSGtrURo1dHyu3jWi4YjEQjjeb6IAD9PNe87AkG_tJ6-xhm8QOy7gaZdq4TL1_etNLNG5IQMe-qInScIjbaZ3GoQOdxiNzdfjiIM0N6PpKlA5-srftfCSeEMFg81OVJDaY_s-hfaNkRu1HOQM4P9TLJyimfeS5yBY5sSmqHHEMyEv7M5cl_C9pOYSqJlqGmjWzil7bBvob5hBk2I")

        Dim encoding As New UTF8Encoding()
        Dim postData = "TransactionFromDate=2018-01-01 13:25&TransactionToDate=2018-01-31 23:59&CompanyName=vaspsumedh"

        Dim bytedata As Byte() = encoding.GetBytes(postData)

        httpRequest.ContentLength = bytedata.Length
        Dim requestStream As Stream = httpRequest.GetRequestStream()
        requestStream.Write(bytedata, 0, bytedata.Length)
        requestStream.Close()
        Dim httpWebResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
        Dim responseStream As New StreamReader(httpWebResponse.GetResponseStream())
        Dim strResponse As String = ""
        Do
            strResponse = responseStream.ReadLine()
        Loop While responseStream.EndOfStream = False
        lblTransactionResponce.Text = strResponse

    End Sub

End Class
Public Class tokenclass
    Public Property username() As String
        Get
            Return m_username
        End Get
        Set(value As String)
            m_username = value
        End Set
    End Property
    Private m_username As String
    Public Property password() As String
        Get
            Return m_password
        End Get
        Set(value As String)
            m_password = value
        End Set
    End Property
    Private m_password As String
    Public Property grant_type() As String
        Get
            Return m_grant_type
        End Get
        Set(value As String)
            m_grant_type = value
        End Set
    End Property
    Private m_grant_type As String

End Class
Public Class TransactionCompleteCheck1

	Public TransactionCompleteCheck()

	Public Property cmtxtnid_10_record() As String()
		Get
			Return m_cmtxtnid_10_record
		End Get
		Set(value As String())
			m_cmtxtnid_10_record = value
		End Set
	End Property
	Private m_cmtxtnid_10_record As String()
End Class