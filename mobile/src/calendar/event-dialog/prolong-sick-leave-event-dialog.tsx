import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { ExtractedIntervals, IntervalSelection } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { confirmProlongSickLeave } from '../../reducers/calendar/sick-leave.action';
import { connect } from 'react-redux';
import { Employee } from '../../reducers/organization/employee.model';
import { Moment } from 'moment';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { Action, Dispatch } from 'redux';
import { getEmployee } from '../../utils/utils';
import { Optional } from 'types';

interface ProlongSickLeaveEventDialogDispatchProps {
    back: () => void;
    closeDialog: () => void;
    confirmProlong: (employeeId: string, calendarEvent: CalendarEvent, prolongedEndDate: Moment) => void;
}

interface ProlongSickLeaveEventDialogProps {
    intervalsBySingleDaySelection: Optional<ExtractedIntervals>;
    intervalSelection: Optional<IntervalSelection>;
    userEmployee: Optional<Employee>;
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
            onAcceptPress={this.onAcceptClick}
            onCancelPress={this.onCancelClick}
            onClosePress={this.onCloseClick}
            disableAccept={disableAccept}/>;
    }

    public get text(): string {
        if (!this.props.intervalsBySingleDaySelection || !this.props.intervalsBySingleDaySelection.sickleave ||
            !this.props.intervalSelection || !this.props.intervalSelection.endDay) {
            return '';
        }

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
        this.props.back();
    };

    private onAcceptClick = () => {
        if (!this.props.userEmployee || !this.props.intervalsBySingleDaySelection ||
            !this.props.intervalsBySingleDaySelection.sickleave || !this.props.intervalSelection ||
            !this.props.intervalSelection.endDay || !this.props.intervalSelection.endDay.date) {
            return;
        }

        this.props.confirmProlong(
            this.props.userEmployee.employeeId,
            this.props.intervalsBySingleDaySelection.sickleave.calendarEvent,
            this.props.intervalSelection.endDay.date);
    };

    private onCloseClick = () => {
        this.props.closeDialog();
    };

    private isProlongEndDateValid(): boolean {
        if (!this.props.userEmployee || !this.props.intervalsBySingleDaySelection ||
            !this.props.intervalsBySingleDaySelection.sickleave || !this.props.intervalSelection ||
            !this.props.intervalSelection.endDay || !this.props.intervalSelection.endDay.date) {
            return false;
        }

        const selectedEndDate = this.props.intervalSelection && this.props.intervalSelection.endDay
            ? this.props.intervalSelection.endDay.date
            : null;

        if (!selectedEndDate) {
            return false;
        }

        return selectedEndDate.isAfter(this.props.intervalsBySingleDaySelection.sickleave.calendarEvent.dates.endDate, 'days');
    }
}

const mapStateToProps = (state: AppState): ProlongSickLeaveEventDialogProps => ({
    intervalsBySingleDaySelection: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    intervalSelection: state.calendar ? state.calendar.calendarEvents.selection.interval : undefined,
    userEmployee: getEmployee(state)
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): ProlongSickLeaveEventDialogDispatchProps => ({
    back: () => {
        dispatch(openEventDialog(EventDialogType.EditSickLeave));
    },
    confirmProlong: (employeeId: string, calendarEvent: CalendarEvent, prolongedEndDate: Moment) => {
        dispatch(confirmProlongSickLeave(employeeId, calendarEvent, prolongedEndDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

export const ProlongSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(ProlongSickLeaveEventDialogImpl);
