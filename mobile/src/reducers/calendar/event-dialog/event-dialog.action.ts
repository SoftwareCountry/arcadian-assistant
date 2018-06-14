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

export interface StartProgress {
    type: 'START-PROGRESS';
}

export const startProgress = (): StartProgress => ({ type: 'START-PROGRESS' });

export interface StopProgress {
    type: 'STOP-PROGRESS';
}

export const stopProgress = (): StopProgress => ({ type: 'STOP-PROGRESS' });

export type EventDialogActions = OpenEventDialog | CloseEventDialog | ChosenTypeDayoff | StartProgress | StopProgress;