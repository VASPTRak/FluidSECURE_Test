Imports log4net
Imports log4net.Config

Public Class TankChart
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
					If (Not Request.QueryString("TankChartId") = Nothing And Not Request.QueryString("TankChartId") = "") Then
						btn_ViewChart.Visible = True

						HDF_TankChartId.Value = Request.QueryString("TankChartId")
						BindTankChartDetails(Request.QueryString("TankChartId"))

						btnFirst.Visible = True
						btnNext.Visible = True
						btnprevious.Visible = True
						btnLast.Visible = True
						txtTankSize.Enabled = False
						txtFuelIncrement.Enabled = False
						RBL_Entry.Enabled = False
						lblHeader.Text = "Edit Tank Chart"
						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message.InnerText = "Record saved"
						End If
					Else
						btn_ViewChart.Visible = False
						btnFirst.Visible = False
						btnNext.Visible = False
						btnprevious.Visible = False
						btnLast.Visible = False
						lblof.Visible = False
						lblHeader.Text = "Add Tank Chart"
						'DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
					End If



					txtTankChartNumber.Focus()
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

	'Private Sub GetFuelType(CustomerId As Integer)
	'	Try

	'		OBJMaster = New MasterBAL()
	'		Dim dtFuelType As DataTable = New DataTable()

	'		dtFuelType = OBJMaster.GetFuelDetails(CustomerId)

	'		ddlFuelType.DataSource = dtFuelType
	'		ddlFuelType.DataTextField = "FuelType"
	'		ddlFuelType.DataValueField = "FuelTypeID"
	'		ddlFuelType.DataBind()
	'		ddlFuelType.Items.Insert(0, New ListItem("Select Product Type", "0"))

	'	Catch ex As Exception

	'		log.Error("Error occurred in GetFuelType Exception is :" + ex.Message)
	'		ErrorMessage.Visible = True
	'		ErrorMessage.InnerText = "Error occurred while getting fuel types, please try again later."

	'	End Try
	'End Sub

	Private Sub BindTankChartDetails(TankChartId As Integer)
		Try

			OBJMaster = New MasterBAL()
			Dim dtTankChart As DataTable = New DataTable()
			dtTankChart = OBJMaster.GetTankChartByTankChartId(TankChartId)
			Dim cnt As Integer = 0
			If (dtTankChart.Rows.Count > 0) Then

				If (Not Session("RoleName") = "SuperAdmin") Then

					Dim dtCust As DataTable = New DataTable()

					dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

					If (dtCust.Rows(0)("CustomerId").ToString() <> dtTankChart.Rows(0)("CompanyId").ToString()) Then

						ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

						Return
					End If

				End If

				txtTankChartNumber.Text = dtTankChart.Rows(0)("TankChartNumber").ToString()
				txtTankChartName.Text = dtTankChart.Rows(0)("TankChartName").ToString()
				txtDescription.Text = dtTankChart.Rows(0)("Description").ToString()
				txtTankSize.Text = dtTankChart.Rows(0)("TankSize").ToString()
                txtFuelIncrement.Text = dtTankChart.Rows(0)("FuelIncrement").ToString()
                Session("TankSizeInches") = dtTankChart.Rows(0)("TankSize").ToString()
                Session("FuelIncrement") = dtTankChart.Rows(0)("FuelIncrement").ToString()
                DDL_Customer.SelectedValue = dtTankChart.Rows(0)("CompanyId").ToString()

				'DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

				'ddlFuelType.SelectedValue = dtTankChart.Rows(0)("FuelTypeId").ToString()

				RBL_Entry.SelectedValue = dtTankChart.Rows(0)("Entries").ToString()

				Dim matchCustomer As ListItem
				matchCustomer = DDL_Customer.Items.FindByValue(dtTankChart.Rows(0)("CompanyId").ToString())
				If IsNothing(matchCustomer) Then
					DDL_Customer.SelectedIndex = 0
				Else
					DDL_Customer.SelectedValue = IIf(dtTankChart.Rows(0)("CompanyId").ToString() = "", 0, dtTankChart.Rows(0)("CompanyId").ToString())
				End If

				If (Not Session("RoleName") = "SuperAdmin") Then
					DDL_Customer.Enabled = False
				End If


				OBJMaster = New MasterBAL()
				Dim dtAlTankChart As DataTable = New DataTable()

				Dim strConditions As String = ""

				If (Not Session("TankChartConditions") Is Nothing And Not Session("TankChartConditions") = "") Then
					strConditions = Session("TankChartConditions")
				Else
					If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
						strConditions = IIf(strConditions = "", " and TC.CompanyId=" & Session("CustomerId"), strConditions & " and TC.CompanyId=" & Session("CustomerId"))
					End If
				End If
				OBJMaster = New MasterBAL()

				HDF_TotalTankChart.Value = OBJMaster.GetTankChartIdByCondition(TankChartId, False, False, False, False, True, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)

				dtAlTankChart = OBJMaster.GetTankChartsByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString())

				dtAlTankChart.PrimaryKey = New DataColumn() {dtAlTankChart.Columns(0)}
				Dim dr As DataRow = dtAlTankChart.Rows.Find(TankChartId)
				If Not IsDBNull(dr) Then

					cnt = dtAlTankChart.Rows.IndexOf(dr) + 1

				End If
				If (cnt >= HDF_TotalTankChart.Value) Then
					btnNext.Enabled = False
					btnLast.Enabled = False
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				ElseIf (cnt <= 1) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = False
					btnprevious.Enabled = False
				ElseIf (cnt > 1 And cnt < HDF_TotalTankChart.Value) Then
					btnNext.Enabled = True
					btnLast.Enabled = True
					btnFirst.Enabled = True
					btnprevious.Enabled = True
				End If
				lblof.Text = cnt & " of " & HDF_TotalTankChart.Value.ToString()

			Else
				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Data Not found. Please try again after some time."
			End If
			txtTankChartNumber.Focus()

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(TankChartId)
			End If

		Catch ex As Exception
			txtTankChartNumber.Focus()
			log.Error("Error occurred in BindFuelDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting fuel data, please try again later."
		Finally


		End Try
	End Sub

	Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		ValidateAndSaveTankChart(False)
	End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Response.Redirect("~/Master/AllTankCharts?Filter=Filter")
	End Sub

	Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("TankChartConditions") Is Nothing) Then
				strConditions = Session("TankChartConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and TC.CompanyId=" & Session("CustomerId"), strConditions & " and TC.CompanyId=" & Session("CustomerId"))
				End If
			End If

			Dim CurrentTankChartId As Integer = HDF_TotalTankChart.Value

			OBJMaster = New MasterBAL()
			Dim TankChartId As Integer = OBJMaster.GetTankChartIdByCondition(CurrentTankChartId, True, False, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_TankChartId.Value = TankChartId
			BindTankChartDetails(TankChartId)


		Catch ex As Exception
			log.Error("Error occurred in First_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("TankChartConditions") Is Nothing) Then
				strConditions = Session("TankChartConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and TC.CompanyId=" & Session("CustomerId"), strConditions & " and TC.CompanyId=" & Session("CustomerId"))
				End If
			End If
			Dim CurrentTankChartId As Integer = HDF_TankChartId.Value

			OBJMaster = New MasterBAL()
			Dim TankChartId As Integer = OBJMaster.GetTankChartIdByCondition(CurrentTankChartId, False, False, False, True, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_TankChartId.Value = TankChartId
			BindTankChartDetails(TankChartId)

		Catch ex As Exception
			log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
		ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("TankChartConditions") Is Nothing) Then
				strConditions = Session("TankChartConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and TC.CompanyId=" & Session("CustomerId"), strConditions & " and TC.CompanyId=" & Session("CustomerId"))
				End If
			End If
			Dim CurrentTankChartId As Integer = HDF_TankChartId.Value

			OBJMaster = New MasterBAL()
			Dim TankChartId As Integer = OBJMaster.GetTankChartIdByCondition(CurrentTankChartId, False, False, True, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_TankChartId.Value = TankChartId
			BindTankChartDetails(TankChartId)

		Catch ex As Exception
			log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
		ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
		Try

			Dim strConditions As String = ""
			If (Not Session("TankChartConditions") Is Nothing) Then
				strConditions = Session("TankChartConditions")
			Else
				If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
					strConditions = IIf(strConditions = "", " and TC.CompanyId=" & Session("CustomerId"), strConditions & " and TC.CompanyId=" & Session("CustomerId"))
				End If
			End If
			Dim CurrentTankChartId As Integer = HDF_TankChartId.Value

			OBJMaster = New MasterBAL()
			Dim TankChartId As Integer = OBJMaster.GetTankChartIdByCondition(CurrentTankChartId, False, True, False, False, False, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString(), strConditions)
			HDF_TankChartId.Value = TankChartId
			BindTankChartDetails(TankChartId)

		Catch ex As Exception
			log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
		ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Function CreateData(TankChartId As Integer) As String
		Try

			Dim data As String = ""
			data = "TankChartId = " & TankChartId & " ; " &
					"TankChartNumber = " & txtTankChartNumber.Text.Replace(",", " ") & " ; " &
					"TankChartName = " & txtTankChartName.Text.Replace(",", " ") & " ; " &
					"Description = " & txtDescription.Text.Replace(",", " ") & " ; " &
					"TankSize = " & txtTankSize.Text.Replace(",", " ") & " ; " &
					"Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
					"Entries = " & RBL_Entry.SelectedItem.Text.Replace(",", " ") & " ; " &
					"FuelIncrement = " & txtFuelIncrement.Text.Replace(",", " ") & " ; "
			'"FuelType = " & ddlFuelType.SelectedItem.Text.Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
		ValidateAndSaveTankChart(True)
	End Sub

	Private Sub ValidateAndSaveTankChart(IsSaveAndAddNew As Boolean)

		Dim TankChartId As Integer = 0
		Try

            If txtTankChartNumber.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Tank Chart Number and try again."
                txtTankChartNumber.Focus()
                Return
            End If

            Dim resultInteger As Integer = 0

            If (txtTankChartNumber.Text <> "" And Not (Integer.TryParse(txtTankChartNumber.Text, resultInteger))) Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Tank Chart Number as Integer and try again."
                txtTankChartNumber.Focus()
                Return
            End If

            If txtTankSize.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Tank Size and try again."
                txtTankSize.Focus()
                Return
            End If

            If txtTankChartName.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Tank Chart Name and try again."
                txtTankChartName.Focus()
                Return
            End If

            If txtFuelIncrement.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter Fuel Increment and try again."
                txtFuelIncrement.Focus()
                Return
            End If

            If (Not HDF_TankChartId.Value = Nothing And Not HDF_TankChartId.Value = "") Then

                TankChartId = HDF_TankChartId.Value

                Dim resultDecimal As Decimal = 0
                resultInteger = 0

                If (Session("TankSizeInches").ToString() <> "" And Not (Decimal.TryParse(Session("TankSizeInches").ToString(), resultDecimal))) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter Tank Size as decimal and try again."
                    txtTankSize.Focus()
                    Return
                End If

                If (Session("FuelIncrement").ToString() <> "" And Not (Integer.TryParse(Session("FuelIncrement").ToString(), resultInteger))) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter Fuel Increment as Integer and try again."
                    txtFuelIncrement.Focus()
                    Return
                End If

            Else
                Session("TankSizeInches") = txtTankSize.Text
                Session("FuelIncrement") = txtFuelIncrement.Text
            End If
			Dim CheckNumberExists As Integer = 0
			OBJMaster = New MasterBAL()
			CheckNumberExists = OBJMaster.CheckTankChartNumberExist(txtTankChartNumber.Text, TankChartId, Convert.ToInt32(DDL_Customer.SelectedValue))

            If CheckNumberExists = -1 Then

                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Tank chart with same number Already Exists."
                txtTankChartNumber.Focus()
                Return

            End If


            Dim CheckNameExists As Integer = 0
			OBJMaster = New MasterBAL()
			CheckNameExists = OBJMaster.CheckTankChartNameExist(txtTankChartName.Text, TankChartId, Convert.ToInt32(DDL_Customer.SelectedValue))

			If CheckNameExists = -1 Then

				ErrorMessage.Visible = True
				ErrorMessage.InnerText = "Tank chart with same name Already Exists."
				txtTankChartName.Focus()
				Return

			End If
			OBJMaster = New MasterBAL()

			Dim result As Integer = 0

			OBJMaster = New MasterBAL()

            result = OBJMaster.SaveUpdateTankChart(TankChartId, txtTankChartNumber.Text, txtTankChartName.Text, txtDescription.Text, Session("TankSizeInches").ToString(), RBL_Entry.SelectedValue,
                                                   Session("FuelIncrement").ToString(), DDL_Customer.SelectedValue, Session("PersonId".ToString()))

            If result > 0 Then
				SaveCoefficient(result)

				If (TankChartId > 0) Then
					message.Visible = True
					message.InnerText = "Record saved"

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(TankChartId)
						CSCommonHelper.WriteLog("Modified", "Tank Charts", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/TankChart"))
					End If
				Else

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Tank Charts", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
					End If
					If (IsSaveAndAddNew = True) Then
						Response.Redirect(String.Format("~/Master/TankChart"))
					Else
						Response.Redirect(String.Format("~/Master/TankChart?TankChartId={0}&RecordIs=New", result))

					End If

				End If

			Else
				If (TankChartId > 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(TankChartId)
						CSCommonHelper.WriteLog("Modified", "Tank Charts", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "tank chart update failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Tank chart update failed, please try again"
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData(result)
						CSCommonHelper.WriteLog("Added", "Tank charts", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank chart Addition failed.")
					End If
					ErrorMessage.Visible = True
					ErrorMessage.InnerText = "Tank chart Addition failed, please try again"
				End If

			End If
			txtTankChartNumber.Focus()
		Catch ex As Exception
			If (TankChartId > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(TankChartId)
					CSCommonHelper.WriteLog("Modified", "Tank Charts", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank chart update failed. Exception is : " & ex.Message)
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData(TankChartId)
					CSCommonHelper.WriteLog("Added", "Tank Charts", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank chart Addition failed. Exception is : " & ex.Message)
				End If
			End If
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
			log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
			txtTankChartNumber.Focus()
		Finally
			'
		End Try
	End Sub

	'Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
	'	GetFuelType(DDL_Customer.SelectedValue)
	'End Sub

	Protected Sub btn_ViewChart_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/TankChartDetails?TankChartId=" & HDF_TankChartId.Value)
	End Sub

	Private Function SaveCoefficient(TankChartId As Integer) As Double()
		Dim coefficient(4) As Double
		Try

			'Implementation of Linest function
			OBJMaster = New MasterBAL()
			Dim dtTankChartDetail As DataTable = New DataTable()
			dtTankChartDetail = OBJMaster.GetTankChartDetailsByTankChartId(TankChartId)


			Dim datapointsx(dtTankChartDetail.Rows.Count - 1) As Double
			Dim datapointsy(dtTankChartDetail.Rows.Count - 1) As Double

			Dim i As Integer = 0
			For Each dr As DataRow In dtTankChartDetail.Rows
				datapointsx(i) = dr("IncrementLevel")
				datapointsy(i) = dr("GallonLevel")
				i = i + 1
			Next

			coefficient = MathNet.Numerics.Fit.Polynomial(datapointsx, datapointsy, 3, MathNet.Numerics.LinearRegression.DirectRegressionMethod.Svd)
			OBJMaster = New MasterBAL()
			OBJMaster.SaveCoefficients(TankChartId, coefficient(0), coefficient(1), coefficient(2), coefficient(3))
			'Dim xval As Double = "68"
			'Dim yval0 As Double = coefficient(0) + (coefficient(1) * xval) + (coefficient(2) * Math.Pow(xval, 2)) + coefficient(3) * Math.Pow(xval, 3)

		Catch ex As Exception
			log.Error("Error occurred in SaveCoefficient Exception is :" + ex.Message)
		End Try
		Return coefficient
	End Function

End Class