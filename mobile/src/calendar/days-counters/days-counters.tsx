import React, { Component } from 'react';
import { View } from 'react-native';
import { styles, calendarScreenColors } from '../styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCounterSeparator } from './days-counter-separator';
import { DaysCountersModel } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { Employee } from '../../reducers/organization/employee.model';
import { calculateDaysCounters, CalendarActions } from '../../reducers/calendar/calendar.action';

interface DaysCountersProps {
    employee: Employee;
    daysCounters: DaysCountersModel;
}

interface DaysCountersDispatchProps {
    calculateDaysCounters: (vacationDaysLeft: number, hoursCredit: number) => void;
}

class DaysCountersImpl extends Component<DaysCountersProps & DaysCountersDispatchProps, {}> {

    public componentWillReceiveProps(nextProps: Readonly<DaysCountersProps>, nextContext: any) {
        const { employee } = nextProps;
        if (this.props.employee !== employee) {
            this.props.calculateDaysCounters(employee.vacationDaysLeft, employee.hoursCredit);
        }
    }

    public render() {
        const { allVacationDays, hoursCredit } = this.props.daysCounters;

        const vacationCounter = allVacationDays
            ? <DaysCounter  textValue={allVacationDays.toString()}
                            title={allVacationDays.title}
                            showIndicator={false} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCredit
            ? <DaysCounter  textValue={hoursCredit.toString()}
                            title={hoursCredit.title}
                            indicatorColor={hoursCredit.isAdditionalWork ? calendarScreenColors.red : calendarScreenColors.blue} />
            : <EmptyDaysCounter />;

        return <View style={styles.daysCounters}>
            { vacationCounter }
            <DaysCounterSeparator />
            { daysoffCounter }
        </View>;
    }
}

const mapStateToProps = (state: AppState): DaysCountersProps => ({
    employee: state.userInfo.employee,
    daysCounters: state.calendar.daysCounters
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>): DaysCountersDispatchProps => ({
    calculateDaysCounters: (vacationDaysLeft: number, hoursCredit: number) => dispatch(calculateDaysCounters(vacationDaysLeft, hoursCredit))
});

export const DaysCounters = connect(mapStateToProps, mapDispatchToProps)(DaysCountersImpl);