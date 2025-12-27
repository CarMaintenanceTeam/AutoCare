import React, { useEffect, useState } from "react";
import { adminCustomerService } from "../../api/adminCustomerService";
import Loading from "../../Components/common/Loading";
import ErrorMessage from "../../Components/common/ErrorMessage";

const AdminCustomers = () => {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;
  const [pagination, setPagination] = useState(null);
  const [search, setSearch] = useState("");
  const [city, setCity] = useState("");
  const [isActive, setIsActive] = useState("all");

  const fetchCustomers = async (page = pageNumber) => {
    try {
      setLoading(true);
      setError(null);

      const response = await adminCustomerService.getCustomers({
        pageNumber: page,
        pageSize,
        search: search || undefined,
        city: city || undefined,
        isActive:
          isActive === "all" ? undefined : isActive === "active" ? true : false,
        sortBy: "Name",
        sortOrder: "Asc",
      });

      setCustomers(response.data || []);
      setPagination(response.pagination || null);
      setPageNumber(page);
    } catch (err) {
      setError(
        err.response?.data?.errors?.[0] ||
          err.response?.data?.error ||
          "Failed to load customers"
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCustomers(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    if (name === "search") setSearch(value);
    if (name === "city") setCity(value);
    if (name === "isActive") setIsActive(value);
  };

  const handleApplyFilters = () => {
    fetchCustomers(1);
  };

  const handlePageChange = (newPage) => {
    if (!pagination) return;
    if (newPage < 1 || newPage > pagination.totalPages) return;
    fetchCustomers(newPage);
  };

  if (loading) {
    return <Loading message="Loading customers..." />;
  }

  return (
    <div className="container mt-3">
      <div className="row mb-3">
        <div className="col-12 d-flex justify-content-between align-items-center">
          <div>
            <h2 className="mb-1 all-services">Customers (Admin)</h2>
            <p className="text-white mb-0">
              Overview of all customer accounts with basic profile and activity
              info.
            </p>
          </div>
          <div className="d-flex gap-2">
            <input
              type="text"
              name="search"
              className="form-control form-control-sm"
              placeholder="Search by name or email..."
              value={search}
              onChange={handleFilterChange}
            />
            <input
              type="text"
              name="city"
              className="form-control form-control-sm"
              placeholder="City..."
              value={city}
              onChange={handleFilterChange}
            />
            <select
              name="isActive"
              className="form-select form-select-sm"
              value={isActive}
              onChange={handleFilterChange}>
              <option value="all">All</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
            </select>
            <button
              type="button"
              className="btn btn-sm btn-primary"
              onClick={handleApplyFilters}>
              Apply
            </button>
          </div>
        </div>
      </div>

      {error && (
        <div className="mb-3">
          <ErrorMessage message={error} onRetry={() => fetchCustomers(1)} />
        </div>
      )}

      {customers.length === 0 ? (
        <div className="alert alert-info">
          <i className="fas fa-info-circle me-2" />
          No customers found.
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr>
                <th>#</th>
                <th>Name</th>
                <th>Email</th>
                <th>Phone</th>
                <th>City</th>
                <th>Vehicles</th>
                <th>Bookings</th>
                <th>Status</th>
                <th>Created At</th>
              </tr>
            </thead>
            <tbody>
              {customers.map((c, index) => (
                <tr key={c.id}>
                  <td>{(pagination?.pageNumber - 1) * pageSize + index + 1}</td>
                  <td>{c.fullName}</td>
                  <td>{c.email}</td>
                  <td>{c.phoneNumber || "-"}</td>
                  <td>{c.city || "-"}</td>
                  <td>{c.vehiclesCount}</td>
                  <td>{c.bookingsCount}</td>
                  <td>
                    <span
                      className={`badge ${
                        c.isActive ? "bg-success" : "bg-secondary"
                      }`}>
                      {c.isActive ? "Active" : "Inactive"}
                    </span>
                  </td>
                  <td>{new Date(c.createdAt).toLocaleDateString()}</td>
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
            {pagination.totalCount} customers)
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

export default AdminCustomers;
