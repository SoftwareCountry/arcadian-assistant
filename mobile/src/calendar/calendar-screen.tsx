import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { DaysCounters } from './days-counters/days-counters';
import { calendarScreenLayout } from './styles';

export class CalendarScreen extends Component {

    public render() {
        return <View style={styles.container}>
            <DaysCounters />
            <Text style={calendarScreenLayout.calendar}>Calendar</Text>
            <Text style={calendarScreenLayout.agenda}>Agenda</Text>
        </View>;
    }
}