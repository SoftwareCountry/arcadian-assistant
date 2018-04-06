import React, { Component } from 'react';
import { View, LayoutChangeEvent, TouchableOpacity } from 'react-native';
import { IntervalBoundary } from '../calendar-page-interval-boundary';
import { switchDayoffTypeStyles } from './styles';
import { CalendarEventsColor } from '../styles';
import { IntervalType } from '../../reducers/calendar/calendar.model';

interface SwitchDayoffTypeProps {
    onIntervalTypeSelected: (selectedType: IntervalType) => void;
    isWorkout: boolean;
}

interface SwitchDayoffTypeState {
    containerHeight: number;
}

export class SwitchDayoffType extends Component<SwitchDayoffTypeProps, SwitchDayoffTypeState> {
    constructor(props: SwitchDayoffTypeProps) {
        super(props);
        this.state = {
            containerHeight: 0
        };
    }

    public render() {
        const boundaryColor = this.props.isWorkout
            ? CalendarEventsColor.workout
            : CalendarEventsColor.dayoff;

        return (
            <View style={switchDayoffTypeStyles.container} onLayout={this.onContainerLayout}>
            {
                this.state.containerHeight !== 0 ?
                    <View style={switchDayoffTypeStyles.intervalBoundaries}>
                        <TouchableOpacity onPress={this.onFirstHalfSelected}>
                            <IntervalBoundary
                                color={boundaryColor}
                                boundary={'left'}
                                size={this.state.containerHeight}
                                style={switchDayoffTypeStyles.intervalBoundary}
                                circleStyle={switchDayoffTypeStyles.intervalBoundary} />
                        </TouchableOpacity>

                        <TouchableOpacity onPress={this.onSecondHalfSelected}>
                            <IntervalBoundary
                                color={boundaryColor}
                                boundary={'right'}
                                size={this.state.containerHeight}
                                style={switchDayoffTypeStyles.intervalBoundary}
                                circleStyle={switchDayoffTypeStyles.intervalBoundary} />
                        </TouchableOpacity>

                        <TouchableOpacity onPress={this.onFullDaySelected}>
                            <IntervalBoundary
                                color={boundaryColor}
                                boundary={'full'}
                                size={this.state.containerHeight}
                                style={switchDayoffTypeStyles.intervalBoundary}
                                circleStyle={switchDayoffTypeStyles.intervalBoundary} />
                        </TouchableOpacity>
                    </View>
                : null
            }
            </View>
        );
    }

    private onContainerLayout = (e: LayoutChangeEvent) => {
        this.setState({ containerHeight: e.nativeEvent.layout.height });
    }

    private onFirstHalfSelected = () => {
        this.props.onIntervalTypeSelected(IntervalType.IntervalLeftBoundary);
    }

    private onSecondHalfSelected = () => {
        this.props.onIntervalTypeSelected(IntervalType.IntervalRightBoundary);
    }

    private onFullDaySelected = () => {
        this.props.onIntervalTypeSelected(IntervalType.IntervalFullBoundary);
    }
}