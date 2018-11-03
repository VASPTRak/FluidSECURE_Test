Public Class AuthorizationSequenceModel
    Private _IMEIUDID As String
    Public Property IMEIUDID() As String
        Get
            Return _IMEIUDID
        End Get
        Set(ByVal value As String)
            _IMEIUDID = value
        End Set
    End Property

    Private _VehicleNumber As String
    Public Property VehicleNumber() As String
        Get
            Return _VehicleNumber
        End Get
        Set(ByVal value As String)
            _VehicleNumber = value
        End Set
    End Property

    Private _OdoMeter As Integer
    Public Property OdoMeter() As Integer
        Get
            Return _OdoMeter
        End Get
        Set(ByVal value As Integer)
            _OdoMeter = value
        End Set
    End Property

    Private _WifiSSId As String
    Public Property WifiSSId() As String
        Get
            Return _WifiSSId
        End Get
        Set(ByVal value As String)
            _WifiSSId = value
        End Set
    End Property

    Private _SiteId As Integer
    Public Property SiteId() As Integer
        Get
            Return _SiteId
        End Get
        Set(ByVal value As Integer)
            _SiteId = value
        End Set
    End Property

    Private _DepartmentNumber As String
    Public Property DepartmentNumber() As String
        Get
            Return _DepartmentNumber
        End Get
        Set(ByVal value As String)
            _DepartmentNumber = value
        End Set
    End Property

    Private _PersonnelPIN As String
    Public Property PersonnelPIN() As String
        Get
            Return _PersonnelPIN
        End Get
        Set(ByVal value As String)
            _PersonnelPIN = value
        End Set
    End Property

    Private _Other As String
    Public Property Other() As String
        Get
            Return _Other
        End Get
        Set(ByVal value As String)
            _Other = value
        End Set
    End Property

    Private _RequestFrom As String
    Public Property RequestFrom() As String
        Get
            Return _RequestFrom
        End Get
        Set(ByVal value As String)
            _RequestFrom = value
        End Set
    End Property

    Public Property CurrentLat() As String
        Get
            Return m_CurrentLat
        End Get
        Set(value As String)
            m_CurrentLat = value
        End Set
    End Property
    Private m_CurrentLat As String

    Public Property CurrentLng() As String
        Get
            Return m_CurrentLng
        End Get
        Set(value As String)
            m_CurrentLng = value
        End Set
    End Property
    Private m_CurrentLng As String

    Private _Hours As String
    Public Property Hours() As String
        Get
            Return _Hours
        End Get
        Set(ByVal value As String)
            _Hours = value
        End Set
    End Property

    Private _RequestFromAPP As String
    Public Property RequestFromAPP() As String
        Get
            Return _RequestFromAPP
        End Get
        Set(ByVal value As String)
            _RequestFromAPP = value
        End Set
    End Property

    Private _FOBNumber As String
    Public Property FOBNumber() As String
        Get
            Return _FOBNumber
        End Get
        Set(ByVal value As String)
            _FOBNumber = value
        End Set
    End Property

    Private _HubId As Integer
    Public Property HubId() As Integer
        Get
            Return _HubId
        End Get
        Set(ByVal value As Integer)
            _HubId = value
        End Set
    End Property

    Private _IsVehicleNumberRequire As String
    Public Property IsVehicleNumberRequire() As String
        Get
            Return _IsVehicleNumberRequire
        End Get
        Set(ByVal value As String)
            _IsVehicleNumberRequire = value
        End Set
    End Property
End Class

