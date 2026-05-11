import axiosInstance from "./axiosInstance";

export const authApi = {
  login: (data: { email: string; password: string }) =>
    axiosInstance.post("/auth/login", data),

  register: (data: {
    fullName: string;
    email: string;
    password: string;
    role: string;
  }) => axiosInstance.post("/auth/register", data),

  vendorRegister: (data: {
    fullName: string;
    email: string;
    password: string;
    companyName: string;
    contactPhone: string;
    address: string;
    taxNumber: string;
    licenseNumber: string;
    documentPath: string;
  }) => axiosInstance.post("/auth/vendor-register", data),
};