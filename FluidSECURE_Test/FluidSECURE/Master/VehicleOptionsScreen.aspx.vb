Imports log4net
Imports log4net.Config

Public Class VehicleOptionsScreen
    Inherits System.Web.UI.Page

    Dim OBJMaster As MasterBAL = New MasterBAL()
    Shared beforeData As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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

                lblHeader.Text = "Vehicle Options"
                BindCustomers(Session("PersonId").ToString(), Session("RoleId").ToString())

            End If
        End If
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

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "GroupAdmin") Then
                DDL_Customer.SelectedIndex = 1
                DDL_Customer.Enabled = False
                DDL_Customer.Visible = False
                divCompany.Visible = False
            End If
            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                If (Session("RoleName") = "GroupAdmin") Then
                    DDL_Customer.SelectedValue = Session("CustomerId")
                Else
                    DDL_Customer.SelectedIndex = 1
                End If
            End If

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting Companies , please try again later."

            ' log.Error("Error occurred in BindCustomers Exception is :" + ex.Message)

        End Try
    End Sub

    Protected Sub btn_Assign_Click(sender As Object, e As EventArgs) Handles btn_assign.Click
        Try

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenVehAssignModelBox();", True)
        Catch ex As Exception
            'log.Error("Error occurred in btn_DisableAllVehOdo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try


    End Sub
    Protected Sub btn_EnableAllVehOdo_Click(sender As Object, e As EventArgs)
        Try
            hdfEnabDisOdo.Value = "1"
            'lblVehodo.Text = "Are you sure you want to Enable odometer for all vehicles in this Company ?"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenVehOdoModelBox();", True)
        Catch ex As Exception
            ' log.Error("Error occurred in btn_EnableAllVehOdo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btn_DisableAllVehOdo_Click(sender As Object, e As EventArgs)
        Try
            hdfEnabDisOdo.Value = "2"
            'lblVehodo.Text = "Are you sure you want to Disable odometer for all vehicles in this Company ?"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenVehOdoModelBox();", True)
        Catch ex As Exception
            'log.Error("Error occurred in btn_DisableAllVehOdo_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btnVehOdoOk_Click(sender As Object, e As EventArgs)
        Try
            Dim flagEnableDisable As Boolean = True
            If hdfEnabDisOdo.Value = "2" Then
                flagEnableDisable = False
                hdfEnabDisOdo.Value = "0"
            ElseIf hdfEnabDisOdo.Value = "1" Then
                flagEnableDisable = True
                hdfEnabDisOdo.Value = "0"
            Else
                Return
            End If

            OBJMaster = New MasterBAL()
            Dim result As Integer = OBJMaster.SetEnableDisableVehOdoByCustID(flagEnableDisable, DDL_Customer.SelectedValue)

            If result = 1 Then
                message.Visible = True
                message.InnerText = "Record Saved."
            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Error occurred while updating data, please try again later."
            End If

        Catch ex As Exception
            'log.Error("Error occurred in btnVehOdoOk_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btnVehAssignOk_Click(sender As Object, e As EventArgs)
        Try
            OBJMaster.SavePersonnalVehicleMappingAgainstCustomer(DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId")))
            message.Visible = True
            message.InnerText = "Record Saved."
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try

    End Sub
    Protected Sub btn_EnableHours_Click(sender As Object, e As EventArgs)
        Try
            hdfOdoHours.Value = "1"

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoHoursBox();", True)
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btn_DisableHours_Click(sender As Object, e As EventArgs)
        Try
            hdfOdoHours.Value = "2"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoHoursBox();", True)
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    'Protected Sub CHK_CheckOdometerReasonable_Checked(sender As Object, e As EventArgs)

    '    If (CHK_CheckOdometerReasonable.Checked = True And txtHoursLimit.Text = "") Then

    '        ErrorMessage.Visible = True
    '        ErrorMessage.InnerText = "Please enter total hours allowed between fueling."
    '        CHK_CheckOdometerReasonable.Checked = False
    '        txtHoursLimit.Focus()
    '        Return

    '    End If
    '    If (CHK_CheckOdometerReasonable.Checked = True And txtOdoLimit.Text = "") Then

    '        ErrorMessage.Visible = True
    '        ErrorMessage.InnerText = "Please enter total miles allowed between fueling."
    '        CHK_CheckOdometerReasonable.Checked = False
    '        txtOdoLimit.Focus()
    '        Return

    '    End If

    '    Try
    '        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoOptBox();", True)
    '    Catch ex As Exception
    '        'log.Error("Error occurred in btn_DisableAllVehOdo_Click Exception is :" + ex.Message)
    '        ErrorMessage.Visible = True
    '        ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
    '    End Try
    'End Sub  IIf(txtOdoLimit.Text = "", "-1", txtOdoLimit.Text), IIf(txtHoursLimit.Text = "", "-1", txtHoursLimit.Text)
    Protected Sub btnSaveOdo_Click(sender As Object, e As EventArgs)
        Try
            Dim OdoReasonability As String
            If hdfOdoReasonability.Value = "2" Then
                OdoReasonability = "N"
                txtOdoLimit.Text = ""
                txtHoursLimit.Text = ""
                hdfOdoReasonability.Value = "0"
            ElseIf hdfOdoReasonability.Value = "1" Then
                OdoReasonability = "Y"
                hdfOdoReasonability.Value = "0"
            Else
                Return
            End If
            OBJMaster = New MasterBAL()
            Dim result As Integer = OBJMaster.SetEnableDisableReasonableOptionsForAllVehicles(DDL_Customer.SelectedValue, OdoReasonability, RBL_OdometerReasonabilityConditions.SelectedValue)
            If result = 1 Then
                message.Visible = True
                message.InnerText = "Record Saved."
            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Error occurred while updating data, please try again later."
            End If

        Catch ex As Exception
            '
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    'Protected Sub btnCancelOdo_Click(sender As Object, e As EventArgs)
    '    CHK_CheckOdometerReasonable.Checked = False
    'End Sub
    Protected Sub btn_EnableReasonability_Click(sender As Object, e As EventArgs)
        hdfOdoReasonability.Value = "1"
        Try
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoOptBox();", True)
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btn_DisableReasonability_Click(sender As Object, e As EventArgs)
        hdfOdoReasonability.Value = "2"
        Try
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoOptBox();", True)
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btnOdoHours_Click(sender As Object, e As EventArgs)
        Try
            Dim Hours As Boolean
            If hdfOdoHours.Value = "2" Then
                Hours = 0
                hdfOdoHours.Value = "0"
            ElseIf hdfOdoHours.Value = "1" Then
                Hours = 1
                hdfOdoHours.Value = "0"
            Else
                Return
            End If

            OBJMaster = New MasterBAL()
            Dim result As Integer = OBJMaster.SetEnableDisableHoursCustID(Hours, DDL_Customer.SelectedValue)
            If result = 1 Then
                message.Visible = True
                message.InnerText = "Record Saved."
            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Error occurred while updating data, please try again later."
            End If

        Catch ex As Exception
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btnOdoLimitAll_Click(sender As Object, e As EventArgs)
        hdfOdoHoursLimit.Value = "1"
        Try
            If (txtOdoLimit.Text = "") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total miles allowed between fueling."
                txtOdoLimit.Focus()
                Return
            End If
            Try
                Dim CheckIntValue As Integer = Convert.ToInt32(txtOdoLimit.Text)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total miles allowed between fueling in Integer."
                txtOdoLimit.Focus()
                Return
            End Try
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoHoursLimitBox();", True)
        Catch ex As Exception
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try

    End Sub
    Protected Sub btnOdoLimit_Click(sender As Object, e As EventArgs)
        hdfOdoHoursLimit.Value = "2"
        Try
            If (txtOdoLimit.Text = "") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total miles allowed between fueling."
                txtOdoLimit.Focus()
                Return
            End If
            Try
                Dim CheckIntValue As Integer = Convert.ToInt32(txtOdoLimit.Text)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total miles allowed between fueling in Integer."
                txtOdoLimit.Focus()
                Return
            End Try
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoHoursLimitBox();", True)
        Catch ex As Exception
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btnHoursLimitAll_Click(sender As Object, e As EventArgs)
        hdfOdoHoursLimit.Value = "3"
        Try
            If (txtHoursLimit.Text = "") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total hours allowed between fueling."
                txtHoursLimit.Focus()
                Return
            End If
            Try
                Dim CheckIntValue As Integer = Convert.ToInt32(txtHoursLimit.Text)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total hours allowed between fueling in Integer."
                txtHoursLimit.Focus()
                Return
            End Try
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoHoursLimitBox();", True)
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btnHoursLimit_Click(sender As Object, e As EventArgs)
        hdfOdoHoursLimit.Value = "4"
        Try
            If (txtHoursLimit.Text = "") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total hours allowed between fueling."
                txtHoursLimit.Focus()
                Return
            End If
            Try
                Dim CheckIntValue As Integer = Convert.ToInt32(txtHoursLimit.Text)
            Catch ex As Exception
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter total hours allowed between fueling in Integer."
                txtHoursLimit.Focus()
                Return
            End Try
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenOdoHoursLimitBox();", True)
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub
    Protected Sub btnOdoHoursLimitOk_Click(sender As Object, e As EventArgs)
        Try
            Dim FlagCondition As Integer = 0
            Dim MilesHours As Integer = 0

            If hdfOdoHoursLimit.Value = "1" Then
                FlagCondition = 1
                MilesHours = Convert.ToInt32(txtOdoLimit.Text)
            ElseIf hdfOdoHoursLimit.Value = "2" Then
                FlagCondition = 2
                MilesHours = Convert.ToInt32(txtOdoLimit.Text)
            ElseIf hdfOdoHoursLimit.Value = "3" Then
                FlagCondition = 3
                MilesHours = Convert.ToInt32(txtHoursLimit.Text)
            ElseIf hdfOdoHoursLimit.Value = "4" Then
                FlagCondition = 4
                MilesHours = Convert.ToInt32(txtHoursLimit.Text)
            End If


            OBJMaster = New MasterBAL()
            Dim result As Integer = OBJMaster.UpdateTotalMilesHoursVehicles(DDL_Customer.SelectedValue, FlagCondition, MilesHours)
            If result = 1 Then
                message.Visible = True
                message.InnerText = "Record Saved."
            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Error occurred while updating data, please try again later."
            End If

        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

End Class