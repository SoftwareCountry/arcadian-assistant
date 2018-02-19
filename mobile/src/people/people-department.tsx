import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { PeopleActions, navigatePeopleDepartment } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';

interface PeopleDepartmentProps {
    departmentId: string;
    employeesSubsetFilter: Function;
}

interface PeopleDepartmentDispatchProps {
    navigatePeopleDepartment: (departmentId: string) => void;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => ({
    departmentId: state.userInfo.employee.departmentId,
    employeesSubsetFilter: state.people.employeesSubsetFilter
});

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    navigatePeopleDepartment: (departmentId: string) => { 
        dispatch(navigatePeopleDepartment()); 
        dispatch(loadEmployeesForDepartment(departmentId));
    },
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & PeopleDepartmentDispatchProps> {  
    public componentDidMount() {
        this.props.navigatePeopleDepartment(this.props.departmentId);
    }

    public render() {
        this.props.employeesSubsetFilter();
        return <EmployeesList />;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);