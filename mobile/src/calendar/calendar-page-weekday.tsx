import React, { Component } from 'react';
import { PixelRatio, StyleSheet, View } from 'react-native';
import { calendarStyles, intervalMargin } from './styles';
import { DayModel } from '../reducers/calendar/calendar.model';
import { OnSelectedDayCallback } from './calendar-page';
import { StyledText } from '../override/styled-text';
import Style from '../layout/style';
import { State, TapGestureHandler, TapGestureHandlerStateChangeEvent } from 'react-native-gesture-handler';
import business from 'moment-business';

//============================================================================
export const WeekDay = (props: { hide: boolean, children: any[] }) =>
    props.hide
        ? null
        : <View style={calendarStyles.weekDay}>{props.children}</View>;

//============================================================================
interface WeekDayCircleProps {
    weekHeight: number;
    day: DayModel;
    selectedDay: DayModel;
}

//============================================================================
export class WeekDayCircle extends Component<WeekDayCircleProps> {

    //----------------------------------------------------------------------------
    public render() {
        const { day, weekHeight } = this.props;

        const height = PixelRatio.roundToNearestPixel(weekHeight);
        const borderRadius = PixelRatio.roundToNearestPixel(height / 2);

        // noinspection JSSuspiciousNameCombination
        const circleStyle = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: height,
                height: height,
                borderRadius: borderRadius,
                borderWidth: 2,
                borderColor: this.isSelectedDay(day) ? Style.color.base : Style.color.transparent,
                backgroundColor: this.isSelectedDay(day) ? Style.color.white : Style.color.transparent,
            }
        ]);

        const innerCircleSize = height * (1 - intervalMargin);

        const innerCircleStyle = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: innerCircleSize,
                height: innerCircleSize,
                borderRadius: borderRadius * (1 - intervalMargin),
                backgroundColor: this.isSelectedDay(day) ? Style.color.base : Style.color.transparent,
            }
        ]);

        const circleTextStyle = StyleSheet.flatten([
            calendarStyles.weekDayNumber,
            {
                color: this.getColorFor(day),
                fontWeight: day.today ? 'bold' as const : 'normal' as const,
                lineHeight: day.today ? 14 : 12,
                fontSize: day.today ? 14 : 12,
            }
        ]);

        return (
            <View style={calendarStyles.weekDayCircleContainer}>
                <View style={circleStyle}>
                    <View style={innerCircleStyle}>
                        <StyledText style={circleTextStyle}>{day.date.date()}</StyledText>
                    </View>
                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private isSelectedDay(day: DayModel) {
        const { selectedDay } = this.props;

        return selectedDay && selectedDay.date && selectedDay.date.isSame(day.date, 'day');
    }

    //----------------------------------------------------------------------------
    private getColorFor(day: DayModel) {

        if (day.belongsToCurrentMonth) {

            if (this.isSelectedDay(day)) {
                return Style.color.white;
            }

            if (business.isWeekendDay(day.date)) {
                return Style.color.gray;
            }

            return Style.color.black;
        }

        return '#dadada';
    }
}

//============================================================================
interface WeekDayTouchableProps {
    day: DayModel;
    onSelectedDay: OnSelectedDayCallback;
    disabled: boolean;
}

//============================================================================
export class WeekDayTouchable extends Component<WeekDayTouchableProps> {

    //----------------------------------------------------------------------------
    public render() {
        return (
            <TapGestureHandler maxDist={50} onHandlerStateChange={this.onTapHandlerStateChange}>
                <View style={calendarStyles.weekDayTouchable}/>
            </TapGestureHandler>
        );
    }

    //----------------------------------------------------------------------------
    private onTapHandlerStateChange = (event: TapGestureHandlerStateChangeEvent) => {
        const { disabled, day, onSelectedDay } = this.props;
        if (disabled || event.nativeEvent.state !== State.ACTIVE) {
            return;
        }

        onSelectedDay(day);
    };
}
