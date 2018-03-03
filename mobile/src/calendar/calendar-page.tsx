import React, { Component, Fragment } from 'react';
import moment, { Moment } from 'moment';
import { View, StyleSheet, LayoutChangeEvent, PixelRatio } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarStyles, calendarIntervalStyles, CalendarEventsColor } from './styles';
import { DayModel, WeekModel, IntervalsModel, IntervalModel } from '../reducers/calendar/calendar.model';
import { StartInterval, EndInterval, Interval } from './calendar-page-interval';
import { CalendarEventsType } from '../reducers/calendar/calendar-events.model';
import { WeekDay, WeekDayCircle, WeekDayTouchable } from './calendar-page-weekday';
import { IntervalBoundary } from './calendar-page-interval-boundary';

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
        hidePrevNextMonthDays: true
    };

    private readonly weekdaysNames = moment()
        .locale('en')
        .localeData()
        .weekdaysShort()
        .map(x => x.substring(0, 2).toUpperCase());

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
                                <StyledText style={calendarStyles.weekDayName}>{weekdayName}</StyledText>
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
        const intervalModels = this.getIntervalsByDate(day.date);
        const dayTextColor = this.getDayTextColor(intervalModels);

        return (
            <View style={calendarStyles.weekDayContainer} key={`${day.date.week()}-${day.date.date()}`}>
                <WeekDay hide={this.props.hidePrevNextMonthDays && !day.belongsToCurrentMonth}>
                    <WeekDayTouchable onSelectedDay={this.props.onSelectedDay} day={day} />
                    <WeekDayCircle day={day} selectedDay={this.props.selectedDay} weekHeight={this.state.weekHeight} customTextColor={dayTextColor} />
                    {
                        this.renderIntervals(intervalModels)
                    }
                </WeekDay>
            </View>
        );
    }

    private getDayTextColor(intervals: IntervalModel[]): string | null {
        if (!intervals || !intervals.length) {
            return null;
        }

        return intervals.some(x => !x.boundary)
            ? '#fff'
            : null;
    }

    private getIntervalsByDate(date: moment.Moment): IntervalModel[] | null {
        if (!this.props.intervals) {
            return null;
        }

        const intervals = this.props.intervals.get(date);

        return intervals;
    }

    private renderIntervals(intervals: IntervalModel[]) {
        if (!intervals) {
            return null;
        }

        return intervals.map((interval, index) => this.renderInterval(interval, index));
    }

    private renderInterval(interval: IntervalModel, elementKey: number): JSX.Element | null {
        const color = CalendarEventsColor.getColor(interval.eventType) || '#fff';

        switch (interval.intervalType) {
            case 'startInterval':
                return <StartInterval key={elementKey} size={this.state.weekHeight} color={color} draft={interval.draft} />;
            case 'endInterval':
                return <EndInterval key={elementKey} size={this.state.weekHeight} color={color} draft={interval.draft} />;
            case 'interval':
                return <Interval key={elementKey} size={this.state.weekHeight} color={color} draft={interval.draft} />;
            case 'intervalFullBoundary':
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color} boundary={'full'} draft={interval.draft} />;
            case 'intervalLeftBoundary':
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color} boundary={'left'} draft={interval.draft} />;
            case 'intervalRightBoundary':
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color} boundary={'right'} draft={interval.draft} />;
            default:
                return null;
        }
    }
}