import React, { Component } from 'react';
import { View } from 'react-native';
import { styles, calendarScreenColors } from '../styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCounterSeparator } from './days-counter-separator';
import { DaysCountersModel } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';

interface DaysCountersProps {
    daysCounters: DaysCountersModel;
}

class DyasCountersImpl extends Component<DaysCountersProps, {}> {
    public render() {
        const { allVacationDays, hoursCredit } = this.props.daysCounters;

        const vacationCounter = allVacationDays
            ? <DaysCounter  value={allVacationDays.toString()}
                            title={allVacationDays.title}
                            showIndicator={false} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCredit
            ? <DaysCounter  value={hoursCredit.toString()}
                            title={hoursCredit.title}
                            indicatorColor={hoursCredit.isWorkOut ? calendarScreenColors.red : calendarScreenColors.blue} />
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

export const DaysCounters = connect(mapStateToProps)(DyasCountersImpl);