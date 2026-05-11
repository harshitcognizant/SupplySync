import { Bell, LogOut, User } from "lucide-react";
import { useAuth } from "../../context/AuthContext";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { notificationApi } from "../../api/notificationApi";

const roleRoutes: Record<string, string> = {
  Admin:               "/admin",
  Vendor:              "/vendor",
  ProcurementOfficer:  "/procurement",
  WarehouseManager:    "/warehouse",
  FinanceOfficer:      "/finance",
  ComplianceOfficer:   "/compliance",
};

const Topbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [unread, setUnread] = useState(0);

  useEffect(() => {
    notificationApi.getUnreadCount()
      .then(res => setUnread(res.data.count))
      .catch(() => {});
  }, []);

  const handleLogout = () => { logout(); navigate("/login"); };

  const baseRoute = roleRoutes[user?.role ?? ""] ?? "/";

  return (
    <header className="h-16 bg-white border-b border-gray-200
                       flex items-center justify-between px-6
                       sticky top-0 z-10">
      <h1 className="text-lg font-semibold text-gray-800">
        SupplySync
      </h1>

      <div className="flex items-center gap-3">
        {/* Notifications */}
        <button
          onClick={() => navigate(`${baseRoute}/notifications`)}
          className="relative p-2 hover:bg-gray-100 rounded-lg
                     transition-colors">
          <Bell className="w-5 h-5 text-gray-600" />
          {unread > 0 && (
            <span className="absolute -top-1 -right-1 bg-red-500
                             text-white text-xs rounded-full w-4 h-4
                             flex items-center justify-center font-bold">
              {unread > 9 ? "9+" : unread}
            </span>
          )}
        </button>

        {/* Profile */}
        <button
          onClick={() => navigate(`${baseRoute}/profile`)}
          className="flex items-center gap-2 px-3 py-1.5
                     hover:bg-gray-100 rounded-lg transition-colors">
          <div className="w-7 h-7 rounded-full bg-primary-600
                          flex items-center justify-center text-white
                          text-xs font-bold">
            {user?.fullName.charAt(0).toUpperCase()}
          </div>
          <div className="text-left">
            <p className="text-sm font-medium text-gray-800 leading-none">
              {user?.fullName}
            </p>
            <p className="text-xs text-gray-400 mt-0.5">
              {user?.role.replace(/([A-Z])/g, " $1").trim()}
            </p>
          </div>
        </button>

        {/* Logout */}
        <button onClick={handleLogout}
          className="flex items-center gap-1 text-sm text-red-600
                     hover:text-red-700 transition-colors p-2
                     hover:bg-red-50 rounded-lg">
          <LogOut className="w-4 h-4" />
        </button>
      </div>
    </header>
  );
};

export default Topbar;