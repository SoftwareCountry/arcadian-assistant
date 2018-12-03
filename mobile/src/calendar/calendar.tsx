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
import { none } from '../types/types-utils';

//============================================================================
interface CalendarProps {
    pages: Nullable<CalendarPageModel[]>;
    intervals?: ReadOnlyIntervalsModel;
    selection?: CalendarSelection;
    disableCalendarDaysBefore?: DayModel;
}

//============================================================================
interface CalendarDispatchProps {
    selectCalendarDay: OnSelectedDayCallback;
    nextCalendarPage: () => void;
    prevCalendarPage: () => void;
}

//============================================================================
class CalendarImpl extends Component<CalendarProps & CalendarDispatchProps> {
    public render() {

        if (none(this.props.pages)) {
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

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CalendarProps => ({
    pages: state.calendar ? state.calendar.calendarEvents.pages : null,
    intervals: state.calendar && state.calendar.calendarEvents.intervals ? state.calendar.calendarEvents.intervals : undefined,
    selection: state.calendar ? state.calendar.calendarEvents.selection : undefined,
    disableCalendarDaysBefore: state.calendar ? state.calendar.calendarEvents.disableCalendarDaysBefore : undefined,
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>): CalendarDispatchProps => ({
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

//----------------------------------------------------------------------------
export const Calendar = connect(stateToProps, dispatchToProps)(CalendarImpl);
