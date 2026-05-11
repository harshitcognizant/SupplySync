import axiosInstance from "./axiosInstance";

export const complianceApi = {
  getAll: () => axiosInstance.get("/compliance"),
  getByEntity: (entityType: string, entityId: number) =>
    axiosInstance.get(`/compliance/entity?entityType=${entityType}&entityId=${entityId}`),
  getFailed: () => axiosInstance.get("/compliance/failed"),
  performCheck: (data: {
    entityType: string;
    entityId: number;
    status: string;
    remarks: string;
  }) => axiosInstance.post("/compliance", data),
};