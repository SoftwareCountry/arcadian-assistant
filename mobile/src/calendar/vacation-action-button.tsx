import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel, DayModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';
import { Moment } from 'moment';

export interface VacationActionButtonProps {
    interval: IntervalModel;
    disabled: boolean;
    selectedDay: DayModel;
    claim: (selectedDay: Moment) => void;
    edit: () => void;
}

export class VacationActionButton extends Component<VacationActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton title={this.title} borderColor={CalendarEventsColor.vacation} onPress={this.onVacationAction} disabled={this.props.disabled} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Request Vacation'
            : 'Edit Vacation';
    }

    public onVacationAction = () => {
        if (!this.props.interval) {
            this.props.claim(this.props.selectedDay.date);
        } else {
            this.props.edit();
        }
    }
}

