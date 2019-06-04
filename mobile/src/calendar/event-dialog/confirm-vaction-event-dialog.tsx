/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState, getEmployee, getEndDay, getStartDay } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import {
    DayModel,
    isIntersectingAnotherVacation,
    ReadOnlyIntervalsModel
} from '../../reducers/calendar/calendar.model';
import { Employee } from '../../reducers/organization/employee.model';
import { confirmVacation } from '../../reducers/calendar/vacation.action';
import { Moment } from 'moment';
import { Optional } from 'types';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';

//============================================================================
interface ClaimVacationEventDialogDispatchProps {
    back: () => void;
    confirmVacation: (employeeId: string, startDate: Moment, endDate: Moment) => void;
    closeDialog: () => void;
}

//============================================================================
interface ClaimVacationEventDialogProps {
    startDay: DayModel;
    endDay: DayModel;
    userEmployee: Optional<Employee>;
    intervals: Optional<ReadOnlyIntervalsModel>;
}

//============================================================================
class ConfirmVacationEventDialogImpl extends Component<ClaimVacationEventDialogProps & ClaimVacationEventDialogDispatchProps> {
    //----------------------------------------------------------------------------
    public render() {
        const { startDay, endDay, intervals } = this.props;

        const disableAccept = !endDay || !intervals
            || isIntersectingAnotherVacation(startDay, endDay, intervals);

        return <EventDialogBase
            title={'Select date to Complete your Vacation'}
            text={this.text}
            icon={'vacation'}
            cancelLabel={'Back'}
            acceptLabel={'Confirm'}
            onAcceptPress={this.acceptAction}
            onCancelPress={this.cancelAction}
            onClosePress={this.closeDialog}
            disableAccept={disableAccept}/>;
    }

    //----------------------------------------------------------------------------
    private cancelAction = () => {
        this.props.back();
    };

    //----------------------------------------------------------------------------
    private acceptAction = () => {
        if (!this.props.userEmployee) {
            return;
        }

        this.props.confirmVacation(this.props.userEmployee.employeeId, this.props.startDay.date, this.props.endDay.date);
    };

    //----------------------------------------------------------------------------
    private closeDialog = () => {
        this.props.closeDialog();
    };

    //----------------------------------------------------------------------------
    private get text(): string {
        return `Your vacation starts on ${this.props.startDay.date.format(eventDialogTextDateFormat)}${this.props.endDay ? ` and completes on ${this.props.endDay.date.format(eventDialogTextDateFormat)}` : ''}`;
    }
}

//============================================================================
const mapStateToProps = (state: AppState): ClaimVacationEventDialogProps => ({
    startDay: getStartDay(state),
    endDay: getEndDay(state),
    userEmployee: getEmployee(state),
    intervals: state.calendar && state.calendar.calendarEvents.intervals ? state.calendar.calendarEvents.intervals : undefined,
});

//============================================================================
const mapDispatchToProps = (dispatch: Dispatch<Action>): ClaimVacationEventDialogDispatchProps => ({
    back: () => {
        dispatch(openEventDialog(EventDialogType.RequestVacation));
    },
    confirmVacation: (employeeId: string, startDate: Moment, endDate: Moment) => {
        dispatch(confirmVacation(employeeId, startDate, endDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

//============================================================================
export const ConfirmVacationEventDialog = connect(mapStateToProps, mapDispatchToProps)(ConfirmVacationEventDialogImpl);
