import React, { useEffect, useState, useRef } from 'react';
import { handleCallback } from '../auth/keycloak';
import './CallbackPage.css';

export default function CallbackPage() {
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [errorMsg, setErrorMsg] = useState('');
  const initialized = useRef(false);

  useEffect(() => {
    if (initialized.current) return;
    initialized.current = true;

    handleCallback().then((success) => {
      if (success) {
        setStatus('success');
        // Redirect na dashboard
        setTimeout(() => {
          window.history.replaceState({}, '', '/dashboard');
          window.dispatchEvent(new PopStateEvent('popstate'));
        }, 800);
      } else {
        // Provjeri da li smo zapravo već uspješno odradili ovo (npr. u Strict Mode)
        if (localStorage.getItem('kc_access_token')) {
          setStatus('success');
          window.history.replaceState({}, '', '/dashboard');
          window.dispatchEvent(new PopStateEvent('popstate'));
          return;
        }

        setStatus('error');
        setErrorMsg('Autentifikacija nije uspjela. Pokušajte ponovo.');
        setTimeout(() => {
          window.history.replaceState({}, '', '/');
          window.dispatchEvent(new PopStateEvent('popstate'));
        }, 3000);
      }
    });
  }, []);

  return (
    <div className="callback-container">
      <div className="callback-card">
        {status === 'loading' && (
          <>
            <div className="callback-spinner" />
            <p className="callback-text">Verifikacija identiteta...</p>
          </>
        )}
        {status === 'success' && (
          <>
            <div className="callback-icon success">✓</div>
            <p className="callback-text">Uspješno ste prijavljeni!</p>
            <p className="callback-sub">Preusmjeravamo vas na Dashboard...</p>
          </>
        )}
        {status === 'error' && (
          <>
            <div className="callback-icon error">✕</div>
            <p className="callback-text">Greška pri prijavi</p>
            <p className="callback-sub">{errorMsg}</p>
          </>
        )}
      </div>
    </div>
  );
}
