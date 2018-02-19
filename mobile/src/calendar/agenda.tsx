import React, { Component } from 'react';
import { calendarScreenLayout, agendaStyles } from './styles';
import { StyleSheet, Text, View } from 'react-native';
import { DaysCounters } from './days-counters/days-counters';
import { AgendaButtons } from './agenda-buttons';
import { Today } from './today';

export class Agenda extends Component {
    public render() {
        return (
            <View style={agendaStyles.container}>
                <Today />
                <AgendaButtons />
            </View>
        );
    }
}

