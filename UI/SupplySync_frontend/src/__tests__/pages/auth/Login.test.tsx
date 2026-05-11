import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import Login from "../../../pages/auth/Login";
import { authApi } from "../../../api/authApi";
import { useAuth } from "../../../context/AuthContext";

jest.mock("../../../api/authApi");
jest.mock("../../../context/AuthContext");
jest.mock("react-hot-toast", () => ({ success: jest.fn(), error: jest.fn() }));

const mockLogin = jest.fn();
const mockNavigate = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

(useAuth as jest.Mock).mockReturnValue({ login: mockLogin });

const renderLogin = () =>
  render(<MemoryRouter><Login /></MemoryRouter>);

describe("Login page", () => {
  beforeEach(() => jest.clearAllMocks());

  it("renders email and password fields", () => {
    renderLogin();
    expect(screen.getByPlaceholderText("you@company.com")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("••••••••")).toBeInTheDocument();
  });

  it("renders Sign In button", () => {
    renderLogin();
    expect(screen.getByRole("button", { name: /sign in/i })).toBeInTheDocument();
  });

  it("shows error on 401 response", async () => {
    (authApi.login as jest.Mock).mockRejectedValue({
      response: { status: 401 },
    });
    renderLogin();
    fireEvent.change(screen.getByPlaceholderText("you@company.com"), {
      target: { value: "wrong@test.com" },
    });
    fireEvent.change(screen.getByPlaceholderText("••••••••"), {
      target: { value: "wrongpass" },
    });
    fireEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await screen.findByText(/invalid email or password/i);
  });

  it("calls login and navigates on success", async () => {
    (authApi.login as jest.Mock).mockResolvedValue({
      data: { fullName: "Alice", role: "Admin", token: "tok" },
    });
    renderLogin();
    fireEvent.change(screen.getByPlaceholderText("you@company.com"), {
      target: { value: "alice@test.com" },
    });
    fireEvent.change(screen.getByPlaceholderText("••••••••"), {
      target: { value: "password" },
    });
    fireEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await waitFor(() => expect(mockLogin).toHaveBeenCalled());
    expect(mockNavigate).toHaveBeenCalledWith("/admin");
  });
});