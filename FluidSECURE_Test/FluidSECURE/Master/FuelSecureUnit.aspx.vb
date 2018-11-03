Imports System.Web.Services
Imports log4net
Imports log4net.Config
Imports Newtonsoft.Json.Linq

Public Class FuelSecureUnit
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FuelSecureUnit))

	Dim OBJMaster As MasterBAL = New MasterBAL()
	Shared beforeData As String
	Shared beforeFuelingTimes As String = ""
	Shared beforeFuelingDay As String = ""
	Shared afterFuelingTimes As String = ""
	Shared afterFuelingDay As String = ""

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
                If Not IsPostBack Then
                    Session("FirmanameValue") = ""
                    Session("WifiSSIDValue") = ""
                    GetCustomers(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                    BindTimeZones()
                    BindTiming()
                    BindDays()
                    If (Not Request.QueryString("SiteId") = Nothing And Not Request.QueryString("SiteId") = "") Then
                        HDF_SiteID.Value = Request.QueryString("SiteId")
                        BindSiteDetails(Request.QueryString("SiteId"))
                        btnFirst.Visible = True
                        btnNext.Visible = True
                        btnprevious.Visible = True
                        btnLast.Visible = True
                        lblHeader.Text = "Edit FluidSecure Link Information"

                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"
                        End If

                        txtSiteNo.Enabled = False

                    Else
                        If Session("RoleName") <> "SuperAdmin" Then
                            Response.Redirect("~/Master/AllFuelSecureUnits.aspx")
                        End If

                        btnFirst.Visible = False
                        btnNext.Visible = False
                        btnprevious.Visible = False
                        btnLast.Visible = False
                        lblof.Visible = False
                        divActivate.Visible = False
                        lblHeader.Text = "Add FluidSecure Link Information"
                        txtSiteNo.Enabled = False

                        Dim result As DataSet
                        OBJMaster = New MasterBAL()
                        result = OBJMaster.checkLaunchedAndExistedVersionAndUpdate("", "", 0, 2)

                        If result IsNot Nothing Then
                            If result.Tables(0) IsNot Nothing Then
                                If result.Tables(0).Rows.Count > 0 Then
                                    txt_FirmwareVer.Text = result.Tables(0).Rows(0)("launchedversion")
                                Else
                                    txt_FirmwareVer.Text = ""
                                End If
                            Else
                                txt_FirmwareVer.Text = ""
                            End If
                        Else
                            txt_FirmwareVer.Text = ""
                        End If

                    End If
                    DDL_TimeZone_SelectedIndexChanged(Nothing, Nothing)

                    txtwifissid.Focus()

                    'Allo Super admin to update current version
                    If Session("RoleName") = "SuperAdmin" Then
                        txt_FirmwareVer.Enabled = True
                    Else
                        txt_FirmwareVer.Enabled = False
                    End If


                End If

                If Session("RoleName") = "SuperAdmin" Then
                    divShowHideReplacingLink.Visible = False
                    txtwifissid.Enabled = True
                Else
                    divShowHideReplacingLink.Visible = True
                    If chk_EnableDisableLinkName.Checked Then
                        txtwifissid.Enabled = True
                    Else
                        txtwifissid.Enabled = False
                    End If

                End If

            End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Private Sub BindTiming()

		Try

			Dim dtTiming As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtTiming = OBJMaster.GetTiming()

			gv_FuelingTimes.DataSource = dtTiming
			gv_FuelingTimes.DataBind()


		Catch ex As Exception

			log.Error("Error occurred in BindTiming Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Fueling Times, please try again later."

		End Try
	End Sub

	Private Sub BindTimeZones()
		Try

			Dim dtTimeZones As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtTimeZones = OBJMaster.GetTimeZones()

			Session("dtTimeZones") = dtTimeZones
			DDL_TimeZone.DataSource = dtTimeZones
			DDL_TimeZone.DataBind()

			DDL_TimeZone.DataTextField = "TimeZone"
			DDL_TimeZone.DataValueField = "TimeZoneId"
			DDL_TimeZone.DataBind()
			DDL_TimeZone.Items.Insert(0, New ListItem("Select Time Zone", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindTimeZones Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Time zones, please try again later."

		End Try
	End Sub

	Private Sub BindDays()
		Try

			Dim dtDays As DataTable = New DataTable()

			dtDays.Columns.Add("DayValue", System.Type.[GetType]("System.Int32"))
			dtDays.Columns.Add("DayName", System.Type.[GetType]("System.String"))

			dtDays.Rows.Add("0", "Select All")
			dtDays.Rows.Add("1", "Sunday")
			dtDays.Rows.Add("2", "Monday")
			dtDays.Rows.Add("3", "Tuesday")
			dtDays.Rows.Add("4", "Wednesday")
			dtDays.Rows.Add("5", "Thursday")
			dtDays.Rows.Add("6", "Friday")
			dtDays.Rows.Add("7", "Saturday")

			GV_FuelingDays.DataSource = dtDays
			GV_FuelingDays.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in BindDays Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindTanks(CustomerId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtTanks As DataTable = New DataTable()
			dtTanks = OBJMaster.GetTankbyConditions(" And T.CustomerId =" & CustomerId, Session("PersonId").ToString(), Session("RoleId").ToString())

			ViewState("dtTanks") = dtTanks
			DDL_Tank.DataSource = dtTanks
			DDL_Tank.DataTextField = "TankNumberNameForView"
            DDL_Tank.DataValueField = "TankNumber"
            DDL_Tank.DataBind()
			DDL_Tank.Items.Insert(0, New ListItem("Select Tank Number", "0"))

		Catch ex As Exception
			log.Error("Error occurred in BindTanks Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
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
                divCompany.Visible = False
                ddlCustomer.Visible = False

            End If

            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                If (Session("RoleName") = "GroupAdmin") Then
                    ddlCustomer.SelectedValue = Session("CustomerId")
                Else
                    ddlCustomer.SelectedIndex = 1
                End If
            End If

			ddlCustomer_SelectedIndexChanged(Nothing, Nothing)

		Catch ex As Exception

			log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try


	End Sub

	Private Sub GetFuelType(CustomerId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtFuelType As DataTable = New DataTable()

			dtFuelType = OBJMaster.GetFuelDetails(CustomerId)

			ddlFuelType.DataSource = dtFuelType
			ddlFuelType.DataTextField = "FuelType"
			ddlFuelType.DataValueField = "FuelTypeID"
			ddlFuelType.DataBind()
			ddlFuelType.Items.Insert(0, New ListItem("Select Product Type", "0"))

		Catch ex As Exception

			log.Error("Error occurred in GetFuelType Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting fuel types, please try again later."

		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		ValidateAndSaveFSLink(False, True)
	End Sub

	Private Sub AddDays(SiteId)
		Try

			Dim dtDays As DataTable = New DataTable("dtSiteDays")
			dtDays.Columns.Add("SiteID", System.Type.[GetType]("System.Int32"))
			dtDays.Columns.Add("DayValue", System.Type.[GetType]("System.Int32"))


			For Each item As GridViewRow In GV_FuelingDays.Rows

				Dim CHK_FuelingDays As CheckBox = TryCast(item.FindControl("CHK_FuelingDays"), CheckBox)
				If (CHK_FuelingDays.Checked = True) Then
					Dim dr As DataRow = dtDays.NewRow()
					dr("SiteID") = SiteId
					dr("DayValue") = GV_FuelingDays.DataKeys(item.RowIndex).Values("DayValue").ToString()
					dtDays.Rows.Add(dr)
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						afterFuelingDay = IIf(afterFuelingDay = "", GV_FuelingDays.DataKeys(item.RowIndex).Values("DayName").ToString(), afterFuelingDay & ";" & GV_FuelingDays.DataKeys(item.RowIndex).Values("DayName").ToString())
					End If
				End If

			Next


			OBJMaster.InsertSiteDays(dtDays, SiteId)

		Catch ex As Exception

			log.Error("Error occurred in AddDays Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while saving days, please try again later."

		End Try
	End Sub

	Private Sub BindSiteDetails(SiteID As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtSite As DataTable = New DataTable()
			dtSite = OBJMaster.GetSiteId(SiteID)
			Dim cnt As Integer = 0
			If (dtSite.Rows.Count > 0) Then
                Dim isValid As Boolean = False
                If (Session("RoleName") = "GroupAdmin") Then
                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                    For Each drCusts As DataRow In dtCustOld.Rows
                        If (drCusts("CustomerId") = dtSite.Rows(0)("CustomerId").ToString()) Then
                            isValid = True
                            Exit For
                        End If

                    Next
                End If


                If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                    Dim dtCust As DataTable = New DataTable()

                    dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                    If (dtCust.Rows(0)("CustomerId").ToString() <> dtSite.Rows(0)("CustomerId").ToString()) Then

                        'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)
                        Response.Redirect("/home")
                        Return
                    End If

                End If

                BindSiteTimings(SiteID)
				BindSiteDays(SiteID)
				txtSiteNo.Text = dtSite.Rows(0)("SiteNumber").ToString()
				txtSiteAddress.Text = dtSite.Rows(0)("SiteAddress").ToString()
				txt_ExportCode.Text = dtSite.Rows(0)("ExportCode").ToString()
				txtLat.Text = dtSite.Rows(0)("Latitude").ToString()
				txtLong.Text = dtSite.Rows(0)("Longitude").ToString()
				CHK_DisableGeoLocation.Checked = dtSite.Rows(0)("DisableGeoLocation").ToString()

				If Not dtSite.Rows(0)("CustomerId").ToString() = Nothing Then
					ddlCustomer.SelectedValue = dtSite.Rows(0)("CustomerId").ToString()
				Else
					ddlCustomer.SelectedValue = 0
				End If

				Session("OldCompanyId") = ddlCustomer.SelectedValue

                If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
                    ddlCustomer.Enabled = False
                End If

                'GetFuelType(ddlCustomer.SelectedValue)
                ddlCustomer_SelectedIndexChanged(Nothing, Nothing)
				BindHoseDetails(SiteID)

				OBJMaster = New MasterBAL()

				Dim strConditions As String = ""
				If (Not Session("FuelLINKConditions") Is Nothing And Not Session("FuelLINKConditions") = "") Then
					strConditions = Session("FuelLINKConditions")
				Else
					If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
						strConditions = IIf(strConditions = "", " and s.CustomerId=" & Session("CustomerId"), strConditions & " and s.CustomerId=" & Session("CustomerId"))
					End If
				End If

				HDF_TotalSite.Value = OBJMaster.GetSiteIdByCondition(SiteID, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)

				OBJMaster = New MasterBAL()
				Dim dtAllSite As DataTable = New DataTable()

				dtAllSite = OBJMaster.GetSiteByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId"), False)

				dtAllSite.PrimaryKey = New DataColumn() {dtAllSite.Columns(0)}
				Dim dr As DataRow = dtAllSite.Rows.Find(SiteID)
				If Not IsDBNull(dr) Then

					cnt = dtAllSite.Rows.IndexOf(dr) + 1

				End If
                If (HDF_TotalSite.Value = 1) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt >= HDF_TotalSite.Value) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                ElseIf (cnt <= 1) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt > 1 And cnt < HDF_TotalSite.Value) Then
                    btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
                'If cnt = 1 And HDF_TotalSite.Value.ToString() = "1" Then
                '	btnNext.Enabled = False
                '	btnLast.Enabled = False
                '	btnFirst.Enabled = False
                '	btnprevious.Enabled = False
                'End If
                lblof.Text = cnt & " of " & HDF_TotalSite.Value.ToString()

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					beforeData = CreateData(SiteID, True)
				End If

			Else

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Data Not found. Please try again after some time."

			End If
		Catch ex As Exception

			log.Error("Error occurred in BindSiteDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting FluidSecure Link, please try again later."
		Finally
			txtwifissid.Focus()
		End Try
	End Sub

	Private Sub BindHoseDetails(SiteId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtHose As DataTable = New DataTable()
			dtHose = OBJMaster.GetHoseIdBySiteId(SiteId)
			Dim cnt As Integer = 0
			If (dtHose.Rows.Count > 0) Then


				'txtankNumber.Text = dtHose.Rows(0)("TankNumber").ToString()
				Try
                    DDL_Tank.SelectedValue = dtHose.Rows(0)("TankNumber").ToString()
                Catch ex As Exception
					DDL_Tank.SelectedIndex = 0
				End Try

				txtPulserRatio.Text = dtHose.Rows(0)("PulserRatio").ToString()
				txtwifissid.Text = dtHose.Rows(0)("WifiSSId").ToString()
				Session("WifiSSIDValue") = dtHose.Rows(0)("WifiSSId").ToString()
				ddlFuelType.SelectedValue = dtHose.Rows(0)("FuelTypeId").ToString()
				txtPumpOffTime.Text = dtHose.Rows(0)("PumpOffTime").ToString()
				txtPumpOnTime.Text = dtHose.Rows(0)("PumpOnTime").ToString()
				'Chk_TankMonitor.Checked = IIf(dtHose.Rows(0)("TankMonitor").ToString() = "Y", True, False)
				'txtTankMonitorNo.Text = dtHose.Rows(0)("TankMonitorNumber").ToString()
				txtIpAddress.Text = dtHose.Rows(0)("IPAddress").ToString()
				HDF_HoseId.Value = dtHose.Rows(0)("HoseId").ToString()

				txt_ReplaceableHoseName.Text = dtHose.Rows(0)("ReplaceableHoseName").ToString()
				txtUnitsmeasured.Text = dtHose.Rows(0)("Unitsmeasured").ToString()
				txt_Pulses.Text = dtHose.Rows(0)("Pulses").ToString()
				DDL_TimeZone.SelectedValue = dtHose.Rows(0)("TimeZoneId").ToString()

				TXT_OriginalNameOfFluidSecure.Text = dtHose.Rows(0)("OriginalNameOfFluidSecure").ToString()
				LBL_TimeZone.Text = dtHose.Rows(0)("TimeZoneName").ToString()
				CHK_IsBusy.Checked = dtHose.Rows(0)("IsBusy").ToString()
				txt_FirmwareVer.Text = dtHose.Rows(0)("CurrentFirmwareVersion").ToString()
                Session("FirmanameValue") = dtHose.Rows(0)("CurrentFirmwareVersion").ToString()

                If Not dtHose.Rows(0)("ConnectedHub").ToString() = "" Then
					lblConnectedHub.Text = dtHose.Rows(0)("ConnectedHub").ToString()
				Else
					lblConnectedHub.Text = "-"
				End If

				If Not dtHose.Rows(0)("HubSiteName").ToString() = "" Then
					lblSite.Text = dtHose.Rows(0)("HubSiteName").ToString()
				Else
					lblSite.Text = "-"
				End If


				If dtHose.Rows(0)("SwitchTimeBounce").ToString() IsNot Nothing And dtHose.Rows(0)("SwitchTimeBounce").ToString() IsNot "" Then
					txtSwitchTimeB.Text = dtHose.Rows(0)("SwitchTimeBounce").ToString()
				Else
					txtSwitchTimeB.Text = " "
				End If

				If Not Convert.ToString(dtHose.Rows(0)("DisplayOrder")) = "" Then
					txtDisplayOrder.Text = Convert.ToString(dtHose.Rows(0)("DisplayOrder"))
				Else
					txtDisplayOrder.Text = "0"
				End If

				If Not Convert.ToString(dtHose.Rows(0)("NumberOfZeroTransaction")) = "" Then
					txtNumberOfZeroTransaction.Text = Convert.ToString(dtHose.Rows(0)("NumberOfZeroTransaction"))
				Else
					txtNumberOfZeroTransaction.Text = "0"
				End If

				CHK_Activate.Checked = dtHose.Rows(0)("Activated").ToString()
				CHK_ReconfigureLink.Checked = dtHose.Rows(0)("ReconfigureLink").ToString()

				Dim Defective As Boolean = dtHose.Rows(0)("IsDefective").ToString()
				If (Defective = True) Then
					divActivate.Visible = True
				Else
					divActivate.Visible = False
				End If

				txtFSNPMacAddress.Text = dtHose.Rows(0)("FSNPMacAddress")

			Else

			End If

		Catch ex As Exception

			log.Error("Error occurred in BindHoseDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting hose, please try again later."

		End Try
	End Sub

	Private Sub BindSiteTimings(SiteId As Integer)
		Try
			beforeFuelingTimes = ""

			OBJMaster = New MasterBAL()
			Dim dtTiming As DataTable = New DataTable()

			dtTiming = OBJMaster.GetSiteTimings(SiteId)
			If dtTiming IsNot Nothing Then
				For Each dr As DataRow In dtTiming.Rows

					For Each rows As GridViewRow In gv_FuelingTimes.Rows
						If (dr("TimeId") = gv_FuelingTimes.DataKeys(rows.RowIndex).Values("TimeId").ToString()) Then
							TryCast(rows.FindControl("CHK_FuelingTimes"), CheckBox).Checked = True
						End If
					Next

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						beforeFuelingTimes = IIf(beforeFuelingTimes = "", dr("TimeText"), beforeFuelingTimes & ";" & dr("TimeText"))
					End If


				Next
			End If


		Catch ex As Exception

			log.Error("Error occurred in BindSiteTimings Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Site Timings, please try again later."

		End Try

	End Sub

	Private Sub BindSiteDays(SiteId As Integer)
		Try
			beforeFuelingDay = ""

			OBJMaster = New MasterBAL()
			Dim dtSiteDays As DataTable = New DataTable()
			dtSiteDays = OBJMaster.GetSiteDays(SiteId)


			If dtSiteDays IsNot Nothing Then
				For Each dr As DataRow In dtSiteDays.Rows

					For Each rows As GridViewRow In GV_FuelingDays.Rows
						If (dr("Day") = GV_FuelingDays.DataKeys(rows.RowIndex).Values("DayValue").ToString()) Then
							TryCast(rows.FindControl("CHK_FuelingDays"), CheckBox).Checked = True
						End If
					Next

					Dim dayname As String = ""

					Select Case dr("Day")
						Case "0"
							dayname = "All"
						Case "1"
							dayname = "Sunday"
						Case "2",
							dayname = "Monday"
						Case "3"
							dayname = "Tuesday"
						Case "4"
							dayname = "Wednesday"
						Case "5"
							dayname = "Thursday"
						Case "6"
							dayname = "Friday"
						Case "7"
							dayname = "Sunday"

					End Select

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						beforeFuelingDay = IIf(beforeFuelingDay = "", dayname, beforeFuelingDay & ";" & dayname)
					End If

				Next

			End If

		Catch ex As Exception

			log.Error("Error occurred in BindSiteDays Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Site days, please try again later."

		End Try

	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/AllFuelSecureUnits.aspx?Filter=Filter")
	End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try


			Dim CurrentSiteId As Integer = HDF_SiteID.Value
			Dim strConditions As String = ""
			If (Not Session("FuelLINKConditions") Is Nothing And Not Session("FuelLINKConditions") = "") Then
				strConditions = Session("FuelLINKConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and s.CustomerId=" & Session("CustomerId"), strConditions & " and s.CustomerId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim SiteID As Integer = OBJMaster.GetSiteIdByCondition(CurrentSiteId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_SiteID.Value = SiteID
			BindSiteDetails(SiteID)

		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try

			Dim CurrentSiteId As Integer = HDF_SiteID.Value
			Dim strConditions As String = ""
			If (Not Session("FuelLINKConditions") Is Nothing And Not Session("FuelLINKConditions") = "") Then
				strConditions = Session("FuelLINKConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and s.CustomerId=" & Session("CustomerId"), strConditions & " and s.CustomerId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim SiteID As Integer = OBJMaster.GetSiteIdByCondition(CurrentSiteId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_SiteID.Value = SiteID
			BindSiteDetails(SiteID)

		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try

			Dim CurrentSiteId As Integer = HDF_SiteID.Value
			Dim strConditions As String = ""
			If (Not Session("FuelLINKConditions") Is Nothing And Not Session("FuelLINKConditions") = "") Then
				strConditions = Session("FuelLINKConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and s.CustomerId=" & Session("CustomerId"), strConditions & " and s.CustomerId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim SiteID As Integer = OBJMaster.GetSiteIdByCondition(CurrentSiteId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_SiteID.Value = SiteID
			BindSiteDetails(SiteID)

		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try

			Dim CurrentSiteId As Integer = HDF_SiteID.Value
			Dim strConditions As String = ""
			If (Not Session("FuelLINKConditions") Is Nothing And Not Session("FuelLINKConditions") = "") Then
				strConditions = Session("FuelLINKConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and s.CustomerId=" & Session("CustomerId"), strConditions & " and s.CustomerId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim SiteID As Integer = OBJMaster.GetSiteIdByCondition(CurrentSiteId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_SiteID.Value = SiteID
			BindSiteDetails(SiteID)

		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			GetFuelType(ddlCustomer.SelectedValue)
			BindTanks(ddlCustomer.SelectedValue)
		Catch ex As Exception
			log.Error("Error occurred in ddlCustomer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
		End Try
	End Sub

	Protected Sub btnCancelFuelingTimes_Click(sender As Object, e As EventArgs)
		Try

			If HDF_SiteID.Value.ToString() = "" Then
				BindTiming()
			Else
				BindTiming()
				BindSiteTimings(HDF_SiteID.Value)
			End If
		Catch ex As Exception
			log.Error("Error occurred in btnCancelFuelingTimes_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
		End Try
	End Sub

	Protected Sub btnCancelFuelingDays_Click(sender As Object, e As EventArgs)
		Try

			If HDF_SiteID.Value.ToString() = "" Then
				BindDays()
			Else
				BindDays()
				BindSiteDays(HDF_SiteID.Value)
			End If
		Catch ex As Exception
			log.Error("Error occurred in btnCancelFuelingDays_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
		End Try
	End Sub

	Protected Sub DDL_TimeZone_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try
			Dim dtTimeZones As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtTimeZones = OBJMaster.GetTimeZones()
			If (DDL_TimeZone.SelectedValue = 0 Or dtTimeZones Is Nothing Or dtTimeZones.Rows.Count = 0) Then
				LBL_TimeZone.Text = "No time zone selected"
			Else
				dtTimeZones.PrimaryKey = New DataColumn() {dtTimeZones.Columns("TimeZoneId")}
				Dim dr As DataRow = dtTimeZones.Rows.Find(DDL_TimeZone.SelectedValue)
				LBL_TimeZone.Text = dr("SelectedTimeZone")
			End If
		Catch ex As Exception
			log.Error("Error occurred in DDL_TimeZone_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
		End Try
	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function AddFSUnitToAllPersons(SiteID As String, CustomerId As String) As String
		Try

			Dim OBJMasterNew As MasterBAL = New MasterBAL()

			OBJMasterNew.InsertSitePersonnelMapping(Convert.ToInt32(HttpContext.Current.Session("PersonId")), SiteID, CustomerId)

			Return 1
		Catch ex As Exception
			log.Error("Error occurred in AddFSUnitToAllPersons Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Private Sub assignVehicleAndPersonnel(SiteId As Integer)
		Try

			Dim OBJMasterNew As MasterBAL = New MasterBAL()
			If chk_personnelMap.Checked Then
				OBJMasterNew.InsertSitePersonnelMapping(Convert.ToInt32(HttpContext.Current.Session("PersonId")), SiteId, Convert.ToInt32(ddlCustomer.SelectedValue))
			End If

			If chk_vehicleMap.Checked Then
				OBJMasterNew.InsertSiteVehicleMapping(Convert.ToInt32(ddlFuelType.SelectedValue), SiteId, Convert.ToInt32(ddlCustomer.SelectedValue))
			End If

		Catch ex As Exception

			log.Error("Error occurred in assignVehicleAndPersonnel Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while Mapping sites to Personnel and vehicle, please try again later."

		End Try
	End Sub

	Private Function CreateData(SiteID As Integer, IsBefore As Boolean) As String
		Try

			Dim FuelingTimes As String = ""
			Dim FuelingDay As String = ""

			If (IsBefore = True) Then
				FuelingTimes = beforeFuelingTimes
				FuelingDay = beforeFuelingDay
			Else
				FuelingTimes = afterFuelingTimes
				FuelingDay = afterFuelingDay
			End If

            Dim data As String = "SiteID = " & SiteID & " ; " &
                                    "FluidSecure Link Number = " & txtSiteNo.Text.Replace(",", " ") & " ; " &
                                    "FluidSecure Link Current Name (SSID) = " & txtwifissid.Text.Replace(",", " ") & " ; " &
                                    "FluidSecure New Name (will change on next fueling) = " & txt_ReplaceableHoseName.Text.Replace(",", " ") & " ; " &
                                    "Tank Number = " & DDL_Tank.SelectedValue.ToString().Replace(",", " ") & " ; " &
                                    "Product in Tank = " & ddlFuelType.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Authorized Fueling Times = " & FuelingTimes & " ; " &
                                    "Authorized Fueling Days = " & FuelingDay & " ; " &
                                    "Export Code = " & txt_ExportCode.Text.Replace(",", " ") & " ; " &
                                    "Units measured = " & txtUnitsmeasured.Text.Replace(",", " ") & " ; " &
                                    "Pulses = " & txt_Pulses.Text.Replace(",", " ") & " ; " &
                                    "Pulser Ratio = " & txtPulserRatio.Text.Replace(",", " ") & " ; " &
                                    "Time Zone = " & DDL_TimeZone.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Selected time zone = " & LBL_TimeZone.Text.Replace(",", " ") & " ; " &
                                    "Assign this FS link to all Vehicles having the same fueling product = " & IIf(chk_vehicleMap.Checked = True, "Yes", "No") & " ; " &
                                    "Assign this FS link to all Personnel = " & IIf(chk_personnelMap.Checked = True, "Yes", "No") & " ; " &
                                    "Disable Geo Location = " & IIf(CHK_DisableGeoLocation.Checked = True, "Yes", "No") & " ; " &
                                    "Latitude = " & txtLat.Text.Replace(",", " ") & " ; " &
                                    "Longitude = " & txtLong.Text.Replace(",", " ") & " ; " &
                                    "Address = " & txtSiteAddress.Text.Replace(",", " ") & " ; " &
                                    "Pump On Time = " & txtPumpOnTime.Text & " ; " &
                                    "Pump Off Time = " & txtPumpOnTime.Text & " ; " &
                                    "MAC Address = " & txtIpAddress.Text.Replace(",", " ") & " ; " &
                                    "Connected Hub = " & lblConnectedHub.Text.Replace(",", " ") & " ; " &
                                    "Company = " & ddlCustomer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Original Name Of FluidSecure Link = " & TXT_OriginalNameOfFluidSecure.Text.Replace(",", " ") & " ; " &
                                    "Current Firmware Version = " & IIf(Session("RoleName") = "CustomerAdmin", Session("FirmanameValue").ToString().Replace(",", " "), txt_FirmwareVer.Text.Replace(",", " ")) & " ; " &
                                    "Is FluidSecure Link Busy = " & IIf(CHK_IsBusy.Checked = True, "Yes", "No") & " ; " &
                                    "Order Sequence = " & txtDisplayOrder.Text.Replace(",", " ") & " ; " &
                                    "FSNP Mac Address = " & txtFSNPMacAddress.Text.Replace(",", " ") & " ; " &
                                    "Pulser Timing Adjust = " & txtSwitchTimeB.Text.Replace(",", " ") & " ; " &
                                    "Number Of Zero Transaction = " & txtNumberOfZeroTransaction.Text.Replace(",", " ") & " ; " &
                                    "Reconfigure Link = " & CHK_ReconfigureLink.Checked & " ; " &
                                    "Enable Link name = " & IIf(chk_EnableDisableLinkName.Checked = True, "Yes", "No") & " ; " &
                                    "Activate = " & CHK_Activate.Checked & " ; "

            '"Tank Monitor = " & IIf(Chk_TankMonitor.Checked = True, "Yes", "No") & " ; " &
            '"Tank Monitor Number = " & txtTankMonitorNo.Text.Replace(",", " ") & " ; " &

            Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
		Session("SaveAndAddNew") = "True"

		ValidateAndSaveFSLink(True, True)
	End Sub

	Private Sub ValidateAndSaveFSLink(IsSaveAndAddNew As Boolean, IsCheckTransfer As Boolean)

		Dim SiteId As Integer = 0
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			afterFuelingDay = ""
			afterFuelingTimes = ""

			ErrorMessage.Visible = False
			message.Visible = False

            If (Not HDF_SiteID.Value = Nothing And Not HDF_SiteID.Value = "") Then
                SiteId = HDF_SiteID.Value
            End If

            If (Not Session("OldCompanyId") Is Nothing And SiteId <> 0 And IsCheckTransfer = True) Then
                If (Session("OldCompanyId") <> ddlCustomer.SelectedValue) Then

                    lblMessageModelForTransferLink.InnerText = "Do you want to transfer this Fluid Secure Link to " & ddlCustomer.SelectedItem.Text & " company. If yes system will delete this fluid secure link and will add new fluid secure link in " & ddlCustomer.SelectedItem.Text & " company"

                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenModelForTransferLinkBox();", True)
                    Return
                End If
            End If



            Dim HoseId As Integer = 0

			If (Not HDF_HoseId.Value = Nothing And Not HDF_HoseId.Value = "") Then

				HoseId = HDF_HoseId.Value

			End If

			Dim resultVal As Integer = ValidateFields(SiteId, HoseId)

			If (resultVal <> 1) Then
				Return
			End If

			OBJMaster = New MasterBAL()

			'Dim TankMonitorNo As Integer
			'If (txtTankMonitorNo.Text = "") Then
			'	TankMonitorNo = Nothing
			'Else
			'	TankMonitorNo = txtTankMonitorNo.Text
			'End If

			Dim CVPulserRatio As Decimal = txt_Pulses.Text / txtUnitsmeasured.Text

			Dim Lat As String = ""
			If (txtLat.Text <> "") Then
				Lat = Convert.ToDouble(txtLat.Text).ToString("0.00000")
			Else
				Lat = txtLat.Text
			End If
			Dim Lng As String = ""
			If (txtLong.Text <> "") Then

				Lng = Convert.ToDouble(txtLong.Text).ToString("0.00000")

			Else
				Lng = txtLong.Text
			End If

			If txt_FirmwareVer.Text = "" Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Current Firmware Version not enter. Please enter current Firmware Version and try again."
				ErrorMessage.Focus()
				Return
			End If

            Dim DisplayOrder As Integer = 0
			If Not txtDisplayOrder.Text = "" Then
				DisplayOrder = Convert.ToInt32(txtDisplayOrder.Text)
			End If

			Dim NumberOfZeroTransaction As Integer = 0
			If Not txtNumberOfZeroTransaction.Text = "" Then
				NumberOfZeroTransaction = Convert.ToInt32(txtNumberOfZeroTransaction.Text)
			End If
			Dim dtTanks As DataTable = New DataTable()
			dtTanks = ViewState("dtTanks")
            Dim dv As DataView = dtTanks.DefaultView

            Dim dttankInfo As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dttankInfo = OBJMaster.GetTankbyConditions(" and t.CustomerId = " & ddlCustomer.SelectedValue.ToString() & " and TankNumber = '" & DDL_Tank.SelectedValue & "'", Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())
            dv.RowFilter = "TankId=" & dttankInfo.Rows(0)("TankId").ToString()
            dtTanks = dv.ToTable()
			ddlFuelType.SelectedValue = dtTanks.Rows(0)("FuelTypeId")
			Dim result As Integer = 0
            If SiteId = 0 Then
                'If Session("RoleName") = "CustomerAdmin" Then
                result = OBJMaster.SaveUpdateSite(SiteId, 0, Nothing, txtSiteAddress.Text, txt_ExportCode.Text, ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId")), Lat, Lng,
                                                 HoseId, DDL_Tank.SelectedValue, CVPulserRatio, txtwifissid.Text.Trim(), ddlFuelType.SelectedValue, "", txtPumpOffTime.Text, txtPumpOnTime.Text,
                                                 "N", Nothing, txt_ReplaceableHoseName.Text, txtUnitsmeasured.Text, txt_Pulses.Text, DDL_TimeZone.SelectedValue, TXT_OriginalNameOfFluidSecure.Text,
                                                 CHK_DisableGeoLocation.Checked, CHK_IsBusy.Checked, txtIpAddress.Text.ToLower(), txt_FirmwareVer.Text, Convert.ToInt32(txtSwitchTimeB.Text), dttankInfo.Rows(0)("TankId").ToString(), DisplayOrder,
                                                  NumberOfZeroTransaction, CHK_Activate.Checked, txtFSNPMacAddress.Text, CHK_ReconfigureLink.Checked) ', chk_isReplaced.Checked
                'Else
                '    result = OBJMaster.SaveUpdateSite(SiteId, 0, Nothing, txtSiteAddress.Text, txt_ExportCode.Text, ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId")), Lat, Lng,
                '                                 HoseId, txtankNumber.Text, CVPulserRatio, txtwifissid.Text, ddlFuelType.SelectedValue, "", txtPumpOffTime.Text, txtPumpOnTime.Text,
                '                                 IIf(Chk_TankMonitor.Checked = True, "Y", "N"), TankMonitorNo, txt_ReplaceableHoseName.Text, txtUnitsmeasured.Text, txt_Pulses.Text, DDL_TimeZone.SelectedValue, TXT_OriginalNameOfFluidSecure.Text,
                '                                 CHK_DisableGeoLocation.Checked, CHK_IsBusy.Checked, txtIpAddress.Text.ToLower(), txt_FirmwareVer.Text, Convert.ToInt32(txtSwitchTimeB.Text)) ', chk_isReplaced.Checked
                'End If
            Else
                If Session("RoleName") <> "SuperAdmin" Then
                    If (Session("WifiSSIDValue").ToString() <> txtwifissid.Text) And chk_EnableDisableLinkName.Checked = False Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "You Do Not have access To change FluidSecure Link Current Name (SSID)."
                        ErrorMessage.Focus()
                        Return
                    End If
                End If
                If Session("RoleName") = "SuperAdmin" Then
                    result = OBJMaster.SaveUpdateSite(SiteId, txtSiteNo.Text, Nothing, txtSiteAddress.Text, txt_ExportCode.Text, ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId")), Lat, Lng,
                                                 HoseId, DDL_Tank.SelectedValue, CVPulserRatio, txtwifissid.Text.Trim(), ddlFuelType.SelectedValue, "", txtPumpOffTime.Text, txtPumpOnTime.Text,
                                                 "N", Nothing, txt_ReplaceableHoseName.Text, txtUnitsmeasured.Text, txt_Pulses.Text, DDL_TimeZone.SelectedValue, TXT_OriginalNameOfFluidSecure.Text,
                                                 CHK_DisableGeoLocation.Checked, CHK_IsBusy.Checked, txtIpAddress.Text.ToLower(), Session("FirmanameValue").ToString, Convert.ToInt32(txtSwitchTimeB.Text), dttankInfo.Rows(0)("TankId").ToString(), DisplayOrder,
                                                      NumberOfZeroTransaction, CHK_Activate.Checked, txtFSNPMacAddress.Text, CHK_ReconfigureLink.Checked) ', chk_isReplaced.Checked
                Else
                    result = OBJMaster.SaveUpdateSite(SiteId, txtSiteNo.Text, Nothing, txtSiteAddress.Text, txt_ExportCode.Text, ddlCustomer.SelectedValue, Convert.ToInt32(Session("PersonId")), Lat, Lng,
                                                 HoseId, DDL_Tank.SelectedValue, CVPulserRatio, txtwifissid.Text.Trim(), ddlFuelType.SelectedValue, "", txtPumpOffTime.Text, txtPumpOnTime.Text,
                                                 "N", Nothing, txt_ReplaceableHoseName.Text, txtUnitsmeasured.Text, txt_Pulses.Text, DDL_TimeZone.SelectedValue, TXT_OriginalNameOfFluidSecure.Text,
                                                 CHK_DisableGeoLocation.Checked, CHK_IsBusy.Checked, txtIpAddress.Text.ToLower(), txt_FirmwareVer.Text, Convert.ToInt32(txtSwitchTimeB.Text), dttankInfo.Rows(0)("TankId").ToString(), DisplayOrder,
                                                      NumberOfZeroTransaction, CHK_Activate.Checked, txtFSNPMacAddress.Text, CHK_ReconfigureLink.Checked) ', chk_isReplaced.Checked
                End If
			End If

			If result > 0 Then
				Try


					'delete and add timing
					Dim dtTimings As DataTable = New DataTable("dtSiteFuelTimings")
					dtTimings.Columns.Add("SiteID", System.Type.[GetType]("System.Int32"))
					dtTimings.Columns.Add("TimeId", System.Type.[GetType]("System.String"))

					For Each item As GridViewRow In gv_FuelingTimes.Rows

						Dim CHK_FuelingTimes As CheckBox = TryCast(item.FindControl("CHK_FuelingTimes"), CheckBox)
						If (CHK_FuelingTimes.Checked = True) Then
							Dim dr As DataRow = dtTimings.NewRow()
							dr("SiteID") = result
							dr("TimeId") = gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeId").ToString()
							dtTimings.Rows.Add(dr)

							If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
								afterFuelingTimes = IIf(afterFuelingTimes = "", gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeText").ToString(), afterFuelingTimes & ";" & gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeText").ToString())
							End If

						End If
					Next

					OBJMaster.InsertSiteFuelTimings(dtTimings, result)

					'delete and add days
					AddDays(result)

				Catch ex As Exception

				End Try
			End If

			If result > 0 Then
				'If (SiteId > 0) Then
				'    message.Visible = True
				'    message.InnerText = "Record saved"
				'    message.Focus()
				'Else
				'    message.Visible = True
				'    message.InnerText = "Record saved"
				'    message.Focus()
				'    Response.Redirect(String.Format("~/Master/FuelSecureUnit?SiteId={0}&RecordIs=New", result))

				'End If
				message.Visible = True
				message.InnerText = "Record saved"
				message.Focus()

				'Assing FS to Personnel and Vehicle by its FuelType
				If (SiteId > 0) Then
					assignVehicleAndPersonnel(SiteId)

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(SiteId, False)
						CSCommonHelper.WriteLog("Modified", "Fluid Secure Units", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If

					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/FuelSecureUnit"))
					Else
						Response.Redirect(String.Format("~/Master/FuelSecureUnit?SiteId={0}&RecordIs=New", result), False)
					End If
				Else
					assignVehicleAndPersonnel(result)

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result, False)
						CSCommonHelper.WriteLog("Added", "Fluid Secure Units", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If

					If (IsSaveAndAddNew = True) Then

						Response.Redirect(String.Format("~/Master/FuelSecureUnit"))

					Else

						Response.Redirect(String.Format("~/Master/FuelSecureUnit?SiteId={0}&RecordIs=New", result))

					End If
				End If

			Else
				If (SiteId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(SiteId, False)
						CSCommonHelper.WriteLog("Modified", "Fluid Secure Units", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "FluidSecure Link update failed")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "FluidSecure Link update failed, please Try again"
					ErrorMessage.Focus()
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result, False)
						CSCommonHelper.WriteLog("Added", "Fluid Secure Units", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "FluidSecure Link Addition failed")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "FluidSecure Link Addition failed, please Try again."
					ErrorMessage.Focus()
				End If
			End If
			txtwifissid.Focus()

		Catch ex As Exception
			txtwifissid.Focus()
			If (SiteId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(SiteId, False)
					CSCommonHelper.WriteLog("Modified", "Fluid Secure Units", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "FluidSecure Link update failed. Exception Is : " & ex.Message)
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(SiteId, False)
					CSCommonHelper.WriteLog("Added", "Fluid Secure Units", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "FluidSecure Link Addition failed. Exception is : " & ex.Message)
				End If
			End If

			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
			ErrorMessage.Focus()
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
		End Try
	End Sub

	Private Function ValidateFields(SiteId As Integer, HoseId As Integer) As Integer
		Try


			If (txtwifissid.Text = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter fluidsecure link current name."
				txtwifissid.Focus()
				Return 0

			End If

			Dim expression As String = "^[a-zA-Z0-9\-_ ]*$"

			Dim match = Regex.Match(txtwifissid.Text, expression, RegexOptions.IgnoreCase)

			If Not match.Success Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Enter only alpanumeric characters in fluidsecure link current name."
				txtwifissid.Focus()
				Return 0
			End If

			match = Regex.Match(txt_ReplaceableHoseName.Text, expression, RegexOptions.IgnoreCase)

			If Not match.Success Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Enter only alpanumeric characters in fluidsecure new name ."
				txt_ReplaceableHoseName.Focus()
				Return 0
			End If

			If (DDL_Tank.SelectedValue = "0" Or DDL_Tank.SelectedValue = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select tank."
				DDL_Tank.Focus()
				Return 0

			End If

			If (ddlFuelType.SelectedValue = "0" Or ddlFuelType.SelectedValue = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select tank."
				DDL_Tank.Focus()
				Return 0

			End If

			If (txtPumpOnTime.Text = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter pump on time."
				txtPumpOnTime.Focus()

				Return 0

			End If

			Dim resultPumpOnTime As Integer = 0

			If (Not (Integer.TryParse(txtPumpOnTime.Text, resultPumpOnTime))) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Pump on time should be integer number."
				txtPumpOnTime.Focus()

				Return 0

			End If

			If (txtUnitsmeasured.Text = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter units measured."
				txtUnitsmeasured.Focus()
				Return 0

			End If

			Dim resultUnitsmeasured As Integer = 0

			If (Not (Integer.TryParse(txtUnitsmeasured.Text, resultUnitsmeasured))) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter Units measured in number format."
				txtUnitsmeasured.Focus()
				Return 0

			End If

			If (txt_Pulses.Text = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter pulses."
				txt_Pulses.Focus()
				Return 0

			End If

			Dim resultPulses As Integer = 0

			If (Not (Integer.TryParse(txt_Pulses.Text, resultPulses))) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter pulses in number format."
				txt_Pulses.Focus()

				Return 0

			End If

			If (txtSwitchTimeB.Text = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter Pulser Timing Adjust."
				txtSwitchTimeB.Focus()

				Return 0

			End If

			Dim resultSwitchTimeB As Integer = 0

			If (Not (Integer.TryParse(txtSwitchTimeB.Text, resultSwitchTimeB))) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Pulser Timing Adjust should be integer number."
				txtSwitchTimeB.Focus()
				Return 0

			End If

			If (DDL_TimeZone.SelectedValue = "0" Or DDL_TimeZone.SelectedValue = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select timezone."
				DDL_TimeZone.Focus()
				Return 0

			End If

			Dim resultNumberOfZeroTransaction As Integer = 0

			If (Not (Integer.TryParse(txtNumberOfZeroTransaction.Text, resultNumberOfZeroTransaction))) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Number Of Zero Transaction should be integer number."
				txtNumberOfZeroTransaction.Focus()
				Return 0

			End If


			If (txtPumpOffTime.Text = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter Pump Off Time."
				txtPumpOffTime.Focus()

				Return 0

			End If

			Dim resultPumpOffTime As Integer = 0

			If (Not (Integer.TryParse(txtPumpOffTime.Text, resultPumpOffTime))) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Pump Off Time should be integer number."
				txtPumpOffTime.Focus()
				Return 0

			End If

			If (ddlCustomer.SelectedValue = "0" Or ddlCustomer.SelectedValue = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select Company."
				ddlCustomer.Focus()
				Return 0

			End If

			If (TXT_OriginalNameOfFluidSecure.Text = "") Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please enter original name of fluidsecure link."
				TXT_OriginalNameOfFluidSecure.Focus()
				Return 0

			End If

			expression = "^[a-zA-Z0-9\-_ ]*$"

			match = Regex.Match(TXT_OriginalNameOfFluidSecure.Text, expression, RegexOptions.IgnoreCase)

			If Not match.Success Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Enter only alpanumeric characters in original name of fluidsecure link."
				TXT_OriginalNameOfFluidSecure.Focus()
				Return 0
			End If

			Dim resultDisplayOrder As Integer = 0

			If (txtDisplayOrder.Text <> "" And Not (Integer.TryParse(txtDisplayOrder.Text, resultDisplayOrder))) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Display order should be integer number."
				txtDisplayOrder.Focus()
				Return 0

			End If

			If (CHK_DisableGeoLocation.Checked = False And (txtLat.Text = "" Or txtLong.Text = "" Or txtSiteAddress.Text = "")) Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select location."

				Return 0
			End If

			Dim CheckIdExists As Boolean = False
			OBJMaster = New MasterBAL()
            Dim result As Integer = 0
            If SiteId <> 0 Then
                CheckIdExists = OBJMaster.SiteIsExists(SiteId, txtSiteNo.Text, Convert.ToInt32(ddlCustomer.SelectedValue))
                If CheckIdExists = True Then


                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "FluidSecure Link Number Already Exists."

                    Return 0

                End If
            End If




            CheckIdExists = False

			OBJMaster = New MasterBAL()
			result = 0

			result = OBJMaster.HoseIsExists(SiteId, "", txtwifissid.Text.Trim(), ddlCustomer.SelectedValue, txt_ReplaceableHoseName.Text, txtFSNPMacAddress.Text)

			If result = -1 Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Hose Number Already Exists."
				ErrorMessage.Focus()
				Return 0

			ElseIf result = -2 Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "FluidSecure Link Current Name Already Exists."
				txtwifissid.Focus()

				Return 0

			ElseIf result = -3 Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "FluidSecure Link New Name Already Exists. Please try another FluidSecure Link New Name."
				txt_ReplaceableHoseName.Focus()
				Return 0
			ElseIf result = -4 Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "FSNP Mac Address Already Exists. Please try another FSNP Mac Address."
				txtFSNPMacAddress.Focus()
				Return 0

			End If

			Return 1

		Catch ex As Exception
			log.Error("Error occurred in ValidateFields Exception is :" + ex.Message)
			Return 0
		End Try

	End Function

	Protected Sub DDL_Tank_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try
			Dim dtTanks As DataTable = New DataTable()
			dtTanks = ViewState("dtTanks")
			Dim dv As DataView = dtTanks.DefaultView
            Dim dttankInfo As DataTable = New DataTable()
            OBJMaster = New MasterBAL()
            dttankInfo = OBJMaster.GetTankbyConditions(" and t.CustomerId = " & ddlCustomer.SelectedValue.ToString() & " and TankNumber = '" & DDL_Tank.SelectedValue & "'", Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())
            dv.RowFilter = "TankId=" & dttankInfo.Rows(0)("TankId").ToString()
            dtTanks = dv.ToTable()
			ddlFuelType.SelectedValue = dtTanks.Rows(0)("FuelTypeId")
		Catch ex As Exception
			log.Error("Error occurred in DDL_Tank_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
		End Try
	End Sub

	<WebMethod()>
	Public Shared Function GetTimeZoneName(Url As String) As String
		Dim timeZoneName As String = ""
		Dim client As New System.Net.WebClient()
		Dim response As Byte() = client.DownloadData(Url)
		Dim content As String = System.Text.Encoding.ASCII.GetString(response)
		Dim obj As JObject = JObject.Parse(content)
		Try
			timeZoneName = obj.SelectToken("timeZoneName").ToString()
			Dim TimeZoneId As Integer = 0
			Dim SelectedText As String = ""
			If (HttpContext.Current.Session("dtTimeZones") IsNot Nothing) Then

				Dim dtTimeZones As DataTable = New DataTable()
				dtTimeZones = HttpContext.Current.Session("dtTimeZones")

				Dim dvTimeZones As DataView = dtTimeZones.DefaultView
				dvTimeZones.RowFilter = "TimeZoneName='" & timeZoneName & "'"
				dtTimeZones = dvTimeZones.ToTable()
				If (dtTimeZones.Rows.Count > 0) Then
					TimeZoneId = dtTimeZones.Rows(0)("TimeZoneId")
					SelectedText = dtTimeZones.Rows(0)("SelectedTimeZone")
					Return TimeZoneId & "," & SelectedText
				End If
			End If

			Return "0,"
		Catch ex As Exception
			'timeZoneName = ex.Message
		End Try
		Return "0,"
	End Function

	Protected Sub btnlblMessageModelForTransferLinkYes_Click(sender As Object, e As EventArgs)
		Dim SaveAndAddNew As Boolean = False

		If (Not Session("SaveAndAddNew") Is Nothing) Then
			If (Session("SaveAndAddNew") = "True") Then
				SaveAndAddNew = True
			End If
		End If
		Dim result As Integer = OBJMaster.DeleteSite(HDF_SiteID.Value, Convert.ToInt32(HttpContext.Current.Session("PersonId")))

		HDF_SiteID.Value = ""
		HDF_HoseId.Value = ""
		ValidateAndSaveFSLink(SaveAndAddNew, False)

	End Sub

    Protected Sub chk_EnableDisableLinkName_CheckedChanged(sender As Object, e As EventArgs)
        Try
            If Session("RoleName") = "SuperAdmin" Then
                txtwifissid.Enabled = True
            Else
                If chk_EnableDisableLinkName.Checked Then
                    txtwifissid.Enabled = True
                Else
                    txtwifissid.Enabled = False
                End If

            End If
        Catch ex As Exception
            log.Error("Error occurred in chk_EnableDisableLinkName_CheckedChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
End Class