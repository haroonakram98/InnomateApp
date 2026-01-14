import api from "@/lib/utils/axios.js"

export interface ReturnDetailRequest {
    productId: number;
    quantity: number;
}

export interface CreateReturnRequest {
    saleId: number;
    reason?: string;
    returnDetails: ReturnDetailRequest[];
}

export interface ReturnDetailResponse {
    productId: number;
    productName?: string;
    quantity: number;
    refundAmount: number;
}

export interface ReturnResponse {
    returnId: number;
    saleId: number;
    returnDate: string;
    totalRefund: number;
    reason?: string;
    returnDetails: ReturnDetailResponse[];
}

export const returnService = {
    createReturn: async (data: CreateReturnRequest): Promise<ReturnResponse> => {
        const response = await api.post("/returns", data);
        return response.data;
    },

    getReturnById: async (id: number): Promise<ReturnResponse> => {
        const response = await api.get(`/returns/${id}`);
        return response.data;
    },

    getReturnsBySaleId: async (saleId: number): Promise<ReturnResponse[]> => {
        const response = await api.get(`/returns/sale/${saleId}`);
        return response.data;
    }
};
