<%@ Page Title="FluidSecure Links Location Report" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="FluidSecureUnitsLocationReport.aspx.vb" Inherits="Fuel_Secure.FluidSecureUnitsLocationReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="datadiv">
        <asp:UpdatePanel ID="UP_Main" runat="server">
            <ContentTemplate>
                <div class="panel panel-primary" style="margin: 20px;">
                    <div class="panel-heading  text-center">
                        <asp:Label class="panel-title" ID="lblHeader" runat="server">FluidSecure Links Locations</asp:Label>
                    </div>
                    <div class="panel-body">
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <p class="text-center red" id="message" runat="server"></p>
                            <p class="text-center red" id="ErrorMessage"></p>
                        </div>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
                            <div ID = "divCompany" runat ="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    Company:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="form-control input-sm" TabIndex="1" AutoPostBack="true" OnSelectedIndexChanged="DDL_Customer_SelectedIndexChanged"></asp:DropDownList>
                            </div>

                            <div class="form-group col-md-3 col-sm-3 textright col-xs-12">
                                <label>
                                    FluidSecure Link:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:DropDownList ID="DDL_Site" runat="server" TabIndex="2" CssClass="form-control input-sm"></asp:DropDownList>

                            </div>
                        </div>
                        <div class="row col-md-12 col-sm-12 text-center col-xs-12">
                            <input type="button" tabindex="3" id="BTN_SelectLocation" onclick="OpenLocationBox();" class="btn btn-primary" value="Show Locations" />

                        </div>
                </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

    </div>

    <div id="LocationBox" style="display: none">
        <div class="LocationPopUp">
            <div style="height: 500px;">
                <div id="map">asd</div>
            </div>
            <div class="col-md-12 text-center" style="margin-top: 30px;">
                <input type="button" value="Close" class="btn btn-default" onclick="closebox();" />
            </div>
        </div>
    </div>
    <script>

        function OpenLocationBox() {
            $("#ErrorMessage").text("");
            initMap();
        }

        function closebox() {
            $("#LocationBox").css("display", "none");
            $("#datadiv").css("display", "block");
        }


        //map functions
        var map;
        var geocoder;
        var address;
        var searchBox;
        var input;
        var storeLat = "";
        var storeLng = "";

        function initMap() {

            var customerId = $('#<%= DDL_Customer.ClientID%>').val();
            var customername = $('#<%= DDL_Customer.ClientID%> option:selected').text();

            var siteId = $('#<%= DDL_Site.ClientID%>').val();
            var site = $('#<%= DDL_Site.ClientID%> option:selected').text();

            if (customerId == "0") {
                closebox();
                $("#ErrorMessage").text("Please select company.")

                return;
            }

            $.ajax({
                type: "POST",
                url: "FluidSecureUnitsLocationReport.aspx/GetLocations",
                data: '{ customerId: "' + customerId + '", siteId: "' + siteId + '", customername: "' + customername + '" , site: "' + site + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    if (data.d == "-1") {
                        closebox();
                        $("#ErrorMessage").text("FluidSecure Link locations not found for selected criteria.")

                        return;
                    }

                    if (data.d == "-2") {
                        closebox();
                        $("#ErrorMessage").text("Error occurred while getting FluidSecure Link locations. Please try again after some time.")

                        return;
                    }

                    $("#LocationBox").css("display", "block");
                    $("#datadiv").css("display", "none");


                    var locations = JSON.parse(data.d);

                    // Display multiple markers on a map
                    var infoWindow = new google.maps.InfoWindow(), marker, i;

                    var markers = [];
                    var bounds = new google.maps.LatLngBounds();
                    map = new google.maps.Map(document.getElementById('map'), {
                        zoom: 2
                    });

                    for (var i = 0; i < locations.length; i++) {

                        var myLatlng = { lat: parseFloat(locations[i].Lat), lng: parseFloat(locations[i].Lng) };
                        var marker = new google.maps.Marker({
                            map: map,
                            position: myLatlng

                        });
                        markers.push(marker);
                        // Allow each marker to have an info window    
                        google.maps.event.addListener(marker, 'click', (function (marker, i) {
                            return function () {
                                infoWindow.setContent(locations[i].Address);
                                infoWindow.open(map, marker);
                                map.setZoom(18);
                            }
                        })(marker, i));
                        
                        bounds.extend(myLatlng);
                    }

                    map.fitBounds(bounds);

                    // Override our map zoom level once our fitBounds function runs (Make sure it only runs once)
                    var boundsListener = google.maps.event.addListener((map), 'bounds_changed', function (event) {
                        this.setZoom(2);
                        google.maps.event.removeListener(boundsListener);
                    });

                },
                failure: function (response) {
                    alert("failed to load locations. Error is : " + response.d);
                }
            });


        }
    </script>
    <script async defer src="https://maps.googleapis.com/maps/api/js?key=AIzaSyANyauXVaXeY_sQLfEHGZIgeB6dn2y3ErU&libraries=places"> </script>
</asp:Content>
