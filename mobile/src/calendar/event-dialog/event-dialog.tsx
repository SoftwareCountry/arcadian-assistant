import React, { Component } from 'react';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';
import { ClaimSickLeaveEventDialog } from './claim-sick-leave-event-dialog';
import { ConfirmSickLeaveEventDialog } from './confirm-sick-leave-event-dialog';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { EditSickLeaveEventDialog } from './edit-sick-leave-event-dialog';
import { ProlongSickLeaveEventDialog } from './prolong-sick-leave-event-dialog';
import { RequestVacationEventDialog } from './request-vacation-event-dialog';
import { ConfirmVacationEventDialog } from './confirm-vaction-event-dialog';
import { EditVacationEventDialog } from './edit-vacation-event-dialog';

interface EventDialogProps {
    dialogType: EventDialogType;
}

export class EventDialog extends Component<EventDialogProps> {
    public render() {
        return this.getEventDialog();
    }

    private getEventDialog() {
        if (!this.props.dialogType) {
            return null;
        }

        switch (this.props.dialogType) {
            case EventDialogType.ClaimSickLeave:
                return <ClaimSickLeaveEventDialog />;
            case EventDialogType.ConfirmStartDateSickLeave:
                return <ConfirmSickLeaveEventDialog />;
            case EventDialogType.EditSickLeave:
                return <EditSickLeaveEventDialog />;
            case EventDialogType.ProlongSickLeave:
                return <ProlongSickLeaveEventDialog />;

            case EventDialogType.RequestVacaltion:
                return <RequestVacationEventDialog />;
            case EventDialogType.ConfirmStartDateVacation:
                return <ConfirmVacationEventDialog />;
            case EventDialogType.EditVacaltion:
                return <EditVacationEventDialog />;
            default:
                throw new Error(`There isn't event dialog implementation for ${this.props.dialogType}`);
        }
    }
}