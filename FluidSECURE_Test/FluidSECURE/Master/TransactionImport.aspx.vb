Imports System.IO
Imports log4net.Config
Imports log4net
Public Class TransactionImport
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(VehicleImport))
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

            If (fileExt <> ".csv" And fileExt <> ".txt") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Only .csv or .txt files allowed to upload!."

                Return
            End If

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Import", "Transactions", "File Name = " & FU_Transactions.FileName.Replace(",", " ") & " ; Company =  " & ddlCustomer.SelectedItem.Text.Replace(",", " "), "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
            End If

            System.Threading.Thread.Sleep("5000")

            Dim allContent As String = New StreamReader(FU_Transactions.FileContent, Encoding.GetEncoding("iso-8859-1")).ReadToEnd()
            allContent = allContent.Replace("""", "")

            Dim returnCnts As String = ConvertStringIntoDatTableAndInsertData(allContent)



            message.Visible = True
            message.InnerText = returnCnts.Split(";")(0) & " transactions imported successfully "

            If (strLog.Trim() <> "") Then
                LB_Error.Visible = True
                Session("Errorlogs") = strLog
                message.InnerText = message.InnerText + " , " + returnCnts.Split(";")(1) + " caused some errors."
            End If

        Catch ex As Exception
            message.InnerText = message.InnerText + " , Error occurred, Please try after some time."
            message.Visible = True
            log.Error("Exception occurred on btnUpload_Click. Exception is : " & ex.Message)
        Finally
            ddlCustomer.Focus()
        End Try

    End Sub
    Protected Function ConvertStringIntoDatTableAndInsertData(data As String) As String
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
            Dim Lines As String() = data.Split(New Char() {ControlChars.Lf})
            Dim Fields As String() = New String() {}
            Fields = Lines(0).Split(New Char() {","c})
            Dim Cols As Integer = Fields.GetLength(0)
            Dim dt As New DataTable()
            dt.TableName = "TransactionDetails"
            dt.Columns.Add("DateTime", GetType(String))
            dt.Columns.Add("VehicleNumber", GetType(String))
            dt.Columns.Add("PersonPIN", GetType(String))
            dt.Columns.Add("FluidSecureLink", GetType(String))
            dt.Columns.Add("FuelQuantity", GetType(String))
            dt.Columns.Add("Odometer", GetType(String))
            dt.Columns.Add("Hours", GetType(String))
            dt.Columns.Add("RowIndex", GetType(Integer))

            Dim Row As DataRow
            For i As Integer = 4 To Lines.GetLength(0) - 1
                Try
                    Fields = Lines(i).Split(New Char() {","c})
                    If (Fields.Length > 7) Then
                        Fields = Fields.Take(7).ToArray()
                        Cols = Fields.Length
                    End If

                    If (Fields.Length = dt.Columns.Count - 1) Then
                        Row = dt.NewRow()
                        For f As Integer = 0 To Cols - 1
                            Row(f) = Fields(f).ToString().Replace("'", "").Trim()
                        Next
                        Row(7) = i + 1
                        dt.Rows.Add(Row)
                    ElseIf (Fields.Length < 7 And Fields.Length > 1) Then

                        Row = dt.NewRow()
                        For f As Integer = 0 To Fields.Length - 1
                            Row(f) = Fields(f).ToString().Replace("'", "").Trim()
                        Next

                        For f As Integer = Fields.Length To 7
                            Row(f) = ""
                        Next

                        Row(7) = i + 1
                        dt.Rows.Add(Row)
                    ElseIf (Fields.Length > 3) Then
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Invalid input format. Incorrect number of columns for the row number " & (i + 1) & ". Please correct the data and retry!"
                        ErrorCnt = ErrorCnt + 1
                    End If
                Catch ex As Exception
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Invalid input format. Incorrect number of columns for the row number " & (i + 1) & ". Please correct the data and retry!"
                    ErrorCnt = ErrorCnt + 1
                    Continue For
                End Try
            Next

            Dim rowIndex As Integer = 0
            Dim isDirty As Boolean = False
            Dim dtVehicle As DataTable = New DataTable()
            Dim dtPersonnel As DataTable = New DataTable()
            Dim dtFSLink As DataTable = New DataTable()
            Dim dtHub As DataTable = New DataTable()
            Dim HubId As Integer = 0
            Dim PersonId As Integer = 0
            Dim Offsite As Boolean = False

            For Each dr As DataRow In dt.Rows
                isDirty = False


                rowIndex = dr("RowIndex")

                If (dr("DateTime") <> "") Then
                    Dim dateval As DateTime
                    If (DateTime.TryParse(dr("DateTime"), dateval)) Then

                    Else
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Transaction Date and Time field is invalid. Check Row  " & rowIndex & " & column 1 in uploaded file."
                        isDirty = True
                    End If

                Else
                    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Transaction Date and Time field is required. Check Row  " & rowIndex & " & column 1 in uploaded file."
                    isDirty = True
                End If

                If (dr("VehicleNumber") <> "") Then

                    dtVehicle = OBJMaster.GetVehicleByCondition(" and V.VehicleNumber = '" & dr("VehicleNumber") & "' and V.CustomerId = " & ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())

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

                If (dr("PersonPIN") <> "") Then
                    dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and LTRIM(RTRIM(ANU.PinNumber))='" & dr("PersonPIN").ToString().Trim() & "' and ANU.CustomerId=" & ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
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

                If (dr("FluidSecureLink") <> "") Then
                    dtFSLink = OBJMaster.GetHoseByCondition("And LTRIM(RTRIM(h.WifiSSId)) ='" & dr("FluidSecureLink").ToString().Trim() & "' and h.CustomerID =" & ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())
                    If Not dtFSLink Is Nothing Then
                        If dtFSLink.Rows.Count = 0 Then
                            strLog = strLog & Environment.NewLine & currentDateTime & "--" & "FluidSecure Link field does not exist.. Check Row  " & rowIndex & " & column 4 in uploaded file."
                            isDirty = True
                        Else
                            dtHub = OBJMaster.GetPersonSiteMappingBySiteId(dtFSLink.Rows(0)("SiteId"), ddlCustomer.SelectedValue)
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

                If (dr("FuelQuantity") <> "") Then
                    Dim Fuelval As Decimal
                    If Not Decimal.TryParse(dr("FuelQuantity"), Fuelval) Then
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fuel Quantity value is invalid. Check Row  " & rowIndex & " & column 5 in uploaded file."
                        isDirty = True
                    End If
                Else
                    'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fuel Quantity field is required. Check Row  " & rowIndex & " & column 5 in uploaded file."
                    'isDirty = True
                End If

                If (dr("Odometer") <> "") Then
                    Dim Odovalue As Integer
                    If Not Integer.TryParse(dr("Odometer"), Odovalue) Then
                        strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Odometer value is invalid. Check Row  " & rowIndex & " & column 6 in uploaded file."
                        isDirty = True
                    End If
                Else
                    'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Odometer field is required. Check Row  " & rowIndex & " & column 6 in uploaded file."
                    'isDirty = True
                End If

                If (dr("Hours") <> "") Then
                    Dim Hoursval As Integer
                    If Not Integer.TryParse(dr("Hours"), Hoursval) Then
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
                    Dim result As Integer = InsertRecord(dr, dtVehicle, dtPersonnel, Offsite, PersonId, HubId, SiteId, FuelTypeId) 'listOfSites
                    If (result = 1) Then
                        cnt = cnt + 1
                    End If
                Else
                    ErrorCnt = ErrorCnt + 1
                End If

                rowIndex = rowIndex + 1
            Next

            Return cnt & ";" & ErrorCnt
        Catch ex As Exception
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Exception occurred while importing file. Exception is : " & ex.Message
            Return "0;0"
        End Try
    End Function
    Private Function InsertRecord(dr As DataRow, dtVehicle As DataTable, dtPersonnel As DataTable, Offsite As Boolean, PersonId As Integer, HubId As Integer, SiteId As Integer, FuelTypeId As Integer) As Integer ', listOfSites As List(Of Integer)
        Try
            Dim dsTransactionValuesData As DataSet
            Dim DepartmentName As String = ""
            Dim FuelTypeName As String = ""
            Dim Email As String = ""
            Dim PersonName As String = ""
            Dim CompanyName As String = ""
            Dim VehicleName As String = ""
            Dim Odometer As Integer = 0
            Dim Hours As Integer = 0

            If (dr("Odometer")) <> "" Then
                Odometer = dr("Odometer")
            End If
            If (dr("Hours")) <> "" Then
                Hours = dr("Hours")
            End If
            'If Not hdf_PersonId.Value = "" Then
            '    PersonId = Convert.ToInt32(hdf_PersonId.Value)
            'End If

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
                        CompanyName = ddlCustomer.SelectedItem.Text
                    End If
                    If dsTransactionValuesData.Tables(4) IsNot Nothing And dsTransactionValuesData.Tables(4).Rows.Count > 0 Then
                        VehicleName = dsTransactionValuesData.Tables(4).Rows(0)("VehicleName").ToString()
                    End If
                End If
            End If



            'Dim result As Integer = OBJMaster.InsertUpdateTransaction(0, dr("FluidSecureLink"), dtPersonnel.Rows(0)("Id").ToString(), 0, dr("FuelQuantity"), )

            Dim result As Integer = OBJMaster.InsertUpdateTransaction(dtVehicle.Rows(0)("VehicleId"), SiteId, PersonId, Odometer, Convert.ToDecimal(dr("FuelQuantity")), FuelTypeId, 0, dr("FluidSecureLink"),
                                                     Convert.ToDateTime(dr("DateTime")), 0, Convert.ToInt32(Session("PersonId")), "W", 0, "", "", "", dtVehicle.Rows(0)("VehicleNumber"), dtVehicle.Rows(0)("DepartmentId"),
                                                     dr("PersonPIN"), "", Hours, False, False, 2, HubId, -1,
                                                        VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, Offsite, Convert.ToInt32(ddlCustomer.SelectedValue), 0, 0, 0, 0, True)

            Return 1

        Catch ex As Exception
            log.Error("Exception occured while importing file. Exception is : " & ex.Message)

            Return 0
        End Try

    End Function
    Protected Sub lnkTemplate_Click(sender As Object, e As EventArgs)

        Dim path As String = Server.MapPath("\Content\Templates\TransactionImportTemplate.csv")
        Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)
        If file.Exists Then 'set appropriate headers
            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.ContentType = "application/octet-stream"
            Response.WriteFile(file.FullName)
            Response.End()
        Else
            Response.Write("This file does not exist.")
        End If

    End Sub
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