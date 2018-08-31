<%@ Page Title="Delivery Veeder TankMonitor Readings" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllDeliveryVeederTankMonitorDetails.aspx.vb" Inherits="Fuel_Secure.AllDeliveryVeederTankMonitorDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	
	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading">
					<h3 class="panel-title text-center">Delivery Veeder TankMonitor Readings</h3>
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
					<%--  <div class="row col-md-12 col-sm-12 col-xs-12 text-right" style="margin-bottom: 10px;">
                        <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new Tank" OnClick="btn_New_Click" />
                    </div>--%>
					<div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

						<asp:GridView ID="gvDeliveryVeederTankMonitor" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
							<Columns>
								<%-- <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "TankId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>--%>
								<asp:TemplateField SortExpression="TankNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Tank Number">
									<ItemTemplate>
										<asp:Label ID="lblTankNumber" Text='<%# DataBinder.Eval(Container.DataItem, "TankNumber")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="PhoneNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Phone Number">
									<ItemTemplate>
										<asp:Label ID="lblPhoneNumber" Text='<%# DataBinder.Eval(Container.DataItem, "PhoneNumber")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="VeederRootMacAddress" ItemStyle-HorizontalAlign="Center" HeaderText="Veeder Root Mac Address">
									<ItemTemplate>
										<asp:Label ID="lblVeederRootMacAddress" Text='<%# DataBinder.Eval(Container.DataItem, "VeederRootMacAddress")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="AppDateTime" ItemStyle-HorizontalAlign="Center" HeaderText="App Date Time">
									<ItemTemplate>
										<asp:Label ID="lblAppDateTime" Text='<%# DataBinder.Eval(Container.DataItem, "AppDateTime")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="VRDateTime" ItemStyle-HorizontalAlign="Center" HeaderText="VR DateTime">
									<ItemTemplate>
										<asp:Label ID="lblVRDateTime" Text='<%# DataBinder.Eval(Container.DataItem, "VRDateTime")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="ProductCode" ItemStyle-HorizontalAlign="Center" HeaderText="Product Code">
									<ItemTemplate>
										<asp:Label ID="lblProductCode" Text='<%# DataBinder.Eval(Container.DataItem, "ProductCode")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="StartDateTime" ItemStyle-HorizontalAlign="Center" HeaderText="Start Date Time">
									<ItemTemplate>
										<asp:Label ID="lblStartDateTime" Text='<%# DataBinder.Eval(Container.DataItem, "StartDateTime")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="EndDateTime" ItemStyle-HorizontalAlign="Center" HeaderText="End Date Time">
									<ItemTemplate>
										<asp:Label ID="lblEndDateTime" Text='<%# DataBinder.Eval(Container.DataItem, "EndDateTime")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="StartVolume" ItemStyle-HorizontalAlign="Center" HeaderText="Start Volume">
									<ItemTemplate>
										<asp:Label ID="lblStartVolume" Text='<%# DataBinder.Eval(Container.DataItem, "StartVolume")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="StartTCVolume" ItemStyle-HorizontalAlign="Center" HeaderText="Start TC Volume">
									<ItemTemplate>
										<asp:Label ID="lblStartTCVolume" Text='<%# DataBinder.Eval(Container.DataItem, "StartTCVolume")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="StartWater" ItemStyle-HorizontalAlign="Center" HeaderText="Start Water">
									<ItemTemplate>
										<asp:Label ID="lbStartWater" Text='<%# DataBinder.Eval(Container.DataItem, "StartWater")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="StartTemp" ItemStyle-HorizontalAlign="Center" HeaderText="Start Temp">
									<ItemTemplate>
										<asp:Label ID="lblStartTemp" Text='<%# DataBinder.Eval(Container.DataItem, "StartTemp")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="EndVolume" ItemStyle-HorizontalAlign="Center" HeaderText="End Volume">
									<ItemTemplate>
										<asp:Label ID="lblEndVolume" Text='<%# DataBinder.Eval(Container.DataItem, "EndVolume")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="EndTCVolume" ItemStyle-HorizontalAlign="Center" HeaderText="End TC Volume">
									<ItemTemplate>
										<asp:Label ID="lblEndTCVolume" Text='<%# DataBinder.Eval(Container.DataItem, "EndTCVolume")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="EndWater" ItemStyle-HorizontalAlign="Center" HeaderText="End Water">
									<ItemTemplate>
										<asp:Label ID="lblEndWater" Text='<%# DataBinder.Eval(Container.DataItem, "EndWater")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="EndTemp" ItemStyle-HorizontalAlign="Center" HeaderText="End Temp">
									<ItemTemplate>
										<asp:Label ID="lblEndTemp" Text='<%# DataBinder.Eval(Container.DataItem, "EndTemp")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="StartHeight" ItemStyle-HorizontalAlign="Center" HeaderText="Start Height">
									<ItemTemplate>
										<asp:Label ID="lblStartHeight" Text='<%# DataBinder.Eval(Container.DataItem, "StartHeight")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="EndHeight" ItemStyle-HorizontalAlign="Center" HeaderText="End Height">
									<ItemTemplate>
										<asp:Label ID="lblEndHeight" Text='<%# DataBinder.Eval(Container.DataItem, "StartTemp")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="CustomerName" ItemStyle-HorizontalAlign="Center" HeaderText="Company">
									<ItemTemplate>
										<asp:Label ID="lblCustomerName" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>

							<HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" />
							<EmptyDataRowStyle Font-Bold="True" ForeColor="Red" BackColor="white" BorderColor="red"
								BorderStyle="Solid" BorderWidth="1px" />
						</asp:GridView>


					</div>
				</div>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>


</asp:Content>
