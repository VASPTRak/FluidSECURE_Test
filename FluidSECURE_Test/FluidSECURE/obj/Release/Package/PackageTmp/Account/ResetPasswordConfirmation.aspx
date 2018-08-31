<%@ Page Title="Forgot Password" Language="vb" MasterPageFile="~/Account/login.Master" AutoEventWireup="true" CodeBehind="ResetPasswordConfirmation.aspx.vb" Inherits="Fuel_Secure.ResetPasswordConfirmation" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="login-container" style="background-image: url(../content/images/header-background-2.png)">
        <div class="panel-body" style="background-color: #ffffff;">
            <div class="login-logo">
                <a href="https://www.fluidsecure.com" target="_blank">
                    <img src="../Content/images/FluidSECURELogo.png" style="max-width: 100%" />
                </a>
            </div>
            <div>
                <p>Your password has been changed. Click
                    <asp:HyperLink ID="login" runat="server" NavigateUrl="~/Account/Login">here</asp:HyperLink>
                    to login </p>
            </div>
        </div>
    </div>
</asp:Content>
