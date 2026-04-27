// ─── Keycloak PKCE Auth Helper ────────────────────────────────────────────────
// Implementira Authorization Code Flow s PKCE za SPA aplikacije.
// Nema potrebe za keycloak-js bibliotekom.

const KEYCLOAK_URL = 'http://localhost:8080';
const REALM = 'conference-app';
const CLIENT_ID = 'conference-backend';
const REDIRECT_URI = `${window.location.origin}/callback`;

const KEYCLOAK_BASE = `${KEYCLOAK_URL}/realms/${REALM}/protocol/openid-connect`;

// ─── Storage Keys ─────────────────────────────────────────────────────────────
const TOKEN_KEY = 'kc_access_token';
const REFRESH_TOKEN_KEY = 'kc_refresh_token';
const ID_TOKEN_KEY = 'kc_id_token';
const CODE_VERIFIER_KEY = 'kc_code_verifier';

// ─── PKCE Utilities ───────────────────────────────────────────────────────────
function base64URLEncode(buffer: ArrayBuffer): string {
  return btoa(String.fromCharCode(...new Uint8Array(buffer)))
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=/g, '');
}

async function generateCodeVerifier(): Promise<string> {
  const array = new Uint8Array(32);
  crypto.getRandomValues(array);
  return base64URLEncode(array.buffer);
}

async function generateCodeChallenge(verifier: string): Promise<string> {
  const encoder = new TextEncoder();
  const data = encoder.encode(verifier);
  const hash = await crypto.subtle.digest('SHA-256', data);
  return base64URLEncode(hash);
}

// ─── Token Parsiranje ─────────────────────────────────────────────────────────
export interface TokenClaims {
  sub: string;
  email?: string;
  name?: string;
  given_name?: string;
  family_name?: string;
  preferred_username?: string;
  realm_access?: { roles: string[] };
  exp: number;
}

export function parseToken(token: string): TokenClaims | null {
  try {
    const payload = token.split('.')[1];
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decoded) as TokenClaims;
  } catch {
    return null;
  }
}

// ─── Provjera validnosti tokena ───────────────────────────────────────────────
export function isTokenValid(token: string): boolean {
  const claims = parseToken(token);
  if (!claims) return false;
  // Provjeravamo 30 sekundi unaprijed
  return claims.exp * 1000 > Date.now() + 30_000;
}

// ─── Dohvat sačuvanog tokena ──────────────────────────────────────────────────
export function getAccessToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function getRefreshToken(): string | null {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function isAuthenticated(): boolean {
  const token = getAccessToken();
  return token !== null && isTokenValid(token);
}

// ─── Pokretanje Login flow-a ──────────────────────────────────────────────────
export async function login(): Promise<void> {
  const verifier = await generateCodeVerifier();
  const challenge = await generateCodeChallenge(verifier);

  localStorage.setItem(CODE_VERIFIER_KEY, verifier);

  const params = new URLSearchParams({
    client_id: CLIENT_ID,
    redirect_uri: REDIRECT_URI,
    response_type: 'code',
    scope: 'openid email profile',
    code_challenge: challenge,
    code_challenge_method: 'S256',
  });

  window.location.href = `${KEYCLOAK_BASE}/auth?${params.toString()}`;
}

// ─── Obrada callback-a ────────────────────────────────────────────────────────
export async function handleCallback(): Promise<boolean> {
  const urlParams = new URLSearchParams(window.location.search);
  const code = urlParams.get('code');
  const error = urlParams.get('error');

  if (error) {
    console.error('Keycloak error:', error, urlParams.get('error_description'));
    return false;
  }

  if (!code) return false;

  const verifier = localStorage.getItem(CODE_VERIFIER_KEY);
  if (!verifier) {
    console.error('Code verifier not found');
    return false;
  }

  try {
    const response = await fetch(`${KEYCLOAK_BASE}/token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: new URLSearchParams({
        grant_type: 'authorization_code',
        client_id: CLIENT_ID,
        redirect_uri: REDIRECT_URI,
        code,
        code_verifier: verifier,
      }),
    });

    if (!response.ok) {
      const err = await response.text();
      console.error('Token exchange failed:', err);
      return false;
    }

    const tokens = await response.json();
    localStorage.setItem(TOKEN_KEY, tokens.access_token);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refresh_token ?? '');
    localStorage.setItem(ID_TOKEN_KEY, tokens.id_token ?? '');
    localStorage.removeItem(CODE_VERIFIER_KEY);

    return true;
  } catch (e) {
    console.error('Token exchange error:', e);
    return false;
  }
}

// ─── Refresh tokena ───────────────────────────────────────────────────────────
export async function refreshToken(): Promise<boolean> {
  const refresh = getRefreshToken();
  if (!refresh) return false;

  try {
    const response = await fetch(`${KEYCLOAK_BASE}/token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: new URLSearchParams({
        grant_type: 'refresh_token',
        client_id: CLIENT_ID,
        refresh_token: refresh,
      }),
    });

    if (!response.ok) return false;

    const tokens = await response.json();
    localStorage.setItem(TOKEN_KEY, tokens.access_token);
    if (tokens.refresh_token) localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refresh_token);
    return true;
  } catch {
    return false;
  }
}

// ─── Logout ───────────────────────────────────────────────────────────────────
export async function logout(): Promise<void> {
  const token = getAccessToken();
  const idToken = localStorage.getItem(ID_TOKEN_KEY);

  // Pozivamo backend logout rutu
  if (token) {
    try {
      await fetch('http://localhost:8082/api/user/logout', {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` },
      });
    } catch (e) {
      console.warn('Backend logout failed (continuing with local logout):', e);
    }
  }

  // Brišemo lokalne tokene
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(ID_TOKEN_KEY);

  // Redirect na Keycloak logout
  const params = new URLSearchParams({
    client_id: CLIENT_ID,
    post_logout_redirect_uri: window.location.origin,
  });
  if (idToken) params.set('id_token_hint', idToken);

  window.location.href = `${KEYCLOAK_BASE}/logout?${params.toString()}`;
}
