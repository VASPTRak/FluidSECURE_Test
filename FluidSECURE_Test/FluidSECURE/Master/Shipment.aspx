<%@ Page Title="Shipment" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Shipment.aspx.vb" Inherits="Fuel_Secure.Shipment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .options label {
            padding-left: 5px;
            padding-right: 10px;
            vertical-align: middle;
        }
    </style>
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
                    <div class="row col-md-12 col-sm-12 col-xs-12 text-center">
                        <div class="form-group col-md-5 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <asp:RadioButtonList ID="RBL_Options" runat="server" RepeatDirection="Horizontal" CssClass="options"
                            OnSelectedIndexChanged="RBL_Options_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="Link Shipment" Value="1" />
                            <asp:ListItem Text="Hub Shipment" Value="2" />
                            <asp:ListItem Text="Card Reader" Value="3" />
                        </asp:RadioButtonList>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12" id="ShipmentForLinkName" runat="server">
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
                        </div>
                    </div>
                    <asp:HiddenField ID="HDF_ShipmentId" runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="HDF_TotalShipments" runat="server" />
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="ShipmentForHubName" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Hub Name
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtHubName" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="32" Width="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFD_HubName" runat="server" ControlToValidate="txtHubName"
                                ErrorMessage="Please Enter Hub Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="ShipmentForCardReader" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Serial Number
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtSerialNumber" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="32" Width="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFD_SerialNumber" runat="server" ControlToValidate="txtSerialNumber"
                                ErrorMessage="Please Serial Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Company<label class="text-danger font-required">&nbsp;[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="19" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:Label ID="lblCustomer" runat="server" ForeColor="Red" Style="display: none;" Text="Please select company."></asp:Label>
                            <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select company."
                                ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation" InitialValue="0"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Address:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox TextMode="MultiLine" Rows="4" ID="txtAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" Width="200" TabIndex="3"></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="RDF_Address" runat="server" ControlToValidate="txtAddress" Display="Dynamic" ErrorMessage="Please enter address." ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>--%>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="PhoneNumberForHub" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Phone Number:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control input-sm" MaxLength="15" Width="130" TabIndex="27" data-toggle="tooltip" title="Only (,),-, space, and + symbols allowed."></asp:TextBox>
                            <asp:Label ID="lblPhoneErrorMsg" runat="server" ForeColor="Red" Style="display: none;" Text="Please enter valid phone number."></asp:Label>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12" id="FrequencyForCardReader" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div id="divFrequency" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Select Frequency<label class="text-danger font-required">&nbsp;[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Frequency" runat="server" CssClass="form-control input-sm" TabIndex="19" AutoPostBack="true" OnSelectedIndexChanged="DDL_Frequency_SelectedIndexChanged">
                                <asp:ListItem Text="High" Value="High"></asp:ListItem>
                                <asp:ListItem Text="Low" Value="Low"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="lblFrequency" runat="server" ForeColor="Red" Style="display: none;" Text="Please select company."></asp:Label>
                            <asp:RequiredFieldValidator ID="RFD_Frequency" runat="server" ControlToValidate="DDL_Frequency" Display="Dynamic" ErrorMessage="Please select Frequency."
                                ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>


                    <div class="row col-md-12 col-sm-12 col-xs-12" id="HubForCardReader" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div id="divHubs" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>FluidSecure Hub:<label class="text-danger font-required">&nbsp;[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_HubForCardReader" runat="server" CssClass="form-control input-sm" TabIndex="19" AutoPostBack="true" OnSelectedIndexChanged="DDL_HubForCardReader_SelectedIndexChanged"></asp:DropDownList>
                            <asp:Label ID="lblHubs" runat="server" ForeColor="Red" Style="display: none;" Text="Please select Hub."></asp:Label>
                            <asp:RequiredFieldValidator ID="RFD_Hubs" runat="server" ControlToValidate="DDL_HubForCardReader" Display="Dynamic" ErrorMessage="Please select Hub."
                                ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation" InitialValue="0"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <asp:Label ID="ShipmentDate" runat="server"></asp:Label>
                                
                        <label class="text-danger font-required">[required]</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtShipmentDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RDF_ShipmentDate" runat="server" ControlToValidate="txtShipmentDate" Display="Dynamic" Style="float: left;"
                                ErrorMessage="Please select Shipment Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="ShipmentValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="ShipmentForReplacement" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Replacement:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="chkIsReplacement" runat="server" OnCheckedChanged="chkIsReplacement_CheckedChanged" AutoPostBack="true" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="ReplacementForLink" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                FluidSecure Links:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Sites" runat="server" CssClass="form-control input-sm" TabIndex="19"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="ReplacementForHub" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                FluidSecure Hub:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Hub" runat="server" CssClass="form-control input-sm" TabIndex="19"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Returned:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="CHK_Returned" runat="server" AutoPostBack="true" OnCheckedChanged="CHK_Returned_CheckedChanged" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="ReturnedDate" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Returned Date:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtReturnedDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="true" TabIndex="5" ValidationGroup="ShipmentValidation"/> <%--OnClientClick="return IsValidPhoneNumber();"--%>
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="6" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
                            UseSubmitBehavior="true" TabIndex="7" ValidationGroup="ShipmentValidation" OnClick="btnSaveAndAddNew_Click" />
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

            $("[id$=txtReturnedDate]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });
            //$('[id$=txtShipmentTime]').timepicker({
            //	timeFormat: 'h:mm p',
            //	interval: 01,
            //	dynamic: false,
            //	dropdown: true,
            //	scrollbar: true

            //});
        }
        //$(document).ready(function () {

        //});

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

        function rblSelect() {
            alert('sad');
        }

        <%--function IsValidPhoneNumber() {
            // debugger;
            var RBL_Options = $("#<%=RBL_Options.ClientID %>  input:checked");
            if (RBL_Options.val() == 2) {
                var phoneNumber = document.getElementById('<%=txtPhoneNumber.ClientID%>').value;

                //it accepts 850-294-2562(us phone number- req date 09-Dec-2016)
                //if (phoneNumber.match(/^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/)) {//phone number accept all number with only (-)+space symbols.
                if (phoneNumber.match(/^[- +()]*[0-9][- +()0-9]*$/) && phoneNumber != '') {

                    $('#MainContent_lblPhoneErrorMsg').hide();
                    Page_ClientValidate("ShipmentValidation");
                }
                else {
                    $('#MainContent_lblPhoneErrorMsg').show();

                    Page_ClientValidate("ShipmentValidation");
                    return false;
                }
            }
            else {
                Page_ClientValidate("ShipmentValidation");
            }
        }--%>

    </script>

</asp:Content>
