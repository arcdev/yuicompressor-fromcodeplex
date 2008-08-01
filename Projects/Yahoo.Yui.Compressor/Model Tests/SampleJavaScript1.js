
// Please note... this requires the Map.js file to be included in the parent page to work!

// global constants
var adMap;
var geocoder = new GClientGeocoder();
var isSearching = false;
var lastSearch = "";

// These need to be replaced by values in the host page...
var countryName = "";
var arrayName = "";
var regionCode = "";

// Zoom arrays: (min, nice, max)
var markerZoom = new Array(11,13,16);
var stateZoom = new Array(5,8,10);
var countryZoom = new Array(0,4,4);

// Country arrays: (full name, lat, long, country code, nice zoom level)
var Australia = new Array('Australia', '-26.98', '133.85', 'AU', countryZoom[1]);
var Canada = new Array('Canada', '53.69', '-100.81', 'CA', countryZoom[1]);
var UnitedStates = new Array('United States', '38.27', '-98.87', 'US', countryZoom[1]);
var UnitedKingdom = new Array('United Kingdom', '54.18', '-2.7', 'UK', stateZoom[0]);

var errorIcon = "<img src='/Image/icon_error.png' align='texttop' style='height:16px;width:16px' /> ";
var infoIcon = "<img src='/Image/icon_information.png' align='texttop' style='height:16px;width:16px' /> "; 
var warningIcon = "<img src='/Image/icon_warning.png' align='texttop' style='height:16px;width:16px' /> ";
var successIcon = "<img src='/Image/icon_check.png' align='texttop' style='height:16px;width:16px' /> ";
var tooManyMarkersResponse = "Maximum length exceeded.";
var tooManyMarkersMessage = "Too many markers to display. Please zoom in or click on the map.";

// Map centering ----------------------------------------------------------------------------------------
function centerMap(target, zoom) {
    if(target){
        try{
            var commaIndex = target.indexOf(",");
        }catch(err){
            var commaIndex = 0;
        }
        if(commaIndex > 0){
            var lat = target.substring(0,target.indexOf(",")-1);
            var lng = target.substring(target.indexOf(",")+1,target.length);
            var point = new GLatLng(lat,lng);
            
            if(point){
                doCenter(point, zoom, target);
            }
        }else{
            geocoder.getLatLng(target, doCenter(point, zoom, target));
        }
    }
}

function doCenter(point, zoom, target) {

    if(!isNaN(parseInt(zoom))){
        if(zoom != adMap._map.getZoom()){
            adMap._map.setZoom(zoom);
        }
    }

    if (!point) {
        showInfoMessage(errorIcon + "Sorry, we cannot find '" + target + "'.", 0);
    } else {
        hideInfoMessage();
        adMap._map.setCenter(point);
        setTimeout("getMarkers()", 1000);
    }
}

// User Feedback ----------------------------------------------------------------------------------------
function toggleSearching(show){
    isSearching = show;

    var searching = $get("mapSearch");
    if(show){
        hideInfoMessage();
        searching.style.display = "block";
    }else{
        searching.style.display = "none";
    }
}

function showInfoMessage(message, timeout){
    var infoMessage = $get("mapMessage");
    var messageFormat = "{0}<a href='#' title='hide information window'></a>";
    
    toggleSearching(false);
    
    infoMessage.innerHTML = String.format(messageFormat,message);
    infoMessage.style.display = "block";
    
    if(timeout > 0){
        window.setTimeout("hideInfoMessage()", timeout);
    }
}

function hideInfoMessage(){
    var infoMessage = $get("mapMessage");
    infoMessage.innerHTML = "";
    infoMessage.style.display = "none";
}

// Markers -------------------------------------------------------------------------------------------

// Used to find a marker by its latlong.  If it can't be found the map is centered on it...
function hilightMarker(target, zoom, hover){
    
    if(target){
        var isFound = false;
        var commaIndex = target.indexOf(",");
        var targetPoint = new GLatLng(target.substring(0,commaIndex), target.substring(commaIndex+1, target.length));
        
        for(x=0;x<markerList.length;x++){
            var point = markerList[x].getPoint();
            if(point.lat() == targetPoint.lat() &&
               point.lng() == targetPoint.lng()){
                isFound = true;
                hideInfoMessage();
                hilightThis(markerList[x], hover);
            }
        }
        
        if(!isFound){
            //centerMap(target, zoom);
            showInfoMessage(infoIcon + "This marker is not in view.  Click the 'Find' icon to go there.");
        }
    }
}

