window.leafletInterop = {
  maps: {},

  createIcon: function (type) {
    const colors = {
      kritik: "#dc3545",
      orta: "#fd7e14",
      dusuk: "#198754",
      resource: "#0d6efd",
      offer: "#6f42c1",
      default: "#6c757d",
    };
    const color = colors[type] || colors["default"];
    return L.divIcon({
      className: "custom-pin",
      html: `<div style="background:${color};width:22px;height:22px;border-radius:50% 50% 50% 0;transform:rotate(-45deg);border:2px solid white;box-shadow:0 1px 4px rgba(0,0,0,0.4);"></div>`,
      iconSize: [22, 22],
      iconAnchor: [11, 22],
      popupAnchor: [0, -22],
    });
  },

  createLiveIcon: function () {
    return L.divIcon({
      className: "live-location-icon",
      html: `<div class="live-pulse"></div><div class="live-dot"></div>`,
      iconSize: [20, 20],
      iconAnchor: [10, 10],
    });
  },

  initMap: function (elementId, lat, lng, zoom, dotNetRef, clickable) {
    const map = L.map(elementId).setView([lat, lng], zoom);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: "&copy; OpenStreetMap katkıda bulunanlar",
    }).addTo(map);

    this.maps[elementId] = { map: map, markers: [], liveMarker: null };

    if (clickable && dotNetRef) {
      map.on("click", function (e) {
        dotNetRef.invokeMethodAsync("OnMapClicked", e.latlng.lat, e.latlng.lng);
      });
    }
  },

  
  addMarker: function (elementId, lat, lng, popupText, iconType) {
    const entry = this.maps[elementId];
    if (!entry) return;

    const marker = L.marker([lat, lng], {
      icon: this.createIcon(iconType),
    }).addTo(entry.map);
    if (popupText) marker.bindPopup(popupText);
    entry.markers.push(marker);
  },

  clearMarkers: function (elementId) {
    const entry = this.maps[elementId];
    if (!entry) return;

    entry.markers.forEach((m) => entry.map.removeLayer(m));
    entry.markers = [];
  },

  setSingleMarker: function (elementId, lat, lng) {
    const entry = this.maps[elementId];
    if (!entry) return;

    this.clearMarkers(elementId);
    this.addMarker(elementId, lat, lng, null, "default");
  },

  setLiveLocationMarker: function (elementId, lat, lng) {
    const entry = this.maps[elementId];
    if (!entry) return;

    if (entry.liveMarker) {
      entry.liveMarker.setLatLng([lat, lng]);
    } else {
      entry.liveMarker = L.marker([lat, lng], {
        icon: this.createLiveIcon(),
        zIndexOffset: 1000,
      })
        .addTo(entry.map)
        .bindPopup("Senin canlı konumun");
    }
  },

  getCurrentPosition: function () {
    return new Promise((resolve, reject) => {
      if (!navigator.geolocation) {
        reject("Bu tarayıcı konum servisini desteklemiyor.");
        return;
      }
      navigator.geolocation.getCurrentPosition(
        (pos) => resolve([pos.coords.latitude, pos.coords.longitude]),
        (err) => reject(err.message),
        { enableHighAccuracy: true, timeout: 10000 },
      );
    });
  },

      map_setView: function (elementId, lat, lng) {
        const entry = this.maps[elementId];
        if (entry) entry.map.setView([lat, lng], 15);
    },
  destroyMap: function (elementId) {
    const entry = this.maps[elementId];
    if (entry) {
      entry.map.remove();
      delete this.maps[elementId];
    }
  },
  
};
