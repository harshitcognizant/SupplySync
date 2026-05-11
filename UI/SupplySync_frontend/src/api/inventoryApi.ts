import axiosInstance from "./axiosInstance";

export const inventoryApi = {
  getAll: () => axiosInstance.get("/inventory"),
  getById: (id: number) => axiosInstance.get(`/inventory/${id}`),
  create: (data: {
    itemName: string;
    sku: string;
    quantityInStock: number;
    unit: string;
  }) => axiosInstance.post("/inventory", data),
  issueItem: (data: {
    inventoryItemId: number;
    quantityIssued: number;
    issuedTo: string;
    issueDate: string;
    remarks: string;
  }) => axiosInstance.post("/inventory/issue", data),
  getAllIssues: () => axiosInstance.get("/inventory/issues"),
  getIssuesByItem: (id: number) =>
    axiosInstance.get(`/inventory/${id}/issues`),
};