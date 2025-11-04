import axios from "@/lib/utils/axios.js";
import { CategoryDTO } from "@/types/category.js";

export const categoryApi = {
    getAll: async (): Promise<CategoryDTO[]> => {
        const res = await axios.get("/v1/Categories");
        return res.data;
    },
};