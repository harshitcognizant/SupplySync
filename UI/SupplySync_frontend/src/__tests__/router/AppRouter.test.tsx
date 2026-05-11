import { render, screen } from "@testing-library/react";
import AppRouter from "../../router/AppRouter";
import { useAuth } from "../../context/AuthContext";

jest.mock("../../context/AuthContext");
// mock all page components to keep tests fast
jest.mock("../../pages/auth/Login", () => () => <div>Login Page</div>);
jest.mock("../../pages/admin/AdminDashboard", () => () => <div>Admin</div>);
jest.mock("../../pages/vendor/VendorDashboard", () => () => <div>Vendor</div>);
jest.mock("../../components/layout/Layout", () => ({ children }: any) => <>{children}</>);
jest.mock("../../router/ProtectedRoute", () => ({ children }: any) => <>{children}</>);

describe("AppRouter", () => {
  it("renders Login page on /login", () => {
    (useAuth as jest.Mock).mockReturnValue({
      user: null, isAuthenticated: false, isLoading: false,
    });
    window.history.pushState({}, "", "/login");
    render(<AppRouter />);
    expect(screen.getByText("Login Page")).toBeInTheDocument();
  });

  it("redirects / to /admin for Admin user", () => {
    (useAuth as jest.Mock).mockReturnValue({
      user: { role: "Admin" }, isAuthenticated: true, isLoading: false,
    });
    window.history.pushState({}, "", "/");
    render(<AppRouter />);
    expect(screen.getByText("Admin")).toBeInTheDocument();
  });

  it("shows 403 on /unauthorized", () => {
    (useAuth as jest.Mock).mockReturnValue({
      user: null, isAuthenticated: false, isLoading: false,
    });
    window.history.pushState({}, "", "/unauthorized");
    render(<AppRouter />);
    expect(screen.getByText(/403/i)).toBeInTheDocument();
  });
});