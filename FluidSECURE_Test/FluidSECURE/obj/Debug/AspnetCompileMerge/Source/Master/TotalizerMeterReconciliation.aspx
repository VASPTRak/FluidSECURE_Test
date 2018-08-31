<%@ Page Title="Totalizer/Meter Reconciliation" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="TotalizerMeterReconciliation.aspx.vb" Inherits="Fuel_Secure.TotalizerMeterReconciliation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="hdnEntryType" Value="" />
            <asp:HiddenField runat="server" ID="hdnTankInventory" Value="" />
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center">
                            <asp:Label runat="server" ID="message" class="text-center green"></asp:Label>
                        </p>
                        <p class="text-center">
                            <asp:Label runat="server" ID="ErrorMessage" class="text-center red"></asp:Label>
                        </p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Select FluidSecure Link:
                        <label class="text-danger font-required">[required]</label></label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_FluidLink" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:DropDownList>
                            <%--  <asp:RequiredFieldValidator ID="RFDTankNo" runat="server" ControlToValidate="txtTankNo"
                                ErrorMessage="Please Enter Tank Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="TankValidation"></asp:RequiredFieldValidator>--%>
                        </div>
                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company
                                <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="2" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <%-- <asp:RequiredFieldValidator ID="RDF_Customer" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TankValidation"></asp:RequiredFieldValidator>--%>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div runat="server" id="StartDiv" class="row">
                        <%--  <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <asp:Button runat="server" ID="BTN_StartType" Class="btn btn-info" TabIndex="3" Text="Pick from Saved LEVELS" ValidationGroup="TankValidation" OnClientClick="return OpenStartTypeBox();" OnClick="BTN_StartType_Click" Style="font-size: smaller" />
                            </div>
                        </div>--%>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <label>
                                    Start Date 
                        <label class="text-danger font-required">[required]</label>:</label>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 col-xs-12">
                                <asp:TextBox ID="txtStartDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RDF_StartDate" runat="server" ControlToValidate="txtStartDate" Display="Dynamic" Style="float: left;"
                                    ErrorMessage="Please select Start Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="StartTankValidation"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <label>
                                    Start Time 
                        <label class="text-danger font-required">[required]</label>:</label>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 col-xs-12">
                                <asp:TextBox ID="txtStartTime" runat="server" Width="120" CssClass="form-control input-sm" TabIndex="5"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RDF_TransTime" runat="server" ControlToValidate="txtStartTime" Display="Dynamic"
                                    ErrorMessage="Please select Start Time ." ForeColor="Red" SetFocusOnError="True" ValidationGroup="StartTankValidation"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <label>
                                    Enter Starting Totalizer/Meter Number:
                        <label class="text-danger font-required">[required]</label></label>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 col-xs-12">
                                <asp:TextBox ID="txtStartLevelQuan" CssClass="form-control input-sm" TabIndex="6" runat="server" MaxLength="10" Width="100"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtStartLevelQuan"
                                    ErrorMessage="Please Enter Starting Totalizer/Meter Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="StartTankValidation"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CV_SQuantity" runat="server" Display="Dynamic" ErrorMessage="Please enter Totalizer/Meter Number in decimal format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True"
                                    Type="Double" ValidationGroup="StartTankValidation" ControlToValidate="txtStartLevelQuan"></asp:CompareValidator>

                            </div>
                        </div>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <asp:Button runat="server" ID="btnStartSave" Class="btn btn-success" Text="Save" OnClick="btnStartSave_Click" ValidationGroup="StartTankValidation" TabIndex="7"></asp:Button>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div runat="server" id="EndDiv" class="row">
                        <%--  <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <asp:Button runat="server" ID="BTN_EndType" Class="btn btn-info" Text="Pick from Saved LEVELS" ValidationGroup="TankValidation" OnClientClick="return OpenEndTypeBox();" OnClick="BTN_EndType_Click" Style="font-size: smaller" TabIndex="8"/>
                            </div>
                        </div>--%>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <label>
                                    End Date 
                        <label class="text-danger font-required">[required]</label>:</label>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 col-xs-12">
                                <asp:TextBox ID="txtEndDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="9"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RDF_EndDate" runat="server" ControlToValidate="txtEndDate" Display="Dynamic" Style="float: left;"
                                    ErrorMessage="Please select End Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="EndTankValidation"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <label>
                                    End Time 
                        <label class="text-danger font-required">[required]</label>:</label>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 col-xs-12">
                                <asp:TextBox ID="txtEndTime" runat="server" Width="120" CssClass="form-control input-sm" TabIndex="10"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEndTime" Display="Dynamic"
                                    ErrorMessage="Please select End Time ." ForeColor="Red" SetFocusOnError="True" ValidationGroup="EndTankValidation"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <label>
                                    Enter Ending Totalizer/Meter Number:
                        <label class="text-danger font-required">[required]</label></label>
                            </div>
                            <div class="form-group col-md-2 col-sm-2 col-xs-12">
                                <asp:TextBox ID="txtEndLevelQuan" CssClass="form-control input-sm" TabIndex="11" runat="server" MaxLength="10" Width="100" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEndLevelQuan"
                                    ErrorMessage="Please Enter Ending Totalizer/Meter Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="EndTankValidation"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CV_EQuantity" runat="server" Display="Dynamic" ErrorMessage="Please enter Totalizer/Meter Number in decimal format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True"
                                    Type="Double" ValidationGroup="EndTankValidation" ControlToValidate="txtEndLevelQuan"></asp:CompareValidator>
                            </div>
                        </div>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                                <asp:Button runat="server" ID="btnEndSave" CssClass="btn btn-success" Text="Save" OnClick="btnEndSave_Click" ValidationGroup="EndTankValidation" TabIndex="12"></asp:Button>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-6 textright col-xs-12">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Button runat="server" ID="btnMainCancel" CssClass="btn btn-success" Text="Cancel" OnClick="btnMainCancel_Click" TabIndex="13"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">

    <script type="text/javascript">

        function OpenStartTypeBox() {

            var company = $('#<%= DDL_Customer.ClientID %>').val();
            var FluidLink = $('#<%= DDL_FluidLink.ClientID %>').val();
            if (FluidLink == "0") {
                $('#<%= ErrorMessage.ClientID %>').show()
                $('#<%= ErrorMessage.ClientID %>').html("Please select FluidSecure Link number.")
                $('#<%= DDL_FluidLink.ClientID %>').focus()
                return false;
            }
            else if (company == "0") {
                $('#<%= ErrorMessage.ClientID %>').show()
                $('#<%= ErrorMessage.ClientID %>').html("Please select Company.")
                $('#<%= DDL_Customer.ClientID %>').focus()
                return false;
            }
            else {

            }
    }

    function OpenEndTypeBox() {
        var company = $('#<%= DDL_Customer.ClientID %>').val();
        var FluidLink = $('#<%= DDL_FluidLink.ClientID %>').val();
        if (FluidLink == "0") {
            $('#<%= ErrorMessage.ClientID %>').show()
            $('#<%= ErrorMessage.ClientID %>').html("Please select FluidSecure Link number.")
            $('#<%= DDL_FluidLink.ClientID %>').focus()
            return false;
        }
        else if (company == "0") {
            $('#<%= ErrorMessage.ClientID %>').show()
            $('#<%= ErrorMessage.ClientID %>').html("Please select Company.")
            $('#<%= DDL_Customer.ClientID %>').focus()
            return false;
        }
        else {

        }
}

