import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';

interface CalendarScreenProps {
    employee: Employee;
}

export class CalendarScreenImpl extends Component<CalendarScreenProps> {
    public render() {
        const username = this.props.employee ? this.props.employee.name : '';

        return <View style={styles.container}>
            <Text>Calendar</Text>
            <Text>Username: {username}</Text>
        </View>;
    }
}

const mapStateToProps = (state: AppState): CalendarScreenProps => ({
    employee: state.userInfo.employee
});

export const CalendarScreen = connect(mapStateToProps)(CalendarScreenImpl);