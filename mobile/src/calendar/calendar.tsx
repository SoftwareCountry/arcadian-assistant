import React, { Component, ComponentClass } from 'react';
import moment, { Moment } from 'moment';
import { connect, MergeProps, InferableComponentEnhancerWithProps, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { CalendarEvents } from '../reducers/calendar/calendar-events';
import { CalendarPage, OnSelectedDayCallback, DayModel } from './calendar-page';
import { CalendarPager } from './calendar-pager';
import { CalendarActions, selectCalendarDay } from '../reducers/calendar/calendar.action';

interface CalendarProps {
    calendarEvents?: CalendarEvents[];
}

interface CalendarDispatchProps {
    selectCalendarDay: OnSelectedDayCallback;
}

export class CalendarImpl extends Component<CalendarProps & CalendarDispatchProps> {
    public render() {
        return <CalendarPager onSelectedDay={this.onSelectedDay} />;
    }

    private onSelectedDay: OnSelectedDayCallback = (day) => {
        this.props.selectCalendarDay(day);
    }
}

const mapStateToProps = (state: AppState): CalendarProps => ({
    calendarEvents: state.calendar.calendarEvents
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>) => ({
    selectCalendarDay: (day: DayModel) => { dispatch(selectCalendarDay(day)); }
});

export const Calendar = connect(mapStateToProps, mapDispatchToProps)(CalendarImpl);