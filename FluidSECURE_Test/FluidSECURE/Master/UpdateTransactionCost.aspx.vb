Imports log4net
Imports log4net.Config

Public Class UpdateTransactionCost
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(UpdateTransactionCost))

    Dim OBJMaster As MasterBAL = New MasterBAL()

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
				If Not IsPostBack Then
					If (Not Request.QueryString("FuelTypeID") = Nothing And Not Request.QueryString("FuelTypeID") = "") Then
						HDF_FuelTypeId.Value = Request.QueryString("FuelTypeID")
						txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
						txtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                        lblProductName.InnerText = Session("FuelType")
                        txtStartTime.Text = DateTime.Now.ToString("hh:mm tt")
                        txtEndTime.Text = DateTime.Now.ToString("hh:mm tt")
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                    End If
					txt_Price.Attributes.Add("OnKeyPress", "return KeyPressProduct(event);")
				End If
			End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.Text = IIf(ErrorMessage.Text <> "", "", "Error occurred while loading details, please try again later.")
		End Try
	End Sub

	Protected Sub btnPostPrice_Click(sender As Object, e As EventArgs)
        Try
			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log

				ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "CheckSession();", True)
				Return
			End If

			Dim newSDate As DateTime
            Dim newEDate As DateTime

            Dim resultDecimal As Decimal = 0

			If (txt_Price.Text <> "" And Not (Decimal.TryParse(txt_Price.Text, resultDecimal))) Then
				ErrorMessage.Visible = True
				ErrorMessage.Text = "Please enter Product price as decimal and try again."
				txt_Price.Focus()
				Return
			End If

			Dim checkValue As Integer = 0

            Try
                If Not txt_Price.Text <> "" Then
                    checkValue = 2
                End If
            Catch ex As Exception
                checkValue = 2
            End Try

            If checkValue = 0 Then
                Try
                    If txtStartTime.Text = "" Then
                        checkValue = 4
                    Else
                        If txtEndTime.Text = "" Then
                            checkValue = 5
                        End If
                    End If
                Catch ex As Exception

                End Try
            End If

            If checkValue = 0 Then
                Try
                    newSDate = DateTime.Parse(txtStartDate.Text & " " & Request.Form(txtStartTime.UniqueID))
                    newEDate = DateTime.Parse(txtEndDate.Text & " " & Request.Form(txtEndTime.UniqueID))
                    If newSDate > newEDate Then
                        checkValue = 3
                    End If
                Catch ex As Exception
                    checkValue = 1
                End Try
            End If

            If checkValue = 0 Then
                ErrorMessage.Visible = False
                message.Visible = False

                Dim count As Integer = 0
                Dim StartDateTime As String = Convert.ToDateTime(txtStartDate.Text & " " & Request.Form(txtStartTime.UniqueID)).ToString("yyyy-MM-dd HH:mm:ss")
                Dim EndDateTime As String = Convert.ToDateTime(txtEndDate.Text & " " & Request.Form(txtEndTime.UniqueID)).ToString("yyyy-MM-dd HH:mm:ss")
                OBJMaster = New MasterBAL()
                count = OBJMaster.PostPriceInTransaction(Convert.ToInt32(Session("PersonId").ToString), Convert.ToInt32(HDF_FuelTypeId.Value), Convert.ToDecimal(txt_Price.Text), StartDateTime, EndDateTime, "", Convert.ToInt32(Session("FuelCustomerId").ToString), 1)
                If (count > 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(HDF_FuelTypeId.Value))
                        CSCommonHelper.WriteLog("Added", "Transaction Cost", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
                    End If
                Else
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData(Convert.ToInt32(HDF_FuelTypeId.Value))
                        CSCommonHelper.WriteLog("Added", "Transaction Cost", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Transaction Cost saving failed.")
                    End If
                End If
                message.Visible = True
                message.Text = "New COST updated in " & count & " transaction records!!"
            ElseIf checkValue = 1 Then
                message.Visible = False
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Date/Time formats are not valid. Please enter valid date/time and try again !!"
                txtStartDate.Focus()
            ElseIf checkValue = 2 Then
                message.Visible = False
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Price field is empty. Please enter Price and try again !!"
                txt_Price.Focus()
            ElseIf checkValue = 3 Then
                message.Visible = False
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Start Date greater than End date. Please enter valid dates and try again !!"
                txtStartDate.Focus()
            ElseIf checkValue = 4 Then
                message.Visible = False
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please Enter Start Time and try again !!"
                txtStartTime.Focus()
            ElseIf checkValue = 5 Then
                message.Visible = False
                ErrorMessage.Visible = True
                ErrorMessage.Text = "Please Enter End Time and try again !!"
                txtEndTime.Focus()
            End If



        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData(Convert.ToInt32(HDF_FuelTypeId.Value))
                CSCommonHelper.WriteLog("Added", "Transaction Cost", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Transaction Cost saving failed. Exception is " & ex.Message)
            End If
            log.Error("Error occurred in btnPostPrice_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting department data, please try again later."
            txt_Price.Focus()
        Finally
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try

    End Sub

    Protected Sub btnHistory_Click(sender As Object, e As EventArgs) Handles btnHistory.Click
        Try
            Dim dtResult As DataTable = New DataTable()
        OBJMaster = New MasterBAL()
        dtResult = OBJMaster.GetHistoryPostPriceInTransaction(0, Convert.ToInt32(HDF_FuelTypeId.Value), 0, "", "", "", Convert.ToUInt32(Session("FuelCustomerId").ToString), 2)

        If Not IsNothing(dtResult) Then
            If dtResult.Rows.Count > 0 Then
                gv_History.DataSource = dtResult
                gv_History.DataBind()
            Else
                gv_History.DataSource = Nothing
                gv_History.DataBind()
            End If
        Else
            gv_History.DataSource = Nothing
            gv_History.DataBind()
        End If

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "OpenHistory();LoadDateTimeControl();", True)

        Catch ex As Exception

            log.Error("Error occurred in btnHistory_Click Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.Text = "Error occurred while getting department data, please try again later."
        Finally
            txt_Price.Focus()
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("Fuel.aspx?FuelTypeID=" & HDF_FuelTypeId.Value, False)
    End Sub

    Private Function CreateData(ProductId As Integer) As String
        Try

            Dim data As String = ""

            data = "ProductId = " & ProductId & " ; " &
                    "Product Name = " & lblProductName.InnerText.Replace(",", " ") & " ; " &
                    "Reset price = " & txt_Price.Text.Replace(",", " ") & " ; " &
                    "Start date = " & txtStartDate.Text.Replace(",", " ") & " ; " &
                    "End Date = " & txtEndDate.Text.Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class
