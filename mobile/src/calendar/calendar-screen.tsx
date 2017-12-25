import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';

export class CalendarScreen extends Component {

    public render() {
        return <View style={styles.container}>
            <Text>Calendar</Text>
        </View>;
    }
}