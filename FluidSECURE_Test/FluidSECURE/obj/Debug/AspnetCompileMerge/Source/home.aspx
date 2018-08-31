<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="home.aspx.vb" Inherits="Fuel_Secure._Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<style>
		th, td, .DashboardLabels {
			text-align: center;
			font-family: 'Open Sans';
			font-weight: bold;
			font-size: 14pt;
			vertical-align: top !important;
		}
	</style>

	<div class="dashboard-header">
		<h2 class="header-text" style="background-image: url(Content/images/header-background.png)">FluidSecure Cloud Dashboard</h2>
	</div>
	
	<asp:UpdatePanel runat="server" ID="UP_Main">
		<ContentTemplate>
			<h3 class="select-company" id="CompanyH3" runat="server"><%= Fuel_Secure.My.Resources.Resource.Company %></h3>
			<div>

				<div class="form-group company-form" style="padding-left: 0px">
					<asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="18" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged" onchange="setvalue()"></asp:DropDownList>
					<asp:RequiredFieldValidator ID="RFD_Customer" runat="server" Font-Size="Small"
						Font-Bold="False" Font-Names="arial" ErrorMessage="Please select company."
						ControlToValidate="DDL_Customer" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
					<h3 class="select-company" runat="server" id="LBL_Company"></h3>
					<asp:HiddenField ID="HDF_CurrentDate" runat="server" />
				</div>

			</div>
			<div class="row col-md-12 col-sm-12 col-xs-12">
				<p class="text-center red" id="ErrorMessage" runat="server"></p>
			</div>


			<div class="home-column">
				<img src="Content/images/home-icon-1.png" />
				<p class="home-labels">Total Quantity Dispensed Today</p>
				<p class="home-quantities">
					<asp:Label ID="LBL_DispensedToday" runat="server"></asp:Label>
				</p>
			</div>

			<div class="home-column">
				<img src="Content/images/home-icon-2.png" />
				<p class="home-labels">Number of Vehicles Using System Today</p>
				<p class="home-quantities">
					<asp:Label ID="LBL_vehiclesFueledToday" runat="server"></asp:Label>
				</p>
			</div>

			<div class="home-column">
				<img src="Content/images/home-icon-3.png" />
				<p class="home-labels">Average Quantity per Vehicle</p>
				<p class="home-quantities">
					<asp:Label ID="LBL_AverageAmountOffueledPerVehicle" runat="server"></asp:Label>
				</p>
			</div>

			<div class="home-column last">
				<img src="Content/images/home-icon-4.png" />
				<p class="home-labels">Total Quantity Dispensed for Current Month</p>
				<p class="home-quantities">
					<asp:Label ID="LBL_DispensedCurrentMonth" runat="server"></asp:Label>
				</p>
			</div>
			<div></div>



		</ContentTemplate>
	</asp:UpdatePanel>


	<div id="support-section">
		<h3>Contact and Support</h3>
		<p><a href="https://www.fluidsecure.com" target="_blank">www.FluidSecure.com</a></p>
		<p>Contact:<a href="mailto:info@fluidsecure.com" target="_blank"> info@fluidsecure.com</a></p>
		<p>Support: Monday through Friday  from 8:00 AM - 6:00 PM EST.</p>
		<p>850-878-4585, select support #1,</p>
		<p>After hours and holidays: 850-878-4585, select #6</p>
	</div>
	<script type="text/javascript">

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


