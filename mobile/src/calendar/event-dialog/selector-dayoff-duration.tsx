import React, { Component } from 'react';
import { LayoutChangeEvent, TouchableOpacity, View, ViewStyle } from 'react-native';
import { IntervalBoundary } from '../calendar-page-interval-boundary';
import { switchDayOffTypeStyles } from './styles';
import { CalendarEventsColor } from '../styles';
import { IntervalType } from '../../reducers/calendar/calendar.model';

//============================================================================
interface SelectorDayOffDurationProps {
    onIntervalTypeSelected: (selectedType: IntervalType) => void;
    isWorkout: boolean;
}

//============================================================================
interface SelectorDayOffDurationState {
    containerHeight: number;
}

//============================================================================
export class SelectorDayOffDuration extends Component<SelectorDayOffDurationProps, SelectorDayOffDurationState> {

    //----------------------------------------------------------------------------
    constructor(props: SelectorDayOffDurationProps) {
        super(props);
        this.state = {
            containerHeight: 0
        };
    }

    //----------------------------------------------------------------------------
    public render() {
        const boundaryColor = this.props.isWorkout
            ? CalendarEventsColor.workout
            : CalendarEventsColor.dayOff;

        return (
            <View style={switchDayOffTypeStyles.container} onLayout={this.onContainerLayout}>
                {
                    this.state.containerHeight !== 0 ?
                        <View style={switchDayOffTypeStyles.intervalBoundaries}>
                            <TouchableOpacity onPress={this.onFullDaySelected}>
                                <IntervalBoundary
                                    color={boundaryColor}
                                    boundary={'full'}
                                    size={this.state.containerHeight}
                                    style={switchDayOffTypeStyles.intervalBoundary as ViewStyle}
                                    circleStyle={switchDayOffTypeStyles.intervalBoundary as ViewStyle}/>
                            </TouchableOpacity>

                            <TouchableOpacity onPress={this.onFirstHalfSelected}>
                                <IntervalBoundary
                                    color={boundaryColor}
                                    boundary={'left'}
                                    size={this.state.containerHeight}
                                    style={switchDayOffTypeStyles.intervalBoundary as ViewStyle}
                                    circleStyle={switchDayOffTypeStyles.intervalBoundary as ViewStyle}/>
                            </TouchableOpacity>

                            <TouchableOpacity onPress={this.onSecondHalfSelected}>
                                <IntervalBoundary
                                    color={boundaryColor}
                                    boundary={'right'}
                                    size={this.state.containerHeight}
                                    style={switchDayOffTypeStyles.intervalBoundary as ViewStyle}
                                    circleStyle={switchDayOffTypeStyles.intervalBoundary as ViewStyle}/>
                            </TouchableOpacity>
                        </View>
                        : null
                }
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private onContainerLayout = (e: LayoutChangeEvent) => {
        this.setState({ containerHeight: e.nativeEvent.layout.height });
    };

    //----------------------------------------------------------------------------
    private onFirstHalfSelected = () => {
        this.props.onIntervalTypeSelected(IntervalType.IntervalLeftBoundary);
    };

    //----------------------------------------------------------------------------
    private onSecondHalfSelected = () => {
        this.props.onIntervalTypeSelected(IntervalType.IntervalRightBoundary);
    };

    //----------------------------------------------------------------------------
    private onFullDaySelected = () => {
        this.props.onIntervalTypeSelected(IntervalType.IntervalFullBoundary);
    };
}
