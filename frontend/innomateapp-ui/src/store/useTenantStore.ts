import { create } from 'zustand';
import { tenantApi, TenantResponseDto, TenantOnboardingDto } from '@/api/tenantApi.js';

interface TenantState {
    tenants: TenantResponseDto[];
    isLoading: boolean;
    error: string | null;

    // Actions
    fetchTenants: () => Promise<void>;
    onboardTenant: (data: TenantOnboardingDto) => Promise<void>;
    clearError: () => void;
}

export const useTenantStore = create<TenantState>((set) => ({
    tenants: [],
    isLoading: false,
    error: null,

    fetchTenants: async () => {
        set({ isLoading: true, error: null });
        try {
            const tenants = await tenantApi.getAll();
            set({ tenants, isLoading: false });
        } catch (err: any) {
            set({ error: err.response?.data?.message || 'Failed to fetch tenants', isLoading: false });
        }
    },

    onboardTenant: async (data: TenantOnboardingDto) => {
        set({ isLoading: true, error: null });
        try {
            await tenantApi.onboard(data);
            const tenants = await tenantApi.getAll();
            set({ tenants, isLoading: false });
        } catch (err: any) {
            const message = err.response?.data?.message || 'Failed to onboard tenant';
            set({ error: message, isLoading: false });
            throw new Error(message);
        }
    },

    clearError: () => set({ error: null })
}));

// Selectors for convenience
export const useTenants = () => useTenantStore((state) => state.tenants);
export const useTenantsLoading = () => useTenantStore((state) => state.isLoading);
export const useTenantsError = () => useTenantStore((state) => state.error);
export const useTenantActions = () => ({
    fetchTenants: useTenantStore((state) => state.fetchTenants),
    onboardTenant: useTenantStore((state) => state.onboardTenant),
    clearError: useTenantStore((state) => state.clearError)
});
