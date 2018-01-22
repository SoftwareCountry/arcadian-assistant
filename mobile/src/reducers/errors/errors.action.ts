import { Action } from 'redux';

export interface LoadFailedError extends Action {
    type: 'LOAD-FAILED-ERROR';
    errorMessage: string;
}

export const loadFailedError = (errorMessage: string): LoadFailedError => ({ type: 'LOAD-FAILED-ERROR', errorMessage: errorMessage || 'something wrong' });