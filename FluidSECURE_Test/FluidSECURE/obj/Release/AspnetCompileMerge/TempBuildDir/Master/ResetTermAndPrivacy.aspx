<%@ Page Title="Reset Terms and Privacy Policys" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ResetTermAndPrivacy.aspx.vb" Inherits="Fuel_Secure.ResetTermAndPrivacy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server" Text="Reset Terms and Privacy Policys"></asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-2 col-sm-2 textright col-xs-12">
                        </div>
                        <div class="form-group col-md-4 col-sm-4 textright col-xs-12">
                            <label style="font-weight:900">
                                Click to Reset Terms and Privacy Policys: </label>
                        </div>
                       <div class="form-group col-md-3 col-sm-3 col-xs-12">
                           <asp:Button runat="server" ID="btnReset" CssClass="btn btn-primary" Style="background-color: #56B148; color: white;" OnClick="btnReset_Click" Text="RESET" ToolTip="RESET"/>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

      <script src="../Scripts/jquery.maskedinput.js"></script>

    <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">
    <script src="/Scripts/jquery.quicksearch.js"></script>

    <script>
        function CheckConfirm() {
            $('#modalMessage').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

        function ClosePopUp() {
            $("#btnClose").click();
            $('body').removeClass("modal-open");
            $('.modal-backdrop').remove();
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
                                <asp:Label ID="lblErrorMessage" runat="server" Text="Are you sure you want to RESET Terms and Privacy Policys ? This will reset all user's Terms and Privacy Policys." style="line-height:40px"></asp:Label>
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                             <asp:Button runat="server" ID="btnConfirmReject" CssClass="btn btn-primary" Style="background-color: #56B148;color: white;" OnClick="btnConfirmReject_Click" Text="CONTINUE" />
                            <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-primary" Style="background-color: cornflowerblue; color: white;" OnClientClick="ClosePopUp();" Text="Cancel" />
                            <input type="button" id="btnClose" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
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