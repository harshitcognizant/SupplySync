import { render, screen, act } from "@testing-library/react";
import { AuthProvider, useAuth } from "../../context/AuthContext";

const mockUser = {
  token: "tok123",
  userId: "u1",
  fullName: "Alice",
  email: "alice@test.com",
  role: "Admin",
};

// Simple component to read auth context
const TestComponent = () => {
  const { user, isAuthenticated, login, logout } = useAuth();
  return (
    <div>
      <p data-testid="name">{user?.fullName ?? "none"}</p>
      <p data-testid="auth">{isAuthenticated ? "yes" : "no"}</p>
      <button onClick={() => login(mockUser)}>Login</button>
      <button onClick={logout}>Logout</button>
    </div>
  );
};

describe("AuthContext", () => {
  beforeEach(() => localStorage.clear());

  it("starts unauthenticated when localStorage is empty", async () => {
    render(<AuthProvider><TestComponent /></AuthProvider>);
    // wait for useEffect
    await screen.findByTestId("auth");
    expect(screen.getByTestId("auth").textContent).toBe("no");
    expect(screen.getByTestId("name").textContent).toBe("none");
  });

  it("login sets user and persists to localStorage", async () => {
    render(<AuthProvider><TestComponent /></AuthProvider>);
    await screen.findByTestId("auth");
    act(() => screen.getByText("Login").click());
    expect(screen.getByTestId("name").textContent).toBe("Alice");
    expect(screen.getByTestId("auth").textContent).toBe("yes");
    expect(JSON.parse(localStorage.getItem("auth")!).fullName).toBe("Alice");
  });

  it("logout clears user and localStorage", async () => {
    render(<AuthProvider><TestComponent /></AuthProvider>);
    await screen.findByTestId("auth");
    act(() => screen.getByText("Login").click());
    act(() => screen.getByText("Logout").click());
    expect(screen.getByTestId("auth").textContent).toBe("no");
    expect(localStorage.getItem("auth")).toBeNull();
  });

  it("restores user from localStorage on mount", async () => {
    localStorage.setItem("auth", JSON.stringify(mockUser));
    render(<AuthProvider><TestComponent /></AuthProvider>);
    expect(await screen.findByText("Alice")).toBeInTheDocument();
  });

  it("throws if useAuth is used outside AuthProvider", () => {
    const consoleError = jest.spyOn(console, "error").mockImplementation(() => {});
    expect(() => render(<TestComponent />)).toThrow(
      "useAuth must be used within AuthProvider"
    );
    consoleError.mockRestore();
  });
});