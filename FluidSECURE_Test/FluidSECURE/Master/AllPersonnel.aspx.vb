
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.AspNet.Identity.EntityFramework
Imports log4net
Imports log4net.Config
Imports System.IO
Imports System.Net.Mail
Imports System.Net
Imports Fuel_Secure.My.Resources
Imports System.Resources

Public Class AllPersonnel
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllPersonnel))


	Dim OBJMaster As MasterBAL

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			If Session("Culture") Is Nothing Then
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(Session("Culture").ToString())
			System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(Session("Culture").ToString())

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

				If (Not IsPostBack) Then
					btnSearch.Text = Resource.Search
					btn_New.Text = Resource.AddNewPersonnel
					btnMyModalActiveInActiveNo.Text = Resource.No
					btnMyModalActiveInActiveSuccess.Text = Resource.Yes


					BindColumns()
					BindDept()
					BindCustomer()
					BindRoles()

					If (Request.QueryString("Filter") = Nothing) Then
						Session("PerConditions") = ""
						Session("PerDDL_ColumnName") = ""
						Session("Pertxt_valueNameValue") = ""
						Session("PerDDL_CustomerValue") = ""
						Session("PerDDL_DeptValue") = ""
						Session("PerChk_IsApproved") = ""
						Session("Per_DDL_RoleId") = ""
					End If

					If (Not Session("PerConditions") Is Nothing And Not Session("PerConditions") = "") Then

						If (Not Session("PerDDL_ColumnName") Is Nothing And Not Session("PerDDL_ColumnName") = "") Then
							DDL_ColumnName.SelectedValue = Session("PerDDL_ColumnName")
							If (Session("PerDDL_ColumnName") = "DepartmentId") Then
								DDL_Dept.SelectedValue = Session("PerDDL_DeptValue")
								DDL_Dept.Visible = True
								txt_value.Visible = False
								DDL_RoleId.Visible = False
								DDL_Customer.Visible = False
								Chk_IsApproved.Visible = False
							ElseIf (Session("PerDDL_ColumnName") = "IsApproved") Then
								Chk_IsApproved.Checked = Session("PerChk_IsApproved")
								Chk_IsApproved.Visible = True
								txt_value.Visible = False
								DDL_RoleId.Visible = False
								DDL_Customer.Visible = False
								DDL_Dept.Visible = False
							ElseIf (Session("PerDDL_ColumnName") = "RoleId") Then
								DDL_RoleId.SelectedValue = Session("Per_DDL_RoleId")
								DDL_RoleId.Visible = True
								txt_value.Visible = False
								Chk_IsApproved.Visible = False
								DDL_Customer.Visible = False
								DDL_Dept.Visible = False
							ElseIf (Session("PerDDL_ColumnName") = "CustomerId") Then
								DDL_Customer.SelectedValue = Session("PerDDL_CustomerValue")
								DDL_Customer.Visible = True
								txt_value.Visible = False
								Chk_IsApproved.Visible = False
								DDL_RoleId.Visible = False
								DDL_Dept.Visible = False
							Else
								DDL_Customer.Visible = False
								txt_value.Visible = True
								Chk_IsApproved.Visible = False
								DDL_RoleId.Visible = False
								DDL_Dept.Visible = False
								If (Not Session("Pertxt_valueNameValue") Is Nothing And Not Session("Pertxt_valueNameValue") = "") Then
									txt_value.Text = Session("Pertxt_valueNameValue")
								Else
									txt_value.Text = ""
								End If
							End If
						End If
					End If

					btnSearch_Click(Nothing, Nothing)
					DDL_ColumnName.Focus()
				End If
			End If


		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	'Private Sub BindAllPersonnel()
	'    Try

	'        OBJMaster = New MasterBAL()
	'        Dim dtPersonnel As DataTable = New DataTable()

	'        dtPersonnel = OBJMaster.GetPersonnelDetails()

	'        gvPersonnel.DataSource = dtPersonnel
	'        gvPersonnel.DataBind()

	'    Catch ex As Exception

	'        log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
	'        ErrorMessage.Visible = True
	'        ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

	'    End Try

	'End Sub

	Private Sub BindColumns()
		Try
			'Dim rsxr As ResXResourceReader
			'Dim rm As ResourceManager = New System.Resources.ResourceManager("Fuel_Secure.Resource", Reflection.Assembly.GetExecutingAssembly())

			''Dim localized = rm.GetString("PersonName")

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetPersonnelColumnNameForSearch(0)

			'Dim dtColumnsTemp As DataTable = New DataTable()
			'dtColumnsTemp = dtColumns.Clone()

			'For Each dr As DataRow In dtColumns.Rows

			'	Dim drNew As DataRow = dtColumnsTemp.NewRow()
			'	drNew("ColumnName") = dr("ColumnName")
			'	drNew("ColumnEnglishName") = rm.GetString(dr("ColumnEnglishName"))

			'	dtColumnsTemp.Rows.Add(drNew)

			'Next

			DDL_ColumnName.DataSource = dtColumns
			DDL_ColumnName.DataValueField = "ColumnName"
			DDL_ColumnName.DataTextField = "ColumnEnglishName"
			DDL_ColumnName.DataBind()
			DDL_ColumnName.Items.Insert(0, New ListItem("Select Column", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindColumns Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = Resource.ErrorMsg1

		End Try
	End Sub

	Private Sub BindDept()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetDepartments(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())

			DDL_Dept.DataSource = dtColumns
			DDL_Dept.DataValueField = "DeptID"
			DDL_Dept.DataTextField = "NAME"
			DDL_Dept.DataBind()
			DDL_Dept.Items.Insert(0, New ListItem("Select Department", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindDept Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = Resource.ErrorMsg2

		End Try

	End Sub

	Private Sub BindCustomer()
		Try
			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

			DDL_Customer.DataSource = dtCust
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))
		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = Resource.ErrorMsg3

		End Try
	End Sub

	Private Sub BindRoles()
		Try

            OBJMaster = New MasterBAL()
            Dim dtAccessLevels As DataTable = New DataTable()
			'Dim dtAccessLevelsCopy As DataTable = New DataTable()
			dtAccessLevels = OBJMaster.GetPersonAccessLevels(Session("RoleName"))
			'dtAccessLevelsCopy = dtAccessLevels.Clone
			'         dtAccessLevelsCopy.Clear()
			'Dim roleManager = New RoleManager(Of IdentityRole)(New RoleStore(Of IdentityRole)(New ApplicationDbContext()))

			'Dim roles = roleManager.Roles.ToList()

			'For index = 0 To dtAccessLevels.Rows.Count - 1

			'    If Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "Support" Then
			'        Dim role = Nothing
			'        If dtAccessLevels(index)("Name") <> "SuperAdmin" Or dtAccessLevels(index)("Name") <> "Support" Then
			'            role = dtAccessLevels(index)("Name")
			'        End If
			'        If Not role Is Nothing Then
			'            dtAccessLevelsCopy.Rows.Add(dtAccessLevels(index).ItemArray)
			'        End If
			'    Else
			'        Dim role = Nothing
			'        If dtAccessLevels(index)("Name") <> "SuperAdmin" Then
			'            role = dtAccessLevels(index)("Name")
			'        End If
			'        If Not role Is Nothing Then
			'            dtAccessLevelsCopy.Rows.Add(dtAccessLevels(index).ItemArray)
			'        End If
			'    End If
			'    If Session("RoleName") = "User" Then
			'        Dim custRole = Nothing
			'        If dtAccessLevels(index)("Name") <> "CustomerAdmin" Then
			'            custRole = dtAccessLevels(index)("Name")
			'        End If
			'        If Not custRole Is Nothing Then
			'            dtAccessLevelsCopy.Rows.Add(dtAccessLevels(index).ItemArray)
			'        End If
			'    End If
			'Next

			DDL_RoleId.DataSource = dtAccessLevels
			DDL_RoleId.DataValueField = "Id"
            DDL_RoleId.DataTextField = "DisplayName"
            DDL_RoleId.DataBind()
            DDL_RoleId.Items.Insert(0, New ListItem("Select Access level", "0"))

        Catch ex As Exception

			log.Error("Error occurred in BindRoles Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = Resource.ErrorMsg4

		End Try


	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			OBJMaster = New MasterBAL()

			Dim strConditions As String = ""
			Session("PerConditions") = ""

			strConditions = " and ISNULL(IsFluidSecureHub,0)=0 "

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions += " and ANU.CustomerId=" & Session("CustomerId")
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ANU." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and ANU." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
			ElseIf ((DDL_Dept.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ANU." + DDL_ColumnName.SelectedValue + " = " + DDL_Dept.SelectedValue + "", strConditions + " and ANU." + DDL_ColumnName.SelectedValue + " = " + DDL_Dept.SelectedValue + "")
			ElseIf ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ANU." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and ANU." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			ElseIf ((DDL_RequestFrom.SelectedValue <> "0") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ANU." + DDL_ColumnName.SelectedValue + " = '" + DDL_RequestFrom.SelectedValue + "'", strConditions + " and ANU." + DDL_ColumnName.SelectedValue + " = '" + DDL_RequestFrom.SelectedValue + "'")
			ElseIf ((DDL_RoleId.SelectedValue <> "0") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ANU." + DDL_ColumnName.SelectedValue + " = '" + DDL_RoleId.SelectedValue + "'", strConditions + " and ANU." + DDL_ColumnName.SelectedValue + " = '" + DDL_RoleId.SelectedValue + "'")
			ElseIf (DDL_ColumnName.SelectedValue <> "0" And DDL_ColumnName.SelectedValue = "IsApproved") Then
				strConditions = IIf(strConditions = "", " and ANU." & DDL_ColumnName.SelectedValue & " = " & IIf(Chk_IsApproved.Checked = True, 1, 0) & "", strConditions & " and ANU." & DDL_ColumnName.SelectedValue & " = " & IIf(Chk_IsApproved.Checked = True, 1, 0) & "")
			ElseIf (DDL_ColumnName.SelectedValue = "SoftUpdate" And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and ANU." & DDL_ColumnName.SelectedValue & " = " & IIf(Chk_SoftUpdate.Checked = True, "'Y'", "'N'") & "", strConditions & " and ANU." & DDL_ColumnName.SelectedValue & " = " & IIf(Chk_SoftUpdate.Checked = True, "'Y'", "'N'") & "")
			End If

			Dim dtPersonnel As DataTable = New DataTable()

			Session("PerConditions") = strConditions
			Session("PerDDL_ColumnName") = DDL_ColumnName.SelectedValue
			Session("Pertxt_valueNameValue") = txt_value.Text
			Session("PerDDL_CustomerValue") = DDL_Customer.SelectedValue
			Session("PerDDL_DeptValue") = DDL_Dept.SelectedValue
			Session("PerChk_IsApproved") = Chk_IsApproved.Checked
			Session("Per_DDL_RoleId") = DDL_RoleId.SelectedValue

			dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtPersonnel") = dtPersonnel
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtPersonnel IsNot Nothing Then
                If dtPersonnel.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtPersonnel.Rows.Count)
                End If
            End If
            gvPersonnel.DataSource = dtPersonnel
			gvPersonnel.DataBind()

			ViewState("Column_Name") = "PersonName"
			ViewState("Sort_Order") = "ASC"

		Catch ex As Exception

			log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = Resource.ErrorMsg5
		Finally
			DDL_ColumnName.Focus()
		End Try
	End Sub

	Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
		Try

			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
								GridViewRow)

			Dim PersonId As Integer = gvPersonnel.DataKeys(gvRow.RowIndex).Values("PersonId").ToString()

			Dim UniqueUserId As String = gvPersonnel.DataKeys(gvRow.RowIndex).Values("Id").ToString()

			Response.Redirect(String.Format("Personnel?PersonId={0}&UniqueUserId={1}", PersonId, UniqueUserId), False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub linkResetPassword_Click(sender As Object, e As EventArgs)
		Try

			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
								GridViewRow)
			Dim PersonId As Integer = gvPersonnel.DataKeys(gvRow.RowIndex).Values("PersonId").ToString()

			Dim UniqueUserId As String = gvPersonnel.DataKeys(gvRow.RowIndex).Values("Id").ToString()
			Response.Redirect(String.Format("ResetPassword?PersonId={0}&UniqueUserId={1}", PersonId, UniqueUserId), False)

		Catch ex As Exception
			log.Error("Error occurred in linkResetPassword_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try
			Dim dt As DataTable = CType(Session("dtPersonnel"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvPersonnel.DataSource = dt
			gvPersonnel.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder
		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvPersonnel_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvPersonnel.Sorting
		Try
			If e.SortExpression = ViewState("Column_Name").ToString() Then
				If ViewState("Sort_Order").ToString() = "ASC" Then
					RebindData(e.SortExpression, "DESC")
				Else
					RebindData(e.SortExpression, "ASC")
				End If

			Else
				RebindData(e.SortExpression, "ASC")
			End If

		Catch ex As Exception
			log.Error("Error occurred in gvPersonnel_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvPersonnel_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPersonnel.PageIndexChanging
		Try
			gvPersonnel.PageIndex = e.NewPageIndex
			Dim dtPersonnel As DataTable = Session("dtPersonnel")

			gvPersonnel.DataSource = dtPersonnel
			gvPersonnel.DataBind()
		Catch ex As Exception
			log.Error("Error occurred in gvPersonnel_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
		Try
			If (DDL_ColumnName.SelectedValue = "DepartmentId") Then
				DDL_Dept.Visible = True
				txt_value.Visible = False
				DDL_RoleId.Visible = False
				DDL_RequestFrom.Visible = False
				Chk_IsApproved.Visible = False
				Chk_SoftUpdate.Visible = False
				DDL_Customer.Visible = False
				DDL_Dept.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "CustomerId") Then
				DDL_Customer.Visible = True
				DDL_Dept.Visible = False
				txt_value.Visible = False
				DDL_RoleId.Visible = False
				DDL_RequestFrom.Visible = False
				Chk_IsApproved.Visible = False
				Chk_SoftUpdate.Visible = False
				DDL_Customer.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "RoleId") Then
				DDL_Dept.Visible = False
				txt_value.Visible = False
				DDL_RoleId.Visible = True
				DDL_RequestFrom.Visible = False
				Chk_IsApproved.Visible = False
				Chk_SoftUpdate.Visible = False
				DDL_Customer.Visible = False
				DDL_RoleId.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "RequestFrom") Then
				DDL_Dept.Visible = False
				txt_value.Visible = False
				DDL_RoleId.Visible = False
				DDL_RequestFrom.Visible = True
				Chk_IsApproved.Visible = False
				Chk_SoftUpdate.Visible = False
				DDL_Customer.Visible = False
				DDL_RequestFrom.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "SoftUpdate") Then
				DDL_Dept.Visible = False
				txt_value.Visible = False
				DDL_RoleId.Visible = False
				DDL_RequestFrom.Visible = False
				Chk_IsApproved.Visible = False
				Chk_SoftUpdate.Visible = True
				DDL_Customer.Visible = False
				Chk_SoftUpdate.Focus()
			ElseIf (DDL_ColumnName.SelectedValue = "IsApproved") Then
				DDL_Dept.Visible = False
				txt_value.Visible = False
				DDL_RoleId.Visible = False
				DDL_RequestFrom.Visible = False
				Chk_IsApproved.Visible = True
				Chk_SoftUpdate.Visible = False
				DDL_Customer.Visible = False
				Chk_IsApproved.Focus()
			Else
				DDL_Dept.Visible = False
				txt_value.Visible = True
				DDL_RoleId.Visible = False
				DDL_RequestFrom.Visible = False
				Chk_IsApproved.Visible = False
				Chk_SoftUpdate.Visible = False
				DDL_Customer.Visible = False
				txt_value.Focus()
			End If

			DDL_Dept.SelectedValue = 0
			DDL_Customer.SelectedValue = 0
			txt_value.Text = ""
			DDL_RoleId.SelectedValue = 0
			DDL_RequestFrom.SelectedValue = 0
			Chk_IsApproved.Checked = False
			Chk_SoftUpdate.Checked = False
		Catch ex As Exception
			log.Error("Error occurred in DDL_ColumnName_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal PersonId As String, ByVal UniqueUserId As String) As String
		Try
			Dim OBJMaster = New MasterBAL()
			Dim beforeData As String = ""
			OBJMaster = New MasterBAL()
			Dim resultInt As Integer = 0

			'check if transactions are available
			'if available then can not delete user
			'if not availabel then delete user
			Dim Context As HttpContext = HttpContext.Current

			Dim dtTransaction As DataTable = New DataTable
			dtTransaction = OBJMaster.GetTransactionDataByPersonId(PersonId)
			If (((Not dtTransaction Is Nothing) And (dtTransaction.Rows.Count = 0)) Or (HttpContext.Current.Session("RoleName") = "SuperAdmin")) Then

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					beforeData = CreateData(PersonId, UniqueUserId)
				End If

				Dim result As Boolean = OBJMaster.DeletePersonnel(PersonId, UniqueUserId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
				If (result = True) Then
					resultInt = 1

					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						CSCommonHelper.WriteLog("Deleted", "Personnel", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
					End If
					'Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
					'Dim user = New ApplicationUser()
					'user = manager.FindById(UniqueUserId)

					'Dim deleteUserResult As IdentityResult = manager.Delete(user)
					'If deleteUserResult.Succeeded Then
					'    resultInt = 1

					'    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					'        CSCommonHelper.WriteLog("Deleted", "Personnel", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
					'    End If

					'Else
					'    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					'        CSCommonHelper.WriteLog("Deleted", "Personnel", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Person deletion failed.")
					'    End If
					'    resultInt = 0
					'End If
				Else
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						CSCommonHelper.WriteLog("Deleted", "Personnel", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Person deletion failed.")
					End If
					resultInt = 0
				End If
			Else
				resultInt = -2
			End If
			Return resultInt
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/Personnel")
	End Sub

	Private Sub SendAuthorizedEmail(UserEmail As String)
		Try

			Dim body As String = String.Empty
			Using sr As New StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/AuthorizedEmail.txt"))
				body = sr.ReadToEnd()
			End Using
			'------------------
			body = body.Replace("UserEmail", UserEmail)

			Dim e As New EmailService()


			Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))

			mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
			mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

			Dim messageSend As New MailMessage()
			messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
			messageSend.[To].Add(New MailAddress(UserEmail))


			messageSend.Subject = ConfigurationManager.AppSettings("AuthorizedEmailSubject")
			messageSend.Body = body

			messageSend.IsBodyHtml = True
			mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))


			mailClient.Send(messageSend)

		Catch ex As Exception

			log.Error(String.Format("Error Occurred while sending Authorized email to user. Error is {0}. Details are email:{1}", ex.Message, UserEmail))

		End Try
	End Sub

	Private Shared Function CreateData(PersonId As Integer, UniqueUserId As String) As String
		Try
			Dim dtPersonnel As DataTable = New DataTable()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtPersonnel = OBJMaster.GetPersonnelByPersonIdAndId(PersonId, UniqueUserId)


			Dim data As String = "PersonId = " & dtPersonnel.Rows(0)("PersonId").ToString() & " ; " &
									"Is Hub user = " & dtPersonnel.Rows(0)("IsUserForHub").ToString() & " ; " &
									"Person Name = " & dtPersonnel.Rows(0)("PersonName").ToString().Replace(",", " ") & " ; " &
									"PIN (additional security) = " & dtPersonnel.Rows(0)("PinNumber").ToString().Replace(",", " ") & " ; " &
									"Fob/Card Number = " & dtPersonnel.Rows(0)("FOBNumber").ToString().Replace(",", " ") & " ; " &
									"Email (Username) = " & dtPersonnel.Rows(0)("Email").ToString().Replace(",", " & ") & " ; " &
									"Phone Number = " & dtPersonnel.Rows(0)("PhoneNumber").ToString().Replace(",", " & ") & " ; " &
									"IMEI Number = " & dtPersonnel.Rows(0)("IMEI_UDID").ToString().Replace(",", " & ") & " ; " &
									"Access Levels = " & dtPersonnel.Rows(0)("RoleName").ToString().Replace(",", " ") & " ; " &
									"Department Name = " & dtPersonnel.Rows(0)("DeptName").ToString().Replace(",", " ") & " ; " &
									"Department Number = " & dtPersonnel.Rows(0)("DeptNumber").ToString().Replace(",", " ") & " ; " &
									"Export Code = " & dtPersonnel.Rows(0)("ExportCode").ToString().Replace(",", " ") & " ; " &
									"Pre-Authorization Transactions Count = " & dtPersonnel.Rows(0)("PreAuth").ToString() & " ; " &
									"Fluid Limit Per Transaction = " & dtPersonnel.Rows(0)("FuelLimitPerTxn").ToString() & " ; " &
									"Fluid Limit Per Day = " & dtPersonnel.Rows(0)("FuelLimitPerDay").ToString() & " ; " &
									"Vehicles Allowed to Fuel = " & BindVehiclesDataToCheckboxList(PersonId) & " ; " &
									"Authorized Fueling Times = " & BindPersonTimings(PersonId) & " ; " &
									"Authorized Fueling links = " & BindSitesDataToCheckboxList(PersonId) & " ; " &
									"Update FluidSecure Link Software on next Fueling? = " & dtPersonnel.Rows(0)("SoftUpdate").ToString() & " ; " &
									"Active = " & dtPersonnel.Rows(0)("IsApproved").ToString() & " ; " &
									"Send Transaction Email = " & dtPersonnel.Rows(0)("SendTransactionEmail").ToString() & " ; " &
									"Additional Email = " & dtPersonnel.Rows(0)("AdditionalEmailId").ToString().Replace(",", " ") & " ; " &
									"Company = " & dtPersonnel.Rows(0)("CompanyName").ToString().Replace(",", " ") & ""

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

	Private Shared Function BindVehiclesDataToCheckboxList(PersonId As Integer) As String
		Dim beforeVehicles As String = ""

		Try

			Dim OBJMaster As MasterBAL = New MasterBAL()
			Dim dtPersonVehicleMapping As DataTable = New DataTable()

			dtPersonVehicleMapping = OBJMaster.GetPersonVehicleMapping(PersonId)

			For Each dr As DataRow In dtPersonVehicleMapping.Rows
				beforeVehicles = IIf(beforeVehicles = "", dr("VehicleNumber"), beforeVehicles & ";" & dr("VehicleNumber"))
			Next

		Catch ex As Exception

			log.Error("Error occurred in BindVehiclesDataToCheckboxList Exception is :" + ex.Message)
		End Try

		Return beforeVehicles

	End Function

	Private Shared Function BindPersonTimings(PersonId As Integer) As String
		Dim beforefuelTimings As String = ""
		Try

			Dim OBJMaster As MasterBAL = New MasterBAL()
			Dim dtPersonTimings As DataTable = New DataTable()

			dtPersonTimings = OBJMaster.GetPersonnelTimings(PersonId)

			If dtPersonTimings IsNot Nothing Then
				For Each dr As DataRow In dtPersonTimings.Rows

					beforefuelTimings = IIf(beforefuelTimings = "", dr("TimeText"), beforefuelTimings & ";" & dr("TimeText"))

				Next
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindPersonTimings Exception is :" + ex.Message)
		End Try

		Return beforefuelTimings

	End Function

	Private Shared Function BindSitesDataToCheckboxList(PersonId As Integer) As String

		Dim beforeFSlinks As String = ""
		Try

			Dim OBJMaster As MasterBAL = New MasterBAL()
			Dim dtPersonSiteMapping As DataTable = New DataTable()

			dtPersonSiteMapping = OBJMaster.GetPersonSiteMapping(PersonId, 0)
			If dtPersonSiteMapping IsNot Nothing Then
				For Each dr As DataRow In dtPersonSiteMapping.Rows

					beforeFSlinks = IIf(beforeFSlinks = "", dr("WifiSSID"), beforeFSlinks & ";" & dr("WifiSSID"))

				Next
			End If

		Catch ex As Exception

			log.Error("Error occurred in BindSitesDataToCheckboxList Exception is :" + ex.Message)

		End Try

		Return beforeFSlinks

	End Function

	Protected Sub linkViewLogs_Click(sender As Object, e As EventArgs)
		Try
			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
								GridViewRow)
			Dim PersonId As Integer = gvPersonnel.DataKeys(gvRow.RowIndex).Values("PersonId").ToString()

			Dim UniqueUserId As String = gvPersonnel.DataKeys(gvRow.RowIndex).Values("Id").ToString()
			Response.Redirect(String.Format("ViewCollectedDiagnosticLogs?PersonId={0}&UniqueUserId={1}", PersonId, UniqueUserId), False)

		Catch ex As Exception

			log.Error("Error occurred in linkViewLogs_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvPersonnel_RowDataBound(sender As Object, e As GridViewRowEventArgs)
		Try
			If (e.Row.RowType = DataControlRowType.DataRow) Then

				Dim PersonId As Integer = gvPersonnel.DataKeys(e.Row.RowIndex).Values("PersonId").ToString()
				Dim filepath As String = Server.MapPath("\DiagnosticLogs\" & PersonId & "\")
				Dim count As Integer = 0
				If (Directory.Exists(filepath)) Then
					Dim directories() As String = Directory.GetDirectories(filepath)

					For Each directoryTemp As String In directories

						Dim files() As String = Directory.GetFiles(directoryTemp)
						If (files.Length > 0) Then
							count = 1
							Exit For
						End If
					Next
				End If

				Dim linkViewLogs As LinkButton = DirectCast(e.Row.FindControl("linkViewLogs"), LinkButton)

				If (count > 0) Then
					linkViewLogs.Visible = True
				Else
					linkViewLogs.Visible = False
				End If

			End If
		Catch ex As Exception
			log.Error("Error occurred in gvPersonnel_RowDataBound Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnMyModalActiveInActiveNo_Click(sender As Object, e As EventArgs)
		checkActiveInActive(False)
	End Sub

	Protected Sub btnMyModalActiveInActiveSuccess_Click(sender As Object, e As EventArgs)
		checkActiveInActive(True)
	End Sub

	Private Sub checkActiveInActive(ActiveInActive As Boolean)
		Try

			Dim personId As Integer = hdfPersonIdActiveInactive.Value
			Dim Id As String = hdfIdActiveInactive.Value
			Dim UserEmail As String = hdfUserEmailActiveInactive.Value
			Dim IsUserForHub As Boolean = hdfIsUserForHubActiveInactive.Value
			Dim PersonActiveClick As Boolean = hdfConfirmActiveInactive.Value

			Dim beforeData As String = ""
			Dim afterData As String = ""

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(personId, Id)
			End If

			OBJMaster = New MasterBAL()

			Dim result As Integer = OBJMaster.UpdateActiveFlag(personId, Id, Session("PersonId"), PersonActiveClick)

			If result = 1 Then

				Try
					If Not IsUserForHub Then
						' Set IMEI Active - Inactive
						If ActiveInActive Then
							OBJMaster = New MasterBAL()
							OBJMaster.UpdateIMEIActiveFlagByPersonId(personId, Session("PersonId"), PersonActiveClick)
						End If
					End If
				Catch ex As Exception
					log.Error("Error occurred in checkActiveInActive + UpdateIMEIActiveFlagByPersonId Exception is :" + ex.Message)
				End Try


				message.InnerText = Resource.RecordSaved
				message.Visible = True
				If (PersonActiveClick = True) Then
					SendAuthorizedEmail(UserEmail)
				End If

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					afterData = CreateData(personId, Id)
				End If

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Modified", "Personnel", beforeData, afterData, HttpContext.Current.Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
				End If


			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Modified", "Personnel", beforeData, afterData, HttpContext.Current.Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Person activation failed.")
				End If
				ErrorMessage.InnerText = Resource.RecordSavingFailed
				ErrorMessage.Visible = True
			End If

		Catch ex As Exception
			ErrorMessage.InnerText = Resource.RecordSavingFailed
			ErrorMessage.Visible = True
			log.Error("Error occurred in checkActiveInActive Exception is :" + ex.Message)
		End Try
	End Sub
End Class