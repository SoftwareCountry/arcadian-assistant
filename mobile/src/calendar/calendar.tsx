import React, { Component } from 'react';
import { View, StyleSheet, ViewStyle } from 'react-native';
import { calendarScreenLayout, calendarStyles } from './styles';
import moment from 'moment';
import { StyledText } from '../override/styled-text';

export class Calendar extends Component {
    public render() {
        // TODO: temp
        const today = moment();

        return (
            <View style={calendarStyles.container}>
                <StyledText style={calendarStyles.containerTitle}>{today.format('MMMM YYYY')}</StyledText>
                <View></View>
            </View>
        );
    }
}

