import axiosInstance from "./axiosInstance";

export const contractApi = {
  getAll: () => axiosInstance.get("/contract"),
  getById: (id: number) => axiosInstance.get(`/contract/${id}`),
  getByVendor: (vendorId: number) =>
    axiosInstance.get(`/contract/vendor/${vendorId}`),
  create: (data: {
    vendorId: number;
    startDate: string;
    endDate: string;
    paymentTerms: string;
    deliveryTerms: string;
    itemPricing: string;
  }) => axiosInstance.post("/contract", data),
  activate: (id: number) => axiosInstance.put(`/contract/${id}/activate`),
  close: (id: number) => axiosInstance.put(`/contract/${id}/close`),
};