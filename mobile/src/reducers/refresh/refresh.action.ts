import { Action } from 'redux';

export interface Refresh extends Action  {
    type: 'REFRESH';
}

export const refresh = (): Refresh => ({ type: 'REFRESH' });
