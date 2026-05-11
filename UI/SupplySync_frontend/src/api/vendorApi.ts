import axiosInstance from "./axiosInstance";

export const vendorApi = {
  getAll: () => axiosInstance.get("/vendor"),
  getById: (id: number) => axiosInstance.get(`/vendor/${id}`),
  getMyProfile: () => axiosInstance.get("/vendor/my-profile"),
  updateStatus: (id: number, data: { status: string; rejectionReason?: string }) =>
    axiosInstance.put(`/vendor/${id}/status`, data),
  suspend: (id: number) => axiosInstance.put(`/vendor/${id}/suspend`),
  reapply: (data: {
  taxNumber: string;
  licenseNumber: string;
  documentPath: string;
  contactPhone: string;
  address: string;
}) => axiosInstance.put("/vendor/reapply", data),
};