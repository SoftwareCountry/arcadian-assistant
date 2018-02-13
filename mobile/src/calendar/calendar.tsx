import React, { Component, ComponentClass } from 'react';
import { connect, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { OnSelectedDayCallback } from './calendar-page';
import { CalendarPager } from './calendar-pager';
import { CalendarActions, selectCalendarDay, selectCalendarMonth } from '../reducers/calendar/calendar.action';
import { WeekModel, DayModel } from '../reducers/calendar/calendar.model';

interface CalendarProps {
    weeks: WeekModel[];
}

interface CalendarDispatchProps {
    selectCalendarDay: OnSelectedDayCallback;
    selectCalendarMonth: (month: number, year: number) => void;
}

export class CalendarImpl extends Component<CalendarProps & CalendarDispatchProps> {
    public render() {
        return <CalendarPager onSelectedDay={this.onSelectedDay} weeks={this.props.weeks} onNextMonth={this.onSelectMonth} onPrevMonth={this.onSelectMonth} />;
    }

    private onSelectedDay: OnSelectedDayCallback = (day) => {
        this.props.selectCalendarDay(day);
    }

    private onSelectMonth = (month: number, year: number) => {
        this.props.selectCalendarMonth(month, year);
    }
}

const mapStateToProps = (state: AppState): CalendarProps => ({
    weeks: state.calendar.calendarEvents.weeks
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>) => ({
    selectCalendarDay: (day: DayModel) => { dispatch(selectCalendarDay(day)); },
    selectCalendarMonth: (month: number, year: number) => { dispatch(selectCalendarMonth(month, year)); },
});

export const Calendar = connect(mapStateToProps, mapDispatchToProps)(CalendarImpl);