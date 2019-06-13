import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState, getEmployee, hasPermission } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { completeSickLeave } from '../../reducers/calendar/sick-leave.action';
import { Employee } from '../../reducers/organization/employee.model';
import { Nullable, Optional } from 'types';
import { Permission } from '../../reducers/user/user-employee-permissions.model';

//============================================================================
interface EditSickLeaveEventDialogDispatchProps {
    prolong: () => void;
    completeSickLeave: (employeeId: string, calendarEvent: CalendarEvent) => void;
    closeDialog: () => void;
}

//============================================================================
interface EditSickLeaveEventDialogProps {
    intervals: Optional<ExtractedIntervals>;
    userEmployee: Optional<Employee>;
    completeSickLeavePermission: Optional<Boolean>;
}

//============================================================================
class EditSickLeaveEventDialogImpl extends Component<EditSickLeaveEventDialogProps & EditSickLeaveEventDialogDispatchProps> {

    //----------------------------------------------------------------------------
    public render() {
        const selectedSickLeave = this.selectedSickLeave();

        const isCompleted = selectedSickLeave ? !selectedSickLeave.calendarEvent.isCompleted : false;
        const isDisabledForComplete = this.props.completeSickLeavePermission === false || isCompleted;

        if (isDisabledForComplete) {
            return <EventDialogBase
                title={'Hey! Hope you feel better'}
                text={this.text}
                icon={'sick_leave'}
                cancelLabel={'Back'}
                onCancelPress={this.closeDialog}
                acceptLabel={'Prolong'}
                onAcceptPress={this.prolong}
                onClosePress={this.closeDialog}/>;
        } else {
            return <EventDialogBase
                title={'Hey! Hope you feel better'}
                text={this.text}
                icon={'sick_leave'}
                cancelLabel={'Prolong'}
                disableCancel={!isCompleted}
                onCancelPress={this.prolong}
                acceptLabel={'Complete'}
                onAcceptPress={this.complete}
                onClosePress={this.closeDialog}/>;
        }
    }

    //----------------------------------------------------------------------------
    private prolong = () => {
        const selectedSickLeave = this.selectedSickLeave();
        if (!selectedSickLeave) {
            return;
        }

        this.props.prolong();
    };

    //----------------------------------------------------------------------------
    private complete = () => {
        const selectedSickLeave = this.selectedSickLeave();
        if (!selectedSickLeave) {
            return;
        }

        this.props.completeSickLeave(selectedSickLeave.userEmployee.employeeId, selectedSickLeave.calendarEvent);
    };

    //----------------------------------------------------------------------------
    private closeDialog = () => {
        this.props.closeDialog();
    };

    //----------------------------------------------------------------------------
    public get text(): string {
        const startDate = this.getSickLeaveStartDate();

        if (!startDate) {
            return `Please select a sick leave you want to edit.`;
        }

        return `Your sick leave has started on ${startDate} and still is not completed.`;
    }

    //----------------------------------------------------------------------------
    private getSickLeaveStartDate(): Nullable<string> {
        if (!this.props.intervals || !this.props.intervals.sickLeave) {
            return null;
        }

        return this.props.intervals.sickLeave.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);
    }

    //----------------------------------------------------------------------------
    private selectedSickLeave = (): Optional<{ userEmployee: Employee, calendarEvent: CalendarEvent }> => {
        const { userEmployee, intervals } = this.props;

        // noinspection RedundantIfStatementJS
        if (!userEmployee || !intervals || !intervals.sickLeave) {
            return undefined;
        }

        return { userEmployee: userEmployee, calendarEvent: intervals.sickLeave.calendarEvent };
    };
}

//----------------------------------------------------------------------------
const mapStateToProps = (state: AppState): EditSickLeaveEventDialogProps => {
    return {
        intervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
        userEmployee: getEmployee(state),
        completeSickLeavePermission: hasPermission(Permission.completeSickLeave, state)
    };
};

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): EditSickLeaveEventDialogDispatchProps => ({
    prolong: () => {
        dispatch(openEventDialog(EventDialogType.ProlongSickLeave));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    },
    completeSickLeave: (employeeId: string, calendarEvent: CalendarEvent) => {
        dispatch(completeSickLeave(employeeId, calendarEvent));
    }
});

export const EditSickLeaveEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditSickLeaveEventDialogImpl);
