import React, { useEffect, useState } from 'react';
import { AuthProvider, useAuth } from '../auth/AuthProvider';
import CallbackPage from '../pages/CallbackPage';
import DashboardPage from '../pages/DashboardPage';
import { login } from '../auth/keycloak';

// ─── Route resolver ───────────────────────────────────────────────────────────
function AppRoutes() {
  const [pathname, setPathname] = useState(window.location.pathname);
  const { isLoggedIn, isLoading } = useAuth();

  useEffect(() => {
    const onPopState = () => setPathname(window.location.pathname);
    window.addEventListener('popstate', onPopState);
    return () => window.removeEventListener('popstate', onPopState);
  }, []);

  // Handle redirects and login triggers in useEffect
  useEffect(() => {
    if (isLoading) return;

    if (pathname === '/') {
      if (isLoggedIn) {
        window.history.replaceState({}, '', '/dashboard');
        setPathname('/dashboard');
      } else {
        login();
      }
    } else if (pathname === '/dashboard' && !isLoggedIn) {
      login();
    }
  }, [pathname, isLoggedIn, isLoading]);

  // Callback stranica – uvijek dostupna (Keycloak redirect)
  if (pathname === '/callback') {
    return <CallbackPage />;
  }

  // Loading state
  if (isLoading) {
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Učitavanje...</p>
      </div>
    );
  }

  // Dashboard – samo ako je logovan
  if (pathname === '/dashboard') {
    if (isLoggedIn) return <DashboardPage />;
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje na prijavu...</p>
      </div>
    );
  }

  // Root ruta "/"
  if (pathname === '/') {
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje...</p>
      </div>
    );
  }

  // 404 fallback
  return (
    <div className="global-loading">
      <p style={{ color: '#7a8bb0', fontSize: '1.1rem' }}>404 – Stranica nije pronađena</p>
      <a href="/" style={{ color: '#3f83f8' }}>← Povratak na početnu</a>
    </div>
  );
}

// ─── Router export ────────────────────────────────────────────────────────────
export function Router() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  );
}
