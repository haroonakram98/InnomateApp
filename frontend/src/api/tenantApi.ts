import axios from "@/lib/utils/axios.js";

export interface TenantOnboardingDto {
    tenantName: string;
    tenantCode: string;
    adminUsername: string;
    adminEmail: string;
    adminPassword: string;
}

export interface TenantResponseDto {
    tenantId: number;
    name: string;
    code: string;
    adminUsername: string;
}

export const tenantApi = {
    onboard: async (data: TenantOnboardingDto): Promise<TenantResponseDto> => {
        const res = await axios.post("/tenants/onboard", data);
        return res.data;
    },
    getAll: async (): Promise<TenantResponseDto[]> => {
        const res = await axios.get("/tenants");
        return res.data;
    },
    getById: async (id: number): Promise<TenantResponseDto> => {
        const res = await axios.get(`/tenants/${id}`);
        return res.data;
    },
};
