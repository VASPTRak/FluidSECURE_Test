Imports System.IO
Imports log4net.Config
Imports log4net

Public Class FSVMFirmwareUpgrades
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FSVMFirmwareUpgrades))
    Dim OBJMaster As MasterBAL = New MasterBAL()
    Shared beforeData As String
    Shared beforeVehicles As String
	Shared afterVehicles As String

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
					If (Not Request.QueryString("FSVMFirmwareId") = Nothing And Not Request.QueryString("FSVMFirmwareId") = "") Then

						hdfFSVMFirmwareID.Value = Request.QueryString("FSVMFirmwareId")
						lblHeader.Text = "Edit FSVMFirmware"
						Dim dtFSVMFirmware As DataTable = New DataTable()
						OBJMaster = New MasterBAL()
						dtFSVMFirmware = OBJMaster.GetFSVMFirmwaresByCondition(" and FSVMFirmwareId = " + hdfFSVMFirmwareID.Value, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
						ViewDiv.Visible = True
						Uploaddiv.Visible = False
						If dtFSVMFirmware IsNot Nothing And dtFSVMFirmware.Rows.Count > 0 Then
							lblFSVMFirmwareName.Text = dtFSVMFirmware.Rows(0)("Version").ToString()
							lblUploadFSVMFirmware.Text = dtFSVMFirmware.Rows(0)("FSVMFirmwareFileName").ToString()
						End If
						BindCompanyAndVehicles(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
						BindCheckBoxVehiclesValues()
						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message.InnerText = "Record saved"
						End If
					Else
						ViewDiv.Visible = False
						Uploaddiv.Visible = True
						txtFSVMFirmwareversionnumber.Focus()
						lblHeader.Text = "Add FSVMFirmware"
						BindCompanyAndVehicles(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), 0)
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
            If hdfFSVMFirmwareID.Value = "" Or hdfFSVMFirmwareID.Value = "0" Then
                Dim FSVMFirmwareId As Integer = 0

                'If (Not HDF_ShipmentId.Value = Nothing And Not HDF_ShipmentId.Value = "") Then

                '    ShipmentId = HDF_ShipmentId.Value

                'End If
                Dim CheckVersionExists As Integer = 0
                OBJMaster = New MasterBAL()
                CheckVersionExists = OBJMaster.FSVMCheckVersionIsExist(txtFSVMFirmwareversionnumber.Text)

                If CheckVersionExists = -1 Then

                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Version " + txtFSVMFirmwareversionnumber.Text + " already exist."

                    Return

                End If

                Dim fileExt As String

                fileExt = System.IO.Path.GetExtension(FU_FSVMFirmware.FileName)

                If (fileExt <> ".bin") Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Only .bin files allowed to upload!."

                    Return
                End If

                Dim FSVMFirmwareFileName As String

                Dim FSVMFirmwareFilePath As String

                Dim folderPath As String = Server.MapPath("~/FSVMFirmwares/ESP32/" & txtFSVMFirmwareversionnumber.Text & "/")

                'Check whether Directory (Folder) exists.
                If Not Directory.Exists(folderPath) Then
                    'If Directory (Folder) does not exists. Create it.
                    Directory.CreateDirectory(folderPath)
                Else
                    Dim newFolderPath As String = Server.MapPath("~/FSVMFirmwares/ESP32/" & txtFSVMFirmwareversionnumber.Text & "_old_" & DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss"))
                    'Directory.CreateDirectory(newFolderPath)
                    Directory.Move(folderPath, newFolderPath)
                    Directory.CreateDirectory(folderPath)
                End If

                'Save the File to the Directory (Folder).
                FU_FSVMFirmware.SaveAs(folderPath & Path.GetFileName(FU_FSVMFirmware.FileName))

                FSVMFirmwareFileName = Path.GetFileName(FU_FSVMFirmware.FileName)
                FSVMFirmwareFilePath = "/FSVMFirmwares/ESP32/" & txtFSVMFirmwareversionnumber.Text & "/" & FSVMFirmwareFileName


                OBJMaster = New MasterBAL()

                Dim result As Integer = 0

                OBJMaster = New MasterBAL()

                'Dim shipmentDatetime As DateTime = Request.Form(txtShipmentDate.UniqueID) & " " & Request.Form(txtShipmentTime.UniqueID)

                result = OBJMaster.SaveUpdateFSVMFirmware(FSVMFirmwareId, FSVMFirmwareFileName, FSVMFirmwareFilePath, txtFSVMFirmwareversionnumber.Text, Convert.ToInt32(Session("PersonId")))

                If result > 0 Then
                    'Save FSVMFirmwareupgrade Mapping with VehicleId
                    SaveFSVMFirmwareupgradeMappingwithVehicleId(result)
                    message.Visible = True
                    message.InnerText = "FSVM Firmware uploaded successfully"
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Added", "FSVMFirmware Upgrades", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    Response.Redirect(String.Format("~/Master/FSVMFirmwareUpgrades?FSVMFirmwareId={0}&RecordIs=New", result))
                Else
                    If (FSVMFirmwareId > 0) Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "FSVMFirmware uploading failed, please try again"
                    Else
                        If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                            CSCommonHelper.WriteLog("Added", "FSVMFirmware Upgrades", "", "Upload FSVMFirmware Name = " & FU_FSVMFirmware.FileName.Replace(",", " ") & " ; FSVMFirmware version number =  " & txtFSVMFirmwareversionnumber.Text.Replace(",", " "), HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "FSVMFirmware uploading failed.")
                        End If
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "FSVMFirmware uploading failed, please try again"
                    End If

                End If
            Else
                Dim result = OBJMaster.SaveUpdateFSVMFirmware(Convert.ToInt32(hdfFSVMFirmwareID.Value), "", "", "", Convert.ToInt32(Session("PersonId")))
                If result > 0 Then
                    'Save FSVMFirmwareupgrade Mapping with VehicleId
                    SaveFSVMFirmwareupgradeMappingwithVehicleId(result)
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Modified", "FSVMFirmware Upgrade", beforeData, writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    Response.Redirect(String.Format("~/Master/FSVMFirmwareUpgrades?FSVMFirmwareId={0}&RecordIs=New", result))
                Else

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result, False)
                        CSCommonHelper.WriteLog("Modified", "FSVMFirmware Upgrade", beforeData, writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "FSVMFirmware uploading failed, please try again"
                End If
            End If

        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Added", "FSVMFirmware Upgrades", "", "Upload FSVMFirmware Name = " & FU_FSVMFirmware.FileName.Replace(",", " ") & " ; FSVMFirmware version number =  " & txtFSVMFirmwareversionnumber.Text.Replace(",", " "), HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "FSVMFirmware uploading failed. Exception is : " & ex.Message)
            End If
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
        Finally

        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllFSVMFirmwareUpgrades")
    End Sub

    Private Sub BindCompanyAndVehicles(PersonId As Integer, RoleId As String, flag As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()

            dtCust = OBJMaster.GetCustomerDetailsByPersonID(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())
            If dtCust IsNot Nothing And dtCust.Rows.Count > 0 Then
                gvCustomers.DataSource = dtCust
                gvCustomers.DataBind()
            End If
        Catch ex As Exception

            log.Error("Error occurred in BindCompanyAndVehicles Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindCheckBoxVehiclesValues()
        Try
            Dim dtFSVMFirmwareFieldVehicles As DataTable = New DataTable()
            dtFSVMFirmwareFieldVehicles = OBJMaster.GetFSVMFirmwareVehicleIdMappingByFSVMFirmwaredID(Convert.ToInt32(hdfFSVMFirmwareID.Value))
            beforeVehicles = ""
            For Each CustomersRows As GridViewRow In gvCustomers.Rows
                Dim checkCheckedFlag As Integer = 0
                ' Dim CheckArray As ArrayList = New ArrayList
                Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                Dim gvVehiclesData As GridView = TryCast(CustomersRows.FindControl("gvVehicles"), GridView)
                If gvVehiclesData IsNot Nothing Then
                    For Each VehiclesRows As GridViewRow In gvVehiclesData.Rows
                        Dim VehicleId As String = gvVehiclesData.DataKeys(VehiclesRows.RowIndex).Values("VehicleId").ToString()
                        Dim VehicleName As String = gvVehiclesData.DataKeys(VehiclesRows.RowIndex).Values("VehicleName").ToString()
                        Dim ChkVehicles As CheckBox = TryCast(VehiclesRows.FindControl("ChkVehicles"), CheckBox)
                        If ChkVehicles IsNot Nothing Then
                            If dtFSVMFirmwareFieldVehicles IsNot Nothing And dtFSVMFirmwareFieldVehicles.Rows.Count > 0 Then
                                For i = 0 To dtFSVMFirmwareFieldVehicles.Rows.Count - 1
                                    If VehicleId = dtFSVMFirmwareFieldVehicles.Rows(i)("VehicleId") Then
                                        If ChkVehicles IsNot Nothing Then
                                            ChkVehicles.Checked = True
                                            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                                                beforeVehicles = IIf(beforeVehicles = "", " Vehicle Name = " + VehicleName + " CustomerName = " + CustomerName, beforeVehicles & ";" & " Vehicle Name = " + VehicleName + " CustomerName= " + CustomerName)
                                            End If
                                            'CheckArray.Add(VehicleId)
                                        Else
                                            checkCheckedFlag = 1
                                        End If
                                        'ElseIf CustomerId = Convert.ToInt32(dtFSVMFirmwareVehicleId.Rows(i)("CustomerId").ToString() And Not CheckArray.Contains(VehicleId)) Then
                                        '    checkCheckedFlag = 1
                                    End If
                                Next
                            End If
                        End If
                    Next
                    Dim DtView As DataView = New DataView(dtFSVMFirmwareFieldVehicles)
                    DtView.RowFilter = "CustomerId = " + CustomerId
                    If DtView.Count = gvVehiclesData.Rows.Count And gvVehiclesData.Rows.Count > 0 Then
                        Dim checkAll As CheckBox = DirectCast(gvVehiclesData.HeaderRow.FindControl("checkAll"), CheckBox)
                        If checkAll IsNot Nothing Then
                            checkAll.Checked = True
                        End If
                    End If
                End If
            Next

            beforeData = CreateData(Convert.ToInt32(hdfFSVMFirmwareID.Value), True)

        Catch ex As Exception
            log.Error("Error occurred in BindCheckBoxVehiclesValues Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

    Protected Sub OnRowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim customerId As String = gvCustomers.DataKeys(e.Row.RowIndex).Value.ToString()
                Dim gvVehiclesData As GridView = TryCast(e.Row.FindControl("gvVehicles"), GridView)


                Dim dtVehicles As DataTable = New DataTable()
                Dim dtActualVehicles As DataTable = New DataTable()
                Dim dcID = New DataColumn("VehicleId", GetType(Int32))
                Dim dcName = New DataColumn("VehicleName", GetType(String))

                dtActualVehicles.Columns.Add(dcID)
                dtActualVehicles.Columns.Add(dcName)


                OBJMaster = New MasterBAL()


                dtVehicles = OBJMaster.GetVehicleByCondition(" And c.CustomerId =" + customerId, Session("PersonId").ToString(), Session("RoleId").ToString())
                If dtVehicles IsNot Nothing And dtVehicles.Rows.Count > 0 Then
                    For i = 0 To dtVehicles.Rows.Count - 1
                        dtActualVehicles.Rows.Add(Convert.ToInt32(dtVehicles.Rows(i)("VehicleId")), dtVehicles.Rows(i)("VehicleName").ToString())
                    Next
                End If


                If dtActualVehicles IsNot Nothing And dtActualVehicles.Rows.Count > 0 Then
                    gvVehiclesData.DataSource = dtActualVehicles
                    gvVehiclesData.DataBind()
                Else
                    gvVehiclesData.DataSource = Nothing
                    gvVehiclesData.DataBind()
                End If



            End If
        Catch ex As Exception
            log.Error("Error occurred in OnRowDataBound Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try

    End Sub

    Private Sub SaveFSVMFirmwareupgradeMappingwithVehicleId(FSVMFirmwareId As Integer)
        Try
            Dim dtFSVMFirmwareVehicleId As DataTable = New DataTable("dtFSVMFirmwareVehicleId")

            dtFSVMFirmwareVehicleId.Columns.Add("FSVMFirmwareId", System.Type.[GetType]("System.Int32"))
            dtFSVMFirmwareVehicleId.Columns.Add("CustomerId", System.Type.[GetType]("System.Int32"))
            dtFSVMFirmwareVehicleId.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
            afterVehicles = ""
            Dim strVehicle As String = ""
            For Each CustomersRows As GridViewRow In gvCustomers.Rows
                Dim CustomerId As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerId").ToString()
                Dim CustomerName As String = gvCustomers.DataKeys(CustomersRows.RowIndex).Values("CustomerName").ToString()
                Dim gvVehiclesData As GridView = TryCast(CustomersRows.FindControl("gvVehicles"), GridView)
                If gvVehiclesData IsNot Nothing Then
                    'Delete previous save mapping against companu anf FSVMFirmware
                    OBJMaster.DeleteFSVMFirmwareVehicleIdMapping(CustomerId, FSVMFirmwareId)
                    For Each VehiclesRows As GridViewRow In gvVehiclesData.Rows
                        Dim VehicleId As String = gvVehiclesData.DataKeys(VehiclesRows.RowIndex).Values("VehicleId").ToString()
                        Dim ChkVehicles As CheckBox = TryCast(VehiclesRows.FindControl("ChkVehicles"), CheckBox)
                        Dim VehicleName As String = gvVehiclesData.DataKeys(VehiclesRows.RowIndex).Values("VehicleName").ToString()
                        If ChkVehicles IsNot Nothing Then
                            If ChkVehicles.Checked Then
                                Dim dr As DataRow = dtFSVMFirmwareVehicleId.NewRow()
                                dr("FSVMFirmwareId") = FSVMFirmwareId
                                dr("CustomerId") = CustomerId
                                dr("VehicleId") = VehicleId
                                dtFSVMFirmwareVehicleId.Rows.Add(dr)
                                afterVehicles = IIf(beforeVehicles = "", " Vehicle Name = " + VehicleName + " CustomerName = " + CustomerName, beforeVehicles & ";" & " Vehicle Name = " + VehicleName + " CustomerName= " + CustomerName)
                            End If
                        End If
                    Next
                End If
            Next

            If dtFSVMFirmwareVehicleId IsNot Nothing And dtFSVMFirmwareVehicleId.Rows.Count > 0 Then
                OBJMaster.InsertFSVMFirmwareVehicleIdMapping(dtFSVMFirmwareVehicleId, FSVMFirmwareId)
            End If

        Catch ex As Exception
            log.Error("Error occurred in SaveFSVMFirmwareupgradeMappingwithVehicleId Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."
        End Try
    End Sub

	Private Shared Function CreateData(FSVMFirmwareId As String, IsBefore As Boolean) As String
		Try

			Dim data As String = ""

			Dim dtTankInventory As DataSet = New DataSet()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtTankInventory = OBJMaster.GetFSVMFirmwareById(FSVMFirmwareId)

			Dim mapping As String = ""

			If IsBefore Then
				mapping = beforeVehicles
			Else
				mapping = afterVehicles
			End If

			data = "FSVMFirmwareId = " & FSVMFirmwareId & " ; " &
									"Upload FSVMFirmware Name = " & dtTankInventory.Tables(0).Rows(0)("FSVMFirmwareFileName").Replace(",", " ") & " ; " &
									"FSVMFirmware version number = " & dtTankInventory.Tables(0).Rows(0)("Version").Replace(",", " ") & " ; " &
									"FSVMFirmware and Vehicles Mapping  = " & mapping & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class