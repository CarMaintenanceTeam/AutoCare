import React, { useState, useEffect } from 'react';
import { bookingService } from '../api/bookingService';
import Loading from '../components/common/Loading';
import ErrorMessage from '../components/common/ErrorMessage';

const Bookings = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [cancellingId, setCancellingId] = useState(null);

  useEffect(() => {
    fetchBookings();
  }, []);

  const fetchBookings = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await bookingService.getMyBookings();
      setBookings(response.data || []);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to load bookings');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (id) => {
    const reason = prompt('Please enter cancellation reason:');
    if (!reason) return;

    try {
      setCancellingId(id);
      await bookingService.cancelBooking(id, reason);
      await fetchBookings();
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to cancel booking');
    } finally {
      setCancellingId(null);
    }
  };

  const getStatusBadge = (status) => {
    const statusClasses = {
      Pending: 'bg-warning',
      Confirmed: 'bg-info',
      InProgress: 'bg-primary',
      Completed: 'bg-success',
      Cancelled: 'bg-danger',
    };
    return statusClasses[status] || 'bg-secondary';
  };

  if (loading) return <Loading message="Loading bookings..." />;

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12">
          <h1 className="mb-2">My Bookings</h1>
          <p className="text-muted">View and manage your service appointments</p>
        </div>
      </div>

      {error && <ErrorMessage message={error} onRetry={fetchBookings} />}

      {bookings.length === 0 ? (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          No bookings yet. Browse service centers to make your first appointment!
        </div>
      ) : (
        <div className="row g-4">
          {bookings.map((booking) => (
            <div key={booking.bookingId} className="col-lg-6">
              <div className="card shadow-sm h-100">
                <div className="card-body">
                  <div className="d-flex justify-content-between align-items-start mb-3">
                    <h5 className="card-title mb-0">
                      Booking #{booking.bookingNumber}
                    </h5>
                    <span className={`badge ${getStatusBadge(booking.status)}`}>
                      {booking.status}
                    </span>
                  </div>

                  <div className="mb-3">
                    <h6 className="text-muted mb-2">Service Center</h6>
                    <p className="mb-1">{booking.serviceCenterName}</p>
                    <p className="text-muted small mb-0">
                      <i className="fas fa-map-marker-alt me-1"></i>
                      {booking.serviceCenterAddress}
                    </p>
                  </div>

                  <div className="row g-3 mb-3">
                    <div className="col-6">
                      <h6 className="text-muted mb-1">Service</h6>
                      <p className="mb-0">{booking.serviceName}</p>
                    </div>
                    <div className="col-6">
                      <h6 className="text-muted mb-1">Vehicle</h6>
                      <p className="mb-0">
                        {booking.vehicleBrand} {booking.vehicleModel}
                        <br />
                        <span className="text-muted small">{booking.vehiclePlateNumber}</span>
                      </p>
                    </div>
                  </div>

                  <div className="mb-3">
                    <h6 className="text-muted mb-1">Appointment</h6>
                    <p className="mb-0">
                      <i className="fas fa-calendar me-2"></i>
                      {new Date(booking.bookingDate).toLocaleDateString('en-US', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                      })}
                    </p>
                    <p className="mb-0">
                      <i className="fas fa-clock me-2"></i>
                      {booking.bookingTime}
                    </p>
                  </div>

                  {booking.customerNotes && (
                    <div className="mb-3">
                      <h6 className="text-muted mb-1">Notes</h6>
                      <p className="mb-0 small">{booking.customerNotes}</p>
                    </div>
                  )}

                  {booking.status === 'Pending' && (
                    <button
                      className="btn btn-outline-danger btn-sm"
                      onClick={() => handleCancel(booking.bookingId)}
                      disabled={cancellingId === booking.bookingId}
                    >
                      {cancellingId === booking.bookingId ? (
                        <>
                          <span className="spinner-border spinner-border-sm me-2"></span>
                          Cancelling...
                        </>
                      ) : (
                        <>
                          <i className="fas fa-times me-2"></i>
                          Cancel Booking
                        </>
                      )}
                    </button>
                  )}

                  {booking.status === 'Cancelled' && booking.cancellationReason && (
                    <div className="alert alert-danger mb-0 mt-2 small">
                      <strong>Cancelled:</strong> {booking.cancellationReason}
                    </div>
                  )}

                  <div className="mt-3 pt-3 border-top">
                    <small className="text-muted">
                      <i className="fas fa-info-circle me-1"></i>
                      Created: {new Date(booking.createdAt).toLocaleString()}
                    </small>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Bookings;
