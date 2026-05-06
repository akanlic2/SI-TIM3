import React, { createContext, useCallback, useContext, useEffect, useState } from 'react';
import {
  getAccessToken,
  isAuthenticated,
  login,
  logout as keycloakLogout,
  parseToken,
  refreshToken,
  type TokenClaims,
} from './keycloak';

// ─── Auth Context Type ────────────────────────────────────────────────────────
interface AuthContextValue {
  isLoggedIn: boolean;
  isLoading: boolean;
  user: TokenClaims | null;
  token: string | null;
  logout: () => Promise<void>;
  loginRedirect: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

// ─── AuthProvider ─────────────────────────────────────────────────────────────
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isLoading, setIsLoading] = useState(true);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [user, setUser] = useState<TokenClaims | null>(null);
  const [token, setToken] = useState<string | null>(null);

  const initAuth = useCallback(async () => {
    setIsLoading(true);

    if (isAuthenticated()) {
      const t = getAccessToken()!;
      setToken(t);
      setUser(parseToken(t));
      setIsLoggedIn(true);
    } else {
      // Pokušavamo refresh
      const refreshed = await refreshToken();
      if (refreshed) {
        const t = getAccessToken()!;
        setToken(t);
        setUser(parseToken(t));
        setIsLoggedIn(true);
      } else {
        setIsLoggedIn(false);
        setUser(null);
        setToken(null);
      }
    }

    setIsLoading(false);
  }, []);

  useEffect(() => {
    initAuth();
  }, [initAuth]);

  const handleLogout = useCallback(async () => {
    await keycloakLogout();
    // Namjerno ne mijenjamo state ovdje jer keycloakLogout radi window.location.href
    // Ako promijenimo state u false, Router bi nas odmah redirektao na login() prije nego
    // što se browser preusmjeri na Keycloak logout!
  }, []);

  const loginRedirect = useCallback(async () => {
    await login();
  }, []);

  return (
    <AuthContext.Provider
      value={{
        isLoggedIn,
        isLoading,
        user,
        token,
        logout: handleLogout,
        loginRedirect,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// ─── useAuth Hook ─────────────────────────────────────────────────────────────
export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
}
