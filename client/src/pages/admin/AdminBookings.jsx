import React, { useEffect, useState } from "react";
import { bookingService } from "../../api/bookingService";
import { adminBookingService } from "../../api/adminBookingService";
import { useAuth } from "../../context/AuthContext";
import Loading from "../../Components/common/Loading";
import ErrorMessage from "../../Components/common/ErrorMessage";

const AdminBookings = () => {
  const { user } = useAuth();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [statusFilter, setStatusFilter] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;
  const [pagination, setPagination] = useState(null);

  const [selectedBooking, setSelectedBooking] = useState(null);
  const [detailsLoading, setDetailsLoading] = useState(false);
  const [detailsError, setDetailsError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [actionLoading, setActionLoading] = useState(false);

  const isAdminLike =
    user?.userType === "Admin" || user?.userType === "Employee";

  const getStatusBadgeClass = (status) => {
    const map = {
      Pending: "bg-warning text-dark",
      Confirmed: "bg-info text-dark",
      InProgress: "bg-primary",
      Completed: "bg-success",
      Cancelled: "bg-danger",
    };
    return map[status] || "bg-secondary";
  };

  useEffect(() => {
    fetchBookings(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchBookings = async (page = pageNumber) => {
    try {
      setLoading(true);
      setError(null);
      const response = await adminBookingService.getBookings({
        pageNumber: page,
        pageSize,
        status: statusFilter || undefined,
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

  const handleStatusChange = (e) => {
    setStatusFilter(e.target.value);
    fetchBookings(1);
  };

  const handlePageChange = (newPage) => {
    if (!pagination) return;
    if (newPage < 1 || newPage > pagination.totalPages) return;
    fetchBookings(newPage);
  };

  const openDetails = async (bookingId) => {
    setShowModal(true);
    setDetailsLoading(true);
    setDetailsError(null);
    try {
      // const response = await bookingService.getBookingById(bookingId);
      const response = await adminBookingService.getBookingById(bookingId);
      setSelectedBooking(response.data);
    } catch (err) {
      setDetailsError(
        err.response?.data?.errors?.[0] || "Failed to load booking details"
      );
    } finally {
      setDetailsLoading(false);
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedBooking(null);
    setDetailsError(null);
  };

  const handleAdminCancel = async () => {
    if (!selectedBooking) return;
    const reason = window.prompt("Enter cancellation reason:");
    if (!reason) return;

    try {
      setActionLoading(true);
      await bookingService.cancelBooking(selectedBooking.id, reason);
      await fetchBookings(pageNumber);
      const refreshed = await bookingService.getBookingById(selectedBooking.id);
      setSelectedBooking(refreshed.data);
    } catch (err) {
      setDetailsError(
        err.response?.data?.errors?.[0] || "Failed to cancel booking"
      );
    } finally {
      setActionLoading(false);
    }
  };

  const handleAdminConfirm = async () => {
    if (!selectedBooking) return;
    const notes = window.prompt(
      "Optional: enter staff notes for confirmation (or leave empty)"
    );

    try {
      setActionLoading(true);
      await adminBookingService.confirmBooking(
        selectedBooking.id,
        notes || undefined
      );
      await fetchBookings(pageNumber);
      const refreshed = await bookingService.getBookingById(selectedBooking.id);
      setSelectedBooking(refreshed.data);
    } catch (err) {
      setDetailsError(
        err.response?.data?.errors?.[0] || "Failed to confirm booking"
      );
    } finally {
      setActionLoading(false);
    }
  };

  const handleAdminStart = async () => {
    if (!selectedBooking) return;
    const notes = window.prompt(
      "Optional: enter staff notes when starting work (or leave empty)"
    );

    try {
      setActionLoading(true);
      await adminBookingService.startBooking(
        selectedBooking.id,
        notes || undefined
      );
      await fetchBookings(pageNumber);
      const refreshed = await bookingService.getBookingById(selectedBooking.id);
      setSelectedBooking(refreshed.data);
    } catch (err) {
      setDetailsError(
        err.response?.data?.errors?.[0] || "Failed to start booking"
      );
    } finally {
      setActionLoading(false);
    }
  };

  const handleAdminComplete = async () => {
    if (!selectedBooking) return;
    const notes = window.prompt(
      "Optional: enter completion notes (or leave empty)"
    );

    try {
      setActionLoading(true);
      await adminBookingService.completeBooking(
        selectedBooking.id,
        notes || undefined
      );
      await fetchBookings(pageNumber);
      const refreshed = await bookingService.getBookingById(selectedBooking.id);
      setSelectedBooking(refreshed.data);
    } catch (err) {
      setDetailsError(
        err.response?.data?.errors?.[0] || "Failed to complete booking"
      );
    } finally {
      setActionLoading(false);
    }
  };

  if (!isAdminLike) {
    return (
      <div className="container mt-5 pt-4">
        <div className="alert alert-danger">
          <i className="fas fa-ban me-2"></i>
          You do not have permission to view this page.
        </div>
      </div>
    );
  }

  if (loading) {
    return <Loading message="Loading bookings overview..." />;
  }

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12 d-flex justify-content-between align-items-center">
          <div>
            <h1 className="mb-2 all-services">Bookings Overview</h1>
            <p className="text-white mb-0">
              Admin view of bookings, statuses, and details. Actions are limited
              to cancellation until more admin endpoints are added.
            </p>
          </div>
          <div className="d-flex align-items-center">
            <label className="me-2 mb-0">Status</label>
            <select
              className="form-select form-select-sm"
              value={statusFilter}
              onChange={handleStatusChange}>
              <option value="">All</option>
              <option value="Pending">Pending</option>
              <option value="Confirmed">Confirmed</option>
              <option value="InProgress">In Progress</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          </div>
        </div>
      </div>

      {error && (
        <ErrorMessage
          message={error}
          onRetry={() => fetchBookings(pageNumber)}
        />
      )}

      {bookings.length === 0 ? (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2"></i>
          No bookings found.
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr>
                <th>#</th>
                <th>Booking No.</th>
                <th>Status</th>
                <th>Date</th>
                <th>Time</th>
                <th>Service</th>
                <th>Vehicle</th>
                <th>Service Center</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {bookings.map((booking, index) => (
                <tr key={booking.id}>
                  <td>{(pagination?.pageNumber - 1) * pageSize + index + 1}</td>
                  <td>{booking.bookingNumber}</td>
                  <td>
                    <span
                      className={`badge ${getStatusBadgeClass(
                        booking.status
                      )}`}>
                      {booking.status}
                    </span>
                  </td>
                  <td>{new Date(booking.bookingDate).toLocaleDateString()}</td>
                  <td>{booking.bookingTime}</td>
                  <td>{booking.serviceName}</td>
                  <td>{booking.vehicleInfo}</td>
                  <td>{booking.serviceCenterName}</td>
                  <td className="text-end">
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => openDetails(booking.id)}>
                      <i className="fas fa-eye me-1"></i>
                      Details
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {pagination && (
        <div className="d-flex justify-content-between align-items-center mt-3">
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

      {showModal && (
        <div
          className="modal show d-block"
          style={{ backgroundColor: "rgba(0,0,0,0.5)" }}>
          <div className="modal-dialog modal-lg modal-dialog-centered">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  Booking Details
                  {selectedBooking && ` #${selectedBooking.bookingNumber}`}
                </h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={closeModal}></button>
              </div>
              <div className="modal-body">
                {detailsLoading && (
                  <Loading message="Loading booking details..." />
                )}
                {detailsError && !detailsLoading && (
                  <ErrorMessage message={detailsError} />
                )}

                {selectedBooking && !detailsLoading && !detailsError && (
                  <div className="row g-3">
                    <div className="col-md-6">
                      <h6>Customer</h6>
                      <p className="mb-1">{selectedBooking.customerName}</p>
                      <p className="mb-1 small">
                        {selectedBooking.customerEmail}
                      </p>
                      {selectedBooking.customerPhone && (
                        <p className="mb-0 small">
                          {selectedBooking.customerPhone}
                        </p>
                      )}
                    </div>
                    <div className="col-md-6">
                      <h6>Service Center</h6>
                      <p className="mb-1">
                        {selectedBooking.serviceCenterName}
                      </p>
                      <p className="mb-1 small">
                        {selectedBooking.serviceCenterAddress}
                      </p>
                      <p className="mb-0 small">
                        {selectedBooking.serviceCenterPhone}
                      </p>
                    </div>
                    <div className="col-md-6">
                      <h6>Service</h6>
                      <p className="mb-1">{selectedBooking.serviceName}</p>
                      <p className="mb-0 small">
                        Price: {selectedBooking.servicePrice} EGP
                      </p>
                    </div>
                    <div className="col-md-6">
                      <h6>Vehicle</h6>
                      <p className="mb-1">{selectedBooking.vehicleInfo}</p>
                      <p className="mb-0 small">
                        Plate: {selectedBooking.plateNumber}
                      </p>
                    </div>
                    <div className="col-md-6">
                      <h6>Appointment</h6>
                      <p className="mb-1">
                        <i className="fas fa-calendar me-1"></i>
                        {new Date(
                          selectedBooking.bookingDate
                        ).toLocaleDateString()}
                      </p>
                      <p className="mb-0">
                        <i className="fas fa-clock me-1"></i>
                        {selectedBooking.bookingTime}
                      </p>
                    </div>
                    <div className="col-md-6">
                      <h6>Status</h6>
                      <p className="mb-1">
                        <span
                          className={`badge me-2 ${getStatusBadgeClass(
                            selectedBooking.status
                          )}`}>
                          {selectedBooking.status}
                        </span>
                      </p>
                      {selectedBooking.cancellationReason && (
                        <p className="mb-0 small text-danger">
                          Cancelled: {selectedBooking.cancellationReason}
                        </p>
                      )}
                    </div>
                    {selectedBooking.customerNotes && (
                      <div className="col-12">
                        <h6>Customer notes</h6>
                        <p className="small mb-0">
                          {selectedBooking.customerNotes}
                        </p>
                      </div>
                    )}
                    {selectedBooking.staffNotes && (
                      <div className="col-12">
                        <h6>Staff notes</h6>
                        <p className="small mb-0">
                          {selectedBooking.staffNotes}
                        </p>
                      </div>
                    )}
                  </div>
                )}
              </div>
              <div className="modal-footer">
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={closeModal}>
                  Close
                </button>
                {selectedBooking && (
                  <>
                    {selectedBooking.status === "Pending" && (
                      <button
                        type="button"
                        className="btn btn-outline-primary me-2"
                        onClick={handleAdminConfirm}
                        disabled={actionLoading}>
                        {actionLoading ? (
                          <>
                            <span className="spinner-border spinner-border-sm me-2"></span>
                            Updating...
                          </>
                        ) : (
                          <>
                            <i className="fas fa-check me-2"></i>
                            Confirm
                          </>
                        )}
                      </button>
                    )}
                    {selectedBooking.status === "Confirmed" && (
                      <button
                        type="button"
                        className="btn btn-outline-primary me-2"
                        onClick={handleAdminStart}
                        disabled={actionLoading}>
                        {actionLoading ? (
                          <>
                            <span className="spinner-border spinner-border-sm me-2"></span>
                            Updating...
                          </>
                        ) : (
                          <>
                            <i className="fas fa-play me-2"></i>
                            Start
                          </>
                        )}
                      </button>
                    )}
                    {selectedBooking.status === "InProgress" && (
                      <button
                        type="button"
                        className="btn btn-outline-success me-2"
                        onClick={handleAdminComplete}
                        disabled={actionLoading}>
                        {actionLoading ? (
                          <>
                            <span className="spinner-border spinner-border-sm me-2"></span>
                            Updating...
                          </>
                        ) : (
                          <>
                            <i className="fas fa-flag-checkered me-2"></i>
                            Complete
                          </>
                        )}
                      </button>
                    )}
                    {selectedBooking.status !== "Completed" &&
                      selectedBooking.status !== "Cancelled" && (
                        <button
                          type="button"
                          className="btn btn-outline-danger"
                          onClick={handleAdminCancel}
                          disabled={actionLoading}>
                          {actionLoading ? (
                            <>
                              <span className="spinner-border spinner-border-sm me-2"></span>
                              Cancelling...
                            </>
                          ) : (
                            <>
                              <i className="fas fa-times me-2"></i>
                              Cancel booking
                            </>
                          )}
                        </button>
                      )}
                  </>
                )}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminBookings;
