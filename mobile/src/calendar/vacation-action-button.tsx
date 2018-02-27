import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';

interface VacationActionButtonProps {
    interval: IntervalModel;
    // TODO: pass needed dispatch actions
}

export class VacationActionButton extends Component<VacationActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton title={this.title} borderColor={CalendarEventsColor.vacation} onPress={this.onVacationAction} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Request Vacation'
            : 'Edit Vacation';
    }

    public onVacationAction = () => {
        // TODO: do something
    }
}

