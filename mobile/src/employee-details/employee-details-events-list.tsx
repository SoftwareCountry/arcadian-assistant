import React, { Component } from 'react';
import { Map } from 'immutable';
import { View, Text, StyleSheet, FlatList, ListRenderItemInfo, ViewStyle, Dimensions, TouchableOpacity } from 'react-native';

import { StyledText } from '../override/styled-text';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { CalendarEvent, CalendarEventType } from '../reducers/calendar/calendar-event.model';
import { eventDialogTextDateFormat } from '../calendar/event-dialog/event-dialog-base';

interface EmployeeDetailsEventsListProps {
    events: CalendarEvent[];
}

export class EmployeeDetailsEventsList extends Component<EmployeeDetailsEventsListProps> {
    private eventTypeToGlyphIcon: Map<string, string> = Map([
        [CalendarEventType.Dayoff, 'dayoff'],
        [CalendarEventType.Vacation, 'vacation'],
        [CalendarEventType.Sickleave, 'sick_leave'],
        [CalendarEventType.Workout, 'workout']
    ]);

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
        const { eventsContainer, eventRow, eventIcon, eventTitle } = layoutStylesForEmployeeDetailsScreen;

        const eventsContainerFlattened = StyleSheet.flatten([
            eventsContainer, {width: Dimensions.get('window').width}
        ]);

        return (
                <View style={eventsContainerFlattened} key={item.calendarEventId}>
                    <View style={eventRow}>
                        <ApplicationIcon name={this.eventTypeToGlyphIcon.get(item.type)} style={eventIcon} />
                        <StyledText style={eventTitle}>{item.type} starts on {item.dates.startDate.format(eventDialogTextDateFormat)} and completes on {item.dates.endDate.format(eventDialogTextDateFormat)} ({item.status})</StyledText>
                        <View style={{ paddingLeft: 10, flexDirection: 'column', alignItems: 'center' }}>
                            <TouchableOpacity><Text style={{ fontSize: 9, color: 'green' }}>APPROVE</Text></TouchableOpacity>
                            <TouchableOpacity><Text style={{ fontSize: 9, color: 'red' }}>REJECT</Text></TouchableOpacity>
                        </View>
                    </View>
                </View>
        );
    }

}
