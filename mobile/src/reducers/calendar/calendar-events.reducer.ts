import { Reducer } from 'redux';
import { CalendarActions, cancelDialog } from './calendar.action';
import { editSickLeave, prolongSickLeave, confirmSickLeave, completeSickLeave, confirmProlongSickLeave } from './sick-leave.action';
import { DayModel, WeekModel, IntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { EventDialogProps } from '../../calendar/event-dialog/event-dialog';
import { claimSickLeaveDialogConfig, prolongSickLeaveDialogConfig, editSickLeaveDialogConfig } from './sick-leave-dialog-config';

export interface DialogActiveState extends EventDialogProps {
    active: boolean;
}

export interface CalendarEventsState {
    weeks: WeekModel[];
    selectedCalendarDay: DayModel;
    intervals: IntervalsModel;
    dialog: DialogActiveState;
}

const createInitState = (): CalendarEventsState => {
    const builder = new CalendarWeeksBuilder();
    const today = moment();
    const weeks = builder.buildWeeks(today.month(), today.year());

    let todayModel: DayModel = null;
    for (let week of weeks) {
        todayModel = week.days.find(day => day.today);

        if (todayModel) {
            break;
        }
    }

    const dialog = { active: false} as DialogActiveState;

    return {
        weeks: weeks,
        selectedCalendarDay: todayModel,
        dialog: dialog,
        intervals: null
    };
};

const initState = createInitState();

export const calendarEventsReducer: Reducer<CalendarEventsState> = (state = initState, action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-CALENDAR-EVENTS-FINISHED':
            const builderIntervals = new CalendarIntervalsBuilder();
            const intervals = builderIntervals.buildIntervals(action.calendarEvents);

            return {
                ...state,
                intervals: intervals
            };
        case 'SELECT-CALENDAR-DAY':
            return {
                ...state,
                selectedCalendarDay: action.day
            };
        case 'SELECT-CALENDAR-MONTH':
            const builder = new CalendarWeeksBuilder();
            const weeks = builder.buildWeeks(action.month, action.year);

            return {
                ...state,
                weeks: weeks
            };
        case 'CLAIM-SICK-LEAVE':
            const claimSickLeaveConfig = claimSickLeaveDialogConfig();

            return {
                ...state,
                dialog: claimSickLeaveConfig
            };
        case 'PROLONG-SICK-LEAVE':
            const prolongSickLeaveConfig = prolongSickLeaveDialogConfig();

            return {
                ...state,
                dialog: prolongSickLeaveConfig
            };

        case 'EDIT-SICK-LEAVE':
            const editSickLeaveConfig = editSickLeaveDialogConfig();

            return {
                ...state,
                dialog: editSickLeaveConfig
            };

        case 'CANCEL-CALENDAR-DIALOG':
            return {
                ...state,
                dialog: { active: false } as DialogActiveState
            };

        default:
            return state;
    }
};