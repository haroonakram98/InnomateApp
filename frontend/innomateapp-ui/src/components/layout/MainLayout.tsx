import React, { useState } from "react";
import {
  Menu,
  LayoutDashboard,
  Settings,
  LogOut,
  Users,
  BarChart3,
  Box,
  Moon,
  Sun,
} from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore.js";
import { useTheme } from "@/hooks/useTheme.js";

const MainLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const navigate = useNavigate();
  const { logout } = useAuthStore();
  const { theme, toggleTheme, isDark } = useTheme();

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  const menu = [
    { icon: <LayoutDashboard size={18} />, label: "Dashboard", path: "/" },
    { icon: <Users size={18} />, label: "Customers", path: "/customers" },
    { icon: <BarChart3 size={18} />, label: "Reports", path: "/reports" },
    { icon: <Box size={18} />, label: "Products", path: "/products" }, // Added Product menu
    { icon: <Settings size={18} />, label: "Settings", path: "/settings" },
  ];

  const handleMenuClick = (path: string) => {
    navigate(path);
    setSidebarOpen(false);
  };

  return (
    <div className="flex h-screen overflow-hidden bg-gray-50 dark:bg-gray-900 transition-colors duration-300">
      {/* Sidebar */}
      <aside
        className={`fixed md:static z-30 bg-white dark:bg-gray-900 shadow-md h-full transform ${
          sidebarOpen ? "translate-x-0" : "-translate-x-full md:translate-x-0"
        } transition-transform duration-300 w-64 md:w-60`}
      >
        <div className="p-4 border-b dark:border-gray-800 flex justify-between items-center">
          <h1 className="text-xl font-bold text-blue-600 dark:text-blue-400">
            SmartOps
          </h1>
          <button className="md:hidden" onClick={() => setSidebarOpen(false)}>
            <Menu size={22} />
          </button>
        </div>

        <nav className="p-3 flex flex-col gap-1">
          {menu.map((m) => (
            <button
              key={m.label}
              onClick={() => handleMenuClick(m.path)}
              className="flex items-center gap-3 px-3 py-2 text-gray-700 dark:text-gray-300 rounded-md hover:bg-blue-50 dark:hover:bg-gray-800"
            >
              {m.icon}
              <span>{m.label}</span>
            </button>
          ))}
        </nav>

        <div className="p-3 border-t dark:border-gray-800">
          <button
            onClick={handleLogout}
            className="flex items-center gap-2 text-red-600 hover:text-red-700"
          >
            <LogOut size={18} />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col transition-colors duration-300">
        {/* Navbar */}
        <header className="flex justify-between items-center bg-white dark:bg-gray-900 shadow px-4 py-3">
          <button className="md:hidden" onClick={() => setSidebarOpen(!sidebarOpen)}>
            <Menu />
          </button>
          <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-200">
            Smart Operations Dashboard
          </h2>
          <button onClick={toggleTheme}>
            {isDark ? (
              <Sun size={20} className="text-yellow-400" />
            ) : (
              <Moon size={20} className="text-gray-500" />
            )}
          </button>
        </header>

        <main className="flex-1 overflow-y-auto p-4 sm:p-6 transition-colors duration-300">
          {children}
        </main>
      </div>
    </div>
  );
};

export default MainLayout;
