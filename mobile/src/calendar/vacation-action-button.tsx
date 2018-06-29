import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';

interface VacationActionButtonProps {
    allIntervals: ReadOnlyIntervalsModel;
    interval: IntervalModel;
    disabled: boolean;
    request: () => void;
    edit: () => void;
}

export class VacationActionButton extends Component<VacationActionButtonProps> {
    public render() {
        const disableActionButton = this.disableCalendarAction();

        return (
            <CalendarActionButton 
                title={this.title} 
                borderColor={CalendarEventsColor.vacation} 
                onPress={this.onVacationAction} 
                disabled={this.props.disabled || disableActionButton} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Request Vacation'
            : 'Edit Vacation';
    }

    public onVacationAction = () => {
        if (!this.props.interval) {
            this.props.request();
        } else {
            this.props.edit();
        }
    }

    private disableCalendarAction(): boolean {
        const { interval, allIntervals } = this.props;

        const disableWhenCompleted = interval && interval.calendarEvent.isCompleted;

        return disableWhenCompleted;
    }
}

