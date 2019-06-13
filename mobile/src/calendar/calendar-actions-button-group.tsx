import React, { Component } from 'react';
import { View } from 'react-native';
import { calendarActionsStyles } from './styles';
import { CalendarActionButtonSeparator } from './calendar-action-button';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { ExtractedIntervals, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { VacationActionButton } from './vacation-action-button';
import { DayOffActionButton } from './dayoff-action-button';
import { SickLeaveActionButton } from './sick-leave-action-button';
import { Action, Dispatch } from 'redux';
import { openEventDialog } from '../reducers/calendar/event-dialog/event-dialog.action';
import { EventDialogType } from '../reducers/calendar/event-dialog/event-dialog-type.model';
import { Optional } from 'types';

//============================================================================
interface ActionButtonGroupProps {
    allIntervals: Optional<ReadOnlyIntervalsModel>;
    intervalsBySingleDaySelection: Optional<ExtractedIntervals>;
    disableActionButtons: boolean;
}

//============================================================================
interface ActionButtonsGroupDispatchProps {
    sickLeaveActions: {
        claim: () => void;
        edit: () => void;
        cancel: () => void;
    };
    vacationActions: {
        request: () => void
        edit: () => void
    };
    dayOff: {
        process: () => void,
        edit: () => void
    };
}

//============================================================================
export class ActionsButtonGroupImpl extends Component<ActionButtonGroupProps & ActionButtonsGroupDispatchProps> {
    //----------------------------------------------------------------------------
    public render() {
        const { intervalsBySingleDaySelection, allIntervals } = this.props;

        if (!intervalsBySingleDaySelection) {
            return null;
        }

        return (
            <View style={calendarActionsStyles.container}>
                <VacationActionButton
                    interval={intervalsBySingleDaySelection.vacation}
                    disabled={this.props.disableActionButtons}
                    {...this.props.vacationActions} />
                <CalendarActionButtonSeparator/>

                <DayOffActionButton
                    interval={intervalsBySingleDaySelection.dayOff}
                    disabled={this.props.disableActionButtons}
                    {...this.props.dayOff} />
                <CalendarActionButtonSeparator/>

                <SickLeaveActionButton
                    allIntervals={allIntervals}
                    interval={intervalsBySingleDaySelection.sickLeave}
                    disabled={this.props.disableActionButtons}
                    {...this.props.sickLeaveActions} />
                <CalendarActionButtonSeparator/>
            </View>
        );
    }
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): ActionButtonGroupProps => ({
    allIntervals: state.calendar ? state.calendar.calendarEvents.intervals : undefined,
    intervalsBySingleDaySelection: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : undefined,
    disableActionButtons: state.calendar ? state.calendar.calendarEvents.disableCalendarActionsButtonGroup : false,
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>): ActionButtonsGroupDispatchProps => ({
    sickLeaveActions: {
        claim: () => {
            dispatch(openEventDialog(EventDialogType.ClaimSickLeave));
        },
        edit: () => {
            dispatch(openEventDialog(EventDialogType.EditSickLeave));
        },
        cancel: () => {
            dispatch(openEventDialog(EventDialogType.CancelSickLeave));
        }
    },
    vacationActions: {
        request: () => {
            dispatch(openEventDialog(EventDialogType.RequestVacation));
        },
        edit: () => {
            dispatch(openEventDialog(EventDialogType.EditVacation));
        }
    },
    dayOff: {
        process: () => {
            dispatch(openEventDialog(EventDialogType.ProcessDayOff));
        },
        edit: () => {
            dispatch(openEventDialog(EventDialogType.EditDayOff));
        }
    }
});

export const CalendarActionsButtonGroup = connect(stateToProps, dispatchToProps)(ActionsButtonGroupImpl);
