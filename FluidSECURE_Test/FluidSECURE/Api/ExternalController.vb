Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports log4net

<RoutePrefix("api/External")>
Public Class ExternalController
    Inherits ApiController

    Public Shared ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Dim OBJServiceBAL As WebServiceBAL = New WebServiceBAL()
    Dim OBJMasterBAL As MasterBAL = New MasterBAL()
    Dim steps As String = ""

#Region "Transactions"

    <HttpPost>
    <Route("ExportTransactions")>
    <Authorize>
    Public Function ExportTransactions(<FromBody()> ByVal model As TransactionsExportAPI) As HttpResponseMessage
        Dim rootObject = New RootTransactionObject()
        rootObject.TransactionsExportDataObj = New List(Of TransactionsExportDATA)()

        Try
            steps = "1"
            Dim TransactionFromDate = model.TransactionFromDate
            Dim TransactionToDate = model.TransactionToDate
            Dim CompanyName As String = model.CompanyName
            steps = "2"

            log.Debug("Transaction From Date : " & TransactionFromDate)
            log.Debug("Transaction To Date : " & TransactionToDate)
            log.Debug("Company Name : " & CompanyName)

            OBJMasterBAL = New MasterBAL()
            Dim FromDate As DateTime = DateTime.Now
            Dim ToDate As DateTime = DateTime.Now
            Try
                FromDate = Convert.ToDateTime(TransactionFromDate)
            Catch ex As Exception
                'Return BadRequest("Incorrect Transaction From Date.")
                log.Error("Incorrect Transaction From Date. Exception is : " & ex.Message)
                rootObject.ResponceText = "fail"
                rootObject.ResponceMessage = "Incorrect Transaction From Date"

                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
            End Try

            Try
                ToDate = Convert.ToDateTime(TransactionToDate)
            Catch ex As Exception
                log.Error("Incorrect Transaction To Date. Exception is : " & ex.Message)
                rootObject.ResponceText = "fail"
                rootObject.ResponceMessage = "Incorrect Transaction To Date"

                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
            End Try
            steps = "3"



            If (CompanyName <> "") Then

                Dim dtCompany As DataTable = New DataTable()
                dtCompany = OBJMasterBAL.GetCustomerDetailsByName(CompanyName)
                steps = "4"
                If dtCompany IsNot Nothing Then
                    If dtCompany.Rows.Count > 0 Then
                        Dim dsTransactionData As DataSet = New DataSet()
                        Dim strConditions As String = " and T.CompanyId = " & dtCompany.Rows(0)("CustomerId").ToString & " "
                        steps = "5"
						dsTransactionData = OBJMasterBAL.GetTransactionRptDetails(FromDate.ToString(), ToDate.ToString(), strConditions, "datetime", 1)
						If dsTransactionData IsNot Nothing Then
                            If dsTransactionData.Tables(0) IsNot Nothing Then
                                If dsTransactionData.Tables(0).Rows.Count > 0 Then
                                    steps = "6"
                                    Dim objTransaction = New TransactionsExportDATA()
                                    For index = 0 To dsTransactionData.Tables(0).Rows.Count - 1
                                        Try
                                            objTransaction = New TransactionsExportDATA()
                                            objTransaction.TransactionDateTime = IIf(dsTransactionData.Tables(0).Rows(index)("TransactionDateTime").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("TransactionDateTime").ToString())
                                            objTransaction.TransactionNumber = IIf(dsTransactionData.Tables(0).Rows(index)("TransactionNumber").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("TransactionNumber").ToString())
                                            objTransaction.CompanyName = IIf(dsTransactionData.Tables(0).Rows(index)("CompanyName").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("CompanyName").ToString())
                                            objTransaction.VehicleNumber = IIf(dsTransactionData.Tables(0).Rows(index)("VehicleNumber").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("VehicleNumber").ToString())
                                            objTransaction.PersonName = IIf(dsTransactionData.Tables(0).Rows(index)("PersonName").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("PersonName").ToString())
                                            objTransaction.PersonPin = IIf(dsTransactionData.Tables(0).Rows(index)("PersonPin").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("PersonPin").ToString())
                                            objTransaction.FluisSecureLink = IIf(dsTransactionData.Tables(0).Rows(index)("WifiSSId").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("WifiSSId").ToString())
                                            objTransaction.CurrentOdometer = IIf(dsTransactionData.Tables(0).Rows(index)("CurrentOdometer").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("CurrentOdometer").ToString())
                                            objTransaction.DepartmentNumber = IIf(dsTransactionData.Tables(0).Rows(index)("DepartmentNumber").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("DepartmentNumber").ToString())
                                            objTransaction.FuelQuantity = IIf(dsTransactionData.Tables(0).Rows(index)("FuelQuantity").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("FuelQuantity").ToString())
                                            objTransaction.FuelType = IIf(dsTransactionData.Tables(0).Rows(index)("FuelType").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("FuelType").ToString())
                                            objTransaction.Hours = IIf(dsTransactionData.Tables(0).Rows(index)("Hours").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("Hours").ToString())
                                            objTransaction.Other = IIf(dsTransactionData.Tables(0).Rows(index)("Other").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("Other").ToString())
                                            objTransaction.SiteNumber = IIf(dsTransactionData.Tables(0).Rows(index)("SiteNumber").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("SiteNumber").ToString())
                                            objTransaction.TransactionCost = IIf(dsTransactionData.Tables(0).Rows(index)("TransactionCost").ToString() Is Nothing, "", dsTransactionData.Tables(0).Rows(index)("TransactionCost").ToString())
                                            rootObject.TransactionsExportDataObj.Add(objTransaction)
                                        Catch ex As Exception
                                            log.Error(String.Format("Exception occurred while prcessing request in GetTransactionsExport. Exception is : {0}. Transaction From Date : {1}, Transaction To Date : {2}, Company : {3}", ex.Message, TransactionFromDate, TransactionToDate, CompanyName))
                                            'Return BadRequest("Incorrect Transaction To Date.")
                                        End Try
                                    Next
                                    steps = "7"
                                    rootObject.ResponceMessage = "success"
                                    rootObject.ResponceText = "Transaction Export"
                                    Return Request.CreateResponse(HttpStatusCode.OK, rootObject)

                                Else
                                    log.Error(String.Format("Transaction Data not found. Transaction From Date : {0}, Transaction To Date : {1}, Company : {2}", TransactionFromDate, TransactionToDate, CompanyName))
                                    rootObject.ResponceMessage = "success"
                                    rootObject.ResponceText = "Transaction Data not found for selected date range."
                                    Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                                End If
                            Else
                                log.Error(String.Format("Transaction Data not found. Transaction From Date : {0}, Transaction To Date : {1}, Company : {2}", TransactionFromDate, TransactionToDate, CompanyName))
                                rootObject.ResponceMessage = "fail"
                                rootObject.ResponceText = "Some error has occurred, please try again after some time."
                                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                            End If
                        Else
                            log.Error(String.Format("Transactions not found. Transaction From Date : {0}, Transaction To Date : {1}, Company : {2}", TransactionFromDate, TransactionToDate, CompanyName))
                            rootObject.ResponceMessage = "fail"
                            rootObject.ResponceText = "Some error has occurred, please try again after some time."
                            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                        End If
                    Else
                        log.Error(String.Format("Some error has occurred, please try again after some time. Data not found. Transaction From Date : {0}, Transaction To Date : {1}, Company : {2}", TransactionFromDate, TransactionToDate, CompanyName))
                        rootObject.ResponceMessage = "fail"
                        rootObject.ResponceText = "Company name not found."
                        Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                    End If
                Else
                    log.Error(String.Format("Company not found. Transaction From Date : {0}, Transaction To Date : {1}, Company : {2}", TransactionFromDate, TransactionToDate, CompanyName))
                    rootObject.ResponceMessage = "fail"
                    rootObject.ResponceText = "Some error has occurred, please try again after some time."
                    Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                End If
            Else
                log.Error(String.Format("Company not found. Transaction From Date : {0}, Transaction To Date : {1}, Company : {2}", TransactionFromDate, TransactionToDate, CompanyName))
                rootObject.ResponceMessage = "fail"
                rootObject.ResponceText = "Company name is required to fetch details. Please include company name in to api and try again."
                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
            End If

        Catch ex As Exception
            log.Error(String.Format("Exception occured while processing. step : {0}", steps))
            rootObject.ResponceMessage = "fail"
            rootObject.ResponceText = "Some error has occurred, please try again after some time."
            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
        End Try

    End Function

    <HttpPost>
    <Route("ImportTransactions")>
    <Authorize>
    Public Function ImportTransactions(ByVal model As TransactionsModels) As HttpResponseMessage
        Try

            Dim token As String = ""
            If (Request.Headers.Contains("Authorization")) Then
                token = Request.Headers.GetValues("Authorization").First()
            End If

            Dim user = Startup.oAuth.AccessTokenFormat.Unprotect(token.Replace("bearer ", ""))
            Dim userId As Integer = 0
            Dim RoleId As String = ""

            Dim dsEmail = New DataSet()
            dsEmail = OBJServiceBAL.IsEmailExists(user.Identity.Name)
            Try

                If Not dsEmail Is Nothing Then
                    If dsEmail.Tables.Count > 0 And dsEmail.Tables(0).Rows.Count > 0 Then
                        userId = dsEmail.Tables(0).Rows(0)("PersonId")
                        RoleId = dsEmail.Tables(0).Rows(0)("RoleId")
                    End If
                End If
            Catch ex As Exception

            End Try



            Dim OBJMaster As MasterBAL = New MasterBAL()
            Dim cnt As Integer = 0
            Dim ErrorCnt As Integer = 0
            Dim returnsCnt As String = ""
            Dim strLog As String = ""
            Dim rowCount As Integer = 0
            Dim rowIndex As Integer = 0
            Dim isDirty As Boolean = False
            Dim dtVehicle As DataTable = New DataTable()
            Dim dtPersonnel As DataTable = New DataTable()
            Dim dtFSLink As DataTable = New DataTable()
            Dim dtHub As DataTable = New DataTable()
            Dim HubId As Integer = 0
            Dim PersonId As Integer = 0
            Dim Offsite As Boolean = False

            For Each transaction As EachTransaction In model.TransactionsModelsObj
                Try

                    OBJMaster = New MasterBAL()

                    rowCount = rowCount + 1

                    Dim TransactionDateTime = transaction.TransactionDateTime
                    Dim VehicleNumber = transaction.VehicleNumber
                    Dim PersonPIN = transaction.PersonPIN
                    Dim FluidSecureLink = transaction.FluidSecureLink
                    Dim FuelQuantity = transaction.FuelQuantity
                    Dim CompanyName = transaction.CompanyName
                    Dim Odometer = transaction.Odometer
                    Dim Hours = transaction.Hours

                    isDirty = False
                    rowIndex = rowCount
                    'row to array and json to json
                    If (CompanyName <> "") Then
                        Dim dtCompany As DataTable = New DataTable()
                        dtCompany = OBJMasterBAL.GetCustomerDetailsByName(CompanyName)
                        If dtCompany IsNot Nothing Then
                            If dtCompany.Rows.Count > 0 Then
                                If (TransactionDateTime <> "") Then
                                    Dim dateval As DateTime
                                    If (DateTime.TryParse(TransactionDateTime, dateval)) Then

                                    Else
                                        strLog = strLog & rowIndex & ") " & "Transaction Date And Time field Is invalid. Check array  " & rowIndex & " & column 1 In json. "
                                        isDirty = True
                                    End If

                                Else
                                    strLog = strLog & rowIndex & ") " & "Transaction Date And Time field Is required. Check array  " & rowIndex & " & column 1 In json. "
                                    isDirty = True
                                End If

                                If (VehicleNumber <> "") Then

                                    dtVehicle = OBJMaster.GetVehicleByCondition(" And LTRIM(RTRIM(V.VehicleNumber)) = '" & VehicleNumber.ToLower().Trim() & "' and V.CustomerId = " & dtCompany.Rows(0)("CustomerId").ToString, userId, RoleId)

                                    If Not dtVehicle Is Nothing Then
                                        If dtVehicle.Rows.Count = 0 Then

                                            strLog = strLog & rowIndex & ") " & "Vehical Number does not exist. Check array " & rowIndex & " & column 2 in json. "
                                            isDirty = True
                                        End If
                                    End If

                                Else
                                    strLog = strLog & rowIndex & ") " & "Vehicle Number field is required. Check array " & rowIndex & " & column 2 in json. "
                                    isDirty = True
                                End If

                                If (PersonPIN <> "") Then
                                    dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and LTRIM(RTRIM(ANU.PinNumber))='" & PersonPIN.ToString().Trim() & "' and ANU.CustomerId=" & dtCompany.Rows(0)("CustomerId").ToString, userId, RoleId)
                                    If Not dtPersonnel Is Nothing Then
                                        If dtPersonnel.Rows.Count > 0 Then
                                            PersonId = dtPersonnel.Rows(0)("PersonId")
                                        End If
                                    End If
                                Else
                                    'strLog = strLog & rowIndex & ")--" & "Person PIN field is required. Check array" & rowIndex & " & column 3 in json."
                                    'isDirty = True
                                End If

                                If (FluidSecureLink <> "") Then
                                    dtFSLink = OBJMaster.GetHoseByCondition("And LTRIM(RTRIM(h.WifiSSId)) ='" & FluidSecureLink.ToString().Trim() & "' and h.CustomerID =" & dtCompany.Rows(0)("CustomerId").ToString, userId, RoleId)
                                    If Not dtFSLink Is Nothing Then
                                        If dtFSLink.Rows.Count = 0 Then
                                            strLog = strLog & rowIndex & ") " & "FluidSecure Link field does not exist.. Check array" & rowIndex & " & column 4 in json. "
                                            isDirty = True
                                        Else
                                            dtHub = OBJMaster.GetPersonSiteMappingBySiteId(dtFSLink.Rows(0)("SiteId"), dtCompany.Rows(0)("CustomerId").ToString)
                                            If Not dtHub Is Nothing Then
                                                If dtHub.Rows.Count > 0 Then
                                                    HubId = dtHub.Rows(0)("PersonId")
                                                    If PersonId = 0 Then
                                                        PersonId = dtHub.Rows(0)("PersonId")
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Else

                                        strLog = strLog & rowIndex & ") " & "FluidSecure Link field is required. Check array" & rowIndex & " & column 4 in json. "
                                        isDirty = True
                                    End If
                                Else
                                    Offsite = True
                                End If

                                If (FuelQuantity <> "") Then
                                    Dim Fuelval As Decimal
                                    If Not Decimal.TryParse(FuelQuantity, Fuelval) Then
                                        strLog = strLog & rowIndex & ") " & "Fuel Quantity value is invalid. Check array" & rowIndex & " & column 5 in json. "
                                        isDirty = True
                                    End If
                                Else
                                    strLog = strLog & rowIndex & ") " & "Fuel Quantity field is required. Check array" & rowIndex & " & column 5 in json. "
                                    isDirty = True
                                End If

                                If (Odometer <> "") Then
                                    Dim Odovalue As Integer
                                    If Not Integer.TryParse(Odometer, Odovalue) Then
                                        strLog = strLog & rowIndex & ") " & "Odometer value is invalid. Check array" & rowIndex & " & column 6 in json. "
                                        isDirty = True
                                    End If
                                End If

                                If (Hours <> "") Then
                                    Dim Hoursval As Integer
                                    If Not Integer.TryParse(Hours, Hoursval) Then
                                        strLog = strLog & rowIndex & ") " & "Hours value is invalid. Check array" & rowIndex & " & column 7 in json. "
                                        isDirty = True
                                    End If
                                End If

                                If (isDirty = False) Then
                                    Dim SiteId As Integer = 0
                                    Dim FuelTypeId As Integer = 0
                                    If dtFSLink.Rows.Count > 0 Then
                                        SiteId = dtFSLink.Rows(0)("SiteId")
                                        FuelTypeId = dtFSLink.Rows(0)("FuelTypeId")
                                    End If

                                    Dim dsTransactionValuesData As DataSet
                                    Dim DepartmentName As String = ""
                                    Dim FuelTypeName As String = ""
                                    Dim Email As String = ""
                                    Dim PersonName As String = ""

                                    Dim VehicleName As String = ""

                                    dsTransactionValuesData = OBJMaster.GetTransactionColumnsValueForSave("", FuelTypeId, PersonId, dtVehicle.Rows(0)("VehicleId"))

                                    If dsTransactionValuesData IsNot Nothing Then
                                        If dsTransactionValuesData.Tables.Count > 0 Then
                                            If dsTransactionValuesData.Tables(0) IsNot Nothing And dsTransactionValuesData.Tables(0).Rows.Count > 0 Then
                                                DepartmentName = dsTransactionValuesData.Tables(0).Rows(0)("DeptName").ToString()
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
                                            Else
                                                CompanyName = dtCompany.Rows(0)("CustomerId").ToString
                                            End If
                                            If dsTransactionValuesData.Tables(4) IsNot Nothing And dsTransactionValuesData.Tables(4).Rows.Count > 0 Then
                                                VehicleName = dsTransactionValuesData.Tables(4).Rows(0)("VehicleName").ToString()
                                            End If
                                        End If
                                    End If

                                    Dim result As Integer = OBJMaster.InsertUpdateTransaction(dtVehicle.Rows(0)("VehicleId"), SiteId, PersonId, Odometer, Convert.ToDecimal(FuelQuantity), FuelTypeId, 0, FluidSecureLink,
                                                             Convert.ToDateTime(TransactionDateTime), 0, userId, "W", 0, "", "", "", dtVehicle.Rows(0)("VehicleNumber"), dtVehicle.Rows(0)("DepartmentId"),
                                                             PersonPIN, "", Hours, False, False, 2, HubId, -1,
                                                                VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, Offsite, Convert.ToInt32(dtCompany.Rows(0)("CustomerId").ToString), 0, 0, 0, 0, True)

                                    If (result = -1) Then
                                        cnt = cnt + 1
                                    ElseIf (result > 1) Then
                                        cnt = cnt + 1
                                    Else
                                        ErrorCnt = ErrorCnt + 1
                                    End If
                                Else
                                    ErrorCnt = ErrorCnt + 1
                                End If

                            Else
                                ErrorCnt = ErrorCnt + 1
                                strLog = strLog & rowIndex & ") " & "Company not found. Check array" & rowIndex & " & column 7 in json. "
                            End If
                        Else
                            ErrorCnt = ErrorCnt + 1
                            strLog = strLog & rowIndex & ") " & "Company not found. Check array" & rowIndex & " & column 7 in json. "
                        End If
                    Else
                        ErrorCnt = ErrorCnt + 1
                        strLog = strLog & rowIndex & ") " & "Company name required. Check array" & rowIndex & " & column 7 in json. "
                    End If

                Catch ex As Exception
                    ErrorCnt = ErrorCnt + 1
                    strLog = strLog & rowIndex & ") " & "Error occured while processing request. array" & rowIndex & " & column 7 in json. "
                End Try
            Next

            Dim resmessage As String = cnt & " transactions imported successfully "

            If (strLog.Trim() <> "") Then
                resmessage = resmessage & "," & ErrorCnt & " caused some errors. Error is : " & strLog
            End If

            Dim rootObject = New RootTransactionObject()
            rootObject.ResponceText = "success"
            rootObject.ResponceMessage = resmessage

            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)

        Catch ex As Exception
            log.Error(String.Format("Exception occured while processing."))
            Dim rootObject = New RootTransactionObject()
            rootObject.ResponceMessage = "fail"
            rootObject.ResponceText = "Some error has occurred, please try again after some time."
            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
        End Try

    End Function

