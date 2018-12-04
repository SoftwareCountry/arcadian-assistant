import { CalendarEventsState, IntervalsSubState, SelectionSubState, DisableCalendarDaysBeforeSubState } from './calendar-events.reducer';
import { CalendarSelectionMode, CalendarSelectionModeType } from './calendar.action';

interface CalendarSelectionModeSubState extends SelectionSubState, DisableCalendarDaysBeforeSubState { }

export const calendarSelectionModeReducer = (state: CalendarEventsState, action: CalendarSelectionMode): CalendarSelectionModeSubState | null => {
    if (action.selectionMode === CalendarSelectionModeType.SingleDay) {
        return {
            selection: {
                ...state.selection,
                interval: undefined
            },
            disableCalendarDaysBefore: undefined
        };
    }

    if (action.selectionMode === CalendarSelectionModeType.Interval) {
        return  {
            selection: {
                ...state.selection,
                interval: {
                    startDay: state.selection.single.day,
                    endDay: undefined,
                    color: action.color
                }
            },
            disableCalendarDaysBefore: state.selection.single.day
        };
    }

    return null;
};
