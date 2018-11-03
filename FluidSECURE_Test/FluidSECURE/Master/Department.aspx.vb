Imports log4net
Imports log4net.Config

Public Class Department
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Department))

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
					GetCustomers(Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString())
					If (Not Request.QueryString("DeptId") = Nothing And Not Request.QueryString("DeptId") = "") Then
						txtDeptIdHide.Text = Request.QueryString("DeptId")
						BindDeptDetails(Request.QueryString("DeptId"))
						'btnAdd.Visible = True
						btnFirst.Visible = True
						btnNext.Visible = True
						btnprevious.Visible = True
						btnLast.Visible = True
						lblHeader.Text = "Edit Department Information"
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

						If DDL_SurType.SelectedValue = 0 Then
							lblDPercentage.Visible = False
							lblVPercentage.Visible = False

							txtVehicleSurcharge.Text = "0.0"
							txtDeptSurcharge.Text = "0.0"
						Else
							lblDPercentage.Visible = True
							lblVPercentage.Visible = True

							txtVehicleSurcharge.Text = "0.0"
							txtDeptSurcharge.Text = "0.0"
						End If

						lblHeader.Text = "Add Department Information"
					End If
					txtVehicleSurcharge.Attributes.Add("OnKeyPress", "return KeyPressProduct(event);")
					txtDeptSurcharge.Attributes.Add("OnKeyPress", "return KeyPressProduct(event);")
					txtDeptNo.Focus()
				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = IIf(ErrorMessage.Text <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Private Sub BindDeptDetails(DeptId As Integer)
		Try


			OBJMaster = New MasterBAL()
			Dim dtDept As DataTable = New DataTable()
			dtDept = OBJMaster.GetDeptbyId(DeptId)
			Dim cnt As Integer = 0
			If (dtDept.Rows.Count > 0) Then
				Dim isValid As Boolean = False
				If (Session("RoleName") = "GroupAdmin") Then
					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
					For Each drCusts As DataRow In dtCustOld.Rows
						If (drCusts("CustomerId") = dtDept.Rows(0)("CustomerId").ToString()) Then
							isValid = True
							Exit For
						End If

					Next
				End If

				If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtDept.Rows(0)("CustomerId").ToString()) Then

						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

						Return
					End If

				End If

				txtDeptNo.Text = dtDept.Rows(0)("NUMBER").ToString()
				txtDeptName.Text = dtDept.Rows(0)("NAME").ToString()
				txtAddress1.Text = dtDept.Rows(0)("ADDRESS1").ToString()
				txtAddress2.Text = dtDept.Rows(0)("ADDRESS2").ToString()
				txtAccNo.Text = dtDept.Rows(0)("ACCT_ID").ToString()
				txtExportCode.Text = dtDept.Rows(0)("ExportCode").ToString()

				DDL_Customer.SelectedValue = IIf(dtDept.Rows(0)("CustomerId").ToString() = "", 0, dtDept.Rows(0)("CustomerId").ToString())
				DDL_SurType.SelectedValue = IIf(dtDept.Rows(0)("SurchargeType").ToString() = "", 0, dtDept.Rows(0)("SurchargeType").ToString())

				If DDL_SurType.SelectedValue = 0 Then
					lblDPercentage.Visible = False
					lblVPercentage.Visible = False

					txtVehicleSurcharge.Text = dtDept.Rows(0)("VehicleSum").ToString()
					txtDeptSurcharge.Text = dtDept.Rows(0)("DeptSum").ToString()

					hdfVehSum.Value = dtDept.Rows(0)("VehicleSum").ToString()
					hdfDeptSum.Value = dtDept.Rows(0)("DeptSum").ToString()
				Else
					lblDPercentage.Visible = True
					lblVPercentage.Visible = True

					txtVehicleSurcharge.Text = dtDept.Rows(0)("VehPercentage").ToString()
					txtDeptSurcharge.Text = dtDept.Rows(0)("DeptPercentage").ToString()

					hdfVehPer.Value = dtDept.Rows(0)("VehPercentage").ToString()
					hdfDeptPer.Value = dtDept.Rows(0)("DeptPercentage").ToString()
				End If

				Dim strConditions As String = ""
				If (Not Session("DeptConditions") Is Nothing And Not Session("DeptConditions") = "") Then
					strConditions = Session("DeptConditions")
				Else
					If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
						strConditions = IIf(strConditions = "", " and DEPT.CustomerId=" & Session("CustomerId"), strConditions & " and DEPT.CustomerId=" & Session("CustomerId"))
					End If
				End If

				OBJMaster = New MasterBAL()

				HDF_TotalDept.Value = OBJMaster.GetDeptIdByCondition(DeptId, False, False, False, False, True, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)

				OBJMaster = New MasterBAL()
				Dim dtAllDept As DataTable = New DataTable()



				dtAllDept = OBJMaster.GetDeptbyConditions(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString())
				dtAllDept.PrimaryKey = New DataColumn() {dtAllDept.Columns(0)}
				Dim dr As DataRow = dtAllDept.Rows.Find(DeptId)
				If Not IsDBNull(dr) Then

					cnt = dtAllDept.Rows.IndexOf(dr) + 1

				End If
                If (HDF_TotalDept.Value = 1) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt >= HDF_TotalDept.Value) Then
                    btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				ElseIf (cnt <= 1) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt > 1 And cnt < HDF_TotalDept.Value) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
				lblof.Text = cnt & " of " & HDF_TotalDept.Value.ToString()

			Else
				ErrorMessage.Visible = True
				ErrorMessage.Text = "Data Not found. Please try again after some time."
			End If

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateBeforeData(dtDept)
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindDeptDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting department data, please try again later."
		Finally
			txtDeptNo.Focus()
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

			If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
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

			log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting customers, please try again later."

		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		ValidateAndSaveDept(False)
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/AllDepartments?Filter=Filter")
	End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("DeptConditions") Is Nothing) Then
				strConditions = Session("DeptConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and DEPT.CustomerId=" & Session("CustomerId"), strConditions & " and DEPT.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentDeptId As Integer = txtDeptIdHide.Text

			OBJMaster = New MasterBAL()
			Dim DeptId As Integer = OBJMaster.GetDeptIdByCondition(CurrentDeptId, True, False, False, False, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtDeptIdHide.Text = DeptId
			BindDeptDetails(DeptId)
		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("DeptConditions") Is Nothing) Then
				strConditions = Session("DeptConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and DEPT.CustomerId=" & Session("CustomerId"), strConditions & " and DEPT.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentDeptId As Integer = txtDeptIdHide.Text

			OBJMaster = New MasterBAL()
			Dim DeptId As Integer = OBJMaster.GetDeptIdByCondition(CurrentDeptId, False, False, False, True, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtDeptIdHide.Text = DeptId
			BindDeptDetails(DeptId)

		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("DeptConditions") Is Nothing) Then
				strConditions = Session("DeptConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and DEPT.CustomerId=" & Session("CustomerId"), strConditions & " and DEPT.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentDeptId As Integer = txtDeptIdHide.Text

			OBJMaster = New MasterBAL()
			Dim DeptId As Integer = OBJMaster.GetDeptIdByCondition(CurrentDeptId, False, False, True, False, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtDeptIdHide.Text = DeptId
			BindDeptDetails(DeptId)
		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("DeptConditions") Is Nothing) Then
				strConditions = Session("DeptConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and DEPT.CustomerId=" & Session("CustomerId"), strConditions & " and DEPT.CustomerId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentDeptId As Integer = txtDeptIdHide.Text

			OBJMaster = New MasterBAL()
			Dim DeptId As Integer = OBJMaster.GetDeptIdByCondition(CurrentDeptId, False, True, False, False, False, Convert.ToInt32(Session("PersonId")), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			txtDeptIdHide.Text = DeptId
			BindDeptDetails(DeptId)
		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub DDL_SurType_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try
			If DDL_SurType.SelectedValue = 0 Then
				lblDPercentage.Visible = False
				lblVPercentage.Visible = False

				txtVehicleSurcharge.Text = hdfVehSum.Value
				txtDeptSurcharge.Text = hdfDeptSum.Value
			Else
				lblDPercentage.Visible = True
				lblVPercentage.Visible = True

				txtVehicleSurcharge.Text = hdfVehPer.Value
				txtDeptSurcharge.Text = hdfVehPer.Value
			End If
		Catch ex As Exception
			log.Error("Error occurred in DDL_SurType_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while getting data, please try again later."
		End Try


	End Sub

	Private Function CreateAfterData() As String
		Try

			Dim data As String = "Department Number = " & txtDeptNo.Text.Replace(",", " ") & " ; " &
									"Department Name = " & txtDeptName.Text.Replace(",", " ") & " ; " &
									"Account Number = " & txtAccNo.Text.Replace(",", " ") & " ; " &
									"Address = " & txtAddress1.Text.Replace(",", " ") & " ; " &
									"Address 2 = " & txtAddress2.Text.Replace(",", " ") & " ; " &
									"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
									"Surcharge Type = " & DDL_SurType.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Vehicle Surcharge (Lum Sum) = " & IIf(DDL_SurType.SelectedValue = "0", txtVehicleSurcharge.Text, "0") & " ; " &
									"Department Surcharge  (Lum Sum) = " & IIf(DDL_SurType.SelectedValue = "0", txtVehicleSurcharge.Text, "0") & " ; " &
									"Vehicle Surcharge (%) = " & IIf(DDL_SurType.SelectedValue = "1", txtVehicleSurcharge.Text, "0") & " ; " &
									"Department Surcharge (%) = " & IIf(DDL_SurType.SelectedValue = "1", txtVehicleSurcharge.Text, "0") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateAfterData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Private Function CreateBeforeData(dtDept As DataTable) As String
		Try

			Dim data As String = "DepartmentId = " & dtDept.Rows(0)("DeptID").ToString() & " ; " &
									"Department Number = " & dtDept.Rows(0)("NUMBER").ToString().Replace(",", " ") & " ; " &
									"Department Name = " & dtDept.Rows(0)("NAME").ToString().Replace(",", " ") & " ; " &
									"Account Number = " & dtDept.Rows(0)("ACCT_ID").ToString().Replace(",", " ") & " ; " &
									"Address = " & dtDept.Rows(0)("ADDRESS1").ToString().Replace(",", " ") & " ; " &
									"Address 2 = " & dtDept.Rows(0)("ADDRESS2").ToString().Replace(",", " ") & " ; " &
									"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Export Code = " & dtDept.Rows(0)("ExportCode").ToString().Replace(",", " & ") & " ; " &
									"Surcharge Type = " & DDL_SurType.SelectedItem.Text.Replace(",", " ") & " ; " &
									"Vehicle Surcharge (Lum Sum) = " & dtDept.Rows(0)("VehicleSum").ToString() & " ; " &
									"Department Surcharge  (Lum Sum) = " & dtDept.Rows(0)("DeptSum").ToString() & " ; " &
									"Vehicle Surcharge (%) = " & dtDept.Rows(0)("VehPercentage").ToString() & " ; " &
									"Department Surcharge (%) = " & dtDept.Rows(0)("DeptPercentage").ToString() & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateBeforeData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
		ValidateAndSaveDept(True)
	End Sub

	Private Sub ValidateAndSaveDept(IsSaveAndAddNew As Boolean)

		Dim DeptId As Integer = 0
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			ErrorMessage.Visible = False
			message.Visible = False


			If (Not txtDeptIdHide.Text = Nothing And Not txtDeptIdHide.Text = "") Then

				DeptId = txtDeptIdHide.Text

			End If

            If txtDeptNo.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Department number and try again."
                txtDeptNo.Focus()
                Return
            End If

            If txtDeptName.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Department name and try again."
                txtDeptName.Focus()
                Return
            End If

            If DDL_Customer.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please select Company and try again."
                DDL_Customer.Focus()
                Return
            End If

            If DDL_SurType.SelectedIndex <> 0 Then
                If DDL_SurType.SelectedIndex <> 1 Then
                    ErrorMessage.Visible = True
                    ErrorMessage.Text = "Please select Surcharge and try again."
                    DDL_SurType.Focus()
                    Return
                End If
            End If

            Dim resultDecimal As Decimal = 0

            If (txtVehicleSurcharge.Text <> "" And Not (Decimal.TryParse(txtVehicleSurcharge.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Vehicle Surcharge as decimal and try again."
                txtVehicleSurcharge.Focus()
                Return
            End If

            If (txtDeptSurcharge.Text <> "" And Not (Decimal.TryParse(txtDeptSurcharge.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please enter Department Surcharge as decimal and try again."
                txtDeptSurcharge.Focus()
                Return
            End If

            Dim CheckIdExists As Integer = 0
			OBJMaster = New MasterBAL()
			CheckIdExists = OBJMaster.DeptIDExists(txtDeptNo.Text, DeptId, Convert.ToInt32(DDL_Customer.SelectedValue), txtDeptName.Text)
			Dim result As Integer = 0

			If CheckIdExists = -1 Then

				ErrorMessage.Visible = True
				ErrorMessage.Text = "Department Number Already Exists."

				Return

			ElseIf CheckIdExists = -2 Then

				ErrorMessage.Visible = True
				ErrorMessage.Text = "Department Name Already Exists."

				Return

			End If

			Dim VehSurSum As Decimal = 0.0
			Dim DeptSurSum As Decimal = 0.0
			Dim VehSurPer As Decimal = 0.0
			Dim DeptSurPer As Decimal = 0.0

			If DDL_SurType.SelectedValue = 0 Then
				VehSurSum = Convert.ToDecimal(txtVehicleSurcharge.Text)
				DeptSurSum = Convert.ToDecimal(txtDeptSurcharge.Text)
				hdfVehSum.Value = txtVehicleSurcharge.Text
				hdfDeptSum.Value = txtDeptSurcharge.Text
				hdfVehPer.Value = "0.0"
				hdfDeptPer.Value = "0.0"
			Else
				VehSurPer = Convert.ToDecimal(txtVehicleSurcharge.Text)
				DeptSurPer = Convert.ToDecimal(txtDeptSurcharge.Text)
				hdfVehSum.Value = "0.0"
				hdfDeptSum.Value = "0.0"
				hdfVehPer.Value = txtVehicleSurcharge.Text
				hdfDeptPer.Value = txtDeptSurcharge.Text
			End If

			OBJMaster = New MasterBAL()

			result = OBJMaster.SaveUpdateDept(DeptId, txtDeptName.Text, txtDeptNo.Text, txtAddress1.Text, txtAddress2.Text, txtAccNo.Text, DDL_Customer.SelectedValue, txtExportCode.Text, Convert.ToInt32(Session("PersonId")), VehSurSum, DeptSurSum, VehSurPer, DeptSurPer, Convert.ToInt32(DDL_SurType.SelectedValue))


			If result > 0 Then

				If (DeptId > 0) Then
					message.Visible = True
					message.Text = "Record saved"
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "DepartmentId = " & DeptId & " ; " & writtenData

						CSCommonHelper.WriteLog("Modified", "Department", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Department"))
					End If
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "DepartmentId = " & result & " ; " & writtenData

						CSCommonHelper.WriteLog("Added", "Department", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Department"))
					Else
						Response.Redirect(String.Format("~/Master/Department?DeptId={0}&RecordIs=New", result))
					End If
				End If

			Else
				If (DeptId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "DepartmentId = " & DeptId & " ; " & writtenData

						CSCommonHelper.WriteLog("Modified", "Department", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Department update failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.Text = "Department update failed, please try again"
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateAfterData()
						writtenData = "DepartmentId = " & result & " ; " & writtenData

						CSCommonHelper.WriteLog("Added", "Department", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Department Addition failed")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.Text = "Department Addition failed, please try again"
				End If

			End If
			txtDeptNo.Focus()
		Catch ex As Exception
			txtDeptNo.Focus()
			ErrorMessage.Visible = True
			ErrorMessage.Text = "Error occurred while saving record, please try again later."
			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
			If (DeptId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateAfterData()
					writtenData = "DepartmentId = " & DeptId & " ; " & writtenData

					CSCommonHelper.WriteLog("Modified", "Department", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Department update failed. Exception is : " & ex.Message)
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateAfterData()
					writtenData = "DepartmentId = " & 0 & " ; " & writtenData

					CSCommonHelper.WriteLog("Added", "Department", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Department Addition failed. Exception is : " & ex.Message)
				End If
			End If
		Finally

		End Try

	End Sub

End Class
