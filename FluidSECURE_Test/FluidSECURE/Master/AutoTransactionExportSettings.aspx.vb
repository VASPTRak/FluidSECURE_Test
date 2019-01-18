Imports log4net
Imports log4net.Config

Public Class AutoTransactionExportSettings
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AutoTransactionExportSettings))

	Dim OBJMaster As MasterBAL = New MasterBAL()
	Shared beforeData As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "CustomerAdmin" And Session("RoleName") <> "GroupAdmin" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then
					txtExecutionTime.Text = DateTime.Now.ToString("hh:mm tt")

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					BindTimeZones()

					If (Not Request.QueryString("AutoTransactionExportSettingId") = Nothing And Not Request.QueryString("AutoTransactionExportSettingId") = "") Then

						HDF_AutoTransactionExportSettingId.Value = Request.QueryString("AutoTransactionExportSettingId")

						BindAutoTransactionExportSettingDetails(Request.QueryString("AutoTransactionExportSettingId"))
						btnFirst.Visible = True
						btnNext.Visible = True
						btnprevious.Visible = True
						btnLast.Visible = True
						lblHeader.Text = "Edit Customized Export Settings"
						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message.InnerText = "Record saved"
						End If
					Else
						btnFirst.Visible = False
						btnNext.Visible = False
						btnprevious.Visible = False
						btnLast.Visible = False
						lblof.Visible = False
						lblHeader.Text = "Add Customized Export Settings"
						DDL_Company_SelectedIndexChanged(Nothing, Nothing)
					End If
					DDL_Company.Focus()

					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

				Else

					txtExecutionTime.Text = Request.Form(txtExecutionTime.UniqueID)

				End If
			End If


		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

		End Try
	End Sub

	Private Sub BindAutoTransactionExportSettingDetails(AutoTransactionExportSettingId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtAutoTransactionExportSetting As DataTable = New DataTable()
			dtAutoTransactionExportSetting = OBJMaster.GetAutoTransactionExportSettingsByAutoTransactionExportSettingId(AutoTransactionExportSettingId)
			Dim cnt As Integer = 0
			If (dtAutoTransactionExportSetting.Rows.Count > 0) Then
				Dim isValid As Boolean = False
				If (Session("RoleName") = "GroupAdmin") Then
					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
					For Each drCusts As DataRow In dtCustOld.Rows
						If (drCusts("CustomerId") = dtAutoTransactionExportSetting.Rows(0)("CompanyId").ToString()) Then
							isValid = True
							Exit For
						End If

					Next
				End If

				If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtAutoTransactionExportSetting.Rows(0)("CompanyId").ToString()) Then

						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

						Return
					End If

				End If

				DDL_Company.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("CompanyId").ToString()
				BindCustomizedExportTemplates(DDL_Company.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
				DDL_ExportOption.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("ExportOption").ToString().TrimEnd().TrimStart()
				TXT_FTPServerPath.Text = dtAutoTransactionExportSetting.Rows(0)("FtpServerPath").ToString()
				txtFTPUsername.Text = dtAutoTransactionExportSetting.Rows(0)("FtpUsername").ToString()
				txtFTPPassword.Text = dtAutoTransactionExportSetting.Rows(0)("FtpPassword").ToString()
				txtEmail.Text = dtAutoTransactionExportSetting.Rows(0)("EmailId").ToString()
				CHK_Active.Checked = dtAutoTransactionExportSetting.Rows(0)("Active").ToString()
				DDL_TimeZone.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("TimeZoneId").ToString()
				DDL_TimeZone_SelectedIndexChanged(Nothing, Nothing)
				DDL_Separator.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("Separator").ToString()
				DDL_CustomizedExportTemplate.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("CustomizedExportTemplateId").ToString()
				chk_IncludePreviouslyExportTransactions.Checked = dtAutoTransactionExportSetting.Rows(0)("IncludePreviouslyExportTransactions").ToString()
				chk_ExportOnlyNewTransactions.Checked = dtAutoTransactionExportSetting.Rows(0)("ExportOnlyNewTransactions").ToString()
				chk_ExportZeroQtyTransactions.Checked = dtAutoTransactionExportSetting.Rows(0)("ExportZeroQtyTransactions").ToString()
				DDL_ExportOption_SelectedIndexChanged(Nothing, Nothing)

				'txtExecutionTime.Text = IIf(dtAutoTransactionExportSetting.Rows(0)("ExecutionTime").ToString() = "", "", Convert.ToDateTime(dtAutoTransactionExportSetting.Rows(0)("ExecutionTime").ToString()).ToString("hh:mm tt"))

				DDL_DateType.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("DateType").ToString()
				ddl_DecimalQTY.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("DecimalPlace").ToString()
				If ddl_DecimalQTY.SelectedValue = "2" Then
					divDecimailType.Visible = True
					ddl_DecimalType.SelectedValue = dtAutoTransactionExportSetting.Rows(0)("DecimalType").ToString()
				Else
					divDecimailType.Visible = False
				End If
				chk_FATransaction.Checked = dtAutoTransactionExportSetting.Rows(0)("IncludeFA").ToString()

				If (dtAutoTransactionExportSetting.Rows(0)("ExecutionTime").ToString() = "") Then
					txtExecutionTime.Text = ""
				Else
					txtExecutionTime.Text = Convert.ToDateTime(dtAutoTransactionExportSetting.Rows(0)("ExecutionTime").ToString()).ToString("hh:mm tt")
				End If
				Dim strConditions As String = ""

				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions += " and ATES.CompanyId=" & Session("CustomerId")
				End If

				OBJMaster = New MasterBAL()

				HDF_TotalAutoTransactionExportSettings.Value = OBJMaster.GetAutoTransactionExportSettingIdByCondition(AutoTransactionExportSettingId, False, False, False, False, True, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), strConditions)

				OBJMaster = New MasterBAL()
				Dim dtAllAutoTransactionExportSettings As DataTable = New DataTable()

				dtAllAutoTransactionExportSettings = OBJMaster.GetAutoTransactionExportSettingsByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())


				dtAllAutoTransactionExportSettings.PrimaryKey = New DataColumn() {dtAllAutoTransactionExportSettings.Columns(0)}
				Dim dr As DataRow = dtAllAutoTransactionExportSettings.Rows.Find(AutoTransactionExportSettingId)
				If Not IsDBNull(dr) Then

					cnt = dtAllAutoTransactionExportSettings.Rows.IndexOf(dr) + 1

				End If
				If (HDF_TotalAutoTransactionExportSettings.Value = 1) Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt >= HDF_TotalAutoTransactionExportSettings.Value) Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				ElseIf (cnt <= 1) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt > 1 And cnt < HDF_TotalAutoTransactionExportSettings.Value) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
				lblof.Text = cnt & " of " & HDF_TotalAutoTransactionExportSettings.Value.ToString()

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					beforeData = CreateData(AutoTransactionExportSettingId)
				End If

			Else
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Data Not found. Please try again after some time."
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindAutoTransactionExportSettingDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Automatic Export Settings data, please try again later."
		Finally
			DDL_Company.Focus()

			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		Dim AutoTransactionExportSettingId As Integer = 0

		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			If (Not HDF_AutoTransactionExportSettingId.Value = Nothing And Not HDF_AutoTransactionExportSettingId.Value = "") Then

				AutoTransactionExportSettingId = HDF_AutoTransactionExportSettingId.Value

			End If


			If (txtEmail.Text = "") Then
				If (TXT_FTPServerPath.Text = "") Then
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Please enter all FTP details or enter email id."
					TXT_FTPServerPath.Focus()
					Return
				ElseIf (txtFTPUsername.Text = "") Then
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Please enter all FTP details or enter email id."
					txtFTPUsername.Focus()
					Return
				ElseIf (txtFTPPassword.Text = "") Then
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Please enter all FTP details or enter email id."
					txtFTPPassword.Focus()
					Return
				End If
			End If

			'Dim checkidexists As Integer = 0
			'OBJMaster = New MasterBAL()
			'checkidexists = OBJMaster.CheckCompanyExist(DDL_Company.SelectedValue, AutoTransactionExportSettingId)

			'If checkidexists = -1 Then

			'	ErrorMessage.Visible = True
			'	ErrorMessage.InnerText = "Settings already exist for selected company."

			'	Return

			'End If

			OBJMaster = New MasterBAL()

			Dim result As Integer = 0

			OBJMaster = New MasterBAL()

			Dim ExecutionTime As DateTime = DateTime.Now.ToString("MM/dd/yyyy") & " " & Request.Form(txtExecutionTime.UniqueID)

			Dim DecimalType As Integer = 0
			If ddl_DecimalQTY.SelectedValue = 2 Then
				DecimalType = Convert.ToInt32(ddl_DecimalType.SelectedValue)
			End If

			result = OBJMaster.SaveUpdateAutoTransactionExportSetting(AutoTransactionExportSettingId, DDL_Company.SelectedValue, DDL_ExportOption.SelectedValue, CHK_Active.Checked, TXT_FTPServerPath.Text,
																	  txtFTPUsername.Text, txtFTPPassword.Text, txtEmail.Text, Convert.ToInt32(Session("PersonId")), DDL_TimeZone.SelectedValue, ExecutionTime,
																	  DDL_Separator.SelectedValue, DDL_CustomizedExportTemplate.SelectedValue, chk_IncludePreviouslyExportTransactions.Checked, chk_ExportOnlyNewTransactions.Checked,
																	  chk_FATransaction.Checked, chk_ExportZeroQtyTransactions.Checked, Convert.ToInt32(ddl_DecimalQTY.SelectedValue.ToString()), DecimalType, DDL_DateType.SelectedValue)


			If result > 0 Then
				If (AutoTransactionExportSettingId > 0) Then
					message.Visible = True
					message.InnerText = "Record saved"
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(AutoTransactionExportSettingId)
						CSCommonHelper.WriteLog("Modified", "Automatic Export Settings", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Automatic Export Settings", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					Response.Redirect(String.Format("~/Master/AutoTransactionExportSettings?AutoTransactionExportSettingId={0}&RecordIs=New", result))
				End If

			Else
				If (AutoTransactionExportSettingId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(AutoTransactionExportSettingId)
						CSCommonHelper.WriteLog("Modified", "Automatic Export Settings", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Automatic Export Settings update failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Automatic Export Settings update failed, please try again"
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Automatic Export Settings", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Automatic Export Settings Addition failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Automatic Export Settings Addition failed, please try again"
				End If

			End If
			DDL_Company.Focus()

		Catch ex As Exception

			If (AutoTransactionExportSettingId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(AutoTransactionExportSettingId)
					CSCommonHelper.WriteLog("Modified", "Automatic Export Settings", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Automatic Export Settings update failed. Exception is : " & ex.Message)
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(AutoTransactionExportSettingId)
					CSCommonHelper.WriteLog("Added", "Automatic Export Settings", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Automatic Export Settings Addition failed. Exception is : " & ex.Message)
				End If
			End If

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
			DDL_Company.Focus()

		Finally

			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

		End Try
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/AllAutoTransactionExportSettings")
	End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try
			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and ATES.CompanyId=" & Session("CustomerId")
			End If
			Dim CurrentAutoTransactionExportSettingId As Integer = HDF_AutoTransactionExportSettingId.Value

			OBJMaster = New MasterBAL()
			Dim AutoTransactionExportSettingId As Integer = OBJMaster.GetAutoTransactionExportSettingIdByCondition(CurrentAutoTransactionExportSettingId, True, False, False, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), strConditions)
			HDF_AutoTransactionExportSettingId.Value = AutoTransactionExportSettingId
			BindAutoTransactionExportSettingDetails(AutoTransactionExportSettingId)
		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try
			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and ATES.CompanyId=" & Session("CustomerId")
			End If
			Dim CurrentAutoTransactionExportSettingId As Integer = HDF_AutoTransactionExportSettingId.Value

			OBJMaster = New MasterBAL()
			Dim AutoTransactionExportSettingId As Integer = OBJMaster.GetAutoTransactionExportSettingIdByCondition(CurrentAutoTransactionExportSettingId, False, False, False, True, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), strConditions)
			HDF_AutoTransactionExportSettingId.Value = AutoTransactionExportSettingId
			BindAutoTransactionExportSettingDetails(AutoTransactionExportSettingId)
		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try
			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and ATES.CompanyId=" & Session("CustomerId")
			End If
			Dim CurrentAutoTransactionExportSettingId As Integer = HDF_AutoTransactionExportSettingId.Value

			OBJMaster = New MasterBAL()
			Dim AutoTransactionExportSettingId As Integer = OBJMaster.GetAutoTransactionExportSettingIdByCondition(CurrentAutoTransactionExportSettingId, False, False, True, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), strConditions)
			HDF_AutoTransactionExportSettingId.Value = AutoTransactionExportSettingId
			BindAutoTransactionExportSettingDetails(AutoTransactionExportSettingId)

		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try
			Dim strConditions As String = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and ATES.CompanyId=" & Session("CustomerId")
			End If
			Dim CurrentAutoTransactionExportSettingId As Integer = HDF_AutoTransactionExportSettingId.Value

			OBJMaster = New MasterBAL()
			Dim AutoTransactionExportSettingId As Integer = OBJMaster.GetAutoTransactionExportSettingIdByCondition(CurrentAutoTransactionExportSettingId, False, True, False, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), strConditions)
			HDF_AutoTransactionExportSettingId.Value = AutoTransactionExportSettingId
			BindAutoTransactionExportSettingDetails(AutoTransactionExportSettingId)

		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindCustomer(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()
			Dim CustomerId As Integer = 0


			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

			DDL_Company.DataSource = dtCust
			DDL_Company.DataTextField = "CustomerName"
			DDL_Company.DataValueField = "CustomerId"
			DDL_Company.DataBind()
			DDL_Company.Items.Insert(0, New ListItem("Select Company", "0"))

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				CustomerId = Convert.ToInt32(Session("CustomerId"))
				DDL_Company.SelectedValue = CustomerId
				'DDL_Company.Visible = False
				'divCompany.Visible = False
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Private Sub BindCustomizedExportTemplates(CompanyId As Integer, PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()

			Dim dtCustomizedExportTemplateNames As DataTable = New DataTable()
			Dim CustomerId As Integer = 0

			Dim strConditions As String = ""

			strConditions = " and CET.CompanyId=" & CompanyId

			dtCustomizedExportTemplateNames = OBJMaster.GetCustomizedExportTemplatesByCondition(strConditions, PersonId, RoleId)

			DDL_CustomizedExportTemplate.DataSource = dtCustomizedExportTemplateNames
			DDL_CustomizedExportTemplate.DataTextField = "CustomizedExportTemplateName"
			DDL_CustomizedExportTemplate.DataValueField = "CustomizedExportTemplateId"
			DDL_CustomizedExportTemplate.DataBind()

			DDL_CustomizedExportTemplate.Items.Insert(0, New ListItem("Select Template", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub


	Private Function CreateData(AutoTransactionExportSettingId As Integer) As String
		Try

			Dim data As String = ""

			data = "AutoTransactionExportSettingId = " & AutoTransactionExportSettingId & " ; " &
					"Company = " & DDL_Company.SelectedItem.Text.Replace(",", " ") & " ; " &
					"File Type = " & DDL_ExportOption.SelectedItem.Text.Replace(",", " ") & " ; " &
					"FTP Server Path  = " & TXT_FTPServerPath.Text.Replace(",", " ") & " ; " &
					"FTP Username  = " & txtFTPUsername.Text.Replace(",", " ") & " ; " &
					"FTP Password  = " & txtFTPPassword.Text.Replace(",", " ") & " ; " &
					"Email  = " & txtEmail.Text.Replace(",", " ") & " ; " &
					"Active = " & CHK_Active.Checked & " ; " &
					"Execution Time = " & txtExecutionTime.Text & " ; " &
					"Time Zone = " & DDL_TimeZone.SelectedItem.Text & " ; " &
					"Separator = " & DDL_Separator.SelectedItem.Text & " ; " &
					"Customized Export Template = " & DDL_CustomizedExportTemplate.SelectedItem.Text & " ; " &
					"Include Previously Export Transactions = " & chk_IncludePreviouslyExportTransactions.Checked & " ; " &
					"Only export transactions that have not been previously exported = " & chk_ExportOnlyNewTransactions.Checked & " ; " &
					"Date Type = " & DDL_DateType.SelectedItem.Text & " ; " &
					"Decimal Add or No = " & ddl_DecimalQTY.SelectedItem.ToString() & " ; " &
					 "Include FA = " & chk_FATransaction.Checked.ToString() & " ; " &
					"Decimal Type = " & ddl_DecimalType.SelectedItem.ToString() & " ; " &
					"Export Zero Quantity Transactions = " & chk_ExportZeroQtyTransactions.Checked & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Private Sub BindTimeZones()
		Try

			Dim dtTimeZones As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtTimeZones = OBJMaster.GetTimeZones()

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
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Protected Sub DDL_ExportOption_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			If (DDL_ExportOption.SelectedValue = "2") Then
				DDL_Separator.Visible = True
				seperatorDiv.Visible = True
			Else
				DDL_Separator.Visible = False
				seperatorDiv.Visible = False
			End If
		Catch ex As Exception
			log.Error("Error occurred in DDL_ExportOption_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Protected Sub DDL_Company_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try
			BindCustomizedExportTemplates(DDL_Company.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
		Catch ex As Exception
			log.Error("Error occurred in DDL_Company_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub

	Protected Sub ddl_DecimalQTY_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			If ddl_DecimalQTY.SelectedValue = 2 Then
				divDecimailType.Visible = True
			Else
				divDecimailType.Visible = False
			End If
		Catch ex As Exception
			log.Error("Error occurred in DDL_Company_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
		End Try
	End Sub
End Class
