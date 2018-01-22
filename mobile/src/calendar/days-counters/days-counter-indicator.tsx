import React, { Component } from 'react';
import { View, StyleSheet } from 'react-native';
import { styles, colors } from '../styles';

type RegisteredStyles =  { [propName: string]: any };

interface TileIndicatorStyles {
    left: RegisteredStyles;
    all: RegisteredStyles;
}

interface TileIndicatorProps {
    value: number;
    leftColor: string;
    allColor: string;
}

export class DaysCounterIndicator extends Component<TileIndicatorProps> {
    private styles: TileIndicatorStyles;
    private readonly allDays = 100;

    public render() {
        this.calculateStyles();

        return (
            <View style={styles.daysCounterIndicator}>
                <View style={this.styles.left} />
                <View style={this.styles.all} />
            </View>
        );
    }

    private calculateStyles() {
        this.styles = StyleSheet.create({
            left: {
                backgroundColor: this.props.leftColor,
                flexGrow: this.props.value
            },
            all: {
                backgroundColor: this.props.allColor,
                flexGrow: this.allDays - this.props.value
            }
        });
    }
}