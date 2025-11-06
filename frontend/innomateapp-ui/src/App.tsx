import { useEffect } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore.js";
import AuthPage from "@/features/auth/AuthPage.js";
import DashboardPage from "@/pages/DashboardPage.js";
import ProductsPage from "./pages/ProductsPage.js";
import CustomerPage from "./pages/CustomerPage.js";
import SalesPage from "./pages/SalesPage.js";

function App() {
  const { isAuthenticated, user, checkToken } = useAuthStore();

  useEffect(() => {
    checkToken();
  }, [checkToken]);

  return (
    <BrowserRouter>
      <Routes>
        {/* Public Route */}
        <Route
          path="/login"
          element={!isAuthenticated ? <AuthPage /> : <Navigate to="/sales" replace />}
        />

        {/* Redirect root to /sales */}
        <Route
          path="/"
          element={
            isAuthenticated ? <Navigate to="/sales" replace /> : <Navigate to="/login" replace />
          }
        />

        {/* Protected Routes */}
        <Route
          path="/sales"
          element={
            isAuthenticated ? <SalesPage /> : <Navigate to="/login" replace />
          }
        />
        
        <Route
          path="/dashboard"
          element={
            isAuthenticated ? <DashboardPage /> : <Navigate to="/login" replace />
          }
        />
        
        <Route
          path="/products"
          element={
            isAuthenticated ? <ProductsPage /> : <Navigate to="/login" replace />
          }
        />
        
        <Route
          path="/customers"
          element={
            isAuthenticated ? <CustomerPage /> : <Navigate to="/login" replace />
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;