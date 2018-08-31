Imports System.IO
Imports log4net.Config
Imports log4net

Public Class FirmwareUpgrades
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FirmwareUpgrades))
    Dim OBJMaster As MasterBAL = New MasterBAL()
    Shared beforeData As String
    Shared beforeLinks As String
	Shared afterLinks As String

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
                If (Not Request.QueryString("FirmwareId") = Nothing And Not Request.QueryString("FirmwareId") = "") Then

                    hdfFirmwareID.Value = Request.QueryString("FirmwareId")
                    lblHeader.Text = "Edit Firmware"
                    Dim dtFirmware As DataTable = New DataTable()
                    OBJMaster = New MasterBAL()
                    dtFirmware = OBJMaster.GetFirmwaresByCondition(" and FirmwareId = " + hdfFirmwareID.Value, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                    ViewDiv.Visible = True
                    Uploaddiv.Visible = False
                    If dtFirmware IsNot Nothing And dtFirmware.Rows.Count > 0 Then
                        lblFirmwareName.Text = dtFirmware.Rows(0)("Version").ToString()
                        lblUploadfirware.Text = dtFirmware.Rows(0)("FirmwareFileName").ToString()
                    End If
                    BindCompanyAndLink(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
                    BindCheckBoxLinksValues()
                    If (Request.QueryString("RecordIs") = "New") Then
                        message.Visible = True
                        message.InnerText = "Record saved"
                    End If
                Else
                    ViewDiv.Visible = False
                    Uploaddiv.Visible = True
                    txtfirmwareversionnumber.Focus()
                    lblHeader.Text = "Add Firmware"
                    BindCompanyAndLink(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
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

			If hdfFirmwareID.Value = "" Or hdfFirmwareID.Value = "0" Then
                Dim FirmwareId As Integer = 0

                'If (Not HDF_ShipmentId.Value = Nothing And Not HDF_ShipmentId.Value = "") Then

                '    ShipmentId = HDF_ShipmentId.Value

                'End If
                Dim CheckVersionExists As Integer = 0
                OBJMaster = New MasterBAL()
                CheckVersionExists = OBJMaster.CheckVersionIsExist(txtfirmwareversionnumber.Text)

                If CheckVersionExists = -1 Then

                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Version " + txtfirmwareversionnumber.Text + " already exist."

                    Return

                End If

                Dim fileExt As String

                fileExt = System.IO.Path.GetExtension(FU_Firware.FileName)

                If (fileExt <> ".bin") Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Only .bin files allowed to upload!."

                    Return
                End If

                Dim FirmwareFileName As String

                Dim FirmwareFilePath As String

                Dim folderPath As String = Server.MapPath("~/Firmwares/" & txtfirmwareversionnumber.Text & "/")

                'Check whether Directory (Folder) exists.
                If Not Directory.Exists(folderPath) Then
                    'If Directory (Folder) does not exists. Create it.
                    Directory.CreateDirectory(folderPath)
                Else
                    Dim newFolderPath As String = Server.MapPath("~/Firmwares/" & txtfirmwareversionnumber.Text & "_old_" & DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss"))
                    'Directory.CreateDirectory(newFolderPath)
                    Directory.Move(folderPath, newFolderPath)
                    Directory.CreateDirectory(folderPath)
                End If

                'Save the File to the Directory (Folder).
                FU_Firware.SaveAs(folderPath & Path.GetFileName(FU_Firware.FileName))

                FirmwareFileName = Path.GetFileName(FU_Firware.FileName)
                FirmwareFilePath = "/Firmwares/" & txtfirmwareversionnumber.Text & "/" & FirmwareFileName


                OBJMaster = New MasterBAL()

                Dim result As Integer = 0

                OBJMaster = New MasterBAL()

                'Dim shipmentDatetime As DateTime = Request.Form(txtShipmentDate.UniqueID) & " " & Request.Form(txtShipmentTime.UniqueID)

                result = OBJMaster.SaveUpdateFirmware(FirmwareId, FirmwareFileName, FirmwareFilePath, txtfirmwareversionnumber.Text, Convert.ToInt32(Session("PersonId")))

                If result > 0 Then
                    'Save Firmwareupgrade Mapping with FluidLinks
                    SaveFirmwareupgradeMappingwithFluidLinks(result)
                    message.Visible = True
                    message.InnerText = "Firmware uploaded successfully"
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Added", "Firmware Upgrades", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    Response.Redirect(String.Format("~/Master/FirmwareUpgrades?FirmwareId={0}&RecordIs=New", result))
                Else
                    If (FirmwareId > 0) Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Firmware uploading failed, please try again"
                    Else
                        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                            CSCommonHelper.WriteLog("Added", "Firmware Upgrades", "", "Upload firware Name = " & FU_Firware.FileName.Replace(",", " ") & " ; Firmware version number =  " & txtfirmwareversionnumber.Text.Replace(",", " "), HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Firmware uploading failed.")
                        End If
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Firmware uploading failed, please try again"
                    End If

                End If
            Else
                Dim result = OBJMaster.SaveUpdateFirmware(Convert.ToInt32(hdfFirmwareID.Value), "", "", "", Convert.ToInt32(Session("PersonId")))
                If result > 0 Then
                    'Save Firmwareupgrade Mapping with FluidLinks
                    SaveFirmwareupgradeMappingwithFluidLinks(result)
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Modified", "Firmware Upgrade", beforeData, writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    Response.Redirect(String.Format("~/Master/FirmwareUpgrades?FirmwareId={0}&RecordIs=New", result))
                Else

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Modified", "Firmware Upgrade", beforeData, writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Firmware uploading failed, please try again"
                End If
            End If

        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Added", "Firmware Upgrades", "", "Upload firware Name = " & FU_Firware.FileName.Replace(",", " ") & " ; Firmware version number =  " & txtfirmwareversionnumber.Text.Replace(",", " "), HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Firmware uploading failed. Exception is : " & ex.Message)
            End If
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
        Finally

        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllFirmwareUpgrade")
    End Sub

    Private Sub BindCompanyAndLink(PersonId As Integer, RoleId As String, flag As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()

            dtCust = OBJMaster.GetCustomerDetailsByPersonID(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())
            If dtCust IsNot Nothing And dtCust.Rows.Count > 0 Then
                gvCustomers.DataSource = dtCust
                gvCustomers.DataBind()
            End If
        Catch ex As Exception

            log.Error("Error occurred in BindCompanyAndLink Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindCheckBoxLinksValues()
        Try
            Dim dtFirmwareFieldLink As DataTable = New DataTable()
            dtFirmwareFieldLink = OBJMaster.GetFirmxareFluidLinkMappingByFirmwaredID(Convert.ToInt32(hdfFirmwareID.Value))
            beforeLinks = ""
            For Each CustomersRows As GridViewRow In gvCustomers.Rows
                Dim checkCheckedFlag As Integer = 0
                ' Dim CheckArray As ArrayList = New ArrayList
                Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                Dim gvLinksLinks As GridView = TryCast(CustomersRows.FindControl("gvLinks"), GridView)
                If gvLinksLinks IsNot Nothing Then
                    For Each LinksRows As GridViewRow In gvLinksLinks.Rows
                        Dim SiteID As String = gvLinksLinks.DataKeys(LinksRows.RowIndex).Values("SiteId").ToString()
                        Dim WifiSSId As String = gvLinksLinks.DataKeys(LinksRows.RowIndex).Values("WifiSSId").ToString()
                        Dim ChkLinks As CheckBox = TryCast(LinksRows.FindControl("ChkLinks"), CheckBox)
                        If ChkLinks IsNot Nothing Then
                            If dtFirmwareFieldLink IsNot Nothing And dtFirmwareFieldLink.Rows.Count > 0 Then
                                For i = 0 To dtFirmwareFieldLink.Rows.Count - 1
                                    If SiteID = dtFirmwareFieldLink.Rows(i)("SiteId") Then
                                        If ChkLinks IsNot Nothing Then
                                            ChkLinks.Checked = True
                                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                beforeLinks = IIf(beforeLinks = "", " Site Link Name = " + WifiSSId + " CustomerName = " + CustomerName, beforeLinks & ";" & " Site Link Name = " + WifiSSId + " CustomerName= " + CustomerName)
                                            End If
                                            'CheckArray.Add(SiteID)
                                        Else
                                            checkCheckedFlag = 1
                                        End If
                                        'ElseIf CustomerId = Convert.ToInt32(dtFirmwareFieldLink.Rows(i)("CustomerId").ToString() And Not CheckArray.Contains(SiteID)) Then
                                        '    checkCheckedFlag = 1
                                    End If
                                Next
                            End If
                        End If
                    Next
                    Dim DtView As DataView = New DataView(dtFirmwareFieldLink)
                    DtView.RowFilter = "CustomerId = " + CustomerId
                    If DtView.Count = gvLinksLinks.Rows.Count And gvLinksLinks.Rows.Count > 0 Then
                        Dim checkAll As CheckBox = DirectCast(gvLinksLinks.HeaderRow.FindControl("checkAll"), CheckBox)
                        If checkAll IsNot Nothing Then
                            checkAll.Checked = True
                        End If
                    End If
                End If
            Next

            beforeData = CreateData(Convert.ToInt32(hdfFirmwareID.Value), True)

        Catch ex As Exception
            log.Error("Error occurred in BindCheckBoxLinksValues Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

    Protected Sub OnRowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim customerId As String = gvCustomers.DataKeys(e.Row.RowIndex).Value.ToString()
                Dim gvLinks As GridView = TryCast(e.Row.FindControl("gvLinks"), GridView)


                Dim dtSites As DataTable = New DataTable()
                Dim dtActualSites As DataTable = New DataTable()
                Dim dcID = New DataColumn("SiteId", GetType(Int32))
                Dim dcName = New DataColumn("WifiSSid", GetType(String))

                dtActualSites.Columns.Add(dcID)
                dtActualSites.Columns.Add(dcName)


                OBJMaster = New MasterBAL()


                dtSites = OBJMaster.GetSiteByCondition(" And c.CustomerId =" + customerId, Session("PersonId").ToString(), Session("RoleId").ToString(), False)
                If dtSites IsNot Nothing And dtSites.Rows.Count > 0 Then
                    For i = 0 To dtSites.Rows.Count - 1
                        dtActualSites.Rows.Add(Convert.ToInt32(dtSites.Rows(i)("SiteId")), dtSites.Rows(i)("WifiSSid").ToString())
                    Next
                End If


                If dtActualSites IsNot Nothing And dtActualSites.Rows.Count > 0 Then
                    gvLinks.DataSource = dtActualSites
                    gvLinks.DataBind()
                Else
                    gvLinks.DataSource = Nothing
                    gvLinks.DataBind()
                End If



            End If
        Catch ex As Exception
            log.Error("Error occurred in OnRowDataBound Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try

    End Sub

    Private Sub SaveFirmwareupgradeMappingwithFluidLinks(FirmwareId As Integer)
        Try
            Dim dtFirmwareFluidSecureLinks As DataTable = New DataTable("dtFirmwareFluidSecureLinks")

            dtFirmwareFluidSecureLinks.Columns.Add("FirmwareId", System.Type.[GetType]("System.Int32"))
            dtFirmwareFluidSecureLinks.Columns.Add("CustomerId", System.Type.[GetType]("System.Int32"))
            dtFirmwareFluidSecureLinks.Columns.Add("SiteID", System.Type.[GetType]("System.Int32"))
            afterLinks = ""
            Dim strSite As String = ""
            For Each CustomersRows As GridViewRow In gvCustomers.Rows
                Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                Dim gvLinksLinks As GridView = TryCast(CustomersRows.FindControl("gvLinks"), GridView)
                If gvLinksLinks IsNot Nothing Then
                    'Delete previous save mapping against companu anf firmware
                    OBJMaster.DeleteFirmwareFluidSecureLinksMapping(CustomerId, FirmwareId)
                    For Each LinksRows As GridViewRow In gvLinksLinks.Rows
                        Dim SiteID As String = gvLinksLinks.DataKeys(LinksRows.RowIndex).Values("SiteId").ToString()
                        Dim ChkLinks As CheckBox = TryCast(LinksRows.FindControl("ChkLinks"), CheckBox)
                        Dim WifiSSId As String = gvLinksLinks.DataKeys(LinksRows.RowIndex).Values("WifiSSId").ToString()
                        If ChkLinks IsNot Nothing Then
                            If ChkLinks.Checked Then
                                Dim dr As DataRow = dtFirmwareFluidSecureLinks.NewRow()
                                dr("FirmwareId") = FirmwareId
                                dr("CustomerId") = CustomerId
                                dr("SiteID") = SiteID
                                dtFirmwareFluidSecureLinks.Rows.Add(dr)
                                afterLinks = IIf(beforeLinks = "", " Site Link Name = " + WifiSSId + " CustomerName = " + CustomerName, beforeLinks & ";" & " Site Link Name = " + WifiSSId + " CustomerName= " + CustomerName)
                            End If
                        End If
                    Next
                End If
            Next

            If dtFirmwareFluidSecureLinks IsNot Nothing And dtFirmwareFluidSecureLinks.Rows.Count > 0 Then
                OBJMaster.InsertFirmwareFluidSecureLinksMapping(dtFirmwareFluidSecureLinks, FirmwareId)
            End If

        Catch ex As Exception
            log.Error("Error occurred in SaveFirmwareupgradeMappingwithFluidLinks Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

	Private Shared Function CreateData(FirmwareId As String, IsBefore As Boolean) As String
		Try

			Dim data As String = ""

			Dim dtTankInventory As DataSet = New DataSet()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtTankInventory = OBJMaster.GetFirmwareById(FirmwareId)

			Dim mapping As String = ""

			If IsBefore Then
				mapping = beforeLinks
			Else
				mapping = afterLinks
			End If

			data = "FirmwareId = " & FirmwareId & " ; " &
									"Upload firware Name = " & dtTankInventory.Tables(0).Rows(0)("FirmwareFileName").Replace(",", " ") & " ; " &
									"Firmware version number = " & dtTankInventory.Tables(0).Rows(0)("Version").Replace(",", " ") & " ; " &
									"Firmware and FuelSecure Link Mapping  = " & mapping & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class