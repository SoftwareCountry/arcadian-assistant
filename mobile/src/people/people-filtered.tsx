import React, {createRef} from 'react';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleRoom } from './people-room';
import {PeopleDepartment} from './people-department';
import { filterEmployees } from '../reducers/search/search.epics';
import {Map, is} from 'immutable';
import { CompanyDepartments } from './company-departments';
import {NavigationScreenProps} from 'react-navigation';
import {EmployeesList} from './employees-list';

interface PeopleProps {
    employees: EmployeesStore;
    loaded: boolean;
}

const mapStateToProps = (state: AppState): PeopleProps => ({
    employees: filterEmployees(state.organization.employees, state.people.filter),
    loaded: state.organization.departments && state.organization.departments.length > 0,
});

class PeopleCompanyFilteredImpl extends React.Component<NavigationScreenProps & PeopleProps> {

    public render() {
        return !this.props.loaded ? <LoadingView/> : <CompanyDepartments />;
    }
}

class PeopleRoomFilteredImpl extends React.Component<NavigationScreenProps & PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return shouldUpdate(this.props, nextProps);
    }

    public render() {
        return !this.props.employees ? <LoadingView/> : <PeopleRoom employees={this.props.employees} navigation={this.props.navigation}/>;
    }
}

class PeopleDepartmentFilteredImpl extends React.Component<NavigationScreenProps & PeopleProps> {

    public shouldComponentUpdate(nextProps: PeopleProps) {
        return shouldUpdate(this.props, nextProps);
    }

    public render() {
        return !this.props.employees ? <LoadingView/> : <PeopleDepartment employees={this.props.employees} navigation={this.props.navigation}/>;
    }
}

function shouldUpdate(curProps: PeopleProps, nextProps: PeopleProps) {
    const somethingUndefined = !curProps.employees || !nextProps.employees;
    const arrays = !is(curProps.employees.employeesById, nextProps.employees.employeesById);
    return somethingUndefined || !somethingUndefined && arrays;
}

export const PeopleCompanyFiltered = connect(mapStateToProps)(PeopleCompanyFilteredImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomFilteredImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentFilteredImpl);
