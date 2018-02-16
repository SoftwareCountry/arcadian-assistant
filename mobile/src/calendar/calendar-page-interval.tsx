import React, { Component } from 'react';
import { calendarIntervalStyles, intervalMargin } from './styles';
import { View, StyleSheet, PixelRatio } from 'react-native';

interface HalfIntervalProps {
    size: number;
    align: 'left' | 'right';
    color: string;
}

export class HalfInterval extends Component<HalfIntervalProps> {
    public render() {
        const { containerStyles, circleStyles, intervalStyles } = this.calculateStyles();

        return (
            <View style={containerStyles}>
                <View style={circleStyles}></View>
                <View style={intervalStyles}></View>
            </View>
        );
    }

    private calculateStyles() {
        const margin = (this.props.size * intervalMargin);
        const size = this.props.size - margin;

        const containerStyles = StyleSheet.flatten([
            calendarIntervalStyles.container,
            this.props.align === 'right'
                ? { flexDirection: 'row-reverse' }
                : {}
        ]);

        const circleStyles = StyleSheet.flatten([
            {
                borderRadius: size,
                height: size,
                width: PixelRatio.roundToNearestPixel(size / 2),
                backgroundColor: this.props.color
            },
            this.props.align === 'right'
                ? {
                    borderTopLeftRadius: 0,
                    borderBottomLeftRadius: 0,
                }
                : {
                    borderTopRightRadius: 0,
                    borderBottomRightRadius: 0,
                }
        ]);

        const intervalStyles = StyleSheet.flatten([
            calendarIntervalStyles.halfInterval,
            {
                height: size,
                backgroundColor: this.props.color
            }
        ]);

        return {
            containerStyles,
            circleStyles,
            intervalStyles
        };
    }
}

type IntervalProps = { size: number, color: string };

export const StartInterval = (props: IntervalProps) => <HalfInterval size={props.size} align={'left'} color={props.color} />;

export const EndInterval = (props: IntervalProps) => <HalfInterval size={props.size} align={'right'} color={props.color} />;

export const Interval = (props: IntervalProps) => {
    const margin = (props.size * intervalMargin);
    const size = props.size - margin;

    const containerStyles = StyleSheet.flatten([
        calendarIntervalStyles.container,
        { justifyContent: 'center' }
    ]);

    const intervalStyles = StyleSheet.flatten([
        calendarIntervalStyles.interval,
        {
            height: size,
            backgroundColor: props.color
        }
    ]);

    return (
        <View style={calendarIntervalStyles.container}>
            <View style={intervalStyles}></View>
        </View>
    );
};

export const IntervalBoundary = (props: IntervalProps) => {
    const margin = (props.size * intervalMargin);
    const size = props.size - margin;

    const containerStyles = StyleSheet.flatten([
        calendarIntervalStyles.container,
        { justifyContent: 'center' }
    ]);
    
    const circleStyles = StyleSheet.flatten([
        {
            borderRadius: PixelRatio.roundToNearestPixel(size / 2),
            height: size,
            width: size,
            backgroundColor: props.color
        }
    ]);

    return (
        <View style={containerStyles}>
            <View style={circleStyles}></View>
        </View>
    );
};