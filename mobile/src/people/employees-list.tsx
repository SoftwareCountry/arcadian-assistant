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
    employees: EmployeeMap;
    myDepartmentId: string;
}

interface EmployeesListState {
    employeesForMyDepartment: Employee[];
}

const mapStateToProps = (state: AppState): EmployeesListProps => ({
    employees: state.organization.employees.employeesById,
    myDepartmentId: state.userInfo.employee ? state.userInfo.employee.departmentId : 'unknown'
});

class EmployeesListImpl extends React.Component<EmployeesListProps> {
    public state: {
        employeesForMyDepartment: Employee[];
    };

    constructor(props: EmployeesListProps) {
        super(props);
        this.state = {
            employeesForMyDepartment: []
        };
    }

    public componentWillReceiveProps(nextProps: Readonly<EmployeesListProps>) {
        const { employees, myDepartmentId } = nextProps;
        
        const employeesForMyDepartment = employees.toArray().filter(function(employee) {
            return employee.departmentId === myDepartmentId;
        });

        if (employeesForMyDepartment.length > 0) {
            this.setState({
                employeesForMyDepartment: employeesForMyDepartment
            });
        }
    }
    
    public render() {
        return (
            <View style={styles.view}>
                <FlatList
                    data={this.state.employeesForMyDepartment}
                    keyExtractor={this.keyExtractor}
                    renderItem={({ item }) => <EmployeesListItem id={item.employeeId} employee={item} />} />
            </View>
        );
    }

    private keyExtractor = (item: Employee) => item.employeeId;
}

export const EmployeesList = connect(mapStateToProps)(EmployeesListImpl);
