Imports log4net
Imports log4net.Config

Public Class CompanyHostingDateReport
    Inherits System.Web.UI.Page

    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CompanyHostingDateReport))

    Dim OBJMaster As MasterBAL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try
			XmlConfigurator.Configure()

        ErrorMessage.Visible = False
        message.Visible = False

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				Response.Redirect("/Account/Login")
			ElseIf Session("RoleName") <> "SuperAdmin" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If Not IsPostBack Then
                txtBeginningHostingDate.Text = DateTime.Now.ToString("MM/dd/yyyy")
                txtEndingHostingDate.Text = DateTime.Now.AddYears(1).ToString("MM/dd/yyyy")
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
            Else
                txtBeginningHostingDate.Text = Request.Form(txtBeginningHostingDate.UniqueID)
                txtEndingHostingDate.Text = Request.Form(txtEndingHostingDate.UniqueID)
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
            End If
        End If

		Catch ex As Exception

			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			ErrorMessage.Visible = True
			ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")

		End Try
	End Sub

	Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

        Try
            OBJMaster = New MasterBAL()
            Dim dtCompany As DataTable = New DataTable()

            Dim strConditions As String = ""
            Dim BeginningHostingDate As DateTime
            Dim EndingHostingDate As DateTime

            If Request.Form(txtBeginningHostingDate.UniqueID) <> "" Then
                BeginningHostingDate = Request.Form(txtBeginningHostingDate.UniqueID)
            Else
                BeginningHostingDate = txtBeginningHostingDate.Text
            End If
            If Request.Form(txtEndingHostingDate.UniqueID) <> "" Then
                EndingHostingDate = Request.Form(txtEndingHostingDate.UniqueID)
            Else
                EndingHostingDate = txtEndingHostingDate.Text
            End If

            If EndingHostingDate < BeginningHostingDate Then
                ErrorMessage.Visible = True
                ErrorMessage.InnerText = "Ending Hosting Date must be greater than Beginning Hosting Date."
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "loadFunction();LoadDateTimeControl();", True)
                Return
            End If


            strConditions = IIf(strConditions = "", " and ISNULL(BeginningHostingDate,CreatedDate) >= '" + BeginningHostingDate + "' and ISNULL(EndingHostingDate,DATEADD(year, 1, CreatedDate)) <= '" + EndingHostingDate.ToString("MM/dd/yyyy") + " 23:59' ", strConditions + " and ISNULL(BeginningHostingDate,CreatedDate) >= '" + BeginningHostingDate + "' and ISNULL(EndingHostingDate,DATEADD(year, 1, CreatedDate)) <= '" + EndingHostingDate.ToString("MM/dd/yyyy") + "' 23:59")

            'get data from server
            dtCompany = OBJMaster.GetCustByConditions(strConditions, Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
            If (Not dtCompany Is Nothing) Then

                If (dtCompany.Rows.Count <= 0) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData()

                        CSCommonHelper.WriteLog("Report Genereated", "Company Hosting Date Reportt", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData()

                    CSCommonHelper.WriteLog("Report Genereated", "Company Hosting Date Reportt", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                Return

            End If
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Company Hosting Date Reportt", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
            End If
            Session("BeginningHostingDate") = BeginningHostingDate.ToString("MM/dd/yyyy")
            Session("EndingHostingDate") = EndingHostingDate.ToString("MM/dd/yyyy")
            Session("CompanyHostingDateDetails") = dtCompany

            Response.Redirect("~/Reports/CompanyHostingDateRpt")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Company Hosting Date Report", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
            End If
            ErrorMessage.InnerText = "Data not found against selected criteria."
            ErrorMessage.Visible = True
            Return
        Finally
            txtBeginningHostingDate.Focus()
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
        End Try

    End Sub

    Private Function CreateData() As String
        Try
            Dim BeginningHostingDate As String
            Dim EndingHostingDate As String

            If Request.Form(txtBeginningHostingDate.UniqueID) <> "" Then
                BeginningHostingDate = Request.Form(txtBeginningHostingDate.UniqueID)
            Else
                BeginningHostingDate = txtBeginningHostingDate.Text
            End If
            If Request.Form(txtEndingHostingDate.UniqueID) <> "" Then
                EndingHostingDate = Request.Form(txtEndingHostingDate.UniqueID)
            Else
                EndingHostingDate = txtEndingHostingDate.Text
            End If

            Dim data As String = "Beginning Hosting Date = " & BeginningHostingDate & " ; " &
                                 "Ending Hosting Date = " & EndingHostingDate & "  "
            Return data
        Catch ex As Exception
            log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

End Class