<%@ Page Title="Add/Edit Vehicle" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Vehicle.aspx.vb" Inherits="Fuel_Secure.Vehicle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .UnitType label {
            padding-left: 5px;
            /*padding-right: 5px;*/
            vertical-align: middle;
            font-size: 12px;
        }

        .Mileage label {
            padding-left: 5px;
            padding-right: 10px;
            vertical-align: middle;
            font-size: 12px;
        }
    </style>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="modal fade" tabindex="-1" role="dialog" id="FuelTypeBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Click box to add all products allowed by the Vehicle:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblFuelMessage" runat="server" Text=""></asp:Label>
                            </div>
                            <div class="row margin10 text-center">
                                <input type="text" class="form-control" id="FuelInput" onkeyup="SearchFuel()" placeholder="Search for Product Name">
                            </div>
                            <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UP_Fuel" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gv_FuelTypes" CssClass="table table-bordered" runat="server" DataKeyNames="FuelTypeId,FuelType" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecific(this);" runat="server" Style="text-align: center;" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="CHK_FuelType" runat="server" onclick="javascript:SelectboxFuel(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="FuelType" HeaderText="Products" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnFuelOk" class="btn btn-success" onclick="ClosePopUpFuel()" value="Ok" />
                            <input type="button" id="btnCloseFuel" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                            <asp:Button ID="btnCancelFuel" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUpFuel()" OnClick="btnCancelFuel_Click" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <div class="modal fade" tabindex="-1" role="dialog" id="PersonSites">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Select Hoses where this vehicle is authorized to receive fuel</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblSiteMessage" runat="server" Text=""></asp:Label>
                            </div>
                            <div class="row margin10">
                                <input type="text" id="siteInput" class="form-control" onkeyup="SearchSite()" placeholder="Search for FluidSecure Link">
                            </div>
                            <div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UP_Sites" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gv_Sites" CssClass="table table-bordered" runat="server" DataKeyNames="SiteID,WifiSSId" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkAll" onclick="javascript:SelectAllCheckboxesSpecificForSite(this);" runat="server" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="CHK_PersonSite" runat="server" onclick="javascript:SelectboxSite(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="WifiSSId" HeaderText="FluidSecure Links" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnVehicleOk" class="btn btn-success" onclick="ClosePopUp()" value="Ok" />
                            <input type="button" id="btnClose" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                            <asp:Button ID="btnCancelVehicle" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUp()" OnClick="btnCancelVehicle_Click" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->


            <!--alert message popup-->
            <div class="modal fade" tabindex="-1" role="dialog" id="VehiclePersonMapping">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3 class="modal-title text-center">FluidSecure</h3>
                        </div>
                        <div class="modal-body">
                            <h4>Do you want to Add this Vehicle to all Personnel in your Organization?</h4>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnVehiclePersonMappingSuccess" class="btn btn-success" onclick="AddMapping()" value="Yes" />
                            <input type="button" id="btnCloseMapping" class="btn btn-default" data-dismiss="modal" value="No" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <!--alert message popup-->
            <div class="modal fade" tabindex="-1" role="dialog" id="VehiclePersonMappingAddNew">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3 class="modal-title text-center">FluidSecure</h3>
                        </div>
                        <div class="modal-body">
                            <h4>Do you want to Add this Vehicle to all Personnel in your Organization?</h4>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnVehiclePersonMappingSuccessAddNew" class="btn btn-success" onclick="AddNewMapping();" value="Yes" />
                            <input type="button" id="btnCloseMappingAddNewClose" style="display:none;" class="btn btn-default" data-dismiss="modal" value="No" />
                            <input type="button" id="btnCloseMappingAddNew" class="btn btn-default" data-dismiss="modal" value="No" onclick="window.location.href = '/Master/Vehicle'" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <div class="panel panel-primary" style="margin: 20px;">
                <div id="mydiv">
                    <img src="/Content/images/ajax-loader.gif" class="ajax-loader" />
                </div>
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicle Number
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtVehicleNumber" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="20" Width="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFD_VehicleNumber" runat="server" ControlToValidate="txtVehicleNumber" Display="Dynamic" ErrorMessage="Please enter vehicle number." ForeColor="Red" SetFocusOnError="True" ValidationGroup="VehicleValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Last Fluid Date/Time:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtLastFuelDateTime" runat="server" CssClass="form-control input-sm" MaxLength="30" TabIndex="14" ReadOnly="True" data-toggle="tooltip" title="not editable from this screen!"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicle Name :</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtVehicleName" runat="server" CssClass="form-control input-sm" MaxLength="25" TabIndex="2" Width="200"></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="RFD_VehicleName" runat="server" ControlToValidate="txtVehicleName" Display="Dynamic" ErrorMessage="Please enter vehicle name." ForeColor="Red" SetFocusOnError="True" ValidationGroup="VehicleValidation"></asp:RequiredFieldValidator>--%>
                            <asp:HiddenField ID="hdfVehicleId" runat="server"></asp:HiddenField>
                            <asp:HiddenField ID="HDF_TotalVehicle" runat="server" />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Current Odometer:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtCurrentOdometer" runat="server" CssClass="form-control input-sm" MaxLength="7" Width="70" TabIndex="15" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                            <asp:CompareValidator ID="CV_CurrOdo" runat="server" Display="Dynamic" ErrorMessage="Please enter current odometer in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="VehicleValidation" ControlToValidate="txtCurrentOdometer"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Description:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="3" Rows="2" TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Previous Odometer:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtPrevOdometer" runat="server" CssClass="form-control input-sm" MaxLength="7" Width="70" TabIndex="16" onkeypress="return onlyNumbers(event);" ReadOnly="True" data-toggle="tooltip" title="not editable from this screen!"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Department
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Dept" runat="server" TabIndex="4" CssClass="form-control input-sm"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFD_Dept" runat="server" ControlToValidate="DDL_Dept" Display="Dynamic" ErrorMessage="Please select department." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="VehicleValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Last Fueler:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtLastFueler" runat="server" CssClass="form-control input-sm" MaxLength="30" TabIndex="17" ReadOnly="True" data-toggle="tooltip" title="not editable from this screen!"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Account ID:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtAccId" runat="server" CssClass="form-control input-sm" MaxLength="20" Width="165" TabIndex="5"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Current Hours:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtCurrentHrs" runat="server" CssClass="form-control input-sm" MaxLength="7" Width="70" TabIndex="18" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                            <asp:CompareValidator ID="CVCurrentHrs" runat="server" Display="Dynamic" ErrorMessage="Please enter current hours in integer format." 
                                ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="VehicleValidation" ControlToValidate="txtCurrentHrs"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-6 col-sm-6 textright col-xs-12">
                            &nbsp;                           
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Previous Hours:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtPreviousHours" runat="server" CssClass="form-control input-sm" MaxLength="7" Width="70" TabIndex="18" onkeypress="return onlyNumbers(event);" ReadOnly="True" data-toggle="tooltip" title="not editable from this screen!"></asp:TextBox>
                            <asp:CompareValidator ID="CVPreviousHours" runat="server" Display="Dynamic" ErrorMessage="Please enter Previous hours in integer format." 
                                ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="VehicleValidation" ControlToValidate="txtPreviousHours"></asp:CompareValidator>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                            Make:
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtMake" runat="server" CssClass="form-control input-sm" MaxLength="15" Width="130" TabIndex="6"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Require Odometer Entry:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <div class="form-group col-md-4 col-sm-4 text-left col-xs-12" style="padding: 0;">
                                <asp:CheckBox ID="CHK_RequireOdometerEntry" runat="server" TabIndex="18" Style="padding: 0;" />
                            </div>
                            <div class="form-group col-md-4 col-sm-4 col-xs-12" style="padding: 0;">
                                <label>Hours:</label>
                            </div>
                            <div class="form-group col-md-4 col-sm-4 col-xs-12" style="padding: 0;">
                                <asp:CheckBox ID="CHK_Hours" runat="server" TabIndex="19" />
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                            Model:
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtModel" runat="server" CssClass="form-control input-sm" MaxLength="15" Width="130" TabIndex="7"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Check Odometer/Hours Reasonability:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <div class="form-group col-md-1 col-sm-1 col-xs-12" style="padding: 0">
                                <asp:CheckBox ID="CHK_CheckOdometerReasonable" runat="server" TabIndex="20" OnCheckedChanged="CHK_CheckOdometerReasonable_CheckedChanged" AutoPostBack="true" />
                            </div>
                            <div class="form-group col-md-11 col-sm-11 col-xs-12">
                                <asp:RadioButtonList ID="RBL_UnitType" runat="server" RepeatDirection="Horizontal" CssClass="UnitType Mileage" TabIndex="20">
                                    <asp:ListItem Text="Mileage" Value="1" Selected="True" />
                                    <asp:ListItem Text="Kilometers" Value="2" />
                                </asp:RadioButtonList>
                            </div>

                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                VIN:
                            </label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtVIN" runat="server" CssClass="form-control input-sm" MaxLength="20" Width="180" TabIndex="8"></asp:TextBox>
                        </div>
                        <div id="hideTotalMiles" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Total Miles allowed between Fueling:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtOdoLimit" runat="server" CssClass="form-control input-sm" MaxLength="6" Width="70" TabIndex="20" onkeypress="return onlyNumbers(event);" data-toggle="tooltip" title='"Total Miles allowed between Fueling" represents the maximum amount of miles the vehicle is allowed to travel between fueling. Example: if a vehicle current miles is 1000 and the total miles between fueling is set to 300, the only mileage that will be accepted is between 1000 – 1300. NOTE: If choosing this option, the check odometer/hours reasonability must be checked.'></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Year:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtYear" runat="server" CssClass="form-control input-sm" MaxLength="4" Width="50" TabIndex="9" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                        </div>
                        <div id="HideTotalHours" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Total Hours allowed between Fueling:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtHoursLimit" runat="server" CssClass="form-control input-sm" MaxLength="6" Width="70" TabIndex="20" 
                                    onkeypress="return onlyNumbers(event);" data-toggle="tooltip" title='"Total Hours allowed between Fueling" represents the maximum hours the vehicle is allowed to run between fueling. Example: if a vehicle current hours is 10 and the total hours between fueling is set to 50, the only hours that will be accepted is between 10 – 60. NOTE: If choosing this option, the check odometer/hours reasonability must be checked.'></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                License Plate Number:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtLicensePlateNumber" runat="server" CssClass="form-control input-sm" MaxLength="8" Width="80" TabIndex="10"></asp:TextBox>
                        </div>

                        <div id="hideShowOdometerReasonabilityeither" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>Odometer/Hours Reasonability either:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:RadioButtonList ID="RBL_OdometerReasonabilityConditions" runat="server" RepeatDirection="Vertical" CssClass="UnitType" TabIndex="21">
                                    <asp:ListItem Text="Allow fueling after 3 entry" Value="1" Selected="True" />
                                    <asp:ListItem Text="Don’t allow fueling unless correct" Value="2" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                License State:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtLicenseState" runat="server" CssClass="form-control input-sm" MaxLength="2" Width="40" TabIndex="11"></asp:TextBox>
                        </div>

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Fluid Limit Per Transaction:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtFuelLimitPerTxn" runat="server" CssClass="form-control input-sm" MaxLength="4" Width="50" data-toggle="tooltip" title="0 means unlimited !" Text="0" TabIndex="22" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                            <asp:CompareValidator ID="CV_FuelLimitPerTxn" runat="server" Display="Dynamic" ErrorMessage="Please enter fuel limit per transaction in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="VehicleValidation" ControlToValidate="txtFuelLimitPerTxn"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Type of Vehicle:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtType" runat="server" CssClass="form-control input-sm" MaxLength="20" Width="165" TabIndex="12"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Fuel Limit Per Day:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtFuelLimitPerDay" runat="server" CssClass="form-control input-sm" MaxLength="4" Width="50" data-toggle="tooltip" title="0 means unlimited !" Text="0" TabIndex="23" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                            <asp:CompareValidator ID="CV_FuelLimitPerDay" runat="server" Display="Dynamic" ErrorMessage="Please enter fuel limit per day in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="VehicleValidation" ControlToValidate="txtFuelLimitPerDay"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Fob/Card Number:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="TXT_FoBNUM" runat="server" CssClass="form-control input-sm" TabIndex="12"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Expected MPG or Liters/100KM:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="TXT_ExpectedMPGPerK" runat="server" CssClass="form-control input-sm" Width="100" TabIndex="24"></asp:TextBox>
                            <asp:CompareValidator ID="CV_ExpectedMPGPerK" runat="server" Display="Dynamic" ErrorMessage="Please enter Expected MPG or Liters/100KM in decimal format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Double" ValidationGroup="VehicleValidation" ControlToValidate="TXT_ExpectedMPGPerK"></asp:CompareValidator>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Select Products<label class="text-danger font-required">[required]:</label>
                            </label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <input type="button" id="BTN_FuelType" onclick="OpenFuelTypeBox();" tabindex="21" value="Click to add products" />
                        </div>                     
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Export Code:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtExportCode" runat="server" CssClass="form-control input-sm" MaxLength="25" Width="200" TabIndex="25"></asp:TextBox>
                        </div>

                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Authorized FluidSecure Links:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <input type="button" id="BTN_PersonSite" tabindex="13" onclick="OpenPersonSiteBox();" value="Add FluidSecure Hoses/Sites" />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Comments:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtComment" runat="server" CssClass="form-control input-sm" MaxLength="2000" TabIndex="26" Rows="2" TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Active:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="CHK_Active" runat="server" />
                        </div>
                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" TabIndex="27" AutoPostBack="true" CssClass="form-control input-sm" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="VehicleValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								FSTag Mac Address:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txtFSTagMacAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="13"></asp:TextBox>
						</div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
							<label>
								Current Firmware Version:</label>
						</div>
						<div class="form-group col-md-3 col-sm-3 col-xs-12">
							<asp:TextBox ID="txt_FirmwareVer" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="28"></asp:TextBox>
						</div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="False" TabIndex="28" ValidationGroup="VehicleValidation" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="29" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
                            UseSubmitBehavior="False" TabIndex="30" ValidationGroup="VehicleValidation" OnClick="btnSaveAndAddNew_Click" />
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center clear col-xs-12" style="margin: 10px 0">
                        <asp:Button ID="btnFirst" runat="server" Text="|<" CssClass="NewDept_ButtonFooter"
                            OnClick="First_Click" /><asp:Button ID="btnprevious" runat="server" Text="<" CssClass="NewDept_ButtonFooter" OnClick="btnprevious_Click" />
                        <asp:Label ID="lblof" runat="server" Text="Label" BorderColor="Black" BorderStyle="Solid"
                            BorderWidth="1px" Font-Bold="True" Font-Names="arial" Font-Size="Small" Width="115px"></asp:Label>
                        <asp:Button ID="btnNext" runat="server" Text=">" CssClass="NewDept_ButtonFooter" OnClick="btnNext_Click" /><asp:Button
                            ID="btnLast" runat="server" Text=">|" CssClass="NewDept_ButtonFooter" OnClick="btnLast_Click" />
                    </div>
                </div>
            </div>


        </ContentTemplate>
    </asp:UpdatePanel>
    <script>
        $(function () {
            loadFunction();
        });
        function loadFunction() {
            $('[data-toggle="tooltip"]').tooltip();
        }
    </script>
    <style>
        .ui-tooltip {
            background-color: #ffffff;
            font-size: 12px;
            padding: 6px;
            z-index: 9999;
        }

        th {
            text-align: center;
        }
    </style>
    <script type="text/javascript">

        function ClosePopUp() {
            $("#btnClose").click();
            $('body').removeClass("modal-open");
            $('#<%= txtLastFuelDateTime.ClientID%>').focus();
            enableDisableButtons(false)
        }

        function ClosePopUpFuel() {
            $("#btnCloseFuel").click();
            $('body').removeClass("modal-open");
            $('#<%= txtExportCode.ClientID%>').focus();
            enableDisableButtons(false)

        }

        function OpenFuelTypeBox() {
            $("#FuelInput").val("");
            SearchFuel();
            enableDisableButtons(true)
            $('#FuelTypeBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });

        }

        function OpenPersonSiteBox() {
            $("#siteInput").val("");
            SearchSite();
            enableDisableButtons(true)
            $('#PersonSites').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function enableDisableButtons(enableDisable) {
            if (document.getElementById('<%= btnFirst.ClientID%>') != null) {
                document.getElementById('<%= btnFirst.ClientID%>').disabled = enableDisable;
                document.getElementById('<%= btnLast.ClientID%>').disabled = enableDisable;
                document.getElementById('<%= btnNext.ClientID%>').disabled = enableDisable;
                document.getElementById('<%= btnprevious.ClientID%>').disabled = enableDisable;
            }

            document.getElementById('<%= btnSave.ClientID%>').disabled = enableDisable;
            document.getElementById('<%= btnCancel.ClientID%>').disabled = enableDisable;
        }
    </script>
    <script language="javascript">

        function SelectAllCheckboxesSpecific(spanChk) {

            var IsChecked = spanChk.checked;
            var Chk = spanChk;
            Parent = document.getElementById('<%= gv_FuelTypes.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
                if (items[i].id != Chk && items[i].type == "checkbox") {
                    if (items[i].checked != IsChecked) {
                        items[i].click();
                    }
                }
            }
        }

        function SelectboxFuel(spanChk) {
            debugger
            var IsChecked = spanChk.checked;
            if (IsChecked == false) {
                Parent = document.getElementById('<%= gv_FuelTypes.ClientID %>');
                var checkBoxSelector = "#<%=gv_FuelTypes.ClientID%> input[id*='chkAll']";

                $(checkBoxSelector).attr('checked', false);
            }
        }

        function SelectAllCheckboxesSpecificForSite(spanChk) {

            var IsChecked = spanChk.checked;
            var Chk = spanChk;
            Parent = document.getElementById('<%= gv_Sites.ClientID %>');
            var items = Parent.getElementsByTagName('input');
            for (i = 0; i < items.length; i++) {
                if (items[i].id != Chk && items[i].type == "checkbox") {
                    if (items[i].checked != IsChecked) {
                        items[i].click();
                    }
                }
            }
        }

        function SelectboxSite(spanChk) {
            debugger
            var IsChecked = spanChk.checked;
            if (IsChecked == false) {
                Parent = document.getElementById('<%= gv_Sites.ClientID%>');
                var checkBoxSelector = "#<%=gv_Sites.ClientID%> input[id*='chkAll']";

                $(checkBoxSelector).attr('checked', false);
            }
        }

    </script>
    <script>
        function SearchSite() {
            var input, filter, table, tr, td, i;
            input = document.getElementById("siteInput");
            filter = input.value.toLowerCase();
            table = document.getElementById('<%= gv_Sites.ClientID %>');
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
        function SearchFuel() {

            var input, filter, table, tr, td, i;
            input = document.getElementById("FuelInput");
            filter = input.value.toLowerCase();
            table = document.getElementById('<%= gv_FuelTypes.ClientID %>');
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

        function AddMapping() {
            $("#btnCloseMapping").click();
            $("#mydiv").show();
            var VehicleId = $("#<%= hdfVehicleId.ClientID  %>").val();
            var CustomerId = $("#<%= DDL_Customer.ClientID  %>").val();
            $.ajax({
                type: "POST",
                url: "Vehicle.aspx/AddVehicleToAllPersons",
                data: '{VehicleId: "' + VehicleId + '",CustomerId : "' + CustomerId + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $("#mydiv").hide();
                },
                failure: function (response) {
                    alert(response.d);
                }
            });


        }

        function OpenPersonVehicleMappingBox(isNew) {
            if (isNew == 1) {
                $('#VehiclePersonMappingAddNew').modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });

            }
            else {
                $('#VehiclePersonMapping').modal({
                    show: true,
                    backdrop: 'static',
                    keyboard: false
                });

            }
        }

        function AddNewMapping() {
            $("#btnCloseMappingAddNewClose").click();
            $("#mydiv").show();
            var VehicleId = $("#<%= hdfVehicleId.ClientID  %>").val();
            var CustomerId = $("#<%= DDL_Customer.ClientID  %>").val();
            $.ajax({
                type: "POST",
                url: "Vehicle.aspx/AddVehicleToAllPersons",
                data: '{VehicleId: "' + VehicleId + '",CustomerId : "' + CustomerId + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $("#mydiv").hide();
                    window.location.href = '/Master/Vehicle';
                },
                failure: function (response) {
                    alert(response.d);
                }
            });


        }
    </script>

</asp:Content>
