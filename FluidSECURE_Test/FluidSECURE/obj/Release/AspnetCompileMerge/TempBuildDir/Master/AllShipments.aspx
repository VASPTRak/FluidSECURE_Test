<%@ Page Title="All Shipments" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllShipments.aspx.vb" Inherits="Fuel_Secure.AllShipments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script>

        function DeleteRecord() {
            $('#btnMyModalClose').click();

            var ShipmentId = $("#hdnShipmentId").val();

            $.ajax({
                type: "POST",
                url: "AllShipments.aspx/DeleteRecord",
                data: '{ShipmentId: "' + ShipmentId + '" }',
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

                window.location.href = "/Master/AllShipments"

            }
            else if (response.d == -2) {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Company is registered to this shipment, so you cannot delete this shipment.");
            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConfirm(ShipmentId) {
            var Role = '<%= Session("RoleName") %>';

            if (Role == "SuperAdmin" || Role == "Support") {

                $("#lblMessage").text("Are you sure you want to delete this shipment?");
                $("#hdnShipmentId").val(ShipmentId);

                $('#myModalSuccess').modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });
            }
            else {
                return;
            }
        }
    </script>
    <style>
        .table tr th {
            text-align: center;
        }
    </style>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading">
                    <h3 class="panel-title text-center">Shipments</h3>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12  col-xs-12">
                        <div class="form-group col-md-2 col-sm-3 hidden-xs"></div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_ColumnName"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
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
                           <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new Shipment" OnClick="btn_New_Click" />
                        </div>
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvShipment" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" DataKeyNames="ShipmentId"
                            AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria" OnRowDataBound="gvShipment_RowDataBound">
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
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "ShipmentId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="FluidSecureUnitName" ItemStyle-HorizontalAlign="Center" HeaderText="FluidSecure Link Name" HeaderStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFluidSecureUnitName" Text='<%# DataBinder.Eval(Container.DataItem, "FluidSecureUnitName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
								<asp:TemplateField SortExpression="HubName" ItemStyle-HorizontalAlign="Center" HeaderText="Hub Name" HeaderStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblHubName" Text='<%# DataBinder.Eval(Container.DataItem, "HubName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="Company" ItemStyle-HorizontalAlign="Center" HeaderText="Shipment Company">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCompanyName" Text='<%# DataBinder.Eval(Container.DataItem, "Company")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="ShipmentDate" ItemStyle-HorizontalAlign="Center" HeaderText="Shipment Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblShipmentDate" Text='<%# DataBinder.Eval(Container.DataItem, "ShipmentDate", "{0:dd-MMM-yyyy}")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="RegisteredCompany" ItemStyle-HorizontalAlign="Center" HeaderText="Registered Company">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRegisteredCompany" Text='<%# DataBinder.Eval(Container.DataItem, "RegisteredCompany")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" HorizontalAlign="Center" />
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
                    <input type="hidden" id="hdnShipmentId" />
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
