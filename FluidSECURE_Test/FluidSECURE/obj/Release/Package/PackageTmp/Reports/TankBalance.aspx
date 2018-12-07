<%@ Page Title="Tank Balance Report" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="TankBalance.aspx.vb" Inherits="Fuel_Secure.TankBalance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .btn-group, .multiselect.dropdown-toggle.btn.btn-default, ul.multiselect-container.dropdown-menu {
            width: 100% !important;
            text-align: left;
        }
    </style>

    <asp:UpdatePanel ID="UP_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Tank Balance Report</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div ID = "divCompany" runat ="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="1" AutoPostBack ="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Customer" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="InventoryValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Tank Number:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="ddl_TankNo" CssClass="form-control input-sm" TabIndex="2" runat="server"></asp:DropDownList>
                           <%-- <asp:TextBox ID="txtTankNo" runat="server" CssClass="form-control input-sm" TabIndex="6" Width="50" onkeypress="return onlyNumbers(event);"></asp:TextBox>--%>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnGenarateReport" CssClass="btn btn-primary" runat="server" OnClientClick="setvalue();" OnClick="btnGenarateReport_Click" Text="Generate Report"
                            UseSubmitBehavior="False" TabIndex="7" ValidationGroup="InventoryValidation" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-primary" runat="server" OnClick="btnCancel_Click" Text="Exit" TabIndex="8"
                            CausesValidation="false" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
     <asp:HiddenField ID="HDF_CurrentDate" runat="server" />

    <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">
    <script type="text/javascript">

        function LoadDateTimeControl() {
            $("[id$=txtStartDateFrom]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $("[id$=txtEndDateTo]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });


            $('[id$=txtStartTimeFrom]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

            });


            $('[id$=txtEndTimeTo]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

            });


        }

        function loadMultiList() {
            $('[id*=lstSites]').multiselect({
                includeSelectAllOption: true,
                allSelectedText: 'All FluidSecure Link',
                maxHeight: 200,
            })
        }

        $(function () {
            $('[id*=lstSites]').multiselect({
                includeSelectAllOption: true,
                allSelectedText: 'All FluidSecure Link',
                maxHeight: 200,
            }).multiselect('selectAll', false).multiselect('updateButtonText');
        });

         function setvalue() {

            var HDF_CurrentDate = $("#<%=HDF_CurrentDate.ClientID%>");

            var localTime = new Date();
            var year = localTime.getFullYear();
            var month = localTime.getMonth() + 1;
            var date = localTime.getDate();
            var hours = localTime.getHours();
            var minutes = localTime.getMinutes();
            var seconds = localTime.getSeconds();

            HDF_CurrentDate.val(month + "/" + date + "/" + year + " " + hours + ":" + minutes + ":" + seconds);

        }
    </script>
    
</asp:Content>

