import React, { Component } from 'react';
import { View } from 'react-native';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, startProgress } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { cancelSickLeave } from '../../reducers/calendar/sick-leave.action';
import { Employee } from '../../reducers/organization/employee.model';

interface CancelSickLeaveEventDialogDispatchProps {
    cancelSickLeave: (employeeId: string, calendarEvent: CalendarEvent) => void;
    closeDialog: () => void;
}

interface CancelSickLeaveEventDialogProps {
    intervals: ExtractedIntervals;
    userEmployee: Employee;
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
    }

    private acceptAction = () => {
        const { userEmployee, intervals } = this.props;

        this.props.cancelSickLeave(userEmployee.employeeId, intervals.sickleave.calendarEvent);
    }

    private closeDialog = () => {
        this.props.closeDialog();
    }

    public get text(): string {
        const { intervals: { sickleave } } = this.props;
        const startDate = sickleave.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);

        return `Your sick leave starts on ${startDate}`;
    }
}

const mapStateToProps = (state: AppState): CancelSickLeaveEventDialogProps => ({
    intervals: state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection,
    userEmployee: state.organization.employees.employeesById.get(state.userInfo.employeeId)
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): CancelSickLeaveEventDialogDispatchProps => ({
    closeDialog: () => { dispatch(closeEventDialog()); },
    cancelSickLeave: (employeeId: string, calendarEvent: CalendarEvent) => { 
        dispatch(startProgress());
        dispatch(cancelSickLeave(employeeId, calendarEvent)); 
    }
});

export const CancelSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(CancelSickLeaveEventDialogImpl);