"use client"

import { useState } from "react"
import MainLayout from "@/components/layout/MainLayout.js"
import SaleForm from "@/components/sales/SaleForm.js"
import { SaleTable } from "@/components/sales/SaleTable.js"
import Button from "@/components/ui/Button.js"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/Dialog.js"
import { motion } from "framer-motion"

export default function SalesPage() {
  const [open, setOpen] = useState(false)
  const [editId, setEditId] = useState<number | null>(null)
  const [refreshKey, setRefreshKey] = useState(0)

  const handleAdd = () => {
    setEditId(null)
    setOpen(true)
  }

  const handleEdit = (id: number) => {
    setEditId(id)
    setOpen(true)
  }

  const handleClose = (refresh = false) => {
    setOpen(false)
    setEditId(null)
    if (refresh) setRefreshKey((prev) => prev + 1)
  }

  return (
    <MainLayout>
      <div className="p-6 space-y-4">
        {/* Header Section */}
        <div className="flex items-center justify-between">
          <motion.h1 className="text-2xl font-semibold" initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }}>
            Sales
          </motion.h1>

          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }}>
            <Button onClick={handleAdd}>+ New Sale</Button>
          </motion.div>
        </div>

        {/* Sales Table */}
        <motion.div
          className="bg-transparent"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.1 }}
        >
          <SaleTable key={refreshKey} onEdit={handleEdit} />
        </motion.div>

        {/* Add/Edit Sale Dialog */}
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>{editId ? "Edit Sale" : "Add New Sale"}</DialogTitle>
            </DialogHeader>
            <SaleForm saleId={editId} onClose={(refresh?: boolean) => handleClose(!!refresh)} />
          </DialogContent>
        </Dialog>
      </div>
    </MainLayout>
  )
}
