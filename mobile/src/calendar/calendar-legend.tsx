import React, { Component } from 'react';
import { View, StyleSheet, PixelRatio } from 'react-native';
import { connect } from 'react-redux';
import { DayModel, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { AppState } from '../reducers/app.reducer';
import { CalendarEventsColor, legendStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';
import { Nullable, Optional } from 'types';
import moment from 'moment';

interface CalendarLegendProps {
    intervals: Optional<ReadOnlyIntervalsModel>;
    selectedDay: DayModel;
}

const mapStateToProps = (state: AppState): CalendarLegendProps => ({
    intervals: state.calendar ? state.calendar.calendarEvents.intervals : undefined,
    selectedDay: state.calendar && state.calendar.calendarEvents.selection.single.day ?  state.calendar.calendarEvents.selection.single.day : {
        date: moment(), today: true, belongsToCurrentMonth: true,
    }
});

class CalendarLegendImpl extends Component<CalendarLegendProps> {
    public render() {
        const selectedEvents = this.getEventsForSelectedDate();
        const legend = selectedEvents.map((type, index) => {
            const style = StyleSheet.flatten([styles.marker, { backgroundColor: CalendarEventsColor.getColor(type) }]);
            return (
                <View key={type + index} style={styles.itemContainer}>
                    <View style={style}/>
                    <StyledText style={styles.label}>{type}</StyledText>
                </View>
            );
        });

        return (
            <View style={styles.container}>
                <View>
                    {legend}
                </View>
            </View>
        );
    }

    private getEventsForSelectedDate() {
        const { intervals, selectedDay } = this.props;

        if (!selectedDay || !intervals) {
            return [];
        }

        return (intervals.get(selectedDay.date) || []).map(interval => interval.calendarEvent.type);
    }
}

export const CalendarLegend = connect(mapStateToProps)(CalendarLegendImpl);
