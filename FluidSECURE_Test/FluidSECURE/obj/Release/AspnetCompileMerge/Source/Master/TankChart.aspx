<%@ Page Title="Tank Chart" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="TankChart.aspx.vb" Inherits="Fuel_Secure.TankChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<style>
		.EntryType label {
			padding-left: 5px;
			/*padding-right: 5px;*/
			vertical-align: middle;
			/*font-size: 12px;*/
		}
	</style>

	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading  text-center">
					<asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
				</div>
				<div class="panel-body">
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center green" id="message" runat="server"></p>
						<p class="text-center red" id="ErrorMessage" runat="server"></p>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Tank Chart Number
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtTankChartNumber" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="10" Width="100"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RDFTankChartNumber" runat="server" ControlToValidate="txtTankChartNumber" ErrorMessage="Please Enter Tank Chart Number."
								Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TankChartValidation"></asp:RequiredFieldValidator>
							<asp:CompareValidator ID="CV_TankChartNumber" runat="server" ControlToValidate="txtTankChartNumber" ErrorMessage="Tank chart number should be integer number"
								ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="TankChartValidation" Display="Dynamic"></asp:CompareValidator>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Tank Size
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtTankSize" CssClass="form-control input-sm" TabIndex="5" runat="server" MaxLength="20" Width="80" Style="float: left; margin-right: 10px;"></asp:TextBox>
							<label style="float: left; line-height: 30px;">Inches</label>
							<asp:RequiredFieldValidator ID="RDFTankSize" runat="server" ControlToValidate="txtTankSize" Style="float: left; width: 100%;"
								ErrorMessage="Please Enter Tank Size." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TankChartValidation"></asp:RequiredFieldValidator>
							<asp:CompareValidator ID="CV_TankSize" runat="server" ControlToValidate="txtTankSize" ErrorMessage="Tank Size should be decimal number"
								ForeColor="Red" Operator="DataTypeCheck" Type="Double" ValidationGroup="TankChartValidation" Display="Dynamic"></asp:CompareValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Tank Chart Name
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtTankChartName" CssClass="form-control input-sm" TabIndex="2" runat="server" MaxLength="20" Width="200"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RFDTankChartName" runat="server" ControlToValidate="txtTankChartName"
								ErrorMessage="Please Enter Tank Chart Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TankChartValidation"></asp:RequiredFieldValidator>
							<asp:HiddenField ID="HDF_TankChartId" runat="server"></asp:HiddenField>
							<asp:HiddenField ID="HDF_TotalTankChart" runat="server" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Fuel Increment
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtFuelIncrement" CssClass="form-control input-sm" TabIndex="6" runat="server" Width="70" MaxLength="5"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RFDFuelIncrement" runat="server" ControlToValidate="txtFuelIncrement"
								ErrorMessage="Please Enter Fuel Increment." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TankChartValidation"></asp:RequiredFieldValidator>
							<asp:CompareValidator ID="CVFuelIncrement" runat="server" ControlToValidate="txtFuelIncrement" ErrorMessage="Fuel increment should be integer number"
								ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="TankChartValidation" Display="Dynamic"></asp:CompareValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Description :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtDescription" CssClass="form-control input-sm" TabIndex="3" runat="server" TextMode="MultiLine" Rows="3" Width="200"></asp:TextBox>
						</div>
						<%--<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Product
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="ddlFuelType" runat="server" TabIndex="7" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFVFuelType" runat="server" ErrorMessage="Please select Product in Tank."
								ControlToValidate="ddlFuelType" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TankChartValidation"></asp:RequiredFieldValidator>
						</div>--%>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Entry :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:RadioButtonList ID="RBL_Entry" runat="server" RepeatDirection="Vertical" CssClass="EntryType" TabIndex="4">
								<asp:ListItem Text="Every 1 inch" Value="1" Selected="True" />
								<asp:ListItem Text="Every 1/2 inch" Value="2" />
								<asp:ListItem Text="Every 1/4 inch" Value="3" />
							</asp:RadioButtonList>
						</div>
						<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Company
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Customer" runat="server" TabIndex="8" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TankChartValidation"></asp:RequiredFieldValidator>
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 text-center col-xs-12">
						<asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" Text="Save" Width="100px"
							UseSubmitBehavior="False" TabIndex="9" ValidationGroup="TankChartValidation" OnClick="btnSave_Click" />
						<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
							UseSubmitBehavior="False" TabIndex="10" />
						<asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
							UseSubmitBehavior="false" TabIndex="11" ValidationGroup="TankChartValidation" OnClick="btnSaveAndAddNew_Click" />
						<asp:Button ID="btn_ViewChart" CssClass="btn" runat="server" Text="View Chart" Width="150px"
							UseSubmitBehavior="false" TabIndex="11" OnClick="btn_ViewChart_Click" />
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

</asp:Content>
