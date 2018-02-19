import React, { Component } from 'react';
import { View, LayoutChangeEvent } from 'react-native';
import { daysCountersStyles } from '../styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCountersModel } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';
import { SelectedDay } from './selected-day';
import { DayModel } from '../../reducers/calendar/calendar.model';

interface DaysCountersProps {
    daysCounters: DaysCountersModel;
}

class DaysCountersImpl extends Component<DaysCountersProps> {

    public render() {
        const { daysCounters: { allVacationDays, hoursCredit } } = this.props;

        const vacationCounter = allVacationDays
            ? <DaysCounter  textValue={allVacationDays.toString()}
                            title={allVacationDays.title} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCredit
            ? <DaysCounter  textValue={hoursCredit.toString()}
                            title={hoursCredit.title} />
            : <EmptyDaysCounter />;

        return (
            <View style={daysCountersStyles.container}>
                    { vacationCounter }
                    { daysoffCounter }
            </View>
        );
    }
}

const mapStateToProps = (state: AppState): DaysCountersProps => ({
    daysCounters: state.calendar.daysCounters
});

export const DaysCounters = connect(mapStateToProps)(DaysCountersImpl);