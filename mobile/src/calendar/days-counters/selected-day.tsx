import React, { Component } from 'react';
import { View, Text } from 'react-native';
import { selectedDayStyles } from '../styles';

interface SelectedDayProps {
    day: string;
    month: string;
}

export  class SelectedDay extends Component<SelectedDayProps> {
    public render() {
        return (
            <View style={selectedDayStyles.container}>
                <Text style={selectedDayStyles.circleCurrentDay}>{this.props.day}</Text>
                <Text style={selectedDayStyles.circleCurrentMonth}>{this.props.month}</Text>
            </View>
        );
    }
}