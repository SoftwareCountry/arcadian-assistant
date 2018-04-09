import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';
import { HoursCreditCounter } from '../reducers/calendar/days-counters.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

interface DayoffActionButtonMapToStateProps {
    hoursCredit: HoursCreditCounter;
}

interface DayoffActionButtonOwnProps {
    interval: IntervalModel;
    disabled: boolean;
    process: () => void;
    edit: () => void;
}

type DayoffActionButtonProps = DayoffActionButtonOwnProps & DayoffActionButtonMapToStateProps;

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
        const { interval, process, edit } = this.props;

        if (!interval) {
            process();
            return;
        }

        if (interval && interval.calendarEvent.isApproved) {
            edit();
        }
    }

    private disableCalendarAction() {
        const { interval } = this.props;

        if (!interval) {
            return false;
        }

        if (interval && interval.calendarEvent.isApproved) {
            return false;
        }

        return true;
    }
}