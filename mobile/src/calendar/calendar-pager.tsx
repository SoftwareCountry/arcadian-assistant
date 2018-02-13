import React, { Component } from 'react';
import { CalendarEvents } from '../reducers/calendar/calendar-events';
import moment, { Moment } from 'moment';
import { calendarStyles } from './styles';
import { View, Button } from 'react-native';
import { StyledText } from '../override/styled-text';
import { CalendarPage, OnSelectedDayCallback } from './calendar-page';

interface CalendarInterval {
    startDate: Moment;
    endDate: Moment;
    color: string;
}

interface CalendarDefaultProps {
    intervals?: CalendarInterval[];
}

interface CalendarProps extends CalendarDefaultProps {
    onSelectedDay: OnSelectedDayCallback;
}

interface CalendarState {
    date: Moment;
}

// TODO: Temporary implementation with Prev Next buttons. Switch on swipe.
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
        this.setState({ date: newDate });
    }

    public onNextClick = () => {
        const newDate = moment(this.state.date);
        newDate.add(1, 'months');
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
            <CalendarPage month={date.month()} year={date.year()} onSelectedDay={this.props.onSelectedDay} />
        </View>;
    }
}