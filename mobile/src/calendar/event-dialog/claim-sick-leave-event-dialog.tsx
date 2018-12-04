import React, { Component } from 'react';
import { View } from 'react-native';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import moment from 'moment';

interface ClaimSickLeaveEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmStartDate: () => void;
}

interface ClaimSickLeaveEventDialogProps {
    startDay: DayModel;
}

class ClaimSickLeaveEventDialogImpl extends Component<ClaimSickLeaveEventDialogProps & ClaimSickLeaveEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
                    title={'Select date to Start your Sick Leave'}
                    text={this.text}
                    icon={'sick_leave'}
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onAcceptPress={this.onAcceptClick}
                    onCancelPress={this.onCancelClick}
                    onClosePress={this.onCloseClick} />;
    }

    private onCancelClick = () => {
        this.props.cancelDialog();
    };

    private onAcceptClick = () => {
        this.props.confirmStartDate();
    };

    private onCloseClick = () => {
        this.props.cancelDialog();
    };

    public get text(): string {
        return `Your sick leave starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}`;
    }
}

const mapStateToProps = (state: AppState): ClaimSickLeaveEventDialogProps => ({
    startDay: state.calendar && state.calendar.calendarEvents.selection.single.day ?  state.calendar.calendarEvents.selection.single.day : {
        date: moment(), today: true, belongsToCurrentMonth: true,
    }
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ClaimSickLeaveEventDialogDispatchProps => ({
    cancelDialog: () => { dispatch(closeEventDialog()); },
    confirmStartDate: () => { dispatch(openEventDialog(EventDialogType.ConfirmStartDateSickLeave)); }
});

export const ClaimSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(ClaimSickLeaveEventDialogImpl);
