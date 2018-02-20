import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarIntervalColor } from './styles';

interface VacationActionButtonProps {
    interval: IntervalModel;
    // TODO: pass needed dispatch actions
}

export class VacationActionButton extends Component<VacationActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton title={this.title} borderColor={CalendarIntervalColor.vacation} onPress={this.onVacationAction} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Request Vacation'
            : 'Review Vacation';
    }

    public onVacationAction = () => {
        // TODO: do something
    }
}

