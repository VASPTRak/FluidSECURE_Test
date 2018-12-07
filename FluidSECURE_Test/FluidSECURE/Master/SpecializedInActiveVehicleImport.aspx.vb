Imports System.Data.OleDb
Imports System.IO
Imports log4net
Imports log4net.Config

Public Class SpecializedInActiveVehicleImport
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(SpecializedInActiveVehicleImport))
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
                    If Session("SpecializedVehicleInactiveImport") <> "SpecializedVehicleInactiveImport" Then
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
            Dim FileName As String = "InActiveVehicles_" & DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss") & FU_Persons.FileName
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
                    Case "Vehicle ID"
                        columnName = "VehicleName"
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

            Dim returnCnts As String = ValidateAndUpdateData(dtData)

            message.Visible = True
            message.InnerText = returnCnts.Split(";")(0) & " vehicles deactivated successfully"

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
                Sname = "'cards$'"
            End If
        Catch ex As Exception
            Sname = "'card$'"
        End Try

        MyCommand = New OleDbDataAdapter("select * from" + "[" + Sname + "]", MyConnection)
        Dim ds As DataSet = New System.Data.DataSet()
        MyCommand.Fill(ds)
        MyConnection.Close()

        Return ds.Tables(0)
    End Function


    Protected Function ValidateAndUpdateData(dtImportData As DataTable) As String
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

            OBJMaster = New MasterBAL()
            Dim dtVehicle As DataTable = New DataTable()
            Dim vehicleId As Integer = 0
            Dim rowIndex As Integer = 0
            Dim isDirty As Boolean = False

            For Each dr As DataRow In dtImportData.Rows
                Try

                    isDirty = False

                    rowIndex = dr("RowIndex")

                    If (dr("VehicleName") <> "") Then

                        dtVehicle = OBJMaster.GetVehicleByCondition(" and V.CustomerId=" & ddlCustomer.SelectedValue & "  And LTRIM(RTRIM(V.VehicleName)) ='" & dr("VehicleName").ToString() & "'", Session("PersonId").ToString(), Session("RoleId").ToString())
                        If Not dtVehicle Is Nothing Then
                            If dtVehicle.Rows.Count <> 0 Then
                                vehicleId = dtVehicle.Rows(0)("VehicleId").ToString()
                            Else
                                strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle (" & dr("VehicleName") & ") not found. Check Row  " & rowIndex
                                isDirty = True
                            End If
                        Else
                            strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle (" & dr("VehicleName") & ") not found. Check Row  " & rowIndex
                            isDirty = True
                        End If
                    Else
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Id field is required. Check Row  " & rowIndex
                        isDirty = True
                    End If


                    If (isDirty = False) Then
                        Dim result As Integer = UpdateRecord(vehicleId, currentDateTime, dr("RowIndex").ToString())
                        If (result > 0) Then
                            cnt = cnt + 1
                        End If
                    Else
                        ErrorCnt = ErrorCnt + 1
                    End If



                Catch ex As Exception
                    log.Error("Exception occured while importing inactive Vehicle file at row " & dr("RowIndex") & " . Exception is : " & ex.Message)
                End Try

            Next

            Return cnt & ";" & ErrorCnt

        Catch ex As Exception
            log.Error("Exception occured while importing file. Exception is : " & ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Exception occurred while importing file. Exception is : " & ex.Message
            Return "0;0"
        End Try
    End Function

    Private Function UpdateRecord(VehicleId As Integer, currentDateTime As String, RowIndex As Integer) As Integer
        Try
            Dim result As Integer = 0

            OBJMaster = New MasterBAL()
            result = OBJMaster.UpdateVehicleInActiveFlag(VehicleId, Convert.ToInt32(Session("PersonId")), False)

            Return result

        Catch ex As Exception
            log.Error("Exception occured while importing file. Exception is : " & ex.Message)
            strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Error occured while importing row " & RowIndex & ". Error is " & ex.Message
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