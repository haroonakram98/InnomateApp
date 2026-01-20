// main.tsx
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.js";

// Get root element safely
const rootElement = document.getElementById("root");

if (!rootElement) {
  throw new Error("❌ Root element with id 'root' not found in index.html");
}

// Create React root
const root = createRoot(rootElement as HTMLElement);

root.render(
  <StrictMode>
    <App />
  </StrictMode>
);