import axios from 'axios';
import type { CreateRoomData, Room } from '../types';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';

export async function fetchRooms(): Promise<Room[]> {
  try {
    const response = await axios.get<Room[]>(`${BASE_URL}/api/rooms`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju dvorana:', error);
    throw new Error('Greška pri dohvatanju dvorana.');
  }
}

export async function createRoom(roomData: CreateRoomData): Promise<Room> {
  try {
    const response = await axios.post<Room>(`${BASE_URL}/api/rooms`, roomData);
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
    throw new Error('Greška pri kreiranju dvorane.');
  }
}
