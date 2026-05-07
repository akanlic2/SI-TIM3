export interface UserProfile {
  id?: string;
  firstName?: string;
  lastName?: string;
  username?: string;
  email?: string;
}

export interface UpdateUserProfileData {
  firstName?: string;
  lastName?: string;
  username?: string;
  email?: string;
  password?: string;
}
