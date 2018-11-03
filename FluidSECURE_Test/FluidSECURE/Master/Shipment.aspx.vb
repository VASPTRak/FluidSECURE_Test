Imports log4net
Imports log4net.Config

Public Class Shipment
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(Shipment))

    Dim OBJMaster As MasterBAL = New MasterBAL()
    Shared beforeData As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            ErrorMessage.Visible = False
            message.Visible = False

            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") <> "SuperAdmin" And Session("RoleName") <> "Support" Then 'And Session("RoleName") <> "CustomerAdmin"
                'Access denied
                Response.Redirect("/home")

            Else
                If Not IsPostBack Then
                    txtShipmentDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                    txtReturnedDate.Text = DateTime.Now.ToString("MM/dd/yyyy")

                    BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

                    If (Not Request.QueryString("ShipmentId") = Nothing And Not Request.QueryString("ShipmentId") = "") Then

                        HDF_ShipmentId.Value = Request.QueryString("ShipmentId")

                        BindShipmentDetails(Request.QueryString("ShipmentId"))
                        btnFirst.Visible = True
                        btnNext.Visible = True
                        btnprevious.Visible = True
                        btnLast.Visible = True
                        lblHeader.Text = "Edit Shipment"
                        If (Request.QueryString("RecordIs") = "New") Then
                            message.Visible = True
                            message.InnerText = "Record saved"
                        End If

                    Else
                        btnFirst.Visible = False
                        btnNext.Visible = False
                        btnprevious.Visible = False
                        btnLast.Visible = False
                        lblof.Visible = False
                        lblHeader.Text = "Add Shipment"
                        RBL_Options.SelectedValue = 1
                        RBL_Options_SelectedIndexChanged(Nothing, Nothing)
                        DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                        chkIsReplacement_CheckedChanged(Nothing, Nothing)
                        CHK_Returned_CheckedChanged(Nothing, Nothing)

                    End If
                    txtFluidSecureUnitName.Focus()

                End If
            End If


            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)

        Catch ex As Exception
            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Private Sub BindShipmentDetails(ShipmentId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtShipment As DataTable = New DataTable()
            dtShipment = OBJMaster.GetShipmentByShipmentID(ShipmentId)
            Dim cnt As Integer = 0
            If (dtShipment.Rows.Count > 0) Then


                txtFluidSecureUnitName.Text = dtShipment.Rows(0)("FluidSecureUnitName").ToString()
                DDL_Customer.SelectedValue = dtShipment.Rows(0)("CompanyId").ToString().TrimEnd().TrimStart()
                DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                txtAddress.Text = dtShipment.Rows(0)("Address").ToString()
                txtPhoneNumber.Text = dtShipment.Rows(0)("PhoneNumber").ToString()
                txtShipmentDate.Text = Convert.ToDateTime(dtShipment.Rows(0)("ShipmentDate").ToString()).ToString("MM/dd/yyyy")
                'txtShipmentTime.Text = Convert.ToDateTime(dtShipment.Rows(0)("ShipmentDate").ToString()).ToString("hh:mm tt")
                txtHubName.Text = dtShipment.Rows(0)("HubName").ToString()
                chkIsReplacement.Checked = dtShipment.Rows(0)("IsReplacement").ToString()
                DDL_Sites.SelectedValue = dtShipment.Rows(0)("ReplacementForSiteId").ToString()
                DDL_Hub.SelectedValue = dtShipment.Rows(0)("ReplacementForHubId").ToString()
                CHK_Returned.Checked = dtShipment.Rows(0)("IsReturned").ToString()
                txtSerialNumber.Text = dtShipment.Rows(0)("SerialNumber").ToString()
                If dtShipment.Rows(0)("Frequency").ToString() <> "" Then
                    DDL_Frequency.SelectedValue = dtShipment.Rows(0)("Frequency").ToString()
                End If
                If dtShipment.Rows(0)("HubForCardReader").ToString() <> "" Then
                    DDL_HubForCardReader.SelectedValue = dtShipment.Rows(0)("HubForCardReader").ToString()
                End If
                If (dtShipment.Rows(0)("ReturnedOn").ToString() = "") Then
                    txtReturnedDate.Text = ""
                Else
                    txtReturnedDate.Text = Convert.ToDateTime(dtShipment.Rows(0)("ReturnedOn").ToString()).ToString("MM/dd/yyyy")
                End If

                RBL_Options.SelectedValue = dtShipment.Rows(0)("ShipmentForLinkOrHub").ToString()
                RBL_Options_SelectedIndexChanged(Nothing, Nothing)
                chkIsReplacement_CheckedChanged(Nothing, Nothing)
                CHK_Returned_CheckedChanged(Nothing, Nothing)

                OBJMaster = New MasterBAL()

                Dim CompanyId As Integer = 0
                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    CompanyId = Convert.ToInt32(Session("CustomerId").ToString())
                End If
                HDF_TotalShipments.Value = OBJMaster.GetShipmentIdByCondition(ShipmentId, False, False, False, False, True, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), CompanyId)

                OBJMaster = New MasterBAL()
                Dim dtAllShipments As DataTable = New DataTable()
                'If Session("CompanyNameHeader") <> Nothing Then
                '	If Session("CompanyNameHeader") <> "" And Session("CompanyNameHeader") <> "Select All" Then
                '		dtAllShipments = OBJMaster.GetShipmentsByCondition(" and SD.Company = '" + Session("CompanyNameHeader").ToString() + "' ", Session("RoleId").ToString())
                '	Else
                '		dtAllShipments = OBJMaster.GetShipmentsByCondition("", Session("RoleId").ToString())
                '	End If
                'Else
                '	dtAllShipments = OBJMaster.GetShipmentsByCondition("", Session("RoleId").ToString())
                'End If

                If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                    dtAllShipments = OBJMaster.GetShipmentsByCondition(" and SD.CompanyId = " + Session("CustomerId").ToString() + " ", Session("RoleId").ToString())
                Else
                    dtAllShipments = OBJMaster.GetShipmentsByCondition("", Session("RoleId").ToString())
                End If

                dtAllShipments.PrimaryKey = New DataColumn() {dtAllShipments.Columns(0)}
                Dim dr As DataRow = dtAllShipments.Rows.Find(ShipmentId)
                If Not IsDBNull(dr) Then

                    cnt = dtAllShipments.Rows.IndexOf(dr) + 1

                End If
                If (cnt >= HDF_TotalShipments.Value) Then
                    btnNext.Enabled = False
                    btnLast.Enabled = False
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                ElseIf (cnt <= 1) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = False
                    btnprevious.Enabled = False
                ElseIf (cnt > 1 And cnt < HDF_TotalShipments.Value) Then
                    btnNext.Enabled = True
                    btnLast.Enabled = True
                    btnFirst.Enabled = True
                    btnprevious.Enabled = True
                End If
                lblof.Text = cnt & " of " & HDF_TotalShipments.Value.ToString()

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    beforeData = CreateData(ShipmentId)
                End If

            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Data Not found. Please try again after some time."
            End If

        Catch ex As Exception

            log.Error("Error occurred in BindShipmentDetails Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting Shipment data, please try again later."
        Finally
            txtFluidSecureUnitName.Focus()
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ValidateAndSaveShipment(False)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("~/Master/AllShipments")
    End Sub

    Protected Sub First_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
        Try
            Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value

            Dim CompanyId As Integer = 0
            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                CompanyId = Convert.ToInt32(Session("CustomerId").ToString())
            End If

            OBJMaster = New MasterBAL()
            Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, True, False, False, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), CompanyId)
            HDF_ShipmentId.Value = ShipmentId
            BindShipmentDetails(ShipmentId)

        Catch ex As Exception
            log.Error("Error occurred in First_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnprevious_Click(sender As Object, e As EventArgs) Handles btnprevious.Click
        Try

            Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value
            Dim CompanyId As Integer = 0
            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                CompanyId = Convert.ToInt32(Session("CustomerId").ToString())
            End If
            OBJMaster = New MasterBAL()
            Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, False, False, False, True, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), CompanyId)
            HDF_ShipmentId.Value = ShipmentId
            BindShipmentDetails(ShipmentId)

        Catch ex As Exception
            log.Error("Error occurred in btnprevious_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Try

            Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value
            Dim CompanyId As Integer = 0
            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                CompanyId = Convert.ToInt32(Session("CustomerId").ToString())
            End If
            OBJMaster = New MasterBAL()
            Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, False, False, True, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), CompanyId)
            HDF_ShipmentId.Value = ShipmentId
            BindShipmentDetails(ShipmentId)

        Catch ex As Exception
            log.Error("Error occurred in btnNext_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
        Try

            Dim CurrentShipmentId As Integer = HDF_ShipmentId.Value
            Dim CompanyId As Integer = 0
            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                CompanyId = Convert.ToInt32(Session("CustomerId").ToString())
            End If
            OBJMaster = New MasterBAL()
            Dim ShipmentId As Integer = OBJMaster.GetShipmentIdByCondition(CurrentShipmentId, False, True, False, False, False, Session("RoleId").ToString(), Convert.ToInt32(Session("PersonId").ToString()), CompanyId)
            HDF_ShipmentId.Value = ShipmentId
            BindShipmentDetails(ShipmentId)

        Catch ex As Exception
            log.Error("Error occurred in btnLast_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub BindCustomer(PersonId As Integer, RoleId As String)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()
            Dim CustomerId As Integer = 0
            If (Session("CustomerId") <> 0 And Not Session("CustomerId") Is Nothing) Then
                CustomerId = Convert.ToInt32(Session("CustomerId"))
                'DDL_Customer.Visible = False
                'divCompany.Visible = False
            End If

            dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, CustomerId)

            DDL_Customer.DataSource = dtCust
            DDL_Customer.DataTextField = "CustomerName"
            DDL_Customer.DataValueField = "CustomerId"
            DDL_Customer.DataBind()

            DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Function CreateData(ShipmentId As Integer) As String
        Try

            Dim data As String = ""

            data = "ShipmentId = " & ShipmentId & " ; " &
                    "FluidSecure Link Name = " & txtFluidSecureUnitName.Text.Replace(",", " ") & " ; " &
                    "Hub Name =" & txtHubName.Text.Replace(",", " ") & " ; " &
                    "Serial Number =" & txtSerialNumber.Text.Replace(",", " ") & " ; " &
                    "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; " &
                    "Address  = " & txtAddress.Text.Replace(",", " ") & " ; " &
                    "PhoneNumber  = " & txtPhoneNumber.Text.Replace(",", " ") & " ; " &
                    "Shipment Date  = " & txtShipmentDate.Text.Replace(",", " ") & " ; " &
                    "ReplacementForSiteId =" & DDL_Sites.SelectedItem.ToString().Replace(",", " ") & " ; " &
                     "HubForCardReader =" & DDL_HubForCardReader.SelectedItem.ToString().Replace(",", " ") & " ; " &
                    "ReplacementForHubId =" & DDL_Hub.SelectedItem.ToString().Replace(",", " ") & " ; " & ";"

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub btnSaveAndAddNew_Click(sender As Object, e As EventArgs)
        ValidateAndSaveShipment(True)
    End Sub


    Private Sub ValidateAndSaveShipment(IsSaveAndAddNew As Boolean)

        Dim ShipmentId As Integer = 0

        Try
            If CSCommonHelper.CheckSessionExpired() = False Then
                'unautorized access error log

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
                Return
            End If

            If (Not HDF_ShipmentId.Value = Nothing And Not HDF_ShipmentId.Value = "") Then

                ShipmentId = HDF_ShipmentId.Value

            End If
            If (chkIsReplacement.Checked = False) Then
                DDL_Hub.SelectedValue = 0
                DDL_Sites.SelectedValue = 0
            Else
                If (RBL_Options.SelectedValue = "1" And DDL_Sites.SelectedValue = 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please select site."
                    DDL_Sites.Focus()
                    Return
                ElseIf (RBL_Options.SelectedValue = "2" And DDL_Hub.SelectedValue = 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please select hub."
                    DDL_Hub.Focus()
                    Return
                ElseIf (RBL_Options.SelectedValue = "3" And DDL_HubForCardReader.SelectedValue = 0) Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please select hub."
                    DDL_HubForCardReader.Focus()
                    Return
                End If
            End If

            If (CHK_Returned.Checked = True And txtReturnedDate.Text = "") Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please select Returned Date."
                txtReturnedDate.Focus()
                Return
            End If

            Dim CheckIdExists As Integer = 0
            Dim ShipmentDateTime As DateTime
            If (RBL_Options.SelectedValue = "1") Then
                If (txtFluidSecureUnitName.Text = "") Then

                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter FluidSecure Link name"

                End If

                Try
                    ShipmentDateTime = Request.Form(txtShipmentDate.UniqueID)
                Catch ex As Exception
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please select valid Shipment Date."
                    Return
                End Try
                OBJMaster = New MasterBAL()
                CheckIdExists = OBJMaster.CheckFluidSecureUnitExist(txtFluidSecureUnitName.Text, ShipmentId)

                If CheckIdExists = -1 Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "FluidSecure Link already exist."
                    Return
                End If
            ElseIf (RBL_Options.SelectedValue = "2") Then
                If (txtHubName.Text = "") Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter Hub name."
                End If
                Try
                    ShipmentDateTime = Request.Form(txtShipmentDate.UniqueID)
                Catch ex As Exception
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please select valid Shipment Date."
                    Return
                End Try

                Dim expression As String = "^[- +()]*[0-9][- +()0-9]*$"
                Dim match = Regex.Match(txtPhoneNumber.Text, expression, RegexOptions.IgnoreCase)

                If txtPhoneNumber.Text <> "" Then
                    If Not match.Success Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "Please enter valid Phone Number."
                        Return
                    End If
                End If


            ElseIf (RBL_Options.SelectedValue = "3") Then
                If (txtSerialNumber.Text = "") Then

                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please enter Serial Number."

                End If
                Try
                    ShipmentDateTime = Request.Form(txtShipmentDate.UniqueID)
                Catch ex As Exception
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please select valid Packing Date."
                    Return
                End Try
                If (DDL_Frequency.SelectedValue.ToString() <> "High" And DDL_Frequency.SelectedValue.ToString() <> "Low") Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Please select valid Frequency."
                    Return
                End If
                OBJMaster = New MasterBAL()
                CheckIdExists = OBJMaster.CheckShipmentCardReaderSerialNumberExist(txtSerialNumber.Text, ShipmentId)

                If CheckIdExists = -1 Then

                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Card Reader Serial Number  already exist."
                    Return
                End If

            End If

            If (RBL_Options.SelectedValue = "1") Then
                txtHubName.Text = ""
                DDL_Hub.SelectedValue = 0
                txtSerialNumber.Text = ""
                DDL_HubForCardReader.SelectedValue = 0
                txtPhoneNumber.Text = ""

            ElseIf (RBL_Options.SelectedValue = "2") Then
                txtFluidSecureUnitName.Text = ""
                DDL_Sites.SelectedValue = 0
                txtSerialNumber.Text = ""
                DDL_HubForCardReader.SelectedValue = 0
            ElseIf (RBL_Options.SelectedValue = "3") Then
                txtFluidSecureUnitName.Text = ""
                DDL_Sites.SelectedValue = 0
                txtHubName.Text = ""
                DDL_Hub.SelectedValue = 0
                txtPhoneNumber.Text = ""
            End If


            OBJMaster = New MasterBAL()

            Dim result As Integer = 0

            OBJMaster = New MasterBAL()

            ShipmentDateTime = Request.Form(txtShipmentDate.UniqueID) '& " " & Request.Form(txtShipmentTime.UniqueID)
            Dim ReturnedOn As DateTime = IIf(txtReturnedDate.Text = "", DateTime.MinValue, Request.Form(txtReturnedDate.UniqueID)) '& " " & Request.Form(txtShipmentTime.UniqueID)

            result = OBJMaster.SaveUpdateShipment(ShipmentId, txtFluidSecureUnitName.Text, DDL_Customer.SelectedItem.Text.ToString().TrimStart().TrimEnd(),
                                                  txtAddress.Text, Convert.ToInt32(Session("PersonId")), txtPhoneNumber.Text.ToString(), shipmentDatetime, DDL_Customer.SelectedValue,
                                                  txtHubName.Text, chkIsReplacement.Checked, DDL_Sites.SelectedValue, DDL_Hub.SelectedValue, CHK_Returned.Checked,
                                                  ReturnedOn, RBL_Options.SelectedValue, txtSerialNumber.Text, DDL_Frequency.SelectedValue, DDL_HubForCardReader.SelectedValue)


            If result > 0 Then
                If (ShipmentId > 0) Then
                    message.Visible = True
                    message.InnerText = "Record saved"
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(ShipmentId)
                        CSCommonHelper.WriteLog("Modified", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    If (IsSaveAndAddNew = True) Then
                        Response.Redirect(String.Format("~/Master/Shipment"))
                    End If
                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result)
                        CSCommonHelper.WriteLog("Added", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                    If (IsSaveAndAddNew = True) Then
                        Response.Redirect(String.Format("~/Master/Shipment"))
                    Else
                        Response.Redirect(String.Format("~/Master/Shipment?ShipmentId={0}&RecordIs=New", result))
                    End If
                End If

            Else
                If (ShipmentId > 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(ShipmentId)
                        CSCommonHelper.WriteLog("Modified", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment update failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Shipment update failed, please try again"
                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(result)
                        CSCommonHelper.WriteLog("Added", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment Addition failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "Shipment Addition failed, please try again"
                End If

            End If
            txtFluidSecureUnitName.Focus()

        Catch ex As Exception

            If (ShipmentId > 0) Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(ShipmentId)
                    CSCommonHelper.WriteLog("Modified", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment update failed. Exception is : " & ex.Message)
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData(ShipmentId)
                    CSCommonHelper.WriteLog("Added", "Shipments", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Shipment Addition failed. Exception is : " & ex.Message)
                End If
            End If

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving record, please try again later."
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
            txtFluidSecureUnitName.Focus()

        Finally

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub RBL_Options_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            If (RBL_Options.SelectedValue = "1") Then
                ShipmentForLinkName.Visible = True
                ShipmentForHubName.Visible = False
                PhoneNumberForHub.Visible = False
                ShipmentForCardReader.Visible = False
                FrequencyForCardReader.Visible = False
                HubForCardReader.Visible = False
                ShipmentForReplacement.Visible = True
                ShipmentDate.Text = "Shipment Date"


                If chkIsReplacement.Checked Then
                    ReplacementForLink.Visible = True
                    ReplacementForHub.Visible = False
                Else
                    ReplacementForLink.Visible = False
                    ReplacementForHub.Visible = False
                End If

            ElseIf (RBL_Options.SelectedValue = "2") Then
                ShipmentForLinkName.Visible = False
                ShipmentForHubName.Visible = True
                PhoneNumberForHub.Visible = True
                ShipmentForCardReader.Visible = False
                FrequencyForCardReader.Visible = False
                HubForCardReader.Visible = False
                ShipmentForReplacement.Visible = True
                ShipmentDate.Text = "Shipment Date"

                If chkIsReplacement.Checked Then
                    ReplacementForLink.Visible = False
                    ReplacementForHub.Visible = True
                Else
                    ReplacementForLink.Visible = False
                    ReplacementForHub.Visible = False
                End If

            ElseIf (RBL_Options.SelectedValue = "3") Then
                ShipmentForLinkName.Visible = False
                ShipmentForHubName.Visible = False
                PhoneNumberForHub.Visible = False
                ShipmentForCardReader.Visible = True
                FrequencyForCardReader.Visible = True
                HubForCardReader.Visible = True
                ShipmentForReplacement.Visible = False
                ShipmentDate.Text = "Packing Date"
                ReplacementForLink.Visible = False
                ReplacementForHub.Visible = False

            End If
        Catch ex As Exception
            log.Error("Error occurred in RBL_Options_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub
    Private Sub BindAllHubs()
        Try
            Dim dtPersonnel As DataTable = New DataTable()
            OBJMaster = New MasterBAL()

            dtPersonnel = OBJMaster.GetPersonnelByNameAndNumberAndEmail(" and ISNULL(ANU.IsFluidSecureHub,0)=1  and ANU.CustomerId = " & DDL_Customer.SelectedValue, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())

            DDL_Hub.DataSource = dtPersonnel
            DDL_Hub.DataValueField = "PersonId"
            DDL_Hub.DataTextField = "HubSiteName"
            DDL_Hub.DataBind()

            DDL_Hub.Items.Insert(0, New ListItem("Select Site", "0"))

            DDL_HubForCardReader.DataSource = dtPersonnel
            DDL_HubForCardReader.DataValueField = "PersonId"
            DDL_HubForCardReader.DataTextField = "HubSiteName"
            DDL_HubForCardReader.DataBind()

            DDL_HubForCardReader.Items.Insert(0, New ListItem("Select Site", "0"))

        Catch ex As Exception

            log.Error("Error occurred in BindAllHubs Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting All Hubs, please try again later."

        End Try

    End Sub

    Private Sub BindSites()
        Try

            Dim dtSites As DataTable = New DataTable()
            OBJMaster = New MasterBAL()

            dtSites = OBJMaster.GetSiteByCondition(" And c.CustomerId =" + DDL_Customer.SelectedValue, Session("PersonId").ToString(), Session("RoleId").ToString(), False)

            DDL_Sites.DataSource = dtSites
            DDL_Sites.DataValueField = "SiteId"
            DDL_Sites.DataTextField = "WifiSSid"
            DDL_Sites.DataBind()

            DDL_Sites.Items.Insert(0, New ListItem("Select FluidSecure Link", "0"))
        Catch ex As Exception

            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred  while getting sites, please try again later."

            log.Error("Error occurred in BindSites Exception is :" + ex.Message)

        End Try
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try

            BindAllHubs()
            BindSites()

        Catch ex As Exception

            log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."

        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub chkIsReplacement_CheckedChanged(sender As Object, e As EventArgs)
        Try
            If (chkIsReplacement.Checked = True) Then
                If (RBL_Options.SelectedValue = "1") Then
                    ReplacementForLink.Visible = True
                    ReplacementForHub.Visible = False
                ElseIf (RBL_Options.SelectedValue = "2") Then
                    ReplacementForLink.Visible = False
                    ReplacementForHub.Visible = True
                End If
            Else
                ReplacementForLink.Visible = False
                ReplacementForHub.Visible = False
            End If

        Catch ex As Exception
            log.Error("Error occurred in chkIsReplacement_CheckedChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while Checking data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub

    Protected Sub CHK_Returned_CheckedChanged(sender As Object, e As EventArgs)
        Try

            If (CHK_Returned.Checked = True) Then
                ReturnedDate.Visible = True
            Else
                ReturnedDate.Visible = False
            End If

        Catch ex As Exception
            log.Error("Error occurred in CHK_Returned_CheckedChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while Checking data, please try again later."
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try
    End Sub
    Protected Sub DDL_Frequency_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try

        Catch ex As Exception

        End Try
    End Sub
    Protected Sub DDL_HubForCardReader_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try

        Catch ex As Exception

        End Try
    End Sub

End Class
