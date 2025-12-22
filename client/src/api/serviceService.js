import apiClient from './config';

export const serviceService = {
  async getAllServices() {
    const response = await apiClient.get('/services');
    return response.data;
  },

  async getServiceById(id) {
    const response = await apiClient.get(`/services/${id}`);
    return response.data;
  },

  async getServicesByServiceCenter(serviceCenterId) {
    const response = await apiClient.get(`/services/by-service-center/${serviceCenterId}`);
    return response.data;
  },
};
