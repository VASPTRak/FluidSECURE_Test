Imports System.Data.SqlClient
Imports log4net.Config
Imports log4net

Public Class WebServiceBAL

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(WebServiceBAL))

    Shared Sub New()
        XmlConfigurator.Configure()
    End Sub

    Public Function GetUserIsApproved(ByVal IMEInum As String) As DataSet
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(0) As SqlParameter
            Dim ParIMEInum = New SqlParameter("@IMEInum", SqlDbType.NVarChar, 1000)
            ParIMEInum.Direction = ParameterDirection.Input
            ParIMEInum.Value = IMEInum
            parcollection(0) = ParIMEInum



            'Dim parcollection() As SqlParameter = New SqlParameter() {}

            Dim ds = New DataSet()

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Service_GetUserIsApproved", parcollection)

            Return ds

        Catch ex As Exception
            log.Error("Error occurred in GetUserIsApproved Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function IsIMEIExists(ByVal IMEInum As String) As DataSet
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(0) As SqlParameter
            Dim ParIMEInum = New SqlParameter("@IMEInum", SqlDbType.NVarChar, 1000)
            ParIMEInum.Direction = ParameterDirection.Input
            ParIMEInum.Value = IMEInum
            parcollection(0) = ParIMEInum

            Dim ds = New DataSet()

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Service_IsIMEIExists", parcollection)

            Return ds

        Catch ex As Exception
            log.Error("Error occurred in IsIMEIExists Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function IsEmailExists(ByVal Email As String) As DataSet
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(0) As SqlParameter
            Dim ParIMEInum = New SqlParameter("@Email", SqlDbType.NVarChar, 1000)
            ParIMEInum.Direction = ParameterDirection.Input
            ParIMEInum.Value = Email
            parcollection(0) = ParIMEInum

            Dim ds = New DataSet()

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Service_IsEmailExists", parcollection)

            Return ds

        Catch ex As Exception
            log.Error("Error occurred in IsEmailExists Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function UpdateIMEIbyEmail(ByVal Email As String, IMEI As String, IMEI_UDID As String) As Integer
        Dim dal = New GeneralizedDAL()
        Dim result As Integer
        Try
            Dim parcollection(2) As SqlParameter

            Dim ParIMEInum = New SqlParameter("@IMEI", SqlDbType.NVarChar, 1000)
            ParIMEInum.Direction = ParameterDirection.Input
            ParIMEInum.Value = IMEI
            parcollection(0) = ParIMEInum


            Dim ParEmail = New SqlParameter("@Email", SqlDbType.NVarChar, 1000)
            ParEmail.Direction = ParameterDirection.Input
            ParEmail.Value = Email
            parcollection(1) = ParEmail

            Dim ParCurrentIMEI = New SqlParameter("@CurrentIMEI", SqlDbType.NVarChar, 1000)
            ParCurrentIMEI.Direction = ParameterDirection.Input
            ParCurrentIMEI.Value = IMEI_UDID
            parcollection(2) = ParCurrentIMEI

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Service_UpdateIMEIbyEmail", parcollection)

            Return result

        Catch ex As Exception
            log.Error("Error occurred in UpdateIMEIbyEmail Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function


    'Public Function GetSiteDetails() As DataSet
    '    Dim dal = New GeneralizedDAL()
    '    Try

    '        Dim parcollection() As SqlParameter = New SqlParameter() {}

    '        Dim ds = New DataSet()

    '        ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Service_GetSiteDetails", parcollection)

    '        Return ds

    '    Catch ex As Exception
    '        log.Error("Error occurred in GetSiteDetails Exception is :" + ex.Message)
    '        Return Nothing
    '    Finally
    '    End Try
    'End Function

    Public Function GetSSIDbySiteId(ByVal Ids As String) As DataSet
        Dim dal = New GeneralizedDAL()
        Try

            Dim parcollection(0) As SqlParameter
            Dim ParIds = New SqlParameter("@Ids", SqlDbType.NVarChar, 1000)
            ParIds.Direction = ParameterDirection.Input
            ParIds.Value = Ids
            parcollection(0) = ParIds

            Dim ds = New DataSet()

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Service_GetSSIDbySiteId", parcollection)

            Return ds

        Catch ex As Exception
            log.Error("Error occurred in GetSSIDbySiteId Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function GetAndroidSSID(ByVal SSID As String, ByVal Ids As String) As DataSet
        Dim dal = New GeneralizedDAL()
        Try

            Dim parcollection(1) As SqlParameter
            Dim Par1 = New SqlParameter("@ssid", SqlDbType.NVarChar, 1000)
            Par1.Direction = ParameterDirection.Input
            Par1.Value = SSID
            parcollection(0) = Par1

            Dim ParIds = New SqlParameter("@Ids", SqlDbType.NVarChar, 1000)
            ParIds.Direction = ParameterDirection.Input
            ParIds.Value = Ids
            parcollection(1) = ParIds

            Dim ds = New DataSet()

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Service_GetAndroidSSID", parcollection)

            Return ds

        Catch ex As Exception
            log.Error("Error occurred in GetAndroidSSID Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function


#Region "Authorization Sequence"

#End Region

    Public Function UpdateIsHoseNameReplaced(SiteId As Integer, HoseId As Integer) As Integer
        Dim dal = New GeneralizedDAL()
        Dim result As Integer
        Try
            Dim parcollection(1) As SqlParameter

            Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
            ParSiteId.Direction = ParameterDirection.Input
            ParSiteId.Value = SiteId
            parcollection(0) = ParSiteId

            Dim ParHoseId = New SqlParameter("@HoseId", SqlDbType.Int)
            ParHoseId.Direction = ParameterDirection.Input
            ParHoseId.Value = HoseId
            parcollection(1) = ParHoseId



            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Service_UpdateIsHoseNameReplaced", parcollection)

            Return result

        Catch ex As Exception
            log.Error("Error occurred in UpdateIMEIbyEmail Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function UpdateMACAddress(ByVal SiteId As Integer, MACAddress As String, RequestFrom As String, HubName As String) As Integer
        Dim dal = New GeneralizedDAL()
        Dim result As Integer
        Try
            Dim parcollection(3) As SqlParameter

            Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
            ParSiteId.Direction = ParameterDirection.Input
            ParSiteId.Value = SiteId
            parcollection(0) = ParSiteId


            Dim ParMACAddress = New SqlParameter("@MACAddress", SqlDbType.NVarChar, 100)
            ParMACAddress.Direction = ParameterDirection.Input
            ParMACAddress.Value = MACAddress
            parcollection(1) = ParMACAddress


            Dim ParRequestFrom = New SqlParameter("@RequestFrom", SqlDbType.NVarChar, 100)
            ParRequestFrom.Direction = ParameterDirection.Input
            ParRequestFrom.Value = RequestFrom
            parcollection(2) = ParRequestFrom


            Dim ParHubName = New SqlParameter("@HubName", SqlDbType.NVarChar, 100)
            ParHubName.Direction = ParameterDirection.Input
            ParHubName.Value = HubName
            parcollection(3) = ParHubName

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Service_UpdateMACAddress", parcollection)

            Return result

        Catch ex As Exception
            log.Error("Error occurred in UpdateMACAddress Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function ChangeBusyStatusOfFluidSecureUnit(ByVal SiteId As Integer, IsBusy As Boolean, BusyFromIMEI_UDID As String) As Integer
        Dim dal = New GeneralizedDAL()
        Dim result As Integer
        Try
            Dim parcollection(2) As SqlParameter

            Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
            ParSiteId.Direction = ParameterDirection.Input
            ParSiteId.Value = SiteId
            parcollection(0) = ParSiteId


            Dim ParIsBusy = New SqlParameter("@IsBusy", SqlDbType.Bit)
            ParIsBusy.Direction = ParameterDirection.Input
            ParIsBusy.Value = IsBusy
            parcollection(1) = ParIsBusy

            Dim ParBusyFromIMEI_UDID = New SqlParameter("@BusyFromIMEI_UDID", SqlDbType.NVarChar, 2000)
            ParBusyFromIMEI_UDID.Direction = ParameterDirection.Input
            ParBusyFromIMEI_UDID.Value = BusyFromIMEI_UDID
            parcollection(2) = ParBusyFromIMEI_UDID

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Service_ChangeBusyStatusOfFluidSecureUnit", parcollection)

            Return result

        Catch ex As Exception
            log.Error("Error occurred in ChangeBusyStatusOfFluidSecureUnit Exception is :" + ex.Message)
            Return 0
        Finally
        End Try
    End Function

    Public Function CheckBusyStatusOfAllFluidSecureUnits(ByVal WaitingTime As Integer) As Integer
        Dim dal = New GeneralizedDAL()
        Dim result As Integer
        Try
            Dim parcollection(0) As SqlParameter

            Dim ParWaitingTime = New SqlParameter("@WaitingTime", SqlDbType.Int)
            ParWaitingTime.Direction = ParameterDirection.Input
            ParWaitingTime.Value = WaitingTime
            parcollection(0) = ParWaitingTime

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Service_CheckBusyStatusOfAllFluidSecureUnits", parcollection)

            Return result

        Catch ex As Exception
            log.Error("Error occurred in CheckBusyStatusOfAllFluidSecureUnits Exception is :" + ex.Message)
            Return 0
        Finally
        End Try
    End Function

    Public Function GetCustomerAdminsByCustomerId(ByVal CustomerId As Integer) As String
        Dim dal = New GeneralizedDAL()
        Dim result As String
        Try
            Dim parcollection(0) As SqlParameter

            Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
            ParCustomerId.Direction = ParameterDirection.Input
            ParCustomerId.Value = CustomerId
            parcollection(0) = ParCustomerId

            result = dal.ExecuteStoredProcedureGetString("usp_tt_Service_GetCustomerAdminsByCustomerId", parcollection)

            Return result

        Catch ex As Exception
            log.Error("Error occurred in GetCustomerAdminsByCustomerId Exception is :" + ex.Message)
            Return ""
        Finally
        End Try
    End Function

    Public Function GetPersonnelByPinNumber(ByVal PinNumber As String, CompanyId As Integer) As DataSet
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(1) As SqlParameter
            Dim ParPinNumber = New SqlParameter("@PinNumber", SqlDbType.NVarChar, 20)
            ParPinNumber.Direction = ParameterDirection.Input
            ParPinNumber.Value = PinNumber
            parcollection(0) = ParPinNumber

            Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
            ParCompanyId.Direction = ParameterDirection.Input
            ParCompanyId.Value = CompanyId
            parcollection(1) = ParCompanyId
            Dim ds = New DataSet()

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_service_GetPersonnelByPinNumber", parcollection)

            Return ds

        Catch ex As Exception
            log.Error("Error occurred in GetPersonnelByPinNumber Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

	Public Function UpdateFSTagMacAddressToVehicle(ByVal FSTagMacAddress As String, personId As Integer, VehicleId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Dim result As Integer
		Try
			Dim parcollection(2) As SqlParameter

			Dim ParpersonId = New SqlParameter("@UserId", SqlDbType.Int)
			ParpersonId.Direction = ParameterDirection.Input
			ParpersonId.Value = personId
			parcollection(0) = ParpersonId


			Dim ParFSTagMacAddress = New SqlParameter("@FSTagMacAddress", SqlDbType.NVarChar, 50)
			ParFSTagMacAddress.Direction = ParameterDirection.Input
			ParFSTagMacAddress.Value = FSTagMacAddress
			parcollection(1) = ParFSTagMacAddress


			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(2) = ParVehicleId

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vehicle_UpdateFSTagMacAddress", parcollection)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in UpdateFSTagMacAddressToVehicle Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

    Public Function CheckDefectiveBluetoothInfoEmail(ByVal HubName As String) As DataTable
        Dim dal = New GeneralizedDAL()
        Dim result As DataSet
        Try
            Dim parcollection(0) As SqlParameter

            Dim ParHubName = New SqlParameter("@HubName", SqlDbType.NVarChar, 100)
            ParHubName.Direction = ParameterDirection.Input
            ParHubName.Value = HubName
            parcollection(0) = ParHubName

            result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_service_CheckDefectiveBluetoothInfoEmail", parcollection)

            Return result.Tables(0)

        Catch ex As Exception
            log.Error("Error occurred in CheckDefectiveBluetoothInfoEmail Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function InsertUpdateDefectiveBluetoothInfoEmailRecord(ByVal HubName As String) As Integer
        Dim dal = New GeneralizedDAL()
        Dim result As Integer
        Try
            Dim parcollection(0) As SqlParameter

            Dim ParHubName = New SqlParameter("@HubName", SqlDbType.NVarChar, 100)
            ParHubName.Direction = ParameterDirection.Input
            ParHubName.Value = HubName
            parcollection(0) = ParHubName

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_service_InsertUpdateDefectiveBluetoothInfoEmailRecord", parcollection)

            Return result

        Catch ex As Exception
            log.Error("Error occurred in InsertUpdateDefectiveBluetoothInfoEmailRecord Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

End Class
