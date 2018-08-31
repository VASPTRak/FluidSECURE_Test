<%@ Page Title="IMEI Person Mapping" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="IMEIPersonMapping.aspx.vb" Inherits="Fuel_Secure.IMEIPersonMapping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <script>
                function DeleteRecord() {
                    $('#btnMyModalClose').click();

                    var IMEIPersonMappingId = $("#hdnIMEIPersonMappingId").val();

                    $.ajax({
                        type: "POST",
                        url: "IMEIPersonMapping.aspx/DeleteRecord",
                        data: JSON.stringify({ IMEIPersonMappingId: IMEIPersonMappingId }),
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

                    if (response.d == 1) {
                        $("#messageNew").text("Mapping deleted Successfully.");
                        $("#ErrorMessageNew").hide();
                        $("#messageNew").show();
                        var personId = $('#<%= HDF_PersonnelId.ClientID %>').val();
                        var uniqueId = $('#<%= HDF_UniqueUserId.ClientID%>').val();
                        window.location.href = "/Master/IMEIPersonMapping.aspx?RecordIs=Delete&PersonId=" + personId + "&UniqueUserId=" + uniqueId;

                    }

                    else {
                        $("#ErrorMessageNew").show();
                        $("#messageNew").hide();

                        $("#ErrorMessageNew").text("Mapping deleted Successfully.");
            }
        }

                function CheckConfirm(IMEIPersonMappingId) {

            $("#lblMessage").text("Are you sure you want to delete this Mapping ?");
            $("#hdnIMEIPersonMappingId").val(IMEIPersonMappingId)

            $('#myModalSuccess').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

            </script>

            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server">IMEI - Person Mapping</asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                IMEI Number:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:TextBox ID="txtIMEINumber" runat="server" CssClass="form-control input-sm" TabIndex="7"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="REV_IMEINumber" runat="server" ControlToValidate="txtIMEINumber" ErrorMessage="comma not allowed. Please remove comma and try again" ValidationExpression="^[^,]+$"
                                Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RegularExpressionValidator>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                            <label>
                                Is Active:</label>
                        </div>
                        <div class="form-group col-md-3 col-sm-3 col-xs-12">
                            <asp:CheckBox ID="CHK_IsActive" runat="server" />
                        </div>
                    </div>
                    <asp:HiddenField ID="HDF_PersonnelId" runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="HDF_UniqueUserId" runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="HDF_IMEIPersonMappingId" runat="server" Value=""></asp:HiddenField>

                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="true" TabIndex="24" ValidationGroup="PersonelValidation" OnClientClick="return IsValidPhoneNumber();" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="25" OnClick="btnCancel_Click" />

                    </div>


                    <div class="row col-md-12 col-sm-12 text-center clear col-xs-12" style="margin: 10px 0">

                        <asp:GridView ID="gvIMEIPersonnel" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true" OnRowDataBound="gvIMEIPersonnel_RowDataBound"
                            DataKeyNames="IMEIPersonMappingId" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found for selected search criteria">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lbl1Edit" runat="server">Edit</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" OnClick="linkEdit_Click" Text="Edit"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <HeaderTemplate>
                                        <asp:Label ID="lbl1Delete" runat="server">Delete</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConfirm(<%# DataBinder.Eval(Container.DataItem, "IMEIPersonMappingId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="IMEI_UDID" ItemStyle-HorizontalAlign="Left" HeaderText="IMEI Number">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHIMEI_UDID" runat="server">IMEI Number</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIMEI_UDID" Text='<%# DataBinder.Eval(Container.DataItem, "IMEI_UDID")%>'
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="IsActive" ItemStyle-HorizontalAlign="Left" HeaderText="Is Active">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHIsActive" runat="server">Is Active</asp:Label>
                                    </HeaderTemplate>
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign ="Middle" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CHK_InActive" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsActive")%>' 
                                            AutoPostBack="true" OnCheckedChanged="CHK_InActive_CheckedChanged" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" />
                            <EmptyDataRowStyle Font-Bold="True" ForeColor="Red" BackColor="white" BorderColor="red"
                                BorderStyle="Solid" BorderWidth="1px" />
                        </asp:GridView>

                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <!--alert message popup-->
    <div class="modal fade" tabindex="-1" role="dialog" id="myModalSuccess">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title text-center">FluidSecure</h3>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hdnIMEIPersonMappingId" />
                    <h4 id="lblMessage"></h4>
                </div>
                <div class="modal-footer nextButton">
                    <button type="button" id="btnMyModalClose" class="btn btn-default" data-dismiss="modal" onclick="return false;">No </button>
                    <input type="button" id="btnMyModalSuccess" class="btn btn-success" onclick="DeleteRecord()" value="Yes" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

</asp:Content>
