Imports log4net
Imports log4net.Config

Public Class TransactionReportBySite
	Inherits System.Web.UI.Page


	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TransactionReportBySite))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				Response.Redirect("/Account/Login")
			ElseIf Session("RoleName") = "User" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If Not IsPostBack Then
					BindTransactionStatus()
					txtTransactionDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
					txtTransactionDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
					txtTransactionTimeFrom.Text = "12:00 AM"
					txtTransactionTimeTo.Text = "11:59 PM"

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

				Else

					txtTransactionDateFrom.Text = Request.Form(txtTransactionDateFrom.UniqueID)
					txtTransactionDateTo.Text = Request.Form(txtTransactionDateTo.UniqueID)
					txtTransactionTimeFrom.Text = Request.Form(txtTransactionTimeFrom.UniqueID)
					txtTransactionTimeTo.Text = Request.Form(txtTransactionTimeTo.UniqueID)

				End If
				txtTransactionDateFrom.Focus()
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

			If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support") Then
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

			grd_Dept.DataSource = dtDept
			grd_Dept.DataBind()

			If grd_Dept.Rows.Count <> 0 Then
				lblDepartmentMessage.Visible = False
				grd_Dept.Visible = True
			ElseIf grd_Dept.Rows.Count = 0 Then
				lblDepartmentMessage.Text = "Department not found for selected Company."
				lblDepartmentMessage.Visible = True
				grd_Dept.Visible = False
			End If

			DDL_Dept.DataTextField = "NAME"
			DDL_Dept.DataValueField = "DeptId"

			DDL_Dept.DataBind()
			DDL_Dept.Items.Insert(0, New ListItem("Select All Department", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindDepartment Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting departments, please try again later."

		End Try
	End Sub

	Private Sub BindAllVehicles(deptId As Integer)
		Try
			Dim dtVehicle As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			If (deptId = 0) Then
				dtVehicle = OBJMaster.GetVehicleByCondition(" and  v.CustomerId  = " & DDL_Customer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())
			Else
				dtVehicle = OBJMaster.GetVehicleByCondition(" and V.DepartmentId  = " & deptId, Session("PersonId").ToString(), Session("RoleId").ToString())
			End If


			gv_Vehicles.DataSource = dtVehicle
			gv_Vehicles.DataBind()

			If gv_Vehicles.Rows.Count <> 0 Then

				lblVehicleMessage.Visible = False
				gv_Vehicles.Visible = True
			ElseIf gv_Vehicles.Rows.Count = 0 Then
				lblVehicleMessage.Text = "Vehicles not found for selected Company."
				lblVehicleMessage.Visible = True
				gv_Vehicles.Visible = False
			End If

			DDL_Vehicle.DataSource = dtVehicle
			DDL_Vehicle.DataValueField = "VehicleId"
			DDL_Vehicle.DataTextField = "VehicleNumber"
			DDL_Vehicle.DataBind()

			DDL_Vehicle.Items.Insert(0, New ListItem("Select All Vehicle", "0"))

			DDL_VehicleType.Items.Clear()
			If dtVehicle.Rows.Count > 0 Then
				Dim dtview As DataView = New DataView(dtVehicle.DefaultView.ToTable(True, "Type"))
				dtview.RowFilter = "Type <> ''"
				DDL_VehicleType.DataSource = dtview.ToTable()
				DDL_VehicleType.DataTextField = "Type"
				DDL_VehicleType.DataValueField = "Type"
				DDL_VehicleType.DataBind()
			End If

			DDL_VehicleType.Items.Insert(0, New ListItem("Select All Vehicle Type", "0"))
			DDL_VehicleType.SelectedIndex = 0

		Catch ex As Exception

			log.Error("Error occurred in BindAllVehicles Exception is :" + ex.Message)
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

			DDL_Fuel.Items.Insert(0, New ListItem("Select All Products", "0"))

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

			Dim IsDeletedLinkAllow As Boolean = False
			If chk_IsDeletedLinkAllow.Checked Then
				IsDeletedLinkAllow = True
			End If

			dtSites = OBJMaster.GetSiteByCondition(" And c.CustomerId =" + CustomerId.ToString(), Session("PersonId").ToString(), Session("RoleId").ToString(), IsDeletedLinkAllow)

			'DDL_Site.DataSource = dtSites
			'DDL_Site.DataValueField = "SiteId"
			'DDL_Site.DataTextField = "WifiSSid"
			'DDL_Site.DataBind()

			'DDL_Site.Items.Insert(0, New ListItem("Select All FluidSecure Link", "0"))

			lstSites.DataSource = dtSites
			lstSites.DataValueField = "SiteId"
			lstSites.DataTextField = "WifiSSid"
			lstSites.DataBind()

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting sites, please try again later."

			log.Error("Error occurred in BindSites Exception is :" + ex.Message)

		End Try
	End Sub

	Private Sub BindAllPersonnels(deptId As Integer)
		Try
			Dim dtPersonnel As DataTable = New DataTable()
			OBJMaster = New MasterBAL()

			If (deptId = 0) Then
				dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(ANU.IsFluidSecureHub,0)=0  and ANU.CustomerId = " & DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
			Else
				dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(ANU.IsFluidSecureHub,0)=0  and ANU.DepartmentId = " & deptId, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
			End If

			grd_Per.DataSource = dtPersonnel
			grd_Per.DataBind()

			If grd_Per.Rows.Count <> 0 Then

				lblPersonnelMessage.Visible = False
				grd_Per.Visible = True
			ElseIf grd_Per.Rows.Count = 0 Then
				lblPersonnelMessage.Text = "Personnel not found for selected Company or Department."
				lblPersonnelMessage.Visible = True
				grd_Per.Visible = False
			End If

			DDL_Personnel.DataSource = dtPersonnel
			DDL_Personnel.DataValueField = "PersonId"
			DDL_Personnel.DataTextField = "Person"
			DDL_Personnel.DataBind()

			DDL_Personnel.Items.Insert(0, New ListItem("Select All Personnel", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

		End Try
	End Sub

	Private Sub BindAllHubs()
		Try
			Dim dtPersonnel As DataTable = New DataTable()
			OBJMaster = New MasterBAL()

			If (DDL_Customer.SelectedValue <> "0") Then
				dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(ANU.IsFluidSecureHub,0)=1  and ANU.CustomerId = " & DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

				grd_Site.DataSource = dtPersonnel
				grd_Site.DataBind()

				If grd_Site.Rows.Count <> 0 Then

					lblSiteMessage.Visible = False
					grd_Site.Visible = True
				ElseIf grd_Site.Rows.Count = 0 Then
					lblSiteMessage.Text = "Site not found for selected Company."
					lblSiteMessage.Visible = True
					grd_Site.Visible = False
				End If

				DDL_HubName.DataSource = dtPersonnel
				DDL_HubName.DataValueField = "PersonId"
				DDL_HubName.DataTextField = "HubSiteName"
				DDL_HubName.DataBind()
			End If

			DDL_HubName.Items.Insert(0, New ListItem("Select Site", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllHubs Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting All Hubs, please try again later."

		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try


			BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
			BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))
			BindAllPersonnels(0)
			BindAllHubs()
			BindAllVehicles(0)
			BindFuelTypes(Convert.ToInt32(DDL_Customer.SelectedValue))
		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();$('[id*=lstSites]').multiselect('selectAll', false).multiselect('updateButtonText');", True)
		End Try
	End Sub

	Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

		Try


			OBJMaster = New MasterBAL()
			Dim dSTran As DataSet = New DataSet()
			Dim startDate As DateTime
			Try
				startDate = Convert.ToDateTime(Request.Form(txtTransactionDateFrom.UniqueID) + " " + Request.Form(txtTransactionTimeFrom.UniqueID)).ToString()
			Catch ex As Exception

				ErrorMessage.InnerText = "Wrong date format selected/enterd for 'From Date'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End Try
			Dim endDate As DateTime
			Try
				endDate = Convert.ToDateTime(Request.Form(txtTransactionDateTo.UniqueID) + " " + Request.Form(txtTransactionTimeTo.UniqueID)).ToString()
			Catch ex As Exception
				ErrorMessage.InnerText = "Wrong date format selected/enterd for 'To Date'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End Try

			If endDate < startDate Then
				ErrorMessage.InnerText = "'From Date' must less than 'To Date'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End If

			Dim strConditions As String = ""
			If (DDL_Customer.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and T.CompanyId = " + DDL_Customer.SelectedValue, strConditions + " and T.CompanyId = " + DDL_Customer.SelectedValue)
			End If

			If (DDL_Dept.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and d.DeptId = " + DDL_Dept.SelectedValue, strConditions + " and d.DeptId = " + DDL_Dept.SelectedValue)
			End If

			If (DDL_Vehicle.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and T.VehicleID = " + DDL_Vehicle.SelectedValue, strConditions + " and T.VehicleID = " + DDL_Vehicle.SelectedValue)
			End If

			If (DDL_VehicleType.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and V.Type = '" + DDL_VehicleType.SelectedValue + "' ", strConditions + " and V.Type = '" + DDL_VehicleType.SelectedValue + "' ")
			End If

			If (DDL_Personnel.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and T.PersonID = " + DDL_Personnel.SelectedValue, strConditions + " and T.PersonID = " + DDL_Personnel.SelectedValue)
			End If

			Dim SelectedSiteIds As String = ""

			For Each item As ListItem In lstSites.Items
				If item.Selected Then
					SelectedSiteIds = IIf(SelectedSiteIds = "", item.Value, SelectedSiteIds + "," + item.Value)
				End If
			Next
			If (SelectedSiteIds <> "") Then
				strConditions = IIf(strConditions = "", " and T.SiteID in ( " + SelectedSiteIds + ")", strConditions + " and T.SiteID in ( " + SelectedSiteIds + ")")
			End If

			'If (DDL_Site.SelectedValue <> "0") Then
			'    strConditions = IIf(strConditions = "", " and T.SiteID = " + DDL_Site.SelectedValue, strConditions + " and T.SiteID = " + DDL_Site.SelectedValue)
			'End If

			If (DDL_Fuel.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and T.fuelTypeId = " + DDL_Fuel.SelectedValue, strConditions + " and T.fuelTypeId = " + DDL_Fuel.SelectedValue)
			End If
			'If (DDL_Fuel.SelectedValue <> "0") Then
			'    strConditions = IIf(strConditions = "", " and T.fuelTypeId = " + DDL_Fuel.SelectedValue, strConditions + " and T.fuelTypeId = " + DDL_Fuel.SelectedValue)

			'End If
			If (DDL_TransactionStatus.SelectedValue = "2") Then
				strConditions = IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 2", strConditions + " and ISNULL(T.TransactionStatus,0) = 2")
			ElseIf (DDL_TransactionStatus.SelectedValue = "0") Then
				strConditions = (IIf(strConditions = "", " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and ISNULL(T.TransactionStatus,0) = 0 and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
			ElseIf (DDL_TransactionStatus.SelectedValue = "1") Then
				strConditions = (IIf(strConditions = "", " and (ISNULL(T.TransactionStatus,0) = 1  And ISNULL(T.IsMissed,0)= 1) and datediff(minute, T.[CreatedDate] ,getdate()) > 15 ", strConditions + " and (ISNULL(T.TransactionStatus,0) = 1 And ISNULL(T.IsMissed,0)= 1)  and datediff(minute, T.[CreatedDate] ,getdate()) > 15"))
			End If

			'strConditions = IIf(strConditions = "", " and ISNULL(t.IsMissed,0) = " + DDL_Missed.SelectedValue, strConditions + " and ISNULL(t.IsMissed,0) = " + DDL_Missed.SelectedValue)

			'strConditions += "  and ISNULL(IsFluidSecureHub,0)=0 "
			If (DDL_HubName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ISNULL(T.HubId,0) = " + DDL_HubName.SelectedValue, strConditions + " and ISNULL(T.HubId,0) = " + DDL_HubName.SelectedValue)
			End If
			'get data from server
			dSTran = OBJMaster.GetTransactionRptDetails(startDate.ToString(), endDate.ToString(), strConditions, "site")
			If (Not dSTran Is Nothing) Then

				If (dSTran.Tables.Count < 3 Or dSTran.Tables(0).Rows.Count <= 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData()

						CSCommonHelper.WriteLog("Report Genereated", "Transaction Report by Site", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
					End If
					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
					Return
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData()

					CSCommonHelper.WriteLog("Report Genereated", "Transaction Report by Site", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
				End If
				ErrorMessage.InnerText = "Data not found against selected criteria."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return

			End If
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData()

				CSCommonHelper.WriteLog("Report Genereated", "Transaction Report by Site", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
			End If
			Session("TransReportBySite") = dSTran

			Session("FromDate") = startDate.ToString("dd-MMM-yyyy hh:mm tt")
			Session("ToDate") = endDate.ToString("dd-MMM-yyyy hh:mm tt")

			Response.Redirect("~/Reports/TransactionReportBySiteReport")


		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData()

				CSCommonHelper.WriteLog("Report Genereated", "Transaction Report by Site", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
			End If
			ErrorMessage.InnerText = "Data not found against selected criteria."
			ErrorMessage.Visible = True
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
			Return
		Finally
			txtTransactionDateFrom.Focus()
		End Try

	End Sub

	Protected Sub DDL_Dept_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			BindAllPersonnels(Convert.ToInt32(DDL_Dept.SelectedValue))
			BindAllVehicles(Convert.ToInt32(DDL_Dept.SelectedValue))
			For Each rows As GridViewRow In grd_Dept.Rows
				If (DDL_Dept.SelectedValue = grd_Dept.DataKeys(rows.RowIndex).Values("DeptId").ToString()) Then
					TryCast(rows.FindControl("RDB_Department"), RadioButton).Checked = True
				Else
					TryCast(rows.FindControl("RDB_Department"), RadioButton).Checked = False
				End If
			Next

		Catch ex As Exception
			log.Error("Error occurred in DDL_Dept_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
		End Try

	End Sub

	Private Function CreateData() As String
		Try
			Dim FluidSecureLink As String = ""

			For Each item As ListItem In lstSites.Items
				If item.Selected Then
					FluidSecureLink = IIf(FluidSecureLink = "", item.Text, FluidSecureLink + " & " + item.Text)
				End If
			Next

			Dim data As String = "Transaction Date From = " & txtTransactionDateFrom.Text.Replace(",", "") & " ; " &
									"Transaction Time From = " & txtTransactionTimeFrom.Text.Replace(",", " ") & " ; " &
									"Transaction Date To = " & txtTransactionDateTo.Text.Replace(",", "") & " ; " &
									"Transaction Time To = " & txtTransactionTimeTo.Text.Replace(",", "") & " ; " &
									"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Vehicle Number = " & DDL_Vehicle.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Vehicle Type = " & DDL_VehicleType.SelectedItem.Text.Replace(",", " ") & " ; " &
									"FluidSecure Link    = " & FluidSecureLink.Replace(",", " ") & " ; " &
									"Transaction Status = " & DDL_TransactionStatus.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Department = " & DDL_Dept.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Personnel = " & DDL_Personnel.SelectedItem.Text & " ; " &
									"Product = " & DDL_Fuel.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Select Site = " & DDL_HubName.SelectedItem.Text.Replace(",", " ") & " ; "

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
					DDL_Vehicle.SelectedValue = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleId").ToString()
					Exit For
				End If
			Next

			If (isChecked = False) Then
				'lblVehicleName.Text = ""
				DDL_Vehicle.SelectedValue = 0
			End If


		Catch ex As Exception
			log.Error("Error occurred in btnOk_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();hideWait();", True)
		End Try
	End Sub

	Protected Sub DDL_Vehicle_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			For Each rows As GridViewRow In gv_Vehicles.Rows
				If (DDL_Vehicle.SelectedValue = gv_Vehicles.DataKeys(rows.RowIndex).Values("VehicleId").ToString()) Then
					TryCast(rows.FindControl("RDB_Vehicle"), RadioButton).Checked = True
				Else
					TryCast(rows.FindControl("RDB_Vehicle"), RadioButton).Checked = False
				End If
			Next

		Catch ex As Exception
			log.Error("Error occurred in DDL_Vehicle_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
		End Try
	End Sub

	Protected Sub btndeptOK_Click(sender As Object, e As EventArgs)
		Try

			Dim isChecked As Boolean = False
			For Each item As GridViewRow In grd_Dept.Rows

				Dim RDB_Department As RadioButton = TryCast(item.FindControl("RDB_Department"), RadioButton)
				If (RDB_Department.Checked = True) Then
					isChecked = True
					HDF_DeptId.Value = grd_Dept.DataKeys(item.RowIndex).Values("DeptId").ToString()
					HDF_DeptNumber.Value = grd_Dept.DataKeys(item.RowIndex).Values("Number").ToString()
					DDL_Dept.SelectedValue = grd_Dept.DataKeys(item.RowIndex).Values("DeptId").ToString()
					Exit For
				End If
			Next

			If (isChecked = False) Then
				DDL_Dept.SelectedValue = 0
			End If
			DDL_Dept_SelectedIndexChanged(Nothing, Nothing)

		Catch ex As Exception
			log.Error("Error occurred in btndeptOK_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();hideWait();", True)
		End Try
	End Sub

	Protected Sub DDL_Personnel_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			For Each rows As GridViewRow In grd_Per.Rows
				If (DDL_Personnel.SelectedValue = grd_Per.DataKeys(rows.RowIndex).Values("PersonId").ToString()) Then
					TryCast(rows.FindControl("RDB_Personnel"), RadioButton).Checked = True
				Else
					TryCast(rows.FindControl("RDB_Personnel"), RadioButton).Checked = False
				End If
			Next

		Catch ex As Exception
			log.Error("Error occurred in DDL_Personnel_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
		End Try
	End Sub

	Protected Sub btnPerOK_Click(sender As Object, e As EventArgs)
		Try

			Dim isChecked As Boolean = False
			For Each item As GridViewRow In grd_Per.Rows

				Dim RDB_Personnel As RadioButton = TryCast(item.FindControl("RDB_Personnel"), RadioButton)
				If (RDB_Personnel.Checked = True) Then
					isChecked = True
					HDF_PersonId.Value = grd_Per.DataKeys(item.RowIndex).Values("PersonId").ToString()
					HDF_Person.Value = grd_Per.DataKeys(item.RowIndex).Values("Person").ToString()
					DDL_Personnel.SelectedValue = grd_Per.DataKeys(item.RowIndex).Values("PersonId").ToString()
					Exit For
				End If
			Next

			If (isChecked = False) Then
				DDL_Personnel.SelectedValue = 0
			End If
			' DDL_Personnel_SelectedIndexChanged(Nothing, Nothing)

		Catch ex As Exception
			log.Error("Error occurred in btnPerOK_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();hideWait();", True)
		End Try
	End Sub

	Protected Sub DDL_HubName_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			For Each rows As GridViewRow In grd_Site.Rows
				If (DDL_HubName.SelectedValue = grd_Site.DataKeys(rows.RowIndex).Values("PersonId").ToString()) Then
					TryCast(rows.FindControl("RDB_Site"), RadioButton).Checked = True
				Else
					TryCast(rows.FindControl("RDB_Site"), RadioButton).Checked = False
				End If
			Next

		Catch ex As Exception
			log.Error("Error occurred in DDL_HubName_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
		End Try
	End Sub

	Protected Sub btnSiteOK_Click(sender As Object, e As EventArgs)
		Try

			Dim isChecked As Boolean = False
			For Each item As GridViewRow In grd_Site.Rows

				Dim RDB_Site As RadioButton = TryCast(item.FindControl("RDB_Site"), RadioButton)
				If (RDB_Site.Checked = True) Then
					isChecked = True
					HDF_PersonId_Site.Value = grd_Site.DataKeys(item.RowIndex).Values("PersonId").ToString()
					HDF_HubSiteName.Value = grd_Site.DataKeys(item.RowIndex).Values("HubSiteName").ToString()
					DDL_HubName.SelectedValue = grd_Site.DataKeys(item.RowIndex).Values("PersonId").ToString()
					Exit For
				End If
			Next

			If (isChecked = False) Then
				DDL_HubName.SelectedValue = 0
			End If
			'DDL_HubName_SelectedIndexChanged(Nothing, Nothing)

		Catch ex As Exception
			log.Error("Error occurred in btnSiteOK_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();hideWait();", True)
		End Try
	End Sub

	Private Sub BindTransactionStatus()
		Try

			DDL_TransactionStatus.Items.Insert(0, New ListItem(ConfigurationManager.AppSettings("CompletedText").ToString(), "2"))
			DDL_TransactionStatus.Items.Insert(1, New ListItem(ConfigurationManager.AppSettings("NotStartedText").ToString(), "0"))
			DDL_TransactionStatus.Items.Insert(2, New ListItem(ConfigurationManager.AppSettings("MissedText").ToString(), "1"))

		Catch ex As Exception
			log.Error("Error occurred in BindTransactionStatus Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub grd_Per_DataBound(sender As Object, e As EventArgs)
		Try
			If (Session("RoleName").ToString() <> "SuperAdmin" And Session("RoleName").ToString() <> "CustomerAdmin") Then
				grd_Per.Columns(2).Visible = False
			End If
		Catch ex As Exception
			log.Error("Error occurred in grd_Per_DataBound Exception is :" + ex.Message)
			ErrorMessage.Visible = True
		End Try
	End Sub

	Protected Sub chk_IsDeletedLinkAllow_CheckedChanged(sender As Object, e As EventArgs)
		Try

			BindSites(Convert.ToInt32(DDL_Customer.SelectedValue.ToString()))
		Catch ex As Exception
			log.Error("Error occurred in chk_IsDeletedLinkAllow_CheckedChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadMultiList();$('[id*=lstSites]').multiselect({includeSelectAllOption: true,allSelectedText: 'All FluidSecure Link',}).multiselect('selectAll', false).multiselect('updateButtonText');", True)
		End Try
	End Sub

End Class