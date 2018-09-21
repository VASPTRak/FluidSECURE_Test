Imports log4net
Imports log4net.Config

Public Class DeletedTransaction
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(DeletedTransaction))

	Dim OBJMaster As MasterBAL
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			ManuallyEditMessage.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied 
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then

					BindTransactionStatus()

					txtTransactionDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
					txtTransactionTime.Text = DateTime.Now.ToString("hh:mm tt")


					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
					If (Not Request.QueryString("TransactionId") = Nothing And Not Request.QueryString("TransactionId") = "") Then

						PreviousOdometer.Visible = True
                        divPrevHours.Visible = True
                        hdfTransactionId.Value = Request.QueryString("TransactionId")
						BindTransactionsDetails(Request.QueryString("TransactionId"))
						btnFirst.Visible = True
						btnNext.Visible = True
						btnprevious.Visible = True
						btnLast.Visible = True
						lblHeader.Text = "Edit Deleted Transaction Information"
						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message.InnerText = "Record saved"
						End If

					Else

						Response.Redirect("~/Master/AllDeletedTransactions.aspx")

						'PreviousOdometer.Visible = False
						'btnFirst.Visible = False
						'btnNext.Visible = False
						'btnprevious.Visible = False
						'btnLast.Visible = False
						'lblof.Visible = False
						'lblHeader.Text = "Add Transaction Information"
					End If

					DDL_Customer.Focus()
				Else

					txtTransactionDate.Text = Request.Form(txtTransactionDate.UniqueID)
					txtTransactionTime.Text = Request.Form(txtTransactionTime.UniqueID)

				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Private Sub BindCustomer(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

			DDL_Customer.DataSource = dtCust
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
		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Private Sub BindDepartment(CustomerId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtDept As DataTable = New DataTable()
			If CustomerId <> 0 Then
				dtDept = OBJMaster.GetDepartmentsByCustomerId(CustomerId)
				DDL_Dept.DataSource = dtDept
			Else
				DDL_Dept.DataSource = dtDept
			End If
			DDL_Dept.DataTextField = "NAME"
			DDL_Dept.DataValueField = "DeptId"

			DDL_Dept.DataBind()
			DDL_Dept.Items.Insert(0, New ListItem("Select Department", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindDepartment Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting departments, please try again later."

		End Try
	End Sub

	Private Sub BindAllVehicles(CustomerId As Integer)
		Try
			Dim dtVehicle As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtVehicle = OBJMaster.GetVehicleByCondition(" and c.CustomerId = " & CustomerId, Session("PersonId").ToString(), Session("RoleId").ToString())

			gv_Vehicles.DataSource = dtVehicle
			gv_Vehicles.DataBind()


			If CustomerId = 0 Then
				lblVehicleMessage.Text = "Please select Company and then select vehicles."
				lblVehicleMessage.Visible = True
				gv_Vehicles.Visible = False
			ElseIf CustomerId <> 0 And gv_Vehicles.Rows.Count <> 0 Then

				lblVehicleMessage.Visible = False
				gv_Vehicles.Visible = True
			ElseIf CustomerId <> 0 And gv_Vehicles.Rows.Count = 0 Then
				lblVehicleMessage.Text = "Vehicles not found for selected Company."
				lblVehicleMessage.Visible = True
				gv_Vehicles.Visible = False
			End If

			'DDL_Vehicle.DataSource = dtVehicle
			'DDL_Vehicle.DataValueField = "VehicleId"
			'DDL_Vehicle.DataTextField = "VehicleNumber"
			'DDL_Vehicle.DataBind()

			'DDL_Vehicle.Items.Insert(0, New ListItem("Select Vehicle", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

		End Try
	End Sub

	Private Sub BindFuelTypes(CompanyId As Integer)
		Try

			Dim dtFuelTpes As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtFuelTpes = OBJMaster.GetFuelDetails(CompanyId)

			DDL_Fuel.DataSource = dtFuelTpes
			DDL_Fuel.DataValueField = "FuelTypeId"
			DDL_Fuel.DataTextField = "FuelType"
			DDL_Fuel.DataBind()

			DDL_Fuel.Items.Insert(0, New ListItem("Select Product", "0"))

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting fuel types, please try again later."

			log.Error("Error occurred in BindFuelTypes Exception is :" + ex.Message)

		End Try
	End Sub

	Private Sub BindSites(CustomerId As Integer)
		Try


			Dim dtSites As DataTable = New DataTable()
			OBJMaster = New MasterBAL()

			dtSites = OBJMaster.GetSiteByCondition(" and c.CustomerId = " & CustomerId, Session("PersonId").ToString(), Session("RoleId").ToString(), False)

			DDL_Site.DataSource = dtSites
			DDL_Site.DataValueField = "SiteId"
			DDL_Site.DataTextField = "WifiSSId"
			DDL_Site.DataBind()

			DDL_Site.Items.Insert(0, New ListItem("Select FluidSecure Link", "0"))

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting sites, please try again later."

			log.Error("Error occurred in BindSites Exception is :" + ex.Message)

		End Try
	End Sub

	Private Sub BindAllPersonnels(CustomerId As Integer)
		Try
			Dim dtPersonnel As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(ANU.IsFluidSecureHub,0) = 0 and cust.CustomerId = " & CustomerId, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			gv_Persons.DataSource = dtPersonnel
			gv_Persons.DataBind()


			If CustomerId = 0 Then
				lblPersonMessage.Text = "Please select Company and then select persons."
				lblPersonMessage.Visible = True
				gv_Persons.Visible = False
			ElseIf CustomerId <> 0 And gv_Persons.Rows.Count <> 0 Then

				lblPersonMessage.Visible = False
				gv_Persons.Visible = True
			ElseIf CustomerId <> 0 And gv_Persons.Rows.Count = 0 Then
				lblPersonMessage.Text = "Persons not found for selected Company."
				lblPersonMessage.Visible = True
				gv_Persons.Visible = False
			End If


			'DDL_Personnel.DataSource = dtPersonnel
			'DDL_Personnel.DataValueField = "PersonId"
			'DDL_Personnel.DataTextField = "Person"
			'DDL_Personnel.DataBind()

			'DDL_Personnel.Items.Insert(0, New ListItem("Select Person", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

		End Try
	End Sub

	Private Sub BindTransactionsDetails(TransactionId As Integer)
		Try


			OBJMaster = New MasterBAL()
			Dim dtTransaction As DataTable = New DataTable()
			dtTransaction = OBJMaster.GetTransactionById(TransactionId, True)
			Dim cnt As Integer = 0
			If (dtTransaction.Rows.Count > 0) Then
				Dim TransactionStatus As String = dtTransaction.Rows(0)("TranStatus").ToString()
				If TransactionStatus <> ConfigurationManager.AppSettings("CompletedText").ToString() And TransactionStatus <> ConfigurationManager.AppSettings("MissedText").ToString() Then

					Response.Redirect("~/Master/AllDeletedTransactions.aspx?Filter=Filter")

					Return
				End If

				If (Not Session("RoleName") = "SuperAdmin") Then

					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtTransaction.Rows(0)("CustomerId").ToString()) Then

						Response.Redirect("/home")

						Return
					End If

				End If




				txtCurrentOdometer.Text = dtTransaction.Rows(0)("CurrentOdometer")

				txtFuelQuantity.Text = dtTransaction.Rows(0)("FuelQuantity")
				txtPreviousOdometer.Text = dtTransaction.Rows(0)("PreviousOdometer")
                txtPreviousHours.Text = dtTransaction.Rows(0)("PreviousHours").ToString()
                txtTransactionDate.Text = Convert.ToDateTime(dtTransaction.Rows(0)("TransactionDateTime").ToString()).ToString("MM/dd/yyyy")
				txtTransactionTime.Text = Convert.ToDateTime(dtTransaction.Rows(0)("TransactionDateTime").ToString()).ToString("hh:mm tt")
				DDL_Customer.SelectedValue = dtTransaction.Rows(0)("CustomerId")
				DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

				DDL_Dept.SelectedValue = dtTransaction.Rows(0)("DeptId")

				DDL_Fuel.SelectedValue = dtTransaction.Rows(0)("FuelTypeId")
				DDL_Site.SelectedValue = dtTransaction.Rows(0)("SiteId")
				'DDL_Personnel.SelectedValue = dtTransaction.Rows(0)("PersonID")
				'DDL_Vehicle.SelectedValue = dtTransaction.Rows(0)("VehicleId")

				For Each rows As GridViewRow In gv_Vehicles.Rows
					If (dtTransaction.Rows(0)("VehicleId") = gv_Vehicles.DataKeys(rows.RowIndex).Values("VehicleId").ToString()) Then
						TryCast(rows.FindControl("RDB_Vehicle"), RadioButton).Checked = True
					End If
				Next

				lblVehicleName.Text = dtTransaction.Rows(0)("VehicleName")

				HDF_VehicleId.Value = dtTransaction.Rows(0)("VehicleId")

				hdf_PersonId.Value = dtTransaction.Rows(0)("PersonId")
				hdf_UniqueId.Value = dtTransaction.Rows(0)("Id")
				LBL_SelectedPerson.Text = dtTransaction.Rows(0)("PersonName").ToString() & "," & dtTransaction.Rows(0)("PersonPIN").ToString()
				hdf_PersonName.Value = dtTransaction.Rows(0)("PersonName").ToString()

				For Each rows As GridViewRow In gv_Persons.Rows
					If (dtTransaction.Rows(0)("PersonId") = gv_Persons.DataKeys(rows.RowIndex).Values("PersonId").ToString()) Then
						TryCast(rows.FindControl("RDB_Person"), RadioButton).Checked = True
					End If
				Next

				HDF_VehicleNumber.Value = dtTransaction.Rows(0)("VehicleNumber")
				lbl_VehicleNumber.Text = dtTransaction.Rows(0)("VehicleNumber")

				txtGuestVehicleNumber.Text = IIf(Convert.ToString(dtTransaction.Rows(0)("VehicleName")).Contains("guest"), dtTransaction.Rows(0)("VehicleNumber"), "")
				txtDeptNo.Text = dtTransaction.Rows(0)("DepartmentNumber")
				If txtDeptNo.Text = "" Then
					Dim dtVehicle As DataTable = New DataTable()
					OBJMaster = New MasterBAL()
					dtVehicle = OBJMaster.GetVehiclebyId(dtTransaction.Rows(0)("VehicleId"))
					txtDeptNo.Text = dtVehicle.Rows(0)("DepartmentNumber")
				End If
				txtPinNumber.Text = IIf(IsDBNull(dtTransaction.Rows(0)("PersonPin")), "", dtTransaction.Rows(0)("PersonPin"))
				txtOther.Text = dtTransaction.Rows(0)("Other")
				txtHours.Text = IIf(IsDBNull(dtTransaction.Rows(0)("Hours")), "", dtTransaction.Rows(0)("Hours"))
				'CHK_IsMissed.Checked = IIf(IsDBNull(dtTransaction.Rows(0)("IsMissed")), "", dtTransaction.Rows(0)("IsMissed"))
				DDL_TransactionStatus.SelectedValue = dtTransaction.Rows(0)("TransactionStatus")

				'txtPulses.Text = dtTransaction.Rows(0)("Pulses").ToString()

				Dim isManuallyEdit As Boolean = dtTransaction.Rows(0)("IsManuallyEdit")


				If (isManuallyEdit = True) Then
					ManuallyEditMessage.Visible = True
				Else
					ManuallyEditMessage.Visible = False
				End If

				OBJMaster = New MasterBAL()

				Dim strConditions As String = ""
				If (Not Session("TranConditions") Is Nothing) Then
					strConditions = Session("TranConditions")
				Else
					If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
						strConditions = IIf(strConditions = "", " and T.CompanyId=" & Session("CustomerId"), strConditions & " and T.CompanyId=" & Session("CustomerId"))
					End If
				End If

				HDF_TotalTransactions.Value = OBJMaster.GetTransactionIdByCondition(TransactionId, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), 0, True, strConditions)

				OBJMaster = New MasterBAL()

				strConditions = IIf(strConditions = "", " and (((ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15) Or ISNULL(T.TransactionStatus,0) = 2) ", strConditions + " and (((ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15) Or ISNULL(T.TransactionStatus,0) = 2) ")

				Dim dtAllTransactions As DataTable = New DataTable()

				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = " and T.CompanyId=" & Session("CustomerId")
				End If
				strConditions = IIf(strConditions = "", " and ISNULL(T.OFFSite,0) = 0", strConditions + " and ISNULL(T.OFFSite,0) = 0")
				dtAllTransactions = OBJMaster.GetTransactionsByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), True)

				dtAllTransactions.PrimaryKey = New DataColumn() {dtAllTransactions.Columns(0)}
				Dim dr As DataRow = dtAllTransactions.Rows.Find(TransactionId)
				If Not IsDBNull(dr) Then

					cnt = dtAllTransactions.Rows.IndexOf(dr) + 1

				End If
				If (cnt >= HDF_TotalTransactions.Value) Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				ElseIf (cnt <= 1) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt > 1 And cnt < HDF_TotalTransactions.Value) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
				lblof.Text = cnt & " of " & HDF_TotalTransactions.Value.ToString()
				lblCost.InnerText = dtTransaction.Rows(0)("TransactionCost")

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					beforeData = CreateData(TransactionId)
				End If

				'chkOFFSite.Checked = dtTransaction.Rows(0)("OFFSite")

			Else
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Data Not found. Please try again after some time."
			End If

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting Transaction Details , please try again later."

			log.Error("Error occurred in BindTransactionsDetails Exception is :" + ex.Message)
		Finally
			DDL_Customer.Focus()
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try

	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

		Dim TransactionId As Integer = 0
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			Dim IsManuallyEdit As Boolean = False

			If (HDF_VehicleId.Value = Nothing Or HDF_VehicleId.Value = "") Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select vehicle."

			End If

			If (hdf_PersonId.Value = Nothing Or hdf_UniqueId.Value = "") Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Please select person."
				Return
			End If

			Dim CurrentOdometer As Integer = 0
            Dim PreviousOdometer As Integer = 0
            Dim PreviousHours As Integer = 0
            Dim Site As Integer = 0
			Dim FQunty As Decimal = 0
			Dim Fuel As Integer = 0

			If txtCurrentOdometer.Text = "" Then CurrentOdometer = 0 Else CurrentOdometer = Convert.ToInt32(txtCurrentOdometer.Text)
			If DDL_Site.SelectedValue = "0" Then Site = 0 Else Site = Convert.ToInt32(DDL_Site.SelectedValue)
			If txtFuelQuantity.Text = "" Then FQunty = 0 Else FQunty = Convert.ToDecimal(txtFuelQuantity.Text)
			If DDL_Fuel.SelectedValue = "0" Then Fuel = 0 Else Fuel = Convert.ToInt32(DDL_Fuel.SelectedValue)
			If txtPreviousOdometer.Text = "" Then PreviousOdometer = 0 Else PreviousOdometer = Convert.ToInt32(txtPreviousOdometer.Text)
            If txtPreviousHours.Text = "" Then PreviousHours = 0 Else PreviousHours = Convert.ToInt32(txtPreviousHours.Text)
            If (Not hdfTransactionId.Value = Nothing And Not hdfTransactionId.Value = "") Then

				TransactionId = hdfTransactionId.Value
				Dim resultReturn As Integer = CheckBeforeSave(TransactionId)
				If (resultReturn = 0) Then
					IsManuallyEdit = False
					'message.Visible = True
					'message.InnerText = "Record saved"

					'If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					'	Dim writtenData As String = CreateData(TransactionId)
					'	CSCommonHelper.WriteLog("Modified and UnDeleted", "Deleted Transactions", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					'End If

					'Return
				Else
					IsManuallyEdit = True
				End If
			End If

			Dim result As Integer = 0

			OBJMaster = New MasterBAL()
			Dim transactionDatetime As DateTime = Request.Form(txtTransactionDate.UniqueID) & " " & Request.Form(txtTransactionTime.UniqueID)

			Dim IsMissed As Boolean = False
			Dim TransactionStatus As Integer = 0

			If (DDL_TransactionStatus.SelectedValue = 0) Then ' not started
				IsMissed = True
				TransactionStatus = 0
			ElseIf (DDL_TransactionStatus.SelectedValue = 1) Then 'Missed
				IsMissed = True
				TransactionStatus = 1
			ElseIf (DDL_TransactionStatus.SelectedValue = 2) Then 'Completed
				IsMissed = False
				TransactionStatus = 2
			ElseIf (DDL_TransactionStatus.SelectedValue = -1) Then 'Not selected
				IsMissed = False
				TransactionStatus = 2

			End If
			Dim dsTransactionValuesData As DataSet
			Dim DepartmentName As String = ""
			Dim FuelTypeName As String = ""
			Dim Email As String = ""
			Dim PersonName As String = ""
			Dim CompanyName As String = ""
			Dim VehicleName As String = ""
			Dim PersonId As Integer = 0
			If Not hdf_PersonId.Value = "" Then
				PersonId = Convert.ToInt32(hdf_PersonId.Value)
			End If

			dsTransactionValuesData = OBJMaster.GetTransactionColumnsValueForSave("", Convert.ToInt32(DDL_Fuel.SelectedValue), PersonId, Convert.ToInt32(HDF_VehicleId.Value))

			If dsTransactionValuesData IsNot Nothing Then
				If dsTransactionValuesData.Tables.Count > 0 Then
					If dsTransactionValuesData.Tables(0) IsNot Nothing And dsTransactionValuesData.Tables(0).Rows.Count > 0 Then
						DepartmentName = dsTransactionValuesData.Tables(0).Rows(0)("DeptName")
					End If
					If dsTransactionValuesData.Tables(1) IsNot Nothing And dsTransactionValuesData.Tables(1).Rows.Count > 0 Then
						FuelTypeName = dsTransactionValuesData.Tables(1).Rows(0)("FuelTypeName")
					End If
					If dsTransactionValuesData.Tables(2) IsNot Nothing And dsTransactionValuesData.Tables(2).Rows.Count > 0 Then
						Email = dsTransactionValuesData.Tables(2).Rows(0)("Email")
						PersonName = dsTransactionValuesData.Tables(2).Rows(0)("PersonName")
					End If
					If dsTransactionValuesData.Tables(3) IsNot Nothing And dsTransactionValuesData.Tables(3).Rows.Count > 0 Then
						CompanyName = dsTransactionValuesData.Tables(3).Rows(0)("CompanyName")
					Else
						CompanyName = DDL_Customer.SelectedItem.Text
					End If
					If dsTransactionValuesData.Tables(4) IsNot Nothing And dsTransactionValuesData.Tables(4).Rows.Count > 0 Then
						VehicleName = dsTransactionValuesData.Tables(4).Rows(0)("VehicleName")
					End If
				End If
			End If

			Dim vehicleNumber As String = ""

			If (HDF_VehicleNumber.Value.Contains("guest")) Then
				vehicleNumber = txtGuestVehicleNumber.Text
			Else
				vehicleNumber = HDF_VehicleNumber.Value
			End If

            result = OBJMaster.UpdateAndUnDeleteTransaction(HDF_VehicleId.Value, Site, PersonId, CurrentOdometer, FQunty, Fuel, 0, Nothing,
                                                     transactionDatetime, TransactionId, Convert.ToInt32(Session("PersonId")), "W", PreviousOdometer, vehicleNumber, txtDeptNo.Text,
                                                     txtPinNumber.Text, txtOther.Text, IIf(txtHours.Text = "", -1, txtHours.Text), IIf(txtPreviousHours.Text = "", -1, txtPreviousHours.Text), IsMissed, False, TransactionStatus, 0, -1,
                                                        VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName, False, Convert.ToInt32(DDL_Customer.SelectedValue), IsManuallyEdit)

            'result = OBJMaster.InsertUpdateTransaction(HDF_VehicleId.Value, DDL_Site.SelectedValue, hdf_PersonId.Value, txtCurrentOdometer.Text, txtFuelQuantity.Text, DDL_Fuel.SelectedValue, 0, Nothing,
            '                                         transactionDatetime, TransactionId, Convert.ToInt32(Session("PersonId")), "W", txtPreviousOdometer.Text, "", "", "", vehicleNumber, txtDeptNo.Text,
            '                                         txtPinNumber.Text, txtOther.Text, IIf(txtHours.Text = "", -1, txtHours.Text), IsMissed, False, TransactionStatus, 0, -1,
            '                                            VehicleName, DepartmentName, FuelTypeName, Email, PersonName, CompanyName)

            If result > 0 Then

				If (TransactionId > 0) Then
					message.Visible = True
					message.InnerText = "Record saved"

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(TransactionId)
						CSCommonHelper.WriteLog("Modified", "Transactions", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					Response.Redirect("~/Master/AllDeletedTransactions.aspx")
					'BindTransactionsDetails(result)
				Else

					'If Not txtCurrentOdometer.Text = "" Then
					'	OBJMaster.UpdateVehicleCurrentOdometer(HDF_VehicleId.Value, CurrentOdometer)
					'End If

					'If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					'	Dim writtenData As String = CreateData(result)
					'	CSCommonHelper.WriteLog("Added", "Transactions", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					'End If

					'Response.Redirect(String.Format("~/Master/Transaction?TransactionId={0}&RecordIs=New", result))
				End If

			Else
				If (TransactionId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(TransactionId)
						CSCommonHelper.WriteLog("Modified", "Transactions", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Transction update failed")
					End If

					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Deleted Transction update failed, please try again"
				Else
					'If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					'	Dim writtenData As String = CreateData(result)
					'	CSCommonHelper.WriteLog("Added", "Transactions", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Transction Addition failed")
					'End If
					'ErrorMessage.Visible = True
					'ErrorMessage.InnerText = "Transction Addition failed, please try again"
				End If

			End If

			DDL_Customer.Focus()

		Catch ex As Exception

			If (TransactionId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(TransactionId)
					CSCommonHelper.WriteLog("Modified", "Transactions", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Transction update failed. Exception is : " & ex.Message)
				End If

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Deleted Transction update failed, please try again"
			Else
				'If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				'	Dim writtenData As String = CreateData(TransactionId)
				'	CSCommonHelper.WriteLog("Added", "Transactions", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Transction Addition failed. Exception is : " & ex.Message)
				'End If
				'ErrorMessage.Visible = True
				'ErrorMessage.InnerText = "Transction Addition failed, please try again"
			End If
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while saving record, please try again later."

			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)

			DDL_Customer.Focus()

		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Private Function CheckBeforeSave(TransactionId) As Integer
		Try

			Dim dtTransaction As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtTransaction = OBJMaster.GetTransactionById(TransactionId, True)

            If (dtTransaction.Rows(0)("CurrentOdometer") <> txtCurrentOdometer.Text Or dtTransaction.Rows(0)("FuelQuantity") <> txtFuelQuantity.Text Or
                dtTransaction.Rows(0)("PreviousOdometer") <> txtPreviousOdometer.Text Or dtTransaction.Rows(0)("FuelTypeId") <> DDL_Fuel.SelectedValue Or
               Convert.ToDateTime(dtTransaction.Rows(0)("TransactionDateTime").ToString()).ToString("hh:mm tt") <> txtTransactionTime.Text Or
               Convert.ToDateTime(dtTransaction.Rows(0)("TransactionDateTime").ToString()).ToString("MM/dd/yyyy") <> txtTransactionDate.Text Or
               dtTransaction.Rows(0)("SiteId") <> DDL_Site.SelectedValue Or dtTransaction.Rows(0)("PersonID") <> hdf_PersonId.Value Or
               dtTransaction.Rows(0)("VehicleId") <> HDF_VehicleId.Value Or dtTransaction.Rows(0)("VehicleNumber") <> HDF_VehicleNumber.Value Or
               dtTransaction.Rows(0)("DepartmentNumber") <> txtDeptNo.Text Or IIf(IsDBNull(dtTransaction.Rows(0)("PersonPin")), "", dtTransaction.Rows(0)("PersonPin")) <> txtPinNumber.Text Or
               dtTransaction.Rows(0)("Other") <> txtOther.Text Or dtTransaction.Rows(0)("TransactionStatus") <> DDL_TransactionStatus.SelectedValue) Or
                IIf(IsDBNull(dtTransaction.Rows(0)("Hours")), "", dtTransaction.Rows(0)("Hours")) <> txtHours.Text Or
               IIf(IsDBNull(dtTransaction.Rows(0)("PreviousHours")), "", dtTransaction.Rows(0)("PreviousHours")) <> txtPreviousHours.Text Then
                Return 1
            Else
                Return 0
			End If

		Catch ex As Exception
			log.Error("Error occurred in CheckBeforeSave Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while Saving data, please try again later."
			Return 0
		End Try
	End Function

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/AllDeletedTransactions.aspx?Filter=Filter")
	End Sub

	'Protected Sub btnCloseVehicle_Click(sender As Object, e As EventArgs)
	'    'If HDF_PersonnelId.Value.ToString() = Nothing Then
	'    '    BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
	'    'Else
	'    '    Dim personid As Integer = Convert.ToInt32(HDF_PersonnelId.Value.ToString())
	'    '    BindVehicles(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
	'    '    BindVehiclesDataToCheckboxList(personid)
	'    'End If
	'    'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();", True)
	'End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try

			Dim CurrentTransactionId As Integer = hdfTransactionId.Value
			Dim strConditions As String = ""
			If (Not Session("TranConditions") Is Nothing) Then
				strConditions = Session("TranConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and T.CompanyId=" & Session("CustomerId"), strConditions & " and T.CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim TransactionId As Integer = OBJMaster.GetTransactionIdByCondition(CurrentTransactionId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), 0, True, strConditions)
			hdfTransactionId.Value = TransactionId
			BindTransactionsDetails(TransactionId)
		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try

			Dim CurrentTransactionId As Integer = hdfTransactionId.Value
			Dim strConditions As String = ""
			If (Not Session("TranConditions") Is Nothing) Then
				strConditions = Session("TranConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and T.CompanyId=" & Session("CustomerId"), strConditions & " and T.CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim TransactionId As Integer = OBJMaster.GetTransactionIdByCondition(CurrentTransactionId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), 0, True, strConditions)
			hdfTransactionId.Value = TransactionId
			BindTransactionsDetails(TransactionId)

		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try

			Dim CurrentTransactionId As Integer = hdfTransactionId.Value
			Dim strConditions As String = ""
			If (Not Session("TranConditions") Is Nothing) Then
				strConditions = Session("TranConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and T.CompanyId=" & Session("CustomerId"), strConditions & " and T.CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim TransactionId As Integer = OBJMaster.GetTransactionIdByCondition(CurrentTransactionId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), 0, True, strConditions)
			hdfTransactionId.Value = TransactionId
			BindTransactionsDetails(TransactionId)

		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try

			Dim CurrentTransactionId As Integer = hdfTransactionId.Value
			Dim strConditions As String = ""
			If (Not Session("TranConditions") Is Nothing) Then
				strConditions = Session("TranConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and T.CompanyId=" & Session("CustomerId"), strConditions & " and T.CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			Dim TransactionId As Integer = OBJMaster.GetTransactionIdByCondition(CurrentTransactionId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), 0, True, strConditions)
			hdfTransactionId.Value = TransactionId
			BindTransactionsDetails(TransactionId)

		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try
			BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))
			BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
			BindAllPersonnels(Convert.ToInt32(DDL_Customer.SelectedValue))
			BindAllVehicles(Convert.ToInt32(DDL_Customer.SelectedValue))
			BindFuelTypes(DDL_Customer.SelectedValue)

		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	'Protected Sub DDL_Vehicle_SelectedIndexChanged(sender As Object, e As EventArgs)

	'    Dim dtVehicle As DataTable = New DataTable()
	'    OBJMaster = New MasterBAL()
	'    'dtVehicle = OBJMaster.GetVehiclebyId(DDL_Vehicle.SelectedValue)

	'    DDL_Dept.SelectedValue = dtVehicle.Rows(0)("DepartmentId")
	'    txtDeptNo.Text = dtVehicle.Rows(0)("DepartmentNumber")
	'    lblVehicleName.Text = dtVehicle.Rows(0)("VehicleName")

	'    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
	'End Sub

	'Protected Sub DDL_Personnel_SelectedIndexChanged(sender As Object, e As EventArgs)
	'    OBJMaster = New MasterBAL()
	'    Dim personId As Integer = Convert.ToInt32(DDL_Personnel.SelectedValue)
	'    Dim dtPerson As DataTable = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" And ANU.PersonId = " & personId & "", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
	'    txtPinNumber.Text = dtPerson.Rows(0)("PinNumber").ToString()
	'    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
	'End Sub

	Protected Sub txtPinNumber_TextChanged(sender As Object, e As EventArgs)
		Try

			OBJMaster = New MasterBAL()
			Dim dtPerson As DataTable = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" And ANU.PinNumber = '" & txtPinNumber.Text & "'", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
			If (dtPerson.Rows.Count > 0) Then
				hdf_PersonId.Value = dtPerson.Rows(0)("PersonId")
				hdf_UniqueId.Value = dtPerson.Rows(0)("Id")
				LBL_SelectedPerson.Text = dtPerson.Rows(0)("PersonName").ToString() & "," & dtPerson.Rows(0)("PinNumber").ToString()
				hdf_PersonName.Value = dtPerson.Rows(0)("PersonName").ToString()
				For Each rows As GridViewRow In gv_Persons.Rows
					If (dtPerson.Rows(0)("PersonId") = gv_Persons.DataKeys(rows.RowIndex).Values("PersonId").ToString()) Then
						TryCast(rows.FindControl("RDB_Person"), RadioButton).Checked = True
					End If
				Next
			Else
				BindAllPersonnels(DDL_Customer.SelectedValue)
				hdf_PersonId.Value = ""
				hdf_UniqueId.Value = ""
				LBL_SelectedPerson.Text = ""
				hdf_PersonName.Value = ""
			End If

		Catch ex As Exception
			log.Error("Error occurred in txtPinNumber_TextChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Protected Sub DDL_Site_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			OBJMaster = New MasterBAL()
			Dim SiteId As Integer = Convert.ToInt32(DDL_Site.SelectedValue)
			Dim dtSites As DataTable = OBJMaster.GetSiteByCondition(" and s.siteId = " & SiteId & "", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), False)
			If (dtSites.Rows.Count > 0) Then
				DDL_Fuel.SelectedValue = dtSites.Rows(0)("FuelTypeId").ToString()
			Else
				DDL_Fuel.SelectedValue = 0
			End If
		Catch ex As Exception
			log.Error("Error occurred in DDL_Site_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Private Function CreateData(TransactionId As Integer) As String
		Try

            Dim data As String = "TransactionId = " & TransactionId & " ; " &
                                    "Vehicle Number = " & HDF_VehicleNumber.Value.Replace(",", " ") & " ; " &
                                    "Vehicle Name = " & lblVehicleName.Text.Replace(",", " ") & " ; " &
                                    "Department = " & DDL_Dept.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Department Number = " & txtDeptNo.Text.Replace(",", " ") & " ; " &
                                    "Guest Vehicle Number = " & txtGuestVehicleNumber.Text.Replace(",", " ") & " ; " &
                                    "FluidSecure Link = " & DDL_Site.Text.Replace(",", " ") & " ; " &
                                    "Fuel Quantity = " & txtFuelQuantity.Text.Replace(",", " ") & " ; " &
                                    "Other = " & txtOther.Text.Replace(",", " ") & " ; " &
                                    "Company = " & DDL_Customer.SelectedItem.Text & " ; " &
                                    "Cost = " & lblCost.InnerText.Trim().Replace(",", " ") & " ; " &
                                    "Transaction Date = " & txtTransactionDate.Text.Replace(",", " ") & " ; " &
                                    "Transaction Time = " & txtTransactionTime.Text.Replace(",", " ") & " ; " &
                                    "Person = " & hdf_PersonName.Value & " ; " &
                                    "Person PIN = " & txtPinNumber.Text.Replace(",", " ") & " ; " &
                                    "Current Odometer = " & txtCurrentOdometer.Text.Replace(",", " ") & " ; " &
                                    "Previous Odometer = " & txtPreviousOdometer.Text.Replace(",", " ") & " ; " &
                                     "Previous Hours = " & txtPreviousHours.Text.Replace(",", " ") & " ; " &
                                    "Hours = " & txtHours.Text.Replace(",", " ") & " ; " &
                                    "Fuel Type = " & DDL_Fuel.SelectedItem.Text.Replace(",", " ") & " ; " &
                                    "Transaction Status = " & DDL_TransactionStatus.SelectedItem.Text.Replace(",", " ") & " ; "

            Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnOk_Click(sender As Object, e As EventArgs)
		Try

			Dim isChecked As Boolean = False
			For Each item As GridViewRow In gv_Vehicles.Rows

				Dim RDB_Vehicle As RadioButton = TryCast(item.FindControl("RDB_Vehicle"), RadioButton)
				If (RDB_Vehicle.Checked = True) Then
					isChecked = True
					HDF_VehicleId.Value = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleId").ToString()
					HDF_VehicleNumber.Value = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber").ToString()

					Dim dtVehicle As DataTable = New DataTable()
					OBJMaster = New MasterBAL()
					dtVehicle = OBJMaster.GetVehiclebyId(HDF_VehicleId.Value)

					DDL_Dept.SelectedValue = dtVehicle.Rows(0)("DepartmentId")
					txtDeptNo.Text = dtVehicle.Rows(0)("DepartmentNumber")
					lblVehicleName.Text = dtVehicle.Rows(0)("VehicleName")
					lbl_VehicleNumber.Text = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber").ToString()
					Exit For
				End If
			Next

			If (isChecked = False) Then
				lblVehicleName.Text = ""
				lbl_VehicleNumber.Text = ""
			End If

		Catch ex As Exception
			log.Error("Error occurred in btnOk_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Protected Sub btnOkPerson_Click(sender As Object, e As EventArgs)
		Try

			Dim isChecked As Boolean = False
			For Each item As GridViewRow In gv_Persons.Rows

				Dim RDB_Person As RadioButton = TryCast(item.FindControl("RDB_Person"), RadioButton)
				If (RDB_Person.Checked = True) Then
					isChecked = True
					hdf_PersonId.Value = gv_Persons.DataKeys(item.RowIndex).Values("PersonId").ToString()
					hdf_UniqueId.Value = gv_Persons.DataKeys(item.RowIndex).Values("Id").ToString()

					Dim dtPerson As DataTable = New DataTable()
					OBJMaster = New MasterBAL()
					dtPerson = OBJMaster.GetPersonnelByPersonIdAndId(hdf_PersonId.Value, hdf_UniqueId.Value)
					LBL_SelectedPerson.Text = dtPerson.Rows(0)("PersonName").ToString() & "," & dtPerson.Rows(0)("PinNumber").ToString()
					txtPinNumber.Text = dtPerson.Rows(0)("PinNumber").ToString()
					hdf_PersonName.Value = dtPerson.Rows(0)("PersonName").ToString()
					Exit For
				End If
			Next

			If (isChecked = False) Then

			End If

		Catch ex As Exception
			log.Error("Error occurred in btnOk_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Private Sub BindTransactionStatus()
		Try

			DDL_TransactionStatus.Items.Insert(0, New ListItem("Select Transaction Status", "-1"))
			DDL_TransactionStatus.Items.Insert(1, New ListItem(ConfigurationManager.AppSettings("MissedText").ToString(), "1"))
			DDL_TransactionStatus.Items.Insert(2, New ListItem(ConfigurationManager.AppSettings("CompletedText").ToString(), "2"))

		Catch ex As Exception
			log.Error("Error occurred in BindTransactionStatus Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

End Class
