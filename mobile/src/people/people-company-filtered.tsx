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

interface PeopleCompanyProps {
    filter: string;
    loaded: boolean;
    employeesPredicate: (employee: Employee) => boolean;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => {
    let filter = state.people.filter;
    let loaded = state.people.departmentsBranch.length > 0 && 
                state.people.departments && state.people.departments.length > 0;

    return ({
        filter,
        loaded,
        employeesPredicate: (employee: Employee) => {
            return (employee.name.includes(filter) ||
                    employee.email.includes(filter) || 
                    employee.position.includes(filter));
        },
    });
};

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps> {
    public render() {
        if (!this.props.loaded) {
            return <LoadingView/>;
        }

        return <View>
                <SearchView type={SearchType.People}/>
                <PeopleCompany searchPredicate={this.props.employeesPredicate}/>
            </View>;
    }
}

export const PeopleCompanyFiltered = connect(mapStateToProps)(PeopleCompanyImpl);
