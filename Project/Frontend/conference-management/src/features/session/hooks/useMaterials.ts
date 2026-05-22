import { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../../../auth/AuthProvider';
import { fetchSessionMaterials } from '../api/sessionApi';
import type { SessionMaterial } from '../types';

export function useMaterials(sessionId: string) {
  const { token } = useAuth();
  const [items, setItems] = useState<SessionMaterial[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    if (!sessionId || !token) {
      setItems([]);
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const materials = await fetchSessionMaterials(sessionId, token);
      setItems(materials);
    } catch (err) {
      console.error('Error fetching materials:', err);
      setError('Greška pri učitavanju materijala');
    } finally {
      setIsLoading(false);
    }
  }, [sessionId, token]);

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
