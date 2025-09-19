import { useEffect, useState } from "react";

function App() {
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetch("http://localhost:5180/api/v1/hello")
      .then(res => res.text())
      .then(data => setMessage(data));
  }, []);

  return (
    <div className="p-8 text-xl">
      <h1 className="font-bold">BankingApp Frontend 🚀</h1>
      <p className="mt-4 text-red-200">{message}</p>
    </div>
  );
}

export default App;
