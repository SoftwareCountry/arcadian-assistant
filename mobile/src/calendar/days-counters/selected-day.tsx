import React, { Component } from 'react';
import { View } from 'react-native';
import { selectedDayStyles } from '../styles';
import { StyledText } from '../../override/styled-text';
import { Moment } from 'moment';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { AppState } from '../../reducers/app.reducer';
import { connect } from 'react-redux';

interface SelectedDayProps {
    selectedCalendarDay: DayModel;
}

class SelectedDayImpl extends Component<SelectedDayProps> {
    public render() {

        let day;
        let month;

        if (this.props.selectedCalendarDay) {
            day = this.props.selectedCalendarDay.date.format('D');
            month = this.props.selectedCalendarDay.date.format('MMMM');
        }

        return (
            <View style={selectedDayStyles.container}>
                <StyledText style={selectedDayStyles.circleCurrentDay}>{day}</StyledText>
                <StyledText style={selectedDayStyles.circleCurrentMonth}>{month}</StyledText>
            </View>
        );
    }
}

const mapStateToProps = (state: AppState): SelectedDayProps => ({
    selectedCalendarDay: state.calendar.calendarEvents.selectedCalendarDay
});

export const SelectedDay = connect(mapStateToProps)(SelectedDayImpl);