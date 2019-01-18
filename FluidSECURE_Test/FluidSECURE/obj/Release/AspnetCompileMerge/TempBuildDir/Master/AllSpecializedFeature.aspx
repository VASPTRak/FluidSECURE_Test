<%@ Page Title="Specialized Feature" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AllSpecializedFeature.aspx.vb" Inherits="Fuel_Secure.AllSpecializedFeature" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script>

        function DeleteRecord() {
            $('#btnMyModalCloseConCustomerMenuLinkDelete').click();

            var CustomerMenuLinkId = $("#hdnCustomerMenuLinkIdConCustomerMenuLinkDelete").val();

            $.ajax({
                type: "POST",
                url: "AllSpecializedFeature.aspx/DeleteRecord",
                data: '{CustomerMenuLinkId: "' + CustomerMenuLinkId + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessdelete,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }

        function OnSuccessdelete(response) {

            $("#myModalConCustomerMenuLinkDelete").hide();

            if (response.d == 1) {
                $("#messageNew").text("Records deleted Successfully.");
                $("#ErrorMessageNew").hide();
                $("#messageNew").show();

                window.location.href = "/Master/AllSpecializedFeature"

            }
            else {
                $("#ErrorMessageNew").show();
                $("#messageNew").hide();

                $("#ErrorMessageNew").text("Records deletion failed.");
            }
        }

        function CheckConCustomerMenuLink(CustomerMenuLinkId) {
            
            $("#lblMessageConCustomerMenuLinkDelete").text("Are you sure you want to delete this Specialized Feature?");
        

            $("#hdnCustomerMenuLinkIdConCustomerMenuLinkDelete").val(CustomerMenuLinkId);

            $('#myModalConCustomerMenuLinkDelete').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        }

    </script>
    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading">
                    <h3 class="panel-title text-center">Specialized Features</h3>
                </div>
                <div class="panel-body">
                    
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                        <p class="text-center green" id="messageNew"></p>
                        <p class="text-center red" id="ErrorMessageNew"></p>
                    </div>
                     <div class="row col-md-12 col-sm-12 col-xs-12" style="margin-bottom: 10px;margin-left: 5px">
                        <div class="row col-md-6 col-sm-6 col-xs-12 text-left">
                            <b><asp:Label runat="server" ID="lblTotalNumberOfRecords"></asp:Label></b>
                        </div>
                    </div>
                    <div class="col-md-12 col-sm-12  col-xs-12" style="overflow-x: auto">

                        <asp:GridView ID="gvCustomerMenuLink" CssClass="table table-bordered table-hover" runat="server" PageSize="10" AllowPaging="true"
                            DataKeyNames="CustomerMenuLinkId,Name" AutoGenerateColumns="False" AllowSorting="true" EmptyDataText="0 records found">
                             <Columns>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkEdit" runat="server" ForeColor="#428BCA" Text="Edit" OnClick="linkEdit_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <%--<asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <a style="color: #428BCA" href="javascript:CheckConCustomerMenuLink(<%# DataBinder.Eval(Container.DataItem, "CustomerMenuLinkId")%>)">Delete</a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>--%>
                                <asp:TemplateField SortExpression="Name" ItemStyle-HorizontalAlign="Left" HeaderText="Specialized Feature">
                                    <ItemTemplate>
                                        <asp:Label ID="lblName" Text='<%# DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" />
                                    </ItemTemplate>
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
    <div class="modal fade" tabindex="-1" role="dialog" id="myModalConCustomerMenuLinkDelete">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title text-center">FluidSecure</h3>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="hdnCustomerMenuLinkIdConCustomerMenuLinkDelete" />
                    <h4 id="lblMessageConCustomerMenuLinkDelete"></h4>
                </div>
                <div class="modal-footer nextButton">
                    <button type="button" id="btnMyModalCloseConCustomerMenuLinkDelete" class="btn btn-default" data-dismiss="modal" onclick="return false;">No</button>
                    <input type="button" id="btnMyModalSuccessConCustomerMenuLinkDelete" class="btn btn-success" onclick="DeleteRecord()" value="Yes" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</asp:Content>