#End Region

#Region "Departments"

    <HttpPost>
    <Route("ExportDepartments")>
    <Authorize>
    Public Function ExportDepartments(<FromBody()> ByVal model As DepartmentsExportAPI) As HttpResponseMessage
        Dim rootObject = New RootDepartmentObject()
        rootObject.DepartmentExportDataObj = New List(Of DepartmentPassingModel)()

        Try
            steps = "1"
            Dim CompanyName As String = model.CompanyName
            steps = "2"
            log.Debug("Company Name : " & CompanyName)

            OBJMasterBAL = New MasterBAL()

            If (CompanyName <> "") Then

                Dim dtCompany As DataTable = New DataTable()
                dtCompany = OBJMasterBAL.GetCustomerDetailsByName(CompanyName)
                steps = "4"
                If dtCompany IsNot Nothing Then
                    If dtCompany.Rows.Count > 0 Then
                        Dim dtDeptData As DataTable = New DataTable()
                        Dim strConditions As String = " and D.CustomerId = " & dtCompany.Rows(0)("CustomerId").ToString & " "
                        steps = "5"
                        dtDeptData = OBJMasterBAL.GetDeptDetails(strConditions)

                        If dtDeptData IsNot Nothing Then
                            If dtDeptData.Rows.Count > 0 Then
                                steps = "6"
                                Dim objDepartment = New DepartmentPassingModel()
                                For index = 0 To dtDeptData.Rows.Count - 1
                                    Try

                                        objDepartment = New DepartmentPassingModel()
                                        objDepartment.CompanyName = IIf(dtDeptData.Rows(index)("CustomerName").ToString() Is Nothing, "", dtDeptData.Rows(index)("CustomerName").ToString())
                                        objDepartment.DepartmentName = IIf(dtDeptData.Rows(index)("DeptName").ToString() Is Nothing, "", dtDeptData.Rows(index)("DeptName").ToString())
                                        objDepartment.DepartmentNumber = IIf(dtDeptData.Rows(index)("DeptNumber").ToString() Is Nothing, "", dtDeptData.Rows(index)("DeptNumber").ToString())
                                        objDepartment.Address = IIf(dtDeptData.Rows(index)("Address1").ToString() Is Nothing, "", dtDeptData.Rows(index)("Address1").ToString())
                                        objDepartment.Address2 = IIf(dtDeptData.Rows(index)("Address2").ToString() Is Nothing, "", dtDeptData.Rows(index)("Address2").ToString())
                                        objDepartment.AccountNumber = IIf(dtDeptData.Rows(index)("Acct_Id").ToString() Is Nothing, "", dtDeptData.Rows(index)("Acct_Id").ToString())
                                        objDepartment.ExportCode = IIf(dtDeptData.Rows(index)("ExportCode").ToString() Is Nothing, "", dtDeptData.Rows(index)("ExportCode").ToString())
                                        rootObject.DepartmentExportDataObj.Add(objDepartment)

                                    Catch ex As Exception
                                        log.Error(String.Format("Exception occurred while prcessing request in GetDeptDetails. Exception is : {0}. Company : {1}", ex.Message, CompanyName))
                                        'Return BadRequest("Incorrect Transaction To Date.")
                                    End Try
                                Next
                                steps = "7"
                                rootObject.ResponceMessage = "success"
                                rootObject.ResponceText = "Department Export"
                                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)

                            Else
                                log.Error(String.Format("Deparments not found. Company : {0}", CompanyName))
                                rootObject.ResponceMessage = "success"
                                rootObject.ResponceText = "Deparments for selected company."
                                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                            End If
                        Else
                            log.Error(String.Format("Error occurred while fetching departments. Company : {0}", CompanyName))
                            rootObject.ResponceMessage = "fail"
                            rootObject.ResponceText = "Some error has occurred, please try again after some time."
                            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                        End If
                    Else
                        log.Error(String.Format("Company not found. Company : {0}", CompanyName))
                        rootObject.ResponceMessage = "fail"
                        rootObject.ResponceText = "Company not found."
                        Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                    End If
                Else
                    log.Error(String.Format("Error occurred while fetching company details. Company : {0}", CompanyName))
                    rootObject.ResponceMessage = "fail"
                    rootObject.ResponceText = "Some error has occurred, please try again after some time."
                    Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
                End If
            Else
                log.Error(String.Format("Company not found. Company : {0}", CompanyName))
                rootObject.ResponceMessage = "fail"
                rootObject.ResponceText = "Company name is required to fetch details. Please include company name in to api and try again.."
                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
            End If

        Catch ex As Exception
            log.Error(String.Format("Exception occured while processing. step : {0}", steps))
            rootObject.ResponceMessage = "fail"
            rootObject.ResponceText = "Some error has occurred, please try again after some time."
            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
        End Try
    End Function

    <HttpPost>
    <Route("ImportDepartments")>
    <Authorize>
    Public Function ImportDepartments(ByVal model As DepartmentModels) As HttpResponseMessage
        Try

            Dim token As String = ""
            If (Request.Headers.Contains("Authorization")) Then
                token = Request.Headers.GetValues("Authorization").First()
            End If

            Dim user = Startup.oAuth.AccessTokenFormat.Unprotect(token.Replace("bearer ", ""))
            Dim userId As Integer = 0
            Dim RoleId As String = ""
            Dim userCustomerId As Integer = 0
            Dim rootObject = New RootDepartmentObject()

            Dim dsEmail = New DataSet()
            dsEmail = OBJServiceBAL.IsEmailExists(user.Identity.Name)
            Try

                If Not dsEmail Is Nothing Then
                    If dsEmail.Tables.Count > 0 And dsEmail.Tables(0).Rows.Count > 0 Then
                        userId = dsEmail.Tables(0).Rows(0)("PersonId")
                        RoleId = dsEmail.Tables(0).Rows(0)("RoleId")
                        userCustomerId = dsEmail.Tables(0).Rows(0)("CustomerId")
                    End If
                End If
            Catch ex As Exception
                log.Error(String.Format("Exception occured in IsEmailExists."))
                rootObject.ResponceMessage = "fail"
                rootObject.ResponceText = "Some error has occurred, please try again after some time."
                Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
            End Try

            Dim userRole = RoleActions.GetRolesById(RoleId).Name

            Dim OBJMaster As MasterBAL = New MasterBAL()
            Dim cnt As Integer = 0
            Dim ErrorCnt As Integer = 0
            Dim returnsCnt As String = ""
            Dim strLog As String = ""
            Dim rowCount As Integer = 0
            Dim rowIndex As Integer = 0
            Dim isDirty As Boolean = False
            Dim PersonId As Integer = 0

            For Each department As DepartmentPassingModel In model.DepartmentPassingObj
                Try

                    OBJMaster = New MasterBAL()

                    rowCount = rowCount + 1

                    Dim DepartmentName = department.DepartmentName
                    Dim DepartmentNumber = department.DepartmentNumber
                    Dim Address = department.Address
                    Dim Address2 = department.Address2
                    Dim AccountNumber = department.AccountNumber
                    Dim CompanyName = department.CompanyName
                    Dim ExportCode = department.ExportCode

                    isDirty = False
                    rowIndex = rowCount
                    'row to array and json to json
                    If (CompanyName <> "") Then
                        Dim dtCompany As DataTable = New DataTable()
                        dtCompany = OBJMasterBAL.GetCustomerDetailsByName(CompanyName)
                        If dtCompany IsNot Nothing Then
                            If dtCompany.Rows.Count > 0 Then

                                'check user is valid for selected company
                                Dim isValid As Boolean = CheckValidPersonForCompany(userRole, PersonId, RoleId, userCustomerId)

                                If (isValid = False) Then
                                    strLog = strLog & rowIndex & ") " & "--" & "Access denied for company : " & CompanyName & ". Check Row  " & rowIndex & " & column 1 in json."
                                    isDirty = True
                                    ErrorCnt = ErrorCnt + 1
                                    Continue For
                                End If

                                Dim CheckIdExists As Integer = 0
                                CheckIdExists = OBJMaster.DeptIDExists(DepartmentNumber, 0, dtCompany.Rows(0)("CustomerId").ToString, DepartmentName)

                                If (DepartmentName <> "") Then

                                    If CheckIdExists = -2 Then
                                        strLog = strLog & rowIndex & ") " & "-- Department Name (" & DepartmentName & ") is already exist. Check Row  " & rowIndex & " & column 2 in json."
                                        isDirty = True
                                    ElseIf (DepartmentName.ToString().Length > 40) Then
                                        strLog = strLog & rowIndex & ") " & "--" & "Department Name (" & DepartmentName & ") is must be less than equal to 40 characters. Check Row  " & rowIndex & " & column 2 in json."
                                        isDirty = True
                                    End If
                                Else
                                    strLog = strLog & rowIndex & ") " & "--" & "Department Name field is required. Check Row  " & rowIndex & " & column 2 in json."
                                    isDirty = True
                                End If

                                If (DepartmentNumber <> "") Then
                                    If CheckIdExists = -1 Then
                                        strLog = strLog & rowIndex & ") " & "-- Department Number (" & DepartmentNumber & ") is already exist. Check Row  " & rowIndex & " & column 3 in json."
                                        isDirty = True
                                    ElseIf (DepartmentNumber.ToString().Length > 10) Then
                                        strLog = strLog & rowIndex & ") " & "--" & "Department Number (" & DepartmentNumber & ") is must be less than equal to 10 characters. Check Row  " & rowIndex & " & column 3 in json."
                                        isDirty = True
                                    End If
                                Else
                                    strLog = strLog & rowIndex & ") " & "--" & "Department Number field is required. Check Row  " & rowIndex & " & column 3 in json."
                                    isDirty = True
                                End If

                                If (AccountNumber <> "") Then
                                    If (AccountNumber.ToString().Length > 10) Then
                                        strLog = strLog & rowIndex & ") " & "--" & "Account number (" & AccountNumber & ") is must be less than equal to 10 characters. Check Row  " & rowIndex & " & column 4 in json."
                                        isDirty = True
                                    End If
                                End If

                                If (Address.ToString().Length > 25) Then
                                    strLog = strLog & rowIndex & ") " & "--" & " Address (" & Address & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 5 in json."
                                    isDirty = True
                                End If

                                If (Address2.ToString().Length > 25) Then
                                    strLog = strLog & rowIndex & ") " & "--" & "Address 2 (" & Address2 & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 6 in json."
                                    isDirty = True
                                End If

                                If (ExportCode.ToString().Length > 25) Then
                                    strLog = strLog & rowIndex & ") " & "--" & "Export code (" & ExportCode & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 7 in json."
                                    isDirty = True
                                End If

                                If (isDirty = False) Then
                                    Dim result As Integer = OBJMaster.SaveUpdateDept(0, DepartmentName, DepartmentNumber, Address, Address2, AccountNumber, dtCompany.Rows(0)("CustomerId").ToString, ExportCode, Convert.ToInt32(PersonId), 0, 0.0, 0.0, 0.0, 0.0, 0)
                                    If (result > 0) Then
                                        cnt = cnt + 1
                                    Else
                                        ErrorCnt = ErrorCnt + 1
                                    End If
                                Else
                                    ErrorCnt = ErrorCnt + 1
                                End If

                            Else
                                ErrorCnt = ErrorCnt + 1
                                strLog = strLog & rowIndex & ") " & "Company not found. Check array" & rowIndex & " & column 7 in json. "
                            End If
                        Else
                            ErrorCnt = ErrorCnt + 1
                            strLog = strLog & rowIndex & ") " & "Company not found. Check array" & rowIndex & " & column 7 in json. "
                        End If
                    Else
                        ErrorCnt = ErrorCnt + 1
                        strLog = strLog & rowIndex & ") " & "Company name required. Check array" & rowIndex & " & column 7 in json. "
                    End If

                Catch ex As Exception
                    ErrorCnt = ErrorCnt + 1
                    strLog = strLog & rowIndex & ") " & "Error occured while processing request. array" & rowIndex & " & column 7 in json. "
                End Try
            Next

            Dim resmessage As String = cnt & " department imported successfully "

            If (strLog.Trim() <> "") Then
                resmessage = resmessage & "," & ErrorCnt & " caused some errors. Error is : " & strLog
            End If

            rootObject = New RootDepartmentObject()
            rootObject.ResponceText = "success"
            rootObject.ResponceMessage = resmessage

            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)

        Catch ex As Exception
            log.Error(String.Format("Exception occured while processing."))
            Dim rootObject = New RootDepartmentObject()
            rootObject.ResponceMessage = "fail"
            rootObject.ResponceText = "Some error has occurred, please try again after some time."
            Return Request.CreateResponse(HttpStatusCode.OK, rootObject)
        End Try

    End Function


#End Region
#Region "Common"
    Private Function CheckValidPersonForCompany(userRole, PersonId, RoleId, userCustomerId) As Integer

        Dim isValid As Boolean = False
        OBJMasterBAL = New MasterBAL()
        If (userRole = "GroupAdmin") Then
            Dim dtCustOld As DataTable = New DataTable()
            dtCustOld = OBJMasterBAL.GetCustomerDetailsByPersonID(Convert.ToInt32(PersonId), RoleId.ToString(), userCustomerId)
            For Each drCusts As DataRow In dtCustOld.Rows
                If (drCusts("CustomerId") = userCustomerId) Then
                    isValid = True
                    Exit For
                End If
            Next
        End If

        If (Not userRole = "SuperAdmin" And Not isValid = True) Then

            Dim dtCustOld As DataTable = New DataTable()

            dtCustOld = OBJMasterBAL.GetCustomerDetailsByPersonID(Convert.ToInt32(PersonId), RoleId.ToString(), userCustomerId)

            If (dtCustOld.Rows(0)("CustomerId").ToString() <> userCustomerId) Then
                Return 0
            End If
        End If

        Return 1
    End Function
#End Region

End Class