Public Class ResponceData
    Public Property MinLimit() As Integer
        Get
            Return m_MinLimit
        End Get
        Set(value As Integer)
            m_MinLimit = value
        End Set
    End Property
    Private m_MinLimit As Integer
    Public Property PulseRatio() As Decimal
        Get
            Return m_PulseRatio
        End Get
        Set(value As Decimal)
            m_PulseRatio = value
        End Set
    End Property
    Private m_PulseRatio As Decimal

    Public Property VehicleId() As Integer
        Get
            Return m_VehicleId
        End Get
        Set(value As Integer)
            m_VehicleId = value
        End Set
    End Property
    Private m_VehicleId As Integer

    Public Property FuelTypeId() As Integer
        Get
            Return m_FuelTypeId
        End Get
        Set(value As Integer)
            m_FuelTypeId = value
        End Set
    End Property
    Private m_FuelTypeId As Integer

    Public Property PersonId() As Integer
        Get
            Return m_PersonId
        End Get
        Set(value As Integer)
            m_PersonId = value
        End Set
    End Property
    Private m_PersonId As Integer

    Public Property PhoneNumber() As String
        Get
            Return m_PhoneNumber
        End Get
        Set(value As String)
            m_PhoneNumber = value
        End Set
    End Property
    Private m_PhoneNumber As String

    Public Property ServerDate() As String
        Get
            Return m_ServerDate
        End Get
        Set(value As String)
            m_ServerDate = value
        End Set
    End Property
    Private m_ServerDate As String

    Public Property PulserStopTime() As String
        Get
            Return m_PulserStopTime
        End Get
        Set(value As String)
            m_PulserStopTime = value
        End Set
    End Property
    Private m_PulserStopTime As String

    Public Property PumpOnTime() As String
        Get
            Return m_PumpOnTime
        End Get
        Set(value As String)
            m_PumpOnTime = value
        End Set
    End Property
    Private m_PumpOnTime As String

    Public Property PumpOffTime() As String
        Get
            Return m_PumpOffTime
        End Get
        Set(value As String)
            m_PumpOffTime = value
        End Set
    End Property
    Private m_PumpOffTime As String

    Public Property TransactionId() As String
        Get
            Return m_TransactionId
        End Get
        Set(value As String)
            m_TransactionId = value
        End Set
    End Property
    Private m_TransactionId As String

    Public Property FirmwareVersion() As String
        Get
            Return m_FirmwareVersion
        End Get
        Set(value As String)
            m_FirmwareVersion = value
        End Set
    End Property
    Private m_FirmwareVersion As String

    Public Property FilePath() As String
        Get
            Return m_FilePath
        End Get
        Set(value As String)
            m_FilePath = value
        End Set
    End Property
    Private m_FilePath As String

    Public Property FOBNumber() As String
        Get
            Return m_FOBNumber
        End Get
        Set(value As String)
            m_FOBNumber = value
        End Set
    End Property
    Private m_FOBNumber As String


    Public Property Company() As String
        Get
            Return m_Company
        End Get
        Set(value As String)
            m_Company = value
        End Set
    End Property
    Private m_Company As String


    Public Property Location() As String
        Get
            Return m_Location
        End Get
        Set(value As String)
            m_Location = value
        End Set
    End Property
    Private m_Location As String


    Public Property PersonName() As String
        Get
            Return m_PersonName
        End Get
        Set(value As String)
            m_PersonName = value
        End Set
    End Property
    Private m_PersonName As String

    Public Property FluidSecureSiteName() As String
        Get
            Return m_FluidSecureSiteName
        End Get
        Set(value As String)
            m_FluidSecureSiteName = value
        End Set
    End Property
    Private m_FluidSecureSiteName As String


    Public Property BluetoothCardReader() As String
        Get
            Return m_BluetoothCardReader
        End Get
        Set(value As String)
            m_BluetoothCardReader = value
        End Set
    End Property
    Private m_BluetoothCardReader As String

    Public Property BluetoothCardReaderMacAddress() As String
        Get
            Return m_BluetoothCardReaderMacAddress
        End Get
        Set(value As String)
            m_BluetoothCardReaderMacAddress = value
        End Set
    End Property
    Private m_BluetoothCardReaderMacAddress As String

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

	Public Property VehicleSum() As Decimal
		Get
			Return m_VehicleSum
		End Get
		Set(value As Decimal)
			m_VehicleSum = value
		End Set
	End Property
	Private m_VehicleSum As Decimal

	Public Property DeptSum() As Decimal
		Get
			Return m_DeptSum
		End Get
		Set(value As Decimal)
			m_DeptSum = value
		End Set
	End Property
	Private m_DeptSum As Decimal

	Public Property VehPercentage() As Decimal
		Get
			Return m_VehPercentage
		End Get
		Set(value As Decimal)
			m_VehPercentage = value
		End Set
	End Property
	Private m_VehPercentage As Decimal

	Public Property DeptPercentage() As Decimal
		Get
			Return m_DeptPercentage
		End Get
		Set(value As Decimal)
			m_DeptPercentage = value
		End Set
	End Property
	Private m_DeptPercentage As Decimal

	Public Property SurchargeType() As String
		Get
			Return m_SurchargeType
		End Get
		Set(value As String)
			m_SurchargeType = value
		End Set
	End Property
	Private m_SurchargeType As String

	Public Property ProductPrice() As Decimal
		Get
			Return m_ProductPrice
		End Get
		Set(value As Decimal)
			m_ProductPrice = value
		End Set
	End Property
	Private m_ProductPrice As Decimal

	Public Property VeederRootMacAddress() As String
		Get
			Return m_VeederRootMacAddress
		End Get
		Set(value As String)
			m_VeederRootMacAddress = value
		End Set
	End Property
	Private m_VeederRootMacAddress As String

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

	Public Property CollectDiagnosticLogs() As String
		Get
			Return m_CollectDiagnosticLogs
		End Get
		Set(value As String)
			m_CollectDiagnosticLogs = value
		End Set
	End Property
    Private m_CollectDiagnosticLogs As String

    Public Property IsGateHub() As String
        Get
            Return m_IsGateHub
        End Get
        Set(value As String)
            m_IsGateHub = value
        End Set
    End Property
	Private m_IsGateHub As String
	Public Property IsVehicleNumberRequire() As String
		Get
			Return m_IsVehicleNumberRequire
		End Get
		Set(value As String)
			m_IsVehicleNumberRequire = value
		End Set
	End Property
    Private m_IsVehicleNumberRequire As String

    Public Property IsTLDCall() As String
        Get
            Return m_IsTLDCall
        End Get
        Set(value As String)
            m_IsTLDCall = value
        End Set
    End Property
    Private m_IsTLDCall As String

