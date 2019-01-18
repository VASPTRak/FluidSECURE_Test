Imports System.IO
Imports log4net.Config
Imports log4net
Public Class TLDFirmwareUpgrades
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TLDFirmwareUpgrades))
    Dim OBJMaster As MasterBAL = New MasterBAL()
    Shared beforeData As String
    Shared beforeTanks As String
    Shared afterTanks As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then

                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") <> "SuperAdmin" Then
                'Access denied
                Response.Redirect("/home")

            Else

                If Not IsPostBack Then
                    If (Not Request.QueryString("TLDFirmwareId") = Nothing And Not Request.QueryString("TLDFirmwareId") = "") Then

                        hdfTLDFirmwareID.Value = Request.QueryString("TLDFirmwareId")
                        lblHeader.Text = "Edit TLD Firmware"
                        Dim dtTLDFirmware As DataTable = New DataTable()
                        OBJMaster = New MasterBAL()
                        dtTLDFirmware = OBJMaster.GetTLDFirmwaresByCondition(" and TLDFirmwareId = " + hdfTLDFirmwareID.Value, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                        ViewDiv.Visible = True
                        Uploaddiv.Visible = False
                        If dtTLDFirmware IsNot Nothing And dtTLDFirmware.Rows.Count > 0 Then
                            lblTLDFirmwareName.Text = dtTLDFirmware.Rows(0)("Version").ToString()
                            lblUploadfirware.Text = dtTLDFirmware.Rows(0)("TLDFirmwareFileName").ToString()
                        End If
                        BindCompanyAndTank(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
                        BindCheckBoxTanksValues()
                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"
                        End If
                    Else
                        ViewDiv.Visible = False
                        Uploaddiv.Visible = True
                        txtTLDfirmwareversionnumber.Focus()
                        lblHeader.Text = "Add TLD Firmware"
                        BindCompanyAndTank(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
                    End If

                End If
            End If


        Catch ex As Exception

            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)

        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            If hdfTLDFirmwareID.Value = "" Or hdfTLDFirmwareID.Value = "0" Then
                Dim TLDFirmwareId As Integer = 0

                'If (Not HDF_ShipmentId.Value = Nothing And Not HDF_ShipmentId.Value = "") Then

                '    ShipmentId = HDF_ShipmentId.Value

                'End If
                Dim CheckVersionExists As Integer = 0
                OBJMaster = New MasterBAL()
                CheckVersionExists = OBJMaster.CheckTLDVersionIsExist(txtTLDfirmwareversionnumber.Text)

                If CheckVersionExists = -1 Then

                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Version " + txtTLDfirmwareversionnumber.Text + " already exist."

                    Return

                End If

                Dim fileExt As String

                fileExt = System.IO.Path.GetExtension(FU_Firware.FileName)

                If (fileExt <> ".bin") Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Only .bin files allowed to upload!."

                    Return
                End If

                Dim TLDFirmwareFileName As String

                Dim TLDFirmwareFilePath As String

                Dim folderPath As String = Server.MapPath("~/TLDFirmwares/" & txtTLDfirmwareversionnumber.Text & "/")

                'Check whether Directory (Folder) exists.
                If Not Directory.Exists(folderPath) Then
                    'If Directory (Folder) does not exists. Create it.
                    Directory.CreateDirectory(folderPath)
                Else
                    Dim newFolderPath As String = Server.MapPath("~/TLDFirmwares/" & txtTLDfirmwareversionnumber.Text & "_old_" & DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss"))
                    'Directory.CreateDirectory(newFolderPath)
                    Directory.Move(folderPath, newFolderPath)
                    Directory.CreateDirectory(folderPath)
                End If

                'Save the File to the Directory (Folder).
                FU_Firware.SaveAs(folderPath & Path.GetFileName(FU_Firware.FileName))

                TLDFirmwareFileName = Path.GetFileName(FU_Firware.FileName)
                TLDFirmwareFilePath = "/TLDFirmwares/" & txtTLDfirmwareversionnumber.Text & "/" & TLDFirmwareFileName


                OBJMaster = New MasterBAL()

                Dim result As Integer = 0

                OBJMaster = New MasterBAL()

                'Dim shipmentDatetime As DateTime = Request.Form(txtShipmentDate.UniqueID) & " " & Request.Form(txtShipmentTime.UniqueID)

                result = OBJMaster.SaveUpdateTLDFirmware(TLDFirmwareId, TLDFirmwareFileName, TLDFirmwareFilePath, txtTLDfirmwareversionnumber.Text, Convert.ToInt32(Session("PersonId")))

                If result > 0 Then
                    'Save TLDFirmwareupgrade Mapping with FluidTanks
                    SaveTLDFirmwareupgradeMappingwithFluidTanks(result)
                    message.Visible = True
                    message.InnerText = "TLD Firmware uploaded successfully"
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Added", "TLD Firmware Upgrades", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    Response.Redirect(String.Format("~/Master/TLDFirmwareUpgrades?TLDFirmwareId={0}&RecordIs=New", result))
                Else
                    If (TLDFirmwareId > 0) Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "TLD Firmware uploading failed, please try again"
                    Else
                        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                            CSCommonHelper.WriteLog("Added", "TLD Firmware Upgrades", "", "Upload firware Name = " & FU_Firware.FileName.Replace(",", " ") & " ; TLD Firmware version number =  " & txtTLDfirmwareversionnumber.Text.Replace(",", " "), HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "TLD Firmware uploading failed.")
                        End If
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "TLD Firmware uploading failed, please try again"
                    End If

                End If
            Else
                Dim result = OBJMaster.SaveUpdateTLDFirmware(Convert.ToInt32(hdfTLDFirmwareID.Value), "", "", "", Convert.ToInt32(Session("PersonId")))
                If result > 0 Then
                    'Save TLDFirmwareupgrade Mapping with FluidTanks
                    SaveTLDFirmwareupgradeMappingwithFluidTanks(result)
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Modified", "TLD Firmware Upgrade", beforeData, writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    Response.Redirect(String.Format("~/Master/TLDFirmwareUpgrades?TLDFirmwareId={0}&RecordIs=New", result))
                Else

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Modified", "TLD Firmware Upgrade", beforeData, writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "TLD Firmware uploading failed, please try again"
                End If
            End If

        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Added", "TLD Firmware Upgrades", "", "Upload firware Name = " & FU_Firware.FileName.Replace(",", " ") & " ; TLD Firmware version number =  " & txtTLDfirmwareversionnumber.Text.Replace(",", " "), HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "TLD Firmware uploading failed. Exception is : " & ex.Message)
            End If
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
        Finally

        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllTLDFirmwareUpgrade?Filter=Filter")
    End Sub

    Private Sub BindCompanyAndTank(PersonId As Integer, RoleId As String, flag As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()

            dtCust = OBJMaster.GetCustomerDetailsByPersonID(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())
            If dtCust IsNot Nothing And dtCust.Rows.Count > 0 Then
                gvCustomers.DataSource = dtCust
                gvCustomers.DataBind()
            End If
        Catch ex As Exception

            log.Error("Error occurred in BindCompanyAndTank Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindCheckBoxTanksValues()
        Try
            Dim dtTLDFirmwareFieldTank As DataTable = New DataTable()
            dtTLDFirmwareFieldTank = OBJMaster.GetTLDFirmxareFluidTankMappingByTLDFirmwaredID(Convert.ToInt32(hdfTLDFirmwareID.Value))
            beforeTanks = ""
            For Each CustomersRows As GridViewRow In gvCustomers.Rows
                Dim checkCheckedFlag As Integer = 0
                ' Dim CheckArray As ArrayList = New ArrayList
                Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                Dim gvTanksTanks As GridView = TryCast(CustomersRows.FindControl("gvTanks"), GridView)
                If gvTanksTanks IsNot Nothing Then
                    For Each TanksRows As GridViewRow In gvTanksTanks.Rows
                        Dim TankID As String = gvTanksTanks.DataKeys(TanksRows.RowIndex).Values("TankId").ToString()
                        Dim TankNumberNameForView As String = gvTanksTanks.DataKeys(TanksRows.RowIndex).Values("TankNumberNameForView").ToString()
                        Dim ChkTanks As CheckBox = TryCast(TanksRows.FindControl("ChkTanks"), CheckBox)
                        If ChkTanks IsNot Nothing Then
                            If dtTLDFirmwareFieldTank IsNot Nothing And dtTLDFirmwareFieldTank.Rows.Count > 0 Then
                                For i = 0 To dtTLDFirmwareFieldTank.Rows.Count - 1
                                    If TankID = dtTLDFirmwareFieldTank.Rows(i)("TankId") Then
                                        If ChkTanks IsNot Nothing Then
                                            ChkTanks.Checked = True
                                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                beforeTanks = IIf(beforeTanks = "", " Tank Name = " + TankNumberNameForView + " CustomerName = " + CustomerName, beforeTanks & ";" & " Tank Name = " + TankNumberNameForView + " CustomerName= " + CustomerName)
                                            End If
                                            'CheckArray.Add(TankID)
                                        Else
                                            checkCheckedFlag = 1
                                        End If
                                        'ElseIf CustomerId = Convert.ToInt32(dtTLDFirmwareFieldTank.Rows(i)("CustomerId").ToString() And Not CheckArray.Contains(TankID)) Then
                                        '    checkCheckedFlag = 1
                                    End If
                                Next
                            End If
                        End If
                    Next
                    Dim DtView As DataView = New DataView(dtTLDFirmwareFieldTank)
                    DtView.RowFilter = "CustomerId = " + CustomerId
                    If DtView.Count = gvTanksTanks.Rows.Count And gvTanksTanks.Rows.Count > 0 Then
                        Dim checkAll As CheckBox = DirectCast(gvTanksTanks.HeaderRow.FindControl("checkAll"), CheckBox)
                        If checkAll IsNot Nothing Then
                            checkAll.Checked = True
                        End If
                    End If
                End If
            Next

            beforeData = CreateData(Convert.ToInt32(hdfTLDFirmwareID.Value), True)

        Catch ex As Exception
            log.Error("Error occurred in BindCheckBoxTanksValues Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

    Protected Sub OnRowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim customerId As String = gvCustomers.DataKeys(e.Row.RowIndex).Value.ToString()
                Dim gvTanks As GridView = TryCast(e.Row.FindControl("gvTanks"), GridView)


                Dim dtTanks As DataTable = New DataTable()
                Dim dtActualTanks As DataTable = New DataTable()
                Dim dcID = New DataColumn("TankId", GetType(Int32))
                Dim dcName = New DataColumn("TankNumberNameForView", GetType(String))

                dtActualTanks.Columns.Add(dcID)
                dtActualTanks.Columns.Add(dcName)


                OBJMaster = New MasterBAL()


                dtTanks = OBJMaster.GetTankbyConditions(" and PROBEMacAddress <> '' and t.CustomerId = " & customerId, Session("PersonId").ToString(), Session("RoleId").ToString())
                If dtTanks IsNot Nothing And dtTanks.Rows.Count > 0 Then
                    For i = 0 To dtTanks.Rows.Count - 1
                        dtActualTanks.Rows.Add(Convert.ToInt32(dtTanks.Rows(i)("TankId")), dtTanks.Rows(i)("TankNumberNameForView").ToString())
                    Next
                End If


                If dtActualTanks IsNot Nothing And dtActualTanks.Rows.Count > 0 Then
                    gvTanks.DataSource = dtActualTanks
                    gvTanks.DataBind()
                Else
                    gvTanks.DataSource = Nothing
                    gvTanks.DataBind()
                End If



            End If
        Catch ex As Exception
            log.Error("Error occurred in OnRowDataBound Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try

    End Sub

    Private Sub SaveTLDFirmwareupgradeMappingwithFluidTanks(TLDFirmwareId As Integer)
        Try
            Dim dtTLDFirmwareFluidSecureTanks As DataTable = New DataTable("dtTLDFirmwareFluidSecureTanks")

            dtTLDFirmwareFluidSecureTanks.Columns.Add("TLDFirmwareId", System.Type.[GetType]("System.Int32"))
            dtTLDFirmwareFluidSecureTanks.Columns.Add("CustomerId", System.Type.[GetType]("System.Int32"))
            dtTLDFirmwareFluidSecureTanks.Columns.Add("TankId", System.Type.[GetType]("System.Int32"))
            afterTanks = ""
            Dim strTank As String = ""
            For Each CustomersRows As GridViewRow In gvCustomers.Rows
                Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                Dim gvTanksTanks As GridView = TryCast(CustomersRows.FindControl("gvTanks"), GridView)
                If gvTanksTanks IsNot Nothing Then
                    'Delete previous save mapping against companu anf TLDfirmware
                    OBJMaster.DeleteTLDFirmwareFluidSecureTanksMapping(CustomerId, TLDFirmwareId)
                    For Each TanksRows As GridViewRow In gvTanksTanks.Rows
                        Dim TankID As String = gvTanksTanks.DataKeys(TanksRows.RowIndex).Values("TankId").ToString()
                        Dim ChkTanks As CheckBox = TryCast(TanksRows.FindControl("ChkTanks"), CheckBox)
                        Dim TankNumberNameForView As String = gvTanksTanks.DataKeys(TanksRows.RowIndex).Values("TankNumberNameForView").ToString()
                        If ChkTanks IsNot Nothing Then
                            If ChkTanks.Checked Then
                                Dim dr As DataRow = dtTLDFirmwareFluidSecureTanks.NewRow()
                                dr("TLDFirmwareId") = TLDFirmwareId
                                dr("CustomerId") = CustomerId
                                dr("TankID") = TankID
                                dtTLDFirmwareFluidSecureTanks.Rows.Add(dr)
                                afterTanks = IIf(beforeTanks = "", " Tank Name = " + TankNumberNameForView + " CustomerName = " + CustomerName, beforeTanks & ";" & " Tank Name = " + TankNumberNameForView + " CustomerName= " + CustomerName)
                            End If
                        End If
                    Next
                End If
            Next

            If dtTLDFirmwareFluidSecureTanks IsNot Nothing And dtTLDFirmwareFluidSecureTanks.Rows.Count > 0 Then
                OBJMaster.InsertTLDFirmwareFluidSecureTanksMapping(dtTLDFirmwareFluidSecureTanks, TLDFirmwareId)
            End If

        Catch ex As Exception
            log.Error("Error occurred in SaveFirmwareupgradeMappingwithFluidTanks Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

    Private Shared Function CreateData(TLDFirmwareId As String, IsBefore As Boolean) As String
        Try

            Dim data As String = ""

            Dim dtTankInventory As DataSet = New DataSet()

            Dim OBJMaster As MasterBAL = New MasterBAL()
            dtTankInventory = OBJMaster.GetTLDFirmwareById(TLDFirmwareId)

            Dim mapping As String = ""

            If IsBefore Then
                mapping = beforeTanks
            Else
                mapping = afterTanks
            End If

            data = "TLDFirmwareId = " & TLDFirmwareId & " ; " &
                                    "Upload firware Name = " & dtTankInventory.Tables(0).Rows(0)("TLDFirmwareFileName").Replace(",", " ") & " ; " &
                                    "TLD Firmware version number = " & dtTankInventory.Tables(0).Rows(0)("Version").Replace(",", " ") & " ; " &
                                    "TLD Firmware and FuelSecure Tank Mapping  = " & mapping & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class