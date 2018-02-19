import React, { Component } from 'react';
import { View } from 'react-native';
import { connect } from 'react-redux';
import { DayModel } from '../reducers/calendar/calendar.model';
import { SelectedDay } from './days-counters/selected-day';
import { AppState } from '../reducers/app.reducer';

interface TodayProps {
    selectedCalendarDay: DayModel;
}
class TodayImpl extends Component<TodayProps> {
    public render() {
        const { selectedCalendarDay } = this.props;


        const selectedDay = selectedCalendarDay
            ? selectedCalendarDay.date
            : null;

        return <SelectedDay day={selectedDay} />;
    }
}

const mapStateToProps = (state: AppState): TodayProps => ({
    selectedCalendarDay: state.calendar.calendarEvents.selectedCalendarDay
});

export const Today = connect(mapStateToProps)(TodayImpl);