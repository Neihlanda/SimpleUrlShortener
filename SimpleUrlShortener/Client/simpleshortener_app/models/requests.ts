export interface CreateShortUrlRequest {
  urlToProcess: string;
  uniqueUsage: boolean;
}
export interface LoginRequest {
  login: string;
  password: string;
}

export interface RegisterRequest {
  login: string;
  password: string;
  confirmPassword: string;
}
