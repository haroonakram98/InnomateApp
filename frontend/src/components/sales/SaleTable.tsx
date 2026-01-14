import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table.js"
import Button from "@/components/ui/Button.js"
import type { SaleDTO } from "@/types/sale.js"

interface SaleTableProps {
  onEdit: (id: number) => void;
  onDelete: (id: number) => void;
  sales: SaleDTO[];
  loading?: boolean;
}

export function SaleTable({ onEdit, onDelete, sales, loading }: SaleTableProps) {
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
              <TableCell className="font-medium">{sale?.invoiceNo || sale.saleId}</TableCell>
              <TableCell>{sale?.customer?.name}</TableCell>
              <TableCell>Rs. {sale.totalAmount.toFixed(2)}</TableCell>
              <TableCell>{sale.saleDate ? new Date(sale.saleDate).toLocaleDateString() : "-"}</TableCell>
              <TableCell>
                <Button variant="outline" size="sm" onClick={() => sale.saleId && onEdit(sale.saleId)}>
                  Edit
                </Button>
              </TableCell>
              <TableCell>
                <Button variant="danger" size="sm" onClick={() => sale.saleId && onDelete(sale.saleId)}>
                  Delete
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
