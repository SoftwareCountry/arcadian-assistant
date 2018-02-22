import React, { Component } from 'react';
import { agendaStyles, agendaSelectedDayStyles } from './styles';
import { StyleSheet, Text, View } from 'react-native';
import { DaysCounters } from './days-counters/days-counters';
import { CalendarActionsButtonGroup } from './calendar-actions-button-group';
import { CalendarLegend } from './calendar-legend';
import { SelectedDay } from './days-counters/selected-day';
import { EventDialog } from './event-dialog/event-dialog';

export class Agenda extends Component {
    public render() {
        return (
            <View style={agendaStyles.container}>
                <View style={agendaSelectedDayStyles.container}>
                    <SelectedDay />
                    <CalendarLegend />
                </View>
                <View>
                    <CalendarActionsButtonGroup />
                    <EventDialog />
                </View>
            </View>
        );
    }
}

