import { z } from "zod";

export const zNumber = (message: string) =>
  z.preprocess((val) => {
    if (val === "" || val === null || val === undefined) return undefined;
    const num = Number(val);
    return isNaN(num) ? undefined : num;
  }, z.number().refine((val) => !isNaN(val), { message }));