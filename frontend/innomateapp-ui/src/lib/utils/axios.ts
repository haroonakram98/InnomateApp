import axios from "axios";
import { useAuthStore } from "@/store/useAuthStore.js";

const instance = axios.create({
  baseURL: "http://localhost:5180/api/v1", // configurable via env
  headers: { "Content-Type": "application/json" },
});

// Auto attach token
instance.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default instance;