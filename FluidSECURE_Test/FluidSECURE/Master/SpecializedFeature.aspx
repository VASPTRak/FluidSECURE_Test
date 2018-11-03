<%@ Page Title="Specialized Feature" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="SpecializedFeature.aspx.vb" Inherits="Fuel_Secure.SpecializedFeature" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .btn-group, .multiselect.dropdown-toggle.btn.btn-default, ul.multiselect-container.dropdown-menu {
            width: 100% !important;
            text-align: left;
        }
    </style>
    <div class="panel panel-primary" style="margin: 20px;">
        <asp:HiddenField ID="hdfCustomerMenuLinkId" runat="server" Value="" />
        <div class="panel-heading  text-center">
            <asp:Label class="panel-title" ID="lblHeader" runat="server">Specialized Feature</asp:Label>
        </div>
        <div class="panel-body">
            <div class="row col-md-12 col-sm-12 col-xs-12">
                <p class="text-center green" id="message" runat="server"></p>
                <p class="text-center red" id="ErrorMessage" runat="server"></p>
            </div>

            <div class="row col-md-12 col-sm-12 col-xs-12">
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                    <label>
                        Specialized Feature
                        <%--<label runat="server" id="lblRequired" class="text-danger font-required">[required]</label>--%>
                        :</label>
                </div>
                <div class="form-group col-md-9 col-sm-9 col-xs-12">
                    <asp:Label class="panel-title" ID="lblSpecializedFeature" runat="server"></asp:Label>
                    <%--<asp:TextBox ID="txtSpecializedFeature" CssClass="form-control input-sm" TabIndex="1" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFDSpecializedFeature" runat="server" ControlToValidate="txtSpecializedFeature"
                        ErrorMessage="Please Enter Specialized Feature." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SpecializedFeature"></asp:RequiredFieldValidator>--%>
                </div>
            </div>

            <div class="row col-md-12 col-sm-12 col-xs-12">
                <div class="panel panel-default" style="width: 100%; margin: 0 auto 0;">
                    <div class="panel-heading" style="text-align: center">
                        <h3 class="panel-title">Configure Specialized Feature</h3>
                    </div>
                    <div class="row col-md-12 col-sm-12 col-xs-12" style="margin-top: 10px !important">
                        <%--  <div class="col-md-4 col-sm-3 col-xs-12">
                        </div>--%>
                        <div class="col-md-12 col-sm-6 col-xs-12 text-center table table-responsive">
                            <table class="form-horizontal" role="form" style="margin: 0px auto">
                                <tr class="form-group">
                                    <td>
                                        <asp:UpdatePanel ID="up_Main" runat="server">
                                            <ContentTemplate>
                                                <asp:GridView ID="gvSpecializedFeature" CssClass="table table-bordered table-hover" runat="server" AutoGenerateColumns="False" EmptyDataText="0 records found"
                                                    DataKeyNames="CustomerMenuLinkId,Name" OnRowDataBound="OnRowDataBound">
                                                    <Columns>
                                                        <asp:BoundField ItemStyle-Width="300px" DataField="Name" HeaderText="Specialized Feature" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <img alt="" style="cursor: pointer" src="/content/images/plus.png" />
                                                                <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                                                    <asp:GridView ID="gvCustomers" CssClass="table table-bordered table-hover" runat="server" AutoGenerateColumns="False" EmptyDataText="0 records found"
                                                                        DataKeyNames="CustomerId,CustomerName">
                                                                        <Columns>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="ChkLinks" runat="server" onclick="javascript:SelectboxSite(this);"></asp:CheckBox>
                                                                                </ItemTemplate>
                                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Customer Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblVersion" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName")%>'
                                                                                        runat="server" />
                                                                                </ItemTemplate>
                                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" />
                                                                        <EmptyDataRowStyle Font-Bold="True" ForeColor="Red" BackColor="white" BorderColor="red"
                                                                            BorderStyle="Solid" BorderWidth="1px" />
                                                                    </asp:GridView>
                                                                </asp:Panel>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle BackColor="#A5BBC5" Font-Bold="True" ForeColor="black" />
                                                    <EmptyDataRowStyle Font-Bold="True" ForeColor="Red" BackColor="white" BorderColor="red"
                                                        BorderStyle="Solid" BorderWidth="1px" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                    UseSubmitBehavior="true" TabIndex="9" ValidationGroup="SpecializedFeature" />
                <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False" OnClick="btnCancel_Click"
                    UseSubmitBehavior="False" TabIndex="10" />
            </div>

        </div>
    </div>

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $("[src*=plus]").live("click", function () {
            //$(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            <%-- var Parent = document.getElementById('<%= gvCustomers.ClientID %>');
            var panel = Parent.getElementsByTagName('div[id*=pnlOrders]";');
            var abc = "#<%=gvCustomers.ClientID%> div[id*='pnlOrders']";--%>
            $(this).closest("tr").find("div[id*='pnlOrders']").show();
            $(this).attr("src", "/content/images/minus.png");
        });
        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "/content/images/plus.png");
            //$(this).closest("tr").next().hide();
            $(this).closest("tr").find("div[id*='pnlOrders']").hide();
        });

        
        function checkAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                //Get the Cell To find out ColumnIndex
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        inputList[i].checked = true;
                    }
                    else {
                        //If the header checkbox is checked
                        //uncheck all checkboxes
                        inputList[i].checked = false;
                    }
                }
            }
        }

            function SelectboxSite(objRef) {
                var IsChecked = objRef.checked;
                if (IsChecked == false) {
                    var GridView = objRef.parentNode.parentNode.parentNode;
                    var inputList = GridView.getElementsByTagName("input");
                    inputList[0].checked = false;
                }
                else 
                {
                    var GridView = objRef.parentNode.parentNode.parentNode;
                    var inputList = GridView.getElementsByTagName("input");
                    var flag = 0
                    for (var i = 1; i < inputList.length; i++) {
                        //Get the Cell To find out ColumnIndex
                        if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                            if (inputList[i].checked) {
                                flag = 0
                            }
                            else {
                                flag = 1
                                break;
                            }
                        }
                    }
                    if(flag == 0)
                    {
                        inputList[0].checked = true;
                    }
                }
            }

    </script>
</asp:Content>
