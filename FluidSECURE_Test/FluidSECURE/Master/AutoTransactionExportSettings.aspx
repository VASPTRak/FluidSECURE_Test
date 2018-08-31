<%@ Page Title="Automatic Export Settings" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AutoTransactionExportSettings.aspx.vb" Inherits="Fuel_Secure.AutoTransactionExportSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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
						<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Company
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Company" runat="server" CssClass="form-control input-sm" TabIndex="1" OnSelectedIndexChanged="DDL_Company_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Company" Display="Dynamic" ErrorMessage="Please select company."
								ForeColor="Red" SetFocusOnError="True" ValidationGroup="AutoTransactionExportSettingValidation" InitialValue="0"></asp:RequiredFieldValidator>

							<asp:HiddenField ID="HDF_AutoTransactionExportSettingId" runat="server"></asp:HiddenField>
							<asp:HiddenField ID="HDF_TotalAutoTransactionExportSettings" runat="server" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								File Type
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_ExportOption" runat="server" CssClass="form-control input-sm" TabIndex="8" AutoPostBack="true" OnSelectedIndexChanged="DDL_ExportOption_SelectedIndexChanged">
								<asp:ListItem Text=".txt" Value="1"></asp:ListItem>
								<asp:ListItem Text=".csv" Value="2"></asp:ListItem>
								<asp:ListItem Text=".xls" Value="3"></asp:ListItem>
								<asp:ListItem Text=".xlsx" Value="4"></asp:ListItem>
							</asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFD_ExportOption" runat="server" ControlToValidate="DDL_ExportOption" Display="Dynamic" ErrorMessage="Please select Export Option."
								ForeColor="Red" SetFocusOnError="True" ValidationGroup="AutoTransactionExportSettingValidation"></asp:RequiredFieldValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Template Name
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_CustomizedExportTemplate" runat="server" CssClass="form-control input-sm" TabIndex="2"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RDF_CustomizedExportTemplate" runat="server" ControlToValidate="DDL_CustomizedExportTemplate" Display="Dynamic" ErrorMessage="Please select Customized Export Template."
								ForeColor="Red" SetFocusOnError="True" ValidationGroup="AutoTransactionExportSettingValidation" InitialValue="0"></asp:RequiredFieldValidator>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="seperatorDiv" runat="server" visible="false">
							<label>
								Separator
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Separator" runat="server" CssClass="form-control input-sm" TabIndex="9" Visible="false">
								<asp:ListItem Text="none" Value="none"></asp:ListItem>
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
							<label>FTP Server Path :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="TXT_FTPServerPath" runat="server" CssClass="form-control input-sm" TabIndex="3"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Active :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="CHK_Active" runat="server" Checked="true" TabIndex="10" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>FTP Username :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtFTPUsername" runat="server" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Email :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtEmail" runat="server" CssClass="form-control input-sm" TabIndex="11"></asp:TextBox>
							<asp:RegularExpressionValidator ID="RDFEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Please enter valid email."
								ForeColor="Red" SetFocusOnError="True" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="AutoTransactionExportSettingValidation"></asp:RegularExpressionValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>FTP Password :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtFTPPassword" runat="server" CssClass="form-control input-sm" TabIndex="5"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Execution Time 
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtExecutionTime" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="12"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RDF_TransTime" runat="server" ControlToValidate="txtExecutionTime" Display="Dynamic"
								ErrorMessage="Please select Execution Time ." ForeColor="Red" SetFocusOnError="True" ValidationGroup="AutoTransactionExportSettingValidation"></asp:RequiredFieldValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Time Zone
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_TimeZone" runat="server" TabIndex="6" CssClass="form-control input-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_TimeZone_SelectedIndexChanged"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFV_TimeZone" runat="server" ErrorMessage="Please select your time zone."
								ControlToValidate="DDL_TimeZone" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="AutoTransactionExportSettingValidation"></asp:RequiredFieldValidator>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Include Previously Export Transactions :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chk_IncludePreviouslyExportTransactions" runat="server" TabIndex="13" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Selected time zone :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:Label ID="LBL_TimeZone" runat="server" Style="font-weight: bold; font-size: 15px"></asp:Label>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Only export transactions that have not been previously exported :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chk_ExportOnlyNewTransactions" runat="server" TabIndex="14" />
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Export Zero Quantity Transactions :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chk_ExportZeroQtyTransactions" runat="server" TabIndex="15" />
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
						<asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
							UseSubmitBehavior="False" TabIndex="14" ValidationGroup="AutoTransactionExportSettingValidation" />
						<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
							UseSubmitBehavior="False" TabIndex="15" OnClick="btnCancel_Click" />
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

	<script src="/Scripts/jquery-migrate-1.2.1.js"></script>
	<script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
	<link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
	<script src="/Scripts/jquery.timepicker.min.js"></script>
	<link rel="stylesheet" href="/Content/jquery.timepicker.min.css">

	<script type="text/javascript">

		function LoadDateTimeControl() {

			$('[id$=txtExecutionTime]').timepicker({
				timeFormat: 'h:mm p',
				interval: 01,
				dynamic: false,
				dropdown: true,
				scrollbar: true

			});
		}

	</script>
</asp:Content>
