import React, { useState, useEffect } from 'react';
import { vehicleService } from '../api/vehicleService';
import Loading from '../Components/common/Loading';
import ErrorMessage from '../Components/common/ErrorMessage';

const Vehicles = () => {
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [editingVehicle, setEditingVehicle] = useState(null);
  const [formData, setFormData] = useState({
    brand: '',
    model: '',
    year: new Date().getFullYear(),
    plateNumber: '',
    vin: '',
    color: '',
  });
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    fetchVehicles();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchVehicles = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await vehicleService.getMyVehicles();
      setVehicles(response.data || []);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to load vehicles');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);

    try {
      if (editingVehicle) {
        await vehicleService.updateVehicle(editingVehicle.id, formData);
      } else {
        await vehicleService.createVehicle(formData);
      }
      await fetchVehicles();
      handleCloseModal();
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to save vehicle');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this vehicle?')) return;

    try {
      await vehicleService.deleteVehicle(id);
      await fetchVehicles();
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to delete vehicle');
    }
  };

  const handleOpenModal = (vehicle = null) => {
    if (vehicle) {
      setEditingVehicle(vehicle);
      setFormData({
        brand: vehicle.brand,
        model: vehicle.model,
        year: vehicle.year,
        plateNumber: vehicle.plateNumber,
        vin: '', // VIN is not exposed in list DTO; keep empty for updates
        color: vehicle.color || '',
      });
    } else {
      setEditingVehicle(null);
      setFormData({
        brand: '',
        model: '',
        year: new Date().getFullYear(),
        plateNumber: '',
        vin: '',
        color: '',
      });
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setEditingVehicle(null);
    setError(null);
  };

  if (loading) return <Loading message="Loading vehicles..." />;

  return (
    <div className="container mt-5 pt-4">
      <div className="row mb-4">
        <div className="col-12">
          <div className="d-flex justify-content-between align-items-center">
            <div>
              <h1 className="mb-2">My Vehicles</h1>
              <p className="text-muted">Manage your registered vehicles</p>
            </div>
            <button className="btn btn-primary" onClick={() => handleOpenModal()}>
              <i className="fas fa-plus me-2"></i>Add Vehicle
            </button>
          </div>
        </div>
      </div>

      {error && !showModal && <ErrorMessage message={error} onRetry={fetchVehicles} />}

      <div className="row g-4">
        {vehicles.length === 0 ? (
          <div className="col-12">
            <div className="alert alert-info">
              <i className="fas fa-info-circle me-2"></i>
              No vehicles registered yet. Add your first vehicle to get started!
            </div>
          </div>
        ) : (
          vehicles.map((vehicle) => (
            <div key={vehicle.id} className="col-md-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-body">
                  <div className="d-flex justify-content-between align-items-start mb-3">
                    <h5 className="card-title mb-0">
                      {vehicle.brand} {vehicle.model}
                    </h5>
                    <div className="btn-group btn-group-sm">
                      <button
                        className="btn btn-outline-primary"
                        onClick={() => handleOpenModal(vehicle)}
                      >
                        <i className="fas fa-edit"></i>
                      </button>
                      <button
                        className="btn btn-outline-danger"
                        onClick={() => handleDelete(vehicle.id)}
                      >
                        <i className="fas fa-trash"></i>
                      </button>
                    </div>
                  </div>
                  <p className="text-muted mb-2">
                    <i className="fas fa-calendar me-2"></i>
                    <strong>Year:</strong> {vehicle.year}
                  </p>
                  <p className="text-muted mb-2">
                    <i className="fas fa-id-card me-2"></i>
                    <strong>Plate:</strong> {vehicle.plateNumber}
                  </p>
                  {vehicle.color && (
                    <p className="text-muted mb-2">
                      <i className="fas fa-palette me-2"></i>
                      <strong>Color:</strong> {vehicle.color}
                    </p>
                  )}
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Modal */}
      {showModal && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  {editingVehicle ? 'Edit Vehicle' : 'Add New Vehicle'}
                </h5>
                <button type="button" className="btn-close" onClick={handleCloseModal}></button>
              </div>
              <form onSubmit={handleSubmit}>
                <div className="modal-body">
                  {error && <ErrorMessage message={error} />}

                  <div className="row g-3">
                    <div className="col-md-6">
                      <label className="form-label">Brand</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.brand}
                        onChange={(e) => setFormData({ ...formData, brand: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label">Model</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.model}
                        onChange={(e) => setFormData({ ...formData, model: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label">Year</label>
                      <input
                        type="number"
                        className="form-control"
                        value={formData.year}
                        onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value, 10) || new Date().getFullYear() })}
                        min="1900"
                        max={new Date().getFullYear() + 1}
                        required
                      />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label">Plate Number</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.plateNumber}
                        onChange={(e) => setFormData({ ...formData, plateNumber: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label">Color (Optional)</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.color}
                        onChange={(e) => setFormData({ ...formData, color: e.target.value })}
                      />
                    </div>
                    {!editingVehicle && (
                      <div className="col-md-6">
                        <label className="form-label">VIN (Optional)</label>
                        <input
                          type="text"
                          className="form-control"
                          value={formData.vin}
                          onChange={(e) => setFormData({ ...formData, vin: e.target.value })}
                        />
                      </div>
                    )}
                  </div>
                </div>
                <div className="modal-footer">
                  <button type="button" className="btn btn-secondary" onClick={handleCloseModal}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary" disabled={submitting}>
                    {submitting ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2"></span>
                        Saving...
                      </>
                    ) : (
                      'Save Vehicle'
                    )}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Vehicles;
