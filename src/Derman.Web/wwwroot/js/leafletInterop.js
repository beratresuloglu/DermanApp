window.leafletInterop = {
    maps: {},

    initMap: function (elementId, lat, lng, zoom, dotNetRef, clickable) {
        const map = L.map(elementId).setView([lat, lng], zoom);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap katkıda bulunanlar'
        }).addTo(map);

        this.maps[elementId] = { map: map, markers: [] };

        if (clickable && dotNetRef) {
            map.on('click', function (e) {
                dotNetRef.invokeMethodAsync('OnMapClicked', e.latlng.lat, e.latlng.lng);
            });
        }
    },

    addMarker: function (elementId, lat, lng, popupText, colorClass) {
        const entry = this.maps[elementId];
        if (!entry) return;

        const marker = L.marker([lat, lng]).addTo(entry.map);
        if (popupText) {
            marker.bindPopup(popupText);
        }
        entry.markers.push(marker);
    },

    clearMarkers: function (elementId) {
        const entry = this.maps[elementId];
        if (!entry) return;

        entry.markers.forEach(m => entry.map.removeLayer(m));
        entry.markers = [];
    },

    setSingleMarker: function (elementId, lat, lng) {
        const entry = this.maps[elementId];
        if (!entry) return;

        this.clearMarkers(elementId);
        this.addMarker(elementId, lat, lng, null, null);
    },

    destroyMap: function (elementId) {
        const entry = this.maps[elementId];
        if (entry) {
            entry.map.remove();
            delete this.maps[elementId];
        }
    }
};