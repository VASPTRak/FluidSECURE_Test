<%@ Page Title="Register Company" Language="vb" AutoEventWireup="false" MasterPageFile="~/Account/login.Master" CodeBehind="RegisterCompany.aspx.vb" Inherits="Fuel_Secure.RegisterCompany" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="HDF_Custd" runat="server"></asp:HiddenField>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Register Company</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company Name
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtCustName" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="50" Width="255"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFDCustName" runat="server" ControlToValidate="txtCustName"
                                ErrorMessage="Please Enter Company Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Contact Name:
                            <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtContactName" runat="server" CssClass="form-control input-sm" MaxLength="30" TabIndex="2" Width="255"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFDContactName" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter Contact Name."
                                ControlToValidate="txtContactName" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator></td>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Contact Address:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtContactAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" Rows="4" TextMode="MultiLine" TabIndex="3"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Contact Phone Number
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtContactNumber" runat="server" CssClass="form-control input-sm" MaxLength="15" TabIndex="4" Width="130" data-toggle="tooltip" title="Only (,),-, space, and + symbols allowed."></asp:TextBox>
                            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Style="display: none;" Text="Please enter valid contact number."></asp:Label>
                            <asp:RequiredFieldValidator ID="RFDContactNo" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter Contact Number."
                                ControlToValidate="txtContactNumber" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                FluidSecure Link Name
                                <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtShiptmentFluidSecureUnitName" runat="server" CssClass="form-control input-sm" MaxLength="32" TabIndex="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVShiptmentFluidSecureUnitName" runat="server" ControlToValidate="txtShiptmentFluidSecureUnitName"
                                ErrorMessage="Please Enter Shipment Fluid Secure Unit Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Contact Email
                               <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtAdminUsername" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="6" Width="300" TextMode="Email"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter Admin Username."
                                ControlToValidate="txtAdminUsername" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RDFEmail" runat="server" ControlToValidate="txtAdminUsername" Display="Dynamic" ErrorMessage="Please enter valid email." ForeColor="Red" SetFocusOnError="True" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="CustValidation"></asp:RegularExpressionValidator>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12" id="trPassword" runat="server">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Admin Password
                              <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtAdminPassword" runat="server" CssClass="form-control input-sm" MaxLength="50" TextMode="Password" Width="300" TabIndex="7"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter Admin Password."
                                ControlToValidate="txtAdminPassword" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Confirm Password
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control input-sm" TextMode="Password" MaxLength="50" Width="300" TabIndex="8"></asp:TextBox>
                            <asp:CompareValidator ID="RequiredFieldValidator3" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Password and confirm password not matched."
                                ControlToValidate="txtConfirmPassword" ControlToCompare="txtAdminPassword" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:CompareValidator>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="true" TabIndex="9" ValidationGroup="CustValidation" OnClientClick="return IsValidPhoneNumber();" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="10" OnClick="btnCancel_Click" />
                    </div>

                </div>
            </div>


        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../Scripts/jquery.maskedinput.js"></script>
    <script>

       <%-- function IsValidPhoneNumber() {
            //debugger;

            var phoneNumber = document.getElementById('<%=txtContactNumber.ClientID%>').value;
            //it accepts 850-294-2562(us phone number- req date 09-Dec-2016)
            //if (phoneNumber.match(/^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/)) {
			if (phoneNumber.match(/^[- +()]*[0-9][- +()0-9]*$/)) {
                document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "none";
                //return true;
                if (Page_ClientValidate("CustValidation"))
                    return true;
                else
                    return false;
            }
            else {
                document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "";
                return false;
            }

		}--%>
		
		function IsValidPhoneNumber() {
			//debugger;

			var phoneNumber = document.getElementById('<%=txtContactNumber.ClientID%>').value;
			//it accepts 850-294-2562(us phone number- req date 09-Dec-2016)
			//if (phoneNumber.match(/^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/)) {
			//phone number accept all number with only (-)+space symbols.
			if (phoneNumber.match(/^[- +()]*[0-9][- +()0-9]*$/)) {
				document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "none";
				//return true;
				if (Page_ClientValidate("CustValidation"))
					return true;
				else
					return false;
			}
			else {
				document.getElementById('<%=lblErrorMsg.ClientID%>').style.display = "";
				return false;
			}

		}

        $(function () {
            loadFuction();
        });
        function loadFuction()
        {
            $('[data-toggle="tooltip"]').tooltip();
            //$("#<%=txtContactNumber.ClientID%>").mask("999-999-9999");
        }

        function SuccessMsg() {

            $('#SuccessMsg').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });

        }
    </script>
    <style>
        .ui-tooltip {
            background-color: #ffffff;
            font-size: 12px;
            padding: 6px;
            z-index: 9999;
        }
    </style>
    <!--alert message popup-->
        <div class="modal fade" tabindex="-1" role="dialog" id="SuccessMsg">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3 class="modal-title text-center">FluidSecure</h3>
                    </div>
                    <div class="modal-body">
                        <h4>Company registered successfully.</h4>
                    </div>
                    <div class="modal-footer nextButton">
                        <input type="button" id="btnMyModalSuccessMsg" class="btn btn-success" onclick="window.location.href = '/Account/login'" value="Ok" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
</asp:Content>
