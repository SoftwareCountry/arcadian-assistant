import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { cancelVacation } from '../../reducers/calendar/vacation.action';
import { Employee } from '../../reducers/organization/employee.model';
import { getEmployee } from '../../reducers/app.reducer';
import { Optional } from 'types';

interface EditVacationEventDialogDispatchProps {
    cancelVacation: (employeeId: string, calendarEvent: CalendarEvent) => void;
    changeVacationStartDate: () => void;
    closeDialog: () => void;
}

interface EditVacationEventDialogProps {
    intervals: Optional<ExtractedIntervals>;
    userEmployee: Optional<Employee>;
}

class EditVacationEventDialogImpl extends Component<EditVacationEventDialogProps & EditVacationEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
            title={'Cancel or change your vacation'}
            text={this.text}
            icon={'vacation'}
            cancelLabel={'Cancel'}
            acceptLabel={'Change'}
            onAcceptPress={this.onAcceptPress}
            onCancelPress={this.onCancelPress}
            onClosePress={this.onCloseDialog}/>;
    }

    private onCancelPress = () => {
        if (!this.props.intervals || !this.props.intervals.vacation || !this.props.userEmployee) {
            return;
        }

        this.props.cancelVacation(this.props.userEmployee.employeeId, this.props.intervals.vacation.calendarEvent);
    };

    private onAcceptPress = () => {
        const { userEmployee, intervals, changeVacationStartDate } = this.props;

        changeVacationStartDate();
    };

    private onCloseDialog = () => {
        this.props.closeDialog();
    };

    public get text(): string {
        if (!this.props.intervals || !this.props.intervals.vacation) {
            return '';
        }

        const startDate = this.props.intervals.vacation.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);
        const endDate = this.props.intervals.vacation.calendarEvent.dates.endDate.format(eventDialogTextDateFormat);

        return `Your vacation starts on ${startDate} and completets on ${endDate}.`;
    }
}

const mapStateToProps = (state: AppState): EditVacationEventDialogProps => ({
    intervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    userEmployee: getEmployee(state),
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): EditVacationEventDialogDispatchProps => ({
    cancelVacation: (employeeId: string, calendarEvent: CalendarEvent) => {
        dispatch(cancelVacation(employeeId, calendarEvent));
    },
    changeVacationStartDate: () => {
        dispatch(openEventDialog(EventDialogType.ChangeVacationStartDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

export const EditVacationEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditVacationEventDialogImpl);
