import React, { Component } from 'react';
import { CalendarEventType } from '../reducers/calendar/calendar-event.model';
import { ApplicationIcon } from '../override/application-icon';
import { ViewStyle } from 'react-native';

interface CalendarEventIconProps {
    type: CalendarEventType;
    style: ViewStyle;
}

export class CalendarEventIcon extends Component<CalendarEventIconProps> {
    private readonly eventTypeToGlyphIcon: { [key in CalendarEventType]: string } = {
        [CalendarEventType.DayOff]: 'dayoff',
        [CalendarEventType.Vacation]: 'vacation',
        [CalendarEventType.SickLeave]: 'sick_leave',
        [CalendarEventType.Workout]: 'dayoff'
    };

    public render() {
        return <ApplicationIcon name={this.eventTypeToGlyphIcon[this.props.type]} style={this.props.style}/>;
    }
}
