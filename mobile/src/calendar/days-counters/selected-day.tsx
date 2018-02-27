import React, { Component } from 'react';
import { View } from 'react-native';
import { selectedDayStyles } from '../styles';
import { StyledText } from '../../override/styled-text';
import { Moment } from 'moment';

interface SelectedDayProps {
    day: Moment;
}

export  class SelectedDay extends Component<SelectedDayProps> {
    public render() {

        const day = this.props.day ? this.props.day.format('D') : '';
        const month = this.props.day ? this.props.day.format('MMMM') : '';

        return (
            <View style={selectedDayStyles.container}>
                <StyledText style={selectedDayStyles.circleCurrentDay}>{day}</StyledText>
                <StyledText style={selectedDayStyles.circleCurrentMonth}>{month}</StyledText>
            </View>
        );
    }
}