import React, { Component } from 'react';
import { View, Text, StyleSheet, ProgressBarAndroid, Animated, ViewStyle, StyleProp, RegisteredStyle, Button } from 'react-native';
import { styles } from '../styles';

interface DaysCounterProps {
    textValue: string;
    title: string;
}

export class DaysCounter extends Component<DaysCounterProps> {

    public render() {
        return (
            <View style={styles.daysCounter}>
                <View style={styles.daysCounterContent}>
                    <Text style={styles.daysCounterContentValue}>{this.props.textValue}</Text>
                    <Text style={styles.daysCounterContentTitle}>{this.props.title}</Text>
                </View>
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (<View style={styles.daysCounter}></View>);