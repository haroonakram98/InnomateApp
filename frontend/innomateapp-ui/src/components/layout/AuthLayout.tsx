import React from "react";
import { useTheme } from "@/hooks/useTheme.js"; // from option 2 setup
import { Moon, Sun } from "lucide-react";

interface AuthLayoutProps {
  children: React.ReactNode;
}

const AuthLayout: React.FC<AuthLayoutProps> = ({ children }) => {
  const { theme, toggleTheme } = useTheme();

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 transition-colors duration-300">
      {/* Theme Toggle */}
      <div className="absolute top-4 right-4">
        <button
          onClick={toggleTheme}
          className="p-2 rounded-full bg-white dark:bg-gray-800 shadow hover:shadow-md transition-all"
          aria-label="Toggle Theme"
        >
          {theme === "dark" ? (
            <Sun size={18} className="text-yellow-400" />
          ) : (
            <Moon size={18} className="text-gray-600" />
          )}
        </button>
      </div>

      {/* Auth Card */}
      <div className="w-full max-w-md bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 p-6 sm:p-8 transition-all duration-300">
        <div className="text-center mb-6">
          <h1 className="text-3xl font-bold text-blue-600 dark:text-blue-400">
            SmartOps
          </h1>
          <p className="text-gray-600 dark:text-gray-400 text-sm mt-1">
            Secure your operations with ease
          </p>
        </div>

        {children}

        <footer className="mt-6 text-center text-xs text-gray-500 dark:text-gray-400">
          © {new Date().getFullYear()} SmartOps. All rights reserved.
        </footer>
      </div>
    </div>
  );
};

export default AuthLayout;
