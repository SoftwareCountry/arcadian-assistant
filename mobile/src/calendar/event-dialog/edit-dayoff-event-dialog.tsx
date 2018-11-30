import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Action, Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { cancelDayoff } from '../../reducers/calendar/dayoff.action';
import { CalendarEvent } from '../../reducers/calendar/calendar-event.model';
import { Employee } from '../../reducers/organization/employee.model';
import { getEmployee } from '../../utils/utils';
import { Optional } from 'types';

interface EditDayoffEventDialogDispatchProps {
    closeDialog: () => void;
    cancelProcessDayoff: (employeeId: string, calendarEvent: CalendarEvent) => void;
}

interface EditDayoffEventDialogProps {
    selectedIntervals: Optional<ExtractedIntervals>;
    userEmployee: Optional<Employee>;
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
    };

    private onAcceptClick = () => {
        if(!this.props.selectedIntervals || !this.props.selectedIntervals.dayoff || !this.props.userEmployee){
            return;
        }

       this.props.cancelProcessDayoff(this.props.userEmployee.employeeId, this.props.selectedIntervals.dayoff.calendarEvent);
    };

    private onCloseClick = () => {
        this.props.closeDialog();
    };

    public get text(): string {
        if(!this.props.selectedIntervals || !this.props.selectedIntervals.dayoff ){
            return '';
        }
        const date = this.props.selectedIntervals.dayoff.calendarEvent.dates.startDate.format(eventDialogTextDateFormat);

        return `Your dayoff starts on ${date}`;
    }
}

const mapStateToProps = (state: AppState): EditDayoffEventDialogProps => ({
    selectedIntervals: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    userEmployee: getEmployee(state),
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): EditDayoffEventDialogDispatchProps => ({
    closeDialog: () => { dispatch(closeEventDialog()); },
    cancelProcessDayoff: (employeeId: string, calendarEvent: CalendarEvent) => { dispatch(cancelDayoff(employeeId, calendarEvent)); }
});

export const EditDayoffEventDialog = connect(mapStateToProps, mapDispatchToProps)(EditDayoffEventDialogImpl);
