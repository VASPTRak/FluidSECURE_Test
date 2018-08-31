<%@ Page Title="Forgot Password" Language="vb" MasterPageFile="~/Account/login.Master" AutoEventWireup="true" CodeBehind="Forgot.aspx.vb" Inherits="Fuel_Secure.ForgotPassword" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="login-container" style="background-image: url(../content/images/header-background-2.png)">
        <div class="panel-body" style="background-color: #ffffff;">
            <div class="login-logo">
                <a href="https://www.fluidsecure.com" target="_blank">
                    <img src="../Content/images/FluidSECURELogo.png" style="max-width: 100%" />
                </a>
            </div>
            <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="True">
                <p class="text-danger">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
            </asp:PlaceHolder>
            <div class="row">
                <div class="col-lg-12">

                    <div id="loginForm" runat="server">
                        <div class="form-group">
                                <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" placeholder="Email" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" ValidationGroup="ConfirmEmail"
                                    CssClass="text-danger" ErrorMessage="The email field is required." />
                        </div>
                        <div class="form-group">
                            <div class="login-button">
                                <div class="row">
                                    <asp:Button runat="server" OnClick="Forgot" Text="Submit" CssClass="form-control btn btn-login" ValidationGroup="ConfirmEmail" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <asp:HyperLink ID="HL_ForgotPassword" runat="server" TabIndex="5" NavigateUrl="~/Account/Login.aspx">Back To Login</asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:PlaceHolder runat="server" ID="DisplayEmail" Visible="false">
                        <p class="text-info">
                            Please check your email to reset your password.
                    <br />
                            <br />
							 <div class="login-button">
                                <div class="row">
                                    <asp:Button runat="server" CssClass="form-control btn btn-login"  ID="btnLoginBackReset" PostBackUrl="~/Account/Login.aspx" Text="Back To Login"  />
                                </div>
                            </div>
                            
                        </p>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
