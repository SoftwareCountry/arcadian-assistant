import React, { Component } from 'react';
import { Moment } from 'moment';
import moment from 'moment';
import { View, StyleSheet } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarStyles } from './styles';

export interface DayModel {
    day: number;
    week: number;
    hide: boolean;
    today: boolean;
    belongsToCurrentMonth: boolean;
}

export interface WeekModel<TDay> {
    days: TDay[];
    weekIndex: number;
}

type DaySelector<TDay> = (day: DayModel) => TDay;
type WeekSelector<TDay, TWeek> = (days: WeekModel<TDay>) => TWeek;

interface CalendarPageDefaultProps {
    weeksPerPage?: number;
    daysPerWeek?: number;
    hidePrevNextMonthDays?: boolean;
}

interface CalendarPageProps {
    month: number;
    year: number;
}

export class CalendarPage extends Component<CalendarPageDefaultProps & CalendarPageProps> {
    public static defaultProps: CalendarPageDefaultProps = {
        weeksPerPage: 6,
        daysPerWeek: 7,
        hidePrevNextMonthDays: false
    };

    private readonly weekdaysNames = moment()
        .locale('en')
        .localeData()
        .weekdaysShort();

    public fillWeekWithPrevMonthDays<TDay>(
        weekModel: WeekModel<TDay>,
        currentWeek: number,
        date: Moment,
        today: Moment,
        daySelector: DaySelector<TDay>
    ) {
        const before = moment(date);
        before.add(-1, 'days');

        while (before.week() === currentWeek) {

            const day = daySelector({
                day: before.date(),
                week: before.week(),
                hide: this.props.hidePrevNextMonthDays,
                today: before.date() === today.date()
                    && before.month() === today.month()
                    && before.year() === today.year(),
                belongsToCurrentMonth: false
            });

            weekModel.days.unshift(day);

            before.add(-1, 'days');
        }
    }

    public createWeeks<TDay, TWeek>(
        currentMonth: number,
        currentYear: number,
        daySelector: DaySelector<TDay>,
        weekSelector: WeekSelector<TDay, TWeek>
    ): TWeek[] {
        const date = moment({
            date: 1, // start filling from the first day of the month
            month: currentMonth,
            year: currentYear });

        const currentWeek = date.week();

        const weeksResult: TWeek[] = [];

        let weekIndex = 1;
        let weekModel: WeekModel<TDay> = { days: [], weekIndex };

        const today = moment();

        this.fillWeekWithPrevMonthDays(
            weekModel,
            currentWeek,
            date,
            today,
            daySelector);

        while (weekIndex <= this.props.weeksPerPage) {
            if (weekModel.days.length === this.props.daysPerWeek) {
                const week = weekSelector(weekModel);

                weeksResult.push(week);
                ++weekIndex;
                weekModel = { days: [], weekIndex };
            }

            const day = daySelector({
                day: date.date(),
                week: date.week(),
                hide: this.props.hidePrevNextMonthDays && date.month() !== currentMonth,
                today: date.date() === today.date()
                    && date.month() === today.month()
                    && date.year() === today.year(),
                belongsToCurrentMonth: date.month() === currentMonth
            });

            weekModel.days.push(day);

            date.add(1, 'days');
        }

        return weeksResult;
    }

    public render() {
        const weeks = this.createWeeks(
            this.props.month,
            this.props.year,
            this.daySelector,
            this.weekSelector);

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

    private daySelector = (day: DayModel): JSX.Element => {
        const circleStyles = StyleSheet.create({
            circle: {
                backgroundColor: day.today ? '#2FAFCC' : '#fff',
                borderRadius: 26 / 2,
                height: 26,
                width: 26,
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

        return <View style={calendarStyles.weekDay} key={`${day.week}-${day.day}`}>
                    <View style={circleStyles.circle}>
                        {
                            !day.hide
                                ? <StyledText style={circleStyles.text}>{day.day}</StyledText>
                                : null
                        }
                    </View>
                </View>;
    }

    private weekSelector = (week: WeekModel<JSX.Element>): JSX.Element => {
        return <View style={calendarStyles.week} key={week.weekIndex}>
        {
            week.days
        }
        </View>;
    }
}