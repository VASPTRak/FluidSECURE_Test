Imports log4net
Imports log4net.Config

Public Class AllFirmwareUpgrade
    Inherits System.Web.UI.Page


    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(AllFirmwareUpgrade))

    Dim OBJMaster As MasterBAL = New MasterBAL()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			XmlConfigurator.Configure()

			message.Visible = False
			ErrorMessage.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
			ElseIf Session("RoleName") <> "SuperAdmin" Then
				'Access denied
				Response.Redirect("/home")

			Else
				If (Not IsPostBack) Then
					BindColumns()
					If Session("RoleName") <> "SuperAdmin" Then
						btn_New.Visible = False
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

	Private Sub BindColumns()
		Try

			OBJMaster = New MasterBAL()
			Dim dtColumns As DataTable = New DataTable()
			dtColumns = OBJMaster.GetFirmwareColumnNameForSearch()

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
            If ((Not txt_value.Text = "") And DDL_Column.SelectedValue <> "0") Then
                strConditions = IIf(strConditions = "", " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text + "%'", strConditions + " and " + DDL_Column.SelectedValue + " like '%" + txt_value.Text + "%'")
            End If

            OBJMaster = New MasterBAL()
            Dim dtFirmwares As DataTable = New DataTable()

            dtFirmwares = OBJMaster.GetFirmwaresByCondition(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

            Session("dtFirmwares") = dtFirmwares
            lblTotalNumberOfRecords.Text = "Total Records: 0"
            If dtFirmwares IsNot Nothing Then
                If dtFirmwares.Rows.Count > 0 Then
                    lblTotalNumberOfRecords.Text = "Total Records: " + Convert.ToString(dtFirmwares.Rows.Count)
                End If
            End If
            gvUploadedFirmware.DataSource = dtFirmwares
            gvUploadedFirmware.DataBind()


            ViewState("Column_Name") = "FirmwareId"
            ViewState("Sort_Order") = "desc"

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

			Dim FirmwareId As Integer = gvUploadedFirmware.DataKeys(gvRow.RowIndex).Values("FirmwareId").ToString()

			Response.Redirect("FirmwareUpgrades.aspx?FirmwareId=" & FirmwareId, False)

		Catch ex As Exception
			log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    Private Sub RebindData(sColimnName As String, sSortOrder As String)
		Try

			Dim dt As DataTable = CType(Session("dtFirmwares"), DataTable)
        dt.DefaultView.Sort = sColimnName + " " + sSortOrder
        gvUploadedFirmware.DataSource = dt
        gvUploadedFirmware.DataBind()
        ViewState("Column_Name") = sColimnName
        ViewState("Sort_Order") = sSortOrder

		Catch ex As Exception
			log.Error("Error occurred in RebindData Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    Protected Sub gvUploadedFirmwareSorting(sender As Object, e As GridViewSortEventArgs) Handles gvUploadedFirmware.Sorting
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
			log.Error("Error occurred in gvUploadedFirmwareSorting Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub

	Protected Sub gvUploadedFirmware_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvUploadedFirmware.PageIndexChanging
		Try
			gvUploadedFirmware.PageIndex = e.NewPageIndex

			Dim dtCust As DataTable = Session("dtFirmwares")

			gvUploadedFirmware.DataSource = dtCust
			gvUploadedFirmware.DataBind()
		Catch ex As Exception
			log.Error("Error occurred in gvUploadedFirmware_PageIndexChanging Exception is :" + ex.Message)
		End Try

	End Sub

	<System.Web.Services.WebMethod(True)>
	Public Shared Function DeleteRecord(ByVal FirmwareId As String) As String
		Try
			Dim OBJMaster = New MasterBAL()
			Dim data As String = CreateData(FirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")), HttpContext.Current.Session("RoleId"))
			Dim result As Integer = OBJMaster.DeleteFirmware(FirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Firmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "Firmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Firmware upgrade record deletion failed.")
				End If
			End If

			Return result
		Catch ex As Exception
			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

	<System.Web.Services.WebMethod(True)> _
    Public Shared Function LaunchFirmware(ByVal FirmwareId As String) As String
		Try
			Dim OBJMaster = New MasterBAL()
			Dim data As String = CreateData(FirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")), HttpContext.Current.Session("RoleId"))
			Dim result As Integer = OBJMaster.LaunchFirmware(FirmwareId, Convert.ToInt32(HttpContext.Current.Session("PersonId")))
			If (result > 0) Then
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Launched", "Firmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Launched", "Firmware Upgrades", data, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Firmware Upgrade launched failed.")
				End If
			End If

			Return result
		Catch ex As Exception
			log.Error("Error occurred in LaunchFirmware Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

    Protected Sub btn_New_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/Master/FirmwareUpgrades")
    End Sub

    Protected Sub gvUploadedFirmware_RowDataBound(sender As Object, e As GridViewRowEventArgs)
		Try

			'Checking the RowType of the Row  
			If (e.Row.RowType = DataControlRowType.DataRow) Then

				Dim IsLaunched As String = gvUploadedFirmware.DataKeys(e.Row.RowIndex).Values("IsLaunched")
				Dim IsUpgradable As String = gvUploadedFirmware.DataKeys(e.Row.RowIndex).Values("IsUpgradable")

				Dim lnkAutoUpgradeEnableDisable As LinkButton = TryCast(e.Row.FindControl("lnkAutoUpgradeEnableDisable"), LinkButton)
				If (IsLaunched = "Yes") Then
					e.Row.Cells(1).Visible = False
					e.Row.Cells(2).Visible = True
					'gvUploadedFirmware.Rows(e.Row.RowIndex).Cells(0).Visible = False
				Else
					e.Row.Cells(1).Visible = True
					e.Row.Cells(2).Visible = False
				End If

				If (IsUpgradable = "Yes") Then
					If IsLaunched = "No" Then
						lnkAutoUpgradeEnableDisable.Enabled = False
						e.Row.Cells(3).Enabled = False
						e.Row.Cells(3).Style.Add("cursor", "not-allowed")
					Else
						If lnkAutoUpgradeEnableDisable IsNot Nothing Then
							lnkAutoUpgradeEnableDisable.Text = "ON"
						End If
					End If
				Else
					If lnkAutoUpgradeEnableDisable IsNot Nothing Then
						If IsLaunched = "No" Then
							lnkAutoUpgradeEnableDisable.Enabled = False
							e.Row.Cells(3).Enabled = False
							e.Row.Cells(3).Style.Add("cursor", "not-allowed")
						Else
							lnkAutoUpgradeEnableDisable.Enabled = True
						End If
					Else
						lnkAutoUpgradeEnableDisable.Enabled = False
						e.Row.Cells(3).Enabled = False
						e.Row.Cells(3).Style.Add("cursor", "not-allowed")
					End If
				End If

			ElseIf (e.Row.RowType = DataControlRowType.Header) Then
				e.Row.Cells(2).Visible = False
			End If

		Catch ex As Exception
			log.Error("Error occurred in gvUploadedFirmware_RowDataBound Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try

	End Sub
    
    Protected Sub lnkAutoUpgradeEnableDisable_Click(sender As Object, e As EventArgs)
		Try

			Dim rowIndex As Integer = TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex
			Dim id As Integer = Convert.ToInt32(gvUploadedFirmware.DataKeys(rowIndex).Values(0))
			OBJMaster = New MasterBAL()
			OBJMaster.checkLaunchedAndExistedVersionAndUpdate(id.ToString, "", 0, 3)
			btnSearch_Click(Nothing, Nothing)
		Catch ex As Exception
			log.Error("Error occurred in lnkAutoUpgradeEnableDisable_Click Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
		End Try
	End Sub

    Private Shared Function CreateData(FirmwareId As Integer, PersonId As Integer, RoleId As String) As String
        Try
            Dim dtFirmware As DataTable = New DataTable()

            Dim OBJMaster As MasterBAL = New MasterBAL()
            dtFirmware = OBJMaster.GetFirmwaresByCondition(" and FirmwareId = " & FirmwareId, PersonId, RoleId)

            Dim dtTankInventory As DataSet = New DataSet()
            dtTankInventory = OBJMaster.GetFirmwareById(FirmwareId)
            Dim mapping As String = ""

            If dtTankInventory IsNot Nothing Then
                If dtTankInventory.Tables(1) IsNot Nothing Then
                    If dtTankInventory.Tables(1).Rows.Count > 0 Then
                        For index = 0 To dtTankInventory.Tables(1).Rows.Count - 1
                            mapping = mapping & " Site Link Name= " & dtTankInventory.Tables(1).Rows(0)("WifiSSId") & " Company Name = " & dtTankInventory.Tables(1).Rows(0)("CustomerName") & ";"
                        Next
                    End If
                End If
            End If

            Dim data As String = "FirmwareId = " & FirmwareId & " ; " &
                                    "Firmware File Name = " & dtFirmware.Rows(0)("FirmwareFileName").ToString().Replace(",", " ") & " ; " &
                                    "Version = " & dtFirmware.Rows(0)("Version").ToString() & " ; " &
                                    "Firmware and FuelSecure Link Mapping  = " & mapping.TrimEnd(";") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class