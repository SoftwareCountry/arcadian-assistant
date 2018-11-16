import React, {createRef} from 'react';
import { FlatList, View, ListRenderItemInfo } from 'react-native';

import { Employee } from '../reducers/organization/employee.model';
import { EmployeesListItem } from './employees-list-item';
import { employeesListStyles as styles } from './styles';
import { employeesAZComparer } from './employee-comparer';
import {NavigationScreenProps} from 'react-navigation';

export interface EmployeesListProps {
    employees: Employee[];
    onItemClicked: (e: Employee) => void;
}

export class EmployeesList extends React.Component<NavigationScreenProps & EmployeesListProps> {

    private flatList = createRef<FlatList<any>>();

    public componentDidMount() {
        if (this.props.navigation) {
            this.props.navigation.setParams({
                tabBarOnPress: this.scrollToTop,
            });
        }
    }

    public render() {
        const employees = this.props.employees.sort(employeesAZComparer);

        return <View style={styles.view}>
                    <FlatList
                        ref = {this.flatList}
                        data={employees}
                        keyExtractor={this.keyExtractor}
                        renderItem={this.renderItem} />
                </View>;
    }

    public scrollToTop = () => {
        this.flatList.current.scrollToIndex({index: 0, animated: true});
    }

    private keyExtractor = (item: Employee) => item.employeeId;

    private renderItem = (itemInfo: ListRenderItemInfo<Employee>) => {
        const { item } = itemInfo;

        return <EmployeesListItem id={item.employeeId} employee={item} onItemClicked={this.props.onItemClicked}/>;
    }
}
