import './App.css';
import Navbar from './Components/NavBar/Nav';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import '@fortawesome/fontawesome-free/css/all.min.css';

import { Routes, Route, Navigate } from 'react-router-dom';
import Home from './Components/HomePage/Home';
import About from './Components/AboutPage/About';
import Services from './Components/ServicesPage/Services';
import Contact from './Components/ContactPage/Contact';
import Footer from './Components/Footer/Footer';
import ProtectedRoute from './Components/ProtectedRoute';

import Dashboard from './pages/Dashboard';
import ServiceCenters from './pages/ServiceCenters';
import ServiceCenterDetail from './pages/ServiceCenterDetail';
import Vehicles from './pages/Vehicles';
import Bookings from './pages/Bookings';
import Login from './pages/auth/Login';
import Register from './pages/auth/Register';
import ServiceDetails from './Components/ServiceDetails/ServiceDetails';
import NearbyServiceCenters from './pages/NearbyServiceCenters';
import AdminLayout from './pages/admin/AdminLayout';
import AdminBookings from './pages/admin/AdminBookings';
import AdminVehicles from './pages/admin/AdminVehicles';
import AdminCustomers from './pages/admin/AdminCustomers';
import AdminServiceCenters from './pages/admin/AdminServiceCenters';
import AdminEmployees from './pages/admin/AdminEmployees';

function App() {
  return (
    <div>
      <Navbar />

      <Routes>
        {/* Public routes */}
        <Route path="/" element={<Navigate to="/home" replace />} />
        <Route path="/home" element={<Home />} />
        <Route path="/about" element={<About />} />
        <Route path="/services" element={<Services />} />
        <Route path="/services/:id" element={<ServiceDetails />} />
        <Route path="/contact" element={<Contact />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* Service centers - list public, detail & booking protected */}
        <Route path="/service-centers" element={<ServiceCenters />} />
        <Route
          path="/service-centers/:id"
          element={
            <ProtectedRoute>
              <ServiceCenterDetail />
            </ProtectedRoute>
          }
        />
        <Route path="/nearby" element={<NearbyServiceCenters />} />

        {/* Protected customer routes */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />

        <Route
          path="/vehicles"
          element={
            <ProtectedRoute>
              <Vehicles />
            </ProtectedRoute>
          }
        />

        <Route
          path="/bookings"
          element={
            <ProtectedRoute>
              <Bookings />
            </ProtectedRoute>
          }
        />

        {/* Admin area (layout + nested sections) */}
        <Route
          path="/admin"
          element={
            <ProtectedRoute>
              <AdminLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<AdminBookings />} />
          <Route path="bookings" element={<AdminBookings />} />
          <Route path="vehicles" element={<AdminVehicles />} />
          <Route path="customers" element={<AdminCustomers />} />
          <Route path="service-centers" element={<AdminServiceCenters />} />
          <Route path="employees" element={<AdminEmployees />} />
        </Route>

        {/* Fallback route */}
        <Route path="*" element={<Navigate to="/home" replace />} />
      </Routes>

      <Footer />
    </div>
  );
}

export default App;
