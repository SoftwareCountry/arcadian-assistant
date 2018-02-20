import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarIntervalColor } from './styles';

interface SickLeaveActionButtonProps {
    interval: IntervalModel;
    // TODO: pass needed dispatch actions
}

export class SickLeaveActionButton extends Component<SickLeaveActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton title={this.title} borderColor={CalendarIntervalColor.sickLeave} onPress={this.onSickLeaveAction} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Claim Sick Leave'
            : 'Review Sick Leave';
    }

    public onSickLeaveAction = () => {
        // TODO: do something
    }
}

