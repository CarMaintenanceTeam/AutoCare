import apiClient from './config';

export const serviceCenterService = {
  async getAllServiceCenters(filters = {}) {
    const params = new URLSearchParams();
    if (filters.pageNumber) params.append('pageNumber', filters.pageNumber);
    if (filters.pageSize) params.append('pageSize', filters.pageSize);
    if (filters.city) params.append('city', filters.city);
    if (filters.isActive !== undefined) params.append('isActive', filters.isActive);
    if (filters.serviceId) params.append('serviceId', filters.serviceId);
    if (filters.sortBy) params.append('sortBy', filters.sortBy);
    if (filters.sortOrder) params.append('sortOrder', filters.sortOrder);

    const query = params.toString();
    const url = query ? `/service-centers?${query}` : '/service-centers';
    const response = await apiClient.get(url);
    // Returns PaginatedApiResponse<ServiceCenterListDto>
    return response.data;
  },

  async getServiceCenterById(id) {
    const response = await apiClient.get(`/service-centers/${id}`);
    return response.data;
  },

  async getNearbyServiceCenters({
    latitude,
    longitude,
    radiusKm = 50,
    pageNumber,
    pageSize,
    serviceId,
  }) {
    const params = new URLSearchParams();
    params.append('latitude', latitude);
    params.append('longitude', longitude);
    params.append('radiusKm', radiusKm);
    if (pageNumber) params.append('pageNumber', pageNumber);
    if (pageSize) params.append('pageSize', pageSize);
    if (serviceId) params.append('serviceId', serviceId);

    const response = await apiClient.get(`/service-centers/nearby?${params.toString()}`);
    // Returns PaginatedApiResponse<ServiceCenterListDto>
    return response.data;
  },
};
