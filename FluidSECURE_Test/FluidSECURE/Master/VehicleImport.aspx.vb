Imports System.IO
Imports log4net.Config
Imports log4net

Public Class VehicleImport
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(VehicleImport))
	Dim OBJMaster As MasterBAL = New MasterBAL()

	Dim strLog As String = ""

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
				divCompany.Visible = False
			End If

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				ddlCustomer.SelectedIndex = 1

			End If

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

			fileExt = System.IO.Path.GetExtension(FU_Vehicles.FileName)

			If (fileExt <> ".csv" And fileExt <> ".txt") Then
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Only .csv or .txt files allowed to upload!."

				Return
			End If

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				CSCommonHelper.WriteLog("Import", "Vehicles", "File Name = " & FU_Vehicles.FileName.Replace(",", " ") & " ; Company =  " & ddlCustomer.SelectedItem.Text.Replace(",", " "), "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
			End If

			System.Threading.Thread.Sleep("5000")

			Dim allContent As String = New StreamReader(FU_Vehicles.FileContent, Encoding.GetEncoding("iso-8859-1")).ReadToEnd()

			Dim returnCnts As String = ConvertStringIntoDatTableAndInsertData(allContent)



			message.Visible = True
			message.InnerText = returnCnts.Split(";")(0) & " vehicles imported successfully "

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
		Dim currentDateTime As String = ""
		Try
			currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

		Catch ex As Exception

			currentDateTime = DateTime.Now.ToString()

		End Try
		Try
			Dim Lines As String() = data.Split(New Char() {ControlChars.Lf})
			Dim Fields As String() = New String() {}
			Fields = Lines(0).Split(New Char() {","c})
			Dim Cols As Integer = Fields.GetLength(0)
			Dim dt As New DataTable()
			dt.TableName = "VehicleDetails"
			dt.Columns.Add("VehicleNumber", GetType(String))
			dt.Columns.Add("VehicleName", GetType(String))
			dt.Columns.Add("Extension", GetType(String))
			dt.Columns.Add("DepartmentNo", GetType(String))
			dt.Columns.Add("DepartmentId", GetType(String))
			dt.Columns.Add("Acc_Id", GetType(String))
			dt.Columns.Add("Make", GetType(String))
			dt.Columns.Add("Model", GetType(String))
			dt.Columns.Add("VIN", GetType(String))
			dt.Columns.Add("Year", GetType(String))
			dt.Columns.Add("LicensePlateNumber", GetType(String))
			dt.Columns.Add("LicenseState", GetType(String))
			dt.Columns.Add("Type", GetType(String))
			dt.Columns.Add("RequireOdometerEntry", GetType(String))
			dt.Columns.Add("Hours", GetType(String))
			dt.Columns.Add("CheckOdometerReasonable", GetType(String))
			dt.Columns.Add("OdoLimit", GetType(String))
			'dt.Columns.Add("FSUnits", GetType(String))
			dt.Columns.Add("FuelType", GetType(String))
			dt.Columns.Add("RowIndex", GetType(Integer))

			Dim Row As DataRow
			For i As Integer = 4 To Lines.GetLength(0) - 1
				Fields = Lines(i).Split(New Char() {","c})



				If (Fields.Length = dt.Columns.Count - 1) Then
					Row = dt.NewRow()
					For f As Integer = 0 To Cols - 1
						Row(f) = Fields(f).ToString().Replace("'", "").Trim()
					Next
					Row(18) = i + 1
					dt.Rows.Add(Row)
				ElseIf (Fields.Length < 18 And Fields.Length > 1) Then

					Row = dt.NewRow()
					For f As Integer = 0 To Fields.Length - 1
						Row(f) = Fields(f).ToString().Replace("'", "").Trim()
					Next

					For f As Integer = Fields.Length To 18
						Row(f) = ""
					Next

					Row(18) = i + 1
					dt.Rows.Add(Row)

				ElseIf (Fields.Length > 3) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Invalid input format. Incorrect number of columns for the row number " & (i + 1) & ". Please correct the data and retry!"
					ErrorCnt = ErrorCnt + 1
				End If
			Next



			'insertDt = dt.Copy()
			'insertDt.Clear()
			Dim CheckVehicleNumberExist As Boolean = False
			OBJMaster = New MasterBAL()
			Dim dtDept As DataTable = New DataTable()

			dtDept = OBJMaster.GetDeptbyConditions(" and Dept.CustomerId = " & ddlCustomer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString())

			Dim dtFuelTpes As DataTable = New DataTable()
			OBJMaster = New MasterBAL()

			dtFuelTpes = OBJMaster.GetFuelDetails(ddlCustomer.SelectedValue)

			Dim rowIndex As Integer = 0
			Dim isDirty As Boolean = False
			For Each dr As DataRow In dt.Rows
				Dim dtSelectedFuels As DataTable = New DataTable()
				CheckVehicleNumberExist = False
				isDirty = False

				rowIndex = dr("RowIndex")

				If (dr("VehicleNumber") <> "") Then

					CheckVehicleNumberExist = OBJMaster.CheckVehicleNumberExist(dr("VehicleNumber"), 0, Convert.ToInt32(ddlCustomer.SelectedValue))

					If CheckVehicleNumberExist = True Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & dr("VehicleNumber") & " is already exist. Check Row  " & rowIndex & " & column 1 in uploaded file."
						isDirty = True
					ElseIf (dr("VehicleNumber").ToString().Length > 10) Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Number (" & dr("VehicleNumber") & ") is must be less than equal to 10 characters. Check Row  " & rowIndex & " & column 1 in uploaded file."
						isDirty = True
					End If
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Number field is required. Check Row  " & rowIndex & " & column 1 in uploaded file."
					isDirty = True
				End If

				'If (dr("VehicleName") <> "") Then
				If (dr("VehicleName").ToString().Length > 25) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Name (" & dr("VehicleName") & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 2 in uploaded file."
					isDirty = True
				End If
				'Else
				'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Name field is required. Check Row  " & rowIndex & " & column 2 in uploaded file."
				'isDirty = True
				'End If

				'If (dr("Extension") <> "") Then
				If (dr("Extension").ToString().Length > 50) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle description (" & dr("Extension") & ") is must be less than equal to 50 characters. Check Row  " & rowIndex & " & column 3 in uploaded file."
					isDirty = True
				End If
				'Else
				'strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle description field is required. Check Row  " & rowIndex & " & column 3 in uploaded file."
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
						strErrorDept = currentDateTime & "--" & "Vehicle department number field not found. Check Row  " & rowIndex & " & column 4 in uploaded file."
					End If
				Else
					flagCheckDept = 2
					strErrorDept = currentDateTime & "--" & "Vehicle department number field is required. Check Row  " & rowIndex & " & column 4 in uploaded file."
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
							strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is not found. Check Row  " & rowIndex & " & column 5 in uploaded file."
							isDirty = True
						Else
							flagCheckDept = 0
						End If
					Else
						strLog = strLog & Environment.NewLine & strErrorDept & Environment.NewLine & currentDateTime & "--" & "Vehicle department name is required. Check Row  " & rowIndex & " & column 5 in uploaded file."
						isDirty = True
					End If
				End If

				If (dr("Acc_Id").ToString().Length > 20) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & " Vehicle Account Id (" & dr("Acc_Id") & ") is must be less than equal to 20 characters. Check Row  " & rowIndex & " & column 6 in uploaded file."
					isDirty = True
				End If


				If (dr("Make").ToString().Length > 15) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Make (" & dr("Make") & ") is must be less than equal to 15 characters. Check Row  " & rowIndex & " & column 7 in uploaded file."
					isDirty = True
				End If

				If (dr("Model").ToString().Length > 15) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Model (" & dr("Model") & ") is must be less than equal to 15 characters. Check Row  " & rowIndex & " & column 8 in uploaded file."
					isDirty = True
				End If

				If (dr("VIN").ToString().Length > 20) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle VIN (" & dr("VIN") & ") is must be less than equal to 25 characters. Check Row  " & rowIndex & " & column 9 in uploaded file."
					isDirty = True
				End If

				If (dr("Year").ToString() <> "") Then

					If (dr("Year").ToString().Length > 4) Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Year (" & dr("Year") & ") is must be less than equal to 4 in length. Check Row  " & rowIndex & " & column 10 in uploaded file."
						isDirty = True
					Else
						Dim Year As Integer
						If (Integer.TryParse(dr("Year"), Year) = False) Then
							strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Year (" & dr("Year") & ") is must be numeric. Check Row  " & rowIndex & " & column 10 in uploaded file."
							isDirty = True
						End If
					End If

				End If

				If (dr("LicensePlateNumber").ToString().Length > 8) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle License Plate Number (" & dr("LicensePlateNumber") & ") is must be less than equal to 9 characters. Check Row  " & rowIndex & " & column 11 in uploaded file."
					isDirty = True
				End If

				If (dr("LicenseState").ToString().Length > 2) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle License State (" & dr("LicenseState") & ") is must be less than equal to 2 characters. Check Row  " & rowIndex & " & column 12 in uploaded file."
					isDirty = True
				End If

				If (dr("Type").ToString().Length > 20) Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Type (" & dr("Type") & ") is must be less than equal to 9 characters. Check Row  " & rowIndex & " & column 13 in uploaded file."
					isDirty = True
				End If



				If (dr("RequireOdometerEntry").ToString().ToUpper() = "Y" Or dr("RequireOdometerEntry").ToString().ToUpper() = "N") Then
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Require Odometer Entry (" & dr("RequireOdometerEntry") & ") is must be Y or N. Check Row  " & rowIndex & " & column 14 in uploaded file."
					isDirty = True
				End If

				If (dr("Hours").ToString().ToUpper() = "Y" Or dr("Hours").ToString().ToUpper() = "N") Then
					dr("Hours") = IIf(dr("Hours").ToString().ToUpper() = "Y", True, False)
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Require Hour Entry (" & dr("RequireOdometerEntry") & ") is must be Y or N. Check Row  " & rowIndex & " & column 15 in uploaded file."
					isDirty = True
				End If

				If (dr("CheckOdometerReasonable").ToString().ToUpper() = "Y" Or dr("CheckOdometerReasonable").ToString().ToUpper() = "N") Then
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Check Odometer Reasonability (" & dr("CheckOdometerReasonable") & ") is must be Y or N. Check Row  " & rowIndex & " & column 16 in uploaded file."
					isDirty = True
				End If

				If (dr("OdoLimit").ToString() <> "") Then

					If (dr("OdoLimit").ToString().Length > 6) Then
						strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Miles Window (" & dr("OdoLimit") & ") is must be less than equal to 6 in length. Check Row  " & rowIndex & " & column 17 in uploaded file."
						isDirty = True
					Else
						Dim odLimit As Integer
						If (Integer.TryParse(dr("OdoLimit"), odLimit) = False) Then
							strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Vehicle Miles Window (" & dr("OdoLimit") & ") is must be numeric. Check Row  " & rowIndex & " & column 17 in uploaded file."
							isDirty = True
						End If
					End If
				ElseIf (dr("CheckOdometerReasonable").ToString().ToUpper() = "Y") Then
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "You have selected check odometer reasonability, but no mileage window was entered. Check Row  " & rowIndex & " & column 17 in uploaded file."
					isDirty = True
				End If

				'Dim listOfSites As New List(Of Integer)

				'If (dr("FSUnits").ToString() <> "") Then

				'    Dim arrayOfSites As String() = dr("FSUnits").ToString().Split(";")

				'    For Each sites As String In arrayOfSites
				'        If (sites <> "") Then

				'            OBJMaster = New MasterBAL()
				'            Dim dtSite As DataTable = New DataTable()
				'            dtSite = OBJMaster.GetSiteByCondition(" And h.WifiSSId ='" & sites.Replace("'", "''") & "' and s.CustomerId =" & ddlCustomer.SelectedValue & "", Session("PersonId").ToString(), Session("RoleId").ToString())
				'            If (dtSite.Rows.Count > 0) Then
				'                listOfSites.Add(dtSite.Rows(0)("SiteId"))
				'            Else
				'                strLog = strLog & Environment.NewLine & currentDateTime & "--" & "FluidSecure Link (" & sites & ") not found. Check Row  " & rowIndex & " & column 18 in uploaded file."
				'                isDirty = True
				'            End If

				'        End If
				'    Next

				'End If

				Dim conditions As String = ""
				Dim dv As DataView = New DataView()
				If (dr("FuelType").ToString() <> "") Then
					dv = dtFuelTpes.DefaultView

					Dim strFuelType As String() = dr("FuelType").ToString().Split(";")

					For index = 0 To strFuelType.Length - 1

						dv.RowFilter = "FuelType like '" + strFuelType(index) + "'"
						dtSelectedFuels = dv.ToTable()
						If (dtSelectedFuels.Rows.Count = 0) Then
							strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fuel Type (" + strFuelType(index) + ") not found. Check Row  " & rowIndex & " & column 18 in uploaded file."
							isDirty = True
							Exit For
						Else
							conditions = conditions + " FuelType like '" + strFuelType(index) + "' or"
						End If

					Next
				Else
					strLog = strLog & Environment.NewLine & currentDateTime & "--" & "Fuel Type Empty. Check Row  " & rowIndex & " & column 18 in uploaded file."
					isDirty = True
				End If


				If (isDirty = False) Then
					conditions = conditions.TrimEnd("r")
					conditions = conditions.TrimEnd("o")
					dv.RowFilter = conditions
					dtSelectedFuels = dv.ToTable()
					Dim result As Integer = InsertRecord(dr, dtSelectedFuels) 'listOfSites
					If (result = 1) Then
						cnt = cnt + 1
					End If
				Else
					ErrorCnt = ErrorCnt + 1
				End If

				rowIndex = rowIndex + 1
			Next


			Return cnt & ";" & ErrorCnt

		Catch ex As Exception
			log.Error("Exception occured while importing file. Exception is : " & ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Exception occurred while importing file. Exception is : " & ex.Message
			Return "0;0"
		End Try
	End Function

	Private Function InsertRecord(dr As DataRow, dtSelectedFuels As DataTable) As Integer ', listOfSites As List(Of Integer)
		Try

            Dim result As Integer = OBJMaster.SaveUpdateVehicle(0, dr("VehicleName"), dr("Make"), dr("Model"), IIf(dr("Year") = "", "-1", dr("Year")), dr("LicensePlateNumber"), dr("VIN"), dr("Type"), dr("Extension"), "", "-1", dr("DepartmentId"),
                                                            0, 0, dr("RequireOdometerEntry"), dr("CheckOdometerReasonable"), IIf(dr("OdoLimit") = "", "-1", dr("OdoLimit")), dr("VehicleNumber"), dr("Acc_Id"), "", Convert.ToInt32(Session("PersonId")), ddlCustomer.SelectedValue,
                                                            "-1", dr("Hours"), 1, dr("LicenseState"), 1, "", True, "", 0, 0, "")

            Dim dtFuelTpes As DataTable = New DataTable()
			OBJMaster = New MasterBAL()

			Dim dtFuelVehicle As DataTable = New DataTable("dtFuelTypeAndVehicle")

			dtFuelVehicle.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
			dtFuelVehicle.Columns.Add("FuelType", System.Type.[GetType]("System.Int32"))

			For Each drFuels As DataRow In dtSelectedFuels.Rows

				Dim drFuel As DataRow = dtFuelVehicle.NewRow()

				drFuel("VehicleId") = result
				drFuel("FuelType") = drFuels("FuelTypeId")
				dtFuelVehicle.Rows.Add(drFuel)

			Next

			OBJMaster.InsertFuelTypeVehicleMapping(dtFuelVehicle, result)

			Dim dtVehicleSite As DataTable = New DataTable()
			dtVehicleSite.Columns.Add("VehicleId", System.Type.[GetType]("System.Int32"))
			dtVehicleSite.Columns.Add("SiteID", System.Type.[GetType]("System.Int32"))

			Dim dtFuel As DataTable = dtSelectedFuels.DefaultView.ToTable(True, "FuelTypeId")
			For Each drFuel As DataRow In dtFuel.Rows
				Dim dtSite As DataTable = OBJMaster.GetSiteByCondition(" and s.CustomerId = " & ddlCustomer.SelectedValue.ToString() & " and h.FuelTypeId =  " + drFuel("FuelTypeId").ToString() & " ", Session("PersonId").ToString(), Session("RoleId").ToString(), False)
				If dtSite IsNot Nothing And dtSite.Rows.Count > 0 Then
					For Each site As DataRow In dtSite.Rows
						Dim drSiteToAdd As DataRow = dtVehicleSite.NewRow()
						drSiteToAdd("VehicleId") = result
						drSiteToAdd("SiteID") = site("SiteID")
						dtVehicleSite.Rows.Add(drSiteToAdd)
					Next
				End If
			Next

			OBJMaster.InsertVehicleSiteMapping(dtVehicleSite, result)

			Return 1

		Catch ex As Exception
			log.Error("Exception occured while importing file. Exception is : " & ex.Message)

			Return 0
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

		Dim path As String = Server.MapPath("\Content\Templates\VehicleImportTemplate.csv")
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
End Class
