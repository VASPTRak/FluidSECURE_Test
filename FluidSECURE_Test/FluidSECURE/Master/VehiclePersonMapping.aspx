<%@ Page Title="Vehicle Person Mapping" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="VehiclePersonMapping.aspx.vb" Inherits="Fuel_Secure.VehiclePersonMapping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .options label {
            padding-left: 5px;
            padding-right: 10px;
            vertical-align: middle;
        }
    </style>

    <asp:UpdatePanel ID="up_Main" runat="server">
        <ContentTemplate>
            <div class="panel panel-primary" style="margin: 20px;">
                <div class="panel-heading  text-center">
                    <asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
                </div>
                <div class="panel-body">
                    <div class="row col-md-12 col-sm-12 col-xs-12">
                        <p class="text-center green" id="message" runat="server"></p>
                        <p class="text-center red" id="ErrorMessage" runat="server"></p>
                    </div>
                    <div class="row col-md-12 col-sm-12  col-xs-12">
                        <div class="form-group col-md-12 col-sm-12  col-xs-12 text-center">
                            <asp:RadioButtonList ID="RBL_Options" runat="server" RepeatDirection="Horizontal" CssClass="options" OnSelectedIndexChanged="RBL_Options_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="Allow this person to fuel all vehicles" Value="1" />
                                <asp:ListItem Text="De-select all vehicles assigned to this person in the database" Value="4" />
                                <asp:ListItem Text="Allow this person to fuel all vehicles in a particular department" Value="2" />
                                <asp:ListItem Text="Allow this person to fuel only certain vehicles" Value="3" Selected="True" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div id="showDepartments" runat="server">
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div class="form-group col-md-3 col-sm-3 col-xs-12"></div>
                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Department
                        <label class="text-danger font-required">[required]</label>:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:DropDownList ID="DDL_Dept" runat="server" TabIndex="4" CssClass="form-control input-sm"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RFD_Dept" runat="server" ControlToValidate="DDL_Dept" Display="Dynamic"
                                    ErrorMessage="Please select department." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="OptionValidation"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>
                    <div id="showGrid" runat="server">

                        <div class="row col-md-12 col-sm-12  col-xs-12">
                            <div class="form-group col-md-2 col-sm-3 hidden-xs"></div>
                            <div class="form-group col-md-3 col-sm-3  col-xs-12">
                                <asp:DropDownList runat="server" CssClass="form-control input-sm" TabIndex="0" ID="DDL_ColumnName">
                                    <asp:ListItem Text="Vehicle Name" Value="VehicleName"></asp:ListItem>
                                    <asp:ListItem Text="Vehicle Number" Value="VehicleNumber"></asp:ListItem>
                                    <asp:ListItem Text="Vehicle's Department Name" Value="Name"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group col-md-3 col-sm-3  col-xs-12">
                                <asp:TextBox ID="txt_value" runat="server" CssClass="form-control input-sm" TabIndex="1"></asp:TextBox>
                            </div>
                            <div class="form-group col-md-3 col-sm-3  col-xs-12">
                                <asp:Button ID="btnSearch" CssClass="btn btn-primary" runat="server" Text="Search" OnClick="btnSearch_Click" TabIndex="11" />
                            </div>
                            <div class="form-group col-md-1 col-sm-3 hidden-xs"></div>
                        </div>

                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <asp:GridView ID="gv_Vehicles" CssClass="table table-bordered" runat="server" DataKeyNames="VehicleId,VehicleNumber,VehicleName" AutoGenerateColumns="False" EmptyDataText="Data Not found." AllowPaging="true"
                                OnPageIndexChanging="OnPaging" PageSize="20">
                                <Columns>
                                    <asp:TemplateField  HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Wrap="true">
                                        <HeaderTemplate>
                                            <asp:Label runat="server" Text ="Select/De-select all vehicles"></asp:Label><br />
                                            <asp:Label runat="server" Text ="shown on this page"></asp:Label><br />
                                            <div>
                                                <div class="col-md-5 col-lg-5 col-sm-12"></div> 
                                            </div>
                                            &nbsp&nbsp &nbsp&nbsp<asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this);"/>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="CheckBox1" runat="server" onclick="Check_Click(this)" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="VehicleName" HeaderText="Vehicle Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="VehicleNumber" HeaderText="Vehicle Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Name" HeaderText="Vehicle's Department" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                    <%--<asp:BoundField DataField="VehicleId" HeaderText="VehicleId" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />--%>
                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                    <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                        <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
                            UseSubmitBehavior="true" TabIndex="24" ValidationGroup="OptionValidation" />
                        <asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
                            UseSubmitBehavior="False" TabIndex="25" OnClick="btnCancel_Click" />
                    </div>

                </div>
            </div>

            <script type="text/javascript">

                function preventBack() { window.history.forward(); }

                setTimeout("preventBack()", 0);

                window.onunload = function () { null };

                function IsDeleteAllVehicles() {
                    var selection = $("#<%=RBL_Options.ClientID%>").find(":checked").val();
                    if (selection == "4") {
                        return confirm("");
                    }
                }
            </script>


            <script type="text/javascript">

                //function Check_Click(objRef) {
                //	//Get the Row based on checkbox
                //	var row = objRef.parentNode.parentNode;
                //	if (objRef.checked) {
                //		//If checked change color to Aqua
                //		//row.style.backgroundColor = "aqua";
                //	}
                //	else {
                //		//If not checked change back to original color
                //		if (row.rowIndex % 2 == 0) {
                //			//Alternating Row Color
                //			//row.style.backgroundColor = "#C2D69B";
                //		}
                //		else {
                //			//row.style.backgroundColor = "white";
                //		}
                //	}

                //	//Get the reference of GridView
                //	var GridView = row.parentNode;

                //	//Get all input elements in Gridview
                //	var inputList = GridView.getElementsByTagName("input");

                //	for (var i = 0; i < inputList.length; i++) {
                //		//The First element is the Header Checkbox
                //		//var headerCheckBox = inputList[0];

                //		//Based on all or none checkboxes
                //		//are checked check/uncheck Header Checkbox
                //		var checked = true;
                //		if (inputList[i].type == "checkbox") {
                //			if (!inputList[i].checked) {
                //				checked = false;
                //				break;
                //			}
                //		}
                //	}
                //	//headerCheckBox.checked = checked;

                //}


                function Check_Click(objRef) {
                    var IsChecked = objRef.checked;
                    if (IsChecked == false) {
                        var GridView = objRef.parentNode.parentNode.parentNode;
                        var inputList = GridView.getElementsByTagName("input");
                        inputList[0].checked = false;
                    }
                    else {
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
                        if (flag == 0) {
                            inputList[0].checked = true;
                        }
                    }
                }

                function checkAll(objRef) {
                    var GridView = objRef.parentNode.parentNode.parentNode;
                    var inputList = GridView.getElementsByTagName("input");
                    for (var i = 0; i < inputList.length; i++) {
                        //Get the Cell To find out ColumnIndex
                        var row = inputList[i].parentNode.parentNode;
                        if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                            if (objRef.checked) {
                                //If the header checkbox is checked
                                //check all checkboxes
                                //and highlight all rows
                                //row.style.backgroundColor = "aqua";
                                inputList[i].checked = true;
                            }
                            else {
                                //If the header checkbox is checked
                                //uncheck all checkboxes
                                //and change rowcolor back to original 
                                if (row.rowIndex % 2 == 0) {
                                    //Alternating Row Color
                                    //row.style.backgroundColor = "#C2D69B";
                                }
                                else {
                                    //row.style.backgroundColor = "white";
                                }
                                inputList[i].checked = false;
                            }
                        }
                    }
                }



                function DeSelectAllVehicle() {
                    $('#btnMyModalClose').click();

                    $.ajax({
                        type: "POST",
                        url: "VehiclePersonMapping.aspx/DeSelectAllVehicle",
                        data: '',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: OnSuccess,
                        failure: function (response) {
                            alert(response.d);
                        }
                    });
                }

                function OnSuccess(response) {
                    debugger
                    var str = response.d.split(":")
                    window.location.href = "/Master/Personnel?PersonId=" + str[0].trim() + "&UniqueUserId=" + str[1].trim() + ""
                }

                function CheckConfirm() {
                    $("#lblMessage").text("Are you sure you want to De-select all vehicles assigned to this person in the database ?");
                    $('#myModalDelete').modal({
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    });
                }
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!--alert message popup-->
    <div class="modal fade" tabindex="-1" role="dialog" id="myModalDelete">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title text-center">De-Select Vehicles</h3>
                </div>
                <div class="modal-body">
                    <h4 id="lblMessage"></h4>
                </div>
                <div class="modal-footer nextButton">
                    <button type="button" id="btnMyModalClose" class="btn btn-default" data-dismiss="modal" onclick="return false;">No</button>
                    <input type="button" id="btnMyModalSuccess" class="btn btn-success" onclick="DeSelectAllVehicle()" value="Yes" />
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

</asp:Content>
