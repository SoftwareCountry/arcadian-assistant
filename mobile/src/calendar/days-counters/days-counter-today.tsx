import React, { Component } from 'react';
import { View, Text, LayoutChangeEvent, ViewStyle, StyleSheet, PixelRatio } from 'react-native';
import { styles, daysCounterTodayStyles } from '../styles';

export  class DaysCounterToday extends Component {
    public render() {
        return (
            <View style={daysCounterTodayStyles.container}>
                <Text style={daysCounterTodayStyles.circleCurrentDay}>13</Text>
                <Text style={daysCounterTodayStyles.circleCurrentMonth}>February</Text>
            </View>
        );
    }
}