import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';

interface ProcessDayoffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmStartDate: () => void;
}

interface ProcessDayoffEventDialogProps {
    startDay: DayModel;
}

class ProcessDayoffEventDialogImpl extends Component<ProcessDayoffEventDialogProps & ProcessDayoffEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
                    title={'Select date to process your dayoff'}
                    text={this.text}
                    icon={'dayoff'}
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onActionPress={this.onAcceptClick}
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
        return `Your dayoff starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}`;
    }
}

const mapStateToProps = (state: AppState): ProcessDayoffEventDialogProps => ({
    startDay: state.calendar.calendarEvents.selection.single.day
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ProcessDayoffEventDialogDispatchProps => ({
    cancelDialog: () => { dispatch(closeEventDialog()); },
    confirmStartDate: () => { dispatch(openEventDialog(EventDialogType.ConfirmDayoffStartDate)); }
});

export const ProcessDayoffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ProcessDayoffEventDialogImpl);