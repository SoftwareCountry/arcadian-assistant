/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import moment from 'moment';
import { Dimensions, FlatList, ListRenderItemInfo, TouchableOpacity, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { Avatar } from '../people/avatar';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import {
    CalendarEvent,
    CalendarEventStatus,
    CalendarEventType,
    DayOffWorkoutStatus,
    SickLeaveStatus,
    VacationStatus
} from '../reducers/calendar/calendar-event.model';
import { EventManagementToolset } from './event-management-toolset';
import { CalendarEventIcon } from '../calendar/calendar-event-icon';
import { Nullable } from 'types';
import { EventActionContainer, EventActionProvider } from './event-action-provider';
import { List } from 'immutable';
import { Employee } from '../reducers/organization/employee.model';
import { Action, Dispatch } from 'redux';
import { openEmployeeDetails } from '../navigation/navigation.actions';
import { connect } from 'react-redux';

//============================================================================
interface EmployeeDetailsEventsListProps {
    eventActions: EventActionContainer[];
    hoursToIntervalTitle: (startWorkingHour: number, finishWorkingHour: number) => Nullable<string>;
    showUserAvatar?: boolean;
}

//============================================================================
interface EmployeeDetailsEventsListDispatchProps {
    onAvatarClicked: (employee: Employee) => void;
}

//============================================================================
class EmployeeDetailsEventsListImpl extends Component<EmployeeDetailsEventsListProps & EmployeeDetailsEventsListDispatchProps> {

    private readonly eventDigitsDateFormat = 'ddd DD/MM/YYYY';

    private readonly vacationDescriptions: { [key in VacationStatus]: string } = {
        [VacationStatus.Requested]: 'waiting for approvals',
        [VacationStatus.Approved]: 'preparing documents',
        [VacationStatus.AccountingReady]: 'documents are ready to sign',
        [VacationStatus.Processed]: 'confirmed',
        [VacationStatus.Cancelled]: 'cancelled',
        [VacationStatus.Rejected]: 'rejected',
    };

    private readonly dayOffWorkoutDescriptions: { [key in DayOffWorkoutStatus]: string } = {
        [DayOffWorkoutStatus.Requested]: 'waiting for approvals',
        [DayOffWorkoutStatus.Approved]: 'confirmed',
        [DayOffWorkoutStatus.Cancelled]: 'cancelled',
        [DayOffWorkoutStatus.Rejected]: 'rejected',
    };

    private readonly sickLeaveDescriptions: { [key in SickLeaveStatus]: string } = {
        [SickLeaveStatus.Completed]: 'completed',
        [SickLeaveStatus.Cancelled]: 'cancelled',
        [SickLeaveStatus.Requested]: 'confirmed',
    };

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
    private prepareEvents(): Nullable<EventActionContainer[]> {
        const { eventActions } = this.props;

        if (!eventActions) {
            return null;
        }

        return List(eventActions)
            .sort(EventActionProvider.compareEventActionContainers)
            .toArray();
    }

    //----------------------------------------------------------------------------
    private keyExtractor = (item: EventActionContainer) => item.event.calendarEventId;

    //----------------------------------------------------------------------------
    private renderItem = (itemInfo: ListRenderItemInfo<EventActionContainer>) => {
        const action = itemInfo.item;
        const {
            eventsContainer, eventRow, eventLeftIcons, eventTypeIconContainer,
            eventLeftIconsTiny, eventTypeIconContainerTiny, eventIcon, eventTextContainer,
            eventTitle, eventDetails
        } = layoutStylesForEmployeeDetailsScreen;

        const leftIconsStyle = this.props.showUserAvatar ? eventLeftIcons : eventLeftIconsTiny;
        const typeIconContainerStyle = this.props.showUserAvatar ? eventTypeIconContainer : eventTypeIconContainerTiny;
        const avatar = this.props.showUserAvatar ? this.avatar(action.employee) : null;

        const now = moment();
        const isOutdated = action.event.dates.endDate.isBefore(now, 'date');

        const eventsContainerFlattened = [
            eventsContainer,
            {
                width: Dimensions.get('window').width,
                opacity: isOutdated ? 0.40 : 1
            }
        ];

        const eventPeriodDescription = this.periodDescription(action.event);
        const eventStatusDescription = this.statusDescription(action.event);

        return (
            <View style={eventsContainerFlattened} key={action.event.calendarEventId}>
                <View style={eventRow}>
                    <View style={leftIconsStyle}>
                        <View style={typeIconContainerStyle}>
                            <CalendarEventIcon type={action.event.type} style={eventIcon as ViewStyle}/>
                        </View>
                        {
                            avatar
                        }
                    </View>
                    <View style={eventTextContainer}>
                        <StyledText style={eventTitle}>{action.employee.name}</StyledText>
                        <StyledText style={eventDetails}>{eventStatusDescription}</StyledText>
                        <StyledText style={eventDetails}>{eventPeriodDescription}</StyledText>
                    </View>
                    <EventManagementToolset eventAction={action}/>
                </View>
            </View>
        );
    };

    //----------------------------------------------------------------------------
    private avatar(employee: Employee): JSX.Element {
        const {
            avatarContainer, avatarOuterFrame, avatarImage
        } = layoutStylesForEmployeeDetailsScreen;

        return (
            <TouchableOpacity
                onPress={() => { this.props.onAvatarClicked(employee); }}
                style={avatarContainer}>

                <Avatar photoUrl={employee.photoUrl}
                        style={avatarOuterFrame as ViewStyle}
                        imageStyle={avatarImage as ViewStyle}/>

            </TouchableOpacity>
        );
    }

    //----------------------------------------------------------------------------
    private periodDescription(event: CalendarEvent): string {
        let description: string;

        const startDate = event.dates.startDate.format(this.eventDigitsDateFormat);
        const endDate = event.dates.endDate.format(this.eventDigitsDateFormat);
        const hours = this.props.hoursToIntervalTitle(event.dates.startWorkingHour, event.dates.finishWorkingHour);

        if (event.isWorkout || event.isDayOff) {
            description = `on ${startDate} (${hours})`;
        } else {
            description = `from ${startDate} to ${endDate}`;
        }

        return description;
    }

    //----------------------------------------------------------------------------
    private statusDescription(event: CalendarEvent): string {

        const status = event.status;

        switch (event.type) {
            case CalendarEventType.Vacation:
                return `Vacation: ${this.vacationDescriptions[status as VacationStatus]}`;
            case CalendarEventType.SickLeave:
                return `Sick leave: ${this.sickLeaveDescriptions[status as SickLeaveStatus]}`;
            case CalendarEventType.DayOff:
                return `Day off: ${this.dayOffWorkoutDescriptions[status as DayOffWorkoutStatus]}`;
            case CalendarEventType.Workout:
                return `Workout: ${this.dayOffWorkoutDescriptions[status as DayOffWorkoutStatus]}`;
        }
    }
}

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): EmployeeDetailsEventsListDispatchProps => ({
    onAvatarClicked: (employee: Employee) => dispatch(openEmployeeDetails(employee.employeeId)),
});

export const EmployeeDetailsEventsList = connect(null, mapDispatchToProps)(EmployeeDetailsEventsListImpl);