// Highlights or unhighlights the input marker depending on the hover flag
function hilightThis(marker, hover){

    if(marker && marker.div){
        var zIndex = 0;
        if(hover){
            zIndex = parseInt(GOverlay.getZIndex(marker.getPoint().lat()-10));
            marker.div.className = 'MapTextTipControlHover';
            marker.div.id = "markerOn";
            marker.setImage('/Image/map_text_pin_2.png');
        }else{
            zIndex = parseInt(GOverlay.getZIndex(marker.getPoint().lat()));
            marker.div.className = 'MapTextTipControl';
            marker.div.id = "markerOff";
            marker.setImage('/Image/map_text_pin.png');
        }
        
        marker.div.style.zIndex = zIndex; //make sure it's on top of all the other markers around it
        
        var images = document.getElementsByTagName("img");
        
        for(x=0;x<images.length;x++){
            
            var theImage = images[x];
            
            if(theImage && theImage.nextSibling){
                if(theImage.nextSibling.id == "markerOn"){
                    theImage.style.zIndex = zIndex + 1; //this is the pin image which needs to be on top of the div
                }
            }
        }
        
        marker.redraw();
    }
}

function addMarkers(data){
    var markers = [];
    var labelTextFormat = "{0}<br><b>{1}, {2}</b>";
    var label = "";
    
    if(data){
        Array.clear(markerList);
    
        for(var i in data)
        {
            // Without these we don't actually have a proper marker.
            if(data[i].PrimaryCity && data[i].Subdivision){
            
                // Format marker text according to returned data.
                label = String.format(labelTextFormat, data[i].PrimaryCity, data[i].Subdivision, data[i].CountryRegion);
            
                if(data[i].Latitude && data[i].Longitude){
                
                    var point = new GLatLng(data[i].Latitude, data[i].Longitude);
                    var icon = createTextIcon();
                    var marker = new TextMarker(point,
                        {icon: icon,
                        labelText: label,
                        labelClass: "MapTextTipControl", 
                        labelOffset: new GSize(-5,-45)});
                    
                    marker.type = "a";
                        
                }else{
                    var point = adMap._map.getCenter();
                    var icon = createBillboardIcon();
                    var marker = new TextMarker(point,
                        {icon: icon,
                        labelText: infoIcon + label,
                        labelClass: "MapTextTipControl", 
                        labelOffset: new GSize(-20,-20)});
                        
                    GEvent.addListener(marker, 'click', onGroupMarkerClick);
                        
                    marker.type = "b";
                }
                
                marker.Id = data[i].Id;
                marker.PCi = data[i].PrimaryCity;
                marker.Sub = data[i].Subdivision;
                marker.CR = data[i].CountryRegion;
                
                Array.add(markerList, marker);
            }
        } // end for

        //redraw markers
        adMap._map.clearOverlays();
        for(var m in markerList){
            if(markerList[m].latlng){ // Make sure we really do have a marker!
                adMap._map.addOverlay(markerList[m]);
            }
        }
    }
    
    if(data[0].CountryRegion){
        if(data.length > 1){
            label = data.length + " opportunities in this area!";
        }else{
            label = data.length + " opportunity in this area!";
        }
        showInfoMessage(successIcon + label, 0);
    }else{
        toggleSearching(false);
    }
}

// get markers ----------------------------------------------------------------------

function onGetMarkersSuccess(result){
    var data = eval(result);
    var noDataMessage = "No opportunities in this area.";
    var noneVisibleResponse = "None Visible";
    var message = "";

    if (!data){
        showInfoMessage(infoIcon + noDataMessage, 0);
    }else{
        if(data[0]){
            addMarkers(data);
         }else{
            showInfoMessage(infoIcon + noDataMessage, 0);
        }
    }
}

function onGetMarkersFailed(result, error){
    var errorMessage = result.get_message();
    
    if(errorMessage == tooManyMarkersResponse){
        errorMessage = tooManyMarkersMessage;
    }
    showInfoMessage(errorIcon + errorMessage, 0);
}

