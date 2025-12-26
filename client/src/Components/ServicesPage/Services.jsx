import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { serviceService } from '../../api/serviceService';
import Loading from '../common/Loading';
import ErrorMessage from '../common/ErrorMessage';

const Services = () => {
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    serviceType: '',
  });
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 9;
  const [pagination, setPagination] = useState(null);

  useEffect(() => {
    fetchServices(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchServices = async (page = pageNumber) => {
    try {
      setLoading(true);
      setError(null);
      const response = await serviceService.getAllServices({
        serviceType: filters.serviceType || undefined,
        pageNumber: page,
        pageSize,
      });

      setServices(response.data || []);
      setPagination(response.pagination || null);
      setPageNumber(page);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to load services');
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (e) => {
    setFilters({ ...filters, serviceType: e.target.value });
  };

  const handleFilterSubmit = (e) => {
    e.preventDefault();
    fetchServices(1);
  };

  const handlePageChange = (newPage) => {
    if (!pagination) return;
    if (newPage < 1 || newPage > pagination.totalPages) return;
    fetchServices(newPage);
  };

  if (loading) return <Loading message="Loading services..." />;

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12 d-flex justify-content-between align-items-center">
          <div>
            <h1 className="mb-2">All Services</h1>
            <p className="text-muted mb-0">
              Browse available maintenance and spare parts services
            </p>
          </div>

          <form className="d-flex" onSubmit={handleFilterSubmit}>
            <select
              className="form-select me-2"
              value={filters.serviceType}
              onChange={handleFilterChange}
            >
              <option value="">All Types</option>
              <option value="Maintenance">Maintenance</option>
              <option value="SpareParts">Spare Parts</option>
            </select>
            <button type="submit" className="btn btn-primary">
              <i className="fas fa-filter me-2"></i>Filter
            </button>
          </form>
        </div>
      </div>

      {error && <ErrorMessage message={error} onRetry={fetchServices} />}

      <div className="row g-4">
        {services.length === 0 ? (
          <div className="col-12">
            <div className="alert alert-info">
              <i className="fas fa-info-circle me-2"></i>
              No services found. Try changing the filters.
            </div>
          </div>
        ) : (
          services.map((service) => (
            <div key={service.id} className="col-md-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-body d-flex flex-column">
                  <h5 className="card-title">{service.nameEn}</h5>
                  <p className="text-muted small mb-2">
                    <i className="fas fa-tag me-2"></i>
                    {service.serviceType}
                  </p>
                  <p className="mb-1">
                    <strong>Price:</strong> {service.basePrice} EGP
                  </p>
                  <p className="text-muted small mb-3">
                    <i className="fas fa-clock me-2"></i>
                    Approx. {service.estimatedDurationMinutes} minutes
                  </p>
                  <p className="text-muted small mb-3">
                    Available at {service.availableAt} service center(s)
                  </p>

                  <div className="mt-auto d-flex justify-content-between align-items-center">
                    <Link
                      to={`/services/${service.id}`}
                      className="btn btn-outline-primary btn-sm"
                    >
                      View Details
                    </Link>
                    <Link
                      to="/service-centers"
                      className="btn btn-link btn-sm text-decoration-none"
                    >
                      Find Service Centers
                    </Link>
                  </div>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {pagination && (
        <div className="d-flex justify-content-between align-items-center mt-4">
          <span className="text-muted">
            Page {pagination.pageNumber} of {pagination.totalPages} ({pagination.totalCount}{' '}
            services)
          </span>
          <div>
            <button
              type="button"
              className="btn btn-outline-secondary btn-sm me-2"
              onClick={() => handlePageChange(pagination.pageNumber - 1)}
              disabled={!pagination.hasPreviousPage}
            >
              Previous
            </button>
            <button
              type="button"
              className="btn btn-outline-secondary btn-sm"
              onClick={() => handlePageChange(pagination.pageNumber + 1)}
              disabled={!pagination.hasNextPage}
            >
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default Services;
