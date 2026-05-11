
import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import VendorDashboard from "../../../pages/vendor/VendorDashboard";
import { useAuth } from "../../../context/AuthContext";
import { vendorApi } from "../../../api/vendorApi";
import { purchaseOrderApi } from "../../../api/purchaseOrderApi";
import { invoiceApi } from "../../../api/invoiceApi";
import { contractApi } from "../../../api/contractApi";

jest.mock("../../../context/AuthContext");
jest.mock("../../../api/vendorApi");
jest.mock("../../../api/purchaseOrderApi");
jest.mock("../../../api/invoiceApi");
jest.mock("../../../api/contractApi");
jest.mock("../../../api/goodsReceiptApi", () => ({
  goodsReceiptApi: { getByPO: jest.fn().mockResolvedValue({ data: [] }) },
}));
jest.mock("react-hot-toast", () => ({ error: jest.fn(), success: jest.fn() }));

(useAuth as jest.Mock).mockReturnValue({
  user: { userId: "u1", fullName: "Vendor A", role: "Vendor", vendorId: 1 },
});

const mockVendor = {
  id: 1, vendorCode: "V001", companyName: "Acme",
  contactEmail: "v@v.com", contactPhone: "123",
  address: "St 1", taxNumber: "T1", licenseNumber: "L1",
  documentPath: "", status: "Approved",
};

beforeEach(() => {
  (vendorApi.getMyProfile as jest.Mock).mockResolvedValue({ data: mockVendor });
  (purchaseOrderApi.getByVendor as jest.Mock).mockResolvedValue({ data: [] });
  (invoiceApi.getByVendor as jest.Mock).mockResolvedValue({ data: [] });
  (contractApi.getByVendor as jest.Mock).mockResolvedValue({ data: [] });
});

const renderDashboard = () =>
  render(<MemoryRouter><VendorDashboard /></MemoryRouter>);

describe("VendorDashboard", () => {
  it("renders dashboard title", async () => {
    renderDashboard();
    await waitFor(() =>
      expect(screen.getByText("Vendor Dashboard")).toBeInTheDocument()
    );
  });

  it("shows stat cards", async () => {
    renderDashboard();
    await waitFor(() =>
      expect(screen.getByText("Total Orders")).toBeInTheDocument()
    );
    expect(screen.getByText("Pending Delivery")).toBeInTheDocument();
  });

  it("shows pending banner when status is Pending", async () => {
    (vendorApi.getMyProfile as jest.Mock).mockResolvedValue({
      data: { ...mockVendor, status: "Pending" },
    });
    renderDashboard();
    await screen.findByText(/account pending approval/i);
  });

  it("shows rejected banner when status is Rejected", async () => {
    (vendorApi.getMyProfile as jest.Mock).mockResolvedValue({
      data: { ...mockVendor, status: "Rejected", rejectionReason: "Fake docs" },
    });
    renderDashboard();
    await screen.findByText(/account rejected/i);
    await screen.findByText(/fake docs/i);
  });
});