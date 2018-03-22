import React, { Component } from 'react';
import { View, StyleSheet, PixelRatio } from 'react-native';
import { connect } from 'react-redux';
import { DayModel, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { AppState } from '../reducers/app.reducer';
import { CalendarEventsColor, legendStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

interface CalendarLegendProps {
    intervals: ReadOnlyIntervalsModel;
    selectedDay: DayModel;
}

const mapStateToProps = (state: AppState): CalendarLegendProps => ({
    intervals: state.calendar.calendarEvents.intervals,
    selectedDay: state.calendar.calendarEvents.selection.single.day
});

class CalendarLegendImpl extends Component<CalendarLegendProps> {
    public render() {
        const selectedEvents = this.getEventsForSelectedDate();
        const legend = selectedEvents.map((type, index) => {
            const style = StyleSheet.flatten([styles.marker, { backgroundColor: CalendarEventsColor.getColor(type) }]);
            return (
                <View key={type + index} style={styles.itemContainer}>
                    <View style={style}></View>
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