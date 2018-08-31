<%@ Page Title="All FluidSecure Hub" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllFluidSecureHub.aspx.vb" Inherits="Fuel_Secure.AllFluidSecureHub" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <script>

        function DeleteRecord() {
            $('#btnMyModalClose').click();

            var PersonId = $("#hdnPersonId").val();
            var UniqueUserId = $("#hdnUniqueUserId").val();

            $.ajax({
                type: "POST",
                url: "AllFluidSecureHub.aspx/DeleteRecord",
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
                $("#messageNew").text("Records deleted Successfully.");
                $("#ErrorMessageNew").hide();
                $("#messageNew").show();
                window.location.href = "/Master/AllFluidSecureHub"

            }
            else if (response.d == -2) {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("FluidSecure Hub is associated to a fueling Transaction(s), you must Delete Transaction(s) first to Delete this FluidSecure Hub.");
            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConfirm(PersonId, UniqueUserId) {
            var Role = '<%= Session("RoleName") %>';

            if (Role == "SuperAdmin") {
                $("#lblMessage").text("Are you sure you want to delete this Hub?");
            }
            else {
                $("#lblMessage").text("Are you sure you want to delete this FluidSecure Hub?");
            }

            $("#hdnPersonId").val(PersonId);
            $("#hdnUniqueUserId").val(UniqueUserId);

            $('#myModalSuccess').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function CheckActiveConfirm(IsApproved, UniqueUserId)
        {

        }
    </script>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading">
                    <h3 class="panel-title text-center">FluidSecure Hub</h3>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12  col-xs-12">
                        <div class="form-group col-md-2 col-sm-3 hidden-xs"></div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_ColumnName" AutoPostBack="True"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
                            <asp:DropDownList runat="server" ID="DDL_Dept" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
                            <asp:DropDownList ID="DDL_Customer" runat="server" Width="150px" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
                            <%--<asp:DropDownList ID="DDL_RoleId" runat="server" Width="150px" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>--%>
                           <%-- <asp:DropDownList ID="DDL_RequestFrom" runat="server" Width="150px" Visible="false" CssClass="form-control input-sm">
                                <asp:ListItem Text="Select Column" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Android" Value="A"></asp:ListItem>
                                <asp:ListItem Text="IPhone" Value="I"></asp:ListItem>
                                <asp:ListItem Text="Web Site" Value="W"></asp:ListItem>
                            </asp:DropDownList>--%>
                            <%--<asp:CheckBox ID="Chk_SoftUpdate" runat="server" Checked="false" Visible="false" />--%>
                            <asp:CheckBox ID="Chk_IsApproved" runat="server" Checked="false" Visible="false" />
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
                     <div class="row col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 10px;margin-left: 5px">
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-left">
                            <b><asp:Label runat="server" ID="lblTotalNumberOfRecords"></asp:Label></b>
                        </div>
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-right">
                            <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new FluidSecure Hub" OnClick="btn_New_Click" />
                        </div>
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvPersonnel" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true"
                            DataKeyNames="Id,PersonId,Email" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                            <Columns>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "PersonId")%>,'<%# DataBinder.Eval(Container.DataItem, "Id")%>')">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                               <%-- <asp:TemplateField HeaderText="Reset Password">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkResetPassword" ForeColor="#428BCA" runat="server" Text="Reset Password" OnClick="linkResetPassword_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Font-Names="Arial" Font-Size="10pt" HorizontalAlign="Center" VerticalAlign="Middle"
                                        Width="70px" />
                                </asp:TemplateField>--%>

                                <asp:TemplateField HeaderText="Active">
                                    <ItemTemplate>
                                        <%-- <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "PersonId")%>,'<%# DataBinder.Eval(Container.DataItem, "Id")%>')">Active</a>--%>
                                        <asp:CheckBox ID="CHK_InActive" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsApproved")%>' OnCheckedChanged="CHK_InActive_CheckedChanged" AutoPostBack="true" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>

                                <asp:TemplateField SortExpression="PersonName" ItemStyle-HorizontalAlign="Center" HeaderText="FluidSecure Hub Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPersonName" Text='<%# DataBinder.Eval(Container.DataItem, "PersonName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="SiteName" ItemStyle-HorizontalAlign="Center" HeaderText="Site Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblHubSiteName" Text='<%# DataBinder.Eval(Container.DataItem, "SiteName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                               <%-- <asp:TemplateField SortExpression="Email" ItemStyle-HorizontalAlign="Center" HeaderText="Email">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmail" Text='<%# DataBinder.Eval(Container.DataItem, "Email")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                              <%--  <asp:TemplateField SortExpression="Roles" ItemStyle-HorizontalAlign="Left"
                                    HeaderText="Roles">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRoles" Text='<%# DataBinder.Eval(Container.DataItem, "Roles")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:TemplateField SortExpression="CustomerName" ItemStyle-HorizontalAlign="Left" HeaderText="Company">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCustomerName" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="DeptName" ItemStyle-HorizontalAlign="Left" HeaderText="Department Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDeptName" Text='<%# DataBinder.Eval(Container.DataItem, "DeptName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="IMEI_UDID" ItemStyle-HorizontalAlign="Left" HeaderText="IMEI Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIMEI_UDID" Text='<%# DataBinder.Eval(Container.DataItem, "IMEI_UDID")%>'
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
                    <h3 class="modal-title text-center">FluidSecure</h3>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hdnPersonId" />
                    <input type="hidden" id="hdnUniqueUserId" />
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
