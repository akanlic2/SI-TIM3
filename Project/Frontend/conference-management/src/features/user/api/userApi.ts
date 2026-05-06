import axios from 'axios';
import type { UpdateUserProfileData, UserProfile } from '../types';

export async function fetchUserProfile(userId: string, token: string): Promise<UserProfile | null> {
  try {
    const res = await axios.get(`/api/user/${userId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = res.data;

    return {
      id: data.id ?? data.sub ?? userId,
      firstName: data.firstName ?? data.given_name ?? '',
      lastName: data.lastName ?? data.family_name ?? '',
      username: data.username ?? data.preferred_username ?? '',
      email: data.email ?? '',
    };
  } catch {
    return null;
  }
}

export async function updateUserProfile(userId: string, token: string, payload: UpdateUserProfileData): Promise<string | null> {
  try {
    await axios.put(`/api/user/${userId}`, payload, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });
    return null;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const message =
        typeof error.response?.data === 'string'
          ? error.response.data
          : error.message;
      return message || `Status ${error.response?.status ?? 'unknown'}`;
    }
    return 'Greška pri čuvanju podataka.';
  }
}
