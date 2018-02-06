import React, { Component } from 'react';
import { View, StyleSheet } from 'react-native';
import { daysCounterSelectedDayStyles } from '../styles';

interface DaysCounterTriangleProps {
    containerWidth: number;
}

export class DaysCounterTriangle extends Component<DaysCounterTriangleProps> {
    public render() {
        const borderWidth = this.props.containerWidth / 2;
        const rectangleStyles = StyleSheet.flatten([
            daysCounterSelectedDayStyles.triangle,
            {
                borderLeftWidth: borderWidth,
                borderRightWidth: borderWidth,
            }
        ]);

        return <View style={rectangleStyles}></View>;
    }
}

