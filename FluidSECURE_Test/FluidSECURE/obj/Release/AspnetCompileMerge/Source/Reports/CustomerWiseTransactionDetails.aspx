﻿<%@ Page Title="Customer Wise Transaction Details" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CustomerWiseTransactionDetails.aspx.vb" Inherits="Fuel_Secure.CustomerWiseTransactionDetails" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Customer Wise Transaction Details</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                    <%--<div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Date From:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionDateFrom" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Date To:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionDateTo" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="3"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Time From:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionTimeFrom" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="2"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Time To:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionTimeTo" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
                        </div>
                    </div>--%>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="1" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                FluidSecure Link:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <%-- <asp:DropDownList ID="DDL_Site" runat="server" TabIndex="5" CssClass="form-control input-sm"></asp:DropDownList>--%>
                            <asp:ListBox ID="lstSites" runat="server" SelectionMode="Multiple" TabIndex="2" CssClass="form-control input-sm"></asp:ListBox>
                        </div>

                        <%--<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Status:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_TransactionStatus" runat="server" TabIndex="9" CssClass="form-control input-sm">
                            </asp:DropDownList>
                        </div>--%>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnGenarateReport" CssClass="btn btn-primary" runat="server" OnClick="btnGenarateReport_Click" Text="Generate Report"
                            UseSubmitBehavior="False" TabIndex="14" ValidationGroup="TransactionValidation" />
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

        //function LoadDateTimeControl() {
        //    $("[id$=txtTransactionDateFrom]").datepicker({
        //        showOn: 'button',
        //        buttonImageOnly: true,
        //        buttonImage: '/Content/images/calendar.png',
        //        changeMonth: true,
        //        changeYear: true,
        //        yearRange: "-100:+0",
        //        maxDate: 0
        //    });

        //    $("[id$=txtTransactionDateTo]").datepicker({
        //        showOn: 'button',
        //        buttonImageOnly: true,
        //        buttonImage: '/Content/images/calendar.png',
        //        changeMonth: true,
        //        changeYear: true,
        //        yearRange: "-100:+0",
        //        maxDate: 0
        //    });


        //    $('[id$=txtTransactionTimeFrom]').timepicker({
        //        timeFormat: 'h:mm p',
        //        interval: 01,
        //        dynamic: false,
        //        dropdown: true,
        //        scrollbar: true

        //    });


        //    $('[id$=txtTransactionTimeTo]').timepicker({
        //        timeFormat: 'h:mm p',
        //        interval: 01,
        //        dynamic: false,
        //        dropdown: true,
        //        scrollbar: true

        //    });


        //}

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

        function showWait() {
            $("#mydiv").show();
        }
        function hideWait() {
            $("#mydiv").hide();
        }
    </script>
</asp:Content>
