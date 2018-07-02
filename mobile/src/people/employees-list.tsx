import React from 'react';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';
import { connect, Dispatch } from 'react-redux';

import { Department } from '../reducers/organization/department.model';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore, EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { EmployeesListItem } from './employees-list-item';
import { employeesListStyles as styles } from './styles';
import { employeesAZSort } from './employee-comparer';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { LoadingView } from '../navigation/loading';
import { SearchView, SearchType } from '../navigation/search-view';

export interface EmployeesListProps {
    employees: Employee[];
    onItemClicked: (e: Employee) => void;
}

export class EmployeesList extends React.Component<EmployeesListProps> {
    public render() {
        const employees = this.props.employees.sort(employeesAZSort);

        return employees.length > 0 ? 
            <View>
                <SearchView type={SearchType.People}/>
                <View style={styles.view}>
                    <FlatList
                        data={employees}
                        keyExtractor={this.keyExtractor}
                        renderItem={this.renderItem} />
                </View>
            </View>
        : <LoadingView/>;
    }

    private keyExtractor = (item: Employee) => item.employeeId;

    private renderItem = (itemInfo: ListRenderItemInfo<Employee>) => {
        const { item } = itemInfo;

        return <EmployeesListItem id={item.employeeId} employee={item} onItemClicked={this.props.onItemClicked}/>;
    }
}