End Class

Public Class RootObject
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
	Public Property ResponceData() As ResponceData
		Get
			Return m_ResponceData
		End Get
		Set(value As ResponceData)
			m_ResponceData = value
		End Set
	End Property
	Private m_ResponceData As ResponceData


    Public Property SSIDDataObj() As List(Of SSIDData)
        Get
            Return m_SSIDDataObj
        End Get
        Set(value As List(Of SSIDData))
            m_SSIDDataObj = value
        End Set
    End Property
    Private m_SSIDDataObj As List(Of SSIDData)


    Public Property objUserData() As UserData
        Get
            Return m_objUserData
        End Get
        Set(value As UserData)
            m_objUserData = value
        End Set
    End Property
    Private m_objUserData As UserData

    Public Property ValidationFailFor() As String
        Get
            Return m_ValidationFailFor
        End Get
        Set(value As String)
            m_ValidationFailFor = value
        End Set
    End Property
    Private m_ValidationFailFor As String

    Public Property PreAuthTransactionsObj() As ResponsePreAuthTransactions
        Get
            Return m_PreAuthTransactionsObj
        End Get
        Set(value As ResponsePreAuthTransactions)
            m_PreAuthTransactionsObj = value
        End Set
    End Property
    Private m_PreAuthTransactionsObj As ResponsePreAuthTransactions

End Class

Public Class Register
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String

    Public Property Mobile() As String
        Get
            Return m_Mobile
        End Get
        Set(value As String)
            m_Mobile = value
        End Set
    End Property
    Private m_Mobile As String

    Public Property Emailid() As String
        Get
            Return m_Emailid
        End Get
        Set(value As String)
            m_Emailid = value
        End Set
    End Property
    Private m_Emailid As String

    Public Property IMEI_UDID() As String
        Get
            Return m_IMEI_UDID
        End Get
        Set(value As String)
            m_IMEI_UDID = value
        End Set
    End Property
    Private m_IMEI_UDID As String

    Public Property ReqFrom() As String
        Get
            Return m_ReqFrom
        End Get
        Set(value As String)
            m_ReqFrom = value
        End Set
    End Property
    Private m_ReqFrom As String
End Class

