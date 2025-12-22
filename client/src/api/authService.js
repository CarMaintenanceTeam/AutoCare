import apiClient from './config';

export const authService = {
  async login(email, password) {
    const response = await apiClient.post('/auth/login', { email, password });
    return response.data;
  },

  async register(userData) {
    const response = await apiClient.post('/auth/register', userData);
    return response.data;
  },

  async refreshToken(refreshToken) {
    const response = await apiClient.post('/auth/refresh-token', { refreshToken });
    return response.data;
  },

  logout() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  },
};
