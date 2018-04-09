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

export class DayoffActionButtonImpl extends Component<DayoffActionButtonProps> {
    public render() {
        return (
            <CalendarActionButton
                title={this.title}
                borderColor={CalendarEventsColor.dayoff}
                onPress={this.onDayoffAction}
                disabled={this.props.disabled} />
        );
    }

    public get title() : string {
        return !this.props.interval
            ? 'Process Dayoff'
            : 'Edit Dayoff';
    }

    public onDayoffAction = () => {
        if (!this.props.interval) {
            this.props.process();
        }
    }
}

const mapStateToProps = (state: AppState, ownProps: DayoffActionButtonOwnProps): DayoffActionButtonProps => ({
    hoursCredit: state.calendar.daysCounters.hoursCredit,
    interval: ownProps.interval,
    disabled: ownProps.disabled,
    process: ownProps.process,
    edit: ownProps.edit
});

export const DayoffActionButton = connect(mapStateToProps)(DayoffActionButtonImpl);