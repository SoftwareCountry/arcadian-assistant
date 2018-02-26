import { Reducer } from 'redux';
import { CalendarActions, cancelDialog } from './calendar.action';
import { editSickLeave, prolongueSickLeave, confirmSickLeave, completeSickLeave, confirmProlongueSickLeave } from './sick-leave.action';
import { DayModel, WeekModel, IntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { EventDialogProps } from '../../calendar/event-dialog/event-dialog';

interface DialogActiveState extends EventDialogProps {
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
            const claimDialog: DialogActiveState = {
                active: true,
                title: 'Select date to Complete your Sick Leave',
                text: 'Your sick leave has started on March 5, 2018 and will be complete on March 14, 2018.',
                icon: 'sick_leave',
                cancel: {
                    label: 'Back',
                    action: cancelDialog
                },
                accept: {
                    label: 'Confirm',
                    action: confirmSickLeave // TODO: add epic to confirmSickLeave
                }
            };

            return {
                ...state,
                dialog: claimDialog
            };
        case 'PROLONGUE-SICK-LEAVE':
            //TODO: move props to button dispatcher
            const prolongueDialog: DialogActiveState = {
                active: true,
                title: 'Select date to Prolongue your sick leave',
                text: 'Your sick leave has started on MM D, YYYY and will be prolongued to MM D, YYYY.',
                icon: 'sick_leave',
                close: cancelDialog,
                cancel: {
                    label: 'Back',
                    action: editSickLeave
                },
                accept: {
                    label: 'Confirm',
                    action: confirmProlongueSickLeave // TODO: add epic to confirmProlongueSickLeave
                }
            };

            return {
                ...state,
                dialog: prolongueDialog
            };

        case 'EDIT-SICK-LEAVE':
            //TODO: move props to button dispatcher
            const dialog: DialogActiveState = {
                active: true,
                title: 'Hey! Hope you feel better',
                text: 'Your sick leave has started on MM D, YYYY and still not completed.',
                icon: 'sick_leave',
                close: cancelDialog,
                cancel: {
                    label: 'Prolongue',
                    action: prolongueSickLeave
                },
                accept: {
                    label: 'Complete',
                    action: completeSickLeave // TODO: add epic to completeSickLeave
                }
            };

            return {
                ...state,
                dialog: dialog
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