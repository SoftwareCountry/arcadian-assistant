import React, { Component } from 'react';
import { PixelRatio, StyleSheet, View } from 'react-native';
import { calendarStyles, intervalMargin } from './styles';
import { DayModel } from '../reducers/calendar/calendar.model';
import { OnSelectedDayCallback } from './calendar-page';
import { StyledText } from '../override/styled-text';
import Style from '../layout/style';
import { State, TapGestureHandler, TapGestureHandlerStateChangeEvent } from 'react-native-gesture-handler';

//============================================================================
export const WeekDay = (props: { hide: boolean, children: any[] }) =>
    props.hide
        ? null
        : <View style={calendarStyles.weekDay}>{props.children}</View>;

//============================================================================
interface WeekDayCircleDefaultProps {
    customTextColor?: string;
}

//============================================================================
interface WeekDayCircleProps extends WeekDayCircleDefaultProps {
    weekHeight: number;
    day: DayModel;
    selectedDay: DayModel;
}

//============================================================================
export class WeekDayCircle extends Component<WeekDayCircleProps> {
    //----------------------------------------------------------------------------
    public static defaultProps: WeekDayCircleDefaultProps = {
        customTextColor: Style.color.black
    };

    //----------------------------------------------------------------------------
    public render() {
        const { day, weekHeight } = this.props;

        const height = PixelRatio.roundToNearestPixel(weekHeight);

        // noinspection JSSuspiciousNameCombination
        const circleStyle = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: height,
                height: height,
                borderRadius: PixelRatio.roundToNearestPixel(height / 2),
                borderWidth: 2,
                borderColor: this.isSelectedDay(day) ? Style.color.base : Style.color.transparent,
                backgroundColor: this.isSelectedDay(day) ? Style.color.white : Style.color.transparent,
            }
        ]);

        const innerCircleSize = (circleStyle.width as number) - (circleStyle.width as number * intervalMargin);

        const innerCircleStyle = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: innerCircleSize,
                height: innerCircleSize,
                borderRadius: circleStyle.borderRadius! - (circleStyle.borderRadius! * intervalMargin),
                backgroundColor: this.isSelectedDay(day) ? Style.color.base : Style.color.transparent,
            }
        ]);

        const circleTextStyle = StyleSheet.flatten([
            calendarStyles.weekDayNumber,
            {
                color: day.belongsToCurrentMonth ?
                    this.isSelectedDay(day) ? Style.color.white : this.props.customTextColor :
                    '#dadada',
                fontWeight: day.today ? 'bold' : 'normal',
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
        return (
            this.props.selectedDay &&
            this.props.selectedDay.date &&
            this.props.selectedDay.date.isSame(day.date, 'day')
        );
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
        if (this.props.disabled || event.nativeEvent.state !== State.ACTIVE) {
            return;
        }

        this.props.onSelectedDay(this.props.day);
    };
}
