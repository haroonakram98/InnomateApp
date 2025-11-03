
import axios from "@/lib/utils/axios.js";
import { LoginRequest, RegisterRequest, AuthResponse } from "@/types/auth.js";


export const login = async (data: LoginRequest): Promise<AuthResponse> => {
  try{
    const response = await axios.post("/Auth/login", data);
    return response.data;
  }
  catch (error: any) {
    debugger;
    throw new Error(error.message || "Login failed. Please try again.");
  }
};

export const register = async (data: RegisterRequest): Promise<AuthResponse> => {
  try
  {
    const response = await axios.post("/auth/register", data);
    return response.data;
  }
  catch (error: any) {
    throw new Error(error.message || "Registeration Failed. Please try again.");
  }
};
