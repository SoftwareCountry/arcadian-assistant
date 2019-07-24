import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import {
    closeEventDialog,
    EventDialogActions,
    openEventDialog
} from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import moment from 'moment';

interface ProcessDayOffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmStartDate: () => void;
}

interface ProcessDayOffEventDialogProps {
    startDay: DayModel;
}

class ProcessDayOffEventDialogImpl extends Component<ProcessDayOffEventDialogProps & ProcessDayOffEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
            title={'Select date to process your day off/workout'}
            text={this.text}
            icon={'day_off'}
            cancelLabel={'Back'}
            acceptLabel={'Confirm'}
            onAcceptPress={this.onAcceptClick}
            onCancelPress={this.onCancelClick}
            onClosePress={this.onCloseClick}/>;
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
        return `Your day off/workout starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}`;
    }
}

const mapStateToProps = (state: AppState): ProcessDayOffEventDialogProps => ({
    startDay: state.calendar && state.calendar.calendarEvents.selection.single.day ? state.calendar.calendarEvents.selection.single.day : {
        date: moment(), today: true, belongsToCurrentMonth: true,
    }
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ProcessDayOffEventDialogDispatchProps => ({
    cancelDialog: () => {
        dispatch(closeEventDialog());
    },
    confirmStartDate: () => {
        dispatch(openEventDialog(EventDialogType.ChooseTypeDayOff));
    }
});

export const ProcessDayOffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ProcessDayOffEventDialogImpl);
