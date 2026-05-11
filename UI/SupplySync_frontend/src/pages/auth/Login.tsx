import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { authApi } from "../../api/authApi";
import toast from "react-hot-toast";
import { Lock, Mail, Loader2 } from "lucide-react";

const Login = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: "", password: "" });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");


  const roleRoutes: Record<string, string> = {
    Admin: "/admin",
    Vendor: "/vendor",
    ProcurementOfficer: "/procurement",
    WarehouseManager: "/warehouse",
    FinanceOfficer: "/finance",
    ComplianceOfficer: "/compliance",
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const res = await authApi.login(form);
      const userData = res.data;
      login(userData);
      toast.success(`Welcome back, ${userData.fullName}!`);
      navigate(roleRoutes[userData.role] ?? "/");
    } catch (err: any) {
      if (err.response?.status === 401) {
        setError("Invalid email or password. Please try again.");
      } else if (err.response?.status === 400) {
        setError("Invalid request. Please check your details.");
      } else {
        setError("Something went wrong. Please try again later.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-900
                    via-primary-800 to-sidebar flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo */}
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-white tracking-tight">
            Supply<span className="text-primary-300">Sync</span>
          </h1>
          <p className="text-white/60 mt-2 text-sm">
            Supply Chain Integration & Vendor Management
          </p>
        </div>

        {/* Card */}
        <div className="bg-white rounded-2xl shadow-2xl p-8">
          <h2 className="text-xl font-semibold text-gray-800 mb-6">
            Sign in to your account
          </h2>
          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="label">Email Address</label>
              <div className="relative">
                <Mail className="absolute left-3 top-2.5 w-4 h-4 text-gray-400" />
                <input
                  type="email"
                  className="input-field pl-10"
                  placeholder="you@company.com"
                  value={form.email}
                  onChange={e => {
                    setError("");
                    setForm({ ...form, email: e.target.value });
                  }}
                  required
                />
              </div>
            </div>

            <div>
              <label className="label">Password</label>
              <div className="relative">
                <Lock className="absolute left-3 top-2.5 w-4 h-4 text-gray-400" />
                <input
                  type="password"
                  className="input-field pl-10"
                  placeholder="••••••••"
                  value={form.password}
                  onChange={e => {
                    setError("");
                    setForm({ ...form, password: e.target.value });
                  }}

                  required
                />
              </div>
            </div>

            {/* Error Message */}
            {error && (
              <div className="flex items-center gap-2 p-3 bg-red-50
                  border border-red-200 rounded-lg">
                <div className="w-4 h-4 rounded-full bg-red-500
                    flex items-center justify-center shrink-0">
                  <span className="text-white text-xs font-bold">!</span>
                </div>
                <p className="text-sm text-red-700 font-medium">{error}</p>
              </div>
            )}

            <button
              type="submit"
              disabled={loading}
              className="btn-primary w-full flex items-center justify-center gap-2"
            >
              {loading
                ? <><Loader2 className="w-4 h-4 animate-spin" /> Signing in...</>
                : "Sign In"
              }
            </button>
          </form>

          <div className="mt-6 text-center text-sm text-gray-500">
            Are you a vendor?{" "}
            <Link to="/register/vendor"
              className="text-primary-600 font-medium hover:underline">
              Register here
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;