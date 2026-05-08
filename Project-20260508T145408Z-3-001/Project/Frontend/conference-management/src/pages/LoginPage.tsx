import { useState } from 'react';
import { useAuth } from '../auth/AuthProvider';
import './LoginPage.css';

export default function LoginPage() {
  const { login, isLoading } = useAuth();
  const [usernameOrEmail, setUsernameOrEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      const success = await login({ usernameOrEmail, password });
      if (!success) {
        setError('Pogrešni kredencijali.');
        return;
      }

      window.history.replaceState({}, '', '/dashboard');
      window.dispatchEvent(new PopStateEvent('popstate'));
    } catch {
      setError('Prijava nije uspjela.');
    }
  };

  return (
    <main className="auth-page">
      <form onSubmit={onSubmit} className="auth-card auth-form">
        <h2 className="auth-title">Prijava</h2>
        <p className="auth-subtitle">Prijavite se na vaš korisnički račun</p>
        <input
          type="text"
          placeholder="Username ili Email"
          className="auth-input"
          value={usernameOrEmail}
          onChange={(e) => setUsernameOrEmail(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Password"
          className="auth-input"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit" className="auth-submit" disabled={isLoading}>Login</button>
        {error && <p className="auth-error">{error}</p>}
        <button
          type="button"
          className="auth-link-btn"
          onClick={() => {
            window.history.pushState({}, '', '/register');
            window.dispatchEvent(new PopStateEvent('popstate'));
          }}
        >
          Nemate račun? Registrujte se
        </button>
      </form>
    </main>
  );
}
