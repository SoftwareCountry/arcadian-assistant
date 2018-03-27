import { EventDialogActions, OpenEventDialog, closeEventDialog } from './event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import { calendarSelectionMode, CalendarSelectionModeType, intervalsBySingleDaySelection, disableCalendarSelection } from '../calendar.action';
import { CalendarEventsColor } from '../../../calendar/styles';
import { EventDialogType } from './event-dialog-type.model';

export const openEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.filter(x => x.type === 'OPEN-EVENT-DIALOG')
        .map((x: OpenEventDialog) => {

            switch (x.dialogType) {
                case EventDialogType.ClaimSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);

                case EventDialogType.ConfirmStartDateSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave);

                case EventDialogType.EditSickLeave:
                    return disableCalendarSelection(true);

                default:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);
            }

        });

export const closeEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.ofType('CLOSE-EVENT-DIALOG')
        .map(x => calendarSelectionMode(CalendarSelectionModeType.SingleDay));