Public Class TransactionComplete
    Public Property VehicleId() As Integer
        Get
            Return m_VehicleId
        End Get
        Set(value As Integer)
            m_VehicleId = value
        End Set
    End Property
    Private m_VehicleId As Integer

    Public Property CurrentOdometer() As Integer
        Get
            Return m_CurrentOdometer
        End Get
        Set(value As Integer)
            m_CurrentOdometer = value
        End Set
    End Property
    Private m_CurrentOdometer As Integer

    Public Property FuelTypeId() As Integer
        Get
            Return m_FuelTypeId
        End Get
        Set(value As Integer)
            m_FuelTypeId = value
        End Set
    End Property
    Private m_FuelTypeId As Integer

    Public Property SiteId() As Integer
        Get
            Return m_SiteId
        End Get
        Set(value As Integer)
            m_SiteId = value
        End Set
    End Property
    Private m_SiteId As Integer

    Public Property PersonId() As Integer
        Get
            Return m_PersonId
        End Get
        Set(value As Integer)
            m_PersonId = value
        End Set
    End Property
    Private m_PersonId As Integer

    Public Property FuelQuantity() As Decimal
        Get
            Return m_FuelQuantity
        End Get
        Set(value As Decimal)
            m_FuelQuantity = value
        End Set
    End Property
    Private m_FuelQuantity As Decimal

    Public Property PhoneNumber() As String
        Get
            Return m_PhoneNumber
        End Get
        Set(value As String)
            m_PhoneNumber = value
        End Set
    End Property
    Private m_PhoneNumber As String

    Public Property WifiSSId() As String
        Get
            Return m_WifiSSId
        End Get
        Set(value As String)
            m_WifiSSId = value
        End Set
    End Property
    Private m_WifiSSId As String

    Public Property TransactionDate() As String
        Get
            Return m_TransactionDate
        End Get
        Set(value As String)
            m_TransactionDate = value
        End Set
    End Property
    Private m_TransactionDate As String

    Public Property TransactionFrom() As String
        Get
            Return m_TransactionFrom
        End Get
        Set(value As String)
            m_TransactionFrom = value
        End Set
    End Property
    Private m_TransactionFrom As String

    Public Property CurrentLat() As String
        Get
            Return m_CurrentLat
        End Get
        Set(value As String)
            m_CurrentLat = value
        End Set
    End Property
    Private m_CurrentLat As String


    Public Property CurrentLng() As String
        Get
            Return m_CurrentLng
        End Get
        Set(value As String)
            m_CurrentLng = value
        End Set
    End Property
    Private m_CurrentLng As String

    Public Property VehicleNumber() As String
        Get
            Return m_VehicleNumber
        End Get
        Set(value As String)
            m_VehicleNumber = value
        End Set
    End Property
    Private m_VehicleNumber As String

    Private _DepartmentNumber As String
    Public Property DepartmentNumber() As String
        Get
            Return _DepartmentNumber
        End Get
        Set(ByVal value As String)
            _DepartmentNumber = value
        End Set
    End Property

    Private _PersonnelPIN As String
    Public Property PersonnelPIN() As String
        Get
            Return _PersonnelPIN
        End Get
        Set(ByVal value As String)
            _PersonnelPIN = value
        End Set
    End Property

    Private _Other As String
    Public Property Other() As String
        Get
            Return _Other
        End Get
        Set(ByVal value As String)
            _Other = value
        End Set
    End Property

    Private _Hours As String
    Public Property Hours() As String
        Get
            Return _Hours
        End Get
        Set(ByVal value As String)
            _Hours = value
        End Set
    End Property

    Private _TransactionId As String
    Public Property TransactionId() As String
        Get
            Return _TransactionId
        End Get
        Set(ByVal value As String)
            _TransactionId = value
        End Set
    End Property

    Private _Pulses As Integer
    Public Property Pulses() As Integer
        Get
            Return _Pulses
        End Get
        Set(ByVal value As Integer)
            _Pulses = value
        End Set
    End Property

    Private _IsLastTransaction As String
    Public Property IsLastTransaction() As String
        Get
            Return _IsLastTransaction
        End Get
        Set(ByVal value As String)
            _IsLastTransaction = value
        End Set
    End Property

    Private _IsFuelingStop As String
    Public Property IsFuelingStop() As String
        Get
            Return _IsFuelingStop
        End Get
        Set(ByVal value As String)
            _IsFuelingStop = value
        End Set
    End Property

End Class

Public Class TransactionCompleteResponce
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
End Class

Public Class CheckRequireOdoResponse

    Public IsOdoMeterRequire As String
    Public IsHoursRequire As String
    Public ResponceMessage As String
    Public ResponceText As String
    Public ValidationFailFor As String
    Public PreviousOdo As String
    Public OdoLimit As String
    Public OdometerReasonabilityConditions As String
    Public CheckOdometerReasonable As String
    Public FOBNumber As String
    Public VehicleNumber As String
    Public IsNewFob As String
    Public PersonPin As String
    Public PreviousHours As String
    Public HoursLimit As String
End Class


Public Class ResponsePreAuthTransactions
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String

    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String

    Public Property SitesObj() As List(Of SSIDData)
        Get
            Return m_SitesObj
        End Get
        Set(value As List(Of SSIDData))
            m_SitesObj = value
        End Set
    End Property
    Private m_SitesObj As List(Of SSIDData)

    Public Property TransactionObj() As List(Of Transactions)
        Get
            Return m_TransactionObj
        End Get
        Set(value As List(Of Transactions))
            m_TransactionObj = value
        End Set
    End Property
    Private m_TransactionObj As List(Of Transactions)
