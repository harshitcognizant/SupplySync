import axiosInstance from "./axiosInstance";

export const invoiceApi = {
  getAll: () => axiosInstance.get("/invoice"),
  getById: (id: number) => axiosInstance.get(`/invoice/${id}`),
  getByVendor: (vendorId: number) =>
    axiosInstance.get(`/invoice/vendor/${vendorId}`),
  submit: (data: {
    purchaseOrderId: number;
    goodsReceiptId: number;
    totalAmount: number;
  }) => axiosInstance.post("/invoice", data),
  review: (id: number, data: { status: string; rejectionReason?: string }) =>
    axiosInstance.put(`/invoice/${id}/review`, data),
  processPayment: (data: {
    invoiceId: number;
    amountPaid: number;
    paymentReference: string;
  }) => axiosInstance.post("/invoice/payment", data),
};