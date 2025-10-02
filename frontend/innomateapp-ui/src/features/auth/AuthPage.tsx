import { useState } from "react";
import AuthLayout from "@/components/layout/AuthLayout.js";
import LoginForm from "./LoginForm.js";
import RegisterForm from "./RegisterForm.js";

const AuthPage = () => {
  const [isLogin, setIsLogin] = useState(true);

  return (
    <AuthLayout>
      {isLogin ? <LoginForm /> : <RegisterForm />}
      <p className="text-center mt-4 text-sm text-gray-600">
        {isLogin ? "Don't have an account?" : "Already have an account?"}
        <button
          className="ml-1 text-blue-600 font-medium hover:underline"
          onClick={() => setIsLogin(!isLogin)}
        >
          {isLogin ? "Register" : "Login"}
        </button>
      </p>
    </AuthLayout>
  );
};

export default AuthPage;
