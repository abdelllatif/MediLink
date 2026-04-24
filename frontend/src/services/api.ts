import axios from 'axios';

// Create an Axios instance pointing to the ASP.NET Core API
const api = axios.create({
  baseURL: 'https://localhost:7198/api', // Default local .NET HTTPS port
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor to add the JWT token to requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export default api;
