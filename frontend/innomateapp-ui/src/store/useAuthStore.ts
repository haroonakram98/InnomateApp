import { create } from "zustand";
import { persist } from "zustand/middleware";
import { AuthResponse, User } from "@/types/auth.js";
import { jwtDecode } from "jwt-decode";

interface DecodedToken {
  exp?: number; // expiration timestamp in seconds
}

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (data: AuthResponse) => void;
  logout: () => void;
  checkToken: () => void;
}
let logoutTimer: ReturnType<typeof setTimeout> | null = null;
export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isAuthenticated: false,

      login: (data: AuthResponse) => {
        debugger;
        // clear any old timer
        if (logoutTimer) clearTimeout(logoutTimer);

        // decode token to find expiry
        const decoded = jwtDecode<DecodedToken>(data.token);
        if (decoded.exp) {
          const expiryTime = decoded.exp * 1000 - Date.now();
          if (expiryTime > 0) {
            logoutTimer = setTimeout(() => {
              get().logout();
            }, expiryTime);
          }
        }

        set({
          user: data.user,
          token: data.token,
          isAuthenticated: true,
        });
        get().checkToken(); // validate immediately
      },

      logout: () => {
        if (logoutTimer) clearTimeout(logoutTimer);
        logoutTimer = null;
        set({
          user: null,
          token: null,
          isAuthenticated: false,
        });
      },

      checkToken: () => {
        const token = get().token;
        if (!token) return;

        try {
          const decoded = jwtDecode<DecodedToken>(token);
          const now = Date.now() / 1000; // current time in seconds
console.log(decoded.exp)
console.log(now)
          if (decoded.exp && decoded.exp < now) {
            console.warn("JWT expired → logging out");
            set({
              user: null,
              token: null,
              isAuthenticated: false,
            });
          }
        } catch (err) {
          console.error("Invalid token → logging out", err);
          set({
            user: null,
            token: null,
            isAuthenticated: false,
          });
        }
      },
    }),
    {
      name: "auth-storage",
    }
  )
);
