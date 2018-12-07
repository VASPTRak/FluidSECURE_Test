Imports System.Web
Imports System.Web.Services
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
Imports Newtonsoft.Json

Public Class FluidSecureAPI
    Implements System.Web.IHttpHandler

    Public Shared ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Dim OBJServiceBAL As WebServiceBAL = New WebServiceBAL()
    Dim OBJMasterBAL As MasterBAL = New MasterBAL()
    Dim steps As String = "0"
    Shared IPAddress As String
    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            log4net.Config.XmlConfigurator.Configure()

            context.Request.InputStream.Position = 0
            TransactionsImport(context)

            context.Request.InputStream.Position = 0
            GetTransactionsExport(context)

        Catch ex As Exception
            log.Error("Exception occurred while processing request. Exception is :" & ex.Message & " . in step : " & steps)
            If (Not ex.InnerException Is Nothing) Then
                log.Error("Inner Expcetion is :" & ex.InnerException.Message & " .")
            End If
            context.Response.Status = "Bad Request"
            context.Response.StatusCode = HttpStatusCode.BadRequest
        End Try
    End Sub


    Private Sub GetTransactionsExport(context As HttpContext)
        Dim requestJson As String = ""
        Try
            steps = "Start03"
            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data

                log.Info("GetTransactionsExport : " & data)

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(TransactionsExportAPI))
                Dim TransactionFromDate = DirectCast(serJsonDetails, TransactionsExportAPI).TransactionFromDate
                Dim TransactionToDate = DirectCast(serJsonDetails, TransactionsExportAPI).TransactionToDate
                Dim CompanyName = DirectCast(serJsonDetails, TransactionsExportAPI).CompanyName

                'Dim UpgradeTransactionStatusResponseObj = New UpgradeTransactionStatusResponse()
                Dim json As String

                log.Debug("Transaction From Date : " & TransactionFromDate)
                log.Debug("Transaction To Date : " & TransactionToDate)
                log.Debug("Company Name : " & CompanyName)

                OBJMasterBAL = New MasterBAL()
                Dim FromDate As DateTime = DateTime.Now
                Dim ToDate As DateTime = DateTime.Now
                Try
                    FromDate = Convert.ToDateTime(TransactionFromDate)
                Catch ex As Exception
                    ErrorInAPI(context, "fail", "Incorrect Transaction 'From' Date.")
                    Return
                End Try

                Try
                    ToDate = Convert.ToDateTime(TransactionToDate)
                Catch ex As Exception
                    ErrorInAPI(context, "fail", "Incorrect Transaction 'TO' Date.")
                    Return
                End Try

                steps = "Start04"

                If (CompanyName <> "") Then

                    Dim dtCompany As DataTable = New DataTable()
                    dtCompany = OBJMasterBAL.GetCustomerDetailsByName(CompanyName)
                    If dtCompany IsNot Nothing Then
                        If dtCompany.Rows.Count > 0 Then
                            Dim dsTransactionData As DataSet = New DataSet()
                            Dim strConditions As String = " and T.CompanyId = " & dtCompany.Rows(0)("CustomerId").ToString & " "
                            dsTransactionData = OBJMasterBAL.GetTransactionRptDetails(FromDate.ToString(), ToDate.ToString(), strConditions, "datetime")
                            If dsTransactionData IsNot Nothing Then
                                If dsTransactionData.Tables(0) IsNot Nothing Then
                                    If dsTransactionData.Tables(0).Rows.Count > 0 Then
                                        Dim rootObject = New RootTransactionObject()
                                        rootObject.TransactionsExportDataObj = New List(Of TransactionsExportDATA)()
                                        Dim objTransaction = New TransactionsExportDATA()
                                        For index = 0 To dsTransactionData.Tables(0).Rows.Count - 1
                                            Try
                                                objTransaction.CompanyName = IIf(dsTransactionData.Tables(0).Rows(0)("CompanyName") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("CompanyName"))
                                                objTransaction.CurrentOdometer = IIf(dsTransactionData.Tables(0).Rows(0)("CurrentOdometer") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("CurrentOdometer"))
                                                objTransaction.DepartmentNumber = IIf(dsTransactionData.Tables(0).Rows(0)("DepartmentNumber") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("DepartmentNumber"))
                                                objTransaction.FuelQuantity = IIf(dsTransactionData.Tables(0).Rows(0)("FuelQuantity") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("FuelQuantity"))
                                                objTransaction.FuelType = IIf(dsTransactionData.Tables(0).Rows(0)("FuelType") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("FuelType"))
                                                objTransaction.Hours = IIf(dsTransactionData.Tables(0).Rows(0)("Hours") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("Hours"))
                                                objTransaction.Other = IIf(dsTransactionData.Tables(0).Rows(0)("Other") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("Other"))
                                                objTransaction.OtherLabel = IIf(dsTransactionData.Tables(0).Rows(0)("OtherLabel") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("OtherLabel"))
                                                objTransaction.PersonPin = IIf(dsTransactionData.Tables(0).Rows(0)("PersonPin") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("PersonPin"))
                                                objTransaction.PulserRatio = IIf(dsTransactionData.Tables(0).Rows(0)("PulserRatio") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("PulserRatio"))
                                                objTransaction.Pulses = IIf(dsTransactionData.Tables(0).Rows(0)("Pulses") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("Pulses"))
                                                objTransaction.PumpOffTime = IIf(dsTransactionData.Tables(0).Rows(0)("PumpOffTime") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("PumpOffTime"))
                                                objTransaction.PumpOnTime = IIf(dsTransactionData.Tables(0).Rows(0)("PumpOnTime") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("PumpOnTime"))
                                                objTransaction.SiteNumber = IIf(dsTransactionData.Tables(0).Rows(0)("SiteNumber") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("SiteNumber"))
                                                objTransaction.TankNumber = IIf(dsTransactionData.Tables(0).Rows(0)("TankNumber") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("TankNumber"))
                                                objTransaction.TransactionCost = IIf(dsTransactionData.Tables(0).Rows(0)("TransactionCost") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("TransactionCost"))
                                                objTransaction.TransactionDateTime = IIf(dsTransactionData.Tables(0).Rows(0)("TransactionDateTime") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("TransactionDateTime"))
                                                objTransaction.TransactionNumber = IIf(dsTransactionData.Tables(0).Rows(0)("TransactionNumber") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("TransactionNumber"))
                                                objTransaction.VehicleNumber = IIf(dsTransactionData.Tables(0).Rows(0)("VehicleNumber") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("VehicleNumber"))
                                                objTransaction.WifiSSId = IIf(dsTransactionData.Tables(0).Rows(0)("WifiSSId") Is Nothing, "", dsTransactionData.Tables(0).Rows(0)("WifiSSId"))
                                                rootObject.TransactionsExportDataObj.Add(objTransaction)
                                            Catch ex As Exception
                                                log.Error("Exception occurred while prcessing request in GetTransactionsExport. Exception Is : " & ex.Message & " . Details : " & requestJson)
                                                ErrorInAPI(context, "fail", "Bad request.")
                                                Return
                                            End Try
                                        Next
                                        rootObject.ResponceMessage = "success"
                                        rootObject.ResponceText = "Transaction Export"
                                        json = javaScriptSerializer.Serialize(rootObject)

                                        'Dim rows As New List(Of Dictionary(Of String, Object))()
                                        'Dim row As Dictionary(Of String, Object) = Nothing
                                        'For Each dr As DataRow In dsTransactionData.Tables(0).Rows
                                        '    row = New Dictionary(Of String, Object)()
                                        '    For Each dc As DataColumn In dsTransactionData.Tables(0).Columns
                                        '        row.Add(dc.ColumnName.Trim(), dr(dc))
                                        '    Next
                                        '    rows.Add(row)
                                        'Next

                                    Else
                                        ErrorInAPI(context, "fail", "Transaction Data not found.")
                                        Return
                                    End If
                                Else
                                    ErrorInAPI(context, "fail", "Transaction Data not found.")
                                    Return
                                End If
                            Else
                                ErrorInAPI(context, "fail", "Transaction Data not found.")
                                Return
                            End If
                        Else
                            ErrorInAPI(context, "fail", "Company name not found.")
                            Return
                        End If
                    Else
                        ErrorInAPI(context, "fail", "Company name not found.")
                        Return
                    End If
                Else
                    ErrorInAPI(context, "fail", "Company name required.")
                    Return
                End If

                steps = "Start05"

                context.Response.Write(json)
                context.Response.Status = "Request completed."
                context.Response.StatusCode = HttpStatusCode.OK
                Return
            End Using

        Catch ex As Exception

            'Dim TransactionFailedLog As ILog = LogManager.GetLogger("RollingLogFileAppender2")
            log.Error("Exception occurred while prcessing request in GetTransactionsExport. Exception is :" & ex.Message & " . Details : " & requestJson)
            ErrorInAPI(context, "fail", "Bad request.")
        End Try
        Return
    End Sub

    Private Sub ErrorInAPI(context As HttpContext, ResponceMessage As String, errorString As String)
        Dim javaScriptSerializer = New JavaScriptSerializer()
        Dim rootOject = New TransactionCompleteResponce()
        rootOject.ResponceMessage = ResponceMessage
        rootOject.ResponceText = errorString

        Dim json As String
        json = javaScriptSerializer.Serialize(rootOject)
        context.Response.Status = errorString
        context.Response.StatusCode = HttpStatusCode.BadRequest
        context.Response.Write(json)

    End Sub

    Private Sub TransactionsImport(context As HttpContext)
        Dim requestJson As String = ""
        Dim OBJMaster As MasterBAL = New MasterBAL()
        Try
            Dim strLog As String = ""
            steps = "Start03"
            Dim data = [String].Empty
            Using inputStream = New StreamReader(context.Request.InputStream)
                data = inputStream.ReadToEnd()
                requestJson = data
                'data = "{'IdOwner': 'Davide'}"
                log.Info("TransactionsImport : " & data)
                data = "[{'TransactionDateTime':'2018-01-01 00:00','VehicleNumber':'Test 33','PersonPIN':'321','FluidSecureLink':'FSAntinna2','FuelQuantity':'12.2','Odometer':'10','Hours':'1','CompanyName':'vaspsumedh'}]"

                Dim javaScriptSerializer = New JavaScriptSerializer()
                Dim serJsonDetails = javaScriptSerializer.Deserialize(data, GetType(List(Of RootObjectTable)))

                Dim cnt As Integer = 0
                Dim ErrorCnt As Integer = 0
                Dim returnsCnt As String = ""
                Dim currentDateTime As String = ""

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

                For Each item As RootObjectTable In serJsonDetails
                    rowCount = rowCount + 1

                    Dim TransactionDateTime = DirectCast(item, RootObjectTable).TransactionDateTime
                    Dim VehicleNumber = DirectCast(item, RootObjectTable).VehicleNumber
                    Dim PersonPIN = DirectCast(item, RootObjectTable).PersonPIN
                    Dim FluidSecureLink = DirectCast(item, RootObjectTable).FluidSecureLink
                    Dim FuelQuantity = DirectCast(item, RootObjectTable).FuelQuantity
                    Dim CompanyName = DirectCast(item, RootObjectTable).CompanyName
                    Dim Odometer = DirectCast(item, RootObjectTable).Odometer
                    Dim Hours = DirectCast(item, RootObjectTable).Hours

                    isDirty = False


                    rowIndex = rowCount
                    If (CompanyName <> "") Then
                        Dim dtCompany As DataTable = New DataTable()
                        dtCompany = OBJMasterBAL.GetCustomerDetailsByName(CompanyName)
                        If dtCompany IsNot Nothing Then
                            If dtCompany.Rows.Count > 0 Then
                                If (TransactionDateTime <> "") Then
                                    Dim dateval As DateTime
                                    If (DateTime.TryParse(TransactionDateTime, dateval)) Then

                                    Else
                                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Transaction Date and Time field is invalid. Check Row  " & rowIndex & " & column 1 in uploaded file."
                                        isDirty = True
                                    End If

                                Else
                                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Transaction Date and Time field is required. Check Row  " & rowIndex & " & column 1 in uploaded file."
                                    isDirty = True
                                End If

                                If (VehicleNumber <> "") Then

                                    dtVehicle = OBJMaster.GetVehicleByCondition(" and V.VehicleNumber = '" & VehicleNumber & "' and V.CustomerId = " & dtCompany.Rows(0)("CustomerId").ToString, 1331, "52d3b8e6-95e6-44ac-b7bf-0be3acb56ff5")

                                    If Not dtVehicle Is Nothing Then
                                        If dtVehicle.Rows.Count = 0 Then

                                            strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehical Number does not exist. Check Row  " & rowIndex & " & column 2 in uploaded file."
                                            isDirty = True
                                        End If
                                    End If

                                Else
                                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Number field is required. Check Row  " & rowIndex & " & column 2 in uploaded file."
                                    isDirty = True
                                End If

                                If (PersonPIN <> "") Then
                                    dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and LTRIM(RTRIM(ANU.PinNumber))='" & PersonPIN.ToString().Trim() & "' and ANU.CustomerId=" & dtCompany.Rows(0)("CustomerId").ToString, 1331, "52d3b8e6-95e6-44ac-b7bf-0be3acb56ff5")
                                    If Not dtPersonnel Is Nothing Then
                                        If dtPersonnel.Rows.Count > 0 Then
                                            PersonId = dtPersonnel.Rows(0)("PersonId")
                                            'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Personal PIN does not exist. Check Row  " & rowIndex & " & column 3 in uploaded file."
                                            'isDirty = True
                                        End If
                                    End If

                                Else

                                    'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Person PIN field is required. Check Row  " & rowIndex & " & column 3 in uploaded file."
                                    'isDirty = True
                                End If

                                If (FluidSecureLink <> "") Then
                                    dtFSLink = OBJMaster.GetHoseByCondition("And LTRIM(RTRIM(h.WifiSSId)) ='" & FluidSecureLink.ToString().Trim() & "' and h.CustomerID =" & dtCompany.Rows(0)("CustomerId").ToString, 1331, "52d3b8e6-95e6-44ac-b7bf-0be3acb56ff5")
                                    If Not dtFSLink Is Nothing Then
                                        If dtFSLink.Rows.Count = 0 Then
                                            strLog = strLog & Environment.NewLine & currentDateTime & "--" & "FluidSecure Link field does not exist.. Check Row  " & rowIndex & " & column 4 in uploaded file."
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

                                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "FluidSecure Link field is required. Check Row  " & rowIndex & " & column 4 in uploaded file."
                                        isDirty = True
                                    End If
                                Else
                                    Offsite = True
                                End If

                                If (FuelQuantity <> "") Then
                                    Dim Fuelval As Decimal
                                    If Not Decimal.TryParse(FuelQuantity, Fuelval) Then
                                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fuel Quantity value is invalid. Check Row  " & rowIndex & " & column 5 in uploaded file."
                                        isDirty = True
                                    End If
                                Else
                                    'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fuel Quantity field is required. Check Row  " & rowIndex & " & column 5 in uploaded file."
                                    'isDirty = True
                                End If

                                If (Odometer <> "") Then
                                    Dim Odovalue As Integer
                                    If Not Integer.TryParse(Odometer, Odovalue) Then
                                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Odometer value is invalid. Check Row  " & rowIndex & " & column 6 in uploaded file."
                                        isDirty = True
                                    End If
                                Else
                                    'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Odometer field is required. Check Row  " & rowIndex & " & column 6 in uploaded file."
                                    'isDirty = True
                                End If

                                If (Hours <> "") Then
                                    Dim Hoursval As Integer
                                    If Not Integer.TryParse(Hours, Hoursval) Then
                                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Hours value is invalid. Check Row  " & rowIndex & " & column 7 in uploaded file."
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



                                    'Dim result As Integer = OBJMaster.InsertUpdateTransaction(0, dr("FluidSecureLink"), dtPersonnel.Rows(0)("Id").ToString(), 0, dr("FuelQuantity"), )

                                    Dim result As Integer = OBJMaster.InsertUpdateTransaction(dtVehicle.Rows(0)("VehicleId"), SiteId, PersonId, Odometer, Convert.ToDecimal(FuelQuantity), FuelTypeId, 0, FluidSecureLink,
                                                     Convert.ToDateTime(TransactionDateTime), 0, 1331, "W", 0, "", "", "", dtVehicle.Rows(0)("VehicleNumber"), dtVehicle.Rows(0)("DepartmentId"),
                                                     PersonPIN, "", Hours, False, False, 2, HubId, -1,
                                                        VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, Offsite, Convert.ToInt32(dtCompany.Rows(0)("CustomerId").ToString), 0, 0, 0, 0, True)



                                    If (result > 1) Then
                                        cnt = cnt + 1
                                    End If
                                Else
                                    ErrorCnt = ErrorCnt + 1
                                End If
                            Else
                                ErrorInAPI(context, "fail", "Company name not found.")
                                Return
                            End If
                        Else
                            ErrorInAPI(context, "fail", "Company name not found.")
                            Return
                        End If
                    Else
                        ErrorInAPI(context, "fail", "Company name required.")
                    Return
                    End If

                Next

                'Dim TransactionFromDate = DirectCast(serJsonDetails, RootObjectTable).TransactionFromDate
                'Dim TransactionToDate = DirectCast(serJsonDetails, RootObjectTable).TransactionToDate
                'Dim CompanyName = DirectCast(serJsonDetails, RootObjectTable).CompanyName

            End Using
        Catch ex As Exception
            log.Error("Exception occurred while prcessing request in TransactionsImport. Exception is :" & ex.Message & " . Details : " & requestJson)
            ErrorInAPI(context, "fail", "Bad request.")
        End Try
        Return
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class


