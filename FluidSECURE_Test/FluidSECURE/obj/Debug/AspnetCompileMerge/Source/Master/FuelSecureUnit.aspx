<%@ Page Title="FluidSecure Link" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="FuelSecureUnit.aspx.vb" Inherits="Fuel_Secure.FuelSecureUnit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<script type="text/javascript">
       <%-- function AddMapping() {
            var SiteID = $("#<%= HDF_SiteID.ClientID%>").val();
            var CustomerId = $("#<%= ddlCustomer.ClientID  %>").val();
            $.ajax({
                type: "POST",
                url: "FuelSecureUnit.aspx/AddFSUnitToAllPersons",
                data: '{SiteID: "' + SiteID + '",CustomerId : "' + CustomerId + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var hdf_OpenBox = $("#<%= hdf_OpenBox.ClientID %>");
                    hdf_OpenBox.val('N');

                    $("#btnCloseMapping").click();
                },
                failure: function (response) {
                    alert(response.d);
                }
            });


            }--%>


		function OpenLocationBox() {

			$("#LocationBox").css("display", "block");
			$("#datadiv").css("display", "none");
			initMap();
		}

		function CloseLocationBox() {

			$("#LocationBox").css("display", "none");
			$("#datadiv").css("display", "block");
			$('#<%= txtPumpOnTime.ClientID%>').focus();
			GetAddress();
		}

		function closebox() {
			$("#LocationBox").css("display", "none");
			$("#datadiv").css("display", "block");
			$('#<%= CHK_DisableGeoLocation.ClientID%>').focus();
		}


		function RemoveOptions(selectbox) {
			var i;
			for (i = selectbox.options.length - 1; i >= 0; i--) {
				selectbox.remove(i);
			}
		}

		function GetWithDecimals(num) {
			return num.toString().match(/^-?\d+(?:\.\d{0,5})?/)[0];
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

			var strLat = $('#<%=txtLat.ClientID%>')[0].value;
			var strLng = $('#<%=txtLong.ClientID%>')[0].value;

			$('#<%=txtsearch.ClientID%>')[0].value = "";
			input = $('#<%=txtsearch.ClientID%>')[0];

			var markers = [];

			if ($('#<%=txtSiteAddress.ClientID%>')[0].value == '') {
				address = 'Tallahassee, FL, United States';
			}
			else {
				address = $('#<%=txtSiteAddress.ClientID%>')[0].value;
			}

			if (strLat == '' && strLng == '') {

				strLat = '';
				strLng = '';
				geocoder = new google.maps.Geocoder();
				map = new google.maps.Map(document.getElementById('map'), {
					zoom: 18
				});


				map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

				searchBox = new google.maps.places.SearchBox((input));

				if (geocoder) {
					geocoder.geocode({ 'address': address }, function (results, status) {
						if (status == google.maps.GeocoderStatus.OK) {
							if (status != google.maps.GeocoderStatus.ZERO_RESULTS) {
								map.setCenter(results[0].geometry.location);
								storeLat = GetWithDecimals(results[0].geometry.location.lat());
								storeLng = GetWithDecimals(results[0].geometry.location.lng());
								var myLatlng = { lat: parseFloat(GetWithDecimals(results[0].geometry.location.lat())), lng: parseFloat(GetWithDecimals(results[0].geometry.location.lng())) };


								map = new google.maps.Map(document.getElementById('map'), {
									zoom: 18,
									center: myLatlng
								});

								map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);


								var marker = new google.maps.Marker({
									position: myLatlng,
									//    map: map,
									title: 'Click to zoom'
								});

								marker.setMap(map);

								var clickEvent = new google.maps.event.addListener(map, "click", function (e) {

									for (var i = 0, marker_new; marker_new = markers[i]; i++) {
										marker_new.setMap(null);
									}

									if (marker) {
										marker.setMap(null);
									}

									myLatlng = { lat: e.latLng.lat(), lng: e.latLng.lng() };
									marker = new google.maps.Marker({
										position: myLatlng,
										title: 'Selected Latitude Longitude'
									});



									marker.setMap(map);
									strLat = e.latLng.lat();
									strLng = e.latLng.lng();

									storeLat = GetWithDecimals(strLat);
									storeLng = GetWithDecimals(strLng);
									GetAddress();

								});
							} else {
								alert("No results found");
							}
						} else {
							alert("Geocode was not successful for the following reason: " + status);
						}
					});
				}
			} else {


				var myLatlng = { lat: parseFloat(strLat), lng: parseFloat(strLng) };


				map = new google.maps.Map(document.getElementById('map'), {
					zoom: 18,
					center: myLatlng
				});



				map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

				storeLat = GetWithDecimals(strLat);
				storeLng = GetWithDecimals(strLng);

				searchBox = new google.maps.places.SearchBox((input));

				var marker = new google.maps.Marker({
					position: myLatlng,
					//    map: map,
					title: 'Click to zoom'
				});

				marker.setMap(map);

				var clickEvent = new google.maps.event.addListener(map, "click", function (e) {

					for (var i = 0, marker_new; marker_new = markers[i]; i++) {
						marker_new.setMap(null);
					}

					marker.setMap(null);
					myLatlng = { lat: e.latLng.lat(), lng: e.latLng.lng() };
					marker = new google.maps.Marker({
						position: myLatlng,
						//    map: map,
						title: 'Selected Latitude Longitude'
					});
					marker.setMap(map);

					strLat = e.latLng.lat();
					strLng = e.latLng.lng();


					storeLat = GetWithDecimals(strLat);
					storeLng = GetWithDecimals(strLng);

					GetAddress();
				});
			}



			// Listen for the event fired when the user selects an item from the
			// pick list. Retrieve the matching places for that item.
			google.maps.event.addListener(searchBox, 'places_changed', function () {

				var places = searchBox.getPlaces();

				if (places.length == 0) {
					return;
				}
				for (var i = 0, marker; marker = markers[i]; i++) {
					marker.setMap(null);
				}

				// For each place, get the icon, place name, and location.
				markers = [];
				var bounds = new google.maps.LatLngBounds();
				for (var i = 0, place; place = places[i]; i++) {
					var image = {
						url: place.icon,
						size: new google.maps.Size(71, 71),
						origin: new google.maps.Point(0, 0),
						anchor: new google.maps.Point(17, 34),
						scaledSize: new google.maps.Size(25, 25)
					};

					// Create a marker for each place.
					markers.push(new google.maps.Marker({
						map: map,
						//icon: image,
						title: place.name,
						position: place.geometry.location
					}));

					storeLat = place.geometry.location.lat();
					storeLng = place.geometry.location.lng();

					bounds.extend(place.geometry.location);
				}

				map.fitBounds(bounds);
			});



			// current map's viewport.
			google.maps.event.addListener(map, 'bounds_changed', function () {

				var bounds = map.getBounds();
				searchBox.setBounds(bounds);

			});

		}
	</script>
	<script async defer src="https://maps.googleapis.com/maps/api/js?key=AIzaSyANyauXVaXeY_sQLfEHGZIgeB6dn2y3ErU&libraries=places"> </script>
	<script>
		function GetAddress() {
			$('#<%=txtLat.ClientID%>').val(parseFloat(storeLat).toFixed(5));
			$('#<%=txtLong.ClientID%>').val(parseFloat(storeLng).toFixed(5));

			var lat = parseFloat($('#<%=txtLat.ClientID%>')[0].value);
			var lng = parseFloat($('#<%=txtLong.ClientID%>')[0].value);
			var latlng = new google.maps.LatLng(lat, lng);
			var geocoder = geocoder = new google.maps.Geocoder();
			geocoder.geocode({ 'latLng': latlng }, function (results, status) {
				if (status == google.maps.GeocoderStatus.OK) {
					if (results[0]) {
						$('#<%=txtSiteAddress.ClientID%>')[0].value = results[0].formatted_address;
					}
				}
			});
		}
	</script>
	<div id="LocationBox" style="display: none">
		<div class="LocationPopUp">
			<div style="height: 500px;">
				<asp:TextBox ID="txtsearch" runat="server" CssClass="txtsearch" placeholder="Enter Search Place"></asp:TextBox>
				<div id="map">asd</div>
			</div>
			<div class="col-md-12 text-center" style="margin-top: 30px;">
				<input type="button" value="Submit" class="btn btn-success" onclick="CloseLocationBox(); return false;" />
				<input type="button" value="Cancel" class="btn btn-default" onclick="closebox();" />
			</div>
		</div>
	</div>
	<div id="datadiv">

		<asp:UpdatePanel ID="up_Main" runat="server">
			<ContentTemplate>

				<div class="modal fade" tabindex="-1" role="dialog" id="FuelingTimes">
					<div class="modal-dialog modal-lg">
						<div class="modal-content">
							<div class="modal-header">
								<h5 class="modal-title text-center">Click Box to Select all Fueling Times this site is Authorized to Fluid at:</h5>
							</div>
							<div class="modal-body">
								<div class="row col-md-12 col-sm-12">
									<asp:Label ID="lblFuelingTimes" runat="server" Text=""></asp:Label>
								</div>
								<div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
									<asp:UpdatePanel ID="UP_FuelingTimes" runat="server">
										<ContentTemplate>
											<asp:GridView ID="gv_FuelingTimes" CssClass="table table-bordered" runat="server" DataKeyNames="TimeId,TimeText" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
												<Columns>
													<asp:TemplateField HeaderText="">
														<ItemTemplate>
															<asp:CheckBox ID="CHK_FuelingTimes" runat="server" onclick="javascript:SelectboxFuelingTimes(this);" />
														</ItemTemplate>
													</asp:TemplateField>
													<asp:BoundField DataField="TimeText" HeaderText="Time" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												</Columns>
											</asp:GridView>

										</ContentTemplate>
									</asp:UpdatePanel>
								</div>
							</div>
							<div class="modal-footer nextButton">
								<input type="button" id="btnFuelingTimesOk" class="btn btn-success" onclick="CloseFuelingTimes()" value="Ok" />
								<input type="button" id="btnCloseFuelingTimes" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
								<asp:Button ID="btnCancelFuelingTimes" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="CloseFuelingTimes()" OnClick="btnCancelFuelingTimes_Click" />
							</div>
						</div>
						<!-- /.modal-content -->
					</div>
					<!-- /.modal-dialog -->
				</div>
				<!-- /.modal -->

				<div class="modal fade" tabindex="-1" role="dialog" id="FuelingDays">
					<div class="modal-dialog modal-lg">
						<div class="modal-content">
							<div class="modal-header">
								<h5 class="modal-title text-center">Click Box to Select all Fueling Days this site is Authorized to Fluid at:</h5>
							</div>
							<div class="modal-body">
								<div class="row col-md-12 col-sm-12">
									<asp:Label ID="LBL_FuelingDays" runat="server" Text=""></asp:Label>
								</div>
								<div class="row col-md-12 col-sm-12 text-center" style="overflow-x: auto; max-height: 400px;">
									<asp:UpdatePanel ID="UP_Days" runat="server">
										<ContentTemplate>
											<asp:GridView ID="GV_FuelingDays" CssClass="table table-bordered" runat="server" DataKeyNames="DayValue,DayName" AutoGenerateColumns="False" EmptyDataText="Data Not found.">
												<Columns>
													<asp:TemplateField HeaderText="">
														<ItemTemplate>
															<asp:CheckBox ID="CHK_FuelingDays" runat="server" onclick="javascript:SelectboxFuelingDays(this);" />
														</ItemTemplate>
													</asp:TemplateField>
													<asp:BoundField DataField="DayName" HeaderText="Day" ReadOnly="True" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
												</Columns>
											</asp:GridView>

										</ContentTemplate>
									</asp:UpdatePanel>
								</div>
							</div>
							<div class="modal-footer nextButton">
								<input type="button" id="btnFuelingDaysOk" class="btn btn-success" onclick="CloseFuelingDays()" value="Ok" />
								<input type="button" id="btnCloseFuelingDays" class="btn btn-success" data-dismiss="modal" style="display: none;" value="Close" />
								<asp:Button ID="btnCancelFuelingDays" runat="server" CssClass="btn btn-default" Text="Cancel" OnClientClick="CloseFuelingDays()" OnClick="btnCancelFuelingDays_Click" />
							</div>
						</div>
						<!-- /.modal-content -->
					</div>
					<!-- /.modal-dialog -->
				</div>
				<!-- /.modal -->

				<%--<!--alert message popup-->
                <div class="modal fade" tabindex="-1" role="dialog" id="FSUnitPersonMapping">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h3 class="modal-title text-center">FluidSecure</h3>
                            </div>
                            <div class="modal-body">
                                <h4>Do you want to Add this FluidSecure Link to all Personnel in your Organization?</h4>
                            </div>
                            <div class="modal-footer nextButton">
                                <input type="button" id="btnFSUnitPersonMappingSuccess" class="btn btn-success" onclick="AddMapping()" value="Yes" />
                                <input type="button" id="btnCloseMapping" class="btn btn-default" data-dismiss="modal" value="No" />
                            </div>
                        </div>
                        <!-- /.modal-content -->
                    </div>
                    <!-- /.modal-dialog -->
                </div>--%>
				<!-- /.modal -->

				<div class="panel panel-primary" style="margin: 20px;">
					<div class="panel-heading  text-center">
						<asp:Label class="panel-title" ID="lblHeader" runat="server"></asp:Label>
					</div>
					<div class="panel-body">
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<p class="text-center green" id="message" runat="server"></p>
							<p class="text-center red" id="ErrorMessage" runat="server"></p>
						</div>

						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									FluidSecure Link Number
                        <label class="text-danger font-required">[Auto Generated]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtSiteNo" CssClass="form-control input-sm" TabIndex="1" runat="server" MaxLength="10" Width="95" onkeypress="return onlyNumbers(event);"></asp:TextBox>
								<%--<asp:RequiredFieldValidator ID="RFVsitenumber" runat="server" ErrorMessage="Please Enter FluidSecure Link Number." ControlToValidate="txtSiteNo" Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CV_SiteNumber" runat="server" ControlToValidate="txtSiteNo" ErrorMessage="FluidSecure Link Number should be integer number" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="SiteValidation" Display="Dynamic"></asp:CompareValidator>
								--%>
								<asp:HiddenField ID="HDF_SiteID" runat="server" />
								<asp:HiddenField ID="HDF_TotalSite" runat="server" />
								<asp:HiddenField ID="HDF_HoseId" runat="server" />
								<asp:HiddenField ID="HDF_TotalHose" runat="server" />
								<%--<asp:HiddenField ID="hdf_OpenBox" runat="server" Value="" />--%>
							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Select Location:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<input type="button" tabindex="18" id="BTN_SelectLocation" onclick="OpenLocationBox();" value="Select Location" />
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									FluidSecure Link Current Name (SSID)
                                <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtwifissid" runat="server" CssClass="form-control input-sm" MaxLength="32" TabIndex="2"></asp:TextBox>
								<asp:RequiredFieldValidator ID="RFVWifiSSID" runat="server" ControlToValidate="txtwifissid"
									ErrorMessage="Please Enter FluidSecure Link Current Name." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="REV_WifiSSIID" runat="server" ControlToValidate="txtwifissid" ErrorMessage="Enter only alpanumeric characters." ValidationExpression="^[a-zA-Z0-9\-_ ]*$"
									Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RegularExpressionValidator>
							</div>

							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>Disable Geo Location:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:CheckBox ID="CHK_DisableGeoLocation" runat="server" TabIndex="19" />
							</div>

						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									FluidSecure New Name (will change on next fueling):</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txt_ReplaceableHoseName" runat="server" CssClass="form-control input-sm" MaxLength="32" TabIndex="3"></asp:TextBox>
								<asp:RegularExpressionValidator ID="REV_ReplaceableHoseName" runat="server" ControlToValidate="txt_ReplaceableHoseName" ErrorMessage="Enter only alpanumeric characters." ValidationExpression="^[a-zA-Z0-9\-_ ]*$"
									Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RegularExpressionValidator>
							</div>

							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Latitude:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txtLat" runat="server" CssClass="form-control input-sm" TabIndex="20" MaxLength="50" Enabled="false"></asp:TextBox>
								<asp:RegularExpressionValidator ID="reqreglan1" runat="server" ControlToValidate="txtLat" Display="Dynamic" ForeColor="Red"
									SetFocusOnError="true" ValidationGroup="SiteValidation" ErrorMessage="Please enter the Valid Latitude" ValidationExpression="^(\+|-)?(?:90(?:(?:\.0{1,6})?)|(?:[0-9]|[1-8][0-9])(?:(?:\.[0-9]{1,5})?))$"></asp:RegularExpressionValidator>
							</div>

						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Tank Number
                                <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<%--<asp:TextBox ID="txtankNumber" runat="server" CssClass="form-control input-sm" MaxLength="3" TabIndex="4" Width="50" onkeypress="return onlyNumbers(event);"></asp:TextBox>--%>
								<asp:DropDownList ID="DDL_Tank" runat="server" TabIndex="4" CssClass="form-control input-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_Tank_SelectedIndexChanged"></asp:DropDownList>
								<asp:RequiredFieldValidator ID="RFDDDL_Tank" runat="server" ControlToValidate="DDL_Tank"
									ErrorMessage="Please select tank." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation" InitialValue="0"></asp:RequiredFieldValidator>
								<%--<asp:CompareValidator ID="CVTankNumber" runat="server" ControlToValidate="DDL_Tank" ErrorMessage="Tank Number  should be integer number" ForeColor="Red" Operator="DataTypeCheck" Display="Dynamic" Type="Integer" ValidationGroup="SiteValidation"></asp:CompareValidator>--%>
							</div>

							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Longitude:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txtLong" runat="server" CssClass="form-control input-sm" TabIndex="21" MaxLength="50" Enabled="false"></asp:TextBox>
								<asp:RegularExpressionValidator ID="reqreglan2" runat="server" ControlToValidate="txtLong" Display="Dynamic" ForeColor="Red"
									SetFocusOnError="true" ValidationGroup="SiteValidation" ErrorMessage="Please enter the Valid Longitude" ValidationExpression="^(\+|-)?(?:180(?:(?:\.0{1,6})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\.[0-9]{1,5})?))$"></asp:RegularExpressionValidator>
							</div>

						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Product in Tank
                                <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:DropDownList ID="ddlFuelType" runat="server" TabIndex="5" CssClass="form-control input-sm" Enabled="false"></asp:DropDownList>
								<asp:RequiredFieldValidator ID="RFVFuelType" runat="server" ErrorMessage="Please select Product in Tank."
									ControlToValidate="ddlFuelType" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
							</div>

							<div class="form-group col-md-3 col-sm-3 textright col-xs-12" id="PWDLabel" runat="server">
								<label>
									Address:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12" id="PWDTextbox" runat="server">
								<asp:TextBox ID="txtSiteAddress" runat="server" CssClass="form-control input-sm" TabIndex="22" Enabled="false"></asp:TextBox>

							</div>


						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<%--     <div class="form-group col-md-4 col-sm-4 textright col-xs-12">
                                <label>Tank Monitor:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:CheckBox ID="Chk_TankMonitor" runat="server" TabIndex="6" />
                            </div>--%>


							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Pump On Time
                                <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txtPumpOnTime" runat="server" CssClass="form-control input-sm" MaxLength="3" Width="50" TabIndex="6" data-toggle="tooltip" title="(Seconds after pump turns on that user has to start pumping)" onkeypress="return onlyNumbers(event);"></asp:TextBox>
								<asp:RequiredFieldValidator ID="RFVPumpOnTime" runat="server" ControlToValidate="txtPumpOnTime"
									ErrorMessage="Please Enter Pump On Time." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
								<asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtPumpOnTime" Display="Dynamic" ErrorMessage="Seconds after pump turns on that user has to start pumping should be integer number" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="SiteValidation"></asp:CompareValidator>
							</div>

							<%--  </div>

                        <div class="row col-md-12 col-sm-12 col-xs-12">--%>

							<%--<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
                                <label>Tank Monitor Number:</label>
                            </div>
                            <div class="form-group col-md-3 col-sm-3 col-xs-12">
                                <asp:TextBox ID="txtTankMonitorNo" runat="server" CssClass="form-control input-sm" MaxLength="3" TabIndex="7" Width="50" onkeypress="return onlyNumbers(event);"></asp:TextBox>
                                <asp:CompareValidator ID="CVTankMonitorNumber" runat="server" ControlToValidate="txtTankMonitorNo" Display="Dynamic" ErrorMessage="Tank Monitor Number should be integer number" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="SiteValidation"></asp:CompareValidator>
                            </div>--%>

							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>Pump Off Time<label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txtPumpOffTime" runat="server" CssClass="form-control input-sm" MaxLength="3" TabIndex="23" Width="50" data-toggle="tooltip" title="(Seconds without fuel flow or cell communication) " onkeypress="return onlyNumbers(event);"></asp:TextBox>
								<asp:RequiredFieldValidator ID="RFVPumpOffTime" runat="server" ControlToValidate="txtPumpOffTime"
									ErrorMessage="Please Enter Pump Off Time." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
								<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtPumpOffTime" ErrorMessage="Seconds without fuel flow or cell communication should be integer number" Display="Dynamic" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="SiteValidation"></asp:CompareValidator>
							</div>

						</div>

						<div class="row col-md-12 col-sm-12 col-xs-12">

							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Authorized Fueling Times
                                 <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<input type="button" id="BTN_SiteTypeTimings" onclick="OpenFuelTimingsBox();" tabindex="8" value="Authorized Fueling Times" />
							</div>

							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
								MAC Address:
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txtIpAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="24"></asp:TextBox>
							</div>


						</div>

						<div class="row col-md-12 col-sm-12 col-xs-12">

							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Authorized Fueling Days
                        <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<input type="button" id="BTN_SiteDays" onclick="OpenSiteDayBox();" tabindex="9" value="Authorized Fueling Days" />
							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>Connected Hub:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:Label ID="lblConnectedHub" runat="server" Style="font-weight: bold; font-size: 15px"></asp:Label>
							</div>
						</div>

						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>Export Code:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txt_ExportCode" runat="server" CssClass="form-control input-sm" MaxLength="10" TabIndex="10"></asp:TextBox>
							</div>

							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>Site Name:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:Label ID="lblSite" runat="server" Style="font-weight: bold; font-size: 15px"></asp:Label>
							</div>
						</div>

						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">Calibrate Tank :</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								&nbsp;
							</div>
							<div id="divCompany" runat="server" class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Company
                                <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:DropDownList ID="ddlCustomer" runat="server" TabIndex="24" AutoPostBack="true" CssClass="form-control input-sm" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged"></asp:DropDownList>
								<asp:RequiredFieldValidator ID="RFV_Cust" runat="server" ControlToValidate="ddlCustomer" Display="Dynamic" ErrorMessage="Please select Company." ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
							</div>
						</div>

						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Units measured
                                    <label class="text-danger font-required">[required]</label>: *</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtUnitsmeasured" runat="server" CssClass="form-control input-sm" TabIndex="10" Width="95" onchange="FindPulserRatio()" data-toggle="tooltip" title="Units measured is the total amount dispensed for calibration and should be whole units. Ex. If 10 gallons are pumped - enter as 10, not 10.0"></asp:TextBox>
								<asp:RequiredFieldValidator ID="RDF_Unitsmeasured" runat="server" ControlToValidate="txtUnitsmeasured"
									ErrorMessage="Please Enter Units measured." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
								<asp:CompareValidator ID="CV_Quantity" runat="server" Display="Dynamic" ErrorMessage="Please enter Units measured in number format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="SiteValidation" ControlToValidate="txtUnitsmeasured"></asp:CompareValidator>

							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Original Name Of FluidSecure Link
                                <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="TXT_OriginalNameOfFluidSecure" runat="server" CssClass="form-control input-sm" MaxLength="32" TabIndex="26"></asp:TextBox>
								<asp:RequiredFieldValidator ID="RDF_OriginalNameOfFluidSecure" runat="server" ControlToValidate="TXT_OriginalNameOfFluidSecure" ErrorMessage="Please Enter Original Name Of FluidSecure Link." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="REV_OriginalNameOfFluidSecure" runat="server" ControlToValidate="TXT_OriginalNameOfFluidSecure" ErrorMessage="Enter only alpanumeric characters." ValidationExpression="^[a-zA-Z0-9\-_ ]*$"
									Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RegularExpressionValidator>
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Pulses
                                    <label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txt_Pulses" runat="server" CssClass="form-control input-sm" MaxLength="5" TabIndex="11" Width="95" onchange="FindPulserRatio()"></asp:TextBox>
								<asp:RequiredFieldValidator ID="RDF_Pulses" runat="server" ControlToValidate="txt_Pulses"
									ErrorMessage="Please Enter Pulses." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
								<asp:CompareValidator ID="CV_Pulses" runat="server" Display="Dynamic" ErrorMessage="Please enter Pulses in number format." ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True" Type="Integer" ValidationGroup="SiteValidation" ControlToValidate="txt_Pulses"></asp:CompareValidator>

							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Current Firmware Version
                                     <label class="text-danger font-required">[required]</label>:</label>
								</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txt_FirmwareVer" runat="server" CssClass="form-control input-sm" TabIndex="27" MaxLength="30" Enabled="false"></asp:TextBox>
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Pulser Ratio:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtPulserRatio" runat="server" CssClass="form-control input-sm" MaxLength="10" TabIndex="12" Width="95" onkeypress="return onlyNumbers(event);" ReadOnly="true"></asp:TextBox>
								<asp:CompareValidator ID="CVPulserRatio" runat="server" ControlToValidate="txtPulserRatio" ErrorMessage="Pulser Ratio should be decimal format." Display="Dynamic" ForeColor="Red" Operator="DataTypeCheck" Type="Double" ValidationGroup="SiteValidation"></asp:CompareValidator>
							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>Is FluidSecure Link Busy:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:CheckBox ID="CHK_IsBusy" runat="server" TabIndex="28" />
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Pulser Timing Adjust:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<div class="col-md-12" style="padding: 0px !important">
									<div class="col-md-4" style="padding: 0px !important">
										<asp:TextBox ID="txtSwitchTimeB" runat="server" CssClass="form-control input-sm" MaxLength="3" TabIndex="13" Width="90" onkeypress="return onlyNumbers(event);" value="30"></asp:TextBox>
									</div>
									<div class="col-md-2" style="padding: 0px !important">
										<h6>
											<asp:Label runat="server" CssClass="form-inline" ID="lblms" Text="In ms"></asp:Label></h6>
									</div>
								</div>
								<asp:RequiredFieldValidator ID="RDF_txtSwitchTimeB" runat="server" ControlToValidate="txtSwitchTimeB"
									ErrorMessage="Please Enter Pulser Timing Adjust." Display="Dynamic" ForeColor="Red" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Time Zone<label class="text-danger font-required">[required]</label>:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:DropDownList ID="DDL_TimeZone" runat="server" TabIndex="14" CssClass="form-control input-sm" AutoPostBack="true" OnSelectedIndexChanged="DDL_TimeZone_SelectedIndexChanged"></asp:DropDownList>
								<asp:RequiredFieldValidator ID="RFV_TimeZone" runat="server" ErrorMessage="Please select your time zone."
									ControlToValidate="DDL_TimeZone" Display="Dynamic" ForeColor="Red" InitialValue="0" SetFocusOnError="True" ValidationGroup="SiteValidation"></asp:RequiredFieldValidator>
							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Selected time zone:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:Label ID="LBL_TimeZone" runat="server" Style="font-weight: bold; font-size: 15px"></asp:Label>
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Assign this FS link to all Vehicles having the same fueling product:</label>

							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:CheckBox ID="chk_vehicleMap" runat="server" TabIndex="15" />
							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Assign this FS link to all Personnel:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:CheckBox ID="chk_personnelMap" runat="server" TabIndex="28" />
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									Number Of Zero Transaction:</label>
							</div>
							<div class="form-group col-md-3 col-sm-3 col-xs-12">
								<asp:TextBox ID="txtNumberOfZeroTransaction" runat="server" TabIndex="16" CssClass="form-control input-sm" Width="50" onkeypress="return onlyNumbers(event);" Text="0" MaxLength="4" />
								<asp:CompareValidator ID="CVNumberOfZeroTransaction" runat="server" ControlToValidate="txtNumberOfZeroTransaction"
									ErrorMessage="Number Of Zero Transaction in number format." Display="Dynamic" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="SiteValidation"></asp:CompareValidator>
							</div>
							<div class="form-group col-md-3 col-sm-3 textright col-xs-12">
								<label>
									Display order:</label>
							</div>

							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txtDisplayOrder" runat="server" TabIndex="29" CssClass="form-control input-sm" Width="50" onkeypress="return onlyNumbers(event);" MaxLength="4" />
								<asp:CompareValidator ID="CVDisplayOrder" runat="server" ControlToValidate="txtDisplayOrder" ErrorMessage="Display Order in numer format."
									Display="Dynamic" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ValidationGroup="SiteValidation"></asp:CompareValidator>
							</div>
						</div>
                        <div class="row col-md-12 col-sm-12 col-xs-12">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>
									FSNP Mac Address:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:TextBox ID="txtFSNPMacAddress" runat="server" CssClass="form-control input-sm" MaxLength="50" TabIndex="17"></asp:TextBox>
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 col-xs-12" id="divActivate" runat="server">
							<div class="form-group col-md-4 col-sm-4 textright col-xs-12">
								<label>Activate:</label>
							</div>
							<div class="form-group col-md-2 col-sm-2 col-xs-12">
								<asp:CheckBox ID="CHK_Activate" runat="server" TabIndex="30"  data-toggle="tooltip" title="(If link is repaired user can re-activate the link by clicking the “Activate” checkbox.) "/>
							</div>
						</div>
						<div class="row col-md-12 col-sm-12 text-center col-xs-12">
							<asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" Text="Save" Width="100px"
								UseSubmitBehavior="False" TabIndex="31" ValidationGroup="SiteValidation" />
							<asp:Button ID="btnCancel" CssClass="btn btn-default" runat="server" Text="Cancel" Width="100px" CausesValidation="False"
								UseSubmitBehavior="False" TabIndex="32" OnClick="btnCancel_Click" />
							<asp:Button ID="btnSaveAndAddNew" CssClass="btn btn-primary" runat="server" Text="Save & Add New" Width="150px"
								UseSubmitBehavior="false" TabIndex="33" ValidationGroup="SiteValidation" OnClick="btnSaveAndAddNew_Click" />
						</div>
						<div class="row col-md-12 col-sm-12 text-center clear col-xs-12" style="margin: 10px 0">
							<asp:Button ID="btnFirst" runat="server" Text="|<" CssClass="NewDept_ButtonFooter"
								OnClick="First_Click" /><asp:Button ID="btnprevious" runat="server" Text="<" CssClass="NewDept_ButtonFooter" OnClick="btnprevious_Click" />
							<asp:Label ID="lblof" runat="server" Text="Label" BorderColor="Black" BorderStyle="Solid"
								BorderWidth="1px" Font-Bold="True" Font-Names="arial" Font-Size="Small" Width="115px"></asp:Label>
							<asp:Button ID="btnNext" runat="server" Text=">" CssClass="NewDept_ButtonFooter" OnClick="btnNext_Click" /><asp:Button
								ID="btnLast" runat="server" Text=">|" CssClass="NewDept_ButtonFooter" OnClick="btnLast_Click" />
						</div>
						<p style="color: #021ffb; font-size: 14px; margin-top: 10px;">* : All calibration quantities must be whole units (gallons, liters, etc.) without tenths.</p>
					</div>
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>

	</div>
	<script>

		//$(function () {
		//    FindPulserRatio();
		//});

		//function OpenPersonFSUnitMappingBox() {

		//    $('#FSUnitPersonMapping').modal({
		//        show: true,
		//        backdrop: 'static',
		//        keyboard: false
		//    });
		//}


		function CloseFuelingTimes() {
			$("#btnCloseFuelingTimes").click();
			$('#BTN_SiteDays').focus();
			enableDisableButtons(false)
		}

		function CloseFuelingDays() {
			$("#btnCloseFuelingDays").click();
			$('#<%= txtUnitsmeasured.ClientID%>').focus();
			enableDisableButtons(false)
		}

		function enableDisableButtons(enableDisable) {
			if (document.getElementById('<%= btnFirst.ClientID%>') != null) {
				document.getElementById('<%= btnFirst.ClientID%>').disabled = enableDisable;
				document.getElementById('<%= btnLast.ClientID%>').disabled = enableDisable;
				document.getElementById('<%= btnNext.ClientID%>').disabled = enableDisable;
				document.getElementById('<%= btnprevious.ClientID%>').disabled = enableDisable;
			}

			document.getElementById('<%= btnSave.ClientID%>').disabled = enableDisable;
			document.getElementById('<%= btnCancel.ClientID%>').disabled = enableDisable;
		}
	</script>
	<style>
		.ui-tooltip {
			background-color: #ffffff;
			font-size: 12px;
			padding: 6px;
			z-index: 9999;
		}

		th {
			text-align: center;
		}
	</style>
	<script type="text/javascript">

		function loadFunction() {
			$('[data-toggle="tooltip"]').tooltip();
			FindPulserRatio();
            <%--var hdf_OpenBox = $("#<%= hdf_OpenBox.ClientID %>");
            if (hdf_OpenBox.val() == "Y") {
                OpenPersonFSUnitMappingBox();
            }--%>
		}
		$(function () {
			loadFunction();
		});

		function FindPulserRatio() {

			var Unitsmeasured = $("#<%=txtUnitsmeasured.ClientID%>");
			var Pulses = $("#<%=txt_Pulses.ClientID%>").val();
			var PulserRatio = $("#<%=txtPulserRatio.ClientID%>");

			if (isNaN(parseInt(Unitsmeasured.val())) != true) {
				Unitsmeasured.val(parseInt(Unitsmeasured.val()));
			}

			if (isNaN(parseInt(Unitsmeasured.val())) != true && isNaN(parseInt(Pulses)) != true) {
				PulserRatio.val((parseInt(Pulses) / parseInt(Unitsmeasured.val())).toFixed(2));
			}

		}

		function OpenSiteDayBox() {
			enableDisableButtons(true);
			$('#FuelingDays').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});

		}

		function OpenFuelTimingsBox() {
			enableDisableButtons(true);
			$('#FuelingTimes').modal({
				show: true,
				backdrop: 'static',
				keyboard: false
			});
		}


	</script>
	<script language="javascript">

		function SelectboxFuelingDays(spanChk) {

			if (spanChk.id.toLowerCase().indexOf("chk_fuelingdays_0") > -1) {
				var IsChecked = spanChk.checked;

				Parent = document.getElementById('<%= GV_FuelingDays.ClientID %>');
				var items = Parent.getElementsByTagName('input');
				for (i = 0; i < items.length; i++) {
					if (items[i].id != spanChk.id && items[i].type == "checkbox") {
						if (items[i].checked != IsChecked) {
							items[i].checked = IsChecked;
						}
					}
				}

			}
			else {
				var checkBoxSelector = "#<%=GV_FuelingDays.ClientID%> input[id*='CHK_FuelingDays_0']";

				var IsChecked = spanChk.checked;
				if (IsChecked == false) {
					Parent = document.getElementById('<%= GV_FuelingDays.ClientID%>');


					$(checkBoxSelector).attr('checked', false);
				}
				else {
					var isAll = false;

					Parent = document.getElementById('<%= GV_FuelingDays.ClientID %>');
					var items = Parent.getElementsByTagName('input');
					for (i = 0; i < items.length; i++) {
						if (items[i].type == "checkbox" && !(items[i].id.toLowerCase().indexOf("chk_fuelingdays_0") > -1)) {
							if (items[i].checked == true) {
								isAll = true;
							}
							else {
								isAll = false;
								break;
							}
						}
					}

					if (isAll == true) {
						$(checkBoxSelector)[0].checked = true;
					}
					else
						$(checkBoxSelector)[0].checked = false;
				}
			}
		}

		function SelectboxFuelingTimes(spanChk) {

			if (spanChk.id.toLowerCase().indexOf("chk_fuelingtimes_0") > -1) {
				var IsChecked = spanChk.checked;

				Parent = document.getElementById('<%= gv_FuelingTimes.ClientID %>');
				var items = Parent.getElementsByTagName('input');
				for (i = 0; i < items.length; i++) {
					if (items[i].id != spanChk.id && items[i].type == "checkbox") {
						if (items[i].checked != IsChecked) {
							items[i].checked = IsChecked;
						}
					}
				}

			}
			else {
				var checkBoxSelector = "#<%=gv_FuelingTimes.ClientID%> input[id*='CHK_FuelingTimes_0']";

				var IsChecked = spanChk.checked;
				if (IsChecked == false) {
					Parent = document.getElementById('<%= gv_FuelingTimes.ClientID%>');


					$(checkBoxSelector).attr('checked', false);
				}
				else {
					var isAll = false;

					Parent = document.getElementById('<%= gv_FuelingTimes.ClientID %>');
					var items = Parent.getElementsByTagName('input');
					for (i = 0; i < items.length; i++) {
						if (items[i].type == "checkbox" && !(items[i].id.toLowerCase().indexOf("chk_fuelingtimes_0") > -1)) {
							if (items[i].checked == true) {
								isAll = true;
							}
							else {
								isAll = false;
								break;
							}
						}
					}

					if (isAll == true) {
						$(checkBoxSelector)[0].checked = true;
					}
					else
						$(checkBoxSelector)[0].checked = false;
				}
			}
		}

	</script>


</asp:Content>
