import React, { Component } from 'react';
import { View, Text, StyleSheet, ViewStyle } from 'react-native';
import { calendarScreenLayout, calendarStyles } from './styles';

export class Calendar extends Component {
    public render() {
        return (
            <View style={calendarStyles.container}>
                <Text style={calendarStyles.containerTitle}>January 2018</Text>
                <View></View>
            </View>
        );
    }
}

