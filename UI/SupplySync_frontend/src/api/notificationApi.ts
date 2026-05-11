import axiosInstance from "./axiosInstance";

export const notificationApi = {
  getMine: () => axiosInstance.get("/notification"),
  getUnreadCount: () => axiosInstance.get("/notification/unread-count"),
  markAsRead: (id: number) => axiosInstance.put(`/notification/${id}/read`),
  markAllAsRead: () => axiosInstance.put("/notification/read-all"),
};