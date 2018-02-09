import React, { Component } from 'react';
import { View } from 'react-native';
import { selectedDayStyles } from '../styles';
import { StyledText } from '../../override/styled-text';

interface SelectedDayProps {
    day: string;
    month: string;
}

export  class SelectedDay extends Component<SelectedDayProps> {
    public render() {
        return (
            <View style={selectedDayStyles.container}>
                <StyledText style={selectedDayStyles.circleCurrentDay}>{this.props.day}</StyledText>
                <StyledText style={selectedDayStyles.circleCurrentMonth}>{this.props.month}</StyledText>
            </View>
        );
    }
}