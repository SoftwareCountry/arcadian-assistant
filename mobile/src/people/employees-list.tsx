import React from 'react';
import { FlatList, Text, View, StyleSheet } from 'react-native';
import { connect } from 'react-redux';

import { Department } from '../reducers/organization/department.model';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore, EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { EmployeesListItem } from './employees-list-item';
import { employeesListStyles as styles } from './styles';

interface EmployeesListProps {
    myDepartmentId: string;
    employeesForMyDepartment: EmployeeMap;
}

const mapStateToProps = (state: AppState): EmployeesListProps => ({
    myDepartmentId: state.userInfo.employee ? state.userInfo.employee.departmentId : 'unknown',
    employeesForMyDepartment: state.userInfo.employees
});

class EmployeesListImpl extends React.Component<EmployeesListProps> {
    public render() {
        return (
            <View style={styles.view}>
                <FlatList
                    data={this.props.employeesForMyDepartment.toArray()}
                    keyExtractor={this.keyExtractor}
                    renderItem={({ item }) => <EmployeesListItem id={item.employeeId} employee={item} />} />
            </View>
        );
    }

    private keyExtractor = (item: Employee) => item.employeeId;
}

export const EmployeesList = connect(mapStateToProps)(EmployeesListImpl);
