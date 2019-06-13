import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';
import { CalendarEventType } from '../reducers/calendar/calendar-event.model';

//============================================================================
interface DayOffActionButtonProps {
    interval?: IntervalModel;
    disabled: boolean;
    process: () => void;
    edit: () => void;
}

//============================================================================
interface DayOffCase {
    disableCalendarButton: boolean;
    action: () => void;
}

//============================================================================
export class DayOffActionButton extends Component<DayOffActionButtonProps> {

    //----------------------------------------------------------------------------
    public render() {
        const disableCalendarAction = this.disableCalendarAction();

        return (
            <CalendarActionButton
                title={this.title}
                borderColor={CalendarEventsColor.dayOff}
                onPress={this.onDayOffAction}
                disabled={this.props.disabled || disableCalendarAction}/>
        );
    }

    //----------------------------------------------------------------------------
    public get title(): string {
        if (!this.props.interval) {
            return 'Day off / Workout';
        }

        return this.props.interval.calendarEvent.type === CalendarEventType.DayOff ? 'Edit Day off' : 'Edit Workout';
    }

    //----------------------------------------------------------------------------
    public onDayOffAction = () => {
        const dayOffCase = this.dayOffCases();

        if (!dayOffCase) {
            return;
        }

        dayOffCase.action();
    };

    //----------------------------------------------------------------------------
    private disableCalendarAction() {
        const dayOffCase = this.dayOffCases();
        return !dayOffCase || dayOffCase.disableCalendarButton;
    }

    //----------------------------------------------------------------------------
    private dayOffCases(): DayOffCase | null {
        const { interval, process, edit } = this.props;

        if (!interval) {
            return { disableCalendarButton: false, action: process };
        }

        if (interval && !interval.calendarEvent.isApproved) {
            return { disableCalendarButton: false, action: edit };
        }

        return null;
    }
}
