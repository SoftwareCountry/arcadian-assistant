import React, { Component } from 'react';
import { View, Text } from 'react-native';
import { daysCounterTodayStyles } from '../styles';

interface DaysCounterTodayProps {
    day: string;
    month: string;
}

export  class DaysCounterToday extends Component<DaysCounterTodayProps> {
    public render() {
        return (
            <View style={daysCounterTodayStyles.container}>
                <Text style={daysCounterTodayStyles.circleCurrentDay}>{this.props.day}</Text>
                <Text style={daysCounterTodayStyles.circleCurrentMonth}>{this.props.month}</Text>
            </View>
        );
    }
}