import React, { Component } from 'react';
import moment from 'moment';
import { Dimensions, FlatList, ListRenderItemInfo, TouchableOpacity, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { Avatar } from '../people/avatar';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
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

        const descriptionStatus = this.descriptionStatus(action.event);
        const description = this.descriptionFromTo(action.event);

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
                        <StyledText style={eventDetails}>{descriptionStatus}</StyledText>
                        <StyledText style={eventDetails}>{description}</StyledText>
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
            <TouchableOpacity onPress={() => {this.props.onAvatarClicked(employee); }} style={avatarContainer}>
                <Avatar photoUrl={employee.photoUrl}
                        style={avatarOuterFrame as ViewStyle}
                        imageStyle={avatarImage as ViewStyle}/>
            </TouchableOpacity>
        );
    }

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

        switch (this.preprocessStatus(event)) {
            case CalendarEventStatus.Requested:
                description = `requests ${event.type.toLowerCase()}`;
                break;
            case CalendarEventStatus.Approved:
                const prefix = event.dates.endDate.isAfter(moment(), 'date') ? 'has coming ' : 'on ';
                description = prefix + event.type.toLowerCase();
                break;
            case CalendarEventStatus.AccountingReady:
                description = `has ${event.type.toLowerCase()} documents ready`;
                break;
            case CalendarEventStatus.Completed:
                description = `has completed ${event.type.toLowerCase()}`;
                break;
            default:
                break;
        }

        return description;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private preprocessStatus(event: CalendarEvent): CalendarEventStatus {
        if (!event.isVacation) {
            return event.status;
        }

        switch (event.status) {
            case CalendarEventStatus.Requested:
            case CalendarEventStatus.Approved:
                return CalendarEventStatus.Requested;
            case CalendarEventStatus.Processed:
                return CalendarEventStatus.Approved;
            default:
                return event.status;
        }
    }
}

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): EmployeeDetailsEventsListDispatchProps => ({
    onAvatarClicked: (employee: Employee) => dispatch(openEmployeeDetails(employee.employeeId)),
});

export const EmployeeDetailsEventsList = connect(null, mapDispatchToProps)(EmployeeDetailsEventsListImpl);
