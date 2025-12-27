import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { serviceCenterService } from "../api/serviceCenterService";
import Loading from "../Components/common/Loading";
import ErrorMessage from "../Components/common/ErrorMessage";
import "./style.css";

const ServiceCenters = () => {
  const [serviceCenters, setServiceCenters] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    city: "",
    searchTerm: "",
  });
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 9;
  const [pagination, setPagination] = useState(null);

  useEffect(() => {
    fetchServiceCenters(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchServiceCenters = async (page = pageNumber) => {
    try {
      setLoading(true);
      setError(null);
      const response = await serviceCenterService.getAllServiceCenters({
        city: filters.city || undefined,
        pageNumber: page,
        pageSize,
      });

      let centers = response.data || [];

      // Client-side search by name since backend doesn't support searchTerm
      if (filters.searchTerm) {
        const term = filters.searchTerm.toLowerCase();
        centers = centers.filter(
          (c) =>
            c.nameEn?.toLowerCase().includes(term) ||
            c.nameAr?.toLowerCase().includes(term)
        );
      }

      setServiceCenters(centers);
      setPagination(response.pagination || null);
      setPageNumber(page);
    } catch (err) {
      setError(
        err.response?.data?.errors?.[0] || "Failed to load service centers"
      );
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    fetchServiceCenters(1);
  };

  const handlePageChange = (newPage) => {
    if (!pagination) return;
    if (newPage < 1 || newPage > pagination.totalPages) return;
    fetchServiceCenters(newPage);
  };

  if (loading) return <Loading message="Loading service centers..." />;

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12">
          <h1 className="mb-3 text-center all-services">Service Centers</h1>
          <p className="text-white text-center">
            Find the best service centers near you
          </p>
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
                      className="form-control "
                      placeholder="Search by name... "
                      value={filters.searchTerm}
                      onChange={(e) =>
                        setFilters({ ...filters, searchTerm: e.target.value })
                      }
                    />
                  </div>
                  <div className="col-md-4">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Filter by city..."
                      value={filters.city}
                      onChange={(e) =>
                        setFilters({ ...filters, city: e.target.value })
                      }
                    />
                  </div>
                  <div className="col-md-3">
                    <button type="submit" className="btn  w-100 search-button">
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
            <div key={center.id} className="col-md-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-body">
                  <h5 className="card-title">{center.nameEn}</h5>
                  <p className="card-text text-muted small mb-1">
                    <i className="fas fa-city me-2"></i>
                    {center.city}
                  </p>
                  <p className="card-text text-white small mb-2">
                    <i className="fas fa-phone me-2"></i>
                    {center.phoneNumber}
                  </p>
                  {center.workingHours && (
                    <p className="card-text text-white small mb-2">
                      <i className="fas fa-clock me-2"></i>
                      {center.workingHours}
                    </p>
                  )}
                  {center.servicesCount !== undefined && (
                    <p className="card-text text-white small mb-2">
                      <i className="fas fa-wrench me-2"></i>
                      {center.servicesCount} services available
                    </p>
                  )}
                  {center.distance != null && (
                    <p className="card-text text-white small mb-2">
                      <i className="fas fa-route me-2"></i>
                      {center.distance.toFixed(1)} km away
                    </p>
                  )}
                  <Link
                    to={`/service-centers/${center.id}`}
                    className="btn search-button btn-sm mt-3">
                    View Services &amp; Book
                  </Link>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {pagination && (
        <div className="d-flex justify-content-between align-items-center mt-4">
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

export default ServiceCenters;
