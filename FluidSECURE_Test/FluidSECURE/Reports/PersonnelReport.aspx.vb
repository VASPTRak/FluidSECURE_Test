Imports log4net
Imports log4net.Config

Public Class PersonnelReport
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(PersonnelReport))

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

					BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					DDL_Customer_SelectedIndexChanged(Nothing, Nothing)

				Else

				End If

				DDL_Customer.Focus()
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

	Private Sub BindAllPersonnels(CustomerId As Integer)
		Try
			Dim dtPersonnel As DataTable = New DataTable()
            OBJMaster = New MasterBAL()

            If (DDL_Dept.SelectedValue.ToString() <> "0") Then
                dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(IsFluidSecureHub,0)=0 and cust.CustomerId = " & CustomerId & " and ANU.DepartmentId = " & DDL_Dept.SelectedValue.ToString() & " ", Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            Else
                dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(IsFluidSecureHub,0)=0 and cust.CustomerId = " & CustomerId, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            End If

            DDL_Personnel.DataSource = dtPersonnel
			DDL_Personnel.DataValueField = "PersonId"
			DDL_Personnel.DataTextField = "Person"
			DDL_Personnel.DataBind()

            DDL_Personnel.Items.Insert(0, New ListItem("Select All Personnel", "-1"))
            DDL_Personnel.Items.Insert(0, New ListItem("Select All Active Personnel", "0"))

        Catch ex As Exception

			log.Error("Error occurred in BindAllPersonnels Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting Personnels, please try again later."

		End Try
	End Sub

	Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
        Try
            BindDepartment(Convert.ToInt32(DDL_Customer.SelectedValue))
            BindAllPersonnels(Convert.ToInt32(DDL_Customer.SelectedValue))
        Catch ex As Exception
            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

		Try


			OBJMaster = New MasterBAL()
			Dim dtPersonnel As DataTable = New DataTable()

			Dim strConditions As String = ""
			If (DDL_Customer.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " where ANU.CustomerId = " + DDL_Customer.SelectedValue, strConditions + " and ANU.CustomerId = " + DDL_Customer.SelectedValue)
			End If


            If (DDL_Personnel.SelectedValue = "0") Then
                strConditions = IIf(strConditions = "", " where ANU.IsApproved=1 ", strConditions + " and ANU.IsApproved=1 ")
            ElseIf (DDL_Personnel.SelectedValue <> "-1") Then
                strConditions = IIf(strConditions = "", " where ANU.PersonId = " + DDL_Personnel.SelectedValue, strConditions + " and ANU.PersonId = " + DDL_Personnel.SelectedValue)
            End If

            If (DDL_Dept.SelectedValue.ToString() <> "0") Then
                strConditions = IIf(strConditions = "", " and ANU.DepartmentId = " + DDL_Dept.SelectedValue + " ", strConditions + " and ANU.DepartmentId = " + DDL_Dept.SelectedValue + " ")
            End If

            strConditions += "  and ISNULL(ANU.IsFluidSecureHub,0)=0 and ISNULL(ANU.IsDeleted,0)=0 order by ANU.PersonName"
            'get data from server
            dtPersonnel = OBJMaster.GetPersonalDetails(strConditions)
			If (Not dtPersonnel Is Nothing) Then

				If (dtPersonnel.Rows.Count <= 0) Then
					If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
						Dim writtenData As String = CreateData()

						CSCommonHelper.WriteLog("Report Genereated", "Personnel Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
					End If
					ErrorMessage.InnerText = "Data not found against selected criteria."
					ErrorMessage.Visible = True
					Return
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					Dim writtenData As String = CreateData()

					CSCommonHelper.WriteLog("Report Genereated", "Personnel Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
				End If
				ErrorMessage.InnerText = "Data not found against selected criteria."
				ErrorMessage.Visible = True
				Return

			End If
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData()

				CSCommonHelper.WriteLog("Report Genereated", "Personnel Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
			End If
			Session("PersonnelDetails") = dtPersonnel

			Response.Redirect("~/Reports/PersonnelRPT")


		Catch ex As Exception
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				Dim writtenData As String = CreateData()

				CSCommonHelper.WriteLog("Report Genereated", "Personnel Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
			End If
			ErrorMessage.InnerText = "Data not found against selected criteria."
			ErrorMessage.Visible = True
			Return
		Finally
			DDL_Customer.Focus()
		End Try

	End Sub

	Private Function CreateData() As String
		Try

            Dim data As String = "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                                 "Department = " & DDL_Dept.SelectedItem.Text.Replace(",", " ") & " ; " &
                                 "Person = " & DDL_Personnel.SelectedItem.Text.Replace(",", " ") & " ; "
            Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

    Protected Sub DDL_Dept_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            BindAllPersonnels(Convert.ToInt32(DDL_Customer.SelectedValue))
        Catch ex As Exception
            log.Error("Error occurred in DDL_Dept_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub BindDepartment(CustomerId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtDept As DataTable = New DataTable()
            If CustomerId <> 0 Then
                dtDept = OBJMaster.GetDepartmentsByCustomerId(CustomerId)
                DDL_Dept.DataSource = dtDept
            Else
                DDL_Dept.DataSource = dtDept
            End If

            DDL_Dept.DataTextField = "NAME"
            DDL_Dept.DataValueField = "DeptId"

            DDL_Dept.DataBind()
            DDL_Dept.Items.Insert(0, New ListItem("Select All Department", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindDepartment Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting departments, please try again later."

        End Try
    End Sub
End Class