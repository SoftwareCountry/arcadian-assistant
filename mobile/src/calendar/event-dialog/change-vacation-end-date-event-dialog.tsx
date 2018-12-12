import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals, IntervalSelection } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { confirmVacationChange } from '../../reducers/calendar/vacation.action';
import { Employee } from '../../reducers/organization/employee.model';
import { Moment } from 'moment';
import { getEmployee } from '../../utils/utils';
import { Optional } from 'types';

interface ChangeVacationEndDateEventDialogDispatchProps {
    back: () => void;
    confirmChangeVacation: (
        employeeId: string,
        calendarEvent: CalendarEvent,
        startDate: Moment,
        endDate: Moment
    ) => void;
    closeDialog: () => void;
}

interface ChangeVacationEndDateEventDialogProps {
    userEmployee: Optional<Employee>;
    interval: Optional<IntervalSelection>;
    intervals: Optional<ExtractedIntervals>;
}

class ChangeVacationEndDateEventDialogImpl extends Component<ChangeVacationEndDateEventDialogProps & ChangeVacationEndDateEventDialogDispatchProps> {
    public render() {
        const disableAccept = !this.props.interval || !this.props.interval.endDay;

        return <EventDialogBase
            title={'Change end date'}
            text={this.text}
            icon={'vacation'}
            cancelLabel={'Back'}
            acceptLabel={'Confirm'}
            onAcceptPress={this.confirmEndDateChange}
            onCancelPress={this.back}
            onClosePress={this.closeDialog}
            disableAccept={disableAccept}/>;
    }

    private back = () => {
        this.props.back();
    };

    private confirmEndDateChange = () => {
        const { interval, userEmployee, intervals } = this.props;

        if (!userEmployee || !interval || !intervals || !intervals.vacation || !interval.startDay || !interval.endDay) {
            return;
        }

        this.props.confirmChangeVacation(
            userEmployee.employeeId,
            intervals.vacation.calendarEvent,
            interval.startDay.date,
            interval.endDay.date);
    };

    private closeDialog = () => {
        this.props.closeDialog();
    };

    public get text(): string {
        const { interval } = this.props;

        const endDate = interval && interval.endDay
            ? interval.endDay.date.format(eventDialogTextDateFormat)
            : '';

        return `Your vacation completes on ${endDate}`;
    }
}

const mapStateToProps = (state: AppState): ChangeVacationEndDateEventDialogProps => ({
    userEmployee: getEmployee(state),
    interval: state.calendar ? state.calendar.calendarEvents.selection.interval : undefined,
    intervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): ChangeVacationEndDateEventDialogDispatchProps => ({
    back: () => {
        dispatch(openEventDialog(EventDialogType.ChangeVacationStartDate));
    },
    confirmChangeVacation: (
        employeeId: string,
        calendarEvent: CalendarEvent,
        startDate: Moment,
        endDate: Moment
    ) => {
        dispatch(confirmVacationChange(employeeId, calendarEvent, startDate, endDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

export const ChangeVacationEndDateEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChangeVacationEndDateEventDialogImpl);
