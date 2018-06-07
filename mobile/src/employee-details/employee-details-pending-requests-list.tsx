import React, { Component } from 'react';
import { Map } from 'immutable';
import { View, Text, StyleSheet, FlatList, ListRenderItemInfo, ViewStyle, Dimensions } from 'react-native';

import { StyledText } from '../override/styled-text';
import { ApplicationIcon } from '../override/application-icon';
import { Avatar } from '../people/avatar';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventType, eventTypeToGlyphIcon, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { eventDialogTextDateFormat, eventDigitsDateFormat } from '../calendar/event-dialog/event-dialog-base';
import { EventManagementToolset } from './event-management-toolset';
import { Employee } from '../reducers/organization/employee.model';

interface EmployeeDetailsPendingRequestsListProps {
    events: CalendarEvent[];
    employeeId: string;
    employee: Employee;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
}

export class EmployeeDetailsPendingRequestsList extends Component<EmployeeDetailsPendingRequestsListProps> {
    public render() {
        return <FlatList
            data={this.props.events}
            keyExtractor={this.keyExtractor}
            renderItem={this.renderItem} />;
    }

    private keyExtractor = (item: CalendarEvent) => item.calendarEventId;

    private renderItem = (itemInfo: ListRenderItemInfo<CalendarEvent>) => {
        const { item } = itemInfo;
        const { eventsContainer, eventRow, eventLeftIcons, eventTypeIconContainer, eventIcon, eventTextContainer, eventTitle, eventDetails, avatarContainer, avatarOuterFrame, avatarImage } = layoutStylesForEmployeeDetailsScreen;

        const eventsContainerFlattened = StyleSheet.flatten([
            eventsContainer, {width: Dimensions.get('window').width}
        ]);

        return (
                <View style={eventsContainerFlattened} key={item.calendarEventId}>
                    <View style={eventRow}>
                    <View style={eventLeftIcons}>
                        <View style={eventTypeIconContainer}>
                            <ApplicationIcon name={eventTypeToGlyphIcon.get(item.type)} style={eventIcon} />
                        </View>
                        <View style={avatarContainer}>
                            <Avatar photo={this.props.employee.photo} style={avatarOuterFrame} imageStyle={avatarImage} />
                        </View>
                    </View>
                        <View style={eventTextContainer}>
                            <StyledText style={eventTitle}>{this.props.employee.name}</StyledText>
                            <StyledText style={eventDetails}>requests {item.type.toLowerCase()}</StyledText>
                            <StyledText style={eventDetails}>{item.descriptionFromTo}</StyledText>
                        </View>
                        <EventManagementToolset event={this.props.events.find(e => e.calendarEventId === item.calendarEventId)} employeeId={this.props.employeeId} eventSetNewStatusAction={this.props.eventSetNewStatusAction} />
                    </View>
                </View>
        );
    }
}
