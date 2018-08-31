<%@ Page Title="Product" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Fuel.aspx.vb" Inherits="Fuel_Secure.Fuel" %>

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
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Product Type
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtFuelType" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="20" Width="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFDFuelType" runat="server" ControlToValidate="txtFuelType"
                                ErrorMessage="Please Enter Fluid Type." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="FuelValidation"></asp:RequiredFieldValidator>
                            <asp:HiddenField ID="HDF_FuelTypeId" runat="server"></asp:HiddenField>
                            <asp:HiddenField ID="HDF_TotalFuelType" runat="server" />
                        </div>
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
                            <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="DDL_Customer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="FuelValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Export Code:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtExportCode" runat="server" CssClass="form-control input-sm" MaxLength="25" Width="200" TabIndex="2"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Product Price:</label>
                        <%--<label class="text-danger font-required">[required]</label>--%>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtProductPrice" runat="server" CssClass="form-control input-sm" MaxLength="25" Width="200" TabIndex="2"></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="RFD_ProductPrice" runat="server" ControlToValidate="txtProductPrice" Display="Dynamic" ErrorMessage="Please enter Product Price." ForeColor="Red" SetFocusOnError="True" ValidationGroup="FuelValidation"></asp:RequiredFieldValidator>--%>
                           <%-- <asp:CompareValidator ID="CV_ProductPrice" runat="server" Display="Dynamic" ErrorMessage="Please enter Quantity in decimal format." ForeColor="Red" Operator="DataTypeCheck"
                                SetFocusOnError="True" Type="Double" ValidationGroup="FuelValidation" ControlToValidate="txtProductPrice"></asp:CompareValidator>--%>
                            <asp:RegularExpressionValidator ID="regexpPrice" runat="server" Display="Dynamic" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="FuelValidation"
                                    ErrorMessage="You are allow to enter till 3 decimal places. [eg. xxx.xxx]" 
                                    ControlToValidate="txtProductPrice"     
                                    ValidationExpression="^[0-9]\d{0,9}(\.\d{0,3})*(,\d+)?$" />
                                                           <%--^\d+\.\d{0,3}$
                                                         <%-- ^[1-9]\d*(\.\d+)?$  --%>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="False" TabIndex="26" ValidationGroup="FuelValidation" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="27" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnUpdatePrice" CssClass="btn btn-default" runat="server" Text="Re-Price" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="27" OnClick="btnUpdatePrice_Click" />
                        <asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
                            UseSubmitBehavior="false" TabIndex="27" ValidationGroup="FuelValidation" OnClick="btnSaveAndAddNew_Click" />
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
</asp:Content>
