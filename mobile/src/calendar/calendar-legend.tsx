import React, { Component } from 'react';
import { View, StyleSheet, PixelRatio } from 'react-native';
import { connect } from 'react-redux';
import { IntervalsModel, DayModel } from '../reducers/calendar/calendar.model';
import { AppState } from '../reducers/app.reducer';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { CalendarIntervalColor, legendStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

interface CalendarLegendProps {
    intervals: IntervalsModel;
    selectedDay: DayModel;
}

const mapStateToProps = (state: AppState): CalendarLegendProps => ({
    intervals: state.calendar.calendarEvents.intervals,
    selectedDay: state.calendar.calendarEvents.selectedCalendarDay
});

class CalendarLegendImpl extends Component<CalendarLegendProps> {
    public render() {
        const selectedEvents = this.getEventsForSelectedDate();
        const legend = selectedEvents.map(type => {
            const style = StyleSheet.flatten([styles.marker, { backgroundColor: CalendarIntervalColor.getColor(type) }]);
            return (
                <View key={type} style={styles.itemContainer}>
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

        return (intervals.get(selectedDay.date) || []).map(interval => interval.eventType);
    }
}

export const CalendarLegend = connect(mapStateToProps)(CalendarLegendImpl);