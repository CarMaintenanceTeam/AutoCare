import logo from './logo.svg';
import './App.css';
import Navbar from './Components/NavBar/Nav';
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";

import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from './Components/HomePage/Home';
import About from './Components/AboutPage/About';
import Services from './Components/ServicesPage/Services';
import Contact from './Components/ContactPage/Contact';
import '@fortawesome/fontawesome-free/css/all.min.css';
import Footer from './Components/Footer/Footer';

function App() {
  return (
  <body>
      <div >
      <Router>
      <Navbar />
      
      <Routes>
        <Route path="home" element={<Home />} />
        <Route path="/about" element={<About />} />
        <Route path="/services" element={<Services />} />
        <Route path="/contact" element={<Contact />} />
      </Routes>
     <Footer/>
    </Router>

    </div>
  </body>
  );
}

export default App;
