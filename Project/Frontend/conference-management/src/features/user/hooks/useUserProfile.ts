import { useEffect, useMemo, useState } from 'react';
import { fetchUserProfile, updateUserProfile } from '../api/userApi';
import type { AuthUser } from '../../../auth/authService';
import type { UserProfile } from '../types';

interface UseUserProfileParams {
  user: AuthUser | null;
  targetUser?: UserProfile | null;
  allowRoleUpdate?: boolean;
}

function toInitialProfile(user: AuthUser | null, targetUser?: UserProfile | null): UserProfile {
  if (targetUser) {
    return {
      id: targetUser.id ?? user?.userId,
      firstName: targetUser.firstName ?? '',
      lastName: targetUser.lastName ?? '',
      username: targetUser.username ?? '',
      email: targetUser.email ?? '',
      role: targetUser.role ?? '',
    };
  }

  return {
    id: user?.userId,
    firstName: user?.firstName ?? '',
    lastName: user?.lastName ?? '',
    username: user?.username ?? '',
    email: user?.email ?? '',
    role: user?.role ?? '',
  };
}

export function useUserProfile({ user, targetUser, allowRoleUpdate = false }: UseUserProfileParams) {
  const initialProfile = useMemo(() => toInitialProfile(user, targetUser), [user, targetUser]);
  const userId = initialProfile.id ?? user?.userId;

  const [profile, setProfile] = useState<UserProfile>(initialProfile);
  const [loading, setLoading] = useState(false);
  const [editing, setEditing] = useState(false);
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    setProfile(initialProfile);
    setEditing(false);
    setPassword('');
    setMessage(null);
  }, [initialProfile]);

  useEffect(() => {
    async function load() {
      if (!userId) return;

      setLoading(true);
      const loadedProfile = await fetchUserProfile(userId);
      if (loadedProfile) {
        setProfile((prev) => ({ ...prev, ...loadedProfile }));
      }
      setLoading(false);
    }

    load();
  }, [userId]);

  const cancelEditing = () => {
    setEditing(false);
    setPassword('');
    setMessage(null);
  };

  const saveProfile = async () => {
    if (!userId) return;

    setLoading(true);
    setMessage(null);

    const error = await updateUserProfile(userId, {
      firstName: profile.firstName,
      lastName: profile.lastName,
      username: profile.username,
      email: profile.email,
      ...(allowRoleUpdate ? { role: profile.role } : {}),
      ...(password.trim() ? { password } : {}),
    });

    if (error) {
      setMessage(`Greška: ${error}`);
      setLoading(false);
      return;
    }

    setMessage('Promjene su sačuvane.');
    setEditing(false);
    setPassword('');
    setLoading(false);
  };

  return {
    profile,
    setProfile,
    loading,
    editing,
    setEditing,
    password,
    setPassword,
    message,
    saveProfile,
    cancelEditing,
  };
}
