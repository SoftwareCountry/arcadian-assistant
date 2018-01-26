import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { User } from '../reducers/organization/user.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';

interface CalendarScreenProps {
    user: User;
    employees: EmployeesStore;
}

export class CalendarScreenImpl extends Component<CalendarScreenProps> {
    private username: string;

    public componentWillReceiveProps() {
        if (this.props.user) {
            const employee = this.props.employees.employeesById.get(this.props.user.employeeId);
            this.username = employee ? employee.name : '';
        }
    }

    public render() {
        return <View style={styles.container}>
            <Text>Calendar</Text>
            <Text>Username: {this.username}</Text>
        </View>;
    }
}

const mapStateToProps = (state: AppState): CalendarScreenProps => ({
    user: state.organization.user,
    employees: state.organization.employees
});

export const CalendarScreen = connect(mapStateToProps)(CalendarScreenImpl);