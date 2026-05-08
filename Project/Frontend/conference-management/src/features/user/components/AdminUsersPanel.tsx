import { useEffect, useMemo, useState } from 'react';
import { useAuth } from '../../../auth/AuthProvider';
import { fetchAllUsers } from '../api/userApi';
import type { UserProfile, UserSummary } from '../types';
import { UserSettingsPanel } from './UserSettingsPanel';
import './AdminUsersPanel.css';

function isAdminRole(roles: string[] | undefined): boolean {
  return roles?.some((role) => role.toLowerCase().includes('admin')) ?? false;
}

export function AdminUsersPanel() {
  const { user } = useAuth();
  const canView = isAdminRole(user?.role ? [user.role] : []);
  const [users, setUsers] = useState<UserSummary[]>([]);
  const [selectedUser, setSelectedUser] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(false);

  const currentUserDisplayName = useMemo(
    () => user?.username ?? user?.email ?? 'Admin',
    [user],
  );

  useEffect(() => {
    async function loadUsers() {
      if (!canView) return;

      setLoading(true);
      const allUsers = await fetchAllUsers();
      setUsers(allUsers);
      setSelectedUser((prev) => prev ?? allUsers[0] ?? null);
      setLoading(false);
    }

    loadUsers();
  }, [canView]);

  if (!canView) return null;

  return (
    <section className="section-block admin-users-panel">
      <div className="section-header">
        <div>
          <h2 className="section-title">Svi korisnici</h2>
          <p className="admin-users-subtitle">Prijavljen kao {currentUserDisplayName}</p>
        </div>
        <span className="badge-count">{users.length} ukupno</span>
      </div>

      <div className="admin-users-layout">
        <div className="admin-users-list">
          {loading ? (
            <div className="admin-users-empty">Učitavanje korisnika...</div>
          ) : users.length === 0 ? (
            <div className="admin-users-empty">Nema dostupnih korisnika</div>
          ) : (
            users.map((item) => {
              const isSelected = selectedUser?.id === item.id;
              const displayName = [item.firstName, item.lastName].filter(Boolean).join(' ') || item.username || item.email || 'Korisnik';

              return (
                <button
                  key={item.id ?? displayName}
                  type="button"
                  className={`admin-user-row ${isSelected ? 'active' : ''}`}
                  onClick={() => setSelectedUser(item)}
                >
                  <div className="admin-user-avatar">{displayName.slice(0, 2).toUpperCase()}</div>
                  <div className="admin-user-meta">
                    <span className="admin-user-name">{displayName}</span>
                    <span className="admin-user-email">{item.email || item.username || '—'}</span>
                  </div>
                </button>
              );
            })
          )}
        </div>

        <div className="admin-users-editor">
          {selectedUser ? (
            <UserSettingsPanel title="Postavke korisnika" targetUser={selectedUser} />
          ) : (
            <div className="admin-users-empty">Odaberite korisnika za uređivanje</div>
          )}
        </div>
      </div>
    </section>
  );
}
