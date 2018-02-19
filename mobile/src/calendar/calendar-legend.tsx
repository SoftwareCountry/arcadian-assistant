import React, { Component } from 'react';
import { View, StyleSheet, PixelRatio } from 'react-native';
import { connect } from 'react-redux';
import { IntervalsModel } from '../reducers/calendar/calendar.model';
import { AppState } from '../reducers/app.reducer';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { calendarIntervalColors } from './styles';
import { StyledText } from '../override/styled-text';

interface CalendarLegendProps {
    intervals: IntervalsModel;
}

const mapStateToProps = (state: AppState): CalendarLegendProps => ({
    intervals: state.calendar.calendarEvents.intervals,
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
    private readonly intervalColors = {
        [CalendarEventsType.Vacation]: calendarIntervalColors.vacation,
        [CalendarEventsType.SickLeave]: calendarIntervalColors.sickLeave,
        [CalendarEventsType.Dayoff]: calendarIntervalColors.dayoff
    };

    private readonly intervalNames = {
        [CalendarEventsType.Vacation]: 'Vacation',
        [CalendarEventsType.SickLeave]: 'Sick Leave',
        [CalendarEventsType.Dayoff]: 'Dayoff'
    };

    public render() {
        const { intervals } = this.props;

        const existingIntervals = [CalendarEventsType.Vacation, CalendarEventsType.SickLeave, CalendarEventsType.Dayoff];
        const legend = existingIntervals.map(type => {
            const style = StyleSheet.flatten([styles.marker, { backgroundColor: this.intervalColors[type] }]);
            return (
                <View key={type} style={styles.itemContainer}>
                    <View style={style}></View>
                    <StyledText style={styles.label}>{this.intervalNames[type]}</StyledText>
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
}

export const CalendarLegend = connect(mapStateToProps)(CalendarLegendImpl);