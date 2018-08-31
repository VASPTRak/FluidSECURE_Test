Imports log4net
Imports log4net.Config

Public Class Fuel
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Fuel))

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
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If Not IsPostBack Then
					BindCustomers(Session("PersonId").ToString(), Session("RoleId").ToString())
					If (Not Request.QueryString("FuelTypeID") = Nothing And Not Request.QueryString("FuelTypeID") = "") Then
						HDF_FuelTypeId.Value = Request.QueryString("FuelTypeID")

						BindFuelDetails(Request.QueryString("FuelTypeID"))
						btnFirst.Visible = True
						btnNext.Visible = True
						btnprevious.Visible = True
						btnLast.Visible = True
						btnUpdatePrice.Visible = True
						lblHeader.Text = "Edit Product Type"
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
						btnUpdatePrice.Visible = False
						lblHeader.Text = "Add Product Type"
					End If
					txtFuelType.Focus()
				End If
			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Private Sub BindCustomers(PersonId As Integer, RoleId As String)
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

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred  while getting Companies , please try again later."

			log.Error("Error occurred in BindCustomers Exception is :" + ex.Message)

		End Try
	End Sub

	Private Sub BindFuelDetails(FuelTypeID As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtFuel As DataTable = New DataTable()
			dtFuel = OBJMaster.GetFuelByTypeId(FuelTypeID)
			Dim cnt As Integer = 0
			If (dtFuel.Rows.Count > 0) Then

				If (Not Session("RoleName") = "SuperAdmin") Then

					Dim dtCustOld As DataTable = New DataTable()

					dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtFuel.Rows(0)("CompanyId").ToString()) Then

						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

						Return
					End If

				End If

				txtFuelType.Text = dtFuel.Rows(0)("FuelType").ToString()
				Session("FuelType") = dtFuel.Rows(0)("FuelType").ToString()
				txtExportCode.Text = dtFuel.Rows(0)("ExportCode").ToString()
				txtProductPrice.Text = dtFuel.Rows(0)("ProductPrice").ToString()

				Dim matchCustomer As ListItem
				matchCustomer = DDL_Customer.Items.FindByValue(dtFuel.Rows(0)("CompanyId").ToString())
				If IsNothing(matchCustomer) Then
					DDL_Customer.SelectedIndex = 0
				Else
					DDL_Customer.SelectedValue = IIf(dtFuel.Rows(0)("CompanyId").ToString() = "", 0, dtFuel.Rows(0)("CompanyId").ToString())
					Session("FuelCustomerId") = IIf(dtFuel.Rows(0)("CompanyId").ToString() = "", 0, dtFuel.Rows(0)("CompanyId").ToString())
				End If

				If (Not Session("RoleName") = "SuperAdmin") Then
					DDL_Customer.Enabled = False
				End If

				OBJMaster = New MasterBAL()

				Dim strConditions As String = ""
				If (Not Session("FuelConditions") Is Nothing And Not Session("FuelConditions") = "") Then
					strConditions = Session("FuelConditions")
				Else
					If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
						strConditions = IIf(strConditions = "", " and CompanyId=" & Session("CustomerId"), strConditions & " and CompanyId=" & Session("CustomerId"))
					End If
				End If

				If strConditions.Contains("F.CompanyId") Then
					strConditions = strConditions.Replace("F.CompanyId", "CompanyId")
				End If
				If strConditions.Contains("F.ExportCode") Then
					strConditions = strConditions.Replace("F.ExportCode", "ExportCode")
				End If
				If strConditions.Contains("F.FuelType") Then
					strConditions = strConditions.Replace("F.FuelType", "FuelType")
				End If

				HDF_TotalFuelType.Value = OBJMaster.GetFuelTypeIdByCondition(FuelTypeID, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)

				OBJMaster = New MasterBAL()
				Dim dtAllFuel As DataTable = New DataTable()

				If Not strConditions.Contains("F.CompanyId") Then
					strConditions = strConditions.Replace("CompanyId", "F.CompanyId")
				End If
				If Not strConditions.Contains("F.ExportCode") Then
					strConditions = strConditions.Replace("ExportCode", "F.ExportCode")
				End If
				If Not strConditions.Contains("F.FuelType") Then
					strConditions = strConditions.Replace("FuelType", "F.FuelType")
				End If

				dtAllFuel = OBJMaster.GetFliudTypeByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString()) 'OBJMaster.GetFuelDetails(DDL_Customer.SelectedValue)

				dtAllFuel.PrimaryKey = New DataColumn() {dtAllFuel.Columns(0)}
				Dim dr As DataRow = dtAllFuel.Rows.Find(FuelTypeID)
				If Not IsDBNull(dr) Then

					cnt = dtAllFuel.Rows.IndexOf(dr) + 1

				End If
				If (cnt >= HDF_TotalFuelType.Value) Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				ElseIf (cnt <= 1) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt > 1 And cnt < HDF_TotalFuelType.Value) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
				If cnt = 1 And HDF_TotalFuelType.Value.ToString() = "1" Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				End If

				lblof.Text = cnt & " of " & HDF_TotalFuelType.Value.ToString()

			Else
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Data Not found. Please try again after some time."
			End If
			txtFuelType.Focus()

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(FuelTypeID)
			End If

		Catch ex As Exception
			txtFuelType.Focus()
			log.Error("Error occurred in BindFuelDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting fuel data, please try again later."
		Finally


		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		ValidateAndSaveProduct(False)
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/Allfuels?Filter=Filter")
	End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try


			Dim CurrentFuelId As Integer = HDF_FuelTypeId.Value
			Dim strConditions As String = ""
			If (Not Session("FuelConditions") Is Nothing And Not Session("FuelConditions") = "") Then
				strConditions = Session("FuelConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and CompanyId=" & Session("CustomerId"), strConditions & " and CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			If strConditions.Contains("F.CompanyId") Then
				strConditions = strConditions.Replace("F.CompanyId", "CompanyId")
			End If
			If strConditions.Contains("F.ExportCode") Then
				strConditions = strConditions.Replace("F.ExportCode", "ExportCode")
			End If
			If strConditions.Contains("F.FuelType") Then
				strConditions = strConditions.Replace("F.FuelType", "FuelType")
			End If
			Dim FuelTypeId As Integer = OBJMaster.GetFuelTypeIdByCondition(CurrentFuelId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_FuelTypeId.Value = FuelTypeId
			BindFuelDetails(FuelTypeId)

		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try

			Dim CurrentFuelId As Integer = HDF_FuelTypeId.Value
			Dim strConditions As String = ""
			If (Not Session("FuelConditions") Is Nothing And Not Session("FuelConditions") = "") Then
				strConditions = Session("FuelConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and CompanyId=" & Session("CustomerId"), strConditions & " and CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			If strConditions.Contains("F.CompanyId") Then
				strConditions = strConditions.Replace("F.CompanyId", "CompanyId")
			End If
			If strConditions.Contains("F.ExportCode") Then
				strConditions = strConditions.Replace("F.ExportCode", "ExportCode")
			End If
			If strConditions.Contains("F.FuelType") Then
				strConditions = strConditions.Replace("F.FuelType", "FuelType")
			End If
			Dim FuelTypeId As Integer = OBJMaster.GetFuelTypeIdByCondition(CurrentFuelId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_FuelTypeId.Value = FuelTypeId
			BindFuelDetails(FuelTypeId)

		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try

			Dim CurrentFuelId As Integer = HDF_FuelTypeId.Value
			Dim strConditions As String = ""
			If (Not Session("FuelConditions") Is Nothing And Not Session("FuelConditions") = "") Then
				strConditions = Session("FuelConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and CompanyId=" & Session("CustomerId"), strConditions & " and CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			If strConditions.Contains("F.CompanyId") Then
				strConditions = strConditions.Replace("F.CompanyId", "CompanyId")
			End If
			If strConditions.Contains("F.ExportCode") Then
				strConditions = strConditions.Replace("F.ExportCode", "ExportCode")
			End If
			If strConditions.Contains("F.FuelType") Then
				strConditions = strConditions.Replace("F.FuelType", "FuelType")
			End If
			Dim FuelTypeId As Integer = OBJMaster.GetFuelTypeIdByCondition(CurrentFuelId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_FuelTypeId.Value = FuelTypeId
			BindFuelDetails(FuelTypeId)

		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try

			Dim CurrentFuelId As Integer = HDF_FuelTypeId.Value
			Dim strConditions As String = ""
			If (Not Session("FuelConditions") Is Nothing And Not Session("FuelConditions") = "") Then
				strConditions = Session("FuelConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and CompanyId=" & Session("CustomerId"), strConditions & " and CompanyId=" & Session("CustomerId"))
				End If
			End If
			OBJMaster = New MasterBAL()
			If strConditions.Contains("F.CompanyId") Then
				strConditions = strConditions.Replace("F.CompanyId", "CompanyId")
			End If
			If strConditions.Contains("F.ExportCode") Then
				strConditions = strConditions.Replace("F.ExportCode", "ExportCode")
			End If
			If strConditions.Contains("F.FuelType") Then
				strConditions = strConditions.Replace("F.FuelType", "FuelType")
			End If
			Dim FuelTypeId As Integer = OBJMaster.GetFuelTypeIdByCondition(CurrentFuelId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_FuelTypeId.Value = FuelTypeId
			BindFuelDetails(FuelTypeId)

		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnUpdatePrice_Click(sender As Object, e As EventArgs) Handles btnUpdatePrice.Click
		Response.Redirect("~/Master/UpdateTransactionCost?FuelTypeID=" + HDF_FuelTypeId.Value)
	End Sub

	Private Function CreateData(ProductId As Integer) As String
		Try

			Dim data As String = ""

			data = "ProductId = " & ProductId & " ; " &
					"Product Type = " & txtFuelType.Text.Replace(",", " ") & " ; " &
					"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
					"Export Code = " & txtExportCode.Text.Replace(",", " ") & " ; " &
					"Product Price = " & txtProductPrice.Text

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
		ValidateAndSaveProduct(True)
	End Sub

	Private Sub ValidateAndSaveProduct(IsSaveAndAddNew As Boolean)

		Dim FuelTypeId As Integer = 0
		Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			If (Not HDF_FuelTypeId.Value = Nothing And Not HDF_FuelTypeId.Value = "") Then

				FuelTypeId = HDF_FuelTypeId.Value

			End If
			Dim CheckIdExists As Integer = 0
			OBJMaster = New MasterBAL()
			CheckIdExists = OBJMaster.CheckFuelTypeExist(txtFuelType.Text, FuelTypeId, Convert.ToInt32(DDL_Customer.SelectedValue))

			If CheckIdExists = -1 Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Product Already Exists."

				Return

			End If

            If txtFuelType.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Fuel Type and try again."
                txtFuelType.Focus()
                Return
            End If

            If DDL_Customer.SelectedIndex = 0 Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select Company and try again."
                DDL_Customer.Focus()
                Return
            End If

            Dim resultDecimal As Decimal = 0

            If (txtProductPrice.Text <> "" And Not (Decimal.TryParse(txtProductPrice.Text, resultDecimal))) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Product price as decimal and try again."
                txtProductPrice.Focus()
                Return
            End If


            Dim result As Integer = 0

			OBJMaster = New MasterBAL()

			Dim price As Decimal = 0
			If Not txtProductPrice.Text = "" Then price = Convert.ToDecimal(txtProductPrice.Text)

			Session("FuelCustomerId") = DDL_Customer.SelectedValue
			Session("FuelType") = txtFuelType.Text
			result = OBJMaster.SaveUpdateFuel(FuelTypeId, txtFuelType.Text, txtExportCode.Text, Convert.ToInt32(Session("PersonId")), DDL_Customer.SelectedValue, price)
			btnUpdatePrice.Visible = True

			If result > 0 Then
				If (FuelTypeId > 0) Then
					message.Visible = True
					message.InnerText = "Record saved"

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(FuelTypeId)
						CSCommonHelper.WriteLog("Modified", "Products", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Fuel"))
					End If
				Else

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Products", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/Fuel"))
					Else
						Response.Redirect(String.Format("~/Master/Fuel?FuelTypeID={0}&RecordIs=New", result))

					End If

				End If

			Else
				If (FuelTypeId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(FuelTypeId)
						CSCommonHelper.WriteLog("Modified", "Products", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Fuel type update failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Fuel type update failed, please try again"
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Products", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Fuel type Addition failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Fuel type Addition failed, please try again"
				End If

			End If

		Catch ex As Exception
			If (FuelTypeId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(FuelTypeId)
					CSCommonHelper.WriteLog("Modified", "Products", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Fuel type update failed. Exception is : " & ex.Message)
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(FuelTypeId)
					CSCommonHelper.WriteLog("Added", "Products", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Fuel type Addition failed. Exception is : " & ex.Message)
				End If
			End If
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
		Finally
			txtFuelType.Focus()

		End Try
	End Sub
End Class