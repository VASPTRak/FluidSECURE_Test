<%@ Page Title="Tank Chart Details" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="TankChartDetails.aspx.vb" Inherits="Fuel_Secure.TankChartDetails" %>

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
						<asp:HiddenField ID="HDF_TankChartId" runat="server" />
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
						</div>
						<div class="form-group col-md-6 col-sm-6 col-xs-12" style="max-height: 700px; overflow-y: auto;">
							<asp:GridView ID="gv_TankChartDetails" CssClass="table table-bordered" runat="server"
								AutoGenerateColumns="False" EmptyDataText="Data Not found." AllowPaging="false" OnPageIndexChanging="gv_TankChartDetails_PageIndexChanging" PageSize="20" DataKeyNames="TankChartDetailId,TankChartId,IncrementLevel"
								  OnRowDataBound="gv_TankChartDetails_RowDataBound">
                                <%--OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit" OnRowUpdating="OnRowUpdating" AutoGenerateEditButton="true"--%>
								<Columns>
									<asp:TemplateField HeaderText="Increment Level" ItemStyle-VerticalAlign="Middle" HeaderStyle-VerticalAlign="Middle" ItemStyle-CssClass=>
										<ItemTemplate>
											<asp:Label ID="LBL_IncLevel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IncrementLevel")%>'></asp:Label>
										</ItemTemplate>
									</asp:TemplateField>
									<%--<asp:BoundField DataField="IncrementLevel" HeaderText="Increment Level" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Right" />--%>
									<%--<asp:BoundField DataField="GallonLevel" HeaderText="Gallon Level" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Right" />--%>
									<asp:TemplateField HeaderText="Gallon Level" ItemStyle-VerticalAlign="Middle" HeaderStyle-VerticalAlign="Middle">
                                        <ItemTemplate>
											<asp:TextBox ID="txtGallonLevel" runat="server" CssClass="form-control input-sm" Style="width: 80px !important" Text='<%# DataBinder.Eval(Container.DataItem, "GallonLevel")%>' AutoPostBack="true" OnTextChanged="txtGallonLevel_TextChanged" ValidationGroup="TankChartDetailsValidation"></asp:TextBox>
											<%--<asp:RequiredFieldValidator ID="RFD_GallonLevel" runat="server" ControlToValidate="txtGallonLevel" Display="Dynamic" ErrorMessage="Please enter Gallon Level" ForeColor="Red"
												SetFocusOnError="True" ValidationGroup="TankChartDetailsValidation"></asp:RequiredFieldValidator>
											<asp:CompareValidator ID="CV_GallonLevel" runat="server" Display="Dynamic" ErrorMessage="Please enter Gallon Level in decimal format." ForeColor="Red"
												Operator="DataTypeCheck" SetFocusOnError="True" Type="Double" ValidationGroup="TankChartDetailsValidation" ControlToValidate="txtGallonLevel"></asp:CompareValidator>--%>
                                            <asp:Label runat="server" ID="lblErrorMessageForGallon" Style="color:red" Visible="false"></asp:Label>
										</ItemTemplate>
										<%--<ItemTemplate>
											<asp:Label ID="LBL_GallonLevel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "GallonLevel")%>'></asp:Label>
										</ItemTemplate>--%>
										<%--<EditItemTemplate>
											<asp:TextBox ID="txtGallonLevel" runat="server" CssClass="form-control input-sm" Style="width: 80px !important" Text='<%# DataBinder.Eval(Container.DataItem, "GallonLevel")%>'></asp:TextBox>
											<asp:RequiredFieldValidator ID="RFD_GallonLevel" runat="server" ControlToValidate="txtGallonLevel" Display="Dynamic" ErrorMessage="Please enter Gallon Level" ForeColor="Red"
												SetFocusOnError="True" ValidationGroup="TankChartDetailsValidation"></asp:RequiredFieldValidator>
											<asp:CompareValidator ID="CV_GallonLevel" runat="server" Display="Dynamic" ErrorMessage="Please enter Gallon Level in decimal format." ForeColor="Red"
												Operator="DataTypeCheck" SetFocusOnError="True" Type="Double" ValidationGroup="TankChartDetailsValidation" ControlToValidate="txtGallonLevel"></asp:CompareValidator>
										</EditItemTemplate>--%>
									</asp:TemplateField>
								</Columns>
							</asp:GridView>
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Update" Width="100px"
                            UseSubmitBehavior="true" TabIndex="16" ValidationGroup="TankChartDetailsValidation" />
						<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
							UseSubmitBehavior="False" TabIndex="25" OnClick="btnCancel_Click" />
					</div>
				</div>
			</div>

		</ContentTemplate>
	</asp:UpdatePanel>

</asp:Content>
