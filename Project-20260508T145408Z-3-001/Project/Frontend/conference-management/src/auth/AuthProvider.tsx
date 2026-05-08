import React, { createContext, useCallback, useContext, useEffect, useState } from 'react';
import { authService, type AuthUser, type LoginRequest, type RegisterRequest } from './authService';

interface AuthContextValue {
  isLoggedIn: boolean;
  isLoading: boolean;
  user: AuthUser | null;
  token: string | null;
  logout: () => void;
  login: (request: LoginRequest) => Promise<boolean>;
  register: (request: RegisterRequest) => Promise<boolean>;
  refreshCurrentUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isLoading, setIsLoading] = useState(true);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [user, setUser] = useState<AuthUser | null>(null);
  const [token, setToken] = useState<string | null>(null);

  const refreshCurrentUser = useCallback(async () => {
    const currentUser = await authService.getCurrentUser();
    setUser(currentUser);
  }, []);

  const initAuth = useCallback(async (): Promise<void> => {
    setIsLoading(true);

    if (authService.isAuthenticated()) {
      setToken(authService.getToken());
      await refreshCurrentUser();
      setIsLoggedIn(true);
      setIsLoading(false);
      return;
    }

    setIsLoggedIn(false);
    setUser(null);
    setToken(null);
    setIsLoading(false);
  }, [refreshCurrentUser]);

  useEffect(() => {
    initAuth();
  }, [initAuth]);

  const logout = useCallback(() => {
    authService.logout();
    setIsLoggedIn(false);
    setUser(null);
    setToken(null);
  }, []);

  const login = useCallback(async (request: LoginRequest): Promise<boolean> => {
    const loggedInUser = await authService.login(request);
    if (!loggedInUser) {
      return false;
    }

    setToken(authService.getToken());
    setUser(loggedInUser);
    setIsLoggedIn(true);
    return true;
  }, []);

  const register = useCallback(async (request: RegisterRequest): Promise<boolean> => {
    await authService.register(request);
    return true;
  }, []);

  return (
    <AuthContext.Provider
      value={{
        isLoggedIn,
        isLoading,
        user,
        token,
        logout,
        login,
        register,
        refreshCurrentUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
}
