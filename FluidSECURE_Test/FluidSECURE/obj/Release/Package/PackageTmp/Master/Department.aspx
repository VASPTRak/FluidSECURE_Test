<%@ Page Title="Department" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Department.aspx.vb" Inherits="Fuel_Secure.Department" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>

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
                                Department Number:
                        <label class="text-danger font-required">[required]</label></label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtDeptNo" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="10" Width="110"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFDDeptNo" runat="server" ControlToValidate="txtDeptNo"
                                ErrorMessage="Please Enter Department Number." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="DeptValidation"></asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtDeptIdHide" runat="server" Visible="False"></asp:TextBox>
                            <asp:HiddenField ID="HDF_TotalDept" runat="server" />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Name:
                                  <label class="text-danger font-required">[required]</label></label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtDeptName" runat="server" CssClass="form-control input-sm" MaxLength="40" TabIndex="7"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVtxtDepartment" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter Department Name."
                                ControlToValidate="txtDeptName" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="DeptValidation"></asp:RequiredFieldValidator></td>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Account Number:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtAccNo" runat="server" CssClass="form-control input-sm" MaxLength="10" Width="95" TabIndex="2"></asp:TextBox>
                        </div>
                        <div ID = "divCompany" runat ="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company:
                        <label class="text-danger font-required">[required]</label></label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Customer" runat="server" TabIndex="8" AutoPostBack="true" CssClass="form-control input-sm"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="DeptValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Address:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtAddress1" runat="server" CssClass="form-control input-sm" MaxLength="25" TabIndex="3" Width="220"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Export Code:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtExportCode" runat="server" CssClass="form-control input-sm" MaxLength="25" TabIndex="9"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Address 2:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control input-sm" MaxLength="25" Width="220" TabIndex="4"></asp:TextBox>
                        </div>

                    </div>

                    <br />
                    <br />
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Surcharge Type:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_SurType" runat="server" TabIndex="5" CssClass="form-control input-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_SurType_SelectedIndexChanged">
                                <asp:ListItem Value="0" Text="Lump Sum Surcharge" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Percentage Surcharge"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Vehicle Surcharge:</label>
                        </div>
                        <div class="form-group col-md-2 col-sm-2 col-xs-12">
                            <asp:TextBox ID="txtVehicleSurcharge" runat="server" CssClass="form-control input-sm" MaxLength="6" Width="200" TabIndex="6"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-1 col-sm-1 col-xs-12">
                            <asp:Label runat="server" ID="lblVPercentage">%</asp:Label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Department Surcharge:</label>
                        </div>
                        <div class="form-group col-md-2 col-sm-2 col-xs-12">
                            <asp:TextBox ID="txtDeptSurcharge" runat="server" CssClass="form-control input-sm" MaxLength="6" Width="200" TabIndex="10"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-1 col-sm-1 col-xs-12">
                            <asp:Label runat="server" ID="lblDPercentage">%</asp:Label>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px" OnClientClick="return Validate();"
                            UseSubmitBehavior="true" TabIndex="11" ValidationGroup="DeptValidation" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="12" OnClientClick="window.location.href='/Master/AllDepartments?Filter=Filter'" />
                        <asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px" OnClientClick="return Validate();"
                            UseSubmitBehavior="true" TabIndex="13" ValidationGroup="DeptValidation" OnClick="btnSaveAndAddNew_Click" />
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
            <asp:HiddenField runat="server" ID="hdfVehSum" Value="0.0" />
            <asp:HiddenField runat="server" ID="hdfDeptSum" Value="0.0" />
            <asp:HiddenField runat="server" ID="hdfVehPer" Value="0.0" />
            <asp:HiddenField runat="server" ID="hdfDeptPer" Value="0.0" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        function KeyPressProduct(e) {

            var keyCode = e.which ? e.which : e.keyCode;
            var ret = ((keyCode < 48 || keyCode > 57));

            if (ret) {
                ret = (keyCode == 46);
                if (ret) {
                    return true
                }
                else
                    return false
            }
            else
                return true;
        }

        function Validate() {
            var vehsur = document.getElementById('<%= txtVehicleSurcharge.ClientID %>')
            var Deptsur = document.getElementById('<%= txtDeptSurcharge.ClientID %>')
            var ErrorMessage = document.getElementById('<%= ErrorMessage.ClientID %>')
            var DDL_SurType = document.getElementById('<%= DDL_SurType.ClientID %>')
            var message = document.getElementById('<%= message.ClientID %>')
            var SelVal = DDL_SurType.options[DDL_SurType.selectedIndex].value;
            ErrorMessage.style.visibility = "hidden";
            if (vehsur.value == "") {
                if (SelVal == "0") {
                    ErrorMessage.innerText = "Vehicle Surcharge must be between $0 to $999.99";
                    ErrorMessage.style.visibility = "visible";
                }
                else {
                    ErrorMessage.innerText = "Vehicle Surcharge percentage must be between 0 % to 99.99 %";
                    ErrorMessage.style.visibility = "visible";
                }
                return false;
            }
            else if (Deptsur.value == "") {
                if (SelVal == "0") {
                    ErrorMessage.innerText = "Department Surcharge must be between $0 to $999.99";
                    ErrorMessage.style.visibility = "visible";
                }
                else {
                    ErrorMessage.innerText = "Department Surcharge percentage must be between 0 % to 99.99 %";
                    ErrorMessage.style.visibility = "visible";
                }
                return false;
            }


            if (SelVal == "0") {
                if (parseFloat(vehsur.value) > 999.99) {
                    ErrorMessage.innerText = "Vehicle Surcharge must be between $0 to $999.99";
                    ErrorMessage.style.visibility = "visible";
                    return false;
                }
                else if (parseFloat(Deptsur.value) > 999.99) {
                    ErrorMessage.innerText = "Department Surcharge must be between $0 to $999.99";
                    ErrorMessage.style.visibility = "visible";
                    return false;
                }
            }
            else {
                if (parseFloat(vehsur.value) > 99.99) {
                    ErrorMessage.innerText = "Vehicle Surcharge percentage must be between 0 % to 99.99 %";
                    ErrorMessage.style.visibility = "visible";
                    return false;
                }
                else if (parseFloat(Deptsur.value) > 99.99) {
                    ErrorMessage.innerText = "Department Surcharge percentage must be between 0 % to 99.99 %";
                    ErrorMessage.style.visibility = "visible";
                    return false;
                }
            }

            if (Page_ClientValidate("DeptValidation"))
                return true;
            else
                return false;

        }
    </script>

</asp:Content>
