import apiClient from './config';

export const adminBookingService = {
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
};