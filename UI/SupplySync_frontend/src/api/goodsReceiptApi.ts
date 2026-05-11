import axiosInstance from "./axiosInstance";

export const goodsReceiptApi = {
  getAll: () => axiosInstance.get("/goodsreceipt"),
  getById: (id: number) => axiosInstance.get(`/goodsreceipt/${id}`),
  getByPO: (poId: number) => axiosInstance.get(`/goodsreceipt/po/${poId}`),
  create: (data: {
    purchaseOrderId: number;
    remarks: string;
    status: string;
    items: { itemName: string; receivedQuantity: number; condition: string }[];
  }) => axiosInstance.post("/goodsreceipt", data),
};