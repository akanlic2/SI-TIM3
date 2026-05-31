import { useState, useEffect } from 'react';
import { fetchLogistics } from '../api/logisticsApi';
import type { LogisticsTask, LogisticsState } from '../types';

export function useLogistics(conferenceId: string, taskTypeFilter?: string): LogisticsState {
  const [items, setItems] = useState<LogisticsTask[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = async () => {
    if (!conferenceId) return;

    setIsLoading(true);
    setError(null);

    try {
      const tasks = await fetchLogistics(conferenceId, taskTypeFilter);
      setItems(tasks);
    } catch (err) {
      console.error('Error fetching logistics:', err);
      setError('Greška pri učitavanju logističkih aktivnosti');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    refresh();
  }, [conferenceId, taskTypeFilter]);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}