function getMarkers(){

    var point = adMap._map.getCenter();
    var thisSearch = point.lat()+point.lng()+adMap._map.getZoom();

    if(!isSearching && thisSearch != lastSearch){ // Avoid consecutive searches
        
        toggleSearching(true);
        
        lastSearch = thisSearch;
        
        Appian.Services.UIService.SearchSponsorsOnMap(point.lat(),
            point.lng(),
            adMap.getMapRadius(0.75, true),
            onGetMarkersSuccess,
            onGetMarkersFailed);
    }
}

// Marker bubble ------------------------------------------------------------------------------

function onGetSponsorshipSuccess(result, marker){
    var data = eval(result);
    var buyFormat = successIcon + "<a href='#' onclick='addToCart({0},{1},this)'>Add '{2}' Billboard to cart</a><br>";
    var unavailFormat = errorIcon + "'{0}' is unavailable<br>";
    var errorFormat = "{0}<b>Sorry...</b> {1}";
    var tempHtml = "";
    var bubbleFormat = "<div class='SponsorBubble'><b>{0}, {1}, {2}</b><div id='list'>{3}</div></div>";
    var bubbleHtml;

    if (!data){
        tempHtml = String.format(errorFormat,
            errorIcon,
            "No opportunities in this area.");
    }else{
        for(var i in data){
            if(data[i].Id){
                if(data[i].IsAvailable){
                    tempHtml += String.format(buyFormat,
                        data[i].IdAdvertisementLocation,
                        data[i].Id,
                        data[i].Description);
                }else{
                    tempHtml += String.format(unavailFormat,
                        data[i].Description);
                }
            }
        }
    }
    
    bubbleHtml = String.format(bubbleFormat,
        marker.PCi,
        marker.Sub,
        marker.CR,
        tempHtml);
    
    marker.openInfoWindowHtml(bubbleHtml + "</div>");
    GEvent.addListener(marker, 'infowindowclose', function(marker){hilightThis(marker,false);});
    
    hilightThis(marker, true);
}

function onGetSponsorshipFailed(result, error){
    showInfoMessage(errorIcon + result.get_message(), 0);
}

function getMarkerBubble(marker){
    if (marker.type == "a"){
        //var bubbleHtml = "<br><img src='/Image/loading.gif' align='texttop'> Loading...";
        //marker.openInfoWindowHtml(bubbleHtml);

        Appian.Services.UIService.GetSponsorshipDetails(marker.Id,
            onGetSponsorshipSuccess,
            onGetSponsorshipFailed,
            marker);
    }
}

// Map event handlers -------------------------------------------------------------------

function onGroupMarkerClick(marker, point){
        hideInfoMessage();
        if(adMap._map.getZoom() <= stateZoom[0]){
            adMap._map.setCenter(point, stateZoom[1]);
        }else{
            adMap._map.setCenter(point, adMap._map.getZoom()+1);
        }
        setTimeout("getMarkers()", 1000);
}

function onMapClick(marker, point){
    if(marker){
        getMarkerBubble(marker);
    }
}

// prevent the enter key from submitting the page ------------------------------------------

function onTextKeyPress(e){
    var keyCode;

    if(e && e.which){
        keyCode = e.which;
    }else{
        if(window.event){
            keyCode = window.event.keyCode;
        }
    }

    if(keyCode){
        if (keyCode == Sys.UI.Key.enter){
            mapSearch();
        }
    }
}

// Country Region change handling ------------------------------------------------------------
function toggleRegions(show){
    var list = $get("regionlist");

    if(show){
        list.style.display = "block";
    }else{
        list.style.display = "none";
    }
}

function selectRegion(country){
    toggleRegions(false);

    if(country[0] != countryName){ //Just in case they re-selected the same country
        countryName = country[0];
        countryCode = country[3];
        
        var countryFlag = $get("countryFlag");
        var currentCountry = $get("currentCountry");
        var searchText = $get("mapSearchText");
        
        countryFlag.src = "/Image/Flags/" + country[3] + ".png";
        currentCountry.innerText = country[0];
        
        if(searchText.value.length > 0){
            mapSearch();
        }else{
            var point = adMap._map.getCenter();
            lastSearch = point.lat()+point.lng()+country[4]; //ensure we don't try to get markers until we've moved country.
            
            centerMap(country[1] + "," + country[2], country[4]);
            //getMarkers();
        }
    }
}

