import React from "react";
import './Nav.css';
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from '../../context/AuthContext';

export default function Navbar() {
  const { isAuthenticated, user, logout } = useAuth();
  const isAdminLike = user?.userType === 'Admin' || user?.userType === 'Employee';
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/home');
  };

  return (
    <nav className="navbar nav-transparent mt-2 navbar-expand-lg">
      <div className="container">
     
        <NavLink className="navbar-brand logo" to="/home">
          AutoCare Pro
        </NavLink>

        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarSupportedContent"
          aria-controls="navbarSupportedContent"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse justify-content-center" id="navbarSupportedContent">
          <ul className="navbar-nav mb-2 mb-lg-0">
            <li className="nav-item">
              <NavLink className="nav-link nav-text" to="/home">Home</NavLink>
            </li>
            <li className="nav-item">
              <NavLink className="nav-link nav-text" to="/about">About</NavLink>
            </li>
            <li className="nav-item">
              <NavLink className="nav-link nav-text" to="/services">Services</NavLink>
            </li>
            <li className="nav-item">
              <NavLink className="nav-link nav-text" to="/service-centers">Service Centers</NavLink>
            </li>
            <li className="nav-item">
              <NavLink className="nav-link nav-text" to="/nearby">Nearby</NavLink>
            </li>
            <li className="nav-item">
              <NavLink className="nav-link nav-text" to="/contact">Contact</NavLink>
            </li>
            <li className="nav-item">
              <NavLink className="nav-link nav-text" to="/contact">Contact</NavLink>
            </li>
            {isAuthenticated && (
              <>
                <li className="nav-item">
                  <NavLink className="nav-link nav-text" to="/dashboard">Dashboard</NavLink>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link nav-text" to="/vehicles">My Vehicles</NavLink>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link nav-text" to="/bookings">My Bookings</NavLink>
                </li>
                {isAdminLike && (
                  <li className="nav-item">
                    <NavLink className="nav-link nav-text" to="/admin">Admin</NavLink>
                  </li>
                )}
              </>
            )}
          </ul>
        </div>

        {isAuthenticated ? (
          <div className="dropdown ms-auto">
            <button 
              className="btn btn-outline-primary dropdown-toggle" 
              type="button" 
              id="userDropdown" 
              data-bs-toggle="dropdown" 
              aria-expanded="false"
            >
              <i className="fas fa-user me-2"></i>
              {user?.fullName}
            </button>
            <ul className="dropdown-menu dropdown-menu-end" aria-labelledby="userDropdown">
              <li><NavLink className="dropdown-item" to="/dashboard">Dashboard</NavLink></li>
              <li><hr className="dropdown-divider" /></li>
              <li><button className="dropdown-item" onClick={handleLogout}>Logout</button></li>
            </ul>
          </div>
        ) : (
          <NavLink className="login-button btn btn-outline-primary ms-auto" to="/login">
            Login
          </NavLink>
        )}
      </div>
    </nav>
  );

}
