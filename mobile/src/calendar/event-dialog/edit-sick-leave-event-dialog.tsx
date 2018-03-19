import React, { Component } from 'react';
import { View } from 'react-native';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, IntervalModel, ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEventsType } from '../../reducers/calendar/calendar-events.model';

interface EditSickLeaveEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmStartDate: () => void;
}

interface EditSickLeaveEventDialogProps {
    editedIntervals: ExtractedIntervals;
}

class EditSickLeaveEventDialogImpl extends Component<EditSickLeaveEventDialogProps & EditSickLeaveEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
                    title={'Hey! Hope you feel better'}
                    text={this.text}
                    icon={'sick_leave'}
                    cancelLabel={'Prolong'}
                    acceptLabel={'Complete'}
                    onActionPress={this.acceptAction}
                    onCancelPress={this.cancelAction}
                    onClosePress={this.cancelAction} />;
    }

    private cancelAction = () => {
        this.props.cancelDialog();
    }

    private acceptAction = () => {
        this.props.confirmStartDate();
    }

    public get text(): string {
        const startDate = this.getSickLeaveStartDate();

        return `Your sick leave has started on ${startDate} and still is not completed.`;
    }

    private getSickLeaveStartDate(): string {
        if (!this.props.editedIntervals.sickleave) {
            return null;
        }

        return this.props.editedIntervals.sickleave.startDate.format(eventDialogTextDateFormat);
    }
}

const mapStateToProps = (state: AppState): EditSickLeaveEventDialogProps => ({
    editedIntervals: state.calendar.calendarEvents.intervalsBySingleDaySelection
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): EditSickLeaveEventDialogDispatchProps => ({
    cancelDialog: () => { dispatch(closeEventDialog()); },
    confirmStartDate: () => {  }
});

export const EditSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditSickLeaveEventDialogImpl);