Imports log4net.Config
Imports log4net

Public Class VehiclePersonMapping
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(VehiclePersonMapping))

	Dim OBJMaster As MasterBAL
	Shared beforeVehicles As String
	Shared afterVehicles As String
	Shared beforeData As String
	Shared PersonIdForDeSelect As String
	Shared UniqueUserId As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			showGrid.Visible = False
			showDepartments.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")
			Else

				If (Not Request.QueryString("PersonId") = Nothing And Not Request.QueryString("PersonId") = "" And Not Request.QueryString("UniqueUserId") = Nothing And Not Request.QueryString("UniqueUserId") = "") Then
					ViewState("PersonId") = Request.QueryString("PersonId")
					ViewState("UniqueUserId") = Request.QueryString("UniqueUserId")

					If (Not IsPostBack) Then
						beforeVehicles = ""
						lblHeader.Text = "Add Vehicles Allowed to Fuel"
						gv_Vehicles.AllowPaging = False
						OBJMaster = New MasterBAL()
						Dim dtPersonnel As DataTable = New DataTable()
						dtPersonnel = OBJMaster.GetPersonnelByPersonIdAndId(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))
					Dim customerId As Integer = Convert.ToInt32(dtPersonnel.Rows(0)("CustomerId").ToString())

						If (Not Session("RoleName") = "SuperAdmin") Then

							Dim dtCustOld As DataTable = New DataTable()

							dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

							If (dtCustOld.Rows(0)("CustomerId").ToString() <> customerId.ToString()) Then

								ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

								Return
							End If

						End If

						ViewState("CurrentCustomerId") = customerId
						BindVehicles(ViewState("CurrentCustomerId"))
						Dim dtPersonVehicleMapping As DataTable = New DataTable()
						OBJMaster = New MasterBAL()
						dtPersonVehicleMapping = OBJMaster.GetPersonVehicleMapping(Request.QueryString("PersonId"))
						BindDept()
						Session("dtPersonVehicleMapping") = dtPersonVehicleMapping

						BindGridCheckboxes(True)

						For Each dr As DataRow In dtPersonVehicleMapping.Rows
							If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
								beforeVehicles = IIf(beforeVehicles = "", dr("VehicleNumber").Replace(",", " "), beforeVehicles & ";" & dr("VehicleNumber").Replace(",", " "))
							End If
						Next

						If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
							beforeData = CreateData(Request.QueryString("PersonId"), True)
						End If

						gv_Vehicles.AllowPaging = True
						BindVehicles(ViewState("CurrentCustomerId"))
						BindGridCheckboxes(False)
						RBL_Options_SelectedIndexChanged(Nothing, Nothing)
					Else
						Dim CheckBoxIndex As Integer
						Dim CheckedBoxArray As ArrayList
						Dim UnCheckedBoxArray As ArrayList
						If ViewState("CheckedBoxArray") IsNot Nothing Then
							CheckedBoxArray = DirectCast(ViewState("CheckedBoxArray"), ArrayList)
						Else
							CheckedBoxArray = New ArrayList()
						End If
						If ViewState("UnCheckedBoxArray") IsNot Nothing Then
							UnCheckedBoxArray = DirectCast(ViewState("UnCheckedBoxArray"), ArrayList)
						Else
							UnCheckedBoxArray = New ArrayList()
						End If
						Dim dtPersonVehicleMapping As DataTable = New DataTable()
						dtPersonVehicleMapping = Session("dtPersonVehicleMapping")

						For i As Integer = 0 To gv_Vehicles.Rows.Count - 1

							'CheckBoxIndex = gv_Vehicles.PageSize * (gv_Vehicles.PageIndex) + (i + 1)
							CheckBoxIndex = gv_Vehicles.DataKeys(i).Values("VehicleId").ToString()
							'Dim CheckBoxIndex As Integer = gv_Vehicles.PageSize * (gv_Vehicles.PageIndex) + (e.Row.RowIndex + 1)

							Dim dtTemp As DataTable = New DataTable()
							Dim dv As DataView = dtPersonVehicleMapping.DefaultView
							dv.RowFilter = "VehicleId = " & gv_Vehicles.DataKeys(i).Values("VehicleId").ToString()
							dtTemp = dv.ToTable()

							If (dtTemp.Rows.Count > 0) Then
								If CheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
									Dim chk As CheckBox = DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox)
									If (chk.Checked = False) Then
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
										CheckedBoxArray.Remove(CheckBoxIndex)
										UnCheckedBoxArray.Add(CheckBoxIndex)
									Else
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
									End If
								ElseIf UnCheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
									Dim chk As CheckBox = DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox)
									If (chk.Checked = True) Then
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
										CheckedBoxArray.Add(CheckBoxIndex)
										UnCheckedBoxArray.Remove(CheckBoxIndex)
									Else
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
									End If
								Else
									DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
									CheckedBoxArray.Add(CheckBoxIndex)
								End If
							Else
								If UnCheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
									Dim chk As CheckBox = DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox)
									If (chk.Checked = True) Then
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
										UnCheckedBoxArray.Remove(CheckBoxIndex)
										CheckedBoxArray.Add(CheckBoxIndex)
									Else
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
									End If
								ElseIf CheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
									Dim chk As CheckBox = DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox)
									If (chk.Checked = False) Then
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
										CheckedBoxArray.Remove(CheckBoxIndex)
										UnCheckedBoxArray.Add(CheckBoxIndex)
									Else
										DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
									End If
								Else
									DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
									UnCheckedBoxArray.Add(CheckBoxIndex)
								End If
							End If

						Next
						ViewState("CheckedBoxArray") = CheckedBoxArray
						ViewState("UnCheckedBoxArray") = UnCheckedBoxArray

						RBL_Options_SelectedIndexChanged(Nothing, Nothing)

					End If
				Else

				End If

			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub


	Private Function BindVehicles(customerId As Integer) As DataTable
		Dim dtVehicles As DataTable = New DataTable()
		Try

			Dim strConditions As String = ""

			'If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
			strConditions = " and V.CustomerId=" & customerId
			'End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				If (DDL_ColumnName.SelectedValue = "Name") Then
					strConditions = IIf(strConditions = "", " and D." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and D." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
				Else
					strConditions = IIf(strConditions = "", " and V." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and V." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
				End If
			End If


			OBJMaster = New MasterBAL()
			dtVehicles = OBJMaster.GetVehicleByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtVehicles") = dtVehicles

			gv_Vehicles.DataSource = dtVehicles
			gv_Vehicles.DataBind()

		Catch ex As Exception

			log.Error("Error occurred in BindVehicles Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Vehicle, please try again later."

		End Try
		Return dtVehicles

	End Function

	Protected Sub OnPaging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
		Try

			Dim dtVehicles As DataTable = Session("dtVehicles")
			gv_Vehicles.PageIndex = e.NewPageIndex
			gv_Vehicles.DataSource = dtVehicles
			gv_Vehicles.DataBind()


			Dim dtPersonVehicleMapping As DataTable = New DataTable()

			dtPersonVehicleMapping = Session("dtPersonVehicleMapping")

			Dim CheckedBoxArray As ArrayList
			Dim UnCheckedBoxArray As ArrayList
			If ViewState("CheckedBoxArray") IsNot Nothing Then
				CheckedBoxArray = DirectCast(ViewState("CheckedBoxArray"), ArrayList)
			Else
				CheckedBoxArray = New ArrayList()
			End If
			If ViewState("UnCheckedBoxArray") IsNot Nothing Then
				UnCheckedBoxArray = DirectCast(ViewState("UnCheckedBoxArray"), ArrayList)
			Else
				UnCheckedBoxArray = New ArrayList()
			End If
			For i As Integer = 0 To gv_Vehicles.Rows.Count - 1

				Dim CheckBoxIndex As Integer = gv_Vehicles.DataKeys(i).Values("VehicleId").ToString() 'gv_Vehicles.PageSize * (gv_Vehicles.PageIndex) + (i + 1)

				If CheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
					DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True

				ElseIf UnCheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
					DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
				Else

					Dim dtTemp As DataTable = New DataTable()
					Dim dv As DataView = dtPersonVehicleMapping.DefaultView
					dv.RowFilter = "VehicleId = " & gv_Vehicles.DataKeys(i).Values("VehicleId").ToString()
					dtTemp = dv.ToTable()
					If (dtTemp.Rows.Count > 0) Then
						DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
						CheckedBoxArray.Add(CheckBoxIndex)
					Else
						DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
						UnCheckedBoxArray.Add(CheckBoxIndex)
					End If
				End If
			Next
			ViewState("CheckedBoxArray") = CheckedBoxArray
			ViewState("UnCheckedBoxArray") = UnCheckedBoxArray

		Catch ex As Exception
			log.Error("Error occurred in OnPaging Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while on paging, please try again later."
		End Try
	End Sub

	Private Sub BindGridCheckboxes(IsNew As Boolean)
		Try

			Dim dtPersonVehicleMapping As DataTable = New DataTable()

			dtPersonVehicleMapping = Session("dtPersonVehicleMapping")

			Dim CheckedBoxArray As ArrayList
			Dim UnCheckedBoxArray As ArrayList
			If ViewState("CheckedBoxArray") IsNot Nothing Then
				CheckedBoxArray = DirectCast(ViewState("CheckedBoxArray"), ArrayList)
			Else
				CheckedBoxArray = New ArrayList()
			End If
			If ViewState("UnCheckedBoxArray") IsNot Nothing Then
				UnCheckedBoxArray = DirectCast(ViewState("UnCheckedBoxArray"), ArrayList)
			Else
				UnCheckedBoxArray = New ArrayList()
			End If
			For i As Integer = 0 To gv_Vehicles.Rows.Count - 1

				Dim CheckBoxIndex As Integer = gv_Vehicles.DataKeys(i).Values("VehicleId").ToString() 'gv_Vehicles.PageSize * (gv_Vehicles.PageIndex) + (i + 1)

				Dim dtTemp As DataTable = New DataTable()
				Dim dv As DataView = dtPersonVehicleMapping.DefaultView
				dv.RowFilter = "VehicleId = " & gv_Vehicles.DataKeys(i).Values("VehicleId").ToString()
				dtTemp = dv.ToTable()

				If (dtTemp.Rows.Count > 0) Then
					If CheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
						Dim chk As CheckBox = DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox)
						If (chk.Checked = False And IsNew = True) Then
							DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
							CheckedBoxArray.Remove(CheckBoxIndex)
							UnCheckedBoxArray.Add(CheckBoxIndex)
						Else
							DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
						End If
					ElseIf UnCheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
						DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
					Else
						DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
						CheckedBoxArray.Add(CheckBoxIndex)
					End If
				Else
					If UnCheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
						Dim chk As CheckBox = DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox)
						If (chk.Checked = True And IsNew = True) Then
							DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
							UnCheckedBoxArray.Remove(CheckBoxIndex)
							CheckedBoxArray.Add(CheckBoxIndex)
						Else
							DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
						End If
					ElseIf CheckedBoxArray.IndexOf(CheckBoxIndex) <> -1 Then
						DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = True
					Else
						DirectCast(gv_Vehicles.Rows(i).Cells(0).FindControl("CheckBox1"), CheckBox).Checked = False
						UnCheckedBoxArray.Add(CheckBoxIndex)
					End If
				End If

			Next

			ViewState("CheckedBoxArray") = CheckedBoxArray
			ViewState("UnCheckedBoxArray") = UnCheckedBoxArray
		Catch ex As Exception
			log.Error("Error occurred in BindGridCheckboxes Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while binding mapping, please try again later."
		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs)
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			afterVehicles = ""

			If (RBL_Options.SelectedValue = 1) Then

				Dim dtVehicles As DataTable = New DataTable()
				OBJMaster = New MasterBAL()
				dtVehicles = OBJMaster.GetVehicleByCondition(" and v.CustomerId=" & ViewState("CurrentCustomerId"), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

				Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

				dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
				dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
				dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
				dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))


				For Each drVehicles As DataRow In dtVehicles.Rows

					Dim dr As DataRow = dtVehicle.NewRow()
					dr("PersonId") = ViewState("PersonId")
					dr("VehicleId") = drVehicles("VehicleId").ToString()
					dr("CreatedDate") = DateTime.Now
					dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
					dtVehicle.Rows.Add(dr)

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						afterVehicles = IIf(afterVehicles = "", drVehicles("VehicleNumber").Replace(",", " "), afterVehicles & ";" & drVehicles("VehicleNumber").Replace(",", " "))
					End If
				Next


				OBJMaster.InsertPersonVehicleMapping(dtVehicle, ViewState("PersonId"))

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(ViewState("PersonId"), False)
					CSCommonHelper.WriteLog("Modifed", "PersonnelVehicleMapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
				End If
				Response.Redirect(String.Format("Personnel?PersonId={0}&UniqueUserId={1}", ViewState("PersonId"), ViewState("UniqueUserId")), False)

			ElseIf (RBL_Options.SelectedValue = 2) Then

				If DDL_Dept.SelectedIndex = 0 Then
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Please select Department and try again."
					DDL_Dept.Focus()
					Return
				End If

				Dim dtVehicles As DataTable = New DataTable()
				OBJMaster = New MasterBAL()
				dtVehicles = OBJMaster.GetVehicleByCondition(" and v.CustomerId=" & ViewState("CurrentCustomerId") & " and v.DepartmentId = " & DDL_Dept.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

				Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

				dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
				dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
				dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
				dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))


				For Each drVehicles As DataRow In dtVehicles.Rows

					Dim dr As DataRow = dtVehicle.NewRow()
					dr("PersonId") = ViewState("PersonId")
					dr("VehicleId") = drVehicles("VehicleId").ToString()
					dr("CreatedDate") = DateTime.Now
					dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
					dtVehicle.Rows.Add(dr)

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						afterVehicles = IIf(afterVehicles = "", drVehicles("VehicleNumber").Replace(",", " "), afterVehicles & ";" & drVehicles("VehicleNumber").Replace(",", " "))
					End If
				Next


				OBJMaster.InsertPersonVehicleMapping(dtVehicle, ViewState("PersonId"))

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(ViewState("PersonId"), False)
					CSCommonHelper.WriteLog("Modifed", "PersonnelVehicleMapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
				End If
				Response.Redirect(String.Format("Personnel?PersonId={0}&UniqueUserId={1}", ViewState("PersonId"), ViewState("UniqueUserId")), False)

			ElseIf (RBL_Options.SelectedValue = 4) Then
				PersonIdForDeSelect = ViewState("PersonId")
				UniqueUserId = ViewState("UniqueUserId")
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(ViewState("PersonId"), False)
					CSCommonHelper.WriteLog("Modifed", "PersonnelVehicleMapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
				End If
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckConfirm();", True)

			Else

				Dim dtPersonnel As DataTable = New DataTable()
				OBJMaster = New MasterBAL()
				dtPersonnel = OBJMaster.GetPersonnelByPersonIdAndId(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))
				'gv_Vehicles.AllowPaging = False
				'BindVehicles(Convert.ToInt32(dtPersonnel.Rows(0)("CustomerId").ToString()))
				'BindGridCheckboxes(False)

				Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

				dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
				dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
				dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
				dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))
				Dim CheckedBoxArray As ArrayList

				If ViewState("CheckedBoxArray") IsNot Nothing Then
					CheckedBoxArray = DirectCast(ViewState("CheckedBoxArray"), ArrayList)
				Else
					CheckedBoxArray = New ArrayList()
				End If
				Dim dtVehicleTemp As DataTable = New DataTable()
				dtVehicleTemp = Session("dtVehicles")

				For Each element As String In CheckedBoxArray
					Dim dtTemp As DataTable = New DataTable()
					Dim dv As DataView = dtVehicleTemp.DefaultView
					dv.RowFilter = "VehicleId = " & element
					dtTemp = dv.ToTable()

					'Dim index As Integer = element - 1
					'Dim drTemp As DataRow = dtTemp.Rows(0)
					Dim dr As DataRow = dtVehicle.NewRow()
					dr("PersonId") = ViewState("PersonId")
					dr("VehicleId") = Convert.ToInt32(element) 'drTemp("VehicleId").ToString()
					dr("CreatedDate") = DateTime.Now
					dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
					dtVehicle.Rows.Add(dr)

					If dtTemp.Rows.Count > 0 Then
						If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
							afterVehicles = IIf(afterVehicles = "", dtTemp.Rows(0)("VehicleNumber").Replace(",", " "), afterVehicles & ";" & dtTemp.Rows(0)("VehicleNumber").Replace(",", " "))
						End If
					End If

				Next

				OBJMaster.InsertPersonVehicleMapping(dtVehicle, ViewState("PersonId"))

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(ViewState("PersonId"), False)
					CSCommonHelper.WriteLog("Modifed", "PersonnelVehicleMapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
				End If

				Response.Redirect(String.Format("Personnel?PersonId={0}&UniqueUserId={1}", ViewState("PersonId"), ViewState("UniqueUserId")), False)
			End If

			'If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
			'    Dim writtenData As String = CreateData(ViewState("PersonId"), False)
			'    CSCommonHelper.WriteLog("Modifed", "PersonnelVehicleMapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
			'End If

			'message.Visible = True
			'message.InnerText = "Record Saved"

			'Response.Redirect(String.Format("Personnel?PersonId={0}&UniqueUserId={1}", ViewState("PersonId"), ViewState("UniqueUserId")), False)

		Catch ex As Exception
			log.Error("Error occurred In btnSave_Click Exception Is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while saving mapping, please try again later."
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData(ViewState("PersonId"), False)
				CSCommonHelper.WriteLog("Modifed", "PersonnelVehicleMapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Personnel Vehicle Mapping failed." + ex.Message + "'")
			End If
		End Try

	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
		Response.Redirect(String.Format("Personnel?PersonId={0}&UniqueUserId={1}", ViewState("PersonId"), ViewState("UniqueUserId")), False)
	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
		Try
			BindVehicles(ViewState("CurrentCustomerId"))
			BindGridCheckboxes(False)

		Catch ex As Exception
			log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub RBL_Options_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			If (RBL_Options.SelectedValue = "1") Then

			ElseIf (RBL_Options.SelectedValue = "2") Then
				showDepartments.Visible = True
				showGrid.Visible = False
			ElseIf (RBL_Options.SelectedValue = "3") Then
				showDepartments.Visible = False
				showGrid.Visible = True
			End If
		Catch ex As Exception
			log.Error("Error occurred in RBL_Options_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindDept()
		Try

			OBJMaster = New MasterBAL()
			Dim dtDept As DataTable = New DataTable()

			dtDept = OBJMaster.GetDeptbyConditions(" and DEPT.CustomerId = " & ViewState("CurrentCustomerId"), Session("PersonId").ToString(), Session("RoleId").ToString())

			DDL_Dept.DataSource = dtDept
			DDL_Dept.DataValueField = "DeptId"
			DDL_Dept.DataTextField = "Name"
			DDL_Dept.DataBind()
			DDL_Dept.Items.Insert(0, New ListItem("Select Department", "0"))

		Catch ex As Exception

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting departments , please try again later."

			log.Error("Error occurred in BindDept Exception is :" + ex.Message)

		End Try
	End Sub

	Private Function CreateData(PersonId As Integer, IsBefore As Boolean) As String
		Try
			Dim data As String = ""
			If (IsBefore = True) Then
				data = "PersonId = " & PersonId & " ; " &
									 "Vehicles Allowed to Fuel = " & beforeVehicles & " ; "
			Else
				data = "PersonId = " & PersonId & " ; " &
									 "Vehicles Allowed to Fuel = " & afterVehicles & " ; "
			End If

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeSelectAllVehicle() As String
		Try

			Dim OBJMaster = New MasterBAL()

			OBJMaster = New MasterBAL()

			Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

			dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
			dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
			dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
			dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))

			OBJMaster.InsertPersonVehicleMapping(dtVehicle, PersonIdForDeSelect)
			Return PersonIdForDeSelect & " : " & UniqueUserId
		Catch ex As Exception
			log.Error("Error occurred in DeSelectAllVehicle Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

End Class
