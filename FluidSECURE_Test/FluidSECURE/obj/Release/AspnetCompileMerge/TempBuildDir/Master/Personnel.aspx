<%@ Page Title="Personnel" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Personnel.aspx.vb" Inherits="Fuel_Secure.Personnel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<%--<div class="modal fade" tabindex="-1" role="dialog" id="VehicleBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Click box(es) to authorize Person to select this Vehicle at the FluidSecure Link:</h5>
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
            </div>--%>
			<!-- /.modal -->

			<div class="modal fade" tabindex="-1" role="dialog" id="PersonSites">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h5 class="modal-title text-center">Click Box to Select all FluidSecure links this person is Authorized to Fluid at:</h5>
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
													<HeaderTemplate>
														<asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecificForSite(this);" runat="server" />
													</HeaderTemplate>
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


			<div class="modal fade" tabindex="-1" role="dialog" id="FuelingTimes">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h5 class="modal-title text-center">Click Box to Select all Fueling Times this person is Authorized to Fluid at:</h5>
						</div>
						<div class="modal-body">
							<div class="row col-md-12 col-sm-12">
								<asp:Label ID="lblFuelingTimes" runat="server" Text=""></asp:Label>
							</div>
							<div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
								<asp:UpdatePanel ID="UP_FuelingTimes" runat="server">
									<ContentTemplate>
										<asp:GridView ID="gv_FuelingTimes" CssClass="table table-bordered" runat="server" DataKeyNames="TimeId,TimeText" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
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
			</div>
			<!-- /.modal -->

			<div class="modal fade" tabindex="-1" role="dialog" id="myModalActiveInActive">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h3 class="modal-title text-center">FluidSecure</h3>
						</div>
						<div class="modal-body">
							<h4 id="lblMessageActiveInactive"></h4>
						</div>
						<div class="modal-footer nextButton">
							<button type="button" id="btnCloseActiveInactiveModel" class="btn btn-default" data-dismiss="modal" style="visibility: hidden"></button>
							<asp:Button runat="server" ID="btnMyModalActiveInActiveNo" class="btn btn-default" OnClick="btnMyModalActiveInActiveNo_Click" Text="No" />
							<asp:Button runat="server" ID="btnMyModalActiveInActiveSuccess" class="btn btn-success" OnClick="btnMyModalActiveInActiveSuccess_Click" Text="Yes" />

						</div>
					</div>
					<!-- /.modal-content -->
				</div>
				<!-- /.modal-dialog -->
			</div>
			<!-- /.modal -->

			<div class="modal fade" tabindex="-1" role="dialog" id="myModalRedirectToVehicleMapping">
				<div class="modal-dialog modal-lg">
					<div class="modal-content">
						<div class="modal-header">
							<h3 class="modal-title text-center">FluidSecure</h3>
						</div>
						<div class="modal-body">
							<h4 runat="server" id="lblMessageRedirectToVehicleMapping"></h4>
						</div>
						<div class="modal-footer nextButton">
							<button type="button" id="btnCloseRedirectToVehicleMappingModel" class="btn btn-default" data-dismiss="modal" style="visibility: hidden"></button>
							<asp:Button runat="server" ID="btnRedirectToVehicleMappingNo" class="btn btn-default" OnClick="btnRedirectToVehicleMappingNo_Click" Text="No" />
							<asp:Button runat="server" ID="btnRedirectToVehicleMappingYes" class="btn btn-success" OnClick="btnRedirectToVehicleMappingYes_Click" Text="Yes" ValidationGroup="PersonelValidation" />

						</div>
					</div>
					<!-- /.modal-content -->
				</div>
				<!-- /.modal-dialog -->
			</div>
			<!-- /.modal -->

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
								Is Hub user:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="CHK_HubUser" OnCheckedChanged="CHK_HubUser_CheckedChanged" AutoPostBack="true" runat="server" />
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Person Name 
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPersonName" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="30" Width="250"></asp:TextBox>
							<%--<asp:RequiredFieldValidator ID="RFDCustName" runat="server" ControlToValidate="txtPersonName"
                                ErrorMessage="Please Enter Personnel Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
							<asp:RegularExpressionValidator ID="REV_PersonName" runat="server" ControlToValidate="txtPersonName" ErrorMessage="comma not allowed. Please remove comma and try again" ValidationExpression="^[^,]+$"
								Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>
							<asp:Label ID="LBL_PersonError" runat="server" ForeColor="Red" Style="display: none;" Text="Please Enter Personnel Name."></asp:Label>

							<asp:HiddenField ID="HDF_PersonnelId" runat="server"></asp:HiddenField>
							<asp:HiddenField ID="HDF_UniqueUserId" runat="server"></asp:HiddenField>
							<asp:HiddenField ID="HDF_TotalPersonnel" runat="server" />
							<asp:HiddenField ID="HDF_PreviousPreAuthCount" runat="server" />
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Pre-Authorization Transactions Count:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPreAuth" runat="server" CssClass="form-control input-sm" MaxLength="2" Width="40" TabIndex="12" onkeypress="return onlyNumbers(event);" Text="0" Style="float: left; margin-right: 20px;"></asp:TextBox>
							Not Used Count : 
                            <asp:Label ID="lblPreAuthNotUsed" runat="server" ForeColor="Red"></asp:Label>
							& Used Count : 
                            <asp:Label ID="lblPreAuthUsed" runat="server" ForeColor="Green"></asp:Label>
							<asp:CompareValidator ID="CMV_PreAuth" runat="server" ControlToValidate="txtPreAuth" Display="Dynamic" ErrorMessage="Please enter pre authorization count in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="PersonelValidation"></asp:CompareValidator>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div id="AccessLevelsShow" runat="server" visible="false">
							<div class="form-group col-md-6 col-sm-6 textright col-xs-12">
							</div>
						</div>
						<div id="AccessLevels" runat="server">
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Access Levels
                        <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:DropDownList ID="DDL_AccessLevels" runat="server" CssClass="form-control input-sm" TabIndex="2" AutoPostBack="true" OnSelectedIndexChanged="DDL_AccessLevels_SelectedIndexChanged"></asp:DropDownList>
								<asp:Label ID="lblAccessLevels" runat="server" ForeColor="Red" Style="display: none;" Text="Please select role."></asp:Label>
								<%--<asp:RequiredFieldValidator ID="RFD_AccessLevels" runat="server" ErrorMessage="Please select role."
                                ControlToValidate="DDL_AccessLevels" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
							</div>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="ChangePWDLbl" runat="server">
							<label>
								Change Password:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12" id="ChangePWDCHK" runat="server">
							<asp:CheckBox ID="CHK_ChangePWD" runat="server" OnCheckedChanged="CHK_ChangePWD_CheckedChanged" AutoPostBack="true" />
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								PIN (additional security):</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPinNumber" runat="server" CssClass="form-control input-sm" MaxLength="20" Width="95" TabIndex="2"></asp:TextBox>
							<asp:RegularExpressionValidator ID="REV_PinNumber" runat="server" ControlToValidate="txtPinNumber" ErrorMessage="comma not allowed. Please remove comma and try again" ValidationExpression="^[^,]+$"
								Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>
							<%--<asp:CompareValidator ID="CVPinNumber" runat="server" ControlToValidate="txtPinNumber" Display="Dynamic" ErrorMessage="Please enter PIN Number in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Double" ValidationGroup="PersonelValidation"></asp:CompareValidator>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Fluid Limit Per Transaction:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtFuelLimitPertxtn" runat="server" CssClass="form-control input-sm" onkeypress="return onlyNumbers(event);" MaxLength="4" Width="50" TabIndex="13" data-toggle="tooltip" title="0 means unlimited !"></asp:TextBox>
							<asp:CompareValidator ID="CMV_FuelLimitPerTxtn" runat="server" ControlToValidate="txtFuelLimitPertxtn" Display="Dynamic" ErrorMessage="Please enter limit in integer format" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="PersonelValidation"></asp:CompareValidator>
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Email (Username)
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtEmail" TextMode="Email" runat="server" CssClass="form-control input-sm" MaxLength="256" TabIndex="3"></asp:TextBox>
							<%--<asp:RequiredFieldValidator ID="RFDEmail" runat="server" ControlToValidate="txtEmail"
                                ErrorMessage="Please Enter email." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
							<asp:Label ID="lblEmail" runat="server" ForeColor="Red" Style="display: none;" Text="Please Enter email."></asp:Label>
							<asp:RegularExpressionValidator ID="RDFEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Please enter valid email." ForeColor="Red" SetFocusOnError="True" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Fluid Limit Per Day:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtFuelLimitPerDay" runat="server" CssClass="form-control input-sm" MaxLength="4" Width="50" TabIndex="14" onkeypress="return onlyNumbers(event);" data-toggle="tooltip" title="0 means unlimited !"></asp:TextBox>
							<asp:CompareValidator ID="CMV_FuelLimitPerDay" runat="server" ControlToValidate="txtFuelLimitPerDay" Display="Dynamic" ErrorMessage="Please enter limit in integer format" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="PersonelValidation"></asp:CompareValidator>
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="PWDLabel" runat="server">
							<label>
								User Password
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12" id="PWDTextbox" runat="server">
							<asp:TextBox ID="txtUserPassword" TextMode="Password" runat="server" CssClass="form-control input-sm" MaxLength="30" TabIndex="4" Width="200"></asp:TextBox>
							<%--<asp:RequiredFieldValidator ID="RFD_Password" runat="server" ControlToValidate="txtUserPassword"
                                ErrorMessage="Please Enter password." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
							<asp:Label ID="LBLPassword" runat="server" ForeColor="Red" Style="display: none;" Text="Please Enter password."></asp:Label>
						</div>
						<div class="form-group col-md-6 col-sm-6" id="CPWDHide1" runat="server">
							&nbsp;
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="VehicleHide" runat="server">
							<label>
								Vehicles Allowed to Fuel:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12" id="VehicleHide1" runat="server">
							<%--<input type="button" id="BTN_Vehicles" tabindex="15" onclick="OpenVehicleTypeBox();" value="Click to add vehicle" />--%>
							<asp:Button ID="btnAddVehicles" runat="server" Text="Click to add vehicle" OnClick="btnAddVehicles_Click" ValidationGroup="PersonelValidation" OnClientClick="return IsValidPhoneNumber();" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="CPWDLabel" runat="server">
							<label>
								Confirm password
                         <label class="text-danger font-required">[required]</label>:
							</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12" id="CPWDTextbox" runat="server">
							<asp:TextBox ID="txtConfirmPassword" TextMode="Password" runat="server" CssClass="form-control input-sm" MaxLength="30" Width="200" TabIndex="5"></asp:TextBox>
							<asp:CompareValidator ID="CompareValidator" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtUserPassword"
								ErrorMessage="Passwords do not match." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:CompareValidator>
							<asp:RequiredFieldValidator ID="RFDCResetPassword" runat="server" ControlToValidate="txtConfirmPassword"
								ErrorMessage="Please Enter Confirm Password." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
						</div>
						<div class="form-group col-md-6 col-sm-6" id="CPWDHide" runat="server">
							&nbsp;
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Fob/Card Number:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="TXT_FoBNUM" runat="server" CssClass="form-control input-sm" TabIndex="16"></asp:TextBox>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Phone Number
                        <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control input-sm" MaxLength="15" Width="130" TabIndex="6" data-toggle="tooltip" title="Only (,),-, space, and + symbols allowed."></asp:TextBox>
							<%--data-toggle="tooltip" title="xxx-xxx-xxxx"--%>
							<%-- <asp:RequiredFieldValidator ID="RFD_PhoneNumber" runat="server" ControlToValidate="txtPhoneNumber"
                                ErrorMessage="Please Enter Phone Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
							<asp:Label ID="lblPhoneError" runat="server" ForeColor="Red" Style="display: none;" Text="Please Enter Phone Number."></asp:Label>
							<asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Style="display: none;" Text="Please enter valid contact number."></asp:Label>
							<%--Please enter valid US contact number in xxx-xxx-xxxx format.--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Authorized Fueling Times:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<input type="button" id="BTN_PersonTiming" tabindex="17" onclick="OpenPersonTimingBox();" value="Click to add Fueling Times" />
						</div>

					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div runat="server" id="divIMEI">
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									IMEI Number:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtIMEINumber" runat="server" CssClass="form-control input-sm" TabIndex="7" Visible="false"></asp:TextBox>
								<%--  <asp:RegularExpressionValidator ID="REV_IMEINumber" runat="server" ControlToValidate="txtIMEINumber" ErrorMessage="comma not allowed. Please remove comma and try again" ValidationExpression="^[^,]+$"
                                Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>--%>
								<asp:Button ID="btnIMEIPersonnelMapping" runat="server" Text="Click to Map IMEI-Personnel" OnClick="btnIMEIPersonnelMapping_Click" />
							</div>
						</div>
						<div runat="server" id="divIMEIAdjustment">
							<div class="form-group col-md-6 col-sm-6 textright col-xs-12">
							</div>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Authorized Fueling links:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<input type="button" id="BTN_PersonSite" tabindex="18" onclick="OpenPersonSiteBox();" value="Click to add FluidSecure links" />
						</div>

					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="UFLSLabel" runat="server">
							<label>
								Update FluidSecure Link Software on next Fueling?</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12" id="UFLSCheckbox" runat="server">
							<asp:CheckBox ID="chkSoftUpdate" runat="server" TabIndex="19" />
						</div>

					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Department Name
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Department" runat="server" CssClass="form-control input-sm" TabIndex="9" AutoPostBack="true" OnSelectedIndexChanged="DDL_Department_SelectedIndexChanged"></asp:DropDownList>
							<asp:Label ID="lbldepartment" runat="server" ForeColor="Red" Style="display: none;" Text="Please select department."></asp:Label>
							<%--<asp:RequiredFieldValidator ID="RFD_Department" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please select department."
                                ControlToValidate="DDL_Department" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Active:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="chkIsApproved" runat="server" TabIndex="20" OnClick="javascript:ConfirmActiveInactive(this);" />
						</div>
					</div>

					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Department Number
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_DepartmentNumber" runat="server" CssClass="form-control input-sm" TabIndex="10" AutoPostBack="true" OnSelectedIndexChanged="DDL_DepartmentNumber_SelectedIndexChanged"></asp:DropDownList>
						</div>
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Send Transaction Email:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:CheckBox ID="CHK_SendTransactionEmail" runat="server" TabIndex="21" OnCheckedChanged="CHK_SendTransactionEmail_CheckedChanged" AutoPostBack="true" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">

						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Export Code:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtExportCode" runat="server" CssClass="form-control input-sm" TabIndex="11" MaxLength="25"></asp:TextBox>
							<asp:RegularExpressionValidator ID="REV_ExportCode" runat="server" ControlToValidate="txtExportCode" ErrorMessage="comma not allowed. Please remove comma and try again" ValidationExpression="^[^,]+$"
								Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>
						</div>
						<div id="divAdditionalEmail" runat="server">
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Email Transaction Receipt :</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtAdditionalEmail" runat="server" CssClass="form-control input-sm" MaxLength="256" TabIndex="23" data-toggle="tooltip" title="For multiple addresses, Separate using semicolon ( ; )"></asp:TextBox>
								<%--<asp:RegularExpressionValidator ID="RE_AdditionalEmail" runat="server" ControlToValidate="txtAdditionalEmail" Display="Dynamic" ErrorMessage="Please enter valid email." ForeColor="Red" SetFocusOnError="True" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>--%>
							</div>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div id="divCollectDiagnosticLogs" runat="server">
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>Collect Diagnostic Logs:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:CheckBox ID="chk_CollectDiagnosticLogs" runat="server" TabIndex="11" />
							</div>
						</div>
						<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Company
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="22" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
							<asp:Label ID="lblCustomer" runat="server" ForeColor="Red" Style="display: none;" Text="Please select company."></asp:Label>
							<%-- <asp:RequiredFieldValidator ID="RFD_Customer" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please select company."
                                ControlToValidate="DDL_Customer" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>--%>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>Is Terms And Privacy Policy Accepted:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:Label ID="lblTearmsAndPolicyAccepted" runat="server" Style="font-weight: 700"></asp:Label>
						</div>
						<div id="div2" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Date of Terms And Privacy Policy Accepted:
							</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:Label ID="lblDateOfAcceptance" runat="server" Style="font-weight: 700"></asp:Label>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 text-center col-xs-12">
						<asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
							UseSubmitBehavior="true" TabIndex="24" ValidationGroup="PersonelValidation" OnClientClick="return IsValidPhoneNumber();" />
						<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
							UseSubmitBehavior="False" TabIndex="25" OnClick="btnCancel_Click" />
						<asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
							UseSubmitBehavior="true" TabIndex="26" OnClientClick="return IsValidPhoneNumber();" ValidationGroup="PersonelValidation" OnClick="btnSaveAndAddNew_Click" />
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

			<asp:HiddenField runat="server" ID="hdfConfirmActiveInactive" Value="" />
			<asp:HiddenField runat="server" ID="hdfDirtyFlag" Value="" />
		</ContentTemplate>
	</asp:UpdatePanel>
	<script src="../Scripts/jquery.maskedinput.js"></script>
	<script>
		function loadFunction() {
			$('[data-toggle="tooltip"]').tooltip();
           <%-- $("#<%=txtPhoneNumber.ClientID%>").mask("999-999-9999");--%>
			if (isNaN(parseInt($("#<%= txtPreAuth.ClientID%>").val())) == true) {
				$("#<%= txtPreAuth.ClientID%>").val("0");
			}

			$("input").change(function () {
				$("#<%= hdfDirtyFlag.ClientID%>").val("1")
			});
		}

		$(function () {
			loadFunction();
		});
		//function ClosePopUp() {


		//    $("#btnClose").click();
		//    $('body').removeClass("modal-open");

		//    $('#BTN_PersonTiming').focus();
		//    enableDisableButtons(false)

		//}

		function ClosePopUpSite() {
			$("#<%=hdfDirtyFlag.ClientID%>").val("1");
			$("#btnCloseSite").click();
			$('body').removeClass("modal-open");
			$('#<%= chkSoftUpdate.ClientID%>').focus();
			enableDisableButtons(false)
		}

		function ClosePopUpFuelingTimes() {
			$("#<%= hdfDirtyFlag.ClientID%>").val("1")
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
			var isHubUser = $('#<%=CHK_HubUser.ClientID%>').prop('checked');
			var PersonName = $('#<%=txtPersonName.ClientID%>').val();
			var PersonPWD = $('#<%=txtUserPassword.ClientID%>').val();
			var PersonEmail = $('#<%=txtEmail.ClientID%>').val();
			var PhoneNumber = $('#<%=txtPhoneNumber.ClientID%>').val();
			var Customer = $('#<%=DDL_Customer.ClientID%>').val();
			var AccessLevels = $('#<%=DDL_AccessLevels.ClientID%>').val();
			var Department = $('#<%=DDL_Department.ClientID%>').val();

			var IsValid = true;


			if (PersonName == "") {
				IsValid = false;
				$("#<%=LBL_PersonError.ClientID()%>").show();
			}
			else {
				$("#<%=LBL_PersonError.ClientID()%>").hide();
			}

			if (Customer == "0") {
				IsValid = false;
				$("#<%=lblCustomer.ClientID()%>").show();
			}
			else {
				$("#<%=lblCustomer.ClientID()%>").hide();
			}


			if (AccessLevels == "0") {
				IsValid = false;
				$("#<%=lblAccessLevels.ClientID()%>").show();
			}
			else {
				$("#<%=lblAccessLevels.ClientID()%>").hide();
			}


			if (Department == "0") {
				IsValid = false;
				$("#<%=lbldepartment.ClientID()%>").show();
			}
			else {
				$("#<%=lbldepartment.ClientID()%>").hide();
			}


			if (isHubUser == false) {

				if (PersonPWD == "") {
					IsValid = false;
					$("#<%=LBLPassword.ClientID()%>").show();
				}
				else {
					$("#<%=LBLPassword.ClientID()%>").hide();
				}

				if (PersonEmail == "") {
					IsValid = false;
					$("#<%=lblEmail.ClientID()%>").show();
				}
				else {
					$("#<%=lblEmail.ClientID()%>").hide();
				}

				if (PhoneNumber == "") {
					IsValid = false;
					$("#<%=lblPhoneError.ClientID()%>").show();
				}
				else {
					$("#<%=lblPhoneError.ClientID()%>").hide();

					var phoneNumber = document.getElementById('<%=txtPhoneNumber.ClientID%>').value;
					//it accepts 850-294-2562(us phone number- req date 09-Dec-2016)
					//if (phoneNumber.match(/^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/)) {
					//phone number accept all number with only (-)+space symbols.
					if (phoneNumber.match(/^[- +()]*[0-9][- +()0-9]*$/)) {
						document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "none";
						//return true;
						//if (Page_ClientValidate("PersonelValidation"))
						//    return true;
						//else
						//    return false;
					}
					else {
						document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "";
						//Page_ClientValidate("PersonelValidation")
						IsValid = false;
					}
				}
			}


			loadFunction();

			if (IsValid == false) {
				return false;
			}
			else {
				if (Page_ClientValidate("PersonelValidation") == true) {

					return true;
				}
				else {
					return false;
				}
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

		//function OpenVehicleTypeBox() {
		//    $("#VehicleInput").val("");
		//    SearchVehicles();
		//    enableDisableButtons(true);
		//    $('#VehicleBox').modal({
		//        show: true,
		//        backdrop: 'static',
		//        keyboard: false
		//    });

		//}

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


		function OpenPersonTimingBox() {
			enableDisableButtons(true);
			$('#FuelingTimes').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}


	</script>
	<script language="javascript">

        <%--function SelectAllCheckboxesSpecificVehicles(spanChk) {

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
        }--%>

      <%--  function SelectboxVehicle(spanChk) {

            var IsChecked = spanChk.checked;
            if (IsChecked == false) {
                Parent = document.getElementById('<%= gv_Vehicles.ClientID%>');
                var checkBoxSelector = "#<%=gv_Vehicles.ClientID%> input[id*='chkAll']";

                $(checkBoxSelector).attr('checked', false);
            }
        }--%>

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

		function SelectAllCheckboxesSpecificFoFuelingTimes(spanChk) {

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
		}

		function SelectboxFuelingTimes(spanChk) {

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
		}

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

		function ConfirmActiveInactive(CheckActiveConfirm) {
			var checkActiveOrInactiveValue = CheckActiveConfirm.checked;
			$('#<%=hdfConfirmActiveInactive.ClientID %>').val(checkActiveOrInactiveValue);

			if ($("#<%= CHK_HubUser.ClientID %>").is(":checked")) {
				$('#<%= btnMyModalActiveInActiveNo.ClientID %>').click();
			}
			else {
				if (checkActiveOrInactiveValue) {
					$("#lblMessageActiveInactive").text("Do you also want to activate all devices associated with this account?");
				}
				else {
					$("#lblMessageActiveInactive").text("Do you also want to deactivate all devices associated with this account?");
				}

				$('#myModalActiveInActive').modal({
					show: true,
					backdrop: 'static',
					keyboard: false
				});
			}



		}

		function RedirectToVehicleMappingModel() {

			$('#myModalRedirectToVehicleMapping').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}

		function CloseRedirectToVehicleMappingModel() {
			$('#btnCloseRedirectToVehicleMappingModel').click();
			$('body').removeClass('modal-open');
			$('.modal-backdrop').remove();
		}





		function AcceptActiveInActive() {
			//$('#btnCloseActiveInactiveModel').click();
			$('body').removeClass('modal-open');
			$('.modal-backdrop').remove();
		}

        <%--  function SearchVehicles() {

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
        }--%>
</script>

	<div class="form-group col-md-6 col-sm-6" id="PWDHide" runat="server">
		&nbsp;
	</div>
</asp:Content>
