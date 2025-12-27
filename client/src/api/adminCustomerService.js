import apiClient from "./config";

export const adminCustomerService = {
  async getCustomers(params = {}) {
    const search = new URLSearchParams();
    if (params.pageNumber) search.append("pageNumber", params.pageNumber);
    if (params.pageSize) search.append("pageSize", params.pageSize);
    if (params.search) search.append("search", params.search);
    if (params.city) search.append("city", params.city);
    if (params.isActive !== undefined && params.isActive !== null) {
      search.append("isActive", params.isActive);
    }
    if (params.sortBy) search.append("sortBy", params.sortBy);
    if (params.sortOrder) search.append("sortOrder", params.sortOrder);

    const query = search.toString();
    const url = query ? `/admin/customers?${query}` : "/admin/customers";
    const response = await apiClient.get(url);
    return response.data; // PaginatedApiResponse<CustomerListDto>
  },
};
