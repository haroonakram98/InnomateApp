import { z } from 'zod';

export const categorySchema = z.object({
    name: z.string()
        .min(1, 'Category name is required')
        .max(100, 'Category name cannot exceed 100 characters'),
    description: z.string()
        .max(500, 'Description cannot exceed 500 characters')
        .optional()
        .or(z.literal(''))
});

export type CategoryFormData = z.infer<typeof categorySchema>;
