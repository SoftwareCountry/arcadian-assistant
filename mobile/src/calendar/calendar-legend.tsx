import React, { Component } from 'react';
import { View, StyleSheet, PixelRatio } from 'react-native';
import { connect } from 'react-redux';
import { IntervalsModel, WeekModel } from '../reducers/calendar/calendar.model';
import { AppState } from '../reducers/app.reducer';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { CalendarIntervalColor, legendStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

interface CalendarLegendProps {
    intervals: IntervalsModel;
    weeks: WeekModel[];
}

const mapStateToProps = (state: AppState): CalendarLegendProps => ({
    intervals: state.calendar.calendarEvents.intervals,
    weeks: state.calendar.calendarEvents.weeks
});

class CalendarLegendImpl extends Component<CalendarLegendProps> {
    private getVisibleCalendarEvents() {
        const { intervals, weeks } = this.props;
        const calendarEventsMap: Map<CalendarEventsType, number> = new Map();

        if (!weeks || !intervals) {
            return calendarEventsMap;
        }

        weeks.forEach(week => {
            week.days.forEach(day => {
                const dayIntervals = intervals.get(day.date) || [];
                dayIntervals.forEach(interval => {
                    let eventCounter = calendarEventsMap.get(interval.eventType) || 0;
                    calendarEventsMap.set(interval.eventType, ++eventCounter);
                });
            });
        });

        return calendarEventsMap;
    }

    public render() {
        const existingIntervals = this.getVisibleCalendarEvents();
        const legend: JSX.Element[] = [];

        existingIntervals.forEach((value, type) => {
            const style = StyleSheet.flatten([styles.marker, { backgroundColor: CalendarIntervalColor.getColor(type) }]);
            legend.push((
                <View key={type} style={styles.itemContainer}>
                    <View style={style}></View>
                    <StyledText style={styles.label}>{type}</StyledText>
                </View>
            ));
        });

        return (
            <View style={styles.container}>
                <View>
                    {legend}
                </View>
            </View>
        );
    }
}

export const CalendarLegend = connect(mapStateToProps)(CalendarLegendImpl);