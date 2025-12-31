export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  Token: string;
  ExpiresAt: string;
  UserId: string;
  FullName: string;
  Email: string;
  Role: string;
}
