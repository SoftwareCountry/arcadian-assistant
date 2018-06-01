import React, { Component } from 'react';
import { View, Text, StyleSheet, FlatList, ListRenderItemInfo, ViewStyle, Dimensions } from 'react-native';

import { StyledText } from '../override/styled-text';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventType, eventTypeToGlyphIcon, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { eventDialogTextDateFormat, eventDigitsDateFormat } from '../calendar/event-dialog/event-dialog-base';
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
        const { eventsContainer, eventRow, eventIcon, eventTitle, eventDetails, avatarContainer } = layoutStylesForEmployeeDetailsScreen;

        const eventsContainerFlattened = StyleSheet.flatten([
            eventsContainer, {width: Dimensions.get('window').width}
        ]);

        return (
                <View style={eventsContainerFlattened} key={item.calendarEventId}>
                    <View style={eventRow}>
                    <View style={{ height: 48, width: 86, marginRight: 8, flexDirection: 'row', alignSelf: 'center', justifyContent: 'space-between' }}>
                        <View style={{
                            backgroundColor: '#D5EFF5', position: 'absolute', top: 1, left: 40, width: 46, height: 46, borderRadius: 23, justifyContent: 'center',
                            alignItems: 'center'
                        }}>
                            <ApplicationIcon name={eventTypeToGlyphIcon.get(item.type)} style={eventIcon} />
                        </View>
                        <View style={avatarContainer}/>
                    </View>
                        <View style={{ flex: 5 }}>
                            <StyledText style={eventTitle}>{this.props.employeeName}</StyledText>
                            <StyledText style={eventDetails}>requests {item.type.toLowerCase()}</StyledText>
                            <StyledText style={eventDetails}>from {item.dates.startDate.format(eventDigitsDateFormat)} to {item.dates.endDate.format(eventDigitsDateFormat)}</StyledText>
                        </View>
                        <EventManagementToolset event={this.props.events.find(e => e.calendarEventId === item.calendarEventId)} employeeId={this.props.employeeId} eventSetNewStatusAction={this.props.eventSetNewStatusAction} />
                    </View>
                </View>
        );
    }
}
