import { Reducer } from 'redux';
import { CalendarActions, cancelDialog } from './calendar.action';
import { editSickLeave, prolongSickLeave, confirmSickLeave, completeSickLeave, confirmProlongSickLeave } from './sick-leave.action';
import { DayModel, WeekModel, IntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { EventDialogModel, ClaimSickLeaveDialogModel, ProlongSickLeaveDialogModel, EditSickLeaveDialogModel } from './event-dialog/event-dialog.model';

export interface DialogActiveState {
    active: boolean;
    model: EventDialogModel<any>;
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

    const defaultDialogState: DialogActiveState = {
        active: false,
        model: null
    };

    return {
        weeks: weeks,
        selectedCalendarDay: todayModel,
        dialog: defaultDialogState,
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

            let dialogModel: ClaimSickLeaveDialogModel = null;
            if (state.dialog.active) {
                dialogModel = state.dialog.model.copy();
                dialogModel.endDate = action.day.date;
            }

            return {
                ...state,
                dialog: {
                    ...state.dialog,
                    model: dialogModel
                },
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
            const claimSickLeaveDialog = new ClaimSickLeaveDialogModel();

            claimSickLeaveDialog.startDate = state.selectedCalendarDay.date;

            return {
                ...state,
                dialog: {
                    active: true,
                    model: claimSickLeaveDialog
                }
            };
        case 'CONFIRM-CLAIM-SICK-LEAVE':
            return {
                ...state,
                dialog: {
                    active: false,
                    model: null
                }
            };
        case 'PROLONG-SICK-LEAVE':
            const prolongSickLeaveDialog = new ProlongSickLeaveDialogModel();

            return {
                ...state,
                dialog: {
                    ...state.dialog,
                    model: prolongSickLeaveDialog
                }
            };

        case 'EDIT-SICK-LEAVE':
            const editSickLeaveDialog = new EditSickLeaveDialogModel();

            return {
                ...state,
                dialog: {
                    ...state.dialog,
                    model: editSickLeaveDialog
                }
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