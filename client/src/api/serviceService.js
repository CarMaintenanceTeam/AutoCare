import apiClient from "./config";

export const serviceService = {
  async getAllServices(filters = {}) {
    const params = new URLSearchParams();
    if (filters.pageNumber) params.append("pageNumber", filters.pageNumber);
    if (filters.pageSize) params.append("pageSize", filters.pageSize);
    if (filters.serviceType) params.append("serviceType", filters.serviceType);
    if (filters.isActive !== undefined)
      params.append("isActive", filters.isActive);
    if (filters.minPrice != null) params.append("minPrice", filters.minPrice);
    if (filters.maxPrice != null) params.append("maxPrice", filters.maxPrice);
    if (filters.sortBy) params.append("sortBy", filters.sortBy);
    if (filters.sortOrder) params.append("sortOrder", filters.sortOrder);

    const query = params.toString();
    const url = query ? `/services?${query}` : "/services";
    const response = await apiClient.get(url);
    // Returns PaginatedApiResponse<ServiceListDto>
    return response.data;
  },

  async getServiceById(id, includeServiceCenters = true) {
    const response = await apiClient.get(
      `/services/${id}?includeServiceCenters=${includeServiceCenters}`
    );
    return response.data;
  },

  async getServicesByServiceCenter(serviceCenterId, options = {}) {
    const params = new URLSearchParams();
    if (options.includeUnavailable !== undefined) {
      params.append("includeUnavailable", options.includeUnavailable);
    }
    if (options.serviceType) {
      params.append("serviceType", options.serviceType);
    }

    const query = params.toString();
    const url = query
      ? `/services/by-service-center/${serviceCenterId}?${query}`
      : `/services/by-service-center/${serviceCenterId}`;

    const response = await apiClient.get(url);
    // Returns ApiResponse<List<ServiceCenterServiceDto>>
    return response.data;
  },
};
