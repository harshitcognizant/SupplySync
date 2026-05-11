import axiosInstance from "./axiosInstance";

export const adminApi = {
  getUsers: () => axiosInstance.get("/admin/users"),
  toggleActive: (id: string) =>
    axiosInstance.put(`/admin/users/${id}/toggle-active`),
  resetPassword: (id: string, newPassword: string) =>
    axiosInstance.put(`/admin/users/${id}/reset-password`, JSON.stringify(newPassword)),
  changeRole: (id: string, role: string) =>
    axiosInstance.put(`/admin/users/${id}/change-role`, JSON.stringify(role)),
};