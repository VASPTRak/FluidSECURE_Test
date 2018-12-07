<%@ Page Title="Vehicle Options" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="VehicleOptionsScreen.aspx.vb" Inherits="Fuel_Secure.VehicleOptionsScreen" %>


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
                            <asp:DropDownList ID="DDL_Customer" runat="server" TabIndex="2" CssClass="form-control input-sm"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="VehicleOptionsValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="Div1" runat="server">
                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>
                                Assign all personnel to all vehicles:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Button ID="btn_assign" runat="server" Text="Assign " OnClick="btn_Assign_Click" ValidationGroup="VehicleOptionsValidation" CssClass="btn btn-primary" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="Div2" runat="server">

                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>Click to Enable/Disable All Vehicle's Odometer:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Button ID="btn_EnableAllVehOdo" runat="server" CssClass="btn btn-primary" Text="Enable" OnClientClick="setValueLable(1)" OnClick="btn_EnableAllVehOdo_Click" ValidationGroup="VehicleOptionsValidation" />
                            <asp:Button ID="btn_DisableAllVehOdo" runat="server" CssClass="btn btn-primary" Text="Disable" OnClientClick="setValueLable(2)" OnClick="btn_DisableAllVehOdo_Click" ValidationGroup="VehicleOptionsValidation" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>
                                Click to Enable/Disable All Vehicle's  Hours:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Button ID="btn_EnableHours" runat="server" Text="Enable" CssClass="btn btn-primary" OnClick="btn_EnableHours_Click" OnClientClick="setOdoHoursValueLabel(1)" ValidationGroup="VehicleOptionsValidation" />
                            <asp:Button ID="btn_DisableHours" runat="server" Text="Disable" CssClass="btn btn-primary" OnClick="btn_DisableHours_Click" OnClientClick="setOdoHoursValueLabel(2)" ValidationGroup="VehicleOptionsValidation" />
                        </div>
                    </div>
                     <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>Odometer/Hours Reasonability either:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:RadioButtonList ID="RBL_OdometerReasonabilityConditions" runat="server" RepeatDirection="Vertical" CssClass="UnitType" TabIndex="21">
                                <asp:ListItem Text="Allow fueling after 3 entry" Value="1" Selected="True" />
                                <asp:ListItem Text="Don’t allow fueling unless correct" Value="2" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                     <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>Click to Enable/Disable Reasonability:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3  col-xs-12">
                            <asp:Button ID="btn_EnableReasonability" runat="server" Text="Enable" CssClass="btn btn-primary" OnClick="btn_EnableReasonability_Click" OnClientClick="setOdoOptValueLabel(1)" ValidationGroup="VehicleOptionsValidation" />
                            <asp:Button ID="btn_DisableReasonability" runat="server" Text="Disable" CssClass="btn btn-primary" OnClick="btn_DisableReasonability_Click" OnClientClick="setOdoOptValueLabel(2)" ValidationGroup="VehicleOptionsValidation" />
                        </div>

                    </div>
                    <%-- <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>Check Odometer/Hours Reasonability:</label>
                        </div>
                        <div class="form-group col-md-1 col-sm-1 col-xs-12" style="padding: 0">
                            <asp:CheckBox ID="CHK_CheckOdometerReasonable" runat="server" TabIndex="20" AutoPostBack="true"    />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <asp:RadioButtonList ID="RBL_UnitType" runat="server" RepeatDirection="Horizontal" CssClass="UnitType Mileage" TabIndex="20">
                                <asp:ListItem Text="Mileage" Value="1" Selected="True" />
                                <asp:ListItem Text="Kilometers" Value="2" />
                            </asp:RadioButtonList>
                        </div>
                    </div>--%>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>
                                Total Miles allowed between Fueling:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtOdoLimit" runat="server" CssClass="form-control input-sm" MaxLength="6" Width="70" TabIndex="20" data-toggle="tooltip" title='"Total Miles allowed between Fueling" represents the maximum amount of miles the vehicle is allowed to travel between fueling. Example: if a vehicle current miles is 1000 and the total miles between fueling is set to 300, the only mileage that will be accepted is between 1000 – 1300. NOTE: If choosing this option, the check odometer/hours reasonability must be checked.'></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-6 textright col-xs-12">
                        </div>
                        <div class="form-group col-md-6 col-sm-6 col-xs-12">
                            <asp:Button ID="btnOdoLimitAll" runat="server" Text="Update For all Vehicles" CssClass="btn btn-primary" OnClick="btnOdoLimitAll_Click" OnClientClick="setOdoHoursLimitValueLabel(1)" ValidationGroup="VehicleOptionsValidation" />
                            <asp:Button ID="btnOdoLimit" runat="server" Text="Update For Vehicle with No Odo Limit" CssClass="btn btn-primary" OnClick="btnOdoLimit_Click" OnClientClick="setOdoHoursLimitValueLabel(2)" ValidationGroup="VehicleOptionsValidation" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-3 textright col-xs-12">
                            <label>
                                Total Hours allowed between Fueling:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtHoursLimit" runat="server" CssClass="form-control input-sm" MaxLength="6" Width="70" TabIndex="20" data-toggle="tooltip" title='"Total Hours allowed between Fueling" represents the maximum hours the vehicle is allowed to run between fueling. Example: if a vehicle current hours is 10 and the total hours between fueling is set to 50, the only hours that will be accepted is between 10 – 60. NOTE: If choosing this option, the check odometer/hours reasonability must be checked.'></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-6 textright col-xs-12">
                        </div>
                        <div class="form-group col-md-6 col-sm-6 col-xs-12">
                            <asp:Button ID="btnHoursLimitAll" runat="server" Text="Update For all Vehicles" CssClass="btn btn-primary" OnClick="btnHoursLimitAll_Click" OnClientClick="setOdoHoursLimitValueLabel(3)" ValidationGroup="VehicleOptionsValidation" />
                            <asp:Button ID="btnHoursLimit" runat="server" Text="Update For Vehicle with No Hour Limit" CssClass="btn btn-primary" OnClick="btnHoursLimit_Click" OnClientClick="setOdoHoursLimitValueLabel(4)" ValidationGroup="VehicleOptionsValidation" />
                        </div>
                    </div>
                   
                   
                    <br>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="Div3" runat="server">
                        <div class="form-group col-md-6 col-sm-3 col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Button ID="btn_Return" runat="server" CssClass="btn btn-default" Text="Cancel" PostBackUrl="~/Master/AllVehicles.aspx" />
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

    <div class="modal fade" tabindex="-1" role="dialog" id="VehOdoModel">
        <div class="modal-dialog modal-lg">
            <div class="modal-content" style="width: 104%;">
                <div class="modal-header">
                    <h5 class="modal-title text-center">FluidSecure</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12" style="padding: 0px;margin-left:0px">
                        <asp:Label ID="lblVehodo" runat="server" Text=""></asp:Label><br />
                        <h4>Selecting this will override all existing settings, and cannot be undone.</h4>
                    </div>
                    <div class="modal-footer nextButton">
                        <asp:Button ID="btnVehOdoOk" runat="server" CssClass="btn btn-success" OnClientClick="CloseVehOdoBox()" Text="Yes" OnClick="btnVehOdoOk_Click" />
                        <%--<input type="button" id="btnVehOdoOk" class="btn btn-success" onclick="CloseVehOdoBox()" value="Yes" />--%>
                        <input type="button" id="btnCloseVehOdo" class="btn btn-success" data-dismiss="modal" style="display: none;" value="No" />
                        <asp:Button ID="btnCancelModal" runat="server" CssClass="btn btn-default" Text="No" OnClientClick="CloseVehOdoBox()" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </div>
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
    <div class="modal fade" tabindex="-1" role="dialog" id="OdoOptModel">
        <div class="modal-dialog modal-lg">
            <div class="modal-content" style="width: 104%;">
                <div class="modal-header">
                    <h5 class="modal-title text-center">FluidSecure</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12" style="padding: 0px;margin-left:0px">
                        <asp:Label ID="lblOdoOpt" runat="server" Text=""></asp:Label><br />
                        <h4>Selecting this will override all existing settings, and cannot be undone.</h4>
                    </div>
                    <div class="modal-footer nextButton">
                        <asp:Button ID="btnSaveOdoOpt" runat="server" CssClass="btn btn-success" OnClientClick="CloseOdoOptBox()" Text="Yes" OnClick="btnSaveOdo_Click" />
                        <%--<input type="button" id="btnVehOdoOk" class="btn btn-success" onclick="CloseVehOdoBox()" value="Yes" />--%>
                        <input type="button" id="btnCloseOdoOpt" class="btn btn-success" data-dismiss="modal" style="display: none;" value="No" />
                        <asp:Button ID="btnCancelOdo" runat="server" CssClass="btn btn-default" Text="No" OnClientClick="CloseOdoOptBox()" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </div>
    <div class="modal fade" tabindex="-1" role="dialog" id="OdoHoursModel">
        <div class="modal-dialog modal-lg">
            <div class="modal-content" style="width: 104%;">
                <div class="modal-header">
                    <h5 class="modal-title text-center">FluidSecure</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12" style="padding: 0px;margin-left:0px">
                        <asp:Label ID="lblOdoHours" runat="server" Text=""></asp:Label><br />
                        <h4>Selecting this will override all existing settings, and cannot be undone.</h4>
                    </div>
                    <div class="modal-footer nextButton">
                        <asp:Button ID="btnOdoHours" runat="server" CssClass="btn btn-success" OnClientClick="CloseOdoHoursBox()" Text="Yes" OnClick="btnOdoHours_Click" />
                        <%--<input type="button" id="btnVehOdoOk" class="btn btn-success" onclick="CloseVehOdoBox()" value="Yes" />--%>
                        <input type="button" id="btnCloseOdoHours" class="btn btn-success" data-dismiss="modal" style="display: none;" value="No" />
                        <asp:Button ID="btnOdoCancleHours" runat="server" CssClass="btn btn-default" Text="No" OnClientClick="CloseOdoHoursBox()" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </div>
    <div class="modal fade" tabindex="-1" role="dialog" id="OdoHoursLimitModel">
        <div class="modal-dialog modal-lg">
            <div class="modal-content" style="width: 104%;">
                <div class="modal-header">
                    <h5 class="modal-title text-center">FluidSecure</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12" style="padding: 0px;margin-left:0px">
                        <asp:Label ID="lblOdoHoursLimit" runat="server" Text=""></asp:Label><br />
                        <h4>Selecting this will override all existing settings, and cannot be undone.</h4>
                    </div>
                    <div class="modal-footer nextButton">
                        <asp:Button ID="btnCloseOdoHoursLimitOk" runat="server" CssClass="btn btn-success" OnClientClick="CloseOdoHoursLimitBox()" Text="Yes" OnClick="btnOdoHoursLimitOk_Click" />
                        <%--<input type="button" id="btnVehOdoOk" class="btn btn-success" onclick="CloseVehOdoBox()" value="Yes" />--%>
                        <input type="button" id="btnCloseOdoHoursLimit" class="btn btn-success" data-dismiss="modal" style="display: none;" value="No" />
                        <asp:Button ID="btnCancelOdoHOursLimit" runat="server" CssClass="btn btn-default" Text="No" OnClientClick="CloseOdoHoursLimitBox()" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </div>

    <script>
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



        function OpenVehOdoModelBox() {
            $('#VehOdoModel').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }
        function CloseVehOdoBox() {
            $("#btnCloseVehOdo").click();
            $('body').removeClass("modal-open");
        }

        function setValueLable(value) {
            //debugger;
            if (value == 1) {
                document.getElementById('<%=lblVehodo.ClientID%>').innerText = "Are you sure you want to Enable odometer entry  for all vehicles in this Company ?";
            }
            else {
                document.getElementById('<%=lblVehodo.ClientID %>').innerText = "Are you sure you want to Disable  odometer entry   for all vehicles in this Company ?";
            }

        }
        function OpenOdoOptBox() {
            $('#OdoOptModel').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }
        function CloseOdoOptBox() {
            $("#btnCloseOdo").click();
            $('body').removeClass("modal-open");
        }
        function setOdoOptValueLabel(value) {
            //debugger;
            if (value == 1) {
                document.getElementById('<%=lblOdoOpt.ClientID%>').innerText = "Are you sure you want to Enable Reasonability for all vehicles in this Company ?";
            }
            else {
                document.getElementById('<%=lblOdoOpt.ClientID%>').innerText = "Are you sure you want to Disable Reasonability for all vehicles in this Company ?";
            }

        }
        function OpenOdoHoursBox() {
            $('#OdoHoursModel').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }
        function CloseOdoHoursBox() {
            $("#btnCloseOdoHours").click();
            $('body').removeClass("modal-open");
        }
        function setOdoHoursValueLabel(value) {
            //debugger;
            if (value == 1) {
                document.getElementById('<%=lblOdoHours.ClientID%>').innerText = "Are you sure you want to Enable  hours for all vehicles in this Company ?";
            }
            else {
                document.getElementById('<%=lblOdoHours.ClientID%>').innerText = "Are you sure you want to Disable  hours for all vehicles in this Company ?";
            }

        }

        function OpenOdoHoursLimitBox() {
            $('#OdoHoursLimitModel').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function CloseOdoHoursLimitBox() {
            $("#btnCloseOdoHoursLimit").click();
            $('body').removeClass("modal-open");
        }


        function setOdoHoursLimitValueLabel(value) {
            //debugger;
            if (value == 1) {
                document.getElementById('<%=lblOdoHoursLimit.ClientID%>').innerText = "Are you sure you want to Update Total Miles allowed between Fueling for all vehicles in this Company ?";
            }
            else if (value == 2) {
                document.getElementById('<%=lblOdoHoursLimit.ClientID%>').innerText = "Are you sure you want to Update Total Miles allowed between Fueling for Vehicle having No Odo Limit in this Company ?";
            }
            else if (value == 3) {
                document.getElementById('<%=lblOdoHoursLimit.ClientID%>').innerText = "Are you sure you want to Update Total Hours allowed between Fueling all vehicles in this Company ?";
            }
            else if (value == 4) {
                document.getElementById('<%=lblOdoHoursLimit.ClientID%>').innerText = "Are you sure you want to Update Total Hours allowed between Fueling  for Vehicle having No Hour Limit in this Company ?";
            }

}
    </script>
</asp:Content>
