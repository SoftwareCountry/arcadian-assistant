import React, { Component } from 'react';
import { TouchableHighlight, TouchableOpacity, View } from 'react-native';
import { calendarActionsStyles } from './styles';
import { CalendarActionButtonSeparator } from './calendar-action-button';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { IntervalsModel, DayModel, IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { VacationActionButton } from './vacation-action-button';
import { DayoffActionButton } from './dayoff-action-button';
import { SickLeaveActionButton } from './sick-leave-action-button';
import { Dispatch } from 'redux';
import { CalendarActions } from '../reducers/calendar/calendar.action';
import { editSickLeave, claimSickLeave, prolongSickLeave } from '../reducers/calendar/sick-leave.action';

interface ActionButtonGroupProps {
    intervalsModel: IntervalsModel;
    selectedCalendarDay: DayModel;
}

interface ActionButtonsGroupDispatchProps {
    sickLeaveActions: {
        claim: () => void;
        edit: () => void;
    };
}

export class ActionsButtonGroupImpl extends Component<ActionButtonGroupProps & ActionButtonsGroupDispatchProps> {
    public render() {
        const { vacation, dayoff, sickleave } = this.getIntervals();

        return (
            <View style={calendarActionsStyles.container}>
                <VacationActionButton interval={vacation} />

                <CalendarActionButtonSeparator />

                <DayoffActionButton interval={dayoff} />

                <CalendarActionButtonSeparator />

                <SickLeaveActionButton interval={sickleave} {...this.props.sickLeaveActions} />

                <CalendarActionButtonSeparator />
            </View>
        );
    }

    private getIntervals(): {vacation: IntervalModel, dayoff: IntervalModel, sickleave: IntervalModel } {
        const { intervalsModel, selectedCalendarDay } = this.props;

        let vacation: IntervalModel;
        let dayoff: IntervalModel;
        let sickleave: IntervalModel;

        if (intervalsModel) {
             const intervals = intervalsModel.get(selectedCalendarDay.date);

             if (intervals) {
                vacation = intervals.find(x => x.eventType === CalendarEventsType.Vacation);
                dayoff = intervals.find(x => x.eventType === CalendarEventsType.Dayoff || x.eventType === CalendarEventsType.AdditionalWork);
                sickleave = intervals.find(x => x.eventType === CalendarEventsType.SickLeave);
             }
        }

        return { vacation, dayoff, sickleave };
    }
}

const mapStateToProps = (state: AppState) => ({
    intervalsModel: state.calendar.calendarEvents.intervals,
    selectedCalendarDay: state.calendar.calendarEvents.selectedCalendarDay
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>) => ({
    sickLeaveActions: {
        claim: () => { dispatch(claimSickLeave()); },
        edit: () => { dispatch(editSickLeave()); },
    }
});

export const CalendarActionsButtonGroup = connect(mapStateToProps, mapDispatchToProps)(ActionsButtonGroupImpl);