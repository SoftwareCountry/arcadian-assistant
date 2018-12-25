import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, IntervalType } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { Moment } from 'moment';
import { SelectorDayoffDuration } from './selector-dayoff-duration';
import { confirmProcessDayoff } from '../../reducers/calendar/dayoff.action';
import { Employee } from '../../reducers/organization/employee.model';
import { HoursCreditType } from '../../reducers/calendar/days-counters.model';
import { getEmployee } from '../../reducers/app.reducer';
import { Optional } from 'types';

interface ConfirmDayoffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmDayoff: (employeeId: string, date: Moment, isWorkout: boolean, intervalType: IntervalType) => void;
    closeDialog: () => void;
}

interface ConfirmDayoffEventDialogProps {
    startDay: Optional<DayModel>;
    isWorkout: boolean;
    userEmployee: Optional<Employee>;
}

interface ConfirmDayoffEventDialogState {
    selectedIntervalType: IntervalType;
}

class ConfirmDayoffEventDialogImpl extends Component<ConfirmDayoffEventDialogProps & ConfirmDayoffEventDialogDispatchProps, ConfirmDayoffEventDialogState> {
    private readonly intervalTypeToText: { [key: string]: string } = {
        [IntervalType.IntervalLeftBoundary]: 'a first half day',
        [IntervalType.IntervalRightBoundary]: 'a second half day',
        [IntervalType.IntervalFullBoundary]: 'a full day'
    };

    constructor(props: ConfirmDayoffEventDialogProps & ConfirmDayoffEventDialogDispatchProps) {
        super(props);
        this.state = {
            selectedIntervalType: IntervalType.IntervalLeftBoundary
        };
    }

    public render() {
        return <EventDialogBase
            title={'Select your duration'}
            text={this.text}
            icon={'dayoff'}
            cancelLabel={'Back'}
            acceptLabel={'Confirm'}
            onAcceptPress={this.onAcceptClick}
            onCancelPress={this.onCancelClick}
            onClosePress={this.onCloseClick}>
            <SelectorDayoffDuration onIntervalTypeSelected={this.onDayoffTypeSelected}
                                    isWorkout={this.props.isWorkout}/>
        </EventDialogBase>;
    }

    private onCancelClick = () => {
        this.props.cancelDialog();
    };

    private onAcceptClick = () => {
        const { startDay, isWorkout, confirmDayoff, userEmployee } = this.props;

        if (!userEmployee || !startDay) {
            return;
        }

        confirmDayoff(
            userEmployee.employeeId,
            startDay.date,
            isWorkout,
            this.state.selectedIntervalType);
    };

    private onCloseClick = () => {
        this.props.closeDialog();
    };

    private onDayoffTypeSelected = (selectedType: IntervalType) => {
        this.setState({ selectedIntervalType: selectedType });
    };

    public get text(): string {
        if (!this.props.startDay) {
            return '';
        }

        const startDate = this.props.startDay.date.format(eventDialogTextDateFormat);
        const intervalTypeToText = this.intervalTypeToText[this.state.selectedIntervalType];
        const dateType = this.props.isWorkout ? 'workout' : 'dayoff';

        return `Your ${dateType} starts on ${startDate} and continues for ${intervalTypeToText}`;
    }
}

const mapStateToProps = (state: AppState): ConfirmDayoffEventDialogProps => ({
    startDay: state.calendar ? state.calendar.calendarEvents.selection.single.day : undefined,
    isWorkout: state.calendar ? state.calendar.eventDialog.chosenHoursCreditType === HoursCreditType.Workout : false,
    userEmployee: getEmployee(state),
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): ConfirmDayoffEventDialogDispatchProps => ({
    cancelDialog: () => {
        dispatch(openEventDialog(EventDialogType.ChooseTypeDayoff));
    },
    confirmDayoff: (
        employeeId: string,
        date: Moment,
        isWorkout: boolean,
        intervalType: IntervalType
    ) => {
        dispatch(confirmProcessDayoff(employeeId, date, isWorkout, intervalType));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

export const ConfirmDayoffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ConfirmDayoffEventDialogImpl);
