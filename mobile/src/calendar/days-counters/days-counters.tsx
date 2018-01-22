import React, { Component } from 'react';
import { View } from 'react-native';
import { styles, colors } from '../styles';
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
        const { vacation, off, sick } = this.props.daysCounters;

        const vacationTile = vacation
            ? <DaysCounter leftDays={vacation.leftDays} allDays={vacation.allDays} title={vacation.title} leftColor={colors.days.left} allColor={colors.days.all} />
            : <EmptyDaysCounter />;

        const offTile = off
            ? <DaysCounter leftDays={off.leftDays} allDays={0} title={sick.title} leftColor={colors.days.left} allColor={colors.days.all} showAllDays={false} />
            : <EmptyDaysCounter />;

        const sickTile = sick
            ? <DaysCounter leftDays={off.leftDays} allDays={sick.allDays} title={sick.title} leftColor={colors.days.sick} allColor={colors.days.all} />
            : <EmptyDaysCounter />;

        return <View style={styles.container}>
            { vacationTile }
            <DaysCounterSeparator />
            { offTile }
            <DaysCounterSeparator />
            { sickTile }
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