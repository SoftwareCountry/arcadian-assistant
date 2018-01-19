import { Action } from 'redux';

export interface ErrorLoadFailed extends Action {
    type: 'ERROR-LOAD-FAILED';
    errorMessage: string;
}