import React, { useEffect, useState } from "react";
import "./Home.css";

import ServicesCard from "../ServicesPage/ServicesCard";
import { Link } from 'react-router-dom';
import { serviceService } from '../../api/serviceService';
import Loading from '../common/Loading';
import ErrorMessage from '../common/ErrorMessage';

export default function Home() {
  const [services, setServices] = useState([]);
  const [servicesLoading, setServicesLoading] = useState(true);
  const [servicesError, setServicesError] = useState(null);

  useEffect(() => {
    const fetchFeaturedServices = async () => {
      try {
        setServicesLoading(true);
        setServicesError(null);
        const response = await serviceService.getAllServices({
          isActive: true,
          pageNumber: 1,
          pageSize: 6,
          sortBy: 'Name',
          sortOrder: 'Asc',
        });
        setServices(response.data || []);
      } catch (err) {
        setServicesError(err.response?.data?.errors?.[0] || 'Failed to load services');
      } finally {
        setServicesLoading(false);
      }
    };

    fetchFeaturedServices();
  }, []);

  const getServiceIcon = (serviceType) => {
    if (serviceType === 'Maintenance') return '🔧';
    if (serviceType === 'SpareParts') return '🛞';
    return '🚗';
  };

  return (
    <>
      <header>
        <div className="header-img d-flex justify-content-center text-center align-items-center vh-100 text-white">
          <div>
            <h2>Professional Car <br /> Maintenance And Repair</h2>
            <p className="mt-5">
              Keep your vehicle running smoothly with our expert maintenance services. <br />
              Quick, reliable, and affordable car care you can trust.
            </p>
            <Link to="/service-centers" className="btn header-button mt-4">
              Book Appointment
            </Link>
          </div>
        </div>
      </header>


                          {/*------- SERVICES SECTION ----------*/}
    <section className="p-4">
  <div className="aboutTitle container text-center position-relative">
    <h2 className="title1">Our Services</h2>
    <h3 className="position-absolute top-50 start-50 translate-middle title2">
      Services
    </h3>
  </div>

  <div className="container mt-4">
    {servicesLoading && (
      <Loading message="Loading featured services..." />
    )}

    {servicesError && !servicesLoading && (
      <ErrorMessage message={servicesError} onRetry={null} />
    )}

    {!servicesLoading && !servicesError && (
      <div className="row g-4">
        {services.slice(0, 6).map((service) => (
          <div key={service.id} className="col-sm-6 col-lg-4 d-flex">
            <ServicesCard
              icon={getServiceIcon(service.serviceType)}
              title={service.nameEn}
              description={service.descriptionEn || 'Professional car maintenance service.'}
              to={`/services/${service.id}`}
            />
          </div>
        ))}
      </div>
    )}
  </div>
  <Link className='text-decoration-none' to='/services'>
    <h2 className='all-services text-end mt-3 me-5'>All Services &gt;&gt;</h2>
  </Link>
</section>


     {/*---------- END OF SERVICES SECTION----------- */}
<div className="section-divider mt-4"></div>


{/* ------------------ABOUT SECTION--------------- */}

<section className='about-section'>

  <div className="aboutTitle container text-center position-relative">
    <h2 className="title1">Know More</h2>
    <h3 className="position-absolute top-50 start-50 translate-middle title2">
      About Us
    </h3>
  </div>


  <div className='container '>
  <div className='row '>
  
     <div className='about d-flex flex-column justify-content-center text-start col-md-7 '>
      <h5>
At AutoCare Pro, we believe every car deserves expert attention and precision. Our team of certified mechanics brings years of experience in car maintenance, diagnostics, and repair — ensuring your vehicle stays safe, reliable, and performing at its best.

We’re committed to providing honest service, transparent pricing, and quality workmanship you can trust. Whether it’s a routine oil change, a full engine check, or a last-minute repair, we treat every car like our own.

Our workshop is equipped with the latest tools and diagnostic technology, allowing us to service all makes and models efficiently and accurately. From minor tune-ups to complex engine overhauls, we’re passionate about getting you back on the road quickly and safely.
  </h5>
<h5 className='mt-4'>
At AutoCare Pro, customer satisfaction drives everything we do. We take the time to explain every repair, offer professional advice, and ensure you understand your vehicle’s needs before any work begins. Our goal is to build lasting relationships — not just fix cars.

We take pride in being a local, trusted auto service center that values integrity, reliability, and excellence. Every member of our team shares a commitment to quality and a genuine love for cars.

Drive in confidence — because with AutoCare Pro, your car is always in good hands.
</h5>


    

    </div>
    <div className='col-md-5 d-flex flex-column justify-content-center'>
      <img
        src="/images/2.jpg"
        className='about-img'
        alt='AutoCare workshop'
      />
    </div>
  
  </div>

</div>

</section>


{/* ------------------END OF ABOUT SECTION--------------- */}


{/* ------------------START OF CONTACT SECTION--------------- */}
<div className="section-divider mt-4"></div>
<section className='contact-section'>
  <div className="aboutTitle container text-center position-relative">
    <h2 className="title1">Get In Touch</h2>
    <h3 className="position-absolute top-50 start-50 translate-middle title2">
      Contact Us
    </h3>
  </div>

  <div className='container mt-5'>
    <div className='row'>
      <div className='col-md-6  text-center'>
        <h4 className='text-white mb-4 fw-bold'>Contact Information</h4>

        <p ><i className="fas fa-map-marker-alt me-2 "></i> 855 AutoCare Lane, Camville, CA 94019</p>
        <p><i className="fas fa-phone me-2 "></i> (532) 435-6787 <br /> Mon-Fri: 9AM–6PM</p>
        <p><i className="fas fa-envelope me-2 "></i> info@autocarepro.com</p>
      </div>

      <div className='col-md-6'>
        <h4 className='text-white mb-4 fw-bold'>Send a Message</h4>
        <form className='contact-form p-4 rounded'>
          <div className='mb-3'>
                <label for="" class="form-label form-title">Name</label>

            <input type='text' className='form-control' placeholder='Name' />
          </div>
          <div className='mb-3'>
                <label for="" class="form-label form-title">Surname</label>

            <input type='text' className='form-control' placeholder='Surname' />
          </div>
          <div className='mb-3'>
                <label for="" class="form-label form-title">Email</label>

            <input type='email' className='form-control' placeholder='Email' />
          </div>
          <div className='mb-3'>
                <label for="" class="form-label form-title">Message</label>

            <textarea className='form-control form-title' rows='3' placeholder='Message'></textarea>
          </div>
          <button type='submit' className='btn  w-100'>Submit</button>
        </form>
      </div>
    </div>
  </div>
</section>



{/* ------------------END OF CONTACT SECTION--------------- */}


    </>
  );
}
