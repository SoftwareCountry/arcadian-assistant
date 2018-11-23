import React, { Component } from 'react';
import { View } from 'react-native';
import styles from '../layout/styles';
import { Calendar } from './calendar';
import { Agenda } from './agenda';
import { TopNavBar } from '../navigation/top-nav-bar';
import { DaysCounters } from './days-counters/days-counters';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';

const navBar =  new TopNavBar('');

interface CalendarScreenProps {
    employee: Employee;
}

class CalendarScreenImplementation extends Component<CalendarScreenProps> {
    public static navigationOptions = navBar.configurate();

    public render() {

        const { employee } = this.props;

        return (
            <View style={styles.container}>
                <DaysCounters employee={employee}/>
                <Calendar/>
                <Agenda/>
            </View>
        );
    }
}

const mapStateToProps = (state: AppState): CalendarScreenProps => {
    const employeeId = state.userInfo.employeeId;
    return {
        employee: employeeId == null ? null : state.organization.employees.employeesById.get(employeeId)
    };
};

export const CalendarScreenImpl = connect(mapStateToProps)(CalendarScreenImplementation);
