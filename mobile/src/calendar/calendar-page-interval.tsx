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

export const IntervalBoundary = (props: IntervalProps & { boundary: 'full' | 'left'| 'right' }) => {
    const margin = 0.65;
    const circleSize = PixelRatio.roundToNearestPixel(props.size - (props.size * margin));
    const halfCircleSize = PixelRatio.roundToNearestPixel(props.size - (props.size * (margin + 0.08)));

    const colors: {
        [boundary: string]: { left: string; right: string }
    } = {
        'full': { left: props.color, right: props.color },
        'left': { left: props.color, right: 'transparent' },
        'right': { left: 'transparent', right: props.color },
    };

    const circleTopDistance = props.size * intervalMargin;

    const containerStyles = StyleSheet.flatten([
        calendarIntervalStyles.container,
        calendarIntervalStyles.boundary,
        {
            justifyContent: 'center',
            backgroundColor: 'transparent',
            height: props.size,
            width: props.size - (props.size * intervalMargin),
            left: '25%',
            opacity: 1
        }
    ]);

    const circleRadius = PixelRatio.roundToNearestPixel(circleSize / 2);

    const circleStyles = StyleSheet.flatten([
        {
            borderRadius: circleRadius,
            height: circleSize,
            width: circleSize,
            backgroundColor: '#fff',
            justifyContent: 'center',
            alignItems: 'center',
            flexDirection: 'row',
            position: 'absolute',
            bottom: 0,
            right: 0
        }
    ]);

    const halfCircleRadius = PixelRatio.roundToNearestPixel(halfCircleSize / 2);

    const leftHalfStyles = StyleSheet.flatten([
        {
            borderRadius: halfCircleSize,
            height: halfCircleSize,
            width: halfCircleRadius,
            backgroundColor: colors[props.boundary].left,
            borderTopRightRadius: 0,
            borderBottomRightRadius: 0
        }
    ]);

    const rightHalfStyles = StyleSheet.flatten([
        {
            borderRadius: halfCircleSize,
            height: halfCircleSize,
            width: halfCircleRadius,
            backgroundColor: colors[props.boundary].right,
            borderTopLeftRadius: 0,
            borderBottomLeftRadius: 0
        }
    ]);

    return (
        <View style={containerStyles}>
            <View style={circleStyles}>
                <View style={leftHalfStyles}></View>
                <View style={rightHalfStyles}></View>
            </View>
        </View>
    );
};