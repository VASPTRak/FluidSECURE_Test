Imports System.Data.SqlClient
Imports log4net.Config
Imports log4net

Public Class MasterBAL

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(MasterBAL))

	Shared Sub New()
		XmlConfigurator.Configure()
	End Sub

#Region "Transaction"
	Public Function GetTransactionColumnsValueForSave(DepartmentNumber As String, fuelTypeOfHose As Integer, PersonId As Integer, VehicleId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(3) {}

			Param(0) = New SqlParameter("@DeptNumber", SqlDbType.NVarChar, 20)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = DepartmentNumber

			Param(1) = New SqlParameter("@FuelTypeID", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = fuelTypeOfHose

			Param(2) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = PersonId

			Param(3) = New SqlParameter("@VehicleId", SqlDbType.Int)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = VehicleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_GetTransactionColumnsValueForSave", Param)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetTransactionColumnsValueForSave Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function InsertUpdateTransaction(VehicleId As Integer, SiteId As Integer, PersonId As Integer, CurrentOdometer As Integer,
											FuelQuantity As Decimal, FuelTypeId As Integer, PhoneNumber As String, WifiSSId As String,
											TransactionDate As DateTime, TransactionId As Integer, UserId As Integer, TransactionFrom As String, previousOdometer As Integer,
											CurrentLat As String, CurrentLng As String, CurrentLocationAddress As String, VehicleNumber As String,
											DepartmentNumber As String, PersonPin As String, Other As String, Hours As Integer, IsMissed As Boolean, IsUpdate As Boolean, TransactionStatus As Integer, HubId As Integer, Pulses As Integer,
											VehicleName As String, DepartmentName As String, FuelTypeName As String, Email As String, PersonName As String, CompanyName As String, OFFSite As Boolean, customerID As Integer,
											PreviousHours As Integer,
											RawOdometer As Integer) As Integer
		Dim result As Integer
		result = 0
		Try
			Dim dal = New GeneralizedDAL()
			Dim parcollection(35) As SqlParameter

			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			Dim ParCurrentOdometer = New SqlParameter("@CurrentOdometer", SqlDbType.Int)
			Dim ParFuelQuantity = New SqlParameter("@FuelQuantity", SqlDbType.Decimal)
			Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
			Dim ParPhoneNumber = New SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 20)
			Dim ParWifiSSId = New SqlParameter("@WifiSSId", SqlDbType.NVarChar, 50)
			Dim ParTransactionDate = New SqlParameter("@TransactionDate", SqlDbType.DateTime)
			Dim ParTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParTransactionFrom = New SqlParameter("@TransactionFrom", SqlDbType.NVarChar, 1)
			Dim ParpreviousOdometer = New SqlParameter("@previousOdometer", SqlDbType.Int)
			Dim ParCurrentLat = New SqlParameter("@CurrentLat", SqlDbType.NVarChar, 50)
			Dim ParCurrentLng = New SqlParameter("@CurrentLng", SqlDbType.NVarChar, 50)
			Dim ParCurrentLocationAddress = New SqlParameter("@CurrentLocationAddress", SqlDbType.NVarChar, 2000)
			Dim ParVehicleNumber = New SqlParameter("@VehicleNumber", SqlDbType.NVarChar, 10)
			Dim ParDepartmentNumber = New SqlParameter("@DepartmentNumber", SqlDbType.NVarChar, 10)
			Dim ParPersonPin = New SqlParameter("@PersonPin", SqlDbType.NVarChar, 20)
			Dim ParOther = New SqlParameter("@Other", SqlDbType.NVarChar, 2000)
			Dim ParHours = New SqlParameter("@Hours", SqlDbType.Int)
			Dim ParIsMissed = New SqlParameter("@IsMissed", SqlDbType.Bit)
			Dim ParIsUpdate = New SqlParameter("@IsUpdate", SqlDbType.Bit)
			Dim ParTransactionStatus = New SqlParameter("@TransactionStatus", SqlDbType.Int)
			Dim ParHubId = New SqlParameter("@HubId", SqlDbType.Int)
			Dim ParPulses = New SqlParameter("@Pulses", SqlDbType.Int)

			Dim ParVehicleName = New SqlParameter("@VehicleName", SqlDbType.NVarChar, 25)
			Dim ParDepartmentName = New SqlParameter("@DepartmentName", SqlDbType.NVarChar, 20)
			Dim ParFuelTypeName = New SqlParameter("@FuelTypeName", SqlDbType.NVarChar, 20)
			Dim ParEmail = New SqlParameter("@Email", SqlDbType.NVarChar, 256)
			Dim ParPersonName = New SqlParameter("@PersonName", SqlDbType.NVarChar, 30)
			Dim ParCompanyName = New SqlParameter("@CompanyName", SqlDbType.NVarChar, 50)

			Dim ParOFFSite = New SqlParameter("@OFFSite", SqlDbType.Bit)
			Dim ParcustomerID = New SqlParameter("@customerID", SqlDbType.Int)

			Dim ParRawOdometer = New SqlParameter("@RawOdometer", SqlDbType.Int)
			Dim ParPreviousHours = New SqlParameter("@PreviousHours", SqlDbType.Int)

			ParVehicleId.Direction = ParameterDirection.Input
			ParSiteId.Direction = ParameterDirection.Input
			ParPersonId.Direction = ParameterDirection.Input
			ParCurrentOdometer.Direction = ParameterDirection.Input
			ParFuelQuantity.Direction = ParameterDirection.Input
			ParFuelTypeId.Direction = ParameterDirection.Input
			ParPhoneNumber.Direction = ParameterDirection.Input
			ParWifiSSId.Direction = ParameterDirection.Input
			ParTransactionDate.Direction = ParameterDirection.Input
			ParTransactionId.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParTransactionFrom.Direction = ParameterDirection.Input
			ParpreviousOdometer.Direction = ParameterDirection.Input
			ParCurrentLat.Direction = ParameterDirection.Input
			ParCurrentLng.Direction = ParameterDirection.Input
			ParCurrentLocationAddress.Direction = ParameterDirection.Input
			ParVehicleNumber.Direction = ParameterDirection.Input
			ParDepartmentNumber.Direction = ParameterDirection.Input
			ParPersonPin.Direction = ParameterDirection.Input
			ParOther.Direction = ParameterDirection.Input
			ParHours.Direction = ParameterDirection.Input
			ParIsMissed.Direction = ParameterDirection.Input
			ParIsUpdate.Direction = ParameterDirection.Input
			ParTransactionStatus.Direction = ParameterDirection.Input
			ParHubId.Direction = ParameterDirection.Input
			ParPulses.Direction = ParameterDirection.Input

			ParVehicleName.Direction = ParameterDirection.Input
			ParDepartmentName.Direction = ParameterDirection.Input
			ParFuelTypeName.Direction = ParameterDirection.Input
			ParEmail.Direction = ParameterDirection.Input
			ParPersonName.Direction = ParameterDirection.Input
			ParCompanyName.Direction = ParameterDirection.Input

			ParOFFSite.Direction = ParameterDirection.Input
			ParcustomerID.Direction = ParameterDirection.Input

			ParRawOdometer.Direction = ParameterDirection.Input
			ParPreviousHours.Direction = ParameterDirection.Input

			ParVehicleId.Value = VehicleId
			ParSiteId.Value = SiteId
			ParPersonId.Value = PersonId
			ParCurrentOdometer.Value = CurrentOdometer
			ParFuelQuantity.Value = FuelQuantity
			ParFuelTypeId.Value = FuelTypeId
			ParPhoneNumber.Value = PhoneNumber
			ParWifiSSId.Value = WifiSSId
			ParTransactionDate.Value = TransactionDate
			ParTransactionId.Value = TransactionId
			ParUserId.Value = UserId
			ParTransactionFrom.Value = TransactionFrom
			ParpreviousOdometer.Value = previousOdometer
			ParCurrentLat.Value = CurrentLat
			ParCurrentLng.Value = CurrentLng
			ParCurrentLocationAddress.Value = CurrentLocationAddress
			ParVehicleNumber.Value = VehicleNumber
			ParDepartmentNumber.Value = DepartmentNumber
			ParPersonPin.Value = PersonPin
			ParOther.Value = Other
			ParHours.Value = IIf(Hours = -1, Nothing, Hours)
			ParIsMissed.Value = IsMissed
			ParIsUpdate.Value = IsUpdate
			ParTransactionStatus.Value = TransactionStatus
			ParHubId.Value = HubId
			ParPulses.Value = IIf(Pulses = -1, Nothing, Pulses)

			ParVehicleName.Value = VehicleName
			ParDepartmentName.Value = DepartmentName
			ParFuelTypeName.Value = FuelTypeName
			ParEmail.Value = Email
			ParPersonName.Value = PersonName
			ParCompanyName.Value = CompanyName

			ParOFFSite.Value = OFFSite
			ParcustomerID.Value = customerID

			ParRawOdometer.Value = RawOdometer
			ParPreviousHours.Value = IIf(PreviousHours = -1, Nothing, PreviousHours)

			parcollection(0) = ParVehicleId
			parcollection(1) = ParSiteId
			parcollection(2) = ParPersonId
			parcollection(3) = ParCurrentOdometer
			parcollection(4) = ParFuelQuantity
			parcollection(5) = ParFuelTypeId
			parcollection(6) = ParPhoneNumber
			parcollection(7) = ParWifiSSId
			parcollection(8) = ParTransactionDate
			parcollection(9) = ParTransactionId
			parcollection(10) = ParUserId
			parcollection(11) = ParTransactionFrom
			parcollection(12) = ParpreviousOdometer
			parcollection(13) = ParCurrentLat
			parcollection(14) = ParCurrentLng
			parcollection(15) = ParCurrentLocationAddress
			parcollection(16) = ParVehicleNumber
			parcollection(17) = ParDepartmentNumber
			parcollection(18) = ParPersonPin
			parcollection(19) = ParOther
			parcollection(20) = ParHours
			parcollection(21) = ParIsMissed
			parcollection(22) = ParIsUpdate
			parcollection(23) = ParTransactionStatus
			parcollection(24) = ParHubId
			parcollection(25) = ParPulses

			parcollection(26) = ParVehicleName
			parcollection(27) = ParDepartmentName
			parcollection(28) = ParFuelTypeName
			parcollection(29) = ParEmail
			parcollection(30) = ParPersonName
			parcollection(31) = ParCompanyName

			parcollection(32) = ParOFFSite
			parcollection(33) = ParcustomerID

			parcollection(34) = ParRawOdometer
			parcollection(35) = ParPreviousHours

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_InsertUpdateTransaction", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in InsertTransaction Exception is :" + ex.Message)
			Return result
		End Try

	End Function

	Public Function GetTransactionColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTransactionColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetTransactionsByCondition(Conditions As String, PersonId As Integer, RoleId As String, FromUndelete As Boolean) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(3) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			Param(3) = New SqlParameter("@FromUndelete", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = FromUndelete

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_GetTransactionsByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTransactionsByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function DeleteTransaction(ByVal TransactionId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)
			ParTransactionId.Direction = ParameterDirection.Input
			ParTransactionId.Value = TransactionId
			parcollection(0) = ParTransactionId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteTransaction Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetTransactionById(TransactionId As Integer, IsDeleted As Boolean) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@TransactionId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = TransactionId

			Param(1) = New SqlParameter("@IsDeleted", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsDeleted

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_GetTransactionById", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetTransactionById Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetTransactionIdByCondition(TransactionId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String,
												CustomerId As Integer, OFFSite As Boolean, IsDeleted As Boolean, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(11) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@TransactionId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = TransactionId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@OFFSite", SqlDbType.Bit)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = OFFSite

			Param(10) = New SqlParameter("@IsDeleted", SqlDbType.Bit)
			Param(10).Direction = ParameterDirection.Input
			Param(10).Value = IsDeleted

			Param(11) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(11).Direction = ParameterDirection.Input
			Param(11).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_GetTransactionIdByCondition", Param)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in GetTransactionIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function InsertPreAuthTransaction(PersonId As Integer, PhoneNumber As String, UserId As Integer, PreAuthCount As Integer) As Integer '
		Dim result As Integer
		result = 0
		Try
			Dim dal = New GeneralizedDAL()
			Dim parcollection(3) As SqlParameter


			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			Dim ParPhoneNumber = New SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 20)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParPreAuthCount = New SqlParameter("@PreAuthCount", SqlDbType.Int)


			ParPersonId.Direction = ParameterDirection.Input
			ParPhoneNumber.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParPreAuthCount.Direction = ParameterDirection.Input

			ParPersonId.Value = PersonId
			ParPhoneNumber.Value = PhoneNumber
			ParUserId.Value = UserId
			ParPreAuthCount.Value = PreAuthCount


			parcollection(0) = ParPersonId
			parcollection(1) = ParPhoneNumber
			parcollection(2) = ParUserId
			parcollection(3) = ParPreAuthCount

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_InsertPreAuthTransaction", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in InsertUpdatePreAuthTransaction Exception is :" + ex.Message)
			Return result
		End Try

	End Function

	Public Function GetPreAuthTransactionsByPersonId(PersonId As Integer, FromHandler As Boolean) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			Param(1) = New SqlParameter("@FromHandler", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = FromHandler

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_GetPreAuthTransactionsByPersonId", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetPreAuthTransactionsByPersonId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function UpdatePreAuthTransactions(SiteId As Integer, CurrentOdometer As Integer, FuelQuantity As Decimal, FuelTypeId As Integer,
											  WifiSSId As String, TransactionDate As DateTime, TransactionId As Integer, TransactionFrom As String, CurrentLat As String, CurrentLng As String,
											  VehicleNumber As String, Pulses As Integer) As Integer
		Dim result As Integer
		result = 0
		Try
			Dim dal = New GeneralizedDAL()
			Dim parcollection(11) As SqlParameter


			Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
			Dim ParCurrentOdometer = New SqlParameter("@CurrentOdometer", SqlDbType.Int)
			Dim ParFuelQuantity = New SqlParameter("@FuelQuantity", SqlDbType.Decimal)
			Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
			Dim ParWifiSSId = New SqlParameter("@WifiSSId", SqlDbType.NVarChar, 32)
			Dim ParTransactionDate = New SqlParameter("@TransactionDate", SqlDbType.DateTime)
			Dim ParTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)
			Dim ParTransactionFrom = New SqlParameter("@TransactionFrom", SqlDbType.NVarChar, 1)
			Dim ParCurrentLat = New SqlParameter("@CurrentLat", SqlDbType.NVarChar, 50)
			Dim ParCurrentLng = New SqlParameter("@CurrentLng", SqlDbType.NVarChar, 50)
			Dim ParVehicleNumber = New SqlParameter("@VehicleNumber ", SqlDbType.NVarChar, 10)
			Dim ParPulses = New SqlParameter("@Pulses ", SqlDbType.Int)


			ParSiteId.Direction = ParameterDirection.Input
			ParCurrentOdometer.Direction = ParameterDirection.Input
			ParFuelQuantity.Direction = ParameterDirection.Input
			ParFuelTypeId.Direction = ParameterDirection.Input
			ParWifiSSId.Direction = ParameterDirection.Input
			ParTransactionDate.Direction = ParameterDirection.Input
			ParTransactionId.Direction = ParameterDirection.Input
			ParTransactionFrom.Direction = ParameterDirection.Input
			ParCurrentLat.Direction = ParameterDirection.Input
			ParCurrentLng.Direction = ParameterDirection.Input
			ParVehicleNumber.Direction = ParameterDirection.Input
			ParPulses.Direction = ParameterDirection.Input

			ParSiteId.Value = SiteId
			ParCurrentOdometer.Value = CurrentOdometer
			ParFuelQuantity.Value = FuelQuantity
			ParFuelTypeId.Value = FuelTypeId
			ParWifiSSId.Value = WifiSSId
			ParTransactionDate.Value = TransactionDate
			ParTransactionId.Value = TransactionId
			ParTransactionFrom.Value = TransactionFrom
			ParCurrentLat.Value = CurrentLat
			ParCurrentLng.Value = CurrentLng
			ParVehicleNumber.Value = VehicleNumber
			ParPulses.Value = Pulses


			parcollection(0) = ParSiteId
			parcollection(1) = ParCurrentOdometer
			parcollection(2) = ParFuelQuantity
			parcollection(3) = ParFuelTypeId
			parcollection(4) = ParWifiSSId
			parcollection(5) = ParTransactionDate
			parcollection(6) = ParTransactionId
			parcollection(7) = ParTransactionFrom
			parcollection(8) = ParCurrentLat
			parcollection(9) = ParCurrentLng
			parcollection(10) = ParVehicleNumber
			parcollection(11) = ParPulses

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transactions_UpdatePreAuthTransactions", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdatePreAuthTransactions Exception is :" + ex.Message)
			Return result
		End Try

	End Function

	Public Function PostPriceInTransaction(PersonId As Integer, FuelTypeID As Integer, ResetPrice As Decimal, FromDate As String, ToDate As String, DateAdded As String, CustomerId As Integer, flag As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(7) As SqlParameter
			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			Dim ParFuelTypeID = New SqlParameter("@FuelTypeID", SqlDbType.Int)
			Dim ParResetPrice = New SqlParameter("@ResetPrice", SqlDbType.Decimal)
			Dim ParFromDate = New SqlParameter("@FromDate", SqlDbType.NVarChar, 25)
			Dim ParToDate = New SqlParameter("@ToDate", SqlDbType.NVarChar, 25)
			Dim ParDateAdded = New SqlParameter("@DateAdded", SqlDbType.NVarChar, 25)
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			Dim Parflag = New SqlParameter("@flag", SqlDbType.Int)

			ParPersonId.Direction = ParameterDirection.Input
			ParFuelTypeID.Direction = ParameterDirection.Input
			ParResetPrice.Direction = ParameterDirection.Input
			ParFromDate.Direction = ParameterDirection.Input
			ParToDate.Direction = ParameterDirection.Input
			ParDateAdded.Direction = ParameterDirection.Input
			ParCustomerId.Direction = ParameterDirection.Input
			Parflag.Direction = ParameterDirection.Input

			ParPersonId.Value = PersonId
			ParFuelTypeID.Value = FuelTypeID
			ParResetPrice.Value = ResetPrice
			ParFromDate.Value = String.Format("{0:yyy-MM-dd HH:mm}", Convert.ToDateTime(FromDate))
			ParToDate.Value = String.Format("{0:yyy-MM-dd HH:mm}", Convert.ToDateTime(ToDate))
			ParDateAdded.Value = DateAdded
			ParCustomerId.Value = CustomerId
			Parflag.Value = flag

			parcollection(0) = ParPersonId
			parcollection(1) = ParFuelTypeID
			parcollection(2) = ParResetPrice
			parcollection(3) = ParFromDate
			parcollection(4) = ParToDate
			parcollection(5) = ParDateAdded
			parcollection(6) = ParCustomerId
			parcollection(7) = Parflag

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction__Price_Cost_History", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in PostPriceInTransaction Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetHistoryPostPriceInTransaction(PersonId As Integer, FuelTypeID As Integer, ResetPrice As Decimal, FromDate As String, ToDate As String, DateAdded As String, CustomerId As Integer, flag As Integer) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()

			Dim parcollection(7) As SqlParameter
			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			Dim ParFuelTypeID = New SqlParameter("@FuelTypeID", SqlDbType.Int)
			Dim ParResetPrice = New SqlParameter("@ResetPrice", SqlDbType.Decimal)
			Dim ParFromDate = New SqlParameter("@FromDate", SqlDbType.NVarChar, 50)
			Dim ParToDate = New SqlParameter("@ToDate", SqlDbType.NVarChar, 50)
			Dim ParDateAdded = New SqlParameter("@DateAdded", SqlDbType.NVarChar, 50)
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			Dim Parflag = New SqlParameter("@flag", SqlDbType.Int)

			ParPersonId.Direction = ParameterDirection.Input
			ParFuelTypeID.Direction = ParameterDirection.Input
			ParResetPrice.Direction = ParameterDirection.Input
			ParFromDate.Direction = ParameterDirection.Input
			ParToDate.Direction = ParameterDirection.Input
			ParDateAdded.Direction = ParameterDirection.Input
			ParCustomerId.Direction = ParameterDirection.Input
			Parflag.Direction = ParameterDirection.Input

			ParPersonId.Value = PersonId
			ParFuelTypeID.Value = FuelTypeID
			ParResetPrice.Value = ResetPrice
			ParFromDate.Value = FromDate
			ParToDate.Value = ToDate
			ParDateAdded.Value = DateAdded
			ParCustomerId.Value = CustomerId
			Parflag.Value = flag

			parcollection(0) = ParPersonId
			parcollection(1) = ParFuelTypeID
			parcollection(2) = ParResetPrice
			parcollection(3) = ParFromDate
			parcollection(4) = ParToDate
			parcollection(5) = ParDateAdded
			parcollection(6) = ParCustomerId
			parcollection(7) = Parflag


			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction__Price_Cost_History", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetHistoryPostPriceInTransaction Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Sub UpdateTransactionCost(TransactionId As Integer, TransactionCost As Decimal)
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(1) As SqlParameter
			Dim ParTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)
			Dim ParTransactionCost = New SqlParameter("@TransactionCost", SqlDbType.Decimal)

			ParTransactionId.Direction = ParameterDirection.Input
			ParTransactionCost.Direction = ParameterDirection.Input


			ParTransactionId.Value = TransactionId
			ParTransactionCost.Value = TransactionCost

			parcollection(0) = ParTransactionId
			parcollection(1) = ParTransactionCost

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_UpdateTransactionCost", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in UpdateTransactionCost Exception is :" + ex.Message)
		Finally

		End Try

	End Sub

    Public Function UpdateAndUnDeleteTransaction(VehicleId As Integer, SiteId As Integer, PersonId As Integer, CurrentOdometer As Integer,
                                            FuelQuantity As Decimal, FuelTypeId As Integer, PhoneNumber As String, WifiSSId As String,
                                            TransactionDate As DateTime, TransactionId As Integer, UserId As Integer, TransactionFrom As String, previousOdometer As Integer, VehicleNumber As String,
                                            DepartmentNumber As String, PersonPin As String, Other As String, Hours As Integer, PreviousHours As Integer, IsMissed As Boolean, IsUpdate As Boolean, TransactionStatus As Integer, HubId As Integer, Pulses As Integer,
                                            VehicleName As String, DepartmentName As String, FuelTypeName As String, Email As String, PersonName As String, CompanyName As String, OFFSite As Boolean, customerID As Integer, IsManuallyEdit As Boolean) As Integer '
        Dim result As Integer
        result = 0
        Try
            Dim dal = New GeneralizedDAL()
            Dim parcollection(32) As SqlParameter

            Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
            Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
            Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
            Dim ParCurrentOdometer = New SqlParameter("@CurrentOdometer", SqlDbType.Int)
            Dim ParFuelQuantity = New SqlParameter("@FuelQuantity", SqlDbType.Decimal)
            Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
            Dim ParPhoneNumber = New SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 20)
            Dim ParWifiSSId = New SqlParameter("@WifiSSId", SqlDbType.NVarChar, 50)
            Dim ParTransactionDate = New SqlParameter("@TransactionDate", SqlDbType.DateTime)
            Dim ParTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)
            Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
            Dim ParTransactionFrom = New SqlParameter("@TransactionFrom", SqlDbType.NVarChar, 1)
            Dim ParpreviousOdometer = New SqlParameter("@previousOdometer", SqlDbType.Int)
            Dim ParVehicleNumber = New SqlParameter("@VehicleNumber", SqlDbType.NVarChar, 10)
            Dim ParDepartmentNumber = New SqlParameter("@DepartmentNumber", SqlDbType.NVarChar, 10)
            Dim ParPersonPin = New SqlParameter("@PersonPin", SqlDbType.NVarChar, 20)
            Dim ParOther = New SqlParameter("@Other", SqlDbType.NVarChar, 2000)
            Dim ParHours = New SqlParameter("@Hours", SqlDbType.Int)
            Dim ParPreviousHours = New SqlParameter("@PreviousHours", SqlDbType.Int)
            Dim ParIsMissed = New SqlParameter("@IsMissed", SqlDbType.Bit)
            Dim ParIsUpdate = New SqlParameter("@IsUpdate", SqlDbType.Bit)
            Dim ParTransactionStatus = New SqlParameter("@TransactionStatus", SqlDbType.Int)
            Dim ParHubId = New SqlParameter("@HubId", SqlDbType.Int)
            Dim ParPulses = New SqlParameter("@Pulses", SqlDbType.Int)
            Dim ParVehicleName = New SqlParameter("@VehicleName", SqlDbType.NVarChar, 25)
            Dim ParDepartmentName = New SqlParameter("@DepartmentName", SqlDbType.NVarChar, 20)
            Dim ParFuelTypeName = New SqlParameter("@FuelTypeName", SqlDbType.NVarChar, 20)
            Dim ParEmail = New SqlParameter("@Email", SqlDbType.NVarChar, 256)
            Dim ParPersonName = New SqlParameter("@PersonName", SqlDbType.NVarChar, 30)
            Dim ParCompanyName = New SqlParameter("@CompanyName", SqlDbType.NVarChar, 50)
            Dim ParOFFSite = New SqlParameter("@OFFSite", SqlDbType.Bit)
            Dim ParcustomerID = New SqlParameter("@customerID", SqlDbType.Int)
            Dim ParIsManuallyEdit = New SqlParameter("@IsManuallyEdit", SqlDbType.Bit)

            ParVehicleId.Direction = ParameterDirection.Input
            ParSiteId.Direction = ParameterDirection.Input
            ParPersonId.Direction = ParameterDirection.Input
            ParCurrentOdometer.Direction = ParameterDirection.Input
            ParFuelQuantity.Direction = ParameterDirection.Input
            ParFuelTypeId.Direction = ParameterDirection.Input
            ParPhoneNumber.Direction = ParameterDirection.Input
            ParWifiSSId.Direction = ParameterDirection.Input
            ParTransactionDate.Direction = ParameterDirection.Input
            ParTransactionId.Direction = ParameterDirection.Input
            ParUserId.Direction = ParameterDirection.Input
            ParTransactionFrom.Direction = ParameterDirection.Input
            ParpreviousOdometer.Direction = ParameterDirection.Input
            ParVehicleNumber.Direction = ParameterDirection.Input
            ParDepartmentNumber.Direction = ParameterDirection.Input
            ParPersonPin.Direction = ParameterDirection.Input
            ParOther.Direction = ParameterDirection.Input
            ParHours.Direction = ParameterDirection.Input
            ParPreviousHours.Direction = ParameterDirection.Input
            ParIsMissed.Direction = ParameterDirection.Input
            ParIsUpdate.Direction = ParameterDirection.Input
            ParTransactionStatus.Direction = ParameterDirection.Input
            ParHubId.Direction = ParameterDirection.Input
            ParPulses.Direction = ParameterDirection.Input
            ParVehicleName.Direction = ParameterDirection.Input
            ParDepartmentName.Direction = ParameterDirection.Input
            ParFuelTypeName.Direction = ParameterDirection.Input
            ParEmail.Direction = ParameterDirection.Input
            ParPersonName.Direction = ParameterDirection.Input
            ParCompanyName.Direction = ParameterDirection.Input
            ParOFFSite.Direction = ParameterDirection.Input
            ParcustomerID.Direction = ParameterDirection.Input
            ParIsManuallyEdit.Direction = ParameterDirection.Input

            ParVehicleId.Value = VehicleId
            ParSiteId.Value = SiteId
            ParPersonId.Value = PersonId
            ParCurrentOdometer.Value = CurrentOdometer
            ParFuelQuantity.Value = FuelQuantity
            ParFuelTypeId.Value = FuelTypeId
            ParPhoneNumber.Value = PhoneNumber
            ParWifiSSId.Value = WifiSSId
            ParTransactionDate.Value = TransactionDate
            ParTransactionId.Value = TransactionId
            ParUserId.Value = UserId
            ParTransactionFrom.Value = TransactionFrom
            ParpreviousOdometer.Value = previousOdometer
            ParVehicleNumber.Value = VehicleNumber
            ParDepartmentNumber.Value = DepartmentNumber
            ParPersonPin.Value = PersonPin
            ParOther.Value = Other
            ParHours.Value = IIf(Hours = -1, Nothing, Hours)
            ParPreviousHours.Value = IIf(PreviousHours = -1, Nothing, PreviousHours)
            ParIsMissed.Value = IsMissed
            ParIsUpdate.Value = IsUpdate
            ParTransactionStatus.Value = TransactionStatus
            ParHubId.Value = HubId
            ParPulses.Value = IIf(Pulses = -1, Nothing, Pulses)
            ParVehicleName.Value = VehicleName
            ParDepartmentName.Value = DepartmentName
            ParFuelTypeName.Value = FuelTypeName
            ParEmail.Value = Email
            ParPersonName.Value = PersonName
            ParCompanyName.Value = CompanyName
            ParOFFSite.Value = OFFSite
            ParcustomerID.Value = customerID
            ParIsManuallyEdit.Value = IsManuallyEdit

            parcollection(0) = ParVehicleId
            parcollection(1) = ParSiteId
            parcollection(2) = ParPersonId
            parcollection(3) = ParCurrentOdometer
            parcollection(4) = ParFuelQuantity
            parcollection(5) = ParFuelTypeId
            parcollection(6) = ParPhoneNumber
            parcollection(7) = ParWifiSSId
            parcollection(8) = ParTransactionDate
            parcollection(9) = ParTransactionId
            parcollection(10) = ParUserId
            parcollection(11) = ParTransactionFrom
            parcollection(12) = ParpreviousOdometer
            parcollection(13) = ParVehicleNumber
            parcollection(14) = ParDepartmentNumber
            parcollection(15) = ParPersonPin
            parcollection(16) = ParOther
            parcollection(17) = ParHours
            parcollection(18) = ParIsMissed
            parcollection(19) = ParIsUpdate
            parcollection(20) = ParTransactionStatus
            parcollection(21) = ParHubId
            parcollection(22) = ParPulses
            parcollection(23) = ParVehicleName
            parcollection(24) = ParDepartmentName
            parcollection(25) = ParFuelTypeName
            parcollection(26) = ParEmail
            parcollection(27) = ParPersonName
            parcollection(28) = ParCompanyName
            parcollection(29) = ParOFFSite
            parcollection(30) = ParcustomerID
            parcollection(31) = ParIsManuallyEdit
            parcollection(32) = ParPreviousHours

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_UpdateAndUnDeleteTransaction", parcollection)

            Return result
        Catch ex As Exception
            log.Error("Error occurred in InsertTransaction Exception is :" + ex.Message)
            Return result
        End Try

    End Function
    Public Function SetIsTransactionMailSent(ByVal TransactionId As Integer, Quantity As Decimal) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)
			ParTransactionId.Direction = ParameterDirection.Input
			ParTransactionId.Value = TransactionId
			parcollection(0) = ParTransactionId

			Dim ParQuantity = New SqlParameter("@Quantity", SqlDbType.Decimal)
			ParQuantity.Direction = ParameterDirection.Input
			ParQuantity.Value = Quantity
			parcollection(1) = ParQuantity

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_SetIsTransactionMailSent", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in SetIsTransactionMailSent Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetFluidSecureLinkForSearch(Conditions As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_GetFluidSecureLinkForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetFluidSecureLinkForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

