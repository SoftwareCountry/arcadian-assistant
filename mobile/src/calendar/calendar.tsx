import React, { Component } from 'react';
import { View, Text, StyleSheet, ViewStyle } from 'react-native';
import { calendarScreenLayout, calendarStyles } from './styles';
import moment from 'moment';

export class Calendar extends Component {
    public render() {
        // TODO: temp
        const today = moment();

        return (
            <View style={calendarStyles.container}>
                <Text style={calendarStyles.containerTitle}>{today.format('MMMM YYYY')}</Text>
                <View></View>
            </View>
        );
    }
}

