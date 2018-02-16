import React, { Component } from 'react';
import { View, StyleSheet, TouchableHighlight } from 'react-native';
import { calendarStyles, intervalMargin } from './styles';
import { DayModel } from '../reducers/calendar/calendar.model';
import { OnSelectedDayCallback } from './calendar-page';
import { StyledText } from '../override/styled-text';

export const WeekDay = (props: { hide: boolean, children: any[] }) =>
    props.hide
        ? null
        : <View style={calendarStyles.weekDay}>{props.children}</View>;

interface WeekDayCircleProps {
    weekHeight: number;
    day: DayModel;
    onSelectedDay: OnSelectedDayCallback;
}

export class WeekDayCircle extends Component<WeekDayCircleProps> {
    public render() {
        const { day, weekHeight } = this.props;

        const circleStyles = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: weekHeight,
                height: weekHeight,
                borderRadius: weekHeight / 2,
                borderWidth: 2,
                borderColor: day.today ? '#2FAFCC' : 'transparent',
                backgroundColor: day.today
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
                backgroundColor: day.today
                    ? '#2FAFCC'
                    : 'transparent'
            }
        ]);

        const circleTextStyles = StyleSheet.flatten({
            color: day.belongsToCurrentMonth
                ? day.today
                    ? '#fff'
                    : '#000'
                : '#dadada'
        });

        return (
            <TouchableHighlight style={circleStyles} onPress={this.onSelectedDay}>
                <View style={innerCircleStyles}>
                    <StyledText style={circleTextStyles}>{day.date.date()}</StyledText>
                </View>
            </TouchableHighlight>
        );
    }

    private onSelectedDay = () => {
        this.props.onSelectedDay(this.props.day);
    }
}