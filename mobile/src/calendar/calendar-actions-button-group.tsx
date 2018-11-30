import React, { Component } from 'react';
import { View } from 'react-native';
import { calendarActionsStyles } from './styles';
import { CalendarActionButtonSeparator } from './calendar-action-button';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { ExtractedIntervals, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { VacationActionButton } from './vacation-action-button';
import { DayoffActionButton } from './dayoff-action-button';
import { SickLeaveActionButton } from './sick-leave-action-button';
import { Action, Dispatch } from 'redux';
import { openEventDialog } from '../reducers/calendar/event-dialog/event-dialog.action';
import { EventDialogType } from '../reducers/calendar/event-dialog/event-dialog-type.model';
import { Nullable, Optional } from 'types';

interface ActionButtonGroupProps {
    allIntervals: Optional<ReadOnlyIntervalsModel>;
    intervalsBySingleDaySelection: Nullable<ExtractedIntervals>;
    disableActionButtons: boolean;
}

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
    dayoff: {
        process: () => void,
        edit: () => void
    };
}

export class ActionsButtonGroupImpl extends Component<ActionButtonGroupProps & ActionButtonsGroupDispatchProps> {
    public render() {
        const { intervalsBySingleDaySelection, allIntervals } = this.props;

        if (!allIntervals || !intervalsBySingleDaySelection) {
            return null;
        }

        return (
            <View style={calendarActionsStyles.container}>
                {intervalsBySingleDaySelection.vacation && < VacationActionButton
                    allIntervals={allIntervals}
                    interval={intervalsBySingleDaySelection.vacation}
                    disabled={this.props.disableActionButtons}
                {...this.props.vacationActions} />
                }
                <CalendarActionButtonSeparator />

                {intervalsBySingleDaySelection.dayoff && <DayoffActionButton
                    interval={intervalsBySingleDaySelection.dayoff}
                    disabled={this.props.disableActionButtons}
                    {...this.props.dayoff} />
                }
                <CalendarActionButtonSeparator />

                {intervalsBySingleDaySelection.sickleave && <SickLeaveActionButton
                    allIntervals={allIntervals}
                    interval={intervalsBySingleDaySelection.sickleave}
                    disabled={this.props.disableActionButtons}
                    {...this.props.sickLeaveActions} />
                }
                <CalendarActionButtonSeparator />
            </View>
        );
    }
}

const mapStateToProps = (state: AppState): ActionButtonGroupProps => ({
    allIntervals: state.calendar ? state.calendar.calendarEvents.intervals : null,
    intervalsBySingleDaySelection: state.calendar ? state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection : null,
    disableActionButtons: state.calendar ? state.calendar.calendarEvents.disableCalendarActionsButtonGroup : false,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): ActionButtonsGroupDispatchProps => ({
    sickLeaveActions: {
        claim: () => { dispatch(openEventDialog(EventDialogType.ClaimSickLeave)); },
        edit: () => { dispatch(openEventDialog(EventDialogType.EditSickLeave)); },
        cancel: () => { dispatch(openEventDialog(EventDialogType.CancelSickLeave)); }
    },
    vacationActions: {
        request: () => { dispatch(openEventDialog(EventDialogType.RequestVacation)); },
        edit: () => { dispatch(openEventDialog(EventDialogType.EditVacation)); }
    },
    dayoff: {
        process: () => { dispatch(openEventDialog(EventDialogType.ProcessDayoff)); },
        edit: () => { dispatch(openEventDialog(EventDialogType.EditDayoff)); }
    }
});

export const CalendarActionsButtonGroup = connect(mapStateToProps, mapDispatchToProps)(ActionsButtonGroupImpl);
