// components/layout/MainLayout.tsx
import React, { useState } from "react";
import {
  Menu,
  ShoppingBag,
  ShoppingCart,
  Settings,
  BarChart3,
  Package,
  Moon,
  Sun,
  ChevronLeft,
  ChevronRight,
  Users,
  Home,
  Tag,
  Truck,
  Building,
} from "lucide-react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore.js";
import ToastContainer from "@/components/ui/ToastContainer.js";
import { useTheme } from "@/hooks/useTheme.js";

const MainLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuthStore();
  const { theme, toggleTheme, isDark } = useTheme();
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);

  const menu = [
    { icon: Home, label: "Dashboard", path: "/dashboard" },
    { icon: ShoppingBag, label: "POS", path: "/sales/new" },
    { icon: ShoppingCart, label: "Sales", path: "/sales" },
    { icon: Truck, label: "Purchases", path: "/purchases" },
    { icon: Package, label: "Stocks", path: "/stocks" },
    { icon: Package, label: "Products", path: "/products" },
    { icon: Tag, label: "Categories", path: "/categories" }, // Added Categories
    { icon: Users, label: "Customers", path: "/customers" },
    { icon: Truck, label: "Suppliers", path: "/suppliers" },
    { icon: Building, label: "Tenants", path: "/tenants", superAdminOnly: true },
  ];

  const filteredMenu = menu.filter(item => !item.superAdminOnly || user?.tenantId === 1);

  const isActivePath = (path: string) => location.pathname === path;

  const toggleSidebar = () => {
    setIsSidebarCollapsed(!isSidebarCollapsed);
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className={`flex h-screen font-sans ${isDark
      ? 'bg-gray-900 text-gray-200'
      : 'bg-gray-100 text-gray-800'
      }`}>
      {/* Sidebar */}
      <aside className={`flex flex-col transition-all duration-300 ${isSidebarCollapsed ? 'w-16' : 'w-64'
        } border-r ${isDark
          ? 'bg-gray-800 border-gray-700'
          : 'bg-white border-gray-200'
        } shadow-lg z-30`}>
        {/* Sidebar Header */}
        <div className={`flex items-center ${isSidebarCollapsed ? 'justify-center' : 'justify-between'
          } h-16 border-b px-4 ${isDark ? 'border-gray-700' : 'border-gray-200'
          }`}>
          {!isSidebarCollapsed && (
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg flex items-center justify-center">
                <ShoppingBag size={20} className="text-white" />
              </div>
              <h1 className={`text-lg font-bold ${isDark ? 'text-white' : 'text-gray-900'
                }`}>
                SmartOps
              </h1>
            </div>
          )}

          <button
            onClick={toggleSidebar}
            className={`p-2 rounded-lg transition-colors ${isDark
              ? 'hover:bg-gray-700 text-gray-400'
              : 'hover:bg-gray-100 text-gray-500'
              }`}
            title={isSidebarCollapsed ? "Expand sidebar" : "Collapse sidebar"}
          >
            {isSidebarCollapsed ? (
              <ChevronRight size={20} />
            ) : (
              <ChevronLeft size={20} />
            )}
          </button>
        </div>

        {/* Navigation Menu */}
        <nav className="flex-grow flex flex-col py-4 space-y-1 px-3">
          {filteredMenu.map((item, idx) => (
            <button
              key={idx}
              onClick={() => navigate(item.path)}
              className={`flex items-center gap-3 w-full p-3 rounded-lg transition-all ${isActivePath(item.path)
                ? "bg-blue-500 text-white shadow-lg shadow-blue-500/25"
                : isDark
                  ? "text-gray-400 hover:bg-gray-700 hover:text-gray-200"
                  : "text-gray-600 hover:bg-gray-100 hover:text-gray-800"
                } ${isSidebarCollapsed ? 'justify-center' : ''}`}
              title={isSidebarCollapsed ? item.label : ""}
            >
              <item.icon size={20} className="flex-shrink-0" />
              {!isSidebarCollapsed && (
                <span className="font-medium text-sm whitespace-nowrap">
                  {item.label}
                </span>
              )}
            </button>
          ))}
        </nav>

        {/* Sidebar Footer */}
        <div className={`p-4 border-t ${isDark ? 'border-gray-700' : 'border-gray-200'
          } space-y-4`}>
          {/* Theme Toggle */}
          <button
            onClick={toggleTheme}
            className={`flex items-center gap-3 w-full p-3 rounded-lg transition-colors ${isDark
              ? 'hover:bg-gray-700 text-gray-400'
              : 'hover:bg-gray-100 text-gray-600'
              } ${isSidebarCollapsed ? 'justify-center' : ''}`}
            title={isSidebarCollapsed ? "Toggle Theme" : ""}
          >
            {isDark ? <Sun size={20} /> : <Moon size={20} />}
            {!isSidebarCollapsed && (
              <span className="font-medium text-sm">
                {isDark ? 'Light Mode' : 'Dark Mode'}
              </span>
            )}
          </button>

          {/* User Info (only show when expanded) */}
          {!isSidebarCollapsed && (
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-blue-600 rounded-full flex items-center justify-center text-white font-semibold text-sm">
                {user?.userName?.charAt(0)?.toUpperCase() || 'U'}
              </div>
              <div className="flex-1 min-w-0">
                <p className={`text-sm font-medium truncate ${isDark ? 'text-white' : 'text-gray-900'
                  }`}>
                  {user?.userName || "User Name"}
                </p>
                <p className={`text-xs truncate ${isDark ? 'text-gray-400' : 'text-gray-500'
                  }`}>
                  {user?.email || "user@example.com"}
                </p>
              </div>
              <button
                onClick={handleLogout}
                className={`p-2 rounded-lg transition-colors ${isDark
                  ? 'hover:bg-gray-700 text-gray-400'
                  : 'hover:bg-gray-100 text-gray-500'
                  }`}
                title="Logout"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
              </button>
            </div>
          )}

          {/* Collapsed user info */}
          {isSidebarCollapsed && (
            <div className="flex flex-col items-center space-y-2">
              <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-600 rounded-full flex items-center justify-center text-white font-semibold text-xs">
                {user?.userName?.charAt(0)?.toUpperCase() || 'U'}
              </div>
              <button
                onClick={handleLogout}
                className={`p-2 rounded-lg transition-colors ${isDark
                  ? 'hover:bg-gray-700 text-gray-400'
                  : 'hover:bg-gray-100 text-gray-500'
                  }`}
                title="Logout"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
              </button>
            </div>
          )}
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 flex flex-col overflow-hidden">
        {/* Top Bar - Only show when sidebar is collapsed */}
        {isSidebarCollapsed && (
          <div className={`h-16 border-b ${isDark ? 'border-gray-700 bg-gray-800' : 'border-gray-200 bg-white'
            } flex items-center justify-between px-6 shadow-sm`}>
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg flex items-center justify-center">
                <ShoppingBag size={20} className="text-white" />
              </div>
              <h1 className={`text-lg font-bold ${isDark ? 'text-white' : 'text-gray-900'
                }`}>
                SmartOps
              </h1>
            </div>

            <div className="flex items-center gap-4">
              <button
                onClick={toggleTheme}
                className={`p-2 rounded-lg transition-colors ${isDark
                  ? 'hover:bg-gray-700 text-gray-400'
                  : 'hover:bg-gray-100 text-gray-500'
                  }`}
                title="Toggle Theme"
              >
                {isDark ? <Sun size={20} /> : <Moon size={20} />}
              </button>

              <div className="flex items-center gap-3">
                <div className="text-right">
                  <p className={`text-sm font-medium ${isDark ? 'text-white' : 'text-gray-900'
                    }`}>
                    {user?.userName || "User Name"}
                  </p>
                  <p className={`text-xs ${isDark ? 'text-gray-400' : 'text-gray-500'
                    }`}>
                    {user?.email || "user@example.com"}
                  </p>
                </div>
                <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-600 rounded-full flex items-center justify-center text-white font-semibold text-sm">
                  {user?.userName?.charAt(0)?.toUpperCase() || 'U'}
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Page Content */}
        <div className="flex-1 overflow-y-auto">
          {children}
        </div>
      </main>

      <ToastContainer />
    </div>
  );
};

export default MainLayout;