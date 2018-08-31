f<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AuthenticationService.aspx.vb" Inherits="Fuel_Secure.AuthonticationService" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table align="center">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblHeading" runat="server" Text ="Authentication Service Call" ></asp:Label>
                </th>
            </tr>
            <tr>
                <td><asp:Label ID="lblIMEIUDID" runat="server" Text="IMEI or UDID"></asp:Label> </td>
                <td><asp:TextBox ID="txtIMEIUDID" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblOdoMeter" runat="server" Text="Odo Meter"></asp:Label> </td>
                <td><asp:TextBox ID="txtOdoMeter" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblVehicleNumber" runat="server" Text="Vehicle Number"></asp:Label> </td>
                <td><asp:TextBox ID="txtVehicleNumber" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblWifiSSId" runat="server" Text="Wifi SSId"></asp:Label> </td>
                <td><asp:TextBox ID="txtWifiSSId" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblSiteId" runat="server" Text="Select Site"></asp:Label> </td>
                <td><asp:DropDownList ID="ddlSiteName" runat="server" Width="150px" TabIndex="1">
                                </asp:DropDownList>   </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnSendRequest" runat ="server" Text="Send Request" OnClick="btnSendRequest_Click" />
                    <asp:Button ID="btnClear" runat ="server" Text="Clear" OnClick="btnClear_Click" />
                </td>
            </tr>
        </table>
                <asp:Label ID="lblResponce" runat ="server" ></asp:Label>
        <br />
        <br />
        <br />
        <table align="center">
            <tr>
                <th colspan="2">
                    <asp:Label ID="Label1" runat="server" Text ="Transaction Complete Call" ></asp:Label>
                </th>
            </tr>
            <tr>
                <td><asp:Label ID="lblVehicleId" runat="server" Text="Vehicle Id"></asp:Label> </td>
                <td><asp:TextBox ID="txtVehicleId" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblCurrentOdoMeter" runat="server" Text="Current Odometer"></asp:Label> </td>
                <td><asp:TextBox ID="txtCurrentOdoMeter" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblFuelTypeId" runat="server" Text="Fluid Type Id"></asp:Label> </td>
                <td><asp:TextBox ID="txtFuelTypeId" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblSite" runat="server" Text="Site Id"></asp:Label> </td>
                <td><asp:DropDownList ID="ddlSiteId" runat="server" Width="150px" TabIndex="1">
                                </asp:DropDownList>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblPersonId" runat="server" Text="Person Id"></asp:Label> </td>
                <td><asp:TextBox ID="txtPerson" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblFuelQuantity" runat="server" Text="Fluid Quantity"></asp:Label> </td>
                <td><asp:TextBox ID="txtFuelQuantity" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblPhoneNumber" runat="server" Text="Phone Number"></asp:Label> </td>
                <td><asp:TextBox ID="txtPhoneNumber" runat="server" ></asp:TextBox>  </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnSendReqToTransaction" runat ="server" Text="Send Request" OnClick="btnSendReqToTransaction_Click" />
                    <asp:Button ID="btnClearTransaction" runat ="server" Text="Clear" OnClick="btnClearTransaction_Click" />
                </td>
            </tr>
            
        </table>
        <asp:Label ID="lblTransactionResponce" runat ="server" ></asp:Label>
    </div>

        <div>
            for registration=
            <asp:Button ID="btnRegisteration" runat ="server" Text="Register" OnClick="btnRegisteration_Click"  />

        </div>

        <div>
            for login=
            <asp:TextBox ID="txtIMEINumber" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>

            <asp:Button ID="btnLogin" runat ="server" Text="Login" OnClick="btnLogin_Click"  />
            <asp:Label ID="lblResponseOfLogin" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
