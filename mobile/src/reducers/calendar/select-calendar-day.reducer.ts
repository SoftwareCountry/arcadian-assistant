import { VacationDaysCounter, HoursCreditCounter } from './days-counters.model';
import { UserActions } from '../user/user.action';
import { Reducer } from 'redux';
import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';
import { DayModel } from '../../calendar/calendar-page';
import { CalendarActions } from './calendar.action';

export const selectCalendarDayReducer: Reducer<DayModel> = (state = null, action: CalendarActions) => {
    switch (action.type) {
        case 'SELECT-CALENDAR-DAY':
            return action.day;
        default:
            return state;
    }
};