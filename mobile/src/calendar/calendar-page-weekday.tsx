import React, { Component } from 'react';
import { View, StyleSheet, PixelRatio, TouchableOpacity, GestureResponderEvent } from 'react-native';
import { calendarStyles, intervalMargin } from './styles';
import { DayModel } from '../reducers/calendar/calendar.model';
import { OnSelectedDayCallback } from './calendar-page';
import { StyledText } from '../override/styled-text';
import { Moment } from 'moment';

export const WeekDay = (props: { hide: boolean, children: any[] }) =>
    props.hide
        ? null
        : <View style={calendarStyles.weekDay}>{props.children}</View>;

interface WeekDayCircleDefaultProps {
    customTextColor?: string;
}

interface WeekDayCircleProps extends WeekDayCircleDefaultProps {
    weekHeight: number;
    day: DayModel;
    selectedDay: DayModel;
}

export class WeekDayCircle extends Component<WeekDayCircleProps> {
    public static defaultProps: WeekDayCircleDefaultProps = {
        customTextColor: '#000'
    };

    public render() {
        const { day, weekHeight } = this.props;

        const height = PixelRatio.roundToNearestPixel(weekHeight - (weekHeight * 0));

        const circleStyles = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: height,
                height: height,
                borderRadius: PixelRatio.roundToNearestPixel(height / 2),
                borderWidth: 2,
                borderColor: this.isSelectedDay(day) ? '#2FAFCC' : 'transparent',
                backgroundColor: this.isSelectedDay(day)
                    ? '#fff'
                    : 'transparent'
            }
        ]);

        const innerCircleSize = (circleStyles.width as number) - (circleStyles.width as number * intervalMargin);

        const innerCircleStyles = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: innerCircleSize,
                height: innerCircleSize,
                borderRadius: circleStyles.borderRadius - (circleStyles.borderRadius * intervalMargin),
                backgroundColor: this.isSelectedDay(day)
                    ? '#2FAFCC'
                    : 'transparent'
            }
        ]);

        const circleTextStyles = StyleSheet.flatten([
            calendarStyles.weekDayNumber,
            {
                color: day.belongsToCurrentMonth
                    ? this.isSelectedDay(day)
                        ? '#fff'
                        : this.props.customTextColor
                    : '#dadada'
            }
        ]);

        return (
            <View style={calendarStyles.weekDayCircleContainer}>
                <View style={circleStyles}>
                    <View style={innerCircleStyles}>
                        <StyledText style={circleTextStyles}>{day.date.date()}</StyledText>
                    </View>
                </View>
            </View>
        );
    }

    private isSelectedDay(day: DayModel) {
        return this.props.selectedDay
            && this.props.selectedDay.date
            && this.props.selectedDay.date.isSame(day.date, 'day');
    }
}

interface WeekDayTouchableProps {
    day: DayModel;
    onSelectedDay: OnSelectedDayCallback;
    disabled: boolean;
}

// Use native events instead of TouchableOpacity, which uses PanResponder under the hood.
class WeekDayTouchableHandler {
    private pageX: number;

    constructor(
        private readonly onPressUp: () => void
    ) { }

    public onTouchStart = (event: GestureResponderEvent) => {
        this.pageX = event.nativeEvent.pageX;
    }

    public onTouchEnd = (event: GestureResponderEvent) => {
        // if we detect a horizontal swipe (x coordinate has been changed between start and end events),
        // stop pressing to aviod unexpected change selected day
        if (this.pageX === event.nativeEvent.pageX) {
            this.onPressUp();
        }
    }
}

export class WeekDayTouchable extends Component<WeekDayTouchableProps> {
    private readonly touchableHandler = new WeekDayTouchableHandler(() => {
        if (!this.props.disabled) {
            this.props.onSelectedDay(this.props.day);
        }
    });

    public render() {
        return <View {...this.touchableHandler} style={calendarStyles.weekDayTouchable}></View>;
    }
}