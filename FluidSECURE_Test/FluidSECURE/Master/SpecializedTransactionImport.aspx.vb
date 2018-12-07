Imports System.Data.OleDb
Imports System.IO
Imports log4net.Config
Imports log4net
Imports ClosedXML.Excel
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.AspNet.Identity
Public Class SpecializedTransactionImport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(SpecializedTransactionImport))
    Dim OBJMaster As MasterBAL = New MasterBAL()

    Dim strLog As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False
            LB_Error.Visible = False
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
                'Access denied
                Response.Redirect("/home")

            Else

                If Session("RoleName") <> "SuperAdmin" Then
                    If Session("SpecializedTransactionImport") <> "SpecializedTransactionImport" Then
                        'Access denied 
                        Response.Redirect("/home")
                    End If
                End If

                If Not IsPostBack Then
                    GetCustomers(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

                    ddlCustomer.Focus()

                End If
            End If

        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Private Sub GetCustomers(personId As Integer, RoleId As String)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCustomer As DataTable = New DataTable()
            dtCustomer = OBJMaster.GetCustomerDetailsByPersonID(personId, RoleId, Session("CustomerId").ToString())
            ddlCustomer.DataSource = dtCustomer
            ddlCustomer.DataTextField = "CustomerName"
            ddlCustomer.DataValueField = "CustomerId"
            ddlCustomer.DataBind()
            ddlCustomer.Items.Insert(0, New ListItem("Select Company", "0"))

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
                ddlCustomer.SelectedIndex = 1
                ddlCustomer.Enabled = False
                ddlCustomer.Visible = False
                divCompany.Visible = False
            End If

            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                If (Session("RoleName") = "GroupAdmin") Then
                    ddlCustomer.SelectedValue = Session("CustomerId")
                Else
                    ddlCustomer.SelectedIndex = 1
                End If
            End If

        Catch ex As Exception

            log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            Dim fileExt As String

            fileExt = System.IO.Path.GetExtension(FU_Transactions.FileName)

            If (fileExt <> ".xls" And fileExt <> ".xlsx") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Only .xls or .xlsx files allowed to upload!."

                Return
            End If

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Specialized Transaction Import", "Transaction", "File Name = " & FU_Transactions.FileName.Replace(",", " ") & " ; Company =  " & ddlCustomer.SelectedItem.Text.Replace(",", " "), "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If

            'System.Threading.Thread.Sleep("5000")

            Dim folderPath As String = Server.MapPath("~/SpecializedImport/")

            'Check whether Directory (Folder) exists.
            If Not Directory.Exists(folderPath) Then
                'If Directory (Folder) does not exists. Create it.
                Directory.CreateDirectory(folderPath)
            End If

            'Save the File to the Directory (Folder).
            Dim FileName As String = "Transactions_" & DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss") & FU_Transactions.FileName
            FU_Transactions.SaveAs(folderPath & Path.GetFileName(FileName))

            Dim FilePath = folderPath & FileName

            Dim dtImportData As DataTable = ImportExcelUsingOLEDB(FilePath)

            If (dtImportData Is Nothing Or dtImportData.Rows.Count < 1) Then
                LB_Error.Visible = True
                Session("Errorlogs") = strLog
                message.InnerText = "No data found in file."
                Return
            End If

            Dim dtData As DataTable = New DataTable()
            For index = 0 To dtImportData.Columns.Count - 1
                Dim columnName As String = ""
                Select Case dtImportData.Rows(0)(index).ToString().Trim().ToLower()
                    Case "Customer Vehicle ID".ToLower()
                        columnName = "VehicleId"
                    Case "Transaction Date".ToLower()
                        columnName = "TransactionDate"
                    Case "Transaction Time".ToLower()
                        columnName = "TransactionTime"
                    Case "Driver ID".ToLower()
                        columnName = "PinNumber"
                    Case "Vehicle Card Department".ToLower()
                        columnName = "DepartmentName"
                    Case "Unit Cost".ToLower()
                        columnName = "CostPerGallon"
                    Case "Gross Cost".ToLower()
                        columnName = "TransactionCost"
                    Case "Units".ToLower()
                        columnName = "FuelQuantity"

                    Case Else
                        columnName = dtImportData.Rows(0)(index)
                End Select

                dtData.Columns.Add(columnName)
            Next

            dtData.Columns.Add("RowIndex")
            dtData.Columns.Add("TransactionDateTime", System.Type.GetType("System.DateTime"))

            Dim count As Integer = 0

            For Each dr As DataRow In dtImportData.Rows
                If (count <> 0) Then
                    'Dim drinsert As DataRow = dtData.NewRow()
                    dtData.Rows.Add(dr.ItemArray)
                    dtData.Rows(count - 1)("RowIndex") = count

                End If
                count = count + 1

            Next

            For Each dr As DataRow In dtData.Rows
                Dim transactionDatetime As DateTime = dr("TransactionDate") & " " & dr("TransactionTime")
                dr("TransactionDateTime") = transactionDatetime
            Next

            Dim dataView As New DataView(dtData)
            dataView.Sort = " TransactionDateTime ASC"
            dtData = dataView.ToTable()

            Dim returnCnts As String = ValidateAndInsertData(dtData)

            message.Visible = True
            message.InnerText = returnCnts.Split(";")(0) & " Transactions imported successfully "

            If (strLog.Trim() <> "") Then
                LB_Error.Visible = True
                Session("Errorlogs") = strLog
                message.InnerText = message.InnerText + " , " + returnCnts.Split(";")(1) + " caused some errors."
            End If

        Catch ex As Exception
            message.InnerText = message.InnerText + " , Error occurred, Please try after some time."
            log.Error("Exception occurred on btnUpload_Click. Exception is : " & ex.Message)
        Finally
            ddlCustomer.Focus()
        End Try

    End Sub

    Public Function ImportExcelUsingOLEDB(ExcelFilePath As String)
        Dim Sname As String = ""
        Dim connStr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ExcelFilePath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1;TypeGuessRows=0;Importmixedtypes=text';"
        Dim MyConnection As OleDbConnection = New OleDbConnection(connStr)
        Dim MyCommand As OleDbDataAdapter = Nothing

        MyConnection = New OleDbConnection(connStr)

        If (MyConnection.State = ConnectionState.Closed) Then
            MyConnection.Open()
        End If
        'Getting Sheet name from ExcelFile.
        Dim dtSheets As DataTable = MyConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)

        Try
            If (dtSheets.Rows.Count > 0) Then
                Sname = dtSheets.Rows(0)("Table_Name")
            Else
                Sname = "'TransactionDetail'"
            End If
        Catch ex As Exception
            Sname = "'TransactionDetail'"
        End Try

        MyCommand = New OleDbDataAdapter("select * from" + "[" + Sname + "]", MyConnection)
        Dim ds As DataSet = New System.Data.DataSet()
        MyCommand.Fill(ds)
        MyConnection.Close()

        Return ds.Tables(0)
    End Function

    Protected Function ValidateAndInsertData(dtImportData As DataTable) As String
        Dim cnt As Integer = 0
        Dim ErrorCnt As Integer = 0
        Dim returnsCnt As String = ""
        Dim currentDateTime As String = ""
        Dim dtImportDataToUpdate As DataTable = New DataTable()
        dtImportDataToUpdate = dtImportData.Copy()
        dtImportDataToUpdate.Clear()

        Try
            currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

        Catch ex As Exception

            currentDateTime = DateTime.Now.ToString()

        End Try

        Try
            OBJMaster = New MasterBAL()
            Dim dtDept As DataTable = New DataTable()
            Dim dtProduct As DataTable = New DataTable()
            Dim dtVehicle As DataTable = New DataTable()

            dtDept = OBJMaster.GetDeptbyConditions(" and Dept.CustomerId = " & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())
            dtProduct = OBJMaster.GetFuelDetails(ddlCustomer.SelectedValue)


            Dim rowIndex As Integer = 0
            Dim isDirty As Boolean = False

            For Each dr As DataRow In dtImportData.Rows
                isDirty = False
                rowIndex = dr("RowIndex")

                If dr("VehicleId") <> "" Then
                    dtVehicle = New DataTable()
                    dtVehicle = OBJMaster.GetVehicleByCondition(" and V.CustomerId=" & ddlCustomer.SelectedValue & "  And LTRIM(RTRIM(V.VehicleName)) ='" & dr("VehicleId").ToString() & "'", Session("PersonId").ToString(), Session("RoleId").ToString())
                    If (dtVehicle IsNot Nothing) Then
                        If dtVehicle.Rows.Count <= 0 Then
                            strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Customer Vehicle ID (" & dr("VehicleId") & ") is not found. Check Row  " & rowIndex
                            isDirty = True
                        Else
                            dr("VehicleId") = dtVehicle.Rows(0)("VehicleId").ToString()
                        End If
                    Else
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Customer Vehicle ID (" & dr("VehicleId") & ") is not found. Check Row  " & rowIndex
                        isDirty = True
                    End If
                Else
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Customer Vehicle ID field is required. Check Row  " & rowIndex
                    isDirty = True
                End If

                Dim fileDepartmentName As String = dr("DepartmentName")
                Dim DepartmentName As String = ""
                Dim strErrorDept As String = ""
                If (fileDepartmentName = "Unassigned") Then
                    dr("DepartmentName") = "Default"
                End If

                If (dr("DepartmentName") <> "") Then
                    Dim drDept() As DataRow = dtDept.Select("NAME='" & dr("DepartmentName") & "'")

                    Dim i As Integer
                    DepartmentName = dr("DepartmentName")

                    dr("DepartmentName") = ""

                    For i = 0 To drDept.GetUpperBound(0)
                        dr("DepartmentName") = drDept(i)("NUMBER")
                    Next i

                    If (dr("DepartmentName") = "") Then
                        'strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is not found. Check Row  " & rowIndex
                        'isDirty = True
                        OBJMaster = New MasterBAL()

                        Dim result As Integer = OBJMaster.SaveUpdateDept(0, DepartmentName, "", "", "", "", ddlCustomer.SelectedValue, "", Convert.ToInt32(Session("PersonId")), 0, 0, 0, 0, 0, True)
                        If (result > 0) Then
                            dtDept = OBJMaster.GetDeptbyConditions(" and Dept.CustomerId = " & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())
                            dr("DepartmentName") = dtDept.Rows(0)("NUMBER")
                        Else
                            strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & " Error occurred while creating Vehicle department. Check Row  " & rowIndex
                            isDirty = True
                        End If

                    End If
                Else
                    strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is required. Check Row  " & rowIndex
                    isDirty = True
                End If

                If (dr("PinNumber") <> "") Then
                    If (dr("PinNumber").ToString().Length > 20) Then
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Driver ID (" & dr("PinNumber") & ") is must be less than equal to 20 characters. Check Row  " & rowIndex
                        isDirty = True
                    End If
                End If

                If (dr("Product") <> "") Then
                    Dim ProductName As String = ""
                    Select Case dr("Product").ToString().Trim().ToLower()
                        Case "UNL".ToLower()
                            dr("Product") = "UNLEADED"
                        Case "DSL".ToLower()
                            dr("Product") = "DIESEL"
                        Case Else
                            dr("Product") = dr("Product")
                    End Select

                    Dim drProduct() As DataRow = dtProduct.Select("FuelType='" & dr("Product") & "'")

                    Dim i As Integer
                    ProductName = dr("Product")

                    dr("Product") = ""

                    For i = 0 To drProduct.GetUpperBound(0)
                        dr("Product") = drProduct(i)("FuelTypeID")
                    Next i

                    If (dr("Product") = "") Then
                        OBJMaster = New MasterBAL()

                        Dim result As Integer = OBJMaster.SaveUpdateFuel(0, ProductName, "", Convert.ToInt32(Session("PersonId")), ddlCustomer.SelectedValue, 0)
                        If (result > 0) Then
                            dtProduct = OBJMaster.GetFuelDetails(ddlCustomer.SelectedValue)
                            dr("Product") = result
                        Else
                            strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & " Error occurred while creating Product. Check Row  " & rowIndex
                            isDirty = True
                        End If
                    End If
                End If



                If (isDirty = False) Then
                    Dim result As Integer = InsertRecord(dr, True)
                    If (result > 0) Then
                        cnt = cnt + 1
                    End If
                Else
                    ErrorCnt = ErrorCnt + 1
                End If

            Next


            Return cnt & ";" & ErrorCnt

        Catch ex As Exception
            log.Error("Exception occured while importing file. Exception is : " & ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Exception occurred while importing file. Exception is : " & ex.Message
            Return "0;0"
        End Try
    End Function

    Private Function InsertRecord(dr As DataRow, FlagForInsertUpdate As Boolean) As Integer
        Dim resultInt As Integer = 0
        Dim currentDateTime As String = ""
        Try

            ' Transaction date time

            Dim user = New ApplicationUser()
            Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()

            Try
                currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

            Catch ex As Exception

                currentDateTime = DateTime.Now.ToString()

            End Try

            Dim dsTransactionValuesData As DataSet
            Dim DepartmentName As String = ""
            Dim FuelTypeName As String = ""
            Dim Email As String = ""
            Dim PersonName As String = ""
            Dim CompanyName As String = ""
            Dim VehicleName As String = ""
            Dim VehicleNumber As String = ""
            Dim PersonId As Integer = 0

            Dim dtPerson As DataTable = New DataTable()
            dtPerson = OBJMaster.GetPersonalDetails(" where ANU.PinNumber = '" & dr("PinNumber") & "' and ANU.CustomerId = " & ddlCustomer.SelectedValue)
            If dtPerson IsNot Nothing Then
                If dtPerson.Rows.Count > 0 Then
                    PersonId = dtPerson.Rows(0)("PersonId")
                End If
            End If

            Dim dtVehicle As DataTable = New DataTable()
            dtVehicle = OBJMaster.GetVehiclebyId(dr("VehicleId"))
            If (dtVehicle IsNot Nothing) Then
                If dtVehicle.Rows.Count > 0 Then
                    VehicleNumber = dtVehicle.Rows(0)("VehicleNumber")
                End If
            End If

            dsTransactionValuesData = OBJMaster.GetTransactionColumnsValueForSave(dr("DepartmentName"), Convert.ToInt32(dr("Product")), PersonId, Convert.ToInt32(dr("VehicleId")))

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
                        CompanyName = ddlCustomer.SelectedItem.Text
                    End If
                    If dsTransactionValuesData.Tables(4) IsNot Nothing And dsTransactionValuesData.Tables(4).Rows.Count > 0 Then
                        VehicleName = dsTransactionValuesData.Tables(4).Rows(0)("VehicleName").ToString()
                    End If
                End If
            End If

            Dim transactionDatetime As DateTime = dr("TransactionDate") & " " & dr("TransactionTime")

            resultInt = OBJMaster.InsertUpdateTransaction(Convert.ToInt32(dr("VehicleId")), 0, PersonId, IIf(dr("Odometer").ToString() = "", "0", dr("Odometer").ToString()), dr("FuelQuantity"), dr("Product"), 0, Nothing,
                                                     transactionDatetime, 0, Convert.ToInt32(Session("PersonId")), "W", 0, "", "", "", VehicleNumber, dr("DepartmentName"),
                                                     dr("PinNumber"), "", -1, False, False, 2, 0, -1,
                                                     VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, True, Convert.ToInt32(ddlCustomer.SelectedValue), -1, 0, 0, 1, False)

            If dr("TransactionCost") <> "" Then
                OBJMaster.UpdateTransactionCost(resultInt, dr("TransactionCost"))
            End If

            '"CostPerGallon"

            Return resultInt

        Catch ex As Exception
            log.Error("Exception occured while importing file. Exception is : " & ex.Message)

            strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Error occured while importing row " & dr("RowIndex").ToString() & ". Error is " & ex.Message

            Return resultInt

        End Try

    End Function

    Protected Sub LB_Error_Click(sender As Object, e As EventArgs)

        Dim ms As New MemoryStream()
        Dim tw As TextWriter = New StreamWriter(ms)
        tw.WriteLine(Session("Errorlogs"))
        tw.Flush()
        Dim bytes As Byte() = ms.ToArray()
        ms.Close()
        Response.Clear()
        Response.ContentType = "application/force-download"
        Response.AddHeader("content-disposition", "attachment;filename=Errorlogs.txt")
        Response.BinaryWrite(bytes)
        Response.[End]()

    End Sub

End Class