import apiClient from './config';

export const serviceCenterService = {
  async getAllServiceCenters(filters = {}) {
    const params = new URLSearchParams();
    if (filters.city) params.append('city', filters.city);
    if (filters.searchTerm) params.append('searchTerm', filters.searchTerm);
    
    const response = await apiClient.get(`/service-centers?${params.toString()}`);
    return response.data;
  },

  async getServiceCenterById(id) {
    const response = await apiClient.get(`/service-centers/${id}`);
    return response.data;
  },

  async getNearbyServiceCenters(latitude, longitude, radiusKm = 50) {
    const response = await apiClient.get(
      `/service-centers/nearby?latitude=${latitude}&longitude=${longitude}&radiusKm=${radiusKm}`
    );
    return response.data;
  },
};
