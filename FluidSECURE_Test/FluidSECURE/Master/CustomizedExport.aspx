<%@ Page Title="Create Customized Export" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CustomizedExport.aspx.vb" Inherits="Fuel_Secure.CustomizedExport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<style>
		input[type="radio"] {
			width: 20px;
			height: 20px;
		}
	</style>
	<asp:UpdatePanel ID="up_Main" runat="server">
		<Triggers>
			<asp:PostBackTrigger ControlID="btnExportTemplate" />
		</Triggers>
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading  text-center">
					<asp:Label class="panel-title" ID="lblHeader" runat="server">Add Customized Export</asp:Label>
				</div>
				<div class="panel-body">
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center green" id="message" runat="server"></p>
						<p class="text-center green" id="message1" runat="server"></p>
						<p class="text-center red" id="ErrorMessage" runat="server"></p>
						<asp:HiddenField ID="hdfCustomizedExportTemplateId" runat="server" />
					</div>
				</div>
				<div class="row col-md-12 col-sm-12 col-xs-12">
					<div id="divCompany" runat="server" class="form-group col-md-6 col-sm-3 textright col-xs-12">
						<label>
							Company
                                <label class="text-danger font-required">[required]</label>:</label>
					</div>
					<div class="form-group col-md-2 col-sm-2 col-xs-12">
						<asp:DropDownList ID="DDL_Customer" runat="server" TabIndex="1" CssClass="form-control input-sm"></asp:DropDownList>
						<asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="ExportValidation"></asp:RequiredFieldValidator>
					</div>
				</div>
				<div class="row col-md-12 col-sm-12 col-xs-12">
					<div class="form-group col-md-6 col-sm-3 textright col-xs-12">
						<label>
							Template Name
                            <label class="text-danger font-required">[required]</label>:</label>
					</div>
					<div class="form-group col-md-2 col-sm-2 col-xs-12">
						<asp:TextBox ID="txtTemplateName" CssClass="form-control input-sm" TabIndex="2" runat="server"></asp:TextBox>
						<asp:RequiredFieldValidator ID="RFD_TemplateName" runat="server" ControlToValidate="txtTemplateName" Display="Dynamic" ErrorMessage="Please enter Template Name." ForeColor="Red" SetFocusOnError="True" ValidationGroup="ExportValidation"></asp:RequiredFieldValidator>
					</div>
				</div>
				<div class="row col-md-12 col-sm-12 col-xs-12">
					<div class="form-group col-md-3 col-sm-2 col-xs-12">
						<label class="text-center col-md-12" style="font-weight: bold">Menu</label>
						<asp:ListBox ID="lstColumns" runat="server" SelectionMode="Multiple" CssClass="form-control input-group" Style="height: 600px; overflow-y: auto; overflow-x: hidden;"></asp:ListBox>
					</div>
					<div class="form-group col-md-2 col-sm-2 col-xs-12 text-center" style="margin-top: 200px;">
						<asp:Button ID="btnMoveRight" CssClass="btn btn-primary" runat="server" Text="ADD" OnClick="btnMoveRight_Click" Width="100px" />
						<br />
						<br />
						<asp:Button ID="btnMoveLeft" CssClass="btn btn-primary" runat="server" Text="REMOVE" OnClick="btnMoveLeft_Click" Width="100px" />
						<br />
						<br />
						<asp:Button ID="btnMoveUp" CssClass="btn btn-primary" runat="server" Text="UP" OnClick="btnMoveUp_Click" Width="100px" />
						<br />
						<br />
						<asp:Button ID="btnMoveDown" CssClass="btn btn-primary" runat="server" Text="DOWN" OnClick="btnMoveDown_Click" Width="100px" />
						<br />
						<br />
						<asp:Button ID="btnClearSelection" CssClass="btn btn-primary" runat="server" Text="Clear Selection" OnClick="btnClearSelection_Click" Width="120px" />
					</div>
					<div class="form-group col-md-7 col-sm-8 col-xs-12" style="height: 600px; overflow-y: auto">
						<asp:GridView ID="gvExportColumns" CssClass="table table-bordered table-hover" OnRowDataBound="gvExportColumns_RowDataBound"
							runat="server" DataKeyNames="FieldId,Index,FieldName" AutoGenerateColumns="False">
							<Columns>
								<asp:TemplateField HeaderText="Select to reorder" HeaderStyle-Width="80">
									<ItemTemplate>
										<asp:RadioButton ID="RDB_UpDwonRow" runat="server" onclick="RadioCheck(this);" OnCheckedChanged="RDB_UpDwonRow_CheckedChanged" AutoPostBack="true" />
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Field Name">
									<ItemTemplate>
										<asp:Label ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "FieldName")%>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Field Length" ControlStyle-Width="100px">
									<ItemTemplate>
										<asp:TextBox ID="txtFieldLength" runat="server" CssClass="form-control input-sm" MaxLength="2" Style="width:40px !important" Text='<%# DataBinder.Eval(Container.DataItem, "FieldLength")%>'></asp:TextBox>
										<asp:RequiredFieldValidator ID="RFD_FieldLength" runat="server" ControlToValidate="txtFieldLength" Display="Dynamic" ErrorMessage="Please enter Field Length other than 0." ForeColor="Red"
											SetFocusOnError="True" ValidationGroup="ExportValidation" InitialValue="0"></asp:RequiredFieldValidator>
										<asp:RequiredFieldValidator ID="RFD_FieldLength1" runat="server" ControlToValidate="txtFieldLength" Display="Dynamic" ErrorMessage="Please enter Field Length." ForeColor="Red"
											SetFocusOnError="True" ValidationGroup="ExportValidation"></asp:RequiredFieldValidator>
										<asp:CompareValidator ID="CV_FieldLength" runat="server" Display="Dynamic" ErrorMessage="Please enter Field Length in integer format." ForeColor="Red"
											Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="VehicleValidation" ControlToValidate="txtFieldLength"></asp:CompareValidator>
										<asp:Label ID="LBL_FieldLenghtError" runat="server" Visible="false" ForeColor="Red"></asp:Label>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Fill">
									<ItemTemplate>
										<%--<asp:TextBox ID="txtPaddingCharacter" runat="server" CssClass="form-control input-sm" MaxLength="1" Width="30" Text='<%# DataBinder.Eval(Container.DataItem, "PaddingCharacter")%>'></asp:TextBox>--%>
										<asp:DropDownList ID="DDL_PaddingCharacter" runat="server" CssClass="form-control" SelectedValue='<%# Eval("PaddingCharacter") %>'>
											<asp:ListItem Text="None" Value=""></asp:ListItem>
											<asp:ListItem Text="0" Value="0"></asp:ListItem>
											<asp:ListItem Text="Space" Value=" "></asp:ListItem>
										</asp:DropDownList>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Justify">
									<ItemTemplate>
										<asp:DropDownList ID="DDL_justification" runat="server" CssClass="form-control" SelectedValue='<%# Eval("Justify") %>'>
											<asp:ListItem Text="None" Value="None"></asp:ListItem>
											<asp:ListItem Text="Left" Value="Left"></asp:ListItem>
											<asp:ListItem Text="Right" Value="Right"></asp:ListItem>
										</asp:DropDownList>
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
							<SelectedRowStyle BackColor="Red" ForeColor="White" />
						</asp:GridView>
					</div>
				</div>
				<div class="row col-md-12 col-sm-12 text-center clear col-xs-12" style="margin: 10px 0">
					<asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" Text="Save" Width="100px"
						UseSubmitBehavior="False" ValidationGroup="ExportValidation" OnClick="btnSave_Click" />
					<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
						UseSubmitBehavior="False" OnClick="btnCancel_Click" />
					<asp:Button ID="btnExportTemplate" CssClass="btn btn-primary" runat="server" Text="Export Template" Width="150px"
						UseSubmitBehavior="False" OnClick="btnExportTemplate_Click" />
				</div>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>
	<!--alert message popup-->
	<div class="modal fade" tabindex="-1" role="dialog" id="NoUpDown">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<h3 class="modal-title text-center">FluidSecure</h3>
				</div>
				<div class="modal-body">
					<h4 id="NoUpDownError"></h4>
				</div>
				<div class="modal-footer nextButton">
					<input type="button" id="btnClose" class="btn btn-success" data-dismiss="modal" value="Close" />
				</div>
			</div>
			<!-- /.modal-content -->
		</div>
		<!-- /.modal-dialog -->
	</div>
	<!-- /.modal -->

	<script type="text/javascript">

		function NoUpDown(NoUpDownError) {
			$("#NoUpDownError").text(NoUpDownError);
			$('#NoUpDown').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}


		function RadioCheck(rb) {

			var gv = document.getElementById("<%=gvExportColumns.ClientID%>");

			var rbs = gv.getElementsByTagName("input");

			var row = rb.parentNode.parentNode;

			for (var i = 0; i < rbs.length; i++) {

				if (rbs[i].type == "radio") {

					if (rbs[i].checked && rbs[i] != rb) {

						rbs[i].checked = false;

						break;

					}
				}

			}

		}

	</script>
</asp:Content>
