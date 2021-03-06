import {
    EventDialogActions,
    OpenEventDialog,
    startEventDialogProgress,
    stopEventDialogProgress
} from './event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import {
    CalendarActions,
    calendarSelectionMode,
    CalendarSelectionModeType,
    disableCalendarSelection,
    disableSelectIntervalsBySingleDaySelection,
    selectIntervalsBySingleDaySelection
} from '../calendar.action';
import { CalendarEventsColor } from '../../../calendar/styles';
import { EventDialogType } from './event-dialog-type.model';
import { from, of } from 'rxjs';
import { VacationActions } from '../vacation.action';
import { SickLeaveActions } from '../sick-leave.action';
import { DayOffActions } from '../dayoff.action';
import { flatMap, map } from 'rxjs/operators';

export const openEventDialogEpic$ = (action$: ActionsObservable<OpenEventDialog>) =>
    action$.ofType('OPEN-EVENT-DIALOG').pipe(
        map((x: OpenEventDialog) => {

            switch (x.dialogType) {
                case EventDialogType.ClaimSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);

                case EventDialogType.ConfirmStartDateSickLeave:
                    return calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave);

                case EventDialogType.EditSickLeave:
                    return [calendarSelectionMode(CalendarSelectionModeType.SingleDay), disableCalendarSelection(true), disableSelectIntervalsBySingleDaySelection(true)];

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

                case EventDialogType.ProcessDayOff:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);

                case EventDialogType.ConfirmDayOffStartDate:
                    return disableCalendarSelection(true);

                case EventDialogType.EditDayOff:
                    return [disableCalendarSelection(true)];

                default:
                    return calendarSelectionMode(CalendarSelectionModeType.SingleDay);
            }

        }),
        flatMap((x: CalendarActions | CalendarActions[]) =>
            Array.isArray(x) ? from(x) : of(x)
        ));

export const closeEventDialogEpic$ = (action$: ActionsObservable<EventDialogActions>) =>
    action$.ofType('CLOSE-EVENT-DIALOG').pipe(
        flatMap(x => of<CalendarActions | EventDialogActions>(
            calendarSelectionMode(CalendarSelectionModeType.SingleDay),
            disableSelectIntervalsBySingleDaySelection(false),
            selectIntervalsBySingleDaySelection(),
            stopEventDialogProgress()
        )));

export const startEventDialogProgressEpic$ = (action$: ActionsObservable<VacationActions | SickLeaveActions | DayOffActions>) =>
    action$.ofType(
        'CANCEL-SICK-LEAVE',
        'CANCEL-VACATION',
        'CANCEL-DAY-OFF',
        'CONFIRM-VACATION-CHANGE',
        'CONFIRM-PROCESS-DAY-OFF',
        'CONFIRM-CLAIM-SICK-LEAVE',
        'CONFIRM-VACATION',
        'CONFIRM-PROLONG-SICK-LEAVE',
        'COMPLETE-SICK-LEAVE'
    ).pipe(
        map(x => startEventDialogProgress()));
