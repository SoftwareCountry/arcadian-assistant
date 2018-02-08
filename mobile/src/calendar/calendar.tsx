import React, { Component } from 'react';
import { View, Text, StyleSheet, ViewStyle } from 'react-native';
import { calendarScreenLayout, calendarStyles } from './styles';
import moment from 'moment';
import { ArcadiaText } from '../override/acradia-text';

export class Calendar extends Component {
    public render() {
        // TODO: temp
        const today = moment();

        return (
            <View style={calendarStyles.container}>
                <ArcadiaText style={calendarStyles.containerTitle}>{today.format('MMMM YYYY')}</ArcadiaText>
                <View></View>
            </View>
        );
    }
}

