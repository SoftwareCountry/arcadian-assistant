import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { Employee } from '../../reducers/organization/employee.model';
import { confirmSickLeave } from '../../reducers/calendar/sick-leave.action';
import moment, { Moment } from 'moment';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { Optional } from 'types';
import { getEmployee } from '../../utils/utils';

interface ClaimSickLeaveEventDialogDispatchProps {
    back: () => void;
    confirmSickLeave: (employeeId: string, startDate: Moment, endDate: Moment) => void;
    closeDialog: () => void;
}

interface ClaimSickLeaveEventDialogProps {
    startDay: DayModel;
    endDay: DayModel;
    userEmployee: Optional<Employee>;
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
    };

    private acceptAction = () => {
        if (!this.props.userEmployee) {
            return;
        }

        this.props.confirmSickLeave(this.props.userEmployee.employeeId, this.props.startDay.date, this.props.endDay.date);
    };

    private closeDialog = () => {
        this.props.closeDialog();
    };

    private get text(): string {
        return `Your sick leave starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}${this.props.endDay ? ` and completes on ${this.props.endDay.date.format(eventDialogTextDateFormat)}` : ''}`;
    }
}

const mapStateToProps = (state: AppState): ClaimSickLeaveEventDialogProps => {

    return {
        startDay: state.calendar && state.calendar.calendarEvents.selection.interval && state.calendar.calendarEvents.selection.interval.startDay ? state.calendar.calendarEvents.selection.interval.startDay : {
            date: moment(), today: true, belongsToCurrentMonth: true,
        },
        endDay: state.calendar && state.calendar.calendarEvents.selection.interval && state.calendar.calendarEvents.selection.interval.endDay ? state.calendar.calendarEvents.selection.interval.endDay : {
            date: moment(), today: true, belongsToCurrentMonth: true,
        },
        userEmployee: getEmployee(state),
    };
};

const mapDispatchToProps = (dispatch: Dispatch<Action>): ClaimSickLeaveEventDialogDispatchProps => ({
    back: () => { dispatch(openEventDialog(EventDialogType.ClaimSickLeave)); },
    confirmSickLeave: (employeeId: string, startDate: Moment, endDate: Moment) => { dispatch(confirmSickLeave(employeeId, startDate, endDate)); },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const ConfirmSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(ConfirmSickLeaveEventDialogImpl);
