﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllFSVMFirmwareUpgrades.aspx.vb" Inherits="Fuel_Secure.AllFSVMFirmwareUpgrades" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script>

        function DeleteRecord() {
            $('#btnMyModalCloseConfirmDelete').click();

            var FSVMFirmwareId = $("#hdnFSVMFirmwareIdConfirmDelete").val();

            $.ajax({
                type: "POST",
                url: "AllFSVMFirmwareUpgrades.aspx/DeleteRecord",
                data: '{FSVMFirmwareId: "' + FSVMFirmwareId + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessdelete,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }

        function OnSuccessdelete(response) {

            $("#myModalConfirmDelete").hide();

            if (response.d == 1) {
                $("#messageNew").text("Records deleted Successfully.");
                $("#ErrorMessageNew").hide();
                $("#messageNew").show();

                window.location.href = "/Master/AllFSVMFirmwareUpgrades"

            }
            else if (response.d == -1) {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("This FSVM Firmware is in luanched mode, so you cannot delete this FSVM Firmware.");
            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConfirm(FSVMFirmwareId) {
            var Role = '<%= Session("RoleName") %>';

            //if (Role == "SuperAdmin") {
            //    $("#lblMessage").text("Are you sure you want to delete this Company.  All associated transactions, personnels, departments, vehicles, sites, hose will also be deleted.");
            //}
            //else {
            $("#lblMessageConfirmDelete").text("Are you sure you want to delete this FSVM Firmware?");
            //}

            $("#hdnFSVMFirmwareIdConfirmDelete").val(FSVMFirmwareId);

            $('#myModalConfirmDelete').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function CheckLaunchConfirm(FSVMFirmwareId) {
            var Role = '<%= Session("RoleName") %>';

            $("#lblMessage").text("Are you sure you want to luanch this FSVM Firmware?");

            $("#hdnFSVMFirmwareId").val(FSVMFirmwareId);

            $('#myModalSuccess').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function LaunchFSVMFirmware() {
            $('#btnMyModalClose').click();

            var FSVMFirmwareId = $("#hdnFSVMFirmwareId").val();

            $.ajax({
                type: "POST",
                url: "AllFSVMFirmwareUpgrades.aspx/LaunchFSVMFirmware",
                data: '{FSVMFirmwareId: "' + FSVMFirmwareId + '" }',
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
                //$("#messageNew").text("Records deleted Successfully.");
                //$("#ErrorMessageNew").hide();
                //$("#messageNew").show();

                window.location.href = "/Master/AllFSVMFirmwareUpgrades"

            }
                //else if (response.d == -2) {
                //    $("#ErrorMessageNew").show();
                //    $("#messageNew").hide();

                //    $("#ErrorMessageNew").text("This Company has been assigned to departments, so you cannot delete this company.'");
                //}
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("launching failed.");
            }
        }
    </script>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading">
                    <h3 class="panel-title text-center">FSVM Firmware Upgrades</h3>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12  col-xs-12">
                        <div class="form-group col-md-2 col-sm-3 hidden-xs"></div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_Column" AutoPostBack="True"></asp:DropDownList>
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
                           <asp:Button ID="btn_New" CssClass="btn btn-primary" runat="server" Text="Add new FSVMFirmware Upgrade" OnClick="btn_New_Click" />
                        </div>
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvUploadedFSVMFirmware" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true"
                            DataKeyNames="FSVMFirmwareId,IsLaunched,IsUpgradable" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria" OnRowDataBound="gvUploadedFSVMFirmware_RowDataBound">
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                            <Columns>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckLaunchConfirm(<%# DataBinder.Eval(Container.DataItem, "FSVMFirmwareId")%>)">Launch</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLaunch" runat="server">Launched</asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Auto Upgrade">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="lnkAutoUpgradeEnableDisable" Text="OFF" OnClick="lnkAutoUpgradeEnableDisable_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "FSVMFirmwareId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="FSVMFirmwareFileName" ItemStyle-HorizontalAlign="Center" HeaderText="FSVMFirmware File Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFSVMFirmwareFileName" Text='<%# DataBinder.Eval(Container.DataItem, "FSVMFirmwareFileName")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="Version" ItemStyle-HorizontalAlign="Left"
                                    HeaderText="Version">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVersion" Text='<%# DataBinder.Eval(Container.DataItem, "Version")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="IsLaunched" ItemStyle-HorizontalAlign="Center" HeaderText="Launched">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsLaunched" Text='<%# DataBinder.Eval(Container.DataItem, "IsLaunched")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="LaunchedDate" ItemStyle-HorizontalAlign="Center" HeaderText="Launched Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLaunchedDate" Text='<%# DataBinder.Eval(Container.DataItem, "LaunchedDate")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="PersonName" ItemStyle-HorizontalAlign="Center" HeaderText="Launched By">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLaunchedBy" Text='<%# DataBinder.Eval(Container.DataItem, "PersonName")%>'
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
                    <input type="hidden" id="hdnFSVMFirmwareId" />
                    <h4 id="lblMessage"></h4>
                </div>
                <div class="modal-footer nextButton">
                    <button type="button" id="btnMyModalClose" class="btn btn-default" data-dismiss="modal" onclick="return false;">Close</button>
                    <input type="button" id="btnMyModalSuccess" class="btn btn-success" onclick="LaunchFSVMFirmware()" value="Ok" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

    <!--alert message popup-->
    <div class="modal fade" tabindex="-1" role="dialog" id="myModalConfirmDelete">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title text-center">FluidSecure</h3>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hdnFSVMFirmwareIdConfirmDelete" />
                    <h4 id="lblMessageConfirmDelete"></h4>
                </div>
                <div class="modal-footer nextButton">
                    <button type="button" id="btnMyModalCloseConfirmDelete" class="btn btn-default" data-dismiss="modal" onclick="return false;">No</button>
                    <input type="button" id="btnMyModalSuccessConfirmDelete" class="btn btn-success" onclick="DeleteRecord()" value="Yes" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</asp:Content>
