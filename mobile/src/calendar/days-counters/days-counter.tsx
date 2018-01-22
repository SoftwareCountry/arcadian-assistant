import React, { Component } from 'react';
import { View, Text, StyleSheet, ProgressBarAndroid, Animated, ViewStyle, StyleProp, RegisteredStyle, Button } from 'react-native';
import { styles, colors } from '../styles';
import { DaysCounterIndicator } from './days-counter-indicator';

interface DaysCounterDefaultProps {
    showAllDays?: boolean;
}

interface DaysCounterProps extends DaysCounterDefaultProps {
    leftDays: number;
    leftColor: string;
    allDays: number;
    allColor: string;
    title: string;
}

export class DaysCounter extends Component<DaysCounterProps> {
    public static readonly defaultProps: DaysCounterDefaultProps = {
        showAllDays: true
    };

    public render() {

        const indicatorValue = this.props.allDays 
            ? (this.props.leftDays / this.props.allDays) * 100
            : 0;

        return (
            <View style={styles.daysCounter}>
                <View style={styles.daysCounterContent}>
                    <Text>
                        <Text style={styles.daysCounterLeftDays}>{this.props.leftDays}</Text>
                        { this.props.showAllDays ? <Text style={styles.daysCounterAllDays}> / {this.props.allDays}</Text> : null }
                    </Text>
                    <Text style={styles.daysCounterTitle}>{this.props.title}</Text>
                </View>
                <DaysCounterIndicator value={indicatorValue} leftColor={this.props.leftColor} allColor={this.props.allColor} />
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (<View style={styles.daysCounter}></View>);