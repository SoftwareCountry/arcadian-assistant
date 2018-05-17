import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { cancelDayoff } from '../../reducers/calendar/dayoff.action';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { Employee } from '../../reducers/organization/employee.model';

interface EditDayoffEventDialogDispatchProps {
    closeDialog: () => void;
    cancelProcessDayoff: (employeeId: string, calendarEvent: CalendarEvent) => void;
}

interface EditDayoffEventDialogProps {
    selectedIntervals: ExtractedIntervals;
    userEmployee: Employee;
}

class EditDayoffEventDialogImpl extends Component<EditDayoffEventDialogProps & EditDayoffEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
                    title={'Cancel your dayoff'}
                    text={this.text}
                    icon={'dayoff'}
                    cancelLabel={'Back'}
                    acceptLabel={'Cancel'}
                    onAcceptPress={this.onAcceptClick}
                    onCancelPress={this.onCancelClick}
                    onClosePress={this.onCloseClick} />;
    }

    private onCancelClick = () => {
        this.props.closeDialog();
    }

    private onAcceptClick = () => {
        const { selectedIntervals: { dayoff }, userEmployee, cancelProcessDayoff } = this.props;

        cancelProcessDayoff(userEmployee.employeeId, dayoff.calendarEvent);
    }

    private onCloseClick = () => {
        this.props.closeDialog();
    }

    public get text(): string {
        const { selectedIntervals: { dayoff } } = this.props;

        const date = dayoff.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);

        return `Your dayoff starts on ${date}`;
    }
}

const mapStateToProps = (state: AppState): EditDayoffEventDialogProps => ({
    selectedIntervals: state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection,
    userEmployee: state.organization.employees.employeesById.get(state.userInfo.employeeId)
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): EditDayoffEventDialogDispatchProps => ({
    closeDialog: () => { dispatch(closeEventDialog()); },
    cancelProcessDayoff: (employeeId: string, calendarEvent: CalendarEvent) => { dispatch(cancelDayoff(employeeId, calendarEvent)); }
});

export const EditDayoffEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditDayoffEventDialogImpl);