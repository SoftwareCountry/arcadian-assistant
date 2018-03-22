import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';

interface SickLeaveActionButtonProps {
    interval: IntervalModel;
    disabled: boolean;
    claim: () => void;
    edit: () => void;
}

export class SickLeaveActionButton extends Component<SickLeaveActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton title={this.title} borderColor={CalendarEventsColor.sickLeave} onPress={this.onSickLeaveAction} disabled={this.props.disabled} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Claim Sick Leave'
            : 'Edit Sick Leave';
    }

    public onSickLeaveAction = () => {
        if (!this.props.interval) {
            this.props.claim();
        } else {
            this.props.edit();
        }
    }
}

