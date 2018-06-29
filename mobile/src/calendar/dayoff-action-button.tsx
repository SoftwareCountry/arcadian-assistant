import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';
import { HoursCreditCounter } from '../reducers/calendar/days-counters.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

interface DayoffActionButtonProps {
    interval: IntervalModel;
    disabled: boolean;
    process: () => void;
    edit: () => void;
}

interface DayoffCase {
    disableCalendatButton: boolean;
    action: () => void;
}

export class DayoffActionButton extends Component<DayoffActionButtonProps> {
    public render() {
        const disableCalendarAction = this.disableCalendarAction();

        return (
            <CalendarActionButton
                title={this.title}
                borderColor={CalendarEventsColor.dayoff}
                onPress={this.onDayoffAction}
                disabled={this.props.disabled || disableCalendarAction} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Process Dayoff'
            : 'Edit Dayoff';
    }

    public onDayoffAction = () => {
        const dayoffCase = this.dayoffCases();

        if (!dayoffCase) {
            return;
        }

        dayoffCase.action();
    }

    private disableCalendarAction() {
        const dayoffCase = this.dayoffCases();
        return !dayoffCase || dayoffCase.disableCalendatButton;
    }

    private dayoffCases(): DayoffCase | null {
        const { interval, process, edit } = this.props;

        if (!interval) {
            return { disableCalendatButton: false, action: process };
        }

        if (interval && !interval.calendarEvent.isApproved) {
            return { disableCalendatButton: false, action: edit };
        }

        return null;
    }
}