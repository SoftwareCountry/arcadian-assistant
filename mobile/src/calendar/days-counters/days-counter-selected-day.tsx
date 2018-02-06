import React, { Component } from 'react';
import { View, Text } from 'react-native';
import { daysCounterSelectedDayStyles } from '../styles';

interface DaysCounterSelectedDayProps {
    day: string;
    month: string;
}

export  class DaysCounterSelectedDay extends Component<DaysCounterSelectedDayProps> {
    public render() {
        return (
            <View style={daysCounterSelectedDayStyles.container}>
                <Text style={daysCounterSelectedDayStyles.circleCurrentDay}>{this.props.day}</Text>
                <Text style={daysCounterSelectedDayStyles.circleCurrentMonth}>{this.props.month}</Text>
            </View>
        );
    }
}