import React from 'react';
import { FlatList, Text, View, StyleSheet } from 'react-native';
import { connect } from 'react-redux';

import { Department } from '../reducers/organization/department.model';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore, EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { EmployeesListItem } from './employees-list-item';
import { employeesListStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

interface EmployeesListProps {
    employeesForMyDepartment: EmployeeMap;
}

const mapStateToProps = (state: AppState): EmployeesListProps => ({
    employeesForMyDepartment: state.people.employees
});

class EmployeesListImpl extends React.Component<EmployeesListProps> {
    public render() {
        const employees = this.props.employeesForMyDepartment;

        return employees.size > 0 ? 
            <View style={styles.view}>
                <FlatList
                    data={employees.toArray()}
                    keyExtractor={this.keyExtractor}
                    renderItem={({ item }) => <EmployeesListItem id={item.employeeId} employee={item} />} />
            </View>
        : (
            <View style={styles.loadingContainer}>
                    <StyledText style={styles.loadingText}>Loading...</StyledText>
            </View>
        );
    }

    private keyExtractor = (item: Employee) => item.employeeId;
}

export const EmployeesList = connect(mapStateToProps)(EmployeesListImpl);
