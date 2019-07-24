import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState, getEmployee } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, IntervalType } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { Moment } from 'moment';
import { SelectorDayOffDuration } from './selector-dayoff-duration';
import { confirmProcessDayOff } from '../../reducers/calendar/dayoff.action';
import { Employee } from '../../reducers/organization/employee.model';
import { HoursCreditType } from '../../reducers/calendar/days-counters.model';
import { Optional } from 'types';

//============================================================================
interface ConfirmDayOffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmDayOff: (employeeId: string, date: Moment, isWorkout: boolean, intervalType: IntervalType) => void;
    closeDialog: () => void;
}

//============================================================================
interface ConfirmDayOffEventDialogProps {
    startDay: Optional<DayModel>;
    isWorkout: boolean;
    userEmployee: Optional<Employee>;
}

//============================================================================
interface ConfirmDayOffEventDialogState {
    selectedIntervalType: IntervalType;
}

//============================================================================
class ConfirmDayOffEventDialogImpl extends Component<ConfirmDayOffEventDialogProps & ConfirmDayOffEventDialogDispatchProps, ConfirmDayOffEventDialogState> {

    //----------------------------------------------------------------------------
    private readonly intervalTypeToText: { [key: string]: string } = {
        [IntervalType.IntervalFullBoundary]: 'a full day',
        [IntervalType.IntervalLeftBoundary]: 'a first half day',
        [IntervalType.IntervalRightBoundary]: 'a second half day',
    };

    //----------------------------------------------------------------------------
    constructor(props: ConfirmDayOffEventDialogProps & ConfirmDayOffEventDialogDispatchProps) {
        super(props);
        this.state = {
            selectedIntervalType: IntervalType.IntervalFullBoundary
        };
    }

    //----------------------------------------------------------------------------
    public render() {
        return <EventDialogBase
            title={'Select your duration'}
            text={this.text}
            icon={'day_off'}
            cancelLabel={'Back'}
            acceptLabel={'Confirm'}
            onAcceptPress={this.onAcceptClick}
            onCancelPress={this.onCancelClick}
            onClosePress={this.onCloseClick}>
            <SelectorDayOffDuration onIntervalTypeSelected={this.onDayOffTypeSelected}
                                    isWorkout={this.props.isWorkout}/>
        </EventDialogBase>;
    }

    //----------------------------------------------------------------------------
    private onCancelClick = () => {
        this.props.cancelDialog();
    };

    //----------------------------------------------------------------------------
    private onAcceptClick = () => {
        const { startDay, isWorkout, confirmDayOff, userEmployee } = this.props;

        if (!userEmployee || !startDay) {
            return;
        }

        confirmDayOff(
            userEmployee.employeeId,
            startDay.date,
            isWorkout,
            this.state.selectedIntervalType);
    };

    //----------------------------------------------------------------------------
    private onCloseClick = () => {
        this.props.closeDialog();
    };

    //----------------------------------------------------------------------------
    private onDayOffTypeSelected = (selectedType: IntervalType) => {
        this.setState({ selectedIntervalType: selectedType });
    };

    //----------------------------------------------------------------------------
    public get text(): string {
        if (!this.props.startDay) {
            return '';
        }

        const startDate = this.props.startDay.date.format(eventDialogTextDateFormat);
        const intervalTypeToText = this.intervalTypeToText[this.state.selectedIntervalType];
        const dateType = this.props.isWorkout ? 'workout' : 'day off';

        return `Your ${dateType} starts on ${startDate} and continues for ${intervalTypeToText}`;
    }
}

//----------------------------------------------------------------------------
const mapStateToProps = (state: AppState): ConfirmDayOffEventDialogProps => ({
    startDay: state.calendar ? state.calendar.calendarEvents.selection.single.day : undefined,
    isWorkout: state.calendar ? state.calendar.eventDialog.chosenHoursCreditType === HoursCreditType.Workout : false,
    userEmployee: getEmployee(state),
});

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): ConfirmDayOffEventDialogDispatchProps => ({
    cancelDialog: () => {
        dispatch(openEventDialog(EventDialogType.ChooseTypeDayOff));
    },
    confirmDayOff: (
        employeeId: string,
        date: Moment,
        isWorkout: boolean,
        intervalType: IntervalType
    ) => {
        dispatch(confirmProcessDayOff(employeeId, date, isWorkout, intervalType));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

export const ConfirmDayOffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ConfirmDayOffEventDialogImpl);
