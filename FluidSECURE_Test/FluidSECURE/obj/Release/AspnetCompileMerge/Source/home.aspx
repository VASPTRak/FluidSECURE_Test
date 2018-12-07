<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="home.aspx.vb" Inherits="Fuel_Secure._Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        th, td, .DashboardLabels {
            text-align: center;
            font-family: 'Open Sans';
            font-weight: bold;
            font-size: 14pt;
            vertical-align: top !important;
        }
    </style>

    <div class="dashboard-header">
        <h2 class="header-text" style="background-image: url(Content/images/header-background.png)">FluidSecure Cloud Dashboard</h2>
    </div>

    <asp:UpdatePanel runat="server" ID="UP_Main">
        <ContentTemplate>
            <h3 class="select-company" id="CompanyH3" runat="server"><%= Fuel_Secure.My.Resources.Resource.Company %></h3>
            <div>
                
                <div class="form-group company-form" style="padding-left: 0px">
                    <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="18" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged" onchange="setvalue()"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RFD_Customer" runat="server" Font-Size="Small"
                        Font-Bold="False" Font-Names="arial" ErrorMessage="Please select company."
                        ControlToValidate="DDL_Customer" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="PersonelValidation"></asp:RequiredFieldValidator>
                    <h3 class="select-company" runat="server" id="LBL_Company"></h3>
                    <asp:HiddenField ID="HDF_CurrentDate" runat="server" />
                </div>

            </div>
            <div class="row col-md-12 col-sm-12 col-xs-12">
                <p class="text-center red" id="ErrorMessage" runat="server"></p>
            </div>


            <div class="home-column">
                <img src="Content/images/home-icon-1.png" />
                <p class="home-labels">Total Quantity Dispensed Today</p>
                <p class="home-quantities">
                    <asp:Label ID="LBL_DispensedToday" runat="server"></asp:Label>
                </p>
            </div>

            <div class="home-column">
                <img src="Content/images/home-icon-2.png" />
                <p class="home-labels">Number of Vehicles Using System Today</p>
                <p class="home-quantities">
                    <asp:Label ID="LBL_vehiclesFueledToday" runat="server"></asp:Label>
                </p>
            </div>

            <div class="home-column">
                <img src="Content/images/home-icon-3.png" />
                <p class="home-labels">Average Quantity per Vehicle</p>
                <p class="home-quantities">
                    <asp:Label ID="LBL_AverageAmountOffueledPerVehicle" runat="server"></asp:Label>
                </p>
            </div>

            <div class="home-column last">
                <img src="Content/images/home-icon-4.png" />
                <p class="home-labels">Total Quantity Dispensed for Current Month</p>
                <p class="home-quantities">
                    <asp:Label ID="LBL_DispensedCurrentMonth" runat="server"></asp:Label>
                </p>
            </div>
            <div></div>



        </ContentTemplate>
    </asp:UpdatePanel>


    <div id="support-section">
        <h3>Contact and Support</h3>
        <p><a href="https://www.fluidsecure.com" target="_blank">www.FluidSecure.com</a></p>
        <p>Contact:<a href="mailto:support@fluidsecure.com" target="_blank"> support@fluidsecure.com</a></p>
        <p>Support: Monday through Friday from 8:00 AM - 6:00 PM (EST).</p>
        <p>850-878-4585, select support #1,</p>
        <p>After hours and holidays: 850-878-4585, select #7</p>
        <p>For Sales contact:<a href="mailto: info@fluidsecure.com" target="_blank"> info@fluidsecure.com</a> or call 850-878-4585 x325</p>
    </div>

     <script src="/Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="/Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="/Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery.timepicker.min.js"></script>
    <link rel="stylesheet" href="/Content/jquery.timepicker.min.css">`
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

		}

		function OpenTermConditionsMessage() {
		    $('#OpenTermConditions').modal({
		        show: true,
		        backdrop: 'static',
		        keyboard: false
		    });
		}

		function CloseOpenTermConditions() {
		    $("#btnCloseOpenTermConditions").click();
		    $('body').removeClass("modal-open");
		}

		function CheckAccept() {
		    var chkTermPolicy = $('#<%=chk_TermsAndConditions.ClientID%>').is(":checked")
		    if (chkTermPolicy) {
                $('#<%=lblErrorTermsAndPolicy.ClientID%>').html("");
		        return true;
		    }
		    else {
		        $('#<%=lblErrorTermsAndPolicy.ClientID%>').html("*Please accept Terms and Privacy Policy and Continue.");
		        return false;
		    }
		    
		}

    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="modal fade" tabindex="-1" role="dialog" id="OpenTermConditions">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #0762AC">
                            <h5 class="modal-title text-center" style="color: white">Updated Term & Privacy Policy</h5>
                        </div>
                        <div class="modal-body">
                            <div class="row col-md-12 col-sm-12">
                                <asp:CheckBox runat="server" ID="chk_TermsAndConditions" /><asp:Label ID="lblTermsAndPolicy" runat="server" Text="I have read and agree to the updated "></asp:Label><a id="hrefTermsPriPol" href="#">Terms and Privacy Policy</a>
                                 </div>
                            <br />
                            <div class="row col-md-12 col-sm-12">
                                <asp:Label ID="lblErrorTermsAndPolicy" Style="color: red" runat="server" Text="" />
                            </div>
                        </div>
                        <div class="modal-footer nextButton">
                            <asp:Button runat="server" ID="btnAccepted" CssClass="btn btn-primary" Style="background-color: #56B148; color: white;" OnClientClick="return CheckAccept();" OnClick="btnAccepted_Click" Text="Accepted and Continue" />
                            <asp:Button runat="server" ID="btnClose" CssClass="btn btn-primary" Style="background-color: #0762AC; color: white;" OnClick="btnClose_Click" Text="Cancel" />
                            <%--<input type="button" id="btnAccepted" class="btn btn-success" onclick="" value="CONTINUE" />--%>
                            <input type="button" id="btnCloseOpenTermConditions" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->
        </ContentTemplate>
    </asp:UpdatePanel>
 
  <script type="text/javascript">
     
      $(function () {
          var fileName = "Terms and Privacy Policy";
          $("#hrefTermsPriPol").click(function () {
              CloseOpenTermConditions();
              $("#dialog").dialog({
                  modal: true,
                  title: fileName,
                  autoOpen: true,
                  width: 710,
                  height: 500,
                  position: ["bottom", 100],
                  buttons: {
                      'Accepted and Continue': function () {
                          AcceptAndCiontinue();
                      },
                      'Cancel': function () {
                          $(this).dialog('close');
                          OpenTermConditionsMessage()
                      }
                  },
                  open: function (event, ui) {
                      var object = "<object data=\"/TermsAndConditions/FluidSecure Terms and Conditions.pdf\" type=\"application/pdf\" width=\"650px\" height=\"500px\">";
                      //object += "If you are unable to view file, you can download from <a href = \"{FileName}\">here</a>";
                      //object += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
                      object += "</object>";
                      $("#dialog").html(object);
                      $(".ui-dialog-titlebar-close").hide();
                      $(".ui-widget-overlay").attr('style', 'opacity:1; z-index:1000;');
                     
                  },
                  show: {
                      effect: "fade in",
                      duration: 1000
                  },
                  hide: {
                      effect: "fade out",
                      duration: 1000
                  },
                  closeOnEscape: false,
                  draggable: false
              });
          });
      });

function AcceptAndCiontinue()
{
    var UniqueId = '<%= Session("UniqueId") %>';

    $.ajax({
        type: "POST",
        url: "home.aspx/AcceptAndCiontinue",
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
          if(response.d = 1)
          {
              window.location = "home.aspx"
          }
          else
          {
              window.location = "account/login.aspx"
          }
      }

</script>
   
<div id="dialog" style="display: none;">
</div>

    <style>
        .ui-button-text
        {
            background : #56B148 !important;
            color : white;
        }

        #dialog
        {
            overflow:hidden;
        }
        .ui-widget-header
        {
            background : #0762AC !important;
            color : white;
        }
    </style>

</asp:Content>


