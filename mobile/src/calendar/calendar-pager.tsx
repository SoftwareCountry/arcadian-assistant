import React, { Component } from 'react';
import moment, { Moment } from 'moment';
import { calendarStyles } from './styles';
import { View, TouchableHighlight } from 'react-native';
import { StyledText } from '../override/styled-text';
import { CalendarPage, OnSelectedDayCallback } from './calendar-page';
import { WeekModel, PeriodsModel } from '../reducers/calendar/calendar.model';

interface CalendarDefaultProps {
    periods?: PeriodsModel;
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

// TODO: Temporary implementation with TouchableHighlight buttons. Switch to swipe.
export class CalendarPager extends Component<CalendarProps, CalendarState> {
    public static defaultProps: CalendarDefaultProps = {
        periods: null
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
                {/*TODO: temp buttons. Remove and use swipe instead */}
                <TouchableHighlight style={{width: 100, height: 20}} onPress={this.onPrevClick}>
                    <StyledText>{'<'}</StyledText>
                </TouchableHighlight>
                <StyledText style={calendarStyles.todayTitle}>
                    {date.format('MMMM YYYY')}
                </StyledText>
                <TouchableHighlight style={{width: 100, height: 20, justifyContent: 'flex-end', flexDirection: 'row'}} onPress={this.onNextClick}>
                    <StyledText>{'>'}</StyledText>
                </TouchableHighlight>
            </View>
            <CalendarPage onSelectedDay={this.props.onSelectedDay} weeks={this.props.weeks} periods={this.props.periods} />
        </View>;
    }
}