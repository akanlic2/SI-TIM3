import { useState, useEffect } from 'react';
import { fetchUsers } from '../api/sessionApi';
import type { User } from '../types';

export function useUsers() {
  const [items, setItems] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const users = await fetchUsers();
      setItems(users);
    } catch (err) {
      console.error('Error fetching users:', err);
      setError('Greška pri učitavanju korisnika');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    refresh();
  }, []);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}