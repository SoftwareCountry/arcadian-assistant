import { Moment } from 'moment';

export interface AuthenticatedState {
    isAuthenticated: true;
    refreshToken: string;
    jwtToken: string;
    validUntil: Moment;
}

export interface NotAuthenticatedState {
    isAuthenticated: false;
}

export type AuthenticationState = AuthenticatedState | NotAuthenticatedState;
