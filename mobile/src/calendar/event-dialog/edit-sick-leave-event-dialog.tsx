import React, { Component } from 'react';
import { View } from 'react-native';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, IntervalModel, ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEventsType, CalendarEvents } from '../../reducers/calendar/calendar-events.model';
import { completeSickLeave } from '../../reducers/calendar/sick-leave.action';
import { Employee } from '../../reducers/organization/employee.model';
import { Moment } from 'moment';

interface EditSickLeaveEventDialogDispatchProps {
    cancelDialog: () => void;
    completeSickLeave: (employeeId: string, calendarEvent: CalendarEvents) => void;
}

interface EditSickLeaveEventDialogProps {
    intervals: ExtractedIntervals;
    userEmployee: Employee;
}

class EditSickLeaveEventDialogImpl extends Component<EditSickLeaveEventDialogProps & EditSickLeaveEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
                    title={'Hey! Hope you feel better'}
                    text={this.text}
                    icon={'sick_leave'}
                    cancelLabel={'Prolong'}
                    acceptLabel={'Complete'}
                    onActionPress={this.acceptAction}
                    onCancelPress={this.cancelAction}
                    onClosePress={this.cancelAction} />;
    }

    private cancelAction = () => {
        this.props.cancelDialog();
    }

    private acceptAction = () => {
        const { userEmployee, intervals } = this.props;

        this.props.completeSickLeave(userEmployee.employeeId, intervals.sickleave.calendarEvent);
    }

    public get text(): string {
        const startDate = this.getSickLeaveStartDate();

        return `Your sick leave has started on ${startDate} and still is not completed.`;
    }

    private getSickLeaveStartDate(): string {
        if (!this.props.intervals.sickleave) {
            return null;
        }

        return this.props.intervals.sickleave.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);
    }
}

const mapStateToProps = (state: AppState): EditSickLeaveEventDialogProps => ({
    intervals: state.calendar.calendarEvents.intervalsBySingleDaySelection,
    userEmployee: state.userInfo.employee
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): EditSickLeaveEventDialogDispatchProps => ({
    cancelDialog: () => { dispatch(closeEventDialog()); },
    completeSickLeave: (employeeId: string, calendarEvent: CalendarEvents) => { dispatch(completeSickLeave(employeeId, calendarEvent)); }
});

export const EditSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditSickLeaveEventDialogImpl);