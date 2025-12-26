import React from 'react'
import "./style.css"

export default function Footer() {
  return (
   <>
   <footer className="mt-5">
      <div className="container text-center">
        <div className="row">
          <div className="col-md-4 mb-3">
            <h6>AutoCare Pro</h6>
            <p>Professional car maintenance and <br />
repair services you can trust</p>
          </div>
          <div className="col-md-4 mb-3">
            <h6>Contact Info</h6>
            <p>
                123 Main Street, <br />
                Anytown, ST 12345  <br />
                (555) 123-4567 <br />
                info@autocarepro.com
            </p>
            
          </div>
          <div className="col-md-4 mb-3">
            <h6>Follow Us</h6>
            <ul className="list-inline social-icons">
              <li className="list-inline-item">
                <a href="https://facebook.com" target="_blank" rel="noreferrer" className="text-white">
                  <i className="fa-brands fa-facebook"></i>
                </a>
              </li>
              <li className="list-inline-item">
                <a href="https://instagram.com" target="_blank" rel="noreferrer" className="text-white">
                  <i className="fa-brands fa-instagram"></i>
                </a>
              </li>
              <li className="list-inline-item">
                <a href="https://twitter.com" target="_blank" rel="noreferrer" className="text-white">
                  <i className="fa-brands fa-x-twitter"></i>
                </a>
              </li>
            </ul>
          </div>
        </div>
        <hr className="mb-4" />  
        <div className="row">
          <div className="col-md-12 text-center">
            <p className='copy-right'> © 2024 AutoCare Pro. All rights reserved.</p>
          </div>
        </div>
      </div>
    </footer>
   </>
  )
}
