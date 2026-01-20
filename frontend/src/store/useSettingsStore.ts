import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type SettingsState = {
  directPrint: boolean;
  useLocalAgent: boolean;
  localAgentUrl: string;
  logoDataUrl?: string;
  toggleDirect: (v?:boolean)=>void;
  toggleLocalAgent: (v?:boolean)=>void;
  setLocalAgentUrl: (u:string)=>void;
  setLogo: (dataUrl?:string)=>void;
};

export const useSettingsStore = create<SettingsState>()(
  persist((set) => ({
    directPrint: false,
    useLocalAgent: false,
    localAgentUrl: 'http://localhost:3800/print',
    logoDataUrl: undefined,
    toggleDirect: (v) => set(s => ({ directPrint: typeof v === 'boolean' ? v : !s.directPrint })),
    toggleLocalAgent: (v) => set(s => ({ useLocalAgent: typeof v === 'boolean' ? v : !s.useLocalAgent })),
    setLocalAgentUrl: (u) => set({ localAgentUrl: u }),
    setLogo: (dataUrl) => set({ logoDataUrl: dataUrl }),
  }), { name: 'app-settings' })
);
