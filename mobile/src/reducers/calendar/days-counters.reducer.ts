import { VacationDaysCounter, HoursCreditCounter } from './days-counters.model';
import { UserActions } from '../user/user.action';
import { Reducer } from 'redux';
import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';

export interface DaysCountersState {
    allVacationDays: VacationDaysCounter;
    hoursCredit: HoursCreditCounter;
}

const initState: DaysCountersState = {
    allVacationDays: null,
    hoursCredit: null
};

export const daysCountersReducer: Reducer<DaysCountersState> = (state = initState, action: UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-EMPLOYEE-FINISHED':

            const allVacationDaysCounter = new VacationDaysCounter(action.employee.vacationDaysLeft);

            const daysConverter = new ConvertHoursCreditToDays();
            const calculatedDays = daysConverter.convert(action.employee.hoursCredit);

            const hoursCreditCounter = new HoursCreditCounter(action.employee.hoursCredit, calculatedDays.days, calculatedDays.rest);

            return {
                allVacationDays: allVacationDaysCounter,
                hoursCredit: hoursCreditCounter,
            };
        default:
            return state;
    }
};