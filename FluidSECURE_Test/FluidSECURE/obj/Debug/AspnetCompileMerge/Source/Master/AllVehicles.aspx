<%@ Page Title="All Vehicles" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllVehicles.aspx.vb" Inherits="Fuel_Secure.AllVehicles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /*select.input-sm {
            height: 10px;
            line-height: 10px;
            padding: 0px !important;
        }*/

        /*.ddlColumn option{
            font-size: 9px !important;
        }


        option:hover {
            background-color: red !important;
        }*/
    </style>
    <script>

        function DeleteRecord() {
            $('#btnMyModalClose').click();

            var vehicleId = $("#hdnvehicleId").val();

            $.ajax({
                type: "POST",
                url: "AllVehicles.aspx/DeleteRecord",
                data: '{vehicleId: "' + vehicleId + '" }',
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

                window.location.href = "/Master/AllVehicles"

            }
            else if (response.d == -2) {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Vehicle is associated to a fueling Transaction(s), you must Delete Transaction(s) first to Delete this vehicle.");
            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConfirm(vehicleid) {
            var Role = '<%= Session("RoleName") %>';

            if (Role == "SuperAdmin") {
                $("#lblMessage").text("Are you sure you want to delete this Vehicle?");
            }
            else {
                $("#lblMessage").text("Are you sure you want to delete this Vehicle?");
            }

            $("#hdnvehicleId").val(vehicleid);

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
                    <h3 class="panel-title text-center">Vehicles</h3>
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
                        <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new vehicle" OnClick="btn_New_Click" />
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvVehicle" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true"
                            DataKeyNames="VehicleId" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
                            <Columns>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "VehicleId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Active" ControlStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CHK_Active" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsActive")%>' OnCheckedChanged="CHK_Active_CheckedChanged" AutoPostBack="true" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="TempVehicleNumber" ItemStyle-HorizontalAlign="Center" HeaderText="Vehicle Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTempVehicleNumber" Text='<%# DataBinder.Eval(Container.DataItem, "VehicleNumber")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="VehicleName" ItemStyle-HorizontalAlign="Center" HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVehicleName" Text='<%# DataBinder.Eval(Container.DataItem, "VehicleName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="EXTENSION" ItemStyle-HorizontalAlign="Left"
                                    HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEXTENSION" Text='<%# DataBinder.Eval(Container.DataItem, "EXTENSION")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="Name" ItemStyle-HorizontalAlign="Center" HeaderText="Department">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDEPT" Text='<%# DataBinder.Eval(Container.DataItem, "Name")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                               <%-- <asp:TemplateField SortExpression="LicensePlateNumber" ItemStyle-HorizontalAlign="Left" HeaderText="License Plate Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLICNO" Text='<%# DataBinder.Eval(Container.DataItem, "LicensePlateNumber")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:TemplateField SortExpression="Make" ItemStyle-HorizontalAlign="Left" HeaderText="Vehicle Make">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVEHMAKE" Text='<%# DataBinder.Eval(Container.DataItem, "Make")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="MODEL" ItemStyle-HorizontalAlign="Left" HeaderText="Vehicle Model">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVEHMODEL" Text='<%# DataBinder.Eval(Container.DataItem, "MODEL")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="YEAR" ItemStyle-HorizontalAlign="Center" HeaderText="Vehicle Year">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVEHYEAR" Text='<%# DataBinder.Eval(Container.DataItem, "YEAR")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="VIN" ItemStyle-HorizontalAlign="Left" HeaderText="Vehicle VIN">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVEHVIN" Text='<%# DataBinder.Eval(Container.DataItem, "VIN")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="CustomerName" ItemStyle-HorizontalAlign="Left" HeaderText="Company">
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
                    <input type="hidden" id="hdnvehicleId" />
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
