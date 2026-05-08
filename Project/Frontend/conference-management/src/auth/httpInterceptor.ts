import axios from 'axios';
import { authService } from './authService';

let initialized = false;

export function setupAuthInterceptor(): void {
  if (initialized) return;
  initialized = true;

  // Velika izmjena: globalni interceptor automatski dodaje lokalni JWT.
  axios.interceptors.request.use((config) => {
    const token = authService.getToken();
    if (token) {
      const headers = config.headers as { set?: (name: string, value: string) => void } | undefined;
      if (headers?.set) {
        headers.set('Authorization', `Bearer ${token}`);
      } else {
        config.headers = { Authorization: `Bearer ${token}` } as never;
      }
    }

    return config;
  });
}
