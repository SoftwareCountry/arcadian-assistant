import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { DayModel, ExtractedIntervals } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { Moment } from 'moment';

interface ChangeVacationStartDateEventDialogDispatchProps {
    back: () => void;
    changeVacationEndDate: () => void;
    closeDialog: () => void;
}

interface ChangeVacationStartDateEventDialogProps {
    selectedSingleDay: DayModel;
    intervals: ExtractedIntervals;
}

class ChangeVacationStartDateEventDialogImpl extends Component<ChangeVacationStartDateEventDialogProps & ChangeVacationStartDateEventDialogDispatchProps> {
    public render() {
        return <EventDialogBase
                    title={'Change start date'}
                    text={this.text}
                    icon={'vacation'}
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onAcceptPress={this.confirmStartDateChange}
                    onCancelPress={this.back}
                    onClosePress={this.closeDialog} />;
    }

    private back = () => {
        this.props.back();
    }

    private confirmStartDateChange = () => {
        const { intervals, changeVacationEndDate } = this.props;

        changeVacationEndDate();
    }

    private closeDialog = () => {
        this.props.closeDialog();
    }

    public get text(): string {
        const { intervals: { vacation }, selectedSingleDay } = this.props;

        const startDate = selectedSingleDay.date;

        return `Your vacation starts on ${startDate.format(eventDialogTextDateFormat)}`;
    }
}

const mapStateToProps = (state: AppState): ChangeVacationStartDateEventDialogProps => ({
    selectedSingleDay: state.calendar.calendarEvents.selection.single.day,
    intervals: state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ChangeVacationStartDateEventDialogDispatchProps => ({
    back: () => { dispatch(openEventDialog(EventDialogType.EditVacation)); },
    changeVacationEndDate: () => { dispatch(openEventDialog(EventDialogType.ChangeVacationEndDate)); },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const ChangeVacationStartDateEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChangeVacationStartDateEventDialogImpl);