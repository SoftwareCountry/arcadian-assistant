import React, { Component } from 'react';
import { Map } from 'immutable';
import { View, Text, StyleSheet, FlatList, ListRenderItemInfo, ViewStyle, Dimensions } from 'react-native';

import { StyledText } from '../override/styled-text';
import { ApplicationIcon } from '../override/application-icon';
import { Avatar } from '../people/avatar';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventType, eventTypeToGlyphIcon, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { eventDialogTextDateFormat } from '../calendar/event-dialog/event-dialog-base';
import { EventManagementToolset } from './event-management-toolset';

interface EmployeeDetailsPendingRequestsListProps {
    events: CalendarEvent[];
    requests: Map<string, CalendarEvent[]>;
    employeeId: string;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
}

export class EmployeeDetailsPendingRequestsList extends Component<EmployeeDetailsPendingRequestsListProps> {
    public render() {
        const { requests } = this.props;

        return <View><StyledText>PENDING REQUESTS</StyledText></View>;
    }

    private pendingRequestsByEmployee = (requests: Map<string, CalendarEvent[]>) => {
        const requestsGrouped = 
            requests.forEach((events: CalendarEvent[], key: string) => (
                <FlatList
                    data={events}
                    keyExtractor={this.keyExtractor}
                    renderItem={this.renderItem} />
            ));

        return <View>{requestsGrouped}</View>;
    }

    private keyExtractor = (item: CalendarEvent) => item.calendarEventId;

    private renderItem = (itemInfo: ListRenderItemInfo<CalendarEvent>) => {
        const { item } = itemInfo;
        const { eventsContainer, eventRow, eventIcon, eventTitle } = layoutStylesForEmployeeDetailsScreen;

        const eventsContainerFlattened = StyleSheet.flatten([
            eventsContainer, {width: Dimensions.get('window').width}
        ]);

        return (
                <View style={eventsContainerFlattened} key={item.calendarEventId}>
                    <View style={eventRow}>
                        <Avatar />
                        <ApplicationIcon name={eventTypeToGlyphIcon.get(item.type)} style={eventIcon} />
                        <StyledText style={eventTitle}>{item.type} starts on {item.dates.startDate.format(eventDialogTextDateFormat)} and completes on {item.dates.endDate.format(eventDialogTextDateFormat)} ({item.status})</StyledText>
                        <EventManagementToolset event={this.props.events.find(e => e.calendarEventId === item.calendarEventId)} employeeId={this.props.employeeId} eventSetNewStatusAction={this.props.eventSetNewStatusAction} />
                    </View>
                </View>
        );
    }
}
