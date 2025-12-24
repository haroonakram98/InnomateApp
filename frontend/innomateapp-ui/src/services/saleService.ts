import axios from "@/lib/utils/axios.js";
import { SaleDTO } from "@/types/sale.js";

export const saleService = {
    getAll: async (): Promise<SaleDTO[]> => {
        const res = await axios.get("/Sales");
        return res.data;
    },
    getById: async (id: number): Promise<SaleDTO> => {
        const res = await axios.get(`/Sales/${id}`);
        return res.data;
    },
};
