<%@ Page Title="Company Hosting Date Report" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CompanyHostingDateReport.aspx.vb" Inherits="Fuel_Secure.CompanyHostingDateReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

     <asp:UpdatePanel ID="UP_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Company Hosting Date Report</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                   <div runat="server" id="divDates">
                        <div class="row col-md-12 col-sm-12 col-xs-12" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>Beginning Hosting Date:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtBeginningHostingDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="7"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RDF_BeginningHostingDate" runat="server" ControlToValidate="txtBeginningHostingDate" Display="Dynamic" Style="float: left;"
                                    ErrorMessage="Please select Beginning Hosting Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>Ending Hosting Date:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtEndingHostingDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="15"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RDF_EndingHostingDate" runat="server" ControlToValidate="txtEndingHostingDate" Display="Dynamic" Style="float: left;"
                                    ErrorMessage="Please select Ending Hosting Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnGenarateReport" CssClass="btn btn-primary" runat="server" OnClick="btnGenarateReport_Click" Text="Generate Report"
                            UseSubmitBehavior="False" TabIndex="3" ValidationGroup="CustValidation" />
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

     <script>

        function LoadDateTimeControl() {

            $("[id$=txtBeginningHostingDate]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $("[id$=txtEndingHostingDate]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-10:+100",
               
            });

        }
         </script>
</asp:Content>
