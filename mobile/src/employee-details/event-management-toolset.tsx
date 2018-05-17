import React, { Component } from 'react';
import { View, Text, TouchableOpacity } from 'react-native';

import { CalendarEvent, CalendarEventType, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';

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
                <Text style={{ fontSize: 9, color: 'green' }}>APPROVE</Text>
            </TouchableOpacity>
            <TouchableOpacity 
                onPress={this.onReject}>
                    <Text style={{ fontSize: 9, color: 'red' }}>REJECT</Text>
            </TouchableOpacity>
        </View> : null;
    }

    private updateCalendarEvent(calendarEvent: CalendarEvent, status: CalendarEventStatus) {
        this.props.eventSetNewStatusAction(this.props.employeeId, calendarEvent, status);
    }
}
