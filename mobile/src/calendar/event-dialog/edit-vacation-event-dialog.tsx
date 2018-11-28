import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import {Action, Dispatch} from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, IntervalModel, ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEventType, CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { cancelVacation } from '../../reducers/calendar/vacation.action';
import { Employee } from '../../reducers/organization/employee.model';

interface EditVacationEventDialogDispatchProps {
    cancelVacation: (employeeId: string, calendarEvent: CalendarEvent) => void;
    changeVacationStartDate: () => void;
    closeDialog: () => void;
}

interface EditVacationEventDialogProps {
    intervals: ExtractedIntervals;
    userEmployee: Employee;
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
                    onClosePress={this.onCloseDialog} />;
    }

    private onCancelPress = () => {
        const { intervals: { vacation }, userEmployee } = this.props;
        this.props.cancelVacation(userEmployee.employeeId, vacation.calendarEvent);
    };

    private onAcceptPress = () => {
        const { userEmployee, intervals, changeVacationStartDate } = this.props;

        changeVacationStartDate();
    };

    private onCloseDialog = () => {
        this.props.closeDialog();
    };

    public get text(): string {
        const { intervals: { vacation } } = this.props;

        const startDate = vacation.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);
        const endDate = vacation.calendarEvent.dates.endDate.format(eventDialogTextDateFormat);

        return `Your vacation starts on ${startDate} and completets on ${endDate}.`;
    }
}

const mapStateToProps = (state: AppState): EditVacationEventDialogProps => ({
    intervals: state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection,
    userEmployee: state.organization.employees.employeesById.get(state.userInfo.employeeId)
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): EditVacationEventDialogDispatchProps => ({
    cancelVacation: (employeeId: string, calendarEvent: CalendarEvent) => { dispatch(cancelVacation(employeeId, calendarEvent)); },
    changeVacationStartDate: () => { dispatch(openEventDialog(EventDialogType.ChangeVacationStartDate)); },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const EditVacationEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditVacationEventDialogImpl);
