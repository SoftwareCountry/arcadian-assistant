export interface PressLogIn {
    type: 'PRESS-LOG-IN';
}

export const pressLogIn = (): PressLogIn => ({ type: 'PRESS-LOG-IN'});

export interface PressLogOut {
    type: 'PRESS-LOG-OUT';
}

export const pressLogOut = (): PressLogOut => ({ type: 'PRESS-LOG-OUT' });

export type AuthActions = PressLogIn | PressLogOut;
