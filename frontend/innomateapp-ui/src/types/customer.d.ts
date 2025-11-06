export interface Customer {
  customerId: number;
  name: string;
  email: string;
  phone?: string;
  address?: string;
  isActive: boolean;
  createdAt?: string;
}

export type CreateCustomerDto = Omit<Customer, "customerId" | "createdAt">;
export type UpdateCustomerDto = Partial<CreateCustomerDto>;