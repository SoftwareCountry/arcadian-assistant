import React, { Component } from 'react';
import moment from 'moment';
import { Map, Set, Iterable } from 'immutable';
import { View, StyleSheet, FlatList, ListRenderItemInfo, Dimensions } from 'react-native';

import { StyledText } from '../override/styled-text';
import { ApplicationIcon } from '../override/application-icon';
import { Avatar } from '../people/avatar/avatar';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { EventManagementToolset } from './event-management-toolset';
import { Employee } from '../reducers/organization/employee.model';
import { CalendarEventIcon } from '../calendar/calendar-event-icon';

interface EmployeeDetailsEventsListProps {
    events: Map<Employee, CalendarEvent[]>;
    eventSetNewStatusAction: (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void;
    hoursToIntervalTitle: (startWorkingHour: number, finishWorkingHour: number) => string;
    showUserAvatar?: boolean;
    canApprove: boolean;
    canReject: boolean;
}

interface EmployeeDetailItem {
    employee: Employee;
    calendarEvent: CalendarEvent;
}

export class EmployeeDetailsEventsList extends Component<EmployeeDetailsEventsListProps> {
    private readonly eventDigitsDateFormat = 'DD/MM/YYYY';

    public render() {

        const events = this.prepareEvents();

        return (<FlatList
                    data={events}
                    keyExtractor={this.keyExtractor}
                    renderItem={this.renderItem} />
        );
    }

    private prepareEvents(): EmployeeDetailItem[] {
        const { events } = this.props;

        if (!events) {
            return null;
        }

        return events
            .sortBy((_, employee) => employee.name)
            .map((calendarEvents, employee) =>
                Set(calendarEvents)
                    .sort((a, b) => b.dates.startDate.valueOf() - a.dates.startDate.valueOf())
                    .map(calendarEvent => ({
                        employee: employee,
                        calendarEvent: calendarEvent
                    } as EmployeeDetailItem))
            )
            .flatten()
            .toArray();
    }

    private keyExtractor = (item: EmployeeDetailItem) => item.calendarEvent.calendarEventId;

    private renderItem = (itemInfo: ListRenderItemInfo<EmployeeDetailItem>) => {
        const { item } = itemInfo;
        const { eventsContainer, eventRow, eventLeftIcons, eventTypeIconContainer, eventLeftIconsTiny, eventTypeIconContainerTiny, eventIcon, eventTextContainer, eventTitle, eventDetails, avatarContainer, avatarOuterFrame, avatarImage } = layoutStylesForEmployeeDetailsScreen;

        const leftIconsStyle = this.props.showUserAvatar ? eventLeftIcons : eventLeftIconsTiny;
        const typeIconContainerStyle = this.props.showUserAvatar ? eventTypeIconContainer : eventTypeIconContainerTiny;

        const now = moment();
        const isOutdated = item.calendarEvent.dates.endDate.isSameOrBefore(now, 'date');

        const eventsContainerFlattened = StyleSheet.flatten([
            eventsContainer, 
            {
                width: Dimensions.get('window').width,
                opacity: isOutdated ? 0.40 : 1
            }
        ]);

        const descriptionStatus = this.descriptionStatus(item.calendarEvent);
        const avatar = item.employee ? <Avatar id={item.employee.employeeId} style={avatarOuterFrame} imageStyle={avatarImage} /> : null;

        return (
            <View style={eventsContainerFlattened} key={item.calendarEvent.calendarEventId}>
                <View style={eventRow}>
                    <View style={leftIconsStyle}>
                        <View style={typeIconContainerStyle}>
                            <CalendarEventIcon type={item.calendarEvent.type} style={eventIcon} />
                        </View>
                        {
                            this.props.showUserAvatar ? 
                            <View style={avatarContainer}>
                                {avatar}
                            </View> : null
                        }
                    </View>
                    <View style={eventTextContainer}>
                        <StyledText style={eventTitle}>{item.employee.name}</StyledText>
                        <StyledText style={eventDetails}>{descriptionStatus}</StyledText>
                        <StyledText style={eventDetails}>{this.descriptionFromTo(item.calendarEvent)}</StyledText>
                    </View>
                    <EventManagementToolset 
                        event={item.calendarEvent} 
                        employeeId={item.employee.employeeId} 
                        eventSetNewStatusAction={this.props.eventSetNewStatusAction}
                        canApprove={this.props.canApprove}
                        canReject={this.props.canReject} 
                    />
                </View>
            </View>
        );
    }

    private descriptionFromTo(event: CalendarEvent): string {
        let description: string;

        if (event.isWorkout || event.isDayoff ) {
            description = `on ${event.dates.startDate.format(this.eventDigitsDateFormat)} (${this.props.hoursToIntervalTitle(event.dates.startWorkingHour, event.dates.finishWorkingHour)})`;
        } else {
            description = `from ${event.dates.startDate.format(this.eventDigitsDateFormat)} to ${event.dates.endDate.format(this.eventDigitsDateFormat)}`;
        }

        return description;
    }

    private descriptionStatus(event: CalendarEvent): string {
        let description: string;

        if (event.isRequested) {
            description = `requests ${event.type.toLowerCase()}`;
        } else if (event.isApproved) {
            const prefix = event.dates.endDate.isAfter(moment(), 'date') ? 'has coming ' : 'on ';
            description = prefix + event.type.toLowerCase();
        } else if (event.isCompleted) {
            description = `has completed ${event.type.toLowerCase()}`;
        }

        return description;
    }
}
