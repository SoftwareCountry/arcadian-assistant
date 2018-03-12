import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';

interface DayoffActionButtonProps {
    interval: IntervalModel;
    disabled: boolean;
    // TODO: pass needed dispatch actions
}

export class DayoffActionButton extends Component<DayoffActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton title={this.title} borderColor={CalendarEventsColor.dayoff} onPress={this.onDayoffAction} disabled={this.props.disabled} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Process Dayoff'
            : 'Edit Dayoff';
    }

    public onDayoffAction = () => {
        // TODO: do something
    }
}

