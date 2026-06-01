import axios from 'axios';
import type { CreateEquipmentData, Equipment, AssignEquipmentData } from '../types';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';

export async function fetchEquipment(): Promise<Equipment[]> {
  try {
    const response = await axios.get<Equipment[]>(`${BASE_URL}/api/equipment`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju opreme:', error);
    throw new Error('Greška pri dohvatanju opreme.');
  }
}

export async function createEquipment(data: CreateEquipmentData): Promise<Equipment> {
  try {
    const response = await axios.post<Equipment>(`${BASE_URL}/api/equipment`, data);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      const responseData = error.response.data as { error?: string } | string;
      const message =
        typeof responseData === 'string'
          ? responseData
          : responseData.error ?? (responseData as any).message;
      throw new Error(message ?? `Status ${error.response?.status ?? 'unknown'}`);
    }
    throw new Error('Greška pri kreiranju opreme.');
  }
}

export async function deleteEquipment(id: string): Promise<void> {
  try {
    await axios.delete(`${BASE_URL}/api/equipment/${id}`);
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      const responseData = error.response.data as { error?: string } | string;
      const message =
        typeof responseData === 'string'
          ? responseData
          : responseData.error ?? (responseData as any).message;
      throw new Error(message ?? `Status ${error.response?.status ?? 'unknown'}`);
    }
    throw new Error('Greška pri brisanju opreme.');
  }
}

export async function decrementEquipmentQuantity(id: string): Promise<Equipment> {
  try {
    const response = await axios.patch<Equipment>(`${BASE_URL}/api/equipment/${id}/decrement`);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      const responseData = error.response.data as { error?: string } | string;
      const message =
        typeof responseData === 'string'
          ? responseData
          : responseData.error ?? (responseData as any).message;
      throw new Error(message ?? `Status ${error.response?.status ?? 'unknown'}`);
    }
    throw new Error('Greška pri smanjenju kolicine opreme.');
  }
}

export async function fetchSessionEquipment(sessionId: string): Promise<Equipment[]> {
  try {
    const response = await axios.get<Equipment[]>(`${BASE_URL}/api/sessions/${sessionId}/equipment`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju opreme za sesiju:', error);
    throw new Error('Greška pri dohvatanju opreme za sesiju.');
  }
}

export async function assignEquipmentToSession(
  sessionId: string,
  data: AssignEquipmentData
): Promise<void> {
  try {
    await axios.post(`${BASE_URL}/api/sessions/${sessionId}/equipment`, data);
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      const responseData = error.response.data as { error?: string } | string;
      const message =
        typeof responseData === 'string'
          ? responseData
          : responseData.error ?? (responseData as any).message;
      throw new Error(message ?? `Status ${error.response?.status ?? 'unknown'}`);
    }
    throw new Error('Greška pri dodjeli opreme sesiji.');
  }
}

export async function unassignEquipmentFromSession(
  sessionId: string,
  equipmentId: string
): Promise<void> {
  try {
    await axios.delete(`${BASE_URL}/api/sessions/${sessionId}/equipment/${equipmentId}`);
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      const responseData = error.response.data as { error?: string } | string;
      const message =
        typeof responseData === 'string'
          ? responseData
          : responseData.error ?? (responseData as any).message;
      throw new Error(message ?? `Status ${error.response?.status ?? 'unknown'}`);
    }
    throw new Error('Greška pri oslobađanju opreme.');
  }
}
