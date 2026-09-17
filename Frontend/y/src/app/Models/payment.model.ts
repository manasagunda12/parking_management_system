export interface Payment {
  paymentId: number;
  sessionId: number;
  amount: number;
  paymentMethod: string;
  paymentStatus: string;
  paymentDate: string;
}