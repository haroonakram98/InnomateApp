import axios from "@/lib/utils/axios.js";
import { DashboardResponse } from "@/types/dashboard.js";

export const dashboardService = {
    getStats: async (): Promise<DashboardResponse> => {
        const res = await axios.get("/Dashboard/stats");
        return res.data;
    },
};
