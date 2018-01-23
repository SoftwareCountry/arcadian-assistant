import React, { Component } from 'react';
import { View } from 'react-native';
import { styles, calendarScreenColors } from '../styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCounterSeparator } from './days-counter-separator';
import { DaysCountersModel } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch, connect } from 'react-redux';
import { CalendarActions, loadDaysCounters, LoadDaysCounters } from '../../reducers/calendar/calendar.action';

interface DaysCountersProps {
    daysCounters: DaysCountersModel;
}

interface DaysCountersDispatchProps {
    loadDaysCounters: () => void;
}

class DyasCountersImpl extends Component<DaysCountersProps & DaysCountersDispatchProps, {}> {
    public componentDidMount() {
        this.props.loadDaysCounters();
    }

    public render() {
        const { allVacationDays, daysOff } = this.props.daysCounters;

        const vacationCounter = allVacationDays
            ? <DaysCounter  value={allVacationDays.timestamp.toString()}
                            title={allVacationDays.title}
                            showIndicator={false} />
            : <EmptyDaysCounter />;

        const daysoffCounter = daysOff
            ? <DaysCounter  value={daysOff.timestamp.toString() + '½'}
                            title={daysOff.title}
                            indicatorColor={calendarScreenColors.return} />
            : <EmptyDaysCounter />;

        return <View style={styles.daysCounters}>
            { vacationCounter }
            <DaysCounterSeparator />
            { daysoffCounter }
        </View>;
    }
}

const mapStateToProps = (state: AppState): DaysCountersProps => ({
    daysCounters: state.calendar.daysCounters
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>): DaysCountersDispatchProps => ({
    loadDaysCounters: () => dispatch(loadDaysCounters())
});

export const DaysCounters = connect(mapStateToProps, mapDispatchToProps)(DyasCountersImpl);