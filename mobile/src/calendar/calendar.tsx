import React, { Component } from 'react';
import { connect } from 'react-redux';
import { AppState, getEmployee } from '../reducers/app.reducer';
import { OnSelectedDayCallback } from './calendar-page';
import { CalendarPager } from './calendar-pager';
import {
    loadCalendarEvents,
    nextCalendarPage,
    prevCalendarPage,
    resetCalendarPages,
    selectCalendarDay
} from '../reducers/calendar/calendar.action';
import {
    CalendarPageModel,
    CalendarSelection,
    DayModel,
    ReadOnlyIntervalsModel
} from '../reducers/calendar/calendar.model';
import { Action, Dispatch } from 'redux';
import { Nullable, Optional } from 'types';
import { none } from '../types/types-utils';
import { NavigationEventSubscription, NavigationScreenProps } from 'react-navigation';
import { Employee } from '../reducers/organization/employee.model';

//============================================================================
interface CalendarProps {
    employee: Optional<Employee>;
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
    resetCalendarPages: () => void;
    loadCalendarEvents: (employee: Employee) => void;
}

//============================================================================
class CalendarImpl extends Component<CalendarProps & CalendarDispatchProps & NavigationScreenProps> {

    private refocusSubscription?: NavigationEventSubscription;
    private willFocusSubscription?: NavigationEventSubscription;

    //----------------------------------------------------------------------------
    public componentDidMount() {
        const { employee } = this.props;

        this.refocusSubscription = this.props.navigation.addListener('refocus', () => {
            this.props.resetCalendarPages();
        });

        this.willFocusSubscription = this.props.navigation.addListener('willFocus', () => {
            if (employee) {
                this.props.loadCalendarEvents(employee);
            }
        });
    }

    //----------------------------------------------------------------------------
    public componentWillUnmount(): void {
        if (this.refocusSubscription) {
            this.refocusSubscription.remove();
        }

        if (this.willFocusSubscription) {
            this.willFocusSubscription.remove();
        }
    }

    //----------------------------------------------------------------------------
    public render() {
        const { pages, intervals, selection, disableCalendarDaysBefore } = this.props;

        if (none(pages)) {
            return null;
        }

        return <CalendarPager
            onSelectedDay={this.onSelectedDay}
            pages={pages}
            intervals={intervals}
            selection={selection}
            onNextPage={this.onNextPage}
            onPrevPage={this.onPrevPage}
            disableBefore={disableCalendarDaysBefore}/>;
    }

    //----------------------------------------------------------------------------
    private onSelectedDay: OnSelectedDayCallback = (day) => {
        this.props.selectCalendarDay(day);
    };

    //----------------------------------------------------------------------------
    private onNextPage = () => {
        this.props.nextCalendarPage();
    };

    //----------------------------------------------------------------------------
    private onPrevPage = () => {
        this.props.prevCalendarPage();
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CalendarProps => ({
    employee: getEmployee(state),
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
    },
    resetCalendarPages: () => {
        dispatch(resetCalendarPages());
    },
    loadCalendarEvents: (employee: Employee) => {
        dispatch(loadCalendarEvents(employee.employeeId));
    }
});

//----------------------------------------------------------------------------
export const Calendar = connect(stateToProps, dispatchToProps)(CalendarImpl);
