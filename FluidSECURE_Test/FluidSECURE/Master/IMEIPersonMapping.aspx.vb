
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Owin
Imports Microsoft.AspNet.Identity.EntityFramework
Imports log4net
Imports log4net.Config
Imports System.Web.Services
Imports System.IO
Imports System.Net.Mail
Imports System.Net

Public Class IMEIPersonMapping
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(IMEIPersonMapping))

    Dim OBJMaster As MasterBAL
    Shared beforeData As String

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

					If (Not Request.QueryString("PersonId") = Nothing And Not Request.QueryString("PersonId") = "") Then
						HDF_PersonnelId.Value = Request.QueryString("PersonId")
						HDF_UniqueUserId.Value = Request.QueryString("UniqueUserId")
						BindIMIEPersonnelDetails(Request.QueryString("PersonId"), Request.QueryString("UniqueUserId"))

						If (Request.QueryString("RecordIs") = "New") Then
							message.Visible = True
							message.InnerText = "Mapping saved."
						ElseIf (Request.QueryString("RecordIs") = "Delete") Then
							message.Visible = True
							message.InnerText = "Mapping deleted."
						End If
					Else
						Response.Redirect("/home")
					End If

					txtIMEINumber.Focus()

				End If
			End If


		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
		End Try

	End Sub

    Private Sub BindIMIEPersonnelDetails(PersonId As Integer, UniqueUserId As String)
        Try

            OBJMaster = New MasterBAL()
            Dim dtPersonnel As DataTable = New DataTable()
            Dim cnt As Integer = 0

            dtPersonnel = OBJMaster.GetPersonnelByPersonIdAndId(PersonId, UniqueUserId)

            If (dtPersonnel.Rows.Count > 0) Then
                Dim isValid As Boolean = False
                If (Session("RoleName") = "GroupAdmin") Then
                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                    For Each drCusts As DataRow In dtCustOld.Rows
                        If (drCusts("CustomerId") = dtPersonnel.Rows(0)("CustomerId").ToString()) Then
                            isValid = True
                            Exit For
                        End If

                    Next
                End If


                If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                    If (dtCustOld.Rows(0)("CustomerId").ToString() <> dtPersonnel.Rows(0)("CustomerId").ToString()) Then

                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
                    End If

                End If


                OBJMaster = New MasterBAL()
                Dim dtIMEIPersonnelMapping As DataTable = New DataTable()

                dtIMEIPersonnelMapping = OBJMaster.GetIMEIPersonnelMappingByPersonId(PersonId)

                If (dtIMEIPersonnelMapping.Rows.Count > 0) Then

                    gvIMEIPersonnel.DataSource = dtIMEIPersonnelMapping
                    gvIMEIPersonnel.DataBind()
                Else
                    gvIMEIPersonnel.DataSource = Nothing
                    gvIMEIPersonnel.DataBind()
                End If

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    beforeData = CreateData(Convert.ToInt32(HDF_PersonnelId.Value))
                End If

            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Data Not found. Please try again after some time."
            End If





        Catch ex As Exception

            log.Error("Error occurred in BindIMIEPersonnelDetails Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting IMIE Personnel Mapping data, please try again later."
        Finally
            txtIMEINumber.Focus()
        End Try
    End Sub

    Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
        Try
            Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent, GridViewRow)
            Dim IMEIPersonMappingId As Integer = gvIMEIPersonnel.DataKeys(gvRow.RowIndex).Values("IMEIPersonMappingId").ToString()
            OBJMaster = New MasterBAL()
            Dim dtIMEIPersonnelMapping As DataTable = New DataTable()

            dtIMEIPersonnelMapping = OBJMaster.GetIMEIPersonnelMappingByIMEIPersonMappingId(IMEIPersonMappingId)

            If (dtIMEIPersonnelMapping.Rows.Count > 0) Then
                txtIMEINumber.Text = dtIMEIPersonnelMapping.Rows(0)("IMEI_UDID")
                CHK_IsActive.Checked = dtIMEIPersonnelMapping.Rows(0)("IsActive")
                HDF_IMEIPersonMappingId.Value = IMEIPersonMappingId
            Else
                txtIMEINumber.Text = ""
                CHK_IsActive.Checked = False
                HDF_IMEIPersonMappingId.Value = ""
            End If

        Catch ex As Exception

            log.Error("Error occurred in linkEdit_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting IMIE Personnel Mapping data, please try again later."
        Finally
            txtIMEINumber.Focus()
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			OBJMaster = New MasterBAL()

            If txtIMEINumber.Text = "" Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Please enter IMEI number and Try again."
                ErrorMessage.Focus()
                Return
            End If

            Dim IMEIPersonMappingId As Integer = 0
            If HDF_IMEIPersonMappingId.Value IsNot "" Then
                IMEIPersonMappingId = Convert.ToInt32(HDF_IMEIPersonMappingId.Value)
            End If

            Dim CheckIMEIExists As Boolean = False
            OBJMaster = New MasterBAL()
            If (txtIMEINumber.Text <> "") Then
                CheckIMEIExists = OBJMaster.CheckDuplicateIMEI_UDIDPersonMapping(txtIMEINumber.Text, Convert.ToInt32(HDF_PersonnelId.Value), IMEIPersonMappingId)

                If CheckIMEIExists = True Then
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "IMEI number already exist."
                    ErrorMessage.Focus()
                    Return
                End If
            End If

            Dim result As Integer = 0
            result = OBJMaster.IMEI_UDIDPersonMappingInsertUpdate(IMEIPersonMappingId, Convert.ToInt32(HDF_PersonnelId.Value), txtIMEINumber.Text, CHK_IsActive.Checked, Session("PersonId").ToString())

            If result > 0 Then
                If (IMEIPersonMappingId > 0) Then
                    message.Visible = True
                    message.InnerText = "Record saved"

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(HDF_PersonnelId.Value))
                        CSCommonHelper.WriteLog("Modified", "IMEI Person Mapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                Else

                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(HDF_PersonnelId.Value))
                        CSCommonHelper.WriteLog("Added", "IMEI Person Mapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If

                End If
                Response.Redirect("~/Master/IMEIPersonMapping.aspx?RecordIs=New&PersonId=" & HDF_PersonnelId.Value & "&UniqueUserId=" & HDF_UniqueUserId.Value, False)
            Else
                If (IMEIPersonMappingId > 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(HDF_PersonnelId.Value))
                        CSCommonHelper.WriteLog("Modified", "IMEI Person Mapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "IMEI Person Mapping update failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "IMEI Person Mapping update failed, please try again"
                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(HDF_PersonnelId.Value))
                        CSCommonHelper.WriteLog("Added", "IMEI Person Mapping", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "IMEI Person Mapping Addition failed.")
                    End If
                    ErrorMessage.Visible = True
                    ErrorMessage.InnerText = "IMEI Person Mapping failed, please try again"
                End If

            End If

        Catch ex As Exception
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while saving IMIE Personnel Mapping data, please try again later."
        Finally
            txtIMEINumber.Focus()
        End Try
    End Sub

	Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
		Response.Redirect("~/Master/Personnel.aspx?PersonId=" & HDF_PersonnelId.Value & "&UniqueUserId=" & HDF_UniqueUserId.Value, False)
	End Sub

	Protected Sub gvIMEIPersonnel_RowDataBound(sender As Object, e As GridViewRowEventArgs)

    End Sub

    Private Shared Function CreateData(PersonId As Integer) As String
        Try

            Dim data As String = ""
            Dim OBJMaster = New MasterBAL()
            Dim dtIMEIPersonnelMapping As DataTable = New DataTable()

            dtIMEIPersonnelMapping = OBJMaster.GetIMEIPersonnelMappingByPersonId(PersonId)

            If dtIMEIPersonnelMapping.Rows.Count > 0 Then

                For index = 0 To dtIMEIPersonnelMapping.Rows.Count - 1
                    data = data & "IMEIPersonMappingId = " & dtIMEIPersonnelMapping.Rows(index)("IMEIPersonMappingId").ToString().Replace(",", " ") & " ; " &
                    "PersonId = " & dtIMEIPersonnelMapping.Rows(index)("PersonId").ToString().Replace(",", " ") & " ; " &
                    "IMEI_UDID = " & dtIMEIPersonnelMapping.Rows(index)("IMEI_UDID").ToString().Replace(",", " ") & " ; " &
                    "Is Active = " & dtIMEIPersonnelMapping.Rows(index)("IsActive").ToString().Replace(",", " ") & " ; "
                Next

            End If

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    <System.Web.Services.WebMethod(True)>
    Public Shared Function DeleteRecord(ByVal IMEIPersonMappingId As String) As String
		Try


			Dim OBJMaster = New MasterBAL()
			Dim beforeData As String = ""
			Dim resultInt As Integer = 0

			Dim Context As HttpContext = HttpContext.Current


			beforeData = CreateData(Convert.ToInt32(HttpContext.Current.Request.QueryString("PersonId")))


			Dim result As Boolean = OBJMaster.DeleteIMEIPersonMapping(IMEIPersonMappingId, HttpContext.Current.Session("PersonId").ToString())
			If (result = True) Then
				resultInt = 1

				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "IMEI Person Mapping", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
				End If
			Else
				If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
					CSCommonHelper.WriteLog("Deleted", "IMEI Person Mapping", beforeData, "", HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "IMEI Person Mapping deletion failed.")
				End If
				resultInt = 0
			End If

			Return resultInt
		Catch ex As Exception

			log.Error("Error occurred in DeleteRecord Exception is :" + ex.Message)
			Return 0
		End Try
	End Function

    Protected Sub CHK_InActive_CheckedChanged(sender As Object, e As EventArgs)
        Try

            Dim cb As CheckBox = sender
            Dim gvRow As GridViewRow = CType(CType(sender, Control).Parent.Parent,
                                          GridViewRow)
            Dim index As Integer = gvRow.RowIndex

            Dim IMEIPersonMappingId As Integer = gvIMEIPersonnel.DataKeys(index).Values("IMEIPersonMappingId").ToString()

            ' Set IMEI Active - Inactive

            OBJMaster = New MasterBAL()
            Dim Result = OBJMaster.UpdateIMEIActiveFlagByPersonId(HDF_PersonnelId.Value, Session("PersonId"), cb.Checked, IMEIPersonMappingId)
            If Result = 1 Then
                message.Visible = True
                message.InnerText = "Record saved."
            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "IMEI Activation failed, please try again"
                cb.Checked = Not (cb.Checked)
            End If

        Catch ex As Exception
            ErrorMessage.InnerText = "Record saving failed."
            ErrorMessage.Visible = True
            log.Error("Error occurred in CHK_InActive_CheckedChanged Exception is :" + ex.Message)
        End Try
    End Sub
End Class
