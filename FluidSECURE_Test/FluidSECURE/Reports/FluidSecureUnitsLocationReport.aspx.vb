Imports Newtonsoft.Json
Imports log4net.Config
Imports log4net

Public Class FluidSecureUnitsLocationReport
    Inherits System.Web.UI.Page

	Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(FluidSecureUnitsLocationReport))

	Dim OBJMaster As MasterBAL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try

			message.Visible = False
        XmlConfigurator.Configure()

			If CSCommonHelper.CheckSessionExpired() = False Then
				'unautorized access error log
				Response.Redirect("/Account/Login")
			ElseIf Session("RoleName") = "User" Then
				'Access denied 
				Response.Redirect("/home")
			Else
				If Not IsPostBack Then

                BindCustomer(Convert.ToInt32(Session("PersonId").ToString()), Session("RoleId").ToString())
                DDL_Customer_SelectedIndexChanged(Nothing, Nothing)
            Else

            End If

            DDL_Customer.Focus()

        End If

		Catch ex As Exception
			log.Error("Error occurred in Page_Load Exception is :" + ex.Message)
			message.Visible = True
			message.InnerText = IIf(message.InnerText <> "", "", "Error occurred while loading details, please try again later.")
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

            log.Error("Error occurred in BindCustomer Exception is :" + ex.Message)
            message.Visible = True
            message.InnerText = "Error occurred while getting companies, please try again later."

        End Try
    End Sub

    Private Sub BindSites(CustomerId As Integer)
        Try


            Dim dtSites As DataTable = New DataTable()
            OBJMaster = New MasterBAL()



            dtSites = OBJMaster.GetSiteByCondition(" And c.CustomerId =" + CustomerId.ToString(), Session("PersonId").ToString(), Session("RoleId").ToString(), False)

            DDL_Site.DataSource = dtSites
            DDL_Site.DataValueField = "SiteId"
            DDL_Site.DataTextField = "WifiSSid"
            DDL_Site.DataBind()

            DDL_Site.Items.Insert(0, New ListItem("Select All FluidSecure Link", "0"))

        Catch ex As Exception

            message.Visible = True
            message.InnerText = "Error occurred  while getting sites, please try again later."

            log.Error("Error occurred in BindSites Exception is :" + ex.Message)

        End Try
    End Sub

    Protected Sub DDL_Customer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Customer.SelectedIndexChanged
		Try
			BindSites(Convert.ToInt32(DDL_Customer.SelectedValue))

		Catch ex As Exception

			log.Error("Error occurred in DDL_Customer_SelectedIndexChanged Exception is :" + ex.Message)
			message.Visible = True
			message.InnerText = "Error occurred while getting data, please try again later."

		End Try

	End Sub


    <System.Web.Services.WebMethod(True)>
    Public Shared Function GetLocations(customerId As String, siteId As String, customername As String, site As String) As String
        Dim writtenData As String = ""
        Try
            writtenData = "Company = " & customername.Replace(",", " ") & " ; " &
                                   "FluidSecure Link = " & Site.Replace(",", " ") & " ; "

            Dim OBJMaster As MasterBAL = New MasterBAL()

            Dim strConditions As String = ""
            Dim dtFluidSecureUnit As DataTable = New DataTable()

            If (customerId <> "0") Then
                strConditions = IIf(strConditions = "", " and H.CustomerId = " + customerId, strConditions + " and H.CustomerId = " + customerId)
            End If

            If (siteId <> "0") Then
                strConditions = IIf(strConditions = "", " and H.SiteId = " + siteId, strConditions + " and H.SiteId = " + siteId)
            End If

            strConditions += " order by H.WifiSSId"
            'get data from server

            dtFluidSecureUnit = OBJMaster.GetSiteByCondition(strConditions, HttpContext.Current.Session("PersonId").ToString(), HttpContext.Current.Session("RoleId").ToString(), False)

            Dim listOfLocations As New List(Of LocationDetails)()

            For Each row As DataRow In dtFluidSecureUnit.Rows

                If (Convert.ToBoolean(row("DisableGeoLocation")) <> True) Then
                    Dim location As LocationDetails = New LocationDetails()

                    location.Lat = row("Latitude")
                    location.Lng = row("Longitude")
                    location.Address = row("SiteAddress")
                    listOfLocations.Add(location)

                End If

            Next

            If (listOfLocations.Count = 0) Then
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Report Genereated", "FluidSecure Links Locations", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria")
                End If
                Dim json = JsonConvert.SerializeObject(-1)

                Return json
            Else
                If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                    CSCommonHelper.WriteLog("Report Genereated", "FluidSecure Links Locations", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "success", "")
                End If
                Dim json = JsonConvert.SerializeObject(listOfLocations)

                Return json
            End If
        Catch ex As Exception
            If (ConfigurationManager.AppSettings("AllowActivityLogin").ToString().ToLower() = "yes") Then
                CSCommonHelper.WriteLog("Report Genereated", "FluidSecure Links Locations", "", writtenData, HttpContext.Current.Session("PersonName").ToString() & "(" & HttpContext.Current.Session("PersonEmail").ToString() & ")", HttpContext.Current.Session("IPAddress").ToString(), "fail", "Data not found against selected criteria. Exception is " & ex.Message)
            End If
            log.Error("Error occurred in GetLocations Exception is :" + ex.Message)

            Dim json = JsonConvert.SerializeObject(-2)

            Return json
        End Try

    End Function

End Class

Public Class LocationDetails

    Public Lat As String
    Public Lng As String
    Public Address As String

End Class
