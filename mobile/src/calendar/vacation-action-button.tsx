import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';

interface VacationActionButtonProps {
    allIntervals: ReadOnlyIntervalsModel;
    interval: IntervalModel;
    hide: boolean;
    request: () => void;
    edit: () => void;
}

export class VacationActionButton extends Component<VacationActionButtonProps> {
    public render() {
        const hideActionButton = this.hideCalendarAction();

        return (
            <CalendarActionButton 
                title={this.title} 
                borderColor={CalendarEventsColor.vacation} 
                onPress={this.onVacationAction} 
                hide={this.props.hide || hideActionButton} />
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

    private hideCalendarAction(): boolean {
        const { interval, allIntervals } = this.props;

        const hideWhenApproved = interval && interval.calendarEvent.isApproved;

        return hideWhenApproved;
    }
}

