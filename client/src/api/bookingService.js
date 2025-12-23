import apiClient from './config';

export const bookingService = {
  async getMyBookings(params = {}) {
    const search = new URLSearchParams();
    if (params.pageNumber) search.append('pageNumber', params.pageNumber);
    if (params.pageSize) search.append('pageSize', params.pageSize);
    if (params.status) search.append('status', params.status);
    if (params.fromDate) search.append('fromDate', params.fromDate);
    if (params.toDate) search.append('toDate', params.toDate);
    if (params.sortBy) search.append('sortBy', params.sortBy);
    if (params.sortOrder) search.append('sortOrder', params.sortOrder);

    const query = search.toString();
    const url = query ? `/bookings?${query}` : '/bookings';
    const response = await apiClient.get(url);
    // Returns PaginatedApiResponse<BookingListDto>
    return response.data;
  },

  async getBookingById(id) {
    const response = await apiClient.get(`/bookings/${id}`);
    return response.data;
  },

  async createBooking(bookingData) {
    const response = await apiClient.post('/bookings', bookingData);
    return response.data;
  },

  async cancelBooking(id, reason) {
    // Backend expects { reason } as body
    const response = await apiClient.post(`/bookings/${id}/cancel`, {
      reason,
    });
    return response.data;
  },
};
