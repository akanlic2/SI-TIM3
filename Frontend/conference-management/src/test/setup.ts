// ─── Vitest Global Test Setup ─────────────────────────────────────────────────
// Ovaj fajl se izvršava prije svakog test fajla.

import '@testing-library/jest-dom/vitest';
import { cleanup } from '@testing-library/react';
import { afterEach, vi } from 'vitest';

// Automatski cleanup nakon svakog testa
afterEach(() => {
  cleanup();
  localStorage.clear();
  vi.restoreAllMocks();
});

// ─── Mock window.location ─────────────────────────────────────────────────────
// Keycloak auth koristi window.location.href za redirect, pa trebamo mock
const locationMock = {
  ...window.location,
  href: 'http://localhost:5173',
  origin: 'http://localhost:5173',
  pathname: '/',
  search: '',
  assign: vi.fn(),
  replace: vi.fn(),
  reload: vi.fn(),
};

Object.defineProperty(window, 'location', {
  value: locationMock,
  writable: true,
});

// ─── Mock crypto.subtle za PKCE ───────────────────────────────────────────────
if (!globalThis.crypto?.subtle) {
  Object.defineProperty(globalThis, 'crypto', {
    value: {
      getRandomValues: (arr: Uint8Array) => {
        for (let i = 0; i < arr.length; i++) {
          arr[i] = Math.floor(Math.random() * 256);
        }
        return arr;
      },
      subtle: {
        digest: async (_algorithm: string, data: ArrayBuffer) => {
          // Vraća mock hash za testove
          const mockHash = new Uint8Array(32);
          const view = new Uint8Array(data);
          for (let i = 0; i < 32; i++) {
            mockHash[i] = view[i % view.length] ^ 0x5a;
          }
          return mockHash.buffer;
        },
      },
    },
  });
}
