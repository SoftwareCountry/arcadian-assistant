import React from 'react';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';
import { connect } from 'react-redux';

import { Department } from '../reducers/organization/department.model';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore, EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { EmployeesListItem } from './employees-list-item';
import { employeesListStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

export interface EmployeesListProps {
    employees: Employee[];
}

export class EmployeesList extends React.Component<EmployeesListProps> {
    public render() {
        const employees = this.props.employees.sort((a, b) => {
            if (a.name < b.name) {
                return -1;
            } else if (a > b) {
                return 1;
            } else {
                return 0;
            }
        });

        return employees.length > 0 ? 
            <View style={styles.view}>
                <FlatList
                    data={employees}
                    keyExtractor={this.keyExtractor}
                    renderItem={this.renderItem} />
            </View>
        : (
            <View style={styles.loadingContainer}>
                    <StyledText style={styles.loadingText}>Loading...</StyledText>
            </View>
        );
    }

    private keyExtractor = (item: Employee) => item.employeeId;

    private renderItem = (itemInfo: ListRenderItemInfo<Employee>) => {
        const { item } = itemInfo;

        return <EmployeesListItem id={item.employeeId} employee={item} />;
    }
}
