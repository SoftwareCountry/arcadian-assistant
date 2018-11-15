export interface AuthenticatedState {
    isAuthenticated: true;
    refreshToken: string;
    jwtToken: string;
    lastUpdated: Date;
}

export interface NotAuthenticatedState {
    isAuthenticated: false;
}

export type AuthenticationState = AuthenticatedState | NotAuthenticatedState;
