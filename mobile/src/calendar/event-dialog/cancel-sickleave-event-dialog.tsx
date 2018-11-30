import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import {Action, Dispatch} from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { cancelSickLeave } from '../../reducers/calendar/sick-leave.action';
import { Employee } from '../../reducers/organization/employee.model';
import { getEmployee } from '../../utils/utils';
import { Optional } from 'types';

interface CancelSickLeaveEventDialogDispatchProps {
    cancelSickLeave: (employeeId: string, calendarEvent: CalendarEvent) => void;
    closeDialog: () => void;
}

interface CancelSickLeaveEventDialogProps {
    intervals: Optional<ExtractedIntervals>;
    userEmployee: Optional<Employee>;
}

class CancelSickLeaveEventDialogImpl extends Component<CancelSickLeaveEventDialogProps & CancelSickLeaveEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
                    title={'Hey! Hope you feel better'}
                    text={this.text}
                    icon={'sick_leave'}
                    cancelLabel={'Back'}
                    acceptLabel={'Cancel'}
                    onAcceptPress={this.acceptAction}
                    onCancelPress={this.cancelAction}
                    onClosePress={this.closeDialog} />;
    }

    private cancelAction = () => {
        this.props.closeDialog();
    };

    private acceptAction = () => {
        if(!this.props.intervals || !this.props.intervals.sickleave || !this.props.userEmployee){
            return;
        }

        this.props.cancelSickLeave(this.props.userEmployee.employeeId, this.props.intervals.sickleave.calendarEvent);
    };

    private closeDialog = () => {
        this.props.closeDialog();
    };

    public get text(): string {
        if(!this.props.intervals || !this.props.intervals.sickleave){
            return '';
        }

        const startDate = this.props.intervals.sickleave.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);

        return `Your sick leave starts on ${startDate}`;
    }
}

const mapStateToProps = (state: AppState): CancelSickLeaveEventDialogProps => ({
    intervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    userEmployee: getEmployee(state),
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): CancelSickLeaveEventDialogDispatchProps => ({
    closeDialog: () => { dispatch(closeEventDialog()); },
    cancelSickLeave: (employeeId: string, calendarEvent: CalendarEvent) => { dispatch(cancelSickLeave(employeeId, calendarEvent)); }
});

export const CancelSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(CancelSickLeaveEventDialogImpl);
