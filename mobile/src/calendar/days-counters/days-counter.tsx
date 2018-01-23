import React, { Component } from 'react';
import { View, Text, StyleSheet, ProgressBarAndroid, Animated, ViewStyle, StyleProp, RegisteredStyle, Button } from 'react-native';
import { styles } from '../styles';
import { DaysCounterIndicator } from './days-counter-indicator';

interface DaysCounterDefaultProps {
    showIndicator?: boolean;
    indicatorColor?: string;    
}

interface DaysCounterProps extends DaysCounterDefaultProps {
    value: string;
    title: string;
}

export class DaysCounter extends Component<DaysCounterProps> {
    public static readonly defaultProps: DaysCounterDefaultProps = {
        showIndicator: true,
        indicatorColor: ''
    };

    public render() {
        return (
            <View style={styles.daysCounter}>
                <View style={styles.daysCounterContent}>
                    <Text style={styles.daysCounterContentValue}>{this.props.value}</Text>
                    <Text style={styles.daysCounterContentTitle}>{this.props.title}</Text>
                </View>
                { this.props.showIndicator ? <DaysCounterIndicator color={this.props.indicatorColor} /> : null }
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (<View style={styles.daysCounter}></View>);