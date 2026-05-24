import { useState, useEffect, useCallback } from 'react';
import { fetchMyNotifications, markNotificationAsRead, markAllNotificationsAsRead } from '../api/notificationApi';
import type { Notification } from '../types';

export function useNotifications() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);

  const loadNotifications = useCallback(async () => {
    const data = await fetchMyNotifications();
    setNotifications(data);
    setUnreadCount(data.filter(n => !n.isRead).length);
  }, []);

  useEffect(() => {
    loadNotifications();
    
    // Polling every 15 seconds
    const interval = setInterval(() => {
      loadNotifications();
    }, 15000);

    return () => clearInterval(interval);
  }, [loadNotifications]);

  const markAsRead = async (id: string) => {
    try {
      await markNotificationAsRead(id);
      setNotifications(prev => 
        prev.map(n => n.notificationId === id ? { ...n, isRead: true } : n)
      );
      setUnreadCount(prev => Math.max(0, prev - 1));
    } catch (e) {
      console.error('Failed to mark as read', e);
    }
  };

  const markAllAsRead = async () => {
    try {
      await markAllNotificationsAsRead();
      setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
      setUnreadCount(0);
    } catch (e) {
      console.error('Failed to mark all as read', e);
    }
  };

  return {
    notifications,
    unreadCount,
    markAsRead,
    markAllAsRead,
    refresh: loadNotifications
  };
}
