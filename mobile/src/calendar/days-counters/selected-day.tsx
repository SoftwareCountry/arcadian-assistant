import React, { Component } from 'react';
import { View, Text } from 'react-native';
import { selectedDayStyles } from '../styles';
import { ArcadiaText } from '../../override/acradia-text';

interface SelectedDayProps {
    day: string;
    month: string;
}

export  class SelectedDay extends Component<SelectedDayProps> {
    public render() {
        return (
            <View style={selectedDayStyles.container}>
                <ArcadiaText style={selectedDayStyles.circleCurrentDay}>{this.props.day}</ArcadiaText>
                <ArcadiaText style={selectedDayStyles.circleCurrentMonth}>{this.props.month}</ArcadiaText>
            </View>
        );
    }
}