import { EventDialogType } from './event-dialog-type.model';
import { SickLeaveActions } from '../sick-leave.action';
import { ChosenTypeDayoff } from '../dayoff.action';

export interface OpenEventDialog {
    type: 'OPEN-EVENT-DIALOG';
    dialogType: EventDialogType;
}

export const openEventDialog = (dialogType: EventDialogType): OpenEventDialog => ({ type: 'OPEN-EVENT-DIALOG', dialogType });

export interface CloseEventDialog {
    type: 'CLOSE-EVENT-DIALOG';
}

export const closeEventDialog = (): CloseEventDialog => ({ type: 'CLOSE-EVENT-DIALOG' });

export interface StartEventDialogProgress {
    type: 'START-EVENT-DIALOG-PROGRESS';
}

export const startProgress = (): StartEventDialogProgress => ({ type: 'START-EVENT-DIALOG-PROGRESS' });

export interface StopEventDialogProgress {
    type: 'STOP-EVENT-DIALOG-PROGRESS';
}

export const stopEventDialogProgress = (): StopEventDialogProgress => ({ type: 'STOP-EVENT-DIALOG-PROGRESS' });

export type EventDialogActions = OpenEventDialog | CloseEventDialog | ChosenTypeDayoff | StartEventDialogProgress | StopEventDialogProgress;