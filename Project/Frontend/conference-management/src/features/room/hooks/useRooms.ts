import { useState, useEffect } from 'react';
import { fetchRooms } from '../api/roomApi';
import type { Room } from '../types';

export function useRooms() {
  const [items, setItems] = useState<Room[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const rooms = await fetchRooms();
      setItems(rooms);
    } catch (err) {
      console.error('Greška pri učitavanju dvorana:', err);
      setError('Greška pri učitavanju dvorana');
      setItems([]);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void refresh();
  }, []);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}
