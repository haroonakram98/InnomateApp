import { useState } from "react";
import { register } from "@/api/auth.js";
import { useAuthStore } from "@/store/useAuthStore.js";
import Input from "@/components/ui/Input.js";
import Button from "@/components/ui/Button.js";

const RegisterForm = () => {
  const { login: setAuth } = useAuthStore();
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const response = await register({ userName, email, password });
      debugger;
      setAuth(response);
    } catch {
      setError("Registration failed. Try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="space-y-4" onSubmit={handleSubmit}>
      <h2 className="text-2xl font-bold text-center">Register</h2>
      <Input label="Name" value={userName} onChange={(e) => setUserName(e.target.value)} required />
      <Input label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
      <Input label="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
      {error && <p className="text-sm text-red-500 text-center">{error}</p>}
      <Button type="submit" loading={loading}>Register</Button>
    </form>
  );
};

export default RegisterForm;
