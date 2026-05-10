import axios from 'axios';
import type { UpdateUserProfileData, UserProfile, UserSummary } from '../types';

interface UserApiResponse {
  userId?: string;
  firstName?: string;
  lastName?: string;
  username?: string;
  email?: string;
  role?: string;
}

export async function fetchUserProfile(userId: string): Promise<UserProfile | null> {
  try {
    const res = await axios.get(`/api/user/${userId}`);
    const data = res.data;

    return {
      id: data.userId ?? userId,
      firstName: data.firstName ?? '',
      lastName: data.lastName ?? '',
      username: data.username ?? '',
      email: data.email ?? '',
      role: data.role ?? '',
    };
  } catch {
    return null;
  }
}

export async function updateUserProfile(userId: string, payload: UpdateUserProfileData): Promise<string | null> {
  try {
    await axios.put(`/api/user/${userId}`, payload, {
      headers: { 'Content-Type': 'application/json' },
    });
    return null;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const responseData = error.response?.data;
      const message =
        typeof responseData === 'string'
          ? responseData
          : typeof responseData?.error === 'string'
            ? responseData.error
            : typeof responseData?.message === 'string'
              ? responseData.message
              : error.message;
      return message || `Status ${error.response?.status ?? 'unknown'}`;
    }
    return 'Greška pri čuvanju podataka.';
  }
}

export async function fetchAllUsers(): Promise<UserSummary[]> {
  try {
    const res = await axios.get('/api/users/all');

    const rootData = res.data;
    const data = Array.isArray(rootData) ? rootData : Array.isArray(rootData?.users) ? rootData.users : [];

    return (data as UserApiResponse[]).map((item) => ({
      id: item.userId ?? '',
      firstName: item.firstName ?? '',
      lastName: item.lastName ?? '',
      username: item.username ?? '',
      email: item.email ?? '',
      role: item.role ?? '',
      roles: item.role ? [item.role] : [],
    }));
  } catch {
    return [];
  }
}
