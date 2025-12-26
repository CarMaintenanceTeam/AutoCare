import React from 'react';
import { Link, NavLink, Outlet, Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const AdminLayout = () => {
  const { user } = useAuth();
  const location = useLocation();
  const isAdminLike = user?.userType === 'Admin' || user?.userType === 'Employee';

  if (!isAdminLike) {
    // Extra protection: if a non-admin/employee somehow reaches /admin,
    // immediately redirect them away instead of showing admin UI.
    return (
      <Navigate
        to="/dashboard"
        replace
        state={{ from: location, unauthorized: true }}
      />
    );
  }

  return (
    <div className="container-fluid mt-4 pt-2">
      <div className="row">
        <aside className="col-md-3 col-lg-2 mb-4">
          <div className="card shadow-sm h-100">
            <div className="card-body p-3">
              <h5 className="card-title mb-3 d-flex align-items-center">
                <i className="fas fa-tools me-2"></i>
                Admin Panel
              </h5>
              <p className="text-muted small mb-3">
                {user?.fullName} ({user?.userType})
              </p>
              <nav className="nav flex-column nav-pills">
                <NavLink
                  to="/admin/bookings"
                  className={({ isActive }) =>
                    'nav-link mb-1 ' + (isActive ? 'active' : 'text-secondary')
                  }
                >
                  <i className="fas fa-calendar-check me-2"></i>
                  Bookings
                </NavLink>
                <NavLink
                  to="/admin/vehicles"
                  className={({ isActive }) =>
                    'nav-link mb-1 ' + (isActive ? 'active' : 'text-secondary')
                  }
                >
                  <i className="fas fa-car me-2"></i>
                  Vehicles
                </NavLink>
                <NavLink
                  to="/admin/customers"
                  className={({ isActive }) =>
                    'nav-link mb-1 ' + (isActive ? 'active' : 'text-secondary')
                  }
                >
                  <i className="fas fa-users me-2"></i>
                  Customers
                </NavLink>
                <NavLink
                  to="/admin/service-centers"
                  className={({ isActive }) =>
                    'nav-link mb-1 ' + (isActive ? 'active' : 'text-secondary')
                  }
                >
                  <i className="fas fa-warehouse me-2"></i>
                  Service Centers
                </NavLink>
                <NavLink
                  to="/admin/employees"
                  className={({ isActive }) =>
                    'nav-link mb-1 ' + (isActive ? 'active' : 'text-secondary')
                  }
                >
                  <i className="fas fa-user-tie me-2"></i>
                  Employees
                </NavLink>
                <hr />
                <Link to="/dashboard" className="nav-link text-secondary">
                  <i className="fas fa-arrow-left me-2"></i>
                  Back to user dashboard
                </Link>
              </nav>
            </div>
          </div>
        </aside>

        <main className="col-md-9 col-lg-10">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default AdminLayout;