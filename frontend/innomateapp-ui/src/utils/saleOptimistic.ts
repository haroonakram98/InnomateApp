import { useSaleStore } from "@/store/useSaleStore.js";
import type { SaleDTO } from "@/types/sale.js";

/**
 * Helpers to insert/replace/remove optimistic (temporary) sales into master store.
 */

export function insertTempSale(temp: SaleDTO) {
  useSaleStore.setState((s) => ({ sales: [temp, ...s.sales] }));
}

export function replaceTempSale(tempId: number, real: SaleDTO) {
  useSaleStore.setState((s) => ({ sales: s.sales.map(x => x.saleId === tempId ? real : x) }));
}

export function removeTempSale(tempId: number) {
  useSaleStore.setState((s) => ({ sales: s.sales.filter(x => x.saleId !== tempId) }));
}
