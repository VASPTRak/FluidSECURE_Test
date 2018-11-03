Imports System.Data.OleDb
Imports System.IO
Imports log4net.Config
Imports log4net
Imports ClosedXML.Excel

Public Class SpecializedVehicleImport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(SpecializedVehicleImport))
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

    Public Function ImportExceltoDatatable(filepath As String) As DataTable
        Dim dt As DataTable = New DataTable()
        Try
            Using workBook As New XLWorkbook(filepath)
                'Read the first Sheet from Excel file.
                Dim workSheet As IXLWorksheet = workBook.Worksheet(1)
                'Loop through the Worksheet rows.
                Dim firstRow As Boolean = True
                For Each row As IXLRow In workSheet.Rows()
                    'Use the first row to add columns to DataTable.
                    If firstRow Then
                        For Each cell As IXLCell In row.Cells()
                            dt.Columns.Add(cell.Value.ToString())
                        Next
                        firstRow = False
                    Else
                        'Add rows to DataTable.
                        dt.Rows.Add()
                        Dim i As Integer = 0
                        For Each cell As IXLCell In row.Cells()
                            dt.Rows(dt.Rows.Count - 1)(i) = cell.Value.ToString()
                            i += 1
                        Next
                    End If
                Next
            End Using

            Return dt
        Catch ex As Exception
            log.Error("Error occurred in ImportExceltoDatatable Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
            Return dt
        End Try
    End Function

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

            fileExt = System.IO.Path.GetExtension(FU_Vehicles.FileName)

            If (fileExt <> ".xls" And fileExt <> ".xlsx") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Only .xls or .xlsx files allowed to upload!."

                Return
            End If

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Specialized Vehicle Import", "Vehicles", "File Name = " & FU_Vehicles.FileName.Replace(",", " ") & " ; Company =  " & ddlCustomer.SelectedItem.Text.Replace(",", " "), "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If

            'System.Threading.Thread.Sleep("5000")

            Dim folderPath As String = Server.MapPath("~/SpecializedImport/")

            'Check whether Directory (Folder) exists.
            If Not Directory.Exists(folderPath) Then
                'If Directory (Folder) does not exists. Create it.
                Directory.CreateDirectory(folderPath)
            End If

            'Save the File to the Directory (Folder).
            Dim FileName As String = "Vehicle_" & DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss") & FU_Vehicles.FileName
            FU_Vehicles.SaveAs(folderPath & Path.GetFileName(FileName))

            Dim FilePath = folderPath & FileName

            Dim dtImportData As DataTable = ImportExceltoDatatable(FilePath)

            If (dtImportData Is Nothing Or dtImportData.Rows.Count < 1) Then
                LB_Error.Visible = True
                Session("Errorlogs") = strLog
                message.InnerText = "No data found in file."
            End If

            Dim MessageToShow As String = ValidateAndInsertData(dtImportData)

        Catch ex As Exception
            message.InnerText = message.InnerText + " , Error occurred, Please try after some time."
            log.Error("Exception occurred on btnUpload_Click. Exception is : " & ex.Message)
        Finally
            ddlCustomer.Focus()
        End Try

    End Sub

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

            Dim CheckVehicleNumberExist As Boolean = False
            OBJMaster = New MasterBAL()
            Dim dtDept As DataTable = New DataTable()

            dtDept = OBJMaster.GetDeptbyConditions(" and Dept.CustomerId = " & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())

            Dim rowIndex As Integer = 0
            Dim isDirty As Boolean = False
            Dim IsUpdate As Boolean = False
            For Each dr As DataRow In dtImportData.Rows

                CheckVehicleNumberExist = False
                isDirty = False
                IsUpdate = False

                rowIndex = dr("RowIndex")

                If (dr("VehicleNumber") <> "") Then

                    CheckVehicleNumberExist = OBJMaster.CheckVehicleNumberExist(dr("VehicleNumber"), 0, Convert.ToInt32(ddlCustomer.SelectedValue))

                    If CheckVehicleNumberExist = True Then
                        IsUpdate = True
                    ElseIf (dr("VehicleNumber").ToString().Length > 10) Then
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Number (" & dr("VehicleNumber") & ") is must be less than equal to 10 characters. Check Row  " & rowIndex
                        isDirty = True
                    End If
                Else
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Number field is required. Check Row  " & rowIndex
                    isDirty = True
                End If

                If (dr("VehicleName").ToString().Length > 25) Then
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Name (" & dr("VehicleName") & ") Is must be less than equal To 25 characters. Check Row  " & rowIndex
                    isDirty = True
                End If

                If (dr("Extension").ToString().Length > 50) Then
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle description (" & dr("Extension") & ") Is must be less than equal To 50 characters. Check Row  " & rowIndex
                    isDirty = True
                End If

                Dim flagCheckDept As Integer = 0
                Dim DepartmentId As String = dr("DepartmentId")
                Dim strErrorDept As String = ""

                If (dr("DepartmentNo") <> "") Then

                    Dim drDept() As DataRow = dtDept.Select("NUMBER='" & dr("DepartmentNo") & "'")

                    Dim i As Integer

                    dr("DepartmentId") = ""

                    For i = 0 To drDept.GetUpperBound(0)
                        dr("DepartmentId") = drDept(i)("DeptID")
                    Next i

                    If (dr("DepartmentId") = "") Then
                        dr("DepartmentId") = DepartmentId
                        flagCheckDept = 1
                        strErrorDept = currentDateTime & "--" & "Vehicle department number field not found. Check Row  " & rowIndex
                    End If
                Else
                    flagCheckDept = 2
                    strErrorDept = currentDateTime & "--" & "Vehicle department number field is required. Check Row  " & rowIndex
                End If

                If flagCheckDept = 1 Or flagCheckDept = 2 Then
                    If (dr("DepartmentId") <> "") Then
                        Dim drDept() As DataRow = dtDept.Select("NAME='" & dr("DepartmentId") & "'")

                        Dim i As Integer

                        dr("DepartmentId") = ""

                        For i = 0 To drDept.GetUpperBound(0)
                            dr("DepartmentId") = drDept(i)("DeptID")
                        Next i

                        If (dr("DepartmentId") = "") Then
                            strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is not found. Check Row  " & rowIndex
                            isDirty = True
                        Else
                            flagCheckDept = 0
                        End If
                    Else
                        strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is required. Check Row  " & rowIndex
                        isDirty = True
                    End If
                End If

                If (dr("Acc_Id").ToString().Length > 20) Then
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Vehicle Account Id (" & dr("Acc_Id") & ") is must be less than equal to 20 characters. Check Row  " & rowIndex
                    isDirty = True
                End If

                If (dr("LicenseState").ToString().Length > 2) Then
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle License State (" & dr("LicenseState") & ") is must be less than equal to 2 characters. Check Row  " & rowIndex
                    isDirty = True
                End If

                If IsUpdate = False Then
                    If (isDirty = False) Then
                        Dim result As Integer = InsertRecord(dr, True)
                        If (result = 1) Then
                            cnt = cnt + 1
                        End If
                    Else
                        ErrorCnt = ErrorCnt + 1
                    End If
                Else
                    If (isDirty = False) Then
                        dtImportDataToUpdate.Rows.Add(dr)
                    Else
                        ErrorCnt = ErrorCnt + 1
                    End If
                End If
                'rowIndex = rowIndex + 1
            Next

            If dtImportDataToUpdate.Rows.Count > 0 Then
                For Each dr As DataRow In dtImportDataToUpdate.Rows
                    Dim result As Integer = InsertRecord(dr, False)
                    If (result = 1) Then
                        cnt = cnt + 1
                    End If
                Next
            End If

            Return cnt & ";" & ErrorCnt

        Catch ex As Exception
            log.Error("Exception occured while importing file. Exception is : " & ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Exception occurred while importing file. Exception is : " & ex.Message
            Return "0;0"
        End Try
    End Function

    Private Function InsertRecord(dr As DataRow, FlagForInsertUpdate As Boolean) As Integer
        Try
            If FlagForInsertUpdate Then

                OBJMaster = New MasterBAL()
                Dim dtVehicle As DataTable = New DataTable()
                dtVehicle = OBJMaster.GetVehicleByCondition(" and V.VehicleNumber = '" & dr("VehicleNumber") & "' and V.CustomerId = " & ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())

                Dim result As Integer = OBJMaster.SaveUpdateVehicle(Convert.ToInt32(dtVehicle.Rows(0)("VehicleId")), dr("VehicleName"), "", "", "-1", "", "", "", dr("Extension"), "", "-1", dr("DepartmentId"),
                                                            0, 0, "N", "N", "-1", dr("VehicleNumber"), dr("Acc_Id"), "", Convert.ToInt32(Session("PersonId")), ddlCustomer.SelectedValue,
                                                            "-1", "0", 1, dr("LicenseState"), 1, "", dr("Active"), "", 0, 0, "")
            Else
                Dim result As Integer = OBJMaster.SaveUpdateVehicle(0, dr("VehicleName"), "", "", "-1", "", "", "", dr("Extension"), "", "-1", dr("DepartmentId"),
                                                            0, 0, "N", "N", "-1", dr("VehicleNumber"), dr("Acc_Id"), "", Convert.ToInt32(Session("PersonId")), ddlCustomer.SelectedValue,
                                                            "-1", "0", 1, dr("LicenseState"), 1, "", dr("Active"), "", 0, 0, "")

            End If


            Return 1

        Catch ex As Exception
            log.Error("Exception occured while importing file. Exception is : " & ex.Message)

            Return 0
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