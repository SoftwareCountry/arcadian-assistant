import React, { Component } from 'react';
import { View, Text, StyleSheet, ProgressBarAndroid, Animated, ViewStyle, StyleProp, RegisteredStyle, Button } from 'react-native';
import { styles } from '../styles';

interface DaysCounterProps {
    textValue: string;
    title: string[];
}

export class DaysCounter extends Component<DaysCounterProps> {

    public render() {
        return (
            <View style={styles.daysCounter}>
                <View style={styles.daysCounterContent}>
                    <Text style={styles.daysCounterContentValue}>{this.props.textValue}</Text>
                    {
                        this.props.title
                            ? this.props.title.map((x, index) => <Text key={index} style={styles.daysCounterContentTitle}>{x}</Text>)
                            : null
                    }
                </View>
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (<View style={styles.daysCounter}></View>);