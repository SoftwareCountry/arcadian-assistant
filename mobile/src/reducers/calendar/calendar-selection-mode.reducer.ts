import { CalendarEventsState, IntervalsSubState, SelectionSubState, DisableDaysCalendarDaysBeforeSubState } from './calendar-events.reducer';
import { CalendarSelectionMode, CalendarSelectionModeType } from './calendar.action';

interface CalendarSelectionModeSubState extends SelectionSubState, DisableDaysCalendarDaysBeforeSubState { }

export const calendarSelectionModeReducer = (state: CalendarEventsState, action: CalendarSelectionMode): CalendarSelectionModeSubState | null => {
    if (action.selectionMode === CalendarSelectionModeType.SingleDay) {
        return {
            selection: {
                mode: action.selectionMode,
                startDay: state.selectedCalendarDay,
                endDay: null,
                color: action.color
            },
            disableCalendarDaysBefore: null
        };
    }

    if (action.selectionMode === CalendarSelectionModeType.Interval) {
        return  {
            selection: {
                mode: action.selectionMode,
                startDay: state.selectedCalendarDay,
                endDay: null,
                color: action.color
            },
            disableCalendarDaysBefore: state.selectedCalendarDay
        };
    }

    return null;
};