import React, { Component } from 'react';
import { StyleSheet, View } from 'react-native';
import { connect } from 'react-redux';
import { DayModel, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { AppState } from '../reducers/app.reducer';
import { CalendarEventsColor, legendStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';
import { Optional } from 'types';
import moment from 'moment';
import { CalendarEvent } from '../reducers/calendar/calendar-event.model';

//============================================================================
interface CalendarLegendProps {
    intervals: Optional<ReadOnlyIntervalsModel>;
    selectedDay: DayModel;
}

//============================================================================
class CalendarLegendImpl extends Component<CalendarLegendProps> {
    //----------------------------------------------------------------------------
    public render() {
        const selectedEvents = this.getEventsForSelectedDate();
        const legend = selectedEvents.map((event, index) => {
            const style = StyleSheet.flatten(
                [
                    styles.marker,
                    { backgroundColor: CalendarEventsColor.getColor(event.type) },
                ]
            );

            return (
                <View key={event.type + index} style={styles.itemContainer}>
                    <View style={style}/>
                    <StyledText style={styles.label}>{event.type}</StyledText>
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

    //----------------------------------------------------------------------------
    private getEventsForSelectedDate(): CalendarEvent[] {
        const { intervals, selectedDay } = this.props;

        if (!selectedDay || !intervals) {
            return [];
        }

        return (intervals.get(selectedDay.date) || []).map(interval => interval.calendarEvent);
    }
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CalendarLegendProps => ({
    intervals: state.calendar ? state.calendar.calendarEvents.intervals : undefined,
    selectedDay: state.calendar && state.calendar.calendarEvents.selection.single.day ? state.calendar.calendarEvents.selection.single.day : {
        date: moment(), today: true, belongsToCurrentMonth: true,
    }
});

export const CalendarLegend = connect(stateToProps)(CalendarLegendImpl);
