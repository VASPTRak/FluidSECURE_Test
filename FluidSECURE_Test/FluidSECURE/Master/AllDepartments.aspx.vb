Imports log4net
Imports log4net.Config

Public Class AllDepartment
	Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllDepartment))

	Dim OBJMaster As MasterBAL


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
				If (Not IsPostBack) Then

					BindColumns()
					BindCustomer()

					If (Request.QueryString("Filter") = Nothing) Then
						Session("DeptConditions") = ""
						Session("DeptDDL_ColumnName") = ""
						Session("Depttxt_valueNameValue") = ""
						Session("DeptDDL_CustomerValue") = ""
					End If

					If (Not Session("DeptDDL_ColumnName") Is Nothing And Not Session("DeptDDL_ColumnName") = "") Then
						DDL_ColumnName.SelectedValue = Session("DeptDDL_ColumnName")
						If (Not Session("DeptDDL_CustomerValue") Is Nothing And Not Session("DeptDDL_CustomerValue") = "") Then
							If (Session("DeptDDL_CustomerValue") <> 0) Then
								DDL_Customer.SelectedValue = Session("DeptDDL_CustomerValue")
								DDL_Customer.Visible = True
								txt_value.Visible = False
							Else
								If (Not Session("Depttxt_valueNameValue") Is Nothing And Not Session("Depttxt_valueNameValue") = "") Then
									txt_value.Text = Session("Depttxt_valueNameValue")
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

	Private Sub BindAllDept()
		Try

			OBJMaster = New MasterBAL()
			Dim dtDept As DataTable = New DataTable()

			dtDept = OBJMaster.GetDepartments(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())

			gvDept.DataSource = dtDept
			gvDept.DataBind()

		Catch ex As Exception

			log.Error("Error occurred in BindAllDept Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting department, please try again later."

		End Try

	End Sub

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetDeptColumnNameForSearch()

			DDL_ColumnName.DataSource = dtColumns
			DDL_ColumnName.DataValueField = "ColumnName"
			DDL_ColumnName.DataTextField = "ColumnEnglishName"
			DDL_ColumnName.DataBind()
			DDL_ColumnName.Items.Insert(0, New ListItem("Select Column", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindColumns Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting search column, please try again later."

		End Try

	End Sub

	Private Sub BindCustomer()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
			DDL_Customer.DataSource = dtColumns
			DDL_Customer.DataValueField = "CustomerId"
			DDL_Customer.DataTextField = "CustomerName"
			DDL_Customer.DataBind()
			DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

		Catch ex As Exception

			log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting customers, please try again later."

		End Try
	End Sub

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
		Try

			Dim strConditions As String = ""

			Session("DeptConditions") = ""

			If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
				strConditions = " and DEPT.CustomerId=" & Session("CustomerId")
			End If

			If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and DEPT." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and DEPT." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
			ElseIf ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
				strConditions = IIf(strConditions = "", " and DEPT." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and DEPT." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
			End If

			Session("DeptConditions") = strConditions
			Session("DeptDDL_ColumnName") = DDL_ColumnName.SelectedValue
			Session("Depttxt_valueNameValue") = txt_value.Text
			Session("DeptDDL_CustomerValue") = DDL_Customer.SelectedValue

			OBJMaster = New MasterBAL()
			Dim dtDept As DataTable = New DataTable()

			dtDept = OBJMaster.GetDeptbyConditions(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

			Session("dtDept") = dtDept
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtDept IsNot Nothing Then
                If dtDept.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtDept.Rows.Count)
                End If
            End If
            gvDept.DataSource = dtDept
			gvDept.DataBind()

			ViewState("Column_Name") = "Name"
			ViewState("Sort_Order") = "ASC"
			RebindData("Name", "ASC")

		Catch ex As Exception

			log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		Finally
			DDL_ColumnName.Focus()
		End Try
	End Sub

	Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
		Try

			Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
								GridViewRow)

			Dim DeptId As Integer = gvDept.DataKeys(gvRow.RowIndex).Values("DeptId").ToString()

			Response.Redirect("Department?DeptId=" & DeptId, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try

			Dim dt As DataTable = CType(Session("dtDept"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvDept.DataSource = dt
			gvDept.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder
		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub gvDept_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvDept.Sorting
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
			log.Error("Error occurred in gvDept_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub gvDept_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvDept.PageIndexChanging
		Try
			gvDept.PageIndex = e.NewPageIndex
			Dim dtDept As DataTable = Session("dtDept")

			gvDept.DataSource = dtDept
			gvDept.DataBind()

		Catch ex As Exception
			log.Error("Error occurred in gvDept_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
		Try
			If (DDL_ColumnName.SelectedValue = "CustomerId") Then
				DDL_Customer.Visible = True
				txt_value.Visible = False
				DDL_Customer.Focus()
			Else
				DDL_Customer.Visible = False
				txt_value.Visible = True
				txt_value.Focus()
			End If
			txt_value.Text = ""
			DDL_Customer.SelectedValue = 0

		Catch ex As Exception
			log.Error("Error occurred in DDL_ColumnName_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal DeptId As String) As String
		Try
			Dim beforeData As String = ""
			Dim OBJMaster = New MasterBAL()
			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(DeptId)
			End If


			Dim result As Integer = OBJMaster.DeleteDept(DeptId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result <> 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Department", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Department", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Department deletion failed.")
				End If
			End If

			Return result

		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	Protected Sub btn_New_Click(sender As Object, e As EventArgs)

		Response.Redirect("~/Master/Department")

	End Sub

	Private Shared Function CreateData(DeptId As Integer) As String
		Try
			Dim dtDept As DataTable = New DataTable()

			Dim OBJMaster As MasterBAL = New MasterBAL()
			dtDept = OBJMaster.GetDeptbyId(DeptId)


			Dim data As String = "DepartmentId = " & dtDept.Rows(0)("DeptID").ToString() & " ; " &
								  "Department Number = " & dtDept.Rows(0)("NUMBER").ToString().Replace(",", " ") & " ; " &
								  "Department Name = " & dtDept.Rows(0)("NAME").ToString().Replace(",", " ") & " ; " &
								  "Account Number = " & dtDept.Rows(0)("ACCT_ID").ToString().Replace(",", " ") & " ; " &
								  "Address = " & dtDept.Rows(0)("ADDRESS1").ToString().Replace(",", " ") & " ; " &
								  "Address 2 = " & dtDept.Rows(0)("ADDRESS2").ToString().Replace(",", " ") & " ; " &
								  "Company = " & dtDept.Rows(0)("CompanyName").ToString().Replace(",", " ") & " ; " &
								  "Export Code = " & dtDept.Rows(0)("ExportCode").ToString().Replace(",", " & ") & " ; " &
								  "Surcharge Type = " & dtDept.Rows(0)("SurchargeTypeName").ToString().Replace(",", " ") & " ; " &
								  "Vehicle Surcharge (Lum Sum) = " & dtDept.Rows(0)("VehicleSum").ToString().Replace(",", " ") & " ; " &
								  "Department Surcharge  (Lum Sum) = " & dtDept.Rows(0)("DeptSum").ToString().Replace(",", " ") & " ; " &
								  "Vehicle Surcharge (%) = " & dtDept.Rows(0)("VehPercentage").ToString().Replace(",", " ") & " ; " &
								  "Department Surcharge (%) = " & dtDept.Rows(0)("DeptPercentage").ToString().Replace(",", " ") & " ; "

			Return data
		Catch ex As Exception
			log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
			Return ""
		End Try

	End Function

    Protected Sub gvDept_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim IsDefault As Boolean = gvDept.DataKeys(e.Row.RowIndex).Values("IsDefault").ToString()

                ' Dim lnkDelete As HyperLink = DirectCast(e.Row.FindControl("lnkDelete"), HyperLink)

                If IsDefault Then
                    'e.Row.Cells(1).Visible = False
                    e.Row.Cells(1).Text = ""
                Else
                    e.Row.Cells(1).Visible = True
                End If

            End If
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while gvDept_RowDataBound. Error is {0}.", ex.Message))
        End Try
    End Sub
End Class