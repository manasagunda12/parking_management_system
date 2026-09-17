export interface ParkingSession {
  sessionId: number;
  vehicleId: number;
  parkingSpaceId: number;
  entryTime: string;
  exitTime?: string;
  durationInHours?: number;
  amount?: number;
  status: string;
}