import React, { Component } from 'react';
import { IntervalProps } from './calendar-page-interval';
import { PixelRatio, StyleSheet, View, ViewStyle } from 'react-native';
import { calendarIntervalStyles, intervalMargin } from './styles';
import Style from '../layout/style';

interface IntervalBoundaryProps extends IntervalProps {
    boundary: 'full' | 'left' | 'right';
}

type IntervalBoundaryColors = { [boundary: string]: { left: string; right: string } };

export class IntervalBoundary extends Component<IntervalBoundaryProps> {
    private readonly margin = 0.65;

    public render() {
        const { size, boundary } = this.props;

        const circleSize = PixelRatio.roundToNearestPixel(size - (size * this.margin));
        const circleTopDistance = size * intervalMargin;

        const containerStyles = StyleSheet.flatten([
            calendarIntervalStyles.container,
            calendarIntervalStyles.boundary,
            {
                justifyContent: 'center',
                backgroundColor: 'transparent',
                height: size,
                width: size - (size * intervalMargin),
                left: '25%',
                opacity: 1
            },
            this.props.style
        ]);

        const circleRadius = PixelRatio.roundToNearestPixel(circleSize / 2);

        const circleStyles = StyleSheet.flatten([
            {
                borderRadius: circleRadius,
                height: circleSize,
                width: circleSize,
                backgroundColor: Style.color.white,
                justifyContent: 'center',
                alignItems: 'center',
                flexDirection: 'row',
                position: 'absolute',
                bottom: 0,
                right: 0
            },
            this.props.circleStyle
        ]);

        const boundaries = boundary === 'full'
            ? this.renderFullBoundary(circleStyles)
            : this.renderHalfBoundary(circleStyles);

        return (
            <View style={containerStyles}>
                {
                    boundaries
                }
            </View>
        );
    }

    private halfCircleSizes(): { halfCircleSize: number, halfCircleRadius: number } {
        const halfCircleSize = Math.round(this.props.size - (this.props.size * (this.margin + 0.08)));
        const halfCircleRadius = Math.round(halfCircleSize / 2);

        return { halfCircleSize, halfCircleRadius };
    }

    private renderFullBoundary(circleStyles: ViewStyle) {
        const { halfCircleSize, halfCircleRadius } = this.halfCircleSizes();

        const fullStyles = StyleSheet.flatten([
            {
                borderRadius: halfCircleRadius,
                height: halfCircleSize,
                width: halfCircleSize,
                backgroundColor: this.props.color
            }
        ]);

        return (
            <View style={circleStyles}>
                <View style={fullStyles}></View>
            </View>
        );
    }

    private renderHalfBoundary(circleStyles: ViewStyle) {
        const { halfCircleSize, halfCircleRadius } = this.halfCircleSizes();

        const colors: IntervalBoundaryColors = {
            'left': { left: this.props.color, right: 'transparent' },
            'right': { left: 'transparent', right: this.props.color },
        };

        const leftHalfStyles = StyleSheet.flatten([
            {
                borderRadius: halfCircleSize,
                height: halfCircleSize,
                width: halfCircleRadius,
                backgroundColor: colors[this.props.boundary].left,
                borderTopRightRadius: 0,
                borderBottomRightRadius: 0
            }
        ]);

        const rightHalfStyles = StyleSheet.flatten([
            {
                borderRadius: halfCircleSize,
                height: halfCircleSize,
                width: halfCircleRadius,
                backgroundColor: colors[this.props.boundary].right,
                borderTopLeftRadius: 0,
                borderBottomLeftRadius: 0
            }
        ]);

        return (
            <View style={circleStyles}>
                <View style={leftHalfStyles}></View>
                <View style={rightHalfStyles}></View>
            </View>
        );
    }
}
