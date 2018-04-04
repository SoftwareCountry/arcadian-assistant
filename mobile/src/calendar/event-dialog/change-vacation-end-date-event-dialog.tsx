import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals, IntervalSelection } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { confirmVacationChange } from '../../reducers/calendar/vacation.action';
import { Employee } from '../../reducers/organization/employee.model';
import { Moment } from 'moment';

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
    userEmployee: Employee;
    interval: IntervalSelection;
    intervals: ExtractedIntervals;
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
                    onActionPress={this.confirmEndDateChange}
                    onCancelPress={this.back}
                    onClosePress={this.closeDialog}
                    disableAccept={disableAccept} />;
    }

    private back = () => {
        this.props.back();
    }

    private confirmEndDateChange = () => {
        const { interval, userEmployee, intervals: { vacation } } = this.props;

        this.props.confirmChangeVacation(
            userEmployee.employeeId,
            vacation.calendarEvent,
            interval.startDay.date,
            interval.endDay.date);
    }

    private closeDialog = () => {
        this.props.closeDialog();
    }

    public get text(): string {
        const { interval } = this.props;

        const endDate = interval && interval.endDay
            ? interval.endDay.date.format(eventDialogTextDateFormat)
            : '';

        return `Your vacation completes on ${endDate}`;
    }
}

const mapStateToProps = (state: AppState): ChangeVacationEndDateEventDialogProps => ({
    userEmployee: state.userInfo.employee,
    interval: state.calendar.calendarEvents.selection.interval,
    intervals: state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ChangeVacationEndDateEventDialogDispatchProps => ({
    back: () => { dispatch(openEventDialog(EventDialogType.ChangeVacationStartDate)); },
    confirmChangeVacation: (
        employeeId: string,
        calendarEvent: CalendarEvent,
        startDate: Moment,
        endDate: Moment
    ) => { dispatch(confirmVacationChange(employeeId, calendarEvent, startDate, endDate)); },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const ChangeVacationEndDateEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChangeVacationEndDateEventDialogImpl);