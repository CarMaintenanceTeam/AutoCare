import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Dashboard = () => {
  const { user } = useAuth();

  return (
    <div className="container mt-5 pt-4">
      <div className="row">
        <div className="col-12">
          <h1 className="mb-4">Welcome, {user?.fullName}!</h1>
          <p className="text-muted mb-5">Manage your vehicles and bookings from your dashboard</p>
        </div>
      </div>

      <div className="row g-4">
        {/* Quick Actions */}
        <div className="col-md-4">
          <div className="card h-100 shadow-sm border-0">
            <div className="card-body text-center p-4">
              <div className="mb-3">
                <i className="fas fa-car fa-3x text-primary"></i>
              </div>
              <h5 className="card-title">My Vehicles</h5>
              <p className="card-text text-muted">
                View and manage your registered vehicles
              </p>
              <Link to="/vehicles" className="btn btn-primary">
                Manage Vehicles
              </Link>
            </div>
          </div>
        </div>

        <div className="col-md-4">
          <div className="card h-100 shadow-sm border-0">
            <div className="card-body text-center p-4">
              <div className="mb-3">
                <i className="fas fa-calendar-check fa-3x text-success"></i>
              </div>
              <h5 className="card-title">My Bookings</h5>
              <p className="card-text text-muted">
                View your booking history and upcoming appointments
              </p>
              <Link to="/bookings" className="btn btn-success">
                View Bookings
              </Link>
            </div>
          </div>
        </div>

        <div className="col-md-4">
          <div className="card h-100 shadow-sm border-0">
            <div className="card-body text-center p-4">
              <div className="mb-3">
                <i className="fas fa-wrench fa-3x text-warning"></i>
              </div>
              <h5 className="card-title">Service Centers</h5>
              <p className="card-text text-muted">
                Find nearby service centers and book appointments
              </p>
              <Link to="/service-centers" className="btn btn-warning text-white">
                Find Centers
              </Link>
            </div>
          </div>
        </div>
      </div>

      <div className="row mt-5">
        <div className="col-12">
          <div className="card shadow-sm border-0">
            <div className="card-body">
              <h5 className="card-title mb-3">
                <i className="fas fa-info-circle text-primary me-2"></i>
                Quick Guide
              </h5>
              <div className="row">
                <div className="col-md-4 mb-3">
                  <h6><i className="fas fa-check-circle text-success me-2"></i>Step 1</h6>
                  <p className="small text-muted">Register your vehicle(s) in the system</p>
                </div>
                <div className="col-md-4 mb-3">
                  <h6><i className="fas fa-check-circle text-success me-2"></i>Step 2</h6>
                  <p className="small text-muted">Browse nearby service centers and available services</p>
                </div>
                <div className="col-md-4 mb-3">
                  <h6><i className="fas fa-check-circle text-success me-2"></i>Step 3</h6>
                  <p className="small text-muted">Book an appointment and track its status</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
