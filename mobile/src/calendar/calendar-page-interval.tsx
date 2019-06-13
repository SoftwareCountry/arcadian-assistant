import React, { Component } from 'react';
import { calendarIntervalStyles, intervalMargin } from './styles';
import { StyleSheet, View, ViewStyle } from 'react-native';

export interface IntervalProps {
    size: number;
    color: string;
    style?: ViewStyle;
    circleStyle?: ViewStyle;
}

interface HalfIntervalProps extends IntervalProps {
    align: 'left' | 'right';
}

export class HalfInterval extends Component<HalfIntervalProps> {
    public render() {
        const { containerStyles, circleStyles, intervalStyles } = this.calculateStyles();

        return (
            <View style={containerStyles}>
                <View style={circleStyles}/>
                <View style={intervalStyles}/>
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
                : {},
            this.props.style
        ]);

        const circleStyles = StyleSheet.flatten([
            {
                borderRadius: size,
                height: Math.round(size),
                width: Math.round(size),
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
                height: Math.round(size),
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

export const StartInterval = (props: IntervalProps) => <HalfInterval size={props.size} align={'left'}
                                                                     color={props.color} style={props.style}/>;

export const EndInterval = (props: IntervalProps) => <HalfInterval size={props.size} align={'right'} color={props.color}
                                                                   style={props.style}/>;

export const Interval = (props: IntervalProps) => {
    const margin = (props.size * intervalMargin);
    const size = props.size - margin;

    const containerStyles = StyleSheet.flatten([
        calendarIntervalStyles.container,
        { justifyContent: 'center' },
        props.style
    ]);

    const intervalStyles = StyleSheet.flatten([
        calendarIntervalStyles.interval,
        {
            height: Math.round(size),
            backgroundColor: props.color
        }
    ]);

    return (
        <View style={containerStyles}>
            <View style={intervalStyles}/>
        </View>
    );
};
