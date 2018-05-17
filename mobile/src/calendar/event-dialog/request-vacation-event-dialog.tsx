import React, { Component } from 'react';
import { View } from 'react-native';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';

interface ClaimVacationEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmStartDate: () => void;
}

interface ClaimVacationEventDialogProps {
    startDay: DayModel;
}

class ClaimVacationEventDialogImpl extends Component<ClaimVacationEventDialogProps & ClaimVacationEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase 
                    title={'Select date to Start your Vacation'} 
                    text={this.text}
                    icon={'vacation'} 
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onAcceptPress={this.onAcceptClick}
                    onCancelPress={this.onCancelClick}
                    onClosePress={this.onCloseClick} />;
    }

    private onCancelClick = () => {
        this.props.cancelDialog();
    }

    private onAcceptClick = () => {
        this.props.confirmStartDate();
    }

    private onCloseClick = () => {
        this.props.cancelDialog();
    }

    public get text(): string {
        return `Your vacation starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}`;
    }
}

const mapStateToProps = (state: AppState): ClaimVacationEventDialogProps => ({
    startDay: state.calendar.calendarEvents.selection.single.day
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ClaimVacationEventDialogDispatchProps => ({
    cancelDialog: () => { dispatch(closeEventDialog()); },
    confirmStartDate: () => { dispatch(openEventDialog(EventDialogType.ConfirmStartDateVacation)); }
});

export const RequestVacationEventDialog = connect(mapStateToProps, mapDispatchToProps)(ClaimVacationEventDialogImpl);