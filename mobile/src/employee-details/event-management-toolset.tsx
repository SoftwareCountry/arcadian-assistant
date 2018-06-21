import React, { Component } from 'react';
import { View, Image, Text, TouchableOpacity } from 'react-native';

import { CalendarEvent, CalendarEventType, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEventManagementToolset } from './styles';

interface EventManagementToolsetProps {
    event: CalendarEvent;
    employeeId: string;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
    canApprove: boolean;
    canReject: boolean;
}

export class EventManagementToolset extends Component<EventManagementToolsetProps> {
    public onApprove = () => {
        this.updateCalendarEvent(this.props.event, CalendarEventStatus.Approved);
    }

    public onReject = () => {
        this.updateCalendarEvent(this.props.event, CalendarEventStatus.Rejected);
    }

    public render() {
        const { toolsetContainer, approveIcon, rejectIcon } = layoutStylesForEventManagementToolset;
        const { canApprove, canReject } = this.props;

        return (this.props.event.status === CalendarEventStatus.Requested) ? 
        <View style={toolsetContainer}>
            {
                canApprove &&
                <TouchableOpacity onPress={this.onApprove}>
                    <ApplicationIcon name={'approve-tick'} style={approveIcon} />
                </TouchableOpacity>
            }
            {
                canReject &&
                <TouchableOpacity onPress={this.onReject}>
                    <ApplicationIcon name={'reject-cross'} style={rejectIcon} />
                </TouchableOpacity>
            }
        </View> : null;
    }

    private updateCalendarEvent(calendarEvent: CalendarEvent, status: CalendarEventStatus) {
        this.props.eventSetNewStatusAction(this.props.employeeId, calendarEvent, status);
    }
}
