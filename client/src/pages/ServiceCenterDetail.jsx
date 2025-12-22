import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { serviceCenterService } from '../api/serviceCenterService';
import { serviceService } from '../api/serviceService';
import { vehicleService } from '../api/vehicleService';
import { bookingService } from '../api/bookingService';
import Loading from '../components/common/Loading';
import ErrorMessage from '../components/common/ErrorMessage';

const ServiceCenterDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [serviceCenter, setServiceCenter] = useState(null);
  const [services, setServices] = useState([]);
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [bookingForm, setBookingForm] = useState({
    vehicleId: '',
    serviceId: '',
    bookingDate: '',
    bookingTime: '',
    customerNotes: '',
  });
  const [bookingLoading, setBookingLoading] = useState(false);
  const [bookingSuccess, setBookingSuccess] = useState(false);

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [centerRes, servicesRes, vehiclesRes] = await Promise.all([
        serviceCenterService.getServiceCenterById(id),
        serviceService.getServicesByServiceCenter(id),
        vehicleService.getMyVehicles(),
      ]);

      setServiceCenter(centerRes.data);
      setServices(servicesRes.data || []);
      setVehicles(vehiclesRes.data || []);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const handleBookingSubmit = async (e) => {
    e.preventDefault();
    setBookingLoading(true);
    setError(null);

    try {
      await bookingService.createBooking({
        ...bookingForm,
        serviceCenterId: parseInt(id),
        vehicleId: parseInt(bookingForm.vehicleId),
        serviceId: parseInt(bookingForm.serviceId),
      });

      setBookingSuccess(true);
      setTimeout(() => {
        navigate('/bookings');
      }, 2000);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to create booking');
    } finally {
      setBookingLoading(false);
    }
  };

  if (loading) return <Loading message="Loading service center..." />;
  if (!serviceCenter) return <ErrorMessage message="Service center not found" />;

  return (
    <div className="container mt-5 pt-4">
      {/* Service Center Info */}
      <div className="row mb-4">
        <div className="col-12">
          <div className="card shadow-sm">
            <div className="card-body">
              <h1 className="mb-3">{serviceCenter.nameEn}</h1>
              <div className="row">
                <div className="col-md-6">
                  <p className="mb-2">
                    <i className="fas fa-map-marker-alt text-primary me-2"></i>
                    <strong>Address:</strong> {serviceCenter.addressEn}
                  </p>
                  <p className="mb-2">
                    <i className="fas fa-city text-primary me-2"></i>
                    <strong>City:</strong> {serviceCenter.city}
                  </p>
                </div>
                <div className="col-md-6">
                  <p className="mb-2">
                    <i className="fas fa-phone text-primary me-2"></i>
                    <strong>Phone:</strong> {serviceCenter.phoneNumber}
                  </p>
                  <p className="mb-2">
                    <i className="fas fa-clock text-primary me-2"></i>
                    <strong>Hours:</strong> {serviceCenter.workingHours}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Available Services */}
      <div className="row mb-4">
        <div className="col-12">
          <h3 className="mb-3">Available Services</h3>
          <div className="row g-3">
            {services.map((service) => (
              <div key={service.serviceId} className="col-md-6">
                <div className="card h-100">
                  <div className="card-body">
                    <h5 className="card-title">{service.nameEn}</h5>
                    <p className="card-text text-muted">{service.descriptionEn}</p>
                    <p className="mb-0">
                      <strong>Price:</strong> {service.customPrice || service.basePrice} EGP
                    </p>
                    <p className="text-muted small">
                      Duration: ~{service.estimatedDurationMinutes} minutes
                    </p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Booking Form */}
      <div className="row">
        <div className="col-12">
          <div className="card shadow-sm">
            <div className="card-body">
              <h4 className="mb-4">Book an Appointment</h4>

              {error && <ErrorMessage message={error} />}

              {bookingSuccess && (
                <div className="alert alert-success">
                  <i className="fas fa-check-circle me-2"></i>
                  Booking created successfully! Redirecting...
                </div>
              )}

              {vehicles.length === 0 ? (
                <div className="alert alert-warning">
                  <i className="fas fa-exclamation-triangle me-2"></i>
                  You need to add a vehicle before booking. 
                  <a href="/vehicles" className="alert-link ms-2">Add Vehicle</a>
                </div>
              ) : (
                <form onSubmit={handleBookingSubmit}>
                  <div className="row g-3">
                    <div className="col-md-6">
                      <label className="form-label">Select Vehicle</label>
                      <select
                        className="form-select"
                        value={bookingForm.vehicleId}
                        onChange={(e) => setBookingForm({ ...bookingForm, vehicleId: e.target.value })}
                        required
                      >
                        <option value="">Choose vehicle...</option>
                        {vehicles.map((vehicle) => (
                          <option key={vehicle.vehicleId} value={vehicle.vehicleId}>
                            {vehicle.brand} {vehicle.model} ({vehicle.plateNumber})
                          </option>
                        ))}
                      </select>
                    </div>

                    <div className="col-md-6">
                      <label className="form-label">Select Service</label>
                      <select
                        className="form-select"
                        value={bookingForm.serviceId}
                        onChange={(e) => setBookingForm({ ...bookingForm, serviceId: e.target.value })}
                        required
                      >
                        <option value="">Choose service...</option>
                        {services.map((service) => (
                          <option key={service.serviceId} value={service.serviceId}>
                            {service.nameEn} - {service.customPrice || service.basePrice} EGP
                          </option>
                        ))}
                      </select>
                    </div>

                    <div className="col-md-6">
                      <label className="form-label">Booking Date</label>
                      <input
                        type="date"
                        className="form-control"
                        value={bookingForm.bookingDate}
                        onChange={(e) => setBookingForm({ ...bookingForm, bookingDate: e.target.value })}
                        min={new Date().toISOString().split('T')[0]}
                        required
                      />
                    </div>

                    <div className="col-md-6">
                      <label className="form-label">Booking Time</label>
                      <input
                        type="time"
                        className="form-control"
                        value={bookingForm.bookingTime}
                        onChange={(e) => setBookingForm({ ...bookingForm, bookingTime: e.target.value })}
                        required
                      />
                    </div>

                    <div className="col-12">
                      <label className="form-label">Notes (Optional)</label>
                      <textarea
                        className="form-control"
                        rows="3"
                        value={bookingForm.customerNotes}
                        onChange={(e) => setBookingForm({ ...bookingForm, customerNotes: e.target.value })}
                        placeholder="Any special requests or notes..."
                      ></textarea>
                    </div>

                    <div className="col-12">
                      <button
                        type="submit"
                        className="btn btn-primary btn-lg"
                        disabled={bookingLoading || bookingSuccess}
                      >
                        {bookingLoading ? (
                          <>
                            <span className="spinner-border spinner-border-sm me-2"></span>
                            Booking...
                          </>
                        ) : (
                          <>
                            <i className="fas fa-calendar-check me-2"></i>
                            Book Appointment
                          </>
                        )}
                      </button>
                    </div>
                  </div>
                </form>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ServiceCenterDetail;
