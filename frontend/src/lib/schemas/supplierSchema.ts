import { z } from 'zod';

export const supplierSchema = z.object({
    name: z.string()
        .min(1, 'Supplier name is required')
        .max(200, 'Supplier name cannot exceed 200 characters'),

    email: z.string()
        .min(1, 'Email is required')
        .email('Invalid email format')
        .max(200, 'Email cannot exceed 200 characters'),

    phone: z.string()
        .min(1, 'Phone number is required')
        .max(20, 'Phone number cannot exceed 20 characters'),

    address: z.string()
        .max(500, 'Address cannot exceed 500 characters')
        .optional()
        .nullable()
        .or(z.literal('')),

    contactPerson: z.string()
        .max(100, 'Contact person name cannot exceed 100 characters')
        .optional()
        .nullable()
        .or(z.literal('')),

    notes: z.string()
        .optional()
        .nullable()
        .or(z.literal('')),
});

export type SupplierSchemaType = z.infer<typeof supplierSchema>;
