Imports log4net
Imports log4net.Config

Public Class TankChartDetails
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(TankChartDetails))
    Dim OBJMaster As MasterBAL
    Shared beforeTankChartDetails As String
    Shared afterTankChartDetails As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            XmlConfigurator.Configure()

            If CSCommonHelper.CheckSessionExpired() = False Then

                'unautorized access error log
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
            ElseIf Session("RoleName") = "User" Or Session("RoleName") = "Reports Only" Or Session("RoleName") = "Support" Then
                'Access denied
                Response.Redirect("/home")
            Else
                If (Not Request.QueryString("TankChartId") = Nothing And Not Request.QueryString("TankChartId") = "") Then
                    If (Not IsPostBack) Then
                        lblHeader.Text = "Tank Chart Details"
                        HDF_TankChartId.Value = Request.QueryString("TankChartId")
                        BindTankChartDetails(Request.QueryString("TankChartId"))

                    End If
                End If
            End If

        Catch ex As Exception

            log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

        End Try
    End Sub

    Private Sub BindTankChartDetails(TankChartId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim dtTankChart As DataTable = New DataTable()
            dtTankChart = OBJMaster.GetTankChartByTankChartId(TankChartId)
            Dim cnt As Integer = 0
            If (dtTankChart.Rows.Count > 0) Then
                Dim isValid As Boolean = False
                If (Session("RoleName") = "GroupAdmin") Then
                    Dim dtCustOld As DataTable = New DataTable()

                    dtCustOld = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())
                    For Each drCusts As DataRow In dtCustOld.Rows
                        If (drCusts("CustomerId") = dtTankChart.Rows(0)("CompanyId").ToString()) Then
                            isValid = True
                            Exit For
                        End If

                    Next
                End If
                If (Not Session("RoleName") = "SuperAdmin" And Not isValid = True) Then

                    Dim dtCust As DataTable = New DataTable()

                    dtCust = OBJMaster.GetCustomerDetailsByPersonID(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString(), Session("CustomerId").ToString())

                    If (dtCust.Rows(0)("CustomerId").ToString() <> dtTankChart.Rows(0)("CompanyId").ToString()) Then

                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "NotValidUser();", True)

                        Return
                    End If

                End If

                OBJMaster = New MasterBAL()
                Dim dtTankChartDetail As DataTable = New DataTable()
                dtTankChartDetail = OBJMaster.GetTankChartDetailsByTankChartId(TankChartId)

                Session("dtTankChartDetail") = dtTankChartDetail
                BindGrid()
            Else
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Data Not found. Please try again after some time."
            End If


        Catch ex As Exception
            log.Error("Error occurred in BindTankChartDetails Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting Tank Chart Details, please try again later."
        End Try
    End Sub

    Protected Sub gv_TankChartDetails_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)

        BindGrid()

    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("/Master/TankChart.aspx?TankChartId=" & HDF_TankChartId.Value, False)
    End Sub

    Protected Sub OnRowEditing(sender As Object, e As GridViewEditEventArgs)
        Try

            gv_TankChartDetails.EditIndex = e.NewEditIndex
            BindGrid()

        Catch ex As Exception
            log.Error("Error occurred in OnRowEditing Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred OnRowEditing, please try again later."
        End Try
    End Sub

    'Protected Sub OnRowUpdating(sender As Object, e As GridViewUpdateEventArgs)
    '	Dim TankChartDetailId As Integer = 0
    '	Dim GallonLevel As String = ""
    '	Dim LncLevel As String = ""
    '	Try
    '		If CSCommonHelper.CheckSessionExpired() = False Then
    '			'unautorized access error log

    '			ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
    '			Return
    '		End If

    '		Dim row As GridViewRow = gv_TankChartDetails.Rows(e.RowIndex)
    '		TankChartDetailId = Convert.ToInt32(gv_TankChartDetails.DataKeys(e.RowIndex).Values("TankChartDetailId"))
    '		Dim TankChartId As Integer = Convert.ToInt32(gv_TankChartDetails.DataKeys(e.RowIndex).Values("TankChartId"))
    '		GallonLevel = DirectCast(row.Cells(0).FindControl("txtGallonLevel"), TextBox).Text
    '		LncLevel = DirectCast(row.Cells(0).FindControl("LBL_IncLevel"), Label).Text

    '           If GallonLevel IsNot Nothing Then

    '               If GallonLevel = "" Then
    '                   ErrorMessage.Visible = True
    '                   ErrorMessage.InnerText = "Please enter Gallon Level and try again."
    '                   ErrorMessage.Focus()
    '                   Return
    '               End If

    '               Dim resultDecimal As Decimal = 0

    '               If (GallonLevel <> "" And Not (Decimal.TryParse(GallonLevel, resultDecimal))) Then
    '                   ErrorMessage.Visible = True
    '                   ErrorMessage.InnerText = "Please enter Gallon Level as decimal and try again."
    '                   ErrorMessage.Focus()
    '                   Return
    '               End If
    '           End If

    '           OBJMaster = New MasterBAL()
    '		OBJMaster.UpdateTankChartDetail(TankChartDetailId, GallonLevel, Session("PersonId"))

    '		gv_TankChartDetails.EditIndex = -1
    '		BindTankChartDetails(TankChartId)

    '		If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '			Dim writtenData As String = CreateData(TankChartDetailId, LncLevel, GallonLevel)
    '			CSCommonHelper.WriteLog("Modified", "Tank Charts Details", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
    '		End If

    '	Catch ex As Exception
    '		log.Error("Error occurred in OnRowUpdating Exception is :" + ex.Message)
    '		ErrorMessage.Visible = True
    '		ErrorMessage.InnerText = "Error occurred OnRowUpdating, please try again later."

    '		If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
    '			Dim writtenData As String = CreateData(TankChartDetailId, LncLevel, GallonLevel)
    '			CSCommonHelper.WriteLog("Modified", "Tank Charts Details", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Tank chart detail update failed. Exception is : " & ex.Message)
    '		End If

    '	End Try
    'End Sub

    Protected Sub OnRowCancelingEdit(sender As Object, e As EventArgs)
        Try
            gv_TankChartDetails.EditIndex = -1
            BindGrid()
        Catch ex As Exception
            log.Error("Error occurred in OnRowCancelingEdit Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Private Sub BindGrid()
        Try
            'beforeTankChartDetails = ""
            Dim dtTankChartDetail As DataTable = Session("dtTankChartDetail")
            gv_TankChartDetails.DataSource = dtTankChartDetail
            gv_TankChartDetails.DataBind()

        Catch ex As Exception
            log.Error("Error occurred in BindGrid Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred on binding grid, please try again later."
        End Try
    End Sub

    Protected Sub gv_TankChartDetails_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim row As GridViewRow = e.Row
                Dim GallonLevel As TextBox = DirectCast(row.Cells(0).FindControl("txtGallonLevel"), TextBox)
                GallonLevel.Focus()

                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then

                    Dim TankChartDetailId As Integer = Convert.ToInt32(gv_TankChartDetails.DataKeys(row.RowIndex).Values("TankChartDetailId"))

                    Dim LBL_IncLevel As Label = DirectCast(row.Cells(0).FindControl("LBL_IncLevel"), Label)

                    beforeTankChartDetails = IIf(beforeTankChartDetails = "", CreateData(TankChartDetailId, LBL_IncLevel.Text, GallonLevel.Text), beforeTankChartDetails & ";" & CreateData(TankChartDetailId, LBL_IncLevel.Text, GallonLevel.Text))

                End If

            End If

        Catch ex As Exception
            log.Error("Error occurred in gv_TankChartDetails_RowDataBound Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred on RowDataBound, please try again later."
        End Try
    End Sub

    Private Function CreateData(TankChartDetailId As Integer, IncrementLevel As String, GallonLevel As String) As String
        Try

            Dim data As String = ""
            data = "TankChartDetailId = " & TankChartDetailId & " ; " &
                    "IncrementLevel = " & IncrementLevel.Replace(",", " ") & " ; " &
                    "GallonLevel = " & GallonLevel.Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            'Validate
            For i = 0 To gv_TankChartDetails.Rows.Count - 1
                Dim txtParentGallonLevel As TextBox = DirectCast(gv_TankChartDetails.Rows(i).FindControl("txtGallonLevel"), TextBox)
                Dim lblErrorMessageForGallon As Label = CType(gv_TankChartDetails.Rows(i).FindControl("lblErrorMessageForGallon"), Label)
                Dim MainGallonLevel As Decimal = 0
                If txtParentGallonLevel.Text = "" Then

                    lblErrorMessageForGallon.Text = "Please enter Gallon Level."
                    lblErrorMessageForGallon.Visible = True
                    txtParentGallonLevel.Focus()
                    Return
                Else
                    Try
                        MainGallonLevel = Convert.ToDecimal(txtParentGallonLevel.Text.ToString())
                    Catch ex As Exception
                        lblErrorMessageForGallon.Text = "Please enter Gallon Level in decimal format."
                        lblErrorMessageForGallon.Visible = True
                        txtParentGallonLevel.Focus()
                        Return
                    End Try
                End If

                For j = (i + 1) To gv_TankChartDetails.Rows.Count - 1
                    Dim txtChildGallonLevel As TextBox = DirectCast(gv_TankChartDetails.Rows(j).FindControl("txtGallonLevel"), TextBox)
                    Dim lblChildErrorMessageForGallon As Label = CType(gv_TankChartDetails.Rows(j).FindControl("lblErrorMessageForGallon"), Label)
                    If txtChildGallonLevel.Text = "" Then
                        lblChildErrorMessageForGallon.Text = "Please enter Gallon Level."
                        lblChildErrorMessageForGallon.Visible = True
                        txtChildGallonLevel.Focus()
                        Return
                    Else
                        Try
                            MainGallonLevel = Convert.ToDecimal(txtChildGallonLevel.Text.ToString())
                        Catch ex As Exception
                            lblChildErrorMessageForGallon.Text = "Please enter Gallon Level in decimal format."
                            lblChildErrorMessageForGallon.Visible = True
                            txtChildGallonLevel.Focus()
                            Return
                        End Try
                    End If
                    If (Convert.ToDecimal(txtParentGallonLevel.Text.Trim()) > Convert.ToDecimal(txtChildGallonLevel.Text.Trim())) Then
                        ErrorMessage.Visible = True
                        ErrorMessage.InnerText = "A smaller quantity being assigned to a higher level while updating data, please modify quantity try again."
                        message.Visible = False
                        txtChildGallonLevel.Focus()
                        Return
                    End If
                Next
            Next


            'Update
            Dim dtTankChartDetail As DataTable = New DataTable("UpdateTankChartDetail")
            dtTankChartDetail.Columns.Add(New DataColumn("TankChartDetailId", System.Type.[GetType]("System.Int32")))
            dtTankChartDetail.Columns.Add(New DataColumn("GallonLevel", System.Type.[GetType]("System.Decimal")))
            For i = 0 To gv_TankChartDetails.Rows.Count - 1
                Dim TankChartDetailId As Integer = Convert.ToInt32(gv_TankChartDetails.DataKeys(i).Value().ToString())
                Dim txtGallonLevel As TextBox = DirectCast(gv_TankChartDetails.Rows(i).FindControl("txtGallonLevel"), TextBox)
                dtTankChartDetail.Rows.Add(TankChartDetailId, txtGallonLevel.Text)
            Next

            If dtTankChartDetail.Rows.Count > 0 Then

                Dim TankChartId As Integer = Convert.ToInt32(gv_TankChartDetails.DataKeys(0).Values("TankChartId"))

                OBJMaster = New MasterBAL()
                OBJMaster.UpdateTankChartDetail(dtTankChartDetail, Session("PersonId"))

                gv_TankChartDetails.EditIndex = -1

                SaveCoefficient(TankChartId)

                BindTankChartDetails(TankChartId)
                message.Visible = True
                ErrorMessage.Visible = False
                message.InnerText = "Updated Successfully."

                For i = 0 To gv_TankChartDetails.Rows.Count - 1

                    Dim GallonLevel As TextBox = DirectCast(gv_TankChartDetails.Rows(i).Cells(0).FindControl("txtGallonLevel"), TextBox)
                    Dim TankChartDetailId As Integer = Convert.ToInt32(gv_TankChartDetails.DataKeys(i).Values("TankChartDetailId"))
                    Dim LBL_IncLevel As Label = DirectCast(gv_TankChartDetails.Rows(i).Cells(0).FindControl("LBL_IncLevel"), Label)

                    afterTankChartDetails = IIf(afterTankChartDetails = "", CreateData(TankChartDetailId, LBL_IncLevel.Text, GallonLevel.Text), afterTankChartDetails & ";" & CreateData(TankChartDetailId, LBL_IncLevel.Text, GallonLevel.Text))
                Next


                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = afterTankChartDetails
                    Dim beforeData As String = beforeTankChartDetails

                    CSCommonHelper.WriteLog("Modified", "Tank Charts Details", beforeData, writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                End If
            End If

        Catch ex As Exception
            log.Error("Error occurred in btnSave_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while updating data, please try again later."
        End Try
    End Sub


    Private Function SaveCoefficient(TankChartId As Integer) As Double()
        Dim coefficient(4) As Double
        Try

            'Implementation of Linest function
            OBJMaster = New MasterBAL()
            Dim dtTankChartDetail As DataTable = New DataTable()
            dtTankChartDetail = OBJMaster.GetTankChartDetailsByTankChartId(TankChartId)


            Dim datapointsx(dtTankChartDetail.Rows.Count - 1) As Double
            Dim datapointsy(dtTankChartDetail.Rows.Count - 1) As Double

            Dim i As Integer = 0
            For Each dr As DataRow In dtTankChartDetail.Rows
                datapointsx(i) = dr("IncrementLevel")
                datapointsy(i) = dr("GallonLevel")
                i = i + 1
            Next

            coefficient = MathNet.Numerics.Fit.Polynomial(datapointsx, datapointsy, 3, MathNet.Numerics.LinearRegression.DirectRegressionMethod.Svd)
            OBJMaster = New MasterBAL()
            OBJMaster.SaveCoefficients(TankChartId, coefficient(0), coefficient(1), coefficient(2), coefficient(3))
            'Dim xval As Double = "68"
            'Dim yval0 As Double = coefficient(0) + (coefficient(1) * xval) + (coefficient(2) * Math.Pow(xval, 2)) + coefficient(3) * Math.Pow(xval, 3)

        Catch ex As Exception
            log.Error("Error occurred in SaveCoefficient Exception is :" + ex.Message)
        End Try
        Return coefficient
    End Function

    Protected Sub txtGallonLevel_TextChanged(sender As Object, e As EventArgs)
        Try
            Dim MainGallonLevel As Decimal = 0
            Dim LowerGallonLevel As Decimal = 0
            Dim HigherGallonLevel As Decimal = 0
            Dim gr As GridViewRow = CType(CType(sender, TextBox).NamingContainer, GridViewRow)
            Dim lblErrorMessageForGallon As Label = CType(gv_TankChartDetails.Rows(gr.RowIndex).FindControl("lblErrorMessageForGallon"), Label)
            Try
                MainGallonLevel = Convert.ToDecimal(CType(sender, TextBox).Text.ToString())
            Catch ex As Exception
                lblErrorMessageForGallon.Text = "Please enter Gallon Level in decimal format."
                lblErrorMessageForGallon.Visible = True
                CType(sender, TextBox).Focus()
                Return
            End Try
            If (gr.DataItemIndex = 0) Then
                Dim txtHigherGallonLevel As TextBox = CType(gv_TankChartDetails.Rows(gr.RowIndex + 1).FindControl("txtGallonLevel"), TextBox)
                HigherGallonLevel = Convert.ToDecimal(txtHigherGallonLevel.Text)
                If (MainGallonLevel > HigherGallonLevel) Then
                    lblErrorMessageForGallon.Text = "A higher quantity being assigned to a lower level."
                    lblErrorMessageForGallon.Visible = True
                    CType(sender, TextBox).Focus()
                Else
                    lblErrorMessageForGallon.Text = ""
                    lblErrorMessageForGallon.Visible = False
                End If
            ElseIf (gr.DataItemIndex >= gv_TankChartDetails.Rows.Count - 1) Then
                Dim txtLowerGallonLevel As TextBox = CType(gv_TankChartDetails.Rows(gr.RowIndex - 1).FindControl("txtGallonLevel"), TextBox)
                LowerGallonLevel = Convert.ToDecimal(txtLowerGallonLevel.Text)
                If (MainGallonLevel < LowerGallonLevel) Then
                    lblErrorMessageForGallon.Text = "A lower quantity being assigned to a higher level."
                    lblErrorMessageForGallon.Visible = True
                    CType(sender, TextBox).Focus()
                Else
                    lblErrorMessageForGallon.Text = ""
                    lblErrorMessageForGallon.Visible = False
                End If
            Else
                Dim txtHigherGallonLevel As TextBox = CType(gv_TankChartDetails.Rows(gr.RowIndex + 1).FindControl("txtGallonLevel"), TextBox)
                HigherGallonLevel = Convert.ToDecimal(txtHigherGallonLevel.Text)
                Dim txtLowerGallonLevel As TextBox = CType(gv_TankChartDetails.Rows(gr.RowIndex - 1).FindControl("txtGallonLevel"), TextBox)
                LowerGallonLevel = Convert.ToDecimal(txtLowerGallonLevel.Text)

                If (MainGallonLevel > HigherGallonLevel) Then
                    lblErrorMessageForGallon.Text = "A higher quantity being assigned to a lower level."
                    lblErrorMessageForGallon.Visible = True
                    CType(sender, TextBox).Focus()
                    Return
                Else
                    lblErrorMessageForGallon.Text = ""
                    lblErrorMessageForGallon.Visible = False
                End If


                If (MainGallonLevel < LowerGallonLevel) Then
                    lblErrorMessageForGallon.Text = "A lower quantity being assigned to a higher level."
                    lblErrorMessageForGallon.Visible = True
                    CType(sender, TextBox).Focus()
                    Return
                Else
                    lblErrorMessageForGallon.Text = ""
                    lblErrorMessageForGallon.Visible = False
                End If
            End If
        Catch ex As Exception
            log.Error("Error occurred in txtGallonLevel_TextChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred, please try again later."
        End Try
    End Sub
End Class
