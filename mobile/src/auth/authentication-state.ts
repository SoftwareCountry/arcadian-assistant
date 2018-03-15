export interface AuthenticatedState {
    isAuthenticated: true;
    refreshToken: string;
    jwtToken: string;
}

export interface NotAuthenticatedState {
    isAuthenticated: false;
}

export type AuthenticationState = AuthenticatedState | NotAuthenticatedState;