Imports log4net
Imports log4net.Config

Public Class AllVehicles
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllVehicles))

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
                    BindDept()
                    BindColumns()
                    BindCustomer()

                    If (Request.QueryString("Filter") = Nothing) Then
                        Session("VehConditions") = ""
                        Session("VehDDL_ColumnName") = ""
                        Session("Vehtxt_valueNameValue") = ""
                        Session("VehDDL_CustomerValue") = ""
                        Session("VehDDL_DeptValue") = ""
                    End If

                    If (Not Session("VehConditions") Is Nothing And Not Session("VehConditions") = "") Then
                        DDL_ColumnName.SelectedValue = Session("VehDDL_ColumnName")
                        If (Not Session("VehDDL_CustomerValue") Is Nothing And Not Session("VehDDL_CustomerValue") = "") Then
                            If (Session("VehDDL_ColumnName") = "CustomerId") Then
                                DDL_Customer.SelectedValue = Session("VehDDL_CustomerValue")
                                DDL_Customer.Visible = True
                                txt_value.Visible = False
                                DDL_Dept.Visible = False
                            ElseIf (Session("VehDDL_ColumnName") = "DepartmentId") Then
                                DDL_Dept.SelectedValue = Session("VehDDL_DeptValue")
                                DDL_Customer.Visible = False
                                txt_value.Visible = False
                                DDL_Dept.Visible = True
                            Else
                                DDL_Customer.Visible = False
                                txt_value.Visible = True
                                DDL_Dept.Visible = False
                                If (Not Session("Vehtxt_valueNameValue") Is Nothing And Not Session("Vehtxt_valueNameValue") = "") Then
                                    txt_value.Text = Session("Vehtxt_valueNameValue")
                                Else
                                    txt_value.Text = ""
                                End If
                            End If
                        End If
                    End If


                    btnSearch_Click(Nothing, Nothing)
                End If
                DDL_ColumnName.Focus()
            End If

        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Private Sub BindDept()
        Try

            OBJMaster = New MasterBAL()
            Dim dtDept As DataTable = New DataTable()

            dtDept = OBJMaster.GetDepartments(Session("PersonId").ToString(), Session("RoleId").ToString(), Session("CustomerId").ToString())

            DDL_Dept.DataSource = dtDept
            DDL_Dept.DataValueField = "DeptId"
            DDL_Dept.DataTextField = "Name"
            DDL_Dept.DataBind()
            DDL_Dept.Items.Insert(0, New ListItem("Select Department", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindDept Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting department, please try again later."

        End Try

    End Sub

    Private Sub BindColumns()
        Try

            OBJMaster = New MasterBAL()
            Dim dtColumns As DataTable = New DataTable()
            dtColumns = OBJMaster.GetVehicleColumnNameForSearch()

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

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
                DDL_Customer.SelectedIndex = 1
            End If

            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                If (Session("RoleName") = "GroupAdmin") Then
                    DDL_Customer.SelectedValue = Session("CustomerId")
                Else
                    DDL_Customer.SelectedIndex = 1
                End If
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try

    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Try

            OBJMaster = New MasterBAL()

            Dim strConditions As String = ""

            Session("VehConditions") = ""


            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                strConditions = " and V.CustomerId=" & Session("CustomerId")
            End If

            If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and V." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and V." + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
            ElseIf ((DDL_Dept.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and V." + DDL_ColumnName.SelectedValue + " = " + DDL_Dept.SelectedValue + "", strConditions + " and V." + DDL_ColumnName.SelectedValue + " = " + DDL_Dept.SelectedValue + "")
            ElseIf ((DDL_Customer.SelectedValue <> 0) And DDL_ColumnName.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and V." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "", strConditions + " and V." + DDL_ColumnName.SelectedValue + " = " + DDL_Customer.SelectedValue + "")
            End If

            strConditions = strConditions & " and V.VehicleNumber <> 'Default'"

            Dim dtVehicle As DataTable = New DataTable()

            Session("VehConditions") = strConditions
            Session("VehDDL_ColumnName") = DDL_ColumnName.SelectedValue
            Session("Vehtxt_valueNameValue") = txt_value.Text
            Session("VehDDL_CustomerValue") = DDL_Customer.SelectedValue
            Session("VehDDL_DeptValue") = DDL_Dept.SelectedValue

            dtVehicle = OBJMaster.GetVehicleByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString())

            Session("dtVehicle") = dtVehicle
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtVehicle IsNot Nothing Then
                If dtVehicle.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtVehicle.Rows.Count)
                End If
            End If
            gvVehicle.DataSource = dtVehicle
            gvVehicle.DataBind()

            ViewState("Column_Name") = "VehicleId"
            ViewState("Sort_Order") = "DESC"

        Catch ex As Exception

            log.Error("Error occurred in btnSearch_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        End Try

    End Sub

    Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
        Try

            Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
                                GridViewRow)

            Dim VehicleId As Integer = gvVehicle.DataKeys(gvRow.RowIndex).Values("VehicleId").ToString()

            Response.Redirect("Vehicle?VehicleId=" & VehicleId, False)
        Catch ex As Exception
            log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub RebindData(sColimnName As String, sSortOrder As String)
        Try

            Dim dt As DataTable = CType(Session("dtVehicle"), DataTable)
            dt.DefaultView.Sort = sColimnName + " " + sSortOrder
            gvVehicle.DataSource = dt
            gvVehicle.DataBind()
            ViewState("Column_Name") = sColimnName
            ViewState("Sort_Order") = sSortOrder
        Catch ex As Exception
            log.Error("Error occurred in RebindData Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub gvVehicle_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvVehicle.Sorting
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
            log.Error("Error occurred in gvVehicle_Sorting Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub gvVehicle_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvVehicle.PageIndexChanging
        Try
            gvVehicle.PageIndex = e.NewPageIndex

            Dim dtVehicle As DataTable = Session("dtVehicle")

            gvVehicle.DataSource = dtVehicle
            gvVehicle.DataBind()

        Catch ex As Exception
            log.Error("Error occurred in gvVehicle_PageIndexChanging Exception is :" + ex.Message)
        End Try

    End Sub

    Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ColumnName.SelectedIndexChanged
        Try

            If (DDL_ColumnName.SelectedValue = "DepartmentId") Then
                DDL_Dept.Visible = True
                txt_value.Visible = False
                DDL_Customer.Visible = False
                DDL_Dept.Focus()
            ElseIf (DDL_ColumnName.SelectedValue = "CustomerId") Then
                DDL_Dept.Visible = False
                txt_value.Visible = False
                DDL_Customer.Visible = True
                DDL_Customer.Focus()
            Else
                DDL_Dept.Visible = False
                txt_value.Visible = True
                DDL_Customer.Visible = False
                txt_value.Focus()
            End If
            txt_value.Text = ""
            DDL_Dept.SelectedValue = 0
            DDL_Customer.SelectedValue = 0

        Catch ex As Exception
            log.Error("Error occurred in DDL_ColumnName_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    <System.Web.Services.WebMethod(True)>
    Public Shared Function DeleteRecord(ByVal vehicleId As String) As String
        Try

            Dim OBJMaster = New MasterBAL()
            Dim beforeData As String = ""

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                beforeData = CreateData(vehicleId)
            End If

            Dim result As Integer = OBJMaster.DeleteVehicle(vehicleId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
            If (result > 0) Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Deleted", "Vehicles", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Deleted", "Vehicles", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Vehicle deletion failed.")
                End If
            End If
            Return result
        Catch ex As Exception
            log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
            Return 0
        End Try
    End Function

    Protected Sub btn_New_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Master/Vehicle")
    End Sub

    Private Shared Function CreateData(VehicleId As Integer) As String
        Try
            Dim dtVehicle As DataTable = New DataTable()

            Dim OBJMaster As MasterBAL = New MasterBAL()
            dtVehicle = OBJMaster.GetVehiclebyId(VehicleId)


            Dim data As String = "VehicleId = " & VehicleId & " ; " &
                                    "Vehicle Number = " & dtVehicle.Rows(0)("VehicleNumber").ToString().Replace(",", " ") & " ; " &
                                    "Vehicle Name = " & dtVehicle.Rows(0)("VehicleName").ToString().Replace(",", " ") & " ; " &
                                    "Description = " & dtVehicle.Rows(0)("Extension").ToString().Replace(",", " ") & " ; " &
                                    "Department = " & dtVehicle.Rows(0)("DeptName").ToString().Replace(",", " ") & " ; " &
                                    "Account ID = " & dtVehicle.Rows(0)("Acc_Id").ToString().Replace(",", " ") & " ; " &
                                    "Make = " & dtVehicle.Rows(0)("Make").ToString().Replace(",", " ") & " ; " &
                                    "Model = " & dtVehicle.Rows(0)("Model").ToString().Replace(",", " ") & " ; " &
                                    "VIN = " & dtVehicle.Rows(0)("VIN").ToString().Replace(",", " ") & " ; " &
                                    "Year = " & dtVehicle.Rows(0)("Year").ToString() & " ; " &
                                    "License Plate Number = " & dtVehicle.Rows(0)("LicensePlateNumber").ToString().Replace(",", " ") & " ; " &
                                    "License State = " & dtVehicle.Rows(0)("LicenseState").ToString().Replace(",", " ") & " ; " &
                                    "Type of Vehicle = " & dtVehicle.Rows(0)("Type").ToString().Replace(",", " ") & " ; " &
                                    "Fob/Card Number = " & dtVehicle.Rows(0)("FOBNumber").ToString().Replace(",", " ") & " ; " &
                                    "Products = " & BindFuelTypesData(VehicleId) & " ; " &
                                    "Authorized FluidSecure Links = " & BindVehicleSiteData(VehicleId) & " ; " &
                                    "Last Fluid Date/Time = " & dtVehicle.Rows(0)("LastFuelDateTime").ToString() & " ; " &
                                    "Current Odometer = " & dtVehicle.Rows(0)("CurrentOdometer").ToString() & " ; " &
                                    "Previous Odometer = " & dtVehicle.Rows(0)("PrevOdometer").ToString() & " ; " &
                                    "Last Fueler = " & dtVehicle.Rows(0)("PersonName").ToString().Replace(",", " ") & " ; " &
                                    "Require Odometer Entry = " & IIf(dtVehicle.Rows(0)("RequireOdometerEntry").ToString() = "1", "Yes", "No") & " ; " &
                                    "Hours = " & IIf(dtVehicle.Rows(0)("Hours").ToString() = True, "Yes", "No") & " ; " &
                                    "Check Odometer Reasonability = " & IIf(dtVehicle.Rows(0)("CheckOdometerReasonable").ToString() = "1", "Yes", "No") & " ; " &
                                    "Mileage / Kilometers = " & IIf(dtVehicle.Rows(0)("MileageOrKilometers").ToString() = "2", "Kilometers", "Mileage") & " ; " &
                                    "Odometer Reasonability either = " & IIf(dtVehicle.Rows(0)("OdometerReasonabilityConditions").ToString() = "2", "Don't allow fueling unless correct", "Allow fueling after 3 entry") & " ; " &
                                    "Total Miles allowed between Fueling = " & dtVehicle.Rows(0)("OdoLimit").ToString() & " ; " &
                                    "Fluid Limit Per Transaction = " & dtVehicle.Rows(0)("FuelLimitPerTxn").ToString() & " ; " &
                                    "Fuel Limit Per Day = " & dtVehicle.Rows(0)("FuelLimitPerDay").ToString() & " ; " &
                                    "Expected MPG or Liters/100KM = " & dtVehicle.Rows(0)("VehicleNumber").ToString() & " ; " &
                                    "Export Code = " & dtVehicle.Rows(0)("ExportCode").ToString().Replace(",", " ") & " ; " &
                                    "Comments = " & dtVehicle.Rows(0)("Comment").ToString().Replace(",", " ") & " ; " &
                                    "Company = " & dtVehicle.Rows(0)("CompanyName").ToString().Replace(",", " ") & "" &
                                    "IsActive = " & dtVehicle.Rows(0)("IsActive").ToString().Replace(",", " ") & ""

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Shared Function BindFuelTypesData(vehicleId As Integer) As String
        Dim products As String = ""
        Try

            Dim OBJMaster = New MasterBAL()
            Dim dtFuelTypeVehicleMapping As DataTable = New DataTable()
            dtFuelTypeVehicleMapping = OBJMaster.GetFuelTypeVehicleMapping(vehicleId)

            If dtFuelTypeVehicleMapping IsNot Nothing Then
                For Each dr As DataRow In dtFuelTypeVehicleMapping.Rows
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        products = IIf(products = "", dr("FuelType"), products & ";" & dr("FuelType"))
                    End If
                Next
            End If

            Return products

        Catch ex As Exception

            log.Error("Error occurred in BindFuelTypesData Exception is :" + ex.Message)

            Return products

        End Try


    End Function

    Private Shared Function BindVehicleSiteData(VehicleId As Integer) As String
        Dim links As String = ""

        Try

            Dim OBJMaster = New MasterBAL()
            Dim dtVehiclesiteMapping As DataTable = New DataTable()

            dtVehiclesiteMapping = OBJMaster.GetVehicleSiteMapping(VehicleId, 0)
            If dtVehiclesiteMapping IsNot Nothing Then
                For Each dr As DataRow In dtVehiclesiteMapping.Rows

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        links = IIf(links = "", dr("WifiSSId"), links & ";" & dr("WifiSSId"))
                    End If
                Next
            End If

            Return links
        Catch ex As Exception

            log.Error("Error occurred in BindVehicleSiteData Exception is :" + ex.Message)

            Return links

        End Try


    End Function

    Protected Sub CHK_Active_CheckedChanged(sender As Object, e As EventArgs)
        Try

            Dim cb As CheckBox = sender
            Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
                                          GridViewRow)
            Dim index As Integer = gvRow.RowIndex

            Dim VehicleId As Integer = gvVehicle.DataKeys(index).Values("VehicleId").ToString()

            Dim beforeData As String = ""
            Dim afterData As String = ""

            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                beforeData = CreateData(VehicleId)
            End If

            OBJMaster = New MasterBAL()

            Dim result As Integer = OBJMaster.UpdateVehicleInActiveFlag(VehicleId, Session("PersonId"), cb.Checked)

            If result = 1 Then
                message.InnerText = "Record saved."
                message.Visible = True

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    afterData = CreateData(VehicleId)
                End If

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Modified", "Vehicles", beforeData, afterData, HttpContext.Current.Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                End If


            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Modified", "Vehicles", beforeData, afterData, HttpContext.Current.Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Vehicle In activation failed.")
                End If
                ErrorMessage.InnerText = "Record saving failed."
                ErrorMessage.Visible = True
            End If

        Catch ex As Exception
            ErrorMessage.InnerText = "Record saving failed."
            ErrorMessage.Visible = True
            log.Error("Error occurred in CHK_Active_CheckedChanged Exception is :" + ex.Message)
        End Try
    End Sub
End Class
