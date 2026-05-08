import axios from 'axios';

const TOKEN_KEY = 'auth_token';
const API_BASE_URL = import.meta.env.VITE_API_URL ?? '';

function toApiUrl(path: string): string {
  return API_BASE_URL ? `${API_BASE_URL}${path}` : path;
}

export interface JwtPayload {
  exp?: number;
  userId?: string;
  username?: string;
  email?: string;
  role?: string;
  sub?: string;
  unique_name?: string;
  name?: string;
  [key: string]: unknown;
}

export interface AuthUser {
  userId: string;
  username: string;
  email: string;
  role: string;
  firstName?: string;
  lastName?: string;
}

export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  email: string;
  role?: string;
}

function parseToken(token: string): JwtPayload | null {
  try {
    const payload = token.split('.')[1];
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decoded) as JwtPayload;
  } catch {
    return null;
  }
}

function mapPayloadToUser(payload: JwtPayload | null): AuthUser | null {
  if (!payload) return null;

  const userId =
    (payload.userId as string | undefined) ??
    (payload.sub as string | undefined) ??
    (payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] as string | undefined) ??
    '';

  const username =
    (payload.username as string | undefined) ??
    (payload.unique_name as string | undefined) ??
    (payload.name as string | undefined) ??
    (payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] as string | undefined) ??
    '';

  const email =
    (payload.email as string | undefined) ??
    (payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] as string | undefined) ??
    '';

  const role =
    (payload.role as string | undefined) ??
    (payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] as string | undefined) ??
    '';

  return { userId, username, email, role };
}

function isExpired(payload: JwtPayload | null): boolean {
  if (!payload?.exp) return true;
  return payload.exp * 1000 <= Date.now();
}

function setToken(token: string) {
  localStorage.setItem(TOKEN_KEY, token);
}

function clearToken() {
  localStorage.removeItem(TOKEN_KEY);
}

async function login(request: LoginRequest): Promise<AuthUser | null> {
  const response = await axios.post(toApiUrl('/api/user/login'), request);
  const token = response.data?.token as string | undefined;
  if (!token) return null;

  setToken(token);
  return mapPayloadToUser(parseToken(token));
}

async function register(request: RegisterRequest): Promise<boolean> {
  await axios.post(toApiUrl('/api/user/register'), request);
  return true;
}

function logout(): void {
  clearToken();
}

function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

function isAuthenticated(): boolean {
  const token = getToken();
  if (!token) return false;

  const payload = parseToken(token);
  return !isExpired(payload);
}

async function getCurrentUser(): Promise<AuthUser | null> {
  const token = getToken();
  if (!token) return null;

  const payload = parseToken(token);
  if (isExpired(payload)) {
    clearToken();
    return null;
  }

  const tokenUser = mapPayloadToUser(payload);

  try {
    // Velika izmjena: korisnik se sada čita iz lokalnog JWT-a i backend baze.
    const response = await axios.get(toApiUrl('/api/user/current'));
    const user = response.data;

    return {
      userId: user.userId ?? tokenUser?.userId ?? '',
      username: user.username ?? tokenUser?.username ?? '',
      email: user.email ?? tokenUser?.email ?? '',
      role: user.role ?? tokenUser?.role ?? '',
      firstName: user.firstName,
      lastName: user.lastName,
    };
  } catch {
    return tokenUser;
  }
}

export const authService = {
  login,
  register,
  logout,
  isAuthenticated,
  getToken,
  getCurrentUser,
};
