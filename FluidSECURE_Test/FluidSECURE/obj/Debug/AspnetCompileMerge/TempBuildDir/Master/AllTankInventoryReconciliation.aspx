<%@ Page Title="All Tank Inventory Reconciliation" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllTankInventoryReconciliation.aspx.vb" Inherits="Fuel_Secure.AllTankInventoryReconciliation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>

        function DeleteRecord() {
            $('#btnMyModalClose').click();

            var TankInventoryId = $("#hdnTankInventoryId").val();

            $.ajax({
                type: "POST",
                url: "AllTankInventoryReconciliation.aspx/DeleteRecord",
                data: '{TankInventoryId: "' + TankInventoryId + '" }',
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

                window.location.href = "/Master/AllTankInventoryReconciliation?Type=" + $('#<%= hdnEntryType.ClientID %>').val()

            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConfirm(TankInventoryId) {
            $("#lblMessage").text("Are you sure you want to delete ?");
            $("#hdnTankInventoryId").val(TankInventoryId);

            $('#myModalSuccess').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }
    </script>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="hdnEntryType" Value="" />
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading">
                    <h3 class="panel-title text-center">Tank Inventory Reconciliation</h3>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12  col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 hidden-xs" id="hiddenDiv" runat="server"></div>
                        <div class="form-group col-md-2 col-sm-3  col-xs-12">
                            <asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_ColumnName" AutoPostBack="True"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12" id="OtherThanDate" runat="server">
                            <asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
                            <asp:DropDownList runat="server" ID="DDL_Customer" Visible="false" CssClass="form-control input-sm"></asp:DropDownList>
                            <asp:DropDownList runat="server" ID="DDL_Datetype" Visible="false" CssClass="form-control input-sm">
                                <asp:ListItem Text="Select Date Type" Value=""></asp:ListItem>
                                <asp:ListItem Text="Start Date" Value="s"></asp:ListItem>
                                <asp:ListItem Text="End Date" Value="e"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="row col-md-7 col-sm-12 col-xs-12" id="TransDate" runat="server">
                            <div class="form-group col-md-2 col-sm-3 col-xs-12" style="padding: 0;">
                                <label>
                                     Date From:</label>
                            </div>
                            <div class="form-group col-md-4 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtDateFrom" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="2"></asp:TextBox>
                            </div>

                            <div class="form-group col-md-2 col-sm-3 col-xs-12" style="padding: 0;">
                                <label>
                                     Date To:</label>
                            </div>
                            <div class="form-group col-md-4 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtDateTo" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-md-2 col-sm-3  col-xs-12">
                            <asp:Button ID="btnSearch" CssClass="btn btn-primary" runat="server" Text="Search" OnClick="btnSearch_Click" TabIndex="4" />
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
                        <asp:Button runat="server" ID="btnTANKRECONICIATIONREPORT" CssClass="btn btn-warning" Text="TANK RECONCILIATION REPORT" OnClick="btnTANKRECONICIATIONREPORT_Click"></asp:Button>
                        <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new Tank Inventory Reconciliation" OnClick="btn_New_Click" />
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvTankInvt" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" DataKeyNames="TankInventoryId"
                            AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
                            <Columns>
                                <asp:TemplateField HeaderText="Edit" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "TankInventoryId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                 <asp:TemplateField SortExpression="TankNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Tank Number" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTankNumber" Text='<%# DataBinder.Eval(Container.DataItem, "TankNumber")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="InventoryDate" ItemStyle-HorizontalAlign="Center" HeaderText="Date" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblInventoryDate" Text='<%# DataBinder.Eval(Container.DataItem, "InventoryDate")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField SortExpression="InventoryTime" ItemStyle-HorizontalAlign="Center" HeaderText="Time" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblInventoryTime" Text='<%# DataBinder.Eval(Container.DataItem, "InventoryTime")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField SortExpression="Quantity" ItemStyle-HorizontalAlign="Center" HeaderText="Quantity" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblQuantity" Text='<%# DataBinder.Eval(Container.DataItem, "Quantity")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                  <asp:TemplateField SortExpression="ENTRY_TYPE" ItemStyle-HorizontalAlign="Center" HeaderText="ENTRY TYPE" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblENTRY_TYPE" Text='<%# DataBinder.Eval(Container.DataItem, "ENTRY_TYPE")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField SortExpression="DateType" ItemStyle-HorizontalAlign="Center" HeaderText="Date Entry Type" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDateType" Text='<%# DataBinder.Eval(Container.DataItem, "DateType")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="CustomerName" ItemStyle-HorizontalAlign="Center" HeaderText="Company" HeaderStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign ="Middle">
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
                    <input type="hidden" id="hdnTankInventoryId" />
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

    <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">
    <script type="text/javascript">

        function LoadDateTimeControl() {
            $("[id$=txtDateFrom]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $("[id$=txtDateTo]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });
        }
    </script>
</asp:Content>