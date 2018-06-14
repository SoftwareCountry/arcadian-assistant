import React, { Component } from 'react';
import { View } from 'react-native';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog, startProgress } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { Employee } from '../../reducers/organization/employee.model';
import { confirmSickLeave } from '../../reducers/calendar/sick-leave.action';
import { Moment } from 'moment';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';

interface ClaimSickLeaveEventDialogDispatchProps {
    back: () => void;
    confirmSickLeave: (employeeId: string, startDate: Moment, endDate: Moment) => void;
    closeDialog: () => void;
}

interface ClaimSickLeaveEventDialogProps {
    startDay: DayModel;
    endDay: DayModel;
    userEmployee: Employee;
}

class ConfirmSickLeaveEventDialogImpl extends Component<ClaimSickLeaveEventDialogProps & ClaimSickLeaveEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase 
                    title={'Select date to Complete your Sick Leave'} 
                    text={this.text}
                    icon={'sick_leave'} 
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onAcceptPress={this.acceptAction}
                    onCancelPress={this.cancelAction}
                    onClosePress={this.closeDialog}
                    disableAccept={!this.props.endDay} />;
    }

    private cancelAction = () => {
        this.props.back();
    }

    private acceptAction = () => {
        this.props.confirmSickLeave(this.props.userEmployee.employeeId, this.props.startDay.date, this.props.endDay.date);
    }

    private closeDialog = () => {
        this.props.closeDialog();
    }

    private get text(): string {
        return `Your sick leave starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}${this.props.endDay ? ` and completes on ${this.props.endDay.date.format(eventDialogTextDateFormat)}` : ''}`;
    }
}

const mapStateToProps = (state: AppState): ClaimSickLeaveEventDialogProps => ({
    startDay: state.calendar.calendarEvents.selection.interval.startDay,
    endDay: state.calendar.calendarEvents.selection.interval.endDay,
    userEmployee: state.organization.employees.employeesById.get(state.userInfo.employeeId)
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ClaimSickLeaveEventDialogDispatchProps => ({
    back: () => { dispatch(openEventDialog(EventDialogType.ClaimSickLeave)); },
    confirmSickLeave: (employeeId: string, startDate: Moment, endDate: Moment) => { 
        dispatch(startProgress());
        dispatch(confirmSickLeave(employeeId, startDate, endDate)); 
    },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const ConfirmSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(ConfirmSickLeaveEventDialogImpl);