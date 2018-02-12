import React, { Component } from 'react';
import moment, { Moment } from 'moment';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { CalendarEvents } from '../reducers/calendar/calendar-events';
import { CalendarPage } from './calendar-page';
import { CalendarPager } from './calendar-pager';

interface CalendarProps {
    calendarEvents: CalendarEvents[];
}

interface CalendarState {
    date: Moment;
}

export class CalendarImpl extends Component<CalendarProps, CalendarState> {
    public render() {
        return <CalendarPager />;
    }
}

const mapStateToProps = (state: AppState): CalendarProps => ({
    calendarEvents: state.calendar.calendarEvents
});

export const Calendar = connect(mapStateToProps)(CalendarImpl);