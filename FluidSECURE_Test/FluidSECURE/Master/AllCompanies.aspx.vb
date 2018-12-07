Imports log4net
Imports log4net.Config

Public Class AllCompanies
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllCompanies))

	Dim OBJMaster As MasterBAL = New MasterBAL()

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try
			XmlConfigurator.Configure()

			message.Visible = False
			ErrorMessage.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If (Not IsPostBack) Then
					BindColumns()
                    If Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "GroupAdmin" Then
                        btn_New.Visible = False
                    End If

					If (Request.QueryString("Filter") = Nothing) Then
						Session("CompanyConditions") = ""
						Session("CompanyDDL_ColumnName") = ""
						Session("Companytxt_valueNameValue") = ""
					End If


					If (Not Session("CompanyDDL_ColumnName") Is Nothing And Not Session("CompanyDDL_ColumnName") = "") Then
						DDL_Column.SelectedValue = Session("CompanyDDL_ColumnName")

						If (Not Session("Companytxt_valueNameValue") Is Nothing And Not Session("Companytxt_valueNameValue") = "") Then
							txt_value.Text = Session("Companytxt_valueNameValue")
						Else
							txt_value.Text = ""
						End If

					End If

					btnSearch_Click(Nothing, Nothing)
					DDL_Column.Focus()
				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try

	End Sub

	Private Sub BindAllCust()
		Try
			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
			gvCust.DataSource = dtCust
			gvCust.DataBind()

		Catch ex As Exception

			log.Error("Error occurred in BindAllCust Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting company, please try again later."

		End Try
	End Sub

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
            Dim dtColumns As DataTable = New DataTable()
            Dim dtSource As DataTable = New DataTable()
            dtColumns = OBJMaster.GetCustmerColumnNameForSearch()
            dtSource = dtColumns.Clone
            If Session("RoleName") <> "SuperAdmin" Then
                For Each r As DataRow In dtColumns.Rows
                    If r("ColumnName") <> "IsCustomerActive" Then
                        dtSource.Rows.Add(r.ItemArray())
                    End If
                Next
                dtColumns = dtSource
            End If
            DDL_Column.DataSource = dtColumns
                DDL_Column.DataValueField = "ColumnName"
                DDL_Column.DataTextField = "ColumnEnglishName"
                DDL_Column.DataBind()


                DDL_Column.Items.Insert(0, New ListItem("Select Column", "0"))

        Catch ex As Exception

			log.Error("Error occurred in BindColumns Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting search column, please try again later."

		End Try

	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			OBJMaster = New MasterBAL()

			Dim strConditions As String = ""
			Session("CompanyConditions") = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions = " and CustomerId=" & Session("CustomerId")
			End If

            If ((Not txt_value.Text = "") And DDL_Column.SelectedValue = "CustomerName") Then
                strConditions = IIf(strConditions = "", " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text.Replace("'", "''") + "%'", strConditions + " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text.Replace("'", "''") + "%'")
            ElseIf ((Not txt_value.Text = "") And DDL_Column.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text + "%'")
            ElseIf DDL_Column.SelectedValue <> "0" And DDL_Column.SelectedValue = "IsCustomerActive" Then
                If chk_Active.Checked Then
                    strConditions = IIf(strConditions = "", " and " + DDL_Column.SelectedValue.ToString() + " = '1' ", strConditions + " and " + DDL_Column.SelectedValue.ToString() + " = '1' ")
                Else
                    strConditions = IIf(strConditions = "", " and " + DDL_Column.SelectedValue.ToString() + " = '0' ", strConditions + " and " + DDL_Column.SelectedValue.ToString() + " = '0' ")
                End If

            End If

            Session("CompanyConditions") = strConditions
			Session("CompanyDDL_ColumnName") = DDL_Column.SelectedValue
			Session("Companytxt_valueNameValue") = txt_value.Text

			OBJMaster = New MasterBAL()
			Dim dtCust As DataTable = New DataTable()

			dtCust = OBJMaster.GetCustByConditions(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtCust") = dtCust
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtCust IsNot Nothing Then
                If dtCust.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtCust.Rows.Count)
                End If
            End If
            gvCust.DataSource = dtCust
			gvCust.DataBind()
			If Session("RoleName") = "CustomerAdmin" Then
				If gvCust.Columns.Count > 0 Then
					'assuming your date-column is the 1.'
					gvCust.Columns(1).Visible = False
                    gvCust.Columns(2).Visible = False
                End If
			End If
			ViewState("Column_Name") = "CustomerName"
			ViewState("Sort_Order") = "ASC"

		Catch ex As Exception

			log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)

			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			DDL_Column.Focus()

		End Try
	End Sub

	Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
		Try

			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
									GridViewRow)

			Dim CustId As Integer = gvCust.DataKeys(gvRow.RowIndex).Values("CustomerId").ToString()

			Response.Redirect("Company.aspx?CustId=" & CustId, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try

			Dim dt As DataTable = CType(Session("dtCust"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvCust.DataSource = dt
			gvCust.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder

		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvCust_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvCust.Sorting
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
			log.Error("Error occurred in gvCust_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Protected Sub gvCust_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvCust.PageIndexChanging
		Try
			gvCust.PageIndex = e.NewPageIndex

			Dim dtCust As DataTable = Session("dtCust")

			gvCust.DataSource = dtCust
			gvCust.DataBind()
		Catch ex As Exception
			log.Error("Error occurred in gvCust_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal CompanyId As String) As String
		Try
			Dim beforeData As String = ""
			Dim OBJMaster = New MasterBAL()
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(CompanyId)
			End If

			Dim result As Integer = OBJMaster.DeleteCustomer(CompanyId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result <> 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Company", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Company", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Company deletion failed.")
				End If
			End If
			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

    <System.Web.Services.WebMethod(True)>
    Public Shared Function ActiveInActiveCompanyRecord(ByVal CustomerId As String, ByVal CheckActiveConfirm As String) As String
        Try
            Dim beforeData As String = ""
            Dim OBJMaster = New MasterBAL()
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                beforeData = CreateData(CustomerId)
            End If

            Dim result As Integer = OBJMaster.ActiveInActiveCompany(CustomerId, CheckActiveConfirm, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
            If (result <> 0) Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Modified", "Company", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Modified", "Company", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Company Active/DeActive failed.")
                End If
            End If
            Return result
        Catch ex As Exception
            log.Error("Error occurred in ActiveInActiveCompanyRecord Exception is :" + ex.Message)
            Return 0
        End Try
    End Function

    Protected Sub btn_New_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/Company")
	End Sub

	Private Shared Function CreateData(CompanyId As Integer) As String
		Try

			Dim data As String = ""

			Dim dtCompany As DataTable = New DataTable()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtCompany = OBJMaster.GetCustomerId(CompanyId)

			data = "CompanyId = " & CompanyId & " ; " &
				"Company Name = " & dtCompany.Rows(0)("CustomerName").ToString().Replace(",", " ") & " ; " &
						"Contact Name = " & dtCompany.Rows(0)("ContactName").ToString().Replace(",", " ") & " ; " &
						"Contact Address = " & dtCompany.Rows(0)("ContactAddress").ToString().Replace(",", " ") & " ; " &
						"Export Code = " & dtCompany.Rows(0)("ExportCode").ToString().Replace(",", " ") & " ; " &
						"Require Login = " & IIf(dtCompany.Rows(0)("IsLoginRequire").ToString() = "True", "Yes", "No") & " ; " &
						"Require Department = " & IIf(dtCompany.Rows(0)("IsDepartmentRequire").ToString() = "True", "Yes", "No") & " ; " &
						"Require Personnel PIN = " & IIf(dtCompany.Rows(0)("IsPersonnelPINRequire").ToString() = "True", "Yes", "No") & " ; " &
						"Require Other = " & IIf(dtCompany.Rows(0)("IsOtherRequire").ToString() = "True", "Yes", "No") & " ; " &
						"Other label = " & dtCompany.Rows(0)("OtherLabel").ToString().Replace(",", " ") & " ; " &
						"Contact Phone Number = " & dtCompany.Rows(0)("ContactNumber").ToString().Replace(",", " ") & " ; "

			Dim dscustomerAdmin = OBJMaster.GetCustomerAdmin(CompanyId)
			If dscustomerAdmin.Tables.Count > 0 Then
				If dscustomerAdmin.Tables(0).Rows.Count > 0 Then
					Dim dtCustAdmin = dscustomerAdmin.Tables(0)
					data += "Contact Email = " & dtCustAdmin.Rows(0)("UserName").ToString() & " ; "
				Else
					data += "Contact Email = " & " ; "
				End If
			Else
				data += "Contact Email = " & " ; "
			End If

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

    Protected Sub DDL_Column_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            If (DDL_Column.SelectedValue = "IsCustomerActive") Then
                chk_Active.Visible = True
                txt_value.Visible = False
                chk_Active.Focus()
            Else
                chk_Active.Visible = False
                txt_value.Visible = True
                txt_value.Focus()
            End If
            txt_value.Text = ""
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while DDL_Column_SelectedIndexChanged. Error is {0}.", ex.Message))
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

End Class
