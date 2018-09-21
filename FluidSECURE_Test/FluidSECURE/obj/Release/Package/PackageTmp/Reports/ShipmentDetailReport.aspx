<%@ Page Title="Shipment Detail Report" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ShipmentDetailReport.aspx.vb" Inherits="Fuel_Secure.ShipmentDetailReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:UpdatePanel ID="UP_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading  text-center">
					<asp:Label class="panel-title" ID="lblHeader" runat="server">Shipment Report</asp:Label>
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
							<asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:DropDownList>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Shipment details for:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_ShipmentFor" runat="server" TabIndex="2" CssClass="form-control input-sm">
								<asp:ListItem Text="FluidSecure Links" Value="1"></asp:ListItem>
								<asp:ListItem Text="FluidSecure Hubs" Value="2"></asp:ListItem>
							</asp:DropDownList>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Returned:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_returned" runat="server" TabIndex="3" CssClass="form-control input-sm">
								<asp:ListItem Text="Select all shipments" Value="1"></asp:ListItem>
								<asp:ListItem Text="Only Returned Shipments" Value="2"></asp:ListItem>
							</asp:DropDownList>
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 text-center col-xs-12">
						<asp:Button ID="btnGenarateReport" CssClass="btn btn-primary" runat="server" OnClick="btnGenarateReport_Click" Text="Generate Report"
							UseSubmitBehavior="False" TabIndex="4"/>
					</div>
				</div>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>

</asp:Content>
