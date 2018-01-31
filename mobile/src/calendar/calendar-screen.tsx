import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { calendarScreenLayout } from './styles';
import { Calendar } from './calendar';
import { Agenda } from './agenda';

export class CalendarScreen extends Component {
    public render() {
        return <View style={styles.container}>
            <Calendar />
            <Agenda />
        </View>;
    }
}