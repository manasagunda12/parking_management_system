export interface Reservation {
  reservationId: number;
  vehicleId: number;
  parkingSpaceId: number;
  reservationStartTime: string;
  reservationEndTime: string;
  status: string;
}