End Class


Public Class Transactions

    Public ResponceMessage As String
    Public ResponceText As String

    Public Property TransactionId() As String
        Get
            Return m_TransactionId
        End Get
        Set(value As String)
            m_TransactionId = value
        End Set
    End Property
    Private m_TransactionId As String

End Class

Public Class UpgradeCurrentVersionWithUgradableVersionMaster
    Public Property HoseId() As String
        Get
            Return m_HoseId
        End Get
        Set(value As String)
            m_HoseId = value
        End Set
    End Property
    Private m_HoseId As String

    Public Property Version() As String
        Get
            Return m_Version
        End Get
        Set(value As String)
            m_Version = value
        End Set
    End Property
    Private m_Version As String



End Class

Public Class IsUpgradeCurrentVersionWithUgradableVersionMaster
    Public Property HoseId() As String
        Get
            Return m_HoseId
        End Get
        Set(value As String)
            m_HoseId = value
        End Set
    End Property
    Private m_HoseId As String

    Public Property PersonId() As String
        Get
            Return m_PersonId
        End Get
        Set(value As String)
            m_PersonId = value
        End Set
    End Property
    Private m_PersonId As String



End Class


Public Class IsUpgradeCurrentVersionWithUgradableVersionFSVMMaster
    Public Property VehicleId() As String
        Get
            Return m_VehicleId
        End Get
        Set(value As String)
            m_VehicleId = value
        End Set
    End Property
    Private m_VehicleId As String

    Public Property PersonId() As String
        Get
            Return m_PersonId
        End Get
        Set(value As String)
            m_PersonId = value
        End Set
    End Property
    Private m_PersonId As String
End Class

Public Class UpgradeCurrentVersionWithUgradableVersionResponse
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
End Class

Public Class IsUpgradeCurrentVersionWithUgradableVersionResponse
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
End Class

Public Class IsUpgradeCurrentVersionWithUgradableVersionFSVMResponse
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
End Class

Public Class UpgradeTransactionStatusMaster
    Public Property TransactionId() As Integer
        Get
            Return m_TransactionId
        End Get
        Set(value As Integer)
            m_TransactionId = value
        End Set
    End Property
    Private m_TransactionId As Integer

    Public Property Status() As Integer
        Get
            Return m_Status
        End Get
        Set(value As Integer)
            m_Status = value
        End Set
    End Property
    Private m_Status As Integer



End Class

Public Class UpgradeTransactionStatusResponse
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
End Class


Public Class UpgradeIsBusyStatusMaster
    Public Property SiteId() As Integer
        Get
            Return m_SiteId
        End Get
        Set(value As Integer)
            m_SiteId = value
        End Set
    End Property
    Private m_SiteId As Integer
End Class

Public Class UpgradeIsBusyStatusResponse
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
End Class

Public Class FSVMUpgradeFileObject
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
    Public Property ResponceData() As FSVMResponceData
        Get
            Return m_ResponceData
        End Get
        Set(value As FSVMResponceData)
            m_ResponceData = value
        End Set
    End Property
    Private m_ResponceData As FSVMResponceData

End Class

Public Class FSVMResponceData
    Public Property FSVMFirmwareVersion() As String
        Get
            Return m_FSVMFirmwareVersion
        End Get
        Set(value As String)
            m_FSVMFirmwareVersion = value
        End Set
    End Property
    Private m_FSVMFirmwareVersion As String
    Public Property FilePath() As String
        Get
            Return m_FilePath
        End Get
        Set(value As String)
            m_FilePath = value
        End Set
    End Property
    Private m_FilePath As String

End Class

Public Class CheckValidPersonPinOrFOBNUmber

    Public PersonFOBNumber As String
    Public PersonPIN As String
End Class

Public Class CheckValidPersonPinOrFOBNUmberResponse

    Public ResponceMessage As String
    Public ResponceText As String
    Public PersonFOBNumber As String
    Public PersonPIN As String
	Public IsNewFob As String
End Class

Public Class CheckVehicleFobOnlyResponse

    Public ResponceMessage As String
    Public ResponceText As String
End Class


