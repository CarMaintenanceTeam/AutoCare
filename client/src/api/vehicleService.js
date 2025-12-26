import apiClient from './config';

export const vehicleService = {
  async getMyVehicles() {
    // Returns ApiResponse<List<VehicleListDto>>
    const response = await apiClient.get('/vehicles');
    return response.data;
  },

  // Note: backend currently does not expose GET /vehicles/{id}
  async getVehicleById(id) {
    const response = await apiClient.get(`/vehicles/${id}`);
    return response.data;
  },

  async createVehicle(vehicleData) {
    // Expects: { brand, model, year, plateNumber, vin?, color? }
    const response = await apiClient.post('/vehicles', vehicleData);
    return response.data;
  },

  async updateVehicle(id, vehicleData) {
    // Backend UpdateVehicleCommand requires vehicleId plus updatable fields only
    const payload = {
      vehicleId: id,
      brand: vehicleData.brand,
      model: vehicleData.model,
      year: vehicleData.year,
      color: vehicleData.color,
    };

    const response = await apiClient.put(`/vehicles/${id}`, payload);
    return response.data;
  },

  async deleteVehicle(id) {
    const response = await apiClient.delete(`/vehicles/${id}`);
    return response.data;
  },
};
