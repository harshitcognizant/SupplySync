import { NavLink } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import {
  LayoutDashboard, Users, FileText, ShoppingCart,
  Package, Receipt, ClipboardCheck, BarChart3,
  Truck, Warehouse
} from "lucide-react";

const navByRole: Record<string, { label: string; icon: any; path: string }[]> = {
  Admin: [
    { label: "Dashboard", icon: LayoutDashboard, path: "/admin" },
    { label: "Users", icon: Users, path: "/admin" },
  ],
  Vendor: [
    { label: "Dashboard", icon: LayoutDashboard, path: "/vendor" },
    { label: "My Orders", icon: ShoppingCart, path: "/vendor" },
    { label: "Invoices", icon: Receipt, path: "/vendor" },
  ],
  ProcurementOfficer: [
    { label: "Dashboard", icon: LayoutDashboard, path: "/procurement" },
    { label: "Vendors", icon: Truck, path: "/procurement" },
    { label: "Contracts", icon: FileText, path: "/procurement" },
    { label: "Purchase Orders", icon: ShoppingCart, path: "/procurement" },
  ],
  WarehouseManager: [
    { label: "Dashboard", icon: LayoutDashboard, path: "/warehouse" },
    { label: "Inventory", icon: Warehouse, path: "/warehouse" },
    { label: "Goods Receipts", icon: Package, path: "/warehouse" },
  ],
  FinanceOfficer: [
    { label: "Dashboard", icon: LayoutDashboard, path: "/finance" },
    { label: "Invoices", icon: Receipt, path: "/finance" },
    { label: "Payments", icon: BarChart3, path: "/finance" },
  ],
  ComplianceOfficer: [
    { label: "Dashboard", icon: LayoutDashboard, path: "/compliance" },
    { label: "Audits", icon: ClipboardCheck, path: "/compliance" },
  ],
};

const Sidebar = () => {
  const { user } = useAuth();
  const links = navByRole[user?.role ?? ""] ?? [];

  return (
    <aside className="w-60 min-h-screen bg-sidebar flex flex-col">
      <div className="p-6 border-b border-white/10">
        <h2 className="text-white font-bold text-xl tracking-tight">
          Supply<span className="text-primary-400">Sync</span>
        </h2>
        <p className="text-white/40 text-xs mt-1">{user?.role}</p>
      </div>
      <nav className="flex-1 p-4 space-y-1">
        {links.map(({ label, icon: Icon, path }) => (
          <NavLink key={label} to={path}
            className="flex items-center gap-3 px-4 py-2.5 rounded-lg
                       text-white/70 hover:text-white hover:bg-white/10
                       transition-all duration-150 text-sm font-medium">
            <Icon className="w-4 h-4" />
            {label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
};

export default Sidebar;