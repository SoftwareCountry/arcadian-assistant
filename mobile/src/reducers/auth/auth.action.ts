export interface StartLoginProcess {
    type: 'START-LOGIN-PROCESS';
}

export const startLoginProcess = (): StartLoginProcess => ({ type: 'START-LOGIN-PROCESS'});

export interface StartLogoutProcess {
    type: 'START-LOGOUT-PROCESS';
}

export const startLogoutProcess = (): StartLogoutProcess => ({ type: 'START-LOGOUT-PROCESS' });

export interface FinishLoginProcess {
    type: 'FINISH-LOGIN-PROCESS';
}

export const finishLoginProcess = (): FinishLoginProcess => ({ type: 'FINISH-LOGIN-PROCESS' });

export interface FinishLogoutProcess {
    type: 'FINISH-LOGOUT-PROCESS';
}

export const finishLogoutProcess = (): FinishLogoutProcess => ({ type: 'FINISH-LOGOUT-PROCESS' });

export type AuthActions = StartLoginProcess | StartLogoutProcess | FinishLoginProcess | FinishLogoutProcess;
