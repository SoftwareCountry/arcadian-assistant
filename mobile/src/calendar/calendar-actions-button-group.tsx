import React, { Component } from 'react';
import { TouchableHighlight, TouchableOpacity, View } from 'react-native';
import { calendarActionsStyles } from './styles';
import { CalendarActionButtonSeparator } from './calendar-action-button';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { IntervalsModel, DayModel, IntervalModel, ExtractedIntervals, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { VacationActionButton } from './vacation-action-button';
import { DayoffActionButton } from './dayoff-action-button';
import { SickLeaveActionButton } from './sick-leave-action-button';
import { Dispatch } from 'redux';
import { CalendarActions } from '../reducers/calendar/calendar.action';
import { openEventDialog } from '../reducers/calendar/event-dialog/event-dialog.action';
import { EventDialogType } from '../reducers/calendar/event-dialog/event-dialog-type.model';

interface ActionButtonGroupProps {
    allIntervals: ReadOnlyIntervalsModel;
    intervalsBySingleDaySelection: ExtractedIntervals;
    disableActionButtons: boolean;
}

interface ActionButtonsGroupDispatchProps {
    sickLeaveActions: {
        claim: () => void;
        edit: () => void;
    };
}

export class ActionsButtonGroupImpl extends Component<ActionButtonGroupProps & ActionButtonsGroupDispatchProps> {
    public render() {
        const { intervalsBySingleDaySelection, allIntervals } = this.props;

        return (
            <View style={calendarActionsStyles.container}>
                <VacationActionButton interval={intervalsBySingleDaySelection.vacation} disabled={this.props.disableActionButtons} />

                <CalendarActionButtonSeparator />

                <DayoffActionButton interval={intervalsBySingleDaySelection.dayoff} disabled={this.props.disableActionButtons} />

                <CalendarActionButtonSeparator />

                <SickLeaveActionButton allIntervals={allIntervals} interval={intervalsBySingleDaySelection.sickleave} disabled={this.props.disableActionButtons} {...this.props.sickLeaveActions} />
                
                <CalendarActionButtonSeparator />
            </View>
        );
    }
}

const mapStateToProps = (state: AppState): ActionButtonGroupProps => ({
    allIntervals: state.calendar.calendarEvents.intervals,
    intervalsBySingleDaySelection: state.calendar.calendarEvents.selectedIntervalsBySingleDaySelection,
    disableActionButtons: state.calendar.calendarEvents.disableCalendarActionsButtonGroup
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>) => ({
    sickLeaveActions: {
        claim: () => { dispatch(openEventDialog(EventDialogType.ClaimSickLeave)); },
        edit: () => { dispatch(openEventDialog(EventDialogType.EditSickLeave)); },
    }
});

export const CalendarActionsButtonGroup = connect(mapStateToProps, mapDispatchToProps)(ActionsButtonGroupImpl);