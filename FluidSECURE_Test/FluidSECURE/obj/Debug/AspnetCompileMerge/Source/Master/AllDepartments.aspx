<%@ Page Title="All Departments" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllDepartments.aspx.vb" Inherits="Fuel_Secure.AllDepartment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>

        function DeleteRecord() {
            $('#btnMyModalClose').click();

            var DeptId = $("#hdnDeptId").val();

            $.ajax({
                type: "POST",
                url: "AllDepartments.aspx/DeleteRecord",
                data: '{DeptId: "' + DeptId + '" }',
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

                window.location.href = "/Master/AllDepartments"

            }
            else if (response.d == -2) {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("DEPARTMENT CANNOT BE DELETED - This Department is assigned to a Vehicle(s) or Personnel(s), please reassign the Vehicle or Personnel to another Department and return to Delete the Department.");
            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConfirm(DeptId) {
            var Role = '<%= Session("RoleName") %>';

            if (Role == "SuperAdmin") {
                $("#lblMessage").text("Are you sure you want to delete this Department.  All associated Users and Vehicles will also be deleted.");
            }
            else {
                $("#lblMessage").text("Are you sure you want to delete this Department?");
            }

            $("#hdnDeptId").val(DeptId);

            $('#myModalSuccess').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }
    </script>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading">
                    <h3 class="panel-title text-center">Departments</h3>
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
                    <div class="row col-md-12 col-sm-12 col-xs-12 text-right" style="margin-bottom: 10px;">
                        <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new Department" OnClick="btn_New_Click" />
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvDept" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" DataKeyNames="DeptId,IsDefault" OnRowDataBound="gvDept_RowDataBound"
                            AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
                            <Columns>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "DeptId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="NUMBER" ItemStyle-HorizontalAlign="Center" HeaderText="Department Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNUMBER" Text='<%# DataBinder.Eval(Container.DataItem, "NUMBER")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="Name" ItemStyle-HorizontalAlign="Center" HeaderText="Department Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblName" Text='<%# DataBinder.Eval(Container.DataItem, "Name")%>'
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

    <!--alert message popup-->
    <div class="modal fade" tabindex="-1" role="dialog" id="myModalSuccess">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title text-center">FluidSecure</h3>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hdnDeptId" />
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
