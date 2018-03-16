import { EventDialogActions, OpenEventDialog, closeEventDialog } from './event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import { CalendarSelectionModeType, calendarSelectionMode } from './calendar.action';
import { CalendarEventsColor } from '../../calendar/styles';
import { EventDialogType } from './event-dialog/event-dialog-type.model';

export const openEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.filter(x => x.type === 'OPEN-EVENT-DIALOG')
        .map((x: OpenEventDialog) => {

            switch (x.dialogType) {
                case EventDialogType.ClaimSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay, CalendarEventsColor.sickLeave);

                case EventDialogType.ConfirmStartDateSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave);

                default:
                    return closeEventDialog();
            }

        });

export const closeEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.filter(x => x.type === 'CLOSE-EVENT-DIALOG')
        .map(x => calendarSelectionMode(CalendarSelectionModeType.SingleDay, null));