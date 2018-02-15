import React from 'react';
import { calendarPeriodStyles } from './styles';
import { View, StyleSheet } from 'react-native';

const HalfPeriod = (props: {
    size: number,
    align: 'left' | 'right',
    color: string
}) => {
    const margin = (props.size * 0.2);
    const size = props.size - margin;

    const containerStyles = StyleSheet.flatten([
        calendarPeriodStyles.container,
        props.align === 'right'
            ? { flexDirection: 'row-reverse' }
            : {}
    ]);

    const circleStyles = StyleSheet.flatten([
        {
            borderRadius: size,
            height: size,
            width: size / 2,
            backgroundColor: props.color
        },
        props.align === 'right'
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
            backgroundColor: props.color
        }
    ]);

    return (
        <View style={containerStyles}>
            <View style={circleStyles}></View>
            <View style={periodStyles}></View>
        </View>
    );
};

type PeriodProps = { size: number, color: string };

export const StartPeriod = (props: PeriodProps) => <HalfPeriod size={props.size} align={'left'} color={props.color} />;

export const EndPeriod = (props: PeriodProps) => <HalfPeriod size={props.size} align={'right'} color={props.color} />;

export const Period = (props: PeriodProps) => {
    const margin = (props.size * 0.2);
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
    const margin = (props.size * 0.2);
    const size = props.size - margin;

    const containerStyles = StyleSheet.flatten([
        calendarPeriodStyles.container,
        { justifyContent: 'center' }
    ]);

    const circleStyles = StyleSheet.flatten([
        {
            borderRadius: size / 2,
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