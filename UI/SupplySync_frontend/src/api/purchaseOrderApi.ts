import axiosInstance from "./axiosInstance";

export const purchaseOrderApi = {
  getAll: () => axiosInstance.get("/purchaseorder"),
  getById: (id: number) => axiosInstance.get(`/purchaseorder/${id}`),
  getByVendor: (vendorId: number) =>
    axiosInstance.get(`/purchaseorder/vendor/${vendorId}`),
  create: (data: {
    vendorId: number;
    contractId: number;
    expectedDeliveryDate: string;
    items: { itemName: string; quantity: number; unitPrice: number }[];
  }) => axiosInstance.post("/purchaseorder", data),
  updateStatus: (id: number, status: string) =>
    axiosInstance.put(`/purchaseorder/${id}/status?status=${status}`),
  cancel: (id: number) => axiosInstance.put(`/purchaseorder/${id}/cancel`),
};