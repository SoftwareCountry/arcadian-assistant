import { User } from './user.model';

export interface LoadUser {
    type: 'LOAD-USER';
}

export const loadUser = (): LoadUser => ({ type: 'LOAD-USER' });

export interface LoadUserFinished {
    type: 'LOAD-USER-FINISHED';
    user: User;
}

export const loadUserFinished = (user: User): LoadUserFinished => ({ type: 'LOAD-USER-FINISHED', user });

export type UserActions = LoadUser | LoadUserFinished;