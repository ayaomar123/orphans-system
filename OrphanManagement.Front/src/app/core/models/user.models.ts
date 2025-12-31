import { UserRole } from './enums';

export interface UserDto {
  id: string;
  fullName: string;
  email: string;
  role: string;
  phone?: string | null;
  isActive: boolean;
  lastLoginAt?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  password: string;
  role: UserRole;
  phone?: string | null;
}

export interface UpdateUserRequest {
  fullName: string;
  email: string;
  role: UserRole;
  phone?: string | null;
  isActive: boolean;
}
