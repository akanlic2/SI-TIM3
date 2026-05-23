import { useState, useEffect } from 'react';
import { fetchSpeakerSessions } from '../api/sessionApi';
import type { Session } from '../types';

// Extended type for backend speaker sessions response
export interface BackendSpeakerSession extends Session {
  conferenceTitle?: string;
  conferenceName?: string;
  conferenceId?: string;
}

export function useSpeakerSessions(token?: string, enabled = false) {
  const [items, setItems] = useState<BackendSpeakerSession[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = async () => {
    if (!token || !enabled) return;

    setIsLoading(true);
    setError(null);

    try {
      const sessions = await fetchSpeakerSessions(token);
      // Map backend conferenceTitle to conferenceName for UI display
      setItems(
       sessions.map((s: any) => ({
        ...s,
        conferenceName: s.conferenceTitle,
        roomName: s.location,
        } as BackendSpeakerSession))
      );
    } catch (err) {
      console.error('Greška pri dohvatanju predavačevih sesija:', err);
      setError('Greška pri učitavanju sesija');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (!token || !enabled) {
      setItems([]);
      setIsLoading(false);
      setError(null);
      return;
    }

    refresh();
  }, [token, enabled]);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}