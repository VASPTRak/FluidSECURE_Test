<%@ Page Title="Personnel Options" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PersonnelOptionsScreen.aspx.vb" Inherits="Fuel_Secure.PersonnelOptionsScreen" %>

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
						<div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
						</div>
						<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Company
                                <label class="text-danger font-required">[required]</label>:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:DropDownList ID="DDL_Customer" runat="server" TabIndex="2" CssClass="form-control input-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged" ValidationGroup="PersonnelOptionsValidation"></asp:DropDownList>
							<%--<asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonnelOptionsValidation"></asp:RequiredFieldValidator>--%>
							<label ID="lblerrorcompany" style="color:red;"></label>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12" id="Div1" runat="server">
						<div class="form-group col-md-6 col-sm-3 textright col-xs-12">
							<label>
								Assign all personnel to all vehicles:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:Button ID="btn_assign" runat="server" Text="Assign " OnClientClick="return validateCompany()" OnClick="btn_Assign_Click" ValidationGroup="PersonnelOptionsValidation" CssClass="btn btn-primary" />
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12" id="Div2" runat="server">
						<div class="form-group col-md-6 col-sm-3 textright col-xs-12">
							<label>
								Assign authorized Fueling Times to all Personnel:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<input type="button" id="BTN_PersonTiming" tabindex="17" onclick="OpenPersonTimingBox();" value="Click to add Fueling Times" Class="btn btn-primary"/>
						</div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12" id="Div4" runat="server">
						<div class="form-group col-md-6 col-sm-3 textright col-xs-12">
							<label>
								Assign Authorized Fueling links to all Personnel:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<input type="button" id="BTN_PersonSite" tabindex="18" onclick="OpenPersonSiteBox();" value="Click to add FluidSecure links" Class="btn btn-primary"/>
						</div>
					</div>
					<br>
					<div class="row col-md-12 col-sm-12 col-xs-12" id="Div3" runat="server">
						<div class="form-group col-md-6 col-sm-3 col-xs-12 hidden-xs">
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:Button ID="btn_Return" runat="server" CssClass="btn btn-default" Text="Cancel" PostBackUrl="~/Master/AllPersonnel.aspx" />
						</div>
					</div>
				</div>
			</div>
			<asp:HiddenField runat="server" ID="hdfEnabDisOdo" Value="0" />
			<asp:HiddenField runat="server" ID="hdfOdoReasonability" Value="0" />
			<asp:HiddenField runat="server" ID="hdfOdoHours" Value="0" />
			<asp:HiddenField runat="server" ID="hdfOdoHoursLimit" Value="0" />

		</ContentTemplate>
	</asp:UpdatePanel>

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
					<asp:Button ID="btnAssignHose" runat="server" CssClass="btn btn-default" Text="Assign" OnClientClick="ClosePopUpSite()" OnClick="btnAssignHose_Click"/>
					<input type="button" id="btnCloseSite" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
					<asp:Button ID="btnCancelSite" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpSite()" />
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
					<asp:Button ID="btnAssignTime" runat="server" CssClass="btn btn-default" Text="Assign" OnClientClick="ClosePopUpFuelingTimes()" OnClick="btnAssignTime_Click"/>
					<input type="button" id="btnCloseFuelingTimes" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
					<asp:Button ID="btnCancelFuelingTimes" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpFuelingTimes()" />
				</div>
			</div>
			<!-- /.modal-content -->
		</div>
		<!-- /.modal-dialog -->
	</div>
	<!-- /.modal -->
	
	<div class="modal fade" tabindex="-1" role="dialog" id="VehAssignModel">
        <div class="modal-dialog modal-lg">
            <div class="modal-content" style="width: 104%;">
                <div class="modal-header">
                    <h5 class="modal-title text-center">FluidSecure</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12" style="padding: 0px;margin-left:0px">
                        <asp:Label ID="lblVehAssign" runat="server" Text="">Are you sure you want to assign all personnel to all vehicles ?</asp:Label><br />
                        <h4 style="float:left">Selecting this will override all existing settings, and cannot be undone.</h4>
                    </div>
                    <div class="modal-footer nextButton">
                        <asp:Button ID="btnVehAssignOk" runat="server" CssClass="btn btn-success" OnClientClick="CloseVehAssignBox()" Text="Yes" OnClick="btnVehAssignOk_Click" />
                        <%--<input type="button" id="btnVehOdoOk" class="btn btn-success" onclick="CloseVehOdoBox()" value="Yes" />--%>
                        <input type="button" id="btnCloseVehAssign" class="btn btn-success" data-dismiss="modal" style="display: none;" value="No" />
                        <asp:Button ID="btnCancelVehAssign" runat="server" CssClass="btn btn-default" Text="No" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </div>
	<script>
		function ClosePopUpSite() {

			$("#btnCloseSite").click();
			$('body').removeClass("modal-open");

		}

		function ClosePopUpFuelingTimes() {

			$("#btnCloseFuelingTimes").click();
			$('body').removeClass("modal-open");
			$('#BTN_PersonSite').focus();

		}

		function OpenPersonSiteBox() {
			var em = document.getElementById("lblerrorcompany");
			var flag = validateCompany()
			if (flag == true) {
				$("#siteInput").val("");
				SearchSite();
				$('#PersonSites').modal({
					show: true,
					backdrop: 'static',
					keyboard: false
				});
				em.innerHTML = ""
			}
			else {
				em.innerHTML = "Please select Company."
			}
		}


		function OpenPersonTimingBox() {
			var em = document.getElementById("lblerrorcompany");
			var flag = validateCompany()
			if (flag == true) {
			$('#FuelingTimes').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
			em.innerHTML = ""
			}
			else {
				em.innerHTML = "Please select Company."
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

		function OpenVehAssignModelBox() {
			$('#VehAssignModel').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}
		function CloseVehAssignBox() {
			$("#btnCloseVehAssign").click();
			$('body').removeClass("modal-open");
		}

		function validateCompany()
		{
			var em = document.getElementById("lblerrorcompany");
			var e = document.getElementById('<%= DDL_Customer.ClientID %>');
			var strUser = e.options[e.selectedIndex].value;
			if (strUser == "0")
			{
				em.innerHTML = "Please select Company."
				return false;
			}
			else {
				em.innerHTML = ""
				return true;
			}
		}
	</script>
</asp:Content>
