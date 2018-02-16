import React, { Component } from 'react';
import { calendarPeriodStyles, periodMargin } from './styles';
import { View, StyleSheet, PixelRatio } from 'react-native';

interface HalfPeriodProps {
    size: number;
    align: 'left' | 'right';
    color: string;
}

export class HalfPeriod extends Component<HalfPeriodProps> {
    public render() {
        const { containerStyles, circleStyles, periodStyles } = this.calculateStyles();

        return (
            <View style={containerStyles}>
                <View style={circleStyles}></View>
                <View style={periodStyles}></View>
            </View>
        );
    }

    private calculateStyles() {
        const margin = (this.props.size * periodMargin);
        const size = this.props.size - margin;

        const containerStyles = StyleSheet.flatten([
            calendarPeriodStyles.container,
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

        const periodStyles = StyleSheet.flatten([
            calendarPeriodStyles.halfPeriod,
            {
                height: size,
                backgroundColor: this.props.color
            }
        ]);

        return {
            containerStyles,
            circleStyles,
            periodStyles
        };
    }
}

type PeriodProps = { size: number, color: string };

export const StartPeriod = (props: PeriodProps) => <HalfPeriod size={props.size} align={'left'} color={props.color} />;

export const EndPeriod = (props: PeriodProps) => <HalfPeriod size={props.size} align={'right'} color={props.color} />;

export const Period = (props: PeriodProps) => {
    const margin = (props.size * periodMargin);
    const size = props.size - margin;

    const containerStyles = StyleSheet.flatten([
        calendarPeriodStyles.container,
        { justifyContent: 'center' }
    ]);

    const periodStyles = StyleSheet.flatten([
        calendarPeriodStyles.period,
        {
            height: size,
            backgroundColor: props.color
        }
    ]);

    return (
        <View style={calendarPeriodStyles.container}>
            <View style={periodStyles}></View>
        </View>
    );
};

export const DotPeriod = (props: PeriodProps) => {
    const margin = (props.size * periodMargin);
    const size = props.size - margin;

    const containerStyles = StyleSheet.flatten([
        calendarPeriodStyles.container,
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