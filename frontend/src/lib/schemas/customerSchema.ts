import { z } from 'zod';

export const customerSchema = z.object({
    name: z.string()
        .min(1, 'Customer name is required')
        .max(200, 'Customer name cannot exceed 200 characters'),

    email: z.string()
        .email('Invalid email format')
        .max(200, 'Email cannot exceed 200 characters')
        .optional()
        .nullable()
        .or(z.literal('')),

    phone: z.string()
        .max(20, 'Phone number cannot exceed 20 characters')
        .optional()
        .nullable()
        .or(z.literal('')),

    address: z.string()
        .max(500, 'Address cannot exceed 500 characters')
        .optional()
        .nullable()
        .or(z.literal('')),
});

export type CustomerSchemaType = z.infer<typeof customerSchema>;
