﻿<%@ Page Title="Tank" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Tank.aspx.vb" Inherits="Fuel_Secure.Tank" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>

			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading  text-center">
					<asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
				</div>
				<div class="panel-body">
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center">
							<asp:Label runat="server" ID="message" class="text-center green"></asp:Label>
						</p>
						<p class="text-center">
							<asp:Label runat="server" ID="ErrorMessage" class="text-center red"></asp:Label>
						</p>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Tank Number:
                        <label class="text-danger font-required">[required]</label></label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtTankNo" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="10" Width="90"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RFDTankNo" runat="server" ControlToValidate="txtTankNo"
								ErrorMessage="Please Enter Tank Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TankValidation"></asp:RequiredFieldValidator>
							<asp:TextBox ID="txtTankIdHide" runat="server" Visible="False"></asp:TextBox>
							<asp:HiddenField ID="HDF_TotalTank" runat="server" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Tank Name:
                                  <label class="text-danger font-required">[required]</label></label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtTankName" runat="server" CssClass="form-control input-sm" MaxLength="25" TabIndex="7"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RFVtxtTank" runat="server" Font-Size="Small"
								Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter Tank Name."
								ControlToValidate="txtTankName" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TankValidation"></asp:RequiredFieldValidator></td>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Product
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="ddlFuelType" runat="server" TabIndex="2" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFVFuelType" runat="server" ErrorMessage="Please select Product in Tank."
								ControlToValidate="ddlFuelType" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TankValidation"></asp:RequiredFieldValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Tank Address:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtAddress" runat="server" CssClass="form-control input-sm" MaxLength="25" TabIndex="3" TextMode="MultiLine"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Export Code:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtExportCode" runat="server" CssClass="form-control input-sm" MaxLength="25" TabIndex="8"></asp:TextBox>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Refill Notice:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtRefillNotice" runat="server" CssClass="form-control input-sm" MaxLength="7" TabIndex="4" Width="90"></asp:TextBox>
							<asp:CompareValidator ID="CV_RefillNotice" runat="server" Display="Dynamic" ErrorMessage="Please enter in integer format." ForeColor="Red"
								Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="TankValidation" ControlToValidate="txtRefillNotice"></asp:CompareValidator>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								PROBE Mac Address:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPROBEMacAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="9"></asp:TextBox>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Tank Monitor:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="Chk_TankMonitor" runat="server" TabIndex="5" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Tank Monitor Number:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtTankMonitorNo" runat="server" CssClass="form-control input-sm" MaxLength="3" TabIndex="5" Width="50" onkeypress="return onlyNumbers(event);"></asp:TextBox>
							<asp:CompareValidator ID="CVTankMonitorNumber" runat="server" ControlToValidate="txtTankMonitorNo" Display="Dynamic" ErrorMessage="Tank Monitor Number should be integer number" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="SiteValidation"></asp:CompareValidator>
						</div>
						<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Company:
                        <label class="text-danger font-required">[required]</label></label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Customer" runat="server" TabIndex="11" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TankValidation"></asp:RequiredFieldValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Tank Chart
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="ddlTankChart" runat="server" TabIndex="6" CssClass="form-control input-sm"></asp:DropDownList>
							<%--<asp:RequiredFieldValidator ID="RFVTankChart" runat="server" ErrorMessage="Please select Tank Chart."
								ControlToValidate="ddlTankChart" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TankValidation"></asp:RequiredFieldValidator>--%>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 text-center col-xs-12">
						<asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
							UseSubmitBehavior="true" TabIndex="12" ValidationGroup="TankValidation" />
						<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
							UseSubmitBehavior="False" TabIndex="13" OnClientClick="window.location.href='/Master/AllTanks?Filter=Filter'" />
						<asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
							UseSubmitBehavior="true" TabIndex="14" ValidationGroup="TankValidation" OnClick="btnSaveAndAddNew_Click" />
					</div>
					<div class="row col-md-12 col-sm-12 text-center clear col-xs-12" style="margin: 10px 0">
						<asp:Button ID="btnFirst" runat="server" Text="|<" CssClass="NewDept_ButtonFooter"
							OnClick="First_Click" /><asp:Button ID="btnprevious" runat="server" Text="<" CssClass="NewDept_ButtonFooter" OnClick="btnprevious_Click" />
						<asp:Label ID="lblof" runat="server" Text="Label" BorderColor="Black" BorderStyle="Solid"
							BorderWidth="1px" Font-Bold="True" Font-Names="arial" Font-Size="Small" Width="115px"></asp:Label>
						<asp:Button ID="btnNext" runat="server" Text=">" CssClass="NewDept_ButtonFooter" OnClick="btnNext_Click" /><asp:Button
							ID="btnLast" runat="server" Text=">|" CssClass="NewDept_ButtonFooter" OnClick="btnLast_Click" />
					</div>
				</div>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>

	<script>

</script>

</asp:Content>
