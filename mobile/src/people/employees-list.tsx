import React from 'react';
import { ListRenderItemInfo, View } from 'react-native';

import { Employee } from '../reducers/organization/employee.model';
import { EmployeesListItem } from './employees-list-item';
import { employeesListStyles as styles } from './styles';
import { employeesAZComparer } from './employee-comparer';
import { FlatList } from 'react-navigation';
import { NoResultView } from '../navigation/search/no-result-view';

//============================================================================
export interface EmployeesListProps {
    employees: Employee[];
    onItemClicked: (e: Employee) => void;
}

//============================================================================
export class EmployeesList extends React.Component<EmployeesListProps> {

    //----------------------------------------------------------------------------
    public render() {
        const employees = this.props.employees.sort(employeesAZComparer);

        return <View style={styles.view}>
            <FlatList
                data={employees}
                keyExtractor={this.keyExtractor}
                renderItem={this.renderItem}
                ListEmptyComponent={<NoResultView/>}
            />
        </View>;
    }

    //----------------------------------------------------------------------------
    private keyExtractor = (item: Employee) => item.employeeId;

    //----------------------------------------------------------------------------
    private renderItem = (itemInfo: ListRenderItemInfo<Employee>) => {
        const { item } = itemInfo;

        return <EmployeesListItem id={item.employeeId} employee={item} onItemClicked={this.props.onItemClicked}/>;
    };
}
