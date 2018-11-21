import React, { Component, Fragment, PureComponent } from 'react';
import moment, { Moment } from 'moment';
import { View, StyleSheet, LayoutChangeEvent, PixelRatio, Dimensions } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarStyles, calendarIntervalStyles, CalendarEventsColor, weekCalendarStyles } from './styles';
import { DayModel, WeekModel, IntervalModel, IntervalType, CalendarSelection, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { StartInterval, EndInterval, Interval } from './calendar-page-interval';
import { WeekDay, WeekDayCircle, WeekDayTouchable } from './calendar-page-weekday';
import { IntervalBoundary } from './calendar-page-interval-boundary';

export type OnSelectedDayCallback = (day: DayModel) => void;

interface CalendarPageDefaultProps {
    hidePrevNextMonthDays?: boolean;
}

interface CalendarPageProps {
    pageDate: Moment;
    weeks: WeekModel[];
    onSelectedDay: OnSelectedDayCallback;
    intervals?: ReadOnlyIntervalsModel;
    selection?: CalendarSelection;
    disableBefore?: DayModel;
    width: number;
    height: number;
}

interface CalendarPageState {
    weekHeight: number;
    weekDayContainerWidth: number;
}

export class CalendarPage extends PureComponent<CalendarPageDefaultProps & CalendarPageProps, CalendarPageState> {
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
            weekHeight: 0,
            weekDayContainerWidth: 0
        };
    }

    public render() {

        const weeksContainerStyles = StyleSheet.flatten([
            calendarStyles.weekContainer,
            {
                width: this.props.width,
                height: this.props.height
            }
        ]);

        const weeks = this.props.weeks.map(week => this.renderWeek(week));

        return (
            this.props.width && this.props.height ?
                <View style={weeksContainerStyles}>
                    <View style={calendarStyles.today}>
                        <StyledText style={calendarStyles.todayTitle}>
                            {this.props.pageDate.format('MMMM YYYY')}
                        </StyledText>
                    </View>
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
            : null
        );
    }

    private onWeeksLayout = (e: LayoutChangeEvent) => {
        // invoke once to reduce perfomance load
        if (!this.state.weekHeight) {
            this.setState({
                weekHeight: PixelRatio.roundToNearestPixel(e.nativeEvent.layout.height)
            });
        }
    };

    private renderWeek(week: WeekModel) {
        return (
            <View style={calendarStyles.week} key={week.weekIndex} onLayout={this.onWeeksLayout}>
                {
                    week.days.map(day => this.renderDay(day))
                }
            </View>
        );
    }

    private onLayoutWeekDayContainer = (e: LayoutChangeEvent) => {
        // TODO: Workaround for https://github.com/facebook/react-native/issues/18137
        if (!this.state.weekDayContainerWidth) {
            const paddings = weekCalendarStyles.paddingLeft + weekCalendarStyles.paddingRight;
            const daysPerWeek = 7;
            const calendarWidth = Dimensions.get('window').width - paddings;
            const width = PixelRatio.roundToNearestPixel(calendarWidth / daysPerWeek);
            this.setState({ weekDayContainerWidth: width });
        }
    };

    private renderDay(day: DayModel) {
        const intervalModels = this.getIntervalsByDate(day.date);
        const disableDay = this.props.disableBefore
            ? day.date.isBefore(this.props.disableBefore.date, 'day')
            : false;

        const weekDayContainerStyles = StyleSheet.flatten([
            calendarStyles.weekDayContainer,
            {
                width: this.state.weekDayContainerWidth
            }
        ]);

        return (
            <View style={weekDayContainerStyles} key={`${day.date.week()}-${day.date.date()}`} onLayout={this.onLayoutWeekDayContainer}>
                <WeekDay hide={this.state.weekDayContainerWidth === 0 || (this.props.hidePrevNextMonthDays && !day.belongsToCurrentMonth)}>
                    <WeekDayTouchable onSelectedDay={this.props.onSelectedDay} day={day} disabled={disableDay} />
                    {
                        this.renderSingleSelection(day, intervalModels)
                    }
                    {
                        this.renderIntervalSelection(day)
                    }
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

    private renderSingleSelection(day: DayModel, intervalModels: IntervalModel[]) {
        if (!this.props.selection
            || !this.props.selection.single
            || !this.props.selection.single.day) {
                return null;
        }

        const dayTextColor = this.getDayTextColor(intervalModels);

        return <WeekDayCircle day={day} selectedDay={this.props.selection.single.day} weekHeight={this.state.weekHeight} customTextColor={dayTextColor} />;
    }

    private renderIntervalSelection(day: DayModel) {
        if (!this.props.selection
            || !this.props.selection.interval
            || !this.props.selection.interval.startDay
            || !this.props.selection.interval.endDay
            || !this.props.selection.interval.color) {
            return null;
        }

        const { startDay, endDay, color } = this.props.selection.interval;

        if (!day.date.isBetween(startDay.date, endDay.date, 'day', '[]')) {
            return null;
        }

        if (startDay.date.isSame(endDay.date, 'day')) {
            return <IntervalBoundary size={this.state.weekHeight} color={color} boundary={'full'} style={calendarIntervalStyles.selection} />;
        }

        if (day.date.isSame(startDay.date, 'day')) {
            return <StartInterval size={this.state.weekHeight} color={color} style={calendarIntervalStyles.selection} />;
        }

        if (day.date.isSame(endDay.date, 'day')) {
            return <EndInterval size={this.state.weekHeight} color={color} style={calendarIntervalStyles.selection} />;
        }

        return <Interval size={this.state.weekHeight} color={color} style={calendarIntervalStyles.selection} />;
    }

    private renderInterval(interval: IntervalModel, elementKey: number): JSX.Element | null {
        const color = CalendarEventsColor.getColor(interval.calendarEvent.type) || '#fff';

        switch (interval.intervalType) {
            case IntervalType.StartInterval:
                return <StartInterval key={elementKey} size={this.state.weekHeight} color={color} />;
            case IntervalType.EndInterval:
                return <EndInterval key={elementKey} size={this.state.weekHeight} color={color} />;
            case IntervalType.Interval:
                return <Interval key={elementKey} size={this.state.weekHeight} color={color} />;
            case IntervalType.IntervalFullBoundary:
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color} boundary={'full'} />;
            case IntervalType.IntervalLeftBoundary:
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color} boundary={'left'} />;
            case IntervalType.IntervalRightBoundary:
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color} boundary={'right'} />;
            default:
                return null;
        }
    }
}
