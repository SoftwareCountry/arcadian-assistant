import React, { Component } from 'react';
import { agendaStyles, agendaSelectedDayStyles } from './styles';
import { StyleSheet, Text, View } from 'react-native';
import { DaysCounters } from './days-counters/days-counters';
import { AgendaButtons } from './agenda-buttons';
import { CalendarLegend } from './calendar-legend';
import { SelectedDay } from './days-counters/selected-day';

export class Agenda extends Component {
    public render() {
        return (
            <View style={agendaStyles.container}>
                <View style={agendaSelectedDayStyles.container}>
                    <SelectedDay />
                    <CalendarLegend />
                </View>
                <AgendaButtons />
            </View>
        );
    }
}

