import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState, getEmployee } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { cancelDayOff } from '../../reducers/calendar/dayoff.action';
import { CalendarEvent, CalendarEventType } from '../../reducers/calendar/calendar-event.model';
import { Employee } from '../../reducers/organization/employee.model';
import { Optional } from 'types';

//============================================================================
interface EditDayOffEventDialogDispatchProps {
    closeDialog: () => void;
    cancelProcessDayOff: (employeeId: string, calendarEvent: CalendarEvent) => void;
}

//============================================================================
interface EditDayOffEventDialogProps {
    selectedIntervals: Optional<ExtractedIntervals>;
    userEmployee: Optional<Employee>;
}

//============================================================================
class EditDayOffEventDialogImpl extends Component<EditDayOffEventDialogProps & EditDayOffEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
            title={`Cancel your ${this.getEventTitle()}`}
            text={this.text}
            icon={'day_off'}
            cancelLabel={'Back'}
            acceptLabel={'Cancel'}
            onAcceptPress={this.onAcceptClick}
            onCancelPress={this.onCancelClick}
            onClosePress={this.onCloseClick}/>;
    }

    //----------------------------------------------------------------------------
    private getEventTitle = (): string => {
        if (!this.props.selectedIntervals || !this.props.selectedIntervals || !this.props.selectedIntervals.dayOff) {
            console.error('Unexpected type for Day off dialog');
            return '';
        }

        return this.props.selectedIntervals.dayOff.calendarEvent.type === CalendarEventType.DayOff ? 'day off' : 'workout';
    };

    //----------------------------------------------------------------------------
    private onCancelClick = () => {
        this.props.closeDialog();
    };

    //----------------------------------------------------------------------------
    private onAcceptClick = () => {
        if (!this.props.selectedIntervals || !this.props.selectedIntervals.dayOff || !this.props.userEmployee) {
            return;
        }

        this.props.cancelProcessDayOff(this.props.userEmployee.employeeId, this.props.selectedIntervals.dayOff.calendarEvent);
    };

    //----------------------------------------------------------------------------
    private onCloseClick = () => {
        this.props.closeDialog();
    };

    //----------------------------------------------------------------------------
    public get text(): string {
        if (!this.props.selectedIntervals || !this.props.selectedIntervals.dayOff) {
            return '';
        }
        const date = this.props.selectedIntervals.dayOff.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);

        return `Your ${this.getEventTitle()} starts on ${date}`;
    }
}

//----------------------------------------------------------------------------
const mapStateToProps = (state: AppState): EditDayOffEventDialogProps => ({
    selectedIntervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    userEmployee: getEmployee(state),
});

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): EditDayOffEventDialogDispatchProps => ({
    closeDialog: () => {
        dispatch(closeEventDialog());
    },
    cancelProcessDayOff: (employeeId: string, calendarEvent: CalendarEvent) => {
        dispatch(cancelDayOff(employeeId, calendarEvent));
    }
});

//----------------------------------------------------------------------------
export const EditDayOffEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditDayOffEventDialogImpl);
