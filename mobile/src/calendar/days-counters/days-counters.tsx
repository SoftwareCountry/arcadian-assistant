import React, { Component } from 'react';
import { View, LayoutChangeEvent } from 'react-native';
import { daysCountersStyles } from '../styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCounterSeparator } from './days-counter-separator';
import { DaysCountersModel } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';
import { SelectedDay } from './selected-day';
import { Triangle } from './triangle';
import { DayModel } from '../../reducers/calendar/calendar.model';

interface DaysCountersProps {
    daysCounters: DaysCountersModel;
    selectedCalendarDay: DayModel;
}

interface DaysCounterState {
    daysCountersWidth: number;
}

class DaysCountersImpl extends Component<DaysCountersProps, DaysCounterState> {
    constructor(props: DaysCountersProps) {
        super(props);
        this.state = {
            daysCountersWidth: 0
        };
    }

    public render() {
        const { daysCounters: { allVacationDays, hoursCredit }, selectedCalendarDay } = this.props;

        const vacationCounter = allVacationDays
            ? <DaysCounter  textValue={allVacationDays.toString()}
                            title={allVacationDays.title} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCredit
            ? <DaysCounter  textValue={hoursCredit.toString()}
                            title={hoursCredit.title} />
            : <EmptyDaysCounter />;


        const selectedDay = selectedCalendarDay 
            ? selectedCalendarDay.date
            : null;

        return (
            <View style={daysCountersStyles.container} onLayout={this.onDaysCountersLayout}>
                <Triangle containerWidth={this.state.daysCountersWidth} />
                <SelectedDay day={selectedDay} />
                <View style={daysCountersStyles.counters}>
                    { vacationCounter }
                    <DaysCounterSeparator />
                    { daysoffCounter }
                </View>
            </View>
        );
    }

    private onDaysCountersLayout = (e: LayoutChangeEvent) => {
        this.setState({
            daysCountersWidth: e.nativeEvent.layout.width
        });
    }
}

const mapStateToProps = (state: AppState): DaysCountersProps => ({
    daysCounters: state.calendar.daysCounters,
    selectedCalendarDay: state.calendar.calendarEvents.selectedCalendarDay
});

export const DaysCounters = connect(mapStateToProps)(DaysCountersImpl);