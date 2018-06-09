import React, { Component } from 'react';
import { View, StyleSheet, FlatList, ListRenderItemInfo, Dimensions } from 'react-native';

import { StyledText } from '../override/styled-text';
import { ApplicationIcon } from '../override/application-icon';
import { Avatar } from '../people/avatar';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventType, eventTypeToGlyphIcon, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { EventManagementToolset } from './event-management-toolset';
import { Employee } from '../reducers/organization/employee.model';

interface EmployeeDetailsEventsListProps {
    events: CalendarEvent[];
    employee: Employee;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
    showUserAvatar?: Boolean;
    pendingRequestMode?: Boolean;
    eventManagementEnabled: Boolean;
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
        const { eventsContainer, eventRow, eventLeftIcons, eventTypeIconContainer, eventLeftIconsTiny, eventTypeIconContainerTiny, eventIcon, eventTextContainer, eventTitle, eventDetails, avatarContainer, avatarOuterFrame, avatarImage } = layoutStylesForEmployeeDetailsScreen;
        
        const leftIconsStyle = this.props.showUserAvatar ? eventLeftIcons : eventLeftIconsTiny;
        const typeIconContainerStyle = this.props.showUserAvatar ? eventTypeIconContainer : eventTypeIconContainerTiny;

        const eventsContainerFlattened = StyleSheet.flatten([
            eventsContainer, {width: Dimensions.get('window').width}
        ]);

        const secondRowEventDetails = this.props.pendingRequestMode ? 'requests ' + item.type.toLowerCase() : item.descriptionStatus;

        return (
                <View style={eventsContainerFlattened} key={item.calendarEventId}>
                    <View style={eventRow}>
                    <View style={leftIconsStyle}>
                        <View style={typeIconContainerStyle}>
                            <ApplicationIcon name={eventTypeToGlyphIcon.get(item.type)} style={eventIcon} />
                        </View>
                        {
                            this.props.showUserAvatar ? 
                            <View style={avatarContainer}>
                                <Avatar photo={this.props.employee.photo} style={avatarOuterFrame} imageStyle={avatarImage} />
                            </View> : null
                        }
                    </View>
                        <View style={eventTextContainer}>
                            <StyledText style={eventTitle}>{this.props.employee.name}</StyledText>
                            <StyledText style={eventDetails}>{secondRowEventDetails}</StyledText>
                            <StyledText style={eventDetails}>{item.descriptionFromTo}</StyledText>
                        </View>
                        {
                            (this.props.pendingRequestMode || this.props.eventManagementEnabled) ? <EventManagementToolset 
                            event={this.props.events.find(e => e.calendarEventId === item.calendarEventId)} 
                            employeeId={this.props.employee.employeeId} 
                            eventSetNewStatusAction={this.props.eventSetNewStatusAction} /> : null
                        }
                    </View>
                </View>
        );
    }
}
