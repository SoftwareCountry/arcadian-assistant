import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, IntervalType } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { Moment } from 'moment';
import { SwitchDayoffType } from './switch-dayoff-type';
import { HoursCreditCounter } from '../../reducers/calendar/days-counters.model';
import { confirmProcessDayoff } from '../../reducers/calendar/dayoff.action';
import { Employee } from '../../reducers/organization/employee.model';

interface ConfirmDayoffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmDayoff: (employeeId: string, date: Moment, isWorkout: boolean, intervalType: IntervalType ) => void;
    closeDialog: () => void;
}

interface ConfirmDayoffEventDialogProps {
    startDay: DayModel;
    hoursCredit: HoursCreditCounter;
    userEmployee: Employee;
}

interface ConfirmDayoffEventDialogState {
    selectedIntervalType: IntervalType;
}

class ConfirmDayoffEventDialogImpl extends Component<ConfirmDayoffEventDialogProps & ConfirmDayoffEventDialogDispatchProps, ConfirmDayoffEventDialogState> {
    private readonly intervalTypeToText = {
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
                    onActionPress={this.onAcceptClick}
                    onCancelPress={this.onCancelClick}
                    onClosePress={this.onCloseClick}>
                        <SwitchDayoffType onIntervalTypeSelected={this.onDayoffTypeSelected} isWorkout={this.props.hoursCredit.isWorkout} />
                </EventDialogBase>;
    }

    private onCancelClick = () => {
        this.props.cancelDialog();
    }

    private onAcceptClick = () => {
        const { startDay, hoursCredit, confirmDayoff, userEmployee } = this.props;

        confirmDayoff(
            userEmployee.employeeId,
            startDay.date,
            hoursCredit.isWorkout,
            this.state.selectedIntervalType);
    }

    private onCloseClick = () => {
        this.props.closeDialog();
    }

    private onDayoffTypeSelected = (selectedType: IntervalType) => {
        this.setState({ selectedIntervalType: selectedType });
    }

    public get text(): string {
        const startDate = this.props.startDay.date.format(eventDialogTextDateFormat);
        const intervalTypeToText = this.intervalTypeToText[this.state.selectedIntervalType];

        return `Your dayoff starts on ${startDate} and continues for ${intervalTypeToText}`;
    }
}

const mapStateToProps = (state: AppState): ConfirmDayoffEventDialogProps => ({
    startDay: state.calendar.calendarEvents.selection.single.day,
    hoursCredit: state.calendar.daysCounters.hoursCredit,
    userEmployee: state.userInfo.employee
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ConfirmDayoffEventDialogDispatchProps => ({
    cancelDialog: () => { dispatch(openEventDialog(EventDialogType.ProcessDayoff)); },
    confirmDayoff: (
        employeeId: string,
        date: Moment,
        isWorkout: boolean,
        intervalType: IntervalType
    ) => { dispatch(confirmProcessDayoff(employeeId, date, isWorkout, intervalType)); },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const ConfirmDayoffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ConfirmDayoffEventDialogImpl);