import { useEffect, useState, useCallback } from "react";

export const useTheme = () => {
  const [theme, setTheme] = useState<string>(() => {
    return localStorage.getItem("theme") || "light";
  });

  // Apply theme to document
  const applyTheme = useCallback((newTheme: string) => {
    if (newTheme === "dark") {
      document.documentElement.classList.add("dark");
      document.documentElement.classList.remove("light");
    } else {
      document.documentElement.classList.remove("dark");
      document.documentElement.classList.add("light");
    }
    localStorage.setItem("theme", newTheme);
  }, []);

  // Toggle between light and dark
  const toggleTheme = useCallback(() => {
    const newTheme = theme === "dark" ? "light" : "dark";
    setTheme(newTheme);
    applyTheme(newTheme);

    // ✅ Broadcast to other tabs
    const bc = new BroadcastChannel("theme");
    bc.postMessage(newTheme);
    bc.close();
  }, [theme, applyTheme]);

  useEffect(() => {
    applyTheme(theme);

    // ✅ Listen for theme changes from other tabs
    const bc = new BroadcastChannel("theme");
    bc.onmessage = (e) => {
      setTheme(e.data);
      applyTheme(e.data);
    };

    return () => bc.close();
  }, [applyTheme, theme]);

  return { theme, toggleTheme, isDark: theme === "dark" };
};
