<%@ Page Title="Deleted Transactions" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllDeletedTransactions.aspx.vb" Inherits="Fuel_Secure.AllDeletedTransactions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<%--<script>

		function DeleteRecord() {
			$('#btnMyModalClose').click();

			var TransactionId = $("#hdnTransactionId").val();

			$.ajax({
				type: "POST",
				url: "AllTransactions.aspx/DeleteRecord",
				data: '{TransactionId: "' + TransactionId + '" }',
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

				window.location.href = "/Master/AllTransactions"

			}
			else if (response.d == -2) {
			}
			else {
				$("#ErrorMessageNew").show();
				$("#messageNew").hide();

				$("#ErrorMessageNew").text("Records deletion failed.");
			}
		}

		function CheckConfirm(TransactionId) {
			var Role = '<%= Session("RoleName") %>';

			if (Role == "SuperAdmin") {
				$("#lblMessage").text("Are you sure you want to delete this transaction?");
			}
			else {
				$("#lblMessage").text("Are you sure you want to delete this transaction?");
			}

			$("#hdnTransactionId").val(TransactionId);

			$('#myModalSuccess').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}
	</script>--%>
    <asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading">
					<h3 class="panel-title text-center">Deleted Transactions</h3>
				</div>
				<div class="panel-body">
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-3 col-sm-3 hidden-xs" id="OtherThanTransDate" runat="server"></div>
						<div class="form-group col-md-2 col-sm-3  col-xs-12">
							<asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_ColumnName" AutoPostBack="True"></asp:DropDownList>
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12" id="OtherThanTransDate1" runat="server">
							<asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
							<asp:DropDownList runat="server" ID="DDL_Customer" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:DropDownList runat="server" ID="DDL_Hub" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:DropDownList runat="server" ID="DDL_Fuel" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
						     <asp:DropDownList runat="server" ID="DDL_WifiSSId" data-placeholder="Choose FluidSecure Link..." Visible="false" CssClass="form-control input-sm"> </asp:DropDownList>
                            <asp:DropDownList runat="server" ID="DDL_Missed" Visible="false" CssClass="form-control input-sm">
                            </asp:DropDownList>
						</div>
						<div class="row col-md-8 col-sm-12 col-xs-12" id="TransDate" runat="server">
							<div class="form-group col-md-3 col-sm-3 col-xs-12" style="padding: 0;">
								<label>
									Transaction Date From:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtTransactionDateFrom" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
							</div>

							<div class="form-group col-md-3 col-sm-3 col-xs-12" style="padding: 0;">
								<label>
									Transaction Date To:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtTransactionDateTo" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="2"></asp:TextBox>
							</div>
						</div>
						<div class="form-group col-md-2 col-sm-3  col-xs-12">
							<asp:Button ID="btnSearch" CssClass="btn btn-primary" runat="server" Text="Search" OnClick="btnSearch_Click" TabIndex="11" />
						</div>
						<div class="form-group col-md-1 col-sm-3 hidden-xs" id="OtherThanTransDate2" runat="server"></div>
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
					<%--<div class="row col-md-12 col-sm-12 col-xs-12 text-right" style="margin-bottom: 10px;">
						<asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new Transaction" OnClick="btn_New_Click" />
					</div>--%>
                    <div class="row col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 10px;margin-left: 5px">
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-left">
                            <b><asp:Label runat="server" ID="lblTotalNumberOfRecords"></asp:Label></b>
                        </div>
                    </div>
					<div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

						<asp:GridView ID="gvTransactions" CssClass="table table-bordered table-hover" runat="server" PageSize="20" AllowPaging="true"
							DataKeyNames="TransactionId,TransactionStatus,TransactionStatusText" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria" OnRowDataBound="gvTransactions_RowDataBound">
							<PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                            <Columns>
								<asp:TemplateField HeaderText="Edit">
									<ItemTemplate>
										<asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<%--<asp:TemplateField HeaderText="Delete">
									<ItemTemplate>
										<a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "TransactionId")%>)">Delete</a>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>--%>
								<asp:TemplateField SortExpression="TransactionNumber" ItemStyle-HorizontalAlign="Left" HeaderText="Transaction #">
									<ItemTemplate>
										<asp:Label ID="lblTransactionNumber" Text='<%# DataBinder.Eval(Container.DataItem, "TransactionNumber")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="TransactionDateTime" ItemStyle-HorizontalAlign="Center" HeaderText="Transaction Date & Time">
									<ItemTemplate>
										<asp:Label ID="lblTransactionDateTime" Text='<%# DataBinder.Eval(Container.DataItem, "TransactionDateTime")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Pulses" ItemStyle-HorizontalAlign="Left" HeaderText="Pulses">
									<ItemTemplate>
										<asp:Label ID="lblPulses" Text='<%# DataBinder.Eval(Container.DataItem, "Pulses")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="FuelQuantity" ItemStyle-HorizontalAlign="Left" HeaderText="Fluid Quantity">
									<ItemTemplate>
										<asp:Label ID="lblFuelQuantity" Text='<%# DataBinder.Eval(Container.DataItem, "FuelQuantity")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="FuelType" ItemStyle-HorizontalAlign="Center" HeaderText="Product">
									<ItemTemplate>
										<asp:Label ID="lblFuelType" Text='<%# DataBinder.Eval(Container.DataItem, "FuelType")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<%--<asp:TemplateField SortExpression="VehicleName" ItemStyle-HorizontalAlign="Center" HeaderText="Vehicle Name">
									<ItemTemplate>
										<asp:Label ID="lblVehicleName" Text='<%# DataBinder.Eval(Container.DataItem, "VehicleName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>--%>
								<asp:TemplateField SortExpression="GuestVehicleNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Vehicle Number">
									<ItemTemplate>
										<asp:Label ID="lblGuestVehicleNumber" Text='<%# DataBinder.Eval(Container.DataItem, "GuestVehicleNumber")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="PersonName" ItemStyle-HorizontalAlign="Left" HeaderText="Person Name">
									<ItemTemplate>
										<asp:Label ID="lblPersonName" Text='<%# DataBinder.Eval(Container.DataItem, "PersonName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="WifiSSId" ItemStyle-HorizontalAlign="Left" HeaderText="FluidSecure Link">
									<ItemTemplate>
										<asp:Label ID="lblWifiSSId" Text='<%# DataBinder.Eval(Container.DataItem, "WifiSSId")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="CurrentOdometer" ItemStyle-HorizontalAlign="Left"
									HeaderText="Current Odometer">
									<ItemTemplate>
										<asp:Label ID="lblCurrentOdometer" Text='<%# DataBinder.Eval(Container.DataItem, "CurrentOdometer")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="IsPreAuthTransaction" ItemStyle-HorizontalAlign="Left"
									HeaderText="Pre-Auth Transaction">
									<ItemTemplate>
										<asp:Label ID="lblIsPreAuthTransaction" Text='<%# DataBinder.Eval(Container.DataItem, "IsPreAuthTransaction")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<%--<asp:TemplateField SortExpression="Company" ItemStyle-HorizontalAlign="Left" HeaderText="Company">
									<ItemTemplate>
										<asp:Label ID="lblCompanyName" Text='<%# DataBinder.Eval(Container.DataItem, "Company")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="TransactionCost" ItemStyle-HorizontalAlign="Center" HeaderText="Transaction Cost">
									<ItemTemplate>
										<asp:Label ID="lblCurrentLat" Text='<%# DataBinder.Eval(Container.DataItem, "TransactionCost")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>--%>
                                <asp:TemplateField SortExpression="HubSiteName" ItemStyle-HorizontalAlign="Left" HeaderText="Site Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblHubSiteName" Text='<%# DataBinder.Eval(Container.DataItem, "HubSiteName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
								<asp:TemplateField SortExpression="TransactionStatusText" ItemStyle-HorizontalAlign="Center" HeaderText="Transaction Status">
									<ItemTemplate>
										<asp:Label ID="lblTransactionStatus" Text='<%# DataBinder.Eval(Container.DataItem, "TransactionStatusText")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
							<HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" />
							<EmptyDataRowStyle Font-Bold="True" ForeColor="Red" BackColor="white" BorderColor="red"
								BorderStyle="Solid" BorderWidth="1px" />
						</asp:GridView>

						<%-- <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "TransactionId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>--%>

						<%--<asp:TemplateField SortExpression="CurrentLat" ItemStyle-HorizontalAlign="Center" HeaderText="Current Latitude">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCurrentLat" Text='<%# DataBinder.Eval(Container.DataItem, "CurrentLat")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="CurrentLng" ItemStyle-HorizontalAlign="Center" HeaderText="Current Longitude">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCurrentLng" Text='<%# DataBinder.Eval(Container.DataItem, "CurrentLng")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="PhoneNumber" ItemStyle-HorizontalAlign="Left" HeaderText="Phone Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPhoneNumber" Text='<%# DataBinder.Eval(Container.DataItem, "PhoneNumber")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
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
					<input type="hidden" id="hdnTransactionId" />
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

	<script src="/Scripts/jquery-migrate-1.2.1.js"></script>
	<script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
	<link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
	<script src="/Scripts/jquery.timepicker.min.js"></script>
	<link rel="stylesheet" href="/Content/jquery.timepicker.min.css">
	<script type="text/javascript">
	
		function LoadDateTimeControl() {
			$("[id$=txtTransactionDateFrom]").datepicker({
				showOn: 'button',
				buttonImageOnly: true,
				buttonImage: '/Content/images/calendar.png',
				changeMonth: true,
				changeYear: true,
				yearRange: "-100:+0",
				maxDate: 0
			});

			$("[id$=txtTransactionDateTo]").datepicker({
				showOn: 'button',
				buttonImageOnly: true,
				buttonImage: '/Content/images/calendar.png',
				changeMonth: true,
				changeYear: true,
				yearRange: "-100:+0",
				maxDate: 0
			});
		}
	</script>

</asp:Content>
