"use client"

import { useState, useEffect } from "react"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table.js"
import Button from "@/components/ui/Button.js"
import type { SaleDTO } from "@/types/sale.js"

interface SaleTableProps {
  onEdit: (id: number) => void
}

export function SaleTable({ onEdit }: SaleTableProps) {
  const [sales, setSales] = useState<SaleDTO[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const mockSales: SaleDTO[] = [
      {
        saleId: 1,
        customerId: 101,
        customerName: "John Doe",
        invoiceNo: "INV-001",
        totalAmount: 5000,
        saleDate: "2025-01-01",
        saleDetails: [
          {
            productId: 1,
            quantity: 2,
            unitPrice: 2500,
            total: 5000,
          },
        ],
        payments: [
          {
            paymentMethod: "Cash",
            amount: 5000,
          },
        ],
      },
    ]

    setTimeout(() => {
      setSales(mockSales)
      setLoading(false)
    }, 500)
  }, [])

  if (loading) {
    return <div className="text-center py-8">Loading...</div>
  }

  if (sales.length === 0) {
    return (
      <div className="text-center py-8 text-muted-foreground">
        No sales records found. Create your first sale to get started.
      </div>
    )
  }

  return (
    <div className="rounded-lg border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Invoice No</TableHead>
            <TableHead>Customer</TableHead>
            <TableHead>Amount</TableHead>
            <TableHead>Date</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {sales.map((sale) => (
            <TableRow key={sale.saleId}>
              <TableCell className="font-medium">{sale.invoiceNo}</TableCell>
              <TableCell>{sale.customerName}</TableCell>
              <TableCell>Rs. {sale.totalAmount.toFixed(2)}</TableCell>
              <TableCell>{sale.saleDate ? new Date(sale.saleDate).toLocaleDateString() : "-"}</TableCell>
              <TableCell>
                <Button variant="outline" size="sm" onClick={() => sale.saleId && onEdit(sale.saleId)}>
                  Edit
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
