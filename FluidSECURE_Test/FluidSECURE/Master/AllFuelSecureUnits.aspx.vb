Imports log4net
Imports log4net.Config

Public Class AllFuelSecureUnits
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllFuelSecureUnits))

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
				Return
			Else
				If (Not IsPostBack) Then
					BindColumns()
					GetCustomers(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
					ddl_FuelType.Visible = False
					ddl_CustomerId.Visible = False

					If (Request.QueryString("Filter") = Nothing) Then
						Session("FuelLINKConditions") = ""
						Session("FuelLINKDDL_ColumnName") = ""
						Session("FuelLINKtxt_valueNameValue") = ""
						Session("FuelLINKDDL_CustomerValue") = ""
					End If

					If (Not Session("FuelLINKDDL_ColumnName") Is Nothing And Not Session("FuelLINKDDL_ColumnName") = "") Then
						DDL_ColumnName.SelectedValue = Session("FuelLINKDDL_ColumnName")
						If (Not Session("FuelLINKDDL_CustomerValue") Is Nothing And Not Session("FuelLINKDDL_CustomerValue") = "") Then
							If (Session("FuelLINKDDL_CustomerValue") <> 0) Then
								ddl_CustomerId.SelectedValue = Session("FuelLINKDDL_CustomerValue")
								ddl_CustomerId.Visible = True
								txt_value.Visible = False
							Else
								If (Not Session("FuelLINKtxt_valueNameValue") Is Nothing And Not Session("FuelLINKtxt_valueNameValue") = "") Then
									txt_value.Text = Session("FuelLINKtxt_valueNameValue")
								Else
									txt_value.Text = ""
								End If
							End If
						End If
					End If

					btnSearch_Click(Nothing, Nothing)
                    DDL_ColumnName.Focus()

                    If Session("RoleName") = "SuperAdmin" Then
                        btn_New.Visible = True
                    Else
                        btn_New.Visible = False
                    End If

                End If
			End If


		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

    Private Sub GetCustomers(PersonId As Integer, RoleId As String)

        Try

            OBJMaster = New MasterBAL()
            Dim dtCustomer As DataTable = New DataTable()
            dtCustomer = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())
            ddl_CustomerId.DataSource = dtCustomer
            ddl_CustomerId.DataTextField = "CustomerName"
            ddl_CustomerId.DataValueField = "CustomerId"
            ddl_CustomerId.DataBind()
            ddl_CustomerId.Items.Insert(0, New ListItem("Select Company", "0"))

        Catch ex As Exception

            log.Error("Error occurred in GetCustomers Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetSiteColumnNameForSearch()

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

	Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Try

            OBJMaster = New MasterBAL()

            Dim strConditions As String = ""
            Session("FuelLINKConditions") = ""
            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                strConditions = " and s.CustomerId=" & Session("CustomerId")
            End If

            If ((Not txt_value.Text = "") And DDL_ColumnName.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and " + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and " + DDL_ColumnName.SelectedValue + " like '%" + txt_value.Text + "%'")
            ElseIf ((ddl_CustomerId.SelectedValue <> "0") And DDL_ColumnName.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and s." + DDL_ColumnName.SelectedValue + " = " + ddl_CustomerId.SelectedValue + "", strConditions + " and s." + DDL_ColumnName.SelectedValue + " = " + ddl_CustomerId.SelectedValue + "")
            End If

            Session("FuelLINKConditions") = strConditions
            Session("FuelLINKDDL_ColumnName") = DDL_ColumnName.SelectedValue
            Session("FuelLINKtxt_valueNameValue") = txt_value.Text
            Session("FuelLINKDDL_CustomerValue") = ddl_CustomerId.SelectedValue

            Dim dtSite As DataTable = New DataTable()

            dtSite = OBJMaster.GetSiteByCondition(strConditions, Session("PersonId").ToString(), Session("RoleId").ToString(), False)

            Session("dtSite") = dtSite
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtSite IsNot Nothing Then
                If dtSite.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtSite.Rows.Count)
                End If
            End If
            gvSite.DataSource = dtSite
            gvSite.DataBind()

            ViewState("Column_Name") = "SiteNumber"
            ViewState("Sort_Order") = "ASC"

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

			Dim SiteID As Integer = gvSite.DataKeys(gvRow.RowIndex).Values("SiteID").ToString()

			Response.Redirect("FuelSecureUnit?SiteID=" & SiteID, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try

			Dim dt As DataTable = CType(Session("dtSite"), DataTable)
			dt.DefaultView.Sort = sColimnName + " " + sSortOrder
			gvSite.DataSource = dt
			gvSite.DataBind()
			ViewState("Column_Name") = sColimnName
			ViewState("Sort_Order") = sSortOrder

		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

    Protected Sub gvSite_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gvSite.Sorting
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
			log.Error("Error occurred in gvSite_Sorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

    Protected Sub gvSite_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvSite.PageIndexChanging
        Try
            gvSite.PageIndex = e.NewPageIndex
            Dim dtSite As DataTable = Session("dtSite")

            gvSite.DataSource = dtSite
            gvSite.DataBind()

        Catch ex As Exception
            log.Error("Error occurred in gvSite_PageIndexChanging Exception is :" + ex.Message)
        End Try

    End Sub

    Protected Sub DDL_ColumnName_SelectedIndexChanged(sender As Object, e As EventArgs)
		Try

			If (DDL_ColumnName.SelectedValue = "CustomerId") Then
				txt_value.Visible = False
				ddl_CustomerId.Visible = True
				ddl_CustomerId.Focus()
			Else
				txt_value.Visible = True
				ddl_CustomerId.Visible = False
				txt_value.Focus()
			End If
			txt_value.Text = ""
			ddl_CustomerId.SelectedValue = 0

		Catch ex As Exception
			log.Error("Error occurred in DDL_ColumnName_SelectedIndexChanged Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    <System.Web.Services.WebMethod(True)> _
    Public Shared Function DeleteRecord(ByVal SiteID As String) As String
		Try
			Dim OBJMaster = New MasterBAL()
			Dim beforeData As String = ""

			If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
				beforeData = CreateData(SiteID)
			End If

			Dim result As Integer = OBJMaster.DeleteSite(SiteID, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Fluid Secure Links", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Fluid Secure Links", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Fluid Secure Link deletion failed.")
				End If
			End If

			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

    Protected Sub btn_New_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Master/FuelSecureUnit")
    End Sub

    Shared Function CreateData(SiteID As Integer) As String
        Try

            Dim dtSite As DataTable = New DataTable()
            Dim dtHose As DataTable = New DataTable()



            Dim OBJMaster As MasterBAL = New MasterBAL()
            dtSite = OBJMaster.GetSiteId(SiteID)
            dtHose = OBJMaster.GetHoseIdBySiteId(SiteID)

            Dim data As String = "SiteID = " & SiteID & " ; " &
                                    "FluidSecure Link Number = " & dtSite.Rows(0)("SiteNumber").ToString().Replace(",", " ") & " ; " &
                                    "FluidSecure Link Current Name (SSID) = " & dtHose.Rows(0)("WifiSSId").ToString().Replace(",", " ") & " ; " &
                                    "FluidSecure New Name (will change on next fueling) = " & dtHose.Rows(0)("ReplaceableHoseName").ToString().Replace(",", " ") & " ; " &
                                    "Tank Number = " & dtHose.Rows(0)("TankNumber").ToString().Replace(",", " ") & " ; " &
                                    "Product in Tank = " & dtHose.Rows(0)("FuelType").ToString().Replace(",", " ") & " ; " &
                                    "Tank Monitor = " & IIf(dtHose.Rows(0)("TankMonitor").ToString() = "Y", "Yes", "No") & " ; " &
                                    "Tank Monitor Number = " & dtHose.Rows(0)("TankMonitorNumber").ToString().Replace(",", " ") & " ; " &
                                    "Authorized Fueling Times = " & BindSiteTimings(SiteID) & " ; " &
                                    "Authorized Fueling Days = " & BindSiteDays(SiteID) & " ; " &
                                    "Export Code = " & dtSite.Rows(0)("ExportCode").ToString().Replace(",", " ") & " ; " &
                                    "Units measured = " & dtHose.Rows(0)("Unitsmeasured").ToString().Replace(",", " ") & " ; " &
                                    "Pulses = " & dtHose.Rows(0)("Pulses").ToString().Replace(",", " ") & " ; " &
                                    "Pulser Ratio = " & dtHose.Rows(0)("PulserRatio").ToString().Replace(",", " ") & " ; " &
                                    "Time Zone = " & dtHose.Rows(0)("ActualTimeZone").ToString().Replace(",", " ") & " ; " &
                                    "Selected time zone = " & dtHose.Rows(0)("TimeZoneName").ToString().Replace(",", " ") & " ; " &
                                    "Disable Geo Location = " & IIf(dtSite.Rows(0)("DisableGeoLocation").ToString() = True, "Yes", "No") & " ; " &
                                    "Latitude = " & dtSite.Rows(0)("Latitude").ToString().Replace(",", " ") & " ; " &
                                    "Longitude = " & dtSite.Rows(0)("Longitude").ToString().Replace(",", " ") & " ; " &
                                    "Address = " & dtSite.Rows(0)("SiteAddress").ToString().Replace(",", " ") & " ; " &
                                    "Pump On Time = " & dtHose.Rows(0)("PumpOnTime").ToString() & " ; " &
                                    "Pump Off Time = " & dtHose.Rows(0)("PumpOffTime").ToString() & " ; " &
                                    "MAC Address = " & dtHose.Rows(0)("IPAddress").ToString().Replace(",", " ") & " ; " &
                                    "Connected Hub = " & dtHose.Rows(0)("ConnectedHub").ToString().Replace(",", " ") & " ; " &
                                    "Company = " & dtSite.Rows(0)("CustomerName").ToString().Replace(",", " ") & " ; " &
                                    "Original Name Of FluidSecure Link = " & dtHose.Rows(0)("OriginalNameOfFluidSecure").ToString().Replace(",", " ") & " ; " &
                                    "Current Firmware Version = " & dtHose.Rows(0)("CurrentFirmwareVersion").ToString().Replace(",", " ") & " ; " &
                                    "Is FluidSecure Link Busy = " & IIf(dtHose.Rows(0)("IsBusy").ToString() = True, "Yes", "No") & " ; " &
                                    "Pulser Timing Adjust = " & dtHose.Rows(0)("SwitchTimeBounce").ToString().Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Shared Function BindSiteTimings(SiteId As Integer) As String
        Dim FuelingTimes As String = ""

        Try

            Dim OBJMaster = New MasterBAL()
            Dim dtTiming As DataTable = New DataTable()

            dtTiming = OBJMaster.GetSiteTimings(SiteId)
            If dtTiming IsNot Nothing Then
                For Each dr As DataRow In dtTiming.Rows

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        FuelingTimes = IIf(FuelingTimes = "", dr("TimeText"), FuelingTimes & ";" & dr("TimeText"))
                    End If

                Next
            End If

            Return FuelingTimes
        Catch ex As Exception
            log.Error("Error occurred in BindSiteTimings Exception is :" + ex.Message)
            Return FuelingTimes
        End Try

    End Function

    Shared Function BindSiteDays(SiteId As Integer) As String
        Dim FuelingDay As String = ""
        Try


            Dim OBJMaster = New MasterBAL()
            Dim dtSiteDays As DataTable = New DataTable()
            dtSiteDays = OBJMaster.GetSiteDays(SiteId)


            If dtSiteDays IsNot Nothing Then
                For Each dr As DataRow In dtSiteDays.Rows

                    Dim dayname As String = ""

                    Select Case dr("Day")
                        Case "0"
                            dayname = "All"
                        Case "1"
                            dayname = "Sunday"
                        Case "2",
                            dayname = "Monday"
                        Case "3"
                            dayname = "Tuesday"
                        Case "4"
                            dayname = "Wednesday"
                        Case "5"
                            dayname = "Thursday"
                        Case "6"
                            dayname = "Friday"
                        Case "7"
                            dayname = "Sunday"

                    End Select

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        FuelingDay = IIf(FuelingDay = "", dayname, FuelingDay & ";" & dayname)
                    End If

                Next

            End If

            Return FuelingDay
        Catch ex As Exception

            log.Error("Error occurred in BindSiteDays Exception is :" + ex.Message)
            Return FuelingDay
        End Try

    End Function

End Class