Public Class TankMonitorReading

	Public ProbeReading As String
	Public ReadingDateTime As String
	Public TLD As String
	Public FromSiteId As String
	Public IMEI_UDID As String
	Public LSB As String
    Public MSB As String
    Public TLDTemperature As String
    Public Response_code As String
End Class

Public Class TankMonitorReadingResponse

	Public ResponceMessage As String
	Public ResponceText As String
End Class

Public Class VR_Inventory

	Public IMEI_UDID As String
	Public VeederRootMacAddress As String
	Public AppDateTime As String
	Public TankNumber As String
	Public VRDateTime As String
	Public ProductCode As String
	Public TankStatus As String
	Public Volume As String
	Public TCVolume As String
	Public Ullage As String
	Public Height As String
	Public Water As String
	Public Temperature As String
	Public WaterVolume As String

End Class

Public Class VR_Delivery

	Public IMEI_UDID As String
	Public VeederRootMacAddress As String
	Public AppDateTime As String
	Public TankNumber As String
	Public VRDateTime As String
	Public ProductCode As String
	Public StartDateTime As String
	Public EndDateTime As String
	Public StartVolume As String
	Public StartTCVolume As String
	Public StartWater As String
	Public StartTemp As String
	Public EndVolume As String
	Public EndTCVolume As String
	Public EndWater As String
	Public EndTemp As String
	Public StartHeight As String
	Public EndHeight As String

End Class

Public Class CheckVRResponse

	Public ResponceMessage As String
	Public ResponceText As String

End Class

Public Class FAVehicleAuthorizationMaster
	Public VehicleRecurringMSG As String
	Public TransactionDate As String
	Public TransactionFrom As String
	Public CurrentLat As String
	Public CurrentLng As String
    Public FSTagMacAddress As String
    Public CurrentFSVMFirmwareVersion As String
End Class

Public Class FAVehicleAuthorizationMasterResponse

	Public ResponceMessage As String
    Public ResponceText As String
    Public IsFSVMUpgradable As String
    Public VehicleId As String
    Public FSVMFirmwareVersion As String
    Public FilePath As String
    Public PIC As String
    Public ESP32 As String
End Class

Public Class CheckAndValidateFSNPDetail

	Public IMEI_UDID As String
	Public FSNPMacAddress As String
	Public FSTagMacAddress As String

End Class

Public Class CheckAndValidateFSNPDetailResponse

	Public ResponceMessage As String
	Public ResponceText As String
	Public MinLimit As String
	Public SiteId As String
	Public PulseRatio As Decimal
	Public VehicleId As String
	Public PersonId As String
	Public FuelTypeId As String
	Public PhoneNumber As String
	Public ServerDate As String
	Public PumpOnTime As String
	Public PumpOffTime As String
	Public PulserStopTime As String
	Public TransactionId As String
	Public FirmwareVersion As String
	Public FilePath As String
	Public FOBNumber As String
	Public Company As String
	Public Location As String
	Public PersonName As String
	Public PrinterName As String
	Public PrinterMacAddress As String
	Public VehicleSum As Decimal
	Public DeptSum As Decimal
	Public VehPercentage As Decimal
	Public DeptPercentage As Decimal
	Public SurchargeType As String
	Public ProductPrice As Decimal
    Public parameter As String
    Public VehicleNumber As String
    Public RequireManualOdo As String
    Public PreviousOdo As String
    Public OdoLimit As String
    Public OdometerReasonabilityConditions As String
    Public CheckOdometerReasonable As String
    Public IsFSNPUpgradable As String
End Class

Public Class SaveVehicleManualOdometerMaster
    Public VehicleId As Integer
    Public Odometer As Integer
End Class

Public Class SaveVehicleManualOdometerResponse

    Public ResponceMessage As String
    Public ResponceText As String
End Class

Public Class CollectDiagnosticLogsDetailsResponse

	Public ResponceMessage As String
	Public ResponceText As String

End Class

Public Class CollectDiagnosticLogsDetails

	Public IMEI_UDID As String
	Public Collectedlogs As String
	Public LogFrom As String
    Public FileName As String
End Class

Public Class Cmtxtnid10Record
    Public Property trid As String
    Public Property qty As String
    Public Property pul As String
End Class

Public Class TransactionCompleteSaveMultipleTransactions
    Public Property cmtxtnid_10_record As List(Of TransactionComplete)
End Class

Public Class DefectiveBluetoothInfoEmailResponce
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String
End Class

Public Class DefectiveBluetoothInfoEmailMaser
    Public Property HubName As String
    Public Property SiteName As String
End Class