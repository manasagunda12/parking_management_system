export interface User {
  userId: number;
  username: string;
  email: string;
  password: string;
  phoneNumber: string;
  role: string;
  isActive: boolean;
  createdAt?: string;
}