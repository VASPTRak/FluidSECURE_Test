Imports System.Security.Cryptography
Imports System.IO
Imports log4net
Imports log4net.Config

Public NotInheritable Class CSCommonHelper
	'Public Shared Function Base64Decode(base64EncodedData As String) As String
	'End Function

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CSCommonHelper))

	Public Shared Function Base64Encode(plainText As String) As String
		Return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText))
	End Function
	'Public Shared Function GetBookmarkThumbURL(localurl As String, UserId As String) As String
	'End Function
	'Public Shared Function GetProfileURL(localurl As String, UserId As String, isThumb As Boolean) As String
	'End Function
	'Public Shared Function GetWebURL(localurl As String, UserId As String, isThumb As Boolean) As String
	'End Function
	'Public Shared Function MD5Encrypt(secret As String) As String
	'End Function


	Shared Function MD5Encrypt(theInput As String) As String

		Using hasher As MD5 = MD5.Create()    ' create hash object

			' Convert to byte array and get hash
			Dim dbytes As Byte() =
				 hasher.ComputeHash(Encoding.UTF8.GetBytes(theInput))

			' sb to create string from bytes
			Dim sBuilder As New StringBuilder()

			' convert byte data to hex string
			For n As Integer = 0 To dbytes.Length - 1
				sBuilder.Append(dbytes(n).ToString("X2"))
			Next n

			Return sBuilder.ToString()
		End Using

	End Function

	Shared Sub WriteLog(Action As String, Screen As String, beforeData As String, writtendata As String, loginPersonName As String, IPAddress As String, requestStatus As String, failureMessage As String)

		If (failureMessage.Contains("Thread was being aborted") = True) Then
			Exit Sub
		End If

		'delete log file before x days==> x=number of days
		Try
			Dim objmaster As MasterBAL = New MasterBAL()
			Dim result As Integer = objmaster.CheckAlreadyDeleted()

			If (result = 1) Then

				Dim days As Integer = ConfigurationManager.AppSettings("NumberOfDaysToDeleteActivityLog").ToString()

				Dim folderPathActivitiFiles As String = System.Web.Hosting.HostingEnvironment.MapPath("~/actvity_logs/")
				Dim directory As New IO.DirectoryInfo(folderPathActivitiFiles)

				For Each file As IO.FileInfo In directory.GetFiles
                    If (Now - file.LastWriteTime).Days > days Then
                        Try
                            file.Delete()
                        Catch ex As Exception
                            log.Error(String.Format("Error Occurred while Deleting activity logs 1. Error is {0}.", ex.Message))
                        End Try
                    End If
                Next

			End If

		Catch ex As Exception
			log.Error(String.Format("Error Occurred while Deleting activity logs. Error is {0}.", ex.Message))
		End Try


		Dim DailyActivityLog As ILog = LogManager.GetLogger("RollingLogFileAppenderForLogActivity")
		Dim ts = DateTime.Now.ToString("yyyy MM dd")
		GlobalContext.Properties("ts") = ts


		Try
			Dim fileName As String = ""
			fileName = DateTime.Now.ToString("yyyy MM dd") & " activity log.csv"

			Dim path As String = System.Web.Hosting.HostingEnvironment.MapPath("~/actvity_logs/" & fileName)
			Dim isExists As Boolean = False

			If (File.Exists(path)) Then
				isExists = True
			End If

			XmlConfigurator.Configure()

			If (isExists = False) Then
				DailyActivityLog.Info("Date,Screen,Action(Added/Modified/Deleted),User Name,Before,After,IP Address,Request Status,Failure Message")
				'Else
				'    writer.WriteLine("")
			End If


			If (Action = "Added") Then
				'writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") & "," & "" & Screen & "," & Action & "," & loginPersonName & ",," & writtendata & "," & IPAddress)
				DailyActivityLog.Info(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") & "," & "" & Screen & "," & Action & "," & loginPersonName & ",," & writtendata & "," & IPAddress & "," & requestStatus & "," & failureMessage)
			Else

				'writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") & "," & "" & Screen & "," & Action & "," & loginPersonName & "," & beforeData & "," & writtendata & "," & IPAddress)
				DailyActivityLog.Info(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") & "," & "" & Screen & "," & Action & "," & loginPersonName & "," & beforeData & "," & writtendata & "," & IPAddress & "," & requestStatus & "," & failureMessage)
			End If

		Catch ex As Exception
			log.Error(String.Format("Error Occurred while WriteLog. Error is {0}.", ex.Message))
		End Try

	End Sub

	Shared Function CheckSessionExpired() As Boolean
		Try
			If (String.IsNullOrEmpty(HttpContext.Current.Session("PersonId")) Or String.IsNullOrEmpty(HttpContext.Current.Session("UniqueId")) Or
				String.IsNullOrEmpty(HttpContext.Current.Session("RoleId")) Or String.IsNullOrEmpty(HttpContext.Current.Session("CustomerId"))) Then
				Return False
			Else
				Return True
			End If
		Catch ex As Exception
			log.Error(String.Format("Error Occurred in CheckSessionExpired. Error is {0}.", ex.Message))
			Return False
		End Try
	End Function
End Class


Public Class UserData
	Public Email As String
	Public PhoneNumber As String
	Public PersonName As String
	Public IsApproved As String
	Public IMEI_UDID As String
	Public IsOdoMeterRequire As String
	Public IsLoginRequire As String
	Public IsDepartmentRequire As String
	Public IsPersonnelPINRequire As String
	Public IsPersonnelPINRequireForHub As String
	Public IsOtherRequire As String
	Public OtherLabel As String
	Public TimeOut As String
	Public AndroidAppLatestVersion As String
	Public AppUpgradeMsgDisplayAfterDays As String
	Public PersonId As String
	Public BluetoothCardReader As String
	Public BluetoothCardReaderMacAddress As String
	Public IsVehicleHasFob As String
	Public IsPersonHasFob As String
	Public FluidSecureSiteName As String
	Public IsAccessForFOBApp As String
	Public LFBluetoothCardReader As String
	Public LFBluetoothCardReaderMacAddress As String
    Public VeederRootMacAddress As String
    Public CollectDiagnosticLogs As String
    Public IsGateHub As String
    Public IsVehicleNumberRequire As String
    Public IsLogging As String
    Public WifiChannelToUse As String
    Public EnbDisHubForFA As String
    Public StayOpenGate As String
End Class

Public Class ArrayData
	Public Property data() As String
		Get
			Return m_data
		End Get
		Set(value As String)
			m_data = value
		End Set
	End Property
	Private m_data As String
	Public Property Message() As String
		Get
			Return m_Message
		End Get
		Set(value As String)
			m_Message = value
		End Set
	End Property
	Private m_Message As String
	Public Property ResponseCode() As String
		Get
			Return m_ResponseCode
		End Get
		Set(value As String)
			m_ResponseCode = value
		End Set
	End Property
	Private m_ResponseCode As String
	Public Property ResponseText() As String
		Get
			Return m_ResponseText
		End Get
		Set(value As String)
			m_ResponseText = value
		End Set
	End Property
	Private m_ResponseText As String
End Class

Public Class GetSSIDList
	Public result As List(Of SSIDData)

End Class

Public Class SSIDData
	Public SiteId As String
	Public SiteNumber As String
	Public SiteName As String
	Public SiteAddress As String
	Public Latitude As String
	Public Longitude As String
	Public HoseId As String
	Public HoseNumber As String
	Public WifiSSId As String
	Public UserName As String
	Public Password As String
	Public MacAddress As String
	Public IsBusy As String
	Public PulserRatio As String
	Public DecimalPulserRatio As Decimal
	Public FuelTypeId As String
	Public ReplaceableHoseName As String
	Public IsHoseNameReplaced As String
	Public ResponceMessage As String
	Public ResponceText As String
	Public IsUpgrade As String
	Public FilePath As String
	Public PulserTimingAdjust As String
	Public BluetoothCardReaderHF As String
	Public BluetoothCardReaderMacAddress As String
	Public PumpOnTime As String
	Public PumpOffTime As String
	Public PulserStopTime As String
	Public LFBluetoothCardReader As String
	Public LFBluetoothCardReaderMacAddress As String
	Public VeederRootMacAddress As String
	Public FSNPMacAddress As String
	Public IsDefective As String
    Public ReconfigureLink As String
    Public IsTLDCall As String
End Class

Public Class HoseNameReplaced
	Public IsHoseNameReplaced As String
	Public SiteId As String
	Public HoseId As String
End Class

Public Class HoseNameReplacedResponce
	Public ResponceMessage As String
	Public ResponceText As String
End Class


Public Class SiteDetailsForUpdateMACAddress
	Public SiteId As String
	Public MACAddress As String
	Public RequestFrom As String
	Public HubName As String
End Class

Public Class SiteDetailsResponseForUpdateMACAddress
	Public ResponceMessage As String
	Public ResponceText As String
End Class

Public Class ChangeBusyStatus
	Public SiteId As String
End Class

Public Class ChangeBusyStatusResponse
	Public ResponceMessage As String
	Public ResponceText As String
End Class
