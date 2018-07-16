import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { View, Text, ScrollView, Dimensions, Animated } from 'react-native';

import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { SearchView, SearchType } from '../navigation/search-view';
import { LoadingView } from '../navigation/loading';
import { PeopleCompany } from './people-company';
import { Map } from 'immutable';

interface PeopleCompanyProps {
    loaded: boolean;
    employees: EmployeesStore;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => {
    let filter = state.people.filter;
    let employees = state.organization.employees;  
    let employeesPredicate = (employee: Employee) => {
        return (employee.name && employee.name.includes(filter) ||
                employee.email && employee.email.includes(filter) || 
                employee.position && employee.position.includes(filter)
        );
    };
    let filteredEmployeesById: Map<string, Employee> = employees.employeesById.filter(employeesPredicate) as Map<string, Employee>;
    let filteredEmployees : EmployeesStore = {employeesById: filteredEmployeesById, employeeIdsByDepartment: employees.employeeIdsByDepartment};


    return ({
        loaded: state.people.departmentsBranch.length > 0 && 
                state.people.departments && state.people.departments.length > 0,
        employees: filteredEmployees,
    });
};

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps> {
    public shouldComponentUpdate(nextProps: PeopleCompanyProps) {
        return this.props.loaded !== nextProps.loaded || !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        if (!this.props.loaded) {
            return <LoadingView/>;
        }

        return <View>
                <SearchView type={SearchType.People}/>
                <PeopleCompany employees={this.props.employees}/>
            </View>;
    }
}

export const PeopleCompanyFiltered = connect(mapStateToProps)(PeopleCompanyImpl);
