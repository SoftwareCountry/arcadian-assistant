import React, { Component } from 'react';
import moment, { Moment } from 'moment';
import { View, StyleSheet, TouchableHighlight } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarStyles } from './styles';
import { DayModel, WeekModel } from '../reducers/calendar/calendar.model';

export type OnSelectedDayCallback = (day: DayModel) => void;

interface CalendarPageDefaultProps {
    weeksPerPage?: number;
    daysPerWeek?: number;
    hidePrevNextMonthDays?: boolean;
}

interface CalendarPageProps {
    weeks: WeekModel[];
    onSelectedDay: OnSelectedDayCallback;
}

export class CalendarPage extends Component<CalendarPageDefaultProps & CalendarPageProps> {
    public static defaultProps: CalendarPageDefaultProps = {
        hidePrevNextMonthDays: false
    };

    private readonly weekdaysNames = moment()
        .locale('en')
        .localeData()
        .weekdaysShort()
        .map(x => x.substring(0, 2));

    public render() {
        const weeks = this.props.weeks.map(week => this.renderWeek(week));

        return (
            <View style={calendarStyles.weeksContainer}>
                <View style={calendarStyles.weeksNames}>
                    {
                        this.weekdaysNames.map((weekdayName, index) =>
                            <View key={`${index}-${weekdayName}`} style={calendarStyles.weekName}>
                                <StyledText style={calendarStyles.weekDayText}>{weekdayName}</StyledText>
                            </View>
                        )
                    }
                </View>
                <View style={calendarStyles.weeks}>
                    {
                        weeks
                    }
                </View>
            </View>
        );
    }

    private renderWeek(week: WeekModel) {
        return (
            <View style={calendarStyles.week} key={week.weekIndex}>
                {
                    week.days.map(day => this.renderDay(day))
                }
            </View>
        );
    }

    private renderDay(day: DayModel) {
        const circleStyles = StyleSheet.create({
            circle: {
                backgroundColor: day.today ? '#2FAFCC' : '#fff',
                borderRadius: 28 / 2,
                height: 28,
                width: 28,
                justifyContent: 'center',
                alignItems: 'center'
            },
            text: {
                color: day.belongsToCurrentMonth
                    ? day.today
                        ? '#fff'
                        : '#000'
                    : '#eee',
            }
        });

        const onSelectedDay = () => {
            this.props.onSelectedDay(day);
        };

        return <View style={calendarStyles.weekDay} key={`${day.date.week()}-${day.date.date()}`}>
                    <TouchableHighlight style={circleStyles.circle} onPress={onSelectedDay}>
                        {
                            this.props.hidePrevNextMonthDays && !day.belongsToCurrentMonth
                                ? <StyledText style={circleStyles.text}></StyledText>
                                : <StyledText style={circleStyles.text}>{day.date.date()}</StyledText>
                        }
                    </TouchableHighlight>
                </View>;
    }
}