import React, { Component } from 'react';
import moment, { Moment } from 'moment';
import { calendarStyles } from './styles';
import { View, Button } from 'react-native';
import { StyledText } from '../override/styled-text';
import { CalendarPage, OnSelectedDayCallback } from './calendar-page';
import { WeekModel } from '../reducers/calendar/calendar.model';

interface CalendarInterval {
    startDate: Moment;
    endDate: Moment;
    color: string;
}

interface CalendarDefaultProps {
    intervals?: CalendarInterval[];
}

interface CalendarProps extends CalendarDefaultProps {
    weeks: WeekModel[];
    onSelectedDay: OnSelectedDayCallback;
    onPrevMonth: (month: number, year: number) => void;
    onNextMonth: (month: number, year: number) => void;
}

interface CalendarState {
    date: Moment;
}

// TODO: Temporary implementation with Prev Next buttons. Switch to swipe.
export class CalendarPager extends Component<CalendarProps, CalendarState> {
    public static defaultProps: CalendarDefaultProps = {
        intervals: []
    };

    constructor(props: CalendarProps) {
        super(props);

        this.state = {
            date: moment()
        };
    }

    public onPrevClick = () => {
        const newDate = moment(this.state.date);
        
        newDate.add(-1, 'months');
        this.props.onPrevMonth(newDate.month(), newDate.year());
        this.setState({ date: newDate });
    }

    public onNextClick = () => {
        const newDate = moment(this.state.date);
        
        newDate.add(1, 'months');
        this.props.onNextMonth(newDate.month(), newDate.year());
        this.setState({ date: newDate });
    }

    public render() {
        const { date } = this.state;

        return <View style={calendarStyles.container}>
            <View style={calendarStyles.today}>
                <Button onPress={this.onPrevClick} title={'<'} />
                <StyledText style={calendarStyles.todayTitle}>
                    {date.format('MMMM YYYY')}
                </StyledText>
                <Button onPress={this.onNextClick} title={'>'} />
            </View>
            <CalendarPage onSelectedDay={this.props.onSelectedDay} weeks={this.props.weeks} />
        </View>;
    }
}