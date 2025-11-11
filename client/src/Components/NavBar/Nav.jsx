import React from "react";
import './Nav.css';
import { NavLink } from "react-router-dom";

export default function Navbar() {
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
              <NavLink className="nav-link nav-text" to="/contact">Contact</NavLink>
            </li>
          </ul>
        </div>

   
        <button className=" login-button btn btn-outline-primary ms-auto" type="submit">
          Login
        </button>
      </div>
    </nav>
  );

}
