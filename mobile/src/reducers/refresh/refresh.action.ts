export interface Refresh {
    type: 'REFRESH';
}

export const refresh = (): Refresh => ({ type: 'REFRESH' });
