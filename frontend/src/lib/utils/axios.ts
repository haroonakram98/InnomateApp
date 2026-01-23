import axios from "axios";
import { useAuthStore } from "@/store/useAuthStore.js";

const API_BASE_URL = import.meta.env.VITE_API_URL || "https://localhost:7219/api";

const instance = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
  // timeout: 10000000, // ⏱ prevents infinite waiting
});

// ✅ Automatically attach token
instance.interceptors.request.use(
  (config) => {
    const token = useAuthStore.getState().token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// ✅ Centralized error handling
instance.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handle network errors
    if (!error.response) {
      console.error("Network error:", error.message);
      return Promise.reject({
        message: "Unable to connect to the server. Please check your network.",
        status: null,
      });
    }

    const { status, data } = error.response;

    // Handle common HTTP errors
    switch (status) {
      case 400:
        console.warn("Bad Request:", data?.detail || data?.message || "Invalid request.");
        break;
      case 401:
        console.warn("Unauthorized. Logging out...");
        useAuthStore.getState().logout?.();
        break;
      case 500:
        console.error("Server Error:", data?.detail || data?.message || "An unexpected error occurred.");
        break;
    }

    // Normalize error shape for UI (Support ProblemDetails and legacy message)
    return Promise.reject({
      message: data?.detail || data?.message || "An unexpected error occurred.",
      errors: data?.errors || null, // Capture structured validation errors
      status,
    });
  }
);

export default instance;
