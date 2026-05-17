import { useEffect, useState } from 'react';
import { AuthProvider, useAuth } from '../auth/AuthProvider';
import DashboardPage from '../pages/DashboardPage';
import ConferencesPage from '../pages/ConferencesPage';
import ConferenceDetailsPage from '../pages/ConferenceDetailsPage';
import RoomsPage from '../pages/RoomsPage';
import SessionsPage from '../pages/SessionsPage';
import AgendaPage from '../pages/AgendaPage';
import LoginPage from '../pages/LoginPage';
import RegisterPage from '../pages/RegisterPage';

function AppRoutes() {
  const [pathname, setPathname] = useState(window.location.pathname);
  const { isLoggedIn, isLoading } = useAuth();

  useEffect(() => {
    const onPopState = () => setPathname(window.location.pathname);
    const onPushState = () => setPathname(window.location.pathname);

    window.addEventListener('popstate', onPopState);

    const originalPushState = window.history.pushState;
    window.history.pushState = function (state, title, url) {
      originalPushState.call(this, state, title, url);
      onPushState();
    };

    return () => {
      window.removeEventListener('popstate', onPopState);
      window.history.pushState = originalPushState;
    };
  }, []);

  useEffect(() => {
    if (isLoading) return;

    const isProtectedRoute =
      pathname === '/dashboard' ||
      pathname === '/conferences' ||
      pathname.startsWith('/conferences/') ||
      pathname === '/rooms';

    if (!isLoggedIn && isProtectedRoute) {
      window.history.replaceState({}, '', '/login');
      setPathname('/login');
      return;
    }

    if (isLoggedIn && (pathname === '/' || pathname === '/login' || pathname === '/register')) {
      window.history.replaceState({}, '', '/dashboard');
      setPathname('/dashboard');
    }
  }, [pathname, isLoggedIn, isLoading]);

  if (pathname === '/login') return <LoginPage />;
  if (pathname === '/register') return <RegisterPage />;

  if (isLoading) {
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Učitavanje...</p>
      </div>
    );
  }

  if (pathname === '/dashboard') {
    if (isLoggedIn) return <DashboardPage />;
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje na prijavu...</p>
      </div>
    );
  }

  if (pathname === '/conferences') {
    if (isLoggedIn) return <ConferencesPage />;
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje na prijavu...</p>
      </div>
    );
  }

  if (pathname === '/rooms') {
    if (isLoggedIn) return <RoomsPage />;
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje na prijavu...</p>
      </div>
    );
  }

  if (pathname.startsWith('/conferences/') && pathname.endsWith('/sessions')) {
    if (isLoggedIn) return <SessionsPage />;
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje na prijavu...</p>
      </div>
    );
  }

  if (pathname.startsWith('/conferences/') && pathname.endsWith('/agenda')) {
    if (isLoggedIn) return <AgendaPage />;
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje na prijavu...</p>
      </div>
    );
  }

  if (pathname.startsWith('/conferences/')) {
    if (isLoggedIn) return <ConferenceDetailsPage />;
    return (
      <div className="global-loading">
        <div className="global-spinner" />
        <p>Preusmjeravanje na prijavu...</p>
      </div>
    );
  }

  if (pathname === '/') {
    return <LoginPage />;
  }

  return (
    <div className="global-loading">
      <p style={{ color: '#7a8bb0', fontSize: '1.1rem' }}>404 – Stranica nije pronađena</p>
      <a href="/" style={{ color: '#3f83f8' }}>← Povratak na početnu</a>
    </div>
  );
}

export function Router() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  );
}