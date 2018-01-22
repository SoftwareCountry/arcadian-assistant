import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { DaysCounters } from './days-counters/days-counters';

export class CalendarScreen extends Component {

    public render() {
        return <View>
            <DaysCounters />
        </View>;
    }
}