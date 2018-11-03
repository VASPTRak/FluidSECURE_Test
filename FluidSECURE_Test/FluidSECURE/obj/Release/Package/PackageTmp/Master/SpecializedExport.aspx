<%@ Page Title="Specialized Export: Hawaii Telecom" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="SpecializedExport.aspx.vb" Inherits="Fuel_Secure.SpecializedExport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="UP_Main" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportTransactions" />
        </Triggers>
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Specialized Export: Hawaii Telecom</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
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
                            <asp:TextBox ID="txtTransactionDateTo" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="6"></asp:TextBox>
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
                            <asp:TextBox ID="txtTransactionTimeTo" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="7"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Exclude Previously Exported Transactions:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox runat ="server" ID="chk_ExcludePreviouslyExportedTransactions" TabIndex="3"/>
                        </div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Name to the file:
                                  <label class="text-danger font-required">[required]</label></label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtFileName" runat="server" CssClass="form-control input-sm" TabIndex="8"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVFileName" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter File Name."
                                ControlToValidate="txtFileName" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator></td>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
						<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="5" AutoPostBack="true"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Customer" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnExportTransactions" CssClass="btn btn-primary" runat="server" OnClick="btnExportTransactions_Click" Text="Export Transactions"
                            UseSubmitBehavior="False" TabIndex="11" ValidationGroup="TransactionValidation" />
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
            $("[id$=txtTransactionDateFrom]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $("[id$=txtTransactionDateTo]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });


            $('[id$=txtTransactionTimeFrom]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

            });


            $('[id$=txtTransactionTimeTo]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

            });

        }
    </script>


</asp:Content>