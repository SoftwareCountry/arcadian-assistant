import React, { Component } from 'react';
import { TouchableHighlight, TouchableOpacity, View } from 'react-native';
import { calendarActionsStyles, CalendarIntervalColor } from './styles';
import { CalendarActionButtonSeparator } from './calendar-action-button';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { IntervalsModel, DayModel, IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { VacationActionButton } from './vacation-action-button';
import { DayoffActionButton } from './dayoff-action-button';
import { SickLeaveActionButton } from './sick-leave-action-button';

interface ActionButtonGroupProps {
    intervalsModel: IntervalsModel;
    selectedCalendarDay: DayModel;
}

export class ActionsButtonGroupImpl extends Component<ActionButtonGroupProps> {
    public render() {
        const { vacation, dayoff, sickleave } = this.getIntervals();

        return (
            <View style={calendarActionsStyles.container}>
                <VacationActionButton interval={vacation} />

                <CalendarActionButtonSeparator />

                <DayoffActionButton interval={dayoff} />

                <CalendarActionButtonSeparator />

                <SickLeaveActionButton interval={sickleave} />

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
                dayoff = intervals.find(x => x.eventType === CalendarEventsType.Dayoff);
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

export const CalendarActionsButtonGroup = connect(mapStateToProps)(ActionsButtonGroupImpl);