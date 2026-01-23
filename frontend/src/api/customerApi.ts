// api/customerApi.ts
import axios from "@/lib/utils/axios.js";
import {
  CustomerDTO,
  CreateCustomerDto,
  UpdateCustomerDto,
} from "@/types/customer.js";

export const customerApi = {
  getAll: async (search?: string): Promise<CustomerDTO[]> => {
    const { data } = await axios.get<CustomerDTO[]>("/Customer", {
      params: { search }
    });
    return data;
  },

  getById: async (id: number): Promise<CustomerDTO> => {
    const { data } = await axios.get<CustomerDTO>(`/Customer/${id}`);
    return data;
  },

  create: async (payload: CreateCustomerDto): Promise<CustomerDTO> => {
    const { data } = await axios.post<CustomerDTO>("/Customer", payload);
    return data;
  },

  update: async (
    id: number,
    payload: UpdateCustomerDto
  ): Promise<CustomerDTO> => {
    const { data } = await axios.put<CustomerDTO>(`/Customer/${id}`, payload);
    return data;
  },

  delete: async (id: number): Promise<void> => {
    await axios.delete(`/Customer/${id}`);
  },

  toggleStatus: async (id: number): Promise<{ isActive: boolean }> => {
    const { data } = await axios.patch(`/Customer/${id}/toggle-status`);
    return data;
  },
};
