import React, { Component } from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { styles, colors } from '../styles';
import { Tile, EmptyTile } from './tile/tile';
import { TileSeparator } from './tile/tile-separator';
import { Days as DaysModel } from '../../reducers/calendar/days.model';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch, connect } from 'react-redux';
import { CalendarActions, loadDays, LoadDays } from '../../reducers/calendar/calendar.action';
import { bindActionCreators } from 'redux';

interface DaysProps {
    days: DaysModel;
}

interface ConnectedDaysProps extends DaysProps {
    dispatch: Dispatch<CalendarActions>;
}

class DaysImpl extends Component<ConnectedDaysProps, {}> {
    public componentDidMount() {
        this.props.dispatch(loadDays());
    }

    public render() {
        const { vacation, off, sick } = this.props.days;

        const vacationTile = vacation
            ? <Tile leftDays={vacation.leftDays} allDays={vacation.allDays} title={vacation.title} leftColor={colors.days.left} allColor={colors.days.all} />
            : <EmptyTile />;

        const offTile = off
            ? <Tile leftDays={off.leftDays} allDays={0} title={sick.title} leftColor={colors.days.left} allColor={colors.days.all} showAllDays={false} />
            : <EmptyTile />;

        const sickTile = sick
            ? <Tile leftDays={off.leftDays} allDays={sick.allDays} title={sick.title} leftColor={colors.days.sick} allColor={colors.days.all} />
            : <EmptyTile />;

        return <View style={styles.container}>
            { vacationTile }
            <TileSeparator />
            { offTile }
            <TileSeparator />
            { sickTile }
        </View>;
    }
}

const mapStateToProps = (state: AppState): DaysProps => ({
    days: state.calendar.days
});

export const Days = connect(mapStateToProps)(DaysImpl);