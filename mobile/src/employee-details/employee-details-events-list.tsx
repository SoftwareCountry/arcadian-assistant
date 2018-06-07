import React, { Component } from 'react';
import { View, Text, StyleSheet, FlatList, ListRenderItemInfo, ViewStyle, Dimensions } from 'react-native';

import { StyledText } from '../override/styled-text';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventType, eventTypeToGlyphIcon, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { eventDialogTextDateFormat } from '../calendar/event-dialog/event-dialog-base';
import { EventManagementToolset } from './event-management-toolset';

interface EmployeeDetailsEventsListProps {
    events: CalendarEvent[];
    employeeId: string;
    employeeName: string;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
}

export class EmployeeDetailsEventsList extends Component<EmployeeDetailsEventsListProps> {
    public render() {
        return (<FlatList
                    data={this.props.events}
                    keyExtractor={this.keyExtractor}
                    renderItem={this.renderItem} />
        );
    }

    private keyExtractor = (item: CalendarEvent) => item.calendarEventId;

    private renderItem = (itemInfo: ListRenderItemInfo<CalendarEvent>) => {
        const { item } = itemInfo;
        const { eventsContainer, eventRow, eventLeftIconsTiny, eventTypeIconContainerTiny, eventIcon, eventTextContainer, eventTitle, eventDetails, avatarContainer } = layoutStylesForEmployeeDetailsScreen;

        const eventsContainerFlattened = StyleSheet.flatten([
            eventsContainer, {width: Dimensions.get('window').width}
        ]);

        return (
                <View style={eventsContainerFlattened} key={item.calendarEventId}>
                    <View style={eventRow}>
                    <View style={eventLeftIconsTiny}>
                        <View style={eventTypeIconContainerTiny}>
                            <ApplicationIcon name={eventTypeToGlyphIcon.get(item.type)} style={eventIcon} />
                        </View>
                    </View>
                        <View style={eventTextContainer}>
                            <StyledText style={eventTitle}>{this.props.employeeName}</StyledText>
                            <StyledText style={eventDetails}>{item.descriptionStatus}</StyledText>
                            <StyledText style={eventDetails}>{item.descriptionFromTo}</StyledText>
                        </View>
                        <EventManagementToolset event={this.props.events.find(e => e.calendarEventId === item.calendarEventId)} employeeId={this.props.employeeId} eventSetNewStatusAction={this.props.eventSetNewStatusAction} />
                    </View>
                </View>
        );
    }
}
