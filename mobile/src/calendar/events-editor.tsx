import React, { Component } from 'react';
import { TouchableHighlight, TouchableOpacity, View } from 'react-native';
import { eventsEditorStyles, CalendarIntervalColor } from './styles';
import { StyledText } from '../override/styled-text';
import { EventsEditorButton, EventsEditorSeparator, OnPressEventsEditorButton } from './events-editor-button';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { IntervalsModel, DayModel, IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';

interface EventsEditorProps {
    intervalsModel: IntervalsModel;
    selectedCalendarDay: DayModel;
}

export class EventsEditorImpl extends Component<EventsEditorProps> {
    public render() {
        const { vacation, dayoff, sickleave } = this.getEvents();

        return (
            <View style={eventsEditorStyles.container}>
                <EventsEditorButton
                    requestTitle={'Request Vacation'}
                    reviewTitle={'Review Vacation'}
                    borderColor={CalendarIntervalColor.vacation}
                    onPress={this.onVacationButtonPress}
                    isRequest={!vacation} />

                <EventsEditorSeparator />

                <EventsEditorButton
                    requestTitle={'Claim Dayoff'}
                    reviewTitle={'Review Dayoff'}
                    borderColor={CalendarIntervalColor.dayoff}
                    onPress={this.onDayoffButtonPress}
                    isRequest={!dayoff} />

                <EventsEditorSeparator />

                <EventsEditorButton
                    requestTitle={'Claim Sick Leave'}
                    reviewTitle={'Review Sick Leave'}
                    borderColor={CalendarIntervalColor.sickLeave}
                    onPress={this.onSickleaveButtonPress}
                    isRequest={!sickleave} />

                <EventsEditorSeparator />
            </View>
        );
    }

    private getEvents(): {vacation: IntervalModel, dayoff: IntervalModel, sickleave: IntervalModel } {
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

    private onVacationButtonPress: OnPressEventsEditorButton = (isRequest) => {
        if (isRequest) {
            // TODO: dispatch
        } else {
            // TODO: dispatch
        }
    }

    private onDayoffButtonPress: OnPressEventsEditorButton = (isRequest) => {
        if (isRequest) {
            // TODO: dispatch
        } else {
            // TODO: dispatch
        }
    }

    private onSickleaveButtonPress: OnPressEventsEditorButton = (isRequest) => {
        if (isRequest) {
            // TODO: dispatch
        } else {
            // TODO: dispatch
        }
    }
}

const mapStateToProps = (state: AppState) => ({
    intervalsModel: state.calendar.calendarEvents.intervals,
    selectedCalendarDay: state.calendar.calendarEvents.selectedCalendarDay
});

export const EventsEditor = connect(mapStateToProps)(EventsEditorImpl);