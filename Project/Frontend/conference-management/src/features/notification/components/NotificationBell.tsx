import React, { useState, useRef, useEffect } from 'react';
import { useNotifications } from '../hooks/useNotifications';
import './NotificationBell.css';

const formatTimeAgo = (dateString: string) => {
  const date = new Date(dateString);
  const now = new Date();
  const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

  if (diffInSeconds < 60) return 'Prije par sekundi';
  if (diffInSeconds < 3600) return `Prije ${Math.floor(diffInSeconds / 60)} min`;
  if (diffInSeconds < 86400) return `Prije ${Math.floor(diffInSeconds / 3600)} h`;
  return `Prije ${Math.floor(diffInSeconds / 86400)} dana`;
};

export const NotificationBell: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const { notifications, unreadCount, markAsRead, markAllAsRead } = useNotifications();
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleNotificationClick = (notificationId: string, isRead: boolean, type: string) => {
    if (!isRead) {
      markAsRead(notificationId);
    }
    
    // Redirect based on type
    if (type.includes('Conference')) {
      window.history.pushState({}, '', '/conferences');
      window.dispatchEvent(new PopStateEvent('popstate'));
    } else if (type.includes('Session') || type.includes('Speaker')) {
      window.history.pushState({}, '', '/dashboard');
      window.dispatchEvent(new PopStateEvent('popstate'));
    }
    setIsOpen(false);
  };

  return (
    <div className="notification-bell-container" ref={containerRef}>
      <button 
        className="notification-btn" 
        onClick={() => setIsOpen(!isOpen)}
        title="Notifikacije"
      >
        🔔
        {unreadCount > 0 && (
          <span className="notification-badge">{unreadCount > 9 ? '9+' : unreadCount}</span>
        )}
      </button>

      {isOpen && (
        <div className="notification-dropdown">
          <div className="notification-header">
            <span className="notification-title">Notifikacije</span>
            {unreadCount > 0 && (
              <button className="mark-all-read-btn" onClick={markAllAsRead}>
                Označi sve kao pročitano
              </button>
            )}
          </div>

          <div className="notification-list">
            {notifications.length === 0 ? (
              <div className="notification-empty">Nemate novih notifikacija.</div>
            ) : (
              notifications.map((notif) => (
                <div 
                  key={notif.notificationId} 
                  className={`notification-item ${notif.isRead ? '' : 'unread'}`}
                  onClick={() => handleNotificationClick(notif.notificationId, notif.isRead, notif.notificationType)}
                >
                  <div className="notification-item-title">{notif.title}</div>
                  <div className="notification-item-content">{notif.content}</div>
                  <div className="notification-item-date">{formatTimeAgo(notif.sentDate)}</div>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
};