#End Region

#Region "Department"

	Public Function DeptIDExists(ByVal DeptNo As String, DeptId As Integer, CustomerId As Integer, DeptNAME As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(3) As SqlParameter
			Dim ParDeptNo = New SqlParameter("@DeptNUMBER", SqlDbType.NVarChar, 10)
			ParDeptNo.Direction = ParameterDirection.Input
			ParDeptNo.Value = DeptNo
			parcollection(0) = ParDeptNo

			Dim ParDeptId = New SqlParameter("@DeptId", SqlDbType.Int)
			ParDeptId.Direction = ParameterDirection.Input
			ParDeptId.Value = DeptId
			parcollection(1) = ParDeptId

			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(2) = ParCustomerId

			Dim ParDeptNAME = New SqlParameter("@DeptNAME", SqlDbType.NVarChar, 20)
			ParDeptNAME.Direction = ParameterDirection.Input
			ParDeptNAME.Value = DeptNAME
			parcollection(3) = ParDeptNAME

			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_DeptCheckNumberExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in DeptIDExists Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function SaveUpdateDept(DeptID As Integer, DeptName As String, DeptNumber As String, DeptAddress1 As String, DeptAddress2 As String, AccountNo As String, CustomerId As Integer, ExportCode As String, UserId As Integer,
								   VehSurSum As Decimal, DeptSurSum As Decimal, VehSurPer As Decimal, DeptSurPer As Decimal, SurchargeType As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(13) As SqlParameter
			Dim ParDeptID = New SqlParameter("@DeptID", SqlDbType.Int)
			Dim ParDeptNAME = New SqlParameter("@DeptNAME", SqlDbType.NVarChar, 20)
			Dim ParDeptNUMBER = New SqlParameter("@DeptNUMBER", SqlDbType.NVarChar, 10)
			Dim ParDeptADDRESS1 = New SqlParameter("@DeptADDRESS1", SqlDbType.VarChar, 25)
			Dim ParDeptADDRESS2 = New SqlParameter("@DeptADDRESS2", SqlDbType.VarChar, 25)
			'Dim ParSURCHARGE = New SqlParameter("@SURCHARGE", SqlDbType.Float)
			'Dim ParCODE = New SqlParameter("@CODE", SqlDbType.VarChar, 30)
			Dim ParACCT_No = New SqlParameter("@ACCT_No", SqlDbType.NVarChar, 10)
			'Dim ParFlag = New SqlParameter("@Flag", SqlDbType.VarChar, 5)

			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParExportCode = New SqlParameter("@ExportCode", SqlDbType.VarChar, 30)

			Dim ParSurchargeType = New SqlParameter("@SurchargeType", SqlDbType.Int)
			Dim ParVehSurSum = New SqlParameter("@VehSurSum", SqlDbType.Decimal)
			Dim ParDeptSurSum = New SqlParameter("@DeptSurSum", SqlDbType.Decimal)
			Dim ParVehSurPer = New SqlParameter("@VehSurPer", SqlDbType.Decimal)
			Dim ParDeptSurPer = New SqlParameter("@DeptSurPer", SqlDbType.Decimal)

			ParDeptID.Direction = ParameterDirection.Input
			ParDeptNAME.Direction = ParameterDirection.Input
			ParDeptNUMBER.Direction = ParameterDirection.Input
			ParDeptADDRESS1.Direction = ParameterDirection.Input
			ParDeptADDRESS2.Direction = ParameterDirection.Input
			'ParSURCHARGE.Direction = ParameterDirection.Input
			'ParCODE.Direction = ParameterDirection.Input
			ParACCT_No.Direction = ParameterDirection.Input
			'ParFlag.Direction = ParameterDirection.Input
			ParCustomerId.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParExportCode.Direction = ParameterDirection.Input
			ParSurchargeType.Direction = ParameterDirection.Input
			ParVehSurSum.Direction = ParameterDirection.Input
			ParDeptSurSum.Direction = ParameterDirection.Input
			ParVehSurPer.Direction = ParameterDirection.Input
			ParDeptSurPer.Direction = ParameterDirection.Input


			'If (Label1.Text = "Edit Department Information") Then 'Department Edit Screen
			'    ParFlag.Value = "Edit"
			'    ParDeptID.Value = Convert.ToInt32(Val(txtDeptNoHide.Text.Trim()))
			'Else
			'    ParFlag.Value = "ADD"
			'    ParDeptID.Value = 0
			'End If
			ParDeptID.Value = DeptID
			ParDeptNAME.Value = DeptName
			'ParDeptNUMBER.Value = txtDeptNo.Text.Trim()
			ParDeptNUMBER.Value = DeptNumber
			ParDeptADDRESS1.Value = DeptAddress1
			ParDeptADDRESS2.Value = DeptAddress2
			'ParSURCHARGE.Value = Convert.ToDouble(Val(txtSurchage.Text.Trim()))
			'ParCODE.Value = txtUploadcode.Text.Trim()
			ParACCT_No.Value = AccountNo
			ParCustomerId.Value = CustomerId
			ParUserId.Value = UserId
			ParExportCode.Value = ExportCode
			ParSurchargeType.Value = SurchargeType
			ParVehSurSum.Value = VehSurSum
			ParDeptSurSum.Value = DeptSurSum
			ParVehSurPer.Value = VehSurPer
			ParDeptSurPer.Value = DeptSurPer


			parcollection(0) = ParDeptID
			parcollection(1) = ParDeptNAME
			parcollection(2) = ParDeptNUMBER
			parcollection(3) = ParDeptADDRESS1
			parcollection(4) = ParDeptADDRESS2
			'parcollection(5) = ParSURCHARGE
			'parcollection(6) = ParCODE
			'parcollection(7) = ParACCT_ID
			'parcollection(8) = ParFlag
			parcollection(5) = ParACCT_No
			parcollection(6) = ParCustomerId
			parcollection(7) = ParUserId
			parcollection(8) = ParExportCode
			parcollection(9) = ParSurchargeType
			parcollection(10) = ParVehSurSum
			parcollection(11) = ParDeptSurSum
			parcollection(12) = ParVehSurPer
			parcollection(13) = ParDeptSurPer

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_DeptInsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateDept Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetDepartments(PersonId As Integer, RoleId As String, CustomerId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = PersonId

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId

			parcollection(2) = New SqlParameter("@CustomerId", SqlDbType.Int)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_DeptDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetDepartments Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetDeptbyConditions(Conditions As String, PersonId As Integer, RoleId As String) As DataTable

		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = PersonId

			parcollection(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Dept_GetDeptbyConditions", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetDeptbyConditions Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetDeptbyId(DeptId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@DeptId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = DeptId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_DeptbyDeptId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetDeptbyId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetDeptIdByCondition(DeptId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId As Integer, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@DeptId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = DeptId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Dept_GetDeptIdByCondition", Param)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in GetDeptIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function DeleteDept(ByVal DeptID As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParDeptID = New SqlParameter("@DeptID", SqlDbType.Int)
			ParDeptID.Direction = ParameterDirection.Input
			ParDeptID.Value = DeptID
			parcollection(0) = ParDeptID

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Dept_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteDept Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetDeptColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Dept_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetDeptColumnNameForSearch Exception is :" + ex.Message)

			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetDepartmentsByCustomerId(CustomerId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@CustomerId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetDepartmentsByCustomerId", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetDepartmentsByCustomerId Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

#End Region

#Region "Customer"

	Public Function GetCustomerDetailsByPersonID(PersonId As Integer, RoleID As String, CustomerId As Integer) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}

			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			ParPersonId.Direction = ParameterDirection.Input
			ParPersonId.Value = PersonId
			parcollection(0) = ParPersonId

			Dim ParRoleID = New SqlParameter("@RoleId", SqlDbType.NVarChar, 200)
			ParRoleID.Direction = ParameterDirection.Input
			ParRoleID.Value = RoleID
			parcollection(1) = ParRoleID

			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.NVarChar, 200)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(2) = ParCustomerId

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetCustomersByPersonID", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetCustomerDetailsByPersonID Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetCustomerDetailsByName(customerName As String) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}

			Dim ParCustomerName = New SqlParameter("@CustomerName", SqlDbType.NVarChar, 200)
			ParCustomerName.Direction = ParameterDirection.Input
			ParCustomerName.Value = customerName
			parcollection(0) = ParCustomerName

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetCustomerDetailsByCustomerName", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetCustomerDetailsByName Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function CustNameIsExists(ByVal DeptNo As String, CustomerId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParDeptNo = New SqlParameter("@CustomerName", SqlDbType.NVarChar, 10)
			ParDeptNo.Direction = ParameterDirection.Input
			ParDeptNo.Value = DeptNo
			parcollection(0) = ParDeptNo

			Dim ParCustomerIdId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerIdId.Direction = ParameterDirection.Input
			ParCustomerIdId.Value = CustomerId
			parcollection(1) = ParCustomerIdId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Customer_CheckCustomerExist", parcollection)



		Catch ex As Exception
			log.Error("Error occurred in CustNameIsExists Exception is :" + ex.Message)
			Return True

		End Try
	End Function

    Public Function SaveUpdateCustomer(CustomerID As Integer, CustomerName As String, ContactName As String, ContactNumber As String, ContactAddress As String, ExportCode As String, UserId As Integer,
                                       IsLoginRequire As Boolean, IsOdometerRequire As Boolean,
                                       IsDepartmentRequire As Boolean, IsPersonnelPINRequire As Boolean,
                                       IsOtherRequire As Boolean, OtherLabel As String, IsActive As Boolean, IsVehicleNumberRequire As Boolean,
                                       Optional Costing As Integer = 1, Optional BeginningHostingDate As DateTime = Nothing, Optional EndingHostingDate As DateTime = Nothing) As Integer
        Try
            Dim result As Integer
            Dim dal = New GeneralizedDAL()
            Dim parcollection(17) As SqlParameter
            Dim ParCustomerID = New SqlParameter("@CustomerID", SqlDbType.Int)
            Dim ParCustomerName = New SqlParameter("@CustomerName", SqlDbType.NVarChar, 50)
            Dim ParContactName = New SqlParameter("@ContactName", SqlDbType.NVarChar, 30)
            Dim ParContactNumber = New SqlParameter("@ContactNumber", SqlDbType.NVarChar, 15)
            Dim ParContactAddress = New SqlParameter("@ContactAddress", SqlDbType.NVarChar, 50)
            Dim ParExportCode = New SqlParameter("@ExportCode", SqlDbType.NVarChar, 50)
            Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
            Dim ParIsLoginRequire = New SqlParameter("@IsLoginRequire", SqlDbType.Bit)
            Dim ParIsOdometerRequire = New SqlParameter("@IsOdometerRequire", SqlDbType.Bit)
            Dim ParIsDepartmentRequire = New SqlParameter("@IsDepartmentRequire", SqlDbType.Bit)
            Dim ParIsPersonnelPINRequire = New SqlParameter("@IsPersonnelPINRequire", SqlDbType.Bit)
            Dim ParIsOtherRequire = New SqlParameter("@IsOtherRequire", SqlDbType.Bit)
            Dim ParOtherLabel = New SqlParameter("@OtherLabel", SqlDbType.NVarChar, 50)
            Dim ParIsActive = New SqlParameter("@IsActive", SqlDbType.Bit)
            Dim ParIsVehicleNumberRequire = New SqlParameter("@IsVehicleNumberRequire", SqlDbType.Bit)
            Dim ParCosting = New SqlParameter("@CostingMethod", SqlDbType.Int)
            Dim ParBeginningHostingDate = New SqlParameter("@BeginningHostingDate", SqlDbType.DateTime)
            Dim ParEndingHostingDate = New SqlParameter("@EndingHostingDate", SqlDbType.DateTime)

            ParCustomerID.Direction = ParameterDirection.Input
            ParCustomerName.Direction = ParameterDirection.Input
            ParContactName.Direction = ParameterDirection.Input
            ParContactNumber.Direction = ParameterDirection.Input
            ParContactAddress.Direction = ParameterDirection.Input
            ParUserId.Direction = ParameterDirection.Input
            ParIsLoginRequire.Direction = ParameterDirection.Input
            ParIsOdometerRequire.Direction = ParameterDirection.Input
            ParIsDepartmentRequire.Direction = ParameterDirection.Input
            ParIsPersonnelPINRequire.Direction = ParameterDirection.Input
            ParIsOtherRequire.Direction = ParameterDirection.Input
            ParOtherLabel.Direction = ParameterDirection.Input
            ParIsActive.Direction = ParameterDirection.Input
            ParIsVehicleNumberRequire.Direction = ParameterDirection.Input
            ParCosting.Direction = ParameterDirection.Input
            ParBeginningHostingDate.Direction = ParameterDirection.Input
            ParEndingHostingDate.Direction = ParameterDirection.Input


            ParCustomerID.Value = CustomerID
            ParCustomerName.Value = CustomerName
            ParContactName.Value = ContactName
            ParContactNumber.Value = ContactNumber
            ParContactAddress.Value = ContactAddress
            ParExportCode.Value = ExportCode
            ParUserId.Value = UserId
            ParIsLoginRequire.Value = IsLoginRequire
            ParIsOdometerRequire.Value = IsOdometerRequire
            ParIsDepartmentRequire.Value = IsDepartmentRequire
            ParIsPersonnelPINRequire.Value = IsPersonnelPINRequire
            ParIsOtherRequire.Value = IsOtherRequire
            ParOtherLabel.Value = OtherLabel
            ParIsActive.Value = IsActive
            ParIsVehicleNumberRequire.Value = IsVehicleNumberRequire
            ParCosting.Value = Costing
            ParBeginningHostingDate.Value = IIf(BeginningHostingDate = Nothing, DateTime.Now, BeginningHostingDate)
            ParEndingHostingDate.Value = IIf(EndingHostingDate = Nothing, DateTime.Now, EndingHostingDate)

            parcollection(0) = ParCustomerID
            parcollection(1) = ParCustomerName
            parcollection(2) = ParContactName
            parcollection(3) = ParContactNumber
            parcollection(4) = ParContactAddress
            parcollection(5) = ParExportCode

            parcollection(6) = ParUserId
            parcollection(7) = ParIsLoginRequire
            parcollection(8) = ParIsOdometerRequire

            parcollection(9) = ParIsDepartmentRequire
            parcollection(10) = ParIsPersonnelPINRequire
            parcollection(11) = ParIsOtherRequire
            parcollection(12) = ParOtherLabel
            parcollection(13) = ParCosting
            parcollection(14) = ParBeginningHostingDate
            parcollection(15) = ParEndingHostingDate
            parcollection(16) = ParIsActive
            parcollection(17) = ParIsVehicleNumberRequire
            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Customer_InsertUpdate", parcollection)

            Return result
        Catch ex As Exception
            log.Error("Error occurred in SaveUpdateCustomer Exception is :" + ex.Message)
            Return 0

        Finally

        End Try

        Return 0

    End Function

    Public Function GetCustomerDetails() As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter() {}

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetCustomerDetails", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetCustomerDetails Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetCustByConditions(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = PersonId

			parcollection(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetCustByConditions", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetCustByConditions Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetCustomerId(CustomerId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetCustById", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetCustomerId Exception is :" + ex.Message)

			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetCustomerAdmin(customerId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = customerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetCustAdminById", Param)

			Return ds
		Catch ex As Exception
			log.Error("Error occurred in GetCustomerAdmin Exception is :" + ex.Message)
			Return Nothing
		End Try
	End Function

	Public Function GetCustIdByCondition(CustomerId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId1 As String, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = CustomerId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId1", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId1

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Customer_GetCustIdByCondition", Param)

			Return result

		Catch ex As Exception

			log.Error("Error occurred in GetCustIdByCondition Exception is :" + ex.Message)

			Return 0
		Finally

		End Try
	End Function

	Public Function DeleteCustomer(ByVal CustomerId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(0) = ParCustomerId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Customer_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteCustomer Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetCustmerColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetCustmerColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function SavePersonnalVehicleMappingAgainstCustomer(CustomerId As Integer, ByVal PersonId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.NVarChar, 10)
			ParPersonId.Direction = ParameterDirection.Input
			ParPersonId.Value = PersonId
			parcollection(0) = ParPersonId

			Dim ParCustomerIdId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerIdId.Direction = ParameterDirection.Input
			ParCustomerIdId.Value = CustomerId
			parcollection(1) = ParCustomerIdId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Company_InsertPersonnelVehicleMapping", parcollection)



		Catch ex As Exception
			log.Error("Error occurred in SavePersonnalVehicleMappingAgainstCustomer Exception is :" + ex.Message)
			Return True

		End Try
	End Function


	Public Function GetTanksWithoutDeliveryByCustomerId(customerId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = customerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Customer_GetTanksWithoutDeliveryByCustomerId", Param)

			Return ds
		Catch ex As Exception
			log.Error("Error occurred in GetTanksWithoutDeliveryByCustomerId Exception is :" + ex.Message)
			Return Nothing
		End Try
	End Function

	Public Function ActiveInActiveCompany(ByVal CustomerId As Integer, ActiveOrInactiveValue As Boolean, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(0) = ParCustomerId

			Dim ParcheckActiveOrInactiveValue = New SqlParameter("@ActiveOrInactiveValue", SqlDbType.Bit)
			ParcheckActiveOrInactiveValue.Direction = ParameterDirection.Input
			ParcheckActiveOrInactiveValue.Value = ActiveOrInactiveValue
			parcollection(1) = ParcheckActiveOrInactiveValue

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(2) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Customer_ActiveInActive", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in ActiveInActiveCompany Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

#End Region

#Region "Fuel"

	Public Function SaveUpdateFuel(FuelTypeID As Integer, FuelType As String, ExportCode As String, UserId As Integer, CompanyId As Integer, ProductPrice As Decimal) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(5) As SqlParameter
			Dim ParFuelTypeID = New SqlParameter("@FuelTypeID", SqlDbType.Int)
			Dim ParFuelType = New SqlParameter("@FuelType", SqlDbType.NVarChar, 20)

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParExportCode = New SqlParameter("@ExportCode", SqlDbType.NVarChar, 50)
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			Dim ParProductPrice = New SqlParameter("@ProductPrice", SqlDbType.Decimal)

			ParFuelTypeID.Direction = ParameterDirection.Input
			ParFuelType.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParExportCode.Direction = ParameterDirection.Input
			ParCompanyId.Direction = ParameterDirection.Input
			ParProductPrice.Direction = ParameterDirection.Input

			ParFuelTypeID.Value = FuelTypeID
			ParFuelType.Value = FuelType
			ParUserId.Value = UserId
			ParExportCode.Value = ExportCode
			ParCompanyId.Value = CompanyId
			ParProductPrice.Value = ProductPrice

			parcollection(0) = ParFuelTypeID
			parcollection(1) = ParFuelType
			parcollection(2) = ParUserId
			parcollection(3) = ParExportCode
			parcollection(4) = ParCompanyId
			parcollection(5) = ParProductPrice

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Fuel_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateFuel Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetFuelDetails(CompanyId As Integer) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}

			parcollection(0) = New SqlParameter("@CompanyId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = CompanyId

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Fuel_GetFuelDetails", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFuelDetails Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetFuelByType(FuelType As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@FuelType", SqlDbType.NVarChar, 30)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = FuelType

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Fuel_GetFuelByType", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFuelByType Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetFuelByTypeId(FuelTypeID As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@FuelTypeID", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = FuelTypeID

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Fuel_GetFuelByTypeId", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetFuelByTypeId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetFuelTypeIdByCondition(FuelTypeID As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId As String, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@FuelTypeID", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = FuelTypeID

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.NVarChar)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Fuel_GetFuelTypeIdByCondition", Param)

			Return result

		Catch ex As Exception

			log.Error("Error occurred in GetFuelTypeIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function DeleteFuel(ByVal FuelTypeID As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParFuelTypeID = New SqlParameter("@FuelTypeID", SqlDbType.Int)
			ParFuelTypeID.Direction = ParameterDirection.Input
			ParFuelTypeID.Value = FuelTypeID
			parcollection(0) = ParFuelTypeID

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Fuel_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteFuel Exception is :" + ex.Message)
			Return 0
		End Try
	End Function


	Public Function GetFliudTypeByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(2) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Fuel_GetFuelByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFuelByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetFluidColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Fuel_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFluidColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function CheckFuelTypeExist(ByVal fueltype As String, FuelTypeId As Integer, CustomerId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParFuelType = New SqlParameter("@FuelType", SqlDbType.NVarChar, 10)
			ParFuelType.Direction = ParameterDirection.Input
			ParFuelType.Value = fueltype
			parcollection(0) = ParFuelType

			Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
			ParFuelTypeId.Direction = ParameterDirection.Input
			ParFuelTypeId.Value = FuelTypeId
			parcollection(1) = ParFuelTypeId

			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(2) = ParCustomerId


			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_Fuel_CheckFuelTypeExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckFuelTypeExist Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

#End Region

#Region "Personnel"

	Public Function InsertPersonTimings(dtTimings As DataTable, PersonId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtPersonTimings"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtTimings
			Param(0).TypeName = "dbo.PersonnelTimings"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@PersonId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId


			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Personnel_InsertPersonnelTimings", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertPersonTimings Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function CheckDuplicateIMEI_UDID(IMEI_UDID As String, PersonId As Integer) As Boolean
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@IMEI_UDID"
			Param(0).SqlDbType = SqlDbType.NVarChar
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IMEI_UDID

			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			ParPersonId.Direction = ParameterDirection.Input
			ParPersonId.Value = PersonId
			Param(1) = ParPersonId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Personnel_CheckDuplicateIMEI_UDID", Param)
			'Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Personnel_CheckDuplicateIMEI_UDID", Param)

			'Return result
		Catch ex As Exception
			log.Error("Error occurred in CheckDuplicateIMEI_UDID Exception is :" + ex.Message)

			Return True
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetPersonnelTimings(PersonId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonnelTimings", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonnelTimings Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function PhoneNumberIsExists(ByVal PhoneNumber As String, PersonId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParPhoneNumber = New SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 2000)
			ParPhoneNumber.Direction = ParameterDirection.Input
			ParPhoneNumber.Value = PhoneNumber
			parcollection(0) = ParPhoneNumber

			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			ParPersonId.Direction = ParameterDirection.Input
			ParPersonId.Value = PersonId
			parcollection(1) = ParPersonId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Personnel_CheckPhoneNumberExist", parcollection)



		Catch ex As Exception
			log.Error("Error occurred in PhoneNumberIsExists Exception is :" + ex.Message)
			Return True

		End Try
	End Function

	Public Function CheckPinNumberExist(ByVal PinNumber As String, PersonId As Integer, CompanyId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParPinNumber = New SqlParameter("@PinNumber", SqlDbType.NVarChar, 20)
			ParPinNumber.Direction = ParameterDirection.Input
			ParPinNumber.Value = PinNumber
			parcollection(0) = ParPinNumber

			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			ParPersonId.Direction = ParameterDirection.Input
			ParPersonId.Value = PersonId
			parcollection(1) = ParPersonId

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId
			parcollection(2) = ParCompanyId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Personnel_CheckPinNumberExist", parcollection)



		Catch ex As Exception
			log.Error("Error occurred in CheckPinNumberExist Exception is :" + ex.Message)
			Return True

		End Try
	End Function

	Public Function GetPersonnelDetails() As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter() {}

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonnelDetails", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonnelDetails Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetTransactionDataByPersonId(PersonId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@personId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = PersonId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_GetTransactionDataByPersonId", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTransactionDataByPersonId Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetPersonnelByNameAndNumberAndEmail(Conditions As String, PersonId As Integer, RoleId As String, Optional FromService As Boolean = False) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(3) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = PersonId

			parcollection(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = RoleId

			parcollection(3) = New SqlParameter("@FromService", SqlDbType.Bit)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = FromService


			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonnelByNameAndNumberAndEmail", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonnelByNameAndNumberAndEmail Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function DeletePersonnel(ByVal PersonnelID As Integer, UniqueUserId As String, UserId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParPersonnelID = New SqlParameter("@PersonnelID", SqlDbType.Int)
			ParPersonnelID.Direction = ParameterDirection.Input
			ParPersonnelID.Value = PersonnelID
			parcollection(0) = ParPersonnelID

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim ParUniqueUserId = New SqlParameter("@UniqueUserId", SqlDbType.NVarChar, 128)
			ParUniqueUserId.Direction = ParameterDirection.Input
			ParUniqueUserId.Value = UniqueUserId
			parcollection(2) = ParUniqueUserId

			dal.ExecuteStoredProcedureGetBoolean("usp_tt_Personnel_Delete", parcollection)
			Return True
		Catch ex As Exception
			log.Error("Error occurred in DeletePersonnel Exception is :" + ex.Message)
			Return False
		End Try
	End Function

	Public Function GetPersonnelByPersonIdAndId(PersonId As Integer, UniqueUserId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			Param(1) = New SqlParameter("@UniqueUserId", SqlDbType.NVarChar, 128)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = UniqueUserId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonnelByPersonIdAndId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonnelByPersonIdAndId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetIsVeederRootMacAddressAlredyUse(condition As String, CompanyId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@condition", SqlDbType.NVarChar)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = condition

			Param(1) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = CompanyId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetIsVeederRootMacAddressAlredyUse", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetIsVeederRootMacAddressAlredyUse Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetPersonDetailByUniqueUserId(UniqueUserId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@UniqueUserId", SqlDbType.NVarChar, 128)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = UniqueUserId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonDetailByUniqueUserId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonDetailByUniqueUserId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetPersonnelColumnNameForSearch(condition As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@condition", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = condition


			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetPersonnelColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetPersonVehicleMapping(PersonId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonVehicleMapping", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonVehicleMapping Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetPersonSiteMapping(PersonId As Integer, siteId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			Param(1) = New SqlParameter("@SiteID", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = siteId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonSiteMapping", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonSiteMapping Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function InsertPersonVehicleMapping(dtPersonAndVehicle As DataTable, PersonId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtPersonAndVehicle"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtPersonAndVehicle
			Param(0).TypeName = "dbo.PersonVehicleMapping"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@PersonId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Personnel_InsertPersonnelVehicleMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertPersonVehicleMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function InsertPersonSiteMapping(dtPersonSite As DataTable, PersonId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtPersonSite"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtPersonSite
			Param(0).TypeName = "dbo.PersonSiteMapping"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@PersonId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId


			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Personnel_InsertPersonnelSiteMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertPersonSiteMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetPersonIdByCondition(PersonId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, LoginPersonId As Integer, RoleId As String, CustomerId As Integer, IsHub As Boolean, Conditions As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = PersonId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			'Param(6) = New SqlParameter("@LoginPersonId", SqlDbType.Int)
			'Param(6).Direction = ParameterDirection.Input
			'Param(6).Value = LoginPersonId
			Param(6) = New SqlParameter("@IsHub", SqlDbType.Bit)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = IsHub

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonIdByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonIdByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetPersonnelByEmail(Email As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@Email", SqlDbType.NVarChar, 128)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Email

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPersonnelByEmail", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonnelByEmail Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function UpdateActiveFlag(ByVal PersonnelID As Integer, UniqueUserId As String, UserId As Integer, IsActive As Boolean) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(3) As SqlParameter
			Dim ParPersonnelID = New SqlParameter("@PersonnelID", SqlDbType.Int)
			ParPersonnelID.Direction = ParameterDirection.Input
			ParPersonnelID.Value = PersonnelID
			parcollection(0) = ParPersonnelID

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim ParUniqueUserId = New SqlParameter("@UniqueUserId", SqlDbType.NVarChar, 128)
			ParUniqueUserId.Direction = ParameterDirection.Input
			ParUniqueUserId.Value = UniqueUserId
			parcollection(2) = ParUniqueUserId

			Dim ParIsActive = New SqlParameter("@IsActive", SqlDbType.Bit)
			ParIsActive.Direction = ParameterDirection.Input
			ParIsActive.Value = IsActive
			parcollection(3) = ParIsActive

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Personnel_UpdateActiveFlag", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateActiveFlag Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetAndUpdateLastHubName() As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter() {}

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Personnel_GetAndUpdateLastHubName", Param)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in GetAndUpdateLastHubName Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetAndUpdateLastHubPersonNumberEntry() As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter() {}

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Personnel_GetAndUpdateLastHubPersonNumberEntry", Param)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in GetAndUpdateLastHubPersonNumberEntry Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function UpdateIMEI_UDIDFromHubName(ByVal HubName As String, IMEI_UDID As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParHubName = New SqlParameter("@HubName", SqlDbType.NVarChar, 2000)
			ParHubName.Direction = ParameterDirection.Input
			ParHubName.Value = HubName
			parcollection(0) = ParHubName

			Dim PaIMEI_UDID = New SqlParameter("@IMEI_UDID", SqlDbType.NVarChar, 2000)
			PaIMEI_UDID.Direction = ParameterDirection.Input
			PaIMEI_UDID.Value = IMEI_UDID
			parcollection(1) = PaIMEI_UDID


			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Personnel_UpdateIMEI_UDIDFromHubName", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateIMEI_UDIDFromHubName Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function AssignedFOBNumberToPerson(ByVal PersonPIN As String, FOBNumber As String, CustomerId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParPersonPIN = New SqlParameter("@PersonPIN", SqlDbType.NVarChar, 20)
			ParPersonPIN.Direction = ParameterDirection.Input
			ParPersonPIN.Value = PersonPIN
			parcollection(0) = ParPersonPIN

			Dim ParFOBNumber = New SqlParameter("@FOBNumber", SqlDbType.NVarChar, -1)
			ParFOBNumber.Direction = ParameterDirection.Input
			ParFOBNumber.Value = FOBNumber
			parcollection(1) = ParFOBNumber

			Dim ParCstomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCstomerId.Direction = ParameterDirection.Input
			ParCstomerId.Value = CustomerId
			parcollection(2) = ParCstomerId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Personnel_AssignedFOBNumberToPerson", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in AssignedFOBNumberToPerson Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function UpdateTermAndPolicysAcceptance(UniqueUserId As String, flag As Boolean) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter

			Dim ParUniqueUserId = New SqlParameter("@UniqueUserId", SqlDbType.NVarChar, 128)
			ParUniqueUserId.Direction = ParameterDirection.Input
			ParUniqueUserId.Value = UniqueUserId
			parcollection(0) = ParUniqueUserId

			Dim Parflag = New SqlParameter("@Flag", SqlDbType.Bit)
			Parflag.Direction = ParameterDirection.Input
			Parflag.Value = flag
			parcollection(1) = Parflag

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Personnel_UpdateTermAndPolicyAcceptance", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateTermAndPolicysAcceptance Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetIMEIPersonnelMappingByPersonId(PersonId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetIMEIPersonnelMappingByPersonId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetIMEIPersonnelMappingByPersonId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetIMEIPersonnelMappingByIMEIPersonMappingId(IMEIPersonMappingId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@IMEIPersonMappingId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IMEIPersonMappingId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetIMEIPersonnelMappingByIMEIPersonMappingId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetIMEIPersonnelMappingByIMEIPersonMappingId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function CheckDuplicateIMEI_UDIDPersonMapping(IMEI_UDID As String, PersonId As Integer, IMEIPersonMappingId As Integer) As Boolean
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(2) {}

			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@IMEI_UDID"
			Param(0).SqlDbType = SqlDbType.NVarChar
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IMEI_UDID

			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			ParPersonId.Direction = ParameterDirection.Input
			ParPersonId.Value = PersonId
			Param(1) = ParPersonId

			Dim ParIMEIPersonMappingId = New SqlParameter("@IMEIPersonMappingId", SqlDbType.Int)
			ParIMEIPersonMappingId.Direction = ParameterDirection.Input
			ParIMEIPersonMappingId.Value = IMEIPersonMappingId
			Param(2) = ParIMEIPersonMappingId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Personnel_CheckDuplicateIMEI_UDIDPersonMapping", Param)

		Catch ex As Exception
			log.Error("Error occurred in CheckDuplicateIMEI_UDIDPersonMapping Exception is :" + ex.Message)

			Return True
		Finally

		End Try
	End Function

	Public Function IMEI_UDIDPersonMappingInsertUpdate(IMEIPersonMappingId As Integer, PersonnelId As Integer, IMEI_UDID As String, IsActive As Boolean, UserId As String, Optional EntryFrom As String = "NonHub") As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(5) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@IMEIPersonMappingId"
			Param(0).SqlDbType = SqlDbType.Int
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IMEIPersonMappingId

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@PersonnelId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonnelId

			Param(2) = New SqlParameter()
			Param(2).ParameterName = "@IMEI_UDID"
			Param(2).SqlDbType = SqlDbType.NVarChar
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IMEI_UDID

			Param(3) = New SqlParameter()
			Param(3).ParameterName = "@IsActive"
			Param(3).SqlDbType = SqlDbType.Bit
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsActive

			Param(4) = New SqlParameter()
			Param(4).ParameterName = "@UserId"
			Param(4).SqlDbType = SqlDbType.Int
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = Convert.ToInt32(UserId)

			Param(5) = New SqlParameter()
			Param(5).ParameterName = "@EntryFrom"
			Param(5).SqlDbType = SqlDbType.NVarChar
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = EntryFrom


			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Personnel_IMEI_UDIDPersonMappingInsertUpdate", Param)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in IMEI_UDIDPersonMappingInsertUpdate Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function DeleteIMEIPersonMapping(ByVal IMEIPersonMappingId As Integer, ByVal UserId As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter

			Dim ParIMEIPersonMappingId = New SqlParameter("@IMEIPersonMappingId", SqlDbType.Int)
			ParIMEIPersonMappingId.Direction = ParameterDirection.Input
			ParIMEIPersonMappingId.Value = IMEIPersonMappingId
			parcollection(0) = ParIMEIPersonMappingId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = Convert.ToInt32(UserId)
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Personnel_DeleteIMEIPersonMapping", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteIMEIPersonMapping Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function UpdateIMEIActiveFlagByPersonId(ByVal PersonnelID As Integer, UserId As Integer, PersonIsActive As Boolean, Optional IMEIPersonMappingId As Integer = 0) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(3) As SqlParameter
			Dim ParPersonnelID = New SqlParameter("@PersonnelID", SqlDbType.Int)
			ParPersonnelID.Direction = ParameterDirection.Input
			ParPersonnelID.Value = PersonnelID
			parcollection(0) = ParPersonnelID

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim ParPersonIsActive = New SqlParameter("@PersonIsActive", SqlDbType.Bit)
			ParPersonIsActive.Direction = ParameterDirection.Input
			ParPersonIsActive.Value = PersonIsActive
			parcollection(2) = ParPersonIsActive

			Dim ParIMEIPersonMappingId = New SqlParameter("@IMEIPersonMappingId", SqlDbType.Int)
			ParIMEIPersonMappingId.Direction = ParameterDirection.Input
			ParIMEIPersonMappingId.Value = IMEIPersonMappingId
			parcollection(3) = ParIMEIPersonMappingId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Personnel_IMEI_UpdateActiveFlagByPersonId", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateIMEIActiveFlagByPersonId Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetPersonAccessLevels(RoleName As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(0) As SqlParameter
			Dim ParRoleName = New SqlParameter("@RoleName", SqlDbType.NVarChar, 20)
			ParRoleName.Direction = ParameterDirection.Input
			ParRoleName.Value = RoleName
			parcollection(0) = ParRoleName

			Dim ds As DataSet = New DataSet()

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Personnel_GetPerson_AccessLevels", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonAccessLevels Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

#End Region

#Region "Access Level"
	Public Function DeleteAccessLevel(ByVal RoleId As String, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParRoleId = New SqlParameter("@RoleId", SqlDbType.NVarChar, 128)
			ParRoleId.Direction = ParameterDirection.Input
			ParRoleId.Value = RoleId
			parcollection(0) = ParRoleId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_AccessLevles_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteAccessLevel Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function UpdateAccessLevel(ByVal RoleId As String, Name As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParRoleId = New SqlParameter("@RoleId", SqlDbType.NVarChar, 128)
			ParRoleId.Direction = ParameterDirection.Input
			ParRoleId.Value = RoleId
			parcollection(0) = ParRoleId

			Dim ParName = New SqlParameter("@Name", SqlDbType.NVarChar, 256)
			ParName.Direction = ParameterDirection.Input
			ParName.Value = Name
			parcollection(1) = ParName

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_AccessLevles_UpdateRole", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateAccessLevel Exception is :" + ex.Message)
			Return 0
		End Try
	End Function
#End Region

#Region "Vehicles"

	Public Function UpdateVehicleCurrentOdometer(VehicleId As Integer, OdoMeter As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(1) As SqlParameter
			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			Dim ParOdoMeter = New SqlParameter("@OdoMeter", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParOdoMeter.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			ParOdoMeter.Value = OdoMeter
			parcollection(0) = ParVehicleId
			parcollection(1) = ParOdoMeter

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vechicle_UpdateCurrentOdometer", parcollection)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in UpdateVehicleCurrentOdometer Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	'Public Function GetSiteByDeptId(Conditions As String) As DataTable
	'    Dim dal = New GeneralizedDAL()
	'    Try

	'        Dim ds As DataSet = New DataSet()

	'        Dim Param As SqlParameter() = New SqlParameter(0) {}

	'        Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
	'        Param(0).Direction = ParameterDirection.Input
	'        Param(0).Value = Conditions

	'        ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Vehicle_GetSiteByDeptID", Param)

	'        Return ds.Tables(0)

	'    Catch ex As Exception
	'        log.Error("Error occurred in GetSiteByDeptId Exception is :" + ex.Message)
	'        Return Nothing
	'    Finally

	'    End Try
	'End Function

	Public Function InsertVehicleSiteMapping(dtVehicleSite As DataTable, VehicleId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtVehicleSite"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtVehicleSite
			Param(0).TypeName = "dbo.VehicleSiteMapping"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@VehicleId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = VehicleId


			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Vehicle_InsertVehicleSiteMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertVehicleSiteMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetVehicleSiteMapping(VehicelId As Integer, SiteId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@VehicleId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = VehicelId

			Param(1) = New SqlParameter("@SiteId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = SiteId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Vehicle_GetVehicleSiteMapping", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetVehicleSiteMapping Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetVehiclebyId(VehicleId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@VehicleId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = VehicleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Vechicle_GetVehicleById", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetVehiclebyId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetVehicleIdByCondition(VehicleId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId As Integer, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@VehicleId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = VehicleId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vehicle_GetVehicleIdByCondition", Param)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in GetVehicleIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

    Public Function SaveUpdateVehicle(VehicleId As Integer, VehicleName As String, Make As String, Model As String, Year As Integer, LicensePlateNumber As String,
                                      VIN As String, Type As String, Extension As String, Comment As String, CurrentOdometer As Integer, DepartmentId As Integer,
                                      FuelLimitPerTxn As Integer, FuelLimitPerDay As Integer, RequireOdometerEntry As String, CheckOdometerReasonable As String, OdoLimit As Integer,
                                      VehicleNumber As String, Acc_Id As String, ExportCode As String,
                                      UserId As Integer, CustomerId As Integer, ExpectedMPGPerK As Decimal, Hours As Boolean, MileageOrKilometers As Integer,
                                      LicenseState As String, OdometerReasonabilityConditions As Integer, FOBNumber As String, Active As Boolean, FSTagMacAddress As String,
                                      CurrentHours As Integer, HoursLimit As Integer, CurrentFSVMFirmwareVersion As String) As Integer
        Try
            Dim result As Integer
            Dim dal = New GeneralizedDAL()
            Dim parcollection(32) As SqlParameter
            Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
            Dim ParVehicleName = New SqlParameter("@VehicleName", SqlDbType.NVarChar, 25)
            Dim ParMake = New SqlParameter("@Make", SqlDbType.NVarChar, 15)
            Dim ParModel = New SqlParameter("@Model", SqlDbType.NVarChar, 15)
            Dim ParYear = New SqlParameter("@Year", SqlDbType.Int)
            Dim ParLicensePlateNumber = New SqlParameter("@LicensePlateNumber", SqlDbType.NVarChar, 8)
            Dim ParVIN = New SqlParameter("@VIN", SqlDbType.NVarChar, 20)
            Dim ParType = New SqlParameter("@Type", SqlDbType.NVarChar, 20)
            Dim ParExtension = New SqlParameter("@Extension", SqlDbType.VarChar, 50)
            Dim ParComment = New SqlParameter("@Comment", SqlDbType.NVarChar, 2000)
            Dim ParCurrentOdometer = New SqlParameter("@CurrentOdometer", SqlDbType.Int)
            Dim ParDepartmentId = New SqlParameter("@DepartmentId", SqlDbType.Int)
            Dim ParFuelLimitPerTxn = New SqlParameter("@FuelLimitPerTxn", SqlDbType.Int)
            Dim ParFuelLimitPerDay = New SqlParameter("@FuelLimitPerDay", SqlDbType.Int)
            Dim ParRequireOdometerEntry = New SqlParameter("@RequireOdometerEntry", SqlDbType.VarChar, 1)
            Dim ParCheckOdometerReasonable = New SqlParameter("@CheckOdometerReasonable", SqlDbType.VarChar, 1)
            Dim ParOdoLimit = New SqlParameter("@OdoLimit", SqlDbType.Int)
            Dim ParVehicleNumber = New SqlParameter("@VehicleNumber", SqlDbType.VarChar, 20)
            Dim ParAcc_Id = New SqlParameter("@Acc_Id", SqlDbType.VarChar, 20)
            Dim ParExportCode = New SqlParameter("@ExportCode", SqlDbType.NVarChar, 50)
            Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
            Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
            Dim ParExpectedMPGPerK = New SqlParameter("@ExpectedMPGPerK", SqlDbType.Decimal)
            Dim ParHours = New SqlParameter("@Hours", SqlDbType.Bit)
            Dim ParMileageOrKilometers = New SqlParameter("@MileageOrKilometers", SqlDbType.Int)
            Dim ParLicenseState = New SqlParameter("@LicenseState", SqlDbType.NVarChar, 2)
            Dim ParOdometerReasonabilityConditions = New SqlParameter("@OdometerReasonabilityConditions", SqlDbType.Int)
            Dim ParFOBNumber = New SqlParameter("@FOBNumber", SqlDbType.NVarChar, -1)
            Dim ParIsActive = New SqlParameter("@IsActive", SqlDbType.Bit)
            Dim ParFSTagMacAddress = New SqlParameter("@FSTagMacAddress", SqlDbType.NVarChar, 50)
            Dim ParCurrentHours = New SqlParameter("@CurrentHours", SqlDbType.Int)
            Dim ParHoursLimit = New SqlParameter("@HoursLimit", SqlDbType.Int)
            Dim ParCurrentFSVMFirmwareVersion = New SqlParameter("@CurrentFSVMFirmwareVersion", SqlDbType.NVarChar, 50)

            ParVehicleId.Direction = ParameterDirection.Input
            ParVehicleName.Direction = ParameterDirection.Input
            ParMake.Direction = ParameterDirection.Input
            ParModel.Direction = ParameterDirection.Input
            ParYear.Direction = ParameterDirection.Input
            ParLicensePlateNumber.Direction = ParameterDirection.Input
            ParVIN.Direction = ParameterDirection.Input
            ParType.Direction = ParameterDirection.Input
            ParExtension.Direction = ParameterDirection.Input
            ParComment.Direction = ParameterDirection.Input
            ParCurrentOdometer.Direction = ParameterDirection.Input
            ParDepartmentId.Direction = ParameterDirection.Input
            ParFuelLimitPerTxn.Direction = ParameterDirection.Input
            ParFuelLimitPerDay.Direction = ParameterDirection.Input
            ParRequireOdometerEntry.Direction = ParameterDirection.Input
            ParCheckOdometerReasonable.Direction = ParameterDirection.Input
            ParOdoLimit.Direction = ParameterDirection.Input
            ParVehicleNumber.Direction = ParameterDirection.Input
            ParAcc_Id.Direction = ParameterDirection.Input
            ParExportCode.Direction = ParameterDirection.Input
            ParUserId.Direction = ParameterDirection.Input
            ParCustomerId.Direction = ParameterDirection.Input
            ParExpectedMPGPerK.Direction = ParameterDirection.Input
            ParHours.Direction = ParameterDirection.Input
            ParMileageOrKilometers.Direction = ParameterDirection.Input
            ParLicenseState.Direction = ParameterDirection.Input
            ParOdometerReasonabilityConditions.Direction = ParameterDirection.Input
            ParFOBNumber.Direction = ParameterDirection.Input
            ParIsActive.Direction = ParameterDirection.Input
            ParFSTagMacAddress.Direction = ParameterDirection.Input
            ParCurrentHours.Direction = ParameterDirection.Input
            ParHoursLimit.Direction = ParameterDirection.Input
            ParCurrentFSVMFirmwareVersion.Direction = ParameterDirection.Input

            ParVehicleId.Value = VehicleId
            ParVehicleName.Value = VehicleName
            ParMake.Value = Make
            ParModel.Value = Model
            ParYear.Value = IIf(Year = -1, Nothing, Year)
            ParLicensePlateNumber.Value = LicensePlateNumber
            ParVIN.Value = VIN
            ParType.Value = Type
            ParExtension.Value = Extension
            ParComment.Value = Comment
            ParCurrentOdometer.Value = IIf(CurrentOdometer = -1, Nothing, CurrentOdometer)
            ParDepartmentId.Value = DepartmentId
            ParFuelLimitPerTxn.Value = FuelLimitPerTxn
            ParFuelLimitPerDay.Value = FuelLimitPerDay
            ParRequireOdometerEntry.Value = RequireOdometerEntry
            ParCheckOdometerReasonable.Value = CheckOdometerReasonable
            ParOdoLimit.Value = IIf(OdoLimit = -1, Nothing, OdoLimit)
            ParVehicleNumber.Value = VehicleNumber
            ParAcc_Id.Value = Acc_Id
            ParExportCode.Value = ExportCode
            ParUserId.Value = UserId
            ParCustomerId.Value = CustomerId
            ParExpectedMPGPerK.Value = IIf(ExpectedMPGPerK = -1, Nothing, ExpectedMPGPerK)
            ParHours.Value = Hours
            ParMileageOrKilometers.Value = MileageOrKilometers
            ParLicenseState.Value = LicenseState
            ParOdometerReasonabilityConditions.Value = OdometerReasonabilityConditions
            ParFOBNumber.Value = FOBNumber
            ParIsActive.Value = Active
            ParFSTagMacAddress.Value = FSTagMacAddress
            ParCurrentHours.Value = IIf(CurrentHours = -1, Nothing, CurrentHours)
            ParHoursLimit.Value = IIf(HoursLimit = -1, Nothing, HoursLimit)
            ParCurrentFSVMFirmwareVersion.Value = CurrentFSVMFirmwareVersion

            parcollection(0) = ParVehicleId
            parcollection(1) = ParVehicleName
            parcollection(2) = ParMake
            parcollection(3) = ParModel
            parcollection(4) = ParYear
            parcollection(5) = ParLicensePlateNumber
            parcollection(6) = ParVIN
            parcollection(7) = ParExtension
            parcollection(8) = ParComment
            parcollection(9) = ParCurrentOdometer
            parcollection(10) = ParFuelLimitPerTxn
            parcollection(11) = ParRequireOdometerEntry
            parcollection(12) = ParCheckOdometerReasonable
            parcollection(13) = ParOdoLimit
            parcollection(14) = ParUserId
            parcollection(15) = ParVehicleNumber
            parcollection(16) = ParAcc_Id
            parcollection(17) = ParExportCode
            parcollection(18) = ParType
            parcollection(19) = ParDepartmentId
            parcollection(20) = ParFuelLimitPerDay
            parcollection(21) = ParCustomerId
            parcollection(22) = ParExpectedMPGPerK
            parcollection(23) = ParHours
            parcollection(24) = ParMileageOrKilometers
            parcollection(25) = ParLicenseState
            parcollection(26) = ParOdometerReasonabilityConditions
            parcollection(27) = ParFOBNumber
            parcollection(28) = ParIsActive
            parcollection(29) = ParFSTagMacAddress
            parcollection(30) = ParCurrentHours
            parcollection(31) = ParHoursLimit
            parcollection(32) = ParCurrentFSVMFirmwareVersion

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vechicle_InsertUpdate", parcollection)

            Return result
        Catch ex As Exception
            log.Error("Error occurred in SaveUpdateVehicle Exception is :" + ex.Message)
            Return 0

        Finally

        End Try

        Return 0

    End Function

    Public Function GetVehicleByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(2) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Vechicle_GetVehicleByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetVehicleByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	'Public Function GetVehicleByCondition(Conditions As String) As DataTable
	'    Dim dal = New GeneralizedDAL()
	'    Try

	'        Dim ds As DataSet = New DataSet()

	'        Dim Param As SqlParameter() = New SqlParameter(0) {}

	'        Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
	'        Param(0).Direction = ParameterDirection.Input
	'        Param(0).Value = Conditions

	'        ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Server_GetVehicleByCondition", Param)

	'        Return ds.Tables(0)

	'    Catch ex As Exception
	'        log.Error("Error occurred in GetVehicleByCondition Exception is :" + ex.Message)
	'        Return Nothing
	'    Finally

	'    End Try
	'End Function

	Public Function DeleteVehicle(ByVal VehicleId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(0) = ParVehicleId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vehicle_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteVehicle Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetSumOfFuelQuantity(PersonId As Integer, VehicleId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			Param(1) = New SqlParameter("@VehicleId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = VehicleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transactions_GetSumOfFuelQuantity", Param)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetSumOfFuelQuantity Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetFuelTypeVehicleMapping(VehicleId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@VehicleId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = VehicleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Vehicle_GetFuelTypeVehicleMapping", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFuelTypeVehicleMapping Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function InsertFuelTypeVehicleMapping(dtFuelTypeAndVehicle As DataTable, VehicleId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtFuelTypeAndVehicle"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtFuelTypeAndVehicle
			Param(0).TypeName = "dbo.FuelTypeAndVehicle"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@VehicleId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = VehicleId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Vehicle_InsertFuelTypeVehicleMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertFuelTypeVehicleMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function CheckVehicleNumberExist(ByVal VehicleNumber As String, VehicleId As Integer, CompanyId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter

			Dim ParVehicleNumber = New SqlParameter("@VehicleNumber", SqlDbType.NVarChar, 10)
			ParVehicleNumber.Direction = ParameterDirection.Input
			ParVehicleNumber.Value = VehicleNumber
			parcollection(0) = ParVehicleNumber

			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(1) = ParVehicleId

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId

			parcollection(2) = ParCompanyId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Vehicle_CheckVehicleNumberExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckVehicleNumberExist Exception is :" + ex.Message)
			Return True

		End Try
	End Function
	Public Function CheckVehicleVINExist(ByVal VIN As String, VehicleId As Integer, CompanyId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter

			Dim ParVehicleNumber = New SqlParameter("@VIN", SqlDbType.NVarChar, 20)
			ParVehicleNumber.Direction = ParameterDirection.Input
			ParVehicleNumber.Value = VIN
			parcollection(0) = ParVehicleNumber

			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(1) = ParVehicleId

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId

			parcollection(2) = ParCompanyId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Vehicle_CheckVehicleVINExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckVehicleVINExist Exception is :" + ex.Message)
			Return True

		End Try
	End Function
	Public Function CheckVehicleFSTagMacAddressExist(ByVal FSTagMacAddress As String, VehicleId As Integer, CompanyId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter

			Dim ParFSTagMacAddress = New SqlParameter("@FSTagMacAddress", SqlDbType.NVarChar, 50)
			ParFSTagMacAddress.Direction = ParameterDirection.Input
			ParFSTagMacAddress.Value = FSTagMacAddress
			parcollection(0) = ParFSTagMacAddress

			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(1) = ParVehicleId

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId

			parcollection(2) = ParCompanyId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_Vehicle_CheckVehicleFSTagMacAddressExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckVehicleFSTagMacAddressExist Exception is :" + ex.Message)
			Return True

		End Try
	End Function

	Public Function GetVehicleColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_vehicle_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetVehicleColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function MapPersonToNewGuestVehicle(UserId As Integer, VehicleId As Integer, CustomerId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(2) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@VehicleId"
			Param(0).SqlDbType = SqlDbType.Int
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = VehicleId

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@UserId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = UserId


			Param(2) = New SqlParameter()
			Param(2).ParameterName = "@CustomerId"
			Param(2).SqlDbType = SqlDbType.Int
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = CustomerId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Vehicle_MapPersonToNewGuestVehicle", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in MapPersonToNewGuestVehicle Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	'Public Function ImportVehicles(dtVehicles As DataTable, CustomerId As Integer, UserId As Integer) As Integer
	'    Dim dal = New GeneralizedDAL()
	'    Try
	'        Dim parcollection(2) As SqlParameter
	'        Dim ParVehicles = New SqlParameter()
	'        ParVehicles.ParameterName = "@dtTableTemp"
	'        ParVehicles.Value = dtVehicles
	'        ParVehicles.SqlDbType = System.Data.SqlDbType.Structured
	'        parcollection(0) = ParVehicles

	'        Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
	'        ParCustomerId.Direction = ParameterDirection.Input
	'        ParCustomerId.Value = CustomerId
	'        parcollection(1) = ParCustomerId

	'        Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
	'        ParUserId.Direction = ParameterDirection.Input
	'        ParUserId.Value = UserId
	'        parcollection(2) = ParUserId

	'        Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vehicle_importVehicles", parcollection)
	'        Return result
	'    Catch ex As Exception
	'        log.Error("Error occurred in ImportVehicles Exception is :" + ex.Message)
	'        Return 0
	'    End Try
	'End Function

	Public Function AssignedFOBNumberToVehicle(ByVal VehicleId As Integer, FOBNumber As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(0) = ParVehicleId

			Dim ParFOBNumber = New SqlParameter("@FOBNumber", SqlDbType.NVarChar, -1)
			ParFOBNumber.Direction = ParameterDirection.Input
			ParFOBNumber.Value = FOBNumber
			parcollection(1) = ParFOBNumber

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vehicle_AssignedFOBNumberToVehicle", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in AssignedFOBNumberToVehicle Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function UpdateVehicleInActiveFlag(ByVal VehicleId As Integer, UserId As Integer, IsActive As Boolean) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(0) = ParVehicleId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim ParIsActive = New SqlParameter("@IsActive", SqlDbType.Bit)
			ParIsActive.Direction = ParameterDirection.Input
			ParIsActive.Value = IsActive
			parcollection(2) = ParIsActive

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vehicle_UpdateInActiveFlag", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateVehicleInActiveFlag Exception is :" + ex.Message)
			Return 0
		End Try
	End Function
#End Region

#Region "Sites"

	Public Function CheckCurrentTimeInTimesTable(SiteId As Integer, PersonId As Integer) As DataSet
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@SiteID", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = SiteId

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Service_checkCurrentTimeInTimesTable", Param)

			Return dsResult

		Catch ex As Exception
			log.Error("Error occurred in CheckCurrentTimeInTimesTable Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetTiming() As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter() {}

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Times_GetTimesDetails", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTiming Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetSiteTimings(SiteID As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@SiteID", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = SiteID

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetSiteTimings", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteTimings Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function InsertSiteFuelTimings(dtTimings As DataTable, SiteId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtSiteFuelTimings"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtTimings
			Param(0).TypeName = "dbo.SiteTimings"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@SiteID"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = SiteId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Site_InsertSiteTimings", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertSiteFuelTimings Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetSiteDays(SiteID As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@SiteID", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = SiteID

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetSiteDays", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteDays Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function InsertSiteDays(dtDays As DataTable, SiteID As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtDays"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtDays
			Param(0).TypeName = "dbo.SiteDays"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@SiteID"
			Param(1).SqlDbType = SqlDbType.Structured
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = SiteID

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Site_InsertSiteDays", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertSiteDays Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function SiteIsExists(ByVal SiteID As Integer, SiteNUMBER As String, CustomerId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParSiteNo = New SqlParameter("@SiteId", SqlDbType.Int)
			ParSiteNo.Direction = ParameterDirection.Input
			ParSiteNo.Value = SiteID
			parcollection(0) = ParSiteNo

			Dim ParSiteNumber = New SqlParameter("@SiteNUMBER", SqlDbType.NVarChar, 10)
			ParSiteNumber.Direction = ParameterDirection.Input
			ParSiteNumber.Value = SiteNUMBER
			parcollection(1) = ParSiteNumber

			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(2) = ParCustomerId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_SiteCheckNumberExist", parcollection)
		Catch ex As Exception
			log.Error("Error occurred in SiteIsExists Exception is :" + ex.Message)
			Return True
		End Try
	End Function

	Public Function SaveUpdateSite(SiteID As Integer, SiteNUMBER As String, SiteName As String, SiteAddress As String, ExportCode As String, CustomerId As Integer, UserId As Integer, Latitude As String, Longitude As String,
								   HoseId As Integer, TankNumber As String, PulserRatio As Decimal, WifiSSID As String, FuelTypeid As Integer, HoseNumber As String, PumpOffTime As Integer,
								   PumpOnTime As Integer, TankMonitor As String, TankMonitorNumber As Integer?, ReplaceableHoseName As String, Unitsmeasured As Integer, Pulses As Integer, TimeZoneId As Integer,
								   OriginalNameOfFluidSecure As String, DisableGeoLocation As Boolean, IsBusy As Boolean, MacAddress As String, FirmwareFN As String, SwitchTimeB As String, TankId As Integer, DisplayOrder As Integer,
								   NumberOfZeroTransaction As Integer, Activate As Boolean, FSNPMacAddress As String, Optional ReconfigureLink As Boolean = True) As Integer ', IpAddress As String, UserName As String, Password As String, IsReplaced As Boolean
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(34) As SqlParameter
			Dim ParSiteID = New SqlParameter("@SiteID", SqlDbType.Int)
			Dim ParSiteNumber = New SqlParameter("@SiteNUMBER", SqlDbType.Int)
			Dim ParSiteName = New SqlParameter("@SiteName", SqlDbType.NVarChar, 20)
			Dim ParSiteAddress = New SqlParameter("@SiteAddress", SqlDbType.NVarChar)
			Dim ParExportCode = New SqlParameter("@ExportCode", SqlDbType.NVarChar, 50)
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParLat = New SqlParameter("@Latitude", SqlDbType.NVarChar)
			Dim ParLong = New SqlParameter("@Longitude", SqlDbType.NVarChar)
			Dim ParHoseID = New SqlParameter("@HoseId", SqlDbType.Int)
			Dim ParTankNumber = New SqlParameter("@TankNumber", SqlDbType.NVarChar, 10)
			Dim ParPulserRatio = New SqlParameter("@PulserRatio", SqlDbType.Decimal)
			Dim ParWifiSSId = New SqlParameter("@WifiSSId", SqlDbType.NVarChar, 32)

			Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
			Dim ParHoseNumber = New SqlParameter("@HoseNumber", SqlDbType.Int)
			Dim ParPumpOffTime = New SqlParameter("@PumpOffTime", SqlDbType.Int)
			Dim ParPumpOnTime = New SqlParameter("@PumpOnTime", SqlDbType.Int)


			Dim ParTankMonitor = New SqlParameter("@TankMonitor", SqlDbType.NVarChar, 1)
			Dim ParTankMonitorNumber = New SqlParameter("@TankMonitorNumber", SqlDbType.Int)

			Dim ParReplaceableHoseName = New SqlParameter("@ReplaceableHoseName", SqlDbType.NVarChar, 50)
			Dim ParUnitsmeasured = New SqlParameter("@Unitsmeasured", SqlDbType.Int)
			Dim ParPulses = New SqlParameter("@Pulses", SqlDbType.Int)
			Dim ParTimeZoneId = New SqlParameter("@TimeZoneId", SqlDbType.Int)
			Dim ParOriginalNameOfFluidSecure = New SqlParameter("@OriginalNameOfFluidSecure", SqlDbType.NVarChar, 50)
			Dim ParDisableGeoLocation = New SqlParameter("@DisableGeoLocation", SqlDbType.Bit)
			Dim ParIsBusy = New SqlParameter("@IsBusy", SqlDbType.Bit)
			Dim ParMacAddress = New SqlParameter("@MacAddress", SqlDbType.NVarChar, 100)
			Dim ParFirmwareFN = New SqlParameter("@FirmwareFN", SqlDbType.NVarChar, 30)
			Dim ParSwitchTimeB = New SqlParameter("@SwitchTimeB", SqlDbType.Int)
			Dim ParTankId = New SqlParameter("@TankId", SqlDbType.Int)
			Dim ParDisplayOrder = New SqlParameter("@DisplayOrder", SqlDbType.Int)
			Dim ParNumberOfZeroTransaction = New SqlParameter("@NumberOfZeroTransaction", SqlDbType.Int)
			Dim ParActivate = New SqlParameter("@Activated", SqlDbType.Bit)
			Dim ParFSNPMacAddress = New SqlParameter("@FSNPMacAddress", SqlDbType.NVarChar, 50)
			Dim ParReconfigureLink = New SqlParameter("@ReconfigureLink", SqlDbType.Bit)

			ParHoseID.Direction = ParameterDirection.Input
			ParTankNumber.Direction = ParameterDirection.Input
			ParPulserRatio.Direction = ParameterDirection.Input
			ParWifiSSId.Direction = ParameterDirection.Input

			ParFuelTypeId.Direction = ParameterDirection.Input
			ParHoseNumber.Direction = ParameterDirection.Input
			ParPumpOffTime.Direction = ParameterDirection.Input
			ParPumpOnTime.Direction = ParameterDirection.Input

			ParTankMonitor.Direction = ParameterDirection.Input
			ParTankMonitorNumber.Direction = ParameterDirection.Input

			ParReplaceableHoseName.Direction = ParameterDirection.Input

			ParSiteID.Direction = ParameterDirection.Input
			ParSiteNumber.Direction = ParameterDirection.Input
			ParSiteName.Direction = ParameterDirection.Input
			ParSiteAddress.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParExportCode.Direction = ParameterDirection.Input
			ParCustomerId.Direction = ParameterDirection.Input
			ParLat.Direction = ParameterDirection.Input
			ParLong.Direction = ParameterDirection.Input

			ParPulses.Direction = ParameterDirection.Input
			ParTimeZoneId.Direction = ParameterDirection.Input
			ParOriginalNameOfFluidSecure.Direction = ParameterDirection.Input
			ParDisableGeoLocation.Direction = ParameterDirection.Input
			ParIsBusy.Direction = ParameterDirection.Input
			ParMacAddress.Direction = ParameterDirection.Input

			ParFirmwareFN.Direction = ParameterDirection.Input
			ParSwitchTimeB.Direction = ParameterDirection.Input
			ParTankId.Direction = ParameterDirection.Input
			ParDisplayOrder.Direction = ParameterDirection.Input
			ParNumberOfZeroTransaction.Direction = ParameterDirection.Input
			ParActivate.Direction = ParameterDirection.Input
			ParFSNPMacAddress.Direction = ParameterDirection.Input
			ParReconfigureLink.Direction = ParameterDirection.Input

			ParSiteID.Value = SiteID
			ParSiteNumber.Value = SiteNUMBER
			ParSiteName.Value = SiteName
			ParSiteAddress.Value = SiteAddress
			ParUserId.Value = UserId
			ParExportCode.Value = ExportCode
			ParCustomerId.Value = CustomerId
			ParLat.Value = Latitude
			ParLong.Value = Longitude

			ParHoseID.Value = HoseId
			ParTankNumber.Value = TankNumber
			ParPulserRatio.Value = PulserRatio
			ParWifiSSId.Value = WifiSSID

			ParFuelTypeId.Value = FuelTypeid
			ParHoseNumber.Value = Nothing
			ParPumpOffTime.Value = PumpOffTime
			ParPumpOnTime.Value = PumpOnTime


			ParTankMonitor.Value = TankMonitor
			ParTankMonitorNumber.Value = TankMonitorNumber

			ParReplaceableHoseName.Value = ReplaceableHoseName
			ParUnitsmeasured.Value = Unitsmeasured
			ParPulses.Value = Pulses
			ParTimeZoneId.Value = TimeZoneId
			ParOriginalNameOfFluidSecure.Value = OriginalNameOfFluidSecure
			ParDisableGeoLocation.Value = DisableGeoLocation
			ParIsBusy.Value = IsBusy
			ParMacAddress.Value = MacAddress

			ParFirmwareFN.Value = FirmwareFN
			ParSwitchTimeB.Value = SwitchTimeB
			ParTankId.Value = TankId
			ParDisplayOrder.Value = DisplayOrder
			ParNumberOfZeroTransaction.Value = NumberOfZeroTransaction
			ParActivate.Value = Activate
			ParFSNPMacAddress.Value = FSNPMacAddress
			ParReconfigureLink.Value = ReconfigureLink

			parcollection(0) = ParSiteID
			parcollection(1) = ParSiteNumber
			parcollection(2) = ParSiteName
			parcollection(3) = ParSiteAddress
			parcollection(4) = ParUserId
			parcollection(5) = ParExportCode
			parcollection(6) = ParCustomerId
			parcollection(7) = ParLat
			parcollection(8) = ParLong

			parcollection(9) = ParHoseID
			parcollection(10) = ParTankNumber
			parcollection(11) = ParPulserRatio
			parcollection(12) = ParWifiSSId

			parcollection(13) = ParFuelTypeId
			parcollection(14) = ParHoseNumber
			parcollection(15) = ParPumpOffTime
			parcollection(16) = ParPumpOnTime


			parcollection(17) = ParTankMonitor
			parcollection(18) = ParTankMonitorNumber

			parcollection(19) = ParReplaceableHoseName
			parcollection(20) = ParUnitsmeasured
			parcollection(21) = ParPulses
			parcollection(22) = ParTimeZoneId
			parcollection(23) = ParOriginalNameOfFluidSecure
			parcollection(24) = ParDisableGeoLocation
			parcollection(25) = ParIsBusy
			parcollection(26) = ParMacAddress
			parcollection(27) = ParFirmwareFN
			parcollection(28) = ParSwitchTimeB
			parcollection(29) = ParTankId
			parcollection(30) = ParDisplayOrder
			parcollection(31) = ParNumberOfZeroTransaction
			parcollection(32) = ParActivate
			parcollection(33) = ParFSNPMacAddress
			parcollection(34) = ParReconfigureLink

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_SiteInsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateSite Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function


	Public Function GetSiteIdByCondition(SiteId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId As Integer, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@SiteId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = SiteId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, (2000))
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Site_GetSiteIdByCondition", Param)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in GetSiteIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function GetSiteId(SiteId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@SiteId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = SiteId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_SitebySiteId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetSiteDetails(Personid As Integer, RoleId As String) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter(1) {}


			parcollection(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Personid

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId
			'@PersonId int,
			'@RoleId nvarchar(256)
			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetSiteDetails", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteDetails Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetSiteColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetSiteByCondition(Conditions As String, PersonId As Integer, RoleId As String, IsDeletedLinkAllow As Boolean) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(3) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions


			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId


			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			Param(3) = New SqlParameter("@IsDeletedLinkAllow", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsDeletedLinkAllow

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetSiteByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function DeleteSite(ByVal SiteId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
			ParSiteId.Direction = ParameterDirection.Input
			ParSiteId.Value = SiteId
			parcollection(0) = ParSiteId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Site_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteSite Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetTimeZones() As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter() {}

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_site_GetTimeZones", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTimeZones Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function InsertSitePersonnelMapping(UserId As Integer, SiteID As Integer, CustomerId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(2) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@SiteID"
			Param(0).SqlDbType = SqlDbType.Int
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = SiteID

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@UserId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = UserId


			Param(2) = New SqlParameter()
			Param(2).ParameterName = "@CustomerId"
			Param(2).SqlDbType = SqlDbType.Int
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = CustomerId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Site_InsertSitePersonnelMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in MapPersonToNewGuestVehicle Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function InsertSiteVehicleMapping(FuelTypeId As Integer, SiteID As Integer, CustomerId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(2) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@SiteID"
			Param(0).SqlDbType = SqlDbType.Int
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = SiteID

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@FuelTypeId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = FuelTypeId


			Param(2) = New SqlParameter()
			Param(2).ParameterName = "@CustomerId"
			Param(2).SqlDbType = SqlDbType.Int
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = CustomerId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Site_InsertSiteVehicleMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertSiteVehicleMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function UpdateDayLightSaving(IsDayLightSaving As Boolean, UserId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@IsDayLightSaving"
			Param(0).SqlDbType = SqlDbType.Bit
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsDayLightSaving

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@UserId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = UserId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Site_UpdateDayLightSaving", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in UpdateDayLightSaving Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetDayLightSaving() As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter() {}

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetDayLightSaving", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetDayLightSaving Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetPersonSiteMappingBySiteId(siteId As Integer, CompanyId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@SiteId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = siteId

			Param(1) = New SqlParameter("@CompanyId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = CompanyId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetPersonSiteMappingBySiteId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonSiteMappingBySiteId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function InsertSinglePersonnelVehicleMapping(PersonId As Integer, VehicleId As Integer, UserId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(2) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@PersonId"
			Param(0).SqlDbType = SqlDbType.Int
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@VehicleId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = VehicleId

			Param(2) = New SqlParameter()
			Param(2).ParameterName = "@UserId"
			Param(2).SqlDbType = SqlDbType.Int
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = UserId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Personnel_InsertSinglePersonnelVehicleMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertSinglePersonnelVehicleMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	'Public Function UpdateAndGetPrinterNameAndBCardReader(ByVal SiteId As Integer, PrinterName As String, BluetoothCardReader As String, Flag As Integer) As DataTable
	'    Dim dal = New GeneralizedDAL()
	'    Try
	'        Dim parcollection(3) As SqlParameter
	'        Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
	'        ParSiteId.Direction = ParameterDirection.Input
	'        ParSiteId.Value = SiteId
	'        parcollection(0) = ParSiteId

	'        Dim ParPrinterName = New SqlParameter("@PrinterName", SqlDbType.NVarChar, 50)
	'        ParPrinterName.Direction = ParameterDirection.Input
	'        ParPrinterName.Value = PrinterName
	'        parcollection(1) = ParPrinterName

	'        Dim ParBluetoothCardReader = New SqlParameter("@BluetoothCardReader", SqlDbType.NVarChar, 50)
	'        ParBluetoothCardReader.Direction = ParameterDirection.Input
	'        ParBluetoothCardReader.Value = BluetoothCardReader
	'        parcollection(2) = ParBluetoothCardReader

	'        Dim ParFlag = New SqlParameter("@Flag", SqlDbType.Int)
	'        ParFlag.Direction = ParameterDirection.Input
	'        ParFlag.Value = Flag
	'        parcollection(3) = ParFlag

	'        Dim resultDS As DataSet = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Hose_PrinterNameBluetoothCardReaderInsertUpdate", parcollection)
	'        Dim result As DataTable = resultDS.Tables(0)
	'        Return result
	'    Catch ex As Exception
	'        log.Error("Error occurred in UpdateAndGetPrinterNameAndBCardReader Exception is :" + ex.Message)
	'        Return Nothing
	'    End Try
	'End Function

#End Region

#Region "Hose"

	Public Function HoseIsExists(ByVal SiteId As Integer, HoseNUMBER As String, WifiSSId As String, CompanyId As Integer, ReplaceableHoseName As String, FSNPMacAddress As String) As Integer ', HoseID As Integer,
		Dim dal = New GeneralizedDAL()
		Dim result As Integer = 0
		Try
			Dim parcollection(5) As SqlParameter
			Dim ParSiteNo = New SqlParameter("@SiteId", SqlDbType.Int)
			ParSiteNo.Direction = ParameterDirection.Input
			ParSiteNo.Value = SiteId
			parcollection(0) = ParSiteNo

			Dim ParSiteNumber = New SqlParameter("@HoseNUMBER", SqlDbType.NVarChar, 10)
			ParSiteNumber.Direction = ParameterDirection.Input
			ParSiteNumber.Value = HoseNUMBER
			parcollection(1) = ParSiteNumber

			'Dim ParSiteID = New SqlParameter("@SiteId", SqlDbType.Int)
			'ParSiteID.Direction = ParameterDirection.Input
			'ParSiteID.Value = SiteId
			'parcollection(2) = ParSiteID

			Dim ParWifiSSId = New SqlParameter("@WifiSSId", SqlDbType.NVarChar)
			ParWifiSSId.Direction = ParameterDirection.Input
			ParWifiSSId.Value = WifiSSId
			parcollection(2) = ParWifiSSId

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId
			parcollection(3) = ParCompanyId

			Dim ParReplaceableHoseName = New SqlParameter("@ReplaceableHoseName", SqlDbType.NVarChar, 50)
			ParReplaceableHoseName.Direction = ParameterDirection.Input
			ParReplaceableHoseName.Value = ReplaceableHoseName
			parcollection(4) = ParReplaceableHoseName

			Dim ParFSNPMacAddress = New SqlParameter("@FSNPMacAddress", SqlDbType.NVarChar, 50)
			ParFSNPMacAddress.Direction = ParameterDirection.Input
			ParFSNPMacAddress.Value = FSNPMacAddress
			parcollection(5) = ParFSNPMacAddress


			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_HoseCheckNumberExist", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in HoseIsExists Exception is :" + ex.Message)
			Return result
		End Try
	End Function

	Public Function SaveUpdateHose(HoseId As Integer, siteID As Integer, TankNumber As Integer, PulserRatio As Integer, WifiSSID As String, FuelTypeid As Integer, HoseNumber As String, PumpOffTime As Integer, PumpOnTime As Integer, TankMonitor As String, TankMonitorNumber As Integer, IpAddress As String, UserName As String, Password As String, UserId As Integer, CustomerId As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(15) As SqlParameter

			Dim ParHoseID = New SqlParameter("@HoseId", SqlDbType.Int)
			Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
			Dim ParTankNumber = New SqlParameter("@TankNumber", SqlDbType.Int)
			Dim ParPulserRatio = New SqlParameter("@PulserRatio", SqlDbType.Int)
			Dim ParWifiSSId = New SqlParameter("@WifiSSId", SqlDbType.NVarChar, 32)

			Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
			Dim ParHoseNumber = New SqlParameter("@HoseNumber", SqlDbType.Int)
			Dim ParPumpOffTime = New SqlParameter("@PumpOffTime", SqlDbType.Int)
			Dim ParPumpOnTime = New SqlParameter("@PumpOnTime", SqlDbType.Int)


			Dim ParTankMonitor = New SqlParameter("@TankMonitor", SqlDbType.NVarChar, 1)
			Dim ParTankMonitorNumber = New SqlParameter("@TankMonitorNumber", SqlDbType.Int)
			Dim ParIPAddress = New SqlParameter("@IPAddress", SqlDbType.NVarChar, 100)
			Dim ParUserName = New SqlParameter("@UserName", SqlDbType.NVarChar, 100)
			Dim ParPassword = New SqlParameter("@Password", SqlDbType.NVarChar, 100)
			Dim ParaUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParaCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)

			ParHoseID.Direction = ParameterDirection.Input
			ParSiteId.Direction = ParameterDirection.Input
			ParTankNumber.Direction = ParameterDirection.Input
			ParPulserRatio.Direction = ParameterDirection.Input
			ParWifiSSId.Direction = ParameterDirection.Input

			ParFuelTypeId.Direction = ParameterDirection.Input
			ParHoseNumber.Direction = ParameterDirection.Input
			ParPumpOffTime.Direction = ParameterDirection.Input
			ParPumpOnTime.Direction = ParameterDirection.Input

			ParTankMonitor.Direction = ParameterDirection.Input
			ParTankMonitorNumber.Direction = ParameterDirection.Input
			ParIPAddress.Direction = ParameterDirection.Input
			ParUserName.Direction = ParameterDirection.Input
			ParPassword.Direction = ParameterDirection.Input
			ParaUserId.Direction = ParameterDirection.Input
			ParaCustomerId.Direction = ParameterDirection.Input

			ParHoseID.Value = HoseId
			ParSiteId.Value = siteID
			ParTankNumber.Value = TankNumber
			ParPulserRatio.Value = PulserRatio
			ParWifiSSId.Value = WifiSSID

			ParFuelTypeId.Value = FuelTypeid
			ParHoseNumber.Value = HoseNumber
			ParPumpOffTime.Value = PumpOffTime
			ParPumpOnTime.Value = PumpOnTime


			ParTankMonitor.Value = TankMonitor
			ParTankMonitorNumber.Value = TankMonitorNumber
			ParIPAddress.Value = IpAddress
			ParUserName.Value = UserName
			ParPassword.Value = Password
			ParaUserId.Value = UserId
			ParaCustomerId.Value = CustomerId

			parcollection(0) = ParHoseID
			parcollection(1) = ParSiteId
			parcollection(2) = ParTankNumber
			parcollection(3) = ParPulserRatio
			parcollection(4) = ParWifiSSId

			parcollection(5) = ParFuelTypeId
			parcollection(6) = ParHoseNumber
			parcollection(7) = ParPumpOffTime
			parcollection(8) = ParPumpOnTime


			parcollection(9) = ParTankMonitor
			parcollection(10) = ParTankMonitorNumber
			parcollection(11) = ParIPAddress
			parcollection(12) = ParUserName
			parcollection(13) = ParPassword

			parcollection(14) = ParaUserId
			parcollection(15) = ParaCustomerId

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Hose_HoseInsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateHose Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetHoseIdByCondition(HoseId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(7) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@HoseId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = HoseId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Hose_GetHoseIdByCondition", Param)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in GetHoseIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function GetHoseId(HoseId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@HoseId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = HoseId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Hose_HosebyHoseId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetHoseId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetHoseIdBySiteId(SiteId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@SiteId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = SiteId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Hose_HosebySiteId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetHoseIdBySiteId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetHoseDetails(PersonId As Integer, RoleId As String) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter(1) {}

			parcollection(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = PersonId

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId
			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Hose_GetHoseDetails", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetHoseDetails Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function
	Public Function GetSiteList(Personid As Integer, RoleId As String) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter(1) {}

			parcollection(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Personid

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetSiteDetails", parcollection)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteList Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetSiteListByCustomerID(CustomerID As Integer) As DataTable
		Try
			Dim dsResult As DataSet = New DataSet()
			Dim dal = New GeneralizedDAL()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@CustomerID", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = CustomerID

			dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Site_GetSiteDetailsbyCustomerID", Param)

			Return dsResult.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetSiteListByCustomerID Exception is :" + ex.Message)
		Finally

		End Try

		Return Nothing

	End Function

	'Public Function GetFuelTypeList() As DataTable
	'    Try
	'        Dim dsResult As DataSet = New DataSet()
	'        Dim dal = New GeneralizedDAL()
	'        Dim parcollection() As SqlParameter = New SqlParameter() {}

	'        dsResult = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Hose_GetFuelTypeDetails", parcollection)

	'        Return dsResult.Tables(0)

	'    Catch ex As Exception
	'        log.Error("Error occurred in GetFuelTypeList Exception is :" + ex.Message)
	'    Finally

	'    End Try

	'    Return Nothing

	'End Function

	Public Function GetHoseByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(2) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Hose_GetHoseByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetHoseByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function
	'Public Function GetHoseByCondition(Conditions As String) As DataTable
	'    Dim dal = New GeneralizedDAL()
	'    Try

	'        Dim ds As DataSet = New DataSet()

	'        Dim Param As SqlParameter() = New SqlParameter(0) {}

	'        Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
	'        Param(0).Direction = ParameterDirection.Input
	'        Param(0).Value = Conditions


	'        ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Server_GetHoseByCondition", Param)

	'        Return ds.Tables(0)

	'    Catch ex As Exception
	'        log.Error("Error occurred in GetHoseByCondition Exception is :" + ex.Message)
	'        Return Nothing
	'    Finally

	'    End Try
	'End Function

	Public Function DeleteHose(ByVal HoseId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParHoseId = New SqlParameter("@HoseId", SqlDbType.Int)
			ParHoseId.Direction = ParameterDirection.Input
			ParHoseId.Value = HoseId
			parcollection(0) = ParHoseId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Hose_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteHose Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetHoseColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Hose_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetHoseColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function UpdateIsDefectiveEmailSent(ByVal SiteId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(0) As SqlParameter
			Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
			ParSiteId.Direction = ParameterDirection.Input
			ParSiteId.Value = SiteId
			parcollection(0) = ParSiteId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Hose_UpdateIsDefectiveEmailSent", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateIsDefectiveEmailSent	 Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

#End Region

#Region "Report"

	Public Function GetTransactionRptDetails(startDate As String, endDate As String, Condition As String, ForWhich As String, Optional ExtraCondition As String = "") As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(4) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = startDate

			parcollection(1) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = endDate

			parcollection(2) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = Condition

			parcollection(3) = New SqlParameter("@ForWhich", SqlDbType.NVarChar, 20)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = ForWhich

			parcollection(4) = New SqlParameter("@ExtraCondition", SqlDbType.NVarChar, 1000)
			parcollection(4).Direction = ParameterDirection.Input
			parcollection(4).Value = ExtraCondition

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetTransactionRptDetails", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetTransactionRptDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function ExportTransactions(startDate As String, endDate As String, Condition As String, flag As Integer, Optional DecimalPlace As Integer = 1, Optional DecimalType As Integer = 1) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(5) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = startDate

			parcollection(1) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = endDate

			parcollection(2) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = Condition

			parcollection(3) = New SqlParameter("@Flag", SqlDbType.Int)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = flag

			parcollection(4) = New SqlParameter("@DecimalPlace", SqlDbType.Int)
			parcollection(4).Direction = ParameterDirection.Input
			parcollection(4).Value = DecimalPlace

			parcollection(5) = New SqlParameter("@DecimalType", SqlDbType.Int)
			parcollection(5).Direction = ParameterDirection.Input
			parcollection(5).Value = DecimalType

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_ExportTransactions", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in ExportTransactions Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetVehicleDetails(Condition As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()

			'parcollection(0) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			'parcollection(0).Direction = ParameterDirection.Input
			'parcollection(0).Value = startDate

			'parcollection(1) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			'parcollection(1).Direction = ParameterDirection.Input
			'parcollection(1).Value = endDate

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetVehicleDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetVehicleDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function


	Public Function GetPersonalDetails(Condition As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetPersonalDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetPersonalDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetFluidSecureUnitDetails(Condition As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetFluidSecureUnitDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFluidSecureUnitDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetDeptDetails(Condition As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetDeptDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetDeptDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetPriceCostHistory(startDate As String, endDate As String, Condition As String) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = startDate

			parcollection(1) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = endDate

			parcollection(2) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = Condition

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report__GetPriceCostHistory", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetPriceCostHistory Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function


	Public Function GetBillingRptDetails(startDate As String, endDate As String, Condition As String, ForWhich As String) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(3) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = startDate

			parcollection(1) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = endDate

			parcollection(2) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = Condition

			parcollection(3) = New SqlParameter("@ForWhich", SqlDbType.NVarChar, 20)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = ForWhich

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_BillingRptDetails", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetBillingRptDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function


	Public Function GetTankReconciliationDetails(Condition As String, StartDateTime As String, EndDateTime As String, CustomerId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(3) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			parcollection(1) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = StartDateTime

			parcollection(2) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = EndDateTime

			parcollection(3) = New SqlParameter("@CustomerId", SqlDbType.NVarChar, 10)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetTankReconciliationDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankReconciliationDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetTotalizerMeterReconciliationDetails(Condition As String, StartDateTime As String, EndDateTime As String, CustomerId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(3) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			parcollection(1) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = StartDateTime

			parcollection(2) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = EndDateTime

			parcollection(3) = New SqlParameter("@CustomerId", SqlDbType.NVarChar, 10)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetTotalizerMeterReconciliationDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTotalizerMeterReconciliationDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetVehiclePerformanceRptDetails(startDate As String, endDate As String, Condition As String) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = startDate

			parcollection(1) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = endDate

			parcollection(2) = New SqlParameter("@Condition", SqlDbType.NVarChar, 20000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = Condition

			'parcollection(2) = New SqlParameter("@SubQueryVehicle", SqlDbType.NVarChar, 5000)
			'parcollection(2).Direction = ParameterDirection.Input
			'parcollection(2).Value = SubQueryVehicle

			'parcollection(3) = New SqlParameter("@SubQueryTransaction", SqlDbType.NVarChar, 20)
			'parcollection(3).Direction = ParameterDirection.Input
			'parcollection(3).Value = SubQueryTransaction

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetVehiclePerformanceRptDetails", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetVehiclePerformanceRptDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetTankTotDataEnteredDetails(Condition As String, StartDateTime As String, EndDateTime As String, CustomerId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(3) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			parcollection(1) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = StartDateTime

			parcollection(2) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = EndDateTime

			parcollection(3) = New SqlParameter("@CustomerId", SqlDbType.NVarChar, 10)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetInventoryDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankTotDataEnteredDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetTankBalanceDetails(Condition As String, CustomerId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(1) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			parcollection(1) = New SqlParameter("@CustomerId", SqlDbType.NVarChar, 10)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetTankBalanceDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankBalanceDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetCustomerWiseTransactionDetails(Condition As String) As DataSet 'startDate As String, endDate As String, 
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()

			'parcollection(0) = New SqlParameter("@StartDateTime", SqlDbType.NVarChar, 500)
			'parcollection(0).Direction = ParameterDirection.Input
			'parcollection(0).Value = startDate

			'parcollection(1) = New SqlParameter("@EndDateTime", SqlDbType.NVarChar, 500)
			'parcollection(1).Direction = ParameterDirection.Input
			'parcollection(1).Value = endDate

			parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Condition

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetCustomerWiseTransactionDetails", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetCustomerWiseTransactionDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

    Public Function GetShipmentDetails(Condition As String) As DataTable
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection() As SqlParameter = New SqlParameter(0) {}
            Dim ds = New DataSet()

            parcollection(0) = New SqlParameter("@Condition", SqlDbType.NVarChar, 2000)
            parcollection(0).Direction = ParameterDirection.Input
            parcollection(0).Value = Condition

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Report_GetShipmentDetails", parcollection)

            Return ds.Tables(0)

        Catch ex As Exception
            log.Error("Error occurred in GetVehicleDetails Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function
#End Region

#Region "Dashboard"

    Public Function GetDetails(CustomerId As Integer, CurrentDate As DateTime) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(1) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@CustomerId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = CustomerId

			parcollection(1) = New SqlParameter("@CurrentDate", SqlDbType.DateTime)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = CurrentDate


			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_dashboard_GetDetails", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

#End Region

#Region "Shipment"
	Public Function CheckValidShipment(ByVal ShipmentFluidSecureUnitName As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(0) As SqlParameter
			Dim ParShipmentFluidSecureUnitName = New SqlParameter("@ShipmentFluidSecureUnitName", SqlDbType.NVarChar, 32)
			ParShipmentFluidSecureUnitName.Direction = ParameterDirection.Input
			ParShipmentFluidSecureUnitName.Value = ShipmentFluidSecureUnitName
			parcollection(0) = ParShipmentFluidSecureUnitName

			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_Shipment_CheckValidShipment", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckValidShipment Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function GetShipmentsByCondition(Conditions As String, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(1) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Shipment_GetShipmentsByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetShipmentsByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function


	Public Function GetShipmentColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Shipment_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetShipmentColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function DeleteShipment(ByVal ShipmentId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParShipmentId = New SqlParameter("@ShipmentId", SqlDbType.Int)
			ParShipmentId.Direction = ParameterDirection.Input
			ParShipmentId.Value = ShipmentId
			parcollection(0) = ParShipmentId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Shipment_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteShipment Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetShipmentByShipmentID(ShipmentId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@ShipmentId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = ShipmentId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Shipment_GetShipmentByShipmentID", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetShipmentByShipmentID Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetShipmentIdByCondition(ShipmentId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, RoleId As String, LoginPersonId As Integer, CompanyId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(8) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@ShipmentId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = ShipmentId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@RoleId", SqlDbType.NVarChar)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = RoleId

			Param(7) = New SqlParameter("@LoginPersonId", SqlDbType.Int)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = LoginPersonId

			Param(8) = New SqlParameter("@CompanyId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CompanyId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Shipment_GetShipmentIdByCondition", Param)

			Return result

		Catch ex As Exception

			log.Error("Error occurred in GetShipmentIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function CheckFluidSecureUnitExist(ByVal FluidSecureUnitName As String, ShipmentId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParFluidSecureUnitName = New SqlParameter("@FluidSecureUnitName", SqlDbType.NVarChar, 32)
			ParFluidSecureUnitName.Direction = ParameterDirection.Input
			ParFluidSecureUnitName.Value = FluidSecureUnitName
			parcollection(0) = ParFluidSecureUnitName

			Dim ParShipmentId = New SqlParameter("@ShipmentId", SqlDbType.Int)
			ParShipmentId.Direction = ParameterDirection.Input
			ParShipmentId.Value = ShipmentId
			parcollection(1) = ParShipmentId

			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_Shipment_CheckFluidSecureUnitExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckFluidSecureUnitExist Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function SaveUpdateShipment(ShipmentId As Integer, FluidSecureUnitName As String, Company As String, Address As String,
									   UserId As Integer, ShipmentDate As DateTime, CompanyId As Integer, HubName As String, IsReplacement As Boolean,
									   ReplacementForSiteId As Integer, ReplacementForHubId As Integer, IsReturned As Boolean, ReturnedOn As DateTime,
									   ShipmentForLinkOrHub As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(13) As SqlParameter
			Dim ParShipmentId = New SqlParameter("@ShipmentId", SqlDbType.Int)
			Dim ParFluidSecureUnitName = New SqlParameter("@FluidSecureUnitName", SqlDbType.NVarChar, 32)
			Dim ParCompany = New SqlParameter("@Company", SqlDbType.NVarChar, 30)
			Dim ParAddress = New SqlParameter("@Address", SqlDbType.NVarChar, 50)
			Dim ParShipmentDate = New SqlParameter("@ShipmentDate", SqlDbType.DateTime)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			Dim ParHubName = New SqlParameter("@HubName", SqlDbType.NVarChar, 32)
			Dim ParIsReplacement = New SqlParameter("@IsReplacement", SqlDbType.NVarChar, 32)
			Dim ParReplacementForSiteId = New SqlParameter("@ReplacementForSiteId", SqlDbType.Int)
			Dim ParReplacementForHubId = New SqlParameter("@ReplacementForHubId", SqlDbType.Int)
			Dim ParIsReturned = New SqlParameter("@IsReturned", SqlDbType.Bit)
			Dim ParReturnedOn = New SqlParameter("@ReturnedOn", SqlDbType.DateTime)
			Dim ParShipmentForLinkOrHub = New SqlParameter("@ShipmentForLinkOrHub", SqlDbType.Int)

			ParShipmentId.Direction = ParameterDirection.Input
			ParFluidSecureUnitName.Direction = ParameterDirection.Input
			ParCompany.Direction = ParameterDirection.Input
			ParAddress.Direction = ParameterDirection.Input
			ParShipmentDate.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParCompanyId.Direction = ParameterDirection.Input
			ParHubName.Direction = ParameterDirection.Input
			ParIsReplacement.Direction = ParameterDirection.Input
			ParReplacementForSiteId.Direction = ParameterDirection.Input
			ParReplacementForHubId.Direction = ParameterDirection.Input
			ParIsReturned.Direction = ParameterDirection.Input
			ParReturnedOn.Direction = ParameterDirection.Input
			ParShipmentForLinkOrHub.Direction = ParameterDirection.Input

			ParShipmentId.Value = ShipmentId
			ParFluidSecureUnitName.Value = FluidSecureUnitName
			ParCompany.Value = Company
			ParAddress.Value = Address
			ParUserId.Value = UserId
			ParShipmentDate.Value = ShipmentDate
			ParCompanyId.Value = CompanyId
			ParHubName.Value = HubName
			ParIsReplacement.Value = IsReplacement
			ParReplacementForSiteId.Value = ReplacementForSiteId
			ParReplacementForHubId.Value = ReplacementForHubId
			ParIsReturned.Value = IsReturned
			ParReturnedOn.Value = IIf(IsReturned = True, ReturnedOn, Nothing)
			ParShipmentForLinkOrHub.Value = ShipmentForLinkOrHub

			parcollection(0) = ParShipmentId
			parcollection(1) = ParFluidSecureUnitName
			parcollection(2) = ParCompany
			parcollection(3) = ParAddress
			parcollection(4) = ParUserId
			parcollection(5) = ParShipmentDate
			parcollection(6) = ParCompanyId
			parcollection(7) = ParHubName
			parcollection(8) = ParIsReplacement
			parcollection(9) = ParReplacementForSiteId
			parcollection(10) = ParReplacementForHubId
			parcollection(11) = ParIsReturned
			parcollection(12) = ParReturnedOn
			parcollection(13) = ParShipmentForLinkOrHub

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Shipment_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateShipment Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function
#End Region

#Region "Common"
	Public Function UpdateFieldsAfterAddingUserFromRegisterCompany(ByVal CompanyId As Integer, Email As String, ShipmentFluidSecureUnitName As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId
			parcollection(0) = ParCompanyId

			Dim ParEmail = New SqlParameter("@Email", SqlDbType.NVarChar, 256)
			ParEmail.Direction = ParameterDirection.Input
			ParEmail.Value = Email
			parcollection(1) = ParEmail

			Dim ParShipmentFluidSecureUnitName = New SqlParameter("@ShipmentFluidSecureUnitName", SqlDbType.NVarChar, 32)
			ParShipmentFluidSecureUnitName.Direction = ParameterDirection.Input
			ParShipmentFluidSecureUnitName.Value = ShipmentFluidSecureUnitName
			parcollection(2) = ParShipmentFluidSecureUnitName

			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_Common_UpdateFieldsAfterAddingUserFromRegisterCompany", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in UpdateFieldsAfterAddingUserFromRegisterCompany Exception is :" + ex.Message)
			Return 0

		End Try
	End Function
#End Region

#Region "Upload Firmware"
	Public Function SaveUpdateFirmware(FirmwareId As Integer, FirmwareFileName As String, FirmwareFilePath As String, Version As String, UserId As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(4) As SqlParameter
			Dim ParFirmwareId = New SqlParameter("@FirmwareId", SqlDbType.Int)
			Dim ParFirmwareFileName = New SqlParameter("@FirmwareFileName", SqlDbType.NVarChar, 2000)
			Dim ParFirmwareFilePath = New SqlParameter("@FirmwareFilePath", SqlDbType.NVarChar, 2000)
			Dim ParVersion = New SqlParameter("@Version", SqlDbType.NVarChar, 2000)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)

			ParFirmwareId.Direction = ParameterDirection.Input
			ParFirmwareFileName.Direction = ParameterDirection.Input
			ParFirmwareFilePath.Direction = ParameterDirection.Input
			ParVersion.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input

			ParFirmwareId.Value = FirmwareId
			ParFirmwareFileName.Value = FirmwareFileName
			ParFirmwareFilePath.Value = FirmwareFilePath
			ParVersion.Value = Version
			ParUserId.Value = UserId

			parcollection(0) = ParFirmwareId
			parcollection(1) = ParFirmwareFileName
			parcollection(2) = ParFirmwareFilePath
			parcollection(3) = ParVersion
			parcollection(4) = ParUserId

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Firmware_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateFirmware Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetFirmwareColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Firmware_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetFirmwareColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetFirmwaresByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(1) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Firmware_GetFirmwaresByCondition", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFirmwaresByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function LaunchFirmware(ByVal FirmwareId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParFirmwareId = New SqlParameter("@FirmwareId", SqlDbType.Int)
			ParFirmwareId.Direction = ParameterDirection.Input
			ParFirmwareId.Value = FirmwareId
			parcollection(0) = ParFirmwareId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Firmware_LaunchFirmware", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in LaunchFirmware Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function CheckVersionIsExist(ByVal Version As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(0) As SqlParameter
			Dim ParVersion = New SqlParameter("@Version", SqlDbType.NVarChar, 2000)
			ParVersion.Direction = ParameterDirection.Input
			ParVersion.Value = Version
			parcollection(0) = ParVersion

			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_Firmware_CheckVersionIsExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckVersionIsExist Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function DeleteFirmware(ByVal FirmwareId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParFirmwareId = New SqlParameter("@FirmwareId", SqlDbType.Int)
			ParFirmwareId.Direction = ParameterDirection.Input
			ParFirmwareId.Value = FirmwareId
			parcollection(0) = ParFirmwareId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Firmware_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteFirmware Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetLaunchedFirmwareDetails() As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter() {}
			Dim ds = New DataSet()


			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Firmware_GetLaunchedFirmwareDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetLaunchedFirmwareDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function checkLaunchedAndExistedVersionAndUpdate(HoseId As String, version As String, personId As Integer, flag As Integer) As DataSet
		Try
			Dim result As DataSet
			Dim dal = New GeneralizedDAL()
			Dim parcollection(3) As SqlParameter

			Dim ParSSID = New SqlParameter("@HoseId", SqlDbType.NVarChar, 200)
			Dim Parversion = New SqlParameter("@version", SqlDbType.NVarChar, 200)
			Dim ParpersonId = New SqlParameter("@personId", SqlDbType.Int)
			Dim Parflag = New SqlParameter("@flag", SqlDbType.Int)

			ParSSID.Direction = ParameterDirection.Input
			ParSSID.Value = HoseId
			parcollection(0) = ParSSID


			Parversion.Direction = ParameterDirection.Input
			Parversion.Value = version
			parcollection(1) = Parversion

			ParpersonId.Direction = ParameterDirection.Input
			ParpersonId.Value = personId
			parcollection(2) = ParpersonId

			Parflag.Direction = ParameterDirection.Input
			Parflag.Value = flag
			parcollection(3) = Parflag

			result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_CheckLaunchedAndExistedVersionAndUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in checkLaunchedAndExistedVersionAndUpdate Exception is :" + ex.Message)
			Return Nothing

		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetFirmxareFluidLinkMappingByFirmwaredID(FirmwareId As Integer) As DataTable
		Dim result As DataSet
		Try

			Dim dal = New GeneralizedDAL()
			Dim parcollection(0) As SqlParameter
			Dim ParFirmwareId = New SqlParameter("@FirmwareId", SqlDbType.Int)


			ParFirmwareId.Direction = ParameterDirection.Input


			ParFirmwareId.Value = FirmwareId


			parcollection(0) = ParFirmwareId


			result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Firmware_GetCompanyNodes", parcollection)

			Return result.Tables(0)
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateFirmware Exception is :" + ex.Message)
			Return Nothing

		Finally

		End Try
	End Function

	Public Function InsertFirmwareFluidSecureLinksMapping(dtFirmwareFluidSecureLinks As DataTable, FirmwareId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtFirmwareFluidSecureLinks"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtFirmwareFluidSecureLinks
			Param(0).TypeName = "dbo.FirmwareFluidSecureLinksMapping"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@FirmwareId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = FirmwareId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Firmware_InsertFirmwareFluidSecureLinksMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertFirmwareFluidSecureLinksMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function DeleteFirmwareFluidSecureLinksMapping(CompanyId As Integer, FirmwareId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@CompanyId"
			Param(0).SqlDbType = SqlDbType.Int
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = CompanyId

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@FirmwareId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = FirmwareId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_Firmware_DeleteFirmwareFluidSecureLinksMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in DeleteFirmwareFluidSecureLinksMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetFirmwareById(ByVal FirmwareId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(0) As SqlParameter
			Dim ParFirmwareId = New SqlParameter("@FirmwareId", SqlDbType.Int)
			ParFirmwareId.Direction = ParameterDirection.Input
			ParFirmwareId.Value = FirmwareId
			parcollection(0) = ParFirmwareId

			Dim result As DataSet = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Firmware_GetFirmwareById", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in GetFirmwareById Exception is :" + ex.Message)
			Return Nothing
		End Try
	End Function

#End Region

#Region "Registration Email validation"

	Public Function checkCurrentEmailIdExistsOrNot(emailId As String) As DataSet
		Try
			Dim result As DataSet
			Dim dal = New GeneralizedDAL()
			Dim parcollection(1) As SqlParameter
			Dim ParemailId = New SqlParameter("@emailId", SqlDbType.NVarChar, 256)

			ParemailId.Direction = ParameterDirection.Input
			ParemailId.Value = emailId
			parcollection(0) = ParemailId

			result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_CheckEmailIdExistsOrNot", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in checkCurrentEmailIdExistsOrNot Exception is :" + ex.Message)
			Return Nothing

		Finally

		End Try

		Return Nothing

	End Function

#End Region

	Public Function SpUpgradeTransactionStatus(TransactionId As Integer, Status As Integer, flag As Integer) As DataSet
		Try
			Dim result As DataSet
			Dim dal = New GeneralizedDAL()
			Dim parcollection(2) As SqlParameter

			Dim ParSSID = New SqlParameter("@TransactionId", SqlDbType.NVarChar, 200)
			Dim Parversion = New SqlParameter("@Status", SqlDbType.NVarChar, 200)
			Dim Parflag = New SqlParameter("@flag", SqlDbType.Int)

			ParSSID.Direction = ParameterDirection.Input
			ParSSID.Value = TransactionId
			parcollection(0) = ParSSID


			Parversion.Direction = ParameterDirection.Input
			Parversion.Value = Status
			parcollection(1) = Parversion


			Parflag.Direction = ParameterDirection.Input
			Parflag.Value = flag
			parcollection(2) = Parflag

			result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Transaction_UpdateTransactionStatus", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SpUpgradeTransactionStatus Exception is :" + ex.Message)
			Return Nothing

		Finally

		End Try

		Return Nothing

	End Function

#Region "AutoTransactionExportSettings"
	Public Function CheckCompanyExist(ByVal CompanyId As Integer, AutoTransactionExportSettingId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId
			parcollection(0) = ParCompanyId

			Dim ParAutoTransactionExportSettingId = New SqlParameter("@AutoTransactionExportSettingId", SqlDbType.Int)
			ParAutoTransactionExportSettingId.Direction = ParameterDirection.Input
			ParAutoTransactionExportSettingId.Value = AutoTransactionExportSettingId
			parcollection(1) = ParAutoTransactionExportSettingId


			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_AutoTransactionExportSettings_CheckCompanyExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckCompanyExist Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function GetAutoTransactionExportSettingsByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(2) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_AutoTransactionExportSettings_GetAutoTransactionExportSettingsByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetAutoTransactionExportSettingsByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function


	Public Function GetAutoTransactionExportSettingColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_AutoTransactionExportSettings_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetAutoTransactionExportSettingColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function DeleteAutoTransactionExportSetting(ByVal AutoTransactionExportSettingId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParAutoTransactionExportSettingId = New SqlParameter("@AutoTransactionExportSettingId", SqlDbType.Int)
			ParAutoTransactionExportSettingId.Direction = ParameterDirection.Input
			ParAutoTransactionExportSettingId.Value = AutoTransactionExportSettingId
			parcollection(0) = ParAutoTransactionExportSettingId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_AutoTransactionExportSettings_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteAutoTransactionExportSetting Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetAutoTransactionExportSettingsByAutoTransactionExportSettingId(AutoTransactionExportSettingId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@AutoTransactionExportSettingId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = AutoTransactionExportSettingId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_AutoTransactionExportSettings_GetAutoTransactionExportSettingsByAutoTransactionExportSettingId", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetAutoTransactionExportSettingsByAutoTransactionExportSettingId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetAutoTransactionExportSettingIdByCondition(AutoTransactionExportSettingId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, RoleId As String, LoginPersonId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(7) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@AutoTransactionExportSettingId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = AutoTransactionExportSettingId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@RoleId", SqlDbType.NVarChar)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = RoleId

			Param(7) = New SqlParameter("@LoginPersonId", SqlDbType.Int)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = LoginPersonId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_AutoTransactionExportSettings_GetAutoTransactionExportSettingIdByCondition", Param)

			Return result

		Catch ex As Exception

			log.Error("Error occurred in GetAutoTransactionExportSettingIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function SaveUpdateAutoTransactionExportSetting(AutoTransactionExportSettingId As Integer, CompanyId As Integer, ExportOption As Integer,
														   Active As Boolean, FtpServerPath As String, FtpUsername As String, FtpPassword As String,
														   EmailId As String, UserId As Integer, TimeZoneId As Integer, ExecutionTime As DateTime, Separator As String,
														   CustomizedExportTemplateId As Integer, IncludePreviouslyExportTransactions As Boolean, ExportOnlyNewTransactions As Boolean,
														   Optional ExportZeroQtyTransactions As Boolean = False, Optional DecimalPlace As Integer = 1, Optional DecimalType As Integer = 0,
														   Optional DateType As String = "MMddyyyy") As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(18) As SqlParameter
			Dim ParAutoTransactionExportSettingId = New SqlParameter("@AutoTransactionExportSettingId", SqlDbType.Int)
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			Dim ParExportOption = New SqlParameter("@ExportOption", SqlDbType.Int)
			Dim ParActive = New SqlParameter("@Active", SqlDbType.Bit)
			Dim ParFtpServerPath = New SqlParameter("@FtpServerPath", SqlDbType.NVarChar, 2000)
			Dim ParFtpUsername = New SqlParameter("@FtpUsername", SqlDbType.NVarChar, 2000)
			Dim ParFtpPassword = New SqlParameter("@FtpPassword", SqlDbType.NVarChar, 2000)
			Dim ParEmailId = New SqlParameter("@EmailId", SqlDbType.NVarChar, 2000)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParTimeZoneId = New SqlParameter("@TimeZoneId", SqlDbType.Int)
			Dim ParExecutionTime = New SqlParameter("@ExecutionTime", SqlDbType.Time)
			Dim ParSeparator = New SqlParameter("@Separator", SqlDbType.NVarChar, 20)
			Dim ParCustomizedExportTemplateId = New SqlParameter("@CustomizedExportTemplateId", SqlDbType.Int)
			Dim ParIncludePreviouslyExportTransactions = New SqlParameter("@IncludePreviouslyExportTransactions", SqlDbType.Bit)
			Dim ParExportOnlyNewTransactions = New SqlParameter("@ExportOnlyNewTransactions", SqlDbType.Bit)
			Dim ParExportZeroQtyTransactions = New SqlParameter("@ExportZeroQtyTransactions", SqlDbType.Bit)
			Dim ParDecimalPlace = New SqlParameter("@DecimalPlace", SqlDbType.Int)
			Dim ParDecimalType = New SqlParameter("@DecimalType", SqlDbType.Int)
			Dim ParDateType = New SqlParameter("@DateType", SqlDbType.NVarChar, 10)

			ParAutoTransactionExportSettingId.Direction = ParameterDirection.Input
			ParCompanyId.Direction = ParameterDirection.Input
			ParExportOption.Direction = ParameterDirection.Input
			ParActive.Direction = ParameterDirection.Input
			ParFtpServerPath.Direction = ParameterDirection.Input
			ParFtpUsername.Direction = ParameterDirection.Input
			ParFtpPassword.Direction = ParameterDirection.Input
			ParEmailId.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParTimeZoneId.Direction = ParameterDirection.Input
			ParExecutionTime.Direction = ParameterDirection.Input
			ParSeparator.Direction = ParameterDirection.Input
			ParCustomizedExportTemplateId.Direction = ParameterDirection.Input
			ParIncludePreviouslyExportTransactions.Direction = ParameterDirection.Input
			ParExportZeroQtyTransactions.Direction = ParameterDirection.Input
			ParDecimalPlace.Direction = ParameterDirection.Input
			ParDecimalType.Direction = ParameterDirection.Input
			ParDateType.Direction = ParameterDirection.Input

			ParAutoTransactionExportSettingId.Value = AutoTransactionExportSettingId
			ParCompanyId.Value = CompanyId
			ParExportOption.Value = ExportOption
			ParActive.Value = Active
			ParFtpServerPath.Value = FtpServerPath
			ParFtpUsername.Value = FtpUsername
			ParFtpPassword.Value = FtpPassword
			ParEmailId.Value = EmailId
			ParUserId.Value = UserId
			ParTimeZoneId.Value = TimeZoneId
			ParExecutionTime.Value = ExecutionTime
			ParSeparator.Value = Separator
			ParCustomizedExportTemplateId.Value = CustomizedExportTemplateId
			ParIncludePreviouslyExportTransactions.Value = IncludePreviouslyExportTransactions
			ParExportOnlyNewTransactions.Value = ExportOnlyNewTransactions
			ParExportZeroQtyTransactions.Value = ExportZeroQtyTransactions
			ParDecimalPlace.Value = DecimalPlace
			ParDecimalType.Value = DecimalType
			ParDateType.Value = DateType

			parcollection(0) = ParAutoTransactionExportSettingId
			parcollection(1) = ParCompanyId
			parcollection(2) = ParExportOption
			parcollection(3) = ParActive
			parcollection(4) = ParFtpServerPath
			parcollection(5) = ParFtpUsername
			parcollection(6) = ParFtpPassword
			parcollection(7) = ParEmailId
			parcollection(8) = ParUserId
			parcollection(9) = ParTimeZoneId
			parcollection(10) = ParExecutionTime
			parcollection(11) = ParSeparator
			parcollection(12) = ParCustomizedExportTemplateId
			parcollection(13) = ParIncludePreviouslyExportTransactions
			parcollection(14) = ParExportOnlyNewTransactions
			parcollection(15) = ParExportZeroQtyTransactions
			parcollection(16) = ParDecimalPlace
			parcollection(17) = ParDecimalType
			parcollection(18) = ParDateType

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_AutoTransactionExportSettings_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateAutoTransactionExportSetting Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetALLPersonWiseExportTransactionFields(PersonId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = PersonId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_AutoTransactionExportSettings_GetALLPersonWiseExportTransactionFields", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetALLPersonWiseExportTransactionFields Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function SavePersonWiseExportTransactionFields(PersonId As Integer, DateFormat As String, TransactionStatus As String,
														   FileType As String, CustomizedExportTemplate As String, AddDecimal As String, CustomerId As String,
														   Separator As String, Nametothefile As String, DecimalType As Integer) As String
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(9) As SqlParameter
			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			Dim ParDateFormat = New SqlParameter("@DateFormat", SqlDbType.NVarChar, 50)
			Dim ParTransactionStatus = New SqlParameter("@TransactionStatus", SqlDbType.NVarChar, 50)
			Dim ParFileType = New SqlParameter("@FileType", SqlDbType.NVarChar, 50)
			Dim ParCustomizedExportTemplate = New SqlParameter("@CustomizedExportTemplate", SqlDbType.NVarChar, 50)
			Dim ParAddDecimal = New SqlParameter("@AddDecimal", SqlDbType.NVarChar, 50)
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.NVarChar, 50)
			Dim ParSeparator = New SqlParameter("@Separator", SqlDbType.NVarChar, 50)
			Dim ParNametothefile = New SqlParameter("@Nametothefile", SqlDbType.NVarChar, 50)
			Dim ParDecimalType = New SqlParameter("@DecimalType", SqlDbType.NVarChar, 50)


			ParPersonId.Direction = ParameterDirection.Input
			ParDateFormat.Direction = ParameterDirection.Input
			ParTransactionStatus.Direction = ParameterDirection.Input
			ParFileType.Direction = ParameterDirection.Input
			ParCustomizedExportTemplate.Direction = ParameterDirection.Input
			ParAddDecimal.Direction = ParameterDirection.Input
			ParCustomerId.Direction = ParameterDirection.Input
			ParSeparator.Direction = ParameterDirection.Input
			ParNametothefile.Direction = ParameterDirection.Input
			ParDecimalType.Direction = ParameterDirection.Input

			ParPersonId.Value = PersonId
			ParDateFormat.Value = DateFormat
			ParTransactionStatus.Value = TransactionStatus
			ParFileType.Value = FileType
			ParCustomizedExportTemplate.Value = CustomizedExportTemplate
			ParAddDecimal.Value = AddDecimal
			ParCustomerId.Value = CustomerId
			ParSeparator.Value = Separator
			ParNametothefile.Value = Nametothefile
			ParDecimalType.Value = DecimalType

			parcollection(0) = ParPersonId
			parcollection(1) = ParDateFormat
			parcollection(2) = ParTransactionStatus
			parcollection(3) = ParFileType
			parcollection(4) = ParCustomizedExportTemplate
			parcollection(5) = ParAddDecimal
			parcollection(6) = ParCustomerId
			parcollection(7) = ParSeparator
			parcollection(8) = ParNametothefile
			parcollection(9) = ParDecimalType

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_AutoTransactionExportSettings_SavePersonWiseExportTransactionFields", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SavePersonWiseExportTransactionFields Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function


#End Region

#Region "Reconciliation - TankInventory"

	Public Function SaveUpdateTankInventory(TankInventoryId As Integer, TankNumber As String, ENTRY_TYPE As String, InventoryDateTime As DateTime, Quantity As Decimal, DateType As String, CompanyId As Integer, UserId As Integer, EndDateForRD As DateTime, FluidLink As String,
											ReadingDateTime As DateTime, ProbeReading As Decimal, FromSiteId As Integer, TLD As String, RecordType As String,
																   Optional Price As Decimal = 0, Optional TLDTemperature As Decimal = 1, Optional Response_code As String = "") As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(17) As SqlParameter
			Dim ParTankInventoryId = New SqlParameter("@TankInventoryId", SqlDbType.Int)
			Dim ParTankNumber = New SqlParameter("@TankNumber", SqlDbType.NVarChar, 10)
			Dim ParENTRY_TYPE = New SqlParameter("@ENTRY_TYPE", SqlDbType.NVarChar, 20)
			Dim ParInventoryDateTime = New SqlParameter("@InventoryDateTime", SqlDbType.DateTime)
			Dim ParQuantity = New SqlParameter("@Quantity", SqlDbType.Decimal)
			Dim ParDateType = New SqlParameter("@DateType", SqlDbType.NVarChar, 2)
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParEndDateForRD = New SqlParameter("@EndDateForRD", SqlDbType.DateTime)
			Dim ParFluidLink = New SqlParameter("@FluidLink", SqlDbType.NVarChar, 20)
			Dim ParRecordType = New SqlParameter("@RecordType", SqlDbType.NVarChar, 11)
			Dim ParReadingDateTime = New SqlParameter("@ReadingDateTime", SqlDbType.DateTime)
			Dim ParProbeReading = New SqlParameter("@ProbeReading", SqlDbType.Decimal)
			Dim ParFromSiteId = New SqlParameter("@FromSiteId", SqlDbType.Int)
			Dim ParTLD = New SqlParameter("@TLD", SqlDbType.NVarChar, 50)
			Dim ParPrice = New SqlParameter("@Price", SqlDbType.Decimal)
			Dim ParTLDTemperature = New SqlParameter("@TLDTemperature", SqlDbType.Decimal)
			Dim ParResponse_code = New SqlParameter("@Response_code", SqlDbType.NVarChar, 50)

			ParTankInventoryId.Direction = ParameterDirection.Input
			ParTankNumber.Direction = ParameterDirection.Input
			ParENTRY_TYPE.Direction = ParameterDirection.Input
			ParInventoryDateTime.Direction = ParameterDirection.Input
			ParQuantity.Direction = ParameterDirection.Input
			ParDateType.Direction = ParameterDirection.Input
			ParCompanyId.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParEndDateForRD.Direction = ParameterDirection.Input
			ParFluidLink.Direction = ParameterDirection.Input
			ParRecordType.Direction = ParameterDirection.Input
			ParReadingDateTime.Direction = ParameterDirection.Input
			ParProbeReading.Direction = ParameterDirection.Input
			ParFromSiteId.Direction = ParameterDirection.Input
			ParTLD.Direction = ParameterDirection.Input
			ParPrice.Direction = ParameterDirection.Input
			ParTLDTemperature.Direction = ParameterDirection.Input
			ParResponse_code.Direction = ParameterDirection.Input

			ParTankInventoryId.Value = TankInventoryId
			ParTankNumber.Value = TankNumber
			ParENTRY_TYPE.Value = ENTRY_TYPE
			ParInventoryDateTime.Value = InventoryDateTime
			ParQuantity.Value = Quantity
			ParDateType.Value = DateType
			ParCompanyId.Value = CompanyId
			ParUserId.Value = UserId
			ParEndDateForRD.Value = EndDateForRD
			ParFluidLink.Value = FluidLink
			ParRecordType.Value = RecordType
			ParReadingDateTime.Value = ReadingDateTime
			ParProbeReading.Value = ProbeReading
			ParFromSiteId.Value = FromSiteId
			ParTLD.Value = TLD
			ParPrice.Value = Price
			ParTLDTemperature.Value = TLDTemperature
			ParResponse_code.Value = Response_code

			parcollection(0) = ParTankInventoryId
			parcollection(1) = ParTankNumber
			parcollection(2) = ParENTRY_TYPE
			parcollection(3) = ParInventoryDateTime
			parcollection(4) = ParQuantity
			parcollection(5) = ParDateType
			parcollection(6) = ParCompanyId
			parcollection(7) = ParUserId
			parcollection(8) = ParEndDateForRD
			parcollection(9) = ParFluidLink
			parcollection(10) = ParRecordType
			parcollection(11) = ParReadingDateTime
			parcollection(12) = ParProbeReading
			parcollection(13) = ParFromSiteId
			parcollection(14) = ParTLD
			parcollection(15) = ParPrice
			parcollection(16) = ParTLDTemperature
			parcollection(17) = ParResponse_code

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankInventoryInsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateTankInventory Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetTankInventorys(PersonId As Integer, RoleId As String, CustomerId As Integer, Entry_Type As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(3) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = PersonId

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId

			parcollection(2) = New SqlParameter("@CustomerId", SqlDbType.Int)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = CustomerId

			parcollection(3) = New SqlParameter("@Entry_Type", SqlDbType.NVarChar, 20)
			parcollection(3).Direction = ParameterDirection.Input
			parcollection(3).Value = Entry_Type

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankInventoryDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankInventorys Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetTankInventorybyConditions(Conditions As String, PersonId As Integer, RoleId As String) As DataTable

		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = PersonId

			parcollection(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankInventory_GetTankInventoryByConditions", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankInventorybyConditions Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetTankInventorybyId(TankInventoryId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@TankInventoryId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = TankInventoryId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankInventoryByTankInventoryId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankInventorybyId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetTankInventoryIdByCondition(DeptId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId As Integer, ENTRY_TYPE As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@DeptId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = DeptId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@ENTRY_TYPE", SqlDbType.NVarChar, 20)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = ENTRY_TYPE


			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankInventory_GetTankInventoryIdByCondition", Param)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in GetTankInventoryIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function DeleteTankInventory(ByVal TankInventoryId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParTankInventoryId = New SqlParameter("@TankInventoryId", SqlDbType.Int)
			ParTankInventoryId.Direction = ParameterDirection.Input
			ParTankInventoryId.Value = TankInventoryId
			parcollection(0) = ParTankInventoryId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankInventory_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteTankInventory Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetTankInventoryColumnNameForSearch(Entry_Type As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim parcollection(0) As SqlParameter
			Dim ParTankInventoryId = New SqlParameter("@Entry_Type", SqlDbType.NVarChar, 10)
			ParTankInventoryId.Direction = ParameterDirection.Input
			ParTankInventoryId.Value = Entry_Type
			parcollection(0) = ParTankInventoryId

			Dim ds As DataSet = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankInventory_GetColumnNameForSearch", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankInventoryColumnNameForSearch Exception is :" + ex.Message)

			Return Nothing
		Finally

		End Try
	End Function

#End Region

#Region "CustomizedExport"
	Public Function GetCustomizedExportFields() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_CustomizedExport_GetCustomizedExportFields", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetCustomizedExportFields Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetCustomizedExportTemplateById(CustomizedExportTemplateId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()


			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@CustomizedExportTemplateId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = CustomizedExportTemplateId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_CustomizedExport_GetCustomizedExportTemplateById", Param)

			Return ds

		Catch ex As Exception

			log.Error("Error occurred in GetCustomizedExportTemplateById Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function SaveUpdateCustomizedExportTemplates(CustomizedExportTemplateId As Integer, CompanyId As Integer, CustomizedExportTemplateName As String, UserId As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(3) As SqlParameter
			Dim ParCustomizedExportTemplateId = New SqlParameter("@CustomizedExportTemplateId", SqlDbType.Int)
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			Dim ParCustomizedExportTemplateName = New SqlParameter("@CustomizedExportTemplateName", SqlDbType.NVarChar, 50)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)

			ParCustomizedExportTemplateId.Direction = ParameterDirection.Input
			ParCompanyId.Direction = ParameterDirection.Input
			ParCustomizedExportTemplateName.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input

			ParCustomizedExportTemplateId.Value = CustomizedExportTemplateId
			ParCompanyId.Value = CompanyId
			ParCustomizedExportTemplateName.Value = CustomizedExportTemplateName
			ParUserId.Value = UserId

			parcollection(0) = ParCustomizedExportTemplateId
			parcollection(1) = ParCompanyId
			parcollection(2) = ParCustomizedExportTemplateName
			parcollection(3) = ParUserId

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_CustomizedExport_InsertUpdateCustomizedExportTemplates", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateCustomizedExportTemplates Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function InsertCustomizedExportTemplateDetails(dtCustomizedExportTemplateDetails As DataTable, CustomizedExportTemplateId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtCustomizedExportTemplateDetails"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtCustomizedExportTemplateDetails
			Param(0).TypeName = "dbo.CustomizedExportTemplateDetailsTemp"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@CustomizedExportTemplateId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = CustomizedExportTemplateId


			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_CustomizedExport_InsertCustomizedExportTemplateDetails", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertCustomizedExportTemplateDetails Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetCustomizedExportTemplatesByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(2) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_CustomizedExport_GetCustomizedExportTemplatesByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetCustomizedExportTemplatesByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetCustomizedExportColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_CustomizedExport_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetCustomizedExportColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function DeleteCustomizedExport(ByVal CustomizedExportTemplateId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParCustomizedExportTemplateId = New SqlParameter("@CustomizedExportTemplateId", SqlDbType.Int)
			ParCustomizedExportTemplateId.Direction = ParameterDirection.Input
			ParCustomizedExportTemplateId.Value = CustomizedExportTemplateId
			parcollection(0) = ParCustomizedExportTemplateId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_CustomizedExport_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteCustomizedExport Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function CheckCustomizedExportTemplateNameExist(ByVal CustomizedExportTemplateId As Integer, CustomizedExportTemplateName As String, CompanyId As Integer) As Boolean
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParCustomizedExportTemplateId = New SqlParameter("@CustomizedExportTemplateId", SqlDbType.Int)
			ParCustomizedExportTemplateId.Direction = ParameterDirection.Input
			ParCustomizedExportTemplateId.Value = CustomizedExportTemplateId
			parcollection(0) = ParCustomizedExportTemplateId

			Dim ParCustomizedExportTemplateName = New SqlParameter("@CustomizedExportTemplateName", SqlDbType.NVarChar, 50)
			ParCustomizedExportTemplateName.Direction = ParameterDirection.Input
			ParCustomizedExportTemplateName.Value = CustomizedExportTemplateName
			parcollection(1) = ParCustomizedExportTemplateName

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId
			parcollection(2) = ParCompanyId

			Return dal.ExecuteStoredProcedureGetBoolean("usp_tt_CustomizedExport_CheckCustomizedExportTemplateNameExist", parcollection)
		Catch ex As Exception
			log.Error("Error occurred in SiteIsExists Exception is :" + ex.Message)
			Return True
		End Try
	End Function


#End Region

#Region "Tank Chart"

	Public Function SaveUpdateTankChart(TankChartId As Integer, TankChartNumber As Integer, TankChartName As String, Description As String, TankSize As Decimal, Entries As Integer,
										FuelIncrement As Integer, CompanyId As Integer, UserId As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(8) As SqlParameter
			Dim ParTankChartId = New SqlParameter("@TankChartId", SqlDbType.Int)
			Dim ParTankChartName = New SqlParameter("@TankChartName", SqlDbType.NVarChar, 20)
			Dim ParDescription = New SqlParameter("@Description", SqlDbType.NVarChar, 50)
			Dim ParTankSize = New SqlParameter("@TankSize", SqlDbType.Decimal)
			Dim ParEntries = New SqlParameter("@Entries", SqlDbType.Int)
			Dim ParFuelIncrement = New SqlParameter("@FuelIncrement", SqlDbType.Int)
			'Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			Dim ParTankChartNumber = New SqlParameter("@TankChartNumber", SqlDbType.Int)


			ParTankChartId.Direction = ParameterDirection.Input
			ParTankChartName.Direction = ParameterDirection.Input
			ParDescription.Direction = ParameterDirection.Input
			ParTankSize.Direction = ParameterDirection.Input
			ParEntries.Direction = ParameterDirection.Input
			ParFuelIncrement.Direction = ParameterDirection.Input
			'ParFuelTypeId.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input
			ParCompanyId.Direction = ParameterDirection.Input
			ParTankChartNumber.Direction = ParameterDirection.Input

			ParTankChartId.Value = TankChartId
			ParTankChartName.Value = TankChartName
			ParDescription.Value = Description
			ParTankSize.Value = TankSize
			ParEntries.Value = Entries
			ParFuelIncrement.Value = FuelIncrement
			'ParFuelTypeId.Value = FuelTypeId
			ParUserId.Value = UserId
			ParCompanyId.Value = CompanyId
			ParTankChartNumber.Value = TankChartNumber

			parcollection(0) = ParTankChartId
			parcollection(1) = ParTankChartName
			parcollection(2) = ParDescription
			parcollection(3) = ParTankSize
			parcollection(4) = ParEntries
			parcollection(5) = ParFuelIncrement
			'parcollection(6) = ParFuelTypeId
			parcollection(6) = ParUserId
			parcollection(7) = ParCompanyId
			parcollection(8) = ParTankChartNumber

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankChart_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateTankChart Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetTankChartByTankChartId(TankChartId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@TankChartId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = TankChartId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankChart_GetTankChartByTankChartId", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetTankChartByTankChartId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetTankChartIdByCondition(TankChartId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean,
											  IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId As String, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@TankChartId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = TankChartId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankChart_GetTankChartIdByCondition", Param)

			Return result

		Catch ex As Exception

			log.Error("Error occurred in GetTankChartIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function DeleteTankChart(ByVal TankChartId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParTankChartId = New SqlParameter("@TankChartId", SqlDbType.Int)
			ParTankChartId.Direction = ParameterDirection.Input
			ParTankChartId.Value = TankChartId
			parcollection(0) = ParTankChartId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankChart_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteTankChart Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetTankChartsByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(2) {}

			Param(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = Conditions

			Param(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = PersonId

			Param(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankChart_GetTankChartsByCondition", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankChartsByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetTankChartColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankChart_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankChartColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function CheckTankChartNameExist(ByVal TankChartName As String, TankChartId As Integer, CustomerId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParTankChartName = New SqlParameter("@TankChartName", SqlDbType.NVarChar, 10)
			ParTankChartName.Direction = ParameterDirection.Input
			ParTankChartName.Value = TankChartName
			parcollection(0) = ParTankChartName

			Dim ParTankChartId = New SqlParameter("@TankChartId", SqlDbType.Int)
			ParTankChartId.Direction = ParameterDirection.Input
			ParTankChartId.Value = TankChartId
			parcollection(1) = ParTankChartId

			Dim ParCustomerId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(2) = ParCustomerId


			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_TankChart_CheckTankChartNameExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckTankChartNameExist Exception is :" + ex.Message)
			Return 0

		End Try
	End Function


	Public Function CheckTankChartNumberExist(ByVal TankChartNumber As String, TankChartId As Integer, CustomerId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(2) As SqlParameter
			Dim ParTankChartNumber = New SqlParameter("@TankChartNumber", SqlDbType.Int)
			ParTankChartNumber.Direction = ParameterDirection.Input
			ParTankChartNumber.Value = TankChartNumber
			parcollection(0) = ParTankChartNumber

			Dim ParTankChartId = New SqlParameter("@TankChartId", SqlDbType.Int)
			ParTankChartId.Direction = ParameterDirection.Input
			ParTankChartId.Value = TankChartId
			parcollection(1) = ParTankChartId

			Dim ParCustomerId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(2) = ParCustomerId


			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_TankChart_CheckTankChartNumberExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in CheckTankChartNumberExist Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function GetTankChartDetailsByTankChartId(TankChartId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim parcollection(0) As SqlParameter
			Dim ParTankChartId = New SqlParameter("@TankChartId", SqlDbType.Int)
			ParTankChartId.Direction = ParameterDirection.Input
			ParTankChartId.Value = TankChartId
			parcollection(0) = ParTankChartId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankChart_GetTankChartDetailsByTankChartId", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankChartDetailsByTankChartId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function UpdateTankChartDetail(TankChartDetailId As Integer, GallonLevel As String, UserId As Integer) As Integer '
		Dim result As Integer
		result = 0
		Try
			Dim dal = New GeneralizedDAL()
			Dim parcollection(2) As SqlParameter


			Dim ParTankChartDetailId = New SqlParameter("@TankChartDetailId", SqlDbType.Int)
			Dim ParGallonLevel = New SqlParameter("@GallonLevel", SqlDbType.NVarChar, 20)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)


			ParTankChartDetailId.Direction = ParameterDirection.Input
			ParGallonLevel.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input

			ParTankChartDetailId.Value = TankChartDetailId
			ParGallonLevel.Value = GallonLevel
			ParUserId.Value = UserId


			parcollection(0) = ParTankChartDetailId
			parcollection(1) = ParGallonLevel
			parcollection(2) = ParUserId

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankChart_UpdateTankChartDetail", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in InsertUpdatePreAuthTransaction Exception is :" + ex.Message)
			Return result
		End Try

	End Function

	Public Function SaveCoefficients(ByVal TankChartId As Integer, Coefficient_0 As Double, Coefficient_1 As Double, Coefficient_2 As Double, Coefficient_3 As Double) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(4) As SqlParameter
			Dim ParTankChartId = New SqlParameter("@TankChartId", SqlDbType.Int)
			ParTankChartId.Direction = ParameterDirection.Input
			ParTankChartId.Value = TankChartId
			parcollection(0) = ParTankChartId

			Dim ParCoefficient_0 = New SqlParameter("@Coefficient_0", SqlDbType.Real)
			ParCoefficient_0.Direction = ParameterDirection.Input
			ParCoefficient_0.Value = Coefficient_0
			parcollection(1) = ParCoefficient_0

			Dim ParCoefficient_1 = New SqlParameter("@Coefficient_1", SqlDbType.Real)
			ParCoefficient_1.Direction = ParameterDirection.Input
			ParCoefficient_1.Value = Coefficient_1
			parcollection(2) = ParCoefficient_1

			Dim ParCoefficient_2 = New SqlParameter("@Coefficient_2", SqlDbType.Real)
			ParCoefficient_2.Direction = ParameterDirection.Input
			ParCoefficient_2.Value = Coefficient_2
			parcollection(3) = ParCoefficient_2

			Dim ParCoefficient_3 = New SqlParameter("@Coefficient_3", SqlDbType.Real)
			ParCoefficient_3.Direction = ParameterDirection.Input
			ParCoefficient_3.Value = Coefficient_3
			parcollection(4) = ParCoefficient_3

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankChart_SaveCoefficients", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveCoefficients Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

#End Region

#Region "TANK"
	Public Function GetTanks(PersonId As Integer, RoleId As String, CustomerId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = PersonId

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId

			parcollection(2) = New SqlParameter("@CustomerId", SqlDbType.Int)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = CustomerId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Tank_TankDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTanks Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetTankColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Tank_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankColumnNameForSearch Exception is :" + ex.Message)

			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetTankbyConditions(Conditions As String, PersonId As Integer, RoleId As String) As DataTable

		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = PersonId

			parcollection(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Tank_GetTankbyCondition", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankbyConditions Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function DeleteTank(ByVal TankID As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParDeptID = New SqlParameter("@TankID", SqlDbType.Int)
			ParDeptID.Direction = ParameterDirection.Input
			ParDeptID.Value = TankID
			parcollection(0) = ParDeptID

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Tank_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteTank Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetTankbyId(TankId As Integer) As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(0) {}

			Param(0) = New SqlParameter("@TankId", SqlDbType.Int)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = TankId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Tank_GetTankbyTankId", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetTankbyId Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetTankIdByCondition(TankId As Integer, IsFirst As Boolean, IsLast As Boolean, IsNext As Boolean, IsPrevious As Boolean, IsCount As Boolean, PersonId As Integer, RoleId As String, CustomerId As Integer, Conditions As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter(9) {}

			Param(0) = New SqlParameter("@IsFirst", SqlDbType.Bit)
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = IsFirst

			Param(1) = New SqlParameter("@IsLast", SqlDbType.Bit)
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = IsLast

			Param(2) = New SqlParameter("@IsNext", SqlDbType.Bit)
			Param(2).Direction = ParameterDirection.Input
			Param(2).Value = IsNext

			Param(3) = New SqlParameter("@IsPrevious", SqlDbType.Bit)
			Param(3).Direction = ParameterDirection.Input
			Param(3).Value = IsPrevious

			Param(4) = New SqlParameter("@TankId", SqlDbType.Int)
			Param(4).Direction = ParameterDirection.Input
			Param(4).Value = TankId

			Param(5) = New SqlParameter("@IsCount", SqlDbType.Bit)
			Param(5).Direction = ParameterDirection.Input
			Param(5).Value = IsCount

			Param(6) = New SqlParameter("@PersonId", SqlDbType.Int)
			Param(6).Direction = ParameterDirection.Input
			Param(6).Value = PersonId

			Param(7) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			Param(7).Direction = ParameterDirection.Input
			Param(7).Value = RoleId

			Param(8) = New SqlParameter("@CustomerId", SqlDbType.Int)
			Param(8).Direction = ParameterDirection.Input
			Param(8).Value = CustomerId

			Param(9) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			Param(9).Direction = ParameterDirection.Input
			Param(9).Value = Conditions

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_Tank_GetTankIdByCondition", Param)

			Return result

		Catch ex As Exception
			log.Error("Error occurred in GetTankIdByCondition Exception is :" + ex.Message)
			Return 0
		Finally

		End Try
	End Function

	Public Function TankIDExists(ByVal TankNo As String, TankId As Integer, CustomerId As Integer, TankNAME As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(3) As SqlParameter
			Dim ParTankNo = New SqlParameter("@TankNUMBER", SqlDbType.NVarChar, 10)
			ParTankNo.Direction = ParameterDirection.Input
			ParTankNo.Value = TankNo
			parcollection(0) = ParTankNo

			Dim ParTankId = New SqlParameter("@TankId", SqlDbType.Int)
			ParTankId.Direction = ParameterDirection.Input
			ParTankId.Value = TankId
			parcollection(1) = ParTankId

			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			ParCustomerId.Direction = ParameterDirection.Input
			ParCustomerId.Value = CustomerId
			parcollection(2) = ParCustomerId

			Dim ParTankNAME = New SqlParameter("@TankNAME", SqlDbType.NVarChar, 20)
			ParTankNAME.Direction = ParameterDirection.Input
			ParTankNAME.Value = TankNAME
			parcollection(3) = ParTankNAME

			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_Tank_CheckNumberExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in TankIDExists Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function SaveUpdateTank(TankId As Integer, TankName As String, TankNo As String, Address As String, CustomerId As Integer, ExportCode As String, FuelType As Integer, RefillNotice As Integer, PROBEMacAddress As String,
								   TankChartId As Integer, PersonId As Integer, TankMonitor As Boolean, TankMonitorNumber As Integer,
								   ConstantA As Decimal, ConstantB As Decimal, ConstantC As Decimal, ConstantD As Decimal) As Integer

		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(16) As SqlParameter
			Dim ParTankId = New SqlParameter("@TankId", SqlDbType.Int)
			Dim ParTankName = New SqlParameter("@TankName", SqlDbType.NVarChar, 25)
			Dim ParTankNo = New SqlParameter("@TankNo", SqlDbType.NVarChar, 10)
			Dim ParAddress = New SqlParameter("@Address", SqlDbType.VarChar, 25)
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			Dim ParExportCode = New SqlParameter("@ExportCode", SqlDbType.NVarChar, 50)
			Dim ParFuelType = New SqlParameter("@FuelType", SqlDbType.Int)
			Dim ParRefillNotice = New SqlParameter("@RefillNotice", SqlDbType.Int)
			Dim ParPROBEMacAddress = New SqlParameter("@PROBEMacAddress", SqlDbType.NVarChar, 50)
			Dim ParTankChartId = New SqlParameter("@TankChartId", SqlDbType.Int)
			Dim ParPersonId = New SqlParameter("@PersonId", SqlDbType.Int)
			Dim ParTankMonitor = New SqlParameter("@TankMonitor", SqlDbType.Bit)
			Dim ParTankMonitorNumber = New SqlParameter("@TankMonitorNumber", SqlDbType.Int)
			Dim ParConstantA = New SqlParameter("@ConstantA", SqlDbType.Decimal)
			Dim ParConstantB = New SqlParameter("@ConstantB", SqlDbType.Decimal)
			Dim ParConstantC = New SqlParameter("@ConstantC", SqlDbType.Decimal)
			Dim ParConstantD = New SqlParameter("@ConstantD", SqlDbType.Decimal)

			ParTankId.Direction = ParameterDirection.Input
			ParTankName.Direction = ParameterDirection.Input
			ParTankNo.Direction = ParameterDirection.Input
			ParAddress.Direction = ParameterDirection.Input
			ParCustomerId.Direction = ParameterDirection.Input
			ParExportCode.Direction = ParameterDirection.Input
			ParFuelType.Direction = ParameterDirection.Input
			ParRefillNotice.Direction = ParameterDirection.Input
			ParPROBEMacAddress.Direction = ParameterDirection.Input
			ParTankChartId.Direction = ParameterDirection.Input
			ParPersonId.Direction = ParameterDirection.Input
			ParTankMonitor.Direction = ParameterDirection.Input
			ParTankMonitorNumber.Direction = ParameterDirection.Input
			ParConstantA.Direction = ParameterDirection.Input
			ParConstantB.Direction = ParameterDirection.Input
			ParConstantC.Direction = ParameterDirection.Input
			ParConstantD.Direction = ParameterDirection.Input

			ParTankId.Value = TankId
			ParTankName.Value = TankName
			ParTankNo.Value = TankNo
			ParAddress.Value = Address
			ParCustomerId.Value = CustomerId
			ParExportCode.Value = ExportCode
			ParFuelType.Value = FuelType
			ParRefillNotice.Value = IIf(RefillNotice = -1, Nothing, RefillNotice)
			ParPROBEMacAddress.Value = PROBEMacAddress
			ParTankChartId.Value = TankChartId
			ParPersonId.Value = PersonId
			ParTankMonitor.Value = TankMonitor
			ParTankMonitorNumber.Value = TankMonitorNumber
			ParConstantA.Value = ConstantA
			ParConstantB.Value = ConstantB
			ParConstantC.Value = ConstantC
			ParConstantD.Value = ConstantD


			parcollection(0) = ParTankId
			parcollection(1) = ParTankName
			parcollection(2) = ParTankNo
			parcollection(3) = ParAddress
			parcollection(4) = ParCustomerId
			parcollection(5) = ParExportCode
			parcollection(6) = ParFuelType
			parcollection(7) = ParRefillNotice
			parcollection(8) = ParPROBEMacAddress
			parcollection(9) = ParTankChartId
			parcollection(10) = ParPersonId
			parcollection(11) = ParTankMonitor
			parcollection(12) = ParTankMonitorNumber
			parcollection(13) = ParConstantA
			parcollection(14) = ParConstantB
			parcollection(15) = ParConstantC
			parcollection(16) = ParConstantD

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Tank_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateTank Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

#End Region

#Region "Tank Monitor"

	Public Function InsertInventoryVeederTankMonitorDetail(ByVal PhoneNumber As String, HubId As Integer, VeederRootMacAddress As String,
														   AppDateTime As DateTime, TankNumber As String, VRDateTime As DateTime, ProductCode As String, TankStatus As String, Volume As Decimal,
														  TCVolume As Decimal, Ullage As Decimal, Height As Decimal, Water As Decimal, Temperature As Decimal, WaterVolume As Decimal, UserId As Integer, CompanyId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(16) As SqlParameter
			Dim ParPhoneNumber = New SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 50)
			ParPhoneNumber.Direction = ParameterDirection.Input
			ParPhoneNumber.Value = PhoneNumber
			parcollection(0) = ParPhoneNumber

			Dim ParHubId = New SqlParameter("@HubId", SqlDbType.Int)
			ParHubId.Direction = ParameterDirection.Input
			ParHubId.Value = HubId
			parcollection(1) = ParHubId

			Dim ParVeederRootMacAddress = New SqlParameter("@VeederRootMacAddress", SqlDbType.NVarChar, 50)
			ParVeederRootMacAddress.Direction = ParameterDirection.Input
			ParVeederRootMacAddress.Value = VeederRootMacAddress
			parcollection(2) = ParVeederRootMacAddress

			Dim ParAppDateTime = New SqlParameter("@AppDateTime", SqlDbType.DateTime)
			ParAppDateTime.Direction = ParameterDirection.Input
			ParAppDateTime.Value = AppDateTime
			parcollection(3) = ParAppDateTime

			Dim ParTankNumber = New SqlParameter("@TankNumber", SqlDbType.NVarChar, 10)
			ParTankNumber.Direction = ParameterDirection.Input
			ParTankNumber.Value = TankNumber
			parcollection(4) = ParTankNumber

			Dim ParVRDateTime = New SqlParameter("@VRDateTime", SqlDbType.DateTime)
			ParVRDateTime.Direction = ParameterDirection.Input
			ParVRDateTime.Value = VRDateTime
			parcollection(5) = ParVRDateTime

			Dim ParProductCode = New SqlParameter("@ProductCode", SqlDbType.NVarChar, 10)
			ParProductCode.Direction = ParameterDirection.Input
			ParProductCode.Value = ProductCode
			parcollection(6) = ParProductCode

			Dim ParTankStatus = New SqlParameter("@TankStatus", SqlDbType.NVarChar, 10)
			ParTankStatus.Direction = ParameterDirection.Input
			ParTankStatus.Value = TankStatus
			parcollection(7) = ParTankStatus

			Dim ParVolume = New SqlParameter("@Volume", SqlDbType.Decimal)
			ParVolume.Direction = ParameterDirection.Input
			ParVolume.Value = Volume
			parcollection(8) = ParVolume

			Dim ParTCVolume = New SqlParameter("@TCVolume", SqlDbType.Decimal)
			ParTCVolume.Direction = ParameterDirection.Input
			ParTCVolume.Value = TCVolume
			parcollection(9) = ParTCVolume

			Dim ParUllage = New SqlParameter("@Ullage", SqlDbType.Decimal)
			ParUllage.Direction = ParameterDirection.Input
			ParUllage.Value = Ullage
			parcollection(10) = ParUllage

			Dim ParHeight = New SqlParameter("@Height", SqlDbType.Decimal)
			ParHeight.Direction = ParameterDirection.Input
			ParHeight.Value = Height
			parcollection(11) = ParHeight

			Dim ParWater = New SqlParameter("@Water", SqlDbType.Decimal)
			ParWater.Direction = ParameterDirection.Input
			ParWater.Value = Water
			parcollection(12) = ParWater

			Dim ParTemperature = New SqlParameter("@Temperature", SqlDbType.Decimal)
			ParTemperature.Direction = ParameterDirection.Input
			ParTemperature.Value = Temperature
			parcollection(13) = ParTemperature

			Dim ParWaterVolume = New SqlParameter("@WaterVolume", SqlDbType.Decimal)
			ParWaterVolume.Direction = ParameterDirection.Input
			ParWaterVolume.Value = WaterVolume
			parcollection(14) = ParWaterVolume


			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(15) = ParUserId

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId
			parcollection(16) = ParCompanyId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankMonitor_InsertInventoryVeederTankMonitorDetail", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in InsertInventoryVeederTankMonitorDetail Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function InsertDeliveryVeederTankMonitorDetail(ByVal PhoneNumber As String, HubId As Integer, VeederRootMacAddress As String,
														   AppDateTime As DateTime, TankNumber As String, VRDateTime As DateTime, ProductCode As String, StartDateTime As DateTime, EndDateTime As DateTime,
														  StartVolume As Decimal, StartTCVolume As Decimal, StartWater As Decimal, StartTemp As Decimal, EndVolume As Decimal, EndTCVolume As Decimal, EndWater As Decimal,
														  EndTemp As Decimal, StartHeight As Decimal, EndHeight As Decimal, UserId As Integer, CompanyId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(20) As SqlParameter
			Dim ParPhoneNumber = New SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 50)
			ParPhoneNumber.Direction = ParameterDirection.Input
			ParPhoneNumber.Value = PhoneNumber
			parcollection(0) = ParPhoneNumber

			Dim ParHubId = New SqlParameter("@HubId", SqlDbType.Int)
			ParHubId.Direction = ParameterDirection.Input
			ParHubId.Value = HubId
			parcollection(1) = ParHubId

			Dim ParVeederRootMacAddress = New SqlParameter("@VeederRootMacAddress", SqlDbType.NVarChar, 50)
			ParVeederRootMacAddress.Direction = ParameterDirection.Input
			ParVeederRootMacAddress.Value = VeederRootMacAddress
			parcollection(2) = ParVeederRootMacAddress

			Dim ParAppDateTime = New SqlParameter("@AppDateTime", SqlDbType.DateTime)
			ParAppDateTime.Direction = ParameterDirection.Input
			ParAppDateTime.Value = AppDateTime
			parcollection(3) = ParAppDateTime

			Dim ParTankNumber = New SqlParameter("@TankNumber", SqlDbType.NVarChar, 10)
			ParTankNumber.Direction = ParameterDirection.Input
			ParTankNumber.Value = TankNumber
			parcollection(4) = ParTankNumber

			Dim ParVRDateTime = New SqlParameter("@VRDateTime", SqlDbType.DateTime)
			ParVRDateTime.Direction = ParameterDirection.Input
			ParVRDateTime.Value = VRDateTime
			parcollection(5) = ParVRDateTime

			Dim ParProductCode = New SqlParameter("@ProductCode", SqlDbType.NVarChar, 10)
			ParProductCode.Direction = ParameterDirection.Input
			ParProductCode.Value = ProductCode
			parcollection(6) = ParProductCode

			Dim ParStartDateTime = New SqlParameter("@StartDateTime", SqlDbType.DateTime)
			ParStartDateTime.Direction = ParameterDirection.Input
			ParStartDateTime.Value = StartDateTime
			parcollection(7) = ParStartDateTime

			Dim ParEndDateTime = New SqlParameter("@EndDateTime", SqlDbType.DateTime)
			ParEndDateTime.Direction = ParameterDirection.Input
			ParEndDateTime.Value = EndDateTime
			parcollection(8) = ParEndDateTime

			Dim ParStartVolume = New SqlParameter("@StartVolume", SqlDbType.Decimal)
			ParStartVolume.Direction = ParameterDirection.Input
			ParStartVolume.Value = StartVolume
			parcollection(9) = ParStartVolume

			Dim ParStartTCVolume = New SqlParameter("@StartTCVolume", SqlDbType.Decimal)
			ParStartTCVolume.Direction = ParameterDirection.Input
			ParStartTCVolume.Value = StartTCVolume
			parcollection(10) = ParStartTCVolume

			Dim ParStartWater = New SqlParameter("@StartWater", SqlDbType.Decimal)
			ParStartWater.Direction = ParameterDirection.Input
			ParStartWater.Value = StartWater
			parcollection(11) = ParStartWater

			Dim ParStartTemp = New SqlParameter("@StartTemp", SqlDbType.Decimal)
			ParStartTemp.Direction = ParameterDirection.Input
			ParStartTemp.Value = StartTemp
			parcollection(12) = ParStartTemp

			Dim ParEndVolume = New SqlParameter("@EndVolume", SqlDbType.Decimal)
			ParEndVolume.Direction = ParameterDirection.Input
			ParEndVolume.Value = EndVolume
			parcollection(13) = ParEndVolume

			Dim ParEndTCVolume = New SqlParameter("@EndTCVolume", SqlDbType.Decimal)
			ParEndTCVolume.Direction = ParameterDirection.Input
			ParEndTCVolume.Value = EndTCVolume
			parcollection(14) = ParEndTCVolume

			Dim ParEndWater = New SqlParameter("@EndWater", SqlDbType.Decimal)
			ParEndWater.Direction = ParameterDirection.Input
			ParEndWater.Value = EndWater
			parcollection(15) = ParEndWater

			Dim ParEndTemp = New SqlParameter("@EndTemp", SqlDbType.Decimal)
			ParEndTemp.Direction = ParameterDirection.Input
			ParEndTemp.Value = EndTemp
			parcollection(16) = ParEndTemp


			Dim ParStartHeight = New SqlParameter("@StartHeight", SqlDbType.Decimal)
			ParStartHeight.Direction = ParameterDirection.Input
			ParStartHeight.Value = StartHeight
			parcollection(17) = ParStartHeight

			Dim ParEndHeight = New SqlParameter("@EndHeight", SqlDbType.Decimal)
			ParEndHeight.Direction = ParameterDirection.Input
			ParEndHeight.Value = EndHeight
			parcollection(18) = ParEndHeight


			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(19) = ParUserId

			Dim ParCompanyId = New SqlParameter("@CompanyId", SqlDbType.Int)
			ParCompanyId.Direction = ParameterDirection.Input
			ParCompanyId.Value = CompanyId
			parcollection(20) = ParCompanyId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_TankMonitor_InsertDeliveryVeederTankMonitorDetail", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in InsertInventoryVeederTankMonitorDetail Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetColumnNameForSearchForInventoryVeeder() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankMonitor_GetColumnNameForSearchForInventoryVeeder", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetColumnNameForSearchForInventoryVeeder Exception is :" + ex.Message)

			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetInventoryVeederTankMonitorDetailsbyCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable

		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = PersonId

			parcollection(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankMonitor_GetInventoryVeederTankMonitorDetailsbyCondition", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetInventoryVeederTankMonitorDetailsbyCondition Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function GetColumnNameForSearchForDeliveryVeeder() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankMonitor_GetColumnNameForSearchForDeliveryVeeder", Param)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetColumnNameForSearchForDeliveryVeeder Exception is :" + ex.Message)

			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetDeliveryVeederTankMonitorDetailsbyCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable

		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(2) {}
			Dim ds = New DataSet()


			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@PersonId", SqlDbType.Int)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = PersonId

			parcollection(2) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(2).Direction = ParameterDirection.Input
			parcollection(2).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_TankMonitor_GetDeliveryVeederTankMonitorDetailsbyCondition", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetInventoryVeederTankMonitorDetailsbyCondition Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function
#End Region

#Region "FSVM"
	Public Function InsertUpdateFSVM(VINAuthorizationID As Integer, TransactionId As Integer, CustomerId As Integer, RPM As String, SPD As String, MIL As String, ODOK As String, PC As String,
									 PID1 As String, PID2 As String, PID3 As String, PID4 As String, PID5 As String, PID6 As String, PID7 As String, PID8 As String, PID9 As String, PID10 As String,
									 PID11 As String, PID12 As String, PID13 As String, PID14 As String, PID15 As String, PID16 As String, PID17 As String, PID18 As String, PID19 As String, PID20 As String) As Integer '
		Dim result As Integer
		result = 0
		Try
			Dim dal = New GeneralizedDAL()
			Dim parcollection(27) As SqlParameter

			Dim ParVINAuthorizationID = New SqlParameter("@VINAuthorizationID", SqlDbType.Int)
			Dim ParTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)
			Dim ParCustomerId = New SqlParameter("@CustomerId", SqlDbType.Int)
			Dim ParRPM = New SqlParameter("@RPM", SqlDbType.NVarChar, 10)
			Dim ParSPD = New SqlParameter("@SPD", SqlDbType.NVarChar, 10)
			Dim ParMIL = New SqlParameter("@MIL", SqlDbType.NVarChar, 10)
			Dim ParODOK = New SqlParameter("@ODOK", SqlDbType.NVarChar, 10)
			Dim ParPC = New SqlParameter("@PC", SqlDbType.NVarChar, 10)
			Dim ParPID1 = New SqlParameter("@PID1", SqlDbType.NVarChar, 10)
			Dim ParPID2 = New SqlParameter("@PID2", SqlDbType.NVarChar, 10)
			Dim ParPID3 = New SqlParameter("@PID3", SqlDbType.NVarChar, 10)
			Dim ParPID4 = New SqlParameter("@PID4", SqlDbType.NVarChar, 10)
			Dim ParPID5 = New SqlParameter("@PID5", SqlDbType.NVarChar, 10)
			Dim ParPID6 = New SqlParameter("@PID6", SqlDbType.NVarChar, 10)
			Dim ParPID7 = New SqlParameter("@PID7", SqlDbType.NVarChar, 10)
			Dim ParPID8 = New SqlParameter("@PID8", SqlDbType.NVarChar, 10)
			Dim ParPID9 = New SqlParameter("@PID9", SqlDbType.NVarChar, 10)
			Dim ParPID10 = New SqlParameter("@PID10", SqlDbType.NVarChar, 10)
			Dim ParPID11 = New SqlParameter("@PID11", SqlDbType.NVarChar, 10)
			Dim ParPID12 = New SqlParameter("@PID12", SqlDbType.NVarChar, 10)
			Dim ParPID13 = New SqlParameter("@PID13", SqlDbType.NVarChar, 10)
			Dim ParPID14 = New SqlParameter("@PID14", SqlDbType.NVarChar, 10)
			Dim ParPID15 = New SqlParameter("@PID15", SqlDbType.NVarChar, 10)
			Dim ParPID16 = New SqlParameter("@PID16", SqlDbType.NVarChar, 10)
			Dim ParPID17 = New SqlParameter("@PID17", SqlDbType.NVarChar, 10)
			Dim ParPID18 = New SqlParameter("@PID18", SqlDbType.NVarChar, 10)
			Dim ParPID19 = New SqlParameter("@PID19", SqlDbType.NVarChar, 10)
			Dim ParPID20 = New SqlParameter("@PID20", SqlDbType.NVarChar, 10)

			ParVINAuthorizationID.Direction = ParameterDirection.Input
			ParTransactionId.Direction = ParameterDirection.Input
			ParCustomerId.Direction = ParameterDirection.Input
			ParRPM.Direction = ParameterDirection.Input
			ParSPD.Direction = ParameterDirection.Input
			ParMIL.Direction = ParameterDirection.Input
			ParODOK.Direction = ParameterDirection.Input
			ParPC.Direction = ParameterDirection.Input
			ParPID1.Direction = ParameterDirection.Input
			ParPID2.Direction = ParameterDirection.Input
			ParPID3.Direction = ParameterDirection.Input
			ParPID4.Direction = ParameterDirection.Input
			ParPID5.Direction = ParameterDirection.Input
			ParPID6.Direction = ParameterDirection.Input
			ParPID7.Direction = ParameterDirection.Input
			ParPID8.Direction = ParameterDirection.Input
			ParPID9.Direction = ParameterDirection.Input
			ParPID10.Direction = ParameterDirection.Input
			ParPID11.Direction = ParameterDirection.Input
			ParPID12.Direction = ParameterDirection.Input
			ParPID13.Direction = ParameterDirection.Input
			ParPID14.Direction = ParameterDirection.Input
			ParPID15.Direction = ParameterDirection.Input
			ParPID16.Direction = ParameterDirection.Input
			ParPID17.Direction = ParameterDirection.Input
			ParPID18.Direction = ParameterDirection.Input
			ParPID19.Direction = ParameterDirection.Input
			ParPID20.Direction = ParameterDirection.Input

			ParVINAuthorizationID.Value = VINAuthorizationID
			ParTransactionId.Value = TransactionId
			ParCustomerId.Value = CustomerId
			ParRPM.Value = RPM
			ParSPD.Value = SPD
			ParMIL.Value = MIL
			ParODOK.Value = ODOK
			ParPC.Value = PC
			ParPID1.Value = PID1
			ParPID2.Value = PID2
			ParPID3.Value = PID3
			ParPID4.Value = PID4
			ParPID5.Value = PID5
			ParPID6.Value = PID6
			ParPID7.Value = PID7
			ParPID8.Value = PID8
			ParPID9.Value = PID9
			ParPID10.Value = PID10
			ParPID11.Value = PID11
			ParPID12.Value = PID12
			ParPID13.Value = PID13
			ParPID14.Value = PID14
			ParPID15.Value = PID15
			ParPID16.Value = PID16
			ParPID17.Value = PID17
			ParPID18.Value = PID18
			ParPID19.Value = PID19
			ParPID20.Value = PID20

			parcollection(0) = ParVINAuthorizationID
			parcollection(1) = ParTransactionId
			parcollection(2) = ParCustomerId
			parcollection(3) = ParRPM
			parcollection(4) = ParSPD
			parcollection(5) = ParMIL
			parcollection(6) = ParPC
			parcollection(7) = ParPID1
			parcollection(8) = ParPID2
			parcollection(9) = ParPID3
			parcollection(10) = ParPID4
			parcollection(11) = ParPID5
			parcollection(12) = ParPID6
			parcollection(13) = ParPID7
			parcollection(14) = ParPID8
			parcollection(15) = ParPID9
			parcollection(16) = ParPID10
			parcollection(17) = ParPID11
			parcollection(18) = ParPID12
			parcollection(19) = ParPID13
			parcollection(20) = ParPID14
			parcollection(21) = ParPID15
			parcollection(22) = ParPID16
			parcollection(23) = ParPID17
			parcollection(24) = ParPID18
			parcollection(25) = ParPID19
			parcollection(26) = ParPID20
			parcollection(27) = ParODOK

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_FSVM_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in InsertUpdateFSVM Exception is :" + ex.Message)
			Return result
		End Try

	End Function

	Public Function GetVehicleRecurringMSGDetailsByTrnsactionId(TransactionId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(0) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@TransactionId", SqlDbType.Int)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = TransactionId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_VehicleRecurringMSG_GetVehicleRecurringMSGDetailsByTrnsactionId", parcollection)

			Return ds

		Catch ex As Exception
			log.Error("Error occurred in GetVehicleRecurringMSGDetailsByTrnsactionId Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function
	Public Function UpdateFSVMTransaction(SiteId As Integer, FuelTypeId As Integer, WifiSSId As String, UserId As Integer, TransactionId As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(4) As SqlParameter

			Dim ParSiteId = New SqlParameter("@SiteId", SqlDbType.Int)
			Dim ParWifiSSId = New SqlParameter("@WifiSSId", SqlDbType.NVarChar, 32)
			Dim ParFuelTypeId = New SqlParameter("@FuelTypeId", SqlDbType.Int)
			Dim ParaUserId = New SqlParameter("@UserId", SqlDbType.Int)
			Dim ParaTransactionId = New SqlParameter("@TransactionId", SqlDbType.Int)

			ParSiteId.Direction = ParameterDirection.Input
			ParWifiSSId.Direction = ParameterDirection.Input
			ParFuelTypeId.Direction = ParameterDirection.Input
			ParaUserId.Direction = ParameterDirection.Input
			ParaTransactionId.Direction = ParameterDirection.Input
			ParSiteId.Value = SiteId
			ParWifiSSId.Value = WifiSSId
			ParFuelTypeId.Value = FuelTypeId
			ParaUserId.Value = UserId
			ParaTransactionId.Value = TransactionId
			parcollection(0) = ParSiteId
			parcollection(1) = ParWifiSSId
			parcollection(2) = ParFuelTypeId
			parcollection(3) = ParaUserId
			parcollection(4) = ParaTransactionId

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Transaction_UpdateFSVMTransaction", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateFSVMTransaction Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function UpdateVehicleManualOdometerAndDifference(VehicleId As Integer, Odometer As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(2) As SqlParameter

			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.Int)
			Dim ParOdometer = New SqlParameter("@Odometer", SqlDbType.Int)
			Dim ParKilometerTOMiles = New SqlParameter("@KilometerTOMiles", SqlDbType.Decimal)

			ParVehicleId.Direction = ParameterDirection.Input
			ParOdometer.Direction = ParameterDirection.Input
			ParKilometerTOMiles.Direction = ParameterDirection.Input

			ParVehicleId.Value = VehicleId
			ParOdometer.Value = Odometer
			ParKilometerTOMiles.Value = Convert.ToDecimal(ConfigurationManager.AppSettings("KilometerTOMiles").ToString())

			parcollection(0) = ParVehicleId
			parcollection(1) = ParOdometer
			parcollection(2) = ParKilometerTOMiles


			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_Vechicle_UpdateVehicleManualOdometerAndDifference", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in UpdateVehicleManualOdometerAndDifference Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function
#End Region

#Region "Upload FSVMFirmware"
	Public Function SaveUpdateFSVMFirmware(FSVMFirmwareId As Integer, FSVMFirmwareFileName As String, FSVMFirmwareFilePath As String, Version As String, UserId As Integer) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(4) As SqlParameter
			Dim ParFSVMFirmwareId = New SqlParameter("@FSVMFirmwareId", SqlDbType.Int)
			Dim ParFSVMFirmwareFileName = New SqlParameter("@FSVMFirmwareFileName", SqlDbType.NVarChar, 2000)
			Dim ParFSVMFirmwareFilePath = New SqlParameter("@FSVMFirmwareFilePath", SqlDbType.NVarChar, 2000)
			Dim ParVersion = New SqlParameter("@Version", SqlDbType.NVarChar, 2000)
			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)

			ParFSVMFirmwareId.Direction = ParameterDirection.Input
			ParFSVMFirmwareFileName.Direction = ParameterDirection.Input
			ParFSVMFirmwareFilePath.Direction = ParameterDirection.Input
			ParVersion.Direction = ParameterDirection.Input
			ParUserId.Direction = ParameterDirection.Input

			ParFSVMFirmwareId.Value = FSVMFirmwareId
			ParFSVMFirmwareFileName.Value = FSVMFirmwareFileName
			ParFSVMFirmwareFilePath.Value = FSVMFirmwareFilePath
			ParVersion.Value = Version
			ParUserId.Value = UserId

			parcollection(0) = ParFSVMFirmwareId
			parcollection(1) = ParFSVMFirmwareFileName
			parcollection(2) = ParFSVMFirmwareFilePath
			parcollection(3) = ParVersion
			parcollection(4) = ParUserId

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_FSVMFirmware_InsertUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateFSVMFirmware Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

	Public Function GetFSVMFirmwareColumnNameForSearch() As DataTable
		Dim dal = New GeneralizedDAL()
		Try

			Dim ds As DataSet = New DataSet()

			Dim Param As SqlParameter() = New SqlParameter() {}

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSVMFirmware_GetColumnNameForSearch", Param)

			Return ds.Tables(0)

		Catch ex As Exception

			log.Error("Error occurred in GetFSVMFirmwareColumnNameForSearch Exception is :" + ex.Message)
			Return Nothing
		Finally

		End Try
	End Function

	Public Function GetFSVMFirmwaresByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter(1) {}
			Dim ds = New DataSet()

			parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
			parcollection(0).Direction = ParameterDirection.Input
			parcollection(0).Value = Conditions

			parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
			parcollection(1).Direction = ParameterDirection.Input
			parcollection(1).Value = RoleId

			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSVMFirmware_GetFSVMFirmwaresByCondition", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetFSVMFirmwaresByCondition Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function LaunchFSVMFirmware(ByVal FSVMFirmwareId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParFSVMFirmwareId = New SqlParameter("@FSVMFirmwareId", SqlDbType.Int)
			ParFSVMFirmwareId.Direction = ParameterDirection.Input
			ParFSVMFirmwareId.Value = FSVMFirmwareId
			parcollection(0) = ParFSVMFirmwareId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_FSVMFirmware_LaunchFSVMFirmware", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in LaunchFSVMFirmware Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function FSVMCheckVersionIsExist(ByVal Version As String) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(0) As SqlParameter
			Dim ParVersion = New SqlParameter("@Version", SqlDbType.NVarChar, 2000)
			ParVersion.Direction = ParameterDirection.Input
			ParVersion.Value = Version
			parcollection(0) = ParVersion

			Return dal.ExecuteStoredProcedureGetInteger("usp_tt_FSVMFirmware_CheckVersionIsExist", parcollection)

		Catch ex As Exception
			log.Error("Error occurred in FSVMCheckVersionIsExist Exception is :" + ex.Message)
			Return 0

		End Try
	End Function

	Public Function DeleteFSVMFirmware(ByVal FSVMFirmwareId As Integer, UserId As Integer) As Integer
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(1) As SqlParameter
			Dim ParFSVMFirmwareId = New SqlParameter("@FSVMFirmwareId", SqlDbType.Int)
			ParFSVMFirmwareId.Direction = ParameterDirection.Input
			ParFSVMFirmwareId.Value = FSVMFirmwareId
			parcollection(0) = ParFSVMFirmwareId

			Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
			ParUserId.Direction = ParameterDirection.Input
			ParUserId.Value = UserId
			parcollection(1) = ParUserId

			Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_FSVMFirmware_Delete", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteFSVMFirmware Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Public Function GetLaunchedFSVMFirmwareDetails() As DataTable
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection() As SqlParameter = New SqlParameter() {}
			Dim ds = New DataSet()


			ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSVMFirmware_GetLaunchedFSVMFirmwareDetails", parcollection)

			Return ds.Tables(0)

		Catch ex As Exception
			log.Error("Error occurred in GetLaunchedFSVMFirmwareDetails Exception is :" + ex.Message)
			Return Nothing
		Finally
		End Try
	End Function

	Public Function FSVMcheckLaunchedAndExistedVersionAndUpdate(VehicleId As String, version As String, personId As Integer, flag As Integer) As DataSet
		Try
			Dim result As DataSet
			Dim dal = New GeneralizedDAL()
			Dim parcollection(3) As SqlParameter

			Dim ParVehicleId = New SqlParameter("@VehicleId", SqlDbType.NVarChar, 200)
			Dim Parversion = New SqlParameter("@version", SqlDbType.NVarChar, 200)
			Dim ParpersonId = New SqlParameter("@personId", SqlDbType.Int)
			Dim Parflag = New SqlParameter("@flag", SqlDbType.Int)

			ParVehicleId.Direction = ParameterDirection.Input
			ParVehicleId.Value = VehicleId
			parcollection(0) = ParVehicleId


			Parversion.Direction = ParameterDirection.Input
			Parversion.Value = version
			parcollection(1) = Parversion

			ParpersonId.Direction = ParameterDirection.Input
			ParpersonId.Value = personId
			parcollection(2) = ParpersonId

			Parflag.Direction = ParameterDirection.Input
			Parflag.Value = flag
			parcollection(3) = Parflag

			result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSVMFirmware_CheckLaunchedAndExistedVersionAndUpdate", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in FSVMcheckLaunchedAndExistedVersionAndUpdate Exception is :" + ex.Message)
			Return Nothing

		Finally

		End Try

		Return Nothing

	End Function

	Public Function GetFSVMFirmwareVehicleIdMappingByFSVMFirmwaredID(FSVMFirmwareId As Integer) As DataTable
		Dim result As DataSet
		Try

			Dim dal = New GeneralizedDAL()
			Dim parcollection(0) As SqlParameter
			Dim ParFSVMFirmwareId = New SqlParameter("@FSVMFirmwareId", SqlDbType.Int)


			ParFSVMFirmwareId.Direction = ParameterDirection.Input


			ParFSVMFirmwareId.Value = FSVMFirmwareId


			parcollection(0) = ParFSVMFirmwareId


			result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSVMFirmware_GetCompanyNodes", parcollection)

			Return result.Tables(0)
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateFSVMFirmware Exception is :" + ex.Message)
			Return Nothing

		Finally

		End Try
	End Function

	Public Function InsertFSVMFirmwareVehicleIdMapping(dtFSVMFirmwareVehicleId As DataTable, FSVMFirmwareId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@dtFSVMFirmwareVehicleId"
			Param(0).SqlDbType = SqlDbType.Structured
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = dtFSVMFirmwareVehicleId
			Param(0).TypeName = "dbo.FSVMFirmwareVehicleIdMapping"

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@FSVMFirmwareId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = FSVMFirmwareId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_FSVMFirmware_InsertFSVMFirmwareVehicleIdMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in InsertFSVMFirmwareVehicleIdMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function DeleteFSVMFirmwareVehicleIdMapping(CompanyId As Integer, FSVMFirmwareId As Integer) As Integer
		Try
			Dim dal = New GeneralizedDAL()
			Dim Param As SqlParameter() = New SqlParameter(1) {}


			Param(0) = New SqlParameter()
			Param(0).ParameterName = "@CompanyId"
			Param(0).SqlDbType = SqlDbType.Int
			Param(0).Direction = ParameterDirection.Input
			Param(0).Value = CompanyId

			Param(1) = New SqlParameter()
			Param(1).ParameterName = "@FSVMFirmwareId"
			Param(1).SqlDbType = SqlDbType.Int
			Param(1).Direction = ParameterDirection.Input
			Param(1).Value = FSVMFirmwareId

			Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_FSVMFirmware_DeleteFSVMFirmwareVehicleIdMapping", Param)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in DeleteFSVMFirmwareVehicleIdMapping Exception is :" + ex.Message)
			Return 0
		Finally
			'sqlConn.Close()
		End Try
	End Function

	Public Function GetFSVMFirmwareById(ByVal FSVMFirmwareId As Integer) As DataSet
		Dim dal = New GeneralizedDAL()
		Try
			Dim parcollection(0) As SqlParameter
			Dim ParFSVMFirmwareId = New SqlParameter("@FSVMFirmwareId", SqlDbType.Int)
			ParFSVMFirmwareId.Direction = ParameterDirection.Input
			ParFSVMFirmwareId.Value = FSVMFirmwareId
			parcollection(0) = ParFSVMFirmwareId

			Dim result As DataSet = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSVMFirmware_GetFSVMFirmwareById", parcollection)
			Return result
		Catch ex As Exception
			log.Error("Error occurred in GetFSVMFirmwareById Exception is :" + ex.Message)
			Return Nothing
		End Try
	End Function


#End Region

#Region "Activity logs"
	Public Function CheckAlreadyDeleted() As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection() As SqlParameter = New SqlParameter() {}

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_ActivityLogs_CheckAlreadyDeleted", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in SaveUpdateCustomizedExportTemplates Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function
#End Region

#Region "Scheduled Maintenance"
	Public Function CheckAlreadyAcceptedScheduledMaintenanceNotification(Email As String) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(0) As SqlParameter
			Dim ParEmail = New SqlParameter("@Email", SqlDbType.NVarChar, 2000)
			ParEmail.Direction = ParameterDirection.Input
			ParEmail.Value = Email
			parcollection(0) = ParEmail

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_ScheduledMaintenance_CheckAlreadyAcceptedScheduledMaintenanceNotification", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in CheckAlreadyAcceptedScheduledMaintenanceNotification Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function


	Public Function AddScheduledMaintenanceNotificationAccepted(Email As String) As Integer
		Try
			Dim result As Integer
			Dim dal = New GeneralizedDAL()
			Dim parcollection(0) As SqlParameter
			Dim ParEmail = New SqlParameter("@Email", SqlDbType.NVarChar, 2000)
			ParEmail.Direction = ParameterDirection.Input
			ParEmail.Value = Email
			parcollection(0) = ParEmail

			result = dal.ExecuteStoredProcedureGetInteger("usp_tt_ScheduledMaintenance_AddScheduledMaintenanceNotificationAccepted", parcollection)

			Return result
		Catch ex As Exception
			log.Error("Error occurred in AddScheduledMaintenanceNotificationAccepted Exception is :" + ex.Message)
			Return 0

		Finally

		End Try

		Return 0

	End Function

#End Region

#Region "Upload FSNP Firmware"
    Public Function SaveUpdateFSNPFirmware(FSNPFirmwareId As Integer, FSNPFirmwareFileName As String, FSNPFirmwareFilePath As String, Version As String, UserId As Integer) As Integer
        Try
            Dim result As Integer
            Dim dal = New GeneralizedDAL()
            Dim parcollection(4) As SqlParameter
            Dim ParFSNPFirmwareId = New SqlParameter("@FSNPFirmwareId", SqlDbType.Int)
            Dim ParFSNPFirmwareFileName = New SqlParameter("@FSNPFirmwareFileName", SqlDbType.NVarChar, 2000)
            Dim ParFSNPFirmwareFilePath = New SqlParameter("@FSNPFirmwareFilePath", SqlDbType.NVarChar, 2000)
            Dim ParVersion = New SqlParameter("@Version", SqlDbType.NVarChar, 2000)
            Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)

            ParFSNPFirmwareId.Direction = ParameterDirection.Input
            ParFSNPFirmwareFileName.Direction = ParameterDirection.Input
            ParFSNPFirmwareFilePath.Direction = ParameterDirection.Input
            ParVersion.Direction = ParameterDirection.Input
            ParUserId.Direction = ParameterDirection.Input

            ParFSNPFirmwareId.Value = FSNPFirmwareId
            ParFSNPFirmwareFileName.Value = FSNPFirmwareFileName
            ParFSNPFirmwareFilePath.Value = FSNPFirmwareFilePath
            ParVersion.Value = Version
            ParUserId.Value = UserId

            parcollection(0) = ParFSNPFirmwareId
            parcollection(1) = ParFSNPFirmwareFileName
            parcollection(2) = ParFSNPFirmwareFilePath
            parcollection(3) = ParVersion
            parcollection(4) = ParUserId

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_FSNPFirmware_InsertUpdate", parcollection)

            Return result
        Catch ex As Exception
            log.Error("Error occurred in SaveUpdateFSNPFirmware Exception is :" + ex.Message)
            Return 0

        Finally

        End Try

        Return 0

    End Function

    Public Function GetFSNPFirmwareColumnNameForSearch() As DataTable
        Dim dal = New GeneralizedDAL()
        Try

            Dim ds As DataSet = New DataSet()

            Dim Param As SqlParameter() = New SqlParameter() {}

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSNPFirmware_GetColumnNameForSearch", Param)

            Return ds.Tables(0)

        Catch ex As Exception

            log.Error("Error occurred in GetFSNPFirmwareColumnNameForSearch Exception is :" + ex.Message)
            Return Nothing
        Finally

        End Try
    End Function

    Public Function GetFSNPFirmwaresByCondition(Conditions As String, PersonId As Integer, RoleId As String) As DataTable
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection() As SqlParameter = New SqlParameter(1) {}
            Dim ds = New DataSet()

            parcollection(0) = New SqlParameter("@Conditions", SqlDbType.NVarChar, 2000)
            parcollection(0).Direction = ParameterDirection.Input
            parcollection(0).Value = Conditions

            parcollection(1) = New SqlParameter("@RoleId", SqlDbType.NVarChar, 2000)
            parcollection(1).Direction = ParameterDirection.Input
            parcollection(1).Value = RoleId

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSNPFirmware_GetFSNPFirmwaresByCondition", parcollection)

            Return ds.Tables(0)

        Catch ex As Exception
            log.Error("Error occurred in GetFSNPFirmwaresByCondition Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function LaunchFSNPFirmware(ByVal FSNPFirmwareId As Integer, UserId As Integer) As Integer
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(1) As SqlParameter
            Dim ParFSNPFirmwareId = New SqlParameter("@FSNPFirmwareId", SqlDbType.Int)
            ParFSNPFirmwareId.Direction = ParameterDirection.Input
            ParFSNPFirmwareId.Value = FSNPFirmwareId
            parcollection(0) = ParFSNPFirmwareId

            Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
            ParUserId.Direction = ParameterDirection.Input
            ParUserId.Value = UserId
            parcollection(1) = ParUserId

            Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_FSNPFirmware_LaunchFSNPFirmware", parcollection)
            Return result
        Catch ex As Exception
            log.Error("Error occurred in LaunchFSNPFirmware Exception is :" + ex.Message)
            Return 0
        End Try
    End Function

    Public Function CheckFSNPVersionIsExist(ByVal Version As String) As Integer
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(0) As SqlParameter
            Dim ParVersion = New SqlParameter("@Version", SqlDbType.NVarChar, 2000)
            ParVersion.Direction = ParameterDirection.Input
            ParVersion.Value = Version
            parcollection(0) = ParVersion

            Return dal.ExecuteStoredProcedureGetInteger("usp_tt_FSNPFirmware_CheckVersionIsExist", parcollection)

        Catch ex As Exception
            log.Error("Error occurred in CheckVersionIsExist Exception is :" + ex.Message)
            Return 0

        End Try
    End Function

    Public Function DeleteFSNPFirmware(ByVal FSNPFirmwareId As Integer, UserId As Integer) As Integer
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(1) As SqlParameter
            Dim ParFSNPFirmwareId = New SqlParameter("@FSNPFirmwareId", SqlDbType.Int)
            ParFSNPFirmwareId.Direction = ParameterDirection.Input
            ParFSNPFirmwareId.Value = FSNPFirmwareId
            parcollection(0) = ParFSNPFirmwareId

            Dim ParUserId = New SqlParameter("@UserId", SqlDbType.Int)
            ParUserId.Direction = ParameterDirection.Input
            ParUserId.Value = UserId
            parcollection(1) = ParUserId

            Dim result As Integer = dal.ExecuteStoredProcedureGetInteger("usp_tt_FSNPFirmware_Delete", parcollection)
            Return result
        Catch ex As Exception
            log.Error("Error occurred in DeleteFSNPFirmware Exception is :" + ex.Message)
            Return 0
        End Try
    End Function

    Public Function GetLaunchedFSNPFirmwareDetails() As DataTable
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection() As SqlParameter = New SqlParameter() {}
            Dim ds = New DataSet()


            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSNPFirmware_GetLaunchedFSNPFirmwareDetails", parcollection)

            Return ds.Tables(0)

        Catch ex As Exception
            log.Error("Error occurred in GetLaunchedFSNPFirmwareDetails Exception is :" + ex.Message)
            Return Nothing
        Finally
        End Try
    End Function

    Public Function CheckLaunchedAndExistedFSNPVersionAndUpdate(HoseId As String, version As String, personId As Integer, flag As Integer) As DataSet
        Try
            Dim result As DataSet
            Dim dal = New GeneralizedDAL()
            Dim parcollection(3) As SqlParameter

            Dim ParSSID = New SqlParameter("@HoseId", SqlDbType.NVarChar, 200)
            Dim Parversion = New SqlParameter("@version", SqlDbType.NVarChar, 200)
            Dim ParpersonId = New SqlParameter("@personId", SqlDbType.Int)
            Dim Parflag = New SqlParameter("@flag", SqlDbType.Int)

            ParSSID.Direction = ParameterDirection.Input
            ParSSID.Value = HoseId
            parcollection(0) = ParSSID


            Parversion.Direction = ParameterDirection.Input
            Parversion.Value = version
            parcollection(1) = Parversion

            ParpersonId.Direction = ParameterDirection.Input
            ParpersonId.Value = personId
            parcollection(2) = ParpersonId

            Parflag.Direction = ParameterDirection.Input
            Parflag.Value = flag
            parcollection(3) = Parflag

            result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_CheckLaunchedAndExistedFSNPVersionAndUpdate", parcollection)

            Return result
        Catch ex As Exception
            log.Error("Error occurred in checkLaunchedAndExistedFSNPVersionAndUpdate Exception is :" + ex.Message)
            Return Nothing

        Finally

        End Try

        Return Nothing

    End Function

    Public Function GetFSNPFirmxareFluidLinkMappingByFSNPFirmwaredID(FSNPFirmwareId As Integer) As DataTable
        Dim result As DataSet
        Try

            Dim dal = New GeneralizedDAL()
            Dim parcollection(0) As SqlParameter
            Dim ParFSNPFirmwareId = New SqlParameter("@FSNPFirmwareId", SqlDbType.Int)


            ParFSNPFirmwareId.Direction = ParameterDirection.Input


            ParFSNPFirmwareId.Value = FSNPFirmwareId


            parcollection(0) = ParFSNPFirmwareId


            result = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSNPFirmware_GetCompanyNodes", parcollection)

            Return result.Tables(0)
        Catch ex As Exception
            log.Error("Error occurred in SaveUpdateFSNPFirmware Exception is :" + ex.Message)
            Return Nothing

        Finally

        End Try
    End Function

    Public Function InsertFSNPFirmwareFluidSecureLinksMapping(dtFSNPFirmwareFluidSecureLinks As DataTable, FSNPFirmwareId As Integer) As Integer
        Try
            Dim dal = New GeneralizedDAL()
            Dim Param As SqlParameter() = New SqlParameter(1) {}


            Param(0) = New SqlParameter()
            Param(0).ParameterName = "@dtFSNPFirmwareFluidSecureLinks"
            Param(0).SqlDbType = SqlDbType.Structured
            Param(0).Direction = ParameterDirection.Input
            Param(0).Value = dtFSNPFirmwareFluidSecureLinks
            Param(0).TypeName = "dbo.FSNPFirmwareFluidSecureLinksMapping"

            Param(1) = New SqlParameter()
            Param(1).ParameterName = "@FSNPFirmwareId"
            Param(1).SqlDbType = SqlDbType.Int
            Param(1).Direction = ParameterDirection.Input
            Param(1).Value = FSNPFirmwareId

            Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_FSNPFirmware_InsertFSNPFirmwareFluidSecureLinksMapping", Param)

            Return 1
        Catch ex As Exception
            log.Error("Error occurred in InsertFSNPFirmwareFluidSecureLinksMapping Exception is :" + ex.Message)
            Return 0
        Finally
            'sqlConn.Close()
        End Try
    End Function

    Public Function DeleteFSNPFirmwareFluidSecureLinksMapping(CompanyId As Integer, FSNPFirmwareId As Integer) As Integer
        Try
            Dim dal = New GeneralizedDAL()
            Dim Param As SqlParameter() = New SqlParameter(1) {}


            Param(0) = New SqlParameter()
            Param(0).ParameterName = "@CompanyId"
            Param(0).SqlDbType = SqlDbType.Int
            Param(0).Direction = ParameterDirection.Input
            Param(0).Value = CompanyId

            Param(1) = New SqlParameter()
            Param(1).ParameterName = "@FSNPFirmwareId"
            Param(1).SqlDbType = SqlDbType.Int
            Param(1).Direction = ParameterDirection.Input
            Param(1).Value = FSNPFirmwareId

            Dim result As Integer = dal.ExecuteStoredProcedureTableValuePrameter("usp_tt_FSNPFirmware_DeleteFSNPFirmwareFluidSecureLinksMapping", Param)

            Return 1
        Catch ex As Exception
            log.Error("Error occurred in DeleteFSNPFirmwareFluidSecureLinksMapping Exception is :" + ex.Message)
            Return 0
        Finally
            'sqlConn.Close()
        End Try
    End Function

    Public Function GetFSNPFirmwareById(ByVal FSNPFirmwareId As Integer) As DataSet
        Dim dal = New GeneralizedDAL()
        Try
            Dim parcollection(0) As SqlParameter
            Dim ParFSNPFirmwareId = New SqlParameter("@FSNPFirmwareId", SqlDbType.Int)
            ParFSNPFirmwareId.Direction = ParameterDirection.Input
            ParFSNPFirmwareId.Value = FSNPFirmwareId
            parcollection(0) = ParFSNPFirmwareId

            Dim result As DataSet = dal.ExecuteStoredProcedureGetDataSet("usp_tt_FSNPFirmware_GetFSNPFirmwareById", parcollection)
            Return result
        Catch ex As Exception
            log.Error("Error occurred in GetFSNPFirmwareById Exception is :" + ex.Message)
            Return Nothing
        End Try
    End Function

#End Region

End Class
