<%@ Page Title="Reset Password" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ResetPassword.aspx.vb" Inherits="Fuel_Secure.ResetPassword1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:HiddenField ID="HDF_PersonnelId" runat="server"></asp:HiddenField>
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
            <div class="row col-md-12 col-sm-12 col-xs-12">
                <div class="col-md-3"></div>
                <div class="col-md-6">
                    <p>Password MUST be 6 characters long and contain one (1) of the following:</p>
                    <p>-  Alphabetic Character</p>
                    <p>- Capital Alphabetic Character</p>
                    <p>- Number</p>
                    <p>- Special Character</p>
                </div>
            </div>
            <div class="row col-md-12 col-sm-12 col-xs-12">
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                </div>
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                    <label>
                        New password
                        <label class="text-danger font-required">[required]</label>:</label>
                </div>
                <div class="form-group col-md-3 col-sm-3 col-xs-12">
                    <asp:TextBox ID="txtPassword" TextMode="Password" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="30" Width="200"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFDResetPassword" runat="server" ControlToValidate="txtPassword"
                        ErrorMessage="Please Enter Password." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row col-md-12 col-sm-12 col-xs-12">
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                </div>
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                    <label>Confirm password
                         <label class="text-danger font-required">[required]</label>:
                    </label>
                </div>
                <div class="form-group col-md-3 col-sm-3 col-xs-12">
                    <asp:TextBox ID="txtConfirmPassword" TextMode="Password" runat="server" CssClass="form-control input-sm" MaxLength="30" Width="200" TabIndex="2"></asp:TextBox>
                    <asp:CompareValidator ID="CompareValidator" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword"
                        ErrorMessage="Passwords do not match." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:CompareValidator>
                    <asp:RequiredFieldValidator ID="RFDCResetPassword" runat="server" ControlToValidate="txtConfirmPassword"
                        ErrorMessage="Please Enter Confirm Password." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px" UseSubmitBehavior="False" TabIndex="26" ValidationGroup="PersonelValidation" />
                <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="110px" CausesValidation="False"
                    UseSubmitBehavior="False" TabIndex="27" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>

    <script>

        function ShowMsg() {

            setTimeout(function () {
                window.location.href = "/Master/AllPersonnel"
            }, 1000)
        }

        function GoToLoginPage() {
            $('#GoToLoginPage').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

    </script>

     <!--alert message popup-->
        <div class="modal fade" tabindex="-1" role="dialog" id="GoToLoginPage">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3 class="modal-title text-center">FluidSecure</h3>
                    </div>
                    <div class="modal-body">
                        <h4>Password Reset Successfully. Please Login Again With New Password.</h4>
                    </div>
                    <div class="modal-footer nextButton">
                       <input type="button" id="btnMyModalSuccess" class="btn btn-success" onclick="window.location.href = '/Account/Login'" value="Ok" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
</asp:Content>
