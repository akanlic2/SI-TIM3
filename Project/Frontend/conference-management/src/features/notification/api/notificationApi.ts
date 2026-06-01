import axios from 'axios';
import type { Notification } from '../types';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';
const API_URL = `${BASE_URL}/api/Notifications`;

export async function fetchMyNotifications(): Promise<Notification[]> {
  try {
    const response = await axios.get<Notification[]>(`${API_URL}/me`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju notifikacija:', error);
    return [];
  }
}

export async function markNotificationAsRead(id: string): Promise<void> {
  await axios.put(`${API_URL}/${id}/read`);
}

export async function markAllNotificationsAsRead(): Promise<void> {
  await axios.put(`${API_URL}/read-all`);
}
