Imports log4net
Imports log4net.Config
Public Class CustomerDetailsByStateCountry
    Inherits System.Web.UI.Page
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(CustomerDetailsByStateCountry))

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
                    BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                    DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
                    DDL_Customer.Focus()
                End If
            End If

        Catch ex As Exception
            Log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = IIf(ErrorMessage.InnerText <> "", "", "Error occurred while loading details, please try again later.")
        End Try
    End Sub

    Private Sub BindCustomer(PersonId As Integer, RoleId As String)
        Try

            OBJMaster = New MasterBAL()
            Dim dtCust As DataTable = New DataTable()

            dtCust = OBJMaster.GetCustomerDetailsByPersonID(PersonId, RoleId, Session("CustomerId").ToString())

            DDL_Customer.DataSource = dtCust
            DDL_Customer.DataTextField = "CustomerName"
            DDL_Customer.DataValueField = "CustomerId"
            DDL_Customer.DataBind()
            DDL_Customer.Items.Insert(0, New ListItem("Select Company", "0"))

            If (Not Session("RoleName") = "SuperAdmin" And Not Session("RoleName") = "Support" And Not Session("RoleName") = "GroupAdmin") Then
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

            Log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
        Try
            BindStatesCountry(Convert.ToInt32(DDL_Customer.SelectedValue))
        Catch ex As Exception
            Log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

    Protected Sub btnGenarateReport_Click(sender As Object, e As EventArgs)

        Try

            OBJMaster = New MasterBAL()
            Dim dSTran As DataSet = New DataSet()
            Dim StrCondition As String = ""

            If ddl_State.SelectedValue <> "0" Then
                StrCondition = StrCondition & " and [State] = '" & ddl_State.SelectedValue.ToString() & "' "
            End If

            If ddl_Country.SelectedValue <> "0" Then
                StrCondition = StrCondition & " and Country = '" & ddl_Country.SelectedValue.ToString() & "' "
            End If

            If DDL_Customer.SelectedValue <> "0" Then
                StrCondition = StrCondition & " and CustomerId = " & DDL_Customer.SelectedValue.ToString() & " "
            End If

            'get data from server
            dSTran = OBJMaster.GetStateCountryWiseCustomerDetails(StrCondition)
            If (Not dSTran Is Nothing) Then

                If (dSTran.Tables.Count < 1) Then
                    If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                        Dim writtenData As String = CreateData()

                        CSCommonHelper.WriteLog("Report Genereated", "Customer Report By State and Country", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                    End If
                    ErrorMessage.InnerText = "Data not found against selected criteria."
                    ErrorMessage.Visible = True
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                    Return
                End If
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    Dim writtenData As String = CreateData()

                    CSCommonHelper.WriteLog("Report Genereated", "Customer Report By State and Country", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria.")
                End If
                ErrorMessage.InnerText = "Data not found against selected criteria."
                ErrorMessage.Visible = True
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
                Return

            End If
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Customer Report By State and Country", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "success", "")
            End If
            Session("StateCountryWiseCustomerReport") = dSTran
            Response.Redirect("~/Reports/CustomerDetailsByStateCountryReport")


        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                Dim writtenData As String = CreateData()

                CSCommonHelper.WriteLog("Report Genereated", "Customer Report By State and Country", "", writtenData, Session("PersonName").ToString() & "(" & Session("PersonEmail").ToString() & ")", Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
            End If
            ErrorMessage.InnerText = "Data not found against selected criteria."
            ErrorMessage.Visible = True
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MSG", "LoadDateTimeControl();", True)
            Return
        End Try

    End Sub

    Private Function CreateData() As String
        Try

            Dim data As String = " State = " & ddl_State.SelectedItem.ToString().Replace(",", "") & " ; " &
                                    " Country = " & ddl_Country.SelectedItem.ToString().Replace(",", " ") & " ; " &
                                    "Company = " & DDL_Customer.SelectedItem.Text.Replace(",", " ") & " ; "

            Return data
        Catch ex As Exception
            Log.Error(String.Format("Error Occurred while CreateData. Error is {0}.", ex.Message))
            Return ""
        End Try

    End Function

    Private Sub BindStatesCountry(CustomerId As Integer)
        Try

            OBJMaster = New MasterBAL()
            Dim ds As DataSet = New DataSet()
            ds = OBJMaster.GetStateCountryFromCustomers(DDL_Customer.SelectedValue)

            ddl_State.DataSource = ds.Tables(0)
            ddl_State.DataTextField = "State"
            ddl_State.DataValueField = "State"
            ddl_State.DataBind()

            ddl_Country.DataSource = ds.Tables(1)
            ddl_Country.DataTextField = "Country"
            ddl_Country.DataValueField = "Country"
            ddl_Country.DataBind()

            ddl_State.Items.Insert(0, New ListItem("Select State", "0"))
            ddl_Country.Items.Insert(0, New ListItem("Select Country", "0"))

            If ds.Tables(0).Rows.Count = 1 Then
                ddl_State.SelectedIndex = 1
            End If

            If ds.Tables(1).Rows.Count = 1 Then
                ddl_Country.SelectedIndex = 1
            End If

        Catch ex As Exception
            log.Error("Error occurred in BindStatesCountry Exception is :" + ex.Message)
            ErrorMessage.Visible = True
            ErrorMessage.InnerText = "Error occurred while getting data, please try again later."
        End Try
    End Sub

End Class