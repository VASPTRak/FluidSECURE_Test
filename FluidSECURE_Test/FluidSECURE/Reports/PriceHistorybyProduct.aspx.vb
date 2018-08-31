Imports log4net
Imports log4net.Config

Public Class PriceHistorybyProduct
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(PriceHistorybyProduct))

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

					txtHistoryDateFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy")
					txtHistoryDateTo.Text = DateTime.Now.ToString("MM/dd/yyyy")

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

				Else

					txtHistoryDateFrom.Text = Request.Form(txtHistoryDateFrom.UniqueID)
					txtHistoryDateTo.Text = Request.Form(txtHistoryDateTo.UniqueID)
				End If
				txtHistoryDateFrom.Focus()
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

	Private Sub BindAllPersonnels()
		Try
			Dim dtPersonnel As DataTable = New DataTable()
			OBJMaster = New MasterBAL()

			If (DDL_Customer.SelectedValue <> "0") Then
				dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(ANU.IsFluidSecureHub,0)=0  and ANU.CustomerId = " & DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
				DDL_Personnel.DataSource = dtPersonnel
				DDL_Personnel.DataValueField = "PersonId"
				DDL_Personnel.DataTextField = "Person"
				DDL_Personnel.DataBind()
			End If

			DDL_Personnel.Items.Insert(0, New ListItem("Select All Personnel", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try

			BindAllPersonnels()
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
			Dim dSPriceHistory As DataSet = New DataSet()

			Dim startDate As DateTime
			Try
				startDate = Convert.ToDateTime(Request.Form(txtHistoryDateFrom.UniqueID) + " 00:00:00.000").ToString()
			Catch ex As Exception

				ErrorMessage.InnerText = "Wrong date format selected/enterd for 'Price Added Date From'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End Try
			Dim endDate As DateTime
			Try
				endDate = Convert.ToDateTime(Request.Form(txtHistoryDateTo.UniqueID) + " 23:59:59.000").ToString()
			Catch ex As Exception
				ErrorMessage.InnerText = "Wrong date format selected/enterd for 'Price Added Date To'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End Try

			If endDate < startDate Then
				ErrorMessage.InnerText = "'Price Added Date From' must less than 'Price Added Date To'."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return
			End If



			Dim strConditions As String = ""
			If (DDL_Customer.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ph.CompanyId = " + DDL_Customer.SelectedValue, strConditions + " and ph.CompanyId = " + DDL_Customer.SelectedValue)
			End If

			If (DDL_Personnel.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ph.Personid = " + DDL_Personnel.SelectedValue, strConditions + " and ph.Personid = " + DDL_Personnel.SelectedValue)
			End If

			If (DDL_Fuel.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ph.FuelTypeID = " + DDL_Fuel.SelectedValue, strConditions + " and ph.FuelTypeID = " + DDL_Fuel.SelectedValue)
			End If

			'get data from server
			dSPriceHistory = OBJMaster.GetPriceCostHistory(startDate.ToString(), endDate.ToString(), strConditions)
			If (Not dSPriceHistory Is Nothing) Then

				If (dSPriceHistory.Tables(0).Rows.Count <= 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData = CreateData()
						CSCommonHelper.WriteLog("Report Genereated", "Price History by Product", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
					End If
					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
					Return
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData = CreateData()
					CSCommonHelper.WriteLog("Report Genereated", "Price History by Product", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
				End If
				ErrorMessage.InnerText = "Data not found against selected criteria."
				ErrorMessage.Visible = True
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
				Return

			End If
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData = CreateData()
				CSCommonHelper.WriteLog("Report Genereated", "Price History by Product", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
			End If
			Session("PriceHistorybyProduct") = dSPriceHistory

			Session("FromDate") = startDate.ToString("dd-MMM-yyyy hh:mm tt")
			Session("ToDate") = endDate.ToString("dd-MMM-yyyy hh:mm tt")

			Response.Redirect("~/Reports/PriceHistorybyProductReport")


		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData = CreateData()
				CSCommonHelper.WriteLog("Report Genereated", "Price History by Product", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
			End If
			ErrorMessage.InnerText = "Data not found against selected criteria."
			ErrorMessage.Visible = True
			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();loadMultiList();", True)
			Return
		Finally
			txtHistoryDateFrom.Focus()
		End Try

	End Sub

	Private Function CreateData() As String
		Try

			Dim data As String = "Price Added Date From = " & txtHistoryDateFrom.Text.Replace(",", "") & " ; " &
								 "Price Added Date To = " & txtHistoryDateTo.Text.Replace(",", "") & " ; " &
								 "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
								 "Personnel = " & DDL_Personnel.SelectedItem.Text.Replace(",", " ") & " ; " &
								 "Product = " & DDL_Fuel.SelectedItem.Text.Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

End Class