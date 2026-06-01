import { useState, useEffect, useCallback } from 'react';
import { fetchEquipment, fetchSessionEquipment } from '../api/equipmentApi';
import type { Equipment } from '../types';

export function useEquipment() {
  const [items, setItems] = useState<Equipment[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const equipment = await fetchEquipment();
      setItems(equipment);
    } catch (err) {
      console.error('Greška pri učitavanju opreme:', err);
      setError('Greška pri učitavanju opreme.');
      setItems([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}

export function useSessionEquipment(sessionId: string) {
  const [items, setItems] = useState<Equipment[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    if (!sessionId) return;
    setIsLoading(true);
    setError(null);

    try {
      const equipment = await fetchSessionEquipment(sessionId);
      setItems(equipment);
    } catch (err) {
      console.error('Greška pri učitavanju opreme za sesiju:', err);
      setError('Greška pri učitavanju opreme za sesiju.');
      setItems([]);
    } finally {
      setIsLoading(false);
    }
  }, [sessionId]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  return {
    items,
    isLoading,
    error,
    refresh,
  };
}
