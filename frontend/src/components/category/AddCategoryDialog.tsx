'use client'

import { DialogContent, DialogTitle, DialogClose } from '@radix-ui/react-dialog'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import Button  from '@/components/ui/Button.js'
import Input from '@/components/ui/Input.js'
import Label from '@/components/ui/Label.js'
import { useCategoryStore } from '@/store/useCategoryStore.js'
import { CategoryDTO, CreateCategoryDTO } from '@/types/category.js'

const schema = z.object({
  name: z.string().min(1, 'Name is required'),
  description: z.string().optional(),
})

type FormData = z.infer<typeof schema>

export function AddCategoryDialog() {
  const { addCategory } = useCategoryStore()
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const onSubmit = (data: FormData) => {
  const newCategory: Omit<CreateCategoryDTO, 'categoryId'> = {
    name: data.name,
    description: data.description
  }

  addCategory(newCategory)
  reset()
}

  return (
    <DialogContent className="bg-background p-6 rounded-xl shadow-lg space-y-4 w-[400px]">
      <DialogTitle className="text-lg font-semibold">Add New Category</DialogTitle>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <Label htmlFor="name">Category Name</Label>
          <Input id="name" {...register('name')} />
          {errors.name && (
            <p className="text-red-500 text-sm mt-1">{errors.name.message}</p>
          )}
        </div>
        <div>
          <Label htmlFor="description">Description</Label>
          <Input id="description" {...register('description')} />
        </div>
        <div className="flex justify-end space-x-2 pt-2">
          <DialogClose asChild>
            <Button type="button" variant="outline">Cancel</Button>
          </DialogClose>
          <Button type="submit" className="bg-blue-600 hover:bg-blue-700">
            Save
          </Button>
        </div>
      </form>
    </DialogContent>
  )
}