function ClosePopUpStart() {
    $("#btnCloseStart").click();
    $('body').removeClass("modal-open");
    //LoadDateTimeControl()
}

function ClosePopUpEnd() {
    $("#btnCloseEnd").click();
    $('body').removeClass("modal-open");
    LoadDateTimeControl()

}

function LoadDateTimeControl() {

    $("[id$=txtStartDate]").datepicker({
        showOn: 'button',
        buttonImageOnly: true,
        buttonImage: '/Content/images/calendar.png',
        changeMonth: true,
        changeYear: true,
        yearRange: "-100:+0",
        maxDate: 0
    });

    $('[id$=txtStartTime]').timepicker({
        timeFormat: 'h:mm p',
        interval: 01,
        dynamic: false,
        dropdown: true,
        scrollbar: true

    });

    $("[id$=txtEndDate]").datepicker({
        showOn: 'button',
        buttonImageOnly: true,
        buttonImage: '/Content/images/calendar.png',
        changeMonth: true,
        changeYear: true,
        yearRange: "-100:+0",
        maxDate: 0
    });

    $('[id$=txtEndTime]').timepicker({
        timeFormat: 'h:mm p',
        interval: 01,
        dynamic: false,
        dropdown: true,
        scrollbar: true

    });
}

function checkRemoveAll(objRef) {
    var GridView = objRef.parentNode.parentNode.parentNode;
    var inputList = GridView.getElementsByTagName("input");
    for (var i = 0; i < inputList.length; i++) {
        //Get the Cell To find out ColumnIndex
        if (inputList[i].type == "radio" && objRef != inputList[i]) {
            if (objRef.checked) {
                inputList[i].checked = false;
            }
        }
    }
}

openStart()
{

    $('#StartDatePicker').modal({ show: true, backdrop: 'static', keyboard: false });
}
    </script>
    <div class="modal fade" tabindex="-1" role="dialog" id="StartDatePicker">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-center">Select to pick Start Level</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12">
                        <asp:Label ID="lblStartLevelFluidNumber" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                        <asp:UpdatePanel ID="UP_StartLevel" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gv_StartLevel" CssClass="table table-bordered table-responsive" runat="server" DataKeyNames="TankInventoryId" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:RadioButton ID="Rdb_StartLevel" runat="server" onclick="javascript:checkRemoveAll(this);" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="InventoryDate" HeaderText="Start Date" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                        <asp:BoundField DataField="InventoryTime" HeaderText="Start Time" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                        <asp:BoundField DataField="Quantity" HeaderText="Totalizer/Meter Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                    </Columns>
                                </asp:GridView>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="modal-footer nextButton">
                    <asp:Button ID="btnStartOk" runat="server" class="btn btn-success" Text="OK" OnClick="btnStartOk_Click" />
                    <input type="button" id="btnCloseStart" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                    <asp:Button ID="btnCancelStart" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpStart()" OnClick="btnCancelStart_Click" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

    <div class="modal fade" tabindex="-1" role="dialog" id="EndDatePicker">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-center">Select to pick End Level</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12">
                        <asp:Label ID="lblEndLevelFluidNumber" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                        <asp:UpdatePanel ID="UP_EndLevel" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gv_EndLevel" CssClass="table table-bordered table-responsive" runat="server" DataKeyNames="TankInventoryId" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:RadioButton ID="Rdb_EndLevel" runat="server" onclick="javascript:checkRemoveAll(this);" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="InventoryDate" HeaderText="End Date" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                        <asp:BoundField DataField="InventoryTime" HeaderText="End Time" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                        <asp:BoundField DataField="Quantity" HeaderText="Totalizer/Meter Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                    </Columns>
                                </asp:GridView>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="modal-footer nextButton">
                    <asp:Button ID="btnEndOk" runat="server" class="btn btn-success" Text="OK" OnClick="btnEndOk_Click" />
                    <input type="button" id="btnCloseEnd" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                    <asp:Button ID="btnCancelEnd" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpEnd()" OnClick="btnCancelEnd_Click" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</asp:Content>

