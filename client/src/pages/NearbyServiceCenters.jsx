import React, { useEffect, useState } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import { serviceService } from "../api/serviceService";
import { serviceCenterService } from "../api/serviceCenterService";

import Loading from "../Components/common/Loading";
import ErrorMessage from "../Components/common/ErrorMessage";

import markerIcon2x from "leaflet/dist/images/marker-icon-2x.png";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";

// Fix default icon paths for Leaflet when bundled
L.Icon.Default.mergeOptions({
  iconRetinaUrl: markerIcon2x,
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

const NearbyServiceCenters = () => {
  const [position, setPosition] = useState(null);
  const [serviceCenters, setServiceCenters] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [geoError, setGeoError] = useState(null);
  const [radiusKm, setRadiusKm] = useState(50);
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 50;
  const [pagination, setPagination] = useState(null);

  const [serviceType, setServiceType] = useState("");
  const [services, setServices] = useState([]);
  const [selectedServiceId, setSelectedServiceId] = useState("");

  useEffect(() => {
    if (!navigator.geolocation) {
      setGeoError("Geolocation is not supported by your browser.");
      setLoading(false);
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (pos) => {
        const coords = {
          lat: pos.coords.latitude,
          lng: pos.coords.longitude,
        };
        setPosition(coords);
        fetchNearby(coords, radiusKm, 1, selectedServiceId);
      },
      (err) => {
        console.error(err);
        setGeoError(
          "Unable to get your location. You can still see nearby centers on the default map."
        );
        // Default to a reasonable location (e.g., Cairo)
        const coords = { lat: 30.0444, lng: 31.2357 };
        setPosition(coords);
        fetchNearby(coords, radiusKm, 1, selectedServiceId);
      }
    );
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    const loadServices = async () => {
      try {
        const response = await serviceService.getAllServices({
          serviceType: serviceType || undefined,
          isActive: true,
          pageSize: 100,
        });
        setServices(response.data || []);
      } catch (err) {
        console.error("Failed to load services for filters", err);
      }
    };

    loadServices();
  }, [serviceType]);

  const fetchNearby = async (coords, radius, page = pageNumber, serviceId) => {
    try {
      setLoading(true);
      setError(null);
      const response = await serviceCenterService.getNearbyServiceCenters({
        latitude: coords.lat,
        longitude: coords.lng,
        radiusKm: radius,
        pageNumber: page,
        pageSize,
        serviceId: serviceId || undefined,
      });

      setServiceCenters(response.data || []);
      setPagination(response.pagination || null);
      setPageNumber(page);
    } catch (err) {
      setError(
        err.response?.data?.errors?.[0] ||
          "Failed to load nearby service centers"
      );
    } finally {
      setLoading(false);
    }
  };

  const handleRadiusChange = (e) => {
    const value = parseInt(e.target.value, 10) || 10;
    setRadiusKm(value);
    if (position) {
      fetchNearby(position, value, 1, selectedServiceId);
    }
  };

  const handleServiceTypeChange = (e) => {
    const value = e.target.value;
    setServiceType(value);
    setSelectedServiceId("");
    if (position) {
      fetchNearby(position, radiusKm, 1, undefined);
    }
  };

  const handleServiceChange = (e) => {
    const value = e.target.value;
    setSelectedServiceId(value);
    if (position) {
      fetchNearby(position, radiusKm, 1, value || undefined);
    }
  };

  const handlePageChange = (newPage) => {
    if (!pagination || !position) return;
    if (newPage < 1 || newPage > pagination.totalPages) return;
    fetchNearby(position, radiusKm, newPage, selectedServiceId);
  };

  if (!position && loading) {
    return <Loading message="Detecting your location..." />;
  }

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12">
          <h1 className="mb-2 all-services text-center">
            Nearby Service Centers
          </h1>
          <p className="text-white text-center mb-3">
            View service centers around your current location and click on a
            marker for details.
          </p>
          {geoError && (
            <div className="alert alert-warning mt-3">
              <i className="fas fa-exclamation-triangle me-2"></i>
              {geoError}
            </div>
          )}
        </div>
      </div>

      {error && (
        <ErrorMessage
          message={error}
          onRetry={() =>
            position && fetchNearby(position, radiusKm, pageNumber)
          }
        />
      )}

      <div className="row mb-4">
        <div className="col-md-3 mb-3">
          <label className="form-label">Search radius (km)</label>
          <input
            type="number"
            className="form-control"
            min="5"
            max="200"
            value={radiusKm}
            onChange={handleRadiusChange}
          />
        </div>
        <div className="col-md-3 mb-3">
          <label className="form-label">Service type</label>
          <select
            className="form-select"
            value={serviceType}
            onChange={handleServiceTypeChange}>
            <option value="">All types</option>
            <option value="Maintenance">Maintenance</option>
            <option value="SpareParts">Spare Parts</option>
          </select>
        </div>
        <div className="col-md-6 mb-3">
          <label className="form-label">Specific service</label>
          <select
            className="form-select"
            value={selectedServiceId}
            onChange={handleServiceChange}>
            <option value="">All services</option>
            {services.map((service) => (
              <option key={service.id} value={service.id}>
                {service.nameEn} ({service.serviceType})
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="row">
        <div className="col-12 mb-4" style={{ height: "400px" }}>
          {position && (
            <MapContainer
              center={[position.lat, position.lng]}
              zoom={12}
              style={{ height: "100%", width: "100%" }}>
              <TileLayer
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              />

              <Marker position={[position.lat, position.lng]}>
                <Popup>
                  <strong>You are here</strong>
                </Popup>
              </Marker>

              {serviceCenters.map((center) => (
                <Marker
                  key={center.id}
                  position={[center.latitude, center.longitude]}>
                  <Popup>
                    <div>
                      <h6 className="mb-1">{center.nameEn}</h6>
                      <p className="mb-1 small">
                        <i className="fas fa-city me-1"></i>
                        {center.city}
                      </p>
                      <p className="mb-1 small">
                        <i className="fas fa-phone me-1"></i>
                        {center.phoneNumber}
                      </p>
                      {center.distance != null && (
                        <p className="mb-1 small">
                          <i className="fas fa-route me-1"></i>
                          {center.distance.toFixed(1)} km away
                        </p>
                      )}
                      <a href={`/service-centers/${center.id}`}>
                        View details &amp; book
                      </a>
                    </div>
                  </Popup>
                </Marker>
              ))}
            </MapContainer>
          )}
        </div>
      </div>

      {pagination && (
        <div className="d-flex justify-content-between align-items-center mt-2">
          <span className="text-muted">
            Page {pagination.pageNumber} of {pagination.totalPages} (
            {pagination.totalCount} centers)
          </span>
          <div>
            <button
              type="button"
              className="btn btn-outline-secondary btn-sm me-2"
              onClick={() => handlePageChange(pagination.pageNumber - 1)}
              disabled={!pagination.hasPreviousPage}>
              Previous
            </button>
            <button
              type="button"
              className="btn btn-outline-secondary btn-sm"
              onClick={() => handlePageChange(pagination.pageNumber + 1)}
              disabled={!pagination.hasNextPage}>
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default NearbyServiceCenters;
