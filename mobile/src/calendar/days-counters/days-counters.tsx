import React, { Component } from 'react';
import { View, Text, LayoutChangeEvent, ViewStyle, StyleSheet } from 'react-native';
import { daysCountersStyles } from '../styles';
import { DaysCounter, EmptyDaysCounter } from './days-counter';
import { DaysCounterSeparator } from './days-counter-separator';
import { DaysCountersModel } from '../../reducers/calendar/days-counters.model';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';
import { SelectedDay } from './selected-day';
import { Triangle } from './triangle';
import moment from 'moment';

interface DaysCountersProps {
    daysCounters: DaysCountersModel;
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
        const { daysCounters: { allVacationDays, hoursCredit } } = this.props;

        const vacationCounter = allVacationDays
            ? <DaysCounter  textValue={allVacationDays.toString()}
                            title={allVacationDays.title} />
            : <EmptyDaysCounter />;

        const daysoffCounter = hoursCredit
            ? <DaysCounter  textValue={hoursCredit.toString()}
                            title={hoursCredit.title} />
            : <EmptyDaysCounter />;

        // TODO: temp
        const currentDate = moment();
        const date = { day: currentDate.format('D'), month: currentDate.format('MMMM') };

        return <View style={daysCountersStyles.container} onLayout={this.onDaysCountersLayout}>
                <Triangle containerWidth={this.state.daysCountersWidth} />
                <SelectedDay {...date} />
                <View style={daysCountersStyles.counters}>
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
    daysCounters: state.calendar.daysCounters
});

export const DaysCounters = connect(mapStateToProps)(DaysCountersImpl);