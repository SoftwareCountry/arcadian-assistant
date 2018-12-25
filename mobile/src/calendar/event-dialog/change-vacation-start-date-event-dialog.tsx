/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import {
    closeEventDialog,
    EventDialogActions,
    openEventDialog
} from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, ExtractedIntervals, ReadOnlyIntervalsModel } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import moment from 'moment';
import { Optional } from 'types';
import { isIntersectingAnotherVacation } from '../../reducers/calendar/calendar.model';

//============================================================================
interface ChangeVacationStartDateEventDialogDispatchProps {
    back: () => void;
    changeVacationEndDate: () => void;
    closeDialog: () => void;
}

//============================================================================
interface ChangeVacationStartDateEventDialogProps {
    selectedSingleDay: DayModel;
    selectedIntervals: Optional<ExtractedIntervals>;
    intervals: Optional<ReadOnlyIntervalsModel>;
}

//============================================================================
class ChangeVacationStartDateEventDialogImpl extends Component<ChangeVacationStartDateEventDialogProps & ChangeVacationStartDateEventDialogDispatchProps> {
    //----------------------------------------------------------------------------
    public render() {
        const { intervals, selectedIntervals, selectedSingleDay } = this.props;

        const disableAccept = !intervals || !selectedIntervals || !selectedIntervals.vacation
            || isIntersectingAnotherVacation(selectedSingleDay, undefined, intervals, [ selectedIntervals.vacation.calendarEvent ]);

        return <EventDialogBase
            title={'Change start date'}
            text={this.text}
            icon={'vacation'}
            cancelLabel={'Back'}
            acceptLabel={'Confirm'}
            onAcceptPress={this.confirmStartDateChange}
            onCancelPress={this.back}
            onClosePress={this.closeDialog}
            disableAccept={disableAccept}/>;
    }

    //----------------------------------------------------------------------------
    private back = () => {
        this.props.back();
    };

    //----------------------------------------------------------------------------
    private confirmStartDateChange = () => {
        const { changeVacationEndDate } = this.props;

        changeVacationEndDate();
    };

    //----------------------------------------------------------------------------
    private closeDialog = () => {
        this.props.closeDialog();
    };

    //----------------------------------------------------------------------------
    public get text(): string {
        if (!this.props.selectedIntervals) {
            return '';
        }

        const { selectedSingleDay } = this.props;

        const startDate = selectedSingleDay.date;

        return `Your vacation starts on ${startDate.format(eventDialogTextDateFormat)}`;
    }
}

//============================================================================
const mapStateToProps = (state: AppState): ChangeVacationStartDateEventDialogProps => ({
    selectedSingleDay: state.calendar && state.calendar.calendarEvents.selection.single.day ? state.calendar.calendarEvents.selection.single.day : {
        date: moment(), today: true, belongsToCurrentMonth: true,
    },
    selectedIntervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    intervals: state.calendar && state.calendar.calendarEvents.intervals ? state.calendar.calendarEvents.intervals : undefined,
});

//============================================================================
const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ChangeVacationStartDateEventDialogDispatchProps => ({
    back: () => {
        dispatch(openEventDialog(EventDialogType.EditVacation));
    },
    changeVacationEndDate: () => {
        dispatch(openEventDialog(EventDialogType.ChangeVacationEndDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

//============================================================================
export const ChangeVacationStartDateEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChangeVacationStartDateEventDialogImpl);
