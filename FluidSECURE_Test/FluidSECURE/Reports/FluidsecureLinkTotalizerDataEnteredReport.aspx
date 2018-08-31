<%@ Page Title="FluidSecure Link Totalizer Data Entered" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="FluidsecureLinkTotalizerDataEnteredReport.aspx.vb" Inherits="Fuel_Secure.FluidsecureLinkTotalizerDataEnteredReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row col-md-12 col-sm-12 col-xs-12" style="text-align: right;margin: 10px 0px;">
        <asp:Button ID="btnBack" CssClass="btn btn-primary" runat="server" OnClick="btnBack_Click" Text="Back" /> 
    </div>

    <rsweb:ReportViewer ID="RPT_FluidsecureLinkTotalizerDataEnteredDetails" runat="server"  CssClass="rptClass" Height="1000px" Width="100%" BackColor="#0762AC" Style="overflow: auto;"></rsweb:ReportViewer>

</asp:Content>
