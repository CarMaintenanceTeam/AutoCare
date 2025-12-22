import apiClient from './config';

export const vehicleService = {
  async getMyVehicles() {
    const response = await apiClient.get('/vehicles');
    return response.data;
  },

  async getVehicleById(id) {
    const response = await apiClient.get(`/vehicles/${id}`);
    return response.data;
  },

  async createVehicle(vehicleData) {
    const response = await apiClient.post('/vehicles', vehicleData);
    return response.data;
  },

  async updateVehicle(id, vehicleData) {
    const response = await apiClient.put(`/vehicles/${id}`, vehicleData);
    return response.data;
  },

  async deleteVehicle(id) {
    const response = await apiClient.delete(`/vehicles/${id}`);
    return response.data;
  },
};
