// models/dashboard.model.ts

export interface Dashboard {
  totalParkingLots: number;
  totalParkingSpaces: number;
  availableSpaces: number;
  occupiedSpaces: number;
  activeSessions: number;
  totalRevenue: number;
}