<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Account/login.Master" CodeBehind="Error.aspx.vb" Inherits="Fuel_Secure._Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<div class="login-container" style="background-image: url(../content/images/header-background-2.png)">
		<div class="panel-body" style="background-color: #ffffff;">
			<div class="login-logo">
				<a href="https://www.fluidsecure.com" target="_blank">
					<img src="../Content/images/FluidSECURELogo.png" style="max-width: 100%" />
				</a>
			</div>

			<div id="loginForm" runat="server">
				<div class="form-group">
					<h3 class="text-center">Session Expired.<br />
						 Please Login Again.</h3>
					<%--<asp:Label ID="lblInfo" runat="server">If error persists please contact administrator.</asp:Label>--%>
				</div>
				<div class="form-group" style="margin-top: 20px;">
					<div class="row">
						<div class=" login-button">
							<asp:Button runat="server" Text="Log in" ID="btnLogin" CssClass="form-control btn btn-login" OnClick="btnLogin_Click" />
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>

</asp:Content>
