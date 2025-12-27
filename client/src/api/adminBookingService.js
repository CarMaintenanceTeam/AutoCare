import apiClient from "./config";

export const adminBookingService = {
  async getBookings(params = {}) {
    const search = new URLSearchParams();
    if (params.pageNumber) search.append("pageNumber", params.pageNumber);
    if (params.pageSize) search.append("pageSize", params.pageSize);
    if (params.status) search.append("status", params.status);
    if (params.fromDate) search.append("fromDate", params.fromDate);
    if (params.toDate) search.append("toDate", params.toDate);
    if (params.sortBy) search.append("sortBy", params.sortBy);
    if (params.sortOrder) search.append("sortOrder", params.sortOrder);

    const query = search.toString();
    const url = query ? `/admin/bookings?${query}` : "/admin/bookings";
    const response = await apiClient.get(url);
    return response.data; // PaginatedApiResponse<BookingListDto>
  },
  async confirmBooking(id, notes) {
    const response = await apiClient.post(`/admin/bookings/${id}/confirm`, {
      notes: notes || null,
    });
    return response.data;
  },

  async startBooking(id, notes) {
    const response = await apiClient.post(`/admin/bookings/${id}/start`, {
      notes: notes || null,
    });
    return response.data;
  },

  async completeBooking(id, notes) {
    const response = await apiClient.post(`/admin/bookings/${id}/complete`, {
      notes: notes || null,
    });
    return response.data;
  },
  async getBookingById(id) {
    const response = await apiClient.get(`/admin/bookings/${id}`);
    return response.data;
  },
};
