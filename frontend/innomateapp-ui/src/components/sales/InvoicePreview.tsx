import React, { useRef, useEffect } from "react";
import { useReactToPrint } from "react-to-print";
import { useSettingsStore } from "@/store/useSettingsStore.js";
import { useToastStore } from "@/store/useToastStore.js";
import { getPreviouslySelectedPrinter, printReceipt } from "@/utils/webusbEscPos.js";
import { SaleDTO } from "@/types/sale.js";
import Button from "@/components/ui/Button.js";
import { motion } from "framer-motion";

type Props = {
  sale: SaleDTO;
  onClose: () => void;
  autoPrint?: boolean;
  directPrint?: boolean;
};

const InvoicePreview: React.FC<Props> = ({ sale, onClose, autoPrint, directPrint }) => {
  const printRef = useRef<HTMLDivElement>(null);
  const logoData = useSettingsStore((s) => s.logoDataUrl);
  const useLocalAgent = useSettingsStore((s) => s.useLocalAgent);
  const localAgentUrl = useSettingsStore((s) => s.localAgentUrl);
  const toast = useToastStore((s) => s.push);

  const handlePrint = useReactToPrint({ contentRef: printRef });

  // 🖨️ Direct thermal printing via WebUSB
  useEffect(() => {
    if (!directPrint) return;
    let mounted = true;
    (async () => {
      try {
        let device = await getPreviouslySelectedPrinter();
        if (!device) {
          device = await (navigator as any).usb.requestDevice({ filters: [] });
          await device.open();
          if (device.configuration === null) await device.selectConfiguration(1);
          await device.claimInterface(0);
        }
        if (!mounted) return;

        const header = sale.customer?.name || sale.customerName || "Customer";
        const lines = [
          "Your Company Name",
          header,
          "Invoice: " + (sale.invoiceNo || sale.saleId),
          "",
          "Items:",
          ...sale.saleDetails.map(
            (sd) =>
              `${sd.productName || "#" + sd.productId} x${sd.quantity} @ ${sd.unitPrice.toFixed(
                2
              )} = ${(sd.quantity * sd.unitPrice).toFixed(2)}`
          ),
          "",
          "Total: " + sale.totalAmount.toFixed(2),
          "",
          "Thank you!",
        ];
        await printReceipt(device, lines);
      } catch (e) {
        console.warn("Direct print failed, falling back to browser print", e);
      }
    })();
    return () => {
      mounted = false;
    };
  }, [directPrint, sale]);

  // 🖨️ Print via Local Agent (Node service)
  useEffect(() => {
    if (!useLocalAgent) return;
    if (!sale) return;
    (async () => {
      try {
        await fetch(localAgentUrl, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ type: "receipt", sale }),
        });
        toast("Printed via local agent", "success");
      } catch (e) {
        console.warn("Local agent print failed", e);
        toast("Local print failed", "error");
      }
    })();
  }, [useLocalAgent, sale, localAgentUrl, toast]);

  return (
    <motion.div
      className="fixed inset-0 bg-black/60 flex items-center justify-center z-50"
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
    >
      <motion.div
        className="bg-white rounded-2xl shadow-2xl w-[90vw] max-w-[850px] h-[90vh] flex flex-col overflow-hidden"
        initial={{ scale: 0.9, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
      >
        {/* Header */}
        <div className="flex justify-between items-center border-b px-5 py-3 bg-gray-50 sticky top-0 z-10">
          <div>
            <h2 className="text-lg font-semibold">Invoice #{sale.invoiceNo || sale.saleId}</h2>
            <p className="text-xs text-gray-500">
              {sale.saleId && sale.saleId < 0 ? "Saving..." : "Saved"}
            </p>
          </div>
          <div className="flex gap-2">
            <Button onClick={handlePrint}>🖨️ Print</Button>
            <Button variant="outline" onClick={onClose}>
              ✖ Close
            </Button>
          </div>
        </div>

        {/* Scrollable content */}
        <div
          ref={printRef}
          className="overflow-y-auto px-6 py-4 flex-1 text-gray-800 font-sans text-sm"
        >
          {/* Header */}
          <div className="text-center mb-4">
            {logoData && <img src={logoData} alt="logo" className="mx-auto h-16 mb-2" />}
            <h1 className="text-2xl font-bold">Umer Electric and Sanitary Store</h1>
            <p className="text-gray-600 text-xs">opposite side Gate.1 Al Rehman Garden phase.2 Sharaqpur Road Lahore 
 — Phone: 03234642768</p>
          </div>

          {/* Customer Info */}
          <div className="grid grid-cols-2 mb-4 text-sm">
            <div>
              <p>
                <strong>Customer:</strong> {sale.customer?.name || sale.customerName}
              </p>
              {sale.customer?.phone && (
                <p>
                  <strong>Phone:</strong> {sale.customer.phone}
                </p>
              )}
            </div>
            <div className="text-right">
              <p>
                <strong>Date:</strong>{" "}
                {new Date(sale.saleDate || "").toLocaleString()}
              </p>
              <p>
                <strong>Invoice #:</strong> {sale.invoiceNo || sale.saleId}
              </p>
            </div>
          </div>

          {/* Products Table */}
          <table className="w-full border-collapse border border-gray-300 mb-4 text-sm">
            <thead className="bg-gray-100">
              <tr>
                <th className="border p-2 text-left">Product</th>
                <th className="border p-2 text-right">Qty</th>
                <th className="border p-2 text-right">Unit Price</th>
                <th className="border p-2 text-right">Total</th>
              </tr>
            </thead>
            <tbody>
              {sale.saleDetails.map((item, i) => (
                <tr key={i} className="hover:bg-gray-50">
                  <td className="border p-2">{item.productName || `#${item.productId}`}</td>
                  <td className="border p-2 text-right">{item.quantity}</td>
                  <td className="border p-2 text-right">{item.unitPrice.toFixed(2)}</td>
                  <td className="border p-2 text-right">
                    {(item.quantity * item.unitPrice).toFixed(2)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {/* Totals */}
          <div className="flex justify-end text-right mb-4">
            <div className="w-1/3">
              <div className="flex justify-between border-t py-1">
                <span className="font-semibold">Grand Total:</span>
                <span className="font-bold">{sale.totalAmount.toFixed(2)}</span>
              </div>
            </div>
          </div>

          {/* Payments */}
          {sale.payments?.length > 0 && (
            <div className="mt-4">
              <h3 className="font-semibold mb-1">Payments</h3>
              <ul className="list-disc pl-5 text-sm">
                {sale.payments.map((p, i) => (
                  <li key={i}>
                    {p.paymentMethod} — {p.amount.toFixed(2)}{" "}
                    {p.referenceNo ? `(Ref: ${p.referenceNo})` : ""}
                  </li>
                ))}
              </ul>
            </div>
          )}

          <div className="text-center mt-6 text-xs text-gray-500">
            Thank you for shopping with us!
          </div>
        </div>
      </motion.div>
    </motion.div>
  );
};

export default InvoicePreview;
