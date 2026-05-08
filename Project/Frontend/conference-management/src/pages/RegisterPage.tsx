import { useState } from 'react';
import { useAuth } from '../auth/AuthProvider';
import axios from 'axios';
import './RegisterPage.css';

export default function RegisterPage() {
  const { register } = useAuth();
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      await register({
        firstName,
        lastName,
        username,
        email,
        password,
        role: 'ucesnik',
      });

      window.history.replaceState({}, '', '/login');
      window.dispatchEvent(new PopStateEvent('popstate'));
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const apiMessage = err.response?.data?.error;
        setError(typeof apiMessage === 'string' ? apiMessage : 'Registracija nije uspjela. Provjerite username/email.');
      } else {
        setError('Registracija nije uspjela. Provjerite username/email.');
      }
    }
  };

  return (
    <main className="auth-page">
      <form onSubmit={onSubmit} className="auth-card auth-form">
        <h2 className="auth-title">Registracija</h2>
        <p className="auth-subtitle">Kreirajte novi korisnički račun</p>

        <div className="auth-grid-2">
          <input className="auth-input" type="text" placeholder="Ime" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
          <input className="auth-input" type="text" placeholder="Prezime" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
        </div>

        <input className="auth-input" type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} required />
        <input className="auth-input" type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <input className="auth-input" type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} required />

        <button type="submit" className="auth-submit">Register</button>
        {error && <p className="auth-error">{error}</p>}
        <button
          type="button"
          className="auth-link-btn"
          onClick={() => {
            window.history.pushState({}, '', '/login');
            window.dispatchEvent(new PopStateEvent('popstate'));
          }}
        >
          Već imate račun? Prijavite se
        </button>
      </form>
    </main>
  );
}
