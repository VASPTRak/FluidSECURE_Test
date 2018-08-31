Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Owin
Imports Microsoft.AspNet.Identity.EntityFramework
Imports log4net
Imports log4net.Config
Imports System.IO


Public Class HubUserImport
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(HubUserImport))
	Dim OBJMaster As MasterBAL = New MasterBAL()

	Dim strLog As String = ""
	Dim currentDateTime As String = ""

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			ErrorMessage.Visible = False
			message.Visible = False
			LB_Error.Visible = False
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then
					BindTiming()
					GetCustomers(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					ddlCustomer.Focus()
				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
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

			If (Not Session("RoleName") = "SuperAdmin") Then
				ddlCustomer.SelectedIndex = 1
				ddlCustomer.Enabled = False
				ddlCustomer.Visible = False
				divCompanyImport.Visible = False
			End If

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				ddlCustomer.SelectedIndex = 1

			End If
			BindVehicles(Convert.ToInt32(ddlCustomer.SelectedValue))
		Catch ex As Exception

			log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			Dim fileExt As String

			fileExt = System.IO.Path.GetExtension(FU_Person.FileName)

			If (fileExt <> ".csv" And fileExt <> ".txt") Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Only .csv or .txt files allowed to upload!."

				Return
			End If

			Dim VehiclesAllowedToFuel As String = ""

			For Each item As GridViewRow In gv_Vehicles.Rows
				Dim CHK_Vehicle As CheckBox = TryCast(item.FindControl("CHK_Vehicle"), CheckBox)
				If (CHK_Vehicle.Checked = True Or gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber").ToString().ToLower().Contains("guest") = True) Then
					VehiclesAllowedToFuel = IIf(VehiclesAllowedToFuel = "", gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleName").ToString(), VehiclesAllowedToFuel & " ; " & gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleName").ToString())
				End If
			Next

			Dim AuthorizedFuelingTime As String = ""

			For Each item As GridViewRow In gv_FuelingTimes.Rows
				Dim CHK_FuelingTimes As CheckBox = TryCast(item.FindControl("CHK_FuelingTimes"), CheckBox)
				If (CHK_FuelingTimes.Checked = True) Then
					AuthorizedFuelingTime = IIf(AuthorizedFuelingTime = "", gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeText").ToString(), AuthorizedFuelingTime & " ; " & gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeText").ToString())
				End If
			Next


			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				CSCommonHelper.WriteLog("Import", "Personnel Hub", "File Name = " & FU_Person.FileName.Replace(",", " ") & " ; Company =  " & ddlCustomer.SelectedItem.Text.Replace(",", " ") &
										" ; Vehicles Allowed to Fuel = " & VehiclesAllowedToFuel & " ; Authorized Fueling Times =" & AuthorizedFuelingTime, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
			End If

			System.Threading.Thread.Sleep("5000")

			Dim allContent As String = New StreamReader(FU_Person.FileContent, Encoding.GetEncoding("iso-8859-1")).ReadToEnd()

			Dim returnCnts As String = ConvertStringIntoDatTableAndInsertData(allContent)



			message.Visible = True
			message.InnerText = returnCnts.Split(";")(0) & " Hub User imported successfully "

			If (strLog.Trim() <> "") Then
				LB_Error.Visible = True
				Session("Errorlogs") = strLog
				message.InnerText = message.InnerText + " , " + returnCnts.Split(";")(1) + " caused some errors."
			End If

		Catch ex As Exception
			message.InnerText = message.InnerText + " , Error occurred, Please try after some time."
			log.Error("Exception occurred on btnUpload_Click. Exception is : " & ex.Message)
		Finally
			ddlCustomer.Focus()
		End Try

	End Sub

	Protected Function ConvertStringIntoDatTableAndInsertData(data As String) As String
		'Dim insertDt As DataTable = New DataTable()
		Dim cnt As Integer = 0
		Dim ErrorCnt As Integer = 0
		Dim returnsCnt As String = ""

		'Dim pattern As String = "^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$"

		Try
			currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

		Catch ex As Exception

			currentDateTime = DateTime.Now.ToString()

		End Try

		Try
			Dim Lines As String() = data.Split(New Char() {ControlChars.Lf})
			Dim Fields As String()
			Fields = Lines(0).Split(New Char() {","c})
			Dim Cols As Integer = Fields.GetLength(0)
			Dim dt As New DataTable()
			dt.TableName = "PersonDetails"
			dt.Columns.Add("PinNumber", GetType(String))
			dt.Columns.Add("DepartmentNo", GetType(String))
			dt.Columns.Add("DepartmentId", GetType(String))
			dt.Columns.Add("LastName", GetType(String))
			dt.Columns.Add("FirstName", GetType(String))
			dt.Columns.Add("MI", GetType(String))
			'dt.Columns.Add("PhoneNumber", GetType(String))
			'dt.Columns.Add("Email", GetType(String))
			dt.Columns.Add("ExportCode", GetType(String))
			dt.Columns.Add("Active", GetType(String))
			'dt.Columns.Add("HubUser", GetType(String))
			dt.Columns.Add("FluidLimitperday", GetType(String))
			dt.Columns.Add("FluidLimitpertransaction", GetType(String))
			dt.Columns.Add("RowIndex", GetType(Integer))

			Dim Row As DataRow
			For i As Integer = 3 To Lines.GetLength(0) - 1
				Fields = Lines(i).Split(New Char() {","c})

				If (Fields.Length = dt.Columns.Count - 1) Then
					Row = dt.NewRow()
					For f As Integer = 0 To Cols - 1
						Row(f) = Fields(f).ToString().Replace("'", "").Trim()
					Next
					Row(10) = i + 1
					dt.Rows.Add(Row)

				ElseIf (Fields.Length < 10 And Fields.Length > 1) Then

					Row = dt.NewRow()
					For f As Integer = 0 To Fields.Length - 1
						Row(f) = Fields(f).ToString().Replace("'", "").Trim()
					Next

					For f As Integer = Fields.Length To 9
						Row(f) = ""
					Next

					Row(10) = i + 1
					dt.Rows.Add(Row)
				ElseIf (Fields.Length > 2) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Invalid input format. Incorrect number of columns for the row number " & (i + 1) & ". Please correct the data and retry!"
					ErrorCnt = ErrorCnt + 1
				End If
			Next

			Dim CheckpersonalEmailExist As Boolean = False
			OBJMaster = New MasterBAL()
			Dim dtDept As DataTable = New DataTable()

			dtDept = OBJMaster.GetDeptbyConditions(" and Dept.CustomerId = " & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())

			Dim rowIndex As Integer = 0
			Dim isDirty As Boolean = False
			For Each dr As DataRow In dt.Rows

				CheckpersonalEmailExist = False
				isDirty = False
				rowIndex = dr("RowIndex")

				If (dr("PinNumber") <> "") Then
					' Dim number As Long
					'If (dr("PinNumber").ToString()) Then
					If (dr("PinNumber").ToString().Length > 20) Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Personnel ID (" & dr("PinNumber") & ") is must be less than equal to 20 characters. Check Row  " & rowIndex & " & column 1 in uploaded file."
						isDirty = True
					Else
						Dim CheckPinNumberExists As Boolean = False
						OBJMaster = New MasterBAL()

						CheckPinNumberExists = OBJMaster.CheckPinNumberExist(dr("PinNumber"), 0, ddlCustomer.SelectedValue)

						If CheckPinNumberExists = True Then
							strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Personnel ID (" & dr("PinNumber") & ") is already exist. Check Row  " & rowIndex & " & column 1 in uploaded file."
							isDirty = True
						End If

					End If
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Personnel ID (" & dr("PinNumber") & ") is must be in number format. Check Row  " & rowIndex & " & column 1 in uploaded file."
					isDirty = True
				End If
				'Else
				'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Personnel ID field is required. Check Row  " & rowIndex & " & column 1 in uploaded file."
				'isDirty = True
				'End If

				Dim flagCheckDept As Integer = 0
				Dim DepartmentId As String = dr("DepartmentId")
				Dim strErrorDept As String = ""

				If (dr("DepartmentNo") <> "") Then

					Dim drDept() As DataRow = dtDept.Select("NUMBER='" & dr("DepartmentNo") & "'")

					Dim i As Integer

					dr("DepartmentId") = ""

					For i = 0 To drDept.GetUpperBound(0)
						dr("DepartmentId") = drDept(i)("DeptID")
					Next i

					If (dr("DepartmentId") = "") Then
						dr("DepartmentId") = DepartmentId
						flagCheckDept = 1
						strErrorDept = currentDateTime & "--" & "Person department number field not found. Check Row  " & rowIndex & " & column 2 in uploaded file."
					End If
				Else
					flagCheckDept = 2
					strErrorDept = currentDateTime & "--" & "Person department number field is required. Check Row  " & rowIndex & " & column 2 in uploaded file."
				End If

				If flagCheckDept = 1 Or flagCheckDept = 2 Then
					If (dr("DepartmentId") <> "") Then
						Dim drDept() As DataRow = dtDept.Select("NAME='" & dr("DepartmentId") & "'")

						Dim i As Integer

						dr("DepartmentId") = ""

						For i = 0 To drDept.GetUpperBound(0)
							dr("DepartmentId") = drDept(i)("DeptID")
						Next i

						If (dr("DepartmentId") = "") Then
							strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Person department name is not found. Check Row  " & rowIndex & " & column 3 in uploaded file."
							isDirty = True
						Else
							flagCheckDept = 0
						End If
					Else
						strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Person department name is required. Check Row  " & rowIndex & " & column 3 in uploaded file."
						isDirty = True
					End If
				End If

				If (dr("ExportCode").ToString().Length > 25) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Export Code (" & dr("ExportCode") & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 7 in uploaded file."
					isDirty = True
				End If


				If (dr("Active").ToString().ToUpper() = "Y" Or dr("Active").ToString().ToUpper() = "N") Then
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Active Flag (" & dr("Active") & ") is must be Y or N. Check Row  " & rowIndex & " & column 8 in uploaded file."
					isDirty = True
				End If

				If (dr("FluidLimitperday").ToString() <> "") Then
					Dim number As Long
					If (Int64.TryParse(dr("FluidLimitperday").ToString(), number)) Then
					Else
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fluid Limit per day (" & dr("FluidLimitperday") & ") is must be in number format. Check Row  " & rowIndex & " & column 9 in uploaded file."
						isDirty = True
					End If
				Else
					'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fluid Limit per day is blank. Fill this value on UI later.Check Row  " & rowIndex & " & column 7 in uploaded file."
				End If

				If (dr("FluidLimitpertransaction").ToString() <> "") Then
					Dim number As Long
					If (Int64.TryParse(dr("FluidLimitpertransaction").ToString(), number)) Then
					Else
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fluid Limit per transaction (" & dr("FluidLimitpertransaction") & ") is must be in number format. Check Row  " & rowIndex & " & column 10 in uploaded file."
						isDirty = True
					End If
				Else
					'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fluid Limit per transaction is blank. Fill this value on UI later.Check Row  " & rowIndex & " & column 8 in uploaded file."
				End If

				If (isDirty = False) Then
					Dim result As Integer = InsertRecord(dr, rowIndex)
					If (result = 1) Then
						cnt = cnt + 1
					End If
				Else
					ErrorCnt = ErrorCnt + 1
				End If

				'rowIndex = rowIndex + 1
			Next


			Return cnt & ";" & ErrorCnt

		Catch ex As Exception
			log.Error("Exception occured while importing file. Exception is : " & ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Exception occurred while importing file. Exception is : " & ex.Message
			Return "0;0"
		End Try
	End Function

	Private Function InsertRecord(dr As DataRow, rowIndex As Integer) As Integer
		Dim resultInt As Integer = 0
		Try
			Dim user = New ApplicationUser()
			Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()

			Dim ApprOn As DateTime
			Dim ApprBy As Integer

			If (dr("Active").ToString().ToUpper() = "Y") Then

				ApprOn = DateTime.Now
				ApprBy = Convert.ToInt32(Session("PersonId"))

			End If
			Dim EmailUserName As String = ""
			Dim PhoneNumber As String = ""


			OBJMaster = New MasterBAL()
			Dim HubPersonNumber As Integer = OBJMaster.GetAndUpdateLastHubPersonNumberEntry()

			EmailUserName = "u" & HubPersonNumber & "@FluidSecureHub.com"
			PhoneNumber = ""



            user = New ApplicationUser() With {
            .UserName = EmailUserName,
            .Email = EmailUserName,
            .PersonName = (dr("LastName") & " " & dr("FirstName") & " " & dr("MI")).ToString().Trim(),
            .DepartmentId = dr("DepartmentId"),
            .PhoneNumber = PhoneNumber,
            .SoftUpdate = "N",
            .CreatedDate = DateTime.Now,
            .CreatedBy = Convert.ToInt32(Session("PersonId")),
            .IsDeleted = False,
            .RoleId = "11df27ed-8d70-46a9-a925-7150326ffe75",
            .IsApproved = IIf(dr("Active").ToString().ToUpper() = "Y", True, False),
            .ApprovedBy = ApprBy,
            .ApprovedOn = ApprOn,
            .ExportCode = dr("ExportCode"),
            .IMEI_UDID = "",
            .CustomerId = Convert.ToInt32(ddlCustomer.SelectedValue),
            .SendTransactionEmail = False,
            .RequestFrom = "W",
            .FuelLimitPerTxn = Nothing,
            .FuelLimitPerDay = Nothing,
            .PreAuth = Nothing,
            .PinNumber = dr("PinNumber"),
            .IsFluidSecureHub = False,
            .IsUserForHub = 1,
            .PasswordResetDate = DateTime.Now,
            .FOBNumber = "",
            .AdditionalEmailId = "",
            .IsPersonnelPINRequire = False,
            .BluetoothCardReader = "",
            .PrinterName = "",
            .PrinterMacAddress = "",
            .HubSiteName = "",
            .BluetoothCardReaderMacAddress = "",
       .LFBluetoothCardReader = "",
       .LFBluetoothCardReaderMacAddress = "",
       .VeederRootMacAddress = "",
       .CollectDiagnosticLogs = False,
       .IsVehicleHasFob = False,
       .IsPersonHasFob = False,
       .IsTermConditionAgreed = False,
       .DateTimeTermConditionAccepted = Nothing,
      .IsGateHub = False
       }
			If (dr("FluidLimitpertransaction") = "") Then
				user.FuelLimitPerTxn = Nothing
			Else
				user.FuelLimitPerTxn = Convert.ToInt32(dr("FluidLimitpertransaction"))
			End If

			If (dr("FluidLimitperday") = "") Then
				user.FuelLimitPerDay = Nothing
			Else
				user.FuelLimitPerDay = Convert.ToInt32(dr("FluidLimitperday"))
			End If

			Dim result As IdentityResult = manager.Create(user, "FluidSecure*123")
			If result.Succeeded Then
				resultInt = 1
				Dim PersonId As Integer
				OBJMaster = New MasterBAL()
				Dim dtPerson As DataTable = New DataTable()
				dtPerson = OBJMaster.GetPersonDetailByUniqueUserId(user.Id)
				PersonId = dtPerson.Rows(0)("PersonId")
				Try
					'Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

					'dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
					'dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
					'dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
					'dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))

					'Dim dtStoredVehicle As DataTable = New DataTable()
					'dtStoredVehicle = OBJMaster.GetVehicleByCondition(" And V.CustomerId=" & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())

					'For Each item As DataRow In dtStoredVehicle.Rows

					'    Dim drVehicle As DataRow = dtVehicle.NewRow()
					'    drVehicle("PersonId") = PersonId
					'    drVehicle("VehicleId") = item("VehicleId").ToString()
					'    drVehicle("CreatedDate") = DateTime.Now
					'    drVehicle("CreatedBy") = Convert.ToInt32(Session("PersonId"))
					'    dtVehicle.Rows.Add(drVehicle)

					'Next


					'OBJMaster.InsertPersonVehicleMapping(dtVehicle, PersonId)

					SaveOtherMapping(PersonId)
				Catch ex As Exception
					log.Error("Exception occured while importing file. Exception Is :   " & ex.Message)

					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Error occured while importing row " & rowIndex & ". Error is " & ex.Message
				End Try

				'Try

				'    'delete and add timing
				'    Dim dtTimings As DataTable = New DataTable("dtPersonSiteTimings")
				'    dtTimings.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
				'    dtTimings.Columns.Add("TimeId", System.Type.[GetType]("System.String"))

				'    Dim dtPersonTimings As DataTable = New DataTable()

				'    dtPersonTimings = OBJMaster.GetTiming()

				'    For Each item As DataRow In dtPersonTimings.Rows

				'        Dim drTimings As DataRow = dtTimings.NewRow()
				'        drTimings("PersonId") = PersonId
				'        drTimings("TimeId") = item("TimeId").ToString()
				'        dtTimings.Rows.Add(drTimings)

				'    Next

				'    OBJMaster.InsertPersonTimings(dtTimings, PersonId)
				'Catch ex As Exception
				'    log.Error("Exception occured while importing file. Exception is : " & ex.Message)

				'    strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Error occured while importing row " & rowIndex & ". Error is " & ex.Message
				'End Try
			Else
				strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Error occured while importing row " & rowIndex & ". Error is " & result.Errors(0).Replace(" '", "")
			End If

			Return resultInt

		Catch ex As Exception
			log.Error("Exception occured while importing file. Exception is : " & ex.Message)

			strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Error occured while importing row " & rowIndex & ". Error is " & ex.Message

			Return resultInt

		End Try

	End Function

	Protected Sub LB_Error_Click(sender As Object, e As EventArgs)

		Dim ms As New MemoryStream()
		Dim tw As TextWriter = New StreamWriter(ms)
		tw.WriteLine(Session("Errorlogs"))
		tw.Flush()
		Dim bytes As Byte() = ms.ToArray()
		ms.Close()
		Response.Clear()
		Response.ContentType = "application/force-download"
		Response.AddHeader("content-disposition", "attachment;filename=Errorlogs.txt")
		Response.BinaryWrite(bytes)
		Response.[End]()

	End Sub

	Protected Sub lnkTemplate_Click(sender As Object, e As EventArgs)

		Dim path As String = Server.MapPath("\Content\Templates\HubUserImportTemplate.csv")
		Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)
		If file.Exists Then 'set appropriate headers
			Response.Clear()
			Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
			Response.AddHeader("Content-Length", file.Length.ToString())
			Response.ContentType = "application/octet-stream"
			Response.WriteFile(file.FullName)
			Response.End()
		Else
			Response.Write("This file does not exist.")
		End If

	End Sub

	Protected Sub btnCancelFuelingTimes_Click(sender As Object, e As EventArgs)
		BindTiming()
	End Sub

	Protected Sub btnCloseVehicle_Click(sender As Object, e As EventArgs)
		Try

			BindVehicles(Convert.ToInt32(ddlCustomer.SelectedValue))
		Catch ex As Exception
			log.Error("Error occurred in btnCloseVehicle_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub BindVehicles(customerId As Integer)

		Try

			Dim dtVehicles As DataTable = New DataTable()
			OBJMaster = New MasterBAL()
			dtVehicles = OBJMaster.GetVehicleByCondition(" and v.CustomerId=" + customerId.ToString(), Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			gv_Vehicles.DataSource = dtVehicles
			gv_Vehicles.DataBind()


			If customerId = 0 Then
				lblVehicleMessage.Text = "Please select Company and then select vehicles."
				lblVehicleMessage.Visible = True
				gv_Vehicles.Visible = False
			ElseIf customerId <> 0 And dtVehicles.Rows.Count <> 0 Then

				lblVehicleMessage.Visible = False
				gv_Vehicles.Visible = True
			ElseIf customerId <> 0 And dtVehicles.Rows.Count = 0 Then
				lblVehicleMessage.Text = "Vehicles not found for selected Company."
				lblVehicleMessage.Visible = True
				gv_Vehicles.Visible = False
			End If


		Catch ex As Exception

			log.Error("Error occurred in BindVehicles Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting vehicles, please try again later."

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
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			BindVehicles(Convert.ToInt32(ddlCustomer.SelectedValue))
		Catch ex As Exception
			log.Error("Error occurred in ddlCustomer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Public Sub SaveOtherMapping(PersonId As Integer)

		Try

			Dim dtVehicle As DataTable = New DataTable("dtPersonAndVehicle")

			dtVehicle.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
			dtVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
			dtVehicle.Columns.Add("CreatedDate", System.Type.[GetType]("System.DateTime"))
			dtVehicle.Columns.Add("CreatedBy", System.Type.[GetType]("System.Int32"))

			For Each item As GridViewRow In gv_Vehicles.Rows
				Dim CHK_Vehicle As CheckBox = TryCast(item.FindControl("CHK_Vehicle"), CheckBox)
				If (CHK_Vehicle.Checked = True Or gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleNumber").ToString().ToLower().Contains("guest") = True) Then
					Dim dr As DataRow = dtVehicle.NewRow()
					dr("PersonId") = PersonId
					dr("VehicleId") = gv_Vehicles.DataKeys(item.RowIndex).Values("VehicleId").ToString()
					dr("CreatedDate") = DateTime.Now
					dr("CreatedBy") = Convert.ToInt32(Session("PersonId"))
					dtVehicle.Rows.Add(dr)
				End If
			Next


			OBJMaster.InsertPersonVehicleMapping(dtVehicle, PersonId)

			'delete and add timing
			Dim dtTimings As DataTable = New DataTable("dtPersonSiteTimings")
			dtTimings.Columns.Add("PersonId", System.Type.[GetType]("System.Int32"))
			dtTimings.Columns.Add("TimeId", System.Type.[GetType]("System.String"))


			For Each item As GridViewRow In gv_FuelingTimes.Rows
				Dim CHK_FuelingTimes As CheckBox = TryCast(item.FindControl("CHK_FuelingTimes"), CheckBox)
				If (CHK_FuelingTimes.Checked = True) Then
					Dim dr As DataRow = dtTimings.NewRow()
					dr("PersonId") = PersonId
					dr("TimeId") = gv_FuelingTimes.DataKeys(item.RowIndex).Values("TimeId").ToString()
					dtTimings.Rows.Add(dr)
				End If
			Next
			OBJMaster.InsertPersonTimings(dtTimings, PersonId)
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while mapping. Error is {0}.", ex.Message))
		End Try

	End Sub


End Class