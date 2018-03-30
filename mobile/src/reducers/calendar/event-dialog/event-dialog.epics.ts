import { EventDialogActions, OpenEventDialog, closeEventDialog } from './event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import { calendarSelectionMode, CalendarSelectionModeType, selectIntervalsBySingleDaySelection, disableCalendarSelection, CalendarActions } from '../calendar.action';
import { CalendarEventsColor } from '../../../calendar/styles';
import { EventDialogType } from './event-dialog-type.model';
import { Observable } from 'rxjs';

export const openEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.ofType('OPEN-EVENT-DIALOG')
        .map((x: OpenEventDialog) => {

            switch (x.dialogType) {
                case EventDialogType.ClaimSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);

                case EventDialogType.ConfirmStartDateSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave);

                case EventDialogType.EditSickLeave:
                    return [
                        calendarSelectionMode(CalendarSelectionModeType.SingleDay),
                        disableCalendarSelection(true)
                    ];

                case EventDialogType.ProlongSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave);

                case EventDialogType.RequestVacaltion:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);

                case EventDialogType.ConfirmStartDateVacation:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.vacation);

                default:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);
            }

        }).flatMap((x: CalendarActions | CalendarActions[]) => 
            Array.isArray(x) ? Observable.from(x) : Observable.of(x)
        );

export const closeEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.ofType('CLOSE-EVENT-DIALOG')
        .map(x => calendarSelectionMode(CalendarSelectionModeType.SingleDay));