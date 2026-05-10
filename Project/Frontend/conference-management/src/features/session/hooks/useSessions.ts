import { useState, useEffect } from 'react';
import { fetchSessions } from '../api/sessionApi';
import type { Session } from '../types';

export function useSessions(conferenceId: string) {
  const [items, setItems] = useState<Session[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = async () => {
    if (!conferenceId) return;

    setIsLoading(true);
    setError(null);

    try {
      const sessions = await fetchSessions(conferenceId);
      setItems(sessions);
    } catch (err) {
      console.error('Error fetching sessions:', err);
      setError('Greška pri učitavanju sesija');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    refresh();
  }, [conferenceId]);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}