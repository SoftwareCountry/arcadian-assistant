import { Action } from 'redux';

export interface ErrorLoadFailed extends Action {
    type: 'ERROR-LOAD-FAILED';
    errorMessage: string;
}

export const errorLoadFailed = (errorMessage: string): ErrorLoadFailed => ({ type: 'ERROR-LOAD-FAILED', errorMessage: errorMessage || 'something wrong' });