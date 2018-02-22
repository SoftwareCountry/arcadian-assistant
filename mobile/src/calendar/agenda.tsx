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
        const dialogOpened = false;

        const displayNone = StyleSheet.flatten({ display: 'none' });

        const controlsStyles = StyleSheet.flatten(dialogOpened ? displayNone : agendaStyles.controls);
        const dialogStyles = StyleSheet.flatten(dialogOpened ? agendaStyles.dialog : displayNone);

        return (
            <View style={agendaStyles.container}>
                <View style={controlsStyles}>
                    <View style={agendaSelectedDayStyles.container}>
                        <SelectedDay />
                        <CalendarLegend />
                    </View>
                    <CalendarActionsButtonGroup />
                </View>

                <View style={dialogStyles}>
                    <EventDialog />
                </View>
            </View>
        );
    }
}

