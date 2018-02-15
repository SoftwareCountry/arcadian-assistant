import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';
import { CalendarModelBuilder, DayModel, WeekModel, CalendarPeriodsBuilder, PeriodsModel } from './calendar.model';
import moment from 'moment';

export interface CalendarEventsState {
    weeks: WeekModel[];
    selectedCalendarDay: DayModel;
    periods: PeriodsModel;
}

const createInitState = (): CalendarEventsState => {
    const builder = new CalendarModelBuilder();
    const today = moment();
    const weeks = builder.buildWeeks(today.month(), today.year());

    let todayModel: DayModel = null;
    for (let week of weeks) {
        todayModel = week.days.find(day => day.today);

        if (todayModel) {
            break;
        }
    }

    return {
        weeks: weeks,
        selectedCalendarDay: todayModel,
        periods: null
    };
};

const initState = createInitState();

export const calendarEventsReducer: Reducer<CalendarEventsState> = (state = initState, action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-CALENDAR-EVENTS-FINISHED':
            const builderPeriods = new CalendarPeriodsBuilder();
            const periods = builderPeriods.buildPeriods(action.calendarEvents);

            return {
                ...state,
                periods: periods
            };
        case 'SELECT-CALENDAR-DAY':
            return {
                ...state,
                selectedCalendarDay: action.day
            };
        case 'SELECT-CALENDAR-MONTH':
            const builder = new CalendarModelBuilder();
            const weeks = builder.buildWeeks(action.month, action.year);

            return {
                ...state,
                weeks: weeks
            };
        default:
            return state;
    }
};