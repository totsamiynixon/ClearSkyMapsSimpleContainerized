jQuery(function ($) {
    window.CSM_Admin.addModule("Sensors", function (options) {
        var hub = {
            instance: null,
            isActive: false
        };
        var map = null;
        var sensors = options.sensors;
        var markers = [];
        initHub();
        ymaps.ready(function () {
            initMap();
            initMarkers();
        });

        //HUB
        function initHub() {
            hub.instance = new signalR.HubConnectionBuilder()
                .withUrl("/adminstatic")
                .configureLogging(signalR.LogLevel.Information)
                .build();
            hub.instance.on("DispatchReading", function (readingModel) {
                var sensor = sensors.find(function (sensor) {
                    return sensor.id == readingModel.sensorId;
                });
                if (!sensor) {
                    return;
                }
                sensor.pollutionLevel = readingModel.pollutionLevel;
                updateMarker(sensor);
            });
            hub.instance.start().then(function () {
                hub.isActive = true;
                //$.connection.hub.disconnected(function () {
                //    if (!hub.isActive) {
                //        return;
                //    }
                //    setTimeout(function () {
                //        $.connection.hub.start();
                //    }, 5000);
                //});
            });
        }
        //MAP
        function initMap() {
            map = new ymaps.Map("map", {
                center: [53.904502, 27.561261],
                zoom: 12,
                //controls: ["zoomControl"]
            },
                {
                    searchControlProvider: 'yandex#search',
                    //restrictMapArea: true
                });
        }
        function initMarkers() {
            sensors.forEach(function (sensor, index, arrya) {
                var marker = {
                    sensorId: sensor.id,
                    value: createMarker(sensor)
                };
                var dropdown = $("#mapDropdownTemplate");
                dropdown.click(function (e) {
                    e.stopPropagation();
                });
                $(document).click(function () {
                    if (dropdown.css("display") == 'block')
                        dropdown.hide();
                });
                marker.value.events.add('click', function (e) {
                    e.originalEvent.domEvent.originalEvent.stopPropagation();
                    var position = e.get("domEvent").get("position");
                    dropdown.css({ top: position[1] - dropdown.height() / 2, left: position[0] - dropdown.width(), position: 'absolute' });
                    dropdown.find("[data-href]").each(function () {
                        $(this).attr("href", $(this).data("href") + "?sensorId=" + sensor.id);
                    });
                    dropdown.show();
                });
                markers.push(marker);
                map.geoObjects.add(marker.value);
            })
        }
        function createMarker(sensor) {
            var marker = new ymaps.Circle([
                [sensor.latitude, sensor.longitude],
                1000
            ], {
                    hintContent: sensor.isVisible ? ("Уровень загрязнения " + sensor.pollutionLevel) : ""
                }, {
                    draggable: false,
                    fillColor: getFillColor(sensor.pollutionLevel, sensor.isVisible),
                    strokeColor: getStrokeColor(sensor.pollutionLevel, sensor.isVisible),
                    strokeOpacity: 0.8,
                    fillOpacity: 0.6,
                    strokeWidth: 3
                });
            return marker;
        }
        function updateMarker(sensor) {
            var marker = markers.find(function (marker) {
                return marker.sensorId == sensor.id;
            });
            if (marker == null) {
                return;
            }
            marker.value.properties.set({ hintContent: "Уровень загрязнения " + sensor.pollutionLevel });
            marker.value.options.set({ fillColor: getFillColor(sensor.pollutionLevel, sensor.isVisible), strokeColor: getStrokeColor(sensor.pollutionLevel, sensor.isVisible) });
        }
        function getFillColor(pollutionLevel, isVisible) {
            if (!isVisible) {
                return "#c2c2c2";
            }
            switch (pollutionLevel) {
                case 0:
                    return "#1adb2d";
                case 1:
                    return "#db971a";
                case 2:
                    return "#e20000"
            }
            return null;
        }
        function getStrokeColor(pollutionLevel, isVisible) {
            if (!isVisible) {
                return "#000";
            }
            switch (pollutionLevel) {
                case 0:
                    return "#106319";
                case 1:
                    return "#8c5e09";
                case 2:
                    return "#770707"
            }
            return null;
        }
    })
});