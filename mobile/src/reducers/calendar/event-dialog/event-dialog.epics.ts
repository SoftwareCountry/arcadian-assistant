import { EventDialogActions, OpenEventDialog, closeEventDialog, stopProgress } from './event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import { calendarSelectionMode, CalendarSelectionModeType, selectIntervalsBySingleDaySelection, disableCalendarSelection, CalendarActions, disableSelectIntervalsBySingleDaySelection } from '../calendar.action';
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
                    return [calendarSelectionMode(CalendarSelectionModeType.SingleDay), disableCalendarSelection(true)];

                case EventDialogType.ProlongSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave);

                case EventDialogType.CancelSickLeave:
                    return disableCalendarSelection(true);

                case EventDialogType.RequestVacation:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);

                case EventDialogType.ConfirmStartDateVacation:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.vacation);

                case EventDialogType.EditVacation:
                    return [calendarSelectionMode(CalendarSelectionModeType.SingleDay), disableCalendarSelection(true)];

                case EventDialogType.ChangeVacationStartDate:
                    return [calendarSelectionMode(CalendarSelectionModeType.SingleDay), disableSelectIntervalsBySingleDaySelection(true)];

                case EventDialogType.ChangeVacationEndDate:
                    return [calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.vacation)];

                case EventDialogType.ProcessDayoff:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);

                case EventDialogType.ConfirmDayoffStartDate:
                    return disableCalendarSelection(true);

                case EventDialogType.EditDayoff:
                    return [disableCalendarSelection(true)];

                default:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);
            }

        }).flatMap((x: CalendarActions | CalendarActions[]) => 
            Array.isArray(x) ? Observable.from(x) : Observable.of(x)
        );

export const closeEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.ofType('CLOSE-EVENT-DIALOG')
        .flatMap(x => Observable.of<CalendarActions | EventDialogActions>(
            calendarSelectionMode(CalendarSelectionModeType.SingleDay),
            disableSelectIntervalsBySingleDaySelection(false),
            selectIntervalsBySingleDaySelection(),
            stopProgress()
        ));