<%@ Page Title="Customer Report By State and Country" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CustomerDetailsByStateCountry.aspx.vb" Inherits="Fuel_Secure.CustomerDetailsByStateCountry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .btn-group, .multiselect.dropdown-toggle.btn.btn-default, ul.multiselect-container.dropdown-menu {
            width: 100% !important;
            text-align: left;
        }
    </style>
    <asp:UpdatePanel ID="UP_Main" runat="server">
        <ContentTemplate>
            <div id="mydiv">
                <img src="/Content/images/ajax-loader.gif" title="Please wait..." class="ajax-loader" />
            </div>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Customer Report By State and Country</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="1" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div id="div1" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                State:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="ddl_State" runat="server" CssClass="form-control input-sm" TabIndex="2"></asp:DropDownList>
                        </div>
                        <div id="div2" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Country:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="ddl_Country" runat="server" CssClass="form-control input-sm" TabIndex="3"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnGenarateReport" CssClass="btn btn-primary" runat="server" OnClick="btnGenarateReport_Click" Text="Generate Report"
                            UseSubmitBehavior="False" TabIndex="14" ValidationGroup="CompanyValidation" />
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
    <script src="/Scripts/jquery.quicksearch.js"></script>
    <script type="text/javascript">

    </script>
</asp:Content>
