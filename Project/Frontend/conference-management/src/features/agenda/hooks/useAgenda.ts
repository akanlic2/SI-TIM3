import { useState, useEffect } from 'react';
import { fetchAgenda } from '../api/agendaApi';
import type { AgendaItem, AgendaState } from '../types';

export function useAgenda(conferenceId: string): AgendaState {
  const [items, setItems] = useState<AgendaItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = async () => {
    if (!conferenceId) return;

    setIsLoading(true);
    setError(null);

    try {
      const agenda = await fetchAgenda(conferenceId);
      setItems(agenda);
    } catch (err) {
      console.error('Error fetching agenda:', err);
      setError('Greška pri učitavanju agende');
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
