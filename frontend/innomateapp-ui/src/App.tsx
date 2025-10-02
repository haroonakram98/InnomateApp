import { useEffect } from "react";
import { useAuthStore } from "@/store/useAuthStore.js";
import AuthPage from "@/features/auth/AuthPage.js";

function App() {
  const { isAuthenticated, user, checkToken } = useAuthStore();

  // Run expiry check once when the app loads
  useEffect(() => {
    checkToken();
  }, [checkToken]);

  console.log("isAuthenticated: ");
console.log(isAuthenticated);
console.log("user: ");
console.log(user);
  return isAuthenticated ? (
    <div className="p-6">
      <h1 className="text-2xl font-bold">Welcome, {user?.userName}</h1>
      <p>Your email: {user?.email}</p>
    </div>
  ) : (
    <AuthPage />
  );
}

export default App;
