<%@ Page Title="All Tank Charts" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllTankCharts.aspx.vb" Inherits="Fuel_Secure.AllTankCharts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<script>

		function DeleteRecord() {
			$('#btnMyModalClose').click();

			var TankChartId = $("#hdnTankChartId").val();

			$.ajax({
				type: "POST",
				url: "AllTankCharts.aspx/DeleteRecord",
				data: '{TankChartId: "' + TankChartId + '" }',
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: OnSuccess,
				failure: function (response) {
					alert(response.d);
				}
			});
		}

		function OnSuccess(response) {

			$("#myModalSuccess").hide();

			if (response.d == 1) {
				$("#messageNew").text("Records deleted Successfully.");
				$("#ErrorMessageNew").hide();
				$("#messageNew").show();

				window.location.href = "/Master/AllTankCharts"

			}
			//else if (response.d == -2) {
			//    $("#ErrorMessageNew").show();
			//    $("#messageNew").hide();

			//    $("#ErrorMessageNew").text("Transactions and hose found found for this Fluid, so you can not delete this Fuel.  Please delete Transactions and hose found for selected Fuel then try again.");
			//}
			else {
				$("#ErrorMessageNew").show();
				$("#messageNew").hide();

				$("#ErrorMessageNew").text("Records deletion failed.");
			}
		}

		function CheckConfirm(TankChartId) {
			var Role = '<%= Session("RoleName") %>';

			if (Role == "SuperAdmin") {
				$("#lblMessage").text("Are you sure you want to delete this Tank Chart?");
			}
			else {
				$("#lblMessage").text("Are you sure you want to delete this Tank Chart?");
			}

			$("#hdnTankChartId").val(TankChartId);

			$('#myModalSuccess').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}
	</script>
	<style>
		.table tr th {
			text-align: center;
		}
	</style>
	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading">
					<h3 class="panel-title text-center">Tank Charts</h3>
				</div>
				<div class="panel-body">
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-2 col-sm-3 hidden-xs"></div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_ColumnName" AutoPostBack="True"></asp:DropDownList>
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
							<asp:DropDownList runat="server" ID="DDL_Customer" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:Button ID="btnSearch" CssClass="btn btn-primary" runat="server" Text="Search" OnClick="btnSearch_Click" TabIndex="11" />
						</div>
						<div class="form-group col-md-1 col-sm-3 hidden-xs"></div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center text-danger">All records shown that match your criteria, click on Edit to see more fields</p>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center green" id="message" runat="server"></p>
						<p class="text-center red" id="ErrorMessage" runat="server"></p>
						<p class="text-center green" id="messageNew"></p>
						<p class="text-center red" id="ErrorMessageNew"></p>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12 text-right" style="margin-bottom: 10px;">
						<asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new Tank Chart" OnClick="btn_New_Click" />
					</div>
					<div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

						<asp:GridView ID="gvTankChart" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" DataKeyNames="TankChartId"
							AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
							<Columns>
								<asp:TemplateField HeaderText="Edit">
									<ItemTemplate>
										<asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Delete">
									<ItemTemplate>
										<a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "TankChartId")%>)">Delete</a>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<asp:TemplateField SortExpression="TankChartNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Tank Chart Number" HeaderStyle-HorizontalAlign="Center">
									<ItemTemplate>
										<asp:Label ID="lblTankChartNumber" Text='<%# DataBinder.Eval(Container.DataItem, "TankChartNumber")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="TankChartName" ItemStyle-HorizontalAlign="Center" HeaderText="Tank Chart Name" HeaderStyle-HorizontalAlign="Center">
									<ItemTemplate>
										<asp:Label ID="lblTankChart" Text='<%# DataBinder.Eval(Container.DataItem, "TankChartName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="CompanyName" ItemStyle-HorizontalAlign="Center" HeaderText="Company Name">
									<ItemTemplate>
										<asp:Label ID="lblCompanyName" Text='<%# DataBinder.Eval(Container.DataItem, "CompanyName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>

							<HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" HorizontalAlign="Center" />
							<EmptyDataRowStyle Font-Bold="True" ForeColor="Red" BackColor="white" BorderColor="red"
								BorderStyle="Solid" BorderWidth="1px" />
						</asp:GridView>


					</div>
				</div>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>

	<!--alert message popup-->
	<div class="modal fade" tabindex="-1" role="dialog" id="myModalSuccess">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<h3 class="modal-title text-center">FluidSecure</h3>
				</div>
				<div class="modal-body">
					<input type="hidden" id="hdnTankChartId" />
					<h4 id="lblMessage"></h4>
				</div>
				<div class="modal-footer nextButton">
					<button type="button" id="btnMyModalClose" class="btn btn-default" data-dismiss="modal" onclick="return false;">No</button>
					<input type="button" id="btnMyModalSuccess" class="btn btn-success" onclick="DeleteRecord()" value="Yes" />
				</div>
			</div>
			<!-- /.modal-content -->
		</div>
		<!-- /.modal-dialog -->
	</div>
	<!-- /.modal -->
</asp:Content>
