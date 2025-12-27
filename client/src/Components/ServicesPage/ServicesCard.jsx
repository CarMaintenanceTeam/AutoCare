import React from "react";
import { Link } from "react-router-dom";
import "./style.css";

// Reusable service card used on Home and Services pages
// "to" controls where the Details button navigates.
const ServicesCard = ({ icon, title, description, to }) => {
  return (
    <div className="card text-center mb-4 service-card shadow-sm border-0">
      <div className="card-body d-flex flex-column justify-content-between">
        {icon && (
          <h2 className="mt-1 fs-3" aria-hidden="true">
            {icon}
          </h2>
        )}
        <h5 className="card-title title mb-2">{title}</h5>
        <p className="card-text text-white small flex-grow-1">{description}</p>
        {to && (
          <div className="text-end mt-3">
            <Link to={to} className="btn details-button btn-sm px-3">
              Details
            </Link>
          </div>
        )}
      </div>
    </div>
  );
};

export default ServicesCard;
