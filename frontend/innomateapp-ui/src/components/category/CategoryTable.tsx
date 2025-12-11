'use client'

import { useEffect } from 'react'
import Button from '@/components/ui/Button.js'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
  TableCaption,
} from '@/components/ui/Table.js'
import { useCategoryStore } from '@/store/useCategoryStore.js'
import { Edit, Trash2, Plus, Search } from 'lucide-react'
import Input  from '@/components/ui/Input.js'
import { Dialog, DialogTrigger } from '@radix-ui/react-dialog'
import { AddCategoryDialog } from './AddCategoryDialog.js'

export function CategoryTable() {
  const { categories, fetchCategories, loading } = useCategoryStore()

  useEffect(() => {
    fetchCategories()
  }, [fetchCategories])

  if (loading)
    return (
      <div className="flex items-center justify-center h-64">
        <span className="text-muted-foreground animate-pulse">Loading...</span>
      </div>
    )

  return (
    <div className="p-6 space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight">
            Category Management
          </h2>
          <p className="text-sm text-muted-foreground">
            Add, view, edit, and delete product categories.
          </p>
        </div>

        <Dialog>
          <DialogTrigger asChild>
            <Button className="bg-blue-600 hover:bg-blue-700">
              <Plus className="mr-2 h-4 w-4" />
              Add New Category
            </Button>
          </DialogTrigger>
          <AddCategoryDialog />
        </Dialog>
      </div>

      <div className="flex items-center justify-between mt-4">
        <div className="relative w-64">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input placeholder="Search categories..." className="pl-8" />
        </div>
      </div>

      <div className="border rounded-xl overflow-hidden bg-card shadow-sm">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[150px]">Category ID</TableHead>
              <TableHead>Category Name</TableHead>
              <TableHead>Description</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {categories.length === 0 ? (
              <TableRow>
                <TableCell colSpan={4} className="text-center text-muted-foreground">
                  No categories found.
                </TableCell>
              </TableRow>
            ) : (
              categories.map((category) => (
                <TableRow key={category.categoryId}>
                  {/* <TableCell>{category.code}</TableCell> */}
                  <TableCell className="font-medium">{category.name}</TableCell>
                  <TableCell>{category.description}</TableCell>
                  <TableCell className="text-right space-x-2">
                    <Button variant="ghost" size="icon">
                      <Edit className="h-4 w-4 text-blue-500" />
                    </Button>
                    <Button variant="ghost" size="icon">
                      <Trash2 className="h-4 w-4 text-red-500" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        <TableCaption>
          Showing {categories.length} results
        </TableCaption>
      </div>
    </div>
  )
}
