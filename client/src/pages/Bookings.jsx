import React, { useState, useEffect } from "react";
import { bookingService } from "../api/bookingService";
import Loading from "../Components/common/Loading";
import ErrorMessage from "../Components/common/ErrorMessage";
import "./style.css";

const Bookings = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [cancellingId, setCancellingId] = useState(null);
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;
  const [pagination, setPagination] = useState(null);

  useEffect(() => {
    fetchBookings(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchBookings = async (page = pageNumber) => {
    try {
      setLoading(true);
      setError(null);
      const response = await bookingService.getMyBookings({
        pageNumber: page,
        pageSize,
        sortBy: "Date",
        sortOrder: "Desc",
      });
      setBookings(response.data || []);
      setPagination(response.pagination || null);
      setPageNumber(page);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || "Failed to load bookings");
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (id) => {
    const reason = prompt("Please enter cancellation reason:");
    if (!reason) return;

    try {
      setCancellingId(id);
      await bookingService.cancelBooking(id, reason);
      await fetchBookings(pageNumber);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || "Failed to cancel booking");
    } finally {
      setCancellingId(null);
    }
  };

  const getStatusBadge = (status) => {
    const statusClasses = {
      Pending: "bg-warning",
      Confirmed: "bg-info",
      InProgress: "bg-primary",
      Completed: "bg-success",
      Cancelled: "bg-danger",
    };
    return statusClasses[status] || "bg-secondary";
  };

  const handlePageChange = (newPage) => {
    if (!pagination) return;
    if (newPage < 1 || newPage > pagination.totalPages) return;
    fetchBookings(newPage);
  };

  if (loading) return <Loading message="Loading bookings..." />;

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12">
          <h1 className="mb-2 text-white">My Bookings</h1>
          <p className="text-white">
            View and manage your service appointments
          </p>
        </div>
      </div>

      {error && <ErrorMessage message={error} onRetry={fetchBookings} />}

      {bookings.length === 0 ? (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          No bookings yet. Browse service centers to make your first
          appointment!
        </div>
      ) : (
        <>
          <div className="row g-4">
            {bookings.map((booking) => (
              <div key={booking.id} className="col-lg-6">
                <div className="card shadow-sm h-100">
                  <div className="card-body">
                    <div className="d-flex justify-content-between align-items-start mb-3">
                      <h5 className="card-title mb-0 ">
                        Booking #{booking.bookingNumber}
                      </h5>
                      <span
                        className={`badge ${getStatusBadge(booking.status)}`}>
                        {booking.status}
                      </span>
                    </div>

                    <div className="mb-3">
                      <h6 className="text-white mb-2">Service Center</h6>
                      <p className="mb-1">{booking.serviceCenterName}</p>
                    </div>

                    <div className="row g-3 mb-3">
                      <div className="col-6">
                        <h6 className="text-white mb-1">Service</h6>
                        <p className="mb-0">{booking.serviceName}</p>
                      </div>
                      <div className="col-6">
                        <h6 className="text-white mb-1">Vehicle</h6>
                        <p className="mb-0">{booking.vehicleInfo}</p>
                      </div>
                    </div>

                    <div className="mb-3">
                      <h6 className="text-white mb-1">Appointment</h6>
                      <p className="mb-0">
                        <i className="fas fa-calendar me-2"></i>
                        {new Date(booking.bookingDate).toLocaleDateString(
                          "en-US",
                          {
                            weekday: "long",
                            year: "numeric",
                            month: "long",
                            day: "numeric",
                          }
                        )}
                      </p>
                      <p className="mb-0">
                        <i className="fas fa-clock me-2"></i>
                        {booking.bookingTime}
                      </p>
                    </div>

                    {booking.status === "Pending" && (
                      <button
                        className="btn btn-outline-danger btn-sm"
                        onClick={() => handleCancel(booking.id)}
                        disabled={cancellingId === booking.id}>
                        {cancellingId === booking.id ? (
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

          {pagination && (
            <div className="d-flex justify-content-between align-items-center mt-4">
              <span className="text-muted">
                Page {pagination.pageNumber} of {pagination.totalPages} (
                {pagination.totalCount} bookings)
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
        </>
      )}
    </div>
  );
};

export default Bookings;
