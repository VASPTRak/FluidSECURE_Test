<%@ Page Title="FluidSecure Hub Report" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="FluidSecureHubReport.aspx.vb" Inherits="Fuel_Secure.FluidSecureHubReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        
    <asp:UpdatePanel ID="UP_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">FluidSecure Hub Report</asp:Label>
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
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="1" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Customer" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonValidation"></asp:RequiredFieldValidator>
                        </div>

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Site:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Personnel" runat="server" TabIndex="2" CssClass="form-control input-sm"></asp:DropDownList>

                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnGenarateReport" CssClass="btn btn-primary" runat="server" OnClick="btnGenarateReport_Click" Text="Generate Report"
                            UseSubmitBehavior="False" TabIndex="3" ValidationGroup="PersonValidation" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
