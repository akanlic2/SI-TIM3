// ─── Testovi za CallbackPage komponentu ───────────────────────────────────────
// Testiramo: prikaz loadinga, uspješnu autentifikaciju, neuspješan callback

import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import CallbackPage from '../CallbackPage';

// ─── Helpers ──────────────────────────────────────────────────────────────────

function createMockJWT(payload: Record<string, unknown>): string {
  const header = btoa(JSON.stringify({ alg: 'RS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  return `${header}.${body}.mock-sig`;
}

function createValidToken(): string {
  return createMockJWT({
    sub: 'user-123',
    exp: Math.floor(Date.now() / 1000) + 3600,
  });
}

// ─── Mock keycloak handleCallback ─────────────────────────────────────────────

vi.mock('../../auth/keycloak', () => ({
  handleCallback: vi.fn(),
}));

import { handleCallback } from '../../auth/keycloak';

// ─── Mock PopStateEvent za navigaciju ─────────────────────────────────────────

let popStateEvents: Array<() => void> = [];
const originalAddEventListener = window.addEventListener;

beforeEach(() => {
  vi.clearAllMocks();
  popStateEvents = [];
  localStorage.clear();

  // Track popstate dispatches
  vi.spyOn(window, 'dispatchEvent').mockImplementation((event) => {
    if (event.type === 'popstate') {
      popStateEvents.push(() => {});
    }
    return true;
  });

  vi.spyOn(window.history, 'replaceState').mockImplementation(() => {});
});

// ─── Testovi ──────────────────────────────────────────────────────────────────

describe('CallbackPage', () => {
  it('prikazuje loading spinner inicijalno', () => {
    // handleCallback nikad ne završi (pending)
    vi.mocked(handleCallback).mockReturnValue(new Promise(() => {}));

    render(<CallbackPage />);

    expect(screen.getByText('Verifikacija identiteta...')).toBeInTheDocument();
  });

  it('prikazuje success poruku nakon uspješnog callback-a', async () => {
    vi.mocked(handleCallback).mockResolvedValue(true);

    render(<CallbackPage />);

    await waitFor(() => {
      expect(screen.getByText('Uspješno ste prijavljeni!')).toBeInTheDocument();
    });

    expect(screen.getByText('Preusmjeravamo vas na Dashboard...')).toBeInTheDocument();
  });

  it('redirecta na /dashboard nakon uspješnog callback-a', async () => {
    vi.mocked(handleCallback).mockResolvedValue(true);
    const replaceStateSpy = vi.spyOn(window.history, 'replaceState');

    render(<CallbackPage />);

    await waitFor(
      () => {
        expect(replaceStateSpy).toHaveBeenCalledWith({}, '', '/dashboard');
      },
      { timeout: 2000 }
    );
  });

  it('prikazuje error poruku kad callback ne uspije', async () => {
    vi.mocked(handleCallback).mockResolvedValue(false);
    // Nema tokena u localStorage = pravi error
    localStorage.removeItem('kc_access_token');

    render(<CallbackPage />);

    await waitFor(() => {
      expect(screen.getByText('Greška pri prijavi')).toBeInTheDocument();
    });

    expect(
      screen.getByText('Autentifikacija nije uspjela. Pokušajte ponovo.')
    ).toBeInTheDocument();
  });

  it('redirecta na / kad callback ne uspije (fallback na login)', async () => {
    vi.mocked(handleCallback).mockResolvedValue(false);
    localStorage.removeItem('kc_access_token');

    const replaceStateSpy = vi.spyOn(window.history, 'replaceState');

    render(<CallbackPage />);

    await waitFor(
      () => {
        expect(replaceStateSpy).toHaveBeenCalledWith({}, '', '/');
      },
      { timeout: 5000 }
    );
  });

  it('tretira kao success ako token postoji u localStorage uprkos failed callback-u (Strict Mode)', async () => {
    // Ovo simulira scenario gdje je React Strict Mode pozvao handleCallback dvaput
    // Prvi poziv je uspio i spremio token, drugi fail-a ali token postoji
    vi.mocked(handleCallback).mockResolvedValue(false);
    localStorage.setItem('kc_access_token', createValidToken());

    render(<CallbackPage />);

    await waitFor(() => {
      expect(screen.getByText('Uspješno ste prijavljeni!')).toBeInTheDocument();
    });
  });
});
