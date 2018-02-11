import React, { Component } from 'react';
import { calendarScreenLayout, agendaStyles } from './styles';
import { StyleSheet, Text, View } from 'react-native';
import { DaysCounters } from './days-counters/days-counters';

export class Agenda extends Component {
    public render() {
        return (
            <View style={agendaStyles.container}>
                <DaysCounters />
            </View>
        );
    }
}

