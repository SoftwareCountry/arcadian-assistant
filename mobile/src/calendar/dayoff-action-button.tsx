import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarIntervalColor } from './styles';

interface DayoffActionButtonProps {
    interval: IntervalModel;
    // TODO: pass needed dispatch actions
}

export class DayoffActionButton extends Component<DayoffActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton title={this.title} borderColor={CalendarIntervalColor.dayoff} onPress={this.onDayoffAction} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Process Dayoff'
            : 'Review Dayoff';
    }

    public onDayoffAction = () => {
        // TODO: do something
    }
}

