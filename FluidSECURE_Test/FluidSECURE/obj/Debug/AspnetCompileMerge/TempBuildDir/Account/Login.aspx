﻿<%@ Page Title="Log in" Language="vb" AutoEventWireup="false" MasterPageFile="~/Account/login.Master" CodeBehind="Login.aspx.vb" Inherits="Fuel_Secure.Login" Async="true" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
	<asp:UpdatePanel ID="UP_Main" runat="server">
		<ContentTemplate>

			<div class="login-container" style="background-image: url(../content/images/header-background-2.png)">

				<div class="panel-body" style="background-color: #ffffff;">

					<div class="login-logo">
						<a href="https://www.fluidsecure.com" target="_blank">
							<img src="../Content/images/FluidSECURELogo.png" style="max-width: 100%" />
						</a>
					</div>

					<asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
						<p class="text-danger red text-center">
							<asp:Literal runat="server" ID="FailureText" />
						</p>
					</asp:PlaceHolder>
					<div class="row">
						<div class="col-lg-12">
							<div class="form-group">
								<asp:TextBox runat="server" ID="Email" TabIndex="1" CssClass="form-control" TextMode="Email" Style="max-width: 100%" placeholder="Email" />
								<asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
									CssClass="text-danger" ErrorMessage="The email field is required." ValidationGroup="LoginValidation" />
							</div>
							<div class="form-group">
								<asp:TextBox runat="server" ID="Password" TabIndex="2" TextMode="Password" CssClass="form-control" Style="max-width: 100%" placeholder="Password" ValidationGroup="LoginValidation" />
								<asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." />
							</div>
							<div class="form-group">
								<asp:DropDownList ID="DrpLanguages" runat="server" CssClass="form-control">
									<asp:ListItem Text="English" Value="en-US"></asp:ListItem>
									<asp:ListItem Text="Spanish" Value="es-ES"></asp:ListItem>
								</asp:DropDownList>
							</div>
							<div class="form-group">
								<div class="checkbox">
									<asp:CheckBox runat="server" ID="RememberMe" TabIndex="3" />
									<asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
								</div>
							</div>
							<div class="form-group">
								<div class="row">
									<div class=" login-button">
										<asp:Button runat="server" OnClick="LogIn" TabIndex="4" Text="Log in" ID="btnLogin" CssClass="form-control btn btn-login" ValidationGroup="LoginValidation" />
									</div>
								</div>
							</div>
							<div class="form-group">
								<div class="row">
									<asp:Button runat="server" ID="btnForgotPassword" CssClass="btn-link" Style="padding-left: 0px; border: 0px;" Text="Forgot Password?" OnClick="btnForgotPassword_Click" CausesValidation="false" UseSubmitBehavior="false" />
									<%--<asp:HyperLink ID="HL_ForgotPassword" runat="server"  TabIndex="5"  NavigateUrl="~/Account/Forgot.aspx">Forgot Password?</asp:HyperLink>--%>
								</div>
							</div>
							<div class="form-group">
								<div class="row">
									<%--<asp:Button runat="server" ID="btnRegisterCompany" CssClass="btn-link" Text="Register your Company" OnClick="btnRegisterCompany_Click" CausesValidation="false"/>--%>
									<asp:HyperLink ID="HL_RegisterCompany" runat="server" TabIndex="6" NavigateUrl="~/Master/RegisterCompany.aspx">Register your Company</asp:HyperLink>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>

		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>