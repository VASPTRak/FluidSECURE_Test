<%@ Page Title="All Companies" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllCompanies.aspx.vb" Inherits="Fuel_Secure.AllCompanies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script>

        function DeleteRecord() {
            $('#btnMyModalClose').click();

            var CompanyId = $("#hdnCompanyId").val();

            $.ajax({
                type: "POST",
                url: "AllCompanies.aspx/DeleteRecord",
                data: '{CompanyId: "' + CompanyId + '" }',
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

                window.location.href = "/Master/AllCompanies"

            }
            else if (response.d == -2) {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("This Company has been assigned to departments, so you cannot delete this company.'");
            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConfirm(CompanyId) {
            var Role = '<%= Session("RoleName") %>';

            if (Role == "SuperAdmin") {
                $("#lblMessage").text("Are you sure you want to delete this Company.  All associated personnels, departments, vehicles, sites, hose will also be deleted.");
            }
            else {
                $("#lblMessage").text("Are you sure you want to delete this Company?");
            }

            $("#hdnCompanyId").val(CompanyId);

            $('#myModalSuccess').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function ConfirmActiveInactive(CheckActiveConfirm, CustomerId) {
            var checkActiveOrInactiveValue = CheckActiveConfirm.checked;
            var CheckValue = 0
            if (checkActiveOrInactiveValue == true) {
                CheckValue = 1;
            }

            $('#<%=hdfConfirmActiveInactive.ClientID %>').val(checkActiveOrInactiveValue);
            $('#<%=hdfIdActiveInactive.ClientID %>').val(CustomerId);

            if (checkActiveOrInactiveValue) {
                $("#lblMessageActiveInactive").text("Are you sure you want to Activate this Company?");
            }
            else {
                $("#lblMessageActiveInactive").text("Are you sure you want to Deactivate this Company?");
            }

            $('#myModalActiveInActive').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function ActivateInactivateCompany(condition) {
            if (condition == 1) {
                $.ajax({
                    type: 'POST',
                    url: 'AllCompanies.aspx/ActiveInActiveCompanyRecord',
                    contentType: 'application/json; charset=utf-8',
                    data: "{ 'CustomerId': '" + $('#<%=hdfIdActiveInactive.ClientID %>').val() + "', 'CheckActiveConfirm': '" + $('#<%=hdfConfirmActiveInactive.ClientID %>').val() + "' }",
                dataType: 'json',
                success: OnActiveSuccess,
                failure: function (response) {
                    alert(response.d);
                }
                });
            }
            else {
                window.location.href = "/Master/AllCompanies"
            }

    }

    function OnActiveSuccess(response) {

        if (response.d == 1) {
            $("#messageNew").text("Record saved Successfully.");
            $("#ErrorMessageNew").hide();
            $("#messageNew").show();
        }
        else {
            $("#ErrorMessageNew").show();
            $("#messageNew").hide();
            $("#ErrorMessageNew").text("Records save failed.");
        }
    }


    </script>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="hdfConfirmActiveInactive" Value="" />
            <asp:HiddenField runat="server" ID="hdfIdActiveInactive" Value="" />
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading">
                    <h3 class="panel-title text-center">Companies</h3>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12  col-xs-12">
                        <div class="form-group col-md-2 col-sm-3 hidden-xs"></div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_Column" AutoPostBack="True" OnSelectedIndexChanged="DDL_Column_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
                            <asp:CheckBox ID="chk_Active" runat="server" Visible="false" TabIndex="1" />
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
                    <div class="row col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 10px; margin-left: 5px">
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-left">
                            <b>
                                <asp:Label runat="server" ID="lblTotalNumberOfRecords"></asp:Label></b>
                        </div>
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-right">
                            <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new Company" OnClick="btn_New_Click" />
                        </div>
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvCust" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true"
                            DataKeyNames="CustomerId" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
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
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "CustomerId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Active">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CHK_InActive" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsCustomerActive")%>' OnClick='<%#String.Format("javascript:ConfirmActiveInactive(this,""{0}"")", Eval("CustomerId")) %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="CompanyNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Company Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCustomerId" Text='<%# DataBinder.Eval(Container.DataItem, "CompanyNumber")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="CustomerName" ItemStyle-HorizontalAlign="Center" HeaderText="Company Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCustomerName" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="ContactName" ItemStyle-HorizontalAlign="Left"
                                    HeaderText="Contact Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblContactName" Text='<%# DataBinder.Eval(Container.DataItem, "ContactName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="ContactNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Contact Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblContactNumber" Text='<%# DataBinder.Eval(Container.DataItem, "ContactNumber")%>'
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
                    <input type="hidden" id="hdnCompanyId" />
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
                    <button type="button" id="btnCloseActiveInactiveModel" class="btn btn-default" data-dismiss="modal" onclick="ActivateInactivateCompany(2);">No</button>
                    <asp:Button runat="server" ID="btnMyModalActiveInActiveSuccess" class="btn btn-success" OnClientClick="ActivateInactivateCompany(1);" Text="Yes" />

                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

</asp:Content>
