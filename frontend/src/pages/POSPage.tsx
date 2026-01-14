// pages/POSPage.tsx
import { useState } from "react";
import MainLayout from "@/components/layout/MainLayout.js";
import SaleForm from "@/components/sales/SaleForm.js";
import { SaleDTO } from "@/types/sale.js";
import InvoicePreview from "@/components/sales/InvoicePreview.js";

export default function POSPage() {
  const [invoiceSale, setInvoiceSale] = useState<SaleDTO | null>(null);

  return (
    <MainLayout>
      <SaleForm 
        onShowInvoice={setInvoiceSale} 
        onClose={() => {}} 
      />

      {invoiceSale && (
        <InvoicePreview
          sale={invoiceSale}
          onClose={() => setInvoiceSale(null)}
          autoPrint={false}
          directPrint={false}
        />
      )}
    </MainLayout>
  );
}