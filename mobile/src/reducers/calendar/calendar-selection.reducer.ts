import { CalendarEventsState, SelectionSubState } from './calendar-events.reducer';
import { SelectCalendarDay } from './calendar.action';

export const singleDaySelectionReducer = (state: CalendarEventsState, action: SelectCalendarDay): SelectionSubState | null => {
    if (state.disableSelection) {
        return null;
    }

    return {
        selection: {
            ...state.selection,
            single: {
                day: action.day
            }
        }
    };
};

export const intervalSelectionReducer = (state: CalendarEventsState, action: SelectCalendarDay): SelectionSubState | null => {
    if (state.disableSelection) {
        return null;
    }

    if (state.selection.interval) {
        return {
            selection: {
                ...state.selection,
                interval: {
                    ...state.selection.interval,
                    endDay: action.day
                }
            }
        };
    }

    return null;
};
