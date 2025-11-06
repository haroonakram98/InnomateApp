import axios from "@/lib/utils/axios.js";
import { Customer, CreateCustomerDto, UpdateCustomerDto } from "@/types/customer.js";

export const getCustomers = async (): Promise<Customer[]> => {
  const res = await axios.get("/Customer");
  return res.data;
};

export const getCustomerById = async (id: number): Promise<Customer> => {
  const res = await axios.get(`/Customers${id}`);
  return res.data;
};

export const createCustomer = async (data: CreateCustomerDto): Promise<Customer> => {
  const res = await axios.post("/Customer", data);
  return res.data;
};

export const updateCustomer = async (id: number, data: UpdateCustomerDto): Promise<void> => {
  await axios.put(`/Customer/${id}`, data);
};

export const deleteCustomer = async (id: number): Promise<void> => {
  await axios.delete(`/Customer/${id}`);
};