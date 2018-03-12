import React, { Component } from 'react';
import { TouchableHighlight, TouchableOpacity, View } from 'react-native';
import { calendarActionsStyles } from './styles';
import { CalendarActionButtonSeparator } from './calendar-action-button';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { IntervalsModel, DayModel, IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { VacationActionButton, VacationActionButtonProps } from './vacation-action-button';
import { DayoffActionButton } from './dayoff-action-button';
import { SickLeaveActionButton } from './sick-leave-action-button';
import { Dispatch } from 'redux';
import { CalendarActions } from '../reducers/calendar/calendar.action';
import { editSickLeave, claimSickLeave, prolongSickLeave } from '../reducers/calendar/sick-leave.action';
import { Moment } from 'moment';
import { claimVacation, editVacation } from '../reducers/calendar/vacation.action';
import { claimDayOff, editDayOff } from '../reducers/calendar/day-off.action';

interface ActionButtonGroupProps {
    intervalsModel: IntervalsModel;
    selectedCalendarDay: DayModel;
    disableActionButtons: boolean;
}

interface ActionButtonDispatchProps {
    claim: (startDate: Moment) => void;
    edit: () => void;
}

interface ActionButtonsGroupDispatchProps {
    vacationActions: ActionButtonDispatchProps;
    dayoffActions: ActionButtonDispatchProps;
    sickLeaveActions: ActionButtonDispatchProps;
}

export class ActionsButtonGroupImpl extends Component<ActionButtonGroupProps & ActionButtonsGroupDispatchProps> {
    public render() {
        const { vacation, dayoff, sickleave } = this.getIntervals();

        const buttonProps: VacationActionButtonProps = {
            disabled: this.props.disableActionButtons,
            selectedDay: this.props.selectedCalendarDay
        } as VacationActionButtonProps;

        const vacationsProps: VacationActionButtonProps = Object.assign(
            {
                interval: dayoff,
                ...this.props.vacationActions
            },
            buttonProps);

        const dayOffProps: VacationActionButtonProps = Object.assign(
            {
                interval: dayoff,
                ...this.props.dayoffActions
            },
            buttonProps);

        const sickLeaveProps: VacationActionButtonProps = Object.assign(
            {
                interval: sickleave,
                ...this.props.sickLeaveActions
            },
            buttonProps);

        return (
            <View style={calendarActionsStyles.container}>
                <VacationActionButton {...vacationsProps} />

                <CalendarActionButtonSeparator />

                <DayoffActionButton {...dayOffProps} />

                <CalendarActionButtonSeparator />

                <SickLeaveActionButton {...sickLeaveProps} />

                <CalendarActionButtonSeparator />
            </View>
        );
    }

    private getIntervals(): { vacation: IntervalModel, dayoff: IntervalModel, sickleave: IntervalModel } {
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

const mapStateToProps = (state: AppState): ActionButtonGroupProps => ({
    intervalsModel: state.calendar.calendarEvents.intervals,
    selectedCalendarDay: state.calendar.calendarEvents.selectedCalendarDay,
    disableActionButtons: state.calendar.calendarEvents.disableCalendarActionsButtonGroup
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>) => ({
    vacationActions: {
        claim: (startDate: Moment) => { dispatch(claimVacation(startDate)); },
        edit: () => { dispatch(editVacation()); }
    },

    dayoffActions: {
        claim: (startDate: Moment) => { dispatch(claimDayOff(startDate)); },
        edit: () => { dispatch(editDayOff()); }
    },

    sickLeaveActions: {
        claim: (startDate: Moment) => { dispatch(claimSickLeave(startDate)); },
        edit: () => { dispatch(editSickLeave()); }
    }
});

export const CalendarActionsButtonGroup = connect(mapStateToProps, mapDispatchToProps)(ActionsButtonGroupImpl);