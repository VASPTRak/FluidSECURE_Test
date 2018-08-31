<%@ Page Title="Personnel Import" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PersonnelImport.aspx.vb" Inherits="Fuel_Secure.PersonnelImport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div id="mydiv">
                    <img src="/Content/images/ajax-loader.gif" class="ajax-loader" />
                </div>
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Import Personnel</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                        <asp:HiddenField ID="HDF_CurrentDate" runat="server" />
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12 text-center" style="margin: 0px 0 10px;">
                        <asp:LinkButton ID="LB_Error" runat="server" OnClick="LB_Error_Click">Click here to download error log file.</asp:LinkButton>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Upload file:
                            <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:FileUpload ID="FU_Person" runat="server" TabIndex="2" />
                            <asp:RequiredFieldValidator if="RDF_Firware" runat="server" Display="Dynamic" ErrorMessage="Please select file to upload." ControlToValidate="FU_Person" ForeColor="Red" SetFocusOnError="True" ValidationGroup="ImportPersonnelValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div id="divCompany" runat ="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="ddlCustomer" runat="server" TabIndex="1" CssClass="form-control input-sm" AutoPostBack ="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="ddlCustomer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="ImportPersonnelValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicles Allowed to Fuel:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <input type="button" id="BTN_Vehicles" tabindex="14" onclick="OpenVehicleTypeBox();" value="Click to add vehicle" />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Authorized Fueling Times:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <input type="button" id="BTN_PersonTiming" tabindex="15" onclick="OpenPersonTimingBox();" value="Click to add Fueling Times" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnUpload" CssClass="btn btn-primary" runat="server" OnClick="btnUpload_Click" Text="Upload" Width="100px"
                            UseSubmitBehavior="true" TabIndex="9" ValidationGroup="ImportPersonnelValidation" OnClientClick="setvalue()" />
                    </div>

                    <div class="row col-md-12 col-sm-12 text-center col-xs-12" style="margin-top: 20px;">
                        <asp:LinkButton ID="lnkTemplate" runat="server" OnClick="lnkTemplate_Click">Click here to download template file.</asp:LinkButton>
                    </div>

                </div>
            </div>

            <div class="modal fade" tabindex="-1" role="dialog" id="VehicleBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Click box(es) to authorize Person to select this Vehicle at the FluidSecure Link:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblVehicleMessage" runat="server" Text="Please select Department."></asp:Label>
                            </div>
                            <div class="row margin10 text-center">
                                <input type="text" class="form-control" id="VehicleInput" onkeyup="SearchVehicles()" placeholder="Search for Vehicle">
                            </div>
                            <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UP_Fuel" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gv_Vehicles" CssClass="table table-bordered" runat="server" DataKeyNames="VehicleId,VehicleNumber,VehicleName" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecificVehicles(this);" runat="server" Style="text-align: center;" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="CHK_Vehicle" runat="server" onclick="javascript:SelectboxVehicle(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="VehicleName" HeaderText="Vehicle Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="VehicleNumber" HeaderText="Vehicle Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="Name" HeaderText="Vehicle's Department" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnVehicleOk" class="btn btn-success" onclick="ClosePopUp()" value="Ok" />
                            <input type="button" id="btnClose" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                            <asp:Button ID="btnCloseVehicle" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUp()" OnClick="btnCloseVehicle_Click" />
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
                            <input type="button" id="btnFuelingTimesOk" class="btn btn-success" onclick="ClosePopUpFuelingTimes()" value="Ok" />
                            <input type="button" id="btnCloseFuelingTimes" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                            <asp:Button ID="btnCancelFuelingTimes" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpFuelingTimes()" OnClick="btnCancelFuelingTimes_Click" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ddlCustomer" />
            <asp:PostBackTrigger ControlID="btnCloseVehicle" />
            <asp:PostBackTrigger ControlID="btnCancelFuelingTimes" />
            <asp:PostBackTrigger ControlID="LB_Error" />
            <asp:PostBackTrigger ControlID="btnUpload" />
            <asp:PostBackTrigger ControlID="lnkTemplate" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function setvalue() {

            var HDF_CurrentDate = $("#<%=HDF_CurrentDate.ClientID%>");

            var localTime = new Date();
            var year = localTime.getFullYear();
            var month = localTime.getMonth() + 1;
            var date = localTime.getDate();
            var hours = localTime.getHours();
            var minutes = localTime.getMinutes();
            var seconds = localTime.getSeconds();

            HDF_CurrentDate.val(month + "/" + date + "/" + year + " " + hours + ":" + minutes + ":" + seconds);

            if (($('#<%=FU_Person.ClientID%>').val() != "") && ($('#<%=ddlCustomer.ClientID%>').val() != "0")) {
                $("#mydiv").show();
            }
        }

        function OpenVehicleTypeBox() {
            $("#VehicleInput").val("");
            SearchVehicles();

            $('#VehicleBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });

        }


        function SearchVehicles() {

            var input, filter, table, tr, td, i;
            input = document.getElementById("VehicleInput");
            filter = input.value.toLowerCase();
            table = document.getElementById('<%= gv_Vehicles.ClientID %>');
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

        function OpenPersonTimingBox() {
            $('#FuelingTimes').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }


        function ClosePopUp() {


            $("#btnClose").click();
            $('body').removeClass("modal-open");

            $('#BTN_PersonTiming').focus();


        }

        function ClosePopUpFuelingTimes() {
            $("#btnCloseFuelingTimes").click();
            $('body').removeClass("modal-open");
            $('#BTN_PersonSite').focus();

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

        function SelectAllCheckboxesSpecificVehicles(spanChk) {

            var IsChecked = spanChk.checked;
            var Chk = spanChk;
            Parent = document.getElementById('<%= gv_Vehicles.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
                if (items[i].id != Chk && items[i].type == "checkbox") {
                    if (items[i].checked != IsChecked) {
                        items[i].click();
                    }
                }
            }
        }

        function SelectboxVehicle(spanChk) {

            var IsChecked = spanChk.checked;
            if (IsChecked == false) {
                Parent = document.getElementById('<%= gv_Vehicles.ClientID%>');
                var checkBoxSelector = "#<%=gv_Vehicles.ClientID%> input[id*='chkAll']";

                $(checkBoxSelector).attr('checked', false);
            }
        }

        function showWaiter() {
            $("#mydiv").hide();
        }

        function stopWaiter() {
            $("#mydiv").hide();
        }
    </script>

</asp:Content>
