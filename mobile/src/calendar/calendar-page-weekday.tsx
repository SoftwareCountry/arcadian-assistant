import React, { Component } from 'react';
import { View, StyleSheet, TouchableHighlight } from 'react-native';
import { calendarStyles, intervalMargin } from './styles';
import { DayModel } from '../reducers/calendar/calendar.model';
import { OnSelectedDayCallback } from './calendar-page';
import { StyledText } from '../override/styled-text';
import { Moment } from 'moment';

export const WeekDay = (props: { hide: boolean, children: any[] }) =>
    props.hide
        ? null
        : <View style={calendarStyles.weekDay}>{props.children}</View>;

interface WeekDayCircleProps {
    weekHeight: number;
    day: DayModel;
    onSelectedDay: OnSelectedDayCallback;
    selectedDay: DayModel;
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

        const circleTextStyles = StyleSheet.flatten({
            color: day.belongsToCurrentMonth
                ? this.isSelectedDay(day)
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

    private isSelectedDay(day: DayModel) {
        return this.props.selectedDay 
            && this.props.selectedDay.date
            && this.props.selectedDay.date.isSame(day.date, 'day');
    }
}