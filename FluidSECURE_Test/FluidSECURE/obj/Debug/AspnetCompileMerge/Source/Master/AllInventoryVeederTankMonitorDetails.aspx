<%@ Page Title="Inventory Veeder TankMonitor Readings" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllInventoryVeederTankMonitorDetails.aspx.vb" Inherits="Fuel_Secure.AllInventoryVeederTankMonitorDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading">
					<h3 class="panel-title text-center">Inventory Veeder TankMonitor Readings</h3>
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

						<asp:GridView ID="gvInventoryVeederTankMonitor" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
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
								<asp:TemplateField SortExpression="TankStatus" ItemStyle-HorizontalAlign="Center" HeaderText="Tank Status">
									<ItemTemplate>
										<asp:Label ID="lblTankStatus" Text='<%# DataBinder.Eval(Container.DataItem, "TankStatus")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Volume" ItemStyle-HorizontalAlign="Center" HeaderText="Volume">
									<ItemTemplate>
										<asp:Label ID="lblVolume" Text='<%# DataBinder.Eval(Container.DataItem, "Volume")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="TCVolume" ItemStyle-HorizontalAlign="Center" HeaderText="TC Volume">
									<ItemTemplate>
										<asp:Label ID="lblTCVolume" Text='<%# DataBinder.Eval(Container.DataItem, "TCVolume")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Ullage" ItemStyle-HorizontalAlign="Center" HeaderText="Ullage">
									<ItemTemplate>
										<asp:Label ID="lblUllage" Text='<%# DataBinder.Eval(Container.DataItem, "Ullage")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Height" ItemStyle-HorizontalAlign="Center" HeaderText="Height">
									<ItemTemplate>
										<asp:Label ID="lblHeight" Text='<%# DataBinder.Eval(Container.DataItem, "Height")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Water" ItemStyle-HorizontalAlign="Center" HeaderText="Water">
									<ItemTemplate>
										<asp:Label ID="lblWater" Text='<%# DataBinder.Eval(Container.DataItem, "Water")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Temperature" ItemStyle-HorizontalAlign="Center" HeaderText="Temperature">
									<ItemTemplate>
										<asp:Label ID="lblTemperature" Text='<%# DataBinder.Eval(Container.DataItem, "Temperature")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="WaterVolume" ItemStyle-HorizontalAlign="Center" HeaderText="Water Volume">
									<ItemTemplate>
										<asp:Label ID="lblWaterVolume" Text='<%# DataBinder.Eval(Container.DataItem, "WaterVolume")%>'
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
