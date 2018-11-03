Imports log4net
Imports log4net.Config

Public Class TotalizerMeterReconciliationR
	Inherits System.Web.UI.Page
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TotalizerMeterReconciliationR))

	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If (Not IsPostBack) Then
					'txtStartDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
					'txtEndDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")
					'txtStartTimeFrom.Text = "12:00 AM"
					'txtEndTimeTo.Text = "11:59 PM"
					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					bindFluidLink()
					bindDates()
					DDL_FluidLink.SelectedIndex = 0
				Else
					'txtStartDateFrom.Text = Request.Form(txtStartDateFrom.UniqueID)
					'txtEndDateTo.Text = Request.Form(txtEndDateTo.UniqueID)
					'txtStartTimeFrom.Text = Request.Form(txtStartTimeFrom.UniqueID)
					'txtEndTimeTo.Text = Request.Form(txtEndTimeTo.UniqueID)
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

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support" And Not Session("RoleName") = "GroupAdmin") Then
                DDL_Customer.SelectedIndex = 1
                DDL_Customer.Enabled = False
                DDL_Customer.Visible = False
                divCompany.Visible = False
            End If


            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                If (Session("RoleName") = "GroupAdmin") Then
                    DDL_Customer.SelectedValue = Session("CustomerId")
                Else
                    DDL_Customer.SelectedIndex = 1
                End If
            End If

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)
		Try
			OBJMaster = New MasterBAL()
			Dim dtTankInv As DataTable = New DataTable()

			If ddlStartDateTime.SelectedIndex = 0 Or ddlEndDateTIme.SelectedIndex = 0 Then
				ErrorMessage.InnerText = "'Start Date/Time' or 'End Date/Time' not selected."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End If

			Dim startDate As DateTime
			Try
				'startDate = Convert.ToDateTime(Request.Form(txtStartDateFrom.UniqueID) + " " + Request.Form(txtStartTimeFrom.UniqueID)).ToString()
				startDate = Convert.ToDateTime(ddlStartDateTime.SelectedValue)
			Catch ex As Exception

				ErrorMessage.InnerText = "Wrong date format selected/enterd for 'Start Date/Time'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End Try
			Dim endDate As DateTime
			Try
				'endDate = Convert.ToDateTime(Request.Form(txtEndDateTo.UniqueID) + " " + Request.Form(txtEndTimeTo.UniqueID)).ToString()
				endDate = Convert.ToDateTime(ddlEndDateTIme.SelectedValue)
			Catch ex As Exception
				ErrorMessage.InnerText = "Wrong date format selected/enterd for 'End Date/Time'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End Try

			If endDate < startDate Then
				ErrorMessage.InnerText = "'Start Date' must less than 'End Date/Time'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End If


			Dim strConditions As String = ""
			Dim CustomerId As String = ""
			If (DDL_Customer.SelectedValue <> "0") Then
				CustomerId = DDL_Customer.SelectedValue.ToString()
			End If

			If (DDL_FluidLink.SelectedValue.ToString() <> "0") Then
				strConditions = IIf(strConditions = "", " and tm.FluidLink = " + DDL_FluidLink.SelectedValue.ToString(), strConditions + " and tm.FluidLink = " + DDL_FluidLink.SelectedValue.ToString())
			End If

			'get data from server
			dtTankInv = OBJMaster.GetTotalizerMeterReconciliationDetails(strConditions, startDate.ToString(), endDate.ToString(), CustomerId)
			If (Not dtTankInv Is Nothing) Then

				If (dtTankInv.Rows.Count <= 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData = CreateData()
						CSCommonHelper.WriteLog("Report Genereated", "Totalizer/Meter Reconciliations Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
					End If
					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
					Return
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData = CreateData()
					CSCommonHelper.WriteLog("Report Genereated", "Totalizer/Meter Reconciliations Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
				End If
				ErrorMessage.InnerText = "Data not found against selected criteria."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return

			End If
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData = CreateData()
				CSCommonHelper.WriteLog("Report Genereated", "Totalizer/Meter Reconciliations Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
			End If
			Session("TotalizerMeterReconciliationDetails") = dtTankInv

			Session("FromDate") = startDate.ToString("dd-MMM-yyyy hh:mm tt")
			Session("ToDate") = endDate.ToString("dd-MMM-yyyy hh:mm tt")
			Session("CustomerIdTankInve") = CustomerId
			Response.Redirect("~/Reports/TotalizerMeterReconciliationReport")


		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData = CreateData()
				CSCommonHelper.WriteLog("Report Genereated", "Totalizer/Meter Reconciliations Report", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
			End If
			ErrorMessage.InnerText = "Data not found against selected criteria."
			ErrorMessage.Visible = True
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
			Return
		Finally
			' txtStartDateFrom.Focus()
			ddlStartDateTime.Focus()
		End Try
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
		Try
			Response.Redirect("/Master/AllTotalizerMeterReconciliation?Type=TM")
		Catch ex As Exception

		End Try
	End Sub

	Protected Sub bindFluidLink()
		Try

			OBJMaster = New MasterBAL()
			Dim dtFluid As DataTable = New DataTable()
			Dim strCondition = ""
			DDL_FluidLink.Items.Clear()
			If DDL_Customer.SelectedValue.ToString() = "0" Then
				strCondition = ""
			Else
				strCondition = "and c.CustomerId = " + DDL_Customer.SelectedValue.ToString()

				dtFluid = OBJMaster.GetSiteByCondition(strCondition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), False)
				If dtFluid.Rows.Count > 0 Then
					Dim dtview As DataView = New DataView(dtFluid.DefaultView.ToTable(True, "SiteNumber", "WifiSSId"))
					dtview.Sort = "SiteNumber"
					DDL_FluidLink.DataSource = dtview
					DDL_FluidLink.DataTextField = "WifiSSId"
					DDL_FluidLink.DataValueField = "SiteNumber"
					DDL_FluidLink.DataBind()
				End If
			End If

			DDL_FluidLink.Items.Insert(0, New ListItem("Select FluidSecure Link", "0"))
			DDL_FluidLink.SelectedIndex = 0

		Catch ex As Exception

			log.Error("Error occurred in bindFluidLink Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
		bindFluidLink()
		bindDates()
	End Sub

	Private Function CreateData() As String
		Dim data As String = ""
		Try

			'For Each item As ListItem In lstSites.Items
			'    If item.Selected Then
			'        FluidSecureLink = IIf(FluidSecureLink = "", item.Text, FluidSecureLink + " & " + item.Text)
			'    End If
			'Next

			'data = "Start Date = " & Request.Form(txtStartDateFrom.UniqueID).Replace(",", " ") & " ; " &
			'       "Start Time = " & Request.Form(txtStartTimeFrom.UniqueID).ToString().Replace(",", " ") & " ; " &
			'       "End Date = " & Request.Form(txtEndDateTo.UniqueID).Replace(",", " ") & " ; " &
			'       "End Time = " & Request.Form(txtEndTimeTo.UniqueID).ToString().Replace(",", " ") & " ; " &
			'       "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
			'       "FluidSecure Link    = " & DDL_FluidLink.SelectedItem.Text.Replace(",", " ") & " "
			data = "Start Date and Time = " & ddlStartDateTime.SelectedValue.Replace(",", " ") & " ; " &
				   "End Date and Time = " & ddlEndDateTIme.SelectedValue.Replace(",", " ") & " ; " &
				   "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
				   "FluidSecure Link    = " & DDL_FluidLink.SelectedItem.Text.Replace(",", " ") & " "
			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			data = ""
		Finally

		End Try
		Return data
	End Function

	Private Sub bindDates()
		Try
			Dim strCondition = ""
			If DDL_FluidLink.SelectedIndex = 0 Then
				strCondition = ""
			Else
				strCondition = " and FluidLink = " + DDL_FluidLink.SelectedValue.ToString()
			End If

			OBJMaster = New MasterBAL()
			Dim dtDate As DataTable = New DataTable()

			ddlStartDateTime.Items.Clear()
			ddlEndDateTIme.Items.Clear()
			If Not DDL_Customer.SelectedValue.ToString() = "0" Then
				strCondition = IIf(strCondition = "", " and Entry_Type = 'TM' and TankInventory.CompanyId = " + DDL_Customer.SelectedValue.ToString(), strCondition + " and Entry_Type = 'TM' and TankInventory.CompanyId = " + DDL_Customer.SelectedValue.ToString())

				dtDate = OBJMaster.GetTankInventorybyConditions(strCondition, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
				If dtDate.Rows.Count > 0 Then
					Dim dtStartview As DataView = New DataView(dtDate)
					' dtStartview.RowFilter = "datetype = 'Start Level'"
					dtStartview.Sort = "ReportDate"
					ddlStartDateTime.DataSource = dtStartview
					ddlStartDateTime.DataTextField = "ReportDate"
					ddlStartDateTime.DataValueField = "ReportDate"
					ddlStartDateTime.DataBind()

					Dim dtEndview As DataView = New DataView(dtDate)
					' dtEndview.RowFilter = "datetype = 'End Level'"
					dtEndview.Sort = "ReportDate"
					ddlEndDateTIme.DataSource = dtEndview
					ddlEndDateTIme.DataTextField = "ReportDate"
					ddlEndDateTIme.DataValueField = "ReportDate"
					ddlEndDateTIme.DataBind()
				End If
			End If

			ddlStartDateTime.Items.Insert(0, New ListItem("Select Start Date/Time", "0"))
			ddlEndDateTIme.Items.Insert(0, New ListItem("Select End Date/Time", "0"))

			ddlStartDateTime.SelectedIndex = 0
			ddlEndDateTIme.SelectedIndex = 0

		Catch ex As Exception
			log.Error(String.Format("Error Occurred while bindDates. Error is {0}.", ex.Message))
		End Try
	End Sub

	Protected Sub DDL_FluidLink_SelectedIndexChanged(sender As Object, e As EventArgs)
		bindDates()
	End Sub

End Class
