import React, { Component } from 'react';
import { View, Text, LayoutChangeEvent, ViewStyle, StyleSheet } from 'react-native';
import { daysCountersStyles } from '../styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCounterSeparator } from './days-counter-separator';
import { DaysCountersModel, TodayCounter } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { Employee } from '../../reducers/organization/employee.model';
import { calculateDaysCounters, CalendarActions, calculateTodayCounter } from '../../reducers/calendar/calendar.action';
import { DaysCounterToday } from './days-counter-today';
import { DaysCounterTriangle } from './days-counter-triangle';

interface DaysCountersProps {
    employee: Employee;
    daysCounters: DaysCountersModel;
    today: TodayCounter;
}

interface DaysCountersDispatchProps {
    calculateDaysCounters: (vacationDaysLeft: number, hoursCredit: number) => void;
    calculateTodayCounter: () => void;
}

interface DaysCounterState {
    daysCountersWidth: number;
}

class DaysCountersImpl extends Component<DaysCountersProps & DaysCountersDispatchProps, DaysCounterState> {
    constructor(props: DaysCountersProps & DaysCountersDispatchProps) {
        super(props);
        this.state = {
            daysCountersWidth: 0
        };
    }

    public componentDidMount() {
        this.props.calculateTodayCounter();
    }

    public componentWillReceiveProps(nextProps: Readonly<DaysCountersProps>, nextContext: any) {
        const { employee } = nextProps;
        if (this.props.employee !== employee) {
            this.props.calculateDaysCounters(employee.vacationDaysLeft, employee.hoursCredit);
        }
    }

    public render() {
        const { daysCounters: { allVacationDays, hoursCredit }, today } = this.props;

        const vacationCounter = allVacationDays
            ? <DaysCounter  textValue={allVacationDays.toString()}
                            title={allVacationDays.title} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCredit
            ? <DaysCounter  textValue={hoursCredit.toString()}
                            title={hoursCredit.title} />
            : <EmptyDaysCounter />;

        return <View style={daysCountersStyles.container} onLayout={this.onDaysCountersLayout}>
                <DaysCounterTriangle containerWidth={this.state.daysCountersWidth} />

                <View style={daysCountersStyles.counters}>
                    <DaysCounterToday {...today} />
                    { vacationCounter }
                    <DaysCounterSeparator />
                    { daysoffCounter }
                </View>
        </View>;
    }

    private onDaysCountersLayout = (e: LayoutChangeEvent) => {
        this.setState({
            daysCountersWidth: e.nativeEvent.layout.width
        });
    }
}

const mapStateToProps = (state: AppState): DaysCountersProps => ({
    employee: state.userInfo.employee,
    daysCounters: state.calendar.daysCounters,
    today: state.calendar.daysCounters.today
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>): DaysCountersDispatchProps => ({
    calculateDaysCounters: (vacationDaysLeft: number, hoursCredit: number) => dispatch(calculateDaysCounters(vacationDaysLeft, hoursCredit)),
    calculateTodayCounter: () => dispatch(calculateTodayCounter())
});

export const DaysCounters = connect(mapStateToProps, mapDispatchToProps)(DaysCountersImpl);