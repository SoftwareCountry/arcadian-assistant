import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { PeopleActions, navigatePeopleDepartment } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { EmployeesStore, EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';

interface PeopleDepartmentProps {
    employeesMap: EmployeeMap;
    departmentId: string;
    employeesSubsetFilterCallback: any;
}

interface PeopleDepartmentDispatchProps {
    navigatePeopleDepartment: (departmentId: string) => void;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => ({
    employeesMap: state.organization.employees.employeesById,
    departmentId: state.userInfo.employee.departmentId,
    employeesSubsetFilterCallback: state.people.employeesDepartmentSubsetFilterCallback
});

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    navigatePeopleDepartment: (departmentId: string) => { 
        dispatch(navigatePeopleDepartment(departmentId)); 
        dispatch(loadEmployeesForDepartment(departmentId));
    },
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & PeopleDepartmentDispatchProps> {  
    public componentDidMount() {
        this.props.navigatePeopleDepartment(this.props.departmentId);
    }

    public render() {
        return <EmployeesList employees={this.props.employeesMap.toArray().filter(this.props.employeesSubsetFilterCallback)} />;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);