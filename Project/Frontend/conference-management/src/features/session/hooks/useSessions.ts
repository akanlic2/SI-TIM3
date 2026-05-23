import { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../../../auth/AuthProvider';
import { fetchSessions } from '../api/sessionApi';
import type { Session } from '../types';

export function useSessions(conferenceId: string) {
  const { token } = useAuth();
  const [items, setItems] = useState<Session[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    if (!conferenceId || !token) return;

    setIsLoading(true);
    setError(null);

    try {
      const sessions = await fetchSessions(conferenceId, token);
      setItems(sessions);
    } catch (err) {
      console.error('Error fetching sessions:', err);
      setError('Greška pri učitavanju sesija');
    } finally {
      setIsLoading(false);
    }
  }, [conferenceId, token]);

  useEffect(() => {
    refresh();
  }, [refresh]);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}