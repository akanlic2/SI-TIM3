import { useAuth } from '../../../auth/AuthProvider';
import { useUserProfile } from '../hooks/useUserProfile';
import type { UserProfile } from '../types';
import './UserSettingsPanel.css';

interface UserSettingsPanelProps {
  title?: string;
  targetUser?: UserProfile | null;
}

function isAdminRole(roles: string[] | undefined): boolean {
  return roles?.some((role) => role.toLowerCase().includes('admin')) ?? false;
}

export function UserSettingsPanel({ title = 'Postavke naloga', targetUser = null }: UserSettingsPanelProps) {
  const { user } = useAuth();
  const canEditRole =
    isAdminRole(user?.role ? [user.role] : []) &&
    !!targetUser?.id &&
    targetUser.id !== user?.userId;
  const {
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
  } = useUserProfile({ user, targetUser, allowRoleUpdate: canEditRole });

  return (
    <>
      <header className="settings-header">
        <h1>{title}</h1>
        <div className="settings-actions">
          {!editing ? (
            <button className="btn-primary" onClick={() => setEditing(true)}>Uredi</button>
          ) : (
            <>
              <button className="btn-primary" onClick={saveProfile} disabled={loading}>Sačuvaj</button>
              <button className="btn-secondary" onClick={cancelEditing}>Odustani</button>
            </>
          )}
        </div>
      </header>

      <section className="settings-card">
        {loading && <div className="loading-inline">Učitavanje...</div>}

        <div className="field-row">
          <label>Ime</label>
          <input
            value={profile.firstName ?? ''}
            onChange={(e) => setProfile({ ...profile, firstName: e.target.value })}
            disabled={!editing}
          />
        </div>

        <div className="field-row">
          <label>Prezime</label>
          <input
            value={profile.lastName ?? ''}
            onChange={(e) => setProfile({ ...profile, lastName: e.target.value })}
            disabled={!editing}
          />
        </div>

        <div className="field-row">
          <label>Korisničko ime</label>
          <input
            value={profile.username ?? ''}
            onChange={(e) => setProfile({ ...profile, username: e.target.value })}
            disabled={!editing}
          />
        </div>

        <div className="field-row">
          <label>Email</label>
          <input
            value={profile.email ?? ''}
            onChange={(e) => setProfile({ ...profile, email: e.target.value })}
            disabled={!editing}
          />
        </div>

        {canEditRole && (
          <div className="field-row">
            <label>Uloga</label>
            <select
              value={profile.role ?? ''}
              onChange={(e) => setProfile({ ...profile, role: e.target.value })}
              disabled={!editing}
            >
              <option value="">Odaberite ulogu</option>
              <option value="ucesnik">Učesnik</option>
              <option value="predavac">Predavač</option>
              <option value="organizator">Organizator</option>
              <option value="admin">Admin</option>
            </select>
          </div>
        )}

        <div className="field-row">
          <label>Lozinka</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={!editing}
            placeholder={editing ? 'Unesite novu lozinku (ostavite prazno da se ne mijenja)' : '********'}
          />
        </div>

        {message && <div className="settings-message">{message}</div>}
      </section>
    </>
  );
}
