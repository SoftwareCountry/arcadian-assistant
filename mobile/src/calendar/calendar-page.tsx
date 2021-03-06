import React, { PureComponent } from 'react';
import moment, { Moment } from 'moment';
import { Dimensions, LayoutChangeEvent, PixelRatio, StyleSheet, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { CalendarEventsColor, calendarIntervalStyles, calendarStyles, weekCalendarStyles } from './styles';
import {
    CalendarSelection,
    DayModel,
    IntervalModel,
    IntervalType,
    ReadOnlyIntervalsModel,
    WeekModel
} from '../reducers/calendar/calendar.model';
import { EndInterval, Interval, StartInterval } from './calendar-page-interval';
import { WeekDay, WeekDayCircle, WeekDayTouchable } from './calendar-page-weekday';
import { IntervalBoundary } from './calendar-page-interval-boundary';
import { Nullable, Optional } from 'types';
import Style from '../layout/style';

export type OnSelectedDayCallback = (day: DayModel) => void;

//============================================================================
interface CalendarPageDefaultProps {
    hidePrevNextMonthDays?: boolean;
}

//============================================================================
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

//============================================================================
interface CalendarPageState {
    weekHeight: number;
    weekDayContainerWidth: number;
}

//============================================================================
export class CalendarPage extends PureComponent<CalendarPageDefaultProps & CalendarPageProps, CalendarPageState> {
    public static defaultProps: CalendarPageDefaultProps = {
        hidePrevNextMonthDays: true
    };

    private readonly weekdaysNames = moment.weekdays(true)
        .map(x => x.substring(0, 2).toUpperCase());

    //----------------------------------------------------------------------------
    constructor(props: CalendarPageDefaultProps & CalendarPageProps) {
        super(props);
        this.state = {
            weekHeight: 0,
            weekDayContainerWidth: 0
        };
    }

    //----------------------------------------------------------------------------
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

    //----------------------------------------------------------------------------
    private onWeeksLayout = (e: LayoutChangeEvent) => {
        // invoke once to reduce performance load
        if (!this.state.weekHeight) {
            this.setState({
                weekHeight: PixelRatio.roundToNearestPixel(e.nativeEvent.layout.height)
            });
        }
    };

    //----------------------------------------------------------------------------
    private renderWeek(week: WeekModel) {
        return (
            <View style={calendarStyles.week} key={week.weekIndex} onLayout={this.onWeeksLayout}>
                {
                    week.days.map(day => this.renderDay(day))
                }
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private onLayoutWeekDayContainer = (e: LayoutChangeEvent) => {
        // TODO: Workaround for https://github.com/facebook/react-native/issues/18137
        if (this.state.weekDayContainerWidth === 0) {
            const paddings = weekCalendarStyles.paddingLeft + weekCalendarStyles.paddingRight;
            const daysPerWeek = 7;
            const calendarWidth = Dimensions.get('window').width - paddings;
            const width = PixelRatio.roundToNearestPixel(calendarWidth / daysPerWeek);
            this.setState({ weekDayContainerWidth: width });
        }
    };

    //----------------------------------------------------------------------------
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
            <View style={weekDayContainerStyles}
                  key={`${day.date.week()}-${day.date.date()}`}
                  onLayout={this.onLayoutWeekDayContainer}>
                <WeekDay
                    hide={this.state.weekDayContainerWidth === 0 || (!!this.props.hidePrevNextMonthDays && !day.belongsToCurrentMonth)}>
                    <WeekDayTouchable onSelectedDay={this.props.onSelectedDay} day={day} disabled={disableDay}/>
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

    //----------------------------------------------------------------------------
    private getIntervalsByDate(date: moment.Moment): Nullable<IntervalModel[]> {
        if (!this.props.intervals || !this.props.intervals.get(date)) {
            return null;
        }

        const intervals = this.props.intervals.get(date);

        return intervals ? intervals : null;
    }

    //----------------------------------------------------------------------------
    private renderIntervals(intervals: IntervalModel[] | null) {
        if (!intervals) {
            return null;
        }

        return intervals.map((interval, index) => this.renderInterval(interval, index));
    }

    //----------------------------------------------------------------------------
    private getDayTextColor(intervals: Nullable<IntervalModel[]>): Optional<string> {
        if (!intervals || !intervals.length) {
            return undefined;
        }

        return intervals.some(x => !x.boundary) ? Style.color.white : undefined;
    }

    //----------------------------------------------------------------------------
    private renderSingleSelection(day: DayModel, intervalModels: IntervalModel[] | null): React.ReactNode {
        if (!this.props.selection ||
            !this.props.selection.single ||
            !this.props.selection.single.day) {
            return null;
        }

        return <WeekDayCircle day={day}
                              selectedDay={this.props.selection.single.day}
                              weekHeight={this.state.weekHeight}/>;
    }

    //----------------------------------------------------------------------------
    private renderIntervalSelection(day: DayModel) {
        if (!this.props.selection ||
            !this.props.selection.interval ||
            !this.props.selection.interval.startDay ||
            !this.props.selection.interval.endDay ||
            !this.props.selection.interval.color) {
            return null;
        }

        const { startDay, endDay, color } = this.props.selection.interval;

        if (!day.date.isBetween(startDay.date, endDay.date, 'day', '[]')) {
            return null;
        }

        if (startDay.date.isSame(endDay.date, 'day')) {
            return <IntervalBoundary size={this.state.weekHeight}
                                     color={color}
                                     boundary={'full'}
                                     style={calendarIntervalStyles.selection as ViewStyle}/>;
        }

        if (day.date.isSame(startDay.date, 'day')) {
            return <StartInterval size={this.state.weekHeight}
                                  color={color}
                                  style={calendarIntervalStyles.selection as ViewStyle}/>;
        }

        if (day.date.isSame(endDay.date, 'day')) {
            return <EndInterval size={this.state.weekHeight}
                                color={color}
                                style={calendarIntervalStyles.selection as ViewStyle}/>;
        }

        return <Interval size={this.state.weekHeight}
                         color={color}
                         style={calendarIntervalStyles.selection as ViewStyle}/>;
    }

    //----------------------------------------------------------------------------
    private renderInterval(interval: IntervalModel, elementKey: number): JSX.Element | null {
        const color = CalendarEventsColor.getColor(interval.calendarEvent.type, interval.calendarEvent.status) || Style.color.white;

        switch (interval.intervalType) {
            case IntervalType.StartInterval:
                return <StartInterval key={elementKey} size={this.state.weekHeight} color={color}/>;

            case IntervalType.EndInterval:
                return <EndInterval key={elementKey} size={this.state.weekHeight} color={color}/>;

            case IntervalType.Interval:
                return <Interval key={elementKey} size={this.state.weekHeight} color={color}/>;

            case IntervalType.IntervalFullBoundary:
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color}
                                         boundary={'full'}/>;

            case IntervalType.IntervalLeftBoundary:
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color}
                                         boundary={'left'}/>;

            case IntervalType.IntervalRightBoundary:
                return <IntervalBoundary key={elementKey} size={this.state.weekHeight} color={color}
                                         boundary={'right'}/>;

            default:
                return null;
        }
    }
}
