
import axios from "axios";
import { LoginRequest, RegisterRequest, AuthResponse } from "@/types/auth.js";

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api/auth";

export const login = async (data: LoginRequest): Promise<AuthResponse> => {
  const response = await axios.post(`${API_URL}/login`, data);
  return response.data;
};

export const register = async (data: RegisterRequest): Promise<AuthResponse> => {
    debugger;
  const response = await axios.post(`${API_URL}/register`, data);
  debugger;
  return response.data;
};
