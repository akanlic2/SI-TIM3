import { useEffect, useMemo, useState } from 'react';
import { fetchUserProfile, updateUserProfile } from '../api/userApi';
import type { TokenClaims } from '../../../auth/keycloak';
import type { UserProfile } from '../types';

interface UseUserProfileParams {
  user: TokenClaims | null;
  token: string | null;
  targetUser?: UserProfile | null;
}

function toInitialProfile(user: TokenClaims | null, targetUser?: UserProfile | null): UserProfile {
  if (targetUser) {
    return {
      id: targetUser.id ?? user?.sub,
      firstName: targetUser.firstName ?? '',
      lastName: targetUser.lastName ?? '',
      username: targetUser.username ?? '',
      email: targetUser.email ?? '',
    };
  }

  return {
    id: user?.sub,
    firstName: user?.given_name ?? '',
    lastName: user?.family_name ?? '',
    username: user?.preferred_username ?? '',
    email: user?.email ?? '',
  };
}

export function useUserProfile({ user, token, targetUser }: UseUserProfileParams) {
  const initialProfile = useMemo(() => toInitialProfile(user, targetUser), [user, targetUser]);
  const userId = initialProfile.id ?? user?.sub;

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
      if (!token || !userId) return;

      setLoading(true);
      const loadedProfile = await fetchUserProfile(userId, token);
      if (loadedProfile) {
        setProfile((prev) => ({ ...prev, ...loadedProfile }));
      }
      setLoading(false);
    }

    load();
  }, [token, userId]);

  const cancelEditing = () => {
    setEditing(false);
    setPassword('');
    setMessage(null);
  };

  const saveProfile = async () => {
    if (!token || !userId) return;

    setLoading(true);
    setMessage(null);

    const error = await updateUserProfile(userId, token, {
      firstName: profile.firstName,
      lastName: profile.lastName,
      username: profile.username,
      email: profile.email,
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
