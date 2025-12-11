/**
 * WebUSB ESC/POS helper with device persistence (basic).
 * Note: WebUSB requires HTTPS or localhost.
 */
import { formatReceiptLines } from './escposEncoder';

const STORAGE_KEY = 'selected_printer_info';

export async function requestPrinter() {
  try {
    const device = await (navigator as any).usb.requestDevice({ filters: [] });
    await device.open();
    if (device.configuration === null) await device.selectConfiguration(1);
    await device.claimInterface(0);
    // Save a minimal reference in localStorage (cannot store device object)
    // We'll save vendorId/productId to help selection later (user still needs to grant permission).
    const info = { vendorId: device.vendorId, productId: device.productId };
    try { localStorage.setItem(STORAGE_KEY, JSON.stringify(info)); } catch(e){}
    return device;
  } catch (e) {
    console.error('WebUSB request failed', e);
    throw e;
  }
}

export async function getPreviouslySelectedPrinter() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const info = JSON.parse(raw);
    // Try to find devices matching info
    const devices = await (navigator as any).usb.getDevices();
    const found = devices.find((d:any)=>d.vendorId===info.vendorId && d.productId===info.productId);
    if (found) {
      if (!found.opened) {
        await found.open();
        if (found.configuration === null) await found.selectConfiguration(1);
        await found.claimInterface(0);
      }
      return found;
    }
    return null;
  } catch (e) {
    console.warn('getPreviouslySelectedPrinter failed', e);
    return null;
  }
}

export async function printRaw(device: any, data: Uint8Array) {
  try {
    // Use endpoint 1 by default; may need adjustment per printer
    await device.transferOut(1, data);
  } catch (e) {
    console.error('WebUSB print failed', e);
    throw e;
  }
}

export async function printReceipt(device: any, lines: string[]) {
  const payload = formatReceiptLines(lines);
  return printRaw(device, payload);
}
