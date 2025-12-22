import apiClient from './config';

export const bookingService = {
  async getMyBookings() {
    const response = await apiClient.get('/bookings');
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
    const response = await apiClient.post(`/bookings/${id}/cancel`, { 
      cancellationReason: reason 
    });
    return response.data;
  },
};
