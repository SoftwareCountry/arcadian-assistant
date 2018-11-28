import React, { Component } from 'react';
import {View, LayoutChangeEvent, TouchableOpacity, ViewStyle} from 'react-native';
import { IntervalBoundary } from '../calendar-page-interval-boundary';
import { switchDayoffTypeStyles } from './styles';
import { CalendarEventsColor } from '../styles';
import { HoursCreditType } from '../../reducers/calendar/days-counters.model';

interface SelectorDayoffTypeProps {
    onTypeSelected: (selectedType: HoursCreditType) => void;
}

interface SelectorDayoffTypeState {
    containerHeight: number;
}

export class SelectorDayoffType extends Component<SelectorDayoffTypeProps, SelectorDayoffTypeState> {
    constructor(props: SelectorDayoffTypeProps) {
        super(props);
        this.state = {
            containerHeight: 0
        };
    }

    public render() {
        return (
            <View style={switchDayoffTypeStyles.container} onLayout={this.onContainerLayout}>
            {
                this.state.containerHeight !== 0 ?
                    <View style={switchDayoffTypeStyles.intervalBoundaries}>
                        <TouchableOpacity onPress={this.onDayoffSelected}>
                            <IntervalBoundary
                                color={CalendarEventsColor.dayoff}
                                boundary={'full'}
                                size={this.state.containerHeight}
                                style={switchDayoffTypeStyles.intervalBoundary as ViewStyle}
                                circleStyle={switchDayoffTypeStyles.intervalBoundary as ViewStyle} />
                        </TouchableOpacity>

                        <TouchableOpacity onPress={this.onWorkoutSelected}>
                            <IntervalBoundary
                                color={CalendarEventsColor.workout}
                                boundary={'full'}
                                size={this.state.containerHeight}
                                style={switchDayoffTypeStyles.intervalBoundary as ViewStyle}
                                circleStyle={switchDayoffTypeStyles.intervalBoundary as ViewStyle} />
                        </TouchableOpacity>
                    </View>
                : null
            }
            </View>
        );
    }

    private onContainerLayout = (e: LayoutChangeEvent) => {
        this.setState({ containerHeight: e.nativeEvent.layout.height });
    };

    private onDayoffSelected = () => {
        this.props.onTypeSelected(HoursCreditType.DaysOff);
    };

    private onWorkoutSelected = () => {
        this.props.onTypeSelected(HoursCreditType.Workout);
    };
}
