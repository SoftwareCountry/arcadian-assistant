import React, { Component, Fragment } from 'react';
import moment, { Moment } from 'moment';
import { View, StyleSheet, TouchableHighlight, LayoutChangeEvent } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarStyles, calendarPeriodStyles, calendarPeriodColors } from './styles';
import { DayModel, WeekModel, PeriodsModel, PeriodModel } from '../reducers/calendar/calendar.model';
import { StartPeriod, EndPeriod, Period, DotPeriod } from './calendar-page-period';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';

export type OnSelectedDayCallback = (day: DayModel) => void;

interface CalendarPageDefaultProps {
    hidePrevNextMonthDays?: boolean;
    periods?: PeriodsModel;
}

interface CalendarPageProps {
    weeks: WeekModel[];
    onSelectedDay: OnSelectedDayCallback;
}

interface CalendarPageState {
    weekHeight: number;
}

export class CalendarPage extends Component<CalendarPageDefaultProps & CalendarPageProps, CalendarPageState> {
    public static defaultProps: CalendarPageDefaultProps = {
        hidePrevNextMonthDays: false
    };

    private readonly weekdaysNames = moment()
        .locale('en')
        .localeData()
        .weekdaysShort()
        .map(x => x.substring(0, 2));

    private readonly periodColors = {
        [CalendarEventsType.Vacation]: calendarPeriodColors.vacation,
        [CalendarEventsType.SickLeave]: calendarPeriodColors.sickLeave,
        [CalendarEventsType.Dayoff]: calendarPeriodColors.dayoff
    };

    constructor(props: CalendarPageDefaultProps & CalendarPageProps) {
        super(props);
        this.state = {
            weekHeight: 0
        };
    }

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

    private onWeeksLayout = (e: LayoutChangeEvent) => {
        // invoke once to reduce perfomance load
        if (!this.state.weekHeight) {
            this.setState({
                weekHeight: e.nativeEvent.layout.height
            });
        }
    }

    private renderWeek(week: WeekModel) {
        return (
            <View style={calendarStyles.week} key={week.weekIndex} onLayout={this.onWeeksLayout}>
                {
                    week.days.map(day => this.renderDay(day))
                }
            </View>
        );
    }

    private renderDay(day: DayModel) {
        return (
            <View style={calendarStyles.weekDayContainer} key={`${day.date.week()}-${day.date.date()}`}>
                <WeekDay hide={this.props.hidePrevNextMonthDays && !day.belongsToCurrentMonth}>
                    <View style={calendarStyles.weekDayCircleContainer}>
                        <WeekDayCircle day={day} onSelectedDay={this.props.onSelectedDay} weekHeight={this.state.weekHeight} />
                    </View>
                    {
                        this.renderPeriods(day.date)
                    }
                </WeekDay>
            </View>
        );
    }

    private renderPeriods(date: Moment) {
        if (!this.props.periods) {
            return null;
        }

        const key = PeriodsModel.generateKey(date);

        const periods = this.props.periods.get(key);

        if (!periods) {
            return null;
        }

        return periods.map((period, index) => this.renderPeriod(period, index));
    }

    private renderPeriod(period: PeriodModel, elementKey: number): JSX.Element | null {
        const color = this.periodColors[period.eventType] || '#fff';

        switch (period.periodType) {
            case 'startPeriod':
                return <StartPeriod key={elementKey} size={this.state.weekHeight} color={color} />;
            case 'endPeriod':
                return <EndPeriod key={elementKey} size={this.state.weekHeight} color={color} />;
            case 'period':
                return <Period key={elementKey} size={this.state.weekHeight} color={color} />;
            case 'dotPeriod':
                return <DotPeriod key={elementKey} size={this.state.weekHeight} color={color} />;
            default:
                return null;
        }
    }
}

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

        const innerCircleSize = (circleStyles.width as number) - (circleStyles.width as number * 0.2);

        const innerCircleStyles = StyleSheet.flatten([
            calendarStyles.weekDayCircle,
            {
                width: innerCircleSize,
                height: innerCircleSize,
                borderRadius: circleStyles.borderRadius - (circleStyles.borderRadius * 0.2),
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