<%@ Page Title="Transaction Report By Personnel" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="TransactionReportByPersonnel.aspx.vb" Inherits="Fuel_Secure.TransactionReportByPersonnel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .btn-group, .multiselect.dropdown-toggle.btn.btn-default, ul.multiselect-container.dropdown-menu {
            width: 100% !important;
            text-align: left;
        }
    </style>
    <asp:UpdatePanel ID="UP_Main" runat="server">
         <ContentTemplate>
            <div id="mydiv">
                <img src="/Content/images/ajax-loader.gif" title="Please wait..." class="ajax-loader" /> 
            </div>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Transaction Report by Personnel</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Date From:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionDateFrom" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Date To:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionDateTo" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="3"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Time From:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionTimeFrom" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="2"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Time To:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionTimeTo" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Department:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Dept" runat="server" TabIndex="5" CssClass="form-control input-sm" Style="width: 150px; float: left; margin-right: 10px;" AutoPostBack="true" OnSelectedIndexChanged="DDL_Dept_SelectedIndexChanged"></asp:DropDownList>
                            <img src="../Content/images/icons8-search-50.png" onclick="OpenDepartmentTypeBox();" style="width: 20px; vertical-align: -webkit-baseline-middle;" />
                        </div>
                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="10" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Customer" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicle Number:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Vehicle" runat="server" TabIndex="6" CssClass="form-control input-sm" Style="width: 150px; float: left; margin-right: 10px;" OnSelectedIndexChanged="DDL_Vehicle_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <img src="../Content/images/icons8-search-50.png" onclick="OpenVehicleTypeBox();" style="width: 20px; vertical-align: -webkit-baseline-middle;" />
                            <%--<input type="button" id="BTN_Vehicles" tabindex="15"  value="Click to add vehicle" />--%>
                            <%--<asp:Label ID="lbl_VehicleNumber" runat="server" TabIndex="2"></asp:Label>--%>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Personnel: </label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Personnel" runat="server" TabIndex="11" Style="width: 150px; float: left; margin-right: 10px;" CssClass="form-control input-sm" OnSelectedIndexChanged="DDL_Personnel_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <img src="../Content/images/icons8-search-50.png" onclick="OpenPersonnelTypeBox();" style="width: 20px; vertical-align: -webkit-baseline-middle;" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Product:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Fuel" runat="server" TabIndex="7" CssClass="form-control input-sm"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Include Deleted FluidSecure Link:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                           <asp:CheckBox ID="chk_IsDeletedLinkAllow" runat="server" AutoPostBack="true" OnCheckedChanged="chk_IsDeletedLinkAllow_CheckedChanged" Checked="true" TabIndex="12"/>
                        </div>
                    </div>
                     <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicle Type:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList runat="server" ID="DDL_VehicleType" CssClass="form-control input-sm" TabIndex="8">
                            </asp:DropDownList>
                        </div>
                         <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                FluidSecure Link:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <%-- <asp:DropDownList ID="DDL_Site" runat="server" TabIndex="5" CssClass="form-control input-sm"></asp:DropDownList>--%>
                            <asp:ListBox ID="lstSites" runat="server" SelectionMode="Multiple" TabIndex="12" CssClass="form-control input-sm"></asp:ListBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Status:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_TransactionStatus" runat="server" TabIndex="9" CssClass="form-control input-sm">
                            </asp:DropDownList>
                        </div>
                             <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Select Site:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_HubName" runat="server" TabIndex="13"   Style="width: 150px; float: left; margin-right: 10px;" CssClass="form-control input-sm" OnSelectedIndexChanged="DDL_HubName_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                             <img src="../Content/images/icons8-search-50.png" onclick="OpenSiteTypeBox();" style="float: left;width: 20px; vertical-align: -webkit-baseline-middle;" />
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnGenarateReport" CssClass="btn btn-primary" runat="server" OnClick="btnGenarateReport_Click" Text="Generate Report"
                            UseSubmitBehavior="False" TabIndex="14" ValidationGroup="TransactionValidation" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="Up_Vehicle" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="HDF_VehicleId" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="HDF_VehicleNumber" runat="server"></asp:HiddenField>

            <asp:HiddenField ID="HDF_DeptId" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="HDF_DeptNumber" runat="server"></asp:HiddenField>

            <asp:HiddenField ID="HDF_PersonId" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="HDF_Person" runat="server"></asp:HiddenField>

            <asp:HiddenField ID="HDF_PersonId_Site" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="HDF_HubSiteName" runat="server"></asp:HiddenField>

            <div class="modal fade" tabindex="-1" role="dialog" id="VehicleBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Select Vehicle to generate report:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblVehicleMessage" runat="server" Text="Please select Department."></asp:Label>
                            </div>
                            <div class="row margin10 text-center">
                                <%--<input type="text" class="form-control" id="VehicleInput" onkeyup="SearchVehicles()" placeholder="Search for Vehicle">--%>
                                <input type="text" name="search" value="" class="form-control" id="id_search" placeholder="Search for Vehicle" autofocus />
                            </div>
                            <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UP_Fuel" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gv_Vehicles" CssClass="table table-bordered" runat="server" DataKeyNames="VehicleId,VehicleNumber,VehicleName" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="RDB_Vehicle" runat="server" onclick="RadioCheck(this);" />
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
                            <%--<input type="button" id="btnVehicleOk" class="btn btn-success" onclick="ClosePopUp()" value="Ok" />--%>
                            <asp:Button ID="btnOk" runat="server" CssClass="btn btn-success" Text="Ok" OnClientClick="CloseVehPopUp()" OnClick="btnOk_Click" />
                            <input type="button" id="btnClose" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                            <%--<asp:Button ID="btnCloseVehicle" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUp()" OnClick="btnCloseVehicle_Click" />--%>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <div class="modal fade" tabindex="-1" role="dialog" id="DepartmentBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Select Department to generate report:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblDepartmentMessage" runat="server" Text="Please select Department."></asp:Label>
                            </div>
                            <div class="row margin10 text-center">
                                <input type="text" name="search" value="" class="form-control" id="id_searchDept" placeholder="Search for Department" autofocus />
                            </div>
                            <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="grd_Dept" CssClass="table table-bordered" runat="server" DataKeyNames="DeptId,NAME,Number" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="RDB_Department" runat="server" onclick="DeptRadioCheck(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="NAME" HeaderText="Department Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="Number" HeaderText="Department Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <asp:Button ID="btndeptOK" runat="server" CssClass="btn btn-success" Text="Ok" OnClientClick="CloseDeptPopUp()" OnClick="btndeptOK_Click" />
                            <input type="button" id="btnCloseDept" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <div class="modal fade" tabindex="-1" role="dialog" id="PersonnelBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Select Personnel to generate report:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblPersonnelMessage" runat="server" Text="Please select Personnel."></asp:Label>
                            </div>
                            <div class="row margin10 text-center">
                                <input type="text" name="search" value="" class="form-control" id="id_searchPer" placeholder="Search for Personnel" autofocus />
                            </div>
                            <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="grd_Per" CssClass="table table-bordered" runat="server" DataKeyNames="PersonId,PinNumber,Person" AutoGenerateColumns="False" EmptyDataText="Data Not found." OnDataBound="grd_Per_DataBound">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="RDB_Personnel" runat="server" onclick="DeptRadioCheck(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Person" HeaderText="Person Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="PinNumber" HeaderText="Person Pin" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <asp:Button ID="btnPerOK" runat="server" CssClass="btn btn-success" Text="Ok" OnClientClick="ClosePerPopUp()" OnClick="btnPerOK_Click" />
                            <input type="button" id="btnClosePer" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <div class="modal fade" tabindex="-1" role="dialog" id="SiteBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Select Site to generate report:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblSiteMessage" runat="server" Text="Please select Site."></asp:Label>
                            </div>
                            <div class="row margin10 text-center">
                                <input type="text" name="search" value="" class="form-control" id="id_searchSite" placeholder="Search for Site" autofocus />
                            </div>
                            <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="grd_Site" CssClass="table table-bordered" runat="server" DataKeyNames="PersonId,HubSiteName" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="RDB_Site" runat="server" onclick="SiteRadioCheck(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="HubSiteName" HeaderText="Hub Site Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <asp:Button ID="btnSiteOK" runat="server" CssClass="btn btn-success" Text="Ok" OnClientClick="CloseSitePopUp()" OnClick="btnSiteOK_Click" />
                            <input type="button" id="btnCloseSite" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->
        </ContentTemplate>
    </asp:UpdatePanel>

    <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">
    <script src="/Scripts/jquery.quicksearch.js"></script>
    <script type="text/javascript">

        function LoadDateTimeControl() {
            $("[id$=txtTransactionDateFrom]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $("[id$=txtTransactionDateTo]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });


            $('[id$=txtTransactionTimeFrom]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

            });


            $('[id$=txtTransactionTimeTo]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

             });

            $('input#id_search').quicksearch('table#MainContent_gv_Vehicles tbody tr');
            $('input#id_searchDept').quicksearch('table#MainContent_grd_Dept tbody tr');
            $('input#id_searchPer').quicksearch('table#MainContent_grd_Per tbody tr');
            $('input#id_searchSite').quicksearch('table#MainContent_grd_Site tbody tr');
        }

        function loadMultiList() {
            $('[id*=lstSites]').multiselect({
                includeSelectAllOption: true,
                allSelectedText: 'All FluidSecure Link',
                maxHeight: 200,
            })
            LoadDateTimeControl();
        }

        $(function () {
            $('[id*=lstSites]').multiselect({
                includeSelectAllOption: true,
                allSelectedText: 'All FluidSecure Link',
                maxHeight: 200,
            }).multiselect('selectAll', false).multiselect('updateButtonText');
        });

        $(function () {
            /*
			Example 1
			*/
            $('input#id_search').quicksearch('table#MainContent_gv_Vehicles tbody tr');
            $('input#id_searchDept').quicksearch('table#MainContent_grd_Dept tbody tr');
            $('input#id_searchPer').quicksearch('table#MainContent_grd_Per tbody tr');
            $('input#id_searchSite').quicksearch('table#MainContent_grd_Site tbody tr');
        });

        function OpenVehicleTypeBox() {
            $("#VehicleInput").val("");
            //SearchVehicles();

            $('#VehicleBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
            showWait();
        }

        function OpenDepartmentTypeBox() {

            $('#DepartmentBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
            showWait();
        }

        function OpenPersonnelTypeBox() {

            $('#PersonnelBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
            showWait();
        }

        function OpenSiteTypeBox() {

            $('#SiteBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
            showWait();
        }

        function RadioCheck(rb) {

            var gv = document.getElementById("<%=gv_Vehicles.ClientID%>");

            var rbs = gv.getElementsByTagName("input");

            var row = rb.parentNode.parentNode;

            for (var i = 0; i < rbs.length; i++) {

                if (rbs[i].type == "radio") {

                    if (rbs[i].checked && rbs[i] != rb) {

                        rbs[i].checked = false;

                        break;

                    }

                }

            }

        }

        function DeptRadioCheck(rb) {

            var gv = document.getElementById("<%=grd_Dept.ClientID%>");

            var rbs = gv.getElementsByTagName("input");

            var row = rb.parentNode.parentNode;

            for (var i = 0; i < rbs.length; i++) {

                if (rbs[i].type == "radio") {

                    if (rbs[i].checked && rbs[i] != rb) {

                        rbs[i].checked = false;

                    }

                }

            }

        }

        function PerRadioCheck(rb) {

            var gv = document.getElementById("<%=grd_Per.ClientID%>");

            var rbs = gv.getElementsByTagName("input");

            var row = rb.parentNode.parentNode;

            for (var i = 0; i < rbs.length; i++) {

                if (rbs[i].type == "radio") {

                    if (rbs[i].checked && rbs[i] != rb) {

                        rbs[i].checked = false;

                    }

                }

            }

        }

        function SiteRadioCheck(rb) {

            var gv = document.getElementById("<%=grd_Site.ClientID%>");

            var rbs = gv.getElementsByTagName("input");

            var row = rb.parentNode.parentNode;

            for (var i = 0; i < rbs.length; i++) {

                if (rbs[i].type == "radio") {

                    if (rbs[i].checked && rbs[i] != rb) {

                        rbs[i].checked = false;

                    }

                }

            }

        }

        function CloseVehPopUp() {
            $("#btnClose").click();

            $('body').removeClass("modal-open");
        }

        function CloseDeptPopUp() {
            $("#btnCloseDept").click();

            $('body').removeClass("modal-open");
        }

        function ClosePerPopUp() {
            $("#btnClosePer").click();

            $('body').removeClass("modal-open");
        }

        function CloseSitePopUp() {
            $("#btnCloseSite").click();

            $('body').removeClass("modal-open");
        }

        function showWait() {
            $("#mydiv").show();
        }
        function hideWait() {
            $("#mydiv").hide();
        }
    </script>
</asp:Content>
