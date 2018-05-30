import React, { Component } from 'react';
import { View, Image, Text, TouchableOpacity } from 'react-native';

import { CalendarEvent, CalendarEventType, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';

// tslint:disable-next-line:no-var-requires
const closeIcon = require('./close-icon.png');
// tslint:disable-next-line:no-var-requires
const tickIcon = require('./tick-icon.png');

interface EventManagementToolsetProps {
    event: CalendarEvent;
    employeeId: string;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
}

export class EventManagementToolset extends Component<EventManagementToolsetProps> {
    public onApprove = () => {
        this.updateCalendarEvent(this.props.event, CalendarEventStatus.Approved);
    }

    public onReject = () => {
        this.updateCalendarEvent(this.props.event, CalendarEventStatus.Rejected);
    }

    public render() {
        return (this.props.event.status === CalendarEventStatus.Requested) ? 
        <View style={{ paddingLeft: 10, flexDirection: 'column', alignItems: 'center' }}>
            <TouchableOpacity 
                onPress={this.onApprove}>
                <Image source={tickIcon} style={{ height: 28, width: 28}} />
            </TouchableOpacity>
            <TouchableOpacity 
                onPress={this.onReject}>
                    <Image source={closeIcon} style={{ height: 28, width: 28}} />
            </TouchableOpacity>
        </View> : null;
    }

    private updateCalendarEvent(calendarEvent: CalendarEvent, status: CalendarEventStatus) {
        this.props.eventSetNewStatusAction(this.props.employeeId, calendarEvent, status);
    }
}
