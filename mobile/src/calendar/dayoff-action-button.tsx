import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';
import { CalendarEventType } from '../reducers/calendar/calendar-event.model';

//============================================================================
interface DayoffActionButtonProps {
    interval?: IntervalModel;
    disabled: boolean;
    process: () => void;
    edit: () => void;
}

//============================================================================
interface DayoffCase {
    disableCalendarButton: boolean;
    action: () => void;
}

//============================================================================
export class DayoffActionButton extends Component<DayoffActionButtonProps> {

    //----------------------------------------------------------------------------
    public render() {
        const disableCalendarAction = this.disableCalendarAction();

        return (
            <CalendarActionButton
                title={this.title}
                borderColor={CalendarEventsColor.dayoff}
                onPress={this.onDayoffAction}
                disabled={this.props.disabled || disableCalendarAction}/>
        );
    }

    //----------------------------------------------------------------------------
    public get title(): string {
        if (!this.props.interval) {
            return 'Dayoff / Workout';
        }

        return this.props.interval.calendarEvent.type === CalendarEventType.Dayoff ? 'Edit Dayoff' : 'Edit Workout';
    }

    //----------------------------------------------------------------------------
    public onDayoffAction = () => {
        const dayoffCase = this.dayoffCases();

        if (!dayoffCase) {
            return;
        }

        dayoffCase.action();
    };

    //----------------------------------------------------------------------------
    private disableCalendarAction() {
        const dayoffCase = this.dayoffCases();
        return !dayoffCase || dayoffCase.disableCalendarButton;
    }

    //----------------------------------------------------------------------------
    private dayoffCases(): DayoffCase | null {
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
