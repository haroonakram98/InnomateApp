import { useEffect } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore.js";
import AuthPage from "@/features/auth/AuthPage.js";
import DashboardPage from "@/pages/DashboardPage.js";

function App() {
  const { isAuthenticated, user, checkToken } = useAuthStore();

  useEffect(() => {
    checkToken();
  }, [checkToken]);

  console.log("isAuthenticated:", isAuthenticated);
  console.log("user:", user);

  return (
    <BrowserRouter>
      <Routes>
        {/* Public Route */}
        <Route
          path="/login"
          element={!isAuthenticated ? <AuthPage /> : <Navigate to="/" replace />}
        />

        {/* Protected Route */}
        <Route
          path="/*"
          element={
            isAuthenticated ? <DashboardPage /> : <Navigate to="/login" replace />
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
