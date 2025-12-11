import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type SettingsState = {
  directPrint: boolean;
  useLocalAgent: boolean;
  localAgentUrl: string;
  logoDataUrl?: string;
  toggleDirectPrint: (v?: boolean) => void; // Changed from toggleDirect
  toggleLocalAgent: (v?: boolean) => void;
  setLocalAgentUrl: (u: string) => void;
  setLogo: (dataUrl?: string) => void;
};

export const useSettingsStore = create<SettingsState>()(
  persist((set) => ({
    directPrint: false,
    useLocalAgent: false,
    localAgentUrl: 'http://localhost:3800/print',
    logoDataUrl: undefined,
    toggleDirectPrint: (v) => set((s) => ({ // Changed from toggleDirect
      directPrint: typeof v === 'boolean' ? v : !s.directPrint 
    })),
    toggleLocalAgent: (v) => set((s) => ({ 
      useLocalAgent: typeof v === 'boolean' ? v : !s.useLocalAgent 
    })),
    setLocalAgentUrl: (u) => set({ localAgentUrl: u }),
    setLogo: (dataUrl) => set({ logoDataUrl: dataUrl }),
  }), { 
    name: 'app-settings' 
  })
);