import { Reducer } from 'redux';
import { combineEpics } from 'redux-observable';
import { pressLogInEpic$, pressLogOutEpic$, listenerAuthStateEpic$ } from './auth.epics';
import { AuthActions } from './auth.action';
import { AuthenticationState } from '../../auth/authentication-state';

export const authEpics$ = combineEpics(
    pressLogInEpic$ as any,
    pressLogOutEpic$ as any,
    listenerAuthStateEpic$ as any
);

export interface AuthState {
    isAuthenticated: boolean | null;
}

const initState: AuthState = {
   isAuthenticated: null
};

export const authReducer = (state: AuthState = initState, action: AuthActions): AuthState => {
    switch (action.type) {
        case 'PRESS-LOG-IN':
            return {
                ...state,
                isAuthenticated: true
            };
        case 'PRESS-LOG-OUT':
            return {
                ...state,
                isAuthenticated: false
            };
        default:
            return state;
   }
};
