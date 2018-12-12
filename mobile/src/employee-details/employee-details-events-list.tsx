import React, { Component } from 'react';
import moment from 'moment';
import { Dimensions, FlatList, ListRenderItemInfo, StyleSheet, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { Avatar } from '../people/avatar';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent } from '../reducers/calendar/calendar-event.model';
import { EventManagementToolset } from './event-management-toolset';
import { CalendarEventIcon } from '../calendar/calendar-event-icon';
import { Nullable } from 'types';
import { EventActionsContainer } from './event-action-provider';
import { List } from 'immutable';

//============================================================================
interface EmployeeDetailsEventsListProps {
    eventActions: EventActionsContainer[];
    hoursToIntervalTitle: (startWorkingHour: number, finishWorkingHour: number) => Nullable<string>;
    showUserAvatar?: boolean;
}

//============================================================================
export class EmployeeDetailsEventsList extends Component<EmployeeDetailsEventsListProps> {
    private readonly eventDigitsDateFormat = 'DD/MM/YYYY';

    //----------------------------------------------------------------------------
    public render() {

        const events = this.prepareEvents();

        return (<FlatList
                scrollEnabled={false}
                data={events}
                keyExtractor={this.keyExtractor}
                renderItem={this.renderItem}/>
        );
    }

    //----------------------------------------------------------------------------
    private prepareEvents(): Nullable<EventActionsContainer[]> {
        const { eventActions } = this.props;

        if (!eventActions) {
            return null;
        }

        return List(eventActions).sort((left, right) => {
            const nameCompareResult = left.employee.name.localeCompare(right.employee.name);
            if (nameCompareResult !== 0) {
                return nameCompareResult;
            }

            return right.event.dates.startDate.valueOf() - left.event.dates.startDate.valueOf();
        }).toArray();
    }

    //----------------------------------------------------------------------------
    private keyExtractor = (item: EventActionsContainer) => item.event.calendarEventId;

    //----------------------------------------------------------------------------
    private renderItem = (itemInfo: ListRenderItemInfo<EventActionsContainer>) => {
        const action = itemInfo.item;
        const { eventsContainer, eventRow, eventLeftIcons, eventTypeIconContainer,
                eventLeftIconsTiny, eventTypeIconContainerTiny, eventIcon, eventTextContainer,
                eventTitle, eventDetails, avatarContainer, avatarOuterFrame,
                avatarImage } = layoutStylesForEmployeeDetailsScreen;

        const leftIconsStyle = this.props.showUserAvatar ? eventLeftIcons : eventLeftIconsTiny;
        const typeIconContainerStyle = this.props.showUserAvatar ? eventTypeIconContainer : eventTypeIconContainerTiny;

        const now = moment();
        const isOutdated = action.event.dates.endDate.isBefore(now, 'date');

        const eventsContainerFlattened = [
            eventsContainer,
            {
                width: Dimensions.get('window').width,
                opacity: isOutdated ? 0.40 : 1
            }
        ];

        const descriptionStatus = this.descriptionStatus(action.event);

        return (
            <View style={eventsContainerFlattened} key={action.event.calendarEventId}>
                <View style={eventRow}>
                    <View style={leftIconsStyle}>
                        <View style={typeIconContainerStyle}>
                            <CalendarEventIcon type={action.event.type} style={eventIcon as ViewStyle}/>
                        </View>
                        {
                            this.props.showUserAvatar ?
                                <View style={avatarContainer}>
                                    <Avatar photoUrl={action.employee.photoUrl}
                                            style={avatarOuterFrame as ViewStyle}
                                            imageStyle={avatarImage as ViewStyle}/>
                                </View> :
                                null
                        }
                    </View>
                    <View style={eventTextContainer}>
                        <StyledText style={eventTitle}>{action.employee.name}</StyledText>
                        <StyledText style={eventDetails}>{descriptionStatus}</StyledText>
                        <StyledText style={eventDetails}>{this.descriptionFromTo(action.event)}</StyledText>
                    </View>
                    <EventManagementToolset eventAction={action}/>
                </View>
            </View>
        );
    };

    //----------------------------------------------------------------------------
    private descriptionFromTo(event: CalendarEvent): string {
        let description: string;

        if (event.isWorkout || event.isDayoff) {
            description = `on ${event.dates.startDate.format(this.eventDigitsDateFormat)} (${this.props.hoursToIntervalTitle(event.dates.startWorkingHour, event.dates.finishWorkingHour)})`;
        } else {
            description = `from ${event.dates.startDate.format(this.eventDigitsDateFormat)} to ${event.dates.endDate.format(this.eventDigitsDateFormat)}`;
        }

        return description;
    }

    //----------------------------------------------------------------------------
    private descriptionStatus(event: CalendarEvent): string {
        let description = '';

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