// Map Search ----------------------------------------------------------------------

function mapSearch(){
    var searchBox = $get("mapSearchText");
    
    hideInfoMessage();
    
    if(searchBox.value.length > 0){
    
        var search = searchBox.value + ", " + this.countryName;
        searchBox.value = "";

        geocoder.getLatLng(search,
            function(tryPoint) {
                if (!tryPoint) {
                    showInfoMessage(infoIcon + "Sorry, we couldn't find the location you were looking for.  Try clicking its location on the map.", 0);
                } else {
                    if(adMap._map.getZoom() <= markerZoom[0]){
                        // This will trigger the getMarkers() through the zoomend event.
                        adMap._map.setCenter(tryPoint, markerZoom[1]);
                    }else{
                        adMap._map.setCenter(tryPoint, adMap._map.getZoom());
                        getMarkers();
                    }
                }
            }
        );
    }else{
        showInfoMessage(warningIcon + "Enter a value then try again or double click on the map.", 0);
    }
}

// Advertising Map Tip Control  -------------------------------------------
function AdMapTipControl() {}

AdMapTipControl.prototype = new GControl();

AdMapTipControl.prototype.initialize = function(map)
{
    var container = document.createElement('div');
    container.className = 'MapTipControl';
    container.innerHTML = '<b>Tip:</b> Search for a location above or double click the map.';

    map.getContainer().appendChild(container);
    return container;
}

AdMapTipControl.prototype.getDefaultPosition = function()
{
    return new GControlPosition(G_ANCHOR_TOP_LEFT, new GSize(15, 95));
}

// Billboard cart -----------------------------------------------------------
function addToCart(idAdvertisementLocation, idAdvertisementType, sender)
{
    if(sender){
        // Disable the link so the user doesn't click again.
        sender.style.color = "#aaa";
        sender.style.textDecoration = "none";
        sender.style.cursor = "default";
        sender.onclick = "";
        sender.title = "Already added";
    }
    
    // Find the .net form controls and do a bit of a dodogy...
    var FormElements = document.getElementsByTagName("input");

    for(i=0;i<FormElements.length;i++)
    {
        if(FormElements[i].id){
            if(FormElements[i].id.indexOf("IdAdvertisementLocation") > 0){
                var AdLoc = FormElements[i];
            }
            if(FormElements[i].id.indexOf("IdAdvertisementType") > 0){
                var AdType = FormElements[i];
            }
            if(FormElements[i].id.indexOf("IdAdvertisementCombined") > 0){
                var AdCombined = FormElements[i];
            }
        }
    }
    
    AdLoc.value = idAdvertisementLocation;
    AdType.value = idAdvertisementType;
    AdCombined.value = AdLoc.value+AdType.value
    __doPostBack(AdCombined.id,"");
}

// Finally, the map itself ------------------------------------------------------------
   
Sys.Application.add_init(function(){

    var country = eval(arrayName);
        
    adMap = $create(Appian.MapBehavior,
    {'Latitude':country[1],
    'Longitude':country[2],
    'Zoom':country[4]},
    null,
    null,
    $get('advertising_map'));

    adMap.initialize();            
    adMap.loadMap();
    adMap._map.setMapType(G_NORMAL_MAP);
    
    adMap._map.enableDoubleClickZoom();
    
    GEvent.addListener(adMap._map, 'click', onMapClick);
    GEvent.addListener(adMap._map, 'dragend', getMarkers);
    GEvent.addListener(adMap._map, 'zoomend', getMarkers);
    
    //adMap._map.enableScrollWheelZoom();
                
    var zoomControl = new MapZoomControl();
    zoomControl.set_MapBehaviour(adMap);
    adMap._map.addControl(zoomControl);
                
    var mapTypeControl = new MapTypeControl();
    adMap._map.addControl(mapTypeControl);
    mapTypeControl.set_MapType('Map');
    
    adMap._map.addControl(new AdMapTipControl());
    
    getMarkers();
});
        