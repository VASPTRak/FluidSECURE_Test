<%@ Page Title="Transaction" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Transaction.aspx.vb" Inherits="Fuel_Secure.Transaction" %>

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
                        <p class="text-center red" id="ManuallyEditMessage" runat="server"></p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicle Number
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <input type="button" id="BTN_Vehicles" tabindex="1" onclick="OpenVehicleTypeBox();" value="Click to add vehicle" />
                            <asp:Label ID="lbl_VehicleNumber" runat="server"></asp:Label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Date 
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="10"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RDF_TranDate" runat="server" ControlToValidate="txtTransactionDate" Display="Dynamic" Style="float: left;"
                                ErrorMessage="Please select Transaction Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicle Name:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Label ID="lblVehicleName" runat="server" TabIndex="2"></asp:Label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Transaction Time 
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtTransactionTime" runat="server" Width="150" CssClass="form-control input-sm" TabIndex="11"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RDF_TransTime" runat="server" ControlToValidate="txtTransactionTime" Display="Dynamic"
                                ErrorMessage="Please select Transaction Time ." ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                        </div>

                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Department
                                <label class="text-danger font-required">[required]:</label></label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Dept" runat="server" TabIndex="3" CssClass="form-control input-sm" Enabled="false"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Dept" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Department." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Person
                                <%--<label class="text-danger font-required">[required]</label>:--%>
                            </label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <input type="button" id="BTN_Persons" tabindex="12" onclick="OpenPersonTypeBox();" value="Click to add Person" />
                            <label runat="server" id="lblPerson" style="color: red"></label>
                            <%-- <asp:DropDownList ID="DDL_Personnel" runat="server" TabIndex="13" CssClass="form-control input-sm" OnSelectedIndexChanged="DDL_Personnel_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>

                            <asp:RequiredFieldValidator ID="RDF_Personnel" runat="server" ControlToValidate="DDL_Personnel" Display="Dynamic" ErrorMessage="Please select Personnel." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>--%>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div runat="server" id="divDepartmentNumber" visible="false">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Department Number:
                                <label class="text-danger font-required">[required]:</label>
                                </label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtDeptNo" CssClass="form-control input-sm" TabIndex="4" runat="server" MaxLength="10" Width="110"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RFD_txtDeptNo" runat="server" ControlToValidate="txtDeptNo" Display="Dynamic" ErrorMessage="Please enter Department number." ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div runat="server" id="divPersonnel">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Person PIN:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtPinNumber" runat="server" CssClass="form-control input-sm" MaxLength="10" Width="95" TabIndex="13" OnTextChanged="txtPinNumber_TextChanged" AutoPostBack="true"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <%-- <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                        </div>--%>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                FluidSecure Link
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">

                            <asp:DropDownList ID="DDL_Site" runat="server" TabIndex="5" CssClass="form-control input-sm" OnSelectedIndexChanged="DDL_Site_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Site" runat="server" ControlToValidate="DDL_Site" Display="Dynamic" ErrorMessage="Please select FluidSecure Link." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>

                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Selected Person
                                <label class="text-danger font-required">[required]</label>:
                            </label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Label ID="LBL_SelectedPerson" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Fuel Type
                        <label class="text-danger font-required">[required]</label>:
                            </label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Fuel" runat="server" TabIndex="6" CssClass="form-control input-sm" Enabled="false"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Fuel" runat="server" ControlToValidate="DDL_Fuel" Display="Dynamic" ErrorMessage="Please select Fuel." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>

                            <asp:HiddenField ID="hdfTransactionId" runat="server"></asp:HiddenField>
                            <asp:HiddenField ID="HDF_TotalTransactions" runat="server" />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Current Odometer:
                            </label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtCurrentOdometer" runat="server" CssClass="form-control input-sm" MaxLength="7" Width="70" TabIndex="14" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                            <%--<label runat="server" id="lblcurrODO" style="color: red"></label>
                            <asp:RequiredFieldValidator ID="RFD_CurrOdo" runat="server" ControlToValidate="txtCurrentOdometer" Display="Dynamic" ErrorMessage="Please enter current odometer." ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CV_CurrOdo" runat="server" Display="Dynamic" ErrorMessage="Please enter current odometer in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="TransactionValidation" ControlToValidate="txtCurrentOdometer"></asp:CompareValidator>--%>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div id="PreviousOdometer" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            </div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            </div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Previous Odometer:
                                </label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtPreviousOdometer" runat="server" CssClass="form-control input-sm" MaxLength="7" Width="70" TabIndex="15" Text="0" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                                <%--<label runat="server" id="lblprevODO" style="color: red"></label>
                                <asp:RequiredFieldValidator ID="RDF_PreviousOdometer" runat="server" ControlToValidate="txtPreviousOdometer" Display="Dynamic" ErrorMessage="Please enter previous odometer." ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CV_PreviousOdometer" runat="server" Display="Dynamic" ErrorMessage="Please enter previous odometer in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="TransactionValidation" ControlToValidate="txtPreviousOdometer"></asp:CompareValidator>--%>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Fuel Quantity
                                <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtFuelQuantity" runat="server" CssClass="form-control input-sm checkDecimal" Width="80px" TabIndex="7" onchange="return onlyDecimal();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFD_FuelQuantity" runat="server" ControlToValidate="txtFuelQuantity" Display="Dynamic" ErrorMessage="Please enter Fuel Quantity." ForeColor="Red" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CV_Quantity" runat="server" Display="Dynamic" ErrorMessage="Please enter Quantity in decimal format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Double" ValidationGroup="TransactionValidation" ControlToValidate="txtFuelQuantity"></asp:CompareValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Current Hours:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtHours" runat="server" CssClass="form-control input-sm" MaxLength="10" Width="95" TabIndex="16" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                            <asp:CompareValidator ID="CVHours" runat="server" ControlToValidate="txtHours" Display="Dynamic" ErrorMessage="Please enter Hours in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="TransactionValidation"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div runat="server" id="divTransactionStatus">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Transaction Status
                                <label class="text-danger font-required">[required]</label>:
                                </label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:DropDownList ID="DDL_TransactionStatus" runat="server" TabIndex="8" CssClass="form-control input-sm">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RDF_TransactionStatus" runat="server" ControlToValidate="DDL_TransactionStatus" Display="Dynamic"
                                    ErrorMessage="Please select Transaction Status." ForeColor="Red" InitialValue="-1" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div id="divPrevHours" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Previous Hours:</label>
                                </label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtPreviousHours" runat="server" CssClass="form-control input-sm" MaxLength="7" Width="70" TabIndex="17" Text="0" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                                <asp:CompareValidator ID="CVPreviousHours" runat="server" Display="Dynamic" ErrorMessage="Please enter previous hours in integer format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="TransactionValidation" ControlToValidate="txtPreviousHours"></asp:CompareValidator>
                            </div>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Other:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtOther" CssClass="form-control input-sm" TabIndex="9" runat="server"></asp:TextBox>
                        </div>
                        <div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company
                                <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="18" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RDF_Customer" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic"
                                ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="TransactionValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                     <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div id="divFinalQuantity" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Final Quantity:</label>
                                </label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:Label Style="font-weight: bold;" ID="LblFinalQuantity" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div runat="server" id="divGuestVehicleNumber" visible="false">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Guest Vehicle Number:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtGuestVehicleNumber" CssClass="form-control input-sm" TabIndex="19" runat="server" MaxLength="10" Width="110"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div id="HideCost" runat="server">
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Cost:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <label style="font-weight: bold">$</label><label runat="server" id="lblCost" style="font-weight: bold"></label>
                            </div>

                        </div>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Cost Per Gallon:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <label style="font-weight: bold">$</label><label runat="server" id="lblCostPerGallon" style="font-weight: bold"></label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Surcharge Type:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <label runat="server" id="lblSurchargeType" style="font-weight: bold"></label>
                            </div>
                        </div>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Vehicle Sum:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <label runat="server" id="lblVehicleSum" style="font-weight: bold"></label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Department Sum:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <label runat="server" id="lblDeptSum" style="font-weight: bold"></label>
                            </div>
                        </div>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Vehicle Percentage:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <label runat="server" id="lblVehPercentage" style="font-weight: bold"></label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Department Percentage:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <label runat="server" id="lblDeptPercentage" style="font-weight: bold"></label>
                            </div>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="False" TabIndex="22" ValidationGroup="TransactionValidation" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="23" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
                            UseSubmitBehavior="False" TabIndex="30" ValidationGroup="TransactionValidation" OnClick="btnSaveAndAddNew_Click" />
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

            <asp:HiddenField ID="HDF_VehicleId" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="HDF_VehicleNumber" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="HDF_HubId" runat="server" Value="0"></asp:HiddenField>

            <div class="modal fade" tabindex="-1" role="dialog" id="VehicleBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Click box to authorize Person to select this Vehicle at the FluidSecure Link:</h5>
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
                            <asp:Button ID="btnOk" runat="server" CssClass="btn btn-success" Text="Ok" OnClientClick="ClosePopUp()" OnClick="btnOk_Click" />
                            <input type="button" id="btnClose" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                            <%--<asp:Button ID="btnCloseVehicle" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="ClosePopUp()" OnClick="btnCloseVehicle_Click" />--%>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->


            <asp:HiddenField ID="hdf_PersonId" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="hdf_UniqueId" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="hdf_PersonName" runat="server"></asp:HiddenField>

            <div class="modal fade" tabindex="-1" role="dialog" id="PersonBox">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title text-center">Click box to select this Person at the FluidSecure Link:</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblPersonMessage" runat="server" Text="Please select company."></asp:Label>
                            </div>
                            <div class="row margin10 text-center">
                                <input type="text" class="form-control" id="PersonInput" onkeyup="SearchPersons()" placeholder="Search for Person">
                            </div>
                            <div class="row col-md-12 col-sm-12" style="overflow-x: auto; max-height: 400px;">
                                <asp:UpdatePanel ID="UP_Person" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gv_Persons" CssClass="table table-bordered" runat="server" DataKeyNames="PersonId,PinNumber,Id" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:RadioButton ID="RDB_Person" runat="server" onclick="RadioCheckPerson(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="PersonName" HeaderText="Person Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="PinNumber" HeaderText="PIN Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <asp:Button ID="btnOkPerson" runat="server" CssClass="btn btn-success" Text="Ok" OnClientClick="ClosePopUpPerson()" OnClick="btnOkPerson_Click" />
                            <input type="button" id="btnCloseperson" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
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
        $(function () {
            /*
			Example 1
			*/
            $('input#id_search').quicksearch('table#MainContent_gv_Vehicles tbody tr');
        });

        function ClosePopUp() {
            $("#btnClose").click();
            $('body').removeClass("modal-open");
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

        function LoadDateTimeControl() {

            $("[id$=txtTransactionDate]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $('[id$=txtTransactionTime]').timepicker({
                timeFormat: 'h:mm p',
                interval: 01,
                dynamic: false,
                dropdown: true,
                scrollbar: true

            });
        }

        function OpenVehicleTypeBox() {
            $("#VehicleInput").val("");
            //SearchVehicles();

            $('#VehicleBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });

        }



		<%--function SearchVehicles() {

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
							td = tr[i].getElementsByTagName("td")[2];
							if (td) {
								if (td.innerText.toLowerCase().indexOf(filter) > -1) {
									tr[i].style.display = "";
								} else {
									td = tr[i].getElementsByTagName("td")[3];
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
					}
				}

			}
		}--%>


        function ClosePopUpPerson() {
            $("#btnCloseperson").click();
            $('body').removeClass("modal-open");
        }

        function RadioCheckPerson(rb) {

            var gv = document.getElementById("<%=gv_Persons.ClientID%>");

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

        function OpenPersonTypeBox() {
            $("#PersonInput").val("");
            SearchPersons();

            $('#PersonBox').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });

        }
        function SearchPersons() {

            var input, filter, table, tr, td, i;
            input = document.getElementById("PersonInput");
            filter = input.value.toLowerCase();
            table = document.getElementById('<%= gv_Persons.ClientID %>');
            if (table != null) {
                tr = table.getElementsByTagName("tr");
                for (i = 0; i < tr.length; i++) {
                    td = tr[i].getElementsByTagName("td")[1];
                    if (td) {
                        if (td.innerText.toLowerCase().indexOf(filter) > -1) {
                            tr[i].style.display = "";
                        } else {
                            td = tr[i].getElementsByTagName("td")[2];
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

            }
        }
    </script>


</asp:Content>
