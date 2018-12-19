/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals, IntervalSelection, ReadOnlyIntervalsModel } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { confirmVacationChange } from '../../reducers/calendar/vacation.action';
import { Employee } from '../../reducers/organization/employee.model';
import { Moment } from 'moment';
import { getEmployee, isIntersectingAnotherVacation } from '../../utils/utils';
import { Optional } from 'types';

//============================================================================
interface ChangeVacationEndDateEventDialogDispatchProps {
    back: () => void;
    confirmChangeVacation: (
        employeeId: string,
        calendarEvent: CalendarEvent,
        startDate: Moment,
        endDate: Moment
    ) => void;
    closeDialog: () => void;
}

//============================================================================
interface ChangeVacationEndDateEventDialogProps {
    userEmployee: Optional<Employee>;
    interval: Optional<IntervalSelection>;
    selectedIntervals: Optional<ExtractedIntervals>;
    intervals: Optional<ReadOnlyIntervalsModel>;
}

//============================================================================
class ChangeVacationEndDateEventDialogImpl extends Component<ChangeVacationEndDateEventDialogProps & ChangeVacationEndDateEventDialogDispatchProps> {
    //----------------------------------------------------------------------------
    public render() {
        const { interval, intervals, selectedIntervals } = this.props;

        const disableAccept = !interval || !interval.endDay || !intervals || !selectedIntervals || !selectedIntervals.vacation
            || isIntersectingAnotherVacation(interval.startDay, interval.endDay, intervals, [selectedIntervals.vacation.calendarEvent]);

        return <EventDialogBase
            title={'Change end date'}
            text={this.text}
            icon={'vacation'}
            cancelLabel={'Back'}
            acceptLabel={'Confirm'}
            onAcceptPress={this.confirmEndDateChange}
            onCancelPress={this.back}
            onClosePress={this.closeDialog}
            disableAccept={disableAccept}/>;
    }

    //----------------------------------------------------------------------------
    private back = () => {
        this.props.back();
    };

    //----------------------------------------------------------------------------
    private confirmEndDateChange = () => {
        const { interval, userEmployee, selectedIntervals } = this.props;

        if (!userEmployee || !interval || !selectedIntervals || !selectedIntervals.vacation || !interval.startDay || !interval.endDay) {
            return;
        }

        this.props.confirmChangeVacation(
            userEmployee.employeeId,
            selectedIntervals.vacation.calendarEvent,
            interval.startDay.date,
            interval.endDay.date);
    };

    //----------------------------------------------------------------------------
    private closeDialog = () => {
        this.props.closeDialog();
    };

    //----------------------------------------------------------------------------
    public get text(): string {
        const { interval } = this.props;

        const endDate = interval && interval.endDay
            ? interval.endDay.date.format(eventDialogTextDateFormat)
            : '';

        return `Your vacation completes on ${endDate}`;
    }
}

//============================================================================
const mapStateToProps = (state: AppState): ChangeVacationEndDateEventDialogProps => ({
    userEmployee: getEmployee(state),
    interval: state.calendar ? state.calendar.calendarEvents.selection.interval : undefined,
    selectedIntervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    intervals: state.calendar && state.calendar.calendarEvents.intervals ? state.calendar.calendarEvents.intervals : undefined,
});

//============================================================================
const mapDispatchToProps = (dispatch: Dispatch<Action>): ChangeVacationEndDateEventDialogDispatchProps => ({
    back: () => {
        dispatch(openEventDialog(EventDialogType.ChangeVacationStartDate));
    },
    confirmChangeVacation: (
        employeeId: string,
        calendarEvent: CalendarEvent,
        startDate: Moment,
        endDate: Moment
    ) => {
        dispatch(confirmVacationChange(employeeId, calendarEvent, startDate, endDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

//============================================================================
export const ChangeVacationEndDateEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChangeVacationEndDateEventDialogImpl);
