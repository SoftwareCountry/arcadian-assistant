import React, { Component } from 'react';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { OnSelectedDayCallback } from './calendar-page';
import { CalendarPager } from './calendar-pager';
import { nextCalendarPage, prevCalendarPage, selectCalendarDay } from '../reducers/calendar/calendar.action';
import {
    CalendarPageModel,
    CalendarSelection,
    DayModel,
    ReadOnlyIntervalsModel
} from '../reducers/calendar/calendar.model';
import { Action, Dispatch } from 'redux';
import { Nullable, Optional } from 'types';

interface CalendarProps {
    pages: Nullable<CalendarPageModel[]>;
    intervals: Nullable<ReadOnlyIntervalsModel>;
    selection: Nullable<CalendarSelection>;
    disableCalendarDaysBefore: Optional<DayModel>;
}

interface CalendarDispatchProps {
    selectCalendarDay: OnSelectedDayCallback;
    nextCalendarPage: () => void;
    prevCalendarPage: () => void;
}

export class CalendarImpl extends Component<CalendarProps & CalendarDispatchProps> {
    public render() {

        if (!this.props.pages || !this.props.intervals || !this.props.selection || !this.props.disableCalendarDaysBefore) {
            return null;
        }

        return <CalendarPager
            onSelectedDay={this.onSelectedDay}
            pages={this.props.pages}
            intervals={this.props.intervals}
            selection={this.props.selection}
            onNextPage={this.onNextPage}
            onPrevPage={this.onPrevPage}
            disableBefore={this.props.disableCalendarDaysBefore}/>;
    }

    private onSelectedDay: OnSelectedDayCallback = (day) => {
        this.props.selectCalendarDay(day);
    };

    private onNextPage = () => {
        this.props.nextCalendarPage();
    };

    private onPrevPage = () => {
        this.props.prevCalendarPage();
    };
}

const mapStateToProps = (state: AppState): CalendarProps => ({
    pages: state.calendar ? state.calendar.calendarEvents.pages : null,
    intervals: state.calendar && state.calendar.calendarEvents.intervals ? state.calendar.calendarEvents.intervals : null,
    selection: state.calendar ? state.calendar.calendarEvents.selection : null,
    disableCalendarDaysBefore: state.calendar ? state.calendar.calendarEvents.disableCalendarDaysBefore : undefined,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): CalendarDispatchProps => ({
    selectCalendarDay: (day: DayModel) => {
        dispatch(selectCalendarDay(day));
    },
    nextCalendarPage: () => {
        dispatch(nextCalendarPage());
    },
    prevCalendarPage: () => {
        dispatch(prevCalendarPage());
    }
});

export const Calendar = connect(mapStateToProps, mapDispatchToProps)(CalendarImpl);
