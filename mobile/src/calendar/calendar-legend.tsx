import React, { Component } from 'react';
import { View, StyleSheet, PixelRatio } from 'react-native';
import { connect } from 'react-redux';
import { IntervalsModel, WeekModel } from '../reducers/calendar/calendar.model';
import { AppState } from '../reducers/app.reducer';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { calendarIntervalColors } from './styles';
import { StyledText } from '../override/styled-text';

interface CalendarLegendProps {
    intervals: IntervalsModel;
    weeks: WeekModel[];
}

const mapStateToProps = (state: AppState): CalendarLegendProps => ({
    intervals: state.calendar.calendarEvents.intervals,
    weeks: state.calendar.calendarEvents.weeks
});

const legendMarkerSize = 12;
const styles = StyleSheet.create({
    container: {
        flex: 1,
        alignItems: 'center',
        paddingTop: 15
    },
    itemContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingBottom: 5
    },
    marker: {
        width: legendMarkerSize,
        height: legendMarkerSize,
        borderRadius: PixelRatio.roundToNearestPixel(legendMarkerSize / 2)
    },
    label: {
        paddingLeft: 10,
        color: '#18515E',
        fontSize: 10,
        lineHeight: 12
    }
});

class CalendarLegendImpl extends Component<CalendarLegendProps> {
    private readonly intervalColors: { [type: string]: string } = {
        [CalendarEventsType.Vacation]: calendarIntervalColors.vacation,
        [CalendarEventsType.SickLeave]: calendarIntervalColors.sickLeave,
        [CalendarEventsType.Dayoff]: calendarIntervalColors.dayoff
    };

    private readonly intervalNames: { [type: string]: string } = {
        [CalendarEventsType.Vacation]: 'Vacation',
        [CalendarEventsType.SickLeave]: 'Sick Leave',
        [CalendarEventsType.Dayoff]: 'Dayoff'
    };

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
                    calendarEventsMap.set(interval.eventType, eventCounter++);
                });
            });
        });

        return calendarEventsMap;
    }

    public render() {
        const { intervals, weeks } = this.props;

        const existingIntervals = this.getVisibleCalendarEvents();
        const legend: JSX.Element[] = [];

        existingIntervals.forEach((value, type) => {
            const style = StyleSheet.flatten([styles.marker, { backgroundColor: this.intervalColors[type] }]);
            legend.push((
                <View key={type} style={styles.itemContainer}>
                    <View style={style}></View>
                    <StyledText style={styles.label}>{this.intervalNames[type]}</StyledText>
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