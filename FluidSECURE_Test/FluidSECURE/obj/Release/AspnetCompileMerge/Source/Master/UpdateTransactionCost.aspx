<%@ Page Title="Update Transaction Cost" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="UpdateTransactionCost.aspx.vb" Inherits="Fuel_Secure.UpdateTransactionCost" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading">
					<h3 class="panel-title text-center">Update Transaction Cost</h3>
				</div>
				<div class="row col-md-12 col-sm-12 col-xs-12">
					<p class="text-center">
						<asp:Label runat="server" ID="message" class="text-center green"></asp:Label>
					</p>
					<p class="text-center">
						<asp:Label runat="server" ID="ErrorMessage" class="text-center red"></asp:Label>
					</p>
				</div>
				<asp:HiddenField ID="HDF_FuelTypeId" runat="server"></asp:HiddenField>
				<div class="panel-body">
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Product Name:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<label runat="server" id="lblProductName" style="font-weight: bold">
							</label>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Reset price
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:TextBox ID="txt_Price" runat="server" CssClass="form-control input-sm col-md-2 col-sm-2" Style="width: 500px; float: left; margin-right: 10px;" TabIndex="1"></asp:TextBox>
							<asp:RegularExpressionValidator ID="regexpPrice" runat="server" Display="Dynamic" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="FuelValidation"
								ErrorMessage="You are allow to enter till 3 decimal places. [eg. xxx.xxx]"
								ControlToValidate="txt_Price"
								ValidationExpression="^[1-9]\d{0,9}(\.\d{0,3})*(,\d+)?$" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Select Tank Number:
							</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="ddl_TankNo" CssClass="form-control input-sm" TabIndex="2" runat="server"></asp:DropDownList>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Start date
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control input-sm col-md-2 col-sm-2" Style="width: 100px; float: left; margin-right: 10px;" TabIndex="3"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Start Time 
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtStartTime" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RDF_StartTime" runat="server" ControlToValidate="txtStartTime" Display="Dynamic"
								ErrorMessage="Please select Start Time ." ForeColor="Red" SetFocusOnError="True" ValidationGroup="FuelValidation"></asp:RequiredFieldValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								End Date
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control input-sm col-md-2 col-sm-2" Style="width: 100px; float: left; margin-right: 10px;" TabIndex="5"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								End Time 
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtEndTime" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="6"></asp:TextBox>
							<asp:RequiredFieldValidator ID="RDF_EndTime" runat="server" ControlToValidate="txtEndTime" Display="Dynamic"
								ErrorMessage="Please select End Time ." ForeColor="Red" SetFocusOnError="True" ValidationGroup="FuelValidation"></asp:RequiredFieldValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-5 col-sm-5 textright col-xs-12">
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:Button runat="server" Text="Post Price" class="btn btn-success" ID="btnPostPrice" OnClick="btnPostPrice_Click" TabIndex="7" />
							<asp:Button runat="server" Text="History" class="btn btn-default" ID="btnHistory" OnClick="btnHistory_Click" TabIndex="8" />
							<asp:Button runat="server" Text="Cancel" class="btn btn-default" ID="btnCancel" OnClick="btnCancel_Click" TabIndex="9" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
						</div>
						<div class="form-group col-md-1 col-sm-1  col-xs-12">
						</div>
					</div>
				</div>
			</div>
			<div class="modal modal-lg fade" tabindex="-1" role="dialog" id="ModalHistory">
				<div class="modal-dialog modal-lg" style="max-width: 100%; width: 1000px;">
					<div class="modal-content modal-lg">
						<div class="modal-header green">
							<h2 class="modal-title text-center">Product Price History</h2>
						</div>
						<div class="modal-body">
							<div class="row col-md-12 col-sm-12 text-center" style="overflow-y: auto;">
								<asp:UpdatePanel ID="UP_Sites" runat="server">
									<ContentTemplate>
										<asp:GridView ID="gv_History" CssClass="table table-bordered" runat="server" DataKeyNames="PriRepostID" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
											<Columns>
												<asp:BoundField DataField="email" HeaderText="Use Name" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="ResetPrice" HeaderText="Price Posted" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="FromDate" HeaderText="From Date" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="ToDate" HeaderText="To Date" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="DateAdded" HeaderText="Date Added" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="FuelType" HeaderText="Product" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="Tank" HeaderText="Tanks" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="CostingType" HeaderText="Costing Type" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
											</Columns>
										</asp:GridView>

									</ContentTemplate>
								</asp:UpdatePanel>
							</div>
						</div>
						<div class="modal-footer nextButton">
							<asp:Button ID="btnCancelVehicle" runat="server" CssClass="btn btn-default" Text="Exit" data-dismiss="modal" value="No" />
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
		</ContentTemplate>
	</asp:UpdatePanel>




	<script type="text/javascript">

		function LoadDateTimeControl() {

			$("[id$=txtStartDate]").datepicker({
				showOn: 'button',
				buttonImageOnly: true,
				buttonImage: '/Content/images/calendar.png',
				changeMonth: true,
				changeYear: true,
				yearRange: "-100:+0",
				maxDate: 0
			});

			$("[id$=txtEndDate]").datepicker({
				showOn: 'button',
				buttonImageOnly: true,
				buttonImage: '/Content/images/calendar.png',
				changeMonth: true,
				changeYear: true,
				yearRange: "-100:+0",
				maxDate: 0
			});

			$('[id$=txtStartTime]').timepicker({
				timeFormat: 'h:mm p',
				interval: 01,
				dynamic: false,
				dropdown: true,
				scrollbar: true

			});

			$('[id$=txtEndTime]').timepicker({
				timeFormat: 'h:mm p',
				interval: 01,
				dynamic: false,
				dropdown: true,
				scrollbar: true

			});
		}

		function KeyPressProduct(e) {
			var keyCode = e.which ? e.which : e.keyCode;
			var ret = ((keyCode < 48 || keyCode > 57));

			if (ret) {
				ret = (keyCode == 46);
				if (ret)
					return true
				else
					return false
			}
			else
				return true;
		}

		function ClosePopUpFuel() {
			$("#btnCloseFuel").click();
			$('body').removeClass("modal-open");
			LoadDateTimeControl();
		}

		function OpenHistory() {
			$('#ModalHistory').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}

	</script>
</asp:Content>
