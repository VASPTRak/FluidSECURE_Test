<%@ Page Title="Company" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Company.aspx.vb" Inherits="Fuel_Secure.Company" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .OtherLabel {
            cursor: help ;
        }
    </style>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="HDF_UniqueUserId" runat="server"></asp:HiddenField>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12" runat="server" id="divCompanyNumber">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                            Company Number:
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <label id="hdrCompanyNumber" runat="server"></label>
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Company Name
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtCustName" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="50" TextMode="MultiLine" Rows="2" onkeypress="return CheckLength();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFDCustName" runat="server" ControlToValidate="txtCustName"
                                ErrorMessage="Please Enter Company Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                            <asp:HiddenField ID="HDF_Custd" runat="server"></asp:HiddenField>
                            <asp:HiddenField ID="HDF_TotalCust" runat="server" />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Require Login:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="CHK_RequireLogin" runat="server" TabIndex="14" />
                            <asp:Label ID="lblRequireLoginInfo" runat="server" Text="(Login screen required on mobile application)"></asp:Label>
                        </div>

                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

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
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Require Department:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="chk_RequireDepartment" runat="server" TabIndex="15" />
                            <asp:Label ID="lbl_RequireDepartment" runat="server" Text="(Department screen required)"></asp:Label>
                        </div>
                        <%-- <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Require Odometer:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="CHK_RequireOdometer" runat="server" TabIndex="4" />
                            <asp:Label ID="lblRequireOdometerInfo" runat="server" Text="(Odometer screen required on mobile application)"></asp:Label>
                        </div>--%>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <%--<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Contact Address:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtContactAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" Rows="4" TextMode="MultiLine" TabIndex="3"></asp:TextBox>
                        </div>--%>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Street Address:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtStreetAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="3"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Require Personnel PIN:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="chk_RequirePersonnelPIN" runat="server" TabIndex="16" />
                            <asp:Label ID="lbl_RequirePersonnelPIN" runat="server" Text="(Personnel PIN screen required on mobile application)"></asp:Label>
                        </div>

                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                City:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control input-sm" TabIndex="4"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Require Other:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="chk_RequireOther" runat="server" TabIndex="17" />
                            <asp:Label ID="lbl_RequireOther" runat="server" Text="(Other screen required)"></asp:Label>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                State:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtState" runat="server" CssClass="form-control input-sm" TabIndex="5"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Other label:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtOtherLabel" runat="server" CssClass="form-control input-sm" MaxLength="25" Style="width: 150px; float: left; margin-right: 10px;" TabIndex="18"></asp:TextBox>
                            <span class="glyphicon glyphicon-question-sign" data-toggle="tooltip" title="This is an additional optional screen that can be used at the HUB. You can customize this screen to be whatever you choose. This information is not validated"></span>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Zip:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtZip" runat="server" CssClass="form-control input-sm" TabIndex="6"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Vehicle Number Require:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="chk_VehicleNumberRequire" runat="server" TabIndex="19" Checked="true" />
                            <asp:Label ID="lbl_RequireVehicleNumber" runat="server" Text="(Vehicle Number screen required on mobile application)"></asp:Label>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Country:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control input-sm" TabIndex="7"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Export Code:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtExportCode" runat="server" CssClass="form-control input-sm" MaxLength="25" Width="210" TabIndex="20"></asp:TextBox>
                        </div>

                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">

                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Contact Phone Number
                        <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtContactNumber" runat="server" CssClass="form-control input-sm" MaxLength="15" TabIndex="8" Width="130" data-toggle="tooltip" title="Only (,),-, space, and + symbols allowed."></asp:TextBox>
                            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Style="display: none;" Text="Please enter valid contact number."></asp:Label>
                            <asp:RequiredFieldValidator ID="RFDContactNo" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Please Enter Contact Number."
                                ControlToValidate="txtContactNumber" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Contact Email
                               <label class="text-danger font-required">[required]</label>:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtAdminUsername" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="21" Width="300" TextMode="Email"></asp:TextBox>
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
                            <asp:TextBox ID="txtAdminPassword" runat="server" CssClass="form-control input-sm" MaxLength="50" TextMode="Password" Width="300" TabIndex="9"></asp:TextBox>
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
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control input-sm" TextMode="Password" MaxLength="50" Width="300" TabIndex="22"></asp:TextBox>
                            <asp:CompareValidator ID="RequiredFieldValidator3" runat="server" Font-Size="Small"
                                Font-Bold="False" Font-Names="arial" ErrorMessage="Password and confirm password not matched."
                                ControlToValidate="txtConfirmPassword" ControlToCompare="txtAdminPassword" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="divPricing" runat="server">
                        <div runat="server" id="DivHideActive" style="visibility: hidden">
                            <div class="form-group col-md-6 col-sm-6 textright col-xs-12">
                            </div>
                        </div>
                        <div runat="server" id="DivShowActive">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Active:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:CheckBox runat="server" ID="chkIsActive" Checked="true" TabIndex="11" />
                            </div>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Costing Method:</label>
                            <%--<label class="text-danger font-required">[required]:</label>--%>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:DropDownList ID="DDL_Costing" runat="server" TabIndex="23" CssClass="form-control input-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_Costing_SelectedIndexChanged">
                                <asp:ListItem Text="Select Costing Method" Value="0" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Fixed Price" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Price Averaging" Value="2"></asp:ListItem>
                                <asp:ListItem Text="First In First Out (FIFO)" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                            <%-- <asp:RequiredFieldValidator ID="RDF_Costing" runat="server" ControlToValidate="DDL_Costing" Display="Dynamic"
                                ErrorMessage="Please select Costing Method." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                            --%>
                            <asp:HiddenField ID="hdfType" runat="server" Value="0" />
                        </div>
                    </div>
                    <div runat="server" id="divDates">
                        <div class="row col-md-12 col-sm-12 col-xs-12" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>Start Date:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <b>
                                    <asp:Label runat="server" ID="lblStartDate" Text="-"></asp:Label>
                                </b>
                            </div>
                        </div>

                        <div class="row col-md-12 col-sm-12 col-xs-12" runat="server">
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>Beginning Hosting Date:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtBeginningHostingDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="7"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RDF_BeginningHostingDate" runat="server" ControlToValidate="txtBeginningHostingDate" Display="Dynamic" Style="float: left;"
                                    ErrorMessage="Please select Beginning Hosting Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>Ending Hosting Date:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtEndingHostingDate" runat="server" Width="100" Style="float: left; margin-right: 10px;" CssClass="form-control input-sm" TabIndex="15"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RDF_EndingHostingDate" runat="server" ControlToValidate="txtEndingHostingDate" Display="Dynamic" Style="float: left;"
                                    ErrorMessage="Please select Ending Hosting Date." ForeColor="Red" SetFocusOnError="True" ValidationGroup="CustValidation"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>
                    <%--<div class="row col-md-12 col-sm-12 col-xs-12" runat="server" id="Div2">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Click to Enable/Disable All Vehicle's Odometer</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:Button ID="btn_EnableAllVehOdo" runat="server" Text="Enable" OnClientClick="setValueLable(1)" OnClick="btn_EnableAllVehOdo_Click" TabIndex="12"  />
                            <asp:Button ID="btn_DisableAllVehOdo" runat="server" Text="Disable" OnClientClick="setValueLable(2)" OnClick="btn_DisableAllVehOdo_Click" TabIndex="13"  />
                        </div>
                    </div>--%>
                    <div class="row col-md-12 col-sm-12 col-xs-12" id="trLabel" runat="server">
                        <p class="green" style="text-align: center">You can reset  Contact Email password from Personnel screen. </p>
                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="true" TabIndex="24" ValidationGroup="CustValidation" OnClientClick="return IsValidPhoneNumber();" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="25" OnClick="btnCancel_Click" />
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

            <asp:HiddenField runat="server" ID="hdfEnableDisable" Value="0" />
            <asp:HiddenField runat="server" ID="hdfCostingMethodValue" Value="0" />

        </ContentTemplate>
    </asp:UpdatePanel>

    <!--VehOdo model-->
    <div class="modal fade" tabindex="-1" role="dialog" id="VehOdoModel">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title text-center">FluidSecure</h5>
                </div>
                <div class="modal-body">
                    <div class="row col-md-12 col-sm-12" style="padding: 0px;">
                        <asp:Label ID="lblVehodo" runat="server" Text=""></asp:Label>
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
    <script src="../Scripts/jquery.maskedinput.js"></script>

    <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">
    <script src="/Scripts/jquery.quicksearch.js"></script>

    <script>

        function LoadDateTimeControl() {

            $("[id$=txtBeginningHostingDate]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                maxDate: 0
            });

            $("[id$=txtEndingHostingDate]").datepicker({
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '/Content/images/calendar.png',
                changeMonth: true,
                changeYear: true,
                yearRange: "-10:+100",

            });

        }

        function CheckLength() {
            var textbox = document.getElementById("<%=txtCustName.ClientID%>").value;
            if (textbox.trim().length >= 50) {
                return false;
            }
            else {
                return true;
            }
        }
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
        function loadFunction() {
            $('[data-toggle="tooltip"]').tooltip();
            //$("#<%=txtContactNumber.ClientID%>").mask("999-999-9999");
            }
        $(function () {
            loadFunction();
        });


        function OpenModalMessage() {
            $('#modalMessage').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function ClosePopUpSite() {
            $("#btnCloseSite").click();
            $('body').removeClass("modal-open");
            $('#<%= txtCustName.ClientID%>').focus();
        }


        function OpenModalWarningMessage() {
            $('#modalWarningMessage').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function ClosePopUpWarningMessage() {
            $("#btnCloseWarningMessage").click();
            $('body').removeClass("modal-open");
            $('#<%= DDL_Costing.ClientID%>').focus();
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
            debugger;
            if (value == 1) {
                document.getElementById('<%=lblVehodo.ClientID %>').innerText = "Are you sure you want to Enable odometer entry for all vehicles in this Company ?";
            }
            else {
                document.getElementById('<%=lblVehodo.ClientID %>').innerText = "Are you sure you want to Disable odometer entry for all vehicles in this Company ?";
            }

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

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="modal fade" tabindex="-1" role="dialog" id="modalMessage">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3 class="modal-title text-center">FluidSecure</h3>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblErrorMessage" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnSiteOk" class="btn btn-success" onclick="ClosePopUpSite();" value="Ok" />
                            <input type="button" id="btnCloseSite" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->

            <div class="modal fade" tabindex="-1" role="dialog" id="modalWarningMessage">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3 class="modal-title text-center">FluidSecure</h3>
                        </div>
                        <div class="modal-body" style="padding:7px;">
                            <div class="row col-md-12 col-sm-12" style="padding-left:0px;margin:0px">
                                <h4 class="modal-title text-center" style="color: red">Warning</h4>
                                <br />
                                <asp:Label ID="lblWarningMessage" runat="server" style="float:left" Text="You have change Costing Method. This could result in unexpected consequences and that all data needs to be reset to a zero starting point if you wish to continue."></asp:Label>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <input type="button" id="btnWarningMessageOk" class="btn btn-success" onclick="ClosePopUpWarningMessage();" value="Close" />
                            <input type="button" id="btnCloseWarningMessage" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
