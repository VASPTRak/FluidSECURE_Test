<%@ Page Title="Shipment" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Shipment.aspx.vb" Inherits="Fuel_Secure.Shipment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                FluidSecure Link Name
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtFluidSecureUnitName" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="32" Width="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFDFluidSecureUnitName" runat="server" ControlToValidate="txtFluidSecureUnitName"
                                ErrorMessage="Please Enter FluidSecure Link Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                            <asp:HiddenField ID="HDF_ShipmentId" runat="server"></asp:HiddenField>
                            <asp:HiddenField ID="HDF_TotalShipments" runat="server" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div id="divCompany" runat ="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Company<label class="text-danger font-required">&nbsp;[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <%--<asp:TextBox ID="txtCompany" runat="server" CssClass="form-control input-sm" MaxLength="30" Width="200" TabIndex="2"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="txtCompany" Display="Dynamic" ErrorMessage="Please enter company." ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>--%>
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="19" AutoPostBack="true" ></asp:DropDownList>
                            <asp:Label ID="lblCustomer" runat="server" ForeColor="Red" Style="display: none;" Text="Please select company."></asp:Label>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Address<label class="text-danger font-required">&nbsp;[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox TextMode="MultiLine" Rows="4" ID="txtAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" Width="200" TabIndex="3"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RDF_Address" runat="server" ControlToValidate="txtAddress" Display="Dynamic" ErrorMessage="Please enter address." ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                         <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Shipment Date 
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtShipmentDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
                              <asp:RequiredFieldValidator ID="RDF_ShipmentDate" runat="server" ControlToValidate="txtShipmentDate" Display="Dynamic" style="float: left;"
                                ErrorMessage="Please select Shipment Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                <%--    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                         <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Shipment Time 
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtShipmentTime" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="5"></asp:TextBox>
                               <asp:RequiredFieldValidator ID="RDF_ShipTime" runat="server" ControlToValidate="txtShipmentTime" Display="Dynamic" style="float: left;"
                                ErrorMessage="Please select Shipment Time." ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>--%>

                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="False" TabIndex="5" ValidationGroup="ShipmentValidation" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="6" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
                            UseSubmitBehavior="false" TabIndex="7" ValidationGroup="ShipmentValidation" OnClick="btnSaveAndAddNew_Click" />
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center clear col-xs-12" style="margin: 10px 0">
                        <asp:Button ID="btnFirst" runat="server" Text="|<" CssClass="NewDept_ButtonFooter"
                            OnClick="First_Click" /><asp:Button ID="btnprevious" runat="server" Text="<" CssClass="NewDept_ButtonFooter" OnClick="btnprevious_Click" />
                        <asp:Label ID="lblof" runat="server" Text="Label" BorderColor="Black" BorderStyle="Solid"
                            BorderWidth="1px" Font-Bold="True" Font-Names="arial" Font-Size="Small" Width="115px"></asp:Label>
                        <asp:Button ID="btnNext" runat="server" Text=">" CssClass="NewDept_ButtonFooter" OnClick="btnNext_Click" /><asp:Button
                            ID="btnLast" runat="server" Text=">|" CssClass="NewDept_ButtonFooter" OnClick="btnLast_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">

    <script type="text/javascript">

        function LoadDateTimeControl() {

            $("[id$=txtShipmentDate]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $('[id$=txtShipmentTime]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

            });
        }
        $(document).ready(function () {



        });

        function IsValidCompany() {
            var Customer = $('#<%=DDL_Customer.ClientID%>').val();
             if (Customer == "0") {
                IsValid = false;
                $("#<%=lblCustomer.ClientID()%>").show();
            }
            else {
                $("#<%=lblCustomer.ClientID()%>").hide();
             }
        }

    </script>

</asp:Content>
