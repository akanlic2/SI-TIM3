export interface Room {
  roomId: string;
  name: string;
  location: string;
  capacity: number;
  description: string;
}

export interface CreateRoomData {
  name: string;
  location: string;
  capacity: number;
  description: string;
}
