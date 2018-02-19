import React, { Component, Fragment } from 'react';
import moment, { Moment } from 'moment';
import { View, StyleSheet, TouchableHighlight, LayoutChangeEvent, PixelRatio } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarStyles, calendarIntervalStyles, calendarIntervalColors } from './styles';
import { DayModel, WeekModel, IntervalsModel, IntervalModel } from '../reducers/calendar/calendar.model';
import { StartInterval, EndInterval, Interval, IntervalBoundary } from './calendar-page-interval';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { WeekDay, WeekDayCircle } from './calendar-page-weekday';

export type OnSelectedDayCallback = (day: DayModel) => void;

interface CalendarPageDefaultProps {
    hidePrevNextMonthDays?: boolean;
    intervals?: IntervalsModel;
}

interface CalendarPageProps {
    weeks: WeekModel[];
    onSelectedDay: OnSelectedDayCallback;
    selectedDay: DayModel;
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
        .map(x => x.substring(0, 2).toUpperCase());

    private readonly intervalColors = {
        [CalendarEventsType.Vacation]: calendarIntervalColors.vacation,
        [CalendarEventsType.SickLeave]: calendarIntervalColors.sickLeave,
        [CalendarEventsType.Dayoff]: calendarIntervalColors.dayoff
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
                weekHeight: PixelRatio.roundToNearestPixel(e.nativeEvent.layout.height)
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
                        <WeekDayCircle day={day} selectedDay={this.props.selectedDay} onSelectedDay={this.props.onSelectedDay} weekHeight={this.state.weekHeight} />
                    </View>
                    {
                        this.renderIntervals(day.date)
                    }
                </WeekDay>
            </View>
        );
    }

    private renderIntervals(date: Moment) {
        if (!this.props.intervals) {
            return null;
        }

        const intervals = this.props.intervals.get(date);

        if (!intervals) {
            return null;
        }

        return intervals.map((interval, index) => this.renderInterval(interval, index));
    }

    private renderInterval(interval: IntervalModel, elementKey: number): JSX.Element | null {
        const color = this.intervalColors[interval.eventType] || '#fff';

        switch (interval.intervalType) {
            case 'startInterval':
                return <StartInterval key={elementKey} size={this.state.weekHeight} color={color} />;
            case 'endInterval':
                return <EndInterval key={elementKey} size={this.state.weekHeight} color={color} />;
            case 'interval':
                return <Interval key={elementKey} size={this.state.weekHeight} color={color} />;
            case 'intervalBoundary':
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color} />;
            default:
                return null;
        }
    }
}