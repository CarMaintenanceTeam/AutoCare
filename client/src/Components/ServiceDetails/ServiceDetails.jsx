import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { serviceService } from '../../api/serviceService';
import Loading from '../common/Loading';
import ErrorMessage from '../common/ErrorMessage';

const ServiceDetails = () => {
  const { id } = useParams();
  const [service, setService] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchService = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await serviceService.getServiceById(id, true);
        setService(response.data);
      } catch (err) {
        setError(err.response?.data?.errors?.[0] || 'Failed to load service details');
      } finally {
        setLoading(false);
      }
    };

    fetchService();
  }, [id]);

  if (loading) return <Loading message="Loading service details..." />;
  if (!service) return <ErrorMessage message={error || 'Service not found'} />;

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12">
          <h1 className="mb-2">{service.nameEn}</h1>
          {service.descriptionEn && (
            <p className="text-muted">{service.descriptionEn}</p>
          )}
        </div>
      </div>

      <div className="row mb-4">
        <div className="col-md-6">
          <div className="card shadow-sm h-100">
            <div className="card-body">
              <h5 className="card-title mb-3">Service Information</h5>
              <p className="mb-2">
                <strong>Type:</strong> {service.serviceType}
              </p>
              <p className="mb-2">
                <strong>Base Price:</strong> {service.basePrice} EGP
              </p>
              <p className="mb-2">
                <strong>Estimated Duration:</strong> {service.estimatedDurationMinutes} minutes
              </p>
              <p className="mb-0">
                <strong>Status:</strong>{' '}
                {service.isActive ? (
                  <span className="badge bg-success">Active</span>
                ) : (
                  <span className="badge bg-secondary">Inactive</span>
                )}
              </p>
            </div>
          </div>
        </div>

        <div className="col-md-6">
          <div className="card shadow-sm h-100">
            <div className="card-body">
              <h5 className="card-title mb-3">Where You Can Get This Service</h5>

              {!service.serviceCenters || service.serviceCenters.length === 0 ? (
                <p className="text-muted mb-0">
                  This service is not currently available at any service centers.
                </p>
              ) : (
                <ul className="list-group list-group-flush">
                  {service.serviceCenters.map((center) => (
                    <li key={center.serviceCenterId} className="list-group-item">
                      <div className="d-flex justify-content-between align-items-center">
                        <div>
                          <h6 className="mb-1">{center.nameEn}</h6>
                          <p className="mb-0 text-muted small">
                            <i className="fas fa-city me-1"></i>
                            {center.city} · {center.phoneNumber}
                          </p>
                        </div>
                        <div className="text-end">
                          <p className="mb-1">
                            <strong>{center.price} EGP</strong>
                          </p>
                          <Link
                            to={`/service-centers/${center.serviceCenterId}`}
                            className="btn btn-sm btn-primary"
                          >
                            View Center &amp; Book
                          </Link>
                        </div>
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </div>
        </div>
      </div>

      <div className="mt-3">
        <Link to="/services" className="btn btn-outline-secondary">
          &larr; Back to services list
        </Link>
      </div>
    </div>
  );
};

export default ServiceDetails;
