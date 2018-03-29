import React, { Component } from 'react';
import { View } from 'react-native';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { Employee } from '../../reducers/organization/employee.model';
import { confirmVacation } from '../../reducers/calendar/vacation.action';
import { Moment } from 'moment';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';

interface ClaimVacationEventDialogDispatchProps {
    back: () => void;
    confirmVacation: (employeeId: string, startDate: Moment, endDate: Moment) => void;
    closeDialog: () => void;
}

interface ClaimVacationEventDialogProps {
    startDay: DayModel;
    endDay: DayModel;
    userEmployee: Employee;
}

class ConfirmVacationEventDialogImpl extends Component<ClaimVacationEventDialogProps & ClaimVacationEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase 
                    title={'Select date to Complete your Vacation'} 
                    text={this.text}
                    icon={'vacation'} 
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onActionPress={this.acceptAction}
                    onCancelPress={this.cancelAction}
                    onClosePress={this.closeDialog}
                    disableAccept={!this.props.endDay} />;
    }

    private cancelAction = () => {
        this.props.back();
    }

    private acceptAction = () => {
        this.props.confirmVacation(this.props.userEmployee.employeeId, this.props.startDay.date, this.props.endDay.date);
    }

    private closeDialog = () => {
        this.props.closeDialog();
    }

    private get text(): string {
        return `Your vacation starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}${this.props.endDay ? ` and completes on ${this.props.endDay.date.format(eventDialogTextDateFormat)}` : ''}`;
    }
}

const mapStateToProps = (state: AppState): ClaimVacationEventDialogProps => ({
    startDay: state.calendar.calendarEvents.selection.interval.startDay,
    endDay: state.calendar.calendarEvents.selection.interval.endDay,
    userEmployee: state.userInfo.employee
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ClaimVacationEventDialogDispatchProps => ({
    back: () => { dispatch(openEventDialog(EventDialogType.RequestVacaltion)); },
    confirmVacation: (employeeId: string, startDate: Moment, endDate: Moment) => { dispatch(confirmVacation(employeeId, startDate, endDate)); },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const ConfirmVacationEventDialog = connect(mapStateToProps, mapDispatchToProps)(ConfirmVacationEventDialogImpl);