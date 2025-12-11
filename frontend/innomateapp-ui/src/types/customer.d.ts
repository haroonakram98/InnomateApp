export interface CustomerDTO {
  customerId: number;
  name: string;
  email?: string;
  phone?: string;
  address?: string;
  createdAt?: string;
}


export type CreateCustomerDto = Omit<CustomerDTO, "customerId" | "createdAt">;
export type UpdateCustomerDto = Partial<CreateCustomerDto> & { customerId: number };