<%@ Page Title="Reset Password" Language="vb" MasterPageFile="~/Account/login.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.vb" Inherits="Fuel_Secure.ResetPassword" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="login-container" style="background-image: url(../content/images/header-background-2.png)">
        <div class="panel-body" style="background-color: #ffffff;">
            <div class="login-logo">
                <a href="https://www.fluidsecure.com" target="_blank">
                    <img src="../Content/images/FluidSECURELogo.png" style="max-width: 100%" />
                </a>
            </div>
            <p class="text-danger">
                <asp:Literal runat="server" ID="ErrorMessage" />
            </p>

            <div class="row">
                <div class="col-lg-12">
                    <div class="form-group">
                      
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" placeholder="Email" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                CssClass="text-danger" ErrorMessage="The email field is required." />
                       
                    </div>
                    <div class="form-group">
                       
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" placeholder="Password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                                CssClass="text-danger" ErrorMessage="The password field is required." />
                       
                    </div>
                    <div class="form-group">
                        
                            <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" placeholder="Confirm Password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmPassword"
                                CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." />
                            <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                       
                    </div>
                    <br />
                    <div class="form-group">
                        <div class="login-button">
                            <div class="row">
                                <asp:Button runat="server" OnClick="Reset_Click" Text="Reset" CssClass="form-control btn btn-login" />
                            </div>
                        </div>
                    </div>
                      <div class="form-group">
                                <div class="row">
                                    <asp:HyperLink ID="HL_ForgotPassword" runat="server" TabIndex="5" NavigateUrl="~/Account/Login.aspx">Back To Login</asp:HyperLink>
                                </div>
                            </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
