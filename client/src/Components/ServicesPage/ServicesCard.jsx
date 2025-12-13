import React from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "./style.css"

const ServicesCard = ({ icon, title, description }) => {
  return (
    <div className="card text-center mb-4 service-card">
      <div className="card-body d-flex flex-column justify-content-between ">
        <h2 className="mt-2 fs-3">{icon}</h2>
        <h5 className="card-title title  mb-2">{title}</h5>
        <p className="card-text text-white">{description}</p>
<div className='text-end'>
    <button type="button" class="btn details-button  w-25">Details</button>     

</div>
 </div>
    </div>
  );
};

export default ServicesCard;
