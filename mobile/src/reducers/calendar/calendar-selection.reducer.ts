import { CalendarEventsState, IntervalsSubState, SelectionSubState } from './calendar-events.reducer';
import { SelectCalendarDay, CalendarSelectionModeType } from './calendar.action';
import { CalendarEvents, CalendarEventsType, DatesInterval, CalendarEventStatus } from './calendar-events.model';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';

export const singleDaySelectionReducer = (state: CalendarEventsState, action: SelectCalendarDay): SelectionSubState | null => {
    if (state.selection.mode === CalendarSelectionModeType.SingleDay) {
        return {
            selection: {
                ...state.selection,
                startDay: action.day
            }
        };
    }

    return null;
};

export const intervalSelectionReducer = (state: CalendarEventsState, action: SelectCalendarDay): SelectionSubState | null => {
    if (state.selection.mode === CalendarSelectionModeType.Interval) {

        return {
            selection: {
                ...state.selection,
                endDay: action.day,
            }
        };
    }

    return null;
};