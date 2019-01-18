<%@ Page Title="FluidSecure Hub" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="FluidSecureHub.aspx.vb" Inherits="Fuel_Secure.FluidSecureHub" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="modal fade" tabindex="-1" role="dialog" id="VehicleBox">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h5 class="modal-title text-center">Click box(es) to authorize FluidSecure Hub to select this Vehicle at the FluidSecure Link:</h5>
						</div>
						<div class="modal-body">
							<div class="row col-md-12 col-sm-12">
								<asp:Label ID="lblVehicleMessage" runat="server" Text="Please select Department."></asp:Label>
							</div>
							<div class="row margin10 text-center">
								<input type="text" class="form-control" id="VehicleInput" onkeyup="SearchVehicles()" placeholder="Search for Vehicle">
							</div>
							<div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
								<asp:UpdatePanel ID="UP_Fuel" runat="server">
									<ContentTemplate>
										<asp:GridView ID="gv_Vehicles" CssClass="table table-bordered" runat="server" DataKeyNames="VehicleId,VehicleNumber,VehicleName" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
											<Columns>
												<asp:TemplateField ItemStyle-HorizontalAlign="Center">
													<HeaderTemplate>
														<asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecificVehicles(this);" runat="server" Style="text-align: center;" />
													</HeaderTemplate>
													<ItemTemplate>
														<asp:CheckBox ID="CHK_Vehicle" runat="server" onclick="javascript:SelectboxVehicle(this);" />
													</ItemTemplate>
												</asp:TemplateField>
												<asp:BoundField DataField="VehicleName" HeaderText="Vehicle Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="VehicleNumber" HeaderText="Vehicle Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												<asp:BoundField DataField="Name" HeaderText="Vehicle's Department" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
											</Columns>
										</asp:GridView>

									</ContentTemplate>
								</asp:UpdatePanel>
							</div>
						</div>
						<div class="modal-footer nextButton">
							<input type="button" id="btnVehicleOk" class="btn btn-success" onclick="ClosePopUp()" value="Ok" />
							<input type="button" id="btnClose" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
							<asp:Button ID="btnCloseVehicle" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUp()" OnClick="btnCloseVehicle_Click" />
						</div>
					</div>
					<!-- /.modal-content -->
				</div>
				<!-- /.modal-dialog -->
			</div>
			<!-- /.modal -->

			<div class="modal fade" tabindex="-1" role="dialog" id="PersonSites">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h5 class="modal-title text-center">Click Box to Select all Sites this FluidSecure Hub is Authorized to Fluid at:</h5>
						</div>
						<div class="modal-body">
							<div class="row col-md-12 col-sm-12">
								<asp:Label ID="lblSiteMessage" runat="server" Text="Please select Company."></asp:Label>
							</div>
							<div class="row margin10">
								<input type="text" id="siteInput" class="form-control" onkeyup="SearchSite()" placeholder="Search for Site Name">
							</div>
							<div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
								<asp:UpdatePanel ID="UP_Sites" runat="server">
									<ContentTemplate>
										<asp:GridView ID="gv_Sites" CssClass="table table-bordered" runat="server" DataKeyNames="SiteID,WifiSSId" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
											<Columns>
												<asp:TemplateField HeaderText="">
													<%--<HeaderTemplate>
                                                        <asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecificForSite(this);" runat="server" />
                                                    </HeaderTemplate>--%>
													<ItemTemplate>
														<asp:CheckBox ID="CHK_PersonSite" runat="server" onclick="javascript:SelectboxSite(this);" />
													</ItemTemplate>
												</asp:TemplateField>
												<asp:BoundField DataField="WifiSSId" HeaderText="Hose Name" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
											</Columns>
										</asp:GridView>

									</ContentTemplate>
								</asp:UpdatePanel>
							</div>
						</div>
						<div class="modal-footer nextButton">
							<input type="button" id="btnSiteOk" class="btn btn-success" onclick="ClosePopUpSite()" value="Ok" />
							<input type="button" id="btnCloseSite" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
							<asp:Button ID="btnCancelSite" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpSite()" OnClick="btnCancelSite_Click" />
						</div>
					</div>
					<!-- /.modal-content -->
				</div>
				<!-- /.modal-dialog -->
			</div>
			<!-- /.modal -->

			<div class="modal fade" tabindex="-1" role="dialog" id="HubIMEIHistory">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h5 class="modal-title text-center">Select form Previous Hub IMEI:</h5>
						</div>
						<div class="modal-body">
							<div class="row col-md-12 col-sm-12">
								<asp:Label ID="LBL_HubIMEIHistory" runat="server" Text=""></asp:Label>
							</div>
							<div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
								<asp:UpdatePanel ID="UP_HubIMEIHistory" runat="server">
									<ContentTemplate>
										<asp:GridView ID="gv_HistoryView" CssClass="table table-bordered" runat="server" DataKeyNames="Name" AutoGenerateColumns="False"
											EmptyDataText="Data Not found." PageSize="10">
											<Columns>
												<asp:TemplateField HeaderText="">
													<ItemTemplate>
														<asp:RadioButton ID="rbHubIMEI" runat="server" onclick="RadioCheck(this);" />
													</ItemTemplate>
												</asp:TemplateField>
												<asp:BoundField HeaderText="IMEI" DataField="Name" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
											</Columns>
										</asp:GridView>

									</ContentTemplate>
								</asp:UpdatePanel>
							</div>
						</div>
						<div class="modal-footer nextButton">
							<asp:Button ID="btnFSHistoryOk" runat="server" class="btn btn-success" OnClientClick="CloseFSHistoryBox()" OnClick="btnFSHistoryOk_Click" Text="Ok" />
							<input type="button" id="btnCloseFSHistoryBox" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
							<asp:Button ID="btnCancelFSHistoryBox" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="CloseFSHistoryBox()" OnClick="btnCancelFSHistory_Click" />
						</div>
					</div>
					<!-- /.modal-content -->
				</div>
				<!-- /.modal-dialog -->
			</div>
			<!-- /.modal -->

			<div class="modal fade" tabindex="-1" role="dialog" id="HubDeviceNumberHistory">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h5 class="modal-title text-center">Select form Previous Hub Device Phone:</h5>
						</div>
						<div class="modal-body">
							<div class="row col-md-12 col-sm-12">
								<asp:Label ID="LBL_DeviceNumberHistory" runat="server" Text=""></asp:Label>
							</div>
							<div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
								<asp:UpdatePanel ID="UpdatePanel1" runat="server">
									<ContentTemplate>
										<asp:GridView ID="gv_DeviceNumber" CssClass="table table-bordered" runat="server" DataKeyNames="Name" AutoGenerateColumns="False"
											EmptyDataText="Data Not found." PageSize="10">
											<Columns>
												<asp:TemplateField HeaderText="">
													<ItemTemplate>
														<asp:RadioButton ID="rbHubDeviceNumber" runat="server" onclick="RadioCheckDeviceNumber(this);" />
													</ItemTemplate>
												</asp:TemplateField>
												<asp:BoundField HeaderText="Device Phone" DataField="Name" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
											</Columns>
										</asp:GridView>

									</ContentTemplate>
								</asp:UpdatePanel>
							</div>
						</div>
						<div class="modal-footer nextButton">
							<asp:Button ID="btnDeviceNumberHistoryOk" runat="server" class="btn btn-success" OnClientClick="CloseDeviceNumberHistoryBox()" OnClick="btnDeviceNumberHistoryOk_Click" Text="Ok" />
							<input type="button" id="btnCloseDeviceNumberHistoryBox" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
							<asp:Button ID="btnCancelDeviceNumber" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="CloseDeviceNumberHistoryBox()" OnClick="btnCancelDeviceNumber_Click" />
						</div>
					</div>
					<!-- /.modal-content -->
				</div>
				<!-- /.modal-dialog -->
			</div>
			<!-- /.modal -->
			<%-- <div class="modal fade" tabindex="-1" role="dialog" id="FuelingTimes">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Click Box to Select all Fueling Times this FluidSecure Hub is Authorized to Fluid at:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblFuelingTimes" runat="server" Text=""></asp:Label>
                            </div>
                            <div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UP_FuelingTimes" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gv_FuelingTimes" CssClass="table table-bordered" runat="server" DataKeyNames="TimeId" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="CHK_FuelingTimes" runat="server" onclick="javascript:SelectboxFuelingTimes(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="TimeText" HeaderText="Time" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnFuelingTimesOk" class="btn btn-success" onclick="ClosePopUpFuelingTimes()" value="Ok" />
                            <input type="button" id="btnCloseFuelingTimes" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                            <asp:Button ID="btnCancelFuelingTimes" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpFuelingTimes()" OnClick="btnCancelFuelingTimes_Click" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>--%>
			<!-- /.modal -->
			<asp:HiddenField ID="HDF_HubIMEICurrentName" runat="server" />
			<asp:HiddenField ID="HDF_HubIMEINameHistory" runat="server" />
			<asp:HiddenField ID="HDF_HubCurrentDeviceNumber" runat="server" />
			<asp:HiddenField ID="HDF_HubDeviceNumberHistory" runat="server" />
			
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
								FluidSecure Hub Name:
                        <%--<label class="text-danger font-required">[required]</label>--%></label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPersonName" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="30" Width="250" ReadOnly="true"></asp:TextBox>
							<%-- <asp:RequiredFieldValidator ID="RFDCustName" runat="server" ControlToValidate="txtPersonName"
                                ErrorMessage="Please Enter FluidSecure Hub Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
							<asp:HiddenField ID="HDF_PersonnelId" runat="server"></asp:HiddenField>
							<asp:HiddenField ID="HDF_UniqueUserId" runat="server"></asp:HiddenField>
							<asp:HiddenField ID="HDF_CompanyId" runat="server"></asp:HiddenField>
							<asp:HiddenField ID="HDF_TotalPersonnel" runat="server" />
							<asp:HiddenField ID="HDF_PreviousPreAuthCount" runat="server" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Email (Username):</label>
							<%--<label class="text-danger font-required">[required]</label>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtEmail" TextMode="Email" runat="server" CssClass="form-control input-sm" MaxLength="256" TabIndex="15" ReadOnly="true"></asp:TextBox>
							<%--<asp:RequiredFieldValidator ID="RFDEmail" runat="server" ControlToValidate="txtEmail"
                                ErrorMessage="Please Enter email." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RDFEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Please enter valid email." ForeColor="Red" SetFocusOnError="True" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>--%>
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Site Name:
                        <%--<label class="text-danger font-required">[required]</label>--%></label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtSiteName" CssClass="form-control input-sm" TabIndex="2" runat="server" MaxLength="30" Width="250"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								HUB Address:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtHUB_Address" runat="server" CssClass="form-control input-sm" TextMode="MultiLine" Rows="4" TabIndex="16"></asp:TextBox>
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Vehicles Allowed to Fuel:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<input type="button" id="BTN_Vehicles" tabindex="3" onclick="OpenVehicleTypeBox();" value="Click to add vehicle" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Authorized Fueling links:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<input type="button" id="BTN_PersonSite" tabindex="17" onclick="OpenPersonSiteBox();" value="Click to add FluidSecure links" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								IMEI Number:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtIMEINumber" runat="server" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
						</div>
						<%-- <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Authorized Fueling Times:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <input type="button" id="BTN_PersonTiming" tabindex="9" onclick="OpenPersonTimingBox();" value="Click to add Fueling Times" />
                        </div>--%>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Device Phone:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtDeviceNumber" runat="server" CssClass="form-control input-sm" MaxLength="15" Width="130" data-toggle="tooltip" title="Only (,),-, space, and + symbols allowed." TabIndex="4"></asp:TextBox>
						<asp:Label ID="lblDeviceError" runat="server" ForeColor="Red" Style="display: none;" Text="Please enter valid Device Phone."></asp:Label>
						</div>
					</div>
					<div id="divViewHistory" runat="server" class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:Button ID="btn_ViewHistory" runat="server" CssClass="btn btn-default" Text="View History" OnClick="btn_ViewHistory_Click" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:Button ID="btn_ViewDeviceHistory" runat="server" CssClass="btn btn-default" Text="View Device Phone History" OnClick="btn_ViewDeviceHistory_Click" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="col-md-6 col-sm-12 col-xs-12" id="UFLSHide" runat="server">
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="UFLSLabel" runat="server">
							<label>
								Update FluidSecure Link Software on next Fueling?</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12" id="UFLSCheckbox" runat="server">
							<asp:CheckBox ID="chkSoftUpdate" runat="server" TabIndex="5" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Gate HUB :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chk_GateHub" runat="server" TabIndex="19" />
						</div>

					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Department
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Department" runat="server" CssClass="form-control input-sm" TabIndex="6"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFD_Department" runat="server" Font-Size="Small"
								Font-Bold="False" Font-Names="arial" ErrorMessage="Please select department."
								ControlToValidate="DDL_Department" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
						</div>

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Active:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chkIsApproved" runat="server" TabIndex="18" />
						</div>

						<%-- <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Stay Open :</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="CHK_StayOpenGate" runat="server" TabIndex="29" Checked="false" />
                        </div>--%>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Export Code:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtExportCode" runat="server" CssClass="form-control input-sm" TabIndex="7" MaxLength="25"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Personnel PIN Require:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="CHK_IsPersonnelPINRequire" runat="server" TabIndex="20" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>HF Bluetooth card reader:</label>
							<%--<label class="text-danger font-required">[required]</label>:</label>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtBCardReader" runat="server" CssClass="form-control input-sm" TabIndex="8" MaxLength="50"></asp:TextBox>
							<%--<asp:RequiredFieldValidator ID="RFD_BCardReader" runat="server" ControlToValidate="txtBCardReader"
                                ErrorMessage="Please Enter Phone Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Vehicle Number Require :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chk_IsVehicleNumberRequire" runat="server" Checked="true" TabIndex="21" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>HF Bluetooth Card Reader Mac Address:</label>
							<%--<label class="text-danger font-required">[required]</label>:</label>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtBluetoothCardReaderMacAddress" runat="server" CssClass="form-control input-sm" TabIndex="9" MaxLength="50"></asp:TextBox>
						</div>

						<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Company
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="22" onchange="ConfirmChange();"></asp:DropDownList>
							<asp:RequiredFieldValidator ID="RFD_Customer" runat="server" Font-Size="Small"
								Font-Bold="False" Font-Names="arial" ErrorMessage="Please select company."
								ControlToValidate="DDL_Customer" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>LF Bluetooth card reader:</label>
							<%--<label class="text-danger font-required">[required]</label>:</label>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtLFBCardReader" runat="server" CssClass="form-control input-sm" TabIndex="10" MaxLength="50"></asp:TextBox>
							<%--<asp:RequiredFieldValidator ID="RFD_BCardReader" runat="server" ControlToValidate="txtBCardReader"
                                ErrorMessage="Please Enter Phone Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
						</div>

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Printer name:
                               <%-- <label class="text-danger font-required">[required]</label>:</label>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPrinterName" runat="server" CssClass="form-control input-sm" TabIndex="23" MaxLength="50"></asp:TextBox>
							<%-- <asp:RequiredFieldValidator ID="RFD_PrinterName" runat="server" ControlToValidate="txtPrinterName"
                                ErrorMessage="Please Enter Printer name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>LF Bluetooth Card Reader Mac Address:</label>
							<%--<label class="text-danger font-required">[required]</label>:</label>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtLFBluetoothCardReaderMacAddress" runat="server" CssClass="form-control input-sm" TabIndex="11" MaxLength="50"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
							Printer MAC Address:
						</div>
						<div class="form-group col-md-2 col-sm-2 col-xs-12">
							<asp:TextBox ID="txtPrinterMACAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="24"></asp:TextBox>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Person Has a Fob/Card:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="CHK_IsPersonHasFob" runat="server" TabIndex="12" />
							<asp:Label ID="lblIsPersonHasFob" runat="server" Text="(MUST use the Fob/Card, and not use a Person PIN on HUB application)"></asp:Label>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Veeder Root Mac Address:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtVeederRootMacAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="25"></asp:TextBox>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Vehicle Has a Fob/Card:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="CHK_IsVehicleHasFob" runat="server" TabIndex="12" />
							<asp:Label ID="lblIsVehicleHasFob" runat="server" Text="(MUST use the Fob/Card, and not use a vehicle number on HUB application)"></asp:Label>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Is Logging:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chk_IsLogging" runat="server" TabIndex="26" Checked="true" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Contact Name:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtContactName" runat="server" CssClass="form-control input-sm" MaxLength="100" TabIndex="12"></asp:TextBox>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Contact Phone Number:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control input-sm" MaxLength="15" Width="130" TabIndex="27" data-toggle="tooltip" title="Only (,),-, space, and + symbols allowed."></asp:TextBox>
							<%--  <asp:RequiredFieldValidator ID="RFD_PhoneNumber" runat="server" ControlToValidate="txtPhoneNumber"
                                ErrorMessage="Please Enter Phone Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
							<asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Style="display: none;" Text="Please enter valid phone number."></asp:Label>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Contact Email:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtContactEmail" TextMode="Email" runat="server" CssClass="form-control input-sm" MaxLength="256" TabIndex="14"></asp:TextBox>
							<asp:RegularExpressionValidator ID="RDFContactEmail" runat="server" ControlToValidate="txtContactEmail" Display="Dynamic" ErrorMessage="Please enter valid contact email." ForeColor="Red" SetFocusOnError="True"
								ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>
						</div>
						<div id="WifiChannelToUse" runat="server">
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>Wifi Channel To Use : </label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:DropDownList ID="DDL_WifiChannelToUse" runat="server" CssClass="form-control input-sm" Width="100" TabIndex="28">
									<asp:ListItem Text="1" Value="1"></asp:ListItem>
									<asp:ListItem Text="6" Value="6"></asp:ListItem>
									<asp:ListItem Text="11" Value="11" Selected="True"></asp:ListItem>
								</asp:DropDownList>
							</div>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Enable FA :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chk_HubForFA" runat="server" TabIndex="29" Checked="false" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Enable Printer :</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chkEnablePrinter" runat="server" TabIndex="30" Checked="false" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 text-center col-xs-12">
						<asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
							UseSubmitBehavior="true" TabIndex="30" ValidationGroup="PersonelValidation" OnClientClick="return IsValidPhoneNumber();" />
						<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
							UseSubmitBehavior="False" TabIndex="31" OnClick="btnCancel_Click" OnClientClick="window.location.href='/Master/AllFluidSecureHub?Filter=Filter'" />
						<asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px" OnClientClick="return IsValidPhoneNumber();"
							UseSubmitBehavior="true" TabIndex="32" ValidationGroup="PersonelValidation" OnClick="btnSaveAndAddNew_Click" />
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

	<!--alert message popup-->
	<div class="modal fade" tabindex="-1" role="dialog" id="ChangeCompany">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<h3 class="modal-title text-center">FluidSecure</h3>
				</div>
				<div class="modal-body">
					<h4>Changing the Company will remove the associated Vehicles, Fueling links, Department, Bluetooth card reader and Printer name. Do you want to continue?</h4>
				</div>
				<div class="modal-footer nextButton">
					<input type="button" id="btnChangeCompanyOk" class="btn btn-success" onclick="ClosePopUpForCompany()" value="Ok" />
					<input type="button" id="btnCloseChangeCompany" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Cancel" />
					<input type="button" id="btnCloseChangeCompany2" class="btn btn-default" data-dismiss="modal" value="Cancel" onclick="CancelCompanyChange()" />
				</div>
			</div>
			<!-- /.modal-content -->
		</div>
		<!-- /.modal-dialog -->
	</div>
	<!-- /.modal -->

	<script src="../Scripts/jquery.maskedinput.js"></script>
	<script>
		function loadFunction() {
			$('[data-toggle="tooltip"]').tooltip();
			<%--$("#<%=txtPhoneNumber.ClientID%>").mask("999-999-9999");--%>
        }

		function CancelCompanyChange() {
			var previousCompanyID = $("#<%=HDF_CompanyId.ClientID%>").val();
        	$('#<%= DDL_Customer.ClientID %>').val(previousCompanyID);
        }

        function ClosePopUpForCompany() {
        	$("#btnCloseChangeCompany").click();
        	$('body').removeClass("modal-open");
        	__doPostBack('<%= DDL_Customer.ClientID %>', '');
		}

		function ConfirmChange() {
			if (($('#<%= HDF_PersonnelId.ClientID %>').val()) != "") {
        		$('#ChangeCompany').modal({
        			show: true,
        			backdrop: 'static',
        			keyboard: false
        		});
        	}
        	else {
        		__doPostBack('<%= DDL_Customer.ClientID %>', '');
            }
		}

		$(function () {
			loadFunction();
		});
		function ClosePopUp() {


			$("#btnClose").click();
			$('body').removeClass("modal-open");

			$('#BTN_PersonTiming').focus();
			enableDisableButtons(false)

		}

		function ClosePopUpSite() {
			$("#btnCloseSite").click();
			$('body').removeClass("modal-open");
			$('#<%= chkSoftUpdate.ClientID%>').focus();
            enableDisableButtons(false)
		}

		function ClosePopUpFuelingTimes() {
			$("#btnCloseFuelingTimes").click();
			$('body').removeClass("modal-open");
			$('#BTN_PersonSite').focus();
			enableDisableButtons(false)
		}

		function enableDisableButtons(enableDisable) {
			if (document.getElementById('<%= btnFirst.ClientID%>') != null) {
        		document.getElementById('<%= btnFirst.ClientID%>').disabled = enableDisable;
            	document.getElementById('<%= btnLast.ClientID%>').disabled = enableDisable;
            	document.getElementById('<%= btnNext.ClientID%>').disabled = enableDisable;
            	document.getElementById('<%= btnprevious.ClientID%>').disabled = enableDisable;
            }

			document.getElementById('<%= btnSave.ClientID%>').disabled = enableDisable;
        	document.getElementById('<%= btnCancel.ClientID%>').disabled = enableDisable;
        }


        function IsValidPhoneNumber() {
        	//debugger;
        	var phoneNumber = document.getElementById('<%=txtPhoneNumber.ClientID%>').value;
        	//it accepts 850-294-2562(us phone number- req date 09-Dec-2016)
        	//if (phoneNumber.match(/^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/)) {//phone number accept all number with only (-)+space symbols.
        	if (phoneNumber.match(/^[- +()]*[0-9][- +()0-9]*$/) || phoneNumber == '') {
        		document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "none";
            	//return true;

        		if (Page_ClientValidate("PersonelValidation"))
        		{

        			return IsValidDeviceNumber();
        		}
            	else
            		return false;


            }
            else {
            	document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "";
            	Page_ClientValidate("PersonelValidation")
            	return false;
            }

		}

		function IsValidDeviceNumber() {
        	//debugger;
			var DeviceNumber = document.getElementById('<%=txtDeviceNumber.ClientID%>').value;
        	//it accepts 850-294-2562(us phone number- req date 09-Dec-2016)
        	//if (phoneNumber.match(/^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/)) {//phone number accept all number with only (-)+space symbols.
			if (DeviceNumber.match(/^[- +()]*[0-9][- +()0-9]*$/) || DeviceNumber == '') {
        		document.getElementById('<%=lblDeviceError.ClientID%>').style.display = "none";
            	//return true;

        		if (Page_ClientValidate("PersonelValidation"))
        		{

        			return true;
        		}
            	else
            		return false;


            }
            else {
            	document.getElementById('<%=lblDeviceError.ClientID%>').style.display = "";
            	Page_ClientValidate("PersonelValidation")
            	return false;
            }

		}

	</script>
	<style>
		.ui-tooltip {
			background-color: #ffffff;
			font-size: 12px;
			padding: 6px;
			z-index: 9999;
		}

		th {
			text-align: center;
		}
	</style>
	<script type="text/javascript">

		function OpenVehicleTypeBox() {
			$("#VehicleInput").val("");
			SearchVehicles();
			enableDisableButtons(true);
			$('#VehicleBox').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});

		}

		function OpenPersonSiteBox() {
			$("#siteInput").val("");
			SearchSite();
			enableDisableButtons(true);
			$('#PersonSites').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}


		//function OpenPersonTimingBox() {
		//    enableDisableButtons(true);
		//    $('#FuelingTimes').modal({
		//        show: true,
		//        backdrop: 'static',
		//        keyboard: false
		//    });
		//}


	</script>
	<script language="javascript">

		function SelectAllCheckboxesSpecificVehicles(spanChk) {

			var IsChecked = spanChk.checked;
			var Chk = spanChk;
			Parent = document.getElementById('<%= gv_Vehicles.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
            	if (items[i].id != Chk && items[i].type == "checkbox") {
            		if (items[i].checked != IsChecked) {
            			items[i].click();
            		}
            	}
            }
		}

		function SelectboxVehicle(spanChk) {

			var IsChecked = spanChk.checked;
			if (IsChecked == false) {
				Parent = document.getElementById('<%= gv_Vehicles.ClientID%>');
            	var checkBoxSelector = "#<%=gv_Vehicles.ClientID%> input[id*='chkAll']";

            	$(checkBoxSelector).attr('checked', false);
            }
		}

		function SelectAllCheckboxesSpecificForSite(spanChk) {

			var IsChecked = spanChk.checked;
			var Chk = spanChk;
			Parent = document.getElementById('<%= gv_Sites.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
            	if (items[i].id != Chk && items[i].type == "checkbox") {
            		if (items[i].checked != IsChecked) {
            			items[i].click();
            		}
            	}
            }
		}

		function SelectboxSite(spanChk) {

			var IsChecked = spanChk.checked;
			if (IsChecked == false) {
				Parent = document.getElementById('<%= gv_Sites.ClientID%>');
            	var checkBoxSelector = "#<%=gv_Sites.ClientID%> input[id*='chkAll']";

            	$(checkBoxSelector).attr('checked', false);
            }
		}

		<%--  function SelectAllCheckboxesSpecificFoFuelingTimes(spanChk) {

            var IsChecked = spanChk.checked;
            var Chk = spanChk;
            Parent = document.getElementById('<%= gv_FuelingTimes.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
                if (items[i].id != Chk && items[i].type == "checkbox") {
                    if (items[i].checked != IsChecked) {
                        items[i].click();
                    }
                }
            }
        }--%>

		<%-- function SelectboxFuelingTimes(spanChk) {

            if (spanChk.id.toLowerCase().indexOf("chk_fuelingtimes_0") > -1) {
                var IsChecked = spanChk.checked;

                Parent = document.getElementById('<%= gv_FuelingTimes.ClientID %>');
                var items = Parent.getElementsByTagName('input');
                for (i = 0; i < items.length; i++) {
                    if (items[i].id != spanChk.id && items[i].type == "checkbox") {
                        if (items[i].checked != IsChecked) {
                            items[i].checked = IsChecked;
                        }
                    }
                }

            }
            else {
                var checkBoxSelector = "#<%=gv_FuelingTimes.ClientID%> input[id*='CHK_FuelingTimes_0']";

                var IsChecked = spanChk.checked;
                if (IsChecked == false) {
                    Parent = document.getElementById('<%= gv_FuelingTimes.ClientID%>');


                    $(checkBoxSelector).attr('checked', false);
                }
                else {
                    var isAll = false;

                    Parent = document.getElementById('<%= gv_FuelingTimes.ClientID %>');
                    var items = Parent.getElementsByTagName('input');
                    for (i = 0; i < items.length; i++) {
                        if (items[i].type == "checkbox" && !(items[i].id.toLowerCase().indexOf("chk_fuelingtimes_0") > -1)) {
                            if (items[i].checked == true) {
                                isAll = true;
                            }
                            else {
                                isAll = false;
                                break;
                            }
                        }
                    }

                    if (isAll == true) {
                        $(checkBoxSelector)[0].checked = true;
                    }
                    else
                        $(checkBoxSelector)[0].checked = false;
                }
            }
        }--%>

	</script>
	<script>
		function SearchSite() {
			var input, filter, table, tr, td, i;
			input = document.getElementById("siteInput");
			filter = input.value.toLowerCase();
			table = document.getElementById('<%= gv_Sites.ClientID %>');
            if (table != null) {
            	tr = table.getElementsByTagName("tr");
            	for (i = 0; i < tr.length; i++) {
            		td = tr[i].getElementsByTagName("td")[1];
            		if (td) {
            			if (td.innerText.toLowerCase().indexOf(filter) > -1) {
            				tr[i].style.display = "";
            			} else {
            				tr[i].style.display = "none";
            			}
            		}
            	}

            }
		}
		function SearchVehicles() {

			var input, filter, table, tr, td, i;
			input = document.getElementById("VehicleInput");
			filter = input.value.toLowerCase();
			table = document.getElementById('<%= gv_Vehicles.ClientID %>');
            if (table != null) {
            	tr = table.getElementsByTagName("tr");
            	for (i = 0; i < tr.length; i++) {
            		td = tr[i].getElementsByTagName("td")[1];
            		if (td) {
            			if (td.innerText.toLowerCase().indexOf(filter) > -1) {
            				tr[i].style.display = "";
            			} else {
            				td = tr[i].getElementsByTagName("td")[2];
            				if (td) {
            					if (td.innerText.toLowerCase().indexOf(filter) > -1) {
            						tr[i].style.display = "";
            					} else {
            						td = tr[i].getElementsByTagName("td")[3];
            						if (td) {
            							if (td.innerText.toLowerCase().indexOf(filter) > -1) {
            								tr[i].style.display = "";
            							} else {
            								tr[i].style.display = "none";
            							}
            						}
            					}
            				}
            			}
            		}
            	}

            }
		}

		function OpenFSHistoryBox() {

			$('#HubIMEIHistory').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});

		}
		function CloseFSHistoryBox() {
			$("#btnCloseFSHistoryBox").click();
			$('body').removeClass("modal-open");


		}
		function RadioCheck(rb) {

			var gv = document.getElementById("<%=gv_HistoryView.ClientID%>");

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


		function OpenDeviceNumberHistoryBox() {

			$('#HubDeviceNumberHistory').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});

		}
		function CloseDeviceNumberHistoryBox() {
			$("#btnCloseDeviceNumberHistoryBox").click();
			$('body').removeClass("modal-open");


		}
		function RadioCheckDeviceNumber(rb) {

			var gv = document.getElementById("<%=gv_DeviceNumber.ClientID%>");

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
