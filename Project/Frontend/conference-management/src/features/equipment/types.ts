export interface Equipment {
  equipmentId: string;
  sessionId?: string | null;
  name: string;
  type: string;
  quantity: number;
  availableQuantity: number;
  isAvailable: boolean;
  availabilityStatus: string;
  createdAt: string;
}

export interface CreateEquipmentData {
  name: string;
  type: string;
  quantity: number;
}

export interface AssignEquipmentData {
  equipmentId: string;
  quantity: number;
}
