export interface Notification {
  notificationId: string;
  userId: string;
  title: string;
  content: string;
  notificationType: string;
  sentDate: string;
  isRead: boolean;
}
