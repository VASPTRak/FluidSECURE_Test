<%@ Page Title="All Personnel" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllPersonnel.aspx.vb" Inherits="Fuel_Secure.AllPersonnel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<script>

		function DeleteRecord() {
			$('#btnMyModalClose').click();

			var PersonId = $("#hdnPersonId").val();
			var UniqueUserId = $("#hdnUniqueUserId").val();

			$.ajax({
				type: "POST",
				url: "AllPersonnel.aspx/DeleteRecord",
				data: JSON.stringify({ PersonId: PersonId, UniqueUserId: UniqueUserId }),
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
				$("#messageNew").text("<%= Fuel_Secure.My.Resources.Resource.UserDeleteMsg2 %>");
				$("#ErrorMessageNew").hide();
				$("#messageNew").show();
				window.location.href = "/Master/AllPersonnel"

			}
			else if (response.d == -2) {
				$("#ErrorMessageNew").show();
				$("#messageNew").hide();

				$("#ErrorMessageNew").text("<%= Fuel_Secure.My.Resources.Resource.UserDeleteMsg3 %>");
			}
			else {
				$("#ErrorMessageNew").show();
				$("#messageNew").hide();

				$("#ErrorMessageNew").text("<%= Fuel_Secure.My.Resources.Resource.UserDeleteMsg4%>");
			}
		}

		function CheckConfirm(PersonId, UniqueUserId) {
			var Role = '<%= Session("RoleName") %>';

			if (Role == "SuperAdmin") {
				$("#lblMessage").text("<%= Fuel_Secure.My.Resources.Resource.UserDeleteMsg1 %>");
			}
			else {
				$("#lblMessage").text("<%= Fuel_Secure.My.Resources.Resource.UserDeleteMsg1 %>");
			}

			$("#hdnPersonId").val(PersonId);
			$("#hdnUniqueUserId").val(UniqueUserId);

			$('#myModalSuccess').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}

		function CheckActiveConfirm(IsApproved, UniqueUserId) {

		}

		function ConfirmActiveInactive(CheckActiveConfirm, Id, PersonId, UserEmail, IsUserForHub) {
			var checkActiveOrInactiveValue = CheckActiveConfirm.checked;
			$('#<%=hdfConfirmActiveInactive.ClientID %>').val(checkActiveOrInactiveValue);
			$('#<%=hdfIdActiveInactive.ClientID %>').val(Id);
			$('#<%=hdfPersonIdActiveInactive.ClientID %>').val(PersonId);
			$('#<%=hdfUserEmailActiveInactive.ClientID %>').val(UserEmail);
			$('#<%=hdfIsUserForHubActiveInactive.ClientID %>').val(IsUserForHub);
			if (IsUserForHub == "False") {
				if (checkActiveOrInactiveValue) {
					$("#lblMessageActiveInactive").text("<%= Fuel_Secure.My.Resources.Resource.CheckActiveValue %>");
				}
				else {
					$("#lblMessageActiveInactive").text("<%= Fuel_Secure.My.Resources.Resource.CheckInActiveValue %>");
				}

				$('#myModalActiveInActive').modal({
					show: true,
					backdrop: 'static',
					keyboard: false
				});
			}
			else {
				$('#<%= btnMyModalActiveInActiveNo.ClientID %>').click();
			}



		}

	</script>
	<asp:UpdatePanel ID="up_Main" runat="server">
		<ContentTemplate>
			<div class="panel panel-primary" style="margin: 20px;">
				<div class="panel-heading">
					<h3 class="panel-title text-center"><%= Fuel_Secure.My.Resources.Resource.Personnel %></h3>
				</div>
				<div class="panel-body">
					<asp:HiddenField runat="server" ID="hdfConfirmActiveInactive" Value="" />
					<asp:HiddenField runat="server" ID="hdfIdActiveInactive" Value="" />
					<asp:HiddenField runat="server" ID="hdfPersonIdActiveInactive" Value="" />
					<asp:HiddenField runat="server" ID="hdfUserEmailActiveInactive" Value="" />
					<asp:HiddenField runat="server" ID="hdfIsUserForHubActiveInactive" Value="" />
					<div class="row col-md-12 col-sm-12  col-xs-12">
						<div class="form-group col-md-2 col-sm-3 hidden-xs"></div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_ColumnName" AutoPostBack="True"></asp:DropDownList>
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
							<asp:DropDownList runat="server" ID="DDL_Dept" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:DropDownList ID="DDL_Customer" runat="server" Width="150px" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:DropDownList ID="DDL_RoleId" runat="server" Width="150px" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
							<asp:DropDownList ID="DDL_RequestFrom" runat="server" Width="150px" Visible="false" CssClass="form-control input-sm">
								<asp:ListItem Text="Select Column" Value="0"></asp:ListItem>
								<asp:ListItem Text="Android" Value="A"></asp:ListItem>
								<asp:ListItem Text="IPhone" Value="I"></asp:ListItem>
								<asp:ListItem Text="Web Site" Value="W"></asp:ListItem>
							</asp:DropDownList>
							<asp:CheckBox ID="Chk_SoftUpdate" runat="server" Checked="false" Visible="false" />
							<asp:CheckBox ID="Chk_IsApproved" runat="server" Checked="false" Visible="false" />
						</div>
						<div class="form-group col-md-3 col-sm-3  col-xs-12">
							<asp:Button ID="btnSearch" CssClass="btn btn-primary" runat="server" OnClick="btnSearch_Click" TabIndex="11" Text="Search" />
						</div>
						<div class="form-group col-md-1 col-sm-3 hidden-xs"></div>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center text-danger"><%= Fuel_Secure.My.Resources.Resource.SearchDesc %></p>
					</div>
					<div class="row col-md-12 col-sm-12 col-xs-12">
						<p class="text-center green" id="message" runat="server"></p>
						<p class="text-center red" id="ErrorMessage" runat="server"></p>
						<p class="text-center green" id="messageNew"></p>
						<p class="text-center red" id="ErrorMessageNew"></p>
					</div>
                     <div class="row col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 10px;margin-left: 5px">
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-left">
                            <b><asp:Label runat="server" ID="lblTotalNumberOfRecords"></asp:Label></b>
                        </div>
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-right">
                            <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add New Personnel" OnClick="btn_New_Click" />
                        </div>
                    </div>
					<div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

						<asp:GridView ID="gvPersonnel" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" OnRowDataBound="gvPersonnel_RowDataBound"
							DataKeyNames="Id,PersonId,Email,IsUserForHub" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
							<PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                            <Columns>
								<asp:TemplateField>
									<HeaderTemplate>
										<asp:Label ID="lbl1Edit" runat="server"><%= Fuel_Secure.My.Resources.Resource.Edit %></asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" OnClick="linkEdit_Click"><%= Fuel_Secure.My.Resources.Resource.Edit %></asp:LinkButton>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Delete">
									<HeaderTemplate>
										<asp:Label ID="lbl1Delete" runat="server"><%= Fuel_Secure.My.Resources.Resource.Delete %></asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "PersonId")%>,'<%# DataBinder.Eval(Container.DataItem, "Id")%>')"><%= Fuel_Secure.My.Resources.Resource.Delete %></a>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Reset Password">
									<HeaderTemplate>
										<asp:Label ID="lbl1Reset" runat="server"><%= Fuel_Secure.My.Resources.Resource.ResetPassword %></asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:LinkButton ID="linkResetPassword" ForeColor="#428BCA" runat="server" OnClick="linkResetPassword_Click"><%= Fuel_Secure.My.Resources.Resource.ResetPassword %></asp:LinkButton>
									</ItemTemplate>
									<ItemStyle Font-Names="Arial" Font-Size="10pt" HorizontalAlign="Center" VerticalAlign="Middle"
										Width="70px" />
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Diagnostic Logs">
									<%--	<HeaderTemplate>
										<asp:Label ID="lbl1Delete" runat="server"><%= Fuel_Secure.My.Resources.Resource.Delete %></asp:Label>
									</HeaderTemplate>--%>
									<ItemTemplate>
										<asp:LinkButton ID="linkViewLogs" runat="server" ForeColor="#428BCA" OnClick="linkViewLogs_Click">Diagnostic Logs</asp:LinkButton>
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Active">
									<HeaderTemplate>
										<asp:Label ID="lbl1Active" runat="server"><%= Fuel_Secure.My.Resources.Resource.Active %></asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:CheckBox ID="CHK_InActive" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsApproved")%>' OnClick='<%#String.Format("javascript:ConfirmActiveInactive(this,""{0}"",""{1}"",""{2}"",""{3}"")", Eval("Id"), Eval("PersonId"), Eval("Email"), Eval("IsUserForHub")) %>' />
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
								</asp:TemplateField>

								<asp:TemplateField SortExpression="PersonName" ItemStyle-HorizontalAlign="Center" HeaderText="Person Name">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1PersonName" runat="server" Text='<%# Fuel_Secure.My.Resources.Resource.PersonName %>'
											CommandName="Sort" CommandArgument="PersonName" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblPersonName" Text='<%# DataBinder.Eval(Container.DataItem, "PersonName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="Email" ItemStyle-HorizontalAlign="Center" HeaderText="Email">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1Email1" runat="server" Text='<%# Fuel_Secure.My.Resources.Resource.Email %>'
											CommandName="Sort" CommandArgument="Email" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblEmail" Text='<%# DataBinder.Eval(Container.DataItem, "Email")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="DisplayName" ItemStyle-HorizontalAlign="Left" HeaderText="Roles">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1Roles" runat="server" Text='<%#	Fuel_Secure.My.Resources.Resource.Roles %>'
											CommandName="Sort" CommandArgument="DisplayName" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblRoles" Text='<%# DataBinder.Eval(Container.DataItem, "DisplayName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="CustomerName" ItemStyle-HorizontalAlign="Left" HeaderText="Company">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1CustomerName" runat="server" Text='<%# Fuel_Secure.My.Resources.Resource.Company %>'
											CommandName="Sort" CommandArgument="CustomerName" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblCustomerName" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="DeptName" ItemStyle-HorizontalAlign="Left" HeaderText="Department Name">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1DeptName" runat="server" Text='<%# Fuel_Secure.My.Resources.Resource.DepartmentName %>'
											CommandName="Sort" CommandArgument="DeptName" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblDeptName" Text='<%# DataBinder.Eval(Container.DataItem, "DeptName")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="DeptNumber" ItemStyle-HorizontalAlign="Left" HeaderText="Department Number">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1DeptNumber" runat="server" Text='<%# Fuel_Secure.My.Resources.Resource.DepartmentNumber %>'
											CommandName="Sort" CommandArgument="DeptNumber" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblDeptNumber" Text='<%# DataBinder.Eval(Container.DataItem, "DeptNumber")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField SortExpression="IMEI_UDID" ItemStyle-HorizontalAlign="Left" HeaderText="IMEI Number">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1IMEi" runat="server" Text='<%# Fuel_Secure.My.Resources.Resource.IMEINumber %>'
											CommandName="Sort" CommandArgument="IMEI_UDID" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblIMEI_UDID" Text='<%# DataBinder.Eval(Container.DataItem, "IMEI_UDID")%>'
											runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<%--<asp:TemplateField SortExpression="IsApproved" ItemStyle-HorizontalAlign="Left" HeaderText="Active">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsApproved" Text='<%# IIf(Boolean.Parse(DataBinder.Eval(Container.DataItem, "IsApproved").ToString()), "Yes", "No")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
								<asp:TemplateField SortExpression="TempPinNumber" ItemStyle-HorizontalAlign="Left" HeaderText="Pin Number">
									<HeaderTemplate>
										<asp:LinkButton ID="lbl1PIN" runat="server" Text='<%# Fuel_Secure.My.Resources.Resource.PinNumber %>'
											CommandName="Sort" CommandArgument="PinNumber" ForeColor="Black"></asp:LinkButton>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="lblPinNumber" Text='<%# DataBinder.Eval(Container.DataItem, "PinNumber")%>'
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

	<!--alert message popup-->
	<div class="modal fade" tabindex="-1" role="dialog" id="myModalSuccess">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<h3 class="modal-title text-center"><%= Fuel_Secure.My.Resources.Resource.FluidSecure %></h3>
				</div>
				<div class="modal-body">
					<input type="hidden" id="hdnPersonId" />
					<input type="hidden" id="hdnUniqueUserId" />
					<h4 id="lblMessage"></h4>
				</div>
				<div class="modal-footer nextButton">
					<button type="button" id="btnMyModalClose" class="btn btn-default" data-dismiss="modal" onclick="return false;"><%= Fuel_Secure.My.Resources.Resource.No %></button>
					<input type="button" id="btnMyModalSuccess" class="btn btn-success" onclick="DeleteRecord()" value="<%= Fuel_Secure.My.Resources.Resource.Yes %>" />
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
					<h3 class="modal-title text-center"><%= Fuel_Secure.My.Resources.Resource.FluidSecure %></h3>
				</div>
				<div class="modal-body">
					<h4 id="lblMessageActiveInactive"></h4>
				</div>
				<div class="modal-footer nextButton">
					<button type="button" id="btnCloseActiveInactiveModel" class="btn btn-default" data-dismiss="modal" style="visibility: hidden"></button>
					<%--<input type="button" id="btnMyModalActiveInActiveClose" class="btn btn-default" onclick="SetActiveInactive('No')" value="<%= Fuel_Secure.My.Resources.Resource.No %>" />
                    <input type="button" id="btnMyModalActiveInActiveSuccess" class="btn btn-success" onclick="SetActiveInactive('Yes')" value="<%= Fuel_Secure.My.Resources.Resource.Yes %>" />--%>

					<asp:Button runat="server" ID="btnMyModalActiveInActiveNo" class="btn btn-default" OnClick="btnMyModalActiveInActiveNo_Click" Text="" />
					<asp:Button runat="server" ID="btnMyModalActiveInActiveSuccess" class="btn btn-success" OnClick="btnMyModalActiveInActiveSuccess_Click" Text="" />

				</div>
			</div>
			<!-- /.modal-content -->
		</div>
		<!-- /.modal-dialog -->
	</div>
	<!-- /.modal -->

</asp:Content>
