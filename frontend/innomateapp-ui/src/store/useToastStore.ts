import { create } from 'zustand';

type Toast = { id: number; message: string; type?: 'success'|'error'|'info'|'warning' };
type State = {
  toasts: Toast[];
  push: (m: string, t?: Toast['type']) => void;
  remove: (id: number) => void;
};

export const useToastStore = create<State>((set) => ({
  toasts: [],
  push: (message, type='info') =>
    set((s) => ({ toasts: [...s.toasts, { id: Date.now(), message, type }] })),
  remove: (id) => set((s) => ({ toasts: s.toasts.filter(t => t.id !== id) })),
}));
