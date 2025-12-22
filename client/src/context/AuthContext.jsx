import React, { createContext, useState, useContext, useEffect } from 'react';
import { authService } from '../api/authService';

const AuthContext = createContext(null);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is logged in on mount
    const storedUser = localStorage.getItem('user');
    const accessToken = localStorage.getItem('accessToken');
    
    if (storedUser && accessToken) {
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password);
      
      if (response.success) {
        const { accessToken, refreshToken, user: userData } = response.data;
        
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('user', JSON.stringify(userData));
        
        setUser(userData);
        return { success: true };
      }
      
      return { success: false, message: 'Login failed' };
    } catch (error) {
      const message = error.response?.data?.errors?.[0] || 'Login failed. Please try again.';
      return { success: false, message };
    }
  };

  const register = async (userData) => {
    try {
      const response = await authService.register(userData);
      
      if (response.success) {
        const { accessToken, refreshToken, user: newUser } = response.data;
        
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('user', JSON.stringify(newUser));
        
        setUser(newUser);
        return { success: true };
      }
      
      return { success: false, message: 'Registration failed' };
    } catch (error) {
      const message = error.response?.data?.errors?.[0] || 'Registration failed. Please try again.';
      return { success: false, message };
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  const value = {
    user,
    login,
    register,
    logout,
    isAuthenticated: !!user,
    loading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
