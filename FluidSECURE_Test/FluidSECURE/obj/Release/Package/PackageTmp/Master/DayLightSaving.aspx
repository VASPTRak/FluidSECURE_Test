<%@ Page Title="Day Light Saving" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="DayLightSaving.aspx.vb" Inherits="Fuel_Secure.DayLightSaving" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
            <script>
                function CheckConfirm() {

                    var DayLightSaving = $("input[type='checkbox']").is(':checked');
                    if (DayLightSaving == true) {
                        $("#lblMessage").text("Are you sure you want to change the day light saving? It will set daylight saving to standard time zone for all the Fluid Secure links in USA");;
                    }
                    else {
                        $("#lblMessage").text("Are you sure you want to change the day light saving? It will set standard to daylight saving time zone for all the Fluid Secure links in USA.")
                    }

                    $('#myModalSuccess').modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    });
                }

                function SaveDetails() {
                    $('#btnMyModalClose').click();

                    var DayLightSavingVal = $("input[type='checkbox']").is(':checked');
                    var DayLightSaving = "";

                    if (DayLightSavingVal == true) {
                        DayLightSaving = "Y";
                    }
                    else {
                        DayLightSaving = "N";
                    }

                    $.ajax({
                        type: "POST",
                        url: "DayLightSaving.aspx/SaveDetails",
                        data: JSON.stringify({ DayLightSaving: DayLightSaving }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: OnSuccess,
                        failure: function (response) {
                            alert(response.d);

                        }
                    });
                }

                function OnSuccess(response) {

                    $("#myModalSuccess").hide();

                    if (response.d == "1") {
                        $("#messageNew").text("Record Saved.");
                        $("#ErrorMessageNew").hide();
                        $("#messageNew").show();
                    }
                    else {
                        $("#ErrorMessageNew").show();
                        $("#messageNew").hide();

                        $("#ErrorMessageNew").text("Some error occured while saving record.");
                    }
                }
            </script>
            <!--alert message popup-->
            <div class="modal fade" tabindex="-1" role="dialog" id="myModalSuccess">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3 class="modal-title text-center">FluidSecure</h3>
                        </div>
                        <div class="modal-body">
                            <h4 id="lblMessage"></h4>
                        </div>
                        <div class="modal-footer nextButton">
                            <button type="button" id="btnMyModalClose" class="btn btn-default" data-dismiss="modal" onclick="return false;">Close</button>
                            <input type="button" id="btnMyModalSuccess" class="btn btn-success" onclick="SaveDetails()" value="Ok" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">Day Light Saving</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                        <p class="text-center green" id="messageNew"></p>
                        <p class="text-center red" id="ErrorMessageNew"></p>
                    </div>

                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>Day Light Saving:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="CHK_DayLightSaving" runat="server" />
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                        </div>
                    </div>

                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClientClick="CheckConfirm()" Text="Save" Width="100px" />
                    </div>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
