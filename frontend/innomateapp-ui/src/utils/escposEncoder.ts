/**
 * Minimal ESC/POS encoder for common Epson/Star printers.
 * Supports: text, bold, underline, align, line feed, cut.
 * This is intentionally small — for complex receipts use escpos-encoder library.
 */

function concatUint8(arrays: Uint8Array[]) {
  const total = arrays.reduce((sum, a) => sum + a.length, 0);
  const out = new Uint8Array(total);
  let offset = 0;
  for (const a of arrays) {
    out.set(a, offset);
    offset += a.length;
  }
  return out;
}

export function text(cmd: string) {
  return new TextEncoder().encode(cmd);
}

export function lf() {
  return new Uint8Array([0x0a]); // line feed
}

export function initialize() {
  return new Uint8Array([0x1b, 0x40]); // ESC @
}

export function bold(on = true) {
  return new Uint8Array([0x1b, 0x45, on ? 1 : 0]); // ESC E n
}

export function underline(on = true) {
  return new Uint8Array([0x1b, 0x2d, on ? 1 : 0]); // ESC - n
}

export function align(mode: 'left'|'center'|'right' = 'left') {
  const m = mode === 'left' ? 0 : mode === 'center' ? 1 : 2;
  return new Uint8Array([0x1b, 0x61, m]); // ESC a n
}

export function cut() {
  return new Uint8Array([0x1d, 0x56, 0x41, 0x00]); // GS V A n (partial cut)
}

export function formatReceiptLines(lines: string[]) {
  const parts = [];
  parts.push(initialize());
  parts.push(align('center'));
  parts.push(bold(true));
  parts.push(text(lines[0] || ''));
  parts.push(lf());
  parts.push(bold(false));
  parts.push(align('left'));
  for (let i = 1; i < lines.length; i++) {
    parts.push(text(lines[i]));
    parts.push(lf());
  }
  parts.push(lf());
  parts.push(cut());
  return concatUint8(parts);
}
