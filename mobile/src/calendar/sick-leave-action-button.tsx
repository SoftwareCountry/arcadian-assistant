import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';

interface SickLeaveActionButtonProps {
    allIntervals: ReadOnlyIntervalsModel;
    interval: IntervalModel;
    disabled: boolean;
    claim: () => void;
    edit: () => void;
    cancel: () => void;
}

export class SickLeaveActionButton extends Component<SickLeaveActionButtonProps> {
    public render() {
        const { interval, allIntervals } = this.props;

        const disableCalendarAction = this.disableCalendarAction();

        return (
            <CalendarActionButton 
                title={this.title} 
                borderColor={CalendarEventsColor.sickLeave} 
                onPress={this.onSickLeaveAction} 
                disabled={this.props.disabled || disableCalendarAction} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Claim Sick Leave'
            : 'Edit Sick Leave';
    }

    public onSickLeaveAction = () => {
        const { interval, claim, edit, cancel } = this.props;

        if (!interval) {
            claim();
        } else if (interval.calendarEvent.isApproved) {
            edit();
        } else {
            cancel();
        }
    }

    private disableCalendarAction(): boolean {
        const { interval, allIntervals } = this.props;

        const disableWhenRequested = !interval
            && allIntervals
            && allIntervals.metadata.calendarEvents.some(x => x.isSickLeave && x.isRequested);

        const disableWhenCompleted = interval && interval.calendarEvent.isCompleted;

        return disableWhenRequested || disableWhenCompleted;
    }
}

