import React from 'react';
import { FlatList, Text, View, StyleSheet } from 'react-native';
import { Department } from '../reducers/organization/department.model';
import { Employee } from '../reducers/organization/employee.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeesListItem } from './employees-list-item';

interface EmployeesListProps {
    // departments: Department[];
    employees: Employee[];
}

const mapStateToProps = (state: AppState): EmployeesListProps => ({
    employees: []
});

const styles  = StyleSheet.create({
    view: {
        flex: 1,
        paddingLeft: 19,
        paddingRight: 19,
        backgroundColor: '#FFF'
    },
    viewHeaderText: {
        fontSize: 12
    },
    separator : {
        height: 15
    }
});

class EmployeesListImpl extends React.Component<EmployeesListProps> {
    public render() {
        return (
            <View style={styles.view}>
                <FlatList
                    ItemSeparatorComponent = {() => <View style={styles.separator}></View>}
                    data = { this.props.employees }
                    keyExtractor = {this.keyExtractor}
                    renderItem = { ({item}) => <EmployeesListItem id = { item.employeeId } employee = { item }/> } />
            </View>
        );
    }

    private keyExtractor = (item: Employee) => item.employeeId;
}

export const EmployeesList = connect(mapStateToProps)(EmployeesListImpl);
