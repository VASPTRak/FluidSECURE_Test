<%@ Page Title="Export Transactions" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ExportTransactions.aspx.vb" Inherits="Fuel_Secure.ExportTransactions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="UP_Main" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportTransactions" />
            <asp:PostBackTrigger ControlID="bttnExportTemplate" />
        </Triggers>
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Standard Export</asp:Label>
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
                                Date format:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList runat="server" ID="DDL_DateType" CssClass="form-control input-sm">
                               <asp:ListItem Text="mmddyyyy" Value="MMddyyyy" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="mmddyy" Value="MMddyy"></asp:ListItem>
                                <asp:ListItem Text="ddmmyyyy" Value="ddMMyyyy"></asp:ListItem>
                                <asp:ListItem Text="ddmmyy" Value="ddMMyy"></asp:ListItem>
                                <asp:ListItem Text="yyyymmdd" Value="yyyyMMdd"></asp:ListItem>
                                <asp:ListItem Text="yymmdd" Value="yyMMdd"></asp:ListItem>
                                <asp:ListItem Text="yyyyddmm" Value="yyyyddMM"></asp:ListItem>
                                <asp:ListItem Text="yyddmm" Value="yyddMM"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <%--   <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Is Missed:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList runat="server" ID="DDL_Missed" CssClass="form-control input-sm">
                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                            </asp:DropDownList>
                        </div>--%>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Status:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_TransactionStatus" runat="server" TabIndex="3" CssClass="form-control input-sm">
                            </asp:DropDownList>
                        </div>

                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="8" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Customer" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>File Type:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_ExportOption" runat="server" CssClass="form-control input-sm" TabIndex="4" AutoPostBack="true" OnSelectedIndexChanged="DDL_ExportOption_SelectedIndexChanged">
                                <asp:ListItem Text=".txt" Value="1" Selected="True"></asp:ListItem>
                                <asp:ListItem Text=".csv" Value="2"></asp:ListItem>
                                <asp:ListItem Text=".xls" Value="3"></asp:ListItem>
                                <asp:ListItem Text=".xlsx" Value="4"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFD_ExportOption" runat="server" ControlToValidate="DDL_ExportOption" Display="Dynamic" ErrorMessage="Please select File Type."
                                ForeColor="Red" SetFocusOnError="True" ValidationGroup="AutoTransactionExportSettingValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="seperatorDiv" runat="server" visible="false">
                            <label>Separator:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Separator" runat="server" CssClass="form-control input-sm" TabIndex="9" Visible="false">
                                <asp:ListItem Text="none" Value="none" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Comma" Value="comma"></asp:ListItem>
                                <asp:ListItem Text="*" Value="*"></asp:ListItem>
                                <asp:ListItem Text="|" Value="|"></asp:ListItem>
                                <asp:ListItem Text="~" Value="~"></asp:ListItem>
                                <asp:ListItem Text=";" Value=";"></asp:ListItem>
                                <asp:ListItem Text=":" Value=":"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Customized Export Template<label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_CustomizedExportTemplate" runat="server" CssClass="form-control input-sm" TabIndex="5"></asp:DropDownList>
                            <%--<asp:RequiredFieldValidator ID="RDF_CustomizedExportTemplate" runat="server" ControlToValidate="DDL_CustomizedExportTemplate" Display="Dynamic" ErrorMessage="Please select Customized Export Template."
                                ForeColor="Red" SetFocusOnError="True" ValidationGroup="AutoTransactionExportSettingValidation" InitialValue="0"></asp:RequiredFieldValidator>--%>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Name to the file:
                                  <label class="text-danger font-required">[required]</label></label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtFileName" runat="server" CssClass="form-control input-sm" TabIndex="10"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVFileName" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter File Name."
                                ControlToValidate="txtFileName" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator></td>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Add Decimal:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="ddl_DecimalQTY" runat="server" CssClass="form-control input-sm" TabIndex="6" AutoPostBack="true" OnSelectedIndexChanged="ddl_DecimalQTY_SelectedIndexChanged">
                                <asp:ListItem Text="Yes" Value="1" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="NO" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div runat="server" id="divDecimailType" visible="false">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>Decimal Type:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:DropDownList ID="ddl_DecimalType" runat="server" CssClass="form-control input-sm" TabIndex="6">
                                    <asp:ListItem Text="Keep Tenths" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Keep Hundredths" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnExportTransactions" CssClass="btn btn-primary" runat="server" OnClick="btnExportTransactions_Click" Text="Export Transactions"
                            UseSubmitBehavior="False" TabIndex="11" ValidationGroup="TransactionValidation" />
                        <asp:Button ID="bttnExportTemplate" CssClass="btn btn-primary" runat="server" OnClick="bttnExportTemplate_Click" Text="Export Template"
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
