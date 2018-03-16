import { SickLeaveActions } from './sick-leave.action';
import { EventDialogType } from './event-dialog/event-dialog-type.model';

export interface OpenEventDialog {
    type: 'OPEN-EVENT-DIALOG';
    dialogType: EventDialogType;
}

export const openEventDialog = (dialogType: EventDialogType): OpenEventDialog => ({ type: 'OPEN-EVENT-DIALOG', dialogType });

export interface CloseEventDialog {
    type: 'CLOSE-EVENT-DIALOG';
}

export const closeEventDialog = (): CloseEventDialog => ({ type: 'CLOSE-EVENT-DIALOG' });

export type EventDialogActions = OpenEventDialog | CloseEventDialog | SickLeaveActions;