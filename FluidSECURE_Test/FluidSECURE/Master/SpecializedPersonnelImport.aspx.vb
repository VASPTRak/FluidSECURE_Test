Imports System.Data.OleDb
Imports System.IO
Imports log4net.Config
Imports log4net
Imports ClosedXML.Excel
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.AspNet.Identity

Public Class SpecializedPersonnelImport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(SpecializedPersonnelImport))
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
                    If Session("SpecializedPersonnelImport") <> "SpecializedPersonnelImport" Then
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

    'Public Function ImportExceltoDatatable(filepath As String) As DataTable
    '    Dim dt As DataTable = New DataTable()
    '    Try
    '        Using workBook As New XLWorkbook(filepath)
    '            'Read the first Sheet from Excel file.
    '            Dim workSheet As IXLWorksheet = workBook.Worksheet(1)
    '            'Loop through the Worksheet rows.
    '            Dim firstRow As Boolean = True
    '            For Each row As IXLRow In workSheet.Rows()
    '                'Use the first row to add columns to DataTable.
    '                If firstRow Then
    '                    For Each cell As IXLCell In row.Cells()
    '                        dt.Columns.Add(cell.Value.ToString())
    '                    Next
    '                    firstRow = False
    '                Else
    '                    'Add rows to DataTable.
    '                    dt.Rows.Add()
    '                    Dim i As Integer = 0
    '                    For Each cell As IXLCell In row.Cells()
    '                        dt.Rows(dt.Rows.Count - 1)(i) = cell.Value.ToString()
    '                        i += 1
    '                    Next
    '                End If
    '            Next
    '        End Using

    '        Return dt
    '    Catch ex As Exception
    '        log.Error("Error occurred in ImportExceltoDatatable Exception is :" + ex.Message)
    '        ErrorMessage.Visible = True
    '        ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
    '        Return dt
    '    End Try
    'End Function

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

            fileExt = System.IO.Path.GetExtension(FU_Persons.FileName)

            If (fileExt <> ".xls" And fileExt <> ".xlsx") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Only .xls or .xlsx files allowed to upload!."

                Return
            End If

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Specialized Persons Import", "Persons", "File Name = " & FU_Persons.FileName.Replace(",", " ") & " ; Company =  " & ddlCustomer.SelectedItem.Text.Replace(",", " "), "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If

            'System.Threading.Thread.Sleep("5000")

            Dim folderPath As String = Server.MapPath("~/SpecializedImport/")

            'Check whether Directory (Folder) exists.
            If Not Directory.Exists(folderPath) Then
                'If Directory (Folder) does not exists. Create it.
                Directory.CreateDirectory(folderPath)
            End If

            'Save the File to the Directory (Folder).
            Dim FileName As String = "Persons_" & DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss") & FU_Persons.FileName
            FU_Persons.SaveAs(folderPath & Path.GetFileName(FileName))

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
                Select Case dtImportData.Rows(0)(index).ToString().Trim()
                    Case "Driver ID"
                        columnName = "PinNumber"
                    Case "Last Name"
                        columnName = "LastName"
                    Case "First Name"
                        columnName = "FirstName"
                    Case "Department"
                        columnName = "DepartmentId"
                    Case "Status"
                        columnName = "Active"
                    Case Else
                        columnName = dtImportData.Rows(0)(index)
                End Select

                dtData.Columns.Add(columnName)
            Next


            dtData.Columns.Add("RowIndex")

            Dim count As Integer = 0

            For Each dr As DataRow In dtImportData.Rows
                If (count <> 0) Then
                    'Dim drinsert As DataRow = dtData.NewRow()
                    dtData.Rows.Add(dr.ItemArray)
                    dtData.Rows(count - 1)("RowIndex") = count
                End If
                count = count + 1

            Next

            Dim returnCnts As String = ValidateAndInsertData(dtData)

            message.Visible = True
            message.InnerText = returnCnts.Split(";")(0) & " persons imported successfully "

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
                Sname = "'ACTIVE DRiver PINS$'"
            End If
        Catch ex As Exception
            Sname = "'ACTIVE DRiver PINS$'"
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

        Try
            currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

        Catch ex As Exception

            currentDateTime = DateTime.Now.ToString()

        End Try

        Try
            Dim CheckPinNumberExists As Boolean = False
            OBJMaster = New MasterBAL()
            Dim dtDept As DataTable = New DataTable()

            dtDept = OBJMaster.GetDeptbyConditions(" and Dept.CustomerId = " & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())

            Dim rowIndex As Integer = 0
            Dim isDirty As Boolean = False
            For Each dr As DataRow In dtImportData.Rows

                CheckPinNumberExists = False
                isDirty = False
                Dim IsUpdate As Boolean = False
                rowIndex = dr("RowIndex")

                If (dr("PinNumber") <> "") Then
                    If (dr("PinNumber").ToString().Length > 20) Then
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Personnel ID (" & dr("PinNumber") & ") is must be less than equal to 20 characters. Check Row  " & rowIndex
                        isDirty = True
                    Else
                        OBJMaster = New MasterBAL()

                        CheckPinNumberExists = OBJMaster.CheckPinNumberExist(dr("PinNumber"), 0, ddlCustomer.SelectedValue)

                        If CheckPinNumberExists = True Then
                            IsUpdate = True
                        End If

                    End If
                Else
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Driver ID (" & dr("PinNumber") & ") is must be in number format. Check Row  " & rowIndex
                    isDirty = True
                End If

                Dim fileDepartmentName As String = dr("DepartmentId")
                Dim DepartmentName As String = ""
                Dim strErrorDept As String = ""
                If (fileDepartmentName = "Unassigned") Then
                    dr("DepartmentId") = "Default"
                End If

                If (dr("DepartmentId") <> "") Then
                    Dim drDept() As DataRow = dtDept.Select("NAME='" & dr("DepartmentId") & "'")

                    Dim i As Integer
                    DepartmentName = dr("DepartmentId")

                    dr("DepartmentId") = ""

                    For i = 0 To drDept.GetUpperBound(0)
                        dr("DepartmentId") = drDept(i)("DeptID")
                    Next i

                    If (dr("DepartmentId") = "") Then
                        'strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is not found. Check Row  " & rowIndex
                        'isDirty = True
                        OBJMaster = New MasterBAL()

                        Dim result As Integer = OBJMaster.SaveUpdateDept(0, DepartmentName, "", "", "", "", ddlCustomer.SelectedValue, "", Convert.ToInt32(Session("PersonId")), 0, 0, 0, 0, 0, True)
                        If (result > 0) Then
                            dtDept = OBJMaster.GetDeptbyConditions(" and Dept.CustomerId = " & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())
                            dr("DepartmentId") = result
                        Else
                            strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & " Error occurred while creating Vehicle department. Check Row  " & rowIndex
                            isDirty = True
                        End If

                    End If
                Else
                    strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is required. Check Row  " & rowIndex
                    isDirty = True
                End If


                If (dr("Active").ToString() = "Active") Then
                    dr("Active") = "Y"
                Else
                    dr("Active") = "N"
                End If


                If IsUpdate = False Then
                    If (isDirty = False) Then
                        Dim result As Integer = InsertRecord(dr, True)
                        If (result > 0) Then
                            cnt = cnt + 1
                        End If
                    Else
                        ErrorCnt = ErrorCnt + 1
                    End If
                Else
                    If (isDirty = False) Then
                        Dim result As Integer = InsertRecord(dr, False)
                        If (result > 0) Then
                            cnt = cnt + 1
                        End If
                    Else
                        ErrorCnt = ErrorCnt + 1
                    End If
                End If

                'rowIndex = rowIndex + 1
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
            Dim user = New ApplicationUser()
            Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()

            Dim ApprOn As DateTime
            Dim ApprBy As Integer

            Try
                currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

            Catch ex As Exception

                currentDateTime = DateTime.Now.ToString()

            End Try

            If (dr("Active").ToString().ToUpper() = "Y") Then

                ApprOn = DateTime.Now
                ApprBy = Convert.ToInt32(Session("PersonId"))

            End If

            Dim EmailUserName As String = ""
            Dim PhoneNumber As String = ""


            OBJMaster = New MasterBAL()
            Dim HubPersonNumber As Integer = OBJMaster.GetAndUpdateLastHubPersonNumberEntry()

            EmailUserName = "u" & HubPersonNumber & "@FluidSecureHub.com"
            PhoneNumber = ""
            If FlagForInsertUpdate Then
                user = New ApplicationUser() With {
            .UserName = EmailUserName,
            .Email = EmailUserName,
            .PersonName = (dr("LastName") & " " & dr("FirstName")).ToString().Trim(),
            .DepartmentId = dr("DepartmentId"),
            .PhoneNumber = PhoneNumber,
            .SoftUpdate = "N",
            .CreatedDate = DateTime.Now,
            .CreatedBy = Convert.ToInt32(Session("PersonId")),
            .IsDeleted = False,
            .RoleId = "11df27ed-8d70-46a9-a925-7150326ffe75",
            .IsApproved = IIf(dr("Active").ToString().ToUpper() = "Y", True, False),
            .ApprovedBy = ApprBy,
            .ApprovedOn = ApprOn,
            .ExportCode = "",
            .IMEI_UDID = "",
            .CustomerId = Convert.ToInt32(ddlCustomer.SelectedValue),
            .SendTransactionEmail = False,
            .RequestFrom = "W",
            .FuelLimitPerTxn = Nothing,
            .FuelLimitPerDay = Nothing,
            .PreAuth = Nothing,
            .PinNumber = dr("PinNumber"),
            .IsFluidSecureHub = False,
            .IsUserForHub = 1,
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
            .IsLogging = 0,
            .IsSpecialImport = 1
       }
                Dim result As IdentityResult

                result = manager.Create(user, "FluidSecure*123")

                If result.Succeeded Then
                    resultInt = 1
                End If

            Else
                Dim dtPersonnel As DataTable = New DataTable()
                dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and LTRIM(RTRIM(ANU.PinNumber))='" & dr("PinNumber").ToString().Trim() & "' and ANU.CustomerId = " & ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

                If (dtPersonnel.Rows.Count > 0) Then
                    user = manager.FindById(dtPersonnel.Rows(0)("Id").ToString())
                    user.PersonName = (dr("LastName") & " " & dr("FirstName")).ToString().Trim()
                    user.DepartmentId = dr("DepartmentId")
                    user.IsApproved = IIf(dr("Active").ToString().ToUpper() = "Y", True, False)
                    user.LastModifiedDate = DateTime.Now
                    user.LastModifiedBy = Convert.ToInt32(Session("PersonId"))

                    Dim result As IdentityResult
                    result = manager.Update(user)
                    If result.Succeeded Then
                        resultInt = 1
                    End If
                End If
            End If

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