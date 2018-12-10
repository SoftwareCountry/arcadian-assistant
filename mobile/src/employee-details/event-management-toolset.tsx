/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { View, TouchableOpacity, Alert } from 'react-native';
import { CalendarEvent, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEventManagementToolset } from './styles';

//============================================================================
interface EventManagementToolsetProps {
    event: CalendarEvent;
    employeeId: string;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
    eventApprove: (employeeId: string, calendarEvent: CalendarEvent) => void;
    canApprove: boolean;
    canReject: boolean;
}

//============================================================================
export class EventManagementToolset extends Component<EventManagementToolsetProps> {
    //----------------------------------------------------------------------------
    public onApprove = () => {
        Alert.alert(
            'Are you sure you want to approve the request?',
            undefined,
            [
                {
                    text: 'Cancel',
                    style: 'cancel',
                },
                {
                    text: 'Approve',
                    onPress: () => {
                        this.approveCalendarEvent(this.props.event);
                    },
                },
            ],
        );
    };

    //----------------------------------------------------------------------------
    public onReject = () => {
        Alert.alert(
            'Are you sure you want to reject the request?',
            undefined,
            [
                {
                    text: 'Cancel',
                    style: 'cancel',
                },
                {
                    text: 'Reject',
                    onPress: () => {
                        this.updateCalendarEvent(this.props.event, CalendarEventStatus.Rejected);
                    },
                },
            ],
        );
    };

    //----------------------------------------------------------------------------
    public render() {
        const { toolsetContainer, approveIcon, rejectIcon } = layoutStylesForEventManagementToolset;
        const { canApprove, canReject } = this.props;

        return (
            <View style={toolsetContainer}>
                {
                    canApprove && this.props.event.status === CalendarEventStatus.Requested &&
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
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private updateCalendarEvent(calendarEvent: CalendarEvent, status: CalendarEventStatus) {
        this.props.eventSetNewStatusAction(this.props.employeeId, calendarEvent, status);
    }

    //----------------------------------------------------------------------------
    private approveCalendarEvent(calendarEvent: CalendarEvent) {
        this.props.eventApprove(this.props.employeeId, calendarEvent);
    }
}
