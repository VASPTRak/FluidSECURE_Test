<%@ Page Title="Department Import" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="DeptImport.aspx.vb" Inherits="Fuel_Secure.DeptImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <div class="panel panel-primary" style="margin: 20px;">
             <div id="mydiv">
            <img src="/Content/images/ajax-loader.gif" class="ajax-loader" />
        </div>
        <div class="panel-heading  text-center">
            <asp:Label class="panel-title" ID="lblHeader" runat="server">Import Departments</asp:Label>
        </div>
        <div class="panel-body">
            <div class="row col-md-12 col-sm-12 col-xs-12">
                <p class="text-center green" id="message" runat="server"></p>
                <p class="text-center red" id="ErrorMessage" runat="server"></p>
                <asp:HiddenField ID="HDF_CurrentDate" runat="server" />
            </div>

            <div class="row col-md-12 col-sm-12 col-xs-12 text-center" style="margin: 0px 0 10px;">
                <asp:LinkButton ID="LB_Error" runat="server" OnClick="LB_Error_Click">Click here to download error log file.</asp:LinkButton>
            </div>

            <div class="row col-md-12 col-sm-12 col-xs-12">
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                </div>
                <div ID = "divCompany" runat ="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                    <label>
                        Company
                        <label class="text-danger font-required">[required]</label>:</label>
                </div>
                <div class="form-group col-md-3 col-sm-3 col-xs-12">
                    <asp:DropDownList ID="ddlCustomer" runat="server" TabIndex="1" CssClass="form-control input-sm"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="ddlCustomer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="ImportDeptValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                </div>
            </div>

            <div class="row col-md-12 col-sm-12 col-xs-12">
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                </div>
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                    <label>
                        Upload file:
                            <label class="text-danger font-required">[required]</label>:</label>
                </div>
                <div class="form-group col-md-3 col-sm-3 col-xs-12">
                    <asp:FileUpload ID="FU_Dept" runat="server" TabIndex="2" />
                    <asp:RequiredFieldValidator if="RDF_Firware" runat="server" Display="Dynamic" ErrorMessage="Please select file to upload." ControlToValidate="FU_Dept" ForeColor="Red" SetFocusOnError="True" ValidationGroup="ImportDeptValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group col-md-3 col-sm-3 textright col-xs-12 hidden-xs">
                </div>
            </div>

            <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                <asp:Button ID="btnUpload" CssClass="btn btn-primary" runat="server" OnClick="btnUpload_Click" Text="Upload" Width="100px"
                    UseSubmitBehavior="true" TabIndex="9" ValidationGroup="ImportDeptValidation" OnClientClick="setvalue()" />
            </div>

            <div class="row col-md-12 col-sm-12 text-center col-xs-12" style="margin-top:20px;">
                <asp:LinkButton ID="lnkTemplate" runat="server" OnClick="lnkTemplate_Click">Click here to download template file.</asp:LinkButton>
            </div>
        </div>
    </div>


    <script type="text/javascript">
        function setvalue() {

            var HDF_CurrentDate = $("#<%=HDF_CurrentDate.ClientID%>");

            var localTime = new Date();
            var year = localTime.getFullYear();
            var month = localTime.getMonth() + 1;
            var date = localTime.getDate();
            var hours = localTime.getHours();
            var minutes = localTime.getMinutes();
            var seconds = localTime.getSeconds();

            HDF_CurrentDate.val(month + "/" + date + "/" + year + " " + hours + ":" + minutes + ":" + seconds);

           if (($('#<%=FU_Dept.ClientID%>').val() != "") && ($('#<%=ddlCustomer.ClientID%>').val() != "0")) {
                $("#mydiv").show();
            }
        }
    </script>



</asp:Content>
