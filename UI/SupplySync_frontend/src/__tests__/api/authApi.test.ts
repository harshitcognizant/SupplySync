import axiosInstance from "../../api/axiosInstance";
import { authApi } from "../../api/authApi";

jest.mock("../../api/axiosInstance");
const mockedAxios = axiosInstance as jest.Mocked<typeof axiosInstance>;

describe("authApi", () => {
  afterEach(() => jest.clearAllMocks());

  it("calls login with correct data", async () => {
    mockedAxios.post.mockResolvedValue({ data: { token: "abc" } });
    const res = await authApi.login({ email: "a@b.com", password: "123" });
    expect(mockedAxios.post).toHaveBeenCalledWith("/auth/login", {
      email: "a@b.com",
      password: "123",
    });
    expect(res.data.token).toBe("abc");
  });

  it("calls register with correct data", async () => {
    mockedAxios.post.mockResolvedValue({ data: { message: "registered" } });
    await authApi.register({
      fullName: "John",
      email: "j@j.com",
      password: "pass",
      role: "Admin",
    });
    expect(mockedAxios.post).toHaveBeenCalledWith("/auth/register", {
      fullName: "John",
      email: "j@j.com",
      password: "pass",
      role: "Admin",
    });
  });

  it("calls vendorRegister with correct data", async () => {
    mockedAxios.post.mockResolvedValue({ data: { message: "ok" } });
    const payload = {
      fullName: "Jane",
      email: "jane@co.com",
      password: "pass",
      companyName: "Acme",
      contactPhone: "123",
      address: "Street 1",
      taxNumber: "TAX1",
      licenseNumber: "LIC1",
      documentPath: "http://doc.url",
    };
    await authApi.vendorRegister(payload);
    expect(mockedAxios.post).toHaveBeenCalledWith(
      "/auth/vendor-register",
      payload
    );
  });
});