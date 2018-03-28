import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { DayModel, ExtractedIntervals, IntervalSelection } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { openEventDialog, closeEventDialog, EventDialogActions } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { confirmProlongSickLeave } from '../../reducers/calendar/sick-leave.action';
import { connect, Dispatch } from 'react-redux';
import { Employee } from '../../reducers/organization/employee.model';
import { Moment } from 'moment';
import { selectIntervalsBySingleDaySelection } from '../../reducers/calendar/calendar.action';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';

interface ProlongSickLeaveEventDialogDispatchProps {
    back: () => void;
    closeDialog: () => void;
    confirmProlong: (employeeId: string, calendarEvent: CalendarEvent, prolongedEndDate: Moment) => void;
}

interface ProlongSickLeaveEventDialogProps {
    intervalsBySingleDaySelection: ExtractedIntervals;
    intervalSelection: IntervalSelection;
    userEmployee: Employee;
}

export class ProlongSickLeaveEventDialogImpl extends Component<ProlongSickLeaveEventDialogDispatchProps & ProlongSickLeaveEventDialogProps> {
    public render() {
        const disableAccept = !this.isProlongEndDateValid();

        return <EventDialogBase
                    title={'Select date to Prolong your Sick Leave'}
                    text={this.text}
                    icon={'sick_leave'}
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onActionPress={this.onAcceptClick}
                    onCancelPress={this.onCancelClick}
                    onClosePress={this.onCloseClick}
                    disableAccept={disableAccept} />;
    }

    public get text(): string {
        const { intervalsBySingleDaySelection: { sickleave } } = this.props;

        const startDate = sickleave
            ? sickleave.calendarEvent.dates.startDate.format(eventDialogTextDateFormat)
            : '';

        const endDate = this.isProlongEndDateValid()
            ? this.props.intervalSelection.endDay.date.format(eventDialogTextDateFormat)
            : '';

        return `Your sick leave has started on ${startDate} and will be prolonged to ${endDate}`;
    }

    private onCancelClick = () => {
        this.props.closeDialog();
    }

    private onAcceptClick = () => {
        const { userEmployee, intervalsBySingleDaySelection: { sickleave }, intervalSelection, confirmProlong } = this.props;
        confirmProlong(userEmployee.employeeId, sickleave.calendarEvent, intervalSelection.endDay.date);
    }

    private onCloseClick = () => {
        this.props.closeDialog();
    }

    private isProlongEndDateValid(): boolean {
        const { intervalSelection, intervalsBySingleDaySelection: { sickleave } } = this.props;

        if (!sickleave) {
            return false;
        }

        const selectedEndDate = intervalSelection && intervalSelection.endDay
            ? intervalSelection.endDay.date
            : null;

        if (!selectedEndDate) {
            return false;
        }

        return selectedEndDate.isAfter(sickleave.calendarEvent.dates.endDate, 'days');
    }
}

const mapStateToProps = (state: AppState): ProlongSickLeaveEventDialogProps => ({
    intervalsBySingleDaySelection: state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection,
    intervalSelection: state.calendar.calendarEvents.selection.interval,
    userEmployee: state.userInfo.employee
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ProlongSickLeaveEventDialogDispatchProps => ({
    back: () => { dispatch(openEventDialog(EventDialogType.EditSickLeave)); },
    confirmProlong: (employeeId: string, calendarEvent: CalendarEvent, prolongedEndDate: Moment) => { dispatch(confirmProlongSickLeave(employeeId, calendarEvent, prolongedEndDate)); },
    closeDialog: () => { dispatch(closeEventDialog()); },
});

export const ProlongSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(ProlongSickLeaveEventDialogImpl);