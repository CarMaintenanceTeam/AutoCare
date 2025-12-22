import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { serviceCenterService } from '../api/serviceCenterService';
import Loading from '../components/common/Loading';
import ErrorMessage from '../components/common/ErrorMessage';

const ServiceCenters = () => {
  const [serviceCenters, setServiceCenters] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    city: '',
    searchTerm: '',
  });

  useEffect(() => {
    fetchServiceCenters();
  }, []);

  const fetchServiceCenters = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await serviceCenterService.getAllServiceCenters(filters);
      setServiceCenters(response.data || []);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to load service centers');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    fetchServiceCenters();
  };

  if (loading) return <Loading message="Loading service centers..." />;

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12">
          <h1 className="mb-3">Service Centers</h1>
          <p className="text-muted">Find the best service centers near you</p>
        </div>
      </div>

      {/* Search Filters */}
      <div className="row mb-4">
        <div className="col-12">
          <div className="card shadow-sm">
            <div className="card-body">
              <form onSubmit={handleSearch}>
                <div className="row g-3">
                  <div className="col-md-5">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Search by name..."
                      value={filters.searchTerm}
                      onChange={(e) => setFilters({ ...filters, searchTerm: e.target.value })}
                    />
                  </div>
                  <div className="col-md-4">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Filter by city..."
                      value={filters.city}
                      onChange={(e) => setFilters({ ...filters, city: e.target.value })}
                    />
                  </div>
                  <div className="col-md-3">
                    <button type="submit" className="btn btn-primary w-100">
                      <i className="fas fa-search me-2"></i>Search
                    </button>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>

      {error && <ErrorMessage message={error} onRetry={fetchServiceCenters} />}

      {/* Service Centers Grid */}
      <div className="row g-4">
        {serviceCenters.length === 0 ? (
          <div className="col-12">
            <div className="alert alert-info">
              <i className="fas fa-info-circle me-2"></i>
              No service centers found. Try adjusting your search filters.
            </div>
          </div>
        ) : (
          serviceCenters.map((center) => (
            <div key={center.serviceCenterId} className="col-md-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-body">
                  <h5 className="card-title">{center.nameEn}</h5>
                  <p className="card-text text-muted small">
                    <i className="fas fa-map-marker-alt me-2"></i>
                    {center.addressEn}
                  </p>
                  <p className="card-text text-muted small">
                    <i className="fas fa-city me-2"></i>
                    {center.city}
                  </p>
                  <p className="card-text text-muted small">
                    <i className="fas fa-phone me-2"></i>
                    {center.phoneNumber}
                  </p>
                  <p className="card-text text-muted small">
                    <i className="fas fa-clock me-2"></i>
                    {center.workingHours}
                  </p>
                  <Link
                    to={`/service-centers/${center.serviceCenterId}`}
                    className="btn btn-primary btn-sm mt-2"
                  >
                    View Services & Book
                  </Link>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default ServiceCenters;
