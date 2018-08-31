Imports System.Net
Imports System.IO
Imports System.Web.Script.Serialization

Public Class WebForm1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        test()



        'testForTransactionComplete()
    End Sub

    Public Function loginAuthTokenGenerator() As String

        Dim userPass As [String] = "F5533A80-B411-480D-A80B-4C7C850E047C" + ":" + "ask@sdf.ert"

        'Dim SecretKey As [String] = userPass + ":" + "AndroidSSID"

        'Dim SecretKey As [String] = userPass + ":" + "GetSSID"
        'Dim SecretKey As [String] = userPass + ":" + "Other"
        'Dim SecretKey As [String] = userPass + ":" + "AuthorizationSequence"
        'Dim SecretKey As [String] = userPass + ":" + "TransactionComplete"

        'Dim userPass As [String] = ":"
        Dim SecretKey As [String] = userPass + ":" + "Register"

        Dim AuthBas64String As [String] = CSCommonHelper.Base64Encode(SecretKey)
        Return AuthBas64String
    End Function

    

    

    Public Sub test()
        Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://localhost:51842/HandlerTrak.ashx"), HttpWebRequest)
        'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://104.237.195.193:1020/HandlerTrak.ashx"), HttpWebRequest)
        'Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create("http://192.168.90.60:1020/HandlerTrak.ashx"), HttpWebRequest)
        httpRequest.Method = "POST"
        httpRequest.ContentType = "application/x-www-form-urlencoded"
        httpRequest.Headers.Add("Authorization", "Basic " + loginAuthTokenGenerator())
        Dim encoding As New ASCIIEncoding()

        'Dim datatopost As String = "Transtech 2nd FLR"
        'Dim datatopost As String = "18.5130440319428,73.7815835645811" '"18.513119,73.781458"  '"18.514475,73.779348"
        Dim datatopost As String = "yo yo#:#7458954#:#ashish_patil@vaspsolutions.com#:#F1RACE-B411-480D-A80B-4C7C850E047d#:#I"
        'Dim datatopost As String = "Authenticate"


        Dim bytedata As Byte() = encoding.GetBytes(datatopost)

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
        'end test

    End Sub


End Class