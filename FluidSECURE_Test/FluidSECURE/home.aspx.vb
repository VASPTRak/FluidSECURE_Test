Imports Fuel_Secure.My.Resources
Imports log4net
Imports log4net.Config

Public Class _Default
	Inherits Page

	Dim OBJMaster As MasterBAL
	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Page))

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
		Try

			If Session("Culture") Is Nothing Then
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(Session("Culture").ToString())
			System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(Session("Culture").ToString())


			XmlConfigurator.Configure()

			ErrorMessage.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession('Session Expired. Please Login Again.')", True)
				'ElseIf Session("RoleName") = "User" Then
				'    'Access denied
				'    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", " alert('Access denied.');top.window.location.href = '/MainMenu.aspx';", True)
			Else
				'CompanyH3.InnerText = Resource.Company
				Dim IsTermConditionAgreed = Session("IsTermConditionAgreed")
				If IsTermConditionAgreed = "False" And Session("RoleName") <> "SuperAdmin" Then
					If Session("PersonId") IsNot Nothing Then

						Session("TempPersonId") = Session("PersonId").ToString()
						Session("TempUniqueId") = Session("UniqueId").ToString()
						Session("TempRoleId") = Session("RoleId").ToString()

						Session("PersonId") = Nothing
						Session("UniqueId") = Nothing
						Session("RoleId") = Nothing
					End If
					ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenTermConditionsMessage();", True)
				Else
					If (Not IsPostBack) Then
						BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
						'Session("CompanyNameHeader") = ""
					End If
				End If
				'Dim lblMasterCompany As Label = DirectCast(Master.FindControl("lblMasterCompany"), Label)
				'Dim lblMasterCompanyName As Label = DirectCast(Master.FindControl("lblMasterCompanyName"), Label)

				'lblMasterCompany.Text = "Company Name: "
				'lblMasterCompanyName.Text = Session("CompanyNameHeader").ToString()

			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub


	Private Sub BindDetails(companyId As Integer)
		Try

			Dim dsDetails As DataSet = New DataSet()
			OBJMaster = New MasterBAL()
			Dim currentDateTime As String = ""

			Try
				currentDateTime = Convert.ToDateTime(HDF_CurrentDate.Value).ToString("MM/dd/yyyy hh:mm:ss tt")

			Catch ex As Exception

				currentDateTime = DateTime.Now.ToString()

			End Try

			dsDetails = OBJMaster.GetDetails(companyId, currentDateTime)

			'LBL_Company.Text = DDL_Customer.SelectedItem.Text
			LBL_DispensedToday.Text = dsDetails.Tables(0).Rows(0)("FuelQuantityForToday")
			LBL_vehiclesFueledToday.Text = dsDetails.Tables(1).Rows(0)("vehiclesFueledToday")
			LBL_AverageAmountOffueledPerVehicle.Text = dsDetails.Tables(2).Rows(0)("AverageAmountOffueledPerVehicle")
			LBL_DispensedCurrentMonth.Text = dsDetails.Tables(3).Rows(0)("FuelQuantityForCurrentMonth")

			Session("CustomerId") = DDL_Customer.SelectedValue


		Catch ex As Exception
			log.Error("Error occurred in BindDetails Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

		End Try


	End Sub

	Private Sub BindCustomer(PersonId As Integer, RoleId As String)
		Try

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, 0)

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select All", "0"))

			If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support") Then
				DDL_Customer.SelectedIndex = 1
				DDL_Customer.Enabled = False
				LBL_Company.Visible = True
				DDL_Customer.Visible = False
				LBL_Company.InnerText = DDL_Customer.SelectedItem.Text
				Session("CompanyNameHeader") = DDL_Customer.SelectedItem.ToString
			End If

			DDL_Customer_SelectedIndexChanged(Nothing, Nothing)


		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try
			BindDetails(DDL_Customer.SelectedValue)
			Try

				Dim hdfHide As HiddenField = DirectCast(Me.Master.FindControl("hdfHide"), HiddenField)
				hdfHide.Value = "homepage"
			Catch ex As Exception

			End Try

			Session("CompanyNameHeader") = DDL_Customer.SelectedItem.ToString

		Catch ex As Exception
			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

		End Try


	End Sub

	Protected Sub btnAccepted_Click(sender As Object, e As EventArgs)
		Dim AcceptedData As String = ""
		Try
			If chk_TermsAndConditions.Checked Then

				Session("PersonId") = Session("TempPersonId").ToString()
				Session("UniqueId") = Session("TempUniqueId").ToString()
				Session("RoleId") = Session("TempRoleId").ToString()

				lblErrorTermsAndPolicy.Text = ""
				OBJMaster = New MasterBAL()
				OBJMaster.UpdateTermAndPolicysAcceptance(Session("UniqueId").ToString(), 0)

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					AcceptedData = CreateData(Session("UniqueId").ToString())
					CSCommonHelper.WriteLog("Added", "Tearm and Policy", "", AcceptedData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "Terms and Privacy Policys Accepted.")
				End If

				Session("IsTermConditionAgreed") = "True"
				Response.Redirect("/home.aspx", False)
			Else
				lblErrorTermsAndPolicy.Text = "*Please accept Terms and Privacy Policy and Continue."
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenTermConditionsMessage();", True)
			End If

		Catch ex As Exception
			log.Error("Error occurred in btnAccepted_Click Exception is :" + ex.Message)
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				AcceptedData = CreateData(Session("UniqueId").ToString())
				CSCommonHelper.WriteLog("Added", "Tearm and Policy", "", AcceptedData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Terms and Privacy Policys Acceptance failed.")
			End If
			Context.GetOwinContext().Authentication.SignOut()
			Session.Abandon()
			Response.Redirect("~/Account/Login.aspx", False)
		End Try
	End Sub

	Protected Sub btnClose_Click(sender As Object, e As EventArgs)
		Try
			Context.GetOwinContext().Authentication.SignOut()
			Session.Abandon()
			Response.Redirect("~/Account/Login.aspx", False)
		Catch ex As Exception
			log.Error("Error occurred in btnClose_Click Exception is :" + ex.Message)
			Context.GetOwinContext().Authentication.SignOut()
			Session.Abandon()
			Response.Redirect("~/Account/Login.aspx", False)
		End Try
	End Sub

	Private Shared Function CreateData(UniqueId As String) As String
		Try
			Dim data As String = ""

			data = "UniqueId = " & UniqueId & " ; " &
				   "Date of Acceptance = " & DateTime.Now.ToString().Replace(",", " ") & " ; " &
				   "Person Name = " & HttpContext.Current.Session("PersonName").ToString().Replace(",", " ") & "  "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	<System.Web.Services.WebMethod(True)>
	Public Shared Function AcceptAndCiontinue() As String
		Dim result As Integer = 0
		Dim AcceptedData As String = ""
		Try
			Dim OBJMaster = New MasterBAL()
			Dim beforeData As String = ""

			HttpContext.Current.Session("PersonId") = HttpContext.Current.Session("TempPersonId").ToString()
			HttpContext.Current.Session("UniqueId") = HttpContext.Current.Session("TempUniqueId").ToString()
			HttpContext.Current.Session("RoleId") = HttpContext.Current.Session("TempRoleId").ToString()

			result = OBJMaster.UpdateTermAndPolicysAcceptance(HttpContext.Current.Session("UniqueId").ToString(), 0)

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				AcceptedData = CreateData(HttpContext.Current.Session("PersonEmail").ToString().ToString())
				CSCommonHelper.WriteLog("Added", "Tearm and Policy", "", AcceptedData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "Terms and Privacy Policys Accepted.")
			End If


			HttpContext.Current.Session("IsTermConditionAgreed") = "True"


		Catch ex As Exception
			log.Error("Error occurred in btnAccepted_Click Exception is :" + ex.Message)
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				AcceptedData = CreateData(HttpContext.Current.Session("PersonEmail").ToString().ToString())
				CSCommonHelper.WriteLog("Added", "Tearm and Policy", "", AcceptedData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Terms and Privacy Policys Acceptance failed.")
			End If
			HttpContext.Current.GetOwinContext().Authentication.SignOut()
			HttpContext.Current.Session.Abandon()
			HttpContext.Current.Response.Redirect("~/Account/Login.aspx", False)
		End Try

		Return result

	End Function

End Class