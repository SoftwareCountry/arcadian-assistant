import React, { Component } from 'react';
import { View, StyleSheet } from 'react-native';
import { styles } from '../styles';

interface TileIndicatorProps {
    color: string;
}

export class DaysCounterIndicator extends Component<TileIndicatorProps> {
    public render() {
        const indicatorStyles = StyleSheet.create({
            indicator: {
                backgroundColor: this.props.color,
                flex: 1
            }
        });

        return (
            <View style={styles.daysCounterIndicator}>
                <View style={indicatorStyles.indicator} />
            </View>
        );
    }
}