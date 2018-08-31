<%@ Page Title="View Collected Diagnostic Logs" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ViewCollectedDiagnosticLogs.aspx.vb" Inherits="Fuel_Secure.ViewCollectedDiagnosticLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:UpdatePanel ID="up_Main" runat="server">
		<Triggers>
			<asp:PostBackTrigger ControlID="gvDiagnosticLogs" />
		</Triggers>
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading">
					<h3 class="panel-title text-center">View Collected Diagnostic Logs</h3>
				</div>
				<div class="panel-body">
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center green" id="message" runat="server"></p>
						<p class="text-center red" id="ErrorMessage" runat="server"></p>
					</div>
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-12 col-sm-12  col-xs-12 text-right">
							<asp:Button ID="btnDeleteALLConfirm" CssClass="btn btn-primary" runat="server" Text="Delete All Log Files" TabIndex="22" OnClientClick="return ConfirmDelete(1)" />
							<asp:Button ID="btnDeleteSelectedLConfirm" CssClass="btn btn-warning" runat="server" Text="Delete Selected Log Files" TabIndex="23" OnClientClick="ConfirmDelete(0)" />
						</div>
					</div>
					<div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

						<asp:GridView ID="gvDiagnosticLogs" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true"
							DataKeyNames="PersonId,Path,FileName" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
							<Columns>
								<asp:TemplateField HeaderText="">
									<HeaderTemplate>
										<asp:CheckBox ID="chkall" runat="server" onclick="CheckAll(this)" />
									</HeaderTemplate>
									<ItemTemplate>
										<asp:CheckBox ID="CHK_Selectlog" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField>
									<ItemTemplate>
										<asp:LinkButton ID="linkDownload" runat="server" ForeColor="#428BCA" OnClick="linkDownload_Click">Download</asp:LinkButton>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<asp:TemplateField SortExpression="FileName" ItemStyle-HorizontalAlign="Center" HeaderText="File Name">
									<ItemTemplate>
										<asp:Label ID="lblFileName" Text='<%# DataBinder.Eval(Container.DataItem, "FileName")%>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="PersonName" ItemStyle-HorizontalAlign="Center" HeaderText="Person Name">
									<ItemTemplate>
										<asp:Label ID="lblPersonName" Text='<%# DataBinder.Eval(Container.DataItem, "PersonName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Email" ItemStyle-HorizontalAlign="Center" HeaderText="Email">
									<ItemTemplate>
										<asp:Label ID="lblEmail" Text='<%# DataBinder.Eval(Container.DataItem, "Email")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="IMEI" ItemStyle-HorizontalAlign="Center" HeaderText="IMEI">
									<ItemTemplate>
										<asp:Label ID="lblIMEI" Text='<%# DataBinder.Eval(Container.DataItem, "IMEI")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
							<HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" />
							<EmptyDataRowStyle Font-Bold="True" ForeColor="Red" BackColor="white" BorderColor="red"
								BorderStyle="Solid" BorderWidth="1px" />
						</asp:GridView>

					</div>
					<div class="row col-md-12 col-sm-12 text-center col-xs-12">
						<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
							UseSubmitBehavior="False" TabIndex="23" OnClientClick="window.location.href='/Master/AllPersonnel?Filter=Filter'" />
					</div>
				</div>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>
	<!--alert message popup-->
	<div class="modal fade" tabindex="-1" role="dialog" id="myModalDeletelog">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<h3 class="modal-title text-center">Delete log files</h3>
				</div>
				<div class="modal-body">
					<h4 id="lblMessage"></h4>
				</div>
				<div class="modal-footer nextButton">
					<button type="button" id="btnMyModalClose" class="btn btn-default" data-dismiss="modal" onclick="return false;">No</button>
					<asp:Button ID="btnDeleteALL" CssClass="btn btn-primary" runat="server" OnClick="btnDeleteALL_Click" Text="Yes" />
					<asp:Button ID="btnDeleteSelected" CssClass="btn btn-primary" runat="server" Text="Yes" OnClick="btnDeleteSelected_Click" />
				</div>
			</div>
			<!-- /.modal-content -->
		</div>
		<!-- /.modal-dialog -->
	</div>
	<!-- /.modal -->

	<script>
		function ConfirmDelete(isall) {
			if (isall == "1") {
				$("#lblMessage").text("Are you sure you want to delete all log files of this user?");
				$('#<%= btnDeleteALL.ClientID %>').show();
				$('#<%= btnDeleteSelected.ClientID %>').hide();
			}
			else if (isall == "0") {
				$("#lblMessage").text("Are you sure you want to delete selected log files of this user?");
				$('#<%= btnDeleteALL.ClientID %>').hide();
				$('#<%= btnDeleteSelected.ClientID %>').show();
			}

			$('#myModalDeletelog').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}
		function CheckAll(oCheckbox) {
			var GridView2 = document.getElementById("<%=gvDiagnosticLogs.ClientID %>");
			for (i = 1; i < GridView2.rows.length; i++) {
				GridView2.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = oCheckbox.checked;
			}
		}
	</script>
</asp:Content>
