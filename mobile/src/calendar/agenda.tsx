import React, { Component } from 'react';
import { agendaStyles, agendaTodayStyles } from './styles';
import { StyleSheet, Text, View } from 'react-native';
import { DaysCounters } from './days-counters/days-counters';
import { AgendaButtons } from './agenda-buttons';
import { Today } from './today';
import { CalendarLegend } from './calendar-legend';

export class Agenda extends Component {
    public render() {
        return (
            <View style={agendaStyles.container}>
                <View style={agendaTodayStyles.container}>
                    <Today />
                    <CalendarLegend />
                </View>
                <AgendaButtons />
            </View>
        );
    }
}

