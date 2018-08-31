Imports log4net
Imports log4net.Config

Public Class Tank
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Tank))

	Dim OBJMaster As MasterBAL = New MasterBAL()
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			'ErrorMessage.Visible = False
			'message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then

					If (Session("RoleName") = "SuperAdmin") Then
						divConstantA.Visible = True
						divConstantB.Visible = True
						divConstantC.Visible = True
						divConstantD.Visible = True
					Else
						divConstantA.Visible = False
						divConstantB.Visible = False
						divConstantC.Visible = False
						divConstantD.Visible = False
					End If

					GetCustomers(Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())
					If (Not Request.QueryString("TankId") = Nothing And Not Request.QueryString("TankId") = "") Then
						txtTankIdHide.Text = Request.QueryString("TankId")
						BindTankDetails(Request.QueryString("TankId"))
						'btnAdd.Visible = True
						btnFirst.Visible = True
						btnNext.Visible = True
						btnprevious.Visible = True
						btnLast.Visible = True
						lblHeader.Text = "Edit Tank Information"
						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message.Text = "Record saved"
						End If
					Else
						btnFirst.Visible = False
						btnNext.Visible = False
						btnprevious.Visible = False
						btnLast.Visible = False
						lblof.Visible = False

						lblHeader.Text = "Add Tank Information"
					End If

					txtTankNo.Focus()
				End If
			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = IIf(ErrorMessage.Text <> "", "", "Error occurred while loading details, please try again later.")

		End Try
	End Sub

	Private Sub BindTankDetails(TankId As Integer)
		Try


			OBJMaster = New MasterBAL()
			Dim dtTank As DataTable = New DataTable()
			dtTank = OBJMaster.GetTankbyId(TankId)
			Dim cnt As Integer = 0
			If (dtTank.Rows.Count > 0) Then

				If (Not Session("RoleName") = "SuperAdmin") Then

					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtTank.Rows(0)("CustomerId").ToString()) Then

                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
					End If

				End If

				txtTankNo.Text = dtTank.Rows(0)("TankNumber").ToString()
				txtTankName.Text = dtTank.Rows(0)("TankName").ToString()
				txtAddress.Text = dtTank.Rows(0)("TankAddress").ToString()
				txtExportCode.Text = dtTank.Rows(0)("ExportCode").ToString()
				txtRefillNotice.Text = dtTank.Rows(0)("RefillNotice").ToString()
				txtPROBEMacAddress.Text = dtTank.Rows(0)("PROBEMacAddress").ToString()

				DDL_Customer.SelectedValue = IIf(dtTank.Rows(0)("CustomerId").ToString() = "", 0, dtTank.Rows(0)("CustomerId").ToString())
				DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
				GetFuelType(DDL_Customer.SelectedValue)
				GetTankChart(DDL_Customer.SelectedValue)
				ddlFuelType.SelectedValue = IIf(dtTank.Rows(0)("FuelTypeId").ToString() = "", 0, dtTank.Rows(0)("FuelTypeId").ToString())
				ddlTankChart.SelectedValue = IIf(dtTank.Rows(0)("TankChartId").ToString() = "", 0, dtTank.Rows(0)("TankChartId").ToString())

				Chk_TankMonitor.Checked = dtTank.Rows(0)("TankMonitor")
				txtTankMonitorNo.Text = IIf(dtTank.Rows(0)("TankMonitorNumber").ToString() = "0", "", dtTank.Rows(0)("TankMonitorNumber").ToString())

				'txtProbeRatio.Text = dtTank.Rows(0)("ProbeRatio").ToString()


				txtConstantA.Text = dtTank.Rows(0)("ConstantA").ToString()
				txtConstantB.Text = dtTank.Rows(0)("ConstantB").ToString()
				txtConstantC.Text = dtTank.Rows(0)("ConstantC").ToString()
				txtConstantD.Text = dtTank.Rows(0)("ConstantD").ToString()


				Dim strConditions As String = ""
				If (Not Session("TankConditions") Is Nothing And Not Session("TankConditions") = "") Then
					strConditions = Session("TankConditions")
				Else
					If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
						strConditions = IIf(strConditions = "", " and dtTanks.CustomerId=" & Session("CustomerId"), strConditions & " and dtTanks.CustomerId=" & Session("CustomerId"))
					End If
				End If

				OBJMaster = New MasterBAL()

				HDF_TotalTank.Value = OBJMaster.GetTankIdByCondition(TankId, False, False, False, False, True, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)

				OBJMaster = New MasterBAL()
				Dim dtAllTank As DataTable = New DataTable()
				If strConditions.Contains("CustomerId") Then strConditions = strConditions.Replace("CustomerId", "t.CustomerId")
				If strConditions.Contains("ExportCode") Then strConditions = strConditions.Replace("ExportCode", "t.ExportCode")
				If strConditions.Contains("TankChartId") Then strConditions = strConditions.Replace("CustomerId", "t.TankChartId")
				dtAllTank = OBJMaster.GetTankbyConditions(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString())
				dtAllTank.PrimaryKey = New DataColumn() {dtAllTank.Columns(0)}
				Dim dr As DataRow = dtAllTank.Rows.Find(TankId)
				If Not IsDBNull(dr) Then

					cnt = dtAllTank.Rows.IndexOf(dr) + 1

				End If
				If (cnt >= HDF_TotalTank.Value) Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				ElseIf (cnt <= 1) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt > 1 And cnt < HDF_TotalTank.Value) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
				lblof.Text = cnt & " of " & HDF_TotalTank.Value.ToString()

			Else
				ErrorMessage.Visible = True
				ErrorMessage.Text = "Data Not found. Please try again after some time."
			End If

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateBeforeData(dtTank)
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindTankDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting Tank data, please try again later."
		Finally
			txtTankNo.Focus()
		End Try

	End Sub

	Private Sub GetCustomers(personid As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCustomer As DataTable = New DataTable()
			dtCustomer = OBJMaster.GetCustomerDetailsByPersonID(personid, RoleId, Session("CustomerId").ToString())
			DDL_Customer.DataSource = dtCustomer
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

			If (Not Session("RoleName") = "SuperAdmin") Then
				DDL_Customer.SelectedIndex = 1
				DDL_Customer.Enabled = False
				DDL_Customer.Visible = False
				divCompany.Visible = False
			End If

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				DDL_Customer.SelectedIndex = 1

			End If
			DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
		Catch ex As Exception

			log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting customers, please try again later."

		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		ValidateAndSaveTank(False)
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/AllTanks?Filter=Filter")
	End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try
			Dim strConditions As String = ""
			If (Not Session("TankConditions") Is Nothing) Then
				strConditions = Session("TankConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and Tanks.CustomerId=" & Session("CustomerId"), strConditions & " and Tanks.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentTankId As Integer = txtTankIdHide.Text

			OBJMaster = New MasterBAL()
			Dim TankId As Integer = OBJMaster.GetTankIdByCondition(CurrentTankId, True, False, False, False, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtTankIdHide.Text = TankId
			BindTankDetails(TankId)

		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("TankConditions") Is Nothing) Then
				strConditions = Session("TankConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and Tanks.CustomerId=" & Session("CustomerId"), strConditions & " and Tanks.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentTankId As Integer = txtTankIdHide.Text

			OBJMaster = New MasterBAL()
			Dim TankId As Integer = OBJMaster.GetTankIdByCondition(CurrentTankId, False, False, False, True, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtTankIdHide.Text = TankId
			BindTankDetails(TankId)

		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
		ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("TankConditions") Is Nothing) Then
				strConditions = Session("TankConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and Tanks.CustomerId=" & Session("CustomerId"), strConditions & " and Tanks.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentTankId As Integer = txtTankIdHide.Text

			OBJMaster = New MasterBAL()
			Dim TankId As Integer = OBJMaster.GetTankIdByCondition(CurrentTankId, False, False, True, False, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtTankIdHide.Text = TankId
			BindTankDetails(TankId)

		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
		ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("TankConditions") Is Nothing) Then
				strConditions = Session("TankConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and Tanks.CustomerId=" & Session("CustomerId"), strConditions & " and Tanks.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentTankId As Integer = txtTankIdHide.Text

			OBJMaster = New MasterBAL()
			Dim TankId As Integer = OBJMaster.GetTankIdByCondition(CurrentTankId, False, True, False, False, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtTankIdHide.Text = TankId
			BindTankDetails(TankId)

		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
		ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Private Function CreateAfterData() As String
		Try

			Dim data As String = "Tank Number = " & txtTankNo.Text.Replace(",", " ") & " ; " &
									"Tank Name = " & txtTankName.Text.Replace(",", " ") & " ; " &
									"PRODUCT = " & ddlFuelType.SelectedItem.Text.Replace(",", " ") & " ; " &
									"ADDRESS = " & txtAddress.Text.Replace(",", " ") & " ; " &
									"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
									"REFILL_NOT = " & txtRefillNotice.Text.Replace(",", " ") & " ; " &
									"PROBEMacAddress = " & txtPROBEMacAddress.Text.Replace(",", " ") & " ; " &
									"TANK_CHART = " & ddlTankChart.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Constant A = " & IIf(txtConstantA.Text = "", "0.0001298", txtConstantA.Text).ToString().Replace(",", " ") & " ; " &
									"Constant B = " & IIf(txtConstantB.Text = "", "0.90696,", txtConstantB.Text).ToString().Replace(",", " ") & " ; " &
									"Constant C = " & IIf(txtConstantC.Text = "", "-0.0777989,", txtConstantC.Text).ToString().Replace(",", " ") & " ; " &
									"Constant D = " & IIf(txtConstantD.Text = "", "-4.458", txtConstantD.Text).ToString().Replace(",", " ") & " ; "
			'"Probe Ratio = " & txtProbeRatio.Text.Replace(",", " ") & " ; " &

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateAfterData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Private Function CreateBeforeData(dtTank As DataTable) As String
		Try

			Dim data As String = "TankId = " & dtTank.Rows(0)("TankId").ToString() & " ; " &
								  "Tank Number = " & dtTank.Rows(0)("TankNumber").ToString().Replace(",", " ") & " ; " &
								  "Tank Name = " & dtTank.Rows(0)("TankName").ToString().Replace(",", " ") & " ; " &
								  "PRODUCT = " & dtTank.Rows(0)("FuelType").ToString().Replace(",", " ") & " ; " &
								  "ADDRESS = " & dtTank.Rows(0)("TankAddress").ToString().Replace(",", " ") & " ; " &
								  "REFILL_NOT = " & dtTank.Rows(0)("RefillNotice").ToString().Replace(",", " ") & " ; " &
								  "Company = " & dtTank.Rows(0)("CustomerName").ToString().Replace(",", " ") & " ; " &
								  "Export Code = " & dtTank.Rows(0)("ExportCode").ToString().Replace(",", " & ") & " ; " &
								  "PROBEMacAddress = " & dtTank.Rows(0)("PROBEMacAddress").ToString().Replace(",", " ") & " ; " &
								  "Tank Monitor = " & IIf(Chk_TankMonitor.Checked = True, "Yes", "No") & " ; " &
								  "Tank Monitor Number = " & txtTankMonitorNo.Text.Replace(",", " ") & " ; " &
								  "TANK_CHART = " & dtTank.Rows(0)("TankChartName").ToString().Replace(",", " ") & " ; " &
								  "Constant A = " & dtTank.Rows(0)("ConstantA").ToString().Replace(",", " ") & " ; " &
								  "Constant B = " & dtTank.Rows(0)("ConstantB").ToString().Replace(",", " ") & " ; " &
								  "Constant C = " & dtTank.Rows(0)("ConstantC").ToString().Replace(",", " ") & " ; " &
								  "Constant D = " & dtTank.Rows(0)("ConstantD").ToString().Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateBeforeData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
		ValidateAndSaveTank(True)
	End Sub

	Private Sub ValidateAndSaveTank(IsSaveAndAddNew As Boolean)

		Dim TankId As Integer = 0
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			ErrorMessage.Visible = False
			message.Visible = False


			If (Not txtTankIdHide.Text = Nothing And Not txtTankIdHide.Text = "") Then

				TankId = txtTankIdHide.Text

			End If

			Dim CheckIdExists As Integer = 0
			OBJMaster = New MasterBAL()
			CheckIdExists = OBJMaster.TankIDExists(txtTankNo.Text, TankId, Convert.ToInt32(DDL_Customer.SelectedValue), txtTankName.Text)
			Dim result As Integer = 0

			If CheckIdExists = -1 Then

				ErrorMessage.Visible = True
				ErrorMessage.Text = "Tank Number Already Exists."
				txtTankNo.Focus()
				Return

			ElseIf CheckIdExists = -2 Then

				ErrorMessage.Visible = True
				ErrorMessage.Text = "Tank Name Already Exists."
                txtTankName.Focus()
                Return
            End If

            If txtTankNo.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Tank Number and try again."
                txtTankNo.Focus()
                Return
            End If

            If txtTankName.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Tank Name and try again."
                txtTankName.Focus()
                Return
            End If

            If ddlFuelType.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select Product and try again."
                DDL_Customer.Focus()
                Return
            End If

            If DDL_Customer.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select Company and try again."
                DDL_Customer.Focus()
                Return
            End If

            Dim resultInteger As Integer = 0

            If (txtRefillNotice.Text <> "" And Not (Decimal.TryParse(txtRefillNotice.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Refill Notice as Integer and try again."
                txtRefillNotice.Focus()
                Return
            End If

            If (txtTankMonitorNo.Text <> "" And Not (Decimal.TryParse(txtTankMonitorNo.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Tank monitor number as Integer and try again."
                txtTankMonitorNo.Focus()
                Return
            End If


            Dim resultDecimal As Decimal = 0

            If (txtConstantA.Text <> "" And Not (Decimal.TryParse(txtConstantA.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Constant A as decimal and try again."
                txtConstantA.Focus()
                Return
            End If

            If (txtConstantB.Text <> "" And Not (Decimal.TryParse(txtConstantB.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Constant B as decimal and try again."
                txtConstantB.Focus()
                Return
            End If

            If (txtConstantC.Text <> "" And Not (Decimal.TryParse(txtConstantC.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Constant C as decimal and try again."
                txtConstantC.Focus()
                Return
            End If

            If (txtConstantD.Text <> "" And Not (Decimal.TryParse(txtConstantD.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Constant D as decimal and try again."
                txtConstantD.Focus()
                Return
            End If



            OBJMaster = New MasterBAL()

			If txtPROBEMacAddress.Text <> "" Then
				Try
					Dim strCondition As String = "  and t.PROBEMacAddress =  '" & txtPROBEMacAddress.Text & "' and t.CustomerId = " & DDL_Customer.SelectedValue.ToString() & " and t.TankId <> " & TankId.ToString()
					Dim dtData As DataTable = OBJMaster.GetTankbyConditions(strCondition, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())
					If dtData.Rows.Count > 0 Then
						ErrorMessage.Visible = True
						ErrorMessage.Text = "PROBE Mac Address already exists."
						txtPROBEMacAddress.Focus()
						Return
					End If
				Catch ex As Exception

				End Try
			End If

			OBJMaster = New MasterBAL()

			If (Chk_TankMonitor.Checked = True And ddlTankChart.SelectedValue = 0) Then
				ErrorMessage.Visible = True
				ErrorMessage.Text = "Please select Tank Chart."
				ddlTankChart.Focus()
				Return
			End If

			Dim TankMonitorNo As Integer
			If (txtTankMonitorNo.Text = "") Then
				TankMonitorNo = Nothing
			Else
				TankMonitorNo = txtTankMonitorNo.Text
			End If


			'Dim ProbeRatio As Decimal = 1.0
			'If txtProbeRatio.Text <> "" Then
			'    ProbeRatio = Convert.ToDecimal(txtProbeRatio.Text)
			'End If

			result = OBJMaster.SaveUpdateTank(TankId, txtTankName.Text, txtTankNo.Text, txtAddress.Text, DDL_Customer.SelectedValue, txtExportCode.Text, Convert.ToInt32(ddlFuelType.SelectedValue),
											  IIf(txtRefillNotice.Text <> "", txtRefillNotice.Text, "-1"), txtPROBEMacAddress.Text, Convert.ToInt32(ddlTankChart.SelectedValue),
											  Convert.ToInt32(Session("PersonId")), Chk_TankMonitor.Checked, TankMonitorNo, IIf(txtConstantA.Text = "", "0.0001298", txtConstantA.Text),
											   IIf(txtConstantB.Text = "", "0.90696,", txtConstantB.Text), IIf(txtConstantC.Text = "", "-0.0777989,", txtConstantC.Text),
											  IIf(txtConstantD.Text = "", "-4.458", txtConstantD.Text))


			If result > 0 Then

				If (TankId > 0) Then
					message.Visible = True
					message.Text = "Record saved"
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "TankId = " & TankId & " ; " & writtenData

						CSCommonHelper.WriteLog("Modified", "Tank", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Tank"))
					End If
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "TankId = " & result & " ; " & writtenData

						CSCommonHelper.WriteLog("Added", "Tank", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Tank"))
					Else
						Response.Redirect(String.Format("~/Master/Tank?TankId={0}&RecordIs=New", result))
					End If
				End If

			Else
				If (TankId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "TankId = " & TankId & " ; " & writtenData

						CSCommonHelper.WriteLog("Modified", "Tank", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank update failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.Text = "Tank update failed, please try again"
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "TankId = " & result & " ; " & writtenData

						CSCommonHelper.WriteLog("Added", "Tank", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank Addition failed")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.Text = "Tank Addition failed, please try again"
				End If

			End If
			txtTankNo.Focus()
		Catch ex As Exception
			txtTankNo.Focus()
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while saving record, please try again later."
			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
			If (TankId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateAfterData()
					writtenData = "TankId = " & TankId & " ; " & writtenData

					CSCommonHelper.WriteLog("Modified", "Tank", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank update failed. Exception is : " & ex.Message)
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateAfterData()
					writtenData = "TankId = " & 0 & " ; " & writtenData

					CSCommonHelper.WriteLog("Added", "Tank", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank Addition failed. Exception is : " & ex.Message)
				End If
			End If
		Finally

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
			ErrorMessage.Text = "Error occurred while getting fuel types, please try again later."

		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			GetFuelType(DDL_Customer.SelectedValue)
			GetTankChart(DDL_Customer.SelectedValue)

		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    Private Sub GetTankChart(CustomerId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtTankChart As DataTable = New DataTable()

            dtTankChart = OBJMaster.GetTankChartsByCondition("  and TC.CompanyId =  " & CustomerId, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())

            ddlTankChart.DataSource = dtTankChart
            ddlTankChart.DataTextField = "TankChartName"
            ddlTankChart.DataValueField = "TankChartId"
            ddlTankChart.DataBind()
            ddlTankChart.Items.Insert(0, New ListItem("Select Tank Chart Type", "0"))

        Catch ex As Exception

            log.Error("Error occurred in GetTankChart Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting Tank Chart, please try again later."

        End Try
    End Sub

